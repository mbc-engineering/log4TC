using Mbc.Log4Tc.Model;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class SimpleFlatSqlWriter : BaseSqlWriter
    {
        internal override async Task WriteLogEntryAsync(DbConnection connection, LogEntry logEntry)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO log_entry (source, hostname, formatted_message, logger, level, plc_timestamp, clock_timestamp, task_index, task_name, task_cycle_counter, app_name, project_name, onlinechange_count) VALUES (@Source, @Hostname, @FormattedMessage, @Logger, @Level, @PlcTimeStamp, @ClockTimeStamp, @TaskIndex, @TaskName, @TaskCycleCounter, @AppName, @ProjectName, @OnlineChangeCount)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@Source", logEntry.Source));
                command.Parameters.Add(CreateParameter(command, "@Hostname", logEntry.Hostname));
                command.Parameters.Add(CreateParameter(command, "@FormattedMessage", logEntry.FormattedMessage));
                command.Parameters.Add(CreateParameter(command, "@Logger", logEntry.Logger));
                command.Parameters.Add(CreateParameter(command, "@Level", logEntry.Level));
                command.Parameters.Add(CreateParameter(command, "@PlcTimeStamp", logEntry.PlcTimestamp));

                if (logEntry.ClockTimestamp.Year > 1970)
                {
                    // SQL unterstütz Zeitstempel ab 1970
                    command.Parameters.Add(CreateParameter(command, "@ClockTimeStamp", logEntry.ClockTimestamp));
                }
                else
                {
                    command.Parameters.Add(CreateParameter(command, "@ClockTimeStamp", DBNull.Value));
                }

                command.Parameters.Add(CreateParameter(command, "@TaskIndex", (short)logEntry.TaskIndex));
                command.Parameters.Add(CreateParameter(command, "@TaskName", logEntry.TaskName));
                command.Parameters.Add(CreateParameter(command, "@TaskCycleCounter", (int)logEntry.TaskCycleCounter));
                command.Parameters.Add(CreateParameter(command, "@AppName", logEntry.AppName));
                command.Parameters.Add(CreateParameter(command, "@ProjectName", logEntry.ProjectName));
                command.Parameters.Add(CreateParameter(command, "@OnlineChangeCount", (int)logEntry.OnlineChangeCount));

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}

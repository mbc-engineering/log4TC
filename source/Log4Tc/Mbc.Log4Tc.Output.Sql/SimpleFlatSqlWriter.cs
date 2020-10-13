using Mbc.Log4Tc.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class SimpleFlatSqlWriter : BaseSqlWriter
    {
        internal override async Task WriteLogEntryAsync(DbTransaction transaction, IEnumerable<LogEntry> logEntries)
        {
            using (DbCommand command = transaction.Connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "INSERT INTO log_entry (source, hostname, formatted_message, logger, level, plc_timestamp, clock_timestamp, task_index, task_name, task_cycle_counter, app_name, project_name, onlinechange_count) VALUES (@Source, @Hostname, @FormattedMessage, @Logger, @Level, @PlcTimeStamp, @ClockTimeStamp, @TaskIndex, @TaskName, @TaskCycleCounter, @AppName, @ProjectName, @OnlineChangeCount)";
                command.CommandType = CommandType.Text;

                int count = 0;
                foreach (var logEntry in logEntries)
                {
                    if (count == 1)
                    {
                        // nur dann ein Prepare ausführen, wenn mehr als eine Ausführung
                        command.Prepare();
                    }

                    AddOrReplace(command.Parameters, CreateParameter(command, "@Source", logEntry.Source));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@Hostname", logEntry.Hostname));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@FormattedMessage", logEntry.FormattedMessage));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@Logger", logEntry.Logger));
                    AddOrReplace(command.Parameters, CreateEnumParameter(command, "@Level", logEntry.Level));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@PlcTimeStamp", logEntry.PlcTimestamp));

                    if (logEntry.ClockTimestamp.Year > 1970)
                    {
                        // SQL unterstütz Zeitstempel ab 1970
                        AddOrReplace(command.Parameters, CreateParameter(command, "@ClockTimeStamp", logEntry.ClockTimestamp));
                    }
                    else
                    {
                        AddOrReplace(command.Parameters, CreateParameter(command, "@ClockTimeStamp", DBNull.Value));
                    }

                    AddOrReplace(command.Parameters, CreateParameter(command, "@TaskIndex", (short)logEntry.TaskIndex));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@TaskName", logEntry.TaskName));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@TaskCycleCounter", (int)logEntry.TaskCycleCounter));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@AppName", logEntry.AppName));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@ProjectName", logEntry.ProjectName));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@OnlineChangeCount", (int)logEntry.OnlineChangeCount));

                    await command.ExecuteNonQueryAsync();
                    count++;
                }
            }
        }
    }
}

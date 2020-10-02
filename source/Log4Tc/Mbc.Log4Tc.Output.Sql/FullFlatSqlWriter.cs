using Mbc.Log4Tc.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class FullFlatSqlWriter : BaseSqlWriter
    {
        internal override async Task WriteLogEntryAsync(DbConnection connection, LogEntry logEntry)
        {
            long logEntryId = await InsertLogEntryAsync(connection, logEntry);
            await InsertArguments(connection, logEntryId, logEntry.Arguments);
        }

        private async Task<long> InsertLogEntryAsync(DbConnection connection, LogEntry logEntry)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO LogEntry (Source, Hostname, FormattedMessage, Message, Logger, Level, PlcTimeStamp, ClockTimeStamp, TaskIndex, TaskName, TaskCycleCounter, AppName, ProjectName, OnlineChangeCount) VALUES (@Source, @Hostname, @FormattedMessage, @Message, @Logger, @Level, @PlcTimeStamp, @ClockTimeStamp, @TaskIndex, @TaskName, @TaskCycleCounter, @AppName, @ProjectName, @OnlineChangeCount)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@Source", logEntry.Source));
                command.Parameters.Add(CreateParameter(command, "@Hostname", logEntry.Hostname));
                command.Parameters.Add(CreateParameter(command, "@FormattedMessage", logEntry.FormattedMessage));
                command.Parameters.Add(CreateParameter(command, "@Message", logEntry.Message));
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
                    command.Parameters.Add(CreateParameter(command, "@ClockTimeStamp", null));
                }

                command.Parameters.Add(CreateParameter(command, "@TaskIndex", logEntry.TaskIndex));
                command.Parameters.Add(CreateParameter(command, "@TaskName", logEntry.TaskName));
                command.Parameters.Add(CreateParameter(command, "@TaskCycleCounter", logEntry.TaskCycleCounter));
                command.Parameters.Add(CreateParameter(command, "@AppName", logEntry.AppName));
                command.Parameters.Add(CreateParameter(command, "@ProjectName", logEntry.ProjectName));
                command.Parameters.Add(CreateParameter(command, "@OnlineChangeCount", logEntry.OnlineChangeCount));

                if (command is MySql.Data.MySqlClient.MySqlCommand mySqlCommand)
                {
                    await mySqlCommand.ExecuteNonQueryAsync();
                    return mySqlCommand.LastInsertedId;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private string ValueToString(object value)
        {
            if (value == null)
                return "null";

            return value.ToString();
        }

        private async Task InsertArguments(DbConnection connection, long logEntryId, IDictionary<int, object> arguments)
        {
            if (arguments.Count == 0)
                return;

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO LogArgument (LogEntryId, Index, Value) VALUES (@LogEntryId, @Index, @Value)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@LogEntryId", logEntryId));

                foreach (var argumentEntry in arguments)
                {
                    command.Parameters.Add(CreateParameter(command, "@Index", argumentEntry.Key));
                    command.Parameters.Add(CreateParameter(command, "@Value", ValueToString(argumentEntry.Value));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task InsertContext(DbConnection connection, long logEntryId, IDictionary<string, object> context)
        {
            if (context.Count == 0)
                return;

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO LogContext (LogEntryId, Name, Value) VALUES (@LogEntryId, @Name, @Value)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@LogEntryId", logEntryId));

                foreach (var contextEntry in context)
                {
                    command.Parameters.Add(CreateParameter(command, "@Name", contextEntry.Key));
                    command.Parameters.Add(CreateParameter(command, "@Value", ValueToString(contextEntry.Value));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

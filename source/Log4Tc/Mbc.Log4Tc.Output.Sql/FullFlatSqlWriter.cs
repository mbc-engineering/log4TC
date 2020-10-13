using Mbc.Log4Tc.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class FullFlatSqlWriter : BaseSqlWriter
    {
        internal override async Task WriteLogEntryAsync(DbTransaction transaction, LogEntry logEntry)
        {
            long logEntryId = await InsertLogEntryAsync(transaction, logEntry);
            await InsertArguments(transaction, logEntryId, logEntry.Arguments);
            await InsertContext(transaction, logEntryId, logEntry.Context);
        }

        private async Task<long> InsertLogEntryAsync(DbTransaction transaction, LogEntry logEntry)
        {
            using (DbCommand command = transaction.Connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "INSERT INTO log_entry (source, hostname, formatted_message, message, logger, level, plc_timestamp, clock_timestamp, task_index, task_name, task_cycle_counter, app_name, project_name, onlinechange_count) VALUES (@Source, @Hostname, @FormattedMessage, @Message, @Logger, @Level, @PlcTimeStamp, @ClockTimeStamp, @TaskIndex, @TaskName, @TaskCycleCounter, @AppName, @ProjectName, @OnlineChangeCount)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@Source", logEntry.Source));
                command.Parameters.Add(CreateParameter(command, "@Hostname", logEntry.Hostname));
                command.Parameters.Add(CreateParameter(command, "@FormattedMessage", logEntry.FormattedMessage));
                command.Parameters.Add(CreateParameter(command, "@Message", logEntry.Message));
                command.Parameters.Add(CreateParameter(command, "@Logger", logEntry.Logger));
                command.Parameters.Add(CreateEnumParameter(command, "@Level", logEntry.Level));
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

                if (command is MySql.Data.MySqlClient.MySqlCommand mySqlCommand)
                {
                    await mySqlCommand.ExecuteNonQueryAsync();
                    return mySqlCommand.LastInsertedId;
                }
                else if (command is Npgsql.NpgsqlCommand npsqlCommand)
                {
                    command.CommandText += " RETURNING id";
                    return Convert.ToInt64(await command.ExecuteScalarAsync());
                }
                else if (command is System.Data.SqlClient.SqlCommand sqlCommand)
                {
                    command.CommandText += "; SELECT SCOPE_IDENTITY()";
                    return Convert.ToInt64(await command.ExecuteScalarAsync());
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

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private DbValueType ValueToType(object value)
        {
            if (value == null)
                return DbValueType.Null;

            return value switch
            {
                float _ => DbValueType.Real,
                double _ => DbValueType.LReal,
                sbyte _ => DbValueType.SInt,
                short _ => DbValueType.Int,
                int _ => DbValueType.Int,
                byte _ => DbValueType.USInt,
                ushort _ => DbValueType.UInt,
                uint _ => DbValueType.UDint,
                string _ => DbValueType.String,
                bool _ => DbValueType.Bool,
                ulong _ => DbValueType.ULarge,
                long _ => DbValueType.Large,
                _ => DbValueType.Null,
            };
        }

        private async Task InsertArguments(DbTransaction transaction, long logEntryId, IDictionary<int, object> arguments)
        {
            if (arguments.Count == 0)
                return;

            using (DbCommand command = transaction.Connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "INSERT INTO log_argument (log_entry_id, idx, value, type) VALUES (@LogEntryId, @Idx, @Value, @Type)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@LogEntryId", logEntryId));

                foreach (var argumentEntry in arguments)
                {
                    AddOrReplace(command.Parameters, CreateParameter(command, "@Idx", argumentEntry.Key));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@Value", ValueToString(argumentEntry.Value)));
                    AddOrReplace(command.Parameters, CreateEnumParameter(command, "@Type", ValueToType(argumentEntry.Value)));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task InsertContext(DbTransaction transaction, long logEntryId, IDictionary<string, object> context)
        {
            if (context.Count == 0)
                return;

            using (DbCommand command = transaction.Connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "INSERT INTO log_context (log_entry_id, name, value, type) VALUES (@LogEntryId, @Name, @Value, @Type)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@LogEntryId", logEntryId));

                foreach (var contextEntry in context)
                {
                    AddOrReplace(command.Parameters, CreateParameter(command, "@Name", contextEntry.Key));
                    AddOrReplace(command.Parameters, CreateParameter(command, "@Value", ValueToString(contextEntry.Value)));
                    AddOrReplace(command.Parameters, CreateEnumParameter(command, "@Type", ValueToType(contextEntry.Value)));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

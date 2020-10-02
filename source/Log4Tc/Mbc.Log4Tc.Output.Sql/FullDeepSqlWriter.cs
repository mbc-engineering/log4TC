using Mbc.Log4Tc.Model;
using Optional;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class FullDeepSqlWriter : BaseSqlWriter
    {
        public FullDeepSqlWriter()
        {
            throw new System.NotImplementedException("Noch nicht fertigimplementiert!");
        }

        private async Task<Option<long>> GetLogSourceIdAsync(DbConnection connection, LogEntry logEntry)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id FROM LogSource WHERE Source = @Source";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@Source", logEntry.Source));

                var result = await command.ExecuteScalarAsync();
                return result == null ? Option.None<long>() : Option.Some((long)result);
            }
        }

        private async Task<long> InsertLogSourceIdAsync(DbConnection connection, LogEntry logEntry)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO LogSource (Source) VALUES (@Source)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@Source", logEntry.Source));

                if (command is MySql.Data.MySqlClient.MySqlCommand mySqlCommand)
                {
                    await mySqlCommand.ExecuteNonQueryAsync();
                    return mySqlCommand.LastInsertedId;
                }
                else
                {
                    // Fallback mit INSERT/SELECT
                    await command.ExecuteNonQueryAsync();

                    var id = await GetLogSourceIdAsync(connection, logEntry);
                    return id.ValueOrFailure();
                }
            }
        }

        private async Task<Option<long>> GetLogHostnameIdAsync(DbConnection connection, LogEntry logEntry)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id FROM LogHostname WHERE Hostname = @Hostname";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@Hostname", logEntry.Hostname));

                var result = await command.ExecuteScalarAsync();
                return result == null ? Option.None<long>() : Option.Some((long)result);
            }
        }

        private async Task<long> InsertLogHostnameIdAsync(DbConnection connection, LogEntry logEntry)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO LogHostname (Hostname) VALUES (@Hostname)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@Hostname", logEntry.Source));

                if (command is MySql.Data.MySqlClient.MySqlCommand mySqlCommand)
                {
                    await mySqlCommand.ExecuteNonQueryAsync();
                    return mySqlCommand.LastInsertedId;
                }
                else
                {
                    // Fallback mit INSERT/SELECT
                    await command.ExecuteNonQueryAsync();

                    var id = await GetLogSourceIdAsync(connection, logEntry);
                    return id.ValueOrFailure();
                }
            }
        }

        internal override async Task WriteLogEntryAsync(DbConnection connection, LogEntry logEntry)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO LogEntry (SourceId, HostnameId, FormattedMessage, Message, Logger, Level, PlcTimeStamp, ClockTimeStamp, TaskIndex, TaskName, TaskCycleCounter, AppName, ProjectName, OnlineChangeCount) VALUES (@SourceId, @HostnameId, @FormattedMessage, @Message, @Logger, @Level, @PlcTimeStamp, @ClockTimeStamp, @TaskIndex, @TaskName, @TaskCycleCounter, @AppName, @ProjectName, @OnlineChangeCount)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(CreateParameter(command, "@SourceId", sourceId.ValueOrFailure()));
                command.Parameters.Add(CreateParameter(command, "@HostnameId", hostnameId.ValueOrFailure()));
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

                await command.ExecuteNonQueryAsync();
            }
        }

    }
}

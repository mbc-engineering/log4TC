using Mbc.Log4Tc.Model;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class SqlOutput : IOutputHandler, IDisposable
    {
        private readonly SqlOutputSettings _settings;
        private readonly Type _connectionType;

        public SqlOutput(SqlOutputSettings settings)
        {
            _settings = settings;
            _connectionType = Type.GetType(_settings.ConnectionType);
        }

        public void Dispose()
        {
        }

        private DbConnection CreateConnection() => (DbConnection)Activator.CreateInstance(_connectionType);

        private DbParameter CreateParameter(DbCommand command, string name, object value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        public async Task ProcesLogEntry(LogEntry logEntry)
        {
            using (DbConnection connection = CreateConnection())
            {
                connection.ConnectionString = _settings.ConnectionString;
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO LogEntry (Source, Hostname, FormattedMessage, Logger, Level, PlcTimeStamp, ClockTimeStamp, TaskIndex, TaskName, TaskCycleCounter, AppName, ProjectName, OnlineChangeCount) VALUES (@Source, @Hostname, @FormattedMessage, @Logger, @Level, @PlcTimeStamp, @ClockTimeStamp, @TaskIndex, @TaskName, @TaskCycleCounter, @AppName, @ProjectName, @OnlineChangeCount)";
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

                    transaction.Commit();
                }
            }
        }
    }
}

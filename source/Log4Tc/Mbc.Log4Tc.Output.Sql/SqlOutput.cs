using Mbc.Log4Tc.Model;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class SqlOutput : IOutputHandler, IDisposable
    {
        private readonly SqlOutputSettings _settings;
        private readonly Type _connectionType;
        private readonly BaseSqlWriter _sqlWriter;

        public SqlOutput(SqlOutputSettings settings)
        {
            _settings = settings;
            _connectionType = _settings.Driver switch
            {
                SqlOutputSettings.DbDriver.MySql => typeof(MySql.Data.MySqlClient.MySqlConnection),
                SqlOutputSettings.DbDriver.Postgres => typeof(Npgsql.NpgsqlConnection),
                SqlOutputSettings.DbDriver.SqlServer => typeof(System.Data.SqlClient.SqlConnection),
                _ => throw new ArgumentException("Invalid driver.", nameof(settings)),
            };
            _sqlWriter = _settings.Scheme switch
            {
                SqlOutputSettings.DbScheme.SimpleFlat => new SimpleFlatSqlWriter(),
                SqlOutputSettings.DbScheme.FullFlat => new FullFlatSqlWriter(),
                _ => throw new ArgumentException("Invalid scheme.", nameof(settings)),
            };

            if (_settings.Driver == SqlOutputSettings.DbDriver.Postgres)
            {
                // Registriert Enum-Type Mappings für Postgres
                Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<LogLevel>("log_level_type");
                Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<DbValueType>("log_value_type");
            }
        }

        public void Dispose()
        {
        }

        private DbConnection CreateConnection() => (DbConnection)Activator.CreateInstance(_connectionType);

        public async Task ProcesLogEntry(LogEntry logEntry)
        {
            using (DbConnection connection = CreateConnection())
            {
                connection.ConnectionString = _settings.ConnectionString;
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    await _sqlWriter.WriteLogEntryAsync(connection, logEntry);

                    transaction.Commit();
                }
            }
        }
    }
}

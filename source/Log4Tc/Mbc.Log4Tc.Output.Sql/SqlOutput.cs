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
        private readonly BaseSqlWriter _sqlWriter;

        public SqlOutput(SqlOutputSettings settings)
        {
            _settings = settings;
            _connectionType = Type.GetType(_settings.ConnectionType);
            _sqlWriter = new SimpleFlatSqlWriter();
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

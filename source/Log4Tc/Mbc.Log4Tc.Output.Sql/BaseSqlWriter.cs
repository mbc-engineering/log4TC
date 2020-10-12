using Mbc.Log4Tc.Model;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Sql
{
    /// <summary>
    /// Basisklasse für alle SQL-Schema Writer um <see cref="LogEntry"/>-Instanzen zu schreiben.
    /// </summary>
    internal abstract class BaseSqlWriter
    {
        internal abstract Task WriteLogEntryAsync(DbConnection connection, LogEntry logEntry);

        /// <summary>
        /// Erzeugt einen <see cref="DbParameter"/> mit dem angegbenen Namen und Wert.
        /// </summary>
        protected DbParameter CreateParameter(DbCommand command, string name, object value, DbType? dbType = null)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            if (dbType.HasValue)
            {
                parameter.DbType = dbType.Value;
            }
            return parameter;
        }

        /// <summary>
        /// Erzeugt einen <see cref="DbParameter"/> und fügt diesen zum <see cref="DbCommand"/>
        /// hinzu oder ersetzt einen bestehenden.
        /// </summary>
        protected void AddOrReplace(DbCommand command, string name, object value)
        {
            if (command.Parameters.Contains(name))
            {
                command.Parameters[name] = CreateParameter(command, name, value);
            }
            else
            {
                command.Parameters.Add(CreateParameter(command, name, value));
            }
        }
    }
}

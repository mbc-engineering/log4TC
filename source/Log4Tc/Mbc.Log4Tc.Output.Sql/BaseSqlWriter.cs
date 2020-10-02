using Mbc.Log4Tc.Model;
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
        protected DbParameter CreateParameter(DbCommand command, string name, object value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }
    }
}

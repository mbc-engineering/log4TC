using Mbc.Log4Tc.Model;
using System;
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
        internal abstract Task WriteLogEntryAsync(DbTransaction transaction, LogEntry logEntry);

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

        protected DbParameter CreateEnumParameter<T>(DbCommand command, string name, T value)
        {
            if (command is System.Data.SqlClient.SqlCommand sqlCommand)
            {
                // SQL-Server spezifisches Enum-Handling -> als String schreiben
                return CreateParameter(command, name, Enum.GetName(typeof(T), value));
            }
            else
            {
                return CreateParameter(command, name, value);
            }
        }

        protected void AddOrReplace(DbParameterCollection parameterCollection, DbParameter parameter)
        {
            if (parameterCollection.Contains(parameter.ParameterName))
            {
                parameterCollection[parameter.ParameterName] = parameter;
            }
            else
            {
                parameterCollection.Add(parameter);
            }
        }
    }
}

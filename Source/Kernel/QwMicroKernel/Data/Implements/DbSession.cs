using System;
using System.Data;

namespace QwMicroKernel.Data.Implements
{
    public abstract class DbSession : IDbSession
    {
        private readonly string _connectionString;

        protected string ConnectionString
        {
            get { return _connectionString; }
        }

        protected DbSession(string connectionString)
        {
            _connectionString = connectionString;
        }

        public abstract IDbDataParameter CreateParameter(string parameterName, object value);
        protected abstract IDbConnection CreateConnection(string connectionString);
        protected abstract IDbDataAdapter CreateAdapter(IDbCommand command);

        protected virtual T CommandActuator<T>(string sqlString, IDbDataParameter[] parameters, Func<IDbCommand, T> commandAction)
        {
            if (string.IsNullOrWhiteSpace(sqlString)) throw new ArgumentNullException("sql");

            using (IDbConnection connection = CreateConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sqlString;
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;

                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    connection.Open();

                    return commandAction(command);
                }
            }
        }

        public virtual object GetSingle(string sqlString, params IDbDataParameter[] parameters)
        {
            return CommandActuator<object>(sqlString, parameters, cmd =>
            {
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return null;
                return result;
            });
        }

        public virtual int QueryCount(string sqlString, params IDbDataParameter[] parameters)
        {
            var result = GetSingle(sqlString, parameters);
            if (result == null) return 0;
            return Convert.ToInt32(result);
        }

        public virtual int ExecuteSql(string sqlString, params IDbDataParameter[] parameters)
        {
            return CommandActuator<int>(sqlString, parameters, cmd =>
            {
                return cmd.ExecuteNonQuery();
            });
        }

        public virtual DataTable Query(string sqlString, params IDbDataParameter[] parameters)
        {
            return CommandActuator<DataTable>(sqlString, parameters, cmd =>
            {
                var adapter = CreateAdapter(cmd);
                var ds = new DataSet();
                adapter.Fill(ds);
                return ds.Tables[0];
            });
        }

        public virtual IDataReader QueryReader(string sqlString, params IDbDataParameter[] parameters)
        {
            return CommandActuator<IDataReader>(sqlString, parameters, cmd =>
            {
                return cmd.ExecuteReader();
            });
        }
    }
}

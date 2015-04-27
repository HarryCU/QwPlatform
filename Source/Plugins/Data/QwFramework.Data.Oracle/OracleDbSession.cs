using System.Data;
using Oracle.ManagedDataAccess.Client;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Oracle
{
    public class OracleDbSession : DbSession
    {
        public OracleDbSession(string connectionString)
            : base(connectionString)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        protected override IDbDataAdapter CreateAdapter(IDbCommand command)
        {
            return new OracleDataAdapter(command as OracleCommand);
        }

        public override IDbDataParameter CreateParameter(string parameterName, object value)
        {
            return new OracleParameter(parameterName, value);
        }
    }
}

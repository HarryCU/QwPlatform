using System.Data;
using Oracle.ManagedDataAccess.Client;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Oracle
{
    public class OracleDbAffairSession : DbAffairSession
    {
        public OracleDbAffairSession(string connectionString)
            : base(connectionString)
        {
        }

        public override IDbDataParameter CreateParameter(string parameterName, object value)
        {
            return new OracleParameter(parameterName, value);
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        protected override IDbDataAdapter CreateAdapter(IDbCommand command)
        {
            return new OracleDataAdapter(command as OracleCommand);
        }
    }
}

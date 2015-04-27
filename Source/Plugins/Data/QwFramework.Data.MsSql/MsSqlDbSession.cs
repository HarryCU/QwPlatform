using System.Data;
using System.Data.SqlClient;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.MsSql
{
    public class MsSqlDbSession : DbSession
    {
        public MsSqlDbSession(string connectionString)
            : base(connectionString)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override IDbDataAdapter CreateAdapter(IDbCommand command)
        {
            return new SqlDataAdapter(command as SqlCommand);
        }

        public override IDbDataParameter CreateParameter(string parameterName, object value)
        {
            return new SqlParameter(parameterName, value);
        }
    }
}

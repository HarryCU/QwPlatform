using System.Data;
using System.Data.SqlClient;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.MsSql
{
    public class MsSqlDbAffairSession : DbAffairSession
    {
        public MsSqlDbAffairSession(string connectionString)
            : base(connectionString)
        {
        }

        public override IDbDataParameter CreateParameter(string parameterName, object value)
        {
            return new SqlParameter(parameterName, value);
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override IDbDataAdapter CreateAdapter(IDbCommand command)
        {
            return new SqlDataAdapter(command as SqlCommand);
        }
    }
}

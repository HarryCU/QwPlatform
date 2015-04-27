using System.Data;
using System.Data.SQLite;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Sqlite
{
    public class SqliteDbAffairSession : DbAffairSession
    {
        public SqliteDbAffairSession(string connectionString)
            : base(connectionString)
        {
        }

        public override IDbDataParameter CreateParameter(string parameterName, object value)
        {
            return new SQLiteParameter(parameterName, value);
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        protected override IDbDataAdapter CreateAdapter(IDbCommand command)
        {
            return new SQLiteDataAdapter(command as SQLiteCommand);
        }
    }
}

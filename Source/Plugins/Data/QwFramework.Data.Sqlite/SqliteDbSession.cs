using System.Data;
using System.Data.SQLite;
using QwMicroKernel.Data.Implements;

namespace QwFramework.Data.Sqlite
{
    public class SqliteDbSession : DbSession
    {
        public SqliteDbSession(string connectionString)
            : base(connectionString)
        {
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        protected override IDbDataAdapter CreateAdapter(IDbCommand command)
        {
            return new SQLiteDataAdapter(command as SQLiteCommand);
        }

        public override IDbDataParameter CreateParameter(string parameterName, object value)
        {
            return new SQLiteParameter(parameterName, value);
        }
    }
}

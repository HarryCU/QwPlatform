using System.Data;

namespace QwMicroKernel.Data
{
    public interface IDbSession
    {
        IDbDataParameter CreateParameter(string parameterName, object value);

        object GetSingle(string sqlString, params IDbDataParameter[] parameters);
        int QueryCount(string sqlString, params IDbDataParameter[] parameters);

        int ExecuteSql(string sqlString, params IDbDataParameter[] parameters);
        DataTable Query(string sqlString, params IDbDataParameter[] parameters);
        IDataReader QueryReader(string sqlString, params IDbDataParameter[] parameters);
    }
}

using System.Data;
using System.Data.Common;

namespace AccountManagement.Application.Interfaces.Data
{
    public interface IDatabaseHelper
    {
        DbConnection CreateConnection();
        DbCommand CreateCommand(string query, DbConnection connection);
        DbDataAdapter CreateDataAdapter(DbCommand command);

        int ExecuteNonQuery(string query, params DbParameter[] parameters);
        object ExecuteScalar(string query, params DbParameter[] parameters);
        DataTable ExecuteDataTable(string query, params DbParameter[] parameters);

        Task<int> ExecuteNonQueryAsync(string query, params DbParameter[] parameters);
        Task<object> ExecuteScalarAsync(string query, params DbParameter[] parameters);
        Task<DataTable> ExecuteDataTableAsync(string query, params DbParameter[] parameters);
    }


}

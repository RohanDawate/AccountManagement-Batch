using AccountManagement.Application.Interfaces.Data;
using System.Data;
using System.Data.Common;

namespace AccountManagement.Infrastructure.DataAccess
{
    public class MockDatabaseHelper : IDatabaseHelper
    {
        public DbConnection CreateConnection() => null;
        public DbCommand CreateCommand(string query, DbConnection connection) => null;
        public DbDataAdapter CreateDataAdapter(DbCommand command) => null;

        public int ExecuteNonQuery(string query, params DbParameter[] parameters) => 1;
        public object ExecuteScalar(string query, params DbParameter[] parameters) => "MockResult";
        public DataTable ExecuteDataTable(string query, params DbParameter[] parameters) => new DataTable();

        public Task<int> ExecuteNonQueryAsync(string query, params DbParameter[] parameters) => Task.FromResult(1);
        public Task<object> ExecuteScalarAsync(string query, params DbParameter[] parameters) => Task.FromResult<object>("MockResult");
        public Task<DataTable> ExecuteDataTableAsync(string query, params DbParameter[] parameters) => Task.FromResult(new DataTable());
    }

}

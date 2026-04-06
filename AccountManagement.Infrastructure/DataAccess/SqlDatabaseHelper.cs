using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using AccountManagement.Application.Interfaces.Data;

namespace AccountManagement.Infrastructure.DataAccess
{
    public class SqlDatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;

        public SqlDatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbConnection CreateConnection() => new SqlConnection(_connectionString);

        public DbCommand CreateCommand(string query, DbConnection connection)
            => new SqlCommand(query, (SqlConnection)connection);

        public DbDataAdapter CreateDataAdapter(DbCommand command)
            => new SqlDataAdapter((SqlCommand)command);

        public int ExecuteNonQuery(string query, params DbParameter[] parameters)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = CreateCommand(query, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(string query, params DbParameter[] parameters)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = CreateCommand(query, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }

        public DataTable ExecuteDataTable(string query, params DbParameter[] parameters)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = CreateCommand(query, conn);
            cmd.Parameters.AddRange(parameters);
            using var adapter = CreateDataAdapter(cmd);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        public async Task<int> ExecuteNonQueryAsync(string query, params DbParameter[] parameters)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var cmd = CreateCommand(query, conn);
            cmd.Parameters.AddRange(parameters);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<object> ExecuteScalarAsync(string query, params DbParameter[] parameters)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var cmd = CreateCommand(query, conn);
            cmd.Parameters.AddRange(parameters);
            return await cmd.ExecuteScalarAsync();
        }

        public async Task<DataTable> ExecuteDataTableAsync(string query, params DbParameter[] parameters)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var cmd = CreateCommand(query, conn);
            cmd.Parameters.AddRange(parameters);
            using var adapter = CreateDataAdapter(cmd);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }
    }

}

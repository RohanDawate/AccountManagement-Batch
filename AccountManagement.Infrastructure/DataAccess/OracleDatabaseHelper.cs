using AccountManagement.Application.Interfaces.Data;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace AccountManagement.Infrastructure.DataAccess
{
    public class OracleDatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;

        public OracleDatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbConnection CreateConnection() => new OracleConnection(_connectionString);

        public DbCommand CreateCommand(string query, DbConnection connection)
            => new OracleCommand(query, (OracleConnection)connection);

        public DbDataAdapter CreateDataAdapter(DbCommand command)
            => new OracleDataAdapter((OracleCommand)command);

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

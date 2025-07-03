// Services/QueryExecutionService.cs - 쿼리 실행 서비스
using System;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace wsDB.OracleConnectionManager.Services
{
    public class QueryExecutionService
    {
        private readonly OracleConnectionService _connectionService;

        public QueryExecutionService()
        {
            _connectionService = OracleConnectionService.Instance;
        }

        public async Task<DataTable> ExecuteQueryAsync(string sql)
        {
            if (!_connectionService.IsConnected || _connectionService.Connection == null)
            {
                throw new InvalidOperationException("데이터베이스에 연결되지 않았습니다.");
            }

            try
            {
                using var command = new OracleCommand(sql, _connectionService.Connection);
                using var adapter = new OracleDataAdapter(command);
                
                var dataTable = new DataTable();
                await Task.Run(() => adapter.Fill(dataTable));
                
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"쿼리 실행 오류: {ex.Message}", ex);
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            if (!_connectionService.IsConnected || _connectionService.Connection == null)
            {
                throw new InvalidOperationException("데이터베이스에 연결되지 않았습니다.");
            }

            try
            {
                using var command = new OracleCommand(sql, _connectionService.Connection);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"쿼리 실행 오류: {ex.Message}", ex);
            }
        }
    }
}

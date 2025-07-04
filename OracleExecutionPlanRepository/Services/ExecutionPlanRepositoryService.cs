// ExecutionPlanRepository/Services/ExecutionPlanRepositoryService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using wsDB.OracleExecutionPlanRepository.Models;

namespace wsDB.OracleExecutionPlanRepository.Services
{
    public class ExecutionPlanRepositoryService
    {
        private readonly string _connectionString;
        private readonly string _dbPath;

        public ExecutionPlanRepositoryService()
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExecutionPlanRepository.db");
            _connectionString = $"Data Source={_dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS ExecutionPlanRecords (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SqlId TEXT NOT NULL,
                    ExecutionLocation TEXT NOT NULL,
                    Query TEXT NOT NULL,
                    BindVariables TEXT,
                    ExecutionPlan TEXT NOT NULL,
                    AnalysisInfo TEXT,
                    CreatedDate TEXT NOT NULL,
                    LastAccessDate TEXT NOT NULL,
                    Notes TEXT
                );

                CREATE INDEX IF NOT EXISTS IX_SqlId ON ExecutionPlanRecords(SqlId);
                CREATE INDEX IF NOT EXISTS IX_ExecutionLocation ON ExecutionPlanRecords(ExecutionLocation);
                CREATE INDEX IF NOT EXISTS IX_CreatedDate ON ExecutionPlanRecords(CreatedDate);
            ";

            createTableCommand.ExecuteNonQuery();
        }

        public async Task<int> SaveRecordAsync(ExecutionPlanRecord record)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            
            if (record.Id == 0)
            {
                // Insert
                command.CommandText = @"
                    INSERT INTO ExecutionPlanRecords 
                    (SqlId, ExecutionLocation, Query, BindVariables, ExecutionPlan, AnalysisInfo, CreatedDate, LastAccessDate, Notes)
                    VALUES (@SqlId, @ExecutionLocation, @Query, @BindVariables, @ExecutionPlan, @AnalysisInfo, @CreatedDate, @LastAccessDate, @Notes);
                    SELECT last_insert_rowid();";
            }
            else
            {
                // Update
                command.CommandText = @"
                    UPDATE ExecutionPlanRecords SET
                        SqlId = @SqlId,
                        ExecutionLocation = @ExecutionLocation,
                        Query = @Query,
                        BindVariables = @BindVariables,
                        ExecutionPlan = @ExecutionPlan,
                        AnalysisInfo = @AnalysisInfo,
                        LastAccessDate = @LastAccessDate,
                        Notes = @Notes
                    WHERE Id = @Id";
                
                command.Parameters.AddWithValue("@Id", record.Id);
            }

            command.Parameters.AddWithValue("@SqlId", record.SqlId ?? "");
            command.Parameters.AddWithValue("@ExecutionLocation", record.ExecutionLocation ?? "");
            command.Parameters.AddWithValue("@Query", record.Query ?? "");
            command.Parameters.AddWithValue("@BindVariables", record.BindVariables ?? "");
            command.Parameters.AddWithValue("@ExecutionPlan", record.ExecutionPlan ?? "");
            command.Parameters.AddWithValue("@AnalysisInfo", record.AnalysisInfo ?? "");
            command.Parameters.AddWithValue("@CreatedDate", record.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@LastAccessDate", record.LastAccessDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@Notes", record.Notes ?? "");

            if (record.Id == 0)
            {
                var result = await command.ExecuteScalarAsync();
                if (result != null && long.TryParse(result.ToString(), out long newId))
                {
                    return (int)newId;
                }
                return 0;
            }
            else
            {
                await command.ExecuteNonQueryAsync();
                return record.Id;
            }
        }

        public async Task<List<ExecutionPlanRecord>> GetAllRecordsAsync()
        {
            var records = new List<ExecutionPlanRecord>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, SqlId, ExecutionLocation, Query, BindVariables, ExecutionPlan, 
                       AnalysisInfo, CreatedDate, LastAccessDate, Notes
                FROM ExecutionPlanRecords
                ORDER BY CreatedDate DESC";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                records.Add(new ExecutionPlanRecord
                {
                    Id = reader.GetInt32(0), // Id
                    SqlId = reader.GetString(1), // SqlId
                    ExecutionLocation = reader.GetString(2), // ExecutionLocation
                    Query = reader.GetString(3), // Query
                    BindVariables = reader.IsDBNull(4) ? "" : reader.GetString(4), // BindVariables
                    ExecutionPlan = reader.GetString(5), // ExecutionPlan
                    AnalysisInfo = reader.IsDBNull(6) ? "" : reader.GetString(6), // AnalysisInfo
                    CreatedDate = DateTime.Parse(reader.GetString(7)), // CreatedDate
                    LastAccessDate = DateTime.Parse(reader.GetString(8)), // LastAccessDate
                    Notes = reader.IsDBNull(9) ? "" : reader.GetString(9) // Notes
                });
            }

            return records;
        }

        public async Task<ExecutionPlanRecord> GetRecordByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, SqlId, ExecutionLocation, Query, BindVariables, ExecutionPlan, 
                       AnalysisInfo, CreatedDate, LastAccessDate, Notes
                FROM ExecutionPlanRecords
                WHERE Id = @Id";
            
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var record = new ExecutionPlanRecord
                {
                    Id = reader.GetInt32(0), // Id
                    SqlId = reader.GetString(1), // SqlId
                    ExecutionLocation = reader.GetString(2), // ExecutionLocation
                    Query = reader.GetString(3), // Query
                    BindVariables = reader.IsDBNull(4) ? "" : reader.GetString(4), // BindVariables
                    ExecutionPlan = reader.GetString(5), // ExecutionPlan
                    AnalysisInfo = reader.IsDBNull(6) ? "" : reader.GetString(6), // AnalysisInfo
                    CreatedDate = DateTime.Parse(reader.GetString(7)), // CreatedDate
                    LastAccessDate = DateTime.Parse(reader.GetString(8)), // LastAccessDate
                    Notes = reader.IsDBNull(9) ? "" : reader.GetString(9) // Notes
                };

                // 접근 시간 업데이트
                await UpdateLastAccessDateAsync(id);
                
                return record;
            }

            return null;
        }

        public async Task<List<ExecutionPlanRecord>> SearchRecordsAsync(string searchText)
        {
            var records = new List<ExecutionPlanRecord>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, SqlId, ExecutionLocation, Query, BindVariables, ExecutionPlan, 
                       AnalysisInfo, CreatedDate, LastAccessDate, Notes
                FROM ExecutionPlanRecords
                WHERE SqlId LIKE @SearchText 
                   OR ExecutionLocation LIKE @SearchText 
                   OR Query LIKE @SearchText
                   OR Notes LIKE @SearchText
                ORDER BY CreatedDate DESC";

            command.Parameters.AddWithValue("@SearchText", $"%{searchText}%");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                records.Add(new ExecutionPlanRecord
                {
                    Id = reader.GetInt32(0), // Id
                    SqlId = reader.GetString(1), // SqlId
                    ExecutionLocation = reader.GetString(2), // ExecutionLocation
                    Query = reader.GetString(3), // Query
                    BindVariables = reader.IsDBNull(4) ? "" : reader.GetString(4), // BindVariables
                    ExecutionPlan = reader.GetString(5), // ExecutionPlan
                    AnalysisInfo = reader.IsDBNull(6) ? "" : reader.GetString(6), // AnalysisInfo
                    CreatedDate = DateTime.Parse(reader.GetString(7)), // CreatedDate
                    LastAccessDate = DateTime.Parse(reader.GetString(8)), // LastAccessDate
                    Notes = reader.IsDBNull(9) ? "" : reader.GetString(9) // Notes
                });
            }

            return records;
        }

        public async Task<bool> DeleteRecordAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM ExecutionPlanRecords WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<List<ExecutionLocationNode>> GetExecutionLocationTreeAsync()
        {
            var records = await GetAllRecordsAsync();
            var rootNodes = new List<ExecutionLocationNode>();
            var nodeDict = new Dictionary<string, ExecutionLocationNode>();

            foreach (var record in records)
            {
                var parts = record.ExecutionLocation.Split('.');
                string currentPath = "";

                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    var parentPath = currentPath;
                    currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}.{part}";

                    if (!nodeDict.ContainsKey(currentPath))
                    {
                        var node = new ExecutionLocationNode
                        {
                            Name = part,
                            FullPath = currentPath
                        };

                        nodeDict[currentPath] = node;

                        if (string.IsNullOrEmpty(parentPath))
                        {
                            rootNodes.Add(node);
                        }
                        else
                        {
                            nodeDict[parentPath].Children.Add(node);
                        }
                    }
                }

                // 레코드를 해당 노드에 추가
                if (nodeDict.ContainsKey(record.ExecutionLocation))
                {
                    nodeDict[record.ExecutionLocation].Records.Add(record);
                }
            }

            return rootNodes;
        }

        private async Task UpdateLastAccessDateAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE ExecutionPlanRecords SET LastAccessDate = @LastAccessDate WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@LastAccessDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task<string> ExportToTextFileAsync(ExecutionPlanRecord record, string filePath)
        {
            var content = $@"SqlId: {record.SqlId}

BindVariables:
{record.BindVariables}

ExecutionLocation: {record.ExecutionLocation}

Query:
{record.Query}

ExecutionPlan:
{record.ExecutionPlan}

AnalysisInfo:
{record.AnalysisInfo}

Notes:
{record.Notes}

CreatedDate: {record.FormattedCreatedDate}
LastAccessDate: {record.FormattedLastAccessDate}
";

            await File.WriteAllTextAsync(filePath, content);
            return filePath;
        }

        public async Task<List<string>> ExportMultipleToTextFileAsync(List<ExecutionPlanRecord> records, string directoryPath)
        {
            var exportedFiles = new List<string>();

            foreach (var record in records)
            {
                var fileName = $"{record.SqlId}_{record.ExecutionLocation}_{record.CreatedDate:yyyyMMdd_HHmmss}.txt"
                    .Replace(".", "_").Replace(":", "_").Replace("/", "_").Replace("\\", "_");
                
                var filePath = Path.Combine(directoryPath, fileName);
                await ExportToTextFileAsync(record, filePath);
                exportedFiles.Add(filePath);
            }

            return exportedFiles;
        }
    }
}
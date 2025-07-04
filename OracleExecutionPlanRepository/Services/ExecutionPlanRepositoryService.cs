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
                    Notes TEXT,
                    SaveStage TEXT DEFAULT 'Unknown',
                    LastUpdateStage TEXT DEFAULT 'Unknown'
                );

                -- 새 컬럼 추가 (기존 테이블이 있는 경우를 위해)
                ALTER TABLE ExecutionPlanRecords ADD COLUMN SaveStage TEXT DEFAULT 'Unknown';
                ALTER TABLE ExecutionPlanRecords ADD COLUMN LastUpdateStage TEXT DEFAULT 'Unknown';

                CREATE INDEX IF NOT EXISTS IX_SqlId ON ExecutionPlanRecords(SqlId);
                CREATE INDEX IF NOT EXISTS IX_ExecutionLocation ON ExecutionPlanRecords(ExecutionLocation);
                CREATE INDEX IF NOT EXISTS IX_CreatedDate ON ExecutionPlanRecords(CreatedDate);
            ";

            createTableCommand.ExecuteNonQuery();
        }

        // public async Task<int> SaveRecordAsync(ExecutionPlanRecord record)
        // {
        //     using var connection = new SqliteConnection(_connectionString);
        //     await connection.OpenAsync();

        //     var command = connection.CreateCommand();

        //     if (record.Id == 0)
        //     {
        //         // Insert
        //         command.CommandText = @"
        //             INSERT INTO ExecutionPlanRecords 
        //             (SqlId, ExecutionLocation, Query, BindVariables, ExecutionPlan, AnalysisInfo, CreatedDate, LastAccessDate, Notes)
        //             VALUES (@SqlId, @ExecutionLocation, @Query, @BindVariables, @ExecutionPlan, @AnalysisInfo, @CreatedDate, @LastAccessDate, @Notes);
        //             SELECT last_insert_rowid();";
        //     }
        //     else
        //     {
        //         // Update
        //         command.CommandText = @"
        //             UPDATE ExecutionPlanRecords SET
        //                 SqlId = @SqlId,
        //                 ExecutionLocation = @ExecutionLocation,
        //                 Query = @Query,
        //                 BindVariables = @BindVariables,
        //                 ExecutionPlan = @ExecutionPlan,
        //                 AnalysisInfo = @AnalysisInfo,
        //                 LastAccessDate = @LastAccessDate,
        //                 Notes = @Notes
        //             WHERE Id = @Id";

        //         command.Parameters.AddWithValue("@Id", record.Id);
        //     }

        //     command.Parameters.AddWithValue("@SqlId", record.SqlId ?? "");
        //     command.Parameters.AddWithValue("@ExecutionLocation", record.ExecutionLocation ?? "");
        //     command.Parameters.AddWithValue("@Query", record.Query ?? "");
        //     command.Parameters.AddWithValue("@BindVariables", record.BindVariables ?? "");
        //     command.Parameters.AddWithValue("@ExecutionPlan", record.ExecutionPlan ?? "");
        //     command.Parameters.AddWithValue("@AnalysisInfo", record.AnalysisInfo ?? "");
        //     command.Parameters.AddWithValue("@CreatedDate", record.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        //     command.Parameters.AddWithValue("@LastAccessDate", record.LastAccessDate.ToString("yyyy-MM-dd HH:mm:ss"));
        //     command.Parameters.AddWithValue("@Notes", record.Notes ?? "");

        //     if (record.Id == 0)
        //     {
        //         var result = await command.ExecuteScalarAsync();
        //         if (result != null && long.TryParse(result.ToString(), out long newId))
        //         {
        //             return (int)newId;
        //         }
        //         return 0;
        //     }
        //     else
        //     {
        //         await command.ExecuteNonQueryAsync();
        //         return record.Id;
        //     }
        // }
        public async Task<int> SaveRecordAsync(ExecutionPlanRecord record)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            
            if (record.Id == 0)
            {
                // Insert - 새 컬럼들 추가
                command.CommandText = @"
                    INSERT INTO ExecutionPlanRecords 
                        (SqlId, ExecutionLocation, Query, BindVariables, ExecutionPlan, AnalysisInfo, CreatedDate, LastAccessDate, Notes, SaveStage, LastUpdateStage)
                        VALUES (@SqlId, @ExecutionLocation, @Query, @BindVariables, @ExecutionPlan, @AnalysisInfo, @CreatedDate, @LastAccessDate, @Notes, @SaveStage, @LastUpdateStage);
                        SELECT last_insert_rowid();";
            }
            else
            {
                // Update - 새 컬럼들 추가
                command.CommandText = @"
                    UPDATE ExecutionPlanRecords SET
                        SqlId = @SqlId,
                        ExecutionLocation = @ExecutionLocation,
                        Query = @Query,
                        BindVariables = @BindVariables,
                        ExecutionPlan = @ExecutionPlan,
                        AnalysisInfo = @AnalysisInfo,
                        LastAccessDate = @LastAccessDate,
                        Notes = @Notes,
                        SaveStage = @SaveStage,
                        LastUpdateStage = @LastUpdateStage
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
            command.Parameters.AddWithValue("@SaveStage", record.SaveStage ?? "Unknown"); // 새로 추가
            command.Parameters.AddWithValue("@LastUpdateStage", record.LastUpdateStage ?? "Unknown"); // 새로 추가

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
                            AnalysisInfo, CreatedDate, LastAccessDate, Notes, SaveStage, LastUpdateStage
                        FROM ExecutionPlanRecords
                        ORDER BY CreatedDate DESC";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                records.Add(new ExecutionPlanRecord
                {
                    Id = reader.GetInt32(0),
                    SqlId = reader.GetString(1),
                    ExecutionLocation = reader.GetString(2),
                    Query = reader.GetString(3),
                    BindVariables = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    ExecutionPlan = reader.GetString(5),
                    AnalysisInfo = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    CreatedDate = DateTime.Parse(reader.GetString(7)),
                    LastAccessDate = DateTime.Parse(reader.GetString(8)),
                    Notes = reader.IsDBNull(9) ? "" : reader.GetString(9),
                    SaveStage = reader.IsDBNull(10) ? "Unknown" : reader.GetString(10), // 새로 추가
                    LastUpdateStage = reader.IsDBNull(11) ? "Unknown" : reader.GetString(11) // 새로 추가
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

        // ExecutionPlanRepositoryService.cs에 추가할 메서드들

        /// <summary>
        /// 쿼리 분석 단계에서 저장 (SqlId, ExecutionLocation, Query만)
        /// </summary>
        public async Task<int> SaveQueryAnalysisAsync(string sqlId, string executionLocation, string query, string bindVariables = "", string notes = "")
        {
            var record = new ExecutionPlanRecord
            {
                SqlId = sqlId,
                ExecutionLocation = executionLocation,
                Query = query,
                BindVariables = bindVariables ?? "",
                ExecutionPlan = "", // 빈 값
                AnalysisInfo = "", // 빈 값
                SaveStage = "QueryAnalysis",
                LastUpdateStage = "QueryAnalysis",
                CreatedDate = DateTime.Now,
                LastAccessDate = DateTime.Now,
                Notes = notes ?? "쿼리 분석 단계에서 저장"
            };

            return await SaveRecordAsync(record);
        }

        /// <summary>
        /// 실행계획 단계에서 저장 또는 업데이트 (기존 레코드가 있으면 업데이트)
        /// </summary>
        public async Task<int> SaveOrUpdateExecutionPlanAsync(string sqlId, string executionLocation, string query, string executionPlan, string bindVariables = "", string notes = "")
        {
            // 기존 레코드 찾기
            var existingRecord = await FindRecordBySqlIdAndLocationAsync(sqlId, executionLocation);

            if (existingRecord != null)
            {
                // 업데이트
                existingRecord.ExecutionPlan = executionPlan;
                existingRecord.LastUpdateStage = "ExecutionPlan";
                existingRecord.LastAccessDate = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(bindVariables))
                    existingRecord.BindVariables = bindVariables;
                if (!string.IsNullOrWhiteSpace(notes))
                    existingRecord.Notes = notes;

                return await SaveRecordAsync(existingRecord);
            }
            else
            {
                // 새로 생성
                var record = new ExecutionPlanRecord
                {
                    SqlId = sqlId,
                    ExecutionLocation = executionLocation,
                    Query = query,
                    BindVariables = bindVariables ?? "",
                    ExecutionPlan = executionPlan,
                    AnalysisInfo = "", // 빈 값
                    SaveStage = "ExecutionPlan",
                    LastUpdateStage = "ExecutionPlan",
                    CreatedDate = DateTime.Now,
                    LastAccessDate = DateTime.Now,
                    Notes = notes ?? "실행계획 단계에서 저장"
                };

                return await SaveRecordAsync(record);
            }
        }

        /// <summary>
        /// 성능 분석 단계에서 저장 또는 업데이트
        /// </summary>
        public async Task<int> SaveOrUpdatePerformanceAnalysisAsync(string sqlId, string executionLocation, string query, string executionPlan, string analysisInfo, string bindVariables = "", string notes = "")
        {
            // 기존 레코드 찾기
            var existingRecord = await FindRecordBySqlIdAndLocationAsync(sqlId, executionLocation);

            if (existingRecord != null)
            {
                // 업데이트
                existingRecord.AnalysisInfo = analysisInfo;
                existingRecord.LastUpdateStage = "PerformanceAnalysis";
                existingRecord.LastAccessDate = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(bindVariables))
                    existingRecord.BindVariables = bindVariables;
                if (!string.IsNullOrWhiteSpace(notes))
                    existingRecord.Notes = notes;

                return await SaveRecordAsync(existingRecord);
            }
            else
            {
                // 새로 생성 (풀 세트)
                var record = new ExecutionPlanRecord
                {
                    SqlId = sqlId,
                    ExecutionLocation = executionLocation,
                    Query = query,
                    BindVariables = bindVariables ?? "",
                    ExecutionPlan = executionPlan,
                    AnalysisInfo = analysisInfo,
                    SaveStage = "PerformanceAnalysis",
                    LastUpdateStage = "PerformanceAnalysis",
                    CreatedDate = DateTime.Now,
                    LastAccessDate = DateTime.Now,
                    Notes = notes ?? "성능 분석 단계에서 저장"
                };

                return await SaveRecordAsync(record);
            }
        }

        /// <summary>
        /// SqlId와 ExecutionLocation으로 기존 레코드 찾기
        /// </summary>
        private async Task<ExecutionPlanRecord> FindRecordBySqlIdAndLocationAsync(string sqlId, string executionLocation)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ExecutionPlanRecords WHERE SqlId = @SqlId AND ExecutionLocation = @ExecutionLocation ORDER BY LastAccessDate DESC LIMIT 1";
            command.Parameters.AddWithValue("@SqlId", sqlId);
            command.Parameters.AddWithValue("@ExecutionLocation", executionLocation);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToRecord(reader);
            }

            return null;
        }

        /// <summary>
        /// SqlDataReader에서 ExecutionPlanRecord로 매핑
        /// </summary>
        private ExecutionPlanRecord MapReaderToRecord(SqliteDataReader reader)
        {
            return new ExecutionPlanRecord
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
                    Notes = reader.IsDBNull(9) ? "" : reader.GetString(9), // Notes
                    // 새로 추가된 필드들 (아직 DB에 컬럼이 없다면 기본값 설정)
                    SaveStage = reader.IsDBNull(10) ? "Unknown" : reader.GetString(10),
                    LastUpdateStage = reader.IsDBNull(11) ? "Unknown" : reader.GetString(11)
            };
        }
    }
}
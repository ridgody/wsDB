// Services/OracleObjectDataService.cs
using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using wsDB.OralceExecutionPlan.DbmsXplan.Models;

namespace wsDB.OracleExecutionPlan.DbmsXplan.Services
{
    public class OracleObjectDataService
    {
        private readonly OracleConnection _connection;

        public OracleObjectDataService(OracleConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            if (_connection.State != ConnectionState.Open)
                throw new InvalidOperationException("데이터베이스 연결이 열려있지 않습니다.");
        }

        public List<PropertyInfo> LoadBasicInfo(string objectName, string objectType, string objectowner)
        {
            List<PropertyInfo> basicInfo = new List<PropertyInfo>();
            
            try
            {
                if (objectType == "TABLE")
                {
                    basicInfo = LoadTableBasicInfo(objectName, objectowner);
                }
                else if (objectType == "INDEX")
                {
                    basicInfo = LoadIndexBasicInfo(objectName, objectowner);
                }
            }
            catch (Exception ex)
            {
                basicInfo.Add(new PropertyInfo { Property = "오류", Value = $"기본 정보 로드 실패: {ex.Message}" });
            }
            
            return basicInfo;
        }

        private List<PropertyInfo> LoadTableBasicInfo(string tableName, string objectowner)
        {
            var basicInfo = new List<PropertyInfo>();
            
            string sql = @"
                SELECT OWNER, TABLE_NAME, TABLESPACE_NAME, STATUS, 
                       NUM_ROWS, BLOCKS, AVG_ROW_LEN, LAST_ANALYZED
                FROM ALL_TABLES 
                WHERE TABLE_NAME = :ObjectName 
                AND OWNER =  :ObjectOwner";
            
            using (OracleCommand cmd = new OracleCommand(sql, _connection))
            {
                cmd.Parameters.Add(":ObjectName", OracleDbType.Varchar2).Value = tableName;
                cmd.Parameters.Add(":ObjectOwner", OracleDbType.Varchar2).Value = objectowner;
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        basicInfo.Add(new PropertyInfo { Property = "소유자", Value = reader["OWNER"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "테이블명", Value = reader["TABLE_NAME"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "테이블스페이스", Value = reader["TABLESPACE_NAME"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "상태", Value = reader["STATUS"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "행 수", Value = reader["NUM_ROWS"]?.ToString() ?? "0" });
                        basicInfo.Add(new PropertyInfo { Property = "블록 수", Value = reader["BLOCKS"]?.ToString() ?? "0" });
                        basicInfo.Add(new PropertyInfo { Property = "평균 행 길이", Value = reader["AVG_ROW_LEN"]?.ToString() ?? "0" });
                        basicInfo.Add(new PropertyInfo { Property = "마지막 분석일", Value = reader["LAST_ANALYZED"]?.ToString() ?? "없음" });
                    }
                }
            }
            
            return basicInfo;
        }

        private List<PropertyInfo> LoadIndexBasicInfo(string indexName, string objectowner)
        {
            var basicInfo = new List<PropertyInfo>();
            
            string sql = @"
                SELECT OWNER, INDEX_NAME, TABLE_NAME, INDEX_TYPE, UNIQUENESS, 
                       STATUS, BLEVEL, LEAF_BLOCKS, DISTINCT_KEYS, LAST_ANALYZED
                FROM ALL_INDEXES 
                WHERE INDEX_NAME = :ObjectName 
                AND OWNER = :ObjectOwner";
            
            using (OracleCommand cmd = new OracleCommand(sql, _connection))
            {
                cmd.Parameters.Add(":ObjectName", OracleDbType.Varchar2).Value = indexName;
                cmd.Parameters.Add(":ObjectOwner", OracleDbType.Varchar2).Value = objectowner;
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        basicInfo.Add(new PropertyInfo { Property = "소유자", Value = reader["OWNER"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "인덱스명", Value = reader["INDEX_NAME"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "테이블명", Value = reader["TABLE_NAME"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "인덱스 유형", Value = reader["INDEX_TYPE"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "유일성", Value = reader["UNIQUENESS"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "상태", Value = reader["STATUS"]?.ToString() ?? "" });
                        basicInfo.Add(new PropertyInfo { Property = "B-Tree 레벨", Value = reader["BLEVEL"]?.ToString() ?? "0" });
                        basicInfo.Add(new PropertyInfo { Property = "리프 블록 수", Value = reader["LEAF_BLOCKS"]?.ToString() ?? "0" });
                        basicInfo.Add(new PropertyInfo { Property = "고유키 수", Value = reader["DISTINCT_KEYS"]?.ToString() ?? "0" });
                        basicInfo.Add(new PropertyInfo { Property = "마지막 분석일", Value = reader["LAST_ANALYZED"]?.ToString() ?? "없음" });
                    }
                }
            }
            
            return basicInfo;
        }

        public List<StatisticInfo> LoadStatistics()
        {
            List<StatisticInfo> statistics = new List<StatisticInfo>();
            
            // 기본 통계 정보 추가
            statistics.Add(new StatisticInfo 
            { 
                StatName = "현재 시각", 
                StatValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                Description = "현재 시각" 
            });
            
            try
            {
                // 간단한 세션 정보 조회
                string sql = @"
                    SELECT 'SESSION_USER' as STAT_NAME, USER as STAT_VALUE FROM DUAL
                    UNION ALL
                    SELECT 'CURRENT_SCHEMA' as STAT_NAME, SYS_CONTEXT('USERENV','CURRENT_SCHEMA') as STAT_VALUE FROM DUAL
                    UNION ALL
                    SELECT 'DB_NAME' as STAT_NAME, SYS_CONTEXT('USERENV','DB_NAME') as STAT_VALUE FROM DUAL";
                
                using (OracleCommand cmd = new OracleCommand(sql, _connection))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statistics.Add(new StatisticInfo 
                            { 
                                StatName = reader["STAT_NAME"]?.ToString() ?? "",
                                StatValue = reader["STAT_VALUE"]?.ToString() ?? "",
                                Description = GetStatisticDescription(reader["STAT_NAME"]?.ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                statistics.Add(new StatisticInfo 
                { 
                    StatName = "오류", 
                    StatValue = "정보 없음", 
                    Description = $"통계 정보 로드 실패: {ex.Message}" 
                });
            }
            
            return statistics;
        }

        private string GetStatisticDescription(string statName)
        {
            var descriptions = new Dictionary<string, string>
            {
                {"SESSION_USER", "현재 세션 사용자"},
                {"CURRENT_SCHEMA", "현재 스키마"},
                {"DB_NAME", "데이터베이스 이름"},
                {"현재 시각", "통계 조회 시각"}
            };
            
            return descriptions.ContainsKey(statName) ? descriptions[statName] : "";
        }

        public List<ColumnInfo> LoadColumnInfo(string tableName, string objectowner)
        {
            List<ColumnInfo> columns = new List<ColumnInfo>();
            
            try
            {
                string sql = @"
                    SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE, DATA_DEFAULT
                    FROM ALL_TAB_COLUMNS 
                    WHERE TABLE_NAME = :ObjectName 
                    AND OWNER = :ObjectOwner
                    ORDER BY COLUMN_ID";
                
                using (OracleCommand cmd = new OracleCommand(sql, _connection))
                {
                    cmd.Parameters.Add(":ObjectName", OracleDbType.Varchar2).Value = tableName;
                    cmd.Parameters.Add(":ObjectOwner", OracleDbType.Varchar2).Value = objectowner;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(new ColumnInfo
                            {
                                ColumnName = reader["COLUMN_NAME"]?.ToString() ?? "",
                                DataType = reader["DATA_TYPE"]?.ToString() ?? "",
                                DataLength = reader["DATA_LENGTH"]?.ToString() ?? "0",
                                Nullable = reader["NULLABLE"]?.ToString() ?? "N",
                                DefaultValue = reader["DATA_DEFAULT"]?.ToString()?.Trim() ?? ""
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                columns.Add(new ColumnInfo
                {
                    ColumnName = "오류",
                    DataType = "ERROR",
                    DataLength = "0",
                    Nullable = "N",
                    DefaultValue = $"컬럼 정보 로드 실패: {ex.Message}"
                });
            }
            
            return columns;
        }

        public List<IndexColumnInfo> LoadIndexInfo(string indexName, string objectowner)
        {
            List<IndexColumnInfo> indexColumns = new List<IndexColumnInfo>();
            
            try
            {
                string sql = @"
                    SELECT COLUMN_NAME, COLUMN_POSITION, DESCEND
                    FROM ALL_IND_COLUMNS 
                    WHERE INDEX_NAME = :ObjectName 
                    AND INDEX_OWNER = :ObjectOwner
                    ORDER BY COLUMN_POSITION";
                
                using (OracleCommand cmd = new OracleCommand(sql, _connection))
                {
                    cmd.Parameters.Add(":ObjectName", OracleDbType.Varchar2).Value = indexName;
                    cmd.Parameters.Add(":ObjectOwner", OracleDbType.Varchar2).Value = objectowner;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            indexColumns.Add(new IndexColumnInfo
                            {
                                ColumnName = reader["COLUMN_NAME"]?.ToString() ?? "",
                                ColumnPosition = reader["COLUMN_POSITION"]?.ToString() ?? "0",
                                DescendFlag = reader["DESCEND"]?.ToString() ?? "ASC"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                indexColumns.Add(new IndexColumnInfo
                {
                    ColumnName = "오류",
                    ColumnPosition = "0",
                    DescendFlag = $"인덱스 정보 로드 실패: {ex.Message}"
                });
            }
            
            return indexColumns;
        }
    }
}
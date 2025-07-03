using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using Oracle.ManagedDataAccess.Client;
using wsDB.OracleExecutionPlan.DbmsXplan.Services;

namespace wsDB.OracleExecutionPlan.DbmsXplan.Views
{
    public partial class ObjectStatisticsWindow : Window
    {
        private OracleConnection dbConnection;
        private string objectType;
        private string objectName;
        private string objectOwner;
        private OracleObjectDataService dataService; 

        public ObjectStatisticsWindow(OracleConnection connection, string objName, string objType, string objectowner)
        {
            try
            {
                InitializeComponent();

                if (connection == null)
                    throw new ArgumentNullException(nameof(connection));

                if (connection.State != ConnectionState.Open)
                    throw new InvalidOperationException("데이터베이스 연결이 열려있지 않습니다.");

                dbConnection = connection;
                objectType = objType ?? "UNKNOWN";
                objectName = objName ?? "UNKNOWN";
                objectOwner = objectowner ?? "UNKNOWN";;

                // 데이터 서비스 초기화
                dataService = new OracleObjectDataService(dbConnection);

                InitializeWindow();
                LoadObjectStatistics();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ObjectStatisticsWindow 초기화 오류: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"ObjectStatisticsWindow 초기화 오류: {ex.Message}", "오류",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                // throw를 제거하여 창이 닫히도록 함
            }
        }

        private void InitializeWindow()
        {
            try
            {
                ObjectTypeText.Text = $"객체 유형: {objectType}";
                ObjectNameText.Text = $"객체명: {objectOwner}.{objectName}";
                Title = $"객체 통계 정보 - {objectOwner}.{objectName}";
                
                // 인덱스인 경우 컬럼 정보 탭 숨기기
                if (objectType == "INDEX")
                {
                    ColumnInfoTab.Visibility = Visibility.Collapsed;
                }
                else
                {
                    IndexInfoTab.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"InitializeWindow 오류: {ex.Message}");
            }
        }

        private void LoadObjectStatistics()
        {
            try
            {
                // 데이터 서비스를 통해 로드
                BasicInfoDataGrid.ItemsSource = dataService.LoadBasicInfo(objectName, objectType, objectOwner);
                StatisticsDataGrid.ItemsSource = dataService.LoadStatistics();
                
                if (objectType == "TABLE")
                {
                    ColumnInfoDataGrid.ItemsSource = dataService.LoadColumnInfo(objectName, objectOwner);
                }
                else if (objectType == "INDEX")
                {
                    IndexInfoDataGrid.ItemsSource = dataService.LoadIndexInfo(objectName, objectOwner);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"통계 정보 로드 중 오류가 발생했습니다: {ex.Message}", "오류", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // private void LoadBasicInfo()
        // {
        //     List<PropertyInfo> basicInfo = new List<PropertyInfo>();
            
        //     try
        //     {
        //         if (objectType == "TABLE")
        //         {
        //             string sql = @"
        //                 SELECT OWNER, TABLE_NAME, TABLESPACE_NAME, STATUS, 
        //                        NUM_ROWS, BLOCKS, AVG_ROW_LEN, LAST_ANALYZED
        //                 FROM ALL_TABLES 
        //                 WHERE TABLE_NAME = :ObjectName 
        //                 AND OWNER = USER";
                    
        //             using (OracleCommand cmd = new OracleCommand(sql, dbConnection))
        //             {
        //                 cmd.Parameters.Add(":ObjectName", OracleDbType.Varchar2).Value = objectName;
        //                 using (OracleDataReader reader = cmd.ExecuteReader())
        //                 {
        //                     if (reader.Read())
        //                     {
        //                         basicInfo.Add(new PropertyInfo { Property = "소유자", Value = reader["OWNER"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "테이블명", Value = reader["TABLE_NAME"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "테이블스페이스", Value = reader["TABLESPACE_NAME"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "상태", Value = reader["STATUS"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "행 수", Value = reader["NUM_ROWS"]?.ToString() ?? "0" });
        //                         basicInfo.Add(new PropertyInfo { Property = "블록 수", Value = reader["BLOCKS"]?.ToString() ?? "0" });
        //                         basicInfo.Add(new PropertyInfo { Property = "평균 행 길이", Value = reader["AVG_ROW_LEN"]?.ToString() ?? "0" });
        //                         basicInfo.Add(new PropertyInfo { Property = "마지막 분석일", Value = reader["LAST_ANALYZED"]?.ToString() ?? "없음" });
        //                     }
        //                 }
        //             }
        //         }
        //         else if (objectType == "INDEX")
        //         {
        //             string sql = @"
        //                 SELECT OWNER, INDEX_NAME, TABLE_NAME, INDEX_TYPE, UNIQUENESS, 
        //                        STATUS, BLEVEL, LEAF_BLOCKS, DISTINCT_KEYS, LAST_ANALYZED
        //                 FROM ALL_INDEXES 
        //                 WHERE INDEX_NAME = :ObjectName 
        //                 AND OWNER = USER";
                    
        //             using (OracleCommand cmd = new OracleCommand(sql, dbConnection))
        //             {
        //                 cmd.Parameters.Add(":ObjectName", OracleDbType.Varchar2).Value = objectName;
        //                 using (OracleDataReader reader = cmd.ExecuteReader())
        //                 {
        //                     if (reader.Read())
        //                     {
        //                         basicInfo.Add(new PropertyInfo { Property = "소유자", Value = reader["OWNER"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "인덱스명", Value = reader["INDEX_NAME"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "테이블명", Value = reader["TABLE_NAME"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "인덱스 유형", Value = reader["INDEX_TYPE"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "유일성", Value = reader["UNIQUENESS"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "상태", Value = reader["STATUS"]?.ToString() ?? "" });
        //                         basicInfo.Add(new PropertyInfo { Property = "B-Tree 레벨", Value = reader["BLEVEL"]?.ToString() ?? "0" });
        //                         basicInfo.Add(new PropertyInfo { Property = "리프 블록 수", Value = reader["LEAF_BLOCKS"]?.ToString() ?? "0" });
        //                         basicInfo.Add(new PropertyInfo { Property = "고유키 수", Value = reader["DISTINCT_KEYS"]?.ToString() ?? "0" });
        //                         basicInfo.Add(new PropertyInfo { Property = "마지막 분석일", Value = reader["LAST_ANALYZED"]?.ToString() ?? "없음" });
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         basicInfo.Add(new PropertyInfo { Property = "오류", Value = $"기본 정보 로드 실패: {ex.Message}" });
        //     }
            
        //     BasicInfoDataGrid.ItemsSource = basicInfo;
        // }

        // private void LoadStatistics()
        // {
        //     List<StatisticInfo> statistics = new List<StatisticInfo>();
            
        //     // 기본 통계 정보 추가
        //     statistics.Add(new StatisticInfo 
        //     { 
        //         StatName = "현재 시각", 
        //         StatValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
        //         Description = "현재 시각" 
        //     });
            
        //     try
        //     {
        //         // 간단한 세션 정보 조회
        //         string sql = @"
        //             SELECT 'SESSION_USER' as STAT_NAME, USER as STAT_VALUE FROM DUAL
        //             UNION ALL
        //             SELECT 'CURRENT_SCHEMA' as STAT_NAME, SYS_CONTEXT('USERENV','CURRENT_SCHEMA') as STAT_VALUE FROM DUAL
        //             UNION ALL
        //             SELECT 'DB_NAME' as STAT_NAME, SYS_CONTEXT('USERENV','DB_NAME') as STAT_VALUE FROM DUAL";
                
        //         using (OracleCommand cmd = new OracleCommand(sql, dbConnection))
        //         {
        //             using (OracleDataReader reader = cmd.ExecuteReader())
        //             {
        //                 while (reader.Read())
        //                 {
        //                     statistics.Add(new StatisticInfo 
        //                     { 
        //                         StatName = reader["STAT_NAME"]?.ToString() ?? "",
        //                         StatValue = reader["STAT_VALUE"]?.ToString() ?? "",
        //                         Description = GetStatisticDescription(reader["STAT_NAME"]?.ToString())
        //                     });
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         statistics.Add(new StatisticInfo 
        //         { 
        //             StatName = "오류", 
        //             StatValue = "정보 없음", 
        //             Description = $"통계 정보 로드 실패: {ex.Message}" 
        //         });
        //     }
            
        //     StatisticsDataGrid.ItemsSource = statistics;
        // }

        // private string GetStatisticDescription(string statName)
        // {
        //     var descriptions = new Dictionary<string, string>
        //     {
        //         {"SESSION_USER", "현재 세션 사용자"},
        //         {"CURRENT_SCHEMA", "현재 스키마"},
        //         {"DB_NAME", "데이터베이스 이름"},
        //         {"현재 시각", "통계 조회 시각"}
        //     };
            
        //     return descriptions.ContainsKey(statName) ? descriptions[statName] : "";
        // }

        // private void LoadColumnInfo()
        // {
        //     List<ColumnInfo> columns = new List<ColumnInfo>();
            
        //     try
        //     {
        //         string sql = @"
        //             SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE, DATA_DEFAULT
        //             FROM ALL_TAB_COLUMNS 
        //             WHERE TABLE_NAME = :ObjectName 
        //             AND OWNER = USER
        //             ORDER BY COLUMN_ID";
                
        //         using (OracleCommand cmd = new OracleCommand(sql, dbConnection))
        //         {
        //             cmd.Parameters.Add(":ObjectName", OracleDbType.Varchar2).Value = objectName;
        //             using (OracleDataReader reader = cmd.ExecuteReader())
        //             {
        //                 while (reader.Read())
        //                 {
        //                     columns.Add(new ColumnInfo
        //                     {
        //                         ColumnName = reader["COLUMN_NAME"]?.ToString() ?? "",
        //                         DataType = reader["DATA_TYPE"]?.ToString() ?? "",
        //                         DataLength = reader["DATA_LENGTH"]?.ToString() ?? "0",
        //                         Nullable = reader["NULLABLE"]?.ToString() ?? "N",
        //                         DefaultValue = reader["DATA_DEFAULT"]?.ToString()?.Trim() ?? ""
        //                     });
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         columns.Add(new ColumnInfo
        //         {
        //             ColumnName = "오류",
        //             DataType = "ERROR",
        //             DataLength = "0",
        //             Nullable = "N",
        //             DefaultValue = $"컬럼 정보 로드 실패: {ex.Message}"
        //         });
        //     }
            
        //     ColumnInfoDataGrid.ItemsSource = columns;
        // }

        // private void LoadIndexInfo()
        // {
        //     List<IndexColumnInfo> indexColumns = new List<IndexColumnInfo>();
            
        //     try
        //     {
        //         string sql = @"
        //             SELECT COLUMN_NAME, COLUMN_POSITION, DESCEND
        //             FROM ALL_IND_COLUMNS 
        //             WHERE INDEX_NAME = :ObjectName 
        //             AND INDEX_OWNER = USER
        //             ORDER BY COLUMN_POSITION";
                
        //         using (OracleCommand cmd = new OracleCommand(sql, dbConnection))
        //         {
        //             cmd.Parameters.Add(":ObjectName", OracleDbType.Varchar2).Value = objectName;
        //             using (OracleDataReader reader = cmd.ExecuteReader())
        //             {
        //                 while (reader.Read())
        //                 {
        //                     indexColumns.Add(new IndexColumnInfo
        //                     {
        //                         ColumnName = reader["COLUMN_NAME"]?.ToString() ?? "",
        //                         ColumnPosition = reader["COLUMN_POSITION"]?.ToString() ?? "0",
        //                         DescendFlag = reader["DESCEND"]?.ToString() ?? "ASC"
        //                     });
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         indexColumns.Add(new IndexColumnInfo
        //         {
        //             ColumnName = "오류",
        //             ColumnPosition = "0",
        //             DescendFlag = $"인덱스 정보 로드 실패: {ex.Message}"
        //         });
        //     }
            
        //     IndexInfoDataGrid.ItemsSource = indexColumns;
        // }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadObjectStatistics();
                MessageBox.Show("통계 정보가 새로고침되었습니다.", "알림", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"새로고침 중 오류가 발생했습니다: {ex.Message}", "오류", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
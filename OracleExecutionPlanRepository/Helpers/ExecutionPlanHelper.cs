// ExecutionPlanRepository/Helpers/ExecutionPlanHelper.cs
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wsDB.OracleExecutionPlanRepository.Models;

namespace wsDB.OracleExecutionPlanRepository.Helpers
{
    public static class ExecutionPlanHelper
    {
        /// <summary>
        /// 텍스트 파일에서 실행계획 레코드를 가져옵니다.
        /// </summary>
        /// <param name="filePath">파일 경로</param>
        /// <returns>ExecutionPlanRecord 객체</returns>
        public static async Task<ExecutionPlanRecord> ImportFromTextFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"파일을 찾을 수 없습니다: {filePath}");

            var content = await File.ReadAllTextAsync(filePath);
            return ParseTextContent(content);
        }

        /// <summary>
        /// 텍스트 내용을 파싱하여 ExecutionPlanRecord 객체를 생성합니다.
        /// </summary>
        /// <param name="content">텍스트 내용</param>
        /// <returns>ExecutionPlanRecord 객체</returns>
        public static ExecutionPlanRecord ParseTextContent(string content)
        {
            var record = new ExecutionPlanRecord
            {
                CreatedDate = DateTime.Now,
                LastAccessDate = DateTime.Now
            };

            try
            {
                // 각 섹션을 정규식으로 추출
                record.SqlId = ExtractSection(content, @"SqlId:\s*(.+?)(?=\n\n|\nBindVariables:|\n[A-Z]|$)");
                record.BindVariables = ExtractSection(content, @"BindVariables:\s*\n(.*?)(?=\n\nExecutionLocation:|\n[A-Z]|$)", RegexOptions.Singleline);
                record.ExecutionLocation = ExtractSection(content, @"ExecutionLocation:\s*(.+?)(?=\n\n|\nQuery:|\n[A-Z]|$)");
                record.Query = ExtractSection(content, @"Query:\s*\n(.*?)(?=\n\nExecutionPlan:|\n[A-Z]|$)", RegexOptions.Singleline);
                record.ExecutionPlan = ExtractSection(content, @"ExecutionPlan:\s*\n(.*?)(?=\n\nAnalysisInfo:|\n[A-Z]|$)", RegexOptions.Singleline);
                record.AnalysisInfo = ExtractSection(content, @"AnalysisInfo:\s*\n(.*?)(?=\n\nNotes:|\n[A-Z]|$)", RegexOptions.Singleline);
                record.Notes = ExtractSection(content, @"Notes:\s*\n(.*?)(?=\n\nCreatedDate:|\n[A-Z]|$)", RegexOptions.Singleline);

                // 날짜 정보 추출
                var createdDateStr = ExtractSection(content, @"CreatedDate:\s*(.+?)(?=\n|$)");
                if (DateTime.TryParse(createdDateStr, out DateTime createdDate))
                {
                    record.CreatedDate = createdDate;
                }

                var lastAccessDateStr = ExtractSection(content, @"LastAccessDate:\s*(.+?)(?=\n|$)");
                if (DateTime.TryParse(lastAccessDateStr, out DateTime lastAccessDate))
                {
                    record.LastAccessDate = lastAccessDate;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"파일 파싱 중 오류가 발생했습니다: {ex.Message}", ex);
            }

            return record;
        }

        /// <summary>
        /// 정규식을 사용하여 텍스트에서 특정 섹션을 추출합니다.
        /// </summary>
        /// <param name="content">전체 텍스트</param>
        /// <param name="pattern">정규식 패턴</param>
        /// <param name="options">정규식 옵션</param>
        /// <returns>추출된 텍스트</returns>
        private static string ExtractSection(string content, string pattern, RegexOptions options = RegexOptions.None)
        {
            var match = Regex.Match(content, pattern, options | RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : "";
        }

        /// <summary>
        /// SQL ID의 유효성을 검사합니다.
        /// </summary>
        /// <param name="sqlId">SQL ID</param>
        /// <returns>유효한 경우 true</returns>
        public static bool IsValidSqlId(string sqlId)
        {
            if (string.IsNullOrWhiteSpace(sqlId))
                return false;

            // SQL ID는 영문자, 숫자, 언더스코어만 허용 (최대 50자)
            return Regex.IsMatch(sqlId, @"^[a-zA-Z0-9_]{1,50}$");
        }

        /// <summary>
        /// 실행 위치의 유효성을 검사합니다.
        /// </summary>
        /// <param name="executionLocation">실행 위치</param>
        /// <returns>유효한 경우 true</returns>
        public static bool IsValidExecutionLocation(string executionLocation)
        {
            if (string.IsNullOrWhiteSpace(executionLocation))
                return false;

            // 실행 위치는 점(.)으로 구분된 계층 구조 (예: sendsms.getHomeServiceSmsList)
            return Regex.IsMatch(executionLocation, @"^[a-zA-Z0-9_]+(\.[a-zA-Z0-9_]+)*$");
        }

        /// <summary>
        /// 자동으로 SQL ID를 생성합니다.
        /// </summary>
        /// <param name="prefix">접두사</param>
        /// <returns>생성된 SQL ID</returns>
        public static string GenerateAutomaticSqlId(string prefix = "AUTO")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}_{timestamp}_{random}";
        }

        /// <summary>
        /// 실행계획에서 주요 정보를 추출합니다.
        /// </summary>
        /// <param name="executionPlan">실행계획 텍스트</param>
        /// <returns>추출된 정보</returns>
        public static ExecutionPlanSummary ExtractExecutionPlanSummary(string executionPlan)
        {
            var summary = new ExecutionPlanSummary();

            if (string.IsNullOrWhiteSpace(executionPlan))
                return summary;

            try
            {
                // Plan hash value 추출
                var planHashMatch = Regex.Match(executionPlan, @"Plan hash value:\s*(\d+)");
                if (planHashMatch.Success)
                {
                    summary.PlanHashValue = planHashMatch.Groups[1].Value;
                }

                // Cost 추출 (첫 번째 라인에서)
                var costMatch = Regex.Match(executionPlan, @"Cost:\s*(\d+)");
                if (costMatch.Success)
                {
                    summary.TotalCost = int.Parse(costMatch.Groups[1].Value);
                }

                // 테이블 스캔 방식 확인
                summary.HasFullTableScan = executionPlan.Contains("TABLE ACCESS FULL");
                summary.HasIndexScan = executionPlan.Contains("INDEX") && (
                    executionPlan.Contains("INDEX RANGE SCAN") ||
                    executionPlan.Contains("INDEX UNIQUE SCAN") ||
                    executionPlan.Contains("INDEX FAST FULL SCAN"));

                // 조인 방식 확인
                summary.HasNestedLoops = executionPlan.Contains("NESTED LOOPS");
                summary.HasHashJoin = executionPlan.Contains("HASH JOIN");
                summary.HasSortMergeJoin = executionPlan.Contains("SORT MERGE JOIN");

                // 단계 수 계산
                var stepMatches = Regex.Matches(executionPlan, @"^\s*\|\s*\*?\s*\d+\s*\|", RegexOptions.Multiline);
                summary.StepCount = stepMatches.Count;
            }
            catch (Exception ex)
            {
                summary.ErrorMessage = $"요약 정보 추출 중 오류: {ex.Message}";
            }

            return summary;
        }
    }

    /// <summary>
    /// 실행계획 요약 정보
    /// </summary>
    public class ExecutionPlanSummary
    {
        public string PlanHashValue { get; set; } = "";
        public int TotalCost { get; set; }
        public int StepCount { get; set; }
        public bool HasFullTableScan { get; set; }
        public bool HasIndexScan { get; set; }
        public bool HasNestedLoops { get; set; }
        public bool HasHashJoin { get; set; }
        public bool HasSortMergeJoin { get; set; }
        public string ErrorMessage { get; set; } = "";

        public string GetScanMethodSummary()
        {
            var methods = new List<string>();
            if (HasFullTableScan) methods.Add("Full Table Scan");
            if (HasIndexScan) methods.Add("Index Scan");
            return methods.Count > 0 ? string.Join(", ", methods) : "Unknown";
        }

        public string GetJoinMethodSummary()
        {
            var methods = new List<string>();
            if (HasNestedLoops) methods.Add("Nested Loops");
            if (HasHashJoin) methods.Add("Hash Join");
            if (HasSortMergeJoin) methods.Add("Sort Merge Join");
            return methods.Count > 0 ? string.Join(", ", methods) : "No Joins";
        }
    }
}
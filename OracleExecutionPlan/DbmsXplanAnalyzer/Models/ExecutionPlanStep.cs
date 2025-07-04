namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models
{
    public class ExecutionPlanStep
    {
        public int Id { get; set; }
        public string Operation { get; set; }
        public string Name { get; set; }
        public long Starts { get; set; }
        public long EstimatedRows { get; set; }
        public long Cost { get; set; }
        public double CpuPercent { get; set; }
        public string EstimatedTime { get; set; }
        public long ActualRows { get; set; }
        public string ActualTime { get; set; }
        public long Buffers { get; set; }
        public long Reads { get; set; }
        public string OMem { get; set; }
        public string OneMem { get; set; }
        public string UsedMem { get; set; }
        public int Level { get; set; }
        public string Filter { get; set; }

           // 새로 추가된 필드들
        public bool HasPredicate { get; set; }  // "*"로 시작하는지 여부
        public string PredicateInfo { get; set; } // Predicate Information에서 가져온 상세 정보
        

        public long Rows => ActualRows;  // 추가

        // 계산된 속성들
        public double ActualTimeSeconds { get; set; }
        public double BufferToRowRatio { get; set; }
        public double EstimateVsActualRatio { get; set; }
        public PerformanceIssueType IssueType { get; set; }
        public PerformanceSeverity Severity { get; set; }
    }

    public enum PerformanceIssueType
    {
        None,
        HighBufferReads,
        HighPhysicalReads,
        CardinalityMismatch,
        LongExecutionTime,
        FullTableScan,
        NestedLoopIssue,
        SortingIssue,
        HashJoinIssue,
        IndexIssue
    }

    public enum PerformanceSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
    
    
    public static class PerformanceSeverityExtensions
    {
        /// <summary>
        /// 두 PerformanceSeverity 중 더 높은 심각도를 반환합니다.
        /// </summary>
        public static PerformanceSeverity Max(this PerformanceSeverity severity1, PerformanceSeverity severity2)
        {
            return (PerformanceSeverity)Math.Max((int)severity1, (int)severity2);
        }

        /// <summary>
        /// 현재 심각도와 비교하여 더 높은 심각도를 반환합니다.
        /// </summary>
        public static PerformanceSeverity CombineWith(this PerformanceSeverity current, PerformanceSeverity other)
        {
            return current.Max(other);
        }

        /// <summary>
        /// 심각도를 숫자 값으로 반환합니다.
        /// </summary>
        public static int ToNumericValue(this PerformanceSeverity severity)
        {
            return (int)severity;
        }
    }

}
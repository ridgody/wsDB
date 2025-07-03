namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models
{
    public class PerformanceAnalysisResult
    {
        public List<ExecutionPlanStep> Steps { get; set; } = new List<ExecutionPlanStep>();
        public List<PerformanceIssue> Issues { get; set; } = new List<PerformanceIssue>();
        public PerformanceSummary Summary { get; set; } = new PerformanceSummary();
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    public class PerformanceIssue
    {
        public int StepId { get; set; }
        public PerformanceIssueType Type { get; set; }
        public PerformanceSeverity Severity { get; set; }
        public string Description { get; set; }
        public string Recommendation { get; set; }
        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();
    }

    public class PerformanceSummary
    {
        public double TotalExecutionTimeSeconds { get; set; }
        public long TotalBufferReads { get; set; }
        public long TotalPhysicalReads { get; set; }
        public long TotalActualRows { get; set; }
        public int TotalSteps { get; set; }
        public int HighImpactSteps { get; set; }
        public int CriticalIssues { get; set; }
        public int HighIssues { get; set; }
        public string MostExpensiveOperation { get; set; }
        public int MostExpensiveStepId { get; set; }
    }

    public static class PerformanceAnalysisExtensions
    {
        /// <summary>
        /// 실행계획에서 가장 문제가 되는 상위 N개 단계를 반환
        /// </summary>
        public static List<ExecutionPlanStep> GetTopProblematicSteps(
            this PerformanceAnalysisResult result, int count = 5)
        {
            return result.Steps
                .Where(s => s.Severity >= PerformanceSeverity.Medium)
                .OrderByDescending(s => s.ActualTimeSeconds)
                .ThenByDescending(s => s.BufferToRowRatio)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// 특정 테이블과 관련된 성능 이슈를 반환
        /// </summary>
        public static List<PerformanceIssue> GetTableRelatedIssues(
            this PerformanceAnalysisResult result, string tableName)
        {
            var tableStepIds = result.Steps
                .Where(s => s.Name.Contains(tableName, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Id)
                .ToHashSet();

            return result.Issues
                .Where(i => tableStepIds.Contains(i.StepId))
                .ToList();
        }

        /// <summary>
        /// 성능 개선 우선순위를 계산
        /// </summary>
        public static List<PerformanceImprovementSuggestion> GetImprovementSuggestions(
            this PerformanceAnalysisResult result)
        {
            var suggestions = new List<PerformanceImprovementSuggestion>();

            // 가장 오래 걸리는 단계들
            var slowSteps = result.Steps
                .Where(s => s.ActualTimeSeconds > 1)
                .OrderByDescending(s => s.ActualTimeSeconds)
                .Take(3);

            foreach (var step in slowSteps)
            {
                suggestions.Add(new PerformanceImprovementSuggestion
                {
                    StepId = step.Id,
                    Priority = CalculatePriority(step),
                    Description = $"Step {step.Id} 최적화",
                    ExpectedImprovement = step.ActualTimeSeconds,
                    Suggestion = GenerateSpecificSuggestion(step)
                });
            }

            return suggestions.OrderByDescending(s => s.Priority).ToList();
        }

        private static int CalculatePriority(ExecutionPlanStep step)
        {
            int priority = 0;
            
            // 실행 시간 기반 우선순위
            if (step.ActualTimeSeconds > 60) priority += 50;
            else if (step.ActualTimeSeconds > 10) priority += 30;
            else if (step.ActualTimeSeconds > 1) priority += 10;

            // Buffer 읽기 기반 우선순위
            if (step.BufferToRowRatio > 10000) priority += 40;
            else if (step.BufferToRowRatio > 1000) priority += 20;

            // 심각도 기반 우선순위
            priority += step.Severity switch
            {
                PerformanceSeverity.Critical => 100,
                PerformanceSeverity.High => 50,
                PerformanceSeverity.Medium => 25,
                _ => 0
            };

            return priority;
        }

        private static string GenerateSpecificSuggestion(ExecutionPlanStep step)
        {
            if (step.Operation.Contains("FULL"))
            {
                return $"테이블 {step.Name}에 적절한 인덱스를 생성하여 Full Scan을 방지하세요.";
            }
            else if (step.Operation.Contains("NESTED LOOPS") && step.BufferToRowRatio > 1000)
            {
                return "Nested Loop 조인을 Hash Join으로 변경하거나 더 효율적인 인덱스를 사용하세요.";
            }
            else if (step.BufferToRowRatio > 1000)
            {
                return $"인덱스 {step.Name}의 선택성을 개선하거나 복합 인덱스를 고려하세요.";
            }
            else
            {
                return "해당 단계의 실행 계획을 재검토하고 최적화 방안을 찾아보세요.";
            }
        }
    }

    public class PerformanceImprovementSuggestion
    {
        public int StepId { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
        public double ExpectedImprovement { get; set; }
        public string Suggestion { get; set; }
    }
}
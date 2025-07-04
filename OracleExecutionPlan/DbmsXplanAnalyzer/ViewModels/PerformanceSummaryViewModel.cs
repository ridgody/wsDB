using System.Windows.Media;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models;

namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.ViewModels
{    
    public class PerformanceSummaryViewModel
    {
        public PerformanceSummary Summary { get; }

        public PerformanceSummaryViewModel(PerformanceSummary summary)
        {
            Summary = summary;
        }

        public string TotalExecutionTimeDisplay => 
            $"총 실행시간: {Summary.TotalExecutionTimeSeconds:F2}초";

        public string TotalBufferReadsDisplay => 
            $"총 Buffer 읽기: {Summary.TotalBufferReads:N0}";

        public string TotalPhysicalReadsDisplay => 
            $"총 Physical 읽기: {Summary.TotalPhysicalReads:N0}";

        public string CriticalIssuesDisplay => 
            $"심각한 이슈: {Summary.CriticalIssues}개";

        public string HighIssuesDisplay => 
            $"높은 이슈: {Summary.HighIssues}개";
        
        public string MediumIssuesDisplay =>     // 추가 필요
        $"중간 이슈: {Summary.MediumIssues}개";

        public string LowIssuesDisplay =>        // 추가 필요
        $"낮은 이슈: {Summary.LowIssues}개";

        public string MostExpensiveDisplay =>
            $"가장 비싼 단계: {Summary.MostExpensiveStepId} ({Summary.MostExpensiveOperation})";
    }
}
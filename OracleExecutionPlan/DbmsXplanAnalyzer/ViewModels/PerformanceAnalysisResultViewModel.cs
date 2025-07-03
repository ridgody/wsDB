using System.Windows.Media;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models;

namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.ViewModels
{
    public class PerformanceAnalysisResultViewModel
    {
        public PerformanceAnalysisResult Result { get; }
        public List<PerformanceIssueViewModel> Issues { get; }
        public List<ExecutionPlanStepViewModel> Steps { get; }
        public PerformanceSummaryViewModel Summary { get; }
        public List<string> Recommendations { get; }

        public PerformanceAnalysisResultViewModel(PerformanceAnalysisResult result)
        {
            Result = result;
            Issues = result.Issues.Select(i => new PerformanceIssueViewModel(i)).ToList();
            Steps = result.Steps.Select(s => new ExecutionPlanStepViewModel(s)).ToList();
            Summary = new PerformanceSummaryViewModel(result.Summary);
            Recommendations = result.Recommendations;
        }
    }

}
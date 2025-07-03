using System.Windows.Media;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models;

namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.ViewModels
{
    public class PerformanceIssueViewModel
    {
        public PerformanceIssue Issue { get; }

        public PerformanceIssueViewModel(PerformanceIssue issue)
        {
            Issue = issue;
        }

        public int StepId => Issue.StepId;
        public string Description => Issue.Description;
        public string Recommendation => Issue.Recommendation;
        public PerformanceSeverity Severity => Issue.Severity;
        
        public string TypeDisplay => Issue.Type switch
        {
            PerformanceIssueType.HighBufferReads => "높은 Buffer 읽기",
            PerformanceIssueType.HighPhysicalReads => "높은 Physical 읽기",
            PerformanceIssueType.CardinalityMismatch => "카디널리티 불일치",
            PerformanceIssueType.LongExecutionTime => "긴 실행시간",
            PerformanceIssueType.FullTableScan => "Full Table Scan",
            PerformanceIssueType.NestedLoopIssue => "Nested Loop 이슈",
            PerformanceIssueType.SortingIssue => "정렬 이슈",
            PerformanceIssueType.HashJoinIssue => "Hash Join 이슈",
            PerformanceIssueType.IndexIssue => "인덱스 이슈",
            _ => "기타"
        };

        public Brush SeverityColor => Issue.Severity switch
        {
            PerformanceSeverity.Critical => Brushes.Red,
            PerformanceSeverity.High => Brushes.Orange,
            PerformanceSeverity.Medium => Brushes.Yellow,
            PerformanceSeverity.Low => Brushes.LightBlue,
            _ => Brushes.Gray
        };
    }
 
}
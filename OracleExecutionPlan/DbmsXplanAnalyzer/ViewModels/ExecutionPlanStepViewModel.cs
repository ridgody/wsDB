using System.Windows.Media;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models;

namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.ViewModels
{
    public class ExecutionPlanStepViewModel
    {
        public ExecutionPlanStep Step { get; }

        public ExecutionPlanStepViewModel(ExecutionPlanStep step)
        {
            Step = step;
        }

        public int Id => Step.Id;
        public string Operation => Step.Operation;
        public string Name => Step.Name;
        public double ActualTimeSeconds => Step.ActualTimeSeconds;
        public long Buffers => Step.Buffers;
        public long Reads => Step.Reads;
        public long ActualRows => Step.ActualRows;
        public long EstimatedRows => Step.EstimatedRows;
        public double BufferToRowRatio => Step.BufferToRowRatio;
        public PerformanceSeverity Severity => Step.Severity;

        public Brush SeverityColor => Step.Severity switch
        {
            PerformanceSeverity.Critical => Brushes.Red,
            PerformanceSeverity.High => Brushes.Orange,
            PerformanceSeverity.Medium => Brushes.Yellow,
            PerformanceSeverity.Low => Brushes.LightBlue,
            _ => Brushes.Gray
        };
    }
 
}
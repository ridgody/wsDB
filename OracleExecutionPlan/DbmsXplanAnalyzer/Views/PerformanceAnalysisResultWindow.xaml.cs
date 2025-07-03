using System.Windows;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.ViewModels;

namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Views
{
    public partial class PerformanceAnalysisResultWindow : Window
    {
        public PerformanceAnalysisResultWindow(PerformanceAnalysisResult result)
        {
            InitializeComponent();
            DataContext = new PerformanceAnalysisResultViewModel(result);
        }
    }
}
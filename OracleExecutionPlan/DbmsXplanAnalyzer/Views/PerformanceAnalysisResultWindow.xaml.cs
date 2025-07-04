using System.Windows;
using wsDB.OracleExecutionPlan.DbmsXplan.Views;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.ViewModels;
using wsDB.OracleExecutionPlanRepository.Services;

namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Views
{
    public partial class PerformanceAnalysisResultWindow : Window
    {
        private readonly ExecutionPlanRepositoryService _repositoryService;
        private readonly PerformanceAnalysisResult _analysisResult;
        private readonly string _originalExecutionPlan;

        public PerformanceAnalysisResultWindow(PerformanceAnalysisResult result, string originalExecutionPlan = "")
        {
            InitializeComponent();
            _repositoryService = new ExecutionPlanRepositoryService();
            _analysisResult = result;
            _originalExecutionPlan = originalExecutionPlan;
            DataContext = new PerformanceAnalysisResultViewModel(result);
        }

        public PerformanceAnalysisResultWindow(PerformanceAnalysisResult result) : this(result, "")
        {
        }


        // 새로운 메서드들 추가
        private async void SaveAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 입력 다이얼로그를 통해 사용자로부터 정보 수집
                var inputDialog = new ExecutionPlanInputDialog();
                inputDialog.Owner = this;
                
                if (inputDialog.ShowDialog() == true)
                {
                    // 분석 정보 생성
                    string analysisInfo = GenerateDetailedAnalysisSummary(_analysisResult);

                    // 성능 분석 단계에서 저장 또는 업데이트 (풀 세트)
                    int savedId = await _repositoryService.SaveOrUpdatePerformanceAnalysisAsync(
                        inputDialog.SqlId,
                        inputDialog.ExecutionLocation,
                        inputDialog.Query,
                        _originalExecutionPlan ?? "실행계획 정보 없음",
                        analysisInfo,
                        inputDialog.BindVariables,
                        "성능 분석 완료 후 저장");

                    MessageBox.Show($"성능 분석 결과가 저장되었습니다.\nSQL ID: {inputDialog.SqlId}\n저장 ID: {savedId}", 
                        "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"저장 중 오류가 발생했습니다: {ex.Message}", "오류", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // 상세 분석 요약 생성 메서드
        private string GenerateDetailedAnalysisSummary(PerformanceAnalysisResult result)
        {
            var summary = $@"=== 성능 분석 결과 상세 ===
        분석 일시: {DateTime.Now:yyyy-MM-dd HH:mm:ss}

        [전체 요약]
        - 총 단계 수: {result.Summary.TotalSteps}
        - 총 실행시간: {result.Summary.TotalExecutionTimeSeconds:F2}초
        - 총 Buffer 읽기: {result.Summary.TotalBufferReads:N0}
        - 총 Physical 읽기: {result.Summary.TotalPhysicalReads:N0}
        - 심각한 이슈: {result.Summary.CriticalIssues}개
        - 높은 이슈: {result.Summary.HighIssues}개
        - 중간 이슈: {result.Summary.MediumIssues}개
        - 낮은 이슈: {result.Summary.LowIssues}개
        - 가장 비싼 단계: Step {result.Summary.MostExpensiveStepId} ({result.Summary.MostExpensiveOperation})

        [주요 성능 이슈]";

            foreach (var issue in result.Issues.Take(10))
            {
                summary += $@"
        - Step {issue.StepId} ({issue.Severity}): {issue.Description}";
            }

            if (result.Recommendations.Any())
            {
                summary += "\n\n[개선 권장사항]";
                foreach (var recommendation in result.Recommendations)
                {
                    summary += $"\n- {recommendation}";
                }
            }

            summary += $@"

        [단계별 상세 정보]";
            foreach (var step in result.Steps.Take(20))
            {
                summary += $@"
        Step {step.Id}: {step.Operation} - Cost: {step.Cost}, Rows: {step.Rows}";
            }

            return summary;
        }
    }
}
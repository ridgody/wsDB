using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using wsDB.OracleQueryAnalyzer.Services;
using wsDB.OracleQueryAnalyzer.Models;
using wsDB.OracleExecutionPlan.DbmsXplan.Views;

namespace wsDB.OracleQueryAnalyzer.Views
{
    public partial class QueryAnalysisWindow : Window
    {
        private readonly OracleConnection _dbConnection;
        private readonly QueryProcessor _queryProcessor;
        private readonly ExecutionPlanService _executionPlanService;

        public QueryAnalysisWindow(OracleConnection connection)
        {
            InitializeComponent();
            _dbConnection = connection;
            _queryProcessor = new QueryProcessor();
            _executionPlanService = new ExecutionPlanService(connection, this);
            
            // Ctrl+V 단축키 설정
            this.KeyDown += QueryAnalysisWindow_KeyDown;
        }

        private void QueryAnalysisWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PasteFromClipboard();
            }
        }

        private void QueryTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true; // 기본 붙여넣기 방지
                PasteFromClipboard();
            }
        }

        private void PasteFromClipboard()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    QueryTextBox.Text = clipboardText;
                    StatusTextBlock.Text = "클립보드에서 쿼리를 가져왔습니다.";
                    ValidateQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"클립보드 붙여넣기 중 오류가 발생했습니다: {ex.Message}", "오류", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "SQL 쿼리 파일 선택",
                Filter = "SQL files (*.sql)|*.sql|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string content = File.ReadAllText(openFileDialog.FileName);
                    QueryTextBox.Text = content;
                    StatusTextBlock.Text = $"파일을 불러왔습니다: {Path.GetFileName(openFileDialog.FileName)}";
                    ValidateQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일 로드 중 오류가 발생했습니다: {ex.Message}", "오류", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void QueryTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateQuery();
        }

        private void ValidateQuery()
        {
            string query = QueryTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(query))
            {
                AnalyzeButton.IsEnabled = false;
                StatusTextBlock.Text = "쿼리를 입력하세요.";
                return;
            }

            var validationResult = _queryProcessor.ValidateQuery(query);
            AnalyzeButton.IsEnabled = validationResult.IsValid;
            StatusTextBlock.Text = validationResult.IsValid ? "분석 준비 완료" : validationResult.ErrorMessage;
        }

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnalyzeButton.IsEnabled = false;
                StatusTextBlock.Text = "실행계획을 분석하고 있습니다...";

                string originalQuery = QueryTextBox.Text.Trim();
                
                // 쿼리 처리 및 힌트 추가
                string processedQuery = _queryProcessor.ProcessQueryForExecution(originalQuery);

                //MessageBox.Show($"processedQuery");

                // 실행계획 분석 실행
                var result = await _executionPlanService.AnalyzeExecutionPlanAsync(processedQuery);
                
                //MessageBox.Show($"processedQuery result");
                
                if (result.Success)
                {
                    // ExecutionPlanAnalyzer 창 열기                 
                    var planAnalyzer = new DbmsXPlanWindow(_dbConnection, result.ExecutionPlan);
                    planAnalyzer.Owner = this;
                    planAnalyzer.Show(); 
                }
                else
                {
                    MessageBox.Show($"실행계획 분석 중 오류가 발생했습니다:\n{result.ErrorMessage}",
                        "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    StatusTextBlock.Text = "분석 실패";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예기치 않은 오류가 발생했습니다: {ex.Message}", "오류", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusTextBlock.Text = "분석 실패";
            }
            finally
            {
                AnalyzeButton.IsEnabled = true;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
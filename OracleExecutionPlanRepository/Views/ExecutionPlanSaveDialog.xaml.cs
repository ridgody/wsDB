// ExecutionPlanRepository/Views/ExecutionPlanSaveDialog.xaml.cs
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using wsDB.OracleExecutionPlanRepository.Models;
using wsDB.OracleExecutionPlanRepository.Services;
using wsDB.OracleExecutionPlanRepository.ViewModels;

namespace wsDB.OracleExecutionPlanRepository.Views
{
    public partial class ExecutionPlanSaveDialog : Window
    {
        public ExecutionPlanRecord ExecutionPlanRecord { get; private set; }
        private readonly ExecutionPlanRepositoryService _repositoryService;

        public ExecutionPlanSaveDialog(ExecutionPlanRecord record = null)
        {
            InitializeComponent();
            _repositoryService = new ExecutionPlanRepositoryService();
            
            var viewModel = new ExecutionPlanSaveDialogViewModel(record);
            DataContext = viewModel;
            
            ExecutionPlanRecord = viewModel.Record;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = DataContext as ExecutionPlanSaveDialogViewModel;
                var record = viewModel.Record;

                // 유효성 검사
                if (string.IsNullOrWhiteSpace(record.SqlId))
                {
                    MessageBox.Show("SQL ID를 입력해주세요.", "입력 오류", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    SqlIdTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(record.ExecutionLocation))
                {
                    MessageBox.Show("실행 위치를 입력해주세요.", "입력 오류", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    ExecutionLocationTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(record.Query))
                {
                    MessageBox.Show("쿼리를 입력해주세요.", "입력 오류", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    QueryTextBox.Focus();
                    return;
                }

                // 날짜 설정
                if (record.Id == 0)
                {
                    record.CreatedDate = DateTime.Now;
                }
                record.LastAccessDate = DateTime.Now;

                bool saveToDb = SaveToDbCheckBox.IsChecked == true;
                bool saveToFile = SaveToFileCheckBox.IsChecked == true;

                // 데이터베이스 저장
                if (saveToDb)
                {
                    var savedId = await _repositoryService.SaveRecordAsync(record);
                    record.Id = savedId;
                    
                    MessageBox.Show("데이터베이스에 저장되었습니다.", "저장 완료", 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // 파일 저장
                if (saveToFile)
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Title = "실행계획 정보 저장",
                        Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*",
                        DefaultExt = "txt",
                        FileName = $"{record.SqlId}_{record.ExecutionLocation}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                            .Replace(".", "_").Replace(":", "_")
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        await _repositoryService.ExportToTextFileAsync(record, saveFileDialog.FileName);
                        MessageBox.Show($"파일로 저장되었습니다.\n{saveFileDialog.FileName}", "저장 완료", 
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                if (!saveToDb && !saveToFile)
                {
                    MessageBox.Show("저장 옵션을 선택해주세요.", "저장 오류", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ExecutionPlanRecord = record;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"저장 중 오류가 발생했습니다: {ex.Message}", "오류", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
// ExecutionPlanRepository/Views/ExecutionPlanImportDialog.xaml.cs
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using wsDB.OracleExecutionPlanRepository.Models;
using wsDB.OracleExecutionPlanRepository.Helpers;

namespace wsDB.OracleExecutionPlanRepository.Views
{
    public partial class ExecutionPlanImportDialog : Window
    {
        public ExecutionPlanRecord ImportedRecord { get; private set; }
        private string _selectedFilePath;
        private string _currentContent;

        public ExecutionPlanImportDialog()
        {
            InitializeComponent();
            UpdatePreview();
        }

        private void ImportOption_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "실행계획 텍스트 파일 선택",
                Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                SelectedFileText.Text = Path.GetFileName(_selectedFilePath);
                ImportFromFileRadio.IsChecked = true;
                UpdatePreview();
            }
        }

        private async void UpdatePreview()
        {
            try
            {
                _currentContent = "";

                if (ImportFromFileRadio.IsChecked == true && !string.IsNullOrEmpty(_selectedFilePath))
                {
                    if (File.Exists(_selectedFilePath))
                    {
                        _currentContent = await File.ReadAllTextAsync(_selectedFilePath);
                    }
                }
                else if (ImportFromClipboardRadio.IsChecked == true)
                {
                    if (Clipboard.ContainsText())
                    {
                        _currentContent = Clipboard.GetText();
                    }
                }

                PreviewTextBox.Text = _currentContent;
                ImportButton.IsEnabled = !string.IsNullOrWhiteSpace(_currentContent);
            }
            catch (Exception ex)
            {
                PreviewTextBox.Text = $"미리보기 오류: {ex.Message}";
                ImportButton.IsEnabled = false;
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_currentContent))
                {
                    MessageBox.Show("가져올 내용이 없습니다.", "가져오기 오류", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 텍스트 내용을 파싱하여 레코드 생성
                ImportedRecord = ExecutionPlanHelper.ParseTextContent(_currentContent);
                
                if (string.IsNullOrWhiteSpace(ImportedRecord.SqlId))
                {
                    ImportedRecord.SqlId = ExecutionPlanHelper.GenerateAutomaticSqlId("IMPORT");
                }

                // 유효성 검사
                if (string.IsNullOrWhiteSpace(ImportedRecord.ExecutionPlan))
                {
                    MessageBox.Show("실행계획 정보를 찾을 수 없습니다.", "가져오기 오류", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"가져오기 중 오류가 발생했습니다: {ex.Message}", "가져오기 오류", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            
            // 창이 활성화될 때 클립보드 옵션이 선택되어 있으면 미리보기 업데이트
            if (ImportFromClipboardRadio.IsChecked == true)
            {
                UpdatePreview();
            }
        }
    }
}
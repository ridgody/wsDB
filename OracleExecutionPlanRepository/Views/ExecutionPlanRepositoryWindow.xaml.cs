// ExecutionPlanRepository/Views/ExecutionPlanRepositoryWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using wsDB.OracleExecutionPlanRepository.Models;
using wsDB.OracleExecutionPlanRepository.Services;

namespace wsDB.OracleExecutionPlanRepository.Views
{
    public partial class ExecutionPlanRepositoryWindow : Window
    {
        private readonly ExecutionPlanRepositoryService _repositoryService;
        private List<ExecutionPlanRecord> _allRecords;
        private List<ExecutionPlanRecord> _filteredRecords;
        private ExecutionPlanRecord _selectedRecord;

        public ExecutionPlanRepositoryWindow()
        {
            InitializeComponent();
            _repositoryService = new ExecutionPlanRepositoryService();
            _allRecords = new List<ExecutionPlanRecord>();
            _filteredRecords = new List<ExecutionPlanRecord>();

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // 트리뷰 로드
                var locationTree = await _repositoryService.GetExecutionLocationTreeAsync();
                ExecutionLocationTreeView.ItemsSource = locationTree;

                // 전체 레코드 로드
                _allRecords = await _repositoryService.GetAllRecordsAsync();
                _filteredRecords = new List<ExecutionPlanRecord>(_allRecords);
                RecordsDataGrid.ItemsSource = _filteredRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 중 오류가 발생했습니다: {ex.Message}", "오류",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformSearch();
        }

        private async void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await PerformSearch();
            }
        }

        private async System.Threading.Tasks.Task PerformSearch()
        {
            try
            {
                string searchText = SearchTextBox.Text.Trim();

                if (string.IsNullOrEmpty(searchText))
                {
                    _filteredRecords = new List<ExecutionPlanRecord>(_allRecords);
                }
                else
                {
                    _filteredRecords = await _repositoryService.SearchRecordsAsync(searchText);
                }

                RecordsDataGrid.ItemsSource = _filteredRecords;

                // 트리뷰에서 해당하는 위치들을 확장
                ExpandMatchingTreeNodes(searchText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"검색 중 오류가 발생했습니다: {ex.Message}", "오류",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExpandMatchingTreeNodes(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return;

            foreach (var item in ExecutionLocationTreeView.Items)
            {
                if (item is ExecutionLocationNode node)
                {
                    ExpandNodeIfMatches(node, searchText);
                }
            }
        }

        private bool ExpandNodeIfMatches(ExecutionLocationNode node, string searchText)
        {
            bool hasMatch = false;

            // 자식 노드들 먼저 확인
            foreach (var child in node.Children)
            {
                if (ExpandNodeIfMatches(child, searchText))
                {
                    hasMatch = true;
                }
            }

            // 현재 노드에 매칭되는 레코드가 있는지 확인
            if (node.Records.Any(r => r.SqlId.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                     r.ExecutionLocation.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                     r.Query.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                     r.Notes.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
            {
                hasMatch = true;
            }

            if (hasMatch)
            {
                node.IsExpanded = true;
            }

            return hasMatch;
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Clear();
            _filteredRecords = new List<ExecutionPlanRecord>(_allRecords);
            RecordsDataGrid.ItemsSource = _filteredRecords;
        }

        private void ExecutionLocationTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ExecutionLocationNode selectedNode)
            {
                // 선택된 노드의 레코드들만 표시
                _filteredRecords = new List<ExecutionPlanRecord>(selectedNode.Records);
                RecordsDataGrid.ItemsSource = _filteredRecords;
            }
        }

        private void RecordsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordsDataGrid.SelectedItem is ExecutionPlanRecord record)
            {
                _selectedRecord = record;
                DisplayRecordDetails(record);
            }
        }

        private void DisplayRecordDetails(ExecutionPlanRecord record)
        {
            QueryDetailTextBox.Text = record.Query;
            BindVariablesDetailTextBox.Text = record.BindVariables;
            ExecutionPlanDetailTextBox.Text = record.ExecutionPlan;
            AnalysisInfoDetailTextBox.Text = record.AnalysisInfo;
            NotesDetailTextBox.Text = record.Notes;
        }

        private async void NotesDetailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_selectedRecord != null)
            {
                _selectedRecord.Notes = NotesDetailTextBox.Text;
                try
                {
                    await _repositoryService.SaveRecordAsync(_selectedRecord);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"메모 저장 중 오류가 발생했습니다: {ex.Message}", "오류",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void RecordsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RecordsDataGrid.SelectedItem is ExecutionPlanRecord record)
            {
                try
                {
                    // 접근 시간 업데이트를 위해 다시 로드
                    var updatedRecord = await _repositoryService.GetRecordByIdAsync(record.Id);
                    if (updatedRecord != null)
                    {
                        DisplayRecordDetails(updatedRecord);

                        // 리스트의 해당 항목도 업데이트
                        var index = _filteredRecords.FindIndex(r => r.Id == record.Id);
                        if (index >= 0)
                        {
                            _filteredRecords[index] = updatedRecord;
                            RecordsDataGrid.Items.Refresh();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"레코드 로드 중 오류가 발생했습니다: {ex.Message}", "오류",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void ExportSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRecords = RecordsDataGrid.SelectedItems.Cast<ExecutionPlanRecord>().ToList();

            if (selectedRecords.Count == 0)
            {
                MessageBox.Show("내보낼 항목을 선택해주세요.", "알림",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var folderDialog = new OpenFileDialog
                {
                    ValidateNames = false,
                    CheckFileExists = false,
                    CheckPathExists = true,
                    FileName = "폴더 선택"
                };

                if (folderDialog.ShowDialog() == true)
                {
                    var directoryPath = Path.GetDirectoryName(folderDialog.FileName);
                    var exportedFiles = await _repositoryService.ExportMultipleToTextFileAsync(selectedRecords, directoryPath);

                    MessageBox.Show($"{exportedFiles.Count}개 파일이 내보내기되었습니다.\n{directoryPath}", "내보내기 완료",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"내보내기 중 오류가 발생했습니다: {ex.Message}", "오류",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRecords = RecordsDataGrid.SelectedItems.Cast<ExecutionPlanRecord>().ToList();

            if (selectedRecords.Count == 0)
            {
                MessageBox.Show("삭제할 항목을 선택해주세요.", "알림",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"선택된 {selectedRecords.Count}개 항목을 삭제하시겠습니까?", "삭제 확인",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int deletedCount = 0;
                    foreach (var record in selectedRecords)
                    {
                        if (await _repositoryService.DeleteRecordAsync(record.Id))
                        {
                            deletedCount++;
                        }
                    }

                    MessageBox.Show($"{deletedCount}개 항목이 삭제되었습니다.", "삭제 완료",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                    // 데이터 새로고침
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"삭제 중 오류가 발생했습니다: {ex.Message}", "오류",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var importDialog = new ExecutionPlanImportDialog();
                importDialog.Owner = this;
                
                if (importDialog.ShowDialog() == true)
                {
                    var importedRecord = importDialog.ImportedRecord;
                    
                    // 저장 다이얼로그 표시
                    var saveDialog = new ExecutionPlanSaveDialog(importedRecord);
                    saveDialog.Owner = this;
                    
                    if (saveDialog.ShowDialog() == true)
                    {
                        MessageBox.Show("실행계획이 성공적으로 가져오기 및 저장되었습니다.", "가져오기 완료",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // 데이터 새로고침
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"가져오기 중 오류가 발생했습니다: {ex.Message}", "가져오기 오류",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
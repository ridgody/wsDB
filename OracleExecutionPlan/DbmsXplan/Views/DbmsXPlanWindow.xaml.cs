
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Oracle.ManagedDataAccess.Client;
using wsDB.OracleExecutionPlan.Core.Text;
using wsDB.OracleExecutionPlan.Core.Parsing;
using wsDB.OracleExecutionPlan.Core.Formatting;
using wsDB.OracleExecutionPlan.UI.Highlighting;
using wsDB.OracleExecutionPlan.UI.Popups;
using wsDB.OracleExecutionPlan.UI.TabManagement;
using wsDB.OracleExecutionPlan.UI.EventHandlers;
using wsDB.OracleExecutionPlan.Helpers;
using wsDB.OracleExecutionPlan.DbmsXplan.Views;
using MouseEventHandler = wsDB.OracleExecutionPlan.UI.EventHandlers.MouseEventHandler;
using wsDB.OracleExecutionPlan.DbmsXplan.Services;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Services;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Views;


namespace wsDB.OracleExecutionPlan.DbmsXplan.Views
{
    public partial class DbmsXPlanWindow : Window
    {
        private OracleConnection dbConnection;
        private Dictionary<string, ObjectStatisticsWindow> openStatWindows;

         private ObjectStatisticsManager objectStatisticsManager;

        // 기능별 매니저들
        private TabManager tabManager;
        private TextHighlightManager highlightManager;
        private PredicatePopupManager popupManager;
        private MouseEventHandler mouseHandler;
        private DbmsXPlanFormatter formatter;
        private TextProcessor textProcessor;
        private DbmsXPlanParser planParser;

         private ExecutionPlanPerformanceAnalyzer performanceAnalyzer;

        public DbmsXPlanWindow(OracleConnection connection)
        {
            InitializeComponent();
            dbConnection = connection;
            openStatWindows = new Dictionary<string, ObjectStatisticsWindow>();

            InitializeManagers();
            SetupEventHandlers();
        }


        public DbmsXPlanWindow(OracleConnection connection, string plan) : this(connection)
        {
            LoadAndFormatExecutionPlan(plan);
        }

        private void InitializeManagers()
        {
            planParser = new DbmsXPlanParser();
            textProcessor = new TextProcessor();
            highlightManager = new TextHighlightManager();
            popupManager = new PredicatePopupManager(planParser);
            tabManager = new TabManager(PlanTabControl);
            mouseHandler = new MouseEventHandler(highlightManager, popupManager, textProcessor);
            formatter = new DbmsXPlanFormatter(planParser);


            objectStatisticsManager = new ObjectStatisticsManager(
            this,
            dbConnection,
            textProcessor,
            UpdateSelectedObjectDisplay
            );

            performanceAnalyzer = new ExecutionPlanPerformanceAnalyzer(planParser);

            // 첫 번째 탭의 RichTextBox 설정
            SetupRichTextBox(PlanRichTextBox);
        }

        private void SetupEventHandlers()
        {
            // 기본 이벤트 핸들러들을 마우스 핸들러로 위임
            PlanRichTextBox.MouseMove += mouseHandler.HandleMouseMove;
            PlanRichTextBox.PreviewMouseLeftButtonDown += mouseHandler.HandleMouseLeftButtonDown;
            PlanRichTextBox.KeyDown += PlanRichTextBox_KeyDown;
            PlanRichTextBox.KeyUp += PlanRichTextBox_KeyUp;
            PlanRichTextBox.PreviewKeyDown += PlanRichTextBox_PreviewKeyDown;
        }

        private void SetupRichTextBox(RichTextBox rtb)
        {
            rtb.MouseMove += mouseHandler.HandleMouseMove;
            rtb.PreviewMouseLeftButtonDown += mouseHandler.HandleMouseLeftButtonDown;
            rtb.KeyDown += PlanRichTextBox_KeyDown;
            rtb.KeyUp += PlanRichTextBox_KeyUp;
            rtb.PreviewKeyDown += PlanRichTextBox_PreviewKeyDown;
        }

        #region 버튼 이벤트
        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileDialogHelper.ShowOpenFileDialog(
                "실행계획 파일 선택",
                "files (*.txt, *.sql, *.log)|*.txt;*.sql;*.log|All files (*.*)|*.*");

            if (filePath != null)
            {
                try
                {
                    string content = TextFileLoader.LoadTextFile(filePath);
                    LoadAndFormatExecutionPlan(content);
                    MessageBox.Show("파일이 성공적으로 로드되었습니다.", "알림");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일 로드 중 오류가 발생했습니다: {ex.Message}", "오류");
                }
            }
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string clipboardText = ClipboardHelper.GetTextSafely();
                if (!string.IsNullOrEmpty(clipboardText))
                {
                    LoadAndFormatExecutionPlan(clipboardText);
                    MessageBox.Show("클립보드 내용이 붙여넣기되었습니다.", "알림");
                }
                else
                {
                    MessageBox.Show("클립보드에 텍스트가 없습니다.", "알림");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"붙여넣기 중 오류가 발생했습니다: {ex.Message}", "오류");
            }
        }

        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RichTextBox rtb = tabManager.GetCurrentRichTextBox();
                string text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;

                if (string.IsNullOrWhiteSpace(text))
                {
                    MessageBox.Show("포맷팅할 내용이 없습니다.", "알림",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                LoadAndFormatExecutionPlan(text);
                MessageBox.Show("실행계획 포맷팅이 완료되었습니다.", "알림",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"포맷팅 중 오류가 발생했습니다: {ex.Message}", "오류",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            tabManager.GetCurrentRichTextBox().Document.Blocks.Clear();
            SelectedObjectText.Text = "없음";
        }

        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            tabManager.AddNewTab(SetupRichTextBox);
        }

        // 성능 분석 버튼 이벤트
        private void AnalyzePerformanceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RichTextBox rtb = tabManager.GetCurrentRichTextBox();
                string executionPlan = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;

                if (string.IsNullOrWhiteSpace(executionPlan))
                {
                    MessageBox.Show("분석할 실행계획이 없습니다.", "알림", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 성능 분석 실행
                var analysisResult = performanceAnalyzer.AnalyzeExecutionPlan(executionPlan);
                
                // 결과 창 표시
                ShowPerformanceAnalysisResult(analysisResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"성능 분석 중 오류가 발생했습니다: {ex.Message}", "오류", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPerformanceAnalysisResult(PerformanceAnalysisResult result)
        {
            var resultWindow = new PerformanceAnalysisResultWindow(result);
            resultWindow.Owner = this;
            resultWindow.Show();
        }
        #endregion

        #region 키보드 이벤트
        private void PlanRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyboardShortcutHandler.HandleKeyDown(e, out bool isCtrlPressed))
            {
                if (isCtrlPressed)
                {
                    mouseHandler.SetCtrlPressed(true);
                    var rtb = sender as RichTextBox;
                    rtb.Cursor = Cursors.Hand;
                }

            }

        }
        
        private void PlanRichTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (KeyboardShortcutHandler.HandleKeyUp(e, out bool isCtrlReleased))
            {
                if (isCtrlReleased)
                {
                    mouseHandler.SetCtrlPressed(false);
                    var rtb = sender as RichTextBox;
                    rtb.Cursor = Cursors.IBeam;
                }
            }
        }

        private void PlanRichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;

            // Ctrl+C 처리
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (!rtb.Selection.IsEmpty)
                {
                    string selectedText = rtb.Selection.Text;
                    Clipboard.SetText(selectedText);
                    e.Handled = true;
                }
            }

            // Ctrl+V로 붙여넣기 시 자동 포맷팅
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = ClipboardHelper.GetTextSafely();

                    if (!string.IsNullOrEmpty(clipboardText))
                    {
                        LoadAndFormatExecutionPlan(clipboardText);
                    }
                    e.Handled = true;
                }
            }

            bool isAltPressed = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
            bool isEnterPressed = e.Key == Key.Enter || e.SystemKey == Key.Enter;
            
            if (isEnterPressed && isAltPressed)            
            {
                if (WordSelectionHandler.HandleCaretWordSelection(rtb))
                {
                    objectStatisticsManager.HandleObjectStatistics(rtb);
                    // HandleObjectStatistics(rtb);
                }
                e.Handled = true;
            }
            // Alt+클릭으로 객체 통계 보기
            //if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Alt) 
            // {
            //     HandleObjectStatistics(rtb);
            //     e.Handled = true;
            // }

        }
        #endregion
 
        private void UpdateSelectedObjectDisplay(string objectName)
        {
            SelectedObjectText.Text = objectName;
        }

        private void LoadAndFormatExecutionPlan(string content)
        { 
            RichTextBox rtb = tabManager.GetCurrentRichTextBox(); 
            formatter.FormatExecutionPlan(rtb, content);
        }


        private string DetermineObjectType(string objectName)
        {
            // 간단한 휴리스틱으로 객체 타입 결정
            // 실제로는 더 정교한 로직이 필요할 수 있음
            if (objectName.Contains("PK_") || objectName.Contains("IDX_") || objectName.Contains("IX_"))
                return "INDEX";
            else
                return "TABLE";
        }
 
        #region 윈도우 생명주기
        protected override void OnClosed(EventArgs e)
        {
            // 열린 모든 통계 창 닫기
            foreach (var window in openStatWindows.Values)
            {
                window.Close();
            }
            openStatWindows.Clear();

            // 팝업 정리
            popupManager.HidePopup();

            base.OnClosed(e);
        }
        #endregion
    }
}

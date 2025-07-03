// MainWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using wsDB.OracleConnectionManager.Services;
using wsDB.OracleConnectionManager.Views;
using wsDB.OracleExecutionPlan.DbmsXplan.Views; 
using wsDB.OracleQueryAnalyzer.Views;

namespace wsDB
{
    public partial class MainWindow : Window
    {
        private readonly OracleConnectionService _connectionService;
        private int _tabCounter = 1;

        public MainWindow()
        {
            InitializeComponent();
            _connectionService = OracleConnectionService.Instance;
            _connectionService.PropertyChanged += ConnectionService_PropertyChanged;
            
            UpdateConnectionStatus();
        }

        private void ConnectionService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OracleConnectionService.ConnectionInfo))
            {
                UpdateConnectionStatus();
            }
        }

        private void UpdateConnectionStatus()
        {
            Dispatcher.Invoke(() =>
            {
                ConnectionStatusText.Text = _connectionService.ConnectionInfo ?? "연결 안됨";
            });
        }

        #region 연결 Menu 이벤트
        private void NewConnectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var connectionWindow = ConnectionWindow.Instance;
            connectionWindow.Owner = this;
            connectionWindow.ShowDialog();
        }

        private void DisconnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _connectionService.Disconnect();
        }

        #endregion

        #region File Menu 이벤트

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region about Menu 이벤트
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Oracle Connection Manager v1.0\n\nOracle 데이터베이스 연결 및 쿼리 실행 도구",
                           "정보", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region 분석 Menu 이벤트
        
        private void QueryAnalysisMenu_Click(object sender, RoutedEventArgs e)
        {
            var dbConnection = _connectionService.Connection;
            if (dbConnection == null || dbConnection.State != System.Data.ConnectionState.Open)
            {
                MessageBox.Show("먼저 데이터베이스에 연결해주세요.", "연결 필요",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {    
                var queryAnalysisWindow = new QueryAnalysisWindow(_connectionService.Connection);

                queryAnalysisWindow.Owner = this;
                queryAnalysisWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"쿼리 분석 창을 열 수 없습니다: {ex.Message}", "오류",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecutionPlanAnalyzerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dbConnection = _connectionService.Connection;
            if (dbConnection == null || dbConnection.State != System.Data.ConnectionState.Open)
            {
                MessageBox.Show("먼저 데이터베이스에 연결해주세요.", "연결 필요",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                DbmsXPlanWindow analyzer = new DbmsXPlanWindow(dbConnection);
                analyzer.Owner = this;
                analyzer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"실행계획 분석기 실행 중 오류가 발생했습니다: {ex.Message}", "오류",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        private void AddNewQueryTab()
        {
            _tabCounter++;
            var tabItem = new TabItem
            {
                Header = $"Query {_tabCounter}"
            };

            var richTextBox = new RichTextBox
            {
                AcceptsReturn = true,
                AcceptsTab = true,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12
            };

            tabItem.Content = richTextBox;
            QueryTabControl.Items.Add(tabItem);
            QueryTabControl.SelectedItem = tabItem;
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.T && 
                (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                AddNewQueryTab();
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }
}

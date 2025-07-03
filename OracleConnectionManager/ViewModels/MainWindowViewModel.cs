// ViewModels/MainWindowViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using wsDB.OracleConnectionManager.Commands;
using wsDB.OracleConnectionManager.Models;
using wsDB.OracleConnectionManager.Services;
using wsDB.Common;

namespace wsDB.OracleConnectionManager.ViewModels
{
    // public class MainWindowViewModel : INotifyPropertyChanged
    public class MainWindowViewModel : ObservableObject
    {
        private readonly OracleConnectionService _connectionService;
        private ObservableCollection<QueryTab> _queryTabs;
        private QueryTab _selectedTab;
        private int _tabCounter = 0;

        public ObservableCollection<QueryTab> QueryTabs
        {
            get => _queryTabs;
            set => SetProperty(ref _queryTabs, value);
        }

        public QueryTab SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public OracleConnectionService ConnectionService => _connectionService;

        public ICommand NewTabCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand ExecuteQueryCommand { get; }

        public MainWindowViewModel()
        {
            _connectionService = OracleConnectionService.Instance;
            _queryTabs = new ObservableCollection<QueryTab>();
            
            NewTabCommand = new RelayCommand(NewTab);
            CloseTabCommand = new RelayCommand(CloseTab, CanCloseTab);
            ExecuteQueryCommand = new RelayCommand(ExecuteQuery, CanExecuteQuery);
            
            // 초기 탭 생성
            NewTab(null);
        }

        private void NewTab(object parameter)
        {
            _tabCounter++;
            var newTab = new QueryTab
            {
                Header = $"Query {_tabCounter}",
                Content = ""
            };
            
            QueryTabs.Add(newTab);
            SelectedTab = newTab;
        }

        private void CloseTab(object parameter)
        {
            if (parameter is QueryTab tab && QueryTabs.Contains(tab))
            {
                QueryTabs.Remove(tab);
                
                if (QueryTabs.Count == 0)
                {
                    NewTab(null);
                }
                else if (SelectedTab == tab)
                {
                    SelectedTab = QueryTabs[0];
                }
            }
        }

        private bool CanCloseTab(object parameter)
        {
            return QueryTabs.Count > 1;
        }

        private void ExecuteQuery(object parameter)
        {
            if (SelectedTab != null && !string.IsNullOrWhiteSpace(SelectedTab.Content))
            {
                // TODO: 실제 쿼리 실행 로직 구현
                System.Windows.MessageBox.Show($"쿼리 실행: {SelectedTab.Content}", "쿼리 실행");
            }
        }

        private bool CanExecuteQuery(object parameter)
        {
            return _connectionService.IsConnected && 
                   SelectedTab != null && 
                   !string.IsNullOrWhiteSpace(SelectedTab.Content);
        }

        // public event PropertyChangedEventHandler PropertyChanged;

        // protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        // {
        //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // }

        // protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        // {
        //     if (Equals(field, value)) return false;
        //     field = value;
        //     OnPropertyChanged(propertyName);
        //     return true;
        // }
    }
}

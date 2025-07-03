// ViewModels/ConnectionWindowViewModel.cs
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
    // public class ConnectionWindowViewModel : INotifyPropertyChanged
    public class ConnectionWindowViewModel : ObservableObject
    {
         private readonly EncryptedConnectionProfileManager _profileManager; // 변경된 부분
        // private readonly ConnectionProfileManager _profileManager;
        private readonly TnsNamesReader _tnsReader;
        private readonly OracleConnectionService _connectionService;
        
        private IConnectionProfile _selectedProfile;
        private string _selectedTnsAlias;
        private ObservableCollection<string> _tnsAliases;

        public ObservableCollection<IConnectionProfile> Profiles => _profileManager.Profiles;
        
        public ObservableCollection<string> TnsAliases
        {
            get => _tnsAliases;
            set => SetProperty(ref _tnsAliases, value);
        }

        public IConnectionProfile SelectedProfile
        {
            get => _selectedProfile;
            set => SetProperty(ref _selectedProfile, value);
        }

        public string SelectedTnsAlias
        {
            get => _selectedTnsAlias;
            set
            {
                SetProperty(ref _selectedTnsAlias, value);
                if (_selectedProfile is TnsConnectionProfile tnsProfile)
                {
                    tnsProfile.TnsAlias = value;
                }
            }
        }

        public ICommand ConnectCommand { get; }
        public ICommand CancelCommand { get; }

        public ConnectionWindowViewModel()
        {
            // _profileManager = new ConnectionProfileManager();
            _profileManager = new EncryptedConnectionProfileManager(); // 암호화 매니저 사용
            _profileManager.LoadProfiles();
            _tnsReader = new TnsNamesReader();
            _connectionService = OracleConnectionService.Instance;

            ConnectCommand = new RelayCommand(Connect, CanConnect);
            CancelCommand = new RelayCommand(Cancel);

            LoadTnsAliases();
        }

        private void LoadTnsAliases()
        {
            TnsAliases = new ObservableCollection<string>(_tnsReader.GetTnsAliases());
        }

        private void Connect(object parameter)
        {
            if (_selectedProfile != null)
            {
                var success = _connectionService.Connect(_selectedProfile);
                if (success && parameter is System.Windows.Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }

        private bool CanConnect(object parameter)
        {
            return _selectedProfile?.IsValid() == true;
        }

        private void Cancel(object parameter)
        {
            if (parameter is System.Windows.Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        // public ConnectionProfileManager? GetProfileManager()
        // {
        //     return _profileManager;
        // }
        
        public EncryptedConnectionProfileManager GetProfileManager() // 반환 타입 변경
        {
            return _profileManager;
        }

        public void RefreshProfiles()
        {
            // 프로파일 목록 다시 로드
            _profileManager.LoadProfiles();

            // TNS 별칭 목록도 다시 로드 (tnsnames.ora 파일이 변경되었을 수 있음)
            LoadTnsAliases();

            // UI 갱신을 위해 PropertyChanged 이벤트 발생
            // OnPropertyChanged(nameof(Profiles));
            // OnPropertyChanged(nameof(TnsAliases));
        }
    }
}
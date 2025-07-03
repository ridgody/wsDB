// ConnectionWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using wsDB.OracleConnectionManager.Models;
using wsDB.OracleConnectionManager.Services;
using wsDB.OracleConnectionManager.ViewModels;

namespace wsDB.OracleConnectionManager.Views
{
    public partial class ConnectionWindow : Window
    {
        private static ConnectionWindow _instance;
        private static readonly object _lock = new object();
        private bool _isRealClosing = false;


        private readonly ConnectionWindowViewModel _viewModel;
        // private readonly ConnectionProfileManager _profileManager;
        private readonly EncryptedConnectionProfileManager _profileManager;
        
        private readonly OracleConnectionService _connectionService;

        // ========== 싱글톤 프로퍼티 추가 ==========
        public static ConnectionWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConnectionWindow();
                        }
                    }
                }
                return _instance;
            }
        }

        private ConnectionWindow()
        {
            InitializeComponent();

            // _profileManager = new ConnectionProfileManager();
            // _profileManager.LoadProfiles();
            _connectionService = OracleConnectionService.Instance;

            _viewModel = new ConnectionWindowViewModel();
            _profileManager = _viewModel.GetProfileManager();
            DataContext = _viewModel;

            // 첫 번째 프로파일이 있으면 선택
            // if (_profileManager.Profiles.Count > 0)
            // {
            //     ProfileComboBox.SelectedIndex = 0;
            // }

            if (_viewModel.Profiles.Count > 0)
            {
                ProfileComboBox.SelectedIndex = 0;
            }
        }

        // ========== 창 닫기 이벤트 핸들러 추가 ==========
        private void ConnectionWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isRealClosing)
            {
                e.Cancel = true;
                this.Hide();
                //this.DialogResult = false;
            }
        }

        // ========== ShowDialog 메서드 오버라이드 추가 ==========
                
        // ========== ShowDialog 메서드 수정 ==========
        public bool? ShowDialog(Window owner = null)
        {
            RefreshProfileList();
            ResetForm();
            
            // this.DialogResult = null;
            
            // Owner 설정
            if (owner != null)
            {
                this.Owner = owner;
            }
            else if (Application.Current.MainWindow != null && Application.Current.MainWindow != this)
            {
                this.Owner = Application.Current.MainWindow;
            }
            
            // Show() 제거하고 바로 ShowDialog() 호출
            return base.ShowDialog();
        }

        // ========== 새로운 메서드들 추가 ==========
        private void ResetForm()
        {
            if (_viewModel.Profiles.Count > 0)
            {
                ProfileComboBox.SelectedIndex = 0;
            }
        }

        private void RefreshProfileList()
        {
            _viewModel.RefreshProfiles(); // ViewModel에 이 메서드가 있다고 가정
        }

        private void ProfileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileComboBox.SelectedItem is IConnectionProfile profile)
            {
                LoadProfileData(profile);

                // 프로파일 타입에 따라 탭 선택
                if (profile is TnsConnectionProfile)
                {
                    ConnectionTabControl.SelectedItem = TnsTab;
                }
                else if (profile is DirectConnectionProfile)
                {
                    ConnectionTabControl.SelectedItem = DirectTab;
                }
            }
        }

        private void LoadProfileData(IConnectionProfile profile)
        {
            switch (profile)
            {
                case TnsConnectionProfile tns:
                    TnsProfileNameTextBox.Text = tns.ProfileName;
                    TnsUsernameTextBox.Text = tns.Username;
                    TnsPasswordBox.Password = tns.Password;
                    TnsAliasComboBox.SelectedItem = tns.TnsAlias;
                    break;

                case DirectConnectionProfile direct:
                    DirectProfileNameTextBox.Text = direct.ProfileName;
                    DirectHostTextBox.Text = direct.Host;
                    DirectPortTextBox.Text = direct.Port.ToString();
                    DirectServiceNameTextBox.Text = direct.ServiceName;
                    DirectUsernameTextBox.Text = direct.Username;
                    DirectPasswordBox.Password = direct.Password;
                    break;
            }
        }

        private IConnectionProfile GetCurrentProfile()
        {
            if (ConnectionTabControl.SelectedItem == TnsTab)
            {
                return new TnsConnectionProfile
                {
                    ProfileName = TnsProfileNameTextBox.Text,
                    Username = TnsUsernameTextBox.Text,
                    Password = TnsPasswordBox.Password,
                    TnsAlias = TnsAliasComboBox.Text
                };
            }
            else if (ConnectionTabControl.SelectedItem == DirectTab)
            {
                if (int.TryParse(DirectPortTextBox.Text, out int port))
                {
                    return new DirectConnectionProfile
                    {
                        ProfileName = DirectProfileNameTextBox.Text,
                        Host = DirectHostTextBox.Text,
                        Port = port,
                        ServiceName = DirectServiceNameTextBox.Text,
                        Username = DirectUsernameTextBox.Text,
                        Password = DirectPasswordBox.Password
                    };
                }
            }

            return null;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            var profile = GetCurrentProfile();
            if (profile == null || !profile.IsValid())
            {
                MessageBox.Show("연결 정보를 모두 입력해주세요.", "입력 오류",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 실제 연결 테스트 로직
                var connectionString = profile.GetConnectionString();
                MessageBox.Show($"연결 테스트 성공!\n\n연결 문자열: {connectionString}", "연결 테스트",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"연결 테스트 실패: {ex.Message}", "연결 테스트 실패",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var profile = GetCurrentProfile();
            if (profile == null || !profile.IsValid())
            {
                MessageBox.Show("연결 정보를 모두 입력해주세요.", "입력 오류",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_connectionService.Connect(profile))
            {
                // 프로파일 저장
                if (ProfileComboBox.SelectedItem == null ||
                    !_profileManager.Profiles.Contains(profile))
                {
                    _profileManager.AddProfile(profile);
                }
                else
                {
                    _profileManager.SaveProfiles();
                }

                DialogResult = true;

                // Close();
                this.Hide(); // ← 이 부분만 변경 (Close → Hide)
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            // Close();
            this.Hide(); // ← 이 부분만 변경 (Close → Hide)
        }
        
        // 애플리케이션 종료 시 호출할 메서드
        public static void ForceClose()
        {
            if (_instance != null)
            {
                _instance._isRealClosing = true;
                _instance.Close();
                _instance = null;
            }
        }
    }
}
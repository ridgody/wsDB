// 향상된 OracleConnectionService.cs (실제 Oracle 연결 포함)
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Oracle.ManagedDataAccess.Client;
using wsDB.OracleConnectionManager.Models;
using wsDB.Common;

namespace wsDB.OracleConnectionManager.Services
{
    // public class OracleConnectionService : INotifyPropertyChanged
    public class OracleConnectionService : ObservableObject
    {
        private static OracleConnectionService _instance;
        private bool _isConnected;
        private string _connectionInfo;
        private IConnectionProfile _currentProfile;
        private OracleConnection _connection;

        public static OracleConnectionService Instance => _instance ??= new OracleConnectionService();

        public bool IsConnected
        {
            get => _isConnected;
            private set => SetProperty(ref _isConnected, value);
        }

        public string ConnectionInfo
        {
            get => _connectionInfo;
            private set => SetProperty(ref _connectionInfo, value);
        }

        public IConnectionProfile CurrentProfile
        {
            get => _currentProfile;
            private set => SetProperty(ref _currentProfile, value);
        }

        public OracleConnection Connection => _connection;

        private OracleConnectionService() 
        {
            ConnectionInfo = "연결 안됨";
        }

        public bool Connect(IConnectionProfile profile)
        {
            try
            {
                if (profile == null || !profile.IsValid())
                {
                    throw new ArgumentException("유효하지 않은 연결 프로파일입니다.");
                }

                // 기존 연결이 있으면 해제
                Disconnect();

                var connectionString = profile.GetConnectionString();
                _connection = new OracleConnection(connectionString);

                _connection.Open();

                // 연결 테스트 쿼리
                using var command = new OracleCommand("SELECT 1 FROM DUAL", _connection);
                command.ExecuteScalar();

                CurrentProfile = profile;
                IsConnected = true;
                ConnectionInfo = $"{profile.ProfileName} ({profile.Username})";

                // 테스트
                // _connection = new OracleConnection();

                return true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                ConnectionInfo = "연결 실패";
                _connection?.Dispose();
                _connection = null;
                
                System.Windows.MessageBox.Show($"연결 오류: {ex.Message}", "연결 실패", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                _connection?.Close();
                _connection?.Dispose();
                _connection = null;
                
                IsConnected = false;
                ConnectionInfo = "연결 안됨";
                CurrentProfile = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"연결 해제 오류: {ex.Message}");
            }
        }

        public bool TestConnection(IConnectionProfile profile)
        {
            try
            {
                if (profile == null || !profile.IsValid())
                {
                    return false;
                }

                var connectionString = profile.GetConnectionString();
                using var testConnection = new OracleConnection(connectionString);
                testConnection.Open();
                
                using var command = new OracleCommand("SELECT 1 FROM DUAL", testConnection);
                command.ExecuteScalar();
                
                return true;
            }
            catch
            {
                return false;
            }
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
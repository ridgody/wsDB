// Models/BaseConnectionProfile.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wsDB.Common;

namespace wsDB.OracleConnectionManager.Models
{
    public abstract class BaseConnectionProfile : ObservableObject, IConnectionProfile
    {
        private string _profileName;
        private string _username;
        private string _password;
        private ConnectionType _type;

        public string ProfileName
        {
            get => _profileName;
            set => SetProperty(ref _profileName, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ConnectionType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public abstract bool IsValid();
        public abstract string GetConnectionString();

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

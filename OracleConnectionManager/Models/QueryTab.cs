
// Models/QueryTab.cs - 쿼리 탭 모델
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wsDB.Common;

namespace wsDB.OracleConnectionManager.Models
{
    // public class QueryTab : INotifyPropertyChanged
    public class QueryTab : ObservableObject
    {
        private string _header;
        private string _content;
        private bool _isModified;

        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        public string Content
        {
            get => _content;
            set
            {
                if (SetProperty(ref _content, value))
                {
                    IsModified = true;
                }
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set => SetProperty(ref _isModified, value);
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
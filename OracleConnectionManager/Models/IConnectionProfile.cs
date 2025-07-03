// Models/IConnectionProfile.cs
using System.ComponentModel;
using wsDB.Common;

namespace wsDB.OracleConnectionManager.Models
{
    public interface IConnectionProfile //: INotifyPropertyChanged
    {
        string ProfileName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        ConnectionType Type { get; set; }
        bool IsValid();
        string GetConnectionString();
    }
}
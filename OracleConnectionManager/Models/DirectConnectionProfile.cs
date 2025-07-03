// Models/DirectConnectionProfile.cs
namespace wsDB.OracleConnectionManager.Models
{
    public class DirectConnectionProfile : BaseConnectionProfile
    {
        private string _host;
        private int _port = 1521;
        private string _serviceName;

        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string ServiceName
        {
            get => _serviceName;
            set => SetProperty(ref _serviceName, value);
        }

        public DirectConnectionProfile()
        {
            Type = ConnectionType.Direct;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ProfileName) &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(Host) &&
                   !string.IsNullOrWhiteSpace(ServiceName) &&
                   Port > 0;
        }

        public override string GetConnectionString()
        {
            return $"Data Source={Host}:{Port}/{ServiceName};User Id={Username};Password={Password};";
        }
    }
}

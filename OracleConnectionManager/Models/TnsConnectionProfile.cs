// Models/TnsConnectionProfile.cs
namespace wsDB.OracleConnectionManager.Models
{
    public class TnsConnectionProfile : BaseConnectionProfile
    {
        private string _tnsAlias;

        public string TnsAlias
        {
            get => _tnsAlias;
            set => SetProperty(ref _tnsAlias, value);
        }

        public TnsConnectionProfile()
        {
            Type = ConnectionType.TNS;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ProfileName) &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(TnsAlias);
        }

        public override string GetConnectionString()
        {
            return $"Data Source={TnsAlias};User Id={Username};Password={Password};";
        }
    }
}
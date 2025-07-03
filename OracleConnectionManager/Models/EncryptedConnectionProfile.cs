using System.Text.Json.Serialization;
using wsDB.OracleConnectionManager.Services;

namespace wsDB.OracleConnectionManager.Models
{
    /// <summary>
    /// 암호화된 TNS 연결 프로파일
    /// </summary>
    public class EncryptedTnsConnectionProfile
    {
        [JsonPropertyName("profileName")]
        public string ProfileName { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("encryptedPassword")]
        public string EncryptedPassword { get; set; }
        
        [JsonPropertyName("tnsAlias")]
        public string TnsAlias { get; set; }

        /// <summary>
        /// 일반 프로파일로 변환 (복호화)
        /// </summary>
        public TnsConnectionProfile ToDecryptedProfile(string decryptionKey = null)
        {
            return new TnsConnectionProfile
            {
                ProfileName = ProfileName,
                Username = Username,
                Password = PasswordEncryptionService.DecryptPassword(EncryptedPassword, decryptionKey),
                TnsAlias = TnsAlias
            };
        }

        /// <summary>
        /// 일반 프로파일에서 변환 (암호화)
        /// </summary>
        public static EncryptedTnsConnectionProfile FromProfile(TnsConnectionProfile profile, string encryptionKey = null)
        {
            return new EncryptedTnsConnectionProfile
            {
                ProfileName = profile.ProfileName,
                Username = profile.Username,
                EncryptedPassword = PasswordEncryptionService.EncryptPassword(profile.Password, encryptionKey),
                TnsAlias = profile.TnsAlias
            };
        }
    }

    /// <summary>
    /// 암호화된 Direct 연결 프로파일
    /// </summary>
    public class EncryptedDirectConnectionProfile
    {
        [JsonPropertyName("profileName")]
        public string ProfileName { get; set; }
        
        [JsonPropertyName("host")]
        public string Host { get; set; }
        
        [JsonPropertyName("port")]
        public int Port { get; set; }
        
        [JsonPropertyName("serviceName")]
        public string ServiceName { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("encryptedPassword")]
        public string EncryptedPassword { get; set; }

        /// <summary>
        /// 일반 프로파일로 변환 (복호화)
        /// </summary>
        public DirectConnectionProfile ToDecryptedProfile(string decryptionKey = null)
        {
            return new DirectConnectionProfile
            {
                ProfileName = ProfileName,
                Host = Host,
                Port = Port,
                ServiceName = ServiceName,
                Username = Username,
                Password = PasswordEncryptionService.DecryptPassword(EncryptedPassword, decryptionKey)
            };
        }

        /// <summary>
        /// 일반 프로파일에서 변환 (암호화)
        /// </summary>
        public static EncryptedDirectConnectionProfile FromProfile(DirectConnectionProfile profile, string encryptionKey = null)
        {
            return new EncryptedDirectConnectionProfile
            {
                ProfileName = profile.ProfileName,
                Host = profile.Host,
                Port = profile.Port,
                ServiceName = profile.ServiceName,
                Username = profile.Username,
                EncryptedPassword = PasswordEncryptionService.EncryptPassword(profile.Password, encryptionKey)
            };
        }
    }
}
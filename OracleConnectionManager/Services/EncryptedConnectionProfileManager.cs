using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using wsDB.OracleConnectionManager.Models;

namespace wsDB.OracleConnectionManager.Services
{
    public class EncryptedConnectionProfileManager
    {
        private const string ProfilesFileName = "connection_profiles_encrypted.json";
        private readonly string _profilesFilePath;
        private readonly string _encryptionKey;

        public ObservableCollection<IConnectionProfile> Profiles { get; private set; }

        public EncryptedConnectionProfileManager(string customEncryptionKey = null)
        {
            _profilesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProfilesFileName);
            _encryptionKey = customEncryptionKey ?? PasswordEncryptionService.GenerateMachineSpecificKey();
            Profiles = new ObservableCollection<IConnectionProfile>();
        }

        public void LoadProfiles()
        {
            try
            {
                if (!File.Exists(_profilesFilePath))
                {
                    CreateDefaultProfiles();
                    return;
                }

                var json = File.ReadAllText(_profilesFilePath);
                var profilesData = JsonSerializer.Deserialize<List<JsonElement>>(json);

                Profiles.Clear();
                foreach (var profileData in profilesData)
                {
                    var profile = DeserializeProfile(profileData);
                    if (profile != null)
                    {
                        Profiles.Add(profile);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"프로파일 로딩 오류: {ex.Message}", "오류", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                CreateDefaultProfiles();
            }
        }

        public void SaveProfiles()
        {
            try
            {
                var profilesData = Profiles.Select(SerializeProfile).ToList();
                var json = JsonSerializer.Serialize(profilesData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_profilesFilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"프로파일 저장 오류: {ex.Message}", "오류", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CreateDefaultProfiles()
        {
            Profiles.Clear();
            Profiles.Add(new TnsConnectionProfile 
            { 
                ProfileName = "Local TNS", 
                Username = "hr", 
                Password = "",  // 기본값은 빈 패스워드
                TnsAlias = "ORCL" 
            });
            Profiles.Add(new DirectConnectionProfile 
            { 
                ProfileName = "Local Direct", 
                Username = "hr", 
                Password = "",  // 기본값은 빈 패스워드
                Host = "localhost", 
                Port = 1521, 
                ServiceName = "ORCL" 
            });
            SaveProfiles();
        }

        private object SerializeProfile(IConnectionProfile profile)
        {
            return profile switch
            {
                TnsConnectionProfile tns => new
                {
                    Type = "TNS",
                    ProfileName = tns.ProfileName,
                    Username = tns.Username,
                    EncryptedPassword = EncryptPassword(tns.Password), // 패스워드 암호화
                    TnsAlias = tns.TnsAlias
                },
                DirectConnectionProfile direct => new
                {
                    Type = "Direct",
                    ProfileName = direct.ProfileName,
                    Username = direct.Username,
                    EncryptedPassword = EncryptPassword(direct.Password), // 패스워드 암호화
                    Host = direct.Host,
                    Port = direct.Port,
                    ServiceName = direct.ServiceName
                },
                _ => null
            };
        }

        private IConnectionProfile DeserializeProfile(JsonElement profileData)
        {
            try
            {
                var type = profileData.GetProperty("Type").GetString();
                
                return type switch
                {
                    "TNS" => new TnsConnectionProfile
                    {
                        ProfileName = profileData.TryGetProperty("ProfileName", out var pn) ? pn.GetString() : "",
                        Username = profileData.TryGetProperty("Username", out var un) ? un.GetString() : "",
                        Password = DecryptPassword(profileData), // 패스워드 복호화
                        TnsAlias = profileData.TryGetProperty("TnsAlias", out var ta) ? ta.GetString() : ""
                    },
                    "Direct" => new DirectConnectionProfile
                    {
                        ProfileName = profileData.TryGetProperty("ProfileName", out var pn2) ? pn2.GetString() : "",
                        Username = profileData.TryGetProperty("Username", out var un2) ? un2.GetString() : "",
                        Password = DecryptPassword(profileData), // 패스워드 복호화
                        Host = profileData.TryGetProperty("Host", out var h) ? h.GetString() : "",
                        Port = profileData.TryGetProperty("Port", out var p) ? p.GetInt32() : 1521,
                        ServiceName = profileData.TryGetProperty("ServiceName", out var sn) ? sn.GetString() : ""
                    },
                    _ => null
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"프로파일 복호화 실패: {ex.Message}");
                return null; // 복호화 실패 시 해당 프로파일 건너뛰기
            }
        }

        /// <summary>
        /// 패스워드 암호화
        /// </summary>
        private string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            try
            {
                return PasswordEncryptionService.EncryptPassword(password, _encryptionKey);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"패스워드 암호화 실패: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 패스워드 복호화 (기존 평문 패스워드도 지원)
        /// </summary>
        private string DecryptPassword(JsonElement profileData)
        {
            // 새로운 암호화된 패스워드 필드 확인
            if (profileData.TryGetProperty("EncryptedPassword", out var encryptedPw))
            {
                string encryptedPassword = encryptedPw.GetString();
                if (!string.IsNullOrEmpty(encryptedPassword))
                {
                    try
                    {
                        return PasswordEncryptionService.DecryptPassword(encryptedPassword, _encryptionKey);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"패스워드 복호화 실패: {ex.Message}");
                        // 복호화 실패 시 기존 평문 패스워드 확인
                    }
                }
            }

            // 기존 평문 패스워드 필드 확인 (하위 호환성)
            if (profileData.TryGetProperty("Password", out var plainPw))
            {
                return plainPw.GetString() ?? "";
            }

            return "";
        }

        public void AddProfile(IConnectionProfile profile)
        {
            if (profile != null && !Profiles.Any(p => p.ProfileName == profile.ProfileName))
            {
                Profiles.Add(profile);
                SaveProfiles();
            }
        }

        public void RemoveProfile(IConnectionProfile profile)
        {
            if (Profiles.Contains(profile))
            {
                Profiles.Remove(profile);
                SaveProfiles();
            }
        }

        /// <summary>
        /// 프로파일 업데이트 (기존과 동일한 이름의 프로파일 교체)
        /// </summary>
        public void UpdateProfile(IConnectionProfile profile)
        {
            if (profile == null) return;

            var existingProfile = Profiles.FirstOrDefault(p => p.ProfileName == profile.ProfileName);
            if (existingProfile != null)
            {
                var index = Profiles.IndexOf(existingProfile);
                Profiles[index] = profile;
                SaveProfiles();
            }
            else
            {
                AddProfile(profile);
            }
        }

        /// <summary>
        /// 기존 평문 프로파일을 암호화된 프로파일로 마이그레이션
        /// </summary>
        public void MigrateFromPlainTextProfiles(string plainTextProfilesPath = null)
        {
            try
            {
                string oldProfilesPath = plainTextProfilesPath ?? 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connection_profiles.json");

                if (!File.Exists(oldProfilesPath))
                    return;

                // 기존 평문 프로파일 로드
                var json = File.ReadAllText(oldProfilesPath);
                var profilesData = JsonSerializer.Deserialize<List<JsonElement>>(json);

                foreach (var profileData in profilesData)
                {
                    var profile = DeserializeOldProfile(profileData);
                    if (profile != null && !Profiles.Any(p => p.ProfileName == profile.ProfileName))
                    {
                        Profiles.Add(profile);
                    }
                }

                SaveProfiles();

                // 마이그레이션 완료 후 기존 파일을 백업으로 이름 변경
                string backupPath = oldProfilesPath + ".backup";
                if (!File.Exists(backupPath))
                {
                    File.Move(oldProfilesPath, backupPath);
                }

                System.Windows.MessageBox.Show("기존 프로파일이 암호화되어 마이그레이션되었습니다.", "마이그레이션 완료", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"프로파일 마이그레이션 오류: {ex.Message}", "오류", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 기존 평문 프로파일 형식 복호화 (마이그레이션용)
        /// </summary>
        private IConnectionProfile DeserializeOldProfile(JsonElement profileData)
        {
            var type = profileData.GetProperty("Type").GetString();
            
            return type switch
            {
                "TNS" => new TnsConnectionProfile
                {
                    ProfileName = profileData.TryGetProperty("ProfileName", out var pn) ? pn.GetString() : "",
                    Username = profileData.TryGetProperty("Username", out var un) ? un.GetString() : "",
                    Password = profileData.TryGetProperty("Password", out var pw) ? pw.GetString() : "",
                    TnsAlias = profileData.TryGetProperty("TnsAlias", out var ta) ? ta.GetString() : ""
                },
                "Direct" => new DirectConnectionProfile
                {
                    ProfileName = profileData.TryGetProperty("ProfileName", out var pn2) ? pn2.GetString() : "",
                    Username = profileData.TryGetProperty("Username", out var un2) ? un2.GetString() : "",
                    Password = profileData.TryGetProperty("Password", out var pw2) ? pw2.GetString() : "",
                    Host = profileData.TryGetProperty("Host", out var h) ? h.GetString() : "",
                    Port = profileData.TryGetProperty("Port", out var p) ? p.GetInt32() : 1521,
                    ServiceName = profileData.TryGetProperty("ServiceName", out var sn) ? sn.GetString() : ""
                },
                _ => null
            };
        }
    }
}
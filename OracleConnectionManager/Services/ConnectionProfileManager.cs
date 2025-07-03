// Services/ConnectionProfileManager.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using wsDB.OracleConnectionManager.Models;

namespace wsDB.OracleConnectionManager.Services
{
    public class ConnectionProfileManager
    {
        private const string ProfilesFileName = "connection_profiles.json";
        private readonly string _profilesFilePath;

        public ObservableCollection<IConnectionProfile> Profiles { get; private set; }

        public ConnectionProfileManager()
        {
            _profilesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProfilesFileName);
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
                TnsAlias = "ORCL" 
            });
            Profiles.Add(new DirectConnectionProfile 
            { 
                ProfileName = "Local Direct", 
                Username = "hr", 
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
                    Password = tns.Password,
                    TnsAlias = tns.TnsAlias
                },
                DirectConnectionProfile direct => new
                {
                    Type = "Direct",
                    ProfileName = direct.ProfileName,
                    Username = direct.Username,
                    Password = direct.Password,
                    Host = direct.Host,
                    Port = direct.Port,
                    ServiceName = direct.ServiceName
                },
                _ => null
            };
        }

        private IConnectionProfile DeserializeProfile(JsonElement profileData)
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
    }
}

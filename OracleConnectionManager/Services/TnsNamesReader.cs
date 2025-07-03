// Services/TnsNamesReader.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace wsDB.OracleConnectionManager.Services
{
    public class TnsNamesReader
    {
        public List<string> GetTnsAliases()
        {
            var aliases = new List<string>();
            
            try
            {
                var tnsPath = GetTnsNamesPath();
                if (string.IsNullOrEmpty(tnsPath) || !File.Exists(tnsPath))
                {
                    return aliases;
                }

                var content = File.ReadAllText(tnsPath);
                var matches = Regex.Matches(content, @"^(\w+)\s*=", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                
                aliases.AddRange(matches.Cast<Match>().Select(match => match.Groups[1].Value));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TNS 파일 읽기 오류: {ex.Message}");
            }

            return aliases.Distinct().OrderBy(x => x).ToList();
        }

        private string GetTnsNamesPath()
        {
            // TNS_ADMIN 환경변수 확인
            var tnsAdmin = Environment.GetEnvironmentVariable("TNS_ADMIN");
            if (!string.IsNullOrEmpty(tnsAdmin))
            {
                var path = Path.Combine(tnsAdmin, "tnsnames.ora");
                if (File.Exists(path)) return path;
            }

            // ORACLE_HOME 환경변수 확인
            var oracleHome = Environment.GetEnvironmentVariable("ORACLE_HOME");
            if (!string.IsNullOrEmpty(oracleHome))
            {
                var path = Path.Combine(oracleHome, "network", "admin", "tnsnames.ora");
                if (File.Exists(path)) return path;
            }

            // 기본 위치들 확인
            var defaultPaths = new[]
            {
                @"C:\app\oracle\product\19c\network\admin\tnsnames.ora",
                @"C:\app\oracle\product\18c\network\admin\tnsnames.ora",
                @"C:\oracle\network\admin\tnsnames.ora"
            };

            return defaultPaths.FirstOrDefault(File.Exists);
        }
    }
}
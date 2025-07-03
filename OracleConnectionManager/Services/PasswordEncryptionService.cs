using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace wsDB.OracleConnectionManager.Services
{
    public static class PasswordEncryptionService
    {
        // 고정된 키 (실제 운영환경에서는 더 안전한 방법 사용)
        private static readonly string DefaultKey = "wsDB_Oracle_Connection_Manager_2024";
        
        /// <summary>
        /// 패스워드를 AES 암호화합니다.
        /// </summary>
        /// <param name="plainPassword">평문 패스워드</param>
        /// <param name="customKey">사용자 지정 키 (선택사항)</param>
        /// <returns>암호화된 패스워드 (Base64)</returns>
        public static string EncryptPassword(string plainPassword, string customKey = null)
        {
            if (string.IsNullOrEmpty(plainPassword))
                return string.Empty;

            try
            {
                string key = customKey ?? DefaultKey;
                byte[] keyBytes = DeriveKeyFromPassword(key);
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.GenerateIV();
                    
                    using (var encryptor = aes.CreateEncryptor())
                    using (var msEncrypt = new MemoryStream())
                    {
                        // IV를 맨 앞에 저장
                        msEncrypt.Write(aes.IV, 0, aes.IV.Length);
                        
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainPassword);
                        }
                        
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"패스워드 암호화 실패: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 암호화된 패스워드를 복호화합니다.
        /// </summary>
        /// <param name="encryptedPassword">암호화된 패스워드 (Base64)</param>
        /// <param name="customKey">사용자 지정 키 (선택사항)</param>
        /// <returns>평문 패스워드</returns>
        public static string DecryptPassword(string encryptedPassword, string customKey = null)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
                return string.Empty;

            try
            {
                string key = customKey ?? DefaultKey;
                byte[] keyBytes = DeriveKeyFromPassword(key);
                byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    
                    // IV 추출 (처음 16바이트)
                    byte[] iv = new byte[16];
                    Array.Copy(encryptedBytes, 0, iv, 0, 16);
                    aes.IV = iv;
                    
                    // 실제 암호화된 데이터
                    byte[] cipherText = new byte[encryptedBytes.Length - 16];
                    Array.Copy(encryptedBytes, 16, cipherText, 0, cipherText.Length);
                    
                    using (var decryptor = aes.CreateDecryptor())
                    using (var msDecrypt = new MemoryStream(cipherText))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"패스워드 복호화 실패: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 패스워드에서 32바이트 키 생성
        /// </summary>
        private static byte[] DeriveKeyFromPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// 머신별 고유 키 생성 (선택사항)
        /// </summary>
        public static string GenerateMachineSpecificKey()
        {
            string machineId = Environment.MachineName + Environment.UserName;
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(machineId));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
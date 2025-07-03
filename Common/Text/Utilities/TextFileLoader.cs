// using System.Diagnostics;
// using System.IO;
// using System.Text;

// namespace wsDB.Common.Text.Utilities
// {
//     /// <summary>
//     /// 텍스트 파일 인코딩 자동 감지 및 로딩 유틸리티
//     /// </summary>
//     public static class TextFileLoader
//     {
//         private static bool _encodingProviderRegistered = false;

//         private static readonly int[] CoreSupportedCodePages = {
//             65001, 1200, 1201, 12000, 12001, 20127, 28591
//         };

//         private static readonly int[] KoreanCodePages = {
//             949, 51949, 1252, 20949, 50225
//         };

//         static TextFileLoader()
//         {
//             RegisterEncodingProvider();
//         }

//         /// <summary>
//         /// 텍스트 파일을 자동 인코딩 감지하여 로드
//         /// </summary>
//         public static string LoadTextFile(string filePath, out Encoding detectedEncoding)
//         {
//             try
//             {
//                 Debug.WriteLine($"텍스트 파일 로딩 시작: {filePath}");
//                 detectedEncoding = DetectFileEncoding(filePath);
//                 string content = File.ReadAllText(filePath, detectedEncoding);
//                 Debug.WriteLine($"파일 로딩 완료: {detectedEncoding.EncodingName}");
//                 return content;
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine($"파일 로딩 실패: {ex.Message}");
//                 detectedEncoding = Encoding.UTF8;
//                 throw new FileLoadException($"파일 로드 실패: {ex.Message}", ex);
//             }
//         }

//         public static string LoadTextFile(string filePath)
//         {
//             return LoadTextFile(filePath, out _);
//         }

//         public static Encoding DetectFileEncoding(string filePath)
//         {
//             byte[] bytes = System.IO.File.ReadAllBytes(filePath);
//             return DetectEncoding(bytes);
//         }

//         private static Encoding DetectEncoding(byte[] bytes)
//         {
//             if (bytes == null || bytes.Length == 0)
//                 return Encoding.UTF8;

//             // BOM 체크
//             Encoding bomEncoding = DetectBOM(bytes);
//             if (bomEncoding != null) return bomEncoding;

//             // UTF-8 체크
//             if (IsValidUTF8(bytes))
//                 return new UTF8Encoding(false);

//             // 한국어 인코딩 시도
//             if (_encodingProviderRegistered)
//             {
//                 foreach (int codePage in KoreanCodePages)
//                 {
//                     try
//                     {
//                         Encoding encoding = Encoding.GetEncoding(codePage);
//                         string text = encoding.GetString(bytes);
//                         if (IsGoodKoreanText(text))
//                             return encoding;
//                     }
//                     catch { continue; }
//                 }
//             }

//             return Encoding.UTF8;
//         }

//         private static void RegisterEncodingProvider()
//         {
//             if (_encodingProviderRegistered) return;
//             try
//             {
//                 Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//                 _encodingProviderRegistered = true;
//             }
//             catch { }
//         }

//         private static Encoding DetectBOM(byte[] bytes)
//         {
//             if (bytes.Length < 2) return null;

//             if (bytes.Length >= 4 && bytes[0] == 0xFF && bytes[1] == 0xFE &&
//                 bytes[2] == 0x00 && bytes[3] == 0x00)
//                 return Encoding.UTF32;

//             if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
//                 return Encoding.UTF8;

//             if (bytes[0] == 0xFF && bytes[1] == 0xFE)
//                 return Encoding.Unicode;

//             if (bytes[0] == 0xFE && bytes[1] == 0xFF)
//                 return Encoding.BigEndianUnicode;

//             return null;
//         }

//         private static bool IsValidUTF8(byte[] bytes)
//         {
//             try
//             {
//                 UTF8Encoding utf8 = new UTF8Encoding(false, true);
//                 string text = utf8.GetString(bytes);
//                 return text.Any(c => c > 127);
//             }
//             catch { return false; }
//         }

//         private static bool IsGoodKoreanText(string text)
//         {
//             if (string.IsNullOrEmpty(text)) return false;

//             int koreanCount = 0;
//             int totalCount = 0;
//             int invalidCount = 0;

//             foreach (char c in text.Take(4096))
//             {
//                 if (char.IsWhiteSpace(c) || char.IsControl(c)) continue;
//                 totalCount++;

//                 if (c == 0xFFFD || c == '�') invalidCount++;
//                 if (IsKoreanCharacter(c)) koreanCount++;
//             }

//             return totalCount > 0 && koreanCount > 0 && (double)invalidCount / totalCount < 0.5;
//         }

//         private static bool IsKoreanCharacter(char c)
//         {
//             return (c >= 0xAC00 && c <= 0xD7AF) ||
//                    (c >= 0x1100 && c <= 0x11FF) ||
//                    (c >= 0x3130 && c <= 0x318F);
//         }
//     }
// }
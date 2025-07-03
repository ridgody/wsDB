// using System;
// using System.IO;
// using System.Text;
// using System.Collections.Generic;
// using System.Diagnostics; 

// namespace wsDB.Common.Text.KorEncoding
// {
//     public class DotNetCoreKoreanEncodingDetector
//     {
//         private static bool _encodingProviderRegistered = false;

//         // .NET Core에서 지원되는 기본 인코딩만
//         private static readonly int[] CoreSupportedCodePages = {
//             65001,  // UTF-8
//             1200,   // UTF-16 LE
//             1201,   // UTF-16 BE
//             12000,  // UTF-32 LE
//             12001,  // UTF-32 BE
//             20127,  // US-ASCII
//             28591   // ISO-8859-1 (Western European)
//         };

//         // 추가 인코딩 프로바이더 등록 후 사용 가능한 한국어 인코딩
//         private static readonly int[] ExtendedKoreanCodePages = {
//             949,    // Windows-949 (UHC/CP949)
//             51949,  // EUC-KR
//             1252,   // Windows-1252
//             20949,  // Korean Wansung
//             50225   // ISO-2022-KR
//         };

//         static DotNetCoreKoreanEncodingDetector()
//         {
//             RegisterEncodingProvider();
//         }

//         private static void RegisterEncodingProvider()
//         {
//             if (_encodingProviderRegistered) return;

//             try
//             {
//                 // .NET Core에서 추가 인코딩 지원을 위해 등록
//                 Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//                 _encodingProviderRegistered = true;
//                 Debug.WriteLine("인코딩 프로바이더 등록 성공");
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine($"인코딩 프로바이더 등록 실패: {ex.Message}");
//                 Debug.WriteLine("기본 인코딩만 사용됩니다.");
//             }
//         }

//         public static Encoding DetectFileEncoding(string filePath)
//         {
//             try
//             {
//                 byte[] bytes = File.ReadAllBytes(filePath);
//                 return DetectEncoding(bytes);
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine($"파일 읽기 실패: {ex.Message}");
//                 return Encoding.UTF8;
//             }
//         }

//         public static Encoding DetectEncoding(byte[] bytes)
//         {
//             if (bytes == null || bytes.Length == 0)
//             {
//                 Debug.WriteLine("빈 파일");
//                 return Encoding.UTF8;
//             }

//             Debug.WriteLine($"파일 크기: {bytes.Length} 바이트");

//             // 1. BOM 체크
//             Encoding bomEncoding = DetectBOM(bytes);
//             if (bomEncoding != null)
//             {
//                 Debug.WriteLine($"BOM 감지: {bomEncoding.EncodingName}");
//                 return bomEncoding;
//             }

//             // 2. UTF-8 유효성 체크
//             if (IsValidUTF8(bytes))
//             {
//                 Debug.WriteLine("UTF-8 (BOM 없음) 감지");
//                 return new UTF8Encoding(false);
//             }

//             // 3. 기본 지원 인코딩 시도
//             Encoding coreEncoding = TryCoreEncodings(bytes);
//             if (coreEncoding != null)
//             {
//                 Debug.WriteLine($"기본 인코딩 감지: {coreEncoding.EncodingName}");
//                 return coreEncoding;
//             }

//             // 4. 확장 한국어 인코딩 시도 (프로바이더 등록 필요)
//             if (_encodingProviderRegistered)
//             {
//                 Encoding koreanEncoding = TryKoreanEncodings(bytes);
//                 if (koreanEncoding != null)
//                 {
//                     Debug.WriteLine($"한국어 인코딩 감지: {koreanEncoding.EncodingName}");
//                     return koreanEncoding;
//                 }
//             }
//             else
//             {
//                 Debug.WriteLine("한국어 인코딩 프로바이더가 등록되지 않아 UTF-8로 시도");
//             }

//             // 5. 마지막 시도: 바이트 패턴으로 한국어 추정
//             if (LooksLikeKoreanBytes(bytes))
//             {
//                 Debug.WriteLine("바이트 패턴으로 한국어 파일 추정됨, UTF-8로 처리");
//                 // UTF-8로 읽되 오류는 무시
//                 return new UTF8Encoding(false, false);
//             }

//             Debug.WriteLine("모든 감지 실패, UTF-8로 폴백");
//             return Encoding.UTF8;
//         }

//         private static Encoding DetectBOM(byte[] bytes)
//         {
//             if (bytes.Length < 2) return null;

//             // UTF-32 LE: FF FE 00 00
//             if (bytes.Length >= 4 && bytes[0] == 0xFF && bytes[1] == 0xFE &&
//                 bytes[2] == 0x00 && bytes[3] == 0x00)
//                 return Encoding.UTF32;

//             // UTF-32 BE: 00 00 FE FF
//             if (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 &&
//                 bytes[2] == 0xFE && bytes[3] == 0xFF)
//                 return new UTF32Encoding(true, true);

//             // UTF-8: EF BB BF
//             if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
//                 return Encoding.UTF8;

//             // UTF-16 LE: FF FE
//             if (bytes[0] == 0xFF && bytes[1] == 0xFE)
//                 return Encoding.Unicode;

//             // UTF-16 BE: FE FF
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

//                 // 비 ASCII 문자가 있는지 체크
//                 foreach (char c in text)
//                 {
//                     if (c > 127) return true;
//                 }

//                 return false;
//             }
//             catch
//             {
//                 return false;
//             }
//         }

//         private static Encoding TryCoreEncodings(byte[] bytes)
//         {
//             Debug.WriteLine("기본 지원 인코딩 시도 중...");

//             foreach (int codePage in CoreSupportedCodePages)
//             {
//                 try
//                 {
//                     Encoding encoding = Encoding.GetEncoding(codePage);
//                     string text = encoding.GetString(bytes);

//                     Debug.WriteLine($"코드페이지 {codePage} ({encoding.EncodingName}) 시도");

//                     if (HasReadableText(text))
//                     {
//                         Debug.WriteLine($"  → 읽기 가능한 텍스트 발견");
//                         return encoding;
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     Debug.WriteLine($"코드페이지 {codePage} 실패: {ex.Message}");
//                 }
//             }

//             return null;
//         }

//         private static Encoding TryKoreanEncodings(byte[] bytes)
//         {
//             Debug.WriteLine("한국어 인코딩 시도 중...");

//             foreach (int codePage in ExtendedKoreanCodePages)
//             {
//                 try
//                 {
//                     Encoding encoding = Encoding.GetEncoding(codePage);
//                     string text = encoding.GetString(bytes);

//                     Debug.WriteLine($"코드페이지 {codePage} ({encoding.EncodingName}) 시도");

//                     if (IsGoodKoreanText(text))
//                     {
//                         Debug.WriteLine($"  → 한글 텍스트 발견!");
//                         return encoding;
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     Debug.WriteLine($"코드페이지 {codePage} 실패: {ex.Message}");
//                 }
//             }

//             return null;
//         }

//         private static bool HasReadableText(string text)
//         {
//             if (string.IsNullOrEmpty(text)) return false;

//             int readableCount = 0;
//             int totalCount = 0;

//             foreach (char c in text)
//             {
//                 if (char.IsWhiteSpace(c) || char.IsControl(c)) continue;

//                 totalCount++;

//                 // 읽을 수 있는 문자 (ASCII, 한글, 기타 유니코드)
//                 if ((c >= 32 && c <= 126) || // ASCII 출력 가능 문자
//                     (c >= 0xAC00 && c <= 0xD7AF) || // 한글
//                     (c >= 0x1100 && c <= 0x11FF) || // 한글 자모
//                     c == 0xFFFD) // 교체 문자도 읽기 가능으로 간주
//                 {
//                     readableCount++;
//                 }
//             }

//             if (totalCount == 0) return false;

//             double readableRatio = (double)readableCount / totalCount;
//             return readableRatio > 1; // 70% 이상 읽기 가능하면 OK
//             //  return readableRatio > 0.7; // 70% 이상 읽기 가능하면 OK
//         }

//         private static bool IsGoodKoreanText(string text)
//         {
//             if (string.IsNullOrEmpty(text)) return false;
        
//             int koreanCount = 0;
//             int totalCount = 0;
//             int invalidCount = 0;
            
//             // 텍스트 샘플 확인 (처음 200자만)
//             string sample = text.Length > 2000 ? text.Substring(0, 2000) : text;
            
//             foreach (char c in sample)
//             {
//                 if (char.IsWhiteSpace(c) || char.IsControl(c)) continue;
                
//                 totalCount++;
                
//                 // 깨진 문자 체크
//                 if (c == 0xFFFD || c == '�' || c == '?')
//                 {
//                     invalidCount++;
//                     continue;
//                 }
                
//                 if (IsKoreanCharacter(c))
//                     koreanCount++;
//             }
            
//             if (totalCount == 0) return false;
            
//             double koreanRatio = (double)koreanCount / totalCount;
//             double invalidRatio = (double)invalidCount / totalCount;
            
//             Debug.WriteLine($"  한글 비율: {koreanRatio:P1}, 깨진 문자: {invalidRatio:P1}");
//             Debug.WriteLine($"  샘플 텍스트: {sample.Substring(0, Math.Min(50, sample.Length))}");
            
//             // 기준 완화: 한글이 1자라도 있고 깨진 문자가 50% 미만이면 성공
//             return koreanCount > 0 && invalidRatio < 0.5;
//         }

//         private static bool IsKoreanCharacter(char c)
//         {
//             return (c >= 0xAC00 && c <= 0xD7AF) ||  // 완성형 한글
//                 (c >= 0x1100 && c <= 0x11FF) ||  // 한글 자모
//                 (c >= 0x3130 && c <= 0x318F);    // 한글 호환 자모
//         }

//         // 바이트 패턴으로 한국어 파일인지 추정
//         private static bool LooksLikeKoreanBytes(byte[] bytes)
//         {
//             // EUC-KR 패턴: A1-FE 범위의 바이트가 연속으로 나타남
//             int koreanBytePatterns = 0;

//             for (int i = 0; i < bytes.Length - 1; i++)
//             {
//                 // EUC-KR/CP949 한글 영역 패턴
//                 if ((bytes[i] >= 0xA1 && bytes[i] <= 0xFE) &&
//                     (bytes[i + 1] >= 0xA1 && bytes[i + 1] <= 0xFE))
//                 {
//                     koreanBytePatterns++;
//                     i++; // 다음 바이트는 건너뛰기
//                 }
//             }

//             // 10개 이상의 한국어 바이트 패턴이 있으면 한국어 파일로 추정
//             bool isKorean = koreanBytePatterns > 10;

//             Debug.WriteLine($"한국어 바이트 패턴 개수: {koreanBytePatterns}, 한국어 파일 추정: {isKorean}");

//             return isKorean;
//         }

//         // 사용 가능한 인코딩 목록 출력
//         public static void ListAvailableEncodings()
//         {
//             Debug.WriteLine("=== 사용 가능한 인코딩 목록 ===");
//             Debug.WriteLine($"인코딩 프로바이더 등록됨: {_encodingProviderRegistered}");

//             foreach (EncodingInfo info in Encoding.GetEncodings())
//             {
//                 Debug.WriteLine($"코드페이지 {info.CodePage}: {info.Name} ({info.DisplayName})");
//             }
//         }
//     }
// }
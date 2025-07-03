// using System.Diagnostics;
// using System.Windows;

// namespace wsDB.Common.Text.Utilities
// {
//     /// <summary>
//     /// 클립보드 관련 유틸리티
//     /// </summary>
//     public static class ClipboardHelper
//     {
//         public static string GetTextSafely()
//         {
//             try
//             {
//                 if (Clipboard.ContainsText(TextDataFormat.Text))
//                     return Clipboard.GetText(TextDataFormat.Text);

//                 if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
//                     return Clipboard.GetText(TextDataFormat.UnicodeText);

//                 if (Clipboard.ContainsText())
//                     return Clipboard.GetText();

//                 return string.Empty;
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine($"클립보드 읽기 오류: {ex.Message}");
//                 return string.Empty;
//             }
//         }

//         public static bool SetTextSafely(string text)
//         {
//             try
//             {
//                 Clipboard.SetText(text);
//                 return true;
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine($"클립보드 설정 오류: {ex.Message}");
//                 return false;
//             }
//         }
//     }
// }
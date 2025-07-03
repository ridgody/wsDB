
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Input;

// namespace wsDB.Common.Text.Utilities
// {
//     /// <summary>
//     /// 키보드 단축키 처리기
//     /// </summary>
//     public static class KeyboardShortcutHandler
//     {
//         public static bool HandleKeyDown(KeyEventArgs e, RichTextBox rtb, Action<string> applyFormattedText)
//         {
//             // Ctrl+V 붙여넣기
//             if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
//             {
//                 if (Clipboard.ContainsText())
//                 {
//                     string clipboardText = ClipboardHelper.GetTextSafely();
//                     if (!string.IsNullOrEmpty(clipboardText))
//                         applyFormattedText(clipboardText);
//                     return true;
//                 }
//             }
//             // Ctrl+A 전체 선택
//             else if (e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
//             {
//                 rtb.SelectAll();
//                 return true;
//             }
//             // Ctrl+C 복사
//             else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
//             {
//                 if (!rtb.Selection.IsEmpty)
//                     ClipboardHelper.SetTextSafely(rtb.Selection.Text);
//                 return true;
//             }

//             return false;
//         }
//     }
// }
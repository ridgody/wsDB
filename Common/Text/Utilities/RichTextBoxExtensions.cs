// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Documents;
// using System.Windows.Input;
// using System.Windows.Media;

// namespace wsDB.Common.Text.Utilities
// {
//     /// <summary>
//     /// RichTextBox 확장 기능
//     /// </summary>
//     public static class RichTextBoxExtensions
//     {
//         public static void SetupEventHandlers(this RichTextBox rtb,
//             MouseEventHandler mouseMove = null,
//             MouseButtonEventHandler mouseDown = null,
//             KeyEventHandler keyDown = null,
//             KeyEventHandler keyUp = null)
//         {
//             if (mouseMove != null) rtb.MouseMove += mouseMove;
//             if (mouseDown != null) rtb.PreviewMouseLeftButtonDown += mouseDown;
//             if (keyDown != null) rtb.KeyDown += keyDown;
//             if (keyUp != null) rtb.KeyUp += keyUp;
//         }

//         public static void ClearAndAddFormattedText(this RichTextBox rtb, string text)
//         {
//             rtb.Document.Blocks.Clear();
//             ApplyFormattedText(rtb, text);
//         }

//         private static void ApplyFormattedText(RichTextBox rtb, string text)
//         {
//             string[] lines = text.Split('\n');

//             foreach (string line in lines)
//             {
//                 Paragraph paragraph = new Paragraph
//                 {
//                     Margin = new Thickness(0),
//                     LineHeight = rtb.FontSize
//                 };

//                 Run run = new Run(line.TrimEnd('\r'));

//                 // 실행계획 라인 색상 구분
//                 if (line.Contains("|") && (line.Contains("SCAN") || line.Contains("ACCESS") ||
//                     line.Contains("JOIN") || line.Contains("HASH")))
//                 {
//                     run.Foreground = new SolidColorBrush(Color.FromRgb(0, 100, 0));
//                 }
//                 else if (line.Contains("TB_") || line.Contains("IDX_"))
//                 {
//                     run.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 150));
//                 }
//                 else if (line.StartsWith("Plan hash") || line.StartsWith("Predicate") ||
//                          line.StartsWith("Statistics"))
//                 {
//                     run.Foreground = new SolidColorBrush(Color.FromRgb(150, 0, 0));
//                     run.FontWeight = FontWeights.Bold;
//                 }

//                 paragraph.Inlines.Add(run);
//                 rtb.Document.Blocks.Add(paragraph);
//             }
//         }
//     }
// }
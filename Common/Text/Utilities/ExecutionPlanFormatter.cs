// using System.Text.RegularExpressions;
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Documents;
// using System.Windows.Input;
// using System.Windows.Media;

// namespace wsDB.Common.Text.Utilities
// {
//     /// <summary>
//     /// 실행계획 포맷터
//     /// </summary>
//     public static class ExecutionPlanFormatter
//     {
//         private static readonly string TablePattern = @"\b[A-Z_][A-Z0-9_]*\.[A-Z_][A-Z0-9_]*\b|\b[A-Z_][A-Z0-9_]*\b(?=\s+\(TABLE\)|\s+\(INDEX\)|$)";
//         private static readonly string IndexPattern = @"\b[A-Z_][A-Z0-9_]*\b(?=\s+\(INDEX\)|(?<=INDEX\s+)[A-Z_][A-Z0-9_]*)";

//         public static void FormatExecutionPlan(RichTextBox rtb, string text)
//         {
//             rtb.Document.Blocks.Clear();
//             string[] lines = text.Split('\n');

//             foreach (string line in lines)
//             {
//                 Paragraph paragraph = new Paragraph { Margin = new Thickness(0) };
//                 ProcessLineWithObjects(paragraph, line);
//                 rtb.Document.Blocks.Add(paragraph);
//             }
//         }

//         private static void ProcessLineWithObjects(Paragraph paragraph, string line)
//         {
//             var matches = new List<(int Start, int Length, string Type, string Text)>();

//             // 테이블 매치
//             foreach (Match match in Regex.Matches(line, TablePattern))
//                 matches.Add((match.Index, match.Length, "TABLE", match.Value));

//             // 인덱스 매치
//             foreach (Match match in Regex.Matches(line, IndexPattern))
//                 matches.Add((match.Index, match.Length, "INDEX", match.Value));

//             matches = matches.OrderBy(m => m.Start).ToList();

//             int currentPos = 0;
//             foreach (var match in matches)
//             {
//                 // 매치 이전 텍스트
//                 if (match.Start > currentPos)
//                     paragraph.Inlines.Add(new Run(line.Substring(currentPos, match.Start - currentPos)));

//                 // 하이라이트된 객체
//                 Run objectRun = new Run(match.Text)
//                 {
//                     FontWeight = FontWeights.Bold,
//                     Foreground = Brushes.DarkBlue,
//                     Cursor = Cursors.Hand,
//                     Tag = $"{match.Type}:{match.Text}"
//                 };

//                 paragraph.Inlines.Add(objectRun);
//                 currentPos = match.Start + match.Length;
//             }

//             // 남은 텍스트
//             if (currentPos < line.Length)
//                 paragraph.Inlines.Add(new Run(line.Substring(currentPos)));
//         }
//     }
// }

// using System.Windows.Documents;
// using System.Windows.Media;

// namespace wsDB.Common.Text.Utilities
// {
//     /// <summary>
//     /// 텍스트 하이라이트 관리자
//     /// </summary>
//     public class TextHighlighter
//     {
//         private TextPointer lastHoveredStart;
//         private TextPointer lastHoveredEnd;

//         public void HighlightWord(TextPointer start, TextPointer end)
//         {
//             if (start != null && end != null)
//             {
//                 try
//                 {
//                     TextRange range = new TextRange(start, end);
//                     range.ApplyPropertyValue(TextElement.BackgroundProperty,
//                         new SolidColorBrush(Color.FromArgb(100, 135, 206, 250)));
//                     range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkBlue);

//                     lastHoveredStart = start;
//                     lastHoveredEnd = end;
//                 }
//                 catch { }
//             }
//         }

//         public void RemoveHighlight()
//         {
//             if (lastHoveredStart != null && lastHoveredEnd != null)
//             {
//                 try
//                 {
//                     TextRange range = new TextRange(lastHoveredStart, lastHoveredEnd);
//                     range.ClearAllProperties();
//                 }
//                 catch { }
//             }
//         }

//         public bool IsSameRange(TextPointer start, TextPointer end)
//         {
//             return AreTextPointersEqual(start, lastHoveredStart) &&
//                    AreTextPointersEqual(end, lastHoveredEnd);
//         }

//         private bool AreTextPointersEqual(TextPointer ptr1, TextPointer ptr2)
//         {
//             if (ptr1 == null && ptr2 == null) return true;
//             if (ptr1 == null || ptr2 == null) return false;
//             return ptr1.CompareTo(ptr2) == 0;
//         }
//     }
// }
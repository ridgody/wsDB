// using System.Windows.Documents;
// using System.Globalization;

// namespace wsDB.Common.Text.Extentions
//     {
//     public static class TextPointerExtensions
//     {
//         /// <summary>
//         /// 지정된 TextPointer에서 단어의 시작 위치를 찾습니다.
//         /// </summary>
//         /// <param name="position">검색을 시작할 TextPointer</param>
//         /// <returns>단어 시작 위치의 TextPointer</returns>
//         public static TextPointer GetWordStart(this TextPointer position)
//         {
//             if (position == null)
//                 return null;

//             var navigator = position.GetPositionAtOffset(0, LogicalDirection.Backward);

//             // 현재 위치에서 뒤로 이동하면서 단어 경계를 찾습니다
//             while (navigator != null && navigator.CompareTo(position.DocumentStart) > 0)
//             {
//                 var previousChar = navigator.GetTextInRun(LogicalDirection.Backward);

//                 if (string.IsNullOrEmpty(previousChar))
//                 {
//                     navigator = navigator.GetNextContextPosition(LogicalDirection.Backward);
//                     continue;
//                 }

//                 var lastChar = previousChar[previousChar.Length - 1];

//                 // 공백, 탭, 줄바꿈 등의 구분자를 만나면 단어의 시작점입니다
//                 if (char.IsWhiteSpace(lastChar) || char.IsPunctuation(lastChar) || char.IsSymbol(lastChar))
//                 {
//                     break;
//                 }

//                 navigator = navigator.GetNextContextPosition(LogicalDirection.Backward);
//             }

//             return navigator ?? position.DocumentStart;
//         }

//         /// <summary>
//         /// 지정된 TextPointer에서 단어의 끝 위치를 찾습니다.
//         /// </summary>
//         /// <param name="position">검색을 시작할 TextPointer</param>
//         /// <returns>단어 끝 위치의 TextPointer</returns>
//         public static TextPointer GetWordEnd(this TextPointer position)
//         {
//             if (position == null)
//                 return null;

//             var navigator = position.GetPositionAtOffset(0, LogicalDirection.Forward);

//             // 현재 위치에서 앞으로 이동하면서 단어 경계를 찾습니다
//             while (navigator != null && navigator.CompareTo(position.DocumentEnd) < 0)
//             {
//                 var nextChar = navigator.GetTextInRun(LogicalDirection.Forward);

//                 if (string.IsNullOrEmpty(nextChar))
//                 {
//                     navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
//                     continue;
//                 }

//                 var firstChar = nextChar[0];

//                 // 공백, 탭, 줄바꿈 등의 구분자를 만나면 단어의 끝점입니다
//                 if (char.IsWhiteSpace(firstChar) || char.IsPunctuation(firstChar) || char.IsSymbol(firstChar))
//                 {
//                     break;
//                 }

//                 navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
//             }

//             return navigator ?? position.DocumentEnd;
//         }

//         /// <summary>
//         /// 현재 위치의 전체 단어를 가져옵니다.
//         /// </summary>
//         /// <param name="position">검색할 TextPointer</param>
//         /// <returns>단어 문자열</returns>
//         public static string GetWord(this TextPointer position)
//         {
//             if (position == null)
//                 return string.Empty;

//             var wordStart = position.GetWordStart();
//             var wordEnd = position.GetWordEnd();

//             if (wordStart == null || wordEnd == null)
//                 return string.Empty;

//             return new TextRange(wordStart, wordEnd).Text;
//         }

//         private static bool IsWordCharacter(char c)
//         {
//             return char.IsLetterOrDigit(c) || c == '_' || c == '.';
//         }

//         public static (TextPointer start, TextPointer end) GetWordBounds(TextPointer position)
//         {
//             if (position == null)
//                 return (null, null);

//             // 현재 위치에서 가장 가까운 단어 문자 찾기
//             TextPointer searchStart = position;

//             // 현재 위치 또는 앞뒤로 단어 문자 찾기
//             string currentText = searchStart.GetTextInRun(LogicalDirection.Forward);
//             if (string.IsNullOrEmpty(currentText) || !IsWordCharacter(currentText[0]))
//             {
//                 currentText = searchStart.GetTextInRun(LogicalDirection.Backward);
//                 if (!string.IsNullOrEmpty(currentText) && IsWordCharacter(currentText[currentText.Length - 1]))
//                 {
//                     searchStart = searchStart.GetPositionAtOffset(-1, LogicalDirection.Backward) ?? searchStart;
//                 }
//                 else
//                 {
//                     return (null, null);
//                 }
//             }

//             // 단어의 시작점 찾기
//             TextPointer start = searchStart;
//             while (start != null)
//             {
//                 TextPointer prevPos = start.GetPositionAtOffset(-1, LogicalDirection.Backward);
//                 if (prevPos == null)
//                     break;

//                 string prevChar = prevPos.GetTextInRun(LogicalDirection.Forward);
//                 if (string.IsNullOrEmpty(prevChar) || !IsWordCharacter(prevChar[0]))
//                     break;

//                 start = prevPos;
//             }

//             // 단어의 끝점 찾기
//             TextPointer end = searchStart;
//             while (end != null)
//             {
//                 string nextChar = end.GetTextInRun(LogicalDirection.Forward);
//                 if (string.IsNullOrEmpty(nextChar) || !IsWordCharacter(nextChar[0]))
//                     break;

//                 TextPointer nextPos = end.GetPositionAtOffset(1, LogicalDirection.Forward);
//                 if (nextPos == null)
//                     break;

//                 end = nextPos;
//             }

//             return (start, end);
//         }
//     }
// }
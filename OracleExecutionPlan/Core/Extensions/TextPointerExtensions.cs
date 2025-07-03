using System.Windows.Documents;

namespace wsDB.OracleExecutionPlan.Core.Extensions
{
    public static class TextPointerExtensions
    {
        public static (TextPointer start, TextPointer end) GetWordBounds(TextPointer position)
        {
            if (position == null)
                return (null, null);

            // 현재 위치에서 단어 문자 찾기
            TextPointer wordPosition = FindNearestWordCharacter(position);
            if (wordPosition == null)
                return (null, null);

            TextPointer start = FindWordStart(wordPosition);
            TextPointer end = FindWordEnd(wordPosition);

            return (start, end);
        }

        private static TextPointer FindNearestWordCharacter(TextPointer position)
        {
            // 현재 위치에서 앞쪽 확인
            string forwardText = position.GetTextInRun(LogicalDirection.Forward);
            if (!string.IsNullOrEmpty(forwardText) && IsWordCharacter(forwardText[0]))
                return position;

            // 뒤쪽 확인
            string backwardText = position.GetTextInRun(LogicalDirection.Backward);
            if (!string.IsNullOrEmpty(backwardText) && IsWordCharacter(backwardText[backwardText.Length - 1]))
            {
                return position.GetPositionAtOffset(-1, LogicalDirection.Backward) ?? position;
            }

            return null;
        }
        private static TextPointer FindWordStart(TextPointer position)
        {
            return FindWordStartRecursive(position);
        }

        private static TextPointer FindWordStartRecursive(TextPointer current)
        {
            if (current == null)
                return null;

            // 이전 위치 확인
            TextPointer prevPos = current.GetPositionAtOffset(-1, LogicalDirection.Backward);
            if (prevPos == null)
                return current;

            // 이전 문자 확인
            string prevText = prevPos.GetTextInRun(LogicalDirection.Forward);
            if (string.IsNullOrEmpty(prevText) || !IsWordCharacter(prevText[0]))
                return current;

            // 재귀적으로 계속 찾기
            return FindWordStartRecursive(prevPos);
        }

        private static TextPointer FindWordEnd(TextPointer position)
        {
            return FindWordEndRecursive(position);
        }

        private static TextPointer FindWordEndRecursive(TextPointer current)
        {
            if (current == null)
                return null;

            // 현재 위치의 문자 확인
            string currentText = current.GetTextInRun(LogicalDirection.Forward);
            if (string.IsNullOrEmpty(currentText) || !IsWordCharacter(currentText[0]))
                return current;

            // 다음 위치로 이동
            TextPointer nextPos = current.GetPositionAtOffset(1, LogicalDirection.Forward);
            if (nextPos == null)
            {
                // 문서 끝에 도달 - 현재 문자가 단어 문자라면 다음 위치 반환
                return current.GetPositionAtOffset(1, LogicalDirection.Forward) ?? current;
            }

            // 재귀적으로 계속 찾기
            return FindWordEndRecursive(nextPos);
        }

        private static bool IsWordCharacter(char c)
        {
            // 문자, 숫자, 언더스코어를 단어 문자로 간주
            return char.IsLetterOrDigit(c) || c == '_';
        }

    }
}
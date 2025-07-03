using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using wsDB.OracleExecutionPlan.Core.Extensions;

namespace wsDB.OracleExecutionPlan.UI.EventHandlers
{
    public static class WordSelectionHandler
    {
        /// <summary>
        /// 마우스 위치에서 단어를 찾아 선택하고 클립보드에 복사
        /// </summary>
        public static bool HandleMouseWordSelection(RichTextBox rtb, Point mousePosition)
        {
            TextPointer pointer = rtb.GetPositionFromPoint(mousePosition, true);

            if (pointer != null)
            {
                var wordBounds = TextPointerExtensions.GetWordBounds(pointer);

                if (wordBounds.start != null && wordBounds.end != null)
                {
                    rtb.Selection.Select(wordBounds.start, wordBounds.end);
                    string selectedWord = rtb.Selection.Text.Trim();

                    if (!string.IsNullOrEmpty(selectedWord))
                    {
                        Clipboard.SetText(selectedWord);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 현재 커서 위치에서 단어를 찾아 선택
        /// </summary>
        public static bool HandleCaretWordSelection(RichTextBox rtb)
        {
            // 현재 커서 위치 가져오기
            TextPointer caretPosition = rtb.CaretPosition;

            if (caretPosition != null)
            {
                var wordBounds = TextPointerExtensions.GetWordBounds(caretPosition);

                if (wordBounds.start != null && wordBounds.end != null)
                {
                    rtb.Selection.Select(wordBounds.start, wordBounds.end);
                    string selectedWord = rtb.Selection.Text.Trim();

                    return !string.IsNullOrEmpty(selectedWord);
                }
            }
            return false;
        }

        /// <summary>
        /// 현재 커서 위치에서 단어를 찾아 선택하고 클립보드에 복사
        /// </summary>
        public static bool HandleCaretWordSelectionWithCopy(RichTextBox rtb)
        {
            if (HandleCaretWordSelection(rtb))
            {
                string selectedWord = rtb.Selection.Text.Trim();
                if (!string.IsNullOrEmpty(selectedWord))
                {
                    Clipboard.SetText(selectedWord);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 선택된 텍스트가 있는지 확인하고 없으면 커서 위치의 단어 선택
        /// </summary>
        public static bool EnsureWordSelection(RichTextBox rtb)
        {
            // 이미 텍스트가 선택되어 있는지 확인
            if (!rtb.Selection.IsEmpty && !string.IsNullOrWhiteSpace(rtb.Selection.Text))
            {
                return true; // 이미 선택된 텍스트가 있음
            }

            // 선택된 텍스트가 없으면 커서 위치의 단어 선택
            return HandleCaretWordSelection(rtb);
        }

        /// <summary>
        /// 현재 선택된 텍스트 또는 커서 위치의 단어 가져오기
        /// </summary>
        public static string GetSelectedOrCurrentWord(RichTextBox rtb)
        {
            // 이미 선택된 텍스트가 있으면 반환
            if (!rtb.Selection.IsEmpty && !string.IsNullOrWhiteSpace(rtb.Selection.Text))
            {
                return rtb.Selection.Text.Trim();
            }

            // 커서 위치의 단어 선택 시도
            if (HandleCaretWordSelection(rtb))
            {
                return rtb.Selection.Text.Trim();
            }

            return string.Empty;
        }
    }
}
using System;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace wsDB.OracleExecutionPlan.Core.Text
{
    public class TextProcessor
    {
        public string CleanTextForObjectSearch(string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
                return string.Empty;

            string cleaned = rawText.Trim();
            cleaned = Regex.Replace(cleaned, @"[^\w가-힣]", "");
            return cleaned;
        }

        public bool AreTextPointersEqual(TextPointer ptr1, TextPointer ptr2)
        {
            if (ptr1 == null && ptr2 == null)
                return true;
            if (ptr1 == null || ptr2 == null)
                return false;
            return ptr1.CompareTo(ptr2) == 0;
        }
    }
}

using System.Text.RegularExpressions;

namespace wsDB.Common.Text.Utilities
{
    /// <summary>
    /// 텍스트 정리 및 검색 유틸리티
    /// </summary>
    public static class TextCleaner
    {
        public static string CleanForObjectSearch(string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
                return string.Empty;

            string cleaned = rawText.Trim();
            cleaned = Regex.Replace(cleaned, @"[^\w가-힣]", "");
            return cleaned;
        }

        public static bool IsValidText(string text, double invalidThreshold = 0.05)
        {
            if (string.IsNullOrEmpty(text)) return true;

            int invalidChars = text.Count(c => c == '�' || c == '\uFFFD');
            return (double)invalidChars / text.Length < invalidThreshold;
        }
    }
}
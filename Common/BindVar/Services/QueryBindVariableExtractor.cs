// ========== Services/QueryBindVariableExtractor.cs ==========
using System.Text.RegularExpressions;

namespace wsDB.Common.BindVar.Services
{
    public static class QueryBindVariableExtractor
    {
        /// <summary>
        /// SQL 쿼리에서 바인드 변수(:변수명)를 추출합니다.
        /// </summary>
        /// <param name="sql">SQL 쿼리</param>
        /// <returns>바인드 변수 목록 (중복 제거됨)</returns>
        public static List<string> ExtractBindVariables(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return new List<string>();

            // :변수명 패턴 찾기 (영문자, 숫자, 언더스코어 허용) 
             var pattern = @":([a-zA-Z_][a-zA-Z0-9_]*|\d+)";
            var matches = Regex.Matches(sql, pattern, RegexOptions.IgnoreCase);

            var variables = new HashSet<string>();
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    // 문자열 리터럴이나 주석 내부의 바인드 변수는 제외
                    if (!IsInStringLiteralOrComment(sql, match.Index))
                    {
                        variables.Add(match.Groups[1].Value);
                    }
                }
            }

            return variables.ToList();
        }

        /// <summary>
        /// 바인드 변수가 문자열 리터럴이나 주석 내부에 있는지 확인합니다.
        /// </summary>
        /// <param name="sql">전체 SQL 문</param>
        /// <param name="position">확인할 위치</param>
        /// <returns>문자열 리터럴이나 주석 내부에 있으면 true</returns>
        private static bool IsInStringLiteralOrComment(string sql, int position)
        {
            bool inSingleQuote = false;
            bool inDoubleQuote = false;
            bool inLineComment = false;
            bool inBlockComment = false;

            for (int i = 0; i < position && i < sql.Length; i++)
            {
                char current = sql[i];
                char next = i + 1 < sql.Length ? sql[i + 1] : '\0';

                // 라인 주석 확인
                if (!inSingleQuote && !inDoubleQuote && !inBlockComment && current == '-' && next == '-')
                {
                    inLineComment = true;
                    i++; // 다음 문자도 건너뛰기
                    continue;
                }

                // 라인 주석 종료 (개행 문자)
                if (inLineComment && (current == '\n' || current == '\r'))
                {
                    inLineComment = false;
                    continue;
                }

                // 블록 주석 시작
                if (!inSingleQuote && !inDoubleQuote && !inLineComment && current == '/' && next == '*')
                {
                    inBlockComment = true;
                    i++; // 다음 문자도 건너뛰기
                    continue;
                }

                // 블록 주석 종료
                if (inBlockComment && current == '*' && next == '/')
                {
                    inBlockComment = false;
                    i++; // 다음 문자도 건너뛰기
                    continue;
                }

                // 주석 내부에 있으면 다른 처리 건너뛰기
                if (inLineComment || inBlockComment)
                    continue;

                // 작은따옴표 처리
                if (current == '\'' && !inDoubleQuote)
                {
                    // 연속된 작은따옴표는 이스케이프된 것으로 처리
                    if (inSingleQuote && next == '\'')
                    {
                        i++; // 다음 문자도 건너뛰기
                        continue;
                    }
                    inSingleQuote = !inSingleQuote;
                }

                // 큰따옴표 처리 (Oracle에서는 주로 식별자용)
                if (current == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                }
            }

            return inSingleQuote || inDoubleQuote || inLineComment || inBlockComment;
        }

        /// <summary>
        /// 쿼리에 바인드 변수가 있는지 확인합니다.
        /// </summary>
        /// <param name="sql">SQL 쿼리</param>
        /// <returns>바인드 변수가 있으면 true</returns>
        public static bool HasBindVariables(string sql)
        {
            return ExtractBindVariables(sql).Count > 0;
        }

        /// <summary>
        /// 바인드 변수 개수를 반환합니다.
        /// </summary>
        /// <param name="sql">SQL 쿼리</param>
        /// <returns>바인드 변수 개수</returns>
        public static int GetBindVariableCount(string sql)
        {
            return ExtractBindVariables(sql).Count;
        }
    }
}
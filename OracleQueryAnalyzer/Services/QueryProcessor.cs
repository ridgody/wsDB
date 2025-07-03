using wsDB.OracleQueryAnalyzer.Models;

namespace wsDB.OracleQueryAnalyzer.Services
{
    public class QueryProcessor
    {
        private static readonly string[] SELECT_KEYWORDS = { "SELECT", "WITH" };
        private const string GATHER_PLAN_HINT = "/*+ gather_plan_statistics */";

        public QueryValidationResult ValidateQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new QueryValidationResult(false, "쿼리가 비어있습니다.");
            }

            string trimmedQuery = query.Trim().ToUpper();
            
            bool isSelectQuery = Array.Exists(SELECT_KEYWORDS, keyword => 
                trimmedQuery.StartsWith(keyword));

            if (!isSelectQuery)
            {
                return new QueryValidationResult(false, "SELECT 또는 WITH로 시작하는 쿼리만 분석할 수 있습니다.");
            }

            return new QueryValidationResult(true, "유효한 쿼리입니다.");
        }

        public string ProcessQueryForExecution(string originalQuery)
        {
            if (string.IsNullOrWhiteSpace(originalQuery))
                return originalQuery;

            string processedQuery = originalQuery.Trim();
            
            // 이미 gather_plan_statistics 힌트가 있는지 확인
            if (processedQuery.Contains("gather_plan_statistics"))
            {
                return processedQuery;
            }

            // WITH절로 시작하는 경우
            if (processedQuery.ToUpper().StartsWith("WITH"))
            {
                return InsertHintAfterWith(processedQuery);
            }
            
            // SELECT절로 시작하는 경우
            if (processedQuery.ToUpper().StartsWith("SELECT"))
            {
                return InsertHintAfterSelect(processedQuery);
            }

            return processedQuery;
        }

        private string InsertHintAfterWith(string query)
        {
            // WITH ... SELECT 패턴에서 SELECT 위치 찾기
            int selectIndex = FindMainSelectPosition(query);
            
            if (selectIndex > 0)
            {
                return query.Insert(selectIndex + 6, $" {GATHER_PLAN_HINT}"); // "SELECT" 다음에 삽입
            }
            
            return query;
        }

        private string InsertHintAfterSelect(string query)
        {
            // 기존 힌트가 있는지 확인
            int selectIndex = query.ToUpper().IndexOf("SELECT");
            int hintStart = query.IndexOf("/*", selectIndex);
            int hintEnd = query.IndexOf("*/", selectIndex);
            
            if (hintStart > selectIndex && hintStart < query.IndexOf(" ", selectIndex + 6) && hintEnd > hintStart)
            {
                // 기존 힌트 내부에 gather_plan_statistics 추가
                string existingHints = query.Substring(hintStart + 2, hintEnd - hintStart - 2);
                if (!existingHints.Contains("gather_plan_statistics"))
                {
                    string newHints = existingHints.TrimEnd() + " gather_plan_statistics ";
                    return query.Substring(0, hintStart + 2) + newHints + query.Substring(hintEnd);
                }
            }
            else
            {
                // 새로운 힌트 추가
                return query.Insert(selectIndex + 6, $" {GATHER_PLAN_HINT}");
            }
            
            return query;
        }

        private int FindMainSelectPosition(string query)
        {
            // WITH절 내의 SELECT들을 건너뛰고 메인 SELECT 찾기
            string upperQuery = query.ToUpper();
            int currentPos = 0;
            int parenDepth = 0;
            
            while (currentPos < upperQuery.Length)
            {
                int selectPos = upperQuery.IndexOf("SELECT", currentPos);
                if (selectPos == -1) break;
                
                // SELECT 이전의 괄호 깊이 확인
                for (int i = currentPos; i < selectPos; i++)
                {
                    if (upperQuery[i] == '(') parenDepth++;
                    else if (upperQuery[i] == ')') parenDepth--;
                }
                
                // 괄호 깊이가 0이면 메인 SELECT
                if (parenDepth == 0)
                {
                    return selectPos;
                }
                
                currentPos = selectPos + 6;
            }
            
            return -1;
        }
    }
}

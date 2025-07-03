using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

namespace wsDB.OracleExecutionPlan.Core.Parsing
{
    public class DbmsXPlanParser
    {
        private Dictionary<string, string> predicateInfo;

        public DbmsXPlanParser()
        {
            predicateInfo = new Dictionary<string, string>();
        }

        public void ParseExecutionPlan(string content)
        {
            predicateInfo.Clear();
            ParsePredicateInformation(content);
        }

        private void ParsePredicateInformation(string content)
        {
            // 다양한 줄바꿈 문자를 모두 고려하여 분할
            string[] lines = content.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            bool inPredicateSection = false;
            string currentId = null;
            List<string> currentPredicateLines = new List<string>();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("Predicate Information"))
                {
                    inPredicateSection = true;
                    continue;
                }

                if (inPredicateSection)
                {
                    if (trimmedLine.StartsWith("Column Projection Information") ||
                        trimmedLine.StartsWith("Hint Report") ||
                        trimmedLine.StartsWith("Query Block Registry")
                        // ||
                        // string.IsNullOrWhiteSpace(trimmedLine)
                        )
                    {
                        if (currentId != null && currentPredicateLines.Count > 0)
                        {
                            // 결과 저장 시에도 일관된 줄바꿈 사용
                            predicateInfo[currentId] = string.Join(Environment.NewLine, currentPredicateLines);
                        }
                        // if (!string.IsNullOrWhiteSpace(trimmedLine))

                        inPredicateSection = false;
                        break;
                    }

                    Match idMatch = Regex.Match(trimmedLine, @"^(\d+)\s*-\s*(.*)");
                    if (idMatch.Success)
                    {
                        if (currentId != null && currentPredicateLines.Count > 0)
                        {
                            predicateInfo[currentId] = string.Join(Environment.NewLine, currentPredicateLines);
                        }

                        currentId = idMatch.Groups[1].Value;
                        currentPredicateLines = new List<string> { idMatch.Groups[2].Value };
                    }
                    else if (currentId != null)
                    {
                        currentPredicateLines.Add(trimmedLine);
                    }
                }
            }


            if (currentId != null && currentPredicateLines.Count > 0)
            {
                predicateInfo[currentId] = string.Join(Environment.NewLine, currentPredicateLines);
            } 
        }

        public string GetPredicateInfo(string id)
        {
            return predicateInfo.ContainsKey(id) ? predicateInfo[id] : null;
        }
    }
}
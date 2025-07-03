using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using wsDB.OracleExecutionPlan.Core.Enums;
using wsDB.OracleExecutionPlan.Core.Parsing;

namespace wsDB.OracleExecutionPlan.Core.Formatting
{
    public class DbmsXPlanFormatter
    {
        private DbmsXPlanParser planParser;

        public DbmsXPlanFormatter(DbmsXPlanParser parser)
        {
            planParser = parser;
        }

        public void FormatExecutionPlan(RichTextBox rtb, string content)
        {            
            planParser.ParseExecutionPlan(content); 
            FormatExecutionPlanWithSections(rtb, content);
        }

        private void FormatExecutionPlanWithSections(RichTextBox rtb, string text)
        { 
            rtb.Document.Blocks.Clear();
 
            // string[] lines = text.Split('\n');
            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            DbmsXPlanSection currentSection = DbmsXPlanSection.Unknown;

 
            foreach (string line in lines)
            {
                DbmsXPlanSection newSection = DetermineSection(line);
                if (newSection != DbmsXPlanSection.Unknown)
                {
                    currentSection = newSection;
                }

                Paragraph paragraph = new Paragraph();
                paragraph.Margin = new Thickness(0);

                ApplySectionFormatting(paragraph, line, currentSection);
                rtb.Document.Blocks.Add(paragraph);
            } 
        }

        private DbmsXPlanSection DetermineSection(string line)
        {
            line = line.Trim();
            
            if (line.StartsWith("Plan hash value:"))
                 return DbmsXPlanSection.ExecutionPlan;
            // if (line.StartsWith("Plan hash value:"))
            //     return ExecutionPlanSection.PlanHash;
            else if (line.StartsWith("Predicate Information"))
                return DbmsXPlanSection.PredicateInformation;
            else if (line.StartsWith("Column Projection Information"))
                return DbmsXPlanSection.ColumnProjection;
            else if (line.StartsWith("Hint Report"))
                return DbmsXPlanSection.HintReport;
            // else if (Regex.IsMatch(line, @"^\s*\|\s*Id\s*\|"))
            //     return ExecutionPlanSection.ExecutionPlan;

            return DbmsXPlanSection.Unknown;
        }

        private void ApplySectionFormatting(Paragraph paragraph, string line, DbmsXPlanSection section)
        {
            switch (section)
            {
                // case ExecutionPlanSection.PlanHash:
                //     FormatPlanHashSection(paragraph, line);
                //     break;
                case DbmsXPlanSection.ExecutionPlan:
                    FormatExecutionPlanSection(paragraph, line);
                    break;
                case DbmsXPlanSection.PredicateInformation:
                    FormatPredicateSection(paragraph, line);
                    break;
                case DbmsXPlanSection.ColumnProjection:
                    FormatColumnProjectionSection(paragraph, line);
                    break;
                case DbmsXPlanSection.HintReport:
                    FormatHintReportSection(paragraph, line);
                    break;
                default:
                    FormatDefaultSection(paragraph, line);
                    break;
            }
        }

        private void FormatExecutionPlanSection(Paragraph paragraph, string line)
        {
            Match idMatch = Regex.Match(line, @"^\s*\|\s*\*?\s*(\d+)\s*\|");
            if (idMatch.Success)
            {
                string id = idMatch.Groups[1].Value;
                FormatExecutionPlanLineWithId(paragraph, line, id);
            }
            else
            {
                ProcessLineWithObjects(paragraph, line);
            }
        }

        private void ProcessLineWithObjects(Paragraph paragraph, string line)
        {
            string tablePattern = @"\b[A-Z_][A-Z0-9_]*\.[A-Z_][A-Z0-9_]*\b|\b[A-Z_][A-Z0-9_]*\b(?=\s+\(TABLE\)|\s+\(INDEX\)|$)";
            string indexPattern = @"\b[A-Z_][A-Z0-9_]*\b(?=\s+\(INDEX\)|(?<=INDEX\s+)[A-Z_][A-Z0-9_]*)";
            string predicatePattern = @"\*(\d+)";  // *숫자 패턴 (예: *1, *2, *10)

            var matches = new List<(int Start, int Length, string Type, string Text)>();

            // 테이블 매칭
            foreach (Match match in Regex.Matches(line, tablePattern))
            {
                matches.Add((match.Index, match.Length, "TABLE", match.Value));
            }

            // 인덱스 매칭
            foreach (Match match in Regex.Matches(line, indexPattern))
            {
                matches.Add((match.Index, match.Length, "INDEX", match.Value));
            }

            // Predicate ID 매칭
            foreach (Match match in Regex.Matches(line, predicatePattern))
            {
                matches.Add((match.Index, match.Length, "PREDICATE", match.Value));
            }

            // 위치별로 정렬
            matches = matches.OrderBy(m => m.Start).ToList();

            int currentPos = 0;

            foreach (var match in matches)
            {
                // 매칭 이전의 일반 텍스트 추가
                if (match.Start > currentPos)
                {
                    paragraph.Inlines.Add(new Run(line.Substring(currentPos, match.Start - currentPos)));
                }

                // 매칭된 객체 강조 처리
                Run objectRun = new Run(match.Text);
                
                switch (match.Type)
                {
                    case "TABLE":
                        objectRun.FontWeight = FontWeights.Bold;
                        objectRun.Foreground = Brushes.DarkBlue;
                        objectRun.Cursor = Cursors.Hand;
                        objectRun.Tag = $"TABLE:{match.Text}";
                        break;
                        
                    case "INDEX":
                        objectRun.FontWeight = FontWeights.Bold;
                        objectRun.Foreground = Brushes.DarkGreen;
                        objectRun.Cursor = Cursors.Hand;
                        objectRun.Tag = $"INDEX:{match.Text}";
                        break;
                        
                    case "PREDICATE":
                        objectRun.FontWeight = FontWeights.Bold;
                        objectRun.Foreground = Brushes.DarkRed;
                        objectRun.Cursor = Cursors.Hand;
                        objectRun.Tag = $"PREDICATE:{match.Text}";
                        objectRun.TextDecorations = TextDecorations.Underline; // 밑줄 추가
                        break;
                }

                paragraph.Inlines.Add(objectRun);
                currentPos = match.Start + match.Length;
            }

            // 남은 텍스트 추가
            if (currentPos < line.Length)
            {
                paragraph.Inlines.Add(new Run(line.Substring(currentPos)));
            }
        }

        private void FormatExecutionPlanLineWithId(Paragraph paragraph, string line, string id)
        {
            string beforeId = line.Substring(0, line.IndexOf(id));
            string afterId = line.Substring(line.IndexOf(id) + id.Length);

            if (!string.IsNullOrEmpty(beforeId))
            {
                paragraph.Inlines.Add(new Run(beforeId));
            }

            Run idRun = new Run(id)
            {
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Blue,
                Background = new SolidColorBrush(Color.FromArgb(50, 173, 216, 230)),
                Cursor = Cursors.Help,
                Tag = $"ID:{id}"
            };

            paragraph.Inlines.Add(idRun);
            ProcessLineWithObjects(paragraph, afterId);
        }

        private void FormatPredicateSection(Paragraph paragraph, string line)
        {
            Run run = new Run(line.TrimEnd('\r'));

            if (Regex.IsMatch(line, @"^\s*\d+\s*-"))
            {
                run.Foreground = Brushes.DarkGreen;
                run.FontWeight = FontWeights.Bold;
            }
            else if (line.Trim().StartsWith("Predicate Information"))
            {
                run.Foreground = Brushes.DarkRed;
                run.FontWeight = FontWeights.Bold;
                run.FontSize = 12;
            }
            else
            {
                run.Foreground = Brushes.DarkGreen;
            }

            paragraph.Inlines.Add(run);
        }

        private void FormatColumnProjectionSection(Paragraph paragraph, string line)
        {
            Run run = new Run(line.TrimEnd('\r'));

            if (line.Trim().StartsWith("Column Projection Information"))
            {
                run.Foreground = Brushes.DarkBlue;
                run.FontWeight = FontWeights.Bold;
                run.FontSize = 12;
            }
            else if (Regex.IsMatch(line, @"^\s*\d+\s*-"))
            {
                run.Foreground = Brushes.Blue;
                run.FontWeight = FontWeights.Bold;
            }
            else
            {
                run.Foreground = Brushes.DarkSlateBlue;
            }

            paragraph.Inlines.Add(run);
        }

        private void FormatHintReportSection(Paragraph paragraph, string line)
        {
            Run run = new Run(line.TrimEnd('\r'));

            if (line.Trim().StartsWith("Hint Report"))
            {
                run.Foreground = Brushes.DarkOrange;
                run.FontWeight = FontWeights.Bold;
                run.FontSize = 12;
            }
            else if (Regex.IsMatch(line, @"^\s*\d+\s*-"))
            {
                run.Foreground = Brushes.Orange;
                run.FontWeight = FontWeights.Bold;
            }
            else
            {
                run.Foreground = Brushes.Chocolate;
            }

            paragraph.Inlines.Add(run);
        }

        private void FormatDefaultSection(Paragraph paragraph, string line)
        {
            ProcessLineWithObjects(paragraph, line);
        }
    }
}

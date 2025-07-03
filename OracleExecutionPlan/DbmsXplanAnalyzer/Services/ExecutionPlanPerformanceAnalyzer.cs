using System.Text.RegularExpressions;
using wsDB.OracleExecutionPlan.Core.Parsing;
using wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Models;

namespace wsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Services
{
    public class ExecutionPlanPerformanceAnalyzer
    {
        private readonly List<ExecutionPlanStep> _steps = new List<ExecutionPlanStep>();
        private readonly List<PerformanceIssue> _issues = new List<PerformanceIssue>();
        private List<string> _columnHeaders = new List<string>();
        private List<int> _columnPositions = new List<int>();

        //  private Dictionary<int, string> _predicateInfo = new Dictionary<int, string>(); // ID별 Predicate 정보

        private DbmsXPlanParser _xplanParser;

         public ExecutionPlanPerformanceAnalyzer(DbmsXPlanParser parser = null)
        {
            _xplanParser = parser;
        }

        // 또는 파서를 설정하는 메서드 추가
        public void SetParser(DbmsXPlanParser parser)
        {
            _xplanParser = parser;
        }

        public PerformanceAnalysisResult AnalyzeExecutionPlan(string executionPlan)
        {
            _steps.Clear();
            _issues.Clear();
            _columnHeaders.Clear();
            _columnPositions.Clear();

            // 파서가 설정되어 있다면 파싱 수행 (이미 DbmsXPlanFormatter에서 했을 수도 있음)
            if (_xplanParser != null)
            {
                _xplanParser.ParseExecutionPlan(executionPlan);
            }

            // 1. 실행계획 파싱
            ParseExecutionPlan(executionPlan);

            // 2. 성능 메트릭 계산
            CalculatePerformanceMetrics();

            // 3. 성능 이슈 분석
            AnalyzePerformanceIssues();

            // 4. 결과 생성
            return GenerateAnalysisResult();
        }

        private void ParseExecutionPlan(string executionPlan)
        {
            var lines = executionPlan.Split('\n');
            bool inDataSection = false;            
            bool inPredicateSection = false;
            string headerLine = null;
            
            int lineNumber = 0;

            foreach (var line in lines)
            {
                lineNumber++;
                // System.Diagnostics.Debug.WriteLine($"Line {lineNumber}: {line}");

                // 헤더 라인 찾기
                if (line.Contains("| Id") && line.Contains("| Operation"))
                {
                    headerLine = line;
                    ParseColumnHeaders(headerLine);
                    inDataSection = true;
                    // System.Diagnostics.Debug.WriteLine($"헤더 라인 발견: {line}");
                    continue;
                }

                // Predicate Information 섹션 시작
                if (line.StartsWith("Predicate Information"))
                {
                    inDataSection = false;
                    inPredicateSection = true;
                    continue;
                }

                // Predicate 정보 파싱
                // if (inPredicateSection)
                // {
                //     ParsePredicateInformation(line);
                // }

                // 구분선 건너뛰기
                if (line.StartsWith("----") || line.StartsWith("==="))
                    continue;

                if (inDataSection && line.StartsWith("|") && !line.Contains("Id") && !line.Contains("Operation"))
                {
                    //  System.Diagnostics.Debug.WriteLine($"데이터 라인 파싱 시도: {line}");
                    var step = ParseExecutionPlanLineEfficient(line);
                    if (step != null)
                    {
                        // System.Diagnostics.Debug.WriteLine($"파싱된 Step: ID={step.Id}, Operation={step.Operation}");
                        _steps.Add(step);
                    }
                    // else
                    // {
                    //     System.Diagnostics.Debug.WriteLine($"파싱 실패한 라인: {line}");
                    // }
                }

                // Column Projection이나 다른 섹션 시작 시 Predicate 섹션 종료
                if (inPredicateSection && (line.StartsWith("Column Projection") ||
                                          line.StartsWith("Hint Report") ||
                                          line.Trim() == ""))
                {
                    inPredicateSection = false;
                }
            }

            // Predicate 정보를 해당 단계에 매핑
            MapPredicateInfoToSteps();
        }
        
        /// <summary>
        /// Predicate Information 섹션 파싱
        /// </summary>
        // private void ParsePredicateInformation(string line)
        // {
        //     if (string.IsNullOrWhiteSpace(line)) return;

        //     // "   1 - filter(COLUMN_NAME='VALUE')" 형태 파싱
        //     var predicateMatch = Regex.Match(line.Trim(), @"^(\d+)\s*-\s*(.+)$");
        //     if (predicateMatch.Success)
        //     {
        //         int id = int.Parse(predicateMatch.Groups[1].Value);
        //         string predicateText = predicateMatch.Groups[2].Value;
                
        //         if (_predicateInfo.ContainsKey(id))
        //         {
        //             _predicateInfo[id] += "\n" + predicateText;
        //         }
        //         else
        //         {
        //             _predicateInfo[id] = predicateText;
        //         }
        //     }
        // }

        /// <summary>
        /// Predicate 정보를 해당 단계에 매핑
        /// </summary>
        private void MapPredicateInfoToSteps()
        {
            // foreach (var step in _steps)
            // {
            //     if (_predicateInfo.TryGetValue(step.Id, out string predicateInfo))
            //     {
            //         step.PredicateInfo = predicateInfo;
            //     }
            // }
            // 파서가 있을 때만 Predicate 정보 매핑
            if (_xplanParser != null)
            {
                foreach (var step in _steps)
                {
                    string predicateInfo = _xplanParser.GetPredicateInfo(step.Id.ToString());
                    if (!string.IsNullOrEmpty(predicateInfo))
                    {
                        step.PredicateInfo = predicateInfo;
                    }
                }
            }
        }

        /// <summary>
        /// 효율적인 실행계획 라인 파싱 (Predicate 지원)
        /// </summary>
        private ExecutionPlanStep ParseExecutionPlanLineEfficient(string line)
        {
            try
            {
                // "|" 기준으로 분할
                var columns = line.Split('|');

                // System.Diagnostics.Debug.WriteLine($"분할된 컬럼 개수: {columns.Length}");
                if (columns.Length < 3) // 최소한 Id, Operation은 있어야 함
                    return null;

                var step = new ExecutionPlanStep();

                // 각 컬럼별로 값 추출
               for (int i = 0; i < _columnHeaders.Count && i + 1 < columns.Length; i++)
                {
                    var columnName = _columnHeaders[i];
                    var columnValue = columns[i + 1].Trim(); // 인덱스를 1 증가시켜서 매핑

                    // System.Diagnostics.Debug.WriteLine($"매핑: {columnName} = '{columnValue}'");
                    MapColumnValue(step, columnName, columnValue);
                }


                // System.Diagnostics.Debug.WriteLine($"파싱 결과: ID={step.Id}, HasPredicate={step.HasPredicate}");
                return step;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"파싱 오류: {ex.Message}, Line: {line}");
                return null;
            }
        }

        /// <summary>
        /// 컬럼명에 따라 ExecutionPlanStep 객체에 값 매핑 (Predicate 지원)
        /// </summary>
        private void MapColumnValue(ExecutionPlanStep step, string columnName, string columnValue)
        {
            switch (columnName.ToUpperInvariant())
            {
                case "ID":
                    // System.Diagnostics.Debug.WriteLine($"ID 파싱 시도: '{columnValue}'");
                    // "*1" 같은 형태에서 "*" 여부 확인 후 숫자 추출
                    step.HasPredicate = columnValue.StartsWith("*");

                    // 숫자 추출 - 여러 패턴 시도
                    var idPatterns = new[]
                    {
                        @"\*\s+(\d+)",           // "*   1" 형태 (별표 + 하나 이상의 공백 + 숫자)
                        @"\*\s*(\d+)",           // "*1" 또는 "* 1" 형태 (별표 + 0개 이상의 공백 + 숫자)
                        @"\|\s*(\d+)",           // "|   1" 형태 (파이프 + 공백들 + 숫자)
                        @"^\s*(\d+)",            // 라인 시작부터 공백들 후 숫자
                        @"(\d+)",                // 단순 숫자
                    };

                    foreach (var pattern in idPatterns)
                    {
                        var idMatch = Regex.Match(columnValue, pattern);
                        if (idMatch.Success && idMatch.Groups.Count > 1)
                        {
                            if (int.TryParse(idMatch.Groups[1].Value, out int id))
                            {
                                step.Id = id;
                                // System.Diagnostics.Debug.WriteLine($"ID 파싱 성공: {id} (패턴: {pattern})");
                                break;
                            }
                        }
                    }
                    
                    // if (step.Id == 0)
                    // {
                    //     System.Diagnostics.Debug.WriteLine($"ID 파싱 실패: '{columnValue}'");
                    // }
                    break;

                case "OPERATION":
                    step.Operation = columnValue;
                    step.Level = CountLeadingSpaces(columnValue);
                    break;

                case "NAME":
                    step.Name = columnValue;
                    break;

                case "STARTS":
                    step.Starts = ParseLongValue(columnValue);
                    break;

                case "E-ROWS":
                    step.EstimatedRows = ParseLongValue(columnValue);
                    break;

                case "COST":
                case "COST (%CPU)":
                    step.Cost = ParseLongValue(columnValue);
                    if (columnValue.Contains("(") && columnValue.Contains("%"))
                    {
                        var cpuMatch = Regex.Match(columnValue, @"\((\d+)%");
                        if (cpuMatch.Success)
                        {
                            step.CpuPercent = double.Parse(cpuMatch.Groups[1].Value);
                        }
                    }
                    break;

                case "E-TIME":
                    step.EstimatedTime = columnValue;
                    break;

                case "A-ROWS":
                    step.ActualRows = ParseLongValue(columnValue);
                    break;

                case "A-TIME":
                    step.ActualTime = columnValue;
                    break;

                case "BUFFERS":
                    step.Buffers = ParseLongValue(columnValue);
                    break;

                case "READS":
                    step.Reads = ParseLongValue(columnValue);
                    break;

                case "OMEM":
                    step.OMem = columnValue;
                    break;

                case "1MEM":
                    step.OneMem = columnValue;
                    break;

                case "USED-MEM":
                    step.UsedMem = columnValue;
                    break;

                default:
                    // 알 수 없는 컬럼은 무시
                    break;
            }
        }


        /// <summary>
        /// 성능 이슈 분석 (Predicate 고려)
        /// </summary>
        private void AnalyzeStepPerformance(ExecutionPlanStep step)
        {
            var severity = PerformanceSeverity.Low;

            // 1. 높은 Buffer 읽기 분석
            if (step.BufferToRowRatio > 1000)
            {
                severity = severity.CombineWith(PerformanceSeverity.High);
                _issues.Add(new PerformanceIssue
                {
                    StepId = step.Id,
                    Type = PerformanceIssueType.HighBufferReads,
                    Severity = PerformanceSeverity.High,
                    Description = $"Step {step.Id}: 높은 Buffer 읽기 비율 ({step.BufferToRowRatio:F1} buffers/row)",
                    Recommendation = "인덱스 효율성을 검토하거나 조건절을 최적화하세요."
                });
            }

            // 2. Predicate가 있는 단계에서 성능 이슈 추가 분석
            if (step.HasPredicate && step.ActualTimeSeconds > 1)
            {
                severity = severity.CombineWith(PerformanceSeverity.Medium);
                _issues.Add(new PerformanceIssue
                {
                    StepId = step.Id,
                    Type = PerformanceIssueType.IndexIssue,
                    Severity = PerformanceSeverity.Medium,
                    Description = $"Step {step.Id}: Predicate가 있는 단계에서 긴 실행시간 ({step.ActualTimeSeconds:F2}초)",
                    Recommendation = $"조건절을 최적화하세요: {step.PredicateInfo ?? "Predicate 정보 확인 필요"}"
                });
            }

            // 3. 높은 Physical 읽기 분석
            if (step.Reads > 1000)
            {
                severity = severity.CombineWith(PerformanceSeverity.Medium);
                _issues.Add(new PerformanceIssue
                {
                    StepId = step.Id,
                    Type = PerformanceIssueType.HighPhysicalReads,
                    Severity = PerformanceSeverity.Medium,
                    Description = $"Step {step.Id}: 높은 Physical 읽기 ({step.Reads:N0})",
                    Recommendation = "데이터가 메모리에 캐시되도록 버퍼 풀 크기를 검토하세요."
                });
            }

            // 4. Cardinality 불일치 분석
            if (step.EstimateVsActualRatio > 10 || step.EstimateVsActualRatio < 0.1)
            {
                severity = severity.CombineWith(PerformanceSeverity.Medium);
                _issues.Add(new PerformanceIssue
                {
                    StepId = step.Id,
                    Type = PerformanceIssueType.CardinalityMismatch,
                    Severity = PerformanceSeverity.Medium,
                    Description = $"Step {step.Id}: 예상({step.EstimatedRows:N0}) vs 실제({step.ActualRows:N0}) 행 수 불일치",
                    Recommendation = "통계 정보를 업데이트하거나 히스토그램을 생성하세요."
                });
            }

            // 5. 긴 실행 시간 분석
            if (step.ActualTimeSeconds > 10)
            {
                var timeSeverity = step.ActualTimeSeconds > 60 ? PerformanceSeverity.Critical : PerformanceSeverity.High;
                severity = severity.CombineWith(timeSeverity);
                _issues.Add(new PerformanceIssue
                {
                    StepId = step.Id,
                    Type = PerformanceIssueType.LongExecutionTime,
                    Severity = timeSeverity,
                    Description = $"Step {step.Id}: 긴 실행 시간 ({step.ActualTimeSeconds:F2}초)",
                    Recommendation = "이 단계의 실행 계획을 최적화하세요."
                });
            }

            // 6. Full Table Scan 분석
            if (step.Operation.Contains("TABLE ACCESS STORAGE FULL") || step.Operation.Contains("TABLE ACCESS FULL"))
            {
                var scanSeverity = step.ActualRows > 100000 ? PerformanceSeverity.High : PerformanceSeverity.Medium;
                severity = severity.CombineWith(scanSeverity);
                _issues.Add(new PerformanceIssue
                {
                    StepId = step.Id,
                    Type = PerformanceIssueType.FullTableScan,
                    Severity = scanSeverity,
                    Description = $"Step {step.Id}: Full Table Scan on {step.Name}",
                    Recommendation = "적절한 인덱스 생성을 고려하세요."
                });
            }

            step.Severity = severity;
        }

        /// <summary>
        /// 컬럼 헤더 파싱 및 위치 정보 저장
        /// </summary>
        private void ParseColumnHeaders(string headerLine)
        {
            _columnHeaders.Clear();
            _columnPositions.Clear();

            var columns = headerLine.Split('|');

            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i].Trim();
                if (!string.IsNullOrEmpty(column))
                {
                    _columnHeaders.Add(column);
                    _columnPositions.Add(i);
                    // System.Diagnostics.Debug.WriteLine($"컬럼 {i}: '{column}'");
                }
            }

            // 디버그용 출력
            // System.Diagnostics.Debug.WriteLine($"발견된 컬럼: {string.Join(", ", _columnHeaders)}");
        }

        /// <summary>
        /// 문자열에서 숫자 값 추출 (K, M, G 단위 지원)
        /// </summary>
        private long ParseLongValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            try
            {
                // 숫자와 단위 분리
                var match = Regex.Match(value, @"(\d+(?:\.\d+)?)\s*([KMG])?", RegexOptions.IgnoreCase);
                if (!match.Success)
                    return 0;

                var numberPart = match.Groups[1].Value;
                var unitPart = match.Groups[2].Value.ToUpperInvariant();

                if (!double.TryParse(numberPart, out double number))
                    return 0;

                // 단위 변환
                return unitPart switch
                {
                    "K" => (long)(number * 1024),
                    "M" => (long)(number * 1024 * 1024),
                    "G" => (long)(number * 1024 * 1024 * 1024),
                    _ => (long)number
                };
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Operation 컬럼의 들여쓰기 레벨 계산
        /// </summary>
        private int CountLeadingSpaces(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            int count = 0;
            foreach (char c in value)
            {
                if (c == ' ')
                    count++;
                else
                    break;
            }
            return count;
        }

        /// <summary>
        /// 시간 문자열을 초로 변환 (00:48:03.77 형식)
        /// </summary>
        private double ConvertTimeToSeconds(string timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr))
                return 0;

            try
            {
                // 00:48:03.77 형식 파싱
                var pattern = @"(\d+):(\d+):(\d+)\.(\d+)";
                var match = Regex.Match(timeStr, pattern);
                
                if (match.Success)
                {
                    var hours = int.Parse(match.Groups[1].Value);
                    var minutes = int.Parse(match.Groups[2].Value);
                    var seconds = int.Parse(match.Groups[3].Value);
                    var centiseconds = int.Parse(match.Groups[4].Value);
                    
                    return hours * 3600 + minutes * 60 + seconds + centiseconds / 100.0;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        // 나머지 메서드들은 기존과 동일...
        private void CalculatePerformanceMetrics()
        {
            foreach (var step in _steps)
            {
                // 실행 시간을 초로 변환
                step.ActualTimeSeconds = ConvertTimeToSeconds(step.ActualTime);

                // Buffer 대 Row 비율
                if (step.ActualRows > 0 && step.Buffers > 0)
                {
                    step.BufferToRowRatio = (double)step.Buffers / step.ActualRows;
                }

                // 예상 vs 실제 행 수 비율
                if (step.EstimatedRows > 0 && step.ActualRows > 0)
                {
                    step.EstimateVsActualRatio = (double)step.ActualRows / step.EstimatedRows;
                }
            }
        }

        private void AnalyzePerformanceIssues()
        {
            foreach (var step in _steps)
            {
                AnalyzeStepPerformance(step);
            }
        }

        private PerformanceAnalysisResult GenerateAnalysisResult()
        {
            var summary = new PerformanceSummary
            {
                TotalExecutionTimeSeconds = _steps.Sum(s => s.ActualTimeSeconds),
                TotalBufferReads = _steps.Sum(s => s.Buffers),
                TotalPhysicalReads = _steps.Sum(s => s.Reads),
                TotalActualRows = _steps.Sum(s => s.ActualRows),
                TotalSteps = _steps.Count,
                HighImpactSteps = _steps.Count(s => s.Severity >= PerformanceSeverity.High),
                CriticalIssues = _issues.Count(i => i.Severity == PerformanceSeverity.Critical),
                HighIssues = _issues.Count(i => i.Severity == PerformanceSeverity.High)
            };

            var mostExpensive = _steps.OrderByDescending(s => s.ActualTimeSeconds).FirstOrDefault();
            if (mostExpensive != null)
            {
                summary.MostExpensiveOperation = mostExpensive.Operation;
                summary.MostExpensiveStepId = mostExpensive.Id;
            }

            return new PerformanceAnalysisResult
            {
                Steps = _steps,
                Issues = _issues,
                Summary = summary,
                Recommendations = GenerateRecommendations()
            };
        }

        private List<string> GenerateRecommendations()
        {
            var recommendations = new List<string>();

            recommendations.Add($"• 총 {_steps.Count}개 단계 분석 완료");
            
            if (_issues.Any(i => i.Type == PerformanceIssueType.FullTableScan))
            {
                recommendations.Add("• Full Table Scan이 발견되었습니다. 적절한 인덱스 생성을 고려하세요.");
            }

            if (_issues.Any(i => i.Type == PerformanceIssueType.CardinalityMismatch))
            {
                recommendations.Add("• 통계 정보 불일치가 발견되었습니다. ANALYZE TABLE 또는 GATHER_TABLE_STATS를 실행하세요.");
            }

            if (_issues.Any(i => i.Type == PerformanceIssueType.HighBufferReads))
            {
                recommendations.Add("• 높은 Buffer 읽기가 발견되었습니다. 인덱스 효율성을 검토하세요.");
            }

            return recommendations;
        }
    }
}



// 3. 실행계획 서비스
using System.Windows;
using Oracle.ManagedDataAccess.Client;
using wsDB.Common.BindVar.Models;
using wsDB.Common.BindVar.Services;
using wsDB.Common.BindVar.Views;
using wsDB.OracleQueryAnalyzer.Models;

namespace wsDB.OracleQueryAnalyzer.Services
{
    public class ExecutionPlanService
    {
        private readonly OracleConnection _connection;
        private readonly Window _ownerWindow;

        public ExecutionPlanService(OracleConnection connection, Window ownerWindow = null)
        {
            _connection = connection;
            _ownerWindow = ownerWindow;
        }

        public async Task<ExecutionPlanResult> AnalyzeExecutionPlanAsync(string query)
        {
            // try
            // {
            //     // 1단계: 통계 수집을 위한 쿼리 실행 (데이터는 읽지 않음)
            //     await ExecuteQueryForStatistics(query);

            //     // 2단계: 실행계획 조회
            //     string executionPlan = await GetExecutionPlan();

            //     return new ExecutionPlanResult(true, executionPlan, null);
            // }
            // catch (Exception ex)
            // {
            //     return new ExecutionPlanResult(false, null, ex.Message);
            // }
            try
            {
                // 1단계: 바인드 변수 확인
                var bindVariables = QueryBindVariableExtractor.ExtractBindVariables(query);
                List<OracleBindVariable> bindVariableValues = null;

                if (bindVariables.Count > 0)
                {
                    var (success, variables) = BindVariableWindow.ShowBindVariableDialog(
                    bindVariables, GetOwnerWindow());
                    
                    if (!success)
                    {
                        return new ExecutionPlanResult(false, null, "사용자가 바인드 변수 입력을 취소했습니다.");
                    }
                    
                    bindVariableValues = variables;
                }

                // 2단계: 통계 수집을 위한 쿼리 실행 (데이터는 읽지 않음)
                await ExecuteQueryForStatistics(query, bindVariableValues);

                // 3단계: 실행계획 조회
                string executionPlan = await GetExecutionPlan();

                return new ExecutionPlanResult(true, executionPlan, null);
            }
            catch (Exception ex)
            {
                return new ExecutionPlanResult(false, null, ex.Message);
            }
        }

        private async Task ExecuteQueryForStatistics(string query, List<OracleBindVariable> bindVariables = null)
        {
            using (var command = new OracleCommand(query, _connection))
            {
                command.CommandTimeout = 300; // 5분 타임아웃

                // 바인드 변수 설정
                if (bindVariables != null)
                {
                    // foreach (var bindVar in bindVariables)
                    // {
                    //     var parameter = new OracleParameter($":{bindVar.Name}", GetOracleDbType(bindVar.Type));
                    //     parameter.Value = ConvertValue(bindVar.Value, bindVar.Type);
                    //     command.Parameters.Add(parameter);
                    // }
                    BindVariableProcessor.AddParametersToCommand(command, bindVariables);
                }

                // 기존 코드 유지...
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int fieldCount = reader.FieldCount;
                        for (int i = 0; i < fieldCount; i++)
                        {
                            // 데이터를 읽지 않고 단순히 다음 행으로 이동
                        }
                    }
                }
            }
        }

        private async Task ExecuteQueryForStatistics(string query)
        {
            using (var command = new OracleCommand(query, _connection))
            {
                command.CommandTimeout = 300; // 5분 타임아웃

                using (var reader = await command.ExecuteReaderAsync())
                {
                    // 데이터를 실제로 읽지 않고 첫 번째 행만 확인하여 실행계획 생성
                    while (await reader.ReadAsync())
                    {
                        // 빠른 처리를 위해 첫 번째 행에서 중단
                        // 하지만 실제로는 모든 데이터를 처리해야 정확한 통계를 얻을 수 있음

                        // 실제 구현에서는 아래와 같이 모든 행을 처리하되 데이터는 읽지 않음
                        int fieldCount = reader.FieldCount;
                        for (int i = 0; i < fieldCount; i++)
                        {
                            // 데이터를 읽지 않고 단순히 다음 행으로 이동
                            // reader.GetValue(i); // 실제 값은 읽지 않음
                        }
                    }
                }
            }
        }

        private async Task<string> GetExecutionPlan()
        {
            const string planQuery = @"
                SELECT plan_table_output 
                FROM TABLE(DBMS_XPLAN.DISPLAY_CURSOR(null, null, 'ALLSTATS LAST'))";

            using (var command = new OracleCommand(planQuery, _connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var planLines = new System.Text.StringBuilder();

                    while (await reader.ReadAsync())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            planLines.AppendLine(reader.GetString(0));
                        }
                    }

                    return planLines.ToString();
                }
            }
        }

        private Window GetOwnerWindow()
        {
            // 생성자에서 전달받은 Owner가 있으면 우선 사용
            if (_ownerWindow != null && _ownerWindow.IsVisible)
            {
                return _ownerWindow;
            }

            // ExecutionAnalyzerWindow 찾기
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType().Name == "ExecutionAnalyzerWindow" && window.IsVisible)
                {
                    return window;
                }
            }

            return Application.Current.MainWindow;
        }
    }
}
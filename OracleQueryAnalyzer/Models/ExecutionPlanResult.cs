namespace wsDB.OracleQueryAnalyzer.Models
{ 
    public class ExecutionPlanResult
    {
        public bool Success { get; }
        public string ExecutionPlan { get; }
        public string ErrorMessage { get; }

        public ExecutionPlanResult(bool success, string executionPlan, string errorMessage)
        {
            Success = success;
            ExecutionPlan = executionPlan;
            ErrorMessage = errorMessage;
        }
    }
}
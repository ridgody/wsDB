namespace wsDB.OracleQueryAnalyzer.Models
{
    public class QueryValidationResult
    {
        public bool IsValid { get; }
        public string ErrorMessage { get; }

        public QueryValidationResult(bool isValid, string errorMessage = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}
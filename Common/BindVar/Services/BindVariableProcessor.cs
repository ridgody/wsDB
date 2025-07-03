// ========== Services/BindVariableProcessor.cs ==========
using Oracle.ManagedDataAccess.Client; 
using System.Windows;
using wsDB.Common.BindVar.Models;
using wsDB.Common.BindVar.Views;

namespace wsDB.Common.BindVar.Services
{
    public class BindVariableProcessor
    {
        
        /// <summary>
        /// OracleCommand에 바인드 변수 파라미터를 추가합니다.
        /// </summary>
        /// <param name="command">Oracle 명령 객체</param>
        /// <param name="bindVariables">바인드 변수 목록</param>
        public static void AddParametersToCommand(OracleCommand command, List<OracleBindVariable> bindVariables)
        {
            if (bindVariables == null) return;

            foreach (var bindVar in bindVariables)
            {
                var parameter = new OracleParameter($":{bindVar.Name}", GetOracleDbType(bindVar.Type))
                {
                    Value = ConvertValue(bindVar.Value, bindVar.Type)
                };
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// 바인드 변수 타입을 Oracle 타입으로 변환합니다.
        /// </summary>
        /// <param name="type">바인드 변수 타입</param>
        /// <returns>Oracle DB 타입</returns>
        public static OracleDbType GetOracleDbType(BindVariableType type)
        {
            return type switch
            {
                BindVariableType.Varchar2 => OracleDbType.Varchar2,
                BindVariableType.Number => OracleDbType.Decimal,
                BindVariableType.Date => OracleDbType.Date,
                _ => OracleDbType.Varchar2
            };
        }

        /// <summary>
        /// 문자열 값을 적절한 타입으로 변환합니다.
        /// </summary>
        /// <param name="value">변환할 값</param>
        /// <param name="type">대상 타입</param>
        /// <returns>변환된 값</returns>
        public static object ConvertValue(string value, BindVariableType type)
        {
            if (string.IsNullOrEmpty(value))
                return DBNull.Value;

            try
            {
                return type switch
                {
                    BindVariableType.Varchar2 => value,
                    BindVariableType.Number => decimal.Parse(value),
                    BindVariableType.Date => DateTime.Parse(value),
                    _ => value
                };
            }
            catch (FormatException)
            {
                // 변환 실패 시 문자열로 처리
                return value;
            }
        }

        /// <summary>
        /// 바인드 변수의 유효성을 검사합니다.
        /// </summary>
        /// <param name="bindVariables">검사할 바인드 변수 목록</param>
        /// <returns>유효성 검사 결과</returns>
        public static (bool IsValid, string ErrorMessage) ValidateBindVariables(List<OracleBindVariable> bindVariables)
        {
            if (bindVariables == null || bindVariables.Count == 0)
                return (true, null);

            foreach (var bindVar in bindVariables)
            {
                if (string.IsNullOrWhiteSpace(bindVar.Name))
                    return (false, "바인드 변수 이름이 비어있습니다.");

                if (string.IsNullOrWhiteSpace(bindVar.Value))
                    continue; // 빈 값은 허용 (NULL로 처리)

                // 타입별 유효성 검사
                switch (bindVar.Type)
                {
                    case BindVariableType.Number:
                        if (!decimal.TryParse(bindVar.Value, out _))
                            return (false, $"'{bindVar.Name}' 변수의 값이 올바른 숫자 형식이 아닙니다.");
                        break;

                    case BindVariableType.Date:
                        if (!DateTime.TryParse(bindVar.Value, out _))
                            return (false, $"'{bindVar.Name}' 변수의 값이 올바른 날짜 형식이 아닙니다.");
                        break;
                }
            }

            return (true, null);
        }
    }
}
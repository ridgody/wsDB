using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using wsDB.Common;

namespace wsDB.OracleExecutionPlanRepository.Models
{
    public class ExecutionPlanRecord : ObservableObject
    {
        private string _sqlId;
        private string _executionLocation;
        private string _query;
        private string _bindVariables;
        private string _executionPlan;
        private string _analysisInfo;
        private DateTime _createdDate;
        private DateTime _lastAccessDate;
        private string _notes;

        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 저장 단계 (QueryAnalysis, ExecutionPlan, PerformanceAnalysis)
        /// </summary>
        public string SaveStage { get; set; } = "";

        /// <summary>
        /// 마지막 업데이트 단계
        /// </summary>
        public string LastUpdateStage { get; set; } = "";

        /// <summary>
        /// 각 필드가 설정되었는지 확인하는 헬퍼 속성들
        /// </summary>
        public bool HasQuery => !string.IsNullOrWhiteSpace(Query);
        public bool HasExecutionPlan => !string.IsNullOrWhiteSpace(ExecutionPlan);
        public bool HasAnalysisInfo => !string.IsNullOrWhiteSpace(AnalysisInfo);

        /// <summary>
        /// 저장 가능한 단계인지 확인
        /// </summary>
        /// <param name="stage">확인할 단계</param>
        /// <returns>저장 가능하면 true</returns>
        public bool CanSaveAtStage(string stage)
        {
            switch (stage.ToLower())
            {
                case "queryanalysis":
                    return !string.IsNullOrWhiteSpace(SqlId) &&
                           !string.IsNullOrWhiteSpace(ExecutionLocation) &&
                           !string.IsNullOrWhiteSpace(Query);
                case "executionplan":
                    return CanSaveAtStage("queryanalysis") && HasExecutionPlan;
                case "performanceanalysis":
                    return CanSaveAtStage("executionplan") && HasAnalysisInfo;
                default:
                    return false;
            }
        }

        public string SqlId
        {
            get => _sqlId;
            set => SetProperty(ref _sqlId, value);
        }

        public string ExecutionLocation
        {
            get => _executionLocation;
            set => SetProperty(ref _executionLocation, value);
        }

        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        public string BindVariables
        {
            get => _bindVariables;
            set => SetProperty(ref _bindVariables, value);
        }

        public string ExecutionPlan
        {
            get => _executionPlan;
            set => SetProperty(ref _executionPlan, value);
        }

        public string AnalysisInfo
        {
            get => _analysisInfo;
            set => SetProperty(ref _analysisInfo, value);
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        public DateTime LastAccessDate
        {
            get => _lastAccessDate;
            set => SetProperty(ref _lastAccessDate, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        // 계산된 속성들
        public string DisplayName => $"{SqlId} - {ExecutionLocation}";
        public string FormattedCreatedDate => CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
        public string FormattedLastAccessDate => LastAccessDate.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
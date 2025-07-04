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
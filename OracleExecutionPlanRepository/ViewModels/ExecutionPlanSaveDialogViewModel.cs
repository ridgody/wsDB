// ExecutionPlanRepository/ViewModels/ExecutionPlanSaveDialogViewModel.cs
using System;
using wsDB.Common;
using wsDB.OracleExecutionPlanRepository.Models;

namespace wsDB.OracleExecutionPlanRepository.ViewModels
{
    public class ExecutionPlanSaveDialogViewModel : ObservableObject
    {
        public ExecutionPlanRecord Record { get; }

        public ExecutionPlanSaveDialogViewModel(ExecutionPlanRecord record = null)
        {
            Record = record ?? new ExecutionPlanRecord
            {
                CreatedDate = DateTime.Now,
                LastAccessDate = DateTime.Now
            };
        }
    }
}
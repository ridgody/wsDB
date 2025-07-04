using wsDB.Common;

namespace wsDB.OracleExecutionPlanRepository.Models
{
    public class ExecutionLocationNode : ObservableObject
    {
        private string _name;
        private string _fullPath;
        private bool _isExpanded;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string FullPath
        {
            get => _fullPath;
            set => SetProperty(ref _fullPath, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public List<ExecutionLocationNode> Children { get; set; } = new List<ExecutionLocationNode>();
        public List<ExecutionPlanRecord> Records { get; set; } = new List<ExecutionPlanRecord>();

        public int RecordCount => Records.Count;
        public string DisplayText => $"{Name} ({RecordCount})";
    }
}
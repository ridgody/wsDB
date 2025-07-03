// ========== ViewModels/BindVariableWindowViewModel.cs ==========
using System.Collections.ObjectModel; 
using wsDB.Common;
using wsDB.Common.BindVar.Models;

namespace wsDB.Common.BindVar.ViewModels
{
    public class BindVariableWindowViewModel : ObservableObject
    {
        private OracleBindVariable _selectedVariable;

        public ObservableCollection<OracleBindVariable> BindVariables { get; }
        public ObservableCollection<BindVariableType> AvailableTypes { get; }

        public OracleBindVariable SelectedVariable
        {
            get => _selectedVariable;
            set => SetProperty(ref _selectedVariable, value);
        }

        public BindVariableWindowViewModel(List<string> variableNames)
        {
            AvailableTypes = new ObservableCollection<BindVariableType>
            {
                BindVariableType.Varchar2,
                BindVariableType.Number,
                BindVariableType.Date
            };

            BindVariables = new ObservableCollection<OracleBindVariable>();
            foreach (var name in variableNames)
            {
                BindVariables.Add(new OracleBindVariable { Name = name, Type = BindVariableType.Varchar2 });
            }

            // 첫 번째 변수 선택
            if (BindVariables.Count > 0)
            {
                SelectedVariable = BindVariables[0];
            }
        }
    }
}
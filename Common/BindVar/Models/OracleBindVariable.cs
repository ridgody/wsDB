// ========== Models/BindVariable.cs ==========
// using System.ComponentModel;
// using System.Runtime.CompilerServices;
// using wsDB.Common;

namespace wsDB.Common.BindVar.Models
{
    public class OracleBindVariable : ObservableObject
    {
        private string _name;
        private BindVariableType _type = BindVariableType.Varchar2;
        private string _value = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public BindVariableType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

    }

    public enum BindVariableType
    {
        Varchar2,
        Number,
        Date
    }
}
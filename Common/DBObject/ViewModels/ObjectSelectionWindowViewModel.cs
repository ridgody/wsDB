using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wsDB.Common;
using wsDB.Common.DBObject.Models;

namespace wsDB.Common.DBObject.ViewModels
{
    public class ObjectSelectionWindowViewModel : ObservableObject
    {
        private DatabaseObject _selectedObject;

        public string Title { get; }
        public ObservableCollection<DatabaseObject> Objects { get; }
        
        public DatabaseObject SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                SetProperty<DatabaseObject>(ref _selectedObject, value);
                // OnPropertyChanged();
                // OnPropertyChanged(nameof(IsObjectSelected));
            }
        }

        public bool IsObjectSelected => SelectedObject != null;

        public ObjectSelectionWindowViewModel(List<DatabaseObject> objects, string objectName)
        {
            Title = $"'{objectName}' 객체 선택";
            Objects = new ObservableCollection<DatabaseObject>(objects);
            
            // 첫 번째 항목 자동 선택
            if (Objects.Count > 0)
            {
                SelectedObject = Objects[0];
            }
        }

        // public event PropertyChangedEventHandler PropertyChanged;

        // protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        // {
        //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // }
    }
}

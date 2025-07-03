using System.Windows;
using System.Windows.Input;
using wsDB.Common.DBObject.Models;
using wsDB.Common.DBObject.ViewModels;

namespace wsDB.Common.DBObject.Views
{
    public partial class ObjectSelectionWindow : Window
    {
        public DatabaseObject SelectedDatabaseObject { get; private set; }

        public ObjectSelectionWindow(List<DatabaseObject> objects, string objectName)
        {
            InitializeComponent();
            
            var viewModel = new ObjectSelectionWindowViewModel(objects, objectName);
            DataContext = viewModel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ObjectSelectionWindowViewModel;
            if (viewModel?.SelectedObject != null)
            {
                SelectedDatabaseObject = viewModel.SelectedObject;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ObjectsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 더블클릭 시 확인 버튼과 동일한 동작
            if (ObjectsDataGrid.SelectedItem != null)
            {
                OkButton_Click(sender, null);
            }
        }
    }
}
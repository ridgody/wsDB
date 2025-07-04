// ========== Views/BindVariableWindow.xaml.cs ==========
using System.Windows;
using wsDB.Common.BindVar.Models;
using wsDB.Common.BindVar.ViewModels;

namespace wsDB.Common.BindVar.Views
{
    public partial class BindVariableWindow : Window
    {
        // private static BindVariableWindow _instance;
        // private static readonly object _lock = new object();
        private bool _isRealClosing = false;
 
        public List<OracleBindVariable> BindVariables { get; private set; }

        private BindVariableWindow()
        {
            InitializeComponent();
            // this.Closing += BindVariableWindow_Closing;
        }

        
        public static (bool Success, List<OracleBindVariable> Variables) ShowBindVariableDialog(
        List<string> variableNames, Window owner = null)
        {
            var window = new BindVariableWindow();
            var viewModel = new BindVariableWindowViewModel(variableNames);
            window.DataContext = viewModel;
            
            if (owner != null)
            {
                window.Owner = owner;
            }

            var result = window.ShowDialog();
            
            if (result == true)
            {
                return (true, window.BindVariables);
            }
            
            return (false, null);
        }
        
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as BindVariableWindowViewModel;
            if (viewModel != null)
            {
                var validation = ValidateInput(viewModel.BindVariables.ToList());
                if (!validation.IsValid)
                {
                    MessageBox.Show(validation.ErrorMessage, "입력 오류",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                BindVariables = viewModel.BindVariables.ToList();
            }

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            // this.Hide();
        }

        private (bool IsValid, string ErrorMessage) ValidateInput(List<OracleBindVariable> bindVariables)
        {
            foreach (var bindVar in bindVariables)
            {
                if (string.IsNullOrWhiteSpace(bindVar.Value))
                {
                    return (false, $"'{bindVar.Name}' 변수의 값을 입력해주세요.");
                }

                // 타입별 유효성 검사
                switch (bindVar.Type)
                {
                    case BindVariableType.Number:
                        if (!decimal.TryParse(bindVar.Value, out _))
                            return (false, $"'{bindVar.Name}' 변수는 올바른 숫자 형식이어야 합니다.");
                        break;

                    case BindVariableType.Date:
                        if (!DateTime.TryParse(bindVar.Value, out _))
                            return (false, $"'{bindVar.Name}' 변수는 올바른 날짜 형식이어야 합니다.\n예: 2024-01-01 또는 2024-01-01 14:30:00");
                        break;
                }
            }

            return (true, null);
        }
    }
}
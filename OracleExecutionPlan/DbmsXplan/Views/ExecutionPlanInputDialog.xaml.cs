using System;
using System.Windows;
using wsDB.OracleExecutionPlanRepository.Helpers;

namespace wsDB.OracleExecutionPlan.DbmsXplan.Views
{
    public partial class ExecutionPlanInputDialog : Window
    {
        public string SqlId { get; private set; }
        public string ExecutionLocation { get; private set; }
        public string Query { get; private set; }
        public string BindVariables { get; private set; }
        public string Notes { get; private set; }

        public ExecutionPlanInputDialog()
        {
            InitializeComponent();
            
            // 기본값 설정
            SqlIdTextBox.Text = ExecutionPlanHelper.GenerateAutomaticSqlId("DBMS");
            ExecutionLocationTextBox.Text = "DbmsXPlan";
            NotesTextBox.Text = "DbmsXPlan에서 실행계획 저장";
            
            // 포커스 설정
            QueryTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // 유효성 검사
            if (string.IsNullOrWhiteSpace(SqlIdTextBox.Text))
            {
                MessageBox.Show("SQL ID를 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                SqlIdTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(ExecutionLocationTextBox.Text))
            {
                MessageBox.Show("실행 위치를 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ExecutionLocationTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(QueryTextBox.Text))
            {
                MessageBox.Show("쿼리를 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                QueryTextBox.Focus();
                return;
            }

            // SQL ID 유효성 검사
            if (!ExecutionPlanHelper.IsValidSqlId(SqlIdTextBox.Text.Trim()))
            {
                MessageBox.Show("SQL ID는 영문자, 숫자, 언더스코어만 사용 가능합니다.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                SqlIdTextBox.Focus();
                return;
            }

            // 실행 위치 유효성 검사
            if (!ExecutionPlanHelper.IsValidExecutionLocation(ExecutionLocationTextBox.Text.Trim()))
            {
                MessageBox.Show("실행 위치는 점(.)으로 구분된 계층 구조여야 합니다.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ExecutionLocationTextBox.Focus();
                return;
            }

            // 값 할당
            SqlId = SqlIdTextBox.Text.Trim();
            ExecutionLocation = ExecutionLocationTextBox.Text.Trim();
            Query = QueryTextBox.Text.Trim();
            BindVariables = BindVariablesTextBox.Text?.Trim() ?? "";
            Notes = NotesTextBox.Text?.Trim() ?? "";

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
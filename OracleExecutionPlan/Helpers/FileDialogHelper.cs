
// FileDialogHelper.cs
using Microsoft.Win32;

namespace wsDB.OracleExecutionPlan.Helpers
{
    public static class FileDialogHelper
    {
        public static string ShowOpenFileDialog(string title, string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }
}
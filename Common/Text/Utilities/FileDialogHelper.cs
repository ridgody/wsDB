
// namespace wsDB.Common.Utilities
// {
//     /// <summary>
//     /// 파일 대화상자 헬퍼
//     /// </summary>
//     public static class FileDialogHelper
//     {
//         public static string ShowOpenFileDialog(string title = "파일 선택",
//             string filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*")
//         {
//             var openFileDialog = new Microsoft.Win32.OpenFileDialog
//             {
//                 Filter = filter,
//                 Title = title
//             };

//             return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
//         }
//     }
// }
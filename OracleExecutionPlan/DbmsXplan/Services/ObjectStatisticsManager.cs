using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Oracle.ManagedDataAccess.Client;
using wsDB.Common.DBObject.Models;
using wsDB.Common.DBObject.Services;
using wsDB.Common.DBObject.Views;
using wsDB.OracleExecutionPlan.Core.Text;
using wsDB.OracleExecutionPlan.DbmsXplan.Views;

namespace wsDB.OracleExecutionPlan.DbmsXplan.Services
{
    public class ObjectStatisticsManager
    {
        private readonly OracleConnection dbConnection;
        private readonly TextProcessor textProcessor;
        private readonly Dictionary<string, ObjectStatisticsWindow> openStatWindows;
        private readonly Action<string> updateSelectedObjectDisplay;

        private Window Owner;

        public ObjectStatisticsManager(Window owner,
                                     OracleConnection dbConnection,
                                     TextProcessor textProcessor,
                                     Action<string> updateSelectedObjectDisplay)
        {
            this.dbConnection = dbConnection;
            this.textProcessor = textProcessor;
            this.updateSelectedObjectDisplay = updateSelectedObjectDisplay;
            this.openStatWindows = new Dictionary<string, ObjectStatisticsWindow>();
            this.Owner = owner;
        }

        public void HandleObjectStatistics(string selectedText)
        {
            if (string.IsNullOrWhiteSpace(selectedText)) return;

            string cleanedText = textProcessor.CleanTextForObjectSearch(selectedText.Trim());

            if (!string.IsNullOrEmpty(cleanedText))
            {
                ShowObjectStatistics(cleanedText);
                updateSelectedObjectDisplay?.Invoke(cleanedText);
            }
        }

        public void HandleObjectStatistics(RichTextBox rtb)
        {
            if (!rtb.Selection.IsEmpty)
            {
                HandleObjectStatistics(rtb.Selection.Text);
            }
        }
        
        private async void ShowObjectStatistics(string objectName)
        {
            try
            {
                // 1. 동일한 이름의 객체들 조회
                var objectService = new DatabaseObjectService(dbConnection);
                var objects = await objectService.GetObjectsByNameAsync(objectName);

                if (objects.Count == 0)
                {
                    MessageBox.Show($"'{objectName}' 객체를 찾을 수 없습니다.", "객체 없음", 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                DatabaseObject selectedObject;

                if (objects.Count == 1)
                {
                    // 객체가 하나면 바로 선택
                    selectedObject = objects[0];
                }
                else
                {
                    // 여러 객체가 있으면 선택 창 표시
                    var selectionWindow = new ObjectSelectionWindow(objects, objectName);
                    selectionWindow.Owner = this.Owner; // 또는 적절한 Owner 창

                    var result = selectionWindow.ShowDialog();
                    if (result != true)
                    {
                        return; // 사용자가 취소
                    }

                    selectedObject = selectionWindow.SelectedDatabaseObject;
                }

                // 2. 선택된 객체로 통계 창 열기
                string windowKey = $"{selectedObject.Owner}.{selectedObject.ObjectName}.{selectedObject.ObjectType}";
                
                if (openStatWindows.ContainsKey(windowKey))
                {
                    openStatWindows[windowKey].Focus();
                    return;
                }

                ObjectStatisticsWindow statWindow = new ObjectStatisticsWindow(
                    dbConnection, 
                    selectedObject.ObjectName, 
                    selectedObject.ObjectType,
                    selectedObject.Owner); // Owner 정보도 전달

                statWindow.Owner = this.Owner; // 또는 적절한 Owner 창
                statWindow.Closed += (s, e) => openStatWindows.Remove(windowKey);
                openStatWindows[windowKey] = statWindow;

                statWindow.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"객체 통계 오류: {ex.Message}");
                MessageBox.Show($"객체 통계를 여는 중 오류가 발생했습니다: {ex.Message}", "오류");
            }
        }

        // private void ShowObjectStatistics(string objectName)
        // {
        //     try
        //     {
        //         if (openStatWindows.ContainsKey(objectName))
        //         {
        //             openStatWindows[objectName].Focus();
        //             return;
        //         }

        //         string objectType = DetermineObjectType(objectName);
        //         ObjectStatisticsWindow statWindow = new ObjectStatisticsWindow(dbConnection, objectName, objectType);
        //         statWindow.Owner = Owner;

        //         statWindow.Closed += (s, e) => openStatWindows.Remove(objectName);
        //         openStatWindows[objectName] = statWindow;

        //         statWindow.Show();
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.WriteLine($"변수 값: {ex.Message}");

        //         MessageBox.Show($"객체 통계를 여는 중 오류가 발생했습니다: {ex.Message}", "오류");
        //     }
        // }

        private string DetermineObjectType(string objectName)
        {
            // 간단한 휴리스틱으로 객체 타입 결정
            // 실제로는 더 정교한 로직이 필요할 수 있음
            if (objectName.Contains("PK_") || objectName.Contains("IDX_") || objectName.Contains("IX_"))
                return "INDEX";
            else
                return "TABLE";
        }
    }
}

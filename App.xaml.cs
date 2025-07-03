// App.xaml.cs
using System.Configuration;
using System.Data;
using System.Windows;
using wsDB.OracleConnectionManager.Services;
using wsDB.OracleConnectionManager.Views;

namespace wsDB
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 프로파일 매니저 초기화
            // var profileManager = new ConnectionProfileManager();
            // profileManager.LoadProfiles();

            // 메인 윈도우 시작
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // 접속 창 표시
            var connectionWindow = ConnectionWindow.Instance;
            // connectionWindow.Owner = mainWindow;
            connectionWindow.ShowDialog();
        }
        
         protected override void OnExit(ExitEventArgs e)
        {
            ConnectionWindow.ForceClose();
            base.OnExit(e);
        }
    }
}
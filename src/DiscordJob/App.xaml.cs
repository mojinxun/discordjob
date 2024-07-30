using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace DiscordJob
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var process = RuningInstance();
            if (process != null)//查找到同进程的话
            {


                Environment.Exit(1);
                return;
            }

            base.OnStartup(e);
        }

        private static Process? RuningInstance()
        {
            var currentProcess = Process.GetCurrentProcess();
            var Processes = Process.GetProcessesByName(currentProcess.ProcessName);
            foreach (var process in Processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    return process;
                }
            }
            return null;
        }
    }
}

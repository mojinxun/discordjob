using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiscordJob
{
    public class TabbarIconCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Application.Current.MainWindow.Show();
        }
    }
}

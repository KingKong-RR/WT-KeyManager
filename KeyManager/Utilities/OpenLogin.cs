using System.Windows;
using KeyManager.ViewModels;
using KeyManager.Views;

namespace KeyManager.Utilities
{
    public static class OpenLogin 
    {
         public static void OpenLoginDialog()
         {
            LoginDialogView login = new LoginDialogView();
            LoginDialogViewModel vm = new LoginDialogViewModel();
            login.DataContext = vm;
            MainWindow mainWindow = (MainWindow) Application.Current.MainWindow;
            login.Owner = mainWindow;
            login.ShowDialog();
            if (login.DialogResult.HasValue && login.DialogResult.Value)
            {

            }
         }
    }
}

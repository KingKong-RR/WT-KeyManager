using System;
using System.Windows;
using KeyManager.Commands;
using KeyManager.Commands.AsyncCommands;
using KeyManager.Models;
using KeyManager.Utilities;
using KeyManager.Views;

namespace KeyManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //Commands
        public RelayCommand DoLogout { get; }
        public RelayCommand DoExit { get; }
        public RelayCommand CsvImport { get; set; }
        public RelayCommand InfoDialog { get; set; }

        private bool _userManagementEnabled;
        public bool UserManagementEnabled
        {
            get { return _userManagementEnabled; }
            set { _userManagementEnabled = value; NotifyPropertyChanged(nameof(UserManagementEnabled)); }
        }

        public enum Tabs
        {
            Customers,
            Groups,
            UserManagement,
            ActivityLog
        }

        //Properties
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                NotifyPropertyChanged(nameof(SelectedIndex));
            }
        }

        public MainWindowViewModel()
        {
            DoLogout = new RelayCommand(() =>
            {
                // open the LoginDialog when User is Logged Out
                OpenLogin.OpenLoginDialog();
            });

            // Close the Application
            DoExit = new RelayCommand(() =>
            {
                Application.Current.Shutdown();
            });

            CsvImport = new RelayCommand(()=>
            {
                try
                {
                    CsvImportDialogView dialog = new CsvImportDialogView();
                    CsvImportDialogViewModel vm = new CsvImportDialogViewModel();
                    dialog.DataContext = vm;
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                    dialog.Owner = mainWindow;
                    dialog.ShowDialog();
                }
                catch (Exception)
                {
                    MessageBox.Show("Unerwarteter Fehler, dem Admin melden.", "Fehler");
                }
            });

            InfoDialog = new RelayCommand(() =>
            {
                InfoDialogView dialog = new InfoDialogView();
                InfoDialogViewModel vm = new InfoDialogViewModel();
                dialog.DataContext = vm;
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                dialog.Owner = mainWindow;
                dialog.ShowDialog();
            });
        }
    }
}

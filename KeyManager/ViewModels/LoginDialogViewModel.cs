using System.Threading.Tasks;
using System.Windows;
using KeyManager.Commands;
using KeyManager.Commands.AsyncCommands;
using KeyManager.Crypto;
using KeyManager.DataBase;
using KeyManager.Models;
using KeyManager.Utilities;

namespace KeyManager.ViewModels
{
    public class LoginDialogViewModel : ViewModelBase
    {
        // Commands
        public IAsyncCommand DoLoginAsync { get; }
        public RelayCommand Cancel { get; }

        public string UserName { get; set; }
        public string Password { get; set; }
        // close action gets set by view
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn // indicates, if we are logged in
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                NotifyPropertyChanged(nameof(_isLoggedIn));
            }
        }

        // Saves the User from the OpenLogin when he is verfied succsessfully
        public static DbUser VerfiedUser;

        //InfoText wehn not logged in
        public string InfoText { get; set; } = "Not logged in";


        public LoginDialogViewModel()
        {
            DoLoginAsync = AsyncCommand.Create(async () =>
            {
                try
                {
                    IsLoggedIn = false;

                    var user = await DataAccessService.GetDbUserAsync(UserName); // gets and stores token

                    if (Hashing.ValidatePassword("Password", user.Password) && user.UserName != null && user.UserType == "Admin")
                    {
                        IsLoggedIn = true;
                        VerfiedUser = user;
                    }
                    else
                    {
                        MessageBox.Show("Sie sind kein Administrator oder das Passwort ist falsch", "Fehler");
                    }

                    InfoText = IsLoggedIn ? "OpenLogin Successful" : "OpenLogin Failed";
                    NotifyPropertyChanged(nameof(InfoText));
                    
                    if (IsLoggedIn)
                    {
                        ViewModelLocator.UserManagementViewModel.UserTypeComboBoxList = await ComboBoxListLoader.ComboBoxUserTypeListLoader();
                        ViewModelLocator.MainWindowViewModel.UserManagementEnabled = VerfiedUser.UserType == "Admin";
                        ViewModelLocator.CustomerViewModel.ButtonIsDisabled = VerfiedUser.UserType == "Sachbearbeiter";
                        ViewModelLocator.GroupViewModel.ButtonIsDisabled = VerfiedUser.UserType == "Sachbearbeiter";
                        ViewModelLocator.UserManagementViewModel.UserManagementEnabled = VerfiedUser.UserType == "Admin";
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("Username oder Password ist falsch");
                    }
                }
                catch (TaskCanceledException e)
                {
                    MessageBox.Show("Verbindungsversuch wurde unterbrochen");
                } // Do nothing

            }, param =>
            {
                // only activate button if we are not logged in
                var execution = ((AsyncCommand<object>)DoLoginAsync).Execution;
                if (execution != null)
                    return !IsLoggedIn && execution.IsCompleted;

                return !IsLoggedIn;
            });

            // Closes the dialog
            // Close button is only active, when OpenLogin command is not running
            Cancel = new RelayCommand(() =>
            {
                //Close the Application
                Application.Current.Shutdown();
                DialogResult = false;
            },
            param => true, true);
        }
    }
}

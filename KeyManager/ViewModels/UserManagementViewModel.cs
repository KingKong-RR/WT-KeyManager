using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using KeyManager.Commands;
using KeyManager.Commands.AsyncCommands;
using KeyManager.DataBase;
using KeyManager.Exceptions;
using KeyManager.Extensions;
using KeyManager.Models;
using KeyManager.Utilities;
using KeyManager.Views;

namespace KeyManager.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        //Commands
        public IAsyncCommand EditUser { get; }
        public IAsyncCommand NewUser { get; set; }
        public IAsyncCommand LoadList { get; set; }
        public ObservableCollection<DbUser> Users { get; } = new ObservableCollection<DbUser>();
        public RelayCommand Cancel { get; }
        public IAsyncCommand DoGetUsersBySearchParameterAsync { get; }


        //Properties
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public bool UserIsDeleted { get; set; }
        public int SearchUserIsDeleted { get; set; }
        public DateTime? SearchUserStartDate { get; set; }
        public DateTime? SearchUserEndDate { get; set; }
        public UserType ChoosenUserType { get; set; }

        private List<UserType> _userTypeComboBoxList;
        public List<UserType> UserTypeComboBoxList
        {
            get { return _userTypeComboBoxList; }
            set { _userTypeComboBoxList = value; NotifyPropertyChanged(nameof(UserTypeComboBoxList)); }
        }

        // close action gets set by view
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); }
        }

        private bool _userManagementEnabled;
        public bool UserManagementEnabled
        {
            get { return _userManagementEnabled; }
            set { _userManagementEnabled = value; NotifyPropertyChanged(nameof(UserManagementEnabled)); }
        }

        public UserManagementViewModel()
        {
            if (TextBoxFilter.ContainsSpecialChars(UserName))
            {
                MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Benutzername überprüfen", "Eingabefehler");
                return;
            }

            // Shows how to retrieve a customer by SearchParameter
            DoGetUsersBySearchParameterAsync = AsyncCommand.Create(async () =>
            {
                try
                {
                    ObservableCollection<DbUser> resultUsers = await DataAccessService.GetUsersAsync(new SearchUserParameter(UserName ?? "", ChoosenUserType != null ? ChoosenUserType.UserTypeId.ToString() : "", SearchUserIsDeleted, SearchUserStartDate, SearchUserEndDate));
                    Users.Clear();
                    Users.AddRange(resultUsers);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Sie sind nicht Autorisiert", "Autorisierungsfehler");
                }
                catch (NotFoundException)
                {
                    MessageBox.Show("Datensatz nicht gefunden, Eingabe überprüfen", "Suchfehler");
                }
                catch (HttpRequestException)
                {
                    // lost connection
                    OpenLogin.OpenLoginDialog();
                }
                catch (InternalServerErrorException)
                {
                    MessageBox.Show("Serverfehler, bitte erneut versuchen", "Serverfehler");
                }
                catch (Exception)
                {
                    MessageBox.Show("Unerwarteter Fehler, dem Admin melden.", "Fehler");
                }
            }, param =>
            {
                // only activate button if we are not logged in
                var execution = ((AsyncCommand<object>)DoGetUsersBySearchParameterAsync).Execution;
                if (execution != null)
                    return execution.IsCompleted;

                return true;
            });

            // Closes the dialog
            // Close button is only active, when Save command is not running
            Cancel = new RelayCommand(() =>
            {
                DialogResult = false;
            }, param =>
            {
                var execution = ((AsyncCommand<object>)DoGetUsersBySearchParameterAsync).Execution;
                if (execution != null) return execution.IsCompleted;
                return true;
            }, true);

            EditUser = AsyncCommand.Create(async (choosenUser) =>
            {
                try
                {

                    EditUserDialogView dialog = new EditUserDialogView();
                    EditUserDialogViewModel vm = new EditUserDialogViewModel(false, "Benutzer bearbeiten", (DbUser)choosenUser);
                    List<UserType> userTypeList = await DataAccessService.GetUserTypesAsync();
                    vm.UserTypeComboBoxList = userTypeList;
                    dialog.DataContext = vm;


                    // determine which ComboBoxItem to select, based on the choosenUser
                    foreach (var tempUserType in userTypeList)
                    {
                        if (tempUserType.UserTypeId == ((DbUser)choosenUser).UserTypeId)
                        {
                            ((EditUserDialogViewModel)dialog.DataContext).UpdateUser.SelectedComboBoxItem = tempUserType;
                        }
                    }

                    //((EditUserDialogViewModel) dialog.DataContext).ActiveUser //-> wird so gemacht da im Code Behind die ViewModel der View zugewisen worden ist, somit müssen dem Objekt DataContext sagen was reinkommt mit einem Cast auf das EditDialogCustomerView.
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                    dialog.Owner = mainWindow;
                    dialog.ShowDialog();
                }
                catch (Exception)
                {
                    MessageBox.Show("Unerwarteter Fehler, dem Admin melden.", "Fehler");
                }

            });

            NewUser = AsyncCommand.Create(async () =>
            {
                try
                {
                    EditUserDialogView dialog = new EditUserDialogView();
                    var newUser = new DbUser();
                    EditUserDialogViewModel vm = new EditUserDialogViewModel(true, "Neuen Benutzer ertsellen", newUser);
                    vm.UserTypeComboBoxList = await DataAccessService.GetUserTypesAsync();
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
        }
    }
}
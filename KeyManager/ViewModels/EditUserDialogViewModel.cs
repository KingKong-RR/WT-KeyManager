using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows;
using KeyManager.BusinessLogic;
using KeyManager.Commands;
using KeyManager.Commands.AsyncCommands;
using KeyManager.DataBase;
using KeyManager.Exceptions;
using KeyManager.Models;
using KeyManager.Utilities;

namespace KeyManager.ViewModels
{
    public class EditUserDialogViewModel : ViewModelBase
    {
        //Commands
        public IAsyncCommand Refresh { get; set; }
        public IAsyncCommand SaveAsync { get; set; }
        public RelayCommand Cancel { get; set; }

        //Properties
        public string Dialogtitle { get; set; }
        public bool EditModeNewUser { get; set; }
        public string PasswordToChange { get; set; }
        public string PasswordToChangeVerifies { get; set; }

        private UpdateUser _updateUser;
        public UpdateUser UpdateUser { get; set; } = new UpdateUser();

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

        public EditUserDialogViewModel(bool editModeNewUser, string dialogTitle, DbUser dbUser)
        {
            EditModeNewUser = editModeNewUser;
            Dialogtitle = dialogTitle;
            UpdateUser.ActiveUser = new DbUser(dbUser);
            UpdateUser.OrigUser = new DbUser(dbUser);
            UpdateUser.SelectedComboBoxItem = new UserType();

            SaveAsync = AsyncCommand.Create(async () =>
            {
                if (TextBoxFilter.ContainsSpecialChars(UpdateUser.ActiveUser.UserName))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Benutzername überprüfen", "Eingabefehler");
                    return;
                }

                try
                {
                    if (EditModeNewUser)
                    {
                        // check if all quired fields are filled
                        if (UpdateUser.SelectedComboBoxItem.UserTypeId == 0 ||
                            UpdateUser.ActiveUser.UserName == null ||
                            PasswordToChange == null)
                        {
                            MessageBox.Show("Bitte alle gekennzeichneten Felder korrekt ausfüllen", "Fehler");
                            return;
                        }
                        if (PasswordToChangeVerifies != null && PasswordToChangeVerifies == PasswordToChange)
                        {
                            if (PasswordToChangeVerifies != PasswordToChange)
                            {
                                MessageBox.Show("Passwörter stimmen nicht überein.", "Fehler");
                                return;
                            }

                            UpdateUser.ActiveUser.UserTypeId = UpdateUser.SelectedComboBoxItem.UserTypeId;
                            UpdateUser.ActiveUser.UserType = UpdateUser.SelectedComboBoxItem.UserTypeName;
                            UpdateUser.ActiveUser.Password = PasswordToChange;

                            await BusinessLogicUser.InsertUser(UpdateUser);
                            DialogResult = true;
                        }
                        else
                        {
                            MessageBox.Show("Passwort stimmt nicht überein", "Fehler");
                            return;
                        }
                    }
                    else //Update/Edit User
                    {
                        // check if all quired fields are filled
                        if (UpdateUser.SelectedComboBoxItem == null || UpdateUser.ActiveUser.UserName == null)
                        {
                            MessageBox.Show("Bitte alle gekennzeichneten Felder korrekt ausfüllen", "Fehler");
                            return;
                        }
                        if (PasswordToChangeVerifies != PasswordToChange)
                        {
                            MessageBox.Show("Passwörter stimmen nicht überein.", "Fehler");
                            return;
                        }

                        UpdateUser.ActiveUser.Password = PasswordToChange;
                        await BusinessLogicUser.UpdateUser(UpdateUser);
                        DialogResult = true;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Sie sind nicht Autorisiert", "Autorisierungsfehler");
                }
                catch (ConflictException)
                {
                    MessageBox.Show("Unerwarteter Fehler, dem Admin melden.", "Fehler");
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
                    MessageBox.Show("Fehler beim Suchen, wenden Sie sich beim Admin", "Fehler");
                }
                try
                {
                    await ViewModelLocator.UserManagementViewModel.DoGetUsersBySearchParameterAsync.ExecuteAsync(null);
                }
                catch (Exception) { } // Es wird absichtlich nicht verarbeitet da es sich nur um den Refresh handelt
            });

            Refresh = AsyncCommand.Create(async () =>
            {
                try
                {
                    ObservableCollection<DbUser> refreshUser = await DataAccessService.GetUsersAsync(new SearchUserParameter(UpdateUser.ActiveUser.UserId));
                    DbUser[] refreshedUser = refreshUser.ToArray();
                    UpdateUser.ActiveUser = refreshedUser[0];
                    UpdateUser.OrigUser = refreshedUser[0];
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Sie sind nicht Autorisiert", "Autorisierungsfehler");
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
            });

            Cancel = new RelayCommand(() =>
            {
                DialogResult = false;
            });
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Transactions;
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
    public class EditGroupDialogViewModel : ViewModelBase
    {
        //Commands
        public IAsyncCommand Refresh { get; set; }
        public IAsyncCommand SaveAsync { get; }
        public RelayCommand Cancel { get; }

        //Properties
        public string Dialogtitle { get; set; }
        public UpdateGroup UpdateGroup { get; set; } = new UpdateGroup();

        private bool _buttonIsDisabled;
        public bool ButtonIsDisabled
        {
            get { return _buttonIsDisabled; }
            set { _buttonIsDisabled = value; NotifyPropertyChanged(nameof(ButtonIsDisabled)); }
        }

        public bool EditModeNewGroup { get; set; }

        // close action gets set by view
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); }
        }

        //public Group OrigGroup { get; set; }

        private bool _codeGenerateCheckBoxStateEnabled;
        public bool CodeGenerateCheckBoxStateEnabled
        {
            get
            {
                if (EditModeNewGroup || UpdateGroup.ActiveGroup.GroupIsDeleted)
                {
                    return _codeGenerateCheckBoxStateEnabled = true;
                }
                return _codeGenerateCheckBoxStateEnabled = false;
            }
            set
            {
                _codeGenerateCheckBoxStateEnabled = value;
                NotifyPropertyChanged(nameof(CodeGenerateCheckBoxStateEnabled));
            }
        }

        private bool _deletedCheckboxStateEnabled;
        public bool DeletedCheckboxStateEnabled
        {
            get
            {
                _deletedCheckboxStateEnabled = !(EditModeNewGroup || UpdateGroup.ActiveGroup.GroupCustomerIsDeleted);
                return _deletedCheckboxStateEnabled;
            }
            set { _deletedCheckboxStateEnabled = value; NotifyPropertyChanged(nameof(DeletedCheckboxStateEnabled)); }
        }

        // used to forward property changed event from ActiveCustomer
        private void ActiveCustomer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GroupIsDeleted")
            {
                NotifyPropertyChanged(nameof(CodeGenerateCheckBoxStateEnabled));
            }
        }

        public EditGroupDialogViewModel(bool editModeNewGroup, string dialogTitle, Group group)
        {
            EditModeNewGroup = editModeNewGroup;
            Dialogtitle = dialogTitle;
            UpdateGroup.ActiveGroup = new Group(group);
            UpdateGroup.OrigGroup = new Group(group);
            ButtonIsDisabled = LoginDialogViewModel.VerfiedUser.UserType == "Sachbearbeiter";

            // register ViewModel to receive property changed events from ActiveCustomer 
            UpdateGroup.ActiveGroup.PropertyChanged += ActiveCustomer_PropertyChanged;

            SaveAsync = AsyncCommand.Create(async () =>
            {
                if (TextBoxFilter.ContainsSpecialChars(UpdateGroup.ActiveGroup.GroupName))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Name überprüfen", "Eingabefehler");
                    return;
                }
                if (TextBoxFilter.ContainsSpecialChars(UpdateGroup.ActiveGroup.PNumber))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld P-Nummer überprüfen", "Eingabefehler");
                    return;
                }

                // If Insert a NewGroup
                if (EditModeNewGroup)
                {
                    try
                    {
                        if (UpdateGroup.ActiveGroup.GroupName == null ||
                            UpdateGroup.ActiveGroup.PNumber == null ||
                            UpdateGroup.ActiveGroup.GroupCustomerName == null)
                        {
                            MessageBox.Show("Bitte alle gekennzeichneten Felder korrekt ausfüllen", "Fehler");
                            return;

                        }
                        using (var txscope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            await BusinessLogicGroup.InsertGroup(UpdateGroup.ActiveGroup);
                            txscope.Complete();
                        }
                        DialogResult = true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Sie sind nicht Autorisiert", "Autorisierungsfehler");
                    }
                    catch (ConflictException)
                    {
                        MessageBox.Show("Überprüfen Sie die Eingabe, Datensatz existiert schon", "Duplikatfehler");
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
                }
                else // Update/Edit Group
                {
                    try
                    {
                        if (UpdateGroup.ActiveGroup.GroupName == null ||
                            UpdateGroup.ActiveGroup.PNumber == null ||
                            UpdateGroup.ActiveGroup.GroupCustomerName == null)
                        {
                            MessageBox.Show("Bitte alle gekennzeichneten Felder korrekt ausfüllen", "Fehler");
                            return;
                        }
                        using (var txscope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            await BusinessLogicGroup.UpdateGroup(UpdateGroup);
                            txscope.Complete();
                        }

                        DialogResult = true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Sie sind nicht Autorisiert", "Autorisierungsfehler");
                    }
                    catch (ConflictException)
                    {
                        MessageBox.Show("Überprüfen Sie die Eingabe, Datensatz existiert schon", "Duplikatfehler");
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
                    try
                    {
                        await ViewModelLocator.GroupViewModel.DoGetGroupsBySearchParameterAsync.ExecuteAsync(null);
                    }
                    catch (Exception) { } // Es wird absichtlich nicht verarbeitet da es sich nur um den Refresh handelt
                }
            });

            Refresh = AsyncCommand.Create(async () =>
            {
                try
                {
                    ObservableCollection<Group> refreshGroup =
                        await DataAccessService.GetGroupsAsync(
                            new SearchGroupParameter(UpdateGroup.ActiveGroup.GroupId));
                    Group[] refreshedGroup = refreshGroup.ToArray();
                    UpdateGroup.ActiveGroup = refreshedGroup[0];
                    UpdateGroup.OrigGroup = refreshedGroup[0];
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

            Cancel = new RelayCommand(() => { DialogResult = false; });
        }

    }
}

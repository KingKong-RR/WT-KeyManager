using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using KeyManager.BusinessLogic;
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
    public class GroupViewModel : ViewModelBase
    {
        //Commands
        public RelayCommand EditGroup { get; set; }
        public RelayCommand NewGroup { get; }
        public ObservableCollection<Group> Groups { get; } = new ObservableCollection<Group>();
        public RelayCommand Cancel { get; }
        public IAsyncCommand DoGetGroupsBySearchParameterAsync { get; }

        //Properties
        public string GroupName { get; set; }
        public string PNumber { get; set; }
        public int SearchGroupIsDeleted { get; set; }
        public DateTime? SearchGroupStartDate { get; set; }
        public DateTime? SearchGroupEndDate { get; set; }

        private bool _buttonIsDisabled;
        public bool ButtonIsDisabled
        {
            get { return _buttonIsDisabled; }
            set { _buttonIsDisabled = value; NotifyPropertyChanged(nameof(ButtonIsDisabled)); }
        }

        private string _groupCustomerName;
        public string GroupCustomerName
        {
            get { return _groupCustomerName; }
            set { _groupCustomerName = value; NotifyPropertyChanged(nameof(GroupCustomerName)); }
        }
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


        public GroupViewModel()
        {
            if (TextBoxFilter.ContainsSpecialChars(GroupName))
            {
                MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Name überprüfen", "Eingabefehler");
                return;
            }
            if (TextBoxFilter.ContainsSpecialChars(GroupCustomerName))
            {
                MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Betreiber-Name überprüfen", "Eingabefehler");
                return;
            }
            if (TextBoxFilter.ContainsSpecialChars(PNumber))
            {
                MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld P-Nummer überprüfen", "Eingabefehler");
                return;
            }

            // Shows how to retrieve a customer by SearchParameter
            DoGetGroupsBySearchParameterAsync = AsyncCommand.Create(async () =>
            {
                try
                {
                    ObservableCollection<Group> resultGroups = await DataAccessService.GetGroupsAsync(new SearchGroupParameter(GroupName ?? "", PNumber ?? "", GroupCustomerName ?? "", SearchGroupIsDeleted, SearchGroupStartDate, SearchGroupEndDate));
                    Groups.Clear();
                    Groups.AddRange(resultGroups);
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
                var execution = ((AsyncCommand<object>)DoGetGroupsBySearchParameterAsync).Execution;
                if (execution != null)
                    return execution.IsCompleted;

                return !IsLoggedIn;
            });

            // Closes the dialog
            // Close button is only active, when Search command is not running
            Cancel = new RelayCommand(() =>
            {
                DialogResult = false;
            }, param =>
            {
                var execution = ((AsyncCommand<object>)DoGetGroupsBySearchParameterAsync).Execution;
                if (execution != null) return execution.IsCompleted;
                return true;
            }, true);


            EditGroup = new RelayCommand((choosenGroup) =>
            {
                try
                {
                    EditGroupDialogView dialog = new EditGroupDialogView();
                    EditGroupDialogViewModel vm =
                        new EditGroupDialogViewModel(false, "Gruppe Bearbeiten", (Group)choosenGroup);
                    dialog.DataContext = vm;
                    Views.MainWindow mainWindow = (Views.MainWindow)Application.Current.MainWindow;
                    dialog.Owner = mainWindow;
                    dialog.ShowDialog();
                }
                catch (Exception)
                {
                    MessageBox.Show("Unerwarteter Fehler, dem Admin melden.", "Fehler");
                }
            });

            NewGroup = new RelayCommand(() =>
            {
                try
                {
                    EditGroupDialogView dialog = new EditGroupDialogView();
                    EditGroupDialogViewModel vm =
                        new EditGroupDialogViewModel(true, "Neue Gruppe erstellen", new Group());
                    dialog.DataContext = vm;
                    Views.MainWindow mainWindow = (Views.MainWindow)Application.Current.MainWindow;
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

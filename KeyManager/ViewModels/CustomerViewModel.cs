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
    public class CustomerViewModel : ViewModelBase
    {
        //Commands
        public RelayCommand EditCustomer { get; set; }
        public RelayCommand NewCustomer { get; }
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();
        public RelayCommand Cancel { get; }
        public RelayCommand NewGroup { get; set; }
        public IAsyncCommand GetGroups { get; set; }
        public IAsyncCommand DoGetCustomersBySearchParameterAsync { get; }

        //Properties
        private Customer _selectedItem;
        public Customer SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged(nameof(CanCreateNewGroup));
            }
        }

        public bool CanCreateNewGroup
        {
            get { return !_selectedItem.CustomerIsDeleted && LoginDialogViewModel.VerfiedUser.UserType != "Sachbearbeiter"; }
            set { NotifyPropertyChanged(nameof(CanCreateNewGroup)); }
        }

        public int CustomerId { get; set; }
        public string KtNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int SearchCustomerIsDeleted { get; set; }
        public string SummPNumber { get; set; }
        public DateTime? SearchCustomerStartDate { get; set; }
        public DateTime? SearchCustomerEndDate { get; set; }

        private bool _buttonIsDisabled;
        public bool ButtonIsDisabled
        {
            get { return _buttonIsDisabled; }
            set { _buttonIsDisabled = value; NotifyPropertyChanged(nameof(ButtonIsDisabled)); }
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

        // Special Property to Close the Dialog
        private bool? _dialogResult;
        private bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); }
        }

        public CustomerViewModel()
        {
            // Shows how to retrieve a customer by SearchParameter
            DoGetCustomersBySearchParameterAsync = AsyncCommand.Create(async () =>
            {
                if (TextBoxFilter.ContainsSpecialChars(CustomerName))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Betreiber überprüfen", "Eingabefehler");
                    return;
                }
                if (TextBoxFilter.ContainsSpecialChars(CustomerCode))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Kundecode überprüfen", "Eingabefehler");
                    return;
                }
                if (TextBoxFilter.ContainsSpecialChars(KtNumber))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld KT-Nummer überprüfen", "Eingabefehler");
                    return;
                }
                if (TextBoxFilter.ContainsSpecialChars(SummPNumber))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Sammel P-Nummer überprüfen", "Eingabefehler");
                    return;
                }

                try
                {
                    ObservableCollection<Customer> resultCustomers = await DataAccessService.GetCustomersAsync(new SearchCustomerParameter(CustomerName ?? "", KtNumber ?? "", CustomerCode ?? "", SummPNumber ?? "", SearchCustomerIsDeleted, SearchCustomerStartDate, SearchCustomerEndDate));
                    if (resultCustomers != null)
                    {
                        Customers.Clear();
                        Customers.AddRange(resultCustomers);
                    }
                    else
                    {
                        MessageBox.Show("Es Wurde Kein Betreiber gefunden", "Fehler");
                    }
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
                var execution = ((AsyncCommand<object>)DoGetCustomersBySearchParameterAsync).Execution;
                if (execution != null)
                    return execution.IsCompleted;

                return !IsLoggedIn;
            });

            // Closes the dialog
            // Close button is only active, when Save command is not running
            Cancel = new RelayCommand(() =>
            {
                DialogResult = false;
            }, param =>
            {
                // This Execution shows that the Cancelbutton is waiting for the save button, when the savebutton is deactivated after click the cancel button do the same
                var execution = ((AsyncCommand<object>)DoGetCustomersBySearchParameterAsync).Execution;
                if (execution != null) return execution.IsCompleted;
                return true;
            }, true);


            EditCustomer = new RelayCommand((choosenCustomer) =>
            {
                try
                {
                    EditCustomerDialogView dialog = new EditCustomerDialogView();
                    EditCustomerDialogViewModel vm = new EditCustomerDialogViewModel(false, "Betreiber bearbeiten", (Customer)choosenCustomer);
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

            NewCustomer = new RelayCommand(() =>
            {
                try
                {
                    EditCustomerDialogView dialog = new EditCustomerDialogView();
                    var newCustomer = new Customer();
                    EditCustomerDialogViewModel vm =
                        new EditCustomerDialogViewModel(true, "Neuen Betreiber erstellen", newCustomer);
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

            NewGroup = new RelayCommand((customer) =>
            {
                try
                {
                    EditGroupDialogView dialog = new EditGroupDialogView();
                    var newGroup = new Group();
                    newGroup.GroupCustomerId = ((Customer)customer).CustomerId;
                    newGroup.GroupCustomerName = ((Customer)customer).CustomerName;
                    EditGroupDialogViewModel vm = new EditGroupDialogViewModel(true, "Neue  Gruppe erstellen", newGroup);
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

            GetGroups = AsyncCommand.Create(async (customer) =>
            {
                try
                {
                    ViewModelLocator.MainWindowViewModel.SelectedIndex = (int)MainWindowViewModel.Tabs.Groups;
                    ViewModelLocator.GroupViewModel.GroupCustomerName = ((Customer)customer).CustomerName;
                    var groupsToCustomer = await DataAccessService.GetGroupsAsync(new SearchGroupParameter("", "", ((Customer)customer).CustomerName, 0, null, null));
                    ViewModelLocator.GroupViewModel.Groups.Clear();
                    ViewModelLocator.GroupViewModel.Groups.AddRange(groupsToCustomer);
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
            });
        }
    }
}

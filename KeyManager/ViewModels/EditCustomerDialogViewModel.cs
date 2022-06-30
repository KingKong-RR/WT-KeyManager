using System;
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
    public class EditCustomerDialogViewModel : ViewModelBase
    {
        //Commands
        public IAsyncCommand SaveAsync { get; }
        public IAsyncCommand Refresh { get; set; }
        public RelayCommand Cancel { get; }

        //Properties
        public bool EditModeNewCustomer { get; set; }
        public UpdateCustomer UpdateCustomer { get; set; } = new UpdateCustomer();
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public string GroupName { get; set; }
        public string PNumber { get; set; }
        public int SearchGroupIsDeleted { get; set; }
        public string GroupCustomerName { get; set; }
        public DateTime? SearchGroupStartDate { get; set; }
        public DateTime? SearchGroupEndDate { get; set; }
        public string Dialogtitle { get; set; }

        private bool _buttonIsDisabled;
        public bool ButtonIsDisabled
        {
            get { return _buttonIsDisabled; }
            set { _buttonIsDisabled = value; NotifyPropertyChanged(nameof(ButtonIsDisabled)); }
        }

        private bool _deletedCheckboxStateEnabled;
        public bool DeletedCheckboxStateEnabled
        {
            get
            {
                _deletedCheckboxStateEnabled = !EditModeNewCustomer;
                return _deletedCheckboxStateEnabled;
            }
            set { _deletedCheckboxStateEnabled = value; NotifyPropertyChanged(nameof(DeletedCheckboxStateEnabled)); }
        }

        // close action gets set by view
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); }
        }



        public EditCustomerDialogViewModel(bool editModeNewCustomer, string dialogTitle, Customer customer)
        {
            EditModeNewCustomer = editModeNewCustomer;
            Dialogtitle = dialogTitle;
            UpdateCustomer.ActiveCustomer = new Customer(customer);
            UpdateCustomer.OrigCustomer = new Customer(customer);
            ButtonIsDisabled = LoginDialogViewModel.VerfiedUser.UserType == "Sachbearbeiter";

            SaveAsync = AsyncCommand.Create(async () =>
            {
                if (TextBoxFilter.ContainsSpecialChars(UpdateCustomer.ActiveCustomer.CustomerName))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Name überprüfen", "Eingabefehler");
                    return;
                }
                if (TextBoxFilter.ContainsSpecialChars(UpdateCustomer.ActiveCustomer.CustomerCode))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Kundennummer überprüfen", "Eingabefehler");
                    return;
                }
                if (TextBoxFilter.ContainsSpecialChars(UpdateCustomer.ActiveCustomer.KtNumber))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld KT-Nummer überprüfen", "Eingabefehler");
                    return;
                }
                if (TextBoxFilter.ContainsSpecialChars(UpdateCustomer.ActiveCustomer.SummPNumber))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Sammel-P-Nummer überprüfen", "Eingabefehler");
                    return;
                }

                try
                {
                    if (EditModeNewCustomer)
                    {
                        // check if all quired fields are filled
                        if (UpdateCustomer.ActiveCustomer.CustomerName == null ||
                            UpdateCustomer.ActiveCustomer.KtNumber == null ||
                            UpdateCustomer.ActiveCustomer.CustomerCode == null ||
                            UpdateCustomer.ActiveCustomer.SummPNumber == null)
                        {
                            MessageBox.Show("Bitte alle gekennzeichneten Felder korrekt ausfüllen", "Fehler");
                            return;
                        }
                        await BusinessLogicCustomer.InsertCustomer(UpdateCustomer.ActiveCustomer);
                        DialogResult = true;
                    }
                    // Customer Update/Edit Customer
                    else
                    {
                        if (UpdateCustomer.ActiveCustomer.CustomerName == null ||
                            UpdateCustomer.ActiveCustomer.KtNumber == null ||
                            UpdateCustomer.ActiveCustomer.CustomerCode == null ||
                            UpdateCustomer.ActiveCustomer.SummPNumber == null)
                        {
                            MessageBox.Show("Bitte alle gekennzeichneten Felder korrekt ausfüllen", "Fehler");
                            return;
                        }

                        UpdateCustomer.Sgp = new SearchGroupParameter(GroupName, PNumber, GroupCustomerName, SearchGroupIsDeleted, SearchGroupStartDate, SearchGroupEndDate);
                        await BusinessLogicCustomer.UpdateCustomer(UpdateCustomer);
                        DialogResult = true;
                    }
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
                    await ViewModelLocator.CustomerViewModel.DoGetCustomersBySearchParameterAsync.ExecuteAsync(null);
                }
                catch (Exception) { } // Es wird absichtlich nicht verarbeitet da es sich nur um den Refresh handelt
            });

            Refresh = AsyncCommand.Create(async () =>
            {
                try
                {
                    ObservableCollection<Customer> refreshCustomer =
                        await DataAccessService.GetCustomersAsync(
                            new SearchCustomerParameter(UpdateCustomer.ActiveCustomer.CustomerId));
                    Customer[] refreshedCustomer = refreshCustomer.ToArray();
                    UpdateCustomer.ActiveCustomer = refreshedCustomer[0];
                    UpdateCustomer.OrigCustomer = refreshedCustomer[0];
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

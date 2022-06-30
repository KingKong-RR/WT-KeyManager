using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using KeyManager.Commands;
using KeyManager.Commands.AsyncCommands;
using KeyManager.Crypto;
using KeyManager.DataBase;
using KeyManager.Models;
using Microsoft.Win32;

namespace KeyManager.ViewModels
{
    class CsvImportDialogViewModel : ViewModelBase
    {
        //Commands
        public IAsyncCommand SearchCsvFileCustomer { get; }
        public IAsyncCommand ImportCsvFileCustomer { get; }
        public IAsyncCommand SearchCsvFileGroup { get; }
        public IAsyncCommand ImportCsvFileGroup { get; }
        public RelayCommand CloseCsvImportDialog { get; }

        //Properties
        public OpenFileDialog OpenfileDialog = new OpenFileDialog();

        private string _csvFilePathCustomer;
        public string CsvFilePathCustomer
        {
            get { return _csvFilePathCustomer; }
            set { _csvFilePathCustomer = value; NotifyPropertyChanged(nameof(CsvFilePathCustomer)); }
        }

        private string _csvFilePathGroup;
        public string CsvFilePathGroup
        {
            get { return _csvFilePathGroup; }
            set { _csvFilePathGroup = value; NotifyPropertyChanged(nameof(CsvFilePathGroup)); }
        }


        private string _csvImportLog;
        public string CsvImportLog
        {
            get { return _csvImportLog; }
            set { _csvImportLog = value; NotifyPropertyChanged(nameof(CsvImportLog)); }
        }


        // close action gets set by view
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); }
        }

        public CsvImportDialogViewModel()
        {
            SearchCsvFileCustomer = AsyncCommand.Create(async () =>
            {
                OpenfileDialog.InitialDirectory = "c:\\";
                OpenfileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                OpenfileDialog.FilterIndex = 1;
                OpenfileDialog.RestoreDirectory = true;
                OpenfileDialog.ShowDialog();
                CsvFilePathCustomer = OpenfileDialog.FileName;
            });

            ImportCsvFileCustomer = AsyncCommand.Create(async () =>
            {
                    try
                    {
                        CsvImportLog = await Utilities.CustomerCsvImporter.CustomerCsvImporterAsync(CsvFilePathCustomer); 
                    }
                    catch (SqlException e)
                    {
                        MessageBox.Show("Fehler beim schrieben des Datensatzes", "Duplikatfehler");
                        CsvImportLog = "Es konnten keine Betreiber in die Datenbank Importiert werden, da ein Fehler aufgetreten ist.\nÜberprüfen Sie bitte die .csv Datei ob Duplikate enthalten sind und ob die Felder korrekt befüllt sind.\nWenden Sie sich an den Administrator.";
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Unerwarteter Fehler", "Fehler");
                        CsvImportLog = "Es konnten keine Betreiber in die Datenbank Importiert werden, da ein Fehler aufgetreten ist.\nÜberprüfen Sie bitte die .csv Datei ob Duplikate enthalten sind und ob die Felder korrekt befüllt sind.\nWenden Sie sich an den Administrator.";
                    }
            });

            SearchCsvFileGroup = AsyncCommand.Create(async () =>
            {
                OpenfileDialog.InitialDirectory = "c:\\";
                OpenfileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                OpenfileDialog.FilterIndex = 1;
                OpenfileDialog.RestoreDirectory = true;
                OpenfileDialog.ShowDialog();
                CsvFilePathGroup = OpenfileDialog.FileName;
            });

            ImportCsvFileGroup = AsyncCommand.Create(async () =>
            {
                {
                    try
                    {
                        CsvImportLog = await Utilities.GroupCsvImporter.GroupCsvImporterAsync(CsvFilePathGroup);
                    }
                    catch (SqlException e)
                    {
                        MessageBox.Show("Fehler beim schrieben des Datensatzes", "Duplikatfehler");
                        CsvImportLog = "Es konnten keine Gruppen in die Datenbank Importiert werden, da ein Fehler aufgetreten ist.\nÜberprüfen Sie bitte die .csv Datei ob Duplikate enthalten sind und ob die Felder korrekt befüllt sind.\nWenden Sie sich an den Administrator.";
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Unerwarteter Fehler", "Fehler");
                        CsvImportLog = "Es konnten keine Gruppen in die Datenbank Importiert werden, da ein Fehler aufgetreten ist.\nÜberprüfen Sie bitte die .csv Datei ob Duplikate enthalten sind und ob die Felder korrekt befüllt sind.\nWenden Sie sich an den Administrator.";
                    }
                }

            });

            CloseCsvImportDialog = new RelayCommand(() =>
            {
                DialogResult = true;
            });
        }
    }
}

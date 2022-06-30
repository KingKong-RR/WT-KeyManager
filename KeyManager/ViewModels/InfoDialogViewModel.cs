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
    public class InfoDialogViewModel : ViewModelBase
    {

        //Commands
        public RelayCommand CloseInfoDialog { get; }

        //Properties
        // close action gets set by view
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); }
        }

        public string InfoText { get; set; }

        public InfoDialogViewModel()
        {

            InfoText = "Applikation für die Verwaltung von kunden-Codes im Lizenzierung.\n" +
                       "Das Programm ist nur für befugtes Personal.\n" +
                       "Mit Hilfe dieser Applikation können Kunden, Gruppen und Codes erstellt und verwaltet werden.\n\n" +
                       "Copyright by:\n" +
                       "Rexhep Rexhepi\n" +
                       "rexhep.rexhepi@wintool.com\n\n" +
                       "2022";



            CloseInfoDialog = new RelayCommand(() => { DialogResult = true; });
        }
    }
}

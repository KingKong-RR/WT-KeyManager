using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using KeyManager.Models;
using KeyManager.ViewModels;

namespace KeyManager.Utilities
{
    public class CustomerCsvImporter : ViewModelBase
    {
        //Properties
        static string _csvImportLog = "";

        public static  async Task<string> CustomerCsvImporterAsync(string csvFilePathCustomer)
        {
            using (StreamReader csvStreamReaderCustomer = new StreamReader(csvFilePathCustomer))
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        while (!csvStreamReaderCustomer.EndOfStream)
                        {
                            var line = csvStreamReaderCustomer.ReadLine();
                            if (line != null)
                            {
                                var values = line.Split(',');
                                Customer customerToImport = new Customer()
                                {
                                    KtNumber = values[0],
                                    CustomerCode = values[1],
                                    CustomerName = values[2],
                                    SummPNumber = values[3]
                                };

                                await BusinessLogic.BusinessLogicCustomer.InsertCustomer(customerToImport);
                                _csvImportLog += "\n" + line;
                            }
                        }

                        // The Complete method commits the transaction. If an exception has been thrown,
                        // Complete is not  called and the transaction is rolled back.
                        scope.Complete();
                    }

                    _csvImportLog += "\n\nDie Betreiber wurden alle korrekt in die Datenbank importiert, der Gruppenimport kann nun getätigt werden\n\n\n";

                    return _csvImportLog;
                }
                catch (SqlException)
                {                    
                    throw;
                }
                catch (Exception e)
                {
                    throw new Exception(""+e);
                }
            }


        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using KeyManager.Crypto;
using KeyManager.DataBase;
using KeyManager.Models;
using KeyManager.ViewModels;

namespace KeyManager.Utilities
{
    public class GroupCsvImporter : ViewModelBase
    {
        //Properties
        static string _csvImportLog = "";

        public static async Task<string> GroupCsvImporterAsync(string csvFilePathGroup)
        {
            using (StreamReader csvStreamReaderGroup = new StreamReader(csvFilePathGroup))
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        while (!csvStreamReaderGroup.EndOfStream)
                        {
                            var line = csvStreamReaderGroup.ReadLine();
                            if (line != null)
                            {
                                var values = line.Split(',');
                                Aes codeToEncrypt = new Aes();

                                ECode eCodeToImport = new ECode()
                                {
                                    Code = values[3],
                                    ECodeServiceCardId = Int32.Parse(values[4])
                                };

                                UCode uCodeToImport = new UCode()
                                {
                                    Code = values[5],
                                    UCodeServiceCardId = Int32.Parse(values[6])
                                };

                                Group groupToImport = new Group()
                                {
                                    GroupName = values[0],
                                    PNumber = values[1],
                                    GroupCustomerName = values[2]
                                };

                                var tempECodeToImport = codeToEncrypt.Encrypt(eCodeToImport.Code);
                                eCodeToImport.Code = tempECodeToImport;
                                var eCodeExists = await DataAccessService.CheckECodeExistsAsync(tempECodeToImport);
                                if (eCodeExists)
                                {
                                    MessageBox.Show("E-Code Existiert bereits.", "Fehler");
                                    throw new Exception("E-Code Existiert bereits.");
                                }

                                eCodeToImport.ECodeCreationDate = DateTime.Now;
                                eCodeToImport.ECodeIsDeleted = false;
                                groupToImport.GroupECodeId =
                                    await DataAccessService.InsertECodeAsync(eCodeToImport, true);

                                var tempUCodeToImport = codeToEncrypt.Encrypt(uCodeToImport.Code);
                                uCodeToImport.Code = tempUCodeToImport;
                                var uCodeExists = await DataAccessService.CheckUCodeExistsAsync(tempUCodeToImport);
                                if (uCodeExists)
                                {
                                    MessageBox.Show("U-Code Existiert bereits.", "Fehler");
                                    throw new Exception();
                                }

                                uCodeToImport.UCodeCreationDate = DateTime.Now;
                                uCodeToImport.UCodeIsDeleted = false;
                                groupToImport.GroupUCodeId =
                                    await DataAccessService.InsertUCodeAsync(uCodeToImport, true);

                                var tempCustomer =
                                    (await DataAccessService.GetCustomersAsyncForCsvImport(
                                        new SearchCustomerParameter(groupToImport.GroupCustomerName)))[0];
                                groupToImport.GroupCustomerId = tempCustomer.CustomerId;
                                groupToImport.GroupCustomerIsDeleted = false;
                                groupToImport.GroupCreationDate = DateTime.Now;
                                groupToImport.GroupStatusCheck = 1;

                                await DataAccessService.InsertGroupAsync(groupToImport, true);
                                _csvImportLog += "\n" + line;
                            }
                        }

                        // The Complete method commits the transaction. If an exception has been thrown,
                        // Complete is not  called and the transaction is rolled back.
                        scope.Complete();
                    }

                    _csvImportLog +=
                        "\n\nDie Gruppen wurden alle korrekt in die Datenbank importiert, Jetzt ann das Tool normal genutzt werden.";

                    return _csvImportLog;
                }
                catch (SqlException e)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new Exception("" + e);
                }
            }

        }
    }
}
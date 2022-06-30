using System;
using System.IO.Packaging;
using System.Threading.Tasks;
using KeyManager.Crypto;
using log4net;
using KeyManager.DataBase;
using KeyManager.Utilities;

namespace KeyManager.BusinessLogic
{
    public class BusinessLogicECode
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataAccessService));

        // Genereate ECode and Return it
        public static async Task<string> GenerateUniqueECodeAsync()
        {
            try
            {
                bool exists;
                string tempEcodePlain;
                string tempEcodeCrypted;
                Aes tempECodeToCheck = new Aes();

                do
                {
                    tempEcodePlain = CharGenerator.GetRandomChars(8);
                    tempEcodeCrypted = tempECodeToCheck.Encrypt(tempEcodePlain);
                    exists = await DataAccessService.CheckECodeExistsAsync(tempEcodeCrypted);

                } while (exists);

                return tempEcodePlain;
            }
            catch (Exception e)
            {
                Log.Error("Could not Generate ECode" + e);
                throw;
            }
        }
    }
}
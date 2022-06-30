using System;
using System.Threading.Tasks;
using KeyManager.Crypto;
using log4net;
using KeyManager.DataBase;
using KeyManager.Utilities;

namespace KeyManager.BusinessLogic
{
    public class BusinessLogicUCode
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataAccessService));
        // Genereate UCode and Return it
        public static async Task<string> GenerateUniqueUCodeAsync()
        {
            try
            {
                bool exists;
                string tempUcodePlain;
                string tempUcodeCrypted;
                Aes tempUCodeToCheck = new Aes();

                do
                {
                    tempUcodePlain = CharGenerator.GetRandomChars(8);
                    tempUcodeCrypted = tempUCodeToCheck.Encrypt(tempUcodePlain);
                    exists = await DataAccessService.CheckUCodeExistsAsync(tempUcodeCrypted);

                } while (exists);

                return tempUcodePlain;
            }
            catch (Exception e)
            {
                Log.Error("Could not Generate UCode" + e);
                throw;
            }
        }
    }
}
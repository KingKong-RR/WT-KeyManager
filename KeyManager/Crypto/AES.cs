using System;
using System.Security.Cryptography;
using System.Text;
using KeyManager.DataBase;
using log4net;

namespace KeyManager.Crypto
{
    public class Aes
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataAccessService));

        AesCryptoServiceProvider crypto_provider;

        //Key is static, because it is needed to encrypt and decrypt the E-Codes, U-Codes and Customercodes
        private static string key = "qwertzuiopüäölkjhgfdsayxcvbnmkzg";

        // The IV is a string like the Salt in the Hashing, it is static too because it is needet to encrypt and decrypt  the E-Codes, U-Codes and Customercodes
        private static string IV = "poiuztgdewahvdly";

        public Aes()
        {

            crypto_provider = new AesCryptoServiceProvider();

            crypto_provider.BlockSize = 128;
            crypto_provider.KeySize = 256;
            crypto_provider.Key = Encoding.ASCII.GetBytes(key);
            crypto_provider.IV = Encoding.ASCII.GetBytes(IV);
            crypto_provider.Mode = CipherMode.CBC;
            crypto_provider.Padding = PaddingMode.PKCS7;

        }

        //Here is the encrypting procedure, so here would be the codes crypted to save them in the DB
        public String Encrypt(String plainCode)
        {
            try
            {
                ICryptoTransform transform = crypto_provider.CreateEncryptor();

                byte[] encryptedBytes = transform.TransformFinalBlock(Encoding.ASCII.GetBytes(plainCode), 0, plainCode.Length);

                string enCryptedCode = Convert.ToBase64String(encryptedBytes);
                return enCryptedCode;
            }
            catch (Exception e)
            {
                Log.Error("Could not encrypt" + e);
                throw;
            }
        }

        //Here is the decrypting procedure, so here would be the codes decrypted to show the im the GUI
        public String Decrypt(String encryptedCode)
        {
            try
            {
                ICryptoTransform transform = crypto_provider.CreateDecryptor();

                byte[] encryptedBytes = Convert.FromBase64String(encryptedCode);
                byte[] decryptedBytes = transform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                string decryptedCode = Encoding.ASCII.GetString(decryptedBytes);
                return decryptedCode;
            }
            catch (Exception e)
            {
                Log.Error("Could not deCrypt" + e);
                throw;
            }
        }
    }
}
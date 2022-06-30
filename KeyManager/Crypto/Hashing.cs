using System;
using KeyManager.DataBase;
using log4net;

namespace KeyManager.Crypto
{
    public class Hashing
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataAccessService));

        //GEnerates a Secure Salt for the PAssword
        private static string GenerateSalt()
        {
            try
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt(11);
                return salt;
            }
            catch (Exception e)
            {
                Log.Error("Could not generate the Salt" + e);
                throw;
            }
        }

        //Hash the password includet the random generated salt by BCrypt
        //returns the hashed Password
        public static string HashPassword(string password)
        {
            try
            {
                var hashedPwd = BCrypt.Net.BCrypt.HashPassword(password, GenerateSalt());
                return hashedPwd;
            }
            catch (Exception e)
            {
                Log.Error("Could not Hash the Password" + e);
                throw;
            }
        }

        //Compare the plain gived password with the correct hased password
        public static bool ValidatePassword(string password, string correctHash)
        {
            try
            {
                var verifiedPwd = BCrypt.Net.BCrypt.Verify(password, correctHash);
                return verifiedPwd;
            }
            catch (Exception e)
            {
                Log.Error("Could not validate the Password" + e);
                throw;
            }
        }
    }
}

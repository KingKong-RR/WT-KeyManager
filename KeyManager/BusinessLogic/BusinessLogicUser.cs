using System;
using System.Threading.Tasks;
using log4net;
using KeyManager.Crypto;
using KeyManager.DataBase;
using KeyManager.Models;

namespace KeyManager.BusinessLogic
{
    public class BusinessLogicUser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataAccessService));

        public static async Task InsertUser(UpdateUser insertUser)
        {
            try
            {

                // Insert a new Customer
                insertUser.ActiveUser.UserCreated = DateTime.Now;
                insertUser.ActiveUser.UserTypeId = insertUser.SelectedComboBoxItem.UserTypeId;
                var tempHasedPwd = Hashing.HashPassword(insertUser.ActiveUser.Password);
                insertUser.ActiveUser.Password = tempHasedPwd;
                insertUser.ActiveUser.UserStatusCheck = 1;
                await DataAccessService.InsertUserAsync(insertUser.ActiveUser, true);
            }
            catch (Exception e)
            {
                Log.Error("Could not InsertUser in DB" + e);
                throw;
            }
        }

        public static async Task UpdateUser(UpdateUser updateUser)
        {
            try
            {
                // Get the UpdateStatus from the Db.
                var tempStatusCheck = await DataAccessService.GetStatusCheckAsync(updateUser.ActiveUser.UserId, "user");
                // Check the Update status, when they are the same than update the group else throw an exception
                if (tempStatusCheck == updateUser.OrigUser.UserStatusCheck)
                {
                    if (updateUser.ActiveUser.Password != null)
                    {
                        var tempHasedPwd = Hashing.HashPassword(updateUser.ActiveUser.Password);
                        updateUser.ActiveUser.Password = tempHasedPwd;
                    }
                    updateUser.ActiveUser.UserType = updateUser.SelectedComboBoxItem.UserTypeName;
                    updateUser.ActiveUser.UserTypeId = updateUser.SelectedComboBoxItem.UserTypeId;
                    if (updateUser.ActiveUser.UserName != updateUser.OrigUser.UserName || updateUser.ActiveUser.UserType != updateUser.OrigUser.UserType || updateUser.ActiveUser.UserIsDeleted != updateUser.OrigUser.UserIsDeleted || updateUser.ActiveUser.Password != null)
                    {
                        updateUser.ActiveUser.UserStatusCheck += 1;
                    }

                    updateUser.ActiveUser.UserTypeId = updateUser.SelectedComboBoxItem.UserTypeId;
                    await DataAccessService.UpdateUserAsync(updateUser.ActiveUser, updateUser.OrigUser, true);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not Update User in DB" + e);
                throw;
            }

        }
    }
}
using System;
using System.Threading.Tasks;
using log4net;
using KeyManager.Crypto;
using KeyManager.DataBase;
using KeyManager.Models;

namespace KeyManager.BusinessLogic
{
    public class BusinessLogicGroup
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataAccessService));

        public static async Task InsertGroup(Group newGroup)
        {
            // Insert a new Group by Customer ID
            var currDateTime = DateTime.Now;

            ECode newEcode = new ECode();
            UCode newUcode = new UCode();
            Aes codeAes = new Aes();

            string tempEcodeString = await BusinessLogicECode.GenerateUniqueECodeAsync();
            string tempUcodeString = await BusinessLogicUCode.GenerateUniqueUCodeAsync();

            newGroup.GroupStatusCheck = 1;

            newEcode.Code = codeAes.Encrypt(tempEcodeString);
            newUcode.Code = codeAes.Encrypt(tempUcodeString);

            newEcode.ECodeServiceCardId = 1;
            newUcode.UCodeServiceCardId = 1;

            newUcode.UCodeCreationDate = currDateTime;
            newEcode.ECodeCreationDate = currDateTime;

            newGroup.GroupECodeId = await DataAccessService.InsertECodeAsync(newEcode, true);
            newGroup.GroupUCodeId = await DataAccessService.InsertUCodeAsync(newUcode, true);

            newGroup.GroupCreationDate = currDateTime;

            await DataAccessService.InsertGroupAsync(newGroup, true);
        }

        public static async Task UpdateGroup(UpdateGroup updateGroup)
        {
            var currDateTime = DateTime.Now;

            try
            {
                // Get the UpdateStatus from the Db.
                var tempStatusCheck = await DataAccessService.GetStatusCheckAsync(updateGroup.ActiveGroup.GroupId, "group");
                // Check the Update status, when they are the same than update the group else throw an exception
                if (tempStatusCheck == updateGroup.OrigGroup.GroupStatusCheck)
                {
                    // When Checkbox = true, generate a new ECode, the old one is set as Deleted
                    if (updateGroup.ECodeCheckBoxChecked)
                    {
                        ECode newEcode = new ECode
                        {
                            ECodeId = updateGroup.ActiveGroup.GroupECodeId,
                            ECodeDeletedDate = currDateTime,
                            ECodeIsDeleted = true
                        };
                        Aes codeAes = new Aes();
                        await DataAccessService.SetECodeDeletedAsync(newEcode, true);
                        string tempEcodeString = await BusinessLogicECode.GenerateUniqueECodeAsync();
                        var tempEcodeEncrypt = codeAes.Encrypt(tempEcodeString);
                        newEcode.Code = tempEcodeEncrypt;
                        newEcode.ECodeServiceCardId = updateGroup.ActiveGroup.GroupECodeServiceCardId + 1;
                        newEcode.ECodeCreationDate = currDateTime;
                        updateGroup.ActiveGroup.GroupECodeId = await DataAccessService.InsertECodeAsync(newEcode, true);
                    }

                    if (updateGroup.UCodeCheckBoxChecked)
                    {
                        UCode newUcode = new UCode
                        {
                            UCodeId = updateGroup.ActiveGroup.GroupUCodeId,
                            UCodeDeletedDate = currDateTime,
                            UCodeIsDeleted = true
                        };
                        Aes codeAes = new Aes();
                        await DataAccessService.SetUCodeDeletedAsync(newUcode, true);
                        string tempUcodeString = await BusinessLogicUCode.GenerateUniqueUCodeAsync();
                        var tempUCodeEncrypt = codeAes.Encrypt(tempUcodeString);
                        newUcode.Code = tempUCodeEncrypt;
                        newUcode.UCodeServiceCardId = updateGroup.ActiveGroup.GroupUCodeServiceCardId + 1;
                        newUcode.UCodeCreationDate = currDateTime;
                        updateGroup.ActiveGroup.GroupUCodeId = await DataAccessService.InsertUCodeAsync(newUcode, true);
                    }

                    if (updateGroup.ActiveGroup.GroupIsDeleted)
                        await BusinessLogicGroup.DeleteGroupAsync(updateGroup.ActiveGroup, updateGroup.OrigGroup);

                    else
                    {
                        if ((updateGroup.ActiveGroup.GroupIsDeleted == false && updateGroup.ECodeCheckBoxChecked == false) || (updateGroup.ActiveGroup.GroupIsDeleted == false && updateGroup.UCodeCheckBoxChecked == false))
                        {
                            await DataAccessService.SetECodeUnDeletedAsync(updateGroup.ActiveGroup.GroupECodeId, true);
                            await DataAccessService.SetUCodeUnDeletedAsync(updateGroup.ActiveGroup.GroupUCodeId, true);
                        }

                        updateGroup.ActiveGroup.GroupStatusCheck += 1;
                        await DataAccessService.UpdateGroupAsync(updateGroup.ActiveGroup, updateGroup.OrigGroup, true);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not Update Group in DB" + e);
                throw;
            }
        }

        //E-Code und U-Code muss gelöscht werden -> ECode und UCode werden über ihre ID welche in der Gruppe verlinkt ist ausgelesen und Ugedatet
        //Gruppe muss gelöscht werden -> Gruppe wird eingelesen und Upgadetet zu gelöscht
        public static async Task DeleteGroupAsync(Group group, Group origGroup)
        {
            try
            {

                var currDateTime = DateTime.Now;

                ECode newEcode = new ECode
                {
                    ECodeServiceCardId = group.GroupECodeServiceCardId,
                    Code = group.GroupECode,
                    ECodeId = group.GroupECodeId,
                    ECodeDeletedDate = currDateTime,
                    ECodeIsDeleted = true
                };
                await DataAccessService.SetECodeDeletedAsync(newEcode, true);

                UCode newUcode = new UCode
                {
                    UCodeServiceCardId = group.GroupUCodeServiceCardId,
                    Code = group.GroupUCode,
                    UCodeId = group.GroupUCodeId,
                    UCodeDeletedDate = currDateTime,
                    UCodeIsDeleted = true
                };
                await DataAccessService.SetUCodeDeletedAsync(newUcode, true);

                if (origGroup == null)
                    origGroup = new Group(group);

                group.GroupDeletedDate = currDateTime;
                group.GroupIsDeleted = true;

                await DataAccessService.UpdateGroupAsync(group, origGroup, true);

            }
            catch (Exception e)
            {
                Log.Error("Could not Delete Group" + e);
                throw;
            }
        }
    }
}
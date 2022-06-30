using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using log4net;
using KeyManager.DataBase;
using KeyManager.Exceptions;
using KeyManager.Models;

namespace KeyManager.BusinessLogic
{
    public class BusinessLogicCustomer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataAccessService));
        
        public static async Task InsertCustomer(Customer newCustomer)
        {
            try
            {
                // Insert a new Customer
                var currDateTime = DateTime.Now;
                newCustomer.CustomerCreationDate = currDateTime;
                newCustomer.CustomerStatusCheck = 1;
                await DataAccessService.InsertCustomerAsync(newCustomer, true);
            }
            catch (SqlException e)
            {
                throw new ConflictException("Duplikatfehler: " + e);
            }
            catch (Exception e)
            {
                throw new Exception(""+e);
            }
        }

        public static async Task UpdateCustomer(UpdateCustomer updateCustomer)
        {
            try
            {
                // Get the UpdateStatus from the Db.
                var tempStatusCheck = await DataAccessService.GetStatusCheckAsync(updateCustomer.ActiveCustomer.CustomerId, "customer");
                // Check the Update status, when they are the same than update the group else throw an exception
                if (tempStatusCheck == updateCustomer.OrigCustomer.CustomerStatusCheck)
                {
                    //If User delets Customer
                    if (updateCustomer.OrigCustomer.CustomerIsDeleted != updateCustomer.ActiveCustomer.CustomerIsDeleted &&
                    updateCustomer.ActiveCustomer.CustomerIsDeleted)
                    {
                        updateCustomer.Sgp.GroupCustomerId = updateCustomer.ActiveCustomer.CustomerId;
                        var resultGroups = await DataAccessService.GetGroupsAsync(updateCustomer.Sgp);
                        foreach (var group in resultGroups)
                        {
                            await BusinessLogicGroup.DeleteGroupAsync(group, null);
                        }

                        updateCustomer.ActiveCustomer.CustomerDeletedDate = DateTime.Now;
                    }

                    updateCustomer.ActiveCustomer.CustomerStatusCheck+=1;
                    await DataAccessService.UpdateCustomerAsync(updateCustomer.ActiveCustomer, updateCustomer.OrigCustomer, true);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not Update Customer in DB" + e);
                throw;
            }
        }

    }
}
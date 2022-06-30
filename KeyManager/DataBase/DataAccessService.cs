using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Navigation;
using KeyManager.Bootstrap;
using log4net;
using KeyManager.Crypto;
using KeyManager.Exceptions;
using KeyManager.Models;
using KeyManager.Utilities;
using KeyManager.ViewModels;

namespace KeyManager.DataBase
{
    public class DataAccessService
    {
        static DbFieldLengths dbFieldLengths = new DbFieldLengths();

        private static readonly ILog Log = LogManager.GetLogger(typeof(DataAccessService));

        private static readonly string ConnectionString = $"Data Source={Config.DataSource};Initial Catalog={Config.InitialCatalogue};User id={Config.User};Password={Config.Password}";

        //SQL-FehlerCodes
        public static int CannotInsertDuplicateKeyRow = 2601;
        public static int CannotInsertDuplicateKeyInObject = 2627;

        public static async Task LogQueryAsync(SqlCommand command, string what, DateTime time)
        {

            string activitLogQuery = $"INSERT INTO [activitylog] ([Time], [UserID], [What]) VALUES ('{time}', '{LoginDialogViewModel.VerfiedUser.UserId}', '{what}')";
            command.CommandText = activitLogQuery;
            await command.ExecuteNonQueryAsync();
        }

        public static async Task<ObservableCollection<Customer>> GetCustomersAsync(SearchCustomerParameter scp)
        {
            try
            {

                string searchByCustomerId = "";

                string isDeletedQuery = "";
                string searchBetweenDates = "";

                if (scp.CustomerId != 0) searchByCustomerId = $" AND [ID] = {scp.CustomerId}";

                if (scp.CustomerDeleted == 1) isDeletedQuery = "1";
                else if (scp.CustomerDeleted == 2) isDeletedQuery = "0";

                if (scp.CustomerStartDate != null && scp.CustomerEndDate != null)
                {
                    searchBetweenDates += $" AND ([DeletedDate] BETWEEN '{scp.CustomerStartDate}' AND '{scp.CustomerEndDate}' OR [CreationDate] BETWEEN '{scp.CustomerStartDate}' AND '{scp.CustomerEndDate}')";
                }
                else
                {
                    searchBetweenDates = "";
                }

                var query = $"SELECT [ID], [KTNumber], [CustomerCode], [Name], [CreationDate], [DeletedDate], [IsDeleted], [SummPNumber], [StatusCheck] FROM [customer] WHERE [Name] LIKE '%'+'{scp.CustomerName}'+'%' AND [KTNumber] LIKE '%'+'{scp.KtNumber}'+'%' AND [CustomerCode] LIKE '%'+'{scp.CustomerCode}'+'%' AND [SummPNumber] LIKE '%'+'{scp.SummPNumber}'+'%' AND [IsDeleted] LIKE '%{isDeletedQuery}%'{searchBetweenDates}{searchByCustomerId}";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    ObservableCollection<Customer> customerList = new ObservableCollection<Customer>();
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var resultCustomer = new Customer
                                {
                                    CustomerId = (int)reader["ID"],
                                    CustomerName = (string)reader["Name"],
                                    CustomerCode = (string)reader["CustomerCode"],
                                    SummPNumber = (string)reader["SummPNumber"],
                                    KtNumber = (string)reader["KTNumber"],
                                    CustomerCreationDate = (DateTime)reader["CreationDate"],
                                    CustomerIsDeleted = (bool)reader["IsDeleted"],
                                    CustomerDeletedDate = reader["DeletedDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DeletedDate"],
                                    CustomerStatusCheck = (int)reader["StatusCheck"],
                                };
                                customerList.Add(resultCustomer);
                            }

                            if (customerList.Count == 0)
                            {
                                throw new NotFoundException("Datensatz auf der DB nicht gefunden");
                            }
                            return customerList;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error" + e);
                throw;
            }
        }

        public static async Task<ObservableCollection<Customer>> GetCustomersAsyncForCsvImport(SearchCustomerParameter scp)
        {
            try
            {

                string searchByCustomerId = "";

                string isDeletedQuery = "";
                string searchBetweenDates = "";

                if (scp.CustomerId != 0) searchByCustomerId = $" AND [ID] = {scp.CustomerId}";

                if (scp.CustomerDeleted == 1) isDeletedQuery = "1";
                else if (scp.CustomerDeleted == 2) isDeletedQuery = "0";

                if (scp.CustomerStartDate != null && scp.CustomerEndDate != null)
                {
                    searchBetweenDates += $" AND ([DeletedDate] BETWEEN '{scp.CustomerStartDate}' AND '{scp.CustomerEndDate}' OR [CreationDate] BETWEEN '{scp.CustomerStartDate}' AND '{scp.CustomerEndDate}')";
                }
                else
                {
                    searchBetweenDates = "";
                }

                var query = $"SELECT [ID], [KTNumber], [CustomerCode], [Name], [CreationDate], [DeletedDate], [IsDeleted], [SummPNumber], [StatusCheck] FROM [customer] WHERE [Name] = '{scp.CustomerName}' AND [KTNumber] LIKE '%'+'{scp.KtNumber}'+'%' AND [CustomerCode] LIKE '%'+'{scp.CustomerCode}'+'%' AND [SummPNumber] LIKE '%'+'{scp.SummPNumber}'+'%' AND [IsDeleted] LIKE '%{isDeletedQuery}%'{searchBetweenDates}{searchByCustomerId}";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    ObservableCollection<Customer> customerList = new ObservableCollection<Customer>();
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var resultCustomer = new Customer
                                {
                                    CustomerId = (int)reader["ID"],
                                    CustomerName = (string)reader["Name"],
                                    CustomerCode = (string)reader["CustomerCode"],
                                    SummPNumber = (string)reader["SummPNumber"],
                                    KtNumber = (string)reader["KTNumber"],
                                    CustomerCreationDate = (DateTime)reader["CreationDate"],
                                    CustomerIsDeleted = (bool)reader["IsDeleted"],
                                    CustomerDeletedDate = reader["DeletedDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DeletedDate"],
                                    CustomerStatusCheck = (int)reader["StatusCheck"],
                                };
                                customerList.Add(resultCustomer);
                            }

                            if (customerList.Count == 0)
                            {
                                throw new NotFoundException("Datensatz auf der DB nicht gefunden");
                            }
                            return customerList;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error" + e);
                throw;
            }
        }


        public static async Task UpdateCustomerAsync(Customer customer, Customer origCustomer, bool writeInActivityLog)
        {

            try
            {
                var query = $"UPDATE [customer] SET [Name] = @name, [KTNumber] = @ktnumber, [CustomerCode] = @customercode, [SummPNumber] = @summpnumber, [StatusCheck] = @statuscheck, [IsDeleted] = @isdeleted {(customer.CustomerIsDeleted ? ", [DeletedDate] = @deleteddate" : ", [DeletedDate] = NULL")} WHERE [ID] = @id ";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@name", SqlDbType.NVarChar, dbFieldLengths.NameLn).Value = customer.CustomerName;
                        command.Parameters.Add("@ktnumber", SqlDbType.NVarChar, dbFieldLengths.KtNumberLn).Value = customer.KtNumber;
                        command.Parameters.Add("@customercode", SqlDbType.NVarChar, dbFieldLengths.CustomerCodeLn).Value = customer.CustomerCode;
                        command.Parameters.Add("@summpnumber", SqlDbType.NVarChar, dbFieldLengths.CustomerCodeLn).Value = customer.SummPNumber;
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = customer.CustomerIsDeleted;
                        if (customer.CustomerIsDeleted) command.Parameters.Add("@deleteddate", SqlDbType.DateTime).Value = customer.CustomerDeletedDate;
                        command.Parameters.Add("@id", SqlDbType.Int).Value = customer.CustomerId;
                        command.Parameters.Add("@statuscheck", SqlDbType.Int).Value = customer.CustomerStatusCheck;

                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"Betreiber={customer.CustomerName} mit ID={customer.CustomerId} aktualisiert; {CompareForActivityLog.CompareCustomer(origCustomer, customer)}", DateTime.Now);

                        if (successfullCount != 1)
                            throw new Exception("Could not Update the Customer in DB");
                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 2601 || e.Number == 2627)
                {
                    throw new ConflictException("Duplikatfehler");
                }
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
                throw new Exception(""+e);
            }
        }


        public static async Task InsertCustomerAsync(Customer customer, bool writeInActivityLog)
        {
            try
            {
                var query =
                    "INSERT INTO [customer] ([KTNumber], [CustomerCode], [Name], [CreationDate], [IsDeleted], [SummPNumber], [StatusCheck]) VALUES (@ktnumber, @customercode, @name, @creationdate, @isdeleted, @summpnumber, @statuscheck)";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ktnumber", SqlDbType.NVarChar, dbFieldLengths.KtNumberLn).Value =
                            customer.KtNumber;
                        command.Parameters.Add("@customercode", SqlDbType.NVarChar, dbFieldLengths.CustomerCodeLn).Value
                            = customer.CustomerCode;
                        command.Parameters.Add("@name", SqlDbType.NVarChar).Value = customer.CustomerName;
                        command.Parameters.Add("@creationdate", SqlDbType.DateTime).Value =
                            customer.CustomerCreationDate;
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = customer.CustomerIsDeleted;
                        command.Parameters.Add("@summpnumber", SqlDbType.NVarChar, dbFieldLengths.PNumberLn).Value =
                            customer.SummPNumber;
                        command.Parameters.Add("@statuscheck", SqlDbType.Int).Value = customer.CustomerStatusCheck;

                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"Betreiber={customer.CustomerName} wurde eingefügt;",
                                customer.CustomerCreationDate);

                        if (successfullCount != 1)
                            throw new Exception("Could not Save the Customer in DB");
                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Number == CannotInsertDuplicateKeyRow || e.Number == CannotInsertDuplicateKeyInObject)
                {
                    throw new ConflictException("Duplikatfehler");
                }
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
            }
        }

        public static async Task<ObservableCollection<Group>> GetGroupsAsync(SearchGroupParameter sgp)
        {
            try
            {
                string searchByGroupId = "";

                string isDeletedQuery = "";
                string searchBetweenDates = "";

                if (sgp.GroupId != 0) searchByGroupId = $" AND [group].[ID] = {sgp.GroupId}";

                if (sgp.GroupDeleted == 1) isDeletedQuery = "1";
                else if (sgp.GroupDeleted == 2) isDeletedQuery = "0";

                if (sgp.GroupStartDate != null && sgp.GroupEndDate != null)
                {
                    searchBetweenDates += $" AND ([group].[DeletedDate] BETWEEN '{sgp.GroupStartDate}' AND '{sgp.GroupEndDate}' OR [group].[CreationDate] BETWEEN '{sgp.GroupStartDate}' AND '{sgp.GroupEndDate}')";
                }
                else
                {
                    searchBetweenDates = "";
                }

                var addWhereClauseForCustId = "";
                //Wenn die customer ID -1 hat also leer ist wird die Methode von GetCustomersAsynch ohne customer ID aufgerufen sonst Die mit.
                if (sgp.GroupCustomerId != -1)
                {
                    addWhereClauseForCustId = " AND [customer].[ID] LIKE '" + sgp.GroupCustomerId + "'";
                }

                var query = $"SELECT [group].[ID], [group].[Name], [group].[PNumber], [group].[CreationDate], [group].[DeletedDate], [group].[IsDeleted], [group].[customerID], [group].[StatusCheck], [customer].[Name] AS [CustomerName], [customer].[IsDeleted] AS [GroupCustomerIsDeleted], [group].[ECodeID], [ecode].[ECode], [ecode].[ECodeServiceCardID], [group].[UCodeID], [ucode].[UCode], [ucode].[UCodeServiceCardID] FROM [group] INNER JOIN [customer] ON [group].[CustomerID]=[customer].[ID] INNER JOIN [ecode] ON [group].[ECodeID]=[ecode].[ID] INNER JOIN [ucode] ON [group].[UCodeID]=[ucode].[ID] WHERE [group].[Name] LIKE '%'+'{sgp.GroupName}'+'%' AND [customer].[Name] LIKE '%'+'{sgp.GroupCustomerName}'+'%' AND [group].[PNumber] LIKE '%'+'{sgp.PNumber}'+'%' AND [group].[IsDeleted] LIKE '%{isDeletedQuery}%' {addWhereClauseForCustId} {searchBetweenDates} {searchByGroupId}";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    ObservableCollection<Group> groupList = new ObservableCollection<Group>();
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var resultGroup = new Group
                                {
                                    GroupId = (int)reader["ID"],
                                    GroupName = (string)reader["Name"],
                                    PNumber = (string)reader["PNumber"],
                                    GroupCreationDate = (DateTime)reader["CreationDate"],
                                    GroupIsDeleted = (bool)reader["IsDeleted"],
                                    GroupCustomerId = (int)reader["CustomerID"],
                                    GroupCustomerName = (string)reader["CustomerName"],
                                    GroupCustomerIsDeleted = (bool)reader["GroupCustomerIsDeleted"],
                                    GroupECodeId = (int)reader["ECodeID"],
                                    GroupECode = (string)reader["ECode"],
                                    GroupECodeServiceCardId = (int)reader["ECodeServiceCardID"],
                                    GroupUCodeId = (int)reader["UCodeID"],
                                    GroupUCode = (string)reader["UCode"],
                                    GroupUCodeServiceCardId = (int)reader["UCodeServiceCardID"],
                                    GroupStatusCheck = (int)reader["StatusCheck"],
                                    GroupDeletedDate = reader["DeletedDate"] == DBNull.Value
                                        ? (DateTime?)null
                                        : (DateTime)reader["DeletedDate"]
                                };
                                Aes codeAes = new Aes();
                                var tempDecryptedECode = codeAes.Decrypt(resultGroup.GroupECode);
                                var tempDecryptedUCode = codeAes.Decrypt(resultGroup.GroupUCode);
                                resultGroup.GroupECode = tempDecryptedECode;
                                resultGroup.GroupUCode = tempDecryptedUCode;
                                groupList.Add(resultGroup);
                            }

                            if (groupList.Count == 0)
                            {
                                throw new NotFoundException("Datensatz auf der DB nicht gefunden");
                            }
                            return groupList;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read the Group from DB" + e);
                throw;
            }
        }

        public static async Task UpdateGroupAsync(Group group, Group origGroup, bool writeInActivityLog)
        {
            try
            {
                var query = $"UPDATE [group] SET [Name] = @name, [PNumber] = @pnumber, [IsDeleted] = @isdeleted, [ECodeID] = @ecodeid, [StatusCheck] = @statuscheck, [UCodeID] = @ucodeid {(group.GroupIsDeleted ? ", [DeletedDate] = @deleteddate" : ", [DeletedDate] = NULL")} WHERE [ID] = @id";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@name", SqlDbType.NVarChar, dbFieldLengths.NameLn).Value = group.GroupName;
                        command.Parameters.Add("@pnumber", SqlDbType.NVarChar, dbFieldLengths.PNumberLn).Value = group.PNumber;
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = group.GroupIsDeleted;
                        command.Parameters.Add("@ecodeid", SqlDbType.Int).Value = group.GroupECodeId;
                        command.Parameters.Add("@ucodeid", SqlDbType.Int).Value = group.GroupUCodeId;
                        if (group.GroupIsDeleted) command.Parameters.Add("@deleteddate", SqlDbType.DateTime).Value = group.GroupDeletedDate;
                        command.Parameters.Add("@id", SqlDbType.Int).Value = group.GroupId;
                        command.Parameters.Add("@statuscheck", SqlDbType.Int).Value = group.GroupStatusCheck;


                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"Gruppe={group.GroupName} mit ID={group.GroupId} aktualisiert; {CompareForActivityLog.CompareGroup(origGroup, group)}", DateTime.Now);

                        if (successfullCount != 1)
                            throw new Exception("Could not Update the Group in DB");

                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Number == CannotInsertDuplicateKeyRow || e.Number == CannotInsertDuplicateKeyInObject)
                {
                    throw new ConflictException("Duplikatfehler");
                }
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
            }
        }

        public static async Task InsertGroupAsync(Group group, bool writeInActivityLog)
        {
            try
            {
                var query = "INSERT INTO [group] ([Name], [PNumber], [CreationDate], [IsDeleted], [ECodeID], [UCodeID], [CustomerID], [StatusCheck]) VALUES ( @name, @pnumber, @creationdate, @isdeleted, @ecodeid, @ucodeid, @customerid, @statuscheck)";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@name", SqlDbType.NVarChar, dbFieldLengths.NameLn).Value = group.GroupName;
                        command.Parameters.Add("@pnumber", SqlDbType.NVarChar, dbFieldLengths.PNumberLn).Value = group.PNumber;
                        command.Parameters.Add("@creationdate", SqlDbType.DateTime).Value = group.GroupCreationDate;
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = group.GroupIsDeleted;
                        command.Parameters.Add("@ecodeid", SqlDbType.Int).Value = group.GroupECodeId;
                        command.Parameters.Add("@ucodeid", SqlDbType.Int).Value = group.GroupUCodeId;
                        command.Parameters.Add("@customerid", SqlDbType.Int).Value = group.GroupCustomerId;
                        command.Parameters.Add("@statuscheck", SqlDbType.Int).Value = group.GroupStatusCheck;

                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"Gruppe={group.GroupName} wurde eingefügt;", group.GroupCreationDate);

                        if (successfullCount != 1)
                            throw new Exception("Could not save the Group in DB");
                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 2601 || e.Number == 2627)
                {
                    throw new ConflictException("Duplikatfehler");
                }
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
            }
        }

        public static async Task<DbUser> GetDbUserAsync(string userName)
        {
            try
            {
                var resultDbUser = new DbUser();

                var query = $"SELECT [user].[ID], [user].[UserName], [user].[Password], [user].[UserTypeID], [user].[StatusCheck], [usertype].[UserType] FROM [user] INNER JOIN [usertype] ON [user].[UserTypeID]=[usertype].[ID] WHERE [user].[UserName] = '{userName}'";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            resultDbUser.UserId = (int)reader["ID"];
                            resultDbUser.UserName = (string)reader["UserName"];
                            resultDbUser.Password = (string)reader["Password"];
                            resultDbUser.UserTypeId = (int)reader["UserTypeID"];
                            resultDbUser.UserType = (string)reader["UserType"];
                            resultDbUser.UserStatusCheck = (int)reader["StatusCheck"];

                            return resultDbUser;
                        }

                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read the DbUser " + userName + " " + e);
                throw;
            }
        }

        public static async Task<ObservableCollection<DbUser>> GetUsersAsync(SearchUserParameter sup)
        {
            try
            {
                string searchByUserId = "";

                string isDeletedQuery = "";
                string searchBetweenDates = "";

                if (sup.UserId != 0) searchByUserId = $" AND [user].[ID] = {sup.UserId}";

                if (sup.UserDeleted == 1) isDeletedQuery = "1";
                else if (sup.UserDeleted == 2) isDeletedQuery = "0";

                if (sup.UserStartDate != null && sup.UserEndDate != null)
                {
                    searchBetweenDates += $" AND ([user].[DeletedDate] BETWEEN '{sup.UserStartDate}' AND '{sup.UserEndDate}' OR [user].[CreationDate] BETWEEN '{sup.UserStartDate}' AND '{sup.UserEndDate}')";
                }
                else
                {
                    searchBetweenDates = "";
                }

                var query = $"SELECT [user].[ID], [user].[UserName], [user].[Password] ,[user].[CreationDate], [user].[DeletedDate], [user].[UserTypeID], [user].[IsDeleted], [user].[StatusCheck], [usertype].[UserType] FROM [user] INNER JOIN [usertype] ON [user].[UserTypeID]=[usertype].[ID] WHERE [user].[UserName] LIKE '%'+'{sup.UserName}'+'%' AND [UserTypeID] LIKE '%'+'{sup.UserType}'+'%' AND [IsDeleted] LIKE '%{isDeletedQuery}%'{searchBetweenDates}{searchByUserId}";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    ObservableCollection<DbUser> userList = new ObservableCollection<DbUser>();
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var resultUser = new DbUser
                                {
                                    UserId = (int)reader["ID"],
                                    UserName = (string)reader["UserName"],
                                    Password = (string)reader["Password"],
                                    UserTypeId = (int)reader["UserTypeID"],
                                    UserType = (string)reader["UserType"],
                                    UserCreated = (DateTime)reader["CreationDate"],
                                    UserIsDeleted = (bool)reader["IsDeleted"],
                                    UserDeleted =
                                        reader["DeletedDate"] == DBNull.Value
                                            ? (DateTime?)null
                                            : (DateTime)reader["DeletedDate"],
                                    UserStatusCheck = (int)reader["StatusCheck"],
                                };
                                userList.Add(resultUser);
                            }

                            if (userList.Count == 0)
                            {
                                throw new NotFoundException("Datensatz auf der DB nicht gefunden");
                            }
                            return userList;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read the User from DB" + e);
                throw;
            }
        }

        public static async Task UpdateUserAsync(DbUser dbUser, DbUser origUser, bool writeInActivityLog)
        {
            try
            {
                var query = $"UPDATE [user] SET [UserName] = @username, [Password] = @password, [UserTypeID] = @usertypeid, [StatusCheck] = @statuscheck, [IsDeleted] = @isdeleted {(dbUser.UserIsDeleted ? ", [DeletedDate] = @deleteddate" : ", [DeletedDate] = NULL")} WHERE [ID] = @id";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@username", SqlDbType.NVarChar, dbFieldLengths.UserNameLn).Value = dbUser.UserName;
                        if (dbUser.Password == null)
                        {
                            dbUser.Password = origUser.Password;
                            command.Parameters.Add("@password", SqlDbType.NVarChar, dbFieldLengths.PasswordLn).Value = dbUser.Password;
                        }
                        else
                        {
                            command.Parameters.Add("@password", SqlDbType.NVarChar, dbFieldLengths.PasswordLn).Value = dbUser.Password;
                        }
                        command.Parameters.Add("@usertypeid", SqlDbType.Int).Value = dbUser.UserTypeId;
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = dbUser.UserIsDeleted;
                        if (dbUser.UserIsDeleted) dbUser.UserDeleted = DateTime.Now;
                        if (dbUser.UserIsDeleted) command.Parameters.Add("@deleteddate", SqlDbType.DateTime).Value = dbUser.UserDeleted;
                        command.Parameters.Add("@id", SqlDbType.Int).Value = dbUser.UserId;
                        command.Parameters.Add("@statuscheck", SqlDbType.Int).Value = dbUser.UserStatusCheck;

                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"Benutzer={dbUser.UserName} mit ID={dbUser.UserId} wurde aktualisiert; {CompareForActivityLog.CompareUser(origUser, dbUser)}", DateTime.Now);

                        if (successfullCount != 1)
                            throw new Exception("Could not Update the User in DB");
                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 2601 || e.Number == 2627)
                {
                    throw new ConflictException("Duplikatfehler");
                }
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
            }
        }

        public static async Task InsertUserAsync(DbUser dbUser, bool writeInActivityLog)
        {
            try
            {
                var query = "INSERT INTO [user] ([UserName], [Password], [CreationDate], [UserTypeID], [IsDeleted], [StatusCheck]) VALUES ( @username, @password, @creationdate, @usertypeid, @isdeleted, @statuscheck)";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@username", SqlDbType.NVarChar, dbFieldLengths.UserNameLn).Value = dbUser.UserName;
                        command.Parameters.Add("@password", SqlDbType.NVarChar, dbFieldLengths.PasswordLn).Value = dbUser.Password;
                        command.Parameters.Add("@creationdate", SqlDbType.DateTime).Value = dbUser.UserCreated;
                        command.Parameters.Add("@usertypeid", SqlDbType.Int).Value = dbUser.UserTypeId;
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = dbUser.UserIsDeleted;
                        command.Parameters.Add("@statuscheck", SqlDbType.Int).Value = dbUser.UserStatusCheck;


                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"Benutzer={dbUser.UserName} wurde Erstellt;", DateTime.Now);


                        if (successfullCount != 1)
                            throw new Exception("Could not Save the User in DB");
                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Number == CannotInsertDuplicateKeyRow || e.Number == CannotInsertDuplicateKeyInObject)
                {
                    throw new ConflictException("Duplikatfehler");
                }
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
            }
        }

        public static async Task<List<UserType>> GetUserTypesAsync()
        {
            try
            {
                var query = "SELECT * FROM [usertype]";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    List<UserType> userTypeList = new List<UserType>();
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var resultUserType = new UserType
                            {
                                UserTypeId = (int)reader["ID"],
                                UserTypeName = (string)reader["UserType"]
                            };
                            userTypeList.Add(resultUserType);
                        }

                        return userTypeList;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read the User from DB" + e);
                throw;
            }
        }

        public static async Task<Boolean> CheckECodeExistsAsync(string eCode)
        {
            try
            {
                var query = $"SELECT COUNT(*) AS ECodeCount FROM [ecode] where [ECode] = '{eCode}'";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        var count = (int)await command.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read the ECodes in DB" + e);
                throw;
            }
        }

        // Im Fehlerfall wird eine Exception geschmissen
        public static async Task<int> InsertECodeAsync(ECode eCode, bool writeInActivityLog)
        {
            try
            {
                var query = "INSERT INTO [ecode] ([ECode], [CreationDate], [IsDeleted], [ECodeServiceCardID]) OUTPUT INSERTED.ID VALUES ( @ecode, @creationdate, @isdeleted, @ecodeservicecardid)";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ecode", SqlDbType.NVarChar, dbFieldLengths.ECodeLn).Value = eCode.Code;
                        command.Parameters.Add("@creationdate", SqlDbType.DateTime).Value = eCode.ECodeCreationDate;
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = 0;
                        command.Parameters.Add("@ecodeservicecardid", SqlDbType.Int).Value = eCode.ECodeServiceCardId;

                        var newEcodeId = (int)await command.ExecuteScalarAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"Ecode={eCode.Code} und die Servicekarten-ID={eCode.ECodeServiceCardId} wurde eingefügt;", eCode.ECodeCreationDate);

                        if (newEcodeId == 0 || newEcodeId <= 0)
                            throw new Exception("Could not Save the ECode in DB");

                        return newEcodeId;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static async Task<Boolean> CheckUCodeExistsAsync(string uCode)
        {
            try
            {
                var query = $"SELECT COUNT(*) AS UCodeCount FROM [ucode] where [UCode] = '{uCode}'";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        var count = (int)await command.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read the UCodes in DB" + e);
                throw;
            }
        }

        // Im Fehlerfall wird eine Exception geschmissen
        public static async Task<int> InsertUCodeAsync(UCode uCode, bool writeInActivityLog)
        {
            try
            {
                var query = "INSERT INTO [ucode] ([UCode], [CreationDate], [IsDeleted], [UCodeServiceCardID]) OUTPUT INSERTED.ID VALUES ( @ucode, @creationdate, @isdeleted, @ucodeservicecardid)";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ucode", SqlDbType.NVarChar, dbFieldLengths.UCodeLn).Value = uCode.Code;
                        command.Parameters.Add("@creationdate", SqlDbType.DateTime).Value = uCode.UCodeCreationDate;
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = 0;
                        command.Parameters.Add("@ucodeservicecardid", SqlDbType.Int).Value = uCode.UCodeServiceCardId;


                        var newUcodeId = (int)await command.ExecuteScalarAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"Ucode={uCode.Code} und die Servicekarten-ID={uCode.UCodeServiceCardId} wurde eingefügt;", uCode.UCodeCreationDate);

                        if (newUcodeId == 0 || newUcodeId <= 0)
                            throw new Exception("Could not Save the User in DB");

                        return newUcodeId;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static async Task SetECodeDeletedAsync(ECode oldECode, bool writeInActivityLog)
        {
            try
            {
                var query = "UPDATE [ecode] SET [IsDeleted] = @isdeleted, [DeletedDate] = @deleteddate WHERE [ID] = @id";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = oldECode.ECodeIsDeleted;
                        command.Parameters.Add("@deleteddate", SqlDbType.DateTime).Value = oldECode.ECodeDeletedDate;
                        command.Parameters.Add("@id", SqlDbType.Int).Value = oldECode.ECodeId;

                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"ECode={oldECode.Code} mit ID={oldECode.ECodeId} und der Servicekarten-ID={oldECode.ECodeServiceCardId} wurde wurde deaktiviert; DeletedDate->{oldECode.ECodeDeletedDate} ", DateTime.Now);

                        if (successfullCount != 1)
                            throw new Exception("Could not Update the ECode in DB");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static async Task SetUCodeDeletedAsync(UCode oldUCode, bool writeInActivityLog)
        {
            try
            {
                var query = "UPDATE [ucode] SET [IsDeleted] = @isdeleted, [DeletedDate] = @deleteddate WHERE [ID] = @id";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@isdeleted", SqlDbType.Bit).Value = oldUCode.UCodeIsDeleted;
                        command.Parameters.Add("@deleteddate", SqlDbType.DateTime).Value = oldUCode.UCodeDeletedDate;
                        command.Parameters.Add("@id", SqlDbType.Int).Value = oldUCode.UCodeId;

                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"UCode={oldUCode.Code} mit ID={oldUCode.UCodeId} und der Servicekarten-ID={oldUCode.UCodeServiceCardId} wurde wurde deaktiviert; DeletedDate->{oldUCode.UCodeDeletedDate}", DateTime.Now);

                        if (successfullCount != 1)
                            throw new Exception("Could not Update the UCode in DB");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static async Task SetECodeUnDeletedAsync(int eCodeId, bool writeInActivityLog)
        {
            try
            {
                var query = "UPDATE [ecode] SET [IsDeleted] = 0, [DeletedDate] = NULL WHERE [ID] = @id";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@id", SqlDbType.Int).Value = eCodeId;

                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"ECode mit ID={eCodeId} reaktiviert;", DateTime.Now);

                        if (successfullCount != 1)
                            throw new Exception("Could not Update the UCode in DB");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }


        public static async Task SetUCodeUnDeletedAsync(int uCodeId, bool writeInActivityLog)
        {
            try
            {
                var query = "UPDATE [ucode] SET [IsDeleted] = 0, [DeletedDate] = NULL WHERE [ID] = @id";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@id", SqlDbType.Int).Value = uCodeId;

                        var successfullCount = await command.ExecuteNonQueryAsync();

                        if (writeInActivityLog)
                            await LogQueryAsync(command, $"UCode mit ID={uCodeId} reaktiviert;", DateTime.Now);

                        if (successfullCount != 1)
                            throw new Exception("Could not Update the UCode in DB");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }


        public static async Task<int> GetStatusCheckAsync(int id, string table)
        {
            int tempStatusCheck = 0;

            try
            {


                var query = $"SELECT [StatusCheck] FROM [{table}] where [ID] = {id}";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                tempStatusCheck = (int)reader["StatusCheck"];
                            }
                        }
                    }
                }

                return tempStatusCheck;
            }
            catch (Exception e)
            {
                Log.Error("Could not read the UCodes in DB" + e);
                throw;
            }
        }


        public static async Task<ObservableCollection<ActivityLog>> GetActivityLogAsync(string user, DateTime? logStartDate, DateTime? logEndDate)
        {
            try
            {
                string searchBetweenDates = "";

                if (logStartDate != null && logEndDate != null)
                    searchBetweenDates += $" AND ([Time] BETWEEN '{logStartDate}' AND '{logEndDate}')";
                
                else
                {
                    searchBetweenDates = "";
                }

                var query = $"SELECT [activitylog].[ID], [activitylog].[Time], [activitylog].[UserID], [activitylog].[What], [user].[UserName] AS [User], [user].[UserTypeID], [usertype].[UserType] AS [UserType] FROM [activitylog] INNER JOIN [user] ON [activitylog].[UserID]=[user].[ID] INNER JOIN [usertype] ON [user].[UserTypeID]=[usertype].[ID] WHERE [user].[UserName] LIKE '%'+'{user}'+'%'{searchBetweenDates}";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    ObservableCollection<ActivityLog> activityLogList = new ObservableCollection<ActivityLog>();
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var resultActivityLog = new ActivityLog()
                                {
                                    ActivityLogId = (int)reader["ID"],
                                    ActivityLogDate = (DateTime)reader["Time"],
                                    ActivityLogUserId = (int)reader["UserID"],
                                    ActivityLogWhatIsChanged = (string)reader["What"],
                                    ActivityLogUser = (string)reader["User"],
                                    ActivityLogUserTypeId = (int)reader["UserTypeID"],
                                    ActivityLogUserType = (string)reader["UserType"]
                                };

                                activityLogList.Add(resultActivityLog);
                            }

                            if (activityLogList.Count == 0)
                            {
                                throw new NotFoundException("Datensatz auf der DB nicht gefunden");
                            }
                            return activityLogList;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read the ActivityLog from DB" + e);
                throw;
            }
        }

    }
}

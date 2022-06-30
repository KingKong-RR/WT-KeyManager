using KeyManager.Models;

namespace KeyManager.Utilities
{
    public class CompareForActivityLog
    {
        public static string CompareCustomer(Customer c1, Customer c2) // c1 ist der Originale Customer und c2 ist der geänderte Customer
        {
            string activityLogString = "";

            if (c1.CustomerId != c2.CustomerId)
                activityLogString += $"ID:{c1.CustomerId}->{c2.CustomerId}; ";

            if (c1.CustomerName != c2.CustomerName)
                activityLogString += $"Name:{c1.CustomerName}->{c2.CustomerName}; ";

            if (c1.CustomerCode != c2.CustomerCode)
                activityLogString += $"CustomerCode:{c1.CustomerCode}->{c2.CustomerCode}; ";

            if (c1.KtNumber != c2.KtNumber)
                activityLogString += $"KTNumber:{c1.KtNumber}->{c2.KtNumber}; ";

            if (c1.SummPNumber != c2.SummPNumber)
                activityLogString += $"SummPNumber:{c1.SummPNumber}->{c2.SummPNumber}; ";

            if (c1.CustomerIsDeleted != c2.CustomerIsDeleted)
                activityLogString += $"IsDelted:{c1.CustomerIsDeleted}->{c2.CustomerIsDeleted}; ";

            if (c1.CustomerCreationDate != c2.CustomerCreationDate)
                activityLogString += $"CreationDate:{c1.CustomerCreationDate}->{c2.CustomerCreationDate}; ";

            if (c1.CustomerDeletedDate != c2.CustomerDeletedDate)
                activityLogString += $"DeletedDate:{c1.CustomerDeletedDate}->{c2.CustomerDeletedDate}; ";

            return activityLogString;
        }

        public static string CompareGroup(Group g1, Group g2) //g1 ist der Originale Group und g2 ist die geänderte Group
        {
            string activityLogString = "";

            if (g1.GroupId != g2.GroupId)
                activityLogString += $"ID:{g1.GroupId}->{g2.GroupId}; ";

            if (g1.GroupName != g2.GroupName)
                activityLogString += $"Name:{g1.GroupName}->{g2.GroupName}; ";

            if (g1.PNumber != g2.PNumber)
                activityLogString += $"PNumber:{g1.PNumber}-->{g2.PNumber}";

            if (g1.GroupCustomerName != g2.GroupCustomerName)
                activityLogString += $"CustomerName:{g1.GroupCustomerName}->{g2.GroupCustomerName}";

            if (g1.GroupECode != g2.GroupECode)
                activityLogString += $"ECode:{g1.GroupECode}->{g2.GroupECode}";

            if (g1.GroupECodeServiceCardId != g2.GroupECodeServiceCardId)
                activityLogString += $"ECodeServicecardID:{g1.GroupECodeServiceCardId}->{g2.GroupECodeServiceCardId}";

            if (g1.GroupUCode != g2.GroupUCode)
                activityLogString += $"UCode:{g1.GroupUCode}->{g2.GroupUCode}";

            if (g1.GroupUCodeServiceCardId != g2.GroupUCodeServiceCardId)
                activityLogString += $"UCodeServicecardID:{g1.GroupUCodeServiceCardId}->{g2.GroupUCodeServiceCardId}";

            if (g1.GroupIsDeleted != g2.GroupIsDeleted)
                activityLogString += $"IsDeleted:{g1.GroupIsDeleted}->{g2.GroupIsDeleted}; ";

            if (g1.GroupCreationDate != g2.GroupCreationDate)
                activityLogString += $"CreationDate:{g1.GroupCreationDate}->{g2.GroupCreationDate}; ";

            if (g1.GroupDeletedDate != g2.GroupDeletedDate)
                activityLogString += $"DeletedDate:{g1.GroupDeletedDate}->{g2.GroupDeletedDate}; ";

            return activityLogString;
        }

        public static string CompareUser(DbUser u1, DbUser u2) //u1 ist der Originale User und u2 ist die geänderte User
        {
            string activityLogString = "";

            if (u1.UserId != u2.UserId)
                activityLogString += $"ID:{u1.UserId}->{u2.UserId}";

            if (u1.UserName != u2.UserName)
                activityLogString += $"UserName:{u1.UserName}->{u2.UserName}";

            if (u1.UserTypeId != u2.UserTypeId)
                activityLogString += $"UserTypeID:{u1.UserTypeId}->{u2.UserTypeId}";

            if (u1.UserType != u2.UserType)
                activityLogString += $"UserType:{u1.UserType}->{u2.UserType}";

            if (u1.UserIsDeleted != u2.UserIsDeleted)
                activityLogString += $"IsDeleted:{u1.UserIsDeleted}->{u2.UserIsDeleted}; ";

            if (u1.UserCreated != u2.UserCreated)
                activityLogString += $"CreationDate:{u1.UserCreated}->{u2.UserCreated}; ";

            if (u1.UserDeleted != u2.UserDeleted)
                activityLogString += $"DeletedDate:{u1.UserDeleted}->{u2.UserDeleted}; ";

            return activityLogString;
        }
    }
}
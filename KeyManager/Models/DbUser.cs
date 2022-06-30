using System;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class DbUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserTypeId { get; set; }
        public string UserType { get; set; }
        public bool UserIsDeleted { get; set; }
        public DateTime UserCreated { get; set; }
        public DateTime? UserDeleted { get; set; }
        public int UserStatusCheck { get; set; }

        public DbUser() { }

        public DbUser(DbUser dBu)
        {
            UserId = dBu.UserId;
            UserName = dBu.UserName;
            Password = dBu.Password;
            UserTypeId = dBu.UserTypeId;
            UserIsDeleted = dBu.UserIsDeleted;
            UserCreated = dBu.UserCreated;
            UserDeleted = dBu.UserDeleted;
            UserStatusCheck = dBu.UserStatusCheck;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

using System;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class SearchUserParameter
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public int UserDeleted { get; set; }
        public DateTime? UserStartDate { get; set; }
        public DateTime? UserEndDate { get; set; }

        public SearchUserParameter(string userName, string userType, int userDeleted, DateTime? userStartDate,
            DateTime? userEnddate)
        {
            UserName = userName;
            UserType = userType;
            UserDeleted = userDeleted;
            UserStartDate = userStartDate;
            UserEndDate = userEnddate;
        }

        public SearchUserParameter(int userId)
        {
            UserId = userId;
            UserName = "";
            UserType = "";
            UserDeleted = 0;
            UserStartDate = null;
            UserEndDate = null;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

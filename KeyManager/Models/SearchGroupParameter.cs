using System;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class SearchGroupParameter
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string PNumber { get; set; }
        public string GroupCustomerName { get; set; }
        public int GroupDeleted { get; set; }
        public DateTime? GroupStartDate { get; set; }
        public DateTime? GroupEndDate { get; set; }
        public int GroupCustomerId { get; set; } = -1;

        public SearchGroupParameter(string groupName, string pNumber, string groupCustomerName, int groupDeleted, DateTime? groupStartDate, DateTime? groupEndDate)
        {
            GroupName = groupName;
            PNumber = pNumber;
            GroupCustomerName = groupCustomerName;
            GroupDeleted = groupDeleted;
            GroupStartDate = groupStartDate;
            GroupEndDate = groupEndDate;
        }

        public SearchGroupParameter(string groupName, string pNumber, string groupCustomerName, int groupDeleted, DateTime? groupStartDate, DateTime? groupEndDate, int groupCustomerId)
        {
            GroupName = groupName;
            PNumber = pNumber;
            GroupCustomerName = groupCustomerName;
            GroupDeleted = groupDeleted;
            GroupStartDate = groupStartDate;
            GroupEndDate = groupEndDate;
            GroupCustomerId = groupCustomerId;
        }

        public SearchGroupParameter(int groupId)
        {
            GroupId = groupId;
            GroupName = "";
            PNumber = "";
            GroupCustomerName = "";
            GroupDeleted = 0;
            GroupStartDate = null;
            GroupEndDate = null;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

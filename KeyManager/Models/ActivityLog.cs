using System;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class ActivityLog
    {
        public int ActivityLogId { get; set; }
        public int ActivityLogUserId { get; set; }
        public string ActivityLogUser { get; set; }
        public int ActivityLogUserTypeId { get; set; }
        public string ActivityLogUserType { get; set; }
        public DateTime ActivityLogDate { get; set; }
        public string ActivityLogWhatIsChanged { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
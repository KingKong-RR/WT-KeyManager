using System;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class UCode
    {
        public int UCodeId { get; set; }
        public string Code { get; set; }
        public DateTime UCodeCreationDate { get; set; }
        public DateTime? UCodeDeletedDate { get; set; }
        public bool UCodeIsDeleted { get; set; }
        public int UCodeServiceCardId { get; set; }

        public UCode() { }

        public UCode(UCode uC)
        {
            UCodeId = uC.UCodeId;
            Code = uC.Code;
            UCodeCreationDate = uC.UCodeCreationDate;
            UCodeDeletedDate = uC.UCodeDeletedDate;
            UCodeIsDeleted = uC.UCodeIsDeleted;
            UCodeServiceCardId = uC.UCodeServiceCardId;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

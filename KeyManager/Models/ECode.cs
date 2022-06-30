using System;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class ECode
    {
        public int ECodeId { get; set; }
        public string Code { get; set; }
        public DateTime ECodeCreationDate { get; set; }
        public DateTime? ECodeDeletedDate { get; set; }
        public bool ECodeIsDeleted { get; set; }
        public int ECodeServiceCardId { get; set; }

        public ECode() { }

        public ECode(ECode eC)
        {
            ECodeId = eC.ECodeId;
            Code = eC.Code;
            ECodeCreationDate = eC.ECodeCreationDate;
            ECodeDeletedDate = eC.ECodeDeletedDate;
            ECodeIsDeleted = eC.ECodeIsDeleted;
            ECodeServiceCardId = eC.ECodeServiceCardId;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

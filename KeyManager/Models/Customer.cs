using System;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string KtNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime CustomerCreationDate { get; set; }
        public DateTime? CustomerDeletedDate { get; set; }
        public bool CustomerIsDeleted { get; set; }
        public string SummPNumber { get; set; }
        public int CustomerStatusCheck { get; set; }

        public Customer() { }

        public Customer(Customer c)
        {
            CustomerId = c.CustomerId;
            KtNumber = c.KtNumber;
            CustomerCode = c.CustomerCode;
            CustomerName = c.CustomerName;
            CustomerCreationDate = c.CustomerCreationDate;
            CustomerDeletedDate = c.CustomerDeletedDate;
            CustomerIsDeleted = c.CustomerIsDeleted;
            SummPNumber = c.SummPNumber;
            CustomerStatusCheck = c.CustomerStatusCheck;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

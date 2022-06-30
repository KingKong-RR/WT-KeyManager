using System;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class SearchCustomerParameter
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string KtNumber { get; set; }
        public string CustomerCode { get; set; }
        public string SummPNumber { get; set; }
        public int CustomerDeleted { get; set; }
        public DateTime? CustomerStartDate { get; set; }
        public DateTime? CustomerEndDate { get; set; }

        public SearchCustomerParameter(string customerName, string ktNumber, string customerCode, string summPNumber, int customerDeleted, DateTime? customerStartDate, DateTime? customerEndDate)
        {
            CustomerName = customerName;
            KtNumber = ktNumber;
            CustomerCode = customerCode;
            SummPNumber = summPNumber;
            CustomerDeleted = customerDeleted;
            CustomerStartDate = customerStartDate;
            CustomerEndDate = customerEndDate;
        }

        public SearchCustomerParameter(int customerId)
        {
            CustomerId = customerId;
            CustomerName = "";
            KtNumber = "";
            CustomerCode = "";
            SummPNumber = "";
            CustomerDeleted = 0;
            CustomerStartDate = null;
            CustomerEndDate = null;
        }

        public SearchCustomerParameter(string customerName)
        {
            CustomerName = customerName;
            KtNumber = "";
            CustomerCode = "";
            SummPNumber = "";
            CustomerDeleted = 0;
            CustomerStartDate = null;
            CustomerEndDate = null;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

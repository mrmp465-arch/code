using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.DTO
{
    public class BankCashAPI
    {
        public long TransactionID { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string OrderNo { get; set; }
        public string OrderInfo { get; set; }
        public long Amount { get; set; }
        public long TotalAmount { get; set; }

        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }
        public string BankCode { get; set; }
        public string Note { get; set; }

        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }

        public int ReturnValue { get; set; }

        public string Description { get; set; }
        public long ReturnTotalValue { get; set; }
        public int Type { get; set; }


    }
}

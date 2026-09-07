using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.BankDirect
{
    public class DataCallback
    {
        public string RefCode { get; set; }
        public string OrderNo { get; set; }
        public int Amount { get; set; }
        public string Mobile { get; set; }
        public string MomoName { get; set; }
        public string OrderInfo { get; set; }
        public string Type { get; set; }
        public string MomoTransId { get; set; }
        

    }
    public class DataCallbackV3
    {
        public string RefCode { get; set; }
        public string OrderNo { get; set; }
        public int Amount { get; set; }
        //public string Mobile { get; set; }
        //public string MomoName { get; set; }
        public string OrderInfo { get; set; }
        public string Type { get; set; }

        public int ResponseCode { get; set; }

        public string Description { get; set; }
        public string Signature { get; set; }
        //public string MomoTransId { get; set; }


    }
    public class DataCallbackV2
    {
      
        public string OrderNo { get; set; }
        public int Amount { get; set; }
        public string BankCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public DateTime TimeBankSuccess { get; set; }
        public string Type { get; set; }
        public string BankTransId { get; set; }


    }
}

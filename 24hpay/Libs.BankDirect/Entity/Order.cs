using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.BankDirect
{
    class Order
    {
        public string Status { get; set; }
        public string BankCode { get; set; }

        public string Url { get; set; }
        public string QRCode { get; set; }
        //public string QRCodeBase64 { get; set; }
        public string LinkOpenApp { get; set; }
        //public string LinkWebView { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int Amount { get; set; }
        public string RefCode { get; set; }
        public string OrderNo { get; set; }
        public int Timeout { get; set; }
    }
}

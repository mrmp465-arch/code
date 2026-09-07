using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.Api
{
    public class OrderInput
    {
        public string Mobile { get; set; }
        public string FullName { get; set; }
        public string OrderNo { get; set; }
        public string Telco { get; set; }
        public string TopupType { get; set; }
        public int Amount { get; set; }
        public int AmountMin { get; set; }
        public int AmountMinAll { get; set; }
        public int Priority { get; set; }
        public string Password { get; set; }
        public string AccountName { get; set; }
       
    }
}

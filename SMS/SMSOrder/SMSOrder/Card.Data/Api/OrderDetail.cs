using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.Api
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int TransactionID { get; set; }
        public string OrderNo { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public int TopupType { get; set; }
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public string Telco { get; set; }
        public int Amount { get; set; }
        public int AmountUser { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastTime { get; set; }
        public int Status { get; set; }
     
    }
}

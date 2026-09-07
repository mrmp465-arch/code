using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.Api
{
    public class OrderGroup
    {
        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string Telco { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }
        public long TotalAmountSuccess { get; set; }
        public long TotalAmount { get; set; }
        public int TotalTranSuccess { get; set; }
        public int TotalTrans { get; set; }
        public int Ignore { get; set; }
        public int Queue { get; set; }
        public int Success { get; set; }
        public int Disable { get; set; }
        public int Lock { get; set; }
        public string SubUser { get; set; }
    }
    public class OrderGroupReport
    {
        public string OrderNo { get; set; }
        public string Telco { get; set; }
        public DateTime CreatedTime { get; set; }
        public int TotalAmountSuccess { get; set; }
        public int TotalAmount { get; set; }
        public DateTime CategoryName
        {
            get { return  new DateTime(CreatedTime.Year, CreatedTime.Month, CreatedTime.Day); }
        }
    }
    public class UserWarning
    {
       
        public string UserName { get; set; }
        public DateTime LastTime { get; set; }
     
    }
}

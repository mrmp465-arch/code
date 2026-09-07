using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.Api
{
    public class OrderOutput
    {
        public int TransactionID { get; set; }
        public int UserId { get; set; }
        public string Telco { get; set; }
        public string RequestNo { get; set; }
        public string Mobile { get; set; }
        public int Amount { get; set; }
        public int TopupType { get; set; }
        public string LogContent { get; set; }
        public int AmountTopupSuccess { get; set; }
        public int AmountPending { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }
        public int Status { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Partners { get; set; }
        public int AmountMin { get; set; }
        public int Priority { get; set; }
        public string OrderNo { get; set; }
        public string Providers { get; set; }
        public int IsConfirm { get; set; }
        public int AmountMinAll { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
    }
    public class OrderOutputExcel
    {
        public int TransactionID { get; set; }
        public string OrderNo { get; set; }
       
        public string RequestNo { get; set; }
        public string Mobile { get; set; }
        public string Telco { get; set; }
        public int TopupType { get; set; }
        public int Amount { get; set; }
       
       
        public int AmountTopupSuccess { get; set; }
        public int AmountPending { get; set; }
      
      
        //public string UserName { get; set; }
        //public string FullName { get; set; }
        public int AmountMin { get; set; }
        public int AmountMinAll { get; set; }
        public int Priority { get; set; }
       
        public int Status { get; set; }
        public int IsConfirm { get; set; }
        public string LogContent { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }
       
     
    }
}

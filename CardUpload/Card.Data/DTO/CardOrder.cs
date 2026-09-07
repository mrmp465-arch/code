using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.DTO
{
    public class CardOrder
    {
        public long Id { get; set; }
        public string OrderNo { get; set; }

       
        public string Telco { get; set; }
        public string ParrentName { get; set; }
        public string UserName { get; set; }
        public int Amount { get; set; }
        public int AmountSuccess { get; set; }
        public double Fee { get; set; }
        public double Reward { get; set; }

        public int Money { get; set; }
        public int MoneyReward { get; set; }
        public string CardSerial { get; set; }
        public string CardCode { get; set; }

        public int Status { get; set; }
        public int IsConfirm { get; set; }

       
       
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
    public class CardOrderGroup
    {
        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public string ParrentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public long TotalAmountSuccess { get; set; }
        public long TotalMoney { get; set; }

        public long TotalMoneyReward { get; set; }

        public long TotalAmount { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalTrans { get; set; }
        public int TotalFail { get; set; }
        public int TotalQueue { get; set; }

        public int TotalConfirm { get; set; }


    }
    public class CardOrderReport2
    {
        public int TotalQueue { get; set; }
        public int TotalTrans { get; set; }
        public string Telco { get; set; }
        public int Amount { get; set; }
    }
    public class CardOrderReport
    {
        public long TotalAmount { get; set; }
        public long TotalRevenue { get; set; }
       
        public long TotalReward { get; set; }
        
        public string Data { get; set; }
        public long TotalTrans { get; set; }

       
    }
}

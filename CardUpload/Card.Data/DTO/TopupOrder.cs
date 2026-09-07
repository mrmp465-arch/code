using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.DTO
{
    public class TopupOrder
    {
        public long Id { get; set; }
        public string Account { get; set; }
        public string OrderNo { get; set; }
        public string Telco { get; set; }
        public string TopupType { get; set; }
        public string ParrentName { get; set; }
        public string UserName { get; set; }
        public int Amount { get; set; }
        public int AmountSuccess { get; set; }
        public double Fee { get; set; }
        public double Reward { get; set; }

        public int Money { get; set; }
        public int MoneyReward { get; set; }
        public int MoneyPriority { get; set; }
        public string CardValue { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public int IsConfirm { get; set; }
        public string RefCode { get; set; }


        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
    public class TopupOrderGroup
    {
        
        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public string ParrentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public long TotalAmountSuccess { get; set; }
        public long TotalMoney { get; set; }
        public long TotalWaiting { get; set; }

        public long TotalCancel{ get; set; }
        public long TotalMoneyReward { get; set; }
        public long TotalMoneyPriority { get; set; }
        public long TotalAmount { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalTrans { get; set; }
        public int TotalFail { get; set; }
        public int TotalQueue { get; set; }

        public int TotalConfirm { get; set; }


    }
    public class TopupOrderReport
    {
        public long TotalAmount { get; set; }
        public long TotalRevenue { get; set; }
       
        public long TotalReward { get; set; }
        public long TotalMoneyPriority { get; set; }
        public string Data { get; set; }
        public long TotalTrans { get; set; }

       
    }
}

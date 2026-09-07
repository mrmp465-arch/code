using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.DTO
{
    public class BuyCard
    {
        public long Id { get; set; }
        public int Type { get; set; }

       
        public string Telco { get; set; }
        public string ParrentName { get; set; }
        public string UserName { get; set; }
        public int Amount { get; set; }
        public int CardNumber { get; set; }
        public double Fee { get; set; }
        public double Reward { get; set; }

        public int Money { get; set; }
        public int MoneyReward { get; set; }
        public int CardValue { get; set; }
       
        public int Status { get; set; }

        public string CardData { get; set; }

        public string RefCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
    
   
    public class BuyCardReport
    {
       // public long TotalAmount { get; set; }
        public long TotalRevenue { get; set; }
       
        public long TotalReward { get; set; }
        
        public string Data { get; set; }
        public long TotalTrans { get; set; }

       
    }
}

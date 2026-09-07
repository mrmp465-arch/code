using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.Api
{
    public class VTTAccount
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
        public int Type { get; set; }
    }
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
        public int Id { get; set; }
        public string CreateUser { get; set; }
        public int Ussd { get; set; }
        public int Status { get; set; }

        public string CallbackUrl { get; set; }

        public string SubUser { get; set; }

        public int PercentC1 { get; set; }
        public int PercentParrent { get; set; }
        public int Percent { get; set; }

        public string ExtData { get; set; }
    }
    public class OrderReport
    {
        public string Mobile { get; set; }
        public string ParrentName { get; set; }
        public string OrderNo { get; set; }
        public string Telco { get; set; }
        public int TopupType { get; set; }
        public int Amount { get; set; }
        public int AmoutSuccess { get; set; }


        public int AmountMin { get; set; }
        public int Type { get; set; }
        public string UserApi { get; set; }
        public string PasswordApi { get; set; }
        public long OrderId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Ussd { get; set; }
        public int Priority { get; set; }
        public string C1Name { get; set; }

        public int PercentC1 { get; set; }
        public int PercentParrent { get; set; }
        public int Percent { get; set; }
        public decimal BidRate { get; set; }

        public int BidFeeHold { get; set; }
        public string Description { get; set; }


    }
    public class OrderReportHistory
    {
        public string Mobile { get; set; }
        public string ParrentName { get; set; }
        public string OrderNo { get; set; }
        public string Telco { get; set; }
        public int TopupType { get; set; }
        public long Amount { get; set; }
        public int PercentParrent { get; set; }
        public int Percent { get; set; }
        public int Type { get; set; }
        public string UserApi { get; set; }
        public long OrderId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Ussd { get; set; }

        public string CardSerial { get; set; }
        public string CardCode { get; set; }

        public int Priority { get; set; }

        public int? BidFee { get; set; }

        public decimal? BidRate { get; set; }

        public string C1Name { get; set; }

        public int PercentC1 { get; set; }

        public decimal PercentRoot { get; set; }
    }
    public class OrderReportHistoryItem
    {
        public long TotalAmount { get; set; }
        public long TotalRevenue { get; set; }
        public long AmountRevenueParrent { get; set; }
        public long TotalBidFee { get; set; }
        public string Telco { get; set; }
        public string Data { get; set; }
        public long TotalTrans { get; set; }

        public string UserName { get; set; }

        public long TotalAmountUSSD { get; set; }

        public int Percent { get; set; }
        public decimal? BidRate { get; set; }
    }
    public class OrderReportHistoryDS
    {
        public long TotalVNP { get; set; }
        public long TotalGarena { get; set; }
        public long TotalVMS { get; set; }
        public long TotalBidFee { get; set; }
        public long TotalZing { get; set; }

        public long TotalMyVTT { get; set; }

        public long TotalVTT { get; set; }


        public string Data { get; set; }




    }
    public class OrderGroupBid
    {

        public long TotalAmount { get; set; }
        public long TotalAmountSuccess { get; set; }

        public decimal BidRate { get; set; }

        public int BidFeeHold { get; set; }
    }
}

using System;
using System.Configuration;
namespace Card.Utility
{
    public class Enums
    {
        public enum FunctionType
        {
            IsView = 0,
            IsInsert = 1,
            IsUpdate = 2,
            IsDelete = 3,
            IsFullControl = 4,
        }

        public enum UserType
        {
            Admin = 1,
            Level1 = 2,
            Level2 = 3,
            Level3 = 4
        }
        public enum Status
        {
            Lock = -2,
            Success = 3,
        }
    }
    public static class FunctionCode
    {
        public const string Device = "device";
        public const string Users = "users";
        public const string AddUser = "adduser";
        public const string Function = "function";
        public const string UserLog = "userlog";
        public const string SystemConfig = "systemconfig";
        public const string Group = "group";
        public const string Content = "content";
        public const string OrderList = "orderlist";
        public const string OrderReport = "ordereport";
        public const string OrderDS = "orderds";

        public const string TopupList = "topuplist";
        public const string TopupReport = "topupreport";
        public const string TopupDS = "topupds";

        public const string BuyCardList = "buycardlist";
        public const string BuyCardReport = "buycardreport";

        public const string OrderActive = "OrderActive";
        public const string OrderTranSearch = "OrderTranSearch";
        public const string OrdeConfirm = "OrderConfirm";
        public const string ReportDaily = "reportdaily";
        public const string ReportOur = "reportour";
        public const string ReporUser = "reportuser";
        public const string OrderVNP = "ordervnp";
        public const string OrderVTT = "ordervtt";
        public const string OrderGRN = "ordergrn";
        public const string OrderVTC = "ordervtc";
        public const string OrderZing = "orderzing";
        public const string OrderVMS = "ordervms";
        public const string OrderSearch = "ordersearch";
        public const string UserWarning = "userwarning";
        public const string ReportBid = "reportbid";
        public const string ReportDS = "reportds";
        public const string MyAccountVTT = "MyAccountVTT";

        public const string BankLog = "banklog";
        public const string BankSearch = "banksearch";
        public const string BankDS = "bankds";
        public const string BankReport = "bankreport";
        public const string BankCashLog = "bankcashlog";
        public const string BankCashReport = "bankcashreport";
        public const string BankCashDS = "bankcashds";
        public const string BankCashSearch = "bankcashsearch";




    }

}

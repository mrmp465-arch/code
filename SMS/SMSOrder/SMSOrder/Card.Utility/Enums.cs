using System;
using System.Configuration;
namespace SMS.Utility
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
        public enum CampaignStatus
        {
            Init = 0,
            Waiting = 1,
            InProcess = 2,
            Success = 3,
            Lock =4,
        }
        public enum TelcoType
        {
            VTT = 1,
            VMS = 2,
            VNP = 3,
           
        }
    }
    public static class FunctionCode
    {
        public const string Users = "users";
        public const string AddUser = "adduser";
        public const string Function = "function";
        public const string UserLog = "userlog";
        public const string Group = "group";
        public const string Content = "content";
        public const string Dictionary = "dictionary";
        public const string Contact = "contact";
        public const string OrdeConfirm = "OrderConfirm";
        public const string Campaign = "campaign";
        public const string ReportDaily = "reportdaily";
        public const string ReportLockNumber = "reportlocknumber";
        public const string AdminConfig = "adminconfig";
        public const string ReportHour = "reportour";
        public const string ReportSim = "reportsim";
        public const string ReportUser = "reportuser";
        public const string ReportGenerate = "reportgenerate";
        public const string SMSSearch = "smssearch";
        public const string ReportRevenue = "reportrevenue";
        public const string ReportTopup = "reporttopup";
        public const string ReportLockSim = "reportlocksim";
        public const string Sim = "sim";
        public const string RegisterVTT = "registervtt";
        public const string USSD = "ussd";
        public const string SMSInbox = "smsinbox";
        public const string SMSOutbox = "smsoutbox";
    }

}

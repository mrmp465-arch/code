using System;
using System.Configuration;
namespace CMS.Utility
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
       
        public const string CardLog = "cardlog";
       
        public const string CardReport = "cardreport";
     
        public const string CardDS = "cardds";
        public const string ReportDaily = "reportdaily";
        public const string BankLog = "banklog";
    
        public const string BankReport = "bankreport";
        public const string BankCashLog = "bankcashlog";
        public const string BankCashReport = "bankcashreport";
      
     

    }

}

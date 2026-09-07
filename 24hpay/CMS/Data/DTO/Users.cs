using System;

namespace CMS.Data.DTO
{
    [Serializable]
    public class Users
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public int Type { get; set; }
        public int Group { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public bool Status { get; set; }
        public bool StatusOrder { get; set; }
        public int Balance { get; set; }
        public int BalanceHold { get; set; }
        public int PercentVNP { get; set; }

        public int PercentVNPQ { get; set; }
        public int PercentVMS { get; set; }
        public int PercentVTT { get; set; }

        public int PercentVTTQ { get; set; }

        public int PercentGarena { get; set; }
        public int PercentZing { get; set; }

        public int PercentVTC { get; set; }
        public string Config { get; set; }
        public int Piority { get; set; }
        public bool StatusVTT { get; set; }
        public bool StatusVMS { get; set; }
        public bool StatusVNP { get; set; }

        public bool StatusZing { get; set; }

        public bool StatusVTC { get; set; }
        public bool StatusGarena { get; set; }
        public string UserAPI { get; set; }
        public string PasswordAPI { get; set; }
        public string C1User { get; set; }
        public int NumberUser { get; set; }
        public int AmountDay { get; set; }
        public int MaxDay { get; set; }
        /// <summary>

    }
    [Serializable]
    public class UserSession
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int Type { get; set; }
    }
    [Serializable]
    public class UserSessionSercure
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public DateTime ExpireTime { get; set; }
    }
    //Quyền Chức năng
    [Serializable]
    public class UserFunction
    {
        public int UserID { get; set; }
        public int FunctionID { get; set; }
        public string FunctionName { get; set; }
        public int FatherID { get; set; }
        public string FatherName { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsFullControl { get; set; }

        public string FunctionCode { get; set; }
    }
   
    //Quyền quản trị dịch vụ
    public class UserService
    {
        public int UserID { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
    }

    public class UserDaily
    {
        public int UserName { get; set; }
        public long Amount { get; set; }
        public long AmountUser { get; set; }

        public DateTime Time { get; set; }
    }
}

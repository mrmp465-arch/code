using CMS.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    
    public class BankGateReport
    {
        public string Time { get; set; }
        public int TotalTransaction { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class Item
    {
        public int Id { get; set; }
        public string value { get; set; }
    }
    public class ModelFunctionDetail
    {
        public List<Functions> ListFunction { get; set; }
        public Functions FunctionDetail { get; set; }
    }

    public class ReturnData
    {
        public long ResponseCode { get; set; }
        public string Description { get; set; }
        public string Extended { get; set; }
    }

    public class UserFunctionModel
    {
        public List<Functions> ListFunction { get; set; }
        public List<UserFunction> UserFunction { get; set; }
    }

    public class FunctionOrder
    {
        public int Id { get; set; }
        public int FatherID { get; set; }
        public int Order { get; set; }
    }
    public class ReportDaily
    {
        public DateTime Day { get; set; }
        public int AmountSuccess { get; set; }
        public int Amount { get; set; }
    }
    public class DataReport
    {
        public string Time { get; set; }
        public long TotalVTT { get; set; }
        public long TotalVNP { get; set; }
        public long TotalVMS { get; set; }
        public long TotalBank { get; set; }
        public long TotalMOMO { get; set; }
        public long TotalVTP { get; set; }

        public long TotalUSDT { get; set; }
        public long TotalBankOut { get; set; }
        public long TotalMOMOOut { get; set; }
        public long Total { get; set; }
    }
    public class BankAccountV3
    {

        public string AccountName { get; set; }

        public string AccountId { get; set; }
        public string BankCode { get; set; }
    }
}
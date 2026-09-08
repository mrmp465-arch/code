using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Libs.Db;
using Libs.Utils;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Collections;
using System.Web.Script.Serialization;

namespace Libs.BankGate.Entity
{
    public class BankAccount
    {

        //public string AccountNumber { get; set; }
        //public string AccountName { get; set; }
        public string BankCode { get; set; }
        public string DisplayName { get; set; }
    }
    public class BankAccountV2
    {
              
        public string BankName { get; set; }
        public string BankCode { get; set; }
    }
    public class BankAccountV3
    {

        public string AccountName { get; set; }

        public string AccountId { get; set; }
        public string BankCode { get; set; }
    }
    public class BankAccountV4
    {

        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
    }
    public class BankAccountV5
    {

        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Url { get; set; }
        public string BankCode { get; set; }
    }
    public class BankAccountV6
    {
        public string BankName { get; set; }

        public string Name { get; set; }
    }
}

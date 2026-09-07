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
        public int Id { get; set; }
        public string BankName { get; set; }
        public string Name { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        //public string Content { get; set; }
    }
    public class BankAccountV2
    {
       
        public string BankName { get; set; }
        public string Name { get; set; }
       
    }
    public class BankAccountV5
    {

        public string BankCode { get; set; }
        public string Name { get; set; }

    }
    public class BankAccountV3
    {

        public string BankName { get; set; }

        public string BankId { get; set; }
        public string BankCode { get; set; }
    }
    public class BankAccountV4
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string Name { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        //public string Content { get; set; }
        //public string QR { get; set; }
    }
    public class MomoAccount
    {
      
       // public string QR { get; set; }
        public string MomoId { get; set; }
        public string MomoName { get; set; }
        //public string Content { get; set; }
    }
}

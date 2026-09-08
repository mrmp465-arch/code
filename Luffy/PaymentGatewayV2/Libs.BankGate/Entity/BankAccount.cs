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
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }

    }

}

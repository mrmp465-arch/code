using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.API;

namespace Libs.VGGTopup
{
    public class Accounts
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public int GG { get; set; }
        public int GGPayment { get; set; }

        public Accounts()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public Accounts Get(string accountName)
        {
            DBHelper db = new DBHelper(Configs.VGGProfileAPIConnectionStrings);
            return db.GetInstanceSP<Accounts>("SP_Account_GetInfoByAccountName"
                , new SqlParameter("@_AccountName", accountName)
                , new SqlParameter("@_SysPartnerKey", Configs.VGGProfileSysPartnerKey)
                );
        }
    }
}

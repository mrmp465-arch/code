using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Libs.Db;

namespace Libs.Report
{
    public class TransactionAutoLog
    {
        public long Id { get; set; }
        public string OrderNo { get; set; }
        public string PartnerCode { get; set; }
        public string CardType { get; set; }
        public int Amount { get; set; }
        public int Quantity { get; set; }
        public string Sign { get; set; }
        public long RequestTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastTime { get; set; }
        public string ReturnValue { get; set; }
        public int Status { get; set; }


        public long Add()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@OrderNo", OrderNo);
            pars[2] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = new SqlParameter("@CardType", CardType);
            pars[4] = new SqlParameter("@Amount", Amount);
            pars[5] = new SqlParameter("@Quantity", Quantity);
            pars[6] = new SqlParameter("@Sign", Sign);
            pars[7] = new SqlParameter("@RequestTime", RequestTime);
            db.ExecuteNonQuerySP("sp_TransactionAutoLog_Insert", pars);
            return Convert.ToInt64(pars[0].Value);
        }


        public int Update()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@ReturnValue", ReturnValue);
            db.ExecuteNonQuerySP("sp_TransactionAutoLog_Update", pars);
            return Convert.ToInt32(pars[0].Value);
        }

    }
}

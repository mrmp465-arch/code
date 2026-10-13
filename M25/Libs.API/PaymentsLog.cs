using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class PaymentsLog 
    {
        public int LogID { get; set; }
        public int ServiceID { get; set; }
        public string LogType { get; set; }
        public byte[] ClassData { get; set; }
        public int DataSize { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public DateTime LogTime { get; set; }
        public int ReturnValue { get; set; }

        public PaymentsLog()
        {

        }

        public PaymentsLog Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<PaymentsLog>("sp_PaymentLog_Select"
                , new SqlParameter("@LogID", LogID));
        }

        public PaymentsLog Get(int logID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<PaymentsLog>("sp_PaymentLog_Select"
                , new SqlParameter("@LogID", logID));
        }

        public DataTable GetTable(int serviceID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_PaymentLog_SelectList", new SqlParameter("@ServiceID", serviceID));
        }

        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_PaymentLog_SelectList", new SqlParameter("@ServiceID", DBNull.Value));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ServiceID", ServiceID);
            pars[2] = new SqlParameter("@LogType", LogType);
            pars[3] = new SqlParameter("@ClassData", ClassData);
            pars[4] = new SqlParameter("@ClassName", ClassName);
            pars[5] = new SqlParameter("@UserName", UserName);
            pars[6] = new SqlParameter("@Description", Description);

            db.ExecuteNonQuerySP("sp_PaymentLog_Insert", pars);
            LogID = Convert.ToInt32(pars[0].Value);
        }

    }
}

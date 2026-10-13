using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class APILog
    {
        public int LogID { get; set; }
        public int TransactionID { get; set; }
        public string Content { get; set; }
        public string LogType { get; set; }
        public DateTime LogTime { get; set; }
        public int PartnerID { get; set; }
        public int ServiceID { get; set; }
        public int ReturnValue { get; set; }

        public APILog()
        {

        }

        public APILog Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<APILog>("sp_APILog_Select"
                , new SqlParameter("@LogID", LogID));
        }

        public APILog Get(int logID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<APILog>("sp_APILog_Select"
                , new SqlParameter("@LogID", logID));
        }

        public DataTable GetTable(int transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_APILog_SelectByTransaction", new SqlParameter("@transactionID", transactionID));
        }

        public DataTable GetTable(int logID, int serviceID, int partnerID, int top)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_APILog_SelectByTransaction",
                logID == 0 ? new SqlParameter("@LogID", DBNull.Value) : new SqlParameter("@LogID", logID),
                serviceID == 0 ? new SqlParameter("@ServiceID", DBNull.Value) : new SqlParameter("@ServiceID", serviceID),
                partnerID == 0 ? new SqlParameter("@PartnerID", DBNull.Value) : new SqlParameter("@PartnerID", partnerID),
                new SqlParameter("@Top", top)
                );
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Content", Content);
            pars[3] = new SqlParameter("@LogType", LogType);
            pars[4] = new SqlParameter("@PartnerID", PartnerID);
            pars[5] = new SqlParameter("@ServiceID", ServiceID);

            db.ExecuteNonQuerySP("sp_Partners_Insert", pars);
            LogID = Convert.ToInt32(pars[0].Value);
        }
        public void AddBossLog(string RequestId,string RequestContent,string ServiceCode,string Ip)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@RequestId", RequestId);
            pars[2] = new SqlParameter("@RequestContent", RequestContent);
            pars[3] = new SqlParameter("@ServiceCode", ServiceCode);
            pars[4] = new SqlParameter("@Ip", Ip);
            db.ExecuteNonQuerySP("sp_APIBossLog_Insert", pars);
            LogID = Convert.ToInt32(pars[0].Value);
        }
    }
}

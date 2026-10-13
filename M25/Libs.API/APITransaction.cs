using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class APITransaction
    {
        public long TransactionID { get; set; }
        public int PartnerID { get; set; }
        public string PartnerCode { get; set; }
        public string ProviderCode { get; set; }
        public int ServiceID { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }
        public string IpAddress { get; set; }
        public DateTime UpdateTime { get; set; }
        public int ReturnValue { get; set; }

        public APITransaction()
        {

        }

        public APITransaction Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<APITransaction>("sp_APITransaction_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public APITransaction Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<APITransaction>("sp_APITransaction_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public APITransaction Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[2] = new SqlParameter("@ServiceCode", ServiceCode);
            pars[3] = new SqlParameter("@CommandCode", CommandCode);
            pars[4] = new SqlParameter("@RequestContent", RequestContent);
            pars[5] = new SqlParameter("@Signature", Signature);
            pars[6] = new SqlParameter("@IpAddress", IpAddress);

            APITransaction transaction = db.GetInstanceSP<APITransaction>("sp_APITransaction_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
            return transaction;
        }

        public void UpdateStatus()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);

            db.ExecuteNonQuerySP("sp_APITransaction_UpdateStatus", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public DataTable GetTable(int top, int partnerID, int serviceID, DateTime createdTime, int status, string commandCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_APITransaction_SelectList",
                new SqlParameter("@Top", top),
                partnerID == 0 ? new SqlParameter("@PartnerID", DBNull.Value) : new SqlParameter("@PartnerID", partnerID),
                serviceID == 0 ? new SqlParameter("@ServiceID", DBNull.Value) : new SqlParameter("@ServiceID", serviceID),
                new SqlParameter("@CreatedTime", createdTime),
                status == 0 ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status),
                commandCode == null ? new SqlParameter("@CommandCode", DBNull.Value) : new SqlParameter("@CommandCode", commandCode)
                );
        }
    }
}

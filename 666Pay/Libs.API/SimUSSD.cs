using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class SimUSSD
    {
        public int Id { get; set; }
        public int? DeviceId { get; set; }
        public string Sim { get; set; }
        public int? Slot { get; set; }
        public string Telco { get; set; }
        public long? Quota { get; set; }
        public long? Amount { get; set; }
        public int? Status { get; set; }
        public int ReturnValue { get; set; }
        public SimUSSD()
        {

        }
        public List<SimUSSD> GetListSimUSSDStatus(string clientId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<SimUSSD>("sp_SimUSSD_SelectList_byClientId", new SqlParameter("@ClientId", clientId));
        }

        public SimUSSD Get(string clientId, int slot)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<SimUSSD>("sp_SimUSSD_SelectList_byClientId_Slot"
                , new SqlParameter("@ClientId", clientId)
                , new SqlParameter("@Slot", slot));
        }

        public int UpdateStatus(string clientId, int slot, string sim, int? status)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ClientId", clientId);
            pars[2] = new SqlParameter("@Slot", slot);
            pars[3] = string.IsNullOrEmpty(sim) ? new SqlParameter("@Sim", DBNull.Value) : new SqlParameter("@Sim", sim);
            pars[4] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            db.ExecuteNonQuerySP("sp_SimUSSD_Update_byClientId", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }

        public int UpdateAmout(string clientId, int slot, long amount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ClientId", clientId);
            pars[2] = new SqlParameter("@Slot", slot);
            pars[3] = new SqlParameter("@Amount", amount);
            db.ExecuteNonQuerySP("sp_SimUSSD_Update_byClientId_Amount", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }

        public List<SimUSSD> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<SimUSSD>("sp_SimUSSD_SelectList", DeviceId == null ? new SqlParameter("@DeviceId", DBNull.Value) : new SqlParameter("@DeviceId", DeviceId)
                );
        }

        public int Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[9];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = DeviceId == null ? new SqlParameter("@DeviceId", DBNull.Value) : new SqlParameter("@DeviceId", DeviceId);
            pars[3] = string.IsNullOrEmpty(Sim) ? new SqlParameter("@Sim", DBNull.Value) : new SqlParameter("@Sim", Sim);
            pars[4] = Slot == null ? new SqlParameter("@Slot", DBNull.Value) : new SqlParameter("@Slot", Slot);
            pars[5] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
            pars[6] = Quota == null ? new SqlParameter("@Quota", DBNull.Value) : new SqlParameter("@Quota", Quota);
            pars[7] = Amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
            pars[8] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);

            db.ExecuteNonQuerySP("sp_SimUSSD_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }

        public SimUSSD Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<SimUSSD>("sp_SimUSSD_Select"
                , new SqlParameter("@Id", Id));
        }


    }

}

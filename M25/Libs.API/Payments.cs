using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.API
{
    public class Payments
    {
        public int ServiceID { get; set; }
        public string Name { get; set; }
        public string ServiceCode { get; set; }
        public string Description { get; set; }
        public byte[] ClassData { get; set; }
        public string ClassName { get; set; }
        public int DataSize { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Status { get; set; }
        public string Config { get; set; }
        public DateTime LastTransactionTime { get; set; }
        public string LastTransactionInfo { get; set; }
        public DateTime StartTime { get; set; }
        public int ErrorCount { get; set; }
        public int ReturnValue { get; set; }


        public Payments()
        {

        }

        public Payments Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Payments>("sp_Payments_Select"
                , new SqlParameter("@ServiceID", ServiceID));
        }

        public Payments Get(int serviceID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Payments>("sp_Payments_Select"
                , new SqlParameter("@ServiceID", serviceID));
        }

        public Payments GetInfor(int serviceID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Payments>("sp_Payments_SelectInfor"
                , new SqlParameter("@ServiceID", serviceID));
        }

        public Payments GetCheck(string serviceCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Payments>("sp_Payments_SelectByServiceCodeForCheck"
                , new SqlParameter("@ServiceCode", serviceCode));
        }

        public Payments GetCheckCache(string serviceCode)
        {
            string KeyCache = string.Format("{0}:{1}", "RedisPayments", serviceCode);
            try
            {
                var result = DataCaching.GetCache<Payments>(KeyCache);
                if (result == null)
                {
                    result = GetCheck(serviceCode);
                    result = DataCaching.SetCache(KeyCache, result);
                }
                return result;
            }
            catch (Exception ex)
            {
                //   ExceptionHandler.Handle(ex, "Partners", "Get:" + KeyCache);
                return null;
            }
        }

        protected void DeleteCache()
        {
            //DataCaching.RemoveCache(string.Format("{0}:{1}_{2}", "RedisPayments", partnerID, serviceID));
            //DataCaching.RemoveCache(string.Format("{0}:{1}", "RedisPayments", "List"));
            DataCaching.RemoveByPattern(@"^(RedisPayments:)(\w+)");

        }

        public Payments Get(string serviceCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Payments>("sp_Payments_SelectByServiceCode"
                , new SqlParameter("@ServiceCode", serviceCode));
        }
      
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_Payments_Delete"
                , new SqlParameter("@ServiceID", ServiceID));
        }

        public void Delete(int serviceID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_Payments_Delete"
                , new SqlParameter("@ServiceID", serviceID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[9];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Name", Name);
            pars[2] = new SqlParameter("@ServiceCode", ServiceCode);
            pars[3] = new SqlParameter("@Description", Description);
            pars[4] = new SqlParameter("@ClassData", ClassData);
            pars[5] = new SqlParameter("@DataSize", DataSize);
            pars[6] = new SqlParameter("@ClassName", ClassName);
            pars[7] = new SqlParameter("@Status", Status);
            pars[8] = new SqlParameter("@Config", Config);

            db.ExecuteNonQuerySP("sp_Payments_Insert", pars);
            ServiceID = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[14];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ServiceID", ServiceID);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@ServiceCode", ServiceCode);
            pars[4] = new SqlParameter("@Description", Description);
            pars[5] = new SqlParameter("@ClassData", ClassData);
            pars[6] = new SqlParameter("@DataSize", DataSize);
            pars[7] = new SqlParameter("@ClassName", ClassName);
            pars[8] = new SqlParameter("@Status", Status);
            pars[9] = new SqlParameter("@Config", Config);
            pars[10] = new SqlParameter("@LastTransactionInfo", string.IsNullOrEmpty(LastTransactionInfo) ? "" : LastTransactionInfo);
            pars[11] = new SqlParameter("@LastTransactionTime", LastTransactionTime);
            pars[12] = new SqlParameter("@StartTime", StartTime);
            pars[13] = new SqlParameter("@ErrorCount", ErrorCount);

            db.ExecuteNonQuerySP("sp_Payments_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);

            DeleteCache();
        }

        public void UpdateInfor()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[12];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ServiceID", ServiceID);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@ServiceCode", ServiceCode);
            pars[4] = new SqlParameter("@Description", Description);
            pars[5] = new SqlParameter("@ClassName", ClassName);
            pars[6] = new SqlParameter("@Status", Status);
            pars[7] = new SqlParameter("@Config", Config);
            pars[8] = new SqlParameter("@LastTransactionInfo", string.IsNullOrEmpty(LastTransactionInfo) ? "" : LastTransactionInfo);
            pars[9] = new SqlParameter("@LastTransactionTime", LastTransactionTime);
            pars[10] = new SqlParameter("@StartTime", StartTime);
            pars[11] = new SqlParameter("@ErrorCount", ErrorCount);

            db.ExecuteNonQuerySP("sp_Payments_UpdateInfor", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public List<Payments> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<Payments>("sp_Payments_SelectList");
        }

        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_Payments_SelectList");
        }
    }
}

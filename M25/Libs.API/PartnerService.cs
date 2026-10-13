using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.API
{
    public class PartnerService
    {
        public int PartnerServiceID { get; set; }
        public int PartnerID { get; set; }
        public int ServiceID { get; set; }
        public DateTime CreatedTime { get; set; }
        public string IPAddress { get; set; }
        public int Status { get; set; }
        public string CommandCode { get; set; }
        public long Quota { get; set; }
        public int Occurs { get; set; }
        public int ReturnValue { get; set; }

        public PartnerService()
        {

        }

        public PartnerService Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<PartnerService>("sp_PartnerService_Select"
                , new SqlParameter("@PartnerID", PartnerID)
                , new SqlParameter("@ServiceID", ServiceID)
                );
        }
        public PartnerService GetCache()
        {
            string KeyCache = string.Format("{0}:{1}_{2}", "RedisPartnerService", PartnerID, ServiceID);
            try
            {
                var result = DataCaching.GetCache<PartnerService>(KeyCache);
                if (result == null)
                {
                    result = Get();
                    result = DataCaching.SetCache(KeyCache, result);
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       
        public PartnerService Get(int partnerID, int serviceID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<PartnerService>("sp_PartnerService_Select"
                , new SqlParameter("@PartnerID", partnerID)
                , new SqlParameter("@ServiceID", serviceID)
                );
        }
        public PartnerService GetCache(int partnerID, int serviceID)
        {
            string KeyCache = string.Format("{0}:{1}_{2}", "RedisPartnerService", partnerID, serviceID);
            try
            {
                var result = DataCaching.GetCache<PartnerService>(KeyCache);
                if (result == null)
                {
                    result = Get(partnerID, serviceID);
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
        //public PartnerService Get(string partnerCode, string serviceCode)
        //{
        //    DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
        //    return db.GetInstanceSP<PartnerService>("sp_PartnerService_Check"
        //        , new SqlParameter("@PartnerCode", partnerCode)
        //        , new SqlParameter("@ServiceCode", serviceCode)
        //        );
        //}
        //public PartnerService GetCache(string partnerCode, string serviceCode)
        //{
        //    string KeyCache = string.Format("{0}:{1}_{2}", "RedisPartnerServiceCode", PartnerID, ServiceID);
        //    try
        //    {
        //        var result = DataCaching.GetCache<PartnerService>(KeyCache);
        //        if (result == null)
        //        {
        //            result = Get(partnerCode, serviceCode);
        //            result = DataCaching.SetCache(KeyCache, result);
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_PartnerService_Delete"
                , new SqlParameter("@PartnerID", PartnerID)
                , new SqlParameter("@ServiceID", ServiceID)
                );
            DeleteCache();
        }  

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerID", PartnerID);
            pars[2] = new SqlParameter("@ServiceID", ServiceID);
            pars[3] = new SqlParameter("@IPAddress", IPAddress);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@CommandCode", CommandCode);

            db.ExecuteNonQuerySP("sp_PartnerService_Insert", pars);
            PartnerServiceID = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerID", PartnerID);
            pars[2] = new SqlParameter("@ServiceID", ServiceID);
            pars[3] = new SqlParameter("@IPAddress", IPAddress);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@CommandCode", CommandCode);            
            pars[6] = new SqlParameter("@Quota", Quota);
            pars[7] = new SqlParameter("@Occurs", Occurs);
            db.ExecuteNonQuerySP("sp_PartnerService_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value); 
            DeleteCache();
        }

        public List<PartnerService> GetList(int partnerID, int serviceID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<PartnerService>("sp_PartnerService_SelectList"
                , partnerID == 0 ? new SqlParameter("@PartnerID", DBNull.Value) : new SqlParameter("@PartnerID", partnerID)
                , serviceID == 0 ? new SqlParameter("@ServiceID", DBNull.Value) : new SqlParameter("@ServiceID", serviceID)
                );
        }
        public List<PartnerService> GetListCache(int partnerID, int serviceID)
        {
            string KeyCache = string.Format("{0}:{1}_{2}", "RedisPartnerServiceList", partnerID, serviceID);
            try
            {
                var result = DataCaching.GetCacheList<PartnerService>(KeyCache);
                if (result == null)
                {
                    result = GetList(partnerID,serviceID);
                    result = DataCaching.SetCacheList(KeyCache, result);
                }
                return result;
            }
            catch (Exception ex)
            {
                // ExceptionHandler.Handle(ex, "CategoryBO", "CachedPage:" + KeyCache);
                return null;
            }
        }
        protected void DeleteCache()
        {
            //DataCaching.RemoveCache(string.Format("{0}:{1}_{2}", "RedisPartnerService", partnerID, serviceID));
            //DataCaching.RemoveCache(string.Format("{0}:{1}", "RedisPartnerService", "List"));
            DataCaching.RemoveByPattern(@"^(RedisPartnerService:)(\w+)");

        }
    }
}

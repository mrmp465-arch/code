using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.API
{
    public enum SignatureType : int
    {
        MD5 = 1,
        RSA = 2,
        SHA256 = 3,
    }

    [Serializable]
    public class Partners
    {
        public int PartnerID { get; set; }
        public string Name { get; set; }
        public string PartnerCode { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }
        public int SignatureType { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string SMSCommand { get; set; }
        public string SMSUrl { get; set; }
        public string SMSPlusCommand { get; set; }
        public string SMSPlusUrl { get; set; }
        public string SMSPlusCheckUrl { get; set; }
        public int ReturnValue { get; set; }
        public string SmsCommand { get; set; }
        public string SmsUrl { get; set; }
        public string SmsPlusCommand { get; set; }
        public string SmPlussUrl { get; set; }
        public string Hotline { get; set; }
        public int RequestType { get; set; }
        public long Balance { get; set; }
        public Partners()
        {

        }

        public string GetBuyCardCondition(int partnerId ,int serviceId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ProviderCodeReturn", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerID", partnerId);
            pars[2] = new SqlParameter("@ServiceID", serviceId);
            db.ExecuteNonQuerySP("sp_Partners_Select_BuyCard_Condition", pars);
            var returnValue = pars[0].Value.ToString();
            return returnValue;
        }

        public Partners Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Partners>("sp_Partners_Select"
                , new SqlParameter("@PartnerID", PartnerID));
        }
        public Partners GetCache()
        {
            string KeyCache = string.Format("{0}:{1}", "RedisPartners", PartnerID);
            try
            {
                var result = DataCaching.GetCache<Partners>(KeyCache);
                if (result == null)
                {
                    result = Get(PartnerID);
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
        public Partners Get(int partnerID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Partners>("sp_Partners_Select"
                , new SqlParameter("@PartnerID", partnerID));
        }
        public Partners GetCache(int partnerID)
        {
            string KeyCache = string.Format("{0}:{1}", "RedisPartners", partnerID);
            try
            {
                var result = DataCaching.GetCache<Partners>(KeyCache);
                if (result == null)
                {
                    result = Get(partnerID);
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
        public Partners Get(string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Partners>("sp_Partners_SelectByPartnerCode"
                , new SqlParameter("@PartnerCode", partnerCode));
        }
        public int CheckActive(string partnerCode)
        {
            string KeyCache = string.Format("{0}:{1}", "RedisPartners", partnerCode);
            try
            {
                var result = DataCaching.GetCache<Partners>(KeyCache);
                if (result == null)
                {
                    return 0;
                }
                return 1;
            }
            catch (Exception ex)
            {
                //   ExceptionHandler.Handle(ex, "Partners", "Get:" + KeyCache);
                return 0;
            }
        }
        public Partners GetCache(string partnerCode)
        {
            string KeyCache = string.Format("{0}:{1}", "RedisPartners", partnerCode);
            try
            {
                var result = DataCaching.GetCache<Partners>(KeyCache);
                if (result == null)
                {
                    result = Get(partnerCode);
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

        public Partners GetSms(string partnerCommand)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Partners>("sp_Partners_Select_SmsPlusCommand"
                , new SqlParameter("@SMSPlusCommand", partnerCommand));
        }

        public Partners GetSmsCache(string partnerCommand)
        {
            string KeyCache = string.Format("{0}:{1}_{2}", "RedisPartners", "SMS", partnerCommand);
            try
            {
                var result = DataCaching.GetCache<Partners>(KeyCache);
                if (result == null)
                {
                    result = GetSms(partnerCommand);
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
        public int Topup(long Amount,string Code)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Amount", Amount);
            pars[2] = new SqlParameter("@PartnerCode", Code);
            db.ExecuteNonQuerySP("sp_PartnersTopup", pars);
           return Convert.ToInt32(pars[0].Value);
        }
        public int Deduct(long Amount, string Code)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Amount", Amount);
            pars[2] = new SqlParameter("@PartnerCode", Code);
            db.ExecuteNonQuerySP("sp_PartnersDeduct", pars);
            return Convert.ToInt32(pars[0].Value);
        }
        public void Delete(int partnerID)
        {
            var item = Get(partnerID);
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_Partners_Delete"
                , new SqlParameter("@PartnerID", partnerID));
            DeleteCache();
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Name", Name);
            pars[2] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = new SqlParameter("@Status", Status);
            pars[4] = new SqlParameter("@SignatureType", SignatureType);
            pars[5] = new SqlParameter("@PrivateKey", PrivateKey);
            pars[6] = new SqlParameter("@PublicKey", PublicKey);
            pars[7] = new SqlParameter("@SMSCommand", SMSCommand);
            pars[8] = new SqlParameter("@SMSUrl", SMSUrl);
            pars[9] = new SqlParameter("@SMSPlusCommand", SMSPlusCommand);
            pars[10] = new SqlParameter("@SMSPlusUrl", SMSPlusUrl);
            pars[11] = new SqlParameter("@SMSPlusCheckUrl", SMSPlusCheckUrl);
            pars[12] = new SqlParameter("@Hotline", Hotline);

            db.ExecuteNonQuerySP("sp_Partners_Insert", pars);
            PartnerID = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[15];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerID", PartnerID);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@SignatureType", SignatureType);
            pars[6] = new SqlParameter("@PrivateKey", PrivateKey);
            pars[7] = new SqlParameter("@PublicKey", PublicKey);
            pars[8] = new SqlParameter("@SMSCommand", SMSCommand);
            pars[9] = new SqlParameter("@SMSUrl", SMSUrl);
            pars[10] = new SqlParameter("@SMSPlusCommand", SMSPlusCommand);
            pars[11] = new SqlParameter("@SMSPlusUrl", SMSPlusUrl);
            pars[12] = new SqlParameter("@SMSPlusCheckUrl", SMSPlusCheckUrl);
            pars[13] = new SqlParameter("@Hotline", Hotline);
            pars[14] = new SqlParameter("@RequestType", RequestType);
            db.ExecuteNonQuerySP("sp_Partners_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);

            UpdateRP();
        }

        public void UpdateRP()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[14];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerID", PartnerID);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@SignatureType", SignatureType);
            pars[6] = new SqlParameter("@PrivateKey", PrivateKey);
            pars[7] = new SqlParameter("@PublicKey", PublicKey);
            pars[8] = new SqlParameter("@SMSCommand", SMSCommand);
            pars[9] = new SqlParameter("@SMSUrl", SMSUrl);
            pars[10] = new SqlParameter("@SMSPlusCommand", SMSPlusCommand);
            pars[11] = new SqlParameter("@SMSPlusUrl", SMSPlusUrl);
            pars[12] = new SqlParameter("@SMSPlusCheckUrl", SMSPlusCheckUrl);
            pars[13] = new SqlParameter("@Hotline", Hotline);
            db.ExecuteNonQuerySP("sp_Partners_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);

            DeleteCache();
        }

        protected void DeleteCache()
        {
            //DataCaching.RemoveCache(string.Format("{0}:{1}", "RedisPartners", PartnerID));
            //DataCaching.RemoveCache(string.Format("{0}:{1}", "RedisPartners", PartnerCode));
            //DataCaching.RemoveCache(string.Format("{0}:{1}", "RedisPartners", "List"));
            DataCaching.RemoveByPattern(@"^(RedisPartners:)(\w+)");
        }
        public List<Partners> GetListByUserId(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@UserId", UserId);
            return db.GetListSP<Partners>("sp_Partners_SelectListByUserId", pars);
        }
        public List<Partners> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<Partners>("sp_Partners_SelectList");
        }
        public List<Partners> GetListCache()
        {
            string KeyCache = string.Format("{0}:{1}", "RedisPartners", "List");
            try
            {
                var result = DataCaching.GetCacheList<Partners>(KeyCache);
                if (result == null)
                {
                    result = GetList();
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

        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_Partners_SelectList");
        }

    }
}

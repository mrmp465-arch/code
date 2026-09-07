using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Db;
using System.Data;
using System.Data.SqlClient;
using Libs.Utils;

namespace Libs.API
{
    [Serializable]
    public class Providers
    {
        public int ProviderId { get; set; }
        public string Name { get; set; }
        public string ProviderCode { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int SignatureType { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public int OrderNo { get; set; }
        public long Quota { get; set; }
        public int Occurs { get; set; }
        public int ReturnValue { get; set; }
        public string ProductCode { get; set; }
        public int Type { get; set; }
        public string GSMUrl { get; set; }

        public string AmoutList { get; set; }

        public string GetCardCondition(string productCode, string partnerCode,int amount=0)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            //DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ProviderCodeReturn", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ProductCode", productCode);
            pars[2] = new SqlParameter("@PartnerCode", partnerCode);
            pars[3] = new SqlParameter("@Amount", amount);
            db.ExecuteNonQuerySP("sp_Providers_Select_Card_Condition", pars);
            var returnValue = pars[0].Value.ToString();
            return returnValue;
        }
        public string GetCardConditionList(string productCode, string partnerCode, int amount = 0)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            //DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ProviderCodeReturn", SqlDbType.VarChar, 250) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ProductCode", productCode);
            pars[2] = new SqlParameter("@PartnerCode", partnerCode);
            pars[3] = new SqlParameter("@Amount", amount);
            db.ExecuteNonQuerySP("sp_Providers_Select_Card_ConditionList", pars);
            var returnValue = pars[0].Value.ToString();
            return returnValue;
        }
        public string GetCardConditionCache(string productCode, string partnerCode)
        {
            string KeyCache = string.Format("{0}", "RedisGetCardCondition");
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                if (result == null)
                {
                    result = GetCardCondition(productCode, partnerCode);
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

        public string GetBuyCardCondition(string productCode, string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ProviderCodeReturn", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ProductCode", productCode);
            pars[2] = new SqlParameter("@PartnerCode", partnerCode);
            db.ExecuteNonQuerySP("sp_Providers_Select_BuyCard_Condition", pars);
            var returnValue = pars[0].Value.ToString();
            return returnValue;
        }

        public string GetBuyCardConditionCache(string productCode, string partnerCode)
        {
            string KeyCache = string.Format("{0}", "RedisGetBuyCardCondition");
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                if (result == null)
                {
                    result = GetBuyCardCondition(productCode, partnerCode);
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

        public string GetTopupMobileCondition(string productCode, string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ProviderCodeReturn", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ProductCode", productCode);
            pars[2] = new SqlParameter("@PartnerCode", partnerCode);
            db.ExecuteNonQuerySP("sp_Providers_Select_TopupMobile_Condition", pars);
            var returnValue = pars[0].Value.ToString();
            return returnValue;
        }

        public string GetBankCondition(string productCode, string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ProviderCodeReturn", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ProductCode", productCode);
            pars[2] = new SqlParameter("@PartnerCode", partnerCode);
            db.ExecuteNonQuerySP("sp_Providers_Select_Bank_Condition", pars);
            var returnValue = pars[0].Value.ToString();
            return returnValue;
        }

        public Providers Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Providers>("sp_Providers_Select", new SqlParameter("@ProviderId", ProviderId));
        }

        public Providers Get(int providerId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Providers>("sp_Providers_Select", new SqlParameter("@ProviderId", providerId));
        }

        public Providers Get(string providerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Providers>("sp_Providers_SelectByProviderCode", new SqlParameter("@ProviderCode", providerCode));
        }
        public Providers GetCache(string providerCode)
        {
            string KeyCache = string.Format("{0}:{1}", "RediProviders", providerCode);
            try
            {
                var result = DataCaching.GetCache<Providers>(KeyCache);
                if (result == null)
                {
                    result = Get(providerCode);
                    result = DataCaching.SetCache(KeyCache, result, 60*5);
                }
                return result;
            }
            catch (Exception ex)
            {
                //   ExceptionHandler.Handle(ex, "Partners", "Get:" + KeyCache);
                return null;
            }
        }
        public List<Providers> GetListByUserId(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@UserId", UserId);
            return db.GetListSP<Providers>("sp_Providers_SelectListByUserId", pars);
        }

        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_Providers_Delete"
                , new SqlParameter("@ProviderId", ProviderId));

            DeleteCache();
        }

        public void Delete(int providerId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_Providers_Delete"
                , new SqlParameter("@ProviderId", providerId));

            DeleteCache();
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[12];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Name", Name);
            pars[2] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[3] = new SqlParameter("@Status", Status);
            pars[4] = new SqlParameter("@SignatureType", SignatureType);
            pars[5] = new SqlParameter("@PrivateKey", PrivateKey);
            pars[6] = new SqlParameter("@PublicKey", PublicKey);
            pars[7] = new SqlParameter("@OrderNo", OrderNo);
            pars[8] = new SqlParameter("@Quota", Quota);
            pars[9] = new SqlParameter("@Occurs", Occurs);
            pars[10] = new SqlParameter("@Type", Type);
            pars[11] = new SqlParameter("@ProductCode", ProductCode);

            db.ExecuteNonQuerySP("sp_Providers_Insert", pars);
            ProviderId = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {

            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[15];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ProviderId", ProviderId);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@SignatureType", SignatureType);
            pars[6] = new SqlParameter("@PrivateKey", PrivateKey);
            pars[7] = new SqlParameter("@PublicKey", PublicKey);
            pars[8] = new SqlParameter("@OrderNo", OrderNo);
            pars[9] = new SqlParameter("@Quota", Quota);
            pars[10] = new SqlParameter("@Occurs", Occurs);
            pars[11] = new SqlParameter("@Type", Type);
            pars[12] = new SqlParameter("@ProductCode", ProductCode);
            pars[13] = new SqlParameter("@GSMUrl", GSMUrl);
            pars[14] = new SqlParameter("@AmoutList", AmoutList);
            db.ExecuteNonQuerySP("sp_Providers_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);

            UpdateRP();
        }


        public void UpdateRP()
        {


            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[15];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@ProviderId", ProviderId);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@SignatureType", SignatureType);
            pars[6] = new SqlParameter("@PrivateKey", PrivateKey);
            pars[7] = new SqlParameter("@PublicKey", PublicKey);
            pars[8] = new SqlParameter("@OrderNo", OrderNo);
            pars[9] = new SqlParameter("@Quota", Quota);
            pars[10] = new SqlParameter("@Occurs", Occurs);
            pars[11] = new SqlParameter("@Type", Type);
            pars[12] = new SqlParameter("@ProductCode", ProductCode);
            pars[13] = new SqlParameter("@GSMUrl", GSMUrl);
            pars[14] = new SqlParameter("@AmoutList", AmoutList);
            db.ExecuteNonQuerySP("sp_Providers_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);

            DeleteCache();
        }

        public List<Providers> GetList(int Type)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var lst = db.GetListSP<Providers>("sp_Providers_SelectList");
            if (lst != null && lst.Count > 0)
                return lst.Where(e => e.Type == Type).ToList();
            else
                return lst;
        }

        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_Providers_SelectList");
        }

        protected void DeleteCache()
        {
            DataCaching.RemoveCache(string.Format("{0}", "RedisGetCardCondition"));
            DataCaching.RemoveCache(string.Format("{0}", "RedisGetBuyCardCondition"));
        }
    }


}

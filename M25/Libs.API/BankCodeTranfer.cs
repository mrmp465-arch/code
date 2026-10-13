using Libs.Db;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.API
{
    public  class BankCodeTranfer
    {
        public int id { get; set; }
        public string code { get; set; }
        public string bin { get; set; }
        public string shortName { get; set; }

        public int isTransfer { get; set; }

        public void DeleteCache()
        {
            //DataCaching.RemoveCache(string.Format("{0}:{1}", "RedisPartners", PartnerID));
            //DataCaching.RemoveCache(string.Format("{0}:{1}", "RedisPartners", PartnerCode));
            //DataCaching.RemoveCache(string.Format("{0}:{1}", "RedisPartners", "List"));
            DataCaching.RemoveCache("BankCodeTranfer");
        }
        public List<BankCodeTranfer> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<BankCodeTranfer>("sp_BankCodeTranfer_SelectList");
        }
        public void Update(int id, int isTransfer)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@id", id);
                pars[2] = new SqlParameter("@isTransfer", isTransfer);
                db.ExecuteNonQuerySP("sp_BankCodeTranfer_Update", pars);

                //var ReturnValue = Convert.ToInt32(pars[0].Value);
                //NLogLogger.Info(new string[] { "Update ReturnValue", ReturnValue.ToString() });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }
        }
        public List<BankCodeTranfer> GetListCache()
        {
            string KeyCache = string.Format("BankCodeTranfer");
            try
            {
                var result = DataCaching.GetCacheList<BankCodeTranfer>(KeyCache);
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
    }

    
}

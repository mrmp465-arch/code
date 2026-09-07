using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Linq;
using System.Web.ModelBinding;

namespace Libs.API
{
    [Serializable]
    public class SystemDataConfig
    {
        public string DataKey { get; set; }
        public string DataValue { get; set; }
        public void Update(string key, string value)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@DataKey", key);
                pars[2] = new SqlParameter("@DataValue", value);
                db.ExecuteNonQuerySP("sp_SystemDataConfig_Update", pars);

                //var ReturnValue = Convert.ToInt32(pars[0].Value);
                //NLogLogger.Info(new string[] { "Update ReturnValue", ReturnValue.ToString() });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }
        }
        public List<SystemDataConfig> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<SystemDataConfig>("sp_SystemDataConfig_SelectList");
        }

        public List<SystemDataConfig> GetListCache()
        {
            string KeyCache = string.Format("SystemConfigData");
            try
            {
                var result = DataCaching.GetCacheList<SystemDataConfig>(KeyCache);
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
        public string GetKey(List<SystemDataConfig> data, string key)
        {
            if (data.Exists(x => x.DataKey == key))
                return data.FirstOrDefault(a => a.DataKey == key).DataValue;
            return "";
        }
        public string GetKey(string key)
        {
            var data = GetListCache();
            if (data.Exists(x => x.DataKey == key))
                return data.FirstOrDefault(a => a.DataKey == key).DataValue;
            return "";
        }
        public void DeleteCache()
        {
            
            DataCaching.RemoveCache("SystemConfigData");
        }
    }
}

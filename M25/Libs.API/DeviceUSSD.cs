using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class DeviceUSSD
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ProviderCode { get; set; }
        public int Status { get; set; }
        public DeviceUSSD()
        {

        }
        public List<DeviceUSSD> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<DeviceUSSD>("sp_DeviceUSSD_SelectList");
        }

        public List<DeviceUSSD> GetListByProvides(string providers)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<DeviceUSSD>("sp_DeviceUSSD_SelectList_byProviders",
                string.IsNullOrEmpty(providers) ? new SqlParameter("@ProviderCodes", DBNull.Value) : new SqlParameter("@ProviderCodes", providers));
        }


    }
}

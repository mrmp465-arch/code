using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class DeviceSimUSSD
    {
        public string ClientId { get; set; }
        public string Sim { get; set; }
        public int Slot { get; set; }
        public string Telco { get; set; }
        public string Name { get; set; }
        public long Quota { get; set; }
        public DeviceSimUSSD()
        {

        }
        public DeviceSimUSSD GetSimUSSDCondition(string providerCode, string cardType)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<DeviceSimUSSD>("sp_Providers_Select_SimUSSD_Condition", new SqlParameter("@ProviderCode", providerCode), new SqlParameter("@CardType", cardType));
        }
    }
}

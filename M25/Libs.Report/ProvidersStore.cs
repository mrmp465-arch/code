using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Db;
using System.Data;
using System.Data.SqlClient;
using Libs.Utils;

namespace Libs.Report
{
    public class ProvidersStore
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProviderCode { get; set; }
        public int Status { get; set; }

        public List<ProvidersStore> GetList()
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            var lst = db.GetListSP<ProvidersStore>("sp_Providers_SelectList");
            return lst;
        }
        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            return db.GetDataTableSP("sp_Providers_SelectList");
        }

    }


}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using System.Linq;

namespace Libs.Report
{
      

    public class ProductsStore
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int SubType { get; set; }
        public int ReturnValue { get; set; }
        public ProductsStore()
        {

        }

        public List<ProductsStore> GetList()
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            var lst=db.GetListSP<ProductsStore>("sp_Products_SelectList");
            return lst;
        }

        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            return db.GetDataTableSP("sp_Products_SelectList");
        } 
    }
}

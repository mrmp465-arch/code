using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Libs.Db;

namespace Libs.Report
{
    public class ProvidersDiscount
    {
        public long Id { get; set; }
        public int Time { get; set; }
        public string ProviderCode { get; set; }
        public DateTime Date { get; set; }
        public decimal Discount { get; set; }
        public decimal Reward { get; set; }


        public DataTable GetTableProvidersDiscount(string providerCode, int year, int month)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ProviderCode", providerCode);
            pars[1] = new SqlParameter("@Year", year);
            pars[2] = new SqlParameter("@Month", month);           
            DataTable dt = db.GetDataTableSP("sp_ProvidersDiscount_Select", pars);           
            return dt;
            
        }

        public long Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };            
            pars[1] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[2] = new SqlParameter("@Date", Date);
            pars[3] = new SqlParameter("@Discount", Discount);
            pars[4] = new SqlParameter("@Reward", Reward);

            db.ExecuteNonQuerySP("sp_ProvidersDiscount_Insert", pars);
            return Convert.ToInt64(pars[0].Value);
        }


    }
}

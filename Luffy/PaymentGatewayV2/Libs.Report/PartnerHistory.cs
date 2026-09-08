using Libs.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Report
{
    public class PartnerHistory
    {
        public DateTime Time { get; set; }
        public string PartnerCode { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
        public long Amount { get; set; }
        public long BalanceBefore { get; set; }
        public long BalanceAfter { get; set; }

        public List<PartnerHistory> GetList(string PartnerCode, int Type, int pageNumber, int pageSize, ref int TotalRecord,  string keyword = "")


        {
            try
            {
                var orderby = "Id DESC";
                var select = " *";
                var where = "";
                if (!string.IsNullOrEmpty(PartnerCode))
                {

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " PartnerCode =" + "'" + PartnerCode + "'";


                }
                if (Type > -1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    if (Type == 1)
                    {
                        where += " Note like N'%nạp thẻ%' ";
                    }
                    if (Type == 2)
                    {
                        where += " Note like N'%nạp bank%' ";
                    }
                    if (Type == 3)
                    {
                        where += " Note like N'%rút bank%' ";
                    }
                    if (Type == 4)
                    {
                        where += " Note like N'%rút tiền%' ";
                    }
                }
               
                if (!string.IsNullOrEmpty(keyword))
                {

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Note like N'%" + keyword + "%' ";


                }
               
                var pars = new SqlParameter[6];
                pars[5] = new SqlParameter("@SelectQuery", select);
                pars[0] = new SqlParameter("@WhereCondition ", where);
                pars[1] = new SqlParameter("@OrderByExpression", orderby);
                pars[2] = new SqlParameter("@PageIndex", pageNumber);
                pars[3] = new SqlParameter("@PageSize", pageSize);
                pars[4] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                var list = db.GetListSP<PartnerHistory>("sp_PartnerHistory_SelectPagedDynamic", pars);
                TotalRecord = Convert.ToInt32(pars[4].Value);
                
                //TotalDeduct = Convert.ToInt64(pars[6].Value);
                //TotalTopup = Convert.ToInt64(pars[7].Value);
                return list;
            }
            catch (Exception ex)
            {
               
                TotalRecord = 0;
                return new List<PartnerHistory>();
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Db;
using Libs.Report;

namespace Libs.Report
{

    public class Graph3DayData
    {
        public int Time { set; get; }
        public long TotalThisDay { set; get; }
        public long TotalLastDay { set; get; }
        public long TotalLast3Day { set; get; }
    }

    public class GraphPieData
    {
        public string value { set; get; }
        public string color { set; get; }
        public string highlight { set; get; }
        public string label { set; get; }
        public string cssClass { set; get; }
        public string totalTran { set; get; }

    }

    public class ProductType1
    {
        public string name { set; get; }
        public string code { set; get; }
        public string cssClass { set; get; }
        
    }



    public class Dashboard
    {
        string[] cssClass =
        {
            
            "text-light-blue",
            "text-gray",
            "text-blue",
            "text-red",
            "text-green",
            "text-yellow",
            "text-aqua",
            "text-lime",
            "text-orange"
        };

        string[] color =
        {
            
            "#3c8dbc",
            "#d2d6de",
            "#0073b7",
            "#f56954",
            "#00a65a",
            "#f39c12",
            "#00c0ef",
            "#01ff70",
            "#ff851b"
        };

        public List<Graph3DayData> Report3DayLineChart(string partnerIds, string provider, string cardType)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = string.IsNullOrEmpty(partnerIds) ? new SqlParameter("@partnerIds", DBNull.Value) : new SqlParameter("@partnerIds", partnerIds);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            //pars[1] = new SqlParameter("@Provider", provider);
            //pars[2] = new SqlParameter("@CardType", cardType);


            return db.GetListSP<Graph3DayData>("sp_CardAPILog_Report3DayLineChart", pars);
        }

        public void ReportDashboardStatic(int Month,string partnerIds, string providerCodes, ref long totalThisMonth, ref long totalLastMonth, ref int totalProvider, ref int totalPartner)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(partnerIds) ? new SqlParameter("@partnerIds", DBNull.Value) : new SqlParameter("@partnerIds", partnerIds);
            pars[1] = string.IsNullOrEmpty(providerCodes) ? new SqlParameter("@ProviderCodes", DBNull.Value) : new SqlParameter("@ProviderCodes", providerCodes);
            pars[2] = new SqlParameter("@TotalThisMonth", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[3] = new SqlParameter("@TotalLastMonth", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[4] = new SqlParameter("@TotalProvider", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[5] = new SqlParameter("@TotalPartner", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@Month", Month);
            db.ExecuteNonQuerySP("sp_CardAPILog_ReportDashboardStatic", pars);
            totalThisMonth = Convert.ToInt64(pars[2].Value);
            totalLastMonth = Convert.ToInt64(pars[3].Value);
            totalProvider = Convert.ToInt32(pars[4].Value);
            totalPartner = Convert.ToInt32(pars[5].Value);

        }

        public List<GraphPieData> ReportPieChart(string partnerIds, string cardType, string provider)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int totalTransaction = 0;
            long totalAmount = 0;
            //string Provider = string.Empty;

            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(partnerIds) ? new SqlParameter("@PartnerIDs", DBNull.Value) : new SqlParameter("@PartnerIDs", partnerIds);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable data = db.GetDataTableSP("sp_CardAPILog_ReportPieChart", pars);
            totalTransaction = Convert.ToInt32(pars[6].Value);
            totalAmount = Convert.ToInt64(pars[7].Value);

            List<GraphPieData> graphPieDatas = new List<GraphPieData>();
            var i = 0;
            graphPieDatas = data.AsEnumerable().Select((row, index) =>    
            new GraphPieData
                {
                    value = ((row.Field<int>("TotalTransaction")*100.0)/totalTransaction).ToString("F2"),
                    color = color[index],
                    highlight = color[index],
                    label = row.Field<string>("CardType").ToUpper(),
                    cssClass = cssClass[index],
                    totalTran = row.Field<int>("TotalTransaction").ToString()

            }).ToList();
            return graphPieDatas;
        }

        private List<ProductType1> GetProductsStyle()
        {


            Products objProducts = new Products();
            var products = objProducts.GetList(1);

            List<ProductType1> productType1s = new List<ProductType1>();

            var i = 0;
            foreach (var p in products)
            {
                var pt = new ProductType1()
                {
                    code = p.Code,
                    name = p.Name,
                    cssClass = cssClass[i],
                };
                productType1s.Add(pt);
                i++;
            }

            return productType1s;
        }
    }

}

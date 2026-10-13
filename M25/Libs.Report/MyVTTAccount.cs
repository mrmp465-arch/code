using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Libs.Report
{
    public class MyVTTAccountReport
    {
        public int Total { get; set; }
        public int TotalReady { get; set; }
        public int TotalError { get; set; }
        public int DayReady { get; set; }
        public int DayTurnReady { get; set; }
        public int MonthReady { get; set; }
        public int MonthTurnReady { get; set; }

        // Lấy báo cáo
        public MyVTTAccountReport Report(string accountType, string source)
        {
            DBHelper db = new DBHelper(Configs.CaptchaReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = string.IsNullOrEmpty(source) ? new SqlParameter("@source", DBNull.Value) : new SqlParameter("@source", source);
            var accountReport = db.GetInstanceSP<MyVTTAccountReport>("sp_MyVTTAccount_Report", pars);
            return accountReport;
        }

      
    }
}

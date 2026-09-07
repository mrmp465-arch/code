using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Libs.Report
{
    public class APICheckCardLog
    {
        public string PartnerCode { get; set; }
        public string CommandCode { get; set; }
        public int UsedCard { get; set; }
        public int UnusedCard { get; set; }
        public int SerialValid { get; set; }
        public int SerialInvalid { get; set; }
        public int CardNotActivated { get; set; }
        public int CardValue { get; set; }

        public int CountOfPartnerCode { get; set; }
        public bool IsFirstRowWithPartnerCode { get; set; }

        public int CountOfCommandCode { get; set; }
        public bool IsFirstRowWithCommandCode { get; set; }

        // Lấy báo cáo
        public List<APICheckCardLog> Report(string partnerCode, string cardType, DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.CaptchaReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = string.IsNullOrEmpty(partnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", partnerCode);
            pars[1] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            pars[2] = new SqlParameter("@BeginTime", beginTime);
            pars[3] = new SqlParameter("@EndTime", endTime);
            var ls = db.GetListSP<APICheckCardLog>("sp_APICheckCardLog_ReportDS", pars);
            return ls;
        }


      
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using DocumentFormat.OpenXml.Packaging;

namespace Libs.Report
{
    public class Utils
    {
        public void  ExportedExcel(DataTable dt, string orderNo, HttpResponse httpResponse)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                //wb.Theme = 
                wb.DeliverToHttpResponse(httpResponse, string.Format("{0}.xlsx", orderNo), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}

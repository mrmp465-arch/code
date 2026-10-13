using CMS.Data.DTO;
using CMS.Data.Factory;
using CMS.Filter;
using CMS.Utility;
using Libs.API;
using Libs.Report;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class CardController : Controller
    {
        // GET: Card
        public UserFunction Permission { get { return ((UserFunction)Session[SessionsManager.SESSION_PERMISSION]); } }
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Data.DTO.Users CurrentFullUser { get { return ((Data.DTO.Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }


        [PermissionFilter(FunctionCode = FunctionCode.CardLog)]
        public ActionResult Index()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddMinutes(-1);
            var ProviderList = new List<Providers>();
            var PartList = new List<Partners>();
            if (CurrentUser.Type == 1 || CurrentUser.Type == 4)
            {
                ProviderList = new Providers().GetList(7).OrderBy(x => x.ProviderCode).ToList();
                PartList = new Partners().GetList().OrderBy(x => x.PartnerCode).ToList();
            }
            else
            {
                //PartList = new Partners().GetListByUserId(CurrentUser.UserID).OrderBy(x => x.PartnerCode).ToList();
                var data = AbstractDAOFactory.Instance().UserPartnersService().GetList(CurrentUser.UserID);
                foreach(var item in data)
                {
                    var p = new Partners
                    {
                        PartnerCode = item.PartnerCode,
                        PartnerID = item.PartnerId
                    };
                    PartList.Add(p);
                }    
            }

            ProviderList.Insert(0, new Providers { ProviderCode = "", Name = "--Nhà cung cấp--" });
            PartList.Insert(0, new Partners { PartnerCode = "", Name = "--Đối tác-" });

            ViewBag.fromDate = fromDate.ToString("yyyy-MM-dd HH:mm");
            ViewBag.toDate = toDate.ToString("yyyy-MM-dd HH:mm");
            ViewBag.PartList = PartList;
            ViewBag.ProviderList = ProviderList;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.CardLog)]
        public ActionResult ListCardLog(string FromDate, string ToDate, int? status, string cardType, string partner, string provider,string key)
        {
            DateTime fromdate = DateTime.ParseExact(FromDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(ToDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            string newFromDate = fromdate.ToString("MM/dd/yyyy HH:mm:ss");
            string newToDate = todate.ToString("MM/dd/yyyy HH:mm:ss");

            string partnerCodes = partner;
            if (CurrentUser.Type != 1 && CurrentUser.Type != 4)
            {
                if (string.IsNullOrEmpty(partnerCodes))
                {
                    var lstPartner = AbstractDAOFactory.Instance().UserPartnersService().GetList(CurrentUser.UserID);
                    if (lstPartner != null && lstPartner.Count > 0)
                    {
                        partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                    }
                }
            }
            ViewBag.UserType = CurrentUser.Type;
            CardAPILog _CardAPILog = new CardAPILog();
            var data = _CardAPILog.GetTableListV2(100, partnerCodes, fromdate, todate, status, cardType, provider,key);
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.CardReport)]
        public ActionResult Report()
        {
            var ProviderList = new List<Providers>();
            var PartList = new List<Partners>();
            if (CurrentUser.Type == 1 || CurrentUser.Type == 4)
            {
                ProviderList = new Providers().GetList(7).OrderBy(x => x.ProviderCode).ToList();
                PartList = new Partners().GetList().OrderBy(x => x.PartnerCode).ToList();
            }
            else
            {
                var data = AbstractDAOFactory.Instance().UserPartnersService().GetList(CurrentUser.UserID);
                foreach (var item in data)
                {
                    var p = new Partners
                    {
                        PartnerCode = item.PartnerCode,
                        PartnerID = item.PartnerId
                    };
                    PartList.Add(p);
                }
            }

            ProviderList.Insert(0, new Providers { ProviderCode = "", Name = "--Nhà cung cấp--" });
            PartList.Insert(0, new Partners { PartnerCode = "", Name = "--Đối tác-" });


            ViewBag.PartList = PartList;
            ViewBag.ProviderList = ProviderList;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.CardReport)]
        public ActionResult ListCardReport(int Year, int Month, int Day, string partner, string provider)
        {

            string partnerCodes = partner;
            if (CurrentUser.Type != 1 && CurrentUser.Type != 4)
            {
                if (string.IsNullOrEmpty(partnerCodes))
                {
                    var lstPartner = AbstractDAOFactory.Instance().UserPartnersService().GetList(CurrentUser.UserID);
                    if (lstPartner != null && lstPartner.Count > 0)
                    {
                        partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                    }
                }
            }
            ViewBag.UserType = CurrentUser.Type;
            CardAPILog _CardAPILog = new CardAPILog();

            var Data = _CardAPILog.ReportCardType2(partnerCodes, provider, Year, Month, Day);



            var lstCardType = Data.GroupBy(x => x.CardType).Select(a => a.Key).OrderBy(x => x).ToList();
            var Time = Data.GroupBy(x => x.Time).Select(a => a.Key).ToList();
            ViewBag.lstCardType = lstCardType;
            ViewBag.Time = Time;
            return PartialView(Data);
        }

        [PermissionFilter(FunctionCode = FunctionCode.CardDS)]
        public ActionResult ReportDS()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            if (CurrentFullUser.Username.ToLower() == "imd")
            {
                ViewBag.fromDate = DateTime.Now.AddDays(-1);
                ViewBag.toDate = DateTime.Now.AddDays(-1);
            }

            var PartList = new List<Partners>();
            if (CurrentUser.Type == 1 || CurrentUser.Type == 4)
            {
                PartList = new Partners().GetList().OrderBy(x => x.PartnerCode).ToList();
            }
            else
            {
                var data = AbstractDAOFactory.Instance().UserPartnersService().GetList(CurrentUser.UserID);
                foreach (var item in data)
                {
                    var p = new Partners
                    {
                        PartnerCode = item.PartnerCode,
                        PartnerID = item.PartnerId
                    };
                    PartList.Add(p);
                }
            }
            PartList.Insert(0, new Partners { PartnerCode = "", Name = "--Đối tác-" });

            ViewBag.PartList = PartList;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.CardDS)]
        public ActionResult ListCardLogDSExcel(string FromDate, string ToDate, string cardType, string partner)
        {
            DateTime fromdate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1);


            string partnerCodes = partner;
            if (CurrentUser.Type != 1 && CurrentUser.Type != 4)
            {
                if (string.IsNullOrEmpty(partnerCodes))
                {
                    var lstPartner = new Partners().GetListByUserId(CurrentUser.UserID);
                    if (lstPartner != null && lstPartner.Count > 0)
                    {
                        partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                    }
                }
            }
            var Data = new CardAPILog().ListReportDoiSoat(partnerCodes, "", cardType, fromdate, todate, 1);
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Card");
                        worksheet.Column(1).Width = 14;
                        worksheet.Column(2).Width = 19;
                        worksheet.Column(3).Width = 19;
                        worksheet.Column(4).Width = 14;
                        worksheet.Column(5).Width = 19;
                        worksheet.Column(6).Width = 19;
                        worksheet.Column(7).Width = 19;
                        worksheet.Column(8).Width = 19;
                        worksheet.Column(9).Width = 19;
                        worksheet.Cells["A1"].Value = "TransactionID";
                        worksheet.Cells["B1"].Value = "CardSerial";
                        worksheet.Cells["C1"].Value = "CardCode";
                        worksheet.Cells["D1"].Value = "CardType";
                        worksheet.Cells["E1"].Value = "CreatTime";
                        worksheet.Cells["F1"].Value = "RequestNo";
                        worksheet.Cells["G1"].Value = "Mệnh giá khai báo";
                        worksheet.Cells["H1"].Value = "Mệnh giá thực";
                        worksheet.Cells["I1"].Value = "Mện giá chốt";
                        worksheet.Row(1).Style.Font.Bold = true;

                        int rowTotal = 1;
                        foreach (var item in Data)
                        {
                            rowTotal++;
                            worksheet.Cells["A" + rowTotal].Value = item.TransactionID;
                            worksheet.Cells["B" + rowTotal].Value = item.CardSerial;
                            worksheet.Cells["C" + rowTotal].Value = item.CardCode;
                            worksheet.Cells["D" + rowTotal].Value = item.CardType;
                            worksheet.Cells["E" + rowTotal].Value = item.CreatTime.ToString("dd/MM/yyyy HH:mm:ss");
                            worksheet.Cells["F" + rowTotal].Value = item.RequestNo;
                            worksheet.Cells["G" + rowTotal].Value = item.AmountUser;
                            worksheet.Cells["H" + rowTotal].Value = item.Amount;
                            worksheet.Cells["I" + rowTotal].Value = item.AmountReal;

                        }
                        rowTotal++;
                        worksheet.Row(rowTotal).Style.Font.Bold = true;
                        worksheet.Cells["A" + rowTotal].Value = "Tổng";
                        worksheet.Cells["I" + rowTotal].Formula = $"sum(I2:I{ rowTotal - 1})";
                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                Response.AddHeader("Content-disposition", "attachment; filename=Card.xlsx");
                Response.ContentType = "text/xls";
                Response.BinaryWrite(bytes);
                Response.End();
            }
            catch (Exception ex)
            {

                NLogLogger.PublishException(ex);
                return RedirectToAction("ReportDS");
            }
            return RedirectToAction("ReportDS");
        }
        [PermissionFilter(FunctionCode = FunctionCode.CardDS)]
        public ActionResult ListCardLogDS(string FromDate, string ToDate, string cardType, string partner)
        {
            DateTime fromdate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1);


            string partnerCodes = partner;
            if (CurrentUser.Type != 1 && CurrentUser.Type != 4)
            {
                if (string.IsNullOrEmpty(partnerCodes))
                {
                    var lstPartner = new Partners().GetListByUserId(CurrentUser.UserID);
                    if (lstPartner != null && lstPartner.Count > 0)
                    {
                        partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                    }
                }
            }
            CardAPILog _CardAPILog = new CardAPILog();
            int Amount = 0;
            var Data = _CardAPILog.ReportDoiSoat(partnerCodes, "", cardType, fromdate, todate, 1, ref Amount);
            ViewBag.lstPartner = Data.GroupBy(x => x.PartnerCode).Select(a => a.Key).OrderBy(x => x).ToList();
            ViewBag.Amount = Amount;
            return PartialView(Data);

        }
    }
}
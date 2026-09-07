using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.BankDirect.GPay;
using Libs.BankDirect.MDrum;
using Libs.BankDirect.MDrumV2;
using Libs.Utils;

namespace BankGateV2.Pages
{
    public partial class Bank : System.Web.UI.Page
    {
        public string OrderNo { get; set; }
        public string QRCodeBase64 { get; set; }
        public string LinkOpenApp { get; set; }
        public string MomoId { get; set; }
        public string MomoName
        {
            get; set;
        }
        public string BankCode
        {
            get; set;
        }
        public string Amount { get; set; }
        public string Amount2 { get; set; }

        public DateTime CurrentTime { get; set; }
        public DateTime ExpTime { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            //var type = AppUtils.GetParam("type", string.Empty);
            var orderNo = AppUtils.GetParam("orderNo", string.Empty);
            if (String.IsNullOrEmpty(orderNo))
            {
                pn_hide_info.Visible = true;
                pn_show_info.Visible = false;
                return;
            }

            var bankinfo = MDrumBankLib.GetBankInfo(orderNo);
            if (bankinfo == null)
            {
                pn_hide_info.Visible = true;
                pn_show_info.Visible = false;
                return;
            }
            //if(bankinfo.CreateDate.Value.AddMinutes(30)<=DateTime.Now)
            //{
            //    pn_hide_info.Visible = true;
            //    pn_show_info.Visible = false;
            //    return;
            //}
            if (bankinfo.Status == "1")
            {
                pnSuccess.Visible = true;
                pn_hide_info.Visible = false;
                pn_show_info.Visible = false;
                return;
            }
            CurrentTime = DateTime.Now;
            ExpTime = bankinfo.CreateDate.Value.AddMinutes(5);
            //if (type=="link")
            //{
            //    Response.Redirect(bankinfo.LinkOpenApp);
            //    return;
            //}

            OrderNo = bankinfo.OrderNo;
            //MomoId = "*******"+bankinfo.MomoId.Substring(bankinfo.MomoId.Length-3);
            MomoId = bankinfo.BankId;
            MomoName = bankinfo.BankName;
            QRCodeBase64 = String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}&addInfo={3}", bankinfo.BankCode, bankinfo.BankId, bankinfo.Amount, bankinfo.OrderNo);
            //Amount2 = bankinfo.Amount.ToString();

            var QRText = VietQrEmvBuilder.BuildEmv(
                   bankinfo.BankCode,
                   bankinfo.BankId,
                   (decimal)bankinfo.Amount,
                   bankinfo.OrderNo
                    );



            QRCodeBase64 = QrHelper.EmvToBase64_500px(QRText);


            Amount = bankinfo.Amount.ToString();
            BankCode = bankinfo.BankCode.ToString();
            switch (bankinfo.BankCode)
            {
                case "VCB":
                    BankCode = "VCB Ngoại Thương VN";
                    break;
                case "BIDV":
                    BankCode = "BIDV";
                    break;
                case "MB":
                    BankCode = "MB Quân đội";
                    break;
                case "ACB":
                    BankCode = "Á Châu ACB";
                    break;
                case "SEAB":
                    BankCode = "SeaBank";
                    break;
                case "NAB":
                    BankCode = "Nam Á";
                    break;
                case "VPB":
                    BankCode = "VPBank";
                    break;
            }    
            //NLogLogger.Info("Amount:" +Amount);
            //LinkOpenApp = String.Format("https://momo.opaps.info/Pages/MomoLink.aspx?orderNo={0}", bankinfo.OrderNo);

        }
    }
}
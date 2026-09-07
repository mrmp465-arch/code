using Libs.API;
using Libs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace BankGateV2.Pages
{
    public partial class Detail : System.Web.UI.Page
    {
        public string Time { get; set; }
        public BankCashAPI BankInfo { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack) return;
            //var type = AppUtils.GetParam("type", string.Empty);
            var orderNo = AppUtils.GetParam("orderNo", string.Empty);

            Libs.Report.BankCashAPI _BankCash = new Libs.Report.BankCashAPI();
            _BankCash.TransactionID = Convert.ToInt64(orderNo);
            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();

            _BankCash = _BankCash.Get();
            if (_BankCash != null)
            {
                BankInfo = _BankCash;
                if (string.IsNullOrEmpty(_BankCash.Mobile))
                {
                    var check2 = new BankTransaction().GetByRefcode(_BankCash.TransactionID.ToString());
                    if (check2 == null)
                    {
                        check2 = new BankTransaction().GetByRefcode(_BankCash.TransactionID.ToString() + "_recall");
                        if (check2 == null)
                        {
                            check2 = new BankTransaction().GetByRefcode(_BankCash.TransactionID.ToString() + "_re2call");
                        }
                    }

                    if (check2 != null)
                    {
                        BankInfo.Mobile = check2.PartnerBankCode + "-" + check2.PartnerBankId + "-" + BankInfo.Note.ToUpper().Replace("CHUYEN TIEN", "").Replace("CHUYEN KHOAN", "");
                    }
                    else
                    {
                        BankInfo.Mobile = "VPB-181010101464516-NGUYEN VAN BAY";
                    }
                }
                if (_BankCash.Mobile.Contains("VPB"))
                {
                    dvVPB.Visible = true;
                }

                if (_BankCash.Mobile.Contains("ACB"))
                {
                    Response.Redirect("DetailACB.aspx?orderNo=" + orderNo);
                    //var lstbankcode = new BankCodeTranfer().GetListCache();

                    //if (lstbankcode.Exists(x => x.code.ToUpper() == BankInfo.BankCode.ToUpper()))
                    //{
                    //    BankInfo.BankCode = lstbankcode.FirstOrDefault(x => x.code.ToUpper() == BankInfo.BankCode.ToUpper()).shortName;
                    //}
                    //dvACB.Visible = true;
                }

                if (_BankCash.Mobile.Contains("SEAB"))
                {
                    var lstbankcode = new BankCodeTranfer().GetListCache();

                    if (lstbankcode.Exists(x => x.code.ToUpper() == BankInfo.BankCode.ToUpper()))
                    {
                        BankInfo.BankCode = lstbankcode.FirstOrDefault(x => x.code.ToUpper() == BankInfo.BankCode.ToUpper()).shortName;
                    }
                    dvSEA.Visible = true;
                }
            }
            else
            {
                pn_hide_info.Visible = true;
            }
        }
        private static readonly string[] ChuSo =
    {
        "không", "một", "hai", "ba", "bốn",
        "năm", "sáu", "bảy", "tám", "chín"
    };

        private static readonly string[] Tien =
        {
        "", "nghìn", "triệu"
    };

        public static string DocTien(int number)
        {
            if (number < 0 || number > 300000000)
                throw new ArgumentException("Chỉ hỗ trợ số từ 0 đến 300 triệu.");

            if (number == 0)
                return "Không đồng";

            string result = "";
            int unitIndex = 0;

            while (number > 0)
            {
                int block = number % 1000;
                if (block != 0)
                {
                    result = DocBaChuSo(block) + " " + Tien[unitIndex] + " " + result;
                }
                number /= 1000;
                unitIndex++;
            }

            result = result.Trim();
            result = char.ToUpper(result[0]) + result.Substring(1);

            return result + " đồng";
        }

        private static string DocBaChuSo(int number)
        {
            int tram = number / 100;
            int chuc = (number % 100) / 10;
            int donvi = number % 10;

            string result = "";

            if (tram > 0)
            {
                result += ChuSo[tram] + " trăm ";
                if (chuc == 0 && donvi > 0)
                    result += "lẻ ";
            }

            if (chuc > 1)
            {
                result += ChuSo[chuc] + " mươi ";
                if (donvi == 1)
                    result += "mốt ";
                else if (donvi == 5)
                    result += "lăm ";
                else if (donvi > 0)
                    result += ChuSo[donvi] + " ";
            }
            else if (chuc == 1)
            {
                result += "mười ";
                if (donvi == 5)
                    result += "lăm ";
                else if (donvi > 0)
                    result += ChuSo[donvi] + " ";
            }
            else if (donvi > 0)
            {
                result += ChuSo[donvi] + " ";
            }

            return result.Trim();
        }
    }
}
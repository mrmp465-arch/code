using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using APIGame;
using APIGame.Entity;
using Lib.Captcha;
using Libs.API;
using Libs.Report;
using Libs.Utils;

public partial class Pages_Topup_Monitor_Detail : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupMonitorDetail);

        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        var _TopupMobileLog = new TopupMobile3rdLog();
        _TopupMobileLog.Id = Convert.ToInt64(Request["id"]);
        _TopupMobileLog = _TopupMobileLog.Get();
        if (_TopupMobileLog != null)
        {
            lblId.Text = _TopupMobileLog.Id.ToString();
            lblCardSerial.Text = _TopupMobileLog.CardSerial;
            lblCardCode.Text = _TopupMobileLog.CardCode;
            lblRequestNo.Text = _TopupMobileLog.RequestNo.ToString();
            lblTransactionID.Text = _TopupMobileLog.TransactionId.ToString();
            lblPartnerCode.Text = _TopupMobileLog.PartnerCode;
            lblProviderCode.Text = _TopupMobileLog.ProviderCode;
            lblTelco.Text = _TopupMobileLog.Telco;
            lblMobile.Text = _TopupMobileLog.Sim;
            lblMobileTarget.Text = _TopupMobileLog.SimTarget;
            lblAmount.Text = _TopupMobileLog.Amount.ToString();
            lblAmountUser.Text = _TopupMobileLog.AmountUser.ToString();
            lblLogContent.Text = _TopupMobileLog.LogContent;
            lblCreatedTime.Text = _TopupMobileLog.CreateTime.ToString();
            lblLastTime.Text = _TopupMobileLog.LastTime.ToString();
            lblStatus.Text = _TopupMobileLog.Status + " (" + ResponseUtils.Description(Convert.ToInt32(_TopupMobileLog.Status)) + ")";
            hdStatus.Value = _TopupMobileLog.Status.ToString();

            if (_TopupMobileLog.Status <= 0)
            {
                txtRecheck.Visible = true;
            }
        }


    }


    protected void txtRecheck_Click(object sender, EventArgs e)
    {
        string urlCheck = string.Empty;
        string privateKey = string.Empty;
        btnActionSuccess.Visible = false;
        btnAcctionFailed.Visible = false;
        if (lblTelco.Text.ToLower() == "vtt")
        {
            urlCheck = "http://localhost:1585/CheckCard.asmx";
            privateKey = "1e7f56ca5fcbf781fa022f4f5dff74d6";
            var signature = Encrypts.MD5(string.Format("{0}|{1}", lblCardSerial.Text, privateKey));
            var service = new APIProxy.VTTService.CheckCard(urlCheck);
            var response = service.CheckSerial(lblCardSerial.Text, signature);
            NLogLogger.Info(new string[] { "CheckCard", response });
            //lblRecheck.Text = response;
            APIResponse res = null;
            if (!string.IsNullOrEmpty(response))
            {
                lblRecheck.Text = response;
                res = serializer.Deserialize<APIResponse>(response);
            }
            else
            {
                lblRecheck.Text = "Không kiểm tra được mã thẻ";
            }

            if (res != null && (hdStatus.Value == ((int)ResponseCode.TransactionSuspicious).ToString()
                                || hdStatus.Value == ((int)ResponseCode.TransactionRejected).ToString()
                                || hdStatus.Value == ((int)ResponseCode.TransactionFailed).ToString()))
            {
                NLogLogger.Info(new string[] { "CheckCard ResponseContent", res.ResponseContent });
                if (!string.IsNullOrEmpty(res.ResponseContent))
                {

                    if (res.ResponseCode == (int)ResponseCode.CardUsed)
                    {
                        var card = serializer.Deserialize<APIProxy.VTTService.VTTCardEntity>(res.ResponseContent);
                        DateTime createTime = DateTime.ParseExact(lblCreatedTime.Text, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                        DateTime lastTime = DateTime.ParseExact(lblLastTime.Text, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                        DateTime dateUsed = DateTime.ParseExact(card.dateUsed, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                        var totalsec = (dateUsed - lastTime).TotalSeconds;
                        string isdn = string.Empty;
                        hdAmount.Value = card.cardValue;
                        txtAmount.Text = card.cardValue;
                        if (lblMobileTarget.Text.Contains(card.isdn))
                        {
                            isdn = "s, khớp ISDN)";
                        }
                        else
                        {
                            isdn = "s, lệch ISDN)";
                        }

                        //if (totalsec >= -3 && totalsec <= 3)
                        //if (dateUsed >= createTime && dateUsed <= lastTime)
                        //{
                        //    btnActionSuccess.Text = "Thành công (Time: " + totalsec + isdn;
                        //    btnActionSuccess.Visible = true;
                        //}
                        //else
                        //{
                        //    btnAcctionFailed.Text = "Thất bại (Time: " + totalsec + isdn;
                        //    btnAcctionFailed.Visible = true;
                        //}
                        btnActionSuccess.Text = "Thành công (Time: " + totalsec + isdn;
                        btnActionSuccess.Visible = true;
                        btnAcctionFailed.Text = "Thất bại (Time: " + totalsec + isdn;
                        btnAcctionFailed.Visible = true;
                    }

                    else if (res.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                    {

                        hdAmount.Value = lblAmountUser.Text;
                        btnAcctionFailed.Text = "Thất bại (Thẻ chưa sử dụng)";
                        btnAcctionFailed.Visible = true;
                    }
                }
                else if (res.ResponseCode == (int)ResponseCode.TransactionLimit)
                {
                    if (AppUtils.IsAdmin || AppUtils.IsTopup)
                    {
                        txtAmount.Visible = true;
                        hdAmount.Value = txtAmount.Text = lblAmountUser.Text;
                        btnActionSuccess.Text = "Thành công";
                        btnAcctionFailed.Text = "Thất bại";
                        btnActionSuccess.Visible = true;
                        btnAcctionFailed.Visible = true;
                    }
                }

                else if (res.ResponseCode == (int)ResponseCode.CardSerialInvalid)
                {
                    if (AppUtils.IsAdmin || AppUtils.IsTopup)
                    {
                        //hdAmount.Value = lblAmountUser.Text;
                        //btnAcctionFailed.Text = "Thất bại";
                        //btnAcctionFailed.Visible = true;
                        txtAmount.Visible = true;
                        hdAmount.Value = txtAmount.Text = lblAmountUser.Text;
                        btnActionSuccess.Text = "Thành công";
                        btnAcctionFailed.Text = "Thất bại";
                        btnActionSuccess.Visible = true;
                        btnAcctionFailed.Visible = true;
                    }
                }
            }
            else
                lblRecheck.Text = response;


        }
        else if (lblTelco.Text.ToLower() == "vnp" || lblTelco.Text.ToLower() == "vms" || lblTelco.Text.ToLower() == "vcoin" || lblTelco.Text.ToLower() == "garena")
        {
            lblRecheck.Text = "Loại thẻ này chưa có phương thức kiểm tra online. Hãy gọi tổng đài để biết tình trạng mã thẻ (Quan trọng nhất là mệnh giá thẻ)";
            if (AppUtils.IsAdmin || AppUtils.IsTopup)
            {
                txtAmount.Visible = true;
                hdAmount.Value = txtAmount.Text = lblAmountUser.Text;
                btnActionSuccess.Text = "Thành công";
                btnAcctionFailed.Text = "Thất bại";
                btnActionSuccess.Visible = true;
                btnAcctionFailed.Visible = true;
            }
        }
        else if (lblTelco.Text.ToLower() == "zing")
        {

            var getTopup = new GameCookie();
            var tranHis = new TransHistory();
            //var tranId = serializer.Deserialize<APIResponse>(lblLogContent.Text.Split('{')[1].Insert(0, "{")).Description.Split(':')[1].Trim();
            string tranId = Regex.Replace(serializer.Deserialize<APIResponse>(lblLogContent.Text.Split('{')[1].Insert(0, "{")).Description, @"[^\d]", "");
            NLogLogger.Info(new string[] { "UtilsZing", "CheckTran", "TranId", tranId });
            var order = new TopupMobileLog().Get(Convert.ToInt64(lblRequestNo.Text));

            var gameType = 0;
            switch (order.TopupType)
            {
                case 9:
                    gameType = 1; //Võ Lâm Truyền Kỳ Miễn Phí
                    break;
                case 11:
                    gameType = 2; //VLTK - Công Thành Chiến
                    break;
                case 12:
                    gameType = 3; //Võ Lâm Truyền Kỳ 1
                    break;
                case 13:
                    gameType = 4; //Kiếm Thế
                    break;
                case 14:
                    gameType = 5; //Tân Thiên Long 3D
                    break;
                case 15:
                    gameType = 6; //Tân Thiên Long 3D
                    break;
                case 17:
                    gameType = 7; //Tân Thiên Long 3D
                    break;
            }

            getTopup = ZingService.GetTopup(order.AccountName, order.Password, ref tranHis, gameType);

            if (getTopup.IsTopup)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("transID", tranId);
                var postTopupResult = Task.Run(() => UtilsZing.PostTask("https://new.pay.zing.vn/ajax/get-result", parameters, getTopup.CookieContainer)).Result;
                NLogLogger.Info(new string[] { "UtilsZing", "CheckTran", "Response", postTopupResult.HtmlContent });
                lblRecheck.Text = postTopupResult.HtmlContent;
                var paymentResult = serializer.Deserialize<PaymentZingCardResult>(postTopupResult.HtmlContent);

                if (paymentResult.returnCode == 2)
                {
                    txtAmount.Visible = true;
                    hdAmount.Value = paymentResult.grossValue;
                    txtAmount.Text = paymentResult.grossValue;

                }
                else
                {
                    txtAmount.Visible = true;
                    hdAmount.Value = txtAmount.Text = lblAmountUser.Text;
                }

            }
            else
            {
                txtAmount.Visible = true;
                hdAmount.Value = txtAmount.Text = lblAmountUser.Text;
                lblRecheck.Text = getTopup.HtmlContent + " --> Không kiểm tra được giao dịch này. (Bạn nên check với tổng đài Zing support)";
            }

            btnActionSuccess.Text = "Thành công";
            btnActionSuccess.Visible = true;
            btnAcctionFailed.Text = "Thất bại";
            btnAcctionFailed.Visible = true;

        }
        else
        {
            lblRecheck.Text = "Loại thẻ này chưa có chưa có phương thức kiểm tra";
        }
    }

    protected void btnActionSuccess_Click(object sender, EventArgs e)
    {
        hdAmount.Value = txtAmount.Text;
        var urlConfirm = "http://localhost:1583/TopupCallbackAppVTT.asmx";
        var privateKey = "1e7f56ca5fcbf781fa022f4f5dff74d6";
        var signature = Encrypts.MD5(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", lblId.Text, 2, hdAmount.Value, string.Empty, string.Empty, privateKey));
        var service = new APIProxy.VTTService.TopupCallbackAppVTT(urlConfirm);
        var res = service.Callback(lblId.Text, 2, Convert.ToInt32(hdAmount.Value), string.Empty, string.Empty, signature);
        Response.Redirect(Request.RawUrl);
    }

    protected void btnAcctionFailed_Click(object sender, EventArgs e)
    {
        var urlConfirm = "http://localhost:1583/TopupCallbackAppVTT.asmx";
        var privateKey = "1e7f56ca5fcbf781fa022f4f5dff74d6";
        var signature = Encrypts.MD5(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", lblId.Text, -5, 0, string.Empty, string.Empty, privateKey));
        var service = new APIProxy.VTTService.TopupCallbackAppVTT(urlConfirm);
        var res = service.Callback(lblId.Text, -5, 0, string.Empty, string.Empty, signature);
        Response.Redirect(Request.RawUrl);
    }

    protected void txtCallBackProvider_Click(object sender, EventArgs e)
    {
        var urlConfirm = "http://localhost:1583/TopupCallbackAppVTT.asmx";
        var privateKey = "1e7f56ca5fcbf781fa022f4f5dff74d6";
        var signature = Encrypts.MD5(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", lblId.Text, -6, lblAmount.Text, string.Empty, string.Empty, privateKey));
        var service = new APIProxy.VTTService.TopupCallbackAppVTT(urlConfirm);
        var res = service.Callback(lblId.Text, -6, Convert.ToInt32(lblAmount.Text), string.Empty, string.Empty, signature);
        Response.Redirect(Request.RawUrl);
    }
}
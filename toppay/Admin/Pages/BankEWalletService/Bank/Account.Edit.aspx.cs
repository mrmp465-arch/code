using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;

using System.Linq;
using System.Text.RegularExpressions;

public partial class Pages_BankEWalletService_Bank_Account_Edit : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();

    public string Solution
    {
        get
        {
            return ViewState["Solution"].ToString();
        }
        set
        {
            ViewState["Solution"] = value;
        }
    }

    public string UrlBaseService
    {
        get
        {
            return ViewState["UrlBaseService"].ToString();
        }
        set
        {
            ViewState["UrlBaseService"] = value;
        }
    }
    public string Id
    {
        get
        {
            return AppUtils.Request("id").ToString();
        }

    }
    public bool RoleUpload { get; set; }

    public bool RoleTransfer { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        serializer.MaxJsonLength = int.MaxValue;

        btnCashForm4.OnClientClick = "validate([" + txtBankIdForm4.ClientID + "," + txtBankNameForm4.ClientID + "," + txtAmountForm4.ClientID + "]," + btnCashForm4.ClientID + ")";
        btAdd.OnClientClick = "validate(" + ")";
        btnSyncBalance.OnClientClick = "validate(" + ")";
        btnReLogin.OnClientClick = "validate(" + ")";
        btnOtpLogin.OnClientClick = "validate(" + ")";

        btnReLogin2.OnClientClick = "validate(" + ")";
        btnOtpLogin2.OnClientClick = "validate(" + ")";

        AppUtils.CheckRoles(Resources.Url.BankAccountEdit);
        RoleUpload = AppUtils.CheckRolesPermission(Resources.Url.BankAccountUpdateImg);
        //if (RoleUpload)
        //    profileimg.Visible = true;

        RoleTransfer = AppUtils.CheckRolesPermission("pages/bankewalletservice/bank/account.transfer.aspx");
        if (!RoleTransfer)
        {
            btnCashForm4.Visible = false;
            divTransfer.Visible = false;
        }
        if (!IsPostBack)
        {
            init();
        }
    }
    private void init()
    {
        //Control

        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");
        var _bank = new BankAccounts();
        _bank.Id = Convert.ToInt32(AppUtils.Request("id"));
        _bank = _bank.Get();



        if (_bank == null)
        {
            Response.Redirect(Resources.Url.BankAccount);
        }

        var lstBank2 = new ITBank().GetList().Where(x => x.Type == "HOLD" && x.Status == 1).ToList();
        foreach (var item in lstBank2)
        {
            item.BankCode += "-" + item.BankName + "-" + item.BankId;
        }
       
        drpBankCode2.DataSource = lstBank2;
        drpBankCode2.DataTextField = "BankCode";
        drpBankCode2.DataValueField = "BankCode";
        drpBankCode2.DataBind();

        drpBankCode2.Items.Insert(0, new ListItem("Chọn từ tài khoản chứa", ""));


        var lstBank3 = new List<ITBank>();
        var lstBank = new BankAccounts().GetList().Where(x => x.StatusExtra == 1 && x.Status == 1 && x.Type.Contains("OUT"));
        foreach (var item in lstBank)
        {
            item.BankCode += "-" + item.BankName + "-" + item.BankId;
            var obj = new ITBank();
            obj.BankCode = item.BankCode;
            lstBank3.Add(obj);
        }
        drpBankCode3.DataSource = lstBank3;
        drpBankCode3.DataTextField = "BankCode";
        drpBankCode3.DataValueField = "BankCode";
        drpBankCode3.DataBind();

        drpBankCode3.Items.Insert(0, new ListItem("Chọn từ tài khoản out", ""));

        var BankExt = new BankAccounts().GetExt(_bank.BankId, _bank.BankCode);
        if (BankExt != null)
        {
            lblDescription.Text = BankExt.Description;
            lblDescription2.Text = BankExt.Description;
        }
        drpBankCode.SelectedValue = _bank.BankCode;
        drpBankType.SelectedValue = _bank.BankType;
        txtBankId.Text = _bank.BankId;
        txtBankName.Text = _bank.BankName;
        txtBankAccount.Text = _bank.BankAccount;
        txtBankPass.Attributes["value"] = rijndaelKey.Decrypt(_bank.BankPass);
        chkIsActive.Checked = Convert.ToBoolean(_bank.Status);
        drpType.SelectedValue = _bank.Type.ToString();
        drpSolution.SelectedValue = _bank.Solution;
        txtBalanceMaxDay.Text = _bank.BalanceMaxDay.ToString();
        txtBalanceMaxMonth.Text = _bank.BalanceMaxMonth.ToString();
        txtComputer.Text = _bank.Computer;
        txtPhone.Text = _bank.PhoneDevice;
        txtPinOtp.Text = _bank.PinOtp;
        txtAppDeviceId.Text = _bank.AppDeviceId;
        txtCloudPhoneId.Text = _bank.CloudPhoneId;
        lblAccount.Text = _bank.Solution + " - Tài khoản " + _bank.BankCode + " : " + _bank.BankId;

        ViewState["Solution"] = _bank.Solution;

        UrlBaseService = "http://127.0.0.1:9002/BankService.ashx";
        //UrlBaseService = "https://localhost:44373/BankService.ashx";
        //UrlBaseService = "http://149.28.130.246:9002/BankService.ashx";

        if (_bank.BankCode == "SEAB")
        {
            pnGroupLogin2.Visible = true;
            pnGroupLogin.Visible = true;
        }
        else
        {
            pnGroupLogin2.Visible = false;
            pnGroupLogin.Visible = true;
        }

        lblBalanceWeb.Text = _bank.BalanceTotal.ToString("N0");
        if (_bank.StatusExtra == 1)
        {
            loginStatus.Attributes["style"] = "border-color: green";
            btnReLogin.Text = "Đã đăng nhập";
            btnReLogin.Enabled = false;
            pnLogin.Visible = true;

            //lblBalanceBank.Text = GetBalanceBank(drpBankCode.SelectedValue, txtBankId.Text).ToString("N0");
        }
        else
        {
            loginStatus.Attributes["style"] = "border-color: red";
            btnReLogin.Text = "Đăng nhập lại";
            btnReLogin.Enabled = true;
            pnLogin.Visible = true;

            lblBalanceBank.Text = "-1";
        }
        //if (_bank.BankCode == "SEAB" || _bank.BankCode == "ACB" || _bank.BankCode == "VPB")
        //{
        //    var bankCode = _bank.BankCode;
        //    var bankId = _bank.BankId;

        //    //lblId.Text = AppUtils.Request("id").ToString();
        //    //lblBankInfo.Text = "Upload - Tài khoản " + bankCode + " : " + bankId;
        //    bankCode = Regex.Replace(bankCode, @"[^a-zA-Z0-9]", "");
        //    bankId = Regex.Replace(bankId, @"[^a-zA-Z0-9]", "");

        //    string UploadFolderPhysical = Path.Combine(@"Z:\24HPAY", bankCode, bankId);

        //    try
        //    {
        //        if (!Directory.Exists(UploadFolderPhysical))
        //        {
        //            Directory.CreateDirectory(UploadFolderPhysical);
        //        }
        //    }
        //    catch
        //    {

        //    }
           
        //}

        if (_bank.BankCode != "VPB" && _bank.BankCode != "ACB" && _bank.BankCode != "SEAB")
        {
            btnCashForm4.Visible = false;
        }
        //ddrBankCode
        var lstbankcode = new BankCodeTranfer().GetListCache();
        // var bankList = BankList.GetBankList();
        drpBankCodeForm4.DataSource = lstbankcode;
        drpBankCodeForm4.DataTextField = "shortName";
        drpBankCodeForm4.DataValueField = "code";
        drpBankCodeForm4.DataBind();
        drpBankCodeForm4.Items.Insert(0, new ListItem("Chọn Bank:", ""));
        btnReLogin2.Text = "Đăng ký ";
        btnReLogin2.Enabled = true;
        pnLogin2.Visible = true;
        if (_bank.BankCode == "VPB")
        {
            DVOTP.Visible = true;
            var key = string.Format("OTP:{0}", _bank.AppDeviceId);

            var resultOTP = OTPDataCaching.GetCache<OTPRequest>(key);
            //NLogLogger.Info("OTP " + serializer.Serialize(resultOTP));
            if (resultOTP != null)

                lbOTP.Text = resultOTP.otp;
        }

        
    }
    public class OTPRequest
    {
        public string pin { get; set; }
        public string state { get; set; }
        public string otp { get; set; }
        public int time { get; set; }
        public string extra { get; set; }
        public string timeCreate { get; set; }

    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");
        var _bank = new BankAccounts();
        _bank.Id = Convert.ToInt32(AppUtils.Request("id"));
        _bank = _bank.Get();

        _bank.BankCode = drpBankCode.SelectedValue;
        _bank.BankId = txtBankId.Text;
        _bank.BankName = txtBankName.Text.Trim();
        _bank.BankAccount = txtBankAccount.Text.Trim();
        _bank.Status = Convert.ToInt32(chkIsActive.Checked);
        _bank.BalanceMaxDay = Convert.ToInt32(txtBalanceMaxDay.Text);
        _bank.BalanceMaxMonth = Convert.ToInt32(txtBalanceMaxMonth.Text);
        _bank.Type = drpType.SelectedValue;
        _bank.BankPass = rijndaelKey.Encrypt(txtBankPass.Text.Trim());
        _bank.Solution = drpSolution.SelectedValue;
        _bank.Computer = txtComputer.Text;
        _bank.PhoneDevice = txtPhone.Text;
        _bank.PinOtp = txtPinOtp.Text;
        _bank.BankType = drpBankType.SelectedValue;
        _bank.AppDeviceId = txtAppDeviceId.Text;
        _bank.CloudPhoneId = txtCloudPhoneId.Text;
        var lstProfileImage = new List<BankProfileImage>();
       
        _bank.Update();

        _bank.DeleteCache();
        //Log User
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "bankupdate",
            ActionName = "Cập nhật bank",
            Description = "Cập nhật bank " + _bank.BankCode + " |" + _bank.BankId
        };
        _userLog.Add();
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccount);
    }
    protected string ConvertBase64(Stream Path)
    {
        using (System.Drawing.Image image = System.Drawing.Image.FromStream(Path))
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
               // return ImgResize2(base64String);
            }
        }
    }
    public static string ImgResize2(string base64Image)
    {
        try
        {
            Random random = new Random();

            // Convert Base64 to Image
            byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);
            System.Drawing.Image image;
            using (MemoryStream ms = new MemoryStream(imageBytesFromBase64))
            {
                image = System.Drawing.Image.FromStream(ms);
            }
            if (image.Height > 960)
            {
                var w = (int)(image.Width * 960 / image.Height);
                image = (System.Drawing.Image)(new Bitmap(image, new System.Drawing.Size(w, 960)));

            }

            // Convert Image back to Base64
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                byte[] imageBytes = ms.ToArray();
                string base64ImageRes = Convert.ToBase64String(imageBytes);
                return base64ImageRes;
            }
        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "ImageHelper", "AddLine", "Base64 Invalid", base64Image });
        }

        return String.Empty;
    }
    public static string ImgResize(string base64Image)
    {
        try
        {
            Random random = new Random();

            // Convert Base64 to Image
            byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);
            System.Drawing.Image image;
            using (MemoryStream ms = new MemoryStream(imageBytesFromBase64))
            {
                image = System.Drawing.Image.FromStream(ms);
            }
            var w = (int)(image.Width * 640 / image.Height);
            image = (System.Drawing.Image)(new Bitmap(image, new System.Drawing.Size(w, 640)));

            // Convert Image back to Base64
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                byte[] imageBytes = ms.ToArray();
                string base64ImageRes = Convert.ToBase64String(imageBytes);
                return base64ImageRes;
            }
        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "ImageHelper", "AddLine", "Base64 Invalid", base64Image });
        }

        return String.Empty;
    }
    protected void bntSyncBalance_Click(object sender, EventArgs e)
    {
        btnSyncBalance.Enabled = false;
        hidAccountTAB.Value = "#account-info";
        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form Sync Click" });

        var balance = GetBalanceBank(drpBankCode.SelectedValue, txtBankId.Text);
        if (balance >= 0)
        {
            var _bank = new BankAccounts();
            var resUpdate = _bank.UpdateBalance(Convert.ToInt32(AppUtils.Request("id")), balance);
            if (resUpdate > 0)
            {
                divResultSync.Attributes["class"] = "callout callout-success";
                syncMessage.InnerHtml = "<i>Bạn đã thực hiện lệnh đồng bộ thành công.</i>";
                lblBalanceWeb.Text = lblBalanceBank.Text = balance.ToString("N0");
            }
            else
            {
                divResultSync.Attributes["class"] = "callout callout-danger";
                syncMessage.InnerText = "<i>Có lỗi trong quá trình thực hiện</i>";
            }
        }
        else
        {
            divResultSync.Attributes["class"] = "callout callout-danger";
            syncMessage.InnerText = "<i>Có lỗi trong quá trình thực hiện</i>";
        }
        btnSyncBalance.Enabled = true;
        divResultSync.Visible = true;
    }

    protected void bntOtpLogin2_Click(object sender, EventArgs e)
    {

        hidAccountTAB.Value = "#account-info";
        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form OtpLogin Click" });

        var login = OtpLogin2(drpBankCode.SelectedValue, txtBankId.Text, txtOtp2.Text.Trim());

        if (login.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            divResultSync.Attributes["class"] = "callout callout-success";
            syncMessage.InnerHtml = "<i>Bạn đã thực hiện đăng ký thành công.</i>";
            loginStatus.Attributes["style"] = "border-color: green";
            btnReLogin2.Text = "Đã đăng nhập";
            btnReLogin2.Enabled = false;

            pnLogin2.Visible = true;
            pnOtpLogin2.Visible = false;
        }

        else
        {
            divResultSync.Attributes["class"] = "callout callout-danger";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);
            loginStatus.Attributes["style"] = "border-color: red";
            btnReLogin.Text = "Đăng ký lại";
            btnReLogin.Enabled = false;

            pnLogin2.Visible = false;
            pnOtpLogin2.Visible = true;
        }



        divResultSync.Visible = true;
    }
    protected void bntReLogin2_Click(object sender, EventArgs e)
    {

        hidAccountTAB.Value = "#account-info";
        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form ReLogin Click" });

        var login = ReLogin2(drpBankCode.SelectedValue, txtBankId.Text);
        if (login.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            divResultSync.Attributes["class"] = "callout callout-success";
            syncMessage.InnerHtml = "<i>Bạn đã thực hiện đăng ký thành công. Vui lòng không ấn F5 hoặc Refresh trang</i>";
            loginStatus.Attributes["style"] = "border-color: green";
            btnReLogin2.Text = "Đã đăng ký";
            btnReLogin2.Enabled = false;

            pnLogin2.Visible = true;


            pnOtpLogin2.Visible = false;
        }
        else if (login.ResponseCode == (int)ResponseCode.OtpRequired)
        {
            divResultSync.Attributes["class"] = "callout callout-warning";
            loginStatus.Attributes["style"] = "border-color: green";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);

            pnLogin2.Visible = false;
            pnOtpLogin2.Visible = true;

            lblDescription2.Text = "Đăng ký thành công";
        }
        else
        {
            divResultSync.Attributes["class"] = "callout callout-danger";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);
            loginStatus.Attributes["style"] = "border-color: red";
            btnReLogin2.Text = "Đăng nhập lại";
            btnReLogin2.Enabled = true;

            pnLogin2.Visible = true;
            pnOtpLogin2.Visible = false;
        }
        divResultSync.Visible = true;
    }
    protected void bntReLogin_Click(object sender, EventArgs e)
    {

        hidAccountTAB.Value = "#account-info";
        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form ReLogin Click" });

        var login = ReLogin(drpBankCode.SelectedValue, txtBankId.Text);
        if (login.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            divResultSync.Attributes["class"] = "callout callout-success";
            syncMessage.InnerHtml = "<i>Bạn đã thực hiện đăng nhập thành công. Vui lòng không ấn F5 hoặc Refresh trang</i>";
            loginStatus.Attributes["style"] = "border-color: green";
            btnReLogin.Text = "Đã đăng nhập";
            btnReLogin.Enabled = false;

            pnLogin.Visible = true;
            pnOtpLogin.Visible = false;
        }
        else if (login.ResponseCode == (int)ResponseCode.OtpRequired)
        {
            divResultSync.Attributes["class"] = "callout callout-warning";
            loginStatus.Attributes["style"] = "border-color: green";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);

            pnLogin.Visible = false;
            pnOtpLogin.Visible = true;
        }
        else
        {
            divResultSync.Attributes["class"] = "callout callout-danger";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);
            loginStatus.Attributes["style"] = "border-color: red";
            btnReLogin.Text = "Đăng nhập lại";
            btnReLogin.Enabled = true;

            pnLogin.Visible = true;
            pnOtpLogin.Visible = false;
        }
        divResultSync.Visible = true;
    }

    protected void bntOtpLogin_Click(object sender, EventArgs e)
    {

        hidAccountTAB.Value = "#account-info";
        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form OtpLogin Click" });

        var login = OtpLogin(drpBankCode.SelectedValue, txtBankId.Text, txtOtp.Text.Trim());

        if (login.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            divResultSync.Attributes["class"] = "callout callout-success";
            syncMessage.InnerHtml = "<i>Bạn đã thực hiện đăng nhập thành công.</i>";
            loginStatus.Attributes["style"] = "border-color: green";
            btnReLogin.Text = "Đã đăng nhập";
            btnReLogin.Enabled = false;

            pnLogin.Visible = true;
            pnOtpLogin.Visible = false;

            if (drpBankCode.SelectedValue == "SEAB")
            {
                pnGroupLogin2.Visible = true;
            }
        }

        else
        {
            divResultSync.Attributes["class"] = "callout callout-danger";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);
            loginStatus.Attributes["style"] = "border-color: red";
            btnReLogin.Text = "Đăng nhập lại";
            btnReLogin.Enabled = false;

            pnLogin.Visible = false;
            pnOtpLogin.Visible = true;
        }



        divResultSync.Visible = true;
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccount);
    }
    protected void btnCashForm3_Click(object sender, EventArgs e)
    {
        hidAccountTAB.Value = "#balance-info";
        hidBalanceTAB.Value = "#detail-form-3";
    }
    protected void btnCashForm4_Click(object sender, EventArgs e)
    {
        //hidAccountTAB.Value = "#balance-info";
        //hidBalanceTAB.Value = "#detail-form-4";

        //calloutForm4.Attributes["class"] = "callout callout-danger";
        //resTextForm4.InnerHtml = "<i>Tính năng này chưa hữu hiệu!</i>";
        //calloutForm4.Visible = true;
        //return;

        btnCashForm4.Enabled = false;

        hidAccountTAB.Value = "#balance-info";
        hidBalanceTAB.Value = "#detail-form-4";

        var requesData = new RequestData()
        {
            PartnerCode = string.Empty,
            CommandCode = "TRANSFER",
            RequestContent = serializer.Serialize(new Transfer()
            {
                TransId = "",
                PartnerBankCode = drpBankCode.SelectedValue,
                PartnerBankId = txtBankId.Text,
                BankCode = drpBankCodeForm4.SelectedValue,
                BankId = txtBankIdForm4.Text,
                BankName = txtBankNameForm4.Text,
                Amount = Convert.ToInt32(txtAmountForm4.Text),
                Comment = txtNoteForm4.Text
            }),
        };

        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form1 Click" });
        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;

        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {

            if (resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
            {
                calloutForm4.Attributes["class"] = "callout callout-warning";
                resTextForm4.InnerHtml = "<i>Hệ thống đã nhận lệnh thành công. Bạn vui lòng xem chi tiết kết quả tại Bank Log</i>";

                var _userLog = new UserLog
                {
                    UserName = AppUtils.UserName,
                    Action = "banktranfer",
                    ActionName = "Chuyển tiền bank",
                    Description = "Chuyển tiền bank " + drpBankCode.SelectedValue + " |" + txtBankId.Text + " |" + txtAmountForm4.Text + " |" + drpBankCodeForm4.SelectedValue + " |" + txtBankIdForm4.Text
                };

                TelegramClient.SendTeleV2("-1003758147588", "Chuyển tiền bank " + drpBankCode.SelectedValue + " | " + txtBankId.Text + " | " + txtAmountForm4.Text + " |" + drpBankCodeForm4.SelectedValue + " | " + txtBankIdForm4.Text +" từ user " + AppUtils.UserName );
                _userLog.Add();
            }

            else
            {
                calloutForm4.Attributes["class"] = "callout callout-danger";
                resTextForm4.InnerHtml = string.Format("<i>Có lỗi trong quá trình thực hiện! ({0})</i>", resObj.Description);
            }

        }
        else
        {
            calloutForm4.Attributes["class"] = "callout callout-danger";
            resTextForm4.InnerHtml = "<i>Có lỗi trong quá trình thực hiện!</i>";
        }

        btnCashForm4.Enabled = true;
        calloutForm4.Visible = true;
    }

    public static async Task<string> CallbackJson(string url, string postData)
    {
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(300);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Response", responseContent });
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Response Is Null" });
            }

        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }

    private int GetBalanceBank(string bankCode, string bankId)
    {
        var requesData = new RequestData()
        {
            CommandCode = "BALANCE_GET",
            RequestContent = string.Format("{0},{1}", bankCode, bankId)
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null && resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            return Convert.ToInt32(resObj.ResponseContent);
        }
        return -1;
    }

    private APIResponse ReLogin(string bankCode, string bankId)
    {
        var requesData = new RequestData()
        {
            CommandCode = "RELOGIN",
            RequestContent = string.Format("{0},{1}", bankCode, bankId)
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            return resObj;
        }
        return new APIResponse((int)ResponseCode.TransactionFailed);
    }

    private APIResponse OtpLogin(string bankCode, string bankId, string otp)
    {
        var requesData = new RequestData()
        {
            CommandCode = "OTPLOGIN",
            RequestContent = string.Format("{0},{1},{2}", bankCode, bankId, otp)
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            return resObj;
        }
        return new APIResponse((int)ResponseCode.TransactionFailed);
    }
    private APIResponse ReLogin2(string bankCode, string bankId)
    {
        var requesData = new RequestData()
        {
            CommandCode = "REG_SMART_OTP",
            RequestContent = string.Format("{0},{1}", bankCode, bankId)
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            return resObj;
        }
        return new APIResponse((int)ResponseCode.TransactionFailed);
    }
    private APIResponse OtpLogin2(string bankCode, string bankId, string otp)
    {
        var requesData = new RequestData()
        {
            CommandCode = "REG_SMART_OTP_CONFIRM",
            RequestContent = string.Format("{0},{1},{2}", bankCode, bankId, otp)
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            return resObj;
        }
        return new APIResponse((int)ResponseCode.TransactionFailed);
    }
    public class Transfer
    {
        public string PartnerBankCode { get; set; }
        public string PartnerBankId { get; set; }
        public string TransId { get; set; } //Transaction cua he thông Pay
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; } // 
        public int Amount { get; set; }
        public string Comment { get; set; }

    }

    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }
}
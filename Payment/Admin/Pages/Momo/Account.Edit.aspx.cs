using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using Telegram.Bot.Types;
using System.Web.Script.Serialization;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using DocumentFormat.OpenXml.Presentation;


public partial class Pages_Momo_Account_Edit : System.Web.UI.Page
{
    //private string urlBaseService = "https://localhost:44373/MomoService.ashx";
    //private string urlBaseService = "http://127.0.0.1:9001/MomoService.ashx";
    JavaScriptSerializer serializer = new JavaScriptSerializer();

    public string DesColor;
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

    protected void Page_Load(object sender, EventArgs e)
    {

        btnCashForm1.OnClientClick = "validate([" + txtAmountForm1.ClientID + "]," + btnCashForm1.ClientID + ")";
        btnCashForm2.OnClientClick = "validate([" + txtBankNumber.ClientID + "," + txtAmountForm2.ClientID + "," + ddlBank.ClientID + "], " + btnCashForm2.ClientID + ")";
        btnCashForm4.OnClientClick = "validate([" + txtMomoIdForm4.ClientID + "," + txtMomoNameForm4.ClientID + "," + txtAmountForm4.ClientID + "]," + btnCashForm4.ClientID + ")";
        btAdd.OnClientClick = "validate(" + ")";
        btReset.OnClientClick = "validate(" + ")";
        btnSyncBalance.OnClientClick = "validate(" + ")";
        btnSenOtpSms.OnClientClick = "validate(" + ")";
        btnOtpLogin.OnClientClick = "validate(" + ")";
        btnOpen.OnClientClick = "validate(" + ")";
        AppUtils.CheckRoles(Resources.Url.MomoAccountEdit);
        serializer.MaxJsonLength = Int32.MaxValue;
        if (!IsPostBack)
        {
            init();
        }
    }
    private void init()
    {
        //Control

        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");
        var _Momo = new MomoAccounts();
        _Momo.Id = Convert.ToInt32(AppUtils.Request("id"));
        _Momo = _Momo.Get();
        //NLogLogger.Info(new string[] { "MomoImage", _Momo.ProfileImage });
        if (_Momo == null)
        {
            Response.Redirect(Resources.Url.MomoAccount);
        }
        txtMomoId.Text = _Momo.MomoId;
        txtMomoName.Text = _Momo.MomoName;
        txtMomoPass.Attributes["value"] = rijndaelKey.Decrypt(_Momo.MomoPass);
        drpStatus.SelectedValue = _Momo.Status.ToString();
        drp20M.SelectedValue = _Momo.ByPass20M.ToString();
        drpType.SelectedValue = _Momo.Type.ToString();
        drpSolution.SelectedValue = _Momo.Solution;
        txtBalanceMaxDay.Text = _Momo.BalanceMaxDay.ToString();
        txtBalanceMaxMonth.Text = _Momo.BalanceMaxMonth.ToString();
        lblAccount.Text = _Momo.Solution + " - Tài khoản " + _Momo.MomoId;

        ViewState["Solution"] = _Momo.Solution;

        var MoMoExt = new MomoAccounts().GetExt(_Momo.MomoId);
        if (MoMoExt != null)
        {
            lblDescription.Text = MoMoExt.Description;
            //if (MoMoExt.Description.StartsWith("Đã"))
            //{
            //    DesColor = "border-color: green";

            //}
            //else
            //{
            //    DesColor = "border-color: red";
            //}
            //NLogLogger.Info(new string[] { "DesColor", DesColor, MoMoExt.Description });
        }

        if (_Momo.Solution == "LD")
        {
            calloutwaring.Visible = true;
            UrlBaseService = "http://127.0.0.1:9001/MomoService.ashx";
        }
        else
        {
            calloutwaring.Visible = false;

            UrlBaseService = "http://127.0.0.1:9002/MomoService.ashx";
            //UrlBaseService = "https://localhost:44373/MomoService.ashx";
            //UrlBaseService = "http://149.28.130.246:9002/MomoService.ashx";

            if (_Momo.StatusExtra == (int)ResponseCode.TransactionSuccessful || _Momo.StatusExtra == -6 || _Momo.StatusExtra == -7 || _Momo.StatusExtra == -8 || _Momo.StatusExtra == -9)
            {
                loginStatus.Attributes["style"] = "border-color: green";
                btnSenOtpSms.Text = "Đã đăng nhập";
                btnSenOtpSms.Enabled = false;
                pnLogin.Visible = true;

                lblBalanceMomo.Text = GetBalanceMomo(txtMomoId.Text).ToString("N0");
                if (_Momo.StatusExtra == -6 || _Momo.StatusExtra == -7 || _Momo.StatusExtra == -8)
                {
                    loginStatus.Attributes["style"] = "border-color: red";
                }
            }
            else
            {
                loginStatus.Attributes["style"] = "border-color: red";
                btnSenOtpSms.Text = "Đăng nhập lại";
                btnSenOtpSms.Enabled = true;
                pnLogin.Visible = true;

                lblBalanceMomo.Text = "-1";
            }
            if (_Momo.StatusExtra == -9 || _Momo.StatusExtra == -7)
            {
                pnCap.Visible = true;
            }

        }

        lblBalanceWeb.Text = _Momo.BalanceTotal.ToString("N0");

        //imge
        var lstProfileImage = new List<MomoProfileImage>();

        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "1",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "2",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "3",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "4",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "5",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "6",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "7",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "8",
            Detection = ""
        });
        if (!string.IsNullOrEmpty(_Momo.ProfileImage))
        {
            try
            {
                lstProfileImage = serializer.Deserialize<List<MomoProfileImage>>(_Momo.ProfileImage);

            }
            catch
            {

            }
        }
        if (lstProfileImage[0].Base64.Contains("/cmspay"))
        {
            img1.ImageUrl = lstProfileImage[0].Base64;
        }
        else
        {
            img1.ImageUrl = "data:image/jpeg;base64," + ImgResize(lstProfileImage[0].Base64);
        }

        if (lstProfileImage[1].Base64.Contains("/cmspay"))
        {
            img2.ImageUrl = lstProfileImage[1].Base64;
        }
        else
        {
            img2.ImageUrl = "data:image/jpeg;base64," + ImgResize(lstProfileImage[1].Base64);
        }

        if (lstProfileImage[2].Base64.Contains("/cmspay"))
        {
            img3.ImageUrl = lstProfileImage[2].Base64;
        }
        else
        {
            img3.ImageUrl = "data:image/jpeg;base64," + ImgResize(lstProfileImage[2].Base64);
        }

        if (lstProfileImage[3].Base64.Contains("/cmspay"))
        {
            img4.ImageUrl = lstProfileImage[3].Base64;
        }
        else
        {
            img4.ImageUrl = "data:image/jpeg;base64," + ImgResize(lstProfileImage[3].Base64);
        }

        if (lstProfileImage[4].Base64.Contains("/cmspay"))
        {
            img5.ImageUrl = lstProfileImage[4].Base64;
        }
        else
        {
            img5.ImageUrl = "data:image/jpeg;base64," + ImgResize(lstProfileImage[4].Base64);
        }

        if (lstProfileImage[5].Base64.Contains("/cmspay"))
        {
            img6.ImageUrl = lstProfileImage[5].Base64;
        }
        else
        {
            img6.ImageUrl = "data:image/jpeg;base64," + ImgResize(lstProfileImage[5].Base64);
        }

        if (lstProfileImage[6].Base64.Contains("/cmspay"))
        {
            img7.ImageUrl = lstProfileImage[6].Base64;
        }
        else
        {
            img7.ImageUrl = "data:image/jpeg;base64," + ImgResize(lstProfileImage[6].Base64);
        }

        if (lstProfileImage[7].Base64.Contains("/cmspay"))
        {
            img8.ImageUrl = lstProfileImage[7].Base64;
        }
        else
        {
            img8.ImageUrl = "data:image/jpeg;base64," + ImgResize(lstProfileImage[7].Base64);
        }

    }

    protected void bntSendOTPMsg_Click(object sender, EventArgs e)
    {

        hidAccountTAB.Value = "#account-info";
        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form ReLogin Click" });

        var login = SendOTPMsg(txtMomoId.Text);
        if (login.ResponseCode == (int)ResponseCode.OtpRequired)
        {
            divResultSync.Attributes["class"] = "callout callout-warning";
            loginStatus.Attributes["style"] = "border-color: green";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);

            pnLogin.Visible = false;
            pnOtpLogin.Visible = true;
        }
        else if (login.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            divResultSync.Attributes["class"] = "callout callout-success";
            syncMessage.InnerHtml = "<i>Bạn đã thực hiện đăng nhập thành công. Vui lòng không ấn F5 hoặc Refresh trang</i>";
            loginStatus.Attributes["style"] = "border-color: green";
            btnSenOtpSms.Text = "Đã đăng nhập";
            btnSenOtpSms.Enabled = false;

            pnLogin.Visible = true;
            pnOtpLogin.Visible = false;
        }
        else
        {
            divResultSync.Attributes["class"] = "callout callout-danger";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);
            loginStatus.Attributes["style"] = "border-color: red";
            btnSenOtpSms.Text = "Đăng nhập lại";
            btnSenOtpSms.Enabled = true;

            pnLogin.Visible = true;
            pnOtpLogin.Visible = false;
        }
        divResultSync.Visible = true;
    }

    protected void bntOtpLogin_Click(object sender, EventArgs e)
    {

        hidAccountTAB.Value = "#account-info";
        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form ReLogin Click" });

        var login = OtpLogin(txtMomoId.Text, txtOtp.Text.Trim());
        if (login.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            divResultSync.Attributes["class"] = "callout callout-success";
            syncMessage.InnerHtml = "<i>Bạn đã thực hiện đăng nhập thành công. Vui lòng không ấn F5 hoặc Refresh trang</i>";
            loginStatus.Attributes["style"] = "border-color: green";
            btnSenOtpSms.Text = "Đã đăng nhập";
            btnSenOtpSms.Enabled = false;

            pnLogin.Visible = true;
            pnOtpLogin.Visible = false;
        }

        else
        {
            divResultSync.Attributes["class"] = "callout callout-danger";
            syncMessage.InnerHtml = string.Format("<i>{0}.</i>", login.Description);
            loginStatus.Attributes["style"] = "border-color: red";
            btnSenOtpSms.Text = "Đăng nhập lại";
            btnSenOtpSms.Enabled = true;

            pnLogin.Visible = false;
            pnOtpLogin.Visible = true;
        }

        divResultSync.Visible = true;
    }
    protected void bntOpen_Click(object sender, EventArgs e)
    {
        var _momo = new MomoAccounts();
        _momo.StopScanAt_Update(Convert.ToInt32(AppUtils.Request("id")), DateTime.Now.AddMinutes(10));
        _momo.UpdateExtra(txtMomoId.Text, 1, DateTime.Now, DateTime.Now, txtMomoName.Text, "Đăng nhập thành công");

        divResultSync.Attributes["class"] = "callout callout-success";
        syncMessage.InnerHtml = "<i>Bạn đã thực hiện thành công</i>";
        loginStatus.Attributes["style"] = "border-color: green";
        divResultSync.Visible = true;
    }
    private APIResponse SendOTPMsg(string momoId)
    {
        var requesData = new RequestData()
        {
            CommandCode = "SEND_OTPMSG",
            RequestContent = string.Format("{0}", momoId)
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            return resObj;
        }
        return new APIResponse((int)ResponseCode.TransactionFailed);
    }

    private APIResponse OtpLogin(string momoId, string otp)
    {
        var requesData = new RequestData()
        {
            CommandCode = "OTPLOGIN",
            RequestContent = string.Format("{0},{1}", momoId, otp)
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            return resObj;
        }
        return new APIResponse((int)ResponseCode.TransactionFailed);
    }
    protected void btReset_Click(object sender, EventArgs e)
    {
       
        var _Momo = new MomoAccounts();
        _Momo.Id = Convert.ToInt32(AppUtils.Request("id"));
        _Momo = _Momo.Get();
        _Momo.BalanceMaxDay = Convert.ToInt32(txtBalanceMaxDay.Text);
        _Momo.Reset();

        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount);

    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");
        var _Momo = new MomoAccounts();
        _Momo.Id = Convert.ToInt32(AppUtils.Request("id"));
        _Momo = _Momo.Get();
        _Momo.MomoId = txtMomoId.Text;
        _Momo.MomoName = txtMomoName.Text.Trim();
        _Momo.Status = Convert.ToInt32(drpStatus.Text);
        _Momo.ByPass20M = Convert.ToInt32(drp20M.Text);
        _Momo.BalanceMaxDay = Convert.ToInt32(txtBalanceMaxDay.Text);
        _Momo.BalanceMaxMonth = Convert.ToInt32(txtBalanceMaxMonth.Text);
        _Momo.Type = drpType.SelectedValue;
        _Momo.MomoPass = rijndaelKey.Encrypt(txtMomoPass.Text.Trim());
        _Momo.Solution = drpSolution.SelectedValue;


        var lstProfileImage = new List<MomoProfileImage>();
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "1",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "2",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "3",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "4",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "5",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "6",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "7",
            Detection = ""
        });
        lstProfileImage.Add(new MomoProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = "8",
            Detection = ""
        });
        if (!string.IsNullOrEmpty(_Momo.ProfileImage))
        {
            try
            {
                lstProfileImage = serializer.Deserialize<List<MomoProfileImage>>(_Momo.ProfileImage);

            }
            catch
            {
            }
        }


        if (fileUpload1.HasFile)
        {
            if (fileUpload1.FileName.Contains(".jpg") || fileUpload1.FileName.Contains(".png") || fileUpload1.FileName.Contains(".jpeg"))
            {

                lstProfileImage[0].ImgName = "1";


                lstProfileImage[0].Base64 = ConvertBase64(fileUpload1.FileContent);
            }

        }
        else
        {
            if (hdimg1.Value == "0")
            {
                lstProfileImage[0].Base64 = "/cmspay/content/noimage.png";
                lstProfileImage[0].Detection = "";
            }


        }
        if (fileUpload2.HasFile)

        {
            if (fileUpload2.FileName.Contains(".jpg") || fileUpload2.FileName.Contains(".png") || fileUpload2.FileName.Contains(".jpeg"))
            {

                lstProfileImage[1].ImgName = "2";

                lstProfileImage[1].Base64 = ConvertBase64(fileUpload2.FileContent);
            }

        }
        else
        {
            if (hdimg2.Value == "0")
            {
                lstProfileImage[1].Detection = "";
                lstProfileImage[1].Base64 = "/cmspay/content/noimage.png";
            }


        }
        if (fileUpload3.HasFile)
        {
            if (fileUpload3.FileName.Contains(".jpg") || fileUpload3.FileName.Contains(".png") || fileUpload3.FileName.Contains(".jpeg"))
            {

                lstProfileImage[2].ImgName = "3";

                lstProfileImage[2].Base64 = ConvertBase64(fileUpload3.FileContent);

            }

        }
        else
        {
            if (hdimg3.Value == "0")
            {
                lstProfileImage[2].Detection = "";
                lstProfileImage[2].Base64 = "/cmspay/content/noimage.png";
            }


        }
        if (fileUpload4.HasFile)
        {
            if (fileUpload4.FileName.Contains(".jpg") || fileUpload4.FileName.Contains(".png") || fileUpload4.FileName.Contains(".jpeg"))
            {

                lstProfileImage[3].ImgName = "4";

                lstProfileImage[3].Base64 = ConvertBase64(fileUpload4.FileContent);
            }

        }
        else
        {
            if (hdimg4.Value == "0")
            {
                lstProfileImage[3].Base64 = "/cmspay/content/noimage.png";
                lstProfileImage[3].Detection = "";
            }

        }

        if (fileUpload5.HasFile)
        {
            if (fileUpload5.FileName.Contains(".jpg") || fileUpload5.FileName.Contains(".png") || fileUpload5.FileName.Contains(".jpeg"))
            {

                lstProfileImage[4].ImgName = "5";


                lstProfileImage[4].Base64 = ConvertBase64(fileUpload5.FileContent);
            }

        }
        else
        {
            if (hdimg5.Value == "0")
            {
                lstProfileImage[4].Base64 = "/cmspay/content/noimage.png";
                lstProfileImage[4].Detection = "";
            }

        }
        if (fileUpload6.HasFile)

        {
            if (fileUpload6.FileName.Contains(".jpg") || fileUpload6.FileName.Contains(".png") || fileUpload6.FileName.Contains(".jpeg"))
            {

                lstProfileImage[5].ImgName = "6";

                lstProfileImage[5].Base64 = ConvertBase64(fileUpload6.FileContent);
            }

        }
        else
        {
            if (hdimg6.Value == "0")
            {
                lstProfileImage[5].Base64 = "/cmspay/content/noimage.png";
                lstProfileImage[5].Detection = "";

            }

        }
        if (fileUpload7.HasFile)
        {
            if (fileUpload7.FileName.Contains(".jpg") || fileUpload7.FileName.Contains(".png") || fileUpload7.FileName.Contains(".jpeg"))
            {

                lstProfileImage[6].ImgName = "7";

                lstProfileImage[6].Base64 = ConvertBase64(fileUpload7.FileContent);

            }

        }
        else
        {
            if (hdimg7.Value == "0")
            {


                lstProfileImage[6].Base64 = "/cmspay/content/noimage.png";
                lstProfileImage[6].Detection = "";
            }

        }
        if (fileUpload8.HasFile)
        {
            if (fileUpload8.FileName.Contains(".jpg") || fileUpload8.FileName.Contains(".png") || fileUpload8.FileName.Contains(".jpeg"))
            {

                lstProfileImage[7].ImgName = "8";

                lstProfileImage[7].Base64 = ConvertBase64(fileUpload8.FileContent);
            }

        }
        else
        {
            if (hdimg8.Value == "0")
            {
                lstProfileImage[7].Base64 = "/cmspay/content/noimage.png";

                lstProfileImage[7].Detection = "";
            }
        }
        //_Momo.Detection = "";
        _Momo.ProfileImage = serializer.Serialize(lstProfileImage);
        if (chkIsActive.Checked)
        {
            _Momo.StatusDetection = 0;
        }
        _Momo.Update();
        NotifyMomo(_Momo.MomoId, _Momo.Status);
        //Log User
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "momoupdate",
            ActionName = "Cập nhật momo",
            Description = "Cập nhật momo " + _Momo.MomoId
        };
        _userLog.Add();
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount);
    }
    public void NotifyMomo(string momoid, int status)
    {
        var obj = new { momoId = momoid, action = "start" };
        if (status != 1)
            obj = new { momoId = momoid, action = "stop" };
        var requesData = new RequestData()
        {
            CommandCode = "NOTIFY_ACTION",
            RequestContent = serializer.Serialize(obj),
        };
        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
    }
    protected void bntSyncBalance_Click(object sender, EventArgs e)
    {
        var requesData = new RequestData()
        {
            CommandCode = "BALANCE_SYNC",
            RequestContent = txtMomoId.Text,
        };

        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form Sync Click" });
        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;

        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            if (resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
            {
                divResultSync.Attributes["class"] = "callout callout-success";
                if (Solution == "LD")
                {
                    syncMessage.InnerHtml = "<i>Bạn đã thực hiện lệnh đồng bộ thành công (Client sẽ thực hiện lệnh và đồng bộ sau ít phút).</i>";
                }
                if (Solution.Contains("API"))
                {
                    syncMessage.InnerHtml = "<i>Bạn đã thực hiện lệnh đồng bộ thành công.</i>";
                }

            }
            else
            {
                divResultSync.Attributes["class"] = "callout callout-danger";
                syncMessage.InnerText = "Có lỗi trong quá trình thực hiện";
            }
        }
        else
        {
            divResultSync.Attributes["class"] = "callout callout-danger";
            syncMessage.InnerText = "Có lỗi trong quá trình thực hiện";
        }
        divResultSync.Visible = true;
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount);
    }

    protected void btStartClient_Click(object sender, EventArgs e)
    {
        hidAccountTAB.Value = "#balance-info";
        hidBalanceTAB.Value = "#detail-form-1";

        var requesData = new RequestData()
        {
            PartnerCode = string.Empty,
            CommandCode = "START_CASHBANK",
            RequestContent = txtMomoId.Text,
        };

        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form1 Click" });
        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;

        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            if (resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
            {
                calloutform1.Attributes["class"] = "callout callout-warning";
                resTextForm1.InnerText = "Lệnh khở động đã được gởi xuống Client (Quan sát bằng mắt hoặc đợi ít nhất 5 phút mới thao tác các lệnh tiếp theo)!";
            }
            else
            {
                calloutform1.Attributes["class"] = "callout callout-danger";
                resTextForm1.InnerText = "Có lỗi trong quá trình thực hiện";
            }
        }

        calloutform1.Visible = true;

    }
    protected void btnCashForm1_Click(object sender, EventArgs e)
    {
        btnCashForm1.Enabled = false;

        hidAccountTAB.Value = "#balance-info";
        hidBalanceTAB.Value = "#detail-form-1";

        var requesData = new RequestData()
        {
            PartnerCode = string.Empty,
            CommandCode = "CASHBANK",
            RequestContent = serializer.Serialize(new CashBank() { MomoId = txtMomoId.Text, Amount = Convert.ToInt32(txtAmountForm1.Text) }),
        };

        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form1 Click" });
        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;

        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            var resObjectContent = serializer.Deserialize<CashBank>(resObj.ResponseContent);

            if (resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
            {
                calloutform1.Attributes["class"] = "callout callout-warning";
                resTextForm1.InnerHtml = "<i>Hệ thống đã nhận lệnh thành công. Bạn vui lòng xem chi tiết kết quả tại Momo Log</i>";
                var _userLog = new UserLog
                {
                    UserName = AppUtils.UserName,
                    Action = "momocash",
                    ActionName = "Rút tiền momo",
                    Description = "Rút tiền momo " + txtMomoId.Text + " | " + txtAmountForm1.Text
                };
                _userLog.Add();
            }
            else
            {
                calloutform1.Attributes["class"] = "callout callout-danger";
                resTextForm1.InnerHtml = string.Format("<i>Có lỗi ! ({0})</i>", resObj.Description);
            }

        }
        else
        {
            calloutform1.Attributes["class"] = "callout callout-danger";
            resTextForm1.InnerHtml = "<i>Có lỗi trong quá trình thực hiện!</i>";
        }
        btnCashForm1.Enabled = true;
        calloutform1.Visible = true;
    }

    protected void btnCashForm2_Click(object sender, EventArgs e)
    {
        hidAccountTAB.Value = "#balance-info";
        hidBalanceTAB.Value = "#detail-form-2";

        var requesData = new RequestData()
        {
            PartnerCode = string.Empty,
            CommandCode = "CASHBANK",
            RequestContent = serializer.Serialize(new CashBank()
            {
                MomoId = txtMomoId.Text,
                BankCode = ddlBank.SelectedValue,
                BankNumber = txtBankNumber.Text,
                BankHolderName = txtBankHolderName.Text,
                Amount = Convert.ToInt32(txtAmountForm2.Text),
                TransferType = rbTransferType.SelectedValue
            }),
        };

        btnCashForm2.Enabled = false;

        var rqJson = serializer.Serialize(requesData);
        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form2 Click", rqJson });
        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;

        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            //var resObjectContent = serializer.Deserialize<CashBank>(resObj.ResponseContent);
            if (resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
            {
                calloutform2.Attributes["class"] = "callout callout-warning";
                resTextForm2.InnerHtml = "<i>Hệ thống đã nhận lệnh thành công. Bạn vui lòng xem chi tiết kết quả tại Momo Log</i>";
                var _userLog = new UserLog
                {
                    UserName = AppUtils.UserName,
                    Action = "momocash",
                    ActionName = "Rút tiền momo",
                    Description = "Rút tiền momo " + txtMomoId.Text + " | " + txtAmountForm2.Text + " | " + ddlBank.SelectedValue + " | " + txtBankNumber.Text
                };
                _userLog.Add();
            }
            else
            {
                calloutform2.Attributes["class"] = "callout callout-danger";
                resTextForm2.InnerHtml = string.Format("<i>Có lỗi ! ({0})</i>", resObj.Description);
            }
        }
        else
        {
            calloutform2.Attributes["class"] = "callout callout-danger";
            resTextForm2.InnerHtml = "<i>Có lỗi trong quá trình thực hiện!</i>";
        }

        btnCashForm2.Enabled = true;
        calloutform2.Visible = true;
    }
    protected void btnCashForm3_Click(object sender, EventArgs e)
    {
        hidAccountTAB.Value = "#balance-info";
        hidBalanceTAB.Value = "#detail-form-3";

    }
    protected void btnCashForm4_Click(object sender, EventArgs e)
    {
        btnCashForm4.Enabled = false;

        hidAccountTAB.Value = "#balance-info";
        hidBalanceTAB.Value = "#detail-form-4";

        var requesData = new RequestData()
        {
            PartnerCode = string.Empty,
            CommandCode = "TRANSFER",
            RequestContent = serializer.Serialize(new Transfer() { TransId = "", PartnerMomoId = txtMomoId.Text, MomoId = txtMomoIdForm4.Text, MomoName = txtMomoNameForm4.Text, Amount = Convert.ToInt32(txtAmountForm4.Text), Note = txtNoteForm4.Text }),
        };

        NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Form1 Click" });
        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;

        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {

            if (resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
            {
                calloutForm4.Attributes["class"] = "callout callout-warning";
                resTextForm4.InnerHtml = "<i>Hệ thống đã nhận lệnh thành công. Bạn vui lòng xem chi tiết kết quả tại Momo Log</i>";
                var _userLog = new UserLog
                {
                    UserName = AppUtils.UserName,
                    Action = "momotranfer",
                    ActionName = "Chuyển tiền momo",
                    Description = "Chuyển tiền momo " + txtMomoId.Text + " | " + txtAmountForm4.Text + " | " + txtMomoIdForm4.Text
                };
                _userLog.Add();
            }

            else
            {
                calloutForm4.Attributes["class"] = "callout callout-danger";

                if (Solution == "LD")
                {
                    resTextForm4.InnerHtml = "<i>Có lỗi trong quá trình thực hiện! (Có thể do timeout hiển thị kết quả, bạn cần xem chi tiết tại log giao dịch với loại CASH)</i>";
                }

                if (Solution == "API")
                {
                    resTextForm4.InnerHtml = string.Format("<i>Có lỗi trong quá trình thực hiện! ({0})</i>", resObj.Description);
                }
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
        client.Timeout = TimeSpan.FromSeconds(30);
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

    private int GetBalanceMomo(string momoId)
    {
        var requesData = new RequestData()
        {
            CommandCode = "BALANCE_GET",
            RequestContent = momoId
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null && resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            return Convert.ToInt32(resObj.ResponseContent);
        }
        else
        {
            blockBalance.Attributes["class"] = "border-color: red";
            return -1;
        }

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
            }
        }
    }
    public static string ImgResize(string base64Image)
    {
        try
        {
            return base64Image;
            //Random random = new Random();

            //// Convert Base64 to Image
            //byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);
            //System.Drawing.Image image;
            //using (MemoryStream ms = new MemoryStream(imageBytesFromBase64))
            //{
            //    image = System.Drawing.Image.FromStream(ms);
            //}

            //image = (System.Drawing.Image)(new Bitmap(image, new System.Drawing.Size(450, 600)));

            //// Convert Image back to Base64
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    image.Save(ms, ImageFormat.Jpeg);
            //    byte[] imageBytes = ms.ToArray();
            //    string base64ImageRes = Convert.ToBase64String(imageBytes);
            //    return base64ImageRes;
            //}
        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "ImageHelper", "AddLine", "Base64 Invalid", base64Image });
        }

        return String.Empty;
    }
    public class CashBank
    {
        public string TransId { get; set; } //Transaction cua he thông Pay
        public string MomoTransId { get; set; }
        public string MomoId { get; set; } // NG nhan
        public string BankCode { get; set; }
        public string BankHolderName { get; set; }
        public string BankNumber { get; set; }
        public int Amount { get; set; }
        public string TransferType { get; set; }
        public DateTime? TimeMomoSuccess { get; set; }
    }

    public class Transfer
    {
        public string TransId { get; set; } //Transaction cua he thông Pay
        public string PartnerMomoId { get; set; }
        public string MomoId { get; set; } // NG nhan
        public string MomoName { get; set; }
        public int Amount { get; set; }
        public string Note { get; set; }

    }

    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }

}
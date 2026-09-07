using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.Utils;

public partial class Pages_Security_Roles_Edit : System.Web.UI.Page
{

    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupEdit);
        if (!IsPostBack)
        {
            init();
        }
    }
    private void loadRoles()
    {


    }
    private void init()
    {
        var transactionId = AppUtils.Request("id");
        var topupOrder = new TopupMobileLog();
        topupOrder.TransactionID = transactionId;
        var topup = topupOrder.Get();

        txtTelco.SelectedValue = topup.Telco;
        txtMobile.Text = topup.Mobile;
        txtFullName.Text = topup.FullName;
        txtAmount.Text = topup.Amount.ToString();
        txtStatus.SelectedValue = topup.Status.ToString();
        txtTopupType.SelectedValue = topup.TopupType.ToString();
        //txtFirtAmout.SelectedValue = topup.AmountMin.ToString();
        txtPriority.SelectedValue = topup.Priority.ToString();
        txtOrderNo.Text = topup.OrderNo;
        txtConfirm.SelectedValue = topup.IsConfirm.ToString();
        txtAmountAll.SelectedValue = topup.AmountMinAll.ToString();
        txtAccountName.Text = topup.AccountName;
        txtPassword.Text = topup.Password;
        txtUssd.SelectedValue = topup.Ussd.ToString();

        if (topup.Telco == "vms")
        {
            phVMS.Visible = true;
            txtDeviceId.Text = Encrypts.MD5(DateTime.Now.ToString()).Substring(0, 0x10).ToLower();
        }

        if (topup.Telco == "zing" && (topup.TopupType == 16 || topup.TopupType == 17))
        {
            phGameM.Visible = true;
            var gameType = -1;
            switch (topup.TopupType)
            {
                case 16:
                    gameType = 7;
                    break;
                case 17:
                    gameType = 8;
                    break;

            }

            txtServer.DataSource = APIGame.ZingService.GetServerM(txtAccountName.Text, txtPassword.Text, gameType).OrderBy(x => x.serverID);
            txtServer.DataValueField = "serverID";
            txtServer.DataTextField = "serverName";
            txtServer.DataBind();
            txtServer.Items.Insert(0, new ListItem("Chọn server:", ""));

            if (!string.IsNullOrEmpty(topup.ExtData))
            {
                var extObject = serializer.Deserialize<Games.ZingGame>(topup.ExtData);
                if (extObject != null)
                {
                    txtServer.SelectedValue = extObject.serverID;
                    var roleSource = APIGame.ZingService.GetRoleM(txtAccountName.Text, txtPassword.Text, gameType, extObject.serverID);
                    txtRole.DataSource = roleSource;
                    txtRole.DataValueField = "roleID";
                    txtRole.DataTextField = "roleName";
                    txtRole.DataBind();
                    txtRole.SelectedValue = extObject.roleID;
                }
            }

        }

        if (!AppUtils.IsAdmin)
        {
            if (topup.AmountTopupSuccess != 0)
            {
                txtTelco.Enabled = false;
                txtMobile.Enabled = false;
                txtFullName.Enabled = false;
                txtAmount.Enabled = false;
                txtTopupType.Enabled = false;
                txtOrderNo.Enabled = false;
                txtAccountName.Enabled = false;
                txtPriority.SelectedValue = topup.Priority.ToString();
            }

            if (topup.IsConfirm == 1 || topup.Status == 0)
            {
                txtTelco.Enabled = false;
                txtMobile.Enabled = false;
                txtFullName.Enabled = false;
                txtAmount.Enabled = false;
                txtTopupType.Enabled = false;
                //txtFirtAmout.Enabled = false;
                txtOrderNo.Enabled = false;
                txtPriority.Enabled = false;
                txtAccountName.Enabled = false;
                txtPassword.Enabled = false;
                //txtFirtAmout.Enabled = false;
                txtAmountAll.Enabled = false;
                txtConfirm.Enabled = false;
                txtStatus.Enabled = false;
                txtOTP.Enabled = false;
                txtUssd.Enabled = false;
                //btnGetOTP.Attributes.CssStyle.Add("display", "none");
            }

            if (topup.Status == -4)
            {
                txtStatus.Enabled = false;
            }
            if (topup.OrderNo.StartsWith("RT_"))
            {
                txtConfirm.Enabled = false;
                if (!AppUtils.IsAdmin)
                {
                    txtPriority.Enabled = false;
                }
            }
            //if (txtStatus.SelectedValue == "0")
            //{
            //    txtStatus.Enabled = false;
            //}
        }

        if (AppUtils.IsAdmin)
        {
            txtPriority.Items.Insert(30, new ListItem("P0", "0"));
        }

    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {

        string Msg = "";
        bool validate = true;
        var extData = string.Empty;
        //if (!IsValidPhone(txtMobile.Text))
        //{
        //    validate = false;
        //    Msg = "Số điện thoại không đúng định dạng!";
        //}

        if (string.IsNullOrEmpty(txtTelco.SelectedValue) || string.IsNullOrEmpty(txtTopupType.SelectedValue) || string.IsNullOrEmpty(txtAmount.Text) ||
            string.IsNullOrEmpty(txtTopupType.SelectedValue) || string.IsNullOrEmpty(txtConfirm.Text)) //|| string.IsNullOrEmpty(txtFirtAmout.Text)
        {

            Msg = "Các trường có * là bắt buộc!";
            validate = false;
        }

        if (Convert.ToInt32(txtTopupType.SelectedValue) == 16 || Convert.ToInt32(txtTopupType.SelectedValue) == 17) //Game Mobile can them Role va Server
        {

            if (string.IsNullOrEmpty(txtServer.SelectedValue) || string.IsNullOrEmpty(txtRole.SelectedValue))
            {
                Msg = "Server và Role là bắt buộc!";
                validate = false;
            }

            //int[] listValue = { 20000, 50000, 100000, 500000, 1000000, 1500000, 5000000, 10000000 };
            //if (Convert.ToInt32(txtAmount.Text) <= 0 || Convert.ToInt32(txtAmount.Text) > 10000000)
            //{
            //    Msg = "Amount bắt buộc phải là số tiền phù hợp các gói cụ thể: 20.000, 50.000, 100.000, 500.000, 1.000.000, 1.500.000, 5.000.000, 10.000.000 đồng !";
            //    validate = false;
            //}

            //if (Convert.ToInt32(txtAmount.Text) <= 0 || Convert.ToInt32(txtAmount.Text) > 10000000)
            //{
            //    Msg = "Amount bắt buộc phải là số tiền phù hợp các gói cụ thể: 20.000, 50.000, 100.000, 500.000, 1.000.000, 1.500.000, 5.000.000, 10.000.000 đồng !";
            //    validate = false;
            //}

            var productID = string.Empty;

            //if (0 < Convert.ToInt32(txtAmount.Text) && Convert.ToInt32(txtAmount.Text) <= 20000)
            //    productID = "com.vng.jxm.item1";
            //else if (20000 < Convert.ToInt32(txtAmount.Text) && Convert.ToInt32(txtAmount.Text) <= 50000)
            //    productID = "com.vng.jxm.item2";
            //else if (50000 < Convert.ToInt32(txtAmount.Text) && Convert.ToInt32(txtAmount.Text) <= 100000)
            //    productID = "com.vng.jxm.item3";
            //else if (100000 < Convert.ToInt32(txtAmount.Text) && Convert.ToInt32(txtAmount.Text) <= 500000)
            //    productID = "com.vng.jxm.item4";
            //else if (500000 < Convert.ToInt32(txtAmount.Text) && Convert.ToInt32(txtAmount.Text) <= 1000000)
            //    productID = "com.vng.jxm.item5";
            //else if (1000000 < Convert.ToInt32(txtAmount.Text) && Convert.ToInt32(txtAmount.Text) <= 1500000)
            //    productID = "com.vng.jxm.item6";
            //else if (1500000 < Convert.ToInt32(txtAmount.Text) && Convert.ToInt32(txtAmount.Text) <= 5000000)
            //    productID = "com.vng.jxm.item14";
            //else if (5000000 < Convert.ToInt32(txtAmount.Text) && Convert.ToInt32(txtAmount.Text) <= 10000000)
            //    productID = "com.vng.jxm.item15";

            //switch (Convert.ToInt32(txtAmount.Text))
            //{
            //    case 20000:
            //        productID = "com.vng.jxm.item1";
            //        break;
            //    case 50000:
            //        productID = "com.vng.jxm.item2";
            //        break;
            //    case 100000:
            //        productID = "com.vng.jxm.item3";
            //        break;
            //    case 500000:
            //        productID = "com.vng.jxm.item4";
            //        break;
            //    case 1000000:
            //        productID = "com.vng.jxm.item5";
            //        break;
            //    case 1500000:
            //        productID = "com.vng.jxm.item6";
            //        break;
            //    case 5000000:
            //        productID = "com.vng.jxm.item14";
            //        break;
            //    case 10000000:
            //        productID = "com.vng.jxm.item15";
            //        break;
            //}

            switch (txtTopupType.SelectedIndex.ToString())
            {
                case "16":
                    productID = "com.vng.jxm.item15"; // Fix gói giữ Point
                    break;
                case "17":
                    //productID = "com.pp.dt3q.item8"; // Fix gói giữ Point
                    productID = "com.pp.dt3q.item116"; // Gói sự kiện đại tiệc 8
                    break;
            }


            extData = serializer.Serialize(new Games.ZingGame() { serverID = txtServer.SelectedValue, roleID = txtRole.SelectedValue, productID = productID, amount = txtAmount.Text, roleName = txtRole.SelectedItem.Text });
        }

        if (validate)
        {
            int userId = 0;
            if (!AppUtils.IsAdmin)
            {
                userId = AppUtils.UserID;
            }
            int ussdValue = 0;
            int.TryParse(txtUssd.SelectedValue, out ussdValue);



            var topupOrder = new TopupMobileLog()
            {
                TransactionID = AppUtils.Request("id"),
                Telco = txtTelco.SelectedValue,
                Mobile = txtMobile.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                Amount = Convert.ToInt32(txtAmount.Text),
                Status = Convert.ToInt32(txtStatus.SelectedValue),
                AmountMin = 0, //Convert.ToInt32(txtFirtAmout.SelectedValue),
                Priority = Convert.ToInt32(txtPriority.SelectedValue),
                TopupType = Convert.ToInt32(txtTopupType.SelectedValue),
                OrderNo = txtOrderNo.Text.Trim(),
                IsConfirm = Convert.ToInt32(txtConfirm.SelectedValue),
                AmountMinAll = Convert.ToInt32(txtAmountAll.SelectedValue),
                AccountName = txtAccountName.Text,
                Password = txtPassword.Text,
                UserId = userId,
                Ussd = ussdValue,
                ExtData = extData
            };
            var result = topupOrder.Update();
            if (result == -2)
            {
                Msg = "Đơn số: " + topupOrder.TransactionID + " không thể chốt vì còn tồn tại giao dịch nghi vấn (-2)</br>" +
                      string.Format("<a href = \"topup.monitor.aspx?r={0}&s=-2\">Click xem chi tiết</a>", topupOrder.TransactionID);
            }
            else
            {
                Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupList);
            }

        }

        AlertBans.Text = Msg;
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);

    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupList);
    }
    //protected void txtGoup_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    loadRoles();
    //}

    public bool IsValidPhone(string Phone)
    {
        try
        {
            if (string.IsNullOrEmpty(Phone))
                return false;
            var r = new Regex(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$");
            return (r.IsMatch(Phone) && Phone.Length > 5 && Phone.Length < 15);
        }
        catch (Exception) { return false; }
    }





    protected void txtServer_SelectedIndexChanged(object sender, EventArgs e)
    {
        var gameType = -1;
        switch (txtTopupType.SelectedIndex.ToString())
        {
            case "16":
                gameType = 7;
                break;
            case "17":
                gameType = 8; // Fix gói giữ Point
                break;
        }

        var roleSource = APIGame.ZingService.GetRoleM(txtAccountName.Text, txtPassword.Text, gameType, txtServer.SelectedValue.ToString());
        if (roleSource.Count == 0)
        {
            roleHelp.Text = "Không tìm thấy nhân vật trong server này!";
        }
        else
        {
            roleHelp.Text = "";
        }

        txtRole.DataSource = roleSource;
        txtRole.DataValueField = "roleID";
        txtRole.DataTextField = "roleName";
        txtRole.DataBind();
    }

}
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Libs.Report;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

public partial class Pages_Security_Roles_Edit_Order : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupEditOrder);
        if (!IsPostBack)
        {
            var orderNo = Request["o"];
            if (!string.IsNullOrEmpty(orderNo))
            {
                txtOrderNo.Text = orderNo.ToString();
                init();
            }
            else
            {
                Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupListOrder);
            }
        }
    }

    private void init()
    {
        txtOrderNo.Enabled = false;
        if (AppUtils.IsAdmin)
        {
            txtPriority.Items.Insert(30, new ListItem("P0", "0"));
        }
        if (txtOrderNo.Text.StartsWith("RT_"))
        {
            txtConfirm.Enabled = false;
            if (!AppUtils.IsAdmin)
            {
                txtPriority.Enabled = false;
            }
        }
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        string Msg = "";
        bool validate = true;

        //if (!IsValidPhone(txtMobile.Text))
        //{
        //    validate = false;
        //    Msg = "Số điện thoại không đúng định dạng!";
        //}
        var userId = Request["u"];
        var url = Request["ur"];
        var currentIsConfirm = Convert.ToInt32(Request["ic"]);
        if (string.IsNullOrEmpty(txtOrderNo.Text) && string.IsNullOrEmpty(userId))
        {
            Msg = "Các trường có * là bắt buộc!";
            validate = false;
        }

        if (validate)
        {
            var orderNo = txtOrderNo.Text;
            int? priority = null;
            int? status = null;
            int? confirm = null;


            if (!string.IsNullOrEmpty(txtPriority.SelectedValue))
                priority = Convert.ToInt32(txtPriority.SelectedValue);
            if (!string.IsNullOrEmpty(txtStatus.SelectedValue))
                status = Convert.ToInt32(txtStatus.SelectedValue);
            if (!string.IsNullOrEmpty(txtConfirm.SelectedValue))
                confirm = Convert.ToInt32(txtConfirm.SelectedValue);

            var topupOrder = new TopupMobileLog();
            var result = topupOrder.UpdateOrder(orderNo, priority, status, Convert.ToInt32(userId), confirm, currentIsConfirm);
            if (result == -2)
            {
                Msg = "Order: " + orderNo + " không thể chốt vì còn tồn tại giao dịch nghi vấn (-2), hoặc đang xử lý (2)</br>" +
                      string.Format("<a href = \"topup.monitor.aspx?o={0}&s=-2\">Click xem chi tiết</a>", orderNo);
            }
            else
            {
                Response.Redirect(url);
            }

        }

        AlertBans.Text = Msg;
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);

    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        var url = Request["ur"];
        Response.Redirect(url);
    }


}
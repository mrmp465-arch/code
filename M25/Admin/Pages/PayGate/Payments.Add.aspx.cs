using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_PayGate_Payments_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PaymentsAdd);
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        if (fileUploadClass.PostedFile == null || fileUploadClass.PostedFile.ContentLength == 0)
        { 
            AlertInfos.Text = "Bạn chưa chọn file thư viện";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }

        var _Payment = new Payments();
        _Payment.Name = txtName.Text.Trim();
        _Payment.ServiceCode = txtServiceCode.Text.Trim().ToLower();
        _Payment.Description = txtDescription.Text.Trim();
        _Payment.ClassName = txtClassName.Text.Trim();
        _Payment.Config = txtConfig.Text.Trim();
        _Payment.Status = Convert.ToInt32(cbxIsActive.Checked);

        _Payment.DataSize = fileUploadClass.PostedFile.ContentLength;
        _Payment.ClassData = new byte[_Payment.DataSize];
        fileUploadClass.PostedFile.InputStream.Read(_Payment.ClassData, 0, _Payment.DataSize);
        _Payment.Add();

        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PaymentsList);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PaymentsList);
    }
}
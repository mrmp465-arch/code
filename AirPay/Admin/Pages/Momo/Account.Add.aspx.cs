using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;

public partial class Pages_Momo_Account_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoAccountAdd);
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");

        var _Momo = new MomoAccounts();
        _Momo.MomoId = txtMomoId.Text;
        _Momo.MomoName = txtMomoName.Text.Trim();
        _Momo.Status = Convert.ToInt32(chkIsActive.Checked);
        _Momo.Type = drpType.SelectedValue;
        _Momo.MomoPass = rijndaelKey.Encrypt(txtMomoPass.Text.Trim());
        _Momo.BalanceMaxDay = Convert.ToInt32(txtBalanceMaxDay.Text);
        _Momo.BalanceMaxMonth = Convert.ToInt32(txtBalanceMaxMonth.Text);
        _Momo.Solution = drpSolution.SelectedValue;
        
        var result=_Momo.Add();
        if (result > 0)
        {
            //Log User
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "momoadd",
                ActionName = "Thêm mới momo",
                Description = "Thêm mới momo " + _Momo.MomoId
            };
            _userLog.Add();

            //phân bổ
            var _PartnerBank = new PartnerMomo();
            _PartnerBank.PartnerId = 1;
            _PartnerBank.MomoId = result;
            _PartnerBank.Status = 1;
            _PartnerBank.OrderNo = 1;
            _PartnerBank.Add();
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount);
        }
        else
        {
            AlertBans.Text = "Có lỗi trong quá trình xử lý";
            if (result == -319)
                AlertBans.Text = "Tài khoản đã tồn tại";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertBan').modal()}); ", true);
        }
       

    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount);
    }
}
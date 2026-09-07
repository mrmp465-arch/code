using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;

public partial class Pages_BankEWalletService_Bank_Account_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankAccountAdd);
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");

        var _bank = new BankAccounts();
        _bank.BankCode = drpBankCode.SelectedValue;
        _bank.BankAccount = txtBankAccount.Text;
        _bank.BankId = txtBankId.Text;
        _bank.BankName = txtBankName.Text.Trim();
        _bank.Status = Convert.ToInt32(chkIsActive.Checked);
        _bank.Type = drpType.SelectedValue;
        _bank.BankPass = rijndaelKey.Encrypt(txtBankPass.Text.Trim());
        _bank.BalanceMaxDay = Convert.ToInt32(txtBalanceMaxDay.Text);
        _bank.BalanceMaxMonth = Convert.ToInt32(txtBalanceMaxMonth.Text);
        _bank.Solution = drpSolution.SelectedValue;
        _bank.Computer = txtComputer.Text;
        _bank.PhoneDevice = txtPhone.Text;
        _bank.PinOtp = txtPinOtp.Text;
        _bank.AppDeviceId = txtAppDeviceId.Text;
        _bank.BankType = drpBankType.SelectedValue;
        var result = _bank.Add();
        if (result > 0)
        {
           
            //Log User
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "bankadd",
                ActionName = "Thêm mới bank",
                Description = "Thêm mới bank " + _bank.BankCode+" |"+ _bank.BankId
            };
            _userLog.Add();
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccount);
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
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccount);
    }
}
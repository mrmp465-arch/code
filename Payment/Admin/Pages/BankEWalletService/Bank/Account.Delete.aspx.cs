using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Pages_BankEWalletService_Bank_Account_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankAccountDelete);
        var _bank = new BankAccounts();
        _bank = _bank.Get(Convert.ToInt32(AppUtils.Request("id")));
       
        if (_bank != null)
        {
            _bank.Delete();
            _bank.DeleteCache();
            //Log User
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "bankdelete",
                ActionName = "Xóa bank",
                Description = "Xóa bank " + _bank.BankCode + " |" + _bank.BankId
            };
            _userLog.Add();
        }    
            
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccount );

    }
}
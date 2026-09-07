using Libs.API;
using System;
using System.Collections.Generic;
using System.IO;
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
            //Log User
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "bankdelete",
                ActionName = "Xóa bank",
                Description = "Xóa bank " + _bank.BankCode + " |" + _bank.BankId
            };
            _userLog.Add();
            try
            {
                string UploadFolderPhysical = Path.Combine(@"Z:\SDPAY", _bank.BankCode, _bank.BankId);
                DeleteAllFiles(UploadFolderPhysical);
            }
            catch
            {

            }
        }    
            
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccount );

    }
    public static void DeleteAllFiles(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return;

        try
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
        }
        catch (Exception ex)
        {
            // Log lỗi nếu cần
            // Logger.Error(ex);
        }
    }
}
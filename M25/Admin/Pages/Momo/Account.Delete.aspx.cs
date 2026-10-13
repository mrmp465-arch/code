using Libs.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Pages_Momo_Account_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoAccountDelete);
        var _Momo = new MomoAccounts();
        _Momo = _Momo.Get(Convert.ToInt32(AppUtils.Request("id")));
       
        if (_Momo != null)
        {
            _Momo.Delete();
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "momodelete",
                ActionName = "Xóa momo",
                Description = "Xóa momo " + _Momo.MomoId
            };
            _userLog.Add();
            try
            {
                string UploadFolderPhysical = Path.Combine(@"Z:\FASTPAY", "MOMO", _Momo.MomoId);
                DeleteAllFiles(UploadFolderPhysical);
            }
            catch
            {

            }
        }    
            
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount );

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
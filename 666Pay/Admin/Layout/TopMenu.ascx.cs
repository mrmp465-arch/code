using System; 
using System.Collections.Generic; 
using System.Web;
using System.Linq;
using Libs.API;
using SharedCache.WinServiceCommon.Hashing;
using Libs.Utils;

public partial class Layout_TopMenu : System.Web.UI.UserControl
{
    public List<MenuModel> lisUrl;
    public List<Roles> lstUrlResources;
    public string Url { get; set; }
    public string Lang { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        Lang= Libs.Utils.GlobalHelper.GetLanguage();
        lisUrl = new List<MenuModel>();
        var Ids = new List<int>();
        var list=  lstUrlResources = new Roles().GetList().OrderBy(x=>x.Group).ToList();
        var Group = 0;
        if (lstUrlResources != null)
        {
            //if (!AppUtils.IsAdmin) {
            if (AppUtils.UserID != 1) {
            var temp = new UsersRole().GetListByUser(AppUtils.UserID);
                if (temp != null&& temp.Count>0)
                {
                    Ids = temp.Select(x => x.RoleId).ToList();
                    list = list.Where(x => Ids.Contains(x.Id)).ToList();
                }
                else
                    list = new List<Roles>();
            } 
            foreach (var item in list)
            {
                if (Group != item.Group)
                {
                    Group = item.Group;  
                    lisUrl.Add(new MenuModel {
                                item = item,
                                listMenu = list.Where(x => x.Group == item.Group && x.IsMenu).ToList()
                                ,list= list.Where(x => x.Group == item.Group).ToList()
                    });
                }
            }
        }
      
      
         
        //ltrMenu.Text = Session["Menu"].ToString(); 
        Url = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
    }
    public string GetUrl2(List<Roles> Urls, string str)
    {
        foreach (var item in Urls)
        {
            if (Url.Contains(item.Url))
                return str;
        }
        return string.Empty;
    }
    public string GetUrl(List<Roles> Urls,string _Url,int parentId, string str)
    {
        if (Url.Contains(_Url))
        {
           
            return str;
        }    
            
        else
        {
            var item = Urls.Where(e => Url.Contains(e.Url)).FirstOrDefault();
            if (item != null && item.ParentId == parentId)
            {
                //NLogLogger.Info(item.Url);
                return str;
            }    
                
            else
                return string.Empty;
        }
    }
    public string GetGroupName(string lang, string groupname)
    {
        if (lang == "vi-vn")
            return groupname;

        switch (groupname)
        {
            case "Nạp bank":
                return "入款";

            case "Rút bank":
                return "出款";
            case "Cấu hình":
                return "账号";
            case "Số dư":
                return "余额";
            case "Kết nối":
                return "连接";
            case "Quản Trị":
                return "管理";
            case "Gạch thẻ":
                return "刮刮卡";
            case "Mua thẻ":
                return "买卡";
        }
        return groupname;
    }
    public string GetMenuName(string lang, string groupname)
    {
        if (lang == "vi-vn")
            return groupname;

        switch (groupname)
        {
            case "Xem log":
                return "查交易";
            case "Tra cứu":
                return "抬头";
            case "Báo cáo":
                return "汇报";
            case "Đối soát":
                return "统计";
            case "Cài đặt bảo mật":
                return "设置谷歌验证码";
            case "Rút tiền":
                return "提款";
            case "Lịch sử giao dịch":
                return "历史";

            case "Đối tác":
                return "商户";

            case "Nhà cung cấp":
                return "供应商";
            case "Người dùng":
                return "用户";
            case "Tính năng":
                return "特征";
            case "Báo cáo đối tác cuối ngày":
                return "当天结束时向合作伙伴汇报";
            case "Thẻ cào":
                return "刮刮卡";
            case "Mua thẻ":
                return "买卡";
            case "Cài đặt":
                return "双因素身份验证";
        }
        return groupname;
    }
}
public class MenuModel{
    public List<Roles> list { get; set;}
    public List<Roles> listMenu { get; set; }
    public Roles item { get; set; }
    
}

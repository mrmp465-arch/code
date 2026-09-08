using System; 
using System.Collections.Generic; 
using System.Web;
using System.Linq;
using Libs.API;

public partial class Layout_TopMenu : System.Web.UI.UserControl
{
    public List<MenuModel> lisUrl;
    public List<Roles> lstUrlResources;
    public string Url { get; set; } 
    protected void Page_Load(object sender, EventArgs e)
    {
        lisUrl = new List<MenuModel>();
        var Ids = new List<int>();
        var list=  lstUrlResources = new Roles().GetList().OrderBy(x=>x.Group).Where(x=>x.Status==1).ToList();
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
            return str;
        else
        {
            var item = Urls.Where(e => Url.Contains(e.Url)).FirstOrDefault();
            if (item != null && item.ParentId == parentId)
                return str;
            else
                return string.Empty;
        }
    }
}
public class MenuModel{
    public List<Roles> list { get; set;}
    public List<Roles> listMenu { get; set; }
    public Roles item { get; set; }
    
}

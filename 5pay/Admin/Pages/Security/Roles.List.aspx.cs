using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.ComponentModel; 

public partial class Pages_Security_Roles_List : System.Web.UI.Page
{
    public List<Roles> lstRole;
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.RolesList);
        if (!IsPostBack)
        {
            BindData();
        }
    }
    protected void BindData()
    { 
       var lst = new Roles().GetList();
        lst = lst.OrderBy(e => e.Group).ToList();
        lstRole = new List<Roles>();
        foreach (var item in lst)
        {
            if (item.ParentId == 0)
            {
                lstRole.Add(item);
               var temp= lst.Where(e => e.ParentId == item.Id);
                if(temp!=null)
                foreach (var item2 in temp)
                {
                        item2.Name ="  ---  "+ item2.Name;
                        lstRole.Add(item2);
                }
            }
        }
        rptList.DataSource = AppUtils.ToDataTable(lstRole);
        rptList.DataBind();
    } 
    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.RolesAdd);
    } 
    public string viewMenu(bool obj,string txt)
    {
        if (obj)
            return txt;
        else
            return "";
    } public string viewStatus(int obj,string txt)
    {
        if (obj==1)
            return txt;
        else
            return "";
    } 
}
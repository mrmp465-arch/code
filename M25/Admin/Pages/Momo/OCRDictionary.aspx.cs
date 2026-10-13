using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Libs.Utils;
using DocumentFormat.OpenXml.Office2010.Excel;

public partial class Pages_Momo_OCRDictionary : System.Web.UI.Page
{
    public JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoOCRDictionary);

        if (!IsPostBack)
        {
           GetList();
        }
    }


    private void GetList()
    {
        int top = Convert.ToInt32(drpTop.SelectedValue);
        int? status = null;
        if (drpStatus.SelectedValue != "-1") status = AppUtils.ToInt32(drpStatus.SelectedValue);
        var lstdata = new MomoOCRDictionary().GetList(top, txtSource.Text, txtDestination.Text, status);
        rptList.DataSource = lstdata;
        rptList.DataBind();
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string DeleteTranInUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.EditTranIn + "?id=" + id;
    }

    protected void btnDel_Click(object sender, EventArgs e)
    {
        
        var id = Convert.ToInt32(myHiddenField.Value);
        var ocrdic = new MomoOCRDictionary();
        ocrdic.Id = id;
        ocrdic.Delete();
        GetList();
    }

    protected void btnAccept_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)(sender);
        var id = Convert.ToInt32(btn.CommandArgument);
        var ocrdic = new MomoOCRDictionary();
        ocrdic.Id = id;
        ocrdic.Status = 1;
        ocrdic.UpdateTime = DateTime.Now;
        ocrdic.Update();

        GetList();
    }
    public string GetStatus(object status)
    {
        if (status.ToString() == "1")
        {
            return "<span class=\"label label-success\">Đã duyệt</span>";
        }

        if (status.ToString() == "0")
        {
            return "<span class=\"label label-danger\">Chưa duyệt</span>";
        }
        return "N/A";
    }

}
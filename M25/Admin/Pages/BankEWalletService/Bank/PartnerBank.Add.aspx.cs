using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;
using Libs.Utils;
using System.Web.Script.Serialization;
public partial class Pages_BankEWalletService_Bank__Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankPartnerAdd);

        if (!IsPostBack)
        {
            
            init();
            BindData();
        }
    }
    private void init()
    {
        var lst = new PartnerBank().GetListPartner();
        lst = lst.OrderBy(x => x.Code).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "Id";
        drpPartner.DataBind();
        //drpPartner.Items.Insert(0, new ListItem("Kênh:", "0"));
        //drpPartner.SelectedValue = "ch01";
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    private void BindData()
    {
        var _Bank = new BankAccounts();
        var data = _Bank.GetList().Where(x=>x.Type!= "OUTALL" ).OrderBy(x => x.Id).ToList();

        var name = txtName.Text.Trim();
        if (!string.IsNullOrEmpty(name))
            data = data.Where(x => x.BankName.Contains(name)).ToList();

        var bankid = txtBankId.Text.Trim();
        if (!string.IsNullOrEmpty(bankid))
            data = data.Where(x => x.BankId.Contains(bankid)).ToList();



        var lstMapping = new PartnerBank().GetList();
        var lstAcount = new List<BankAccounts>();
        foreach (var item in data)
        {
            if (!lstMapping.Exists(x => x.BId == item.Id))
            {
                lstAcount.Add(item);
            }
        }
        rptList.DataSource = lstAcount;
        rptList.DataBind();
    }
    //protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    txtCallbackUrl.Text = "http://127.0.0.1:1592/Callback/MDrumCallback.ashx?partnercode=" + drpPartner.SelectedValue;
    //    txtPartnerCode.Text = drpPartner.SelectedValue;
    //    txtPartnerName.Text = drpPartner.SelectedItem.Text;
    //}
    protected void btAdd_Click(object sender, EventArgs e)
    {
        //check parnter
    
        var partnerId = int.Parse(drpPartner.SelectedValue);
        //nếu chưa có thì thêm mới
        if (partnerId > 0)
        {
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
                Label lbPId = (Label)rptList.Items[i].FindControl("lblId");


                if (cbx.Checked)
                {
                    var _PartnerBank = new PartnerBank();
                    _PartnerBank.PartnerId = partnerId;
                    _PartnerBank.BId = Convert.ToInt32(lbPId.Text);
                    _PartnerBank.Status = 1;
                    _PartnerBank.OrderNo = 1;
                    _PartnerBank.Add();
                }
                //NLogLogger.Info(new string[] { "Data", "lbPId", lbPId.Text });



                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerBank) });
                //thêm mapping
                
            }
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnerBank);
        }
        else
        {
            AlertInfos.Text = "Vui lòng chọn lại đối tác";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
       
    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnerBank);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;
using System.Globalization;


public partial class Pages_Security_PartnerTransaction : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnerTransaction);

        if (!IsPostBack)
        {
            init();
            GetList();
        }
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        
        GetList();
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        var code = drpPartner.SelectedValue;
        var amount = long.Parse(txtAmount.Text);
        if(!string.IsNullOrEmpty(code) && amount>0)
        {
            var _tran = new PartnerTransaction();
            _tran.PartnerCode = code;
            _tran.Amount = amount;
            _tran.Note = txtNote.Text;
            _tran.Type = 1;
            _tran.Add();
            GetList();
        }    
       
    }
    private void init()
    {
        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        if(lst.Count!=1)
        {
            drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));
        }
        else
        {
            drpPartner.Visible = false;
            btView.Visible = false;
        }
        long balance = 0;
        if (AppUtils.IsPartner)
        {
            foreach (var item in lst)
            {
                balance += item.Balance;
            }
        }
        lblTotal.Text = " Số dư: " + balance.ToString("#,#").Replace(",", ".");


    }
    public string GetStatus(object status)
    {
        switch (int.Parse(status.ToString()))
        {
            case 1:
                return "Hoàn thành";
            case 0:
                return "Chờ thanh toán";
            case -1:
                return "Hủy";
        }
        return "";
    }
    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        var _tran = new PartnerTransaction();
        _tran.Id = Convert.ToInt32(e.CommandArgument.ToString());
        _tran.Status = -1;
        _tran.Update();
        GetList();

    }
    protected void Update_Command(Object sender, CommandEventArgs e)
    {
        
        var _tran = new PartnerTransaction();
        _tran.Id = Convert.ToInt32(e.CommandArgument.ToString());
        _tran.Confirm();
        GetList();

    }
    private void GetList()
    {
        string partnerCodes = drpPartner.SelectedValue;
        if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                }
            }

        }
        PartnerTransaction _Transaction = new PartnerTransaction();
        rptList.DataSource = _Transaction.GetList(partnerCodes, 1);
        rptList.DataBind();


    }
}
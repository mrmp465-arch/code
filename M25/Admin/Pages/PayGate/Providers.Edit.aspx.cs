using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;
using Libs.Utils;

public partial class Pages_PayGate_Provider_Edit : System.Web.UI.Page
{
    public string Type { get; set; }
    public string productType { get; set; }
    public bool GetCheckCardType(string _ProductCode)
    {
        if (!string.IsNullOrEmpty(productType))
        {
            var lst = productType.Split('|');
            if (lst.Contains(_ProductCode))
                return true;
        }
        return false;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ProviderEdit);
        if (!IsPostBack)
        {
            init();
            //      txtType.SelectedValue = Type = Request["type"];
        }
    }

    private void init()
    {


        var providerId = Convert.ToInt32(AppUtils.Request("id"));
        var _Provider = new Providers().Get(providerId);
        Type = _Provider.Type.ToString();
        if (_Provider == null)
        {
            Response.Redirect(Resources.Url.ProviderList);
        }

        lblProviderCode.Text = _Provider.ProviderCode;
        lblProviderId.Text = _Provider.ProviderId.ToString();
        txtName.Text = _Provider.Name;
        txtProviderCode.Text = _Provider.ProviderCode;
        chkIsActive.Checked = Convert.ToBoolean(_Provider.Status);
        txtSignatureType.Text = _Provider.SignatureType.ToString();
        txtPrivateKey.Text = _Provider.PrivateKey;
        txtPublicKey.Text = _Provider.PublicKey;
        txtOrderNo.Text = _Provider.OrderNo.ToString();
        txtQuota.Text = _Provider.Quota.ToString();
        drpOccurs.SelectedValue = _Provider.Occurs.ToString();
        productType = _Provider.ProductCode;
        txtType.SelectedValue = _Provider.Type.ToString();
        txtGSMUrl.Text = _Provider.GSMUrl;
        txtAmount.Text = _Provider.AmoutList;
        DataView dv1 = new Products().GetTable().DefaultView;
        if (string.IsNullOrEmpty(Type))
            Type = "7";
        dv1.RowFilter = "Status=1 and Type = " + Type;
        rptList.DataSource = dv1.ToTable();
        rptList.DataBind();



        var LstPartners = new Partners().GetList().OrderBy(x => x.PartnerCode).ToList() ;
        DataTable table = new DataTable();
        table.Columns.Add("PartnerId", typeof(int));
        table.Columns.Add("PartnerCode", typeof(string));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("IsCheck", typeof(bool));
        if (LstPartners != null)
        {
            LstPartners = LstPartners.Where(e => e.Status == 1).ToList();
            var listPartnerProvider = new PartnerProvider().GetList(0, providerId);
            if (listPartnerProvider == null)
                listPartnerProvider = new List<PartnerProvider>();
            foreach (var item in LstPartners)
            {
                var temp = listPartnerProvider.FirstOrDefault(e => e.PartnerId == item.PartnerID);
                if (temp != null)
                {
                    table.Rows.Add(item.PartnerID, item.PartnerCode, item.Name, true);

                }
                else //if (LstUserPartner.Count(e => e.PartnerId == item.PartnerID) == 0)
                    table.Rows.Add(item.PartnerID, item.PartnerCode, item.Name, false);
            }
        }

        rptListPartner.DataSource = table;
        rptListPartner.DataBind();


    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {

        var providerId = Convert.ToInt32(AppUtils.Request("id"));

        var _Provider = new Providers() { ProviderId = providerId };
        _Provider = _Provider.Get();

        _Provider.Name = txtName.Text;
        _Provider.ProviderCode = txtProviderCode.Text;
        _Provider.Status = Convert.ToInt32(chkIsActive.Checked);
        _Provider.SignatureType = AppUtils.ToInt32(txtSignatureType.Text);
        _Provider.PrivateKey = txtPrivateKey.Text;
        _Provider.PublicKey = txtPublicKey.Text;
        _Provider.OrderNo = AppUtils.ToInt32(txtOrderNo.Text);
        _Provider.Quota = AppUtils.ToInt64(txtQuota.Text);
        _Provider.Occurs = Convert.ToInt32(drpOccurs.SelectedValue);
        _Provider.Type = AppUtils.ToInt32(txtType.SelectedValue);
        _Provider.GSMUrl = txtGSMUrl.Text;
        _Provider.AmoutList = txtAmount.Text;

        var list = new Products().GetList(_Provider.Type, 7);
        var _ProductCode = new List<string>();
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxProCode");
            HiddenField txtProduct = (HiddenField)rptList.Items[i].FindControl("txtProCode");
            if (cbx.Checked)
                _ProductCode.Add(txtProduct.Value);
        }
        _Provider.ProductCode = String.Join("|", _ProductCode.ToArray());
        _Provider.Update();
        string KeyCache = string.Format("{0}:{1}", "RediProviders", _Provider.ProviderCode);
        DataCaching.RemoveCache(KeyCache);

        var LstPartners = new Partners().GetList();
        if (LstPartners != null)
        {
            var LstPartnerProvider = new PartnerProvider().GetList(0, 0);
            if (LstPartnerProvider == null)
                LstPartnerProvider = new List<PartnerProvider>();
            for (int i = 0; i < rptListPartner.Items.Count; i++)
            {

                CheckBox IsCheck = (CheckBox)rptListPartner.Items[i].FindControl("cbxIsCheckPartner");
                HiddenField txtPartnerId = (HiddenField)rptListPartner.Items[i].FindControl("txtPartnerId");
                HiddenField txtPartnerCode = (HiddenField)rptListPartner.Items[i].FindControl("txtPartnerCode");
                PartnerProvider item = LstPartnerProvider.FirstOrDefault(a => a.PartnerId == Convert.ToInt32(txtPartnerId.Value) && a.ProviderId == _Provider.ProviderId);
                if (IsCheck.Checked)
                {
                    if (item == null)
                    {
                        var _PartnerProvider = new PartnerProvider();
                        _PartnerProvider.PartnerId = Convert.ToInt32(txtPartnerId.Value);
                        _PartnerProvider.PartnerCode = txtPartnerCode.Value;
                        _PartnerProvider.ProviderId = _Provider.ProviderId;
                        _PartnerProvider.ProviderCode = _Provider.ProviderCode;
                        _PartnerProvider.Add();
                    }
                }
                else
                if (item != null)
                    item.Delete();
            }
        }


        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderList + "?type=" + _Provider.Type);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderList);
    }

    protected void btApply_Click(object sender, EventArgs e)
    {

    }
}
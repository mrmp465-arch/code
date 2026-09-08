using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using System.Data;
using System.IO;
using Libs.Utils;
using System.Web.Script.Serialization;

public partial class Pages_CardStore_CardReseller_Orders : System.Web.UI.Page
{
    public string Type;
    public int OrderId;
    public List<ProvidersStore> lstProviders;
    int PageSize = 20;
    public Pager pages;
    int page = 1;
    public bool btIsView = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardResellerOrders);
        int Status = 0;
        Type = Request["Type"];
        if (!string.IsNullOrEmpty(Request["OrderId"]))
            int.TryParse(Request["OrderId"], out OrderId);
        lstProviders = new ProvidersStore().GetList();
        if (!IsPostBack)
        {
            if (Session["CheckRefresh"] == null)
                Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString());
        } 
        if (Type == "update" || Type == "delete" || Type == "add" || Type == "count" || Type == "gencode")
        {
            if (!IsPostBack)
            { 
                bool Result = false;
                var db = new Orders();
                if (Type == "gencode")
                {
                    db.No = NewOrderNo();
                    Result = true;
                }
                else if (OrderId > 0)
                {
                    db.Id = OrderId;
                    var item = db.Get();
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(Request["Status"]))
                            int.TryParse(Request["Status"], out Status);
                        var Provider = Request["Provider"];
                        if (Type == "count")
                        {
                            if (Provider != item.ProviderCode)
                            {
                                db = db.GetCountCardPacket(OrderId, null);
                                if (Provider != item.ProviderCode)
                                    db.ReturnValue = 0;
                                Result = true;
                            }

                        }
                        else if (Type == "update")
                        {
                            if (item.ProviderCode != Provider)
                            {
                                var dbPack = new Packet();
                                var dbCardStore = new CardStore();
                                dbPack.Update_byOrderId(item.Id, Provider, Status);
                                if (Provider != item.ProviderCode)
                                {
                                    int totalRecord = 0;
                                    var lst = dbPack.GetListPage(null, item.Id, null, null, null, null, null, null, null, null, 1, int.MaxValue, out totalRecord);
                                    if (totalRecord > 0)
                                    {
                                        foreach (var item2 in lst)
                                        {
                                            dbCardStore.Update_byPacketId(item2.Id, item2.ProviderCode, item2.CardType, item2.ExpireDate, item2.CardValue, item2.IsActive);
                                        }
                                    }
                                }
                            }

                            item.Name = Request["Name"];
                            item.Status = Status;
                            item.ProviderCode = Request["Provider"];
                            item.Update();
                            Result = true;
                        }
                        else if (Type == "delete")
                        {
                            var dbPack = new Packet();
                            var dbCardStore = new CardStore();
                            int totalRecord = 0;

                            var lst = dbPack.GetListPage(null, item.Id, null, null, null, null, null, null, null, null, 1, int.MaxValue, out totalRecord);
                            if (totalRecord > 0)
                            {
                                foreach (var item2 in lst)
                                {
                                    dbCardStore.DeletebyPacketId(item2.Id);
                                }
                            }
                            dbPack.DeletebyOrderId(item.Id);
                            item.Delete();
                            Result = true;
                        }
                    }
                }
                else if (Type == "add")
                {
                    db.No = Request["OrderNo"] + "";
                    db.Name = Request["Name"] + "";
                    db.ProviderCode = Request["Provider"] + "";
                    if (!string.IsNullOrEmpty(Request["Status"]))
                        int.TryParse(Request["Status"], out Status);
                    db.Status = Status;
                    db.Add();
                    Result = true;
                }
                string json = "{\"Result\":\"" + Result +
                                "\",\"Type\":\"" + Type +
                                "\",\"No\":\"" + db.No +
                                "\",\"Id\":\"" + db.Id +
                                "\",\"Provider\":\"" + db.ProviderCode +
                                "\",\"Status\":\"" + db.Status +
                                "\",\"Pack\":\"" + db.Status +
                                "\",\"Card\":\"" + db.ReturnValue + "\"}";
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.Write(json);
                Response.End();
            }
        }
        else
        {
            init();
            if (!string.IsNullOrEmpty(Request["page"]))
                int.TryParse(Request["page"], out page);
            GetList(page);
        } 
    }
    private void init()
    {
        var _Providers = new ProvidersStore();
        drpProviders.DataSource = _Providers.GetList();
        drpProviders.DataBind();
        drpProviders.DataTextField = "Name";
        drpProviders.DataValueField = "ProviderCode";
        drpProviders.DataBind();
        drpProviders.Items.Insert(0, new ListItem("Provider:", "")); 
    }
    private void GetList(int page)
    {
        int totalRecord;
        int? OrderId = null;
        string CardCode = txtCardSerial.Text;
        string ProviderCode = drpProviders.SelectedValue;

        int? status = null;
        if(!string.IsNullOrEmpty(drpStatus.SelectedValue))
            status= Convert.ToInt32(drpStatus.SelectedValue);
        if (txtIsUpload.Value != "true" || !string.IsNullOrEmpty(CardCode))
        {
            if (!string.IsNullOrEmpty(txtOrderId.Value))
                OrderId = int.Parse(txtOrderId.Value);            
        }
        else
            txtIsUpload.Value = "false";
        rptList.DataSource = new Orders().GetTable(OrderId,null, null, ProviderCode, status, page, PageSize, out totalRecord);
        rptList.DataBind();
        pages = new Pager(totalRecord, page, PageSize);
    }
    public string GetactivePage(int page1, int page2)
    {
        if (page1 == page2)
            return "active";
        else
            return "";
    }
    protected void btView_Click(object sender, EventArgs e)
    { 
        setDefaultVaule();
        if (!string.IsNullOrEmpty(txtCardSerial.Text))
        {
            var item = new Packet().GetbyCard(txtCardSerial.Text.Trim(),"");
            if (item != null)
            {
                txtPackId.Value = item.Id.ToString();
                txtOrderId.Value = item.OrderId.ToString();
                btIsView = true;
            }
            else
            {
                txtPackId.Value ="0";
                txtOrderId.Value ="0";
            }
        } 
        GetList(1);  
    }
   protected void setDefaultVaule()
    {

        txtPackId.Value = "0";
        txtOrderId.Value = "";  
        txtProviderCode.Value = "";
        txtCardType.Value = "";
        txtCardValue.Value = "";
        txtIsActive.Value = "";
        txtExpireDate.Value = "";
        txtNo.Value = "";
        
    }
    public string GetStatus(int status)
    {
        if (status == 1)
            return "1";
        else if (status == 0)
            return "0";
        else
            return status.ToString();
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    { 
        string MsgType = string.Empty;
        string Msg = "";

        if (Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString())
        {
            Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString());
            if (FileUploadExcel.HasFile)
            {
                try {
                    string fileN = DateTime.Now.ToString("dd_MM_yyyy_hhmmss") + "_" + FileUploadExcel.FileName;
                    string pathFile = Server.MapPath("~/Excels/CardStore/" + fileN);
                    FileUploadExcel.SaveAs(pathFile);

                    int PackId = 0;
                    int CardValue = 0;
                    bool IsActive = true;
                    bool IsSold = false;

                    var ProviderCode = txtProviderCode.Value;
                    var CardType = txtCardType.Value;

                    if (!string.IsNullOrEmpty(txtPackId.Value))
                        int.TryParse(txtPackId.Value, out PackId);
                    if (!string.IsNullOrEmpty(txtCardValue.Value))
                        if (!string.IsNullOrEmpty(txtIsActive.Value))
                            int.TryParse(txtCardValue.Value, out CardValue);
                    bool.TryParse(txtIsActive.Value, out IsActive);
                    var ExpireDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(txtExpireDate.Value))
                        ExpireDate = DateTime.ParseExact(txtExpireDate.Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    int i = 0;
                    string[] lines = System.IO.File.ReadAllLines(pathFile);
                    if (lines != null && lines.Count() > 0)
                    {
                        lines = lines.Where(a => !string.IsNullOrEmpty(a)).ToArray();
                    }
                    var TotalLine = lines.Count();
                    string CardExist = "";
                    int CardExistCount = 0;
                    var _pack=new Packet() { Id = PackId }.Get(); 
                    foreach (string line in lines)
                    {
                        if (i++ == 0) continue;
                        var cols = line.Split('\t', ' ');
                        try
                        {

                            if (!string.IsNullOrEmpty(cols[4]))
                                ExpireDate = DateTime.ParseExact(cols[4], "dd/MM/yyyy", CultureInfo.InvariantCulture);                            
                            
                            CardStore obj = new CardStore
                            {
                                PacketId = PackId,
                                ProviderCode = ProviderCode,
                                CardType = CardType,
                                CardValue = int.Parse(cols[1]),
                                IsActive = IsActive,
                                IsSold = IsSold,
                                CardSerial = cols[2],
                                CardCode = cols[3],
                                ExpireDate = ExpireDate 
                            };
                            obj.Add();                            
                            if (obj.Id == -1)
                            {
                                CardExist += "<tr><td> " + cols[2] + "</td><td> dòng " + i + "</td> </tr>";
                                CardExistCount++;
                            }
                            else if (obj.Id <= 0)
                                break;
                            else {
                                _pack.NumberCardUp++;
                            }

                        }
                        catch (Exception ex)
                        {
                            Msg = ex.Message;
                            break;
                        }
                    }
                    _pack.Update();
                    if (CardExistCount > 0)
                        CardExist = "<br/><br/>"+ CardExistCount + " Mã thẻ đã tồn tại: <table style='width:100%'>" + CardExist + "</table>";
                    if (i < TotalLine)
                    {
                        AlertInfos.Text = " Đã import được " + (i - 1 - CardExistCount) + "/" + (TotalLine - 1) + " Thẻ <br/> Lỗi từ dòng " + i + CardExist;
                        MsgType = "AlertInfo";
                    }
                    else if (CardExistCount > 0)
                    {
                        AlertInfos.Text = " Import thành công! <br/>" + (TotalLine - 1 - CardExistCount) + " Thẻ " + CardExist;
                        MsgType = "AlertInfo";
                    }
                    else
                    {
                        AlertSuccesss.Text = " Import thành công! <br/>" + (TotalLine - 1 - CardExistCount) + " Thẻ " + CardExist;
                        MsgType = "AlertSuccess";
                    }

                    txtProviderCode.Value = "";
                    txtCardType.Value = "";
                    txtCardValue.Value = "";
                    txtIsActive.Value = "";
                  
                    //txtOrderId.Value = ""; 
                }
                catch (Exception ex)
                {
                    AlertBans.Text = "Lỗi: " + ex.Message;
                    MsgType = "AlertBan";
                }
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() { $('#" + MsgType + "').modal()}); $('#aspnetForm').reset();", true);
            }
        }
        txtIsUpload.Value = "true";
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["CheckRefresh"] = Session["CheckRefresh"];
    }
    public string NewOrderNo()
    {
        return new Orders().GenOrderCode();
    }
}
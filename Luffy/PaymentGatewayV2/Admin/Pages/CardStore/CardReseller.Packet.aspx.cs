using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.Utils;
using System.Web.Script.Serialization;

public partial class Pages_CardStore_CardReseller_Packet : System.Web.UI.Page
{
    public string Provider;
    public string OrderNo;
    public string Type;
    public int PackId;
    public int OrderId = 0;
    public List<ProductsStore> lstProducts;
    public List<ProvidersStore> lstProviders;
    public int Status = 0;
    bool IsActive = false;
    int CardValue = 0;
    int NumberCard = 0;
    int NumberCardSole = 0;
    int NumberCardUp = 0;
    DateTime ExpireDate;
    int PageSize = 20;
    public Pager pages;
    int page = 1;
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardResellerOrders);
        Provider = Request["Provider"];
        OrderNo = Request["OrderNo"];
        Type = Request["Type"];
        if (!string.IsNullOrEmpty(Request["PackId"]))
            int.TryParse(Request["PackId"], out PackId);
        if (!string.IsNullOrEmpty(Request["OrderId"]))
            int.TryParse(Request["OrderId"], out OrderId);


        lstProducts = new ProductsStore().GetList();
        lstProviders = new ProvidersStore().GetList();

        if (!IsPostBack)
        {
            bool Result = false;
            var db = new Packet();
            var dbOrders = new Orders();
            if (Type == "update" || Type == "delete" || Type == "add" || Type == "count" ||Type== "updatestatus")
            {
                if (PackId > 0)
                {  
                        db.Id = PackId;
                        var item = db.Get();
                        if (item != null)
                        {
                            var CardType = Request["CardType"];
                            Provider = Request["ProviderCode"];
                            var strExpireDate = Request["ExpireDate"]; 
                            if (!string.IsNullOrEmpty(Request["IsActive"]))
                                bool.TryParse(Request["IsActive"], out IsActive);
                            if (!string.IsNullOrEmpty(Request["NumberCard"]))
                                int.TryParse(Request["NumberCard"], out NumberCard);
                            if (!string.IsNullOrEmpty(Request["NumberCardSole"]))
                                int.TryParse(Request["NumberCardSole"], out NumberCardSole);
                            if (!string.IsNullOrEmpty(Request["NumberCardUp"]))
                                int.TryParse(Request["NumberCardUp"], out NumberCardUp);
                            try
                            {
                             if (!string.IsNullOrEmpty(strExpireDate) && strExpireDate != "undefined")
                                 ExpireDate = DateTime.ParseExact(Request["ExpireDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                                {
                                ExpireDate = item.ExpireDate;
                            }
                            
                             
                            if (!string.IsNullOrEmpty(Request["CardValue"]))
                                int.TryParse(Request["CardValue"], out CardValue);
                            if (!string.IsNullOrEmpty(Request["Status"]))
                                int.TryParse(Request["Status"], out Status);
                            if (Type == "count")
                            {
                                if (item.ProviderCode != Provider || item.CardType != CardType || item.CardValue != CardValue)//|| item.ExpireDate != ExpireDate
                            {
                                    dbOrders = dbOrders.GetCountCardPacket(null, PackId);
                                    Result = true;
                                } 
                            }else if (Type == "update")
                            { 
                                if (item.ProviderCode != Provider || item.CardType != CardType || item.CardValue != CardValue)
                                    new CardStore().Update_byPacketId(item.Id, Provider, CardType, ExpireDate, CardValue,item.IsActive);

                                item.Name = Request["Name"];
                                item.NumberCard = NumberCard;
                                item.NumberCardSole = NumberCardSole;
                                item.NumberCardUp = NumberCardUp;
                                item.ProviderCode = Provider;
                                item.CardType = CardType;
                                item.ExpireDate = ExpireDate;
                                item.CardValue = CardValue;
                                item.Status = Status; 
                                item.Update(); 
                                Result = true;
                            }else if(Type== "updatestatus")
                            {
                                if (item.IsActive != IsActive)
                                    new CardStore().Update_byPacketId(item.Id, item.ProviderCode, item.CardType, item.ExpireDate,item.CardValue, IsActive);
                                item.IsActive = IsActive;
                                item.Update();
                                Result = true;
                            }
                            if (Type == "delete")
                            {
                                new CardStore().DeletebyPacketId(item.Id);
                                item.Delete();
                                Result = true;
                            }
                        } 
                }
                else if (Type == "add")
                {
                    if (!string.IsNullOrEmpty(Request["OrderId"]))
                        int.TryParse(Request["OrderId"], out OrderId);
                    if (!string.IsNullOrEmpty(Request["IsActive"]))
                        bool.TryParse(Request["IsActive"], out IsActive);
                    if (!string.IsNullOrEmpty(Request["Status"]))
                        int.TryParse(Request["Status"], out Status);
                    if (!string.IsNullOrEmpty(Request["NumberCard"]))
                        int.TryParse(Request["NumberCard"], out NumberCard);
                    if (!string.IsNullOrEmpty(Request["NumberCardSole"]))
                        int.TryParse(Request["NumberCardSole"], out NumberCardSole);
                    if (!string.IsNullOrEmpty(Request["NumberCardUp"]))
                        int.TryParse(Request["NumberCardUp"], out NumberCardUp);
                    if (!string.IsNullOrEmpty(Request["CardValue"]))
                        int.TryParse(Request["CardValue"], out CardValue);
                    try
                    {
                        if (!string.IsNullOrEmpty(Request["ExpireDate"]))
                            ExpireDate = DateTime.ParseExact(Request["ExpireDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        ExpireDate = DateTime.Now;
                    }
                    


                    db.NumberCard = NumberCard;
                    db.Name = Request["Name"];
                    db.OrderId = OrderId;
                    db.OrderNo = OrderNo;
                    db.ExpireDate = ExpireDate;
                    db.NumberCardSole = NumberCardSole;
                    db.ProviderCode = Provider;
                    db.CardValue = CardValue;
                    db.CardType = Request["CardType"];
                    db.Status = Status;
                    db.ProviderCode = Request["ProviderCode"];
                    db.IsActive = IsActive;
                    db.Add();
                    Result = true;
                }
                var json1 = new JavaScriptSerializer().Serialize(db);
                string json = "{\"Result\":\"" + Result + "\",\"Type\":\"" + Type + "\",\"Id\":\"" + db.Id +
                    "\",\"Card\":\"" + dbOrders.ReturnValue +
                    "\",\"OrderId\":\"" + db.OrderId +
                    "\",\"ProviderCode\":\"" + db.ProviderCode +
                    "\",\"CardType\":\"" + db.CardType +
                    "\",\"CardValue\":\"" + db.CardValue +
                    "\",\"ExpireDate\":\"" + db.ExpireDate +
                    "\",\"IsActive\":\"" + db.IsActive +
                    "\"}";
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.Write(json);
                Response.End();
            }
            else
            {

                int totalRecord;
                if (!string.IsNullOrEmpty(Request["page"]))
                    int.TryParse(Request["page"], out page);
                if (string.IsNullOrEmpty(OrderNo))
                    OrderNo += "0";
                int? _PackId = null;
                if (PackId > 0) _PackId = PackId;

                rptList.DataSource = db.GetTable(_PackId, null, OrderNo, Provider, null, null, null, null, "", null, page, PageSize, out totalRecord);
                rptList.DataBind();
                pages = new Pager(totalRecord, page, PageSize);
            }
        }
    }
    public string GetactivePage(int page1, int page2)
    {
        if (page1 == page2)
            return "active";
        else
            return "";
    }

    public string GetStatus(int status)
    {
        if (status == 1)
            return "Tự động";
        else if (status == 0)
            return "Nhập tay";
        else
            return status.ToString();
    }
}
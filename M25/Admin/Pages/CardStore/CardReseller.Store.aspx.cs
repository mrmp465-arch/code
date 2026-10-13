using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.API;
using Libs.Utils;

public partial class Pages_CardStore_CardReseller_Store : System.Web.UI.Page
{
    public string PacketId;
    public int StoreId;
    public int packetId;
    public string Type;
    public string CardSerial;
    public string CardCode;
    public bool IsSold;
    public bool IsActive;

    int PageSize =20;
    public Pager pages;
    int page = 1;
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardResellerOrders);
        PacketId = Request["PacketId"];
        Type = Request["Type"];
        CardSerial = Request["CardSerial"];
        CardCode = Request["CardCode"];
        if (!string.IsNullOrEmpty(Request["StoreId"]))
            int.TryParse(Request["StoreId"], out StoreId);

        if (!string.IsNullOrEmpty(Request["PacketId"]))
            int.TryParse(Request["PacketId"], out packetId);

        if (!IsPostBack)
        {
            bool Result = false;
            var db = new CardStore();
            if (Type == "update" || Type == "delete" ||Type== "updatestatus")
            {
                if (StoreId > 0)
                {
                    db.Id = StoreId;
                    var item= db.Get();
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(Request["IsActive"]))
                            bool.TryParse(Request["IsActive"], out IsActive);
                        if (Type == "update")
                        {
                            if (!string.IsNullOrEmpty(Request["IsSold"]))
                                bool.TryParse(Request["IsSold"], out IsSold);  
                            item.IsSold = IsSold;
                            item.IsActive = IsActive; 
                            item.Update();
                            Result = true;
                        } else if (Type== "updatestatus") {
                            item.IsActive = IsActive;
                            item.Update();
                            Result = true;

                        } else if (Type == "delete")
                        {
                            db.Delete();
                            Result = true;
                        }
                    } 
                } 
                string json = "{\"Result\":\""+Result+ "\",\"Type\":\"" + Type + "\",\"Id\":\"" + db.Id + "\"}";
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

                rptList.DataSource = db.GetTable(packetId, null, null, CardSerial, CardCode, null, null, null,page,PageSize,out totalRecord);
                rptList.DataBind();
                pages = new Pager(totalRecord, page, PageSize);
            } 
        }
    }
    public string GetactivePage(int page1, int page2)
    {
        if (page1 == page2)
            return "Active";
        else
            return "";
    }
      
    public string GetSold(bool IsSold)
    {
        if (IsSold)
            return "Đã bán";
        else  
            return "Chưa bán";
    }
}
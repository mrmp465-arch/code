using Libs.API;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Monitor_Card_OrderDetail : System.Web.UI.Page
{
    public string OrderNo { get; set; }
    public string PartnerCode { get; set; }
    public int Status { get; set; }
    string urlService = "http://localhost:1598/VPGJsonService.ashx";
    //string urlService = "http://149.28.130.246:1598/VPGJsonService.ashx";
    string serviceCode = "buycard";
    string commandCode = "buycard";
    public int OrderId { get; set; }
    //public int Status { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardOrder);
        OrderId = Convert.ToInt32(Request["id"]);
        CardOrder _order = new CardOrder().Get(OrderId);
       
        OrderNo = _order.OrderNo;
        PartnerCode = _order.PartnerCode;
        OrderNo = _order.OrderNo;
        Status = _order.Status;
        if (!IsPostBack)
        {
            
            //Response.Write(String.Format("<script>Lỗi mua thẻ {0}</script>", "-317"));
            BindData();
        }
    }
    private void BindData()
    {

       
        var lstPacket = new CardOrderPacket().GetList(OrderId);
       
        rptList1.DataSource = lstPacket;
        rptList1.DataBind();
    }
    protected void btnCreate_Click(object sender, EventArgs e)
    {
        var order = new CardOrderPacket { OrderNo = OrderNo, OrderId = OrderId, Status = 0, CardType = drpCardType.SelectedValue, CardValue = int.Parse(drpCardValue.SelectedValue), NumberCard = int.Parse(txtNumberCard.Text) };
        order.Add();
        txtNumberCard.Text = "";
        BindData();
    }
    protected void btApply_Click1(object sender, EventArgs e)
    {
        updatePacket(rptList1);
    }
    protected void btApply_Click2(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + "pages/monitor/card.order.aspx");
    }
    public void updatePacket(Repeater rptList)
    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            DropDownList cbx = (DropDownList)rptList.Items[i].FindControl("drpCardTypeItem");
            DropDownList tqt = (DropDownList)rptList.Items[i].FindControl("drpCardValueItem");
            TextBox tbx = (TextBox)rptList.Items[i].FindControl("txtNumberCardItem");
            Label lbPId = (Label)rptList.Items[i].FindControl("lblCardId");

            //var tbxValue = tbx.Text;
            //if (tbx.Text.Contains(','))
            //    tbxValue = tbx.Text.Split(',')[1];

            //var tqtValue = tqt.Text;
            //if (tqt.Text.Contains(','))
            //    tqtValue = tqt.Text.Split(',')[1];
            if(tbx.Enabled)
            {
                var _Provider = new CardOrderPacket();

                _Provider = _Provider.Get(Convert.ToInt32(lbPId.Text));

                _Provider.NumberCard = int.Parse(tbx.Text);
                _Provider.CardValue = Convert.ToInt32(tqt.SelectedValue);
                _Provider.CardType = cbx.SelectedValue;
                if (string.IsNullOrEmpty(_Provider.Data))
                    _Provider.Data = "";
                _Provider.Update();
            }    
           
        }
        BindData();
        //Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderList + "?Type=" + _Type);
    }
    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());

        var order = new CardOrderPacket { Id = Id };
        order.Delete();
        BindData();
    }
    protected void Confirm_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());
        
        var _Provider = new CardOrderPacket();
        _Provider = _Provider.Get(Id);
        if (_Provider.Status == 1)
            return;
        //gọi api sang get thẻ
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string requestContent = serializer.Serialize(new BuyCardRequest()
        {
            
            AccountName = PartnerCode,
            Amount = _Provider.CardValue,
            Quantity = _Provider.NumberCard,
            Provider = _Provider.CardType,
            OrderNo = _Provider.OrderNo+"_" +DateTime.Now.ToString("yyyyMMddHHmmss")
           
        });
        var partner = new Partners().Get(PartnerCode);
        var signature = Encrypts.MD5(PartnerCode + "buycard" + "buycard" + requestContent + partner.PublicKey);
        var requestData = new RequestData()
        {
            PartnerCode = PartnerCode,
            CommandCode = "buycard",
            RequestContent = requestContent,
            ServiceCode = "buycard",
            Signature = signature
        };
        NLogLogger.Info(new string[] { "Card Buy", "Response Core", serializer.Serialize(requestData) });
        var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
        NLogLogger.Info(new string[] { "Card Buy", "Response Core", serviceResponse });

        var resPonse = serializer.Deserialize<APIResponse>(serviceResponse);
        if(resPonse.ResponseCode==1)
        {
            _Provider.Status = 1;
            _Provider.Data = resPonse.ResponseContent;
            _Provider.Update();
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "<script> alert('Mua thẻ thành công')</script>");
        }
        else
        {
            //Response.Write(String.Format("<script>Lỗi mua thẻ {0}</script>", resPonse.ResponseCode));
            //báo lỗi
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", String.Format("<script> alert('Lỗi mua thẻ {0}')</script>", resPonse.ResponseCode));
        }

        BindData();
    }
    protected void Download_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());
        var _Provider = new CardOrderPacket();
        _Provider = _Provider.Get(Id);
        //gọi api sang get thẻ
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var lstcard = serializer.Deserialize<List<CardSeri>>(_Provider.Data);
        var lstData = new List<CardSeriExcel>();
        foreach(var item in lstcard)
        {
            var newitem = new CardSeriExcel();
            newitem.Serial = "'" + item.Serial;
            newitem.Pin = "'" + item.Pin;
            newitem.ExpireDate = item.ExpireDate.ToString("dd/MM/yyyy");
            lstData.Add(newitem);
        }    
        ExportToExcel(lstData, _Provider.OrderNo+"_" + _Provider.CardType+"_" + (_Provider.CardValue/1000));
        //BindData();
    }
    protected void ExportToExcel(List<CardSeriExcel> data, string name)
    {
        Response.Clear();
        Response.Buffer = true;
        //Response.Charset = "UTF-8"; 
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + name + ".xls");
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
        Response.ContentType = "application/ms-excel";
        EnableViewState = false;
        var myCItrad = new CultureInfo("VI-VN", true);
        var oStringWriter = new StringWriter(myCItrad);
        var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);


        var grid = new DataGrid { DataSource = data };
        grid.DataBind();
        grid.RenderControl(oHtmlTextWriter);

        Response.Write(oStringWriter.ToString());
        Response.Flush();
        Response.End();
    }
    public string GetStatus(object str)
    {
        if (str.ToString() == "1")
            return "Hoàn thành";

        return "Chưa hoàn thành";
    }
    public class BuyCardRequest
    {
        public string Provider { get; set; } // CardType
        public int Amount { get; set; }
        public int Quantity { get; set; }
        public string AccountName { get; set; }
        public long AccountId { get; set; }
        public string OrderNo { get; set; }
    }
    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }

    }
    private string PostJson(string uri, string postData)
    {
        var request = (HttpWebRequest)WebRequest.Create(uri);
        request.ContentType = "application/json";
        request.Method = "POST";//GET
                                //request.Accept = "JSON";
        using (Stream requestStream = request.GetRequestStream())
        {
            byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
            requestStream.Write(postDatabytes, 0, postDatabytes.Length);
        }
        var webResponse = request.GetResponse();
        if (webResponse == null)
        {
            return "Unable to connect to the remote server";
        }
        var sr = new StreamReader(webResponse.GetResponseStream());
        return sr.ReadToEnd().Trim();
    }
    public class CardSeri
    {
        public string Serial { get; set; }
        public string Pin { get; set; }
        public DateTime ExpireDate { get; set; }
    }
    public class CardSeriExcel
    {
        public string Serial { get; set; }
        public string Pin { get; set; }
        public string ExpireDate { get; set; }
    }
}
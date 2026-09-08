using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Libs.Report;
using System.ComponentModel;

public partial class Pages_CardStore_Packet_Monitor : System.Web.UI.Page
{
    int PageSize = 20;
    public Pager pages;
    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize)
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - 3;
            var endPage = currentPage + 2;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 6)
                {
                    startPage = endPage - 5;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PacketMonitor);

        if (!IsPostBack)
        {
            if (AppUtils.Request("page") > 0)
            {
                BindDataPageWise(Convert.ToInt32(AppUtils.Request("page")));
            }
            else
            {
                BindDataPageWise(1);
            }

        }
    }

    protected void BindData()
    {
        DataView dv1 = new ProvidersStore().GetTable().DefaultView;
        dv1.Sort = "OrderNo asc";
        dv1.RowFilter = "Type = 1";
        DataTable sortedDT1 = dv1.ToTable();
        rptPacketList.DataSource = sortedDT1;
        rptPacketList.DataBind();
    }

    protected void Page_Changed(object sender, EventArgs e)
    {
        int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
        //(sender as LinkButton).Enabled= false;
        this.BindDataPageWise(pageIndex);
    }

    protected void BindDataPageWise(int pageIndex)
    {
        int totalRecord;
        var lstData = new Packet().GetListMonitorPage(null, null, null, null, null, pageIndex, PageSize, out totalRecord);
        rptPacketList.DataSource = AppUtils.ToDataTable(lstData);
        rptPacketList.DataBind();
        pages = new Pager(totalRecord, pageIndex, PageSize);
    }
   
    

    public string Getactive(int page1, int page2)
    {
        if (page1 == page2)
            return "active";
        else
            return "";
    }
    public string GetStatus(object str)
    {
        if (str.ToString() == "1")
            return "Ngày";
        else if (str.ToString() == "2")
            return "Tuần";
        else if (str.ToString() == "3")
            return "Tháng";
        else if (str.ToString() == "4")
            return "Giờ";
        return "";
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        BindDataPageWise(1);
    }
}
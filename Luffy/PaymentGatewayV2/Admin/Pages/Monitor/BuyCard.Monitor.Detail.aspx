<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BuyCard.Monitor.Detail.aspx.cs" Inherits="Pages_Monitor_BuyCard_Monitor_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Mua thẻ & Topup 
            <small>
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BuyCardMonitor %>">Xem log giao dịch nạp thẻ</a>
            </small>
        </h1>
    </section>
    <section class="content">
        <div class="box box-solid">
            <div class="box-header with-border">
                <i class="fa fa-text-width"></i>
                <h3 class="box-title">
                    <asp:Label ID="lblTtitle" runat="server" Text="Chi tiết thông tin giao dịch" CssClass="title"></asp:Label></h3>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <dl class="dl-horizontal">
                    <dt>TransactionID</dt><dd><asp:Label ID="lblTransactionID" runat="server" Text=""></asp:Label></dd> 
                    <dt>PartnerCode</dt><dd><asp:Label ID="lblPartnerCode" runat="server" Text=""></asp:Label></dd> 
                    <dt>OrderNo</dt><dd><asp:Label ID="lblOrderNo" runat="server" Text=""></asp:Label></dd> 
                    <dt>AccountName</dt><dd><asp:Label ID="lblAccountName" runat="server" Text=""></asp:Label></dd> 
                    <dt>CardType</dt><dd><asp:Label ID="lblCardType" runat="server" Text=""></asp:Label></dd> 
                    <dt>Amount</dt><dd><asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></dd> 
                    <dt>Quantity</dt><dd><asp:Label ID="lblQuantity" runat="server" Text=""></asp:Label></dd> 
                    <% if (AppUtils.IsAdmin)
                       { %>
                    <dt>Provider</dt><dd><asp:Label ID="lblProvider" runat="server" Text=""></asp:Label></dd> 
                    <% } %>
                    <dt>CreatedTime</dt><dd><asp:Label ID="lblCreatedTime" runat="server" Text=""></asp:Label></dd> 
                    <dt>ListCards</dt><dd><asp:Label ID="lblListCards" runat="server" Text=""></asp:Label></dd> 
                    <dt>LogContent</dt><dd><asp:Label ID="lblLogContent" runat="server" Text=""></asp:Label></dd> 
                    <dt>Status</dt><dd><asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></dd>   
                </dl>
            </div>
            <!-- /.box-body -->
        </div>
    </section>

</asp:Content>


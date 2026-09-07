<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardAPI.Monitor.Detail.aspx.cs" Inherits="Pages_Monitor_CardAPI_Monitor_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Thẻ cào 
            <small>
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.CardAPIMonitor %>">Xem log giao dịch nạp thẻ</a>
            </small>
        </h1>
        <ol class="breadcrumb">
                <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
                <li class="active">Xem chi tiết</li>
        </ol>
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
                    <dt>TransactionID</dt>
                    <dd>
                        <asp:Label ID="lblTransactionID" runat="server" Text=""></asp:Label></dd>
                    <dt>RefCode</dt>
                    <dd>
                        <asp:Label ID="lblRefCode" runat="server" Text=""></asp:Label></dd>
                    <dt>CardSerial</dt>
                    <dd>
                        <asp:Label ID="lblCardSerial" runat="server" Text=""></asp:Label></dd>
                    <dt>CardCode</dt>
                    <dd>
                        <asp:Label ID="lblCardCode" runat="server" Text=""></asp:Label></dd>
                    <dt>AccountName</dt>
                    <dd>
                        <asp:Label ID="lblAccountName" runat="server" Text=""></asp:Label></dd>
                    <dt>Amount</dt>
                    <dd>
                        <asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></dd>
                    <dt>CardType</dt> <dd>
                        <asp:Label ID="lblCardType" runat="server" Text=""></asp:Label></dd>
                        <% if (AppUtils.IsAdmin)
                        {%>
                    <dt>Provider</dt>
                    <dd><asp:Label ID="lblProvider" runat="server" Text=""></asp:Label></dd>
                        <%}%>
                    <dt>CreatTime</dt>
                    <dd>
                        <asp:Label ID="lblCreatTime" runat="server" Text=""></asp:Label></dd>
                     <dt>LastTime</dt>
                    <dd>
                        <asp:Label ID="lblLastTime" runat="server" Text=""></asp:Label></dd>
                    <dt>Status</dt>
                    <dd>
                        <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></dd>
                    <dt>CallbackUrl</dt>
                    <dd>
                        <asp:Label ID="lblCallbackUrl" runat="server" Text=""></asp:Label></dd>
                    <dt>Log</dt>
                    <dd><asp:Label ID="txtLog"  runat="server"></asp:Label></dd>
                     <% if (AppUtils.IsAdmin)
                        {%>
                    <dt>----------</dt>
                    <dd>-------------------------</dd>
                    <dt><asp:LinkButton ID="txtRecheck" runat="server" OnClick="txtRecheck_Click" Visible="False">Kiểm tra lại &gt;&gt;</asp:LinkButton></dt>
                    <dd><asp:Label ID="lblRecheck"  runat="server"></asp:Label></dd>
                    <%}%>
                    <dt></dt>
                    <dd></dd>

                </dl>
            </div>
            <!-- /.box-body -->
        </div>
    </section>

</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.Monitor.Detail.aspx.cs" Inherits="Pages_Topup_Monitor_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Topup
            <small>
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.CardAPIMonitor %>">Xem Chi Tiết logs</a>
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
                    <dt>Id</dt>
                    <dd>
                        <asp:Label ID="lblId" runat="server" Text=""></asp:Label></dd>
                    <dt>RequesNo</dt>
                    <dd>
                        <asp:Label ID="lblRequestNo" runat="server" Text=""></asp:Label></dd>
                    <dt>TransactionID</dt>
                    <dd>
                        <asp:Label ID="lblTransactionID" runat="server" Text=""></asp:Label></dd>
                    <dt>CardSerial</dt>
                    <dd>
                        <asp:Label ID="lblCardSerial" runat="server" Text=""></asp:Label></dd>
                    <dt>CardCode</dt>
                    <dd>
                        <asp:Label ID="lblCardCode" runat="server" Text=""></asp:Label></dd>
                    <% if (AppUtils.IsAdmin)
                        { %>
                    <dt>PartnerCode</dt>
                    <dd>
                        <asp:Label ID="lblPartnerCode" runat="server" Text=""></asp:Label></dd>
                    <dt>ProviderCode</dt>
                    <dd>
                        <asp:Label ID="lblProviderCode" runat="server" Text=""></asp:Label></dd>
                    <% } %>
                    <dt>Telco</dt>
                    <dd>
                        <asp:Label ID="lblTelco" runat="server" Text=""></asp:Label></dd>
                    <dt>Mobile</dt>
                    <dd>
                        <asp:Label ID="lblMobile" runat="server" Text=""></asp:Label></dd>
                    <dt>MobileTarget</dt>
                    <dd>
                        <asp:Label ID="lblMobileTarget" runat="server" Text=""></asp:Label></dd>
                    <dt>Amount</dt>
                    <dd>
                        <asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></dd>
                    <dt>AmountUser</dt>
                    <dd>
                        <asp:Label ID="lblAmountUser" runat="server" Text=""></asp:Label></dd>
                    <dt>CreatedTime</dt>
                    <dd>
                        <asp:Label ID="lblCreatedTime" runat="server" Text=""></asp:Label></dd>
                    <dt>LastTime</dt>
                    <dd>
                        <asp:Label ID="lblLastTime" runat="server" Text=""></asp:Label></dd>
                    <dt>Status</dt>
                    <dd>
                        <asp:Label ID="lblStatus" runat="server"></asp:Label>
                        <asp:HiddenField ID="hdStatus" runat="server"></asp:HiddenField></dd>
                    <dt>LogContent</dt>
                    <dd>
                        <asp:Label ID="lblLogContent" runat="server" Text=""></asp:Label></dd>
                    <% if (AppUtils.IsAdmin || AppUtils.IsTopup)
                       {%>
                        <dt>----------</dt>
                        <dd>-------------------------</dd>
                        <dt><asp:LinkButton ID="txtRecheck" runat="server" OnClick="txtRecheck_Click" Visible="False">Kiểm tra mã thẻ &gt;&gt;</asp:LinkButton></dt>
                        <dd><asp:Label ID="lblRecheck" runat="server"></asp:Label></dd>
                        <dt>&nbsp;</dt>
                        <dd>&nbsp;</dd>
                        <dt><asp:HiddenField ID="hdAmount" runat="server"></asp:HiddenField>
                            <asp:TextBox ID="txtAmount" runat="server" Visible="False" CssClass="form-control" placeholder="Amount"></asp:TextBox></dt>
                        <dd>
                            <asp:Button ID="btnActionSuccess" runat="server" Text="Thành công" class="btn btn-danger" OnClick="btnActionSuccess_Click" OnClientClick="return confirm('Bạn có chắc thực hiện hành động này?')" Visible="False" style="margin-right: 10px;"/> 
                            <asp:Button ID="btnAcctionFailed" runat="server" Text="Thất bại" class="btn btn-warning" OnClick="btnAcctionFailed_Click" OnClientClick="return confirm('Bạn có chắc thực hiện hành động này?')" Visible="False"/> 
                        </dd>
                    <%}%>
                    
                    <% if (AppUtils.IsAdmin)
                       {%>
                        <dd>
                            <asp:LinkButton ID="btnCallbackProvider" runat="server"  OnClick="txtCallBackProvider_Click">ReCallback ĐL</asp:LinkButton>
                        </dd>
                    <%}%>
                    <dt></dt>
                    <dd></dd>
                </dl>
            </div>
            <!-- /.box-body -->
        </div>
    </section>

</asp:Content>


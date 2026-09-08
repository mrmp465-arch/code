<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankCash.Monitor.Detail.aspx.cs" Inherits="Pages_Monitor_BankCash_Monitor_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Ngân hàng -Cash
            <small>Chi tiết thông tin giao dịch</small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Log chi tiết</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Chi tiết thông tin giao dịch</h3>
                    </div>
                    <!-- /.box-header -->
                    <!-- /.box-header -->
                    <div class="box-body">
                        <dl class="dl-horizontal">
                            <dt>Id</dt>
                            <dd>
                                <asp:Label ID="lblTransactionID" runat="server" Text=""></asp:Label></dd>
                            
                            <dd style="display:none">
                                <asp:Label ID="lblOrderNo" runat="server" Text=""></asp:Label>

                            </dd>
                            <%-- <dt>OrderInfo</dt>
                            <dd>
                                <asp:Label ID="lblOrderInfo" runat="server" Text=""></asp:Label>

                            </dd>--%>
                            <dt>RefCode</dt>
                            <dd>
                                <asp:Label ID="lblRefCode" runat="server" Text=""></asp:Label>

                            </dd>
                            <dt>Ngân hàng</dt>
                            <dd>
                                <asp:Label ID="lblBankCode" runat="server" Text=""></asp:Label></dd>
                            <dt>Tên tk</dt>
                            <dd>
                                <asp:Label ID="lblBankAccountName" runat="server" Text=""></asp:Label></dd>
                            <dt>Số tk</dt>
                            <dd>
                                <asp:Label ID="lblBankAccountNumber" runat="server" Text=""></asp:Label></dd>
                           
                           <dd style="display:none">
                                <asp:Label ID="lblAccountName" runat="server" Text=""></asp:Label></dd>
                            <%--<dt>AmountUser</dt>
                               <dd>
                                <asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></dd>--%>

                             <dt>Số tiền</dt>
                               <dd>
                                <asp:Label ID="lblTotalAmount" runat="server" Text=""></asp:Label></dd>

                              
                         
                            <% if (AppUtils.IsAdmin)
                                {%>
                            <dt>Provider</dt>
                            <dd>
                                <asp:Label ID="lblProvider" runat="server" Text=""></asp:Label></dd>
                            <%}%>
                            <dt>Thời gian</dt>
                            <dd>
                                <asp:Label ID="lblCreatTime" runat="server" Text=""></asp:Label></dd>
                            <dt>Cập nhật</dt>
                            <dd>
                                <asp:Label ID="lblLastTime" runat="server" Text=""></asp:Label></dd>
                            <dt>Trạng thái</dt>
                            <dd>
                                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></dd>
                           
                            <dd style="display:none">
                                <asp:Label ID="lblCallbackUrl" runat="server" Text=""></asp:Label></dd>
                            <dt>Log</dt>
                            <dd>
                                <asp:Label ID="txtLog" runat="server"></asp:Label></dd>
                            <% if (AppUtils.IsAdmin)
                                {%>
                            <dt>----------</dt>
                            <dd>-------------------------</dd>
                            <dt>
                                <asp:LinkButton ID="txtRecheck" runat="server" OnClick="txtRecheck_Click" Visible="False">Kiểm tra lại &gt;&gt;</asp:LinkButton></dt>
                            <dd>
                                <asp:Label ID="lblRecheck" runat="server"></asp:Label></dd>
                            <%}%>
                            <dt></dt>
                            <dd></dd>

                        </dl>
                    </div>
                    <!-- /.box-body -->
                    <!-- /.box-body -->
                </div>
                <!-- /.box -->
            </div>
            <!-- /.col -->
        </div>
        <!-- /.row -->
    </section>
    <!-- /.content -->



</asp:Content>

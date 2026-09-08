<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankGateAPI.Monitor.Detail.aspx.cs" Inherits="Pages_Monitor_BankGateAPI_Monitor_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Ngân hàng
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
                            <dt>TransactionID</dt>
                            <dd>
                                <asp:Label ID="lblTransactionID" runat="server" Text=""></asp:Label></dd>
                             <dt>OrderNo</dt>
                            <dd>
                                <asp:Label ID="lblOrderNo" runat="server" Text=""></asp:Label>

                            </dd>
                             <dt>OrderInfo</dt>
                            <dd>
                                <asp:Label ID="lblOrderInfo" runat="server" Text=""></asp:Label>

                            </dd>
                            <dt>RefCode</dt>
                            <dd>
                                <asp:Label ID="lblRefCode" runat="server" Text=""></asp:Label>

                            </dd>
                            <dt>BankCode</dt>
                            <dd>
                                <asp:Label ID="lblBankCode" runat="server" Text=""></asp:Label></dd>
                            <dt>BankAccountName</dt>
                            <dd>
                                <asp:Label ID="lblBankAccountName" runat="server" Text=""></asp:Label></dd>
                            <dt>BankAccountNumber</dt>
                            <dd>
                                <asp:Label ID="lblBankAccountNumber" runat="server" Text=""></asp:Label></dd>
                            <dt>AccountName</dt>
                            <dd>
                                <asp:Label ID="lblAccountName" runat="server" Text=""></asp:Label></dd>
                            <dt>AmountUser</dt>
                               <dd>
                                <asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></dd>

                             <dt>Amount</dt>
                               <dd>
                                <asp:Label ID="lblTotalAmount" runat="server" Text=""></asp:Label></dd>

                              <dt>Mobile</dt>
                            <dd>
                                <asp:Label ID="lblMobile" runat="server" Text=""></asp:Label>

                            </dd>

                               <dt>Email</dt>
                            <dd>
                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>

                            </dd>
                         
                            <% if (AppUtils.IsAdmin)
                                {%>
                            <dt>Provider</dt>
                            <dd>
                                <asp:Label ID="lblProvider" runat="server" Text=""></asp:Label></dd>
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
								 <dt>Json</dt>
                            <dd>
                                <asp:Label ID="lblCallbackdata" runat="server" Text=""></asp:Label></dd>
                           
                            <dt>CallbackUrl</dt>
                            <dd>
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

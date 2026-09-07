<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankCash.Monitor.Detail.aspx.cs" Inherits="Pages_Monitor_BankCash_Monitor_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1><%= Resources.Pay.Cash%>
            <small>Chi tiết thông tin giao dịch</small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active"><%= Resources.Pay.ViewLog%></li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title"><%= Resources.Pay.ViewLog%></h3>
                    </div>
                    <!-- /.box-header -->
                    <!-- /.box-header -->
                    <div class="box-body">
                        <dl class="dl-horizontal">
                            <dt>TransactionID</dt>
                            <dd>
                                <asp:Label ID="lblTransactionID" runat="server" Text=""></asp:Label></dd>
                             <dt>OrderInfo</dt>
                            <dd>
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
                            <dt>BankCode</dt>
                            <dd>
                                <asp:Label ID="lblBankCode" runat="server" Text=""></asp:Label></dd>
                            <dt>BankAccountName</dt>
                            <dd>
                                <asp:Label ID="lblBankAccountName" runat="server" Text=""></asp:Label></dd>
                            <dt>BankAccountNumber</dt>
                            <dd>
                                <asp:Label ID="lblBankAccountNumber" runat="server" Text=""></asp:Label></dd>
                            <dt>Nội dung</dt>
                            <dd>
                                <asp:Label ID="lblAccountName" runat="server" Text=""></asp:Label></dd>
                            <%--<dt>AmountUser</dt>
                               <dd>
                                <asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></dd>--%>

                             <dt>Amount</dt>
                               <dd>
                                <asp:Label ID="lblTotalAmount" runat="server" Text=""></asp:Label></dd>

                              
                         
                            <% if (AppUtils.IsAdmin)
                                {%>
                            <dt style="display:none">Provider</dt>
                            <dd style="display:none">
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
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Log callback</h3>
                    </div>
                    <!-- /.box-header -->
                    <!-- /.box-header -->
                   <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-bordered" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>Request</th>
                                    <th>Respone</th>
                                    <th>Time</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td style="word-break:break-all;""><%#Eval("Request")%></td>
                                            
                                           
                                           <td style="word-break:break-all;"><%# DecodeFromUtf8(Eval("Respone").ToString()) %></td>
                                            <td><%#Eval("LogTime", "{0:dd/MM HH:mm:ss}") %></td>
                                          

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <div style="text-align:right;margin:15px 15px;">
                            <% if ((AppUtils.IsAdmin || AppUtils.IsPartner))
                                { %>
                            <asp:LinkButton ID="lnCallback" runat="server" OnClick="lnCallback_Click" Visible="false" CssClass="btn btn-info">Callback </asp:LinkButton>
                            <% } %>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- /.row -->
    </section>
    <!-- /.content -->



</asp:Content>

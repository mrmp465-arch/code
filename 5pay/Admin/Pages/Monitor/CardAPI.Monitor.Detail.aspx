<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardAPI.Monitor.Detail.aspx.cs" Inherits="Pages_Monitor_CardAPI_Monitor_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1><%= Resources.Pay.Card%>
            <small>
               <%= Resources.Pay.LogDetail%>
            </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active"><%= Resources.Pay.LogDetail%></li>
        </ol>
    </section>
    <section class="content">
        <div class="row">
            
      <div class="col-xs-12">
          <div class="box box-solid">
              <div class="box-header with-border">
                  <i class="fa fa-text-width"></i>
                  <h3 class="box-title">
                      <%= Resources.Pay.LogDetail%></h3>
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
                      <dt>CardType</dt>
                      <dd>
                          <asp:Label ID="lblCardType" runat="server" Text=""></asp:Label></dd>
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
          </div>
      </div>
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
                                            <td style="word-break: break-all;"><%#Eval("Request")%></td>


                                            <td style="word-break: break-all;"><%# DecodeFromUtf8(Eval("Respone").ToString()) %></td>
                                            <td><%#Eval("LogTime", "{0:dd/MM HH:mm:ss}") %></td>


                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>


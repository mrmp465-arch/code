<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankGateAPI.Search.aspx.cs" Inherits="Pages_Monitor_BankGateAPI_Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <span id="AlertInfos"></span>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1>Ngân hàng
            <small>Tra cứu giao dịch </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Tra cứu</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Tra cứu giao dịch</h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-1">
                            <asp:DropDownList CssClass="form-control" ID="drpTop" runat="server">
                                <asp:ListItem Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-1">
                            <div class="input-group">
                                <span class="input-group-addon">ID</span>
                                <asp:TextBox CssClass="form-control" ID="txtTransactionID" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">RefCode</span>
                                <asp:TextBox CssClass="form-control" ID="txtRefCode" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">OrderNo</span>
                                <asp:TextBox CssClass="form-control" ID="txtOrderNo" runat="server"></asp:TextBox>
                            </div>
                        </div>
                         <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">OrderInfo</span>
                                <asp:TextBox CssClass="form-control" ID="txtOrderInfo" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">Mobile</span>
                                <asp:TextBox CssClass="form-control" ID="txtMobile" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">Email</span>
                                <asp:TextBox CssClass="form-control" ID="txtEmail" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <asp:DropDownList CssClass="form-control" ID="drpBankCode" runat="server">
                            </asp:DropDownList>
                        </div>
                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                        </div>
                        <% } %>

                        <% if (AppUtils.IsAdmin || AppUtils.IsProvider)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpProvider" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                            <div class="input-group">
                                <span class="input-group-addon">CreatTime </span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtCreatTime" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                   
                                    <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                                        { %>
                                    <th>Partner</th>
                                    <% } %>
                                     <th>AccountName</th>
                                    <th>RefCode</th>
                                     <th>BankCode</th>
                                    <th>AmountUser</th>
                                    <th>Amount</th>
                                    <th>CreatedTime</th>
                                    

                                    <% if (AppUtils.IsAdmin || AppUtils.IsProvider)
                                        {%>
                                    <th>Provider</th>
                                    <%}%>
                                    <th>OrderNo</th>
                                    <th>OrderInfo</th>
                                    <th>Status</th>
                                    <th>Log</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("TransactionID")%></td>
                                            
                                            <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                                                { %>
                                            <td><%#Eval("PartnerCode")%></td>
                                            <% } %>
                                            <td><%#Eval("FullName") %></td>
                                            <td><%#Eval("RefCode") %></td>
                                            <td><%#Eval("BankCode") %></td>
                                            <%--<td><%#Convert.ToDecimal(Eval("Amount")).ToString("#,#").Replace(",", ".") %></td>
                                            <td><%#Convert.ToDecimal(Eval("TotalAmount")).ToString("#,#").Replace(",", ".") %></td>--%>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalAmount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}") %></td>
                                            <%--<td><%#Eval("LastTime", "{0:dd/MM HH:mm:ss}") %></td>--%>

                                            <% if (AppUtils.IsAdmin || AppUtils.IsProvider)
                                                {%>
                                            <td><%#Eval("ProviderCode") %></td>
                                            <%}%>
                                            <td><%#Eval("OrderNo") %></td>
                                             <td><%#Eval("OrderInfo") %></td>
                                            <td><%#Eval("Status") %></td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH + Resources.Url.BankGateAPIMonitorDetail%>?id=<%#Eval("TransactionID") %>">xem</a>
                                                 <% if ((AppUtils.IsAdmin || AppUtils.IsPartner) )
                                                    { %>
                                                <asp:Label runat="server" Visible='<%# Eval("PartnerCode").ToString() != "zab" && Eval("PartnerCode").ToString() != "b53" %>'>&nbsp;&nbsp;<a href="<%=Constant.ADMIN_PATH + "pages/monitor/BankGateAPI.FixMomo.aspx"%>?id=<%#Eval("TransactionID") %>">Sửa </a></asp:Label>
                                                <% } %>

                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('.txtCreatTime').datepicker({
                autoclose: true,
                Accepts: 'bottom'
            });
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

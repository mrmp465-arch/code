<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankGateAPI.Monitor.aspx.cs" Inherits="Pages_Monitor_BankGateAPI_Monitor" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1><%= Resources.Pay.Deposit%>
            <small><%= Resources.Pay.ViewLog%> </small>
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
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList CssClass="form-control" ID="drpTop" runat="server">
                                <asp:ListItem Text="50 row" Value="50" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="100 row" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 row" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 row" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-1">
                            <asp:TextBox CssClass="form-control" ID="txtTransactionID" placeholder="ID" runat="server"></asp:TextBox>
                        </div>
                        <% } %>


                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner || AppUtils.IsTopup)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>
                        <% } %>

                        <% if (AppUtils.IsAdmin || AppUtils.IsProvider)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2" style="display: none">
                            <asp:DropDownList ID="drpProvider" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <%--<div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control" ID="drpService" runat="server">
                            </asp:DropDownList>
                        </div>--%>
                        <div class="col-xs-12 col-sm-6 col-md-1">

                            <asp:DropDownList CssClass="form-control" ID="drpStatus" runat="server">
                                <%-- <asp:ListItem Text="Trạng thái" Value="-99" Selected="True"></asp:ListItem>
         <asp:ListItem Text="Thành công" Value="1"></asp:ListItem>
         <asp:ListItem Text="Đang xử lý" Value="0"></asp:ListItem>
         <asp:ListItem Text="Thất bại" Value="-1"></asp:ListItem>--%>
                            </asp:DropDownList>

                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-1">
                            <asp:DropDownList CssClass="form-control select2" ID="drpBankCode" runat="server">
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.RefCode%></span>
                                <asp:TextBox CssClass="form-control" ID="txtRefCode" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">Code</span>
                                <asp:TextBox CssClass="form-control" ID="txtOrderNo" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.OrderInfo%></span>
                                <asp:TextBox CssClass="form-control" ID="txtOrderInfo" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.DepositAmount%></span>
                                <asp:TextBox CssClass="form-control" ID="txtAmount" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.AccountReceive%></span>
                                <asp:TextBox CssClass="form-control" ID="txtBankId" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px"><%= Resources.Pay.From%></span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtFromDate" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px"><%= Resources.Pay.To%></span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtCreatTime" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.ReportTime%></span>
                                <asp:DropDownList CssClass="form-control" ID="drpType" runat="server">

                                    <asp:ListItem Text="Lastime" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="CreateTime" Value="2"></asp:ListItem>

                                </asp:DropDownList>
                            </div>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text=""></asp:Button>
                            <asp:Button ID="btExcel" runat="server" CssClass="btn btn-info" OnClick="ExportTran2_Click" Text="Export Excel"></asp:Button>
                            <asp:Button ID="btCallbankk" runat="server" CssClass="btn btn-info btcallback" OnClick="Callbackall_Click" OnClientClick="return confirm('Bạn có muốn thực hiện?')" Text="CallbackAll"></asp:Button>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-4" style="font-weight: bold; font-size: 105%; padding-top: 6px; float: right">
                            <asp:Label runat="server" ID="lblTotal"></asp:Label>
                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>ID</th>

                                    <% if (AppUtils.IsAdmin || AppUtils.IsPartner || AppUtils.IsTopup)
                                        { %>
                                    <th><%= Resources.Pay.Partner%></th>
                                    <% } %>

                                    <th><%= Resources.Pay.RefCode%></th>
                                    <th><%= Resources.Pay.BankCode%></th>
                                    <th><%= Resources.Pay.AccountReceive%></th>
                                    <th><%= Resources.Pay.AmountUser%></th>
                                    <th><%= Resources.Pay.RealAmount%></th>
                                    <th><%= Resources.Pay.Fee%></th>
                                  <%--  <% if (AppUtils.UserName.Contains("admin"))
                                        { %>
                                    <th>Hoa hồng</th>
                                    <%}%>--%>
                                    <th><%= Resources.Pay.CreatedTime%></th>
                                    <th><%= Resources.Pay.LastTime%></th>


                                    <th>Code</th>
                                    <th><%= Resources.Pay.OrderInfo%></th>
                                    <% if (AppUtils.IsAdmin)
                                        {%>
                                    <th>Người duyệt</th>
                                    <%}%>
                                    <th><%= Resources.Pay.Status%></th>
                                    <th><%= Resources.Pay.Operation%></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("TransactionID")%></td>

                                            <% if (AppUtils.IsAdmin || AppUtils.IsPartner || AppUtils.IsTopup)
                                                { %>
                                            <td><%#Eval("PartnerCode")%></td>
                                            <% } %>

                                            <td><%#Eval("RefCode") %></td>

                                            <td><%#Eval("BankCode") %></td>
                                            <td><%#Eval("BankAccountNumber") %></td>
                                            <%--<td><%#Convert.ToDecimal(Eval("Amount")).ToString("#,#")..Replace(".", ",") %></td>
                                            <td><%#Convert.ToDecimal(Eval("TotalAmount")).ToString("#,#")..Replace(".", ",") %></td>--%>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(".", ",") %></td>
                                            <td style="color: red"><%#Convert.ToInt64(Eval("TotalAmount")).ToString("N0").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("Fee")).ToString("N0").Replace(".", ",") %></td>
                                            <%--<% if (AppUtils.UserName.Contains("admin"))
                                                { %>
                                            <td><%#Convert.ToInt64(Eval("Reward")).ToString("N0").Replace(".", ",") %></td>
                                            <% } %>--%>
                                            <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}") %></td>
                                            <td><%#Eval("LastTime", "{0:dd/MM HH:mm:ss}") %></td>
                                            <%-- <td><%#Eval("OrderNo") %></td>--%>

                                            <td><%#GetOrderNo(Eval("FullName"),Eval("Signature"),Eval("BankCode"),Eval("PartnerCode")) %></td>
                                            <td><%#Eval("OrderInfo") %></td>
                                            <td><%#GetSignature(Eval("Signature")) %></td>
                                            <td style="width: 80px"><%#GetStatusExtra(Eval("Status"),Eval("PartnerCode"),Eval("Amount"),Eval("TotalAmount")) %></td>

                                            <td style="font-size: 105%">
                                                <a href="<%=Constant.ADMIN_PATH + Resources.Url.BankGateAPIMonitorDetail%>?id=<%#Eval("TransactionID") %>">[<%= Resources.Pay.View%>] </a>

                                                <% if ((AppUtils.IsAdmin || AppUtils.IsPartner))
                                                    { %>
                                                |
                                                <asp:LinkButton ID="lnCallback" runat="server" OnCommand="Callback_Command" CommandName="Callback" CommandArgument='<%#Eval("TransactionID")%>' Visible='<%# Eval("Status").ToString() == "1" || Eval("Status").ToString() == "2" %>'> [Callback] </asp:LinkButton>


                                                <% } %>

                                                <% if ((AppUtils.IsAdmin && RoleFix))
                                                    { %>

                                                <asp:HyperLink NavigateUrl='<%# FixtUrl(Eval("TransactionID").ToString()) %>' Visible='<%# Eval("Status").ToString() == "0" %>' runat="server"> [Sửa đơn] </asp:HyperLink>
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
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=txtFromDate.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });
            $('#<%=txtCreatTime.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });

        });
        $(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true, "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>
    <style>
        .label {
            width: 76px !important;
            padding: 3px;
            display: block
        }

        .btcallback {
            display: none;
        }

        .txtCreatTime {
            width: 138px !important;
            padding: 3px !important;
        }
    </style>
</asp:Content>

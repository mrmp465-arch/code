<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankCash.Report.aspx.cs" Inherits="Pages_Monitor_BankCash_Report" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1><%= Resources.Pay.Cash%>
            <small><%= Resources.Pay.Report%> </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active"><%= Resources.Pay.Report%> </li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">

        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i><%= Resources.Pay.Report%> </li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab"><%= Resources.Pay.ReportTime%> </a></li>

                <%if (AppUtils.IsAdmin)
                    {%>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankCashReportBankCode %>">Theo BankCode</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankCashReportPartner %>">Theo đối tác</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankCashReportService %>">Theo nhà cung cấp</a></li>
                <% }%>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <asp:DropDownList CssClass="form-control" ID="drpBankCode" runat="server">
                            </asp:DropDownList>
                        </div>
                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <asp:DropDownList ID="drlType" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Tất cả" Value="0" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Tự động" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Duyệt tay" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2">
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
                        <div class="col-xs-12 col-sm-6 col-md-4">
                            <div class="input-group date-group " style="float: left; margin-right: 15px;">
                                <asp:DropDownList ID="drpYear" runat="server" OnSelectedIndexChanged="drpYear_SelectedIndexChanged" AutoPostBack="true" CssClass="input-group-addon"></asp:DropDownList>
                                <asp:DropDownList ID="drpMonth" runat="server" OnSelectedIndexChanged="drpMonth_SelectedIndexChanged" AutoPostBack="true" CssClass="input-group-addon"></asp:DropDownList>
                                <asp:DropDownList ID="drpDay" runat="server" CssClass="input-group-addon"></asp:DropDownList>
                            </div>
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                            <style type="text/css">
                                .date-group select {
                                    width: 80px;
                                }
                            </style>
                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <td style="width: 10px;">#</td>
                                    <th>Time</th>
                                    <th>TotalTransaction</th>
                                    <th>%</th>
                                    <th>TotalAmount</th>

                                    <th>%</th>


                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><%#Eval("Time")%></td>
                                            <td><%#Convert.ToInt32(Eval("TotalTransaction")).ToString("#,#").Replace(".", ",") %></td>

                                            <td><%# AppUtils.AmountToPercent(Eval("TotalTransaction").ToString(),lblTotalTransaction.Text) %></td>

                                            <td><%#Convert.ToInt64(Eval("TotalAmount")).ToString("#,#").Replace(".", ",") %></td>

                                           <td><%# AmountToPercent(Convert.ToInt64(Eval("TotalAmount")).ToString("#,#").Replace(",", "").Replace(".", ""),lblTotalAmount.Text.Replace(",", "").Replace(".", "")) %></td>
                                            


                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-info">
                        <div style="padding: 10px">
                            <%= Resources.Pay.CashOrderNumberSuccess%> : <b>
                                <asp:Label ID="lblTotalTransaction" runat="server" Text="0"></asp:Label></b>
                            , <%= Resources.Pay.CashAmount%> : <b>
                                <asp:Label ID="lblTotalAmount" runat="server" Text="0"></asp:Label></b>
                        </div>
                    </div>
                    <!-- /.box-body -->
                </div>

            </div>
        </div>

        <!-- /.row -->
    </section>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript">
        $(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>
</asp:Content>

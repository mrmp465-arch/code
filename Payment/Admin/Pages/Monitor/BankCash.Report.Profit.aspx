<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="BankCash.Report.Profit.aspx.cs" Inherits="Pages_Monitor_BankCash_Report_Profit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Rút Bank
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Báo cáo doanh số" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Báo cáo</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Báo cáo doanh số</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Theo thời gian</a></li>
                <%--<li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReportCardType %>">Theo loại thẻ</a></li>
                <%if (AppUtils.IsAdmin)
                    {%>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReportPartner %>">Theo đối tác</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReportProvider %>">Theo nhà cung cấp</a></li>

                <% }%>--%>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">

                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.From%> </span>
                                <asp:TextBox ID="txtBeginTime1" runat="server" CssClass="form-control txtBeginTime"></asp:TextBox>
                            </div>

                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.To%></span>
                                <asp:TextBox ID="txtEndTime1" runat="server" CssClass="form-control txtEndTime"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">


                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>


                        </div>
                    </div>
                    <div class="box-body no-padding" style="clear: both;">
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <td style="width: 10px;">#</td>
                                    <th>Ngày</th>
                                    <th>Số lệnh rút</th>
                                    <th>Số tiền rút</th>
                                    <th>Phí rút</th>
                                    <th>Hoa hồng rút</th>
                                   <th>Lợi nhuận rút</th>
                                    <th>Fit %</th>


                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><%#Eval("Time")%></td>
                                            <td><%#Eval("TotalTransaction")%></td>
                                            <td><%#Convert.ToInt64(Eval("TotalAmount")).ToString("#,#").Replace(".", ",") %> </td>
                                            <td><%#Convert.ToInt64(Eval("Fee")).ToString("#,#").Replace(".", ",")%> </td>
                                             <td><%#Convert.ToInt64(Eval("Reward")).ToString("#,#").Replace(".", ",")%> </td>
                                            <td><%#Convert.ToInt64(Eval("Profit")).ToString("#,#").Replace(".", ",")%> </td>
                                            <td><%# AppUtils.AmountToPercentFit(Convert.ToInt64(Eval("TotalAmount")).ToString(),Eval("Profit").ToString()) %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr style="font-weight: bold">
                                    <td colspan="2">Tổng</td>
                                    <td>

                                        <asp:Label ID="lblTotalTransaction" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td>

                                        <asp:Label ID="lblTotalAmount" runat="server" Text=""></asp:Label>
                                    </td>

                                    <td>

                                        <asp:Label ID="lblFee" runat="server" Text=""></asp:Label>
                                    </td>
                                      <td>

      <asp:Label ID="lblReward" runat="server" Text=""></asp:Label>
  </td>
                                    <td>

                                        <asp:Label ID="lblProfit" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td>

                                        <asp:Label ID="lblFit" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                </div>
                <div class="chart tab-pane " id="sales-chart">
                </div>
            </div>
        </div>
    </section>
    <!-- /.content -->
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.txtBeginTime').datepicker({
                autoclose: true,
                Accepts: 'bottom'
            });
            $('.txtEndTime').datepicker({
                autoclose: true,
                horizontal: 'right',
                vertical: 'bottom'

            });

        });
        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>
</asp:Content>

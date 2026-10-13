<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report.aspx.cs" MasterPageFile="~/Layout/Layout.master" Inherits="Pages_BankEWalletService_Bank_Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Bank
            <small>Báo cáo giao dịch </small>
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
                <li class="pull-left header"><i class="fa fa-inbox"></i>Báo Cáo</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Theo thời gian</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankReportDaily %>">Theo khoảng thời gian</a></li>

            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">BankId</span>
                                <asp:TextBox ID="txtBankId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
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
                        <div class="table-responsive">
                            <div id="dgrid" class="dataTables_wrapper form-inline" role="grid">
                                <table class="table table-striped" id="TableResponsive">
                                    <thead>
                                        <tr>
                                            <td style="width: 10px;">#</td>
                                            <th>Time</th>
                                            <th>In</th>
                                            <th>Out</th>

                                            <th>Tranfer </th>


                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptList" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td></td>
                                                    <td><%#Eval("Time")%></td>
                                                    <td><%#Convert.ToInt64(Eval("TotalAmountIn")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalIn")) %>)</td>
                                                    <td><%#Convert.ToInt64(Eval("TotalAmountOut")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalOut")) %>)</td>

                                                    <td><%#Convert.ToInt64(Eval("TotalAmountTranfer")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalTranfer")) %>)</td>

                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <tr style="font-weight: bold">
                                            <td colspan="2">Tổng</td>
                                            <td>

                                                <asp:Label ID="lblTotalIn" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblTotalOut" runat="server" Text=""></asp:Label>
                                            </td>

                                            <td>

                                                <asp:Label ID="lblTotalTranfer" runat="server" Text=""></asp:Label>
                                            </td>

                                        </tr>
                                    </tbody>
                                </table>
                            </div>
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
    <script type="text/javascript">
        $(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

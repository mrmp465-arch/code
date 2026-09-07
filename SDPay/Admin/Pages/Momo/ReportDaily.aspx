<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportDaily.aspx.cs" MasterPageFile="~/Layout/Layout.master" Inherits="Pages_Momo_ReportDaily" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Momo
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
               
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoReport %>">Theo thời gian</a></li>
                 <li class="active"><a href="#revenue-chart" data-toggle="tab">Theo khoảng thời gian</a></li>

            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">

                        <div class="col-xs-12 col-sm-4 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Từ ngày </span>
                                <asp:TextBox ID="txtBeginTime1" runat="server" CssClass="form-control txtBeginTime"></asp:TextBox>
                            </div>

                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Tới</span>
                                <asp:TextBox ID="txtEndTime1" runat="server" CssClass="form-control txtEndTime"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:Button ID="btView1" runat="server" CssClass="btn btn-info " OnClick="btView_Click1" Text="Xem"></asp:Button>
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
                                            <th>Cash</th>

                                            <th>TranferInternal </th>
                                            <th>TranferOutside </th>

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
                                                    <td><%#Convert.ToInt64(Eval("TotalAmountCash")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalCash")) %>)</td>
                                                    <td><%#Convert.ToInt64(Eval("TotalAmountTranferInternal")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalTranferInternal")) %>)</td>
                                                    <td><%#Convert.ToInt64(Eval("TotalAmountTranferOutside")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalTranferOutside")) %>)</td>
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

                                                <asp:Label ID="lblTotalCash" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblTotalTranferInternal" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblTotalTranferOutside" runat="server" Text=""></asp:Label>
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
    <style type="text/css">
        table.table-bordered.dataTable tbody th, table.table-bordered.dataTable tbody td {
            vertical-align: middle !important;
            text-align: center;
        }

        table tr td, table tr th {
            vertical-align: middle !important;
            text-align: center;
        }

        table .right {
            text-align: right;
        }
    </style>
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
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
    </script>
</asp:Content>

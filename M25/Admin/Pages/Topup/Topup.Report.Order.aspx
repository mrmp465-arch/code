<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.Report.Order.aspx.cs" Inherits="Pages_Topup_Report_Order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Thẻ cào
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
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.TopupReport %>">Theo thời gian</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.TopupReportCardType %>">Theo loại thẻ</a></li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Theo Order</a></li>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="txtTelCo" runat="server" CssClass="form-control">
                                <asp:ListItem Selected="True" Value="">Telco:</asp:ListItem>
                                <asp:ListItem Value="vtt">VTT</asp:ListItem>
                                <asp:ListItem Value="vms">VMS</asp:ListItem>
                                <asp:ListItem Value="vnp">VNP</asp:ListItem>
                                <asp:ListItem Value="gosu">GOSU</asp:ListItem>
                                <asp:ListItem Value="zing">ZING</asp:ListItem>
                                <asp:ListItem Value="garena">GARENA</asp:ListItem>
                                <asp:ListItem Value="vcoin">VCOIN</asp:ListItem>
                                <asp:ListItem Value="gate">GATE</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="txtUsers" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6">

                            <div class="input-group date-group " style="float: left; margin-right: 15px;">
                                <asp:DropDownList ID="drpYear" runat="server" CssClass="input-group-addon"></asp:DropDownList>
                                <asp:DropDownList ID="drpMonth" runat="server" CssClass="input-group-addon"></asp:DropDownList>
                                <asp:DropDownList ID="drpDay" runat="server" CssClass="input-group-addon"></asp:DropDownList>
                            </div>
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                            <style type="text/css">
                                .date-group select {
                                    width: 80px;
                                }
                            </style>

                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding" style="clear: both;">
                        <table class="table table-striped" id="TableResponsive" style="margin-top: 20px; clear: both;">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>OrderNo</th>
                                    <th>TotalTransaction</th>
                                    <th>%</th>
                                    <th>TotalAmount</th>
                                    <th>%</th>  
                                    <th>TotalRequest</th> 
                                    <th>TotalSuccess</th> 
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr id="tr<%# Container.ItemIndex + 1 %>" data-url="&CardType=<%#Eval("OrderNo") %>&UserId=<%#Eval("UserId") %> %>">
                                            <td></td>
                                            <td><%#Eval("OrderNo") %></td>
                                            <td><%#Convert.ToInt32(Eval("TotalTransaction")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%# AppUtils.AmountToPercent(Eval("TotalTransaction").ToString(),lblTotalTransaction.Text) %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalAmount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%# AppUtils.AmountToPercent(Eval("TotalAmount").ToString(),lblTotalAmount.Text) %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalRequestAmount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalSuccessAmount")).ToString("N0").Replace(",", ".") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <div class="box-info">
                        <div style="padding: 10px">
                            Tổng số giao dịch: <b>
                                <asp:Label ID="lblTotalTransaction" runat="server" Text="0"></asp:Label></b>
                            , tổng giá trị giao dịch: <b>
                                <asp:Label ID="lblTotalAmount" runat="server" Text="0"></asp:Label></b>
                        </div>
                    </div>
                </div>

            </div>
        </div>
        <!-- /.row -->
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
</asp:Content>

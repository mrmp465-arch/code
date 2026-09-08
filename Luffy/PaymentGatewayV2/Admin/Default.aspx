<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>


<script runat="server">

</script>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- Morris charts -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/morris.js/morris.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Dashboard
        </h1>
        <ol class="breadcrumb">
            <li><a href="Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Dashboard</li>
        </ol>
    </section>
    <asp:Panel ID="PanelContent" runat="server">
    <% if (!AppUtils.IsTopup)
        { %>
    <section class="content">
        <!-- Info boxes -->
        <div class="row">
            <div class="box-body">
                <div class="col-xs-3">
                    <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="drpPartner_SelectedIndexChanged" >
                        <asp:ListItem Value="">Partner:</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-xs-3">
                    <asp:DropDownList ID="drpCardType" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="drpCardType_SelectedIndexChanged" >
                        <asp:ListItem Value="">Telco:</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <% if (AppUtils.IsAdmin)
                { %>
                 <div class="col-xs-3"  >
                                <asp:DropDownList ID="drpMonth" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="drpMonth_SelectedIndexChanged" ></asp:DropDownList>
                                
                            </div>
                 <% } %>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-3 col-xs-6">
                <!-- small box -->
                <div class="small-box bg-aqua">
                    <div class="inner">
                        <%--<h3><%= Convert.ToInt64(totalMonthAmount).ToString("N0").Replace(",", ".") %></h3>--%>
                        <h3><%= totalMonthAmount%></h3>
                        <p>Doanh số Charging trong tháng</p>
                    </div>
                    <div class="icon">
                        <i class="ion ion-bag"></i>
                    </div>
                    <a href="pages/monitor/cardapi.report.aspx" class="small-box-footer">Xem thêm <i class="fa fa-arrow-circle-right"></i></a>
                </div>
            </div>
            <!-- ./col -->
            <div class="col-lg-3 col-xs-6">
                <!-- small box -->
                <div class="small-box bg-green">
                    <div class="inner">
                        <h3><%= monthRate %><sup style="font-size: 20px">%</sup></h3>
                        <p>Tỷ lệ Charging so với tháng trước</p>
                    </div>
                    <div class="icon">
                        <i class="ion ion-stats-bars"></i>
                    </div>
                    <a href="pages/monitor/cardapi.report.aspx" class="small-box-footer">Xem thêm <i class="fa fa-arrow-circle-right"></i></a>
                </div>
            </div>
            <% if (AppUtils.IsPartner)
                { %>
           
            <!-- ./col -->
            <div class="col-lg-3 col-xs-6">
                <!-- small box -->
                <div class="small-box bg-red tex">
                    <div class="inner">
                        <h3><%= balance.ToString("N0").Replace(",", ".") %></h3>
                        <p>Số dư</p>
                    </div>
                    <div class="icon">
                        <i class="ion ion-social-bitcoin-outline"></i>
                    </div>
                    <a href="pages/security/partnertransaction.aspx" class="small-box-footer">Xem thêm <i class="fa fa-arrow-circle-right"></i></a>
                </div>
            </div>
            <!-- ./col -->
            <% } %>
            <% if (AppUtils.IsAdmin)
                { %>
            <!-- ./col -->
            <div class="col-lg-3 col-xs-6">
                <!-- small box -->
                <div class="small-box bg-yellow">
                    <div class="inner">
                        <h3><%= partnerCount %></h3>
                        <p>Đối tác</p>
                    </div>
                    <div class="icon">
                        <i class="ion ion-person-add"></i>
                    </div>
                    <a href="pages/paygate/partners.list.aspx" class="small-box-footer">Xem thêm <i class="fa fa-arrow-circle-right"></i></a>
                </div>
            </div>
            <!-- ./col -->
            <div class="col-lg-3 col-xs-6">
                <!-- small box -->
                <div class="small-box bg-red tex">
                    <div class="inner">
                        <h3><%= providerCount %></h3>
                        <p>Nhà cung cấp</p>
                    </div>
                    <div class="icon">
                        <i class="ion ion-person-add"></i>
                    </div>
                    <a href="pages/paygate/providers.list.aspx" class="small-box-footer">Xem thêm <i class="fa fa-arrow-circle-right"></i></a>
                </div>
            </div>
            <!-- ./col -->
            <% } %>
        </div>
        <!-- /.row -->
        <div class="row">
            <div class="col-md-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Biểu đồ doanh số trong ngày</h3>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-8">
                                <p class="text-center"><strong>Hôm nay, hôm qua và 7 ngày trước</strong></p>
                                <div class="chart">
                                    <div class="chart" id="line-chart" style="height: 300px;"></div>
                                </div>
                                <!-- /.chart-responsive -->
                            </div>
                            <!-- /.col -->


                            <div class="col-md-4">
                                <p class="text-center">
                                    <strong>Tỷ lệ % thẻ</strong>
                                </p>
                                <div class="box-body">
                                    <div class="row">
                                        <div class="col-md-8">
                                            <div class="chart-responsive">
                                                <canvas id="pieChart" height="155" width="328" style="width: 328px; height: 155px;"></canvas>
                                            </div>
                                            <!-- ./chart-responsive -->
                                        </div>
                                        <!-- /.col -->
                                        <div class="col-md-4">
                                            <ul class="chart-legend clearfix">
                                                <asp:Repeater ID="rptListProduct" runat="server">
                                                    <ItemTemplate>
                                                        <li><i class="fa fa-circle-o <%#Eval("cssClass")%>"></i>Thẻ <%#Eval("label")%></li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ul>
                                        </div>
                                        <!-- /.col -->
                                    </div>
                                    <!-- /.row -->
                                </div>
                                <!-- /.box-body -->
                                <div class="box-footer no-padding">
                                    <ul class="nav nav-pills nav-stacked">
                                        <asp:Repeater ID="rptListProductTrans" runat="server">
                                            <ItemTemplate>
                                                <li><a href="<%=Constant.ADMIN_PATH %>pages/monitor/cardapi.report.cardtype.aspx"><%#Eval("label")%><span class="pull-right <%#Eval("cssClass")%>"><%#Convert.ToInt32(Eval("totalTran")).ToString("N0").Replace(",", ".")%></span></a></li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                                <!-- /.footer -->
                            </div>




                        </div>
                        <!-- /.row -->
                    </div>
                    <!-- ./box-body -->

                </div>
                <!-- /.box -->
            </div>
            <!-- /.col -->
        </div>
        <!-- /.row -->

        <!-- Main row -->
        <div class="row">
            <!-- Left col -->
            <div class="col-md-8">

                <div class="row">
                    <div class="col-md-6">
                    </div>
                    <!-- /.col -->


                </div>
                <!-- /.row -->


            </div>
            <!-- /.col -->


        </div>
        <!-- /.row -->
    </section>
    <% } %>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- page script -->
    <!-- Morris.js charts -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/raphael/2.1.0/raphael-min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/morris.js/morris.min.js"></script>
    <!-- ChartJS -->
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/Chart.js/Chart.js"></script>
    <!-- FastClick -->
    <script src="<%= Constant.ADMIN_PATH %>Content/dist/js/pages/dashboard2.js"></script>
    <script>
        var LineData = <%=graphLineData%>;
        var PieData = <%=graphPieData%>;

    </script>

</asp:Content>


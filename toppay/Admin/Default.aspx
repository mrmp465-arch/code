<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- Morris charts -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/morris.js/morris.css">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
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
                <div class="col-xs-3">
                    <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2" AutoPostBack="True" OnSelectedIndexChanged="drpPartner_SelectedIndexChanged">
                        <asp:ListItem Value="">Partner:</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-xs-3">
                    <asp:DropDownList ID="drpCardType" runat="server" CssClass="form-control select2" AutoPostBack="True" OnSelectedIndexChanged="drpCardType_SelectedIndexChanged">
                        <asp:ListItem Value="">Type:</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <% if (AppUtils.IsAdmin)
                    { %>
                <div class="col-xs-3">
                    <asp:DropDownList ID="drpMonth" runat="server" CssClass="form-control select2" AutoPostBack="True" OnSelectedIndexChanged="drpMonth_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <% } %>
            </div>
            <div class="row">
                <div class="box-body"></div>
            </div>
            <div class="row">
                <div class="col-lg-3 col-xs-6">
                    <!-- small box -->
                    <div class="small-box bg-aqua">
                        <div class="inner">
                            <%--<h3><%= Convert.ToInt64(totalMonthAmount).ToString("N0").Replace(".", ",") %></h3>--%>
                            <h3><%= totalMonthAmount%></h3>
                            <p><%= Resources.Pay.MonthStatitics%></p>
                        </div>
                        <div class="icon">
                            <i class="ion ion-bag"></i>
                        </div>
                        <a href="#" class="small-box-footer">Xem thêm <i class="fa fa-arrow-circle-right"></i></a>
                    </div>
                </div>
                <!-- ./col -->
                <div class="col-lg-3 col-xs-6">
                    <!-- small box -->
                    <div class="small-box bg-green">
                        <div class="inner">
                            <h3><%= monthRate %><sup style="font-size: 20px">%</sup></h3>
                            <p><%= Resources.Pay.PreviousMonthCharge%></p>
                        </div>
                        <div class="icon">
                            <i class="ion ion-stats-bars"></i>
                        </div>
                        <a href="#" class="small-box-footer">Xem thêm <i class="fa fa-arrow-circle-right"></i></a>
                    </div>
                </div>
                <% if (AppUtils.IsPartner)
                    { %>

                <!-- ./col -->
                <div class="col-lg-3 col-xs-6">
                    <!-- small box -->
                    <div class="small-box bg-red tex">
                        <div class="inner">
                            <h3><%= balance.ToString("N0").Replace(".", ",") %></h3>
                            <p><%= Resources.Pay.Balance%></p>
                        </div>
                        <div class="icon">
                            <i class="ion ion-social-bitcoin-outline"></i>
                        </div>
                        <a href="pages/security/myhistory.aspx" class="small-box-footer"><%= Resources.Pay.History%> <i class="fa fa-arrow-circle-right"></i></a>
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
            <div class="row">
                <!-- /.row -->
                <% if (!checkAdd)
                    { %>

                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title"><%= Resources.Pay.DayStatitics%></h3>
                        </div>

                        <!-- /.box-header -->
                        <div class="box-body">
                            <div class="row">
                                <div class="col-md-12">
                                    <p class="text-center"><span class="text-yellow"><%= Resources.Pay.Today%></span> - <span class="text-green"><%= Resources.Pay.Yesterday%></span> - <span class="text-aqua"><%= Resources.Pay.Last7day%></span></p>
                                    <div class="chart">
                                        <div class="chart" id="line-chart" style="height: 300px;"></div>
                                    </div>
                                    <!-- /.chart-responsive -->
                                </div>
                                <!-- /.col -->


                                <div class="col-md-4" style="display:none">
                                    <p class="text-center">
                                        <strong><%= Resources.Pay.Rate%>  % </strong>
                                    </p>
                                    <div class="box-body">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="chart-responsive">
                                                    <canvas id="pieChart" height="155" width="328" style="width: 328px; height: 155px;"></canvas>
                                                </div>
                                                <!-- ./chart-responsive -->
                                            </div>
                                            <!-- /.col -->
                                            <div class="col-md-6">
                                                <ul class="chart-legend clearfix">
                                                    <asp:Repeater ID="rptListProduct" runat="server">
                                                        <ItemTemplate>
                                                            <li><i class="fa fa-circle-o <%#Eval("cssClass")%>"></i><%#Eval("label")%></li>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ul>
                                            </div>
                                            <div class="col-md-4" style="display: none">
                                                <ul class="nav nav-pills nav-stacked">
                                                    <asp:Repeater ID="rptListProductTrans" runat="server">
                                                        <ItemTemplate>
                                                            <li><a href="<%=Constant.ADMIN_PATH %>pages/monitor/bankgateapi.report.aspx"><%#Eval("label")%><span class="pull-right <%#Eval("cssClass")%>"><%#Convert.ToInt32(Eval("totalTran")).ToString("N0").Replace(".", ",")%></span></a></li>
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
                <% } %>
                <% if (!AppUtils.IsAdmin && checkAdd)
                    { %>


                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title">Tạo giao dịch bank</h3>
                        </div>

                        <!-- /.box-header -->
                        <div class="box-body">

                            <div class="form-group row">
                                <%-- <div class="col-xs-3 col-sm-6 col-md-4 col-lg-3">
                                    <asp:DropDownList CssClass="form-control" ID="drpBankCode" runat="server">
                                          <asp:ListItem Text="--Chọn loại bank--" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="VCB" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>--%>
                                <div class="col-md-3">

                                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Số tiền cần nạp"></asp:TextBox>
                                </div>
                                <label id="textMoneyVND" class="control-label col-md-2" style="margin-top: 8px">
                                    &nbsp;
                                </label>
                                <div class="col-md-2">
                                    <asp:Button ID="btAdd" runat="server" CssClass="btn btn-primary" OnClick="btAdd_Click" Text="Tạo lệnh"></asp:Button>
                                </div>
                            </div>
                            <br />
                            <div class="form-group" style="height: 295px; margin-top: 15px">
                                <div id="dvBankInfo" runat="server" visible="false">
                                    <div class="row">

                                        <div class="col-md-6">
                                            <div style="width: 300px; margin-left: 0px;">
                                                <asp:Image ID="qrCode" runat="server" Width="300" />

                                            </div>
                                            <div style="margin-top: 10px; margin-left: 45px;">
                                                <dt>Tài khoản</dt>
                                                <dd>

                                                    <asp:Label ID="lbAccountNumber" runat="server" Text=""></asp:Label>
                                                </dd>
                                                <br />

                                                <dt>Nội dung chuyển tiền</dt>
                                                <dd>

                                                    <asp:Label ID="lbOrderNo" runat="server" Text=""></asp:Label>
                                                </dd>
                                            </div>

                                        </div>

                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title">Danh sách giao dịch bank</h3>
                        </div>
                        <!-- /.box-header -->
                        <div class="box-body">
                            <div class="row">
                                <div class="col-xs-12">
                                    <div class="box">
                                        <div class="box-body no-padding">
                                            <div style="clear: both"></div>
                                            <div class="table-responsive">
                                                <table class="table table-striped" id="data3">
                                                    <thead>
                                                        <tr>
                                                            <th>Tài khoản nhận</th>
                                                            <th>Nội dung chuyển tiền</th>
                                                            <th>Số tiền</th>
                                                            <th>Số tiền thực nhận</th>
                                                            <th>CreatedTime</th>
                                                            <th>LastTime</th>
                                                            <th>Trạng thái</th>


                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater ID="rpBanklog" runat="server">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td><%#Eval("BankCode") %> - <%#Eval("BankAccountNumber") %> - <%#Eval("BankAccountName") %></td>
                                                                    <td><%#Eval("OrderNo") %></td>
                                                                    <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Convert.ToInt64(Eval("TotalAmount")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}") %></td>
                                                                    <td><%#Eval("LastTime", "{0:dd/MM HH:mm:ss}") %></td>

                                                                    <td style="width: 80px"><%#GetStatusExtra(Eval("Status")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <% } %>

                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title"><%= Resources.Pay.Statitics%></h3>
                        </div>

                        <!-- /.box-header -->
                        <div class="box-body">
                            <div class="row">
                                <div class="col-xs-12">
                                    <div class="box">
                                        <div class="box-tools" style="margin-top: 10px;">

                                            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                                                <div class="input-group">
                                                    <span class="input-group-addon"><%= Resources.Pay.Partner%></span>
                                                    <asp:DropDownList ID="drpPartner2" runat="server" CssClass="form-control select2">
                                                        <asp:ListItem Value="">Partner:</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                                                <div class="input-group">
                                                    <span class="input-group-addon" style="padding: 5px;"><%= Resources.Pay.From%></span>
                                                    <asp:TextBox CssClass="form-control txtCreatTime" ID="txtBeginTime" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                                                <div class="input-group">
                                                    <span class="input-group-addon" style="padding: 5px;"><%= Resources.Pay.To%></span>
                                                    <asp:TextBox CssClass="form-control txtCreatTime" ID="txtEndTime" runat="server"></asp:TextBox>
                                                </div>
                                            </div>




                                            <div class="col-xs-12 col-sm-6 col-md-1">
                                                <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text=""></asp:Button>
                                            </div>

                                        </div>

                                        <div class="box-body no-padding">
                                            <div style="clear: both"></div>
                                            <div class="table-responsive">
                                                <table class="table table-striped" id="data1">
                                                    <thead>
                                                        <tr>


                                                            <th><%= Resources.Pay.TransactionType%></th>
                                                            <th>Tổng</th>
                                                            <th><%= Resources.Pay.DepositAmount%></th>
                                                            <% if (AppUtils.UserName != "akb" && AppUtils.UserName != "bim")
                                                                { %>
                                                            <th><%= Resources.Pay.DepositFee%></th>
                                                            <% } %>
                                                            <th><%= Resources.Pay.DepositOrderNumber%></th>
                                                            <%--<th><%= Resources.Pay.DepositOrderNumberSuccess%></th>
                                                            <th><%= Resources.Pay.DepositRateSuccess%></th>--%>

                                                            <th><%= Resources.Pay.CashAmount%></th>
                                                            <% if (AppUtils.UserName != "akb" && AppUtils.UserName != "bim")
                                                                { %>
                                                            <th><%= Resources.Pay.CashFee%></th>
                                                            <% } %>
                                                            <th><%= Resources.Pay.CashOrderNumber%></th>
                                                            <th>In-Out</th>
                                                            <%-- <th><%= Resources.Pay.CashOrderNumberSuccess%></th>
                                                            <th><%= Resources.Pay.CashRateSuccess%></th>--%>
                                                            <th><%= Resources.Pay.Topupbalance%></th>

                                                            <th><%= Resources.Pay.Withdrawbalance%></th>
                                                            <th><%= Resources.Pay.CashFlow%></th>
                                                            <% if (AppUtils.IsAdmin)
                                                                { %>


                                                            <th>Fit %</th>
                                                            <% } %>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater ID="rptListBank" runat="server">
                                                            <ItemTemplate>
                                                                <tr>

                                                                    <td><%#Eval("Type")%></td>
                                                                      <td><%#GetInOut3(Eval("TotalAmountSuccess"),Eval("TotalAmountSuccessCash")) %> (<%#GetInOut3(Eval("TotalTransSuccess"),Eval("TotalTransSuccessCash")) %>) </td>
                                                                    <td><%#Convert.ToInt64(Eval("TotalAmountSuccess")).ToString("N0").Replace(".", ",") %></td>
                                                                    <% if (AppUtils.UserName != "akb" && AppUtils.UserName != "bim")
                                                                    { %>
                                                                    <td><%#Convert.ToInt64(Eval("TotalFee")).ToString("N0").Replace(".", ",") %></td>
                                                                    <% } %>
                                                                    <td><%#Eval("TotalTransSuccess")%>/<%#Eval("TotalTrans")%></td>
                                                                    <%--  <td></td>
                                                                    <td><%#GetPerCent(Eval("TotalTrans"),Eval("TotalTransSuccess")) %> %</td>--%>

                                                                    <td><%#Convert.ToInt64(Eval("TotalAmountSuccessCash")).ToString("N0").Replace(".", ",") %></td>
                                                                    <% if (AppUtils.UserName != "akb" && AppUtils.UserName != "bim")
                                                                    { %>
                                                                    <td><%#Convert.ToInt64(Eval("TotalFeeCash")).ToString("N0").Replace(".", ",") %></td>
                                                                    <% } %>
                                                                    <td><%#Eval("TotalTransSuccessCash")%>/<%#Eval("TotalTransCash")%></td>
                                                                    <td><%#GetInOut(Eval("TotalAmountSuccess"),Eval("TotalAmountSuccessCash"),Eval("TotalFee"),Eval("TotalFeeCash")) %></td>
                                                                    <td><%#Convert.ToInt64(Eval("TotalTopup")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Convert.ToInt64(Eval("TotalDeduct")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#GetInOut2(Eval("TotalAmountSuccess"),Eval("TotalAmountSuccessCash"),Eval("TotalDeduct"),Eval("TotalTopup"),Eval("TotalFee"),Eval("TotalFeeCash")) %></td>
                                                                    <% if (AppUtils.IsAdmin)
                                                                        { %>

                                                                    <td><%#Eval("Fit")%></td>
                                                                    <% } %>
                                                                    <%-- <td</td>
                                                                    <td><%#GetPerCent(Eval("TotalTransCash"),Eval("TotalTransSuccessCash")) %> %</td>--%>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <!-- /.row -->
                        </div>
                        <!-- ./box-body -->

                    </div>
                    <!-- /.box -->
                </div>


                <% if (AppUtils.IsAdmin)
                    { %>
                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title"><%= Resources.Pay.ReportPartner%></h3>
                        </div>

                        <!-- /.box-header -->
                        <div class="box-body">
                            <div class="row">
                                <div class="col-xs-12">
                                    <div class="box">
                                        <div class="box-tools" style="margin-top: 10px;">



                                            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                                                <div class="input-group">
                                                    <span class="input-group-addon" style="padding: 5px;"><%= Resources.Pay.From%></span>
                                                    <asp:TextBox CssClass="form-control txtCreatTime" ID="txtBeginTime2" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                                                <div class="input-group">
                                                    <span class="input-group-addon" style="padding: 5px;"><%= Resources.Pay.To%></span>
                                                    <asp:TextBox CssClass="form-control txtCreatTime" ID="txtEndTime2" runat="server"></asp:TextBox>
                                                </div>
                                            </div>




                                            <div class="col-xs-12 col-sm-6 col-md-2">
                                                <asp:Button ID="Button1" runat="server" CssClass="btn btn-primary" OnClick="btView_Click2" Text="Xem"></asp:Button>
                                                <asp:Button ID="btExcel" runat="server" CssClass="btn btn-info" OnClick="ExportTran2_Click" Text="Export Excel"></asp:Button>
                                            </div>

                                        </div>

                                        <div class="box-body no-padding">
                                            <div style="clear: both"></div>
                                            <div class="table-responsive">
                                                <table class="table table-striped" id="data2">
                                                    <thead>
                                                        <tr>

                                                            <th><%= Resources.Pay.Partner%></th>
                                                            <th><%= Resources.Pay.TransactionType%></th>

                                                            <th><%= Resources.Pay.DepositAmount%></th>
                                                            <th><%= Resources.Pay.DepositFee%></th>
                                                            <th><%= Resources.Pay.DepositOrderNumber%></th>
                                                            <%-- <th><%= Resources.Pay.DepositOrderNumberSuccess%></th>
                                                            <th><%= Resources.Pay.DepositRateSuccess%></th>--%>

                                                            <th><%= Resources.Pay.CashAmount%></th>
                                                            <th><%= Resources.Pay.CashFee%></th>
                                                            <th><%= Resources.Pay.CashOrderNumber%></th>
                                                            <%--<th><%= Resources.Pay.CashOrderNumberSuccess%></th>
                                                            <th><%= Resources.Pay.CashRateSuccess%></th>--%>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater ID="rptListBank2" runat="server">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td rowspan="<%#GetPartCol(Eval("PartnerCode").ToString()) %>" style='vertical-align: top; font-weight: bold; display: <%# Eval("Type").ToString()=="BANK" ? "" : "none" %>;'><%#Eval("PartnerCode")%></td>
                                                                    <td><%#Eval("Type")%></td>

                                                                    <td><%#Convert.ToInt64(Eval("TotalAmountSuccess")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Convert.ToInt64(Eval("TotalFee")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Eval("TotalTransSuccess")%>/<%#Eval("TotalTrans")%></td>
                                                                    <%-- <td></td>
                                                                    <td><%#GetPerCent(Eval("TotalTrans"),Eval("TotalTransSuccess")) %> %</td>--%>

                                                                    <td><%#Convert.ToInt64(Eval("TotalAmountSuccessCash")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Convert.ToInt64(Eval("TotalFeeCash")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Eval("TotalTransSuccessCash")%>/<%#Eval("TotalTransCash")%></td>

                                                                    <%--<td></td>
                                                                    <td><%#GetPerCent(Eval("TotalTransCash"),Eval("TotalTransSuccessCash")) %> %</td>--%>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <!-- /.row -->
                        </div>
                        <!-- ./box-body -->

                    </div>
                    <!-- /.box -->
                </div>
                <% } %>
                <!-- /.col -->
            </div>



            </div>
            </div>
        </section>
        <% } %>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <!-- page script -->
    <!-- Morris.js charts -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/raphael/2.1.0/raphael-min.js"></script>

    <!-- ChartJS -->
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/morris.js/morris.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/Chart.js/Chart.js"></script>
    <!-- FastClick -->
    <script src="<%= Constant.ADMIN_PATH %>Content/dist/js/pages/dashboard2.js?vs=2024"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript">


        function formatPrice(price) {
            var fixedToSix = (Math.round(price * 1000000) / 1000000);
            return (Math.round(fixedToSix) == fixedToSix + 0.000001 ? fixedToSix + 0.000001 : fixedToSix);
        }
        Number.prototype.formatMoney = function (c, d, t) {
            var n = this,
                c = isNaN(c = Math.abs(c)) ? 2 : c,
                d = d == undefined ? "." : d,
                t = t == undefined ? "," : t,
                s = n < 0 ? "-" : "",
                i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
                j = (j = i.length) > 3 ? j % 3 : 0;
            return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
        };

        var unformat = function (value, decimal) {
            // Recursively unformat arrays:
            //            if (isArray(value)) {
            //                return map(value, function (val) {
            //                    return unformat(val, decimal);
            //                });
            //            }
            // Fails silently (need decent errors):
            value = value || 0;
            // Return the value as-is if it's already a number:
            if (typeof value === "number") return value;
            // Default decimal point is "." but could be set to eg. "," in opts:
            decimal = decimal || ",";
            // Build regex to strip out everything except digits, decimal point and minus sign:
            var regex = new RegExp("[^0-9-" + decimal + "]", ["g"]),
                unformatted = parseFloat(
                    ("" + value)
                        .replace(/\((.*)\)/, "-$1") // replace bracketed values with negatives
                        .replace(regex, '') // strip out any cruft
                        .replace(decimal, ',') // make sure decimal point is standard
                );
            // This will fail silently which may cause trouble, let's wait and see:
            return !isNaN(unformatted) ? unformatted : 0;
        };
        var DocTienBangChu = function (SoTien) {
            var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
            var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");
            var lan = 0;
            var i = 0;
            var so = 0;
            var KetQua = "";
            var tmp = "";
            var ViTri = new Array();
            if (SoTien < 0) return "Số tiền âm !";
            if (SoTien == 0) return "Không";
            if (SoTien > 0) {
                so = SoTien;
            }
            else {
                so = -SoTien;
            }
            if (SoTien > 8999999999999999) {
                //SoTien = 0;
                return "Số quá lớn!";
            }
            ViTri[5] = Math.floor(so / 1000000000000000);
            if (isNaN(ViTri[5]))
                ViTri[5] = "0";
            so = so - parseFloat(ViTri[5].toString()) * 1000000000000000;
            ViTri[4] = Math.floor(so / 1000000000000);
            if (isNaN(ViTri[4]))
                ViTri[4] = "0";
            so = so - parseFloat(ViTri[4].toString()) * 1000000000000;
            ViTri[3] = Math.floor(so / 1000000000);
            if (isNaN(ViTri[3]))
                ViTri[3] = "0";
            so = so - parseFloat(ViTri[3].toString()) * 1000000000;
            ViTri[2] = parseInt(so / 1000000);
            if (isNaN(ViTri[2]))
                ViTri[2] = "0";
            ViTri[1] = parseInt((so % 1000000) / 1000);
            if (isNaN(ViTri[1]))
                ViTri[1] = "0";
            ViTri[0] = parseInt(so % 1000);
            if (isNaN(ViTri[0]))
                ViTri[0] = "0";
            if (ViTri[5] > 0) {
                lan = 5;
            }
            else if (ViTri[4] > 0) {
                lan = 4;
            }
            else if (ViTri[3] > 0) {
                lan = 3;
            }
            else if (ViTri[2] > 0) {
                lan = 2;
            }
            else if (ViTri[1] > 0) {
                lan = 1;
            }
            else {
                lan = 0;
            }
            for (i = lan; i >= 0; i--) {
                tmp = DocSo3ChuSo(ViTri[i]);
                KetQua += tmp;
                if (ViTri[i] > 0) KetQua += Tien[i];
                if ((i > 0) && (tmp.length > 0)) KetQua += ',';//&& (!string.IsNullOrEmpty(tmp))
            }
            if (KetQua.substring(KetQua.length - 1) == ',') {
                KetQua = KetQua.substring(0, KetQua.length - 1);
            }
            KetQua = KetQua.substring(1, 2).toUpperCase() + KetQua.substring(2);
            //KetQua += " GG";
            return KetQua;//.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
        }
        //Hàm chuyển số thành chữ
        var DocSo3ChuSo = function (baso) {
            var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
            var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");
            var tram;
            var chuc;
            var donvi;
            var KetQua = "";
            tram = parseInt(baso / 100);
            chuc = parseInt((baso % 100) / 10);
            donvi = baso % 10;
            if (tram == 0 && chuc == 0 && donvi == 0) return "";
            if (tram != 0) {
                KetQua += ChuSo[tram] + " trăm ";
                if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
            }
            if ((chuc != 0) && (chuc != 1)) {
                KetQua += ChuSo[chuc] + " mươi";
                if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
            }
            if (chuc == 1) KetQua += " mười ";
            switch (donvi) {
                case 1:
                    if ((chuc != 0) && (chuc != 1)) {
                        KetQua += " mốt ";
                    }
                    else {
                        KetQua += ChuSo[donvi];
                    }
                    break;
                case 5:
                    if (chuc == 0) {
                        KetQua += ChuSo[donvi];
                    }
                    else {
                        KetQua += " lăm ";
                    }
                    break;
                default:
                    if (donvi != 0) {
                        KetQua += ChuSo[donvi];
                    }
                    break;
            }
            return KetQua;
        }
    </script>
    <script type="text/javascript">

        var refreshPageInterval = 120;

        setInterval("countDownPageRefresh()", 1000);//1 s gọi 1 lần
        $(document).mouseover(function () {
            funcResetRefreshPageInterval();
        });
        $(window).scroll(function () {
            funcResetRefreshPageInterval();
        });
        window.onkeypress = funcResetRefreshPageInterval;
        function funcResetRefreshPageInterval() {
            refreshPageInterval = 120;
        }
        function countDownPageRefresh() {
            refreshPageInterval = refreshPageInterval - 1;
            //console.log(refreshPageInterval);

            if (refreshPageInterval <= 0) {
                funcResetRefreshPageInterval();

                location.reload();

            }
        }

    </script>
    <script>

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
        //$(function () {
        //    var table = $('#data1').DataTable({
        //        responsive: true, "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
        //    });
        //    new $.fn.dataTable.FixedHeader(table);
        //});
        //$(function () {
        //    var table = $('#data2').DataTable({
        //        responsive: true, "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
        //    });
        //    new $.fn.dataTable.FixedHeader(table);
        //});
        $(function () {
            $('.txtCreatTime').datepicker({
                autoclose: true,
                Accepts: 'bottom'
            });
        });

        var LineData = <%=graphLineData%>;
        var PieData = <%=graphPieData%>;

        $("#<%=txtAmount.ClientID %>").keyup(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                //var textMoneyVND = DocTienBangChu(price) + " đồng";
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND").html(formatMoneyVNDVal);
            }
        }).blur(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                //var textMoneyVND = DocTienBangChu(price) + " đồng";
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND").html(formatMoneyVNDVal);
            }
        });
    </script>
    <style>
        .text-gate {
            color: #e12d2c;
        }

        .text-vtt {
            color: #d2d6de;
        }

        .text-vms {
            color: #d1332b;
        }

        .text-vnp {
            color: #57adee;
        }

        .text-momo {
            color: #ad2b73;
        }

        .text-acb {
            color: #e0f542;
        }

        .text-vcb {
            color: #6a885d;
        }

        .text-bidv {
            color: #7bfc73;
        }

        .text-mb {
            color: #1c1ed2;
        }

        .text-tpb {
            color: #5e2e86;
        }

        .text-icb {
            color: #d52a29;
        }

        #data1 tr:last-child {
            font-weight: bold;
        }



        @media screen and (max-width: 767px) {
            .table-responsive {
                width: 100%;
                margin-bottom: 15px;
                overflow-y: hidden;
                -ms-overflow-style: -ms-autohiding-scrollbar;
                border: 1px solid #ddd;
            }
        }
    </style>
</asp:Content>


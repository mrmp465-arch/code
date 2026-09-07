<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="BankGateAPI.Report.Profit2.aspx.cs" Inherits="Pages_Monitor_BankGateAPI_Report_Profit2" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <script type="text/javascript">
        var exportThisWithParameter = (function () {
            var uri = 'data:application/vnd.ms-excel;base64,',
                template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"  xmlns="http://www.w3.org/TR/REC-html40"><head> <!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets> <x:ExcelWorksheet><x:Name>{worksheet}</x:Name> <x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions> </x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook> </xml><![endif]--></head><body> <table>{table}</table></body></html>',
                base64 = function (s) {
                    return window.btoa(unescape(encodeURIComponent(s)))
                },
                format = function (s, c) {
                    return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; })
                }
            return function (tableID, excelName) {
                tableID = document.getElementById(tableID)
                var ctx = { worksheet: excelName || 'Worksheet', table: tableID.innerHTML.replace(/<td/g, "<td style='text-align:center;vertical-align:middle;'").replace(/<th/g, "<th style='text-align:center;vertical-align:middle;'") }
                window.location.href = uri + base64(format(template, ctx))
            }
        })()
    </script>
    <section class="content-header">
        <h1>Nạp bank
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
                <li><a href="<%=Constant.ADMIN_PATH + "pages/monitor/bankgateAPI.report.profit.aspx"%>">Theo thời gian</a></li>
                <li class="active"><a href="#"  data-toggle="tab">Theo đối tác</a></li>
                <%--<li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReportCardType %>">Theo loại thẻ</a></li>
                <%if (AppUtils.IsAdmin)
                    {%>
                <li
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReportProvider %>">Theo nhà cung cấp</a></li>

                <% }%>--%>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">



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
                    <!-- /.box-header -->
                    <div class="box-body no-padding" style="clear: both;">
                        <div class="table-responsive">
                            <div id="dgrid" class="dataTables_wrapper form-inline" role="grid">
                                <table class="table table-striped" id="TableResponsive">
                                    <thead>
                                        <tr>
                                          
                                            <th>Đối tác</th>
                                            <th>Số lệnh nạp</th>
                                            <th>Số tiền nạp</th>
                                            <th>Phí nạp</th>
                                            <th>Hoa hồng nạp</th>
                                            <th>Phí NCC</th>
                                            <th>Lợi nhuận nạp</th>

                                            <th>Fit %</th>

                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptList" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    
                                                    <td><%#Eval("PartnerCode")%></td>
                                                    <td><%#Eval("TotalTransaction")%></td>
                                                    <td><%#Convert.ToInt64(Eval("TotalAmount")).ToString("#,#").Replace(".", ",") %> </td>
                                                    <td><%#Convert.ToInt64(Eval("Fee")).ToString("#,#").Replace(".", ",")%> </td>
                                                    <td><%#Convert.ToInt64(Eval("Reward")).ToString("#,#").Replace(".", ",")%> </td>
                                                    <td><%#Convert.ToInt64(Eval("FeeProvider")).ToString("#,#").Replace(".", ",")%> </td>
                                                    <td><%#Convert.ToInt64(Eval("Profit")).ToString("#,#").Replace(".", ",")%> </td>
                                                    <td><%# AppUtils.AmountToPercentFit(Convert.ToInt64(Eval("TotalAmount")).ToString(),Eval("Profit").ToString()) %></td>

                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <tr style="font-weight: bold">
                                            <td colspan="1">Tổng</td>
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

                                                <asp:Label ID="lblFeeProvider" runat="server" Text=""></asp:Label>
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
                    </div>
                     <div class="box-footer">
     <input type="button" class="btn btn-info pull-right" value="export Excel" onclick="exportThisWithParameter('TableResponsive', 'DoiSoat')" />
 </div>
                    <div class="box-body no-padding" style="clear: both;">
                        <div class="table-responsive">
                            <div id="dgrid2" class="dataTables_wrapper form-inline" role="grid">
                                <table class="table table-striped" id="TableResponsive2">
                                    <thead>
                                        <tr>
                                           
                                            <th>Đối tác</th>
                                            <th>Số lệnh nạp</th>
                                            <th>Số tiền nạp</th>
                                            <th>Phí nạp</th>
                                            <th>Hoa hồng nạp</th>
                                            <th>Phí NCC</th>
                                            <th>Lợi nhuận nạp</th>

                                            <th>Fit %</th>

                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptList2" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                  
                                                    <td><%#Eval("PartnerCode")%></td>
                                                    <td><%#Eval("TotalTransaction")%></td>
                                                    <td><%#Convert.ToInt64(Eval("TotalAmount")).ToString("#,#").Replace(".", ",") %> </td>
                                                    <td><%#Convert.ToInt64(Eval("Fee")).ToString("#,#").Replace(".", ",")%> </td>
                                                    <td><%#Convert.ToInt64(Eval("Reward")).ToString("#,#").Replace(".", ",")%> </td>
                                                    <td><%#Convert.ToInt64(Eval("FeeProvider")).ToString("#,#").Replace(".", ",")%> </td>
                                                    <td><%#Convert.ToInt64(Eval("Profit")).ToString("#,#").Replace(".", ",")%> </td>
                                                    <td><%# AppUtils.AmountToPercentFit(Convert.ToInt64(Eval("TotalAmount")).ToString(),Eval("Profit").ToString()) %></td>

                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <tr style="font-weight: bold">
                                            <td colspan="1">Tổng</td>
                                            <td>

                                                <asp:Label ID="lblTotalTransaction2" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblTotalAmount2" runat="server" Text=""></asp:Label>
                                            </td>

                                            <td>

                                                <asp:Label ID="lblFee2" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblReward2" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblFeeProvider2" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblProfit2" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblFit2" runat="server" Text=""></asp:Label>
                                            </td>

                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                </div>
                     <div class="box-footer">
     <input type="button" class="btn btn-info pull-right" value="export Excel" onclick="exportThisWithParameter('TableResponsive2', 'DoiSoat')" />
 </div>
                <%-- <div class="box-info">
                        <div style="padding: 10px">
                            Tổng số giao dịch: <b>
                                <asp:Label ID="lblTotalTransaction" runat="server" Text="0"></asp:Label></b>
                            , tổng giá trị giao dịch: <b>
                                <asp:Label ID="lblTotalAmount" runat="server" Text="0"></asp:Label></b>
                            , tổng lợi nhuận: <b>
                                <asp:Label ID="lblTotalProfit" runat="server" Text="0"></asp:Label></b>
                            , % lợi nhuận tổng: <b>
                                <asp:Label ID="txtTotalFit" runat="server" Text="0"></asp:Label></b>
                        </div>
                    </div>--%>
            </div>
             </div>
            <div class="chart tab-pane " id="sales-chart">
            </div>
        </div>
      
    </section>
    <!-- /.content -->
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
     
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

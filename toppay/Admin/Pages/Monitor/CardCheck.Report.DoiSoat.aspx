<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardCheck.Report.DoiSoat.aspx.cs" Inherits="Pages_Monitor_CardCheck_Report_DoiSoat" %>

<asp:Content ID="Header1" ContentPlaceHolderID="head" runat="Server">
    <meta http-equiv="content-type" content="application/vnd.ms-excel; charset=UTF-8">
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
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Thẻ cào
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Báo cáo Tài Khoản - Đối soát check thẻ" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Đối soát</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-md-3 col-sm-6 col-xs-12">
                <div class="info-box">
                    <span class="info-box-icon bg-aqua"><i class="ion ion-ios-people-outline"></i></span>

                    <div class="info-box-content">
                        <span class="info-box-text">Tài khoản sãn sàng</span>
                        <span class="info-box-number"><%=AccountReport.TotalReady.ToString("N0") %> / <%=AccountReport.Total.ToString("N0") %></span>
                    </div>
                    <!-- /.info-box-content -->
                </div>
                <!-- /.info-box -->
            </div>
            <!-- /.col -->
            <div class="col-md-3 col-sm-6 col-xs-12">
                <div class="info-box">
                    <span class="info-box-icon bg-red"><i class="icon ion-ios-people-outline"></i></span>

                    <div class="info-box-content">
                        <span class="info-box-text">Tài khoản lỗi</span>
                        <span class="info-box-number"><%=AccountReport.TotalError.ToString("N0") %> / <%=AccountReport.Total.ToString("N0") %></span>
                    </div>
                    <!-- /.info-box-content -->
                </div>
                <!-- /.info-box -->
            </div>
            <!-- /.col -->

            <!-- fix for small devices only -->
            <div class="clearfix visible-sm-block"></div>

            <div class="col-md-3 col-sm-6 col-xs-12">
                <div class="info-box">
                    <span class="info-box-icon bg-green"><i class="ion ion-ios-people-outline"></i></span>

                    <div class="info-box-content">
                        <span class="info-box-text">Sãn sàng trong ngày</span>
                        <span class="info-box-number"><%=AccountReport.DayReady.ToString("N0") %> ~ <%=AccountReport.DayTurnReady.ToString("N0") %> </span>
                    </div>
                    <!-- /.info-box-content -->
                </div>
                <!-- /.info-box -->
            </div>
            <!-- /.col -->
            <div class="col-md-3 col-sm-6 col-xs-12">
                <div class="info-box">
                    <span class="info-box-icon bg-yellow"><i class="ion ion-ios-people-outline"></i></span>

                    <div class="info-box-content">
                        <span class="info-box-text">Sãn sàng trong tháng</span>
                        <span class="info-box-number"><%=AccountReport.MonthReady.ToString("N0") %> ~ <%=AccountReport.MonthTurnReady.ToString("N0") %></span>
                    </div>
                    <!-- /.info-box-content -->
                </div>
                <!-- /.info-box -->
            </div>
            <!-- /.col -->
        </div>
        <!-- /.row -->

        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Đối soát theo</li>
                <%if (AppUtils.IsAdmin || AppUtils.IsPartner)
                    {%>
                <li class="active"><a href="#partner" data-toggle="tab">Đối tác</a></li>
                <%} %>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="partner">
                    <div class="box-tools" style="margin-top: 10px;">
                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                            { %>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:DropDownList ID="drpPartner1" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:DropDownList ID="drpCardType1" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>


                        <div class="col-xs-12 col-sm-4 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Từ ngày </span>
                                <asp:TextBox ID="txtBeginTime1" runat="server" CssClass="form-control txtBeginTime"></asp:TextBox>
                            </div>

                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-3">
                            <div class="input-group" id="myModalWithDatePicker">
                                <span class="input-group-addon">Đến ngày</span>
                                <asp:TextBox ID="txtEndTime1" runat="server" CssClass="form-control txtEndTime"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:Button ID="btView1" runat="server" CssClass="btn btn-info " OnClick="btView_Click1" Text="Xem"></asp:Button>
                        </div>
                    </div>
                    <div class="box-body " style="clear: both; overflow-x: auto">
                        <table class="table table-bordered nowrap" id="TableResponsive1" style="margin-top: 20px; clear: both;" cellspacing="0" width="100%">
                            <thead>
                                <tr>
                                    <th>Đối tác</th>
                                    <th>Loại thẻ</th>
                                    <th>Mệnh giá</th>
                                    <th>Thẻ đúng Serial</th>
                                    <th>Thẻ sai Serial</th>
                                    <th><i>(Thẻ chưa dùng)</i></th>
                                    <th><i>(Thẻ đã dùng)</i></th>
                                    <th><i>(Thẻ chưa kích hoạt)</i></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server"><%-- OnItemDataBound="rptList_OnItemDataBound">--%>
                                    <ItemTemplate>
                                        <tr>
                                            <td style='display: <%# ((bool) Eval("IsFirstRowWithPartnerCode")) ? "" : "none" %>;' rowspan="<%# Eval("CountOfPartnerCode") %>"><%#Eval("PartnerCode")%></td>
                                            <td style='display: <%# ((bool) Eval("IsFirstRowWithCommandCode")) ? "" : "none" %>;' rowspan="<%# Eval("CountOfCommandCode") %>"><%#Eval("CommandCode")%></td>
                                            <td class="right"><%#Convert.ToInt32(Eval("CardValue")).ToString("N0")%></td>
                                            <td class="right"><%#Convert.ToInt32(Eval("SerialValid")).ToString("N0")%></td>
                                            <td class="right"><%#Convert.ToInt32(Eval("SerialInvalid")).ToString("N0")%></td>
                                            <td class="right"><%#Convert.ToInt32(Eval("UsedCard")).ToString("N0")%></td>
                                            <td class="right"><%#Convert.ToInt32(Eval("UnusedCard")).ToString("N0")%></td>
                                            <td class="right"><%#Convert.ToInt32(Eval("CardNotActivated")).ToString("N0")%></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr class="Total">
                                    <td colspan="3" style="text-align: center;"><b>Total</b></td>
                                    <td class="right"><b><%=totalSerialValid.ToString("N0")%></b></td>
                                    <td class="right"><b><%=totalSerialInvalid.ToString("N0")%></b></td>
                                    <td colspan="3" style="text-align: center;"><b>#</b></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <input type="button" class="btn btn-info pull-right" value="export Excel" onclick="exportThisWithParameter('TableResponsive1', 'DoiSoatThe')" />
                    </div>
                </div>

            </div>
        </div>


    </section>
    <!-- /.content -->
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
                orientation: "bottom"
            });
            $('.txtEndTime').datepicker({
                autoclose: true,
                orientation: "bottom"

            });

        });
    </script>
</asp:Content>

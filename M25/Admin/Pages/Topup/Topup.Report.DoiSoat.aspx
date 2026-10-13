<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.Report.DoiSoat.aspx.cs" Inherits="Pages_Topup_Report_DoiSoat" %>

<%@ Import Namespace="System.Linq" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Topup
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Đối soát thẻ" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Đối soát</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Đối soát Topup</li>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane <% if (Type == "1" || Type == null)
                    {%>active <% }%>"
                    id="partner">
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="txtTelco" runat="server" CssClass="form-control">
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
                            <asp:Button ID="btView1" runat="server" CssClass="btn btn-primary" OnClick="btView_Click1" Text="Xem"></asp:Button>
                        </div>
                    </div>
                    <div class="box-body " style="clear: both; overflow-x: auto">
                        <table class="table table-bordered nowrap" id="TableResponsive1" style="margin-top: 20px; clear: both;" cellspacing="0" width="100%">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>User</th>
                                    <th>Telco</th>
                                    <th>Amount</th>
                                    <th>Quantity</th>
                                    <th>Total</th>
                                </tr>
                            </thead>
                            <tbody>
                                <% long Total = 0;
                                    if (lstReportDoiSoat1 != null)
                                    {
                                        int i = 1;
                                        int UserId = 0;
                                        string Telco = "";
                                        foreach (var item in lstReportDoiSoat1)
                                        {
                                            int count = 0;
                                            if (UserId != item.UserId)
                                            {
                                                count = lstReportDoiSoat1.Count(e => e.UserId == item.UserId);
                                                UserId = item.UserId;
                                                Telco = "";
                                            }
                                            int count2 = 0;
                                            if (Telco != item.Telco)
                                            {
                                                count2 = lstReportDoiSoat1.Count(e => e.Telco == item.Telco && e.UserId == item.UserId);
                                                Telco = item.Telco;
                                            }  %>
                                <tr style="vertical-align: middle;">
                                    <%if (count > 0)
                                        { %>
                                    <td rowspan="<%=count %>"><%=i++ %></td>
                                    <td rowspan="<%=count %>"><%= item.UserName %></td>
                                    <%} %>
                                    <%if (count2 > 0)
                                        { %><td rowspan="<%=count2 %>"><%= item.Telco %></td>
                                    <%}%>
                                    <td class="right"><%= Convert.ToInt64(item.Amount).ToString("N0") %></td>
                                    <td class="right"><%= item.ReturnValue.ToString("N0") %></td>
                                    <td class="right"><%= (item.ReturnValue * Convert.ToInt64(item.Amount)).ToString("N0") %></td>
                                </tr>
                                <% Total += item.ReturnValue * Convert.ToInt64(item.Amount);
                                        }
                                    } %>
                                <tr class="Total">
                                    <td colspan="5" style="text-align: center;"><b>Total</b></td>
                                    <td class="right"><b><%= Total.ToString("N0") %></b></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <div class="col-xs-12">
                            <input type="button" class="btn btn-primary pull-right" value="Generate Excel Table" onclick="exportThisWithParameter('TableResponsive1', 'DoiSoatThe')">
                            <%--<asp:Button runat="server" Style="margin-right: 15px;" CssClass="btn btn-primary pull-right" ID="ExportTran" Text="Generate Excel full Trans" OnClick="ExportTran_Click"></asp:Button>--%>
                            <button ID="ExportTran" type="button" class="btn btn-primary pull-right" style="margin-right: 5px;" runat="server" OnServerClick="ExportTran_Click">
                                <i class="fa fa-download"></i> Generate Excel full Trans
                            </button>
                        </div>
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

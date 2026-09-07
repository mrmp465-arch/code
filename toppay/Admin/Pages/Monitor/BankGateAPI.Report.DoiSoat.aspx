<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankGateAPI.Report.DoiSoat.aspx.cs" Inherits="Pages_Monitor_BankGateAPI_Report_DoiSoat" %>

<%@ Import Namespace="System.Linq" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
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
        <h1><%= Resources.Pay.Deposit%>
            <small>
                <asp:Label ID="lblTtitle" runat="server" Text="Đối soát ngân  hàng" CssClass="title"></asp:Label>
            </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active"><%= Resources.Pay.Reconcilation%></li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i><%= Resources.Pay.Reconcilation%> </li>
                <%if (AppUtils.IsAdmin || AppUtils.IsPartner)
                    {%>
                <li class="<% if (Type == "1" || Type == null)
                    {%>active <% }%>"><a href="#partner" data-toggle="tab"><%= Resources.Pay.Partner%></a></li>
                <%} %>
                <%if (AppUtils.IsAdmin || AppUtils.IsProvider)
                    {%>
                <li class="<% if (Type == "2")
                    {%>active <% }%>"><a href="#Provider" data-toggle="tab">Nhà cung cấp</a></li>
                <% }%>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane <% if (Type == "1" || Type == null)
                    {%>active <% }%>"
                    id="partner">
                    <div class="box-tools" style="margin-top: 10px;">
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Nhóm</span>
                                <asp:DropDownList ID="drpGroup" runat="server" CssClass="form-control" AutoPostBack="false">
                                    <asp:ListItem Text="Tất cả" Value="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="mrX" Value="mrX"> </asp:ListItem>
                                    <asp:ListItem Text="mrD" Value="mrD"></asp:ListItem>
                                    <asp:ListItem Text="mrLee" Value="mrLee"> </asp:ListItem>
                                    <asp:ListItem Text="mark" Value="mark"></asp:ListItem>
                                    <asp:ListItem Text="hyn" Value="hyn"></asp:ListItem>
                                    <asp:ListItem Text="nc" Value="nc"></asp:ListItem>
                                    <asp:ListItem Text="loki" Value="loki"></asp:ListItem>
                                    <asp:ListItem Text="hng" Value="hng"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <% } %>
                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                            { %>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:DropDownList ID="drpPartner1" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>

                        <% } %>

                        <div class="col-xs-12 col-sm-4 col-md-2" style="display: none">

                            <asp:DropDownList ID="drpBankCode1" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>


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

                        <div class="col-xs-12 col-sm-6 col-md-2" style="display:none">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox runat="server" ID="cbCard" Checked="true" />
                                </span>
                                <span class="form-control">Gộp thẻ</span>
                            </div>
                            <!-- /input-group -->
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:Button ID="btView1" runat="server" CssClass="btn btn-info " OnClick="btView_Click1" Text="Xem"></asp:Button>
                        </div>
                    </div>
                    <div class="box-body " style="clear: both; overflow-x: auto">
                        <table class="table table-bordered nowrap" id="TableResponsive1" style="margin-top: 20px; clear: both;" cellspacing="0" width="100%">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th><%= Resources.Pay.Partner%></th>
                                    <th><%= Resources.Pay.TransactionType%></th>

                                    <th><%= Resources.Pay.DepositOrderNumberSuccess%></th>
                                    <th><%= Resources.Pay.DepositAmount%></th>
                                </tr>
                            </thead>
                            <tbody>
                                <% decimal Total = 0;
                                    if (lstReportDoiSoat1 != null)
                                    {
                                        int i = 1;
                                        string PartnerCode = "";
                                        string BankCode = "";
                                        foreach (var item in lstReportDoiSoat1)
                                        {
                                            int count = 0;
                                            if (PartnerCode != item.PartnerCode)
                                            {
                                                count = lstReportDoiSoat1.Count(e => e.PartnerCode == item.PartnerCode);
                                                PartnerCode = item.PartnerCode;
                                                BankCode = "";
                                            }
                                            int count2 = 0;
                                            if (BankCode != item.BankCode)
                                            {
                                                count2 = lstReportDoiSoat1.Count(e => e.BankCode == item.BankCode && e.PartnerCode == item.PartnerCode);
                                                BankCode = item.BankCode;
                                            }  %>
                                <tr style="vertical-align: middle;">

                                    <%if (count > 0)
                                        { %>
                                    <td rowspan="<%=count %>"><%=i++ %></td>
                                    <td rowspan="<%=count %>"><%= item.PartnerCode %></td>
                                    <%} %>
                                    <%if (count2 > 0)
                                        { %><td rowspan="<%=count2 %>"><%= item.BankCode %></td>
                                    <%} %>
                                    <%--<td style="vertical-align: middle; text-align: center;"><%= item.Provider %></td>--%>

                                    <td class="right"><%= item.ReturnValue.ToString("N0").Replace(".", ".") %></td>
                                    <td class="right"><%= Convert.ToInt64((item.ReturnTotalValue)).ToString("N0").Replace(".", ".") %></td>
                                </tr>
                                <% Total += item.ReturnTotalValue;
                                        }
                                    } %>
                                <tr class="Total">
                                    <td colspan="4" style="text-align: center;"><b>Total</b></td>
                                    <td class="right"><b><%= Total.ToString("N0").Replace(".", ".") %></b></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <button id="ExportTran" type="button" class="btn btn-primary pull-right" style="margin-right: 5px;" runat="server" onserverclick="ExportTran_Click">
                            <i class="fa fa-download"></i>Export Excel
                        </button>
                    </div>
                </div>
                <div class="chart tab-pane <% if (Type == "2")
                    {%> active <% }%>"
                    id="Provider">
                    <div class="box-tools" style="margin-top: 10px;">
                        <% if (AppUtils.IsAdmin || AppUtils.IsProvider)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpProvider2" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <%} %>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:DropDownList ID="drpBankCode2" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>


                        <div class="col-xs-12 col-sm-4 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Từ ngày </span>
                                <asp:TextBox ID="txtBeginTime2" runat="server" CssClass="form-control txtBeginTime"></asp:TextBox>
                            </div>

                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Tới</span>
                                <asp:TextBox ID="txtEndTime2" runat="server" CssClass="form-control txtEndTime"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:Button ID="btView2" runat="server" CssClass="btn btn-info " OnClick="btView_Click2" Text="Xem"></asp:Button>
                        </div>
                    </div>
                    <div class="box-body " style="clear: both; overflow-x: auto">
                        <table class="table table-bordered" id="TableResponsive2" style="margin-top: 20px; clear: both;">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Provider</th>
                                    <th>Type</th>

                                    <th>Quantity</th>
                                    <th>Total</th>
                                </tr>
                            </thead>
                            <tbody>
                                <% decimal Total2 = 0;
                                    if (lstReportDoiSoat2 != null)
                                    {

                                        int i = 1;
                                        string ProviderCode = "";
                                        string BankCode = "";
                                        foreach (var item in lstReportDoiSoat2)
                                        {
                                            int count = 0;
                                            if (ProviderCode != item.ProviderCode)
                                            {
                                                count = lstReportDoiSoat2.Count(e => e.ProviderCode == item.ProviderCode);
                                                ProviderCode = item.ProviderCode;
                                                BankCode = "";
                                            }
                                            int count2 = 0;
                                            if (BankCode != item.BankCode)
                                            {
                                                count2 = lstReportDoiSoat2.Count(e => e.BankCode == item.BankCode && e.ProviderCode == item.ProviderCode);
                                                BankCode = item.BankCode;
                                            } %>
                                <tr>

                                    <%if (count > 0)
                                        { %>
                                    <td rowspan="<%=count %>"><%=i++ %></td>
                                    <td rowspan="<%=count %>"><%= item.ProviderCode %></td>
                                    <%} %>
                                    <%if (count2 > 0)
                                        { %><td rowspan="<%=count2 %>"><%= item.BankCode %></td>
                                    <%} %>
                                    <%--<td style="vertical-align: middle; text-align: center;"><%= item.Provider %></td>--%>

                                    <td class="right"><%= item.ReturnValue.ToString("N0")%></td>
                                    <td class="right"><%= Convert.ToInt64((item.ReturnTotalValue)).ToString("N0") %></td>
                                </tr>
                                <% Total2 += item.ReturnTotalValue;
                                        }
                                    } %>
                                <tr class="Total">
                                    <td colspan="4" style="text-align: center;"><b>Total</b></td>
                                    <td class="right"><b><%= Total2.ToString("N0")%></b></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <button id="Button1" type="button" class="btn btn-primary pull-right" style="margin-right: 5px;" runat="server" onserverclick="ExportTran2_Click">
                            <i class="fa fa-download"></i>Export Excel
                        </button>
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

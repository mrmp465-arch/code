<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankGateAPI.Report.Partner2.aspx.cs" Inherits="Pages_Monitor_BankGateAPI_Report_Partner2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1><%= Resources.Pay.Deposit%>
            <small><%= Resources.Pay.Report%> </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active"><%= Resources.Pay.Report%></li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">

        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i><%= Resources.Pay.Report%></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankGateAPIReport %>"><%= Resources.Pay.ReportTime%></a></li>

                <%if (AppUtils.IsAdmin)
                    {%>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankGateAPIReportBankCode %>">Theo BankCode</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankGateAPIReportPartner %>">Theo đối tác</a></li>
                <li class="active"><a href="#">Tổng hợp</a></li>
                <% }%>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <asp:DropDownList CssClass="form-control" ID="drpBankCode" runat="server">
                            </asp:DropDownList>
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
                                <table class="table table-striped" id="TableResponsive" style="margin-top: 20px; clear: both;">
                                    <thead>
                                        <tr>

                                            <th rowspan="2">#</th>
                                            <th style="text-align: center;" colspan="<%=Partner.Count%>">Partner</th>

                                        </tr>
                                        <tr>
                                            <% foreach (var item in Partner)
                                                { %>
                                            <td><b><%=item%></b></td>
                                            <%} %>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <% foreach (var time in Time)
                                            { %>
                                        <tr>
                                            <td>
                                                <b><%=time%></b>
                                            </td>
                                            <% foreach (var item in Partner)
                                                { %>

                                            <%if (Data.Exists(x => x.Time == time && x.PartnerCode == item))
                                                {%>
                                            <td><%= Convert.ToInt64(Data.FirstOrDefault(x => x.Time == time && x.PartnerCode == item).TotalAmount).ToString("#,#").Replace(".", ",")%> </td>
                                            <%}
                                                else
                                                {%>


                                            <td>0</td>

                                            <%}
                                                } %>
                                        </tr>

                                        <%} %>
                                        <tr>
                                            <td><b>Tổng</b></td>
                                            <% foreach (var item in Partner)
                                                { %>

                                            <%if (DataTotal.Exists(x => x.PartnerCode == item))
                                                {%>
                                            <td><b><%= Convert.ToInt64(DataTotal.FirstOrDefault(x =>  x.PartnerCode == item).TotalAmount).ToString("#,#").Replace(".", ",")%> </b></td>
                                            <%}
                                                else
                                                {%>


                                            <td>0</td>

                                            <%}
                                            } %>
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

   <%-- <style>
        .table-responsive {
            overflow-x: unset;
        }
    </style>--%>
</asp:Content>

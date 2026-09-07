<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardAPI.Report.Partner2.aspx.cs" Inherits="Pages_Monitor_CardAPI_Report_Partner2" %>

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
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReport %>">Theo thời gian</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReportCardType %>">Theo loại thẻ</a></li>
                <%if (AppUtils.IsAdmin)
                    {%>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReportPartner %>">Theo đối tác</a></li>

                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIReportProvider %>">Theo nhà cung cấp</a></li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Tổng hợp</a></li>
                <% }%>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:DropDownList ID="drpCardType" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-8 col-md-5">

                            <div class="input-group date-group" style="float: left; margin-right: 15px;">
                                <asp:DropDownList ID="drpYear" runat="server" CssClass="input-group-addon"></asp:DropDownList>
                                <asp:DropDownList ID="drpMonth" runat="server" CssClass="input-group-addon"></asp:DropDownList>
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
                    <div class="box-body no-padding" style="clear: both;">
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
                                            <td><%=Data.FirstOrDefault(x => x.Time == time && x.PartnerCode == item).TotalAmount.ToString("N0").Replace(",", ".")%> </td>
                                            <%}
                                            else
                                            {%>


                                            <td>0</td>

                                            <%}
                                                } %>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
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
</asp:Content>

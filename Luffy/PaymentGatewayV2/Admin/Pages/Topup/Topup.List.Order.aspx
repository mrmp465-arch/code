<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.List.Order.aspx.cs" Inherits="Pages_Topup_Topup_List_Order" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <span id="AlertInfos"></span>
            </div>
        </div>
    </div>
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Topup
            <small>
                <asp:Label ID="lblTtitle" runat="server" Text="Danh sách Order" CssClass="title"></asp:Label>
            </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Xem log</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Theo dõi đơn hàng</li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.TopupList%>">Danh sách Order</a></li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Order hoạt động</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.TopupListOrderConfirm%>">Order đã chốt</a></li>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-info">
                        <div style="padding: 10px">
                            Tổng hoàn thành: <b>
                                <asp:Label ID="lblTotalSuccess" runat="server" Text="0"></asp:Label></b> | Tổng yêu cầu: <b>
                                <asp:Label ID="lblTotalRequest" runat="server" Text="0"></asp:Label></b> | Tổng chưa nạp: <b>
                                <asp:Label ID="lblTotalMiss" runat="server" Text="0"></asp:Label></b> | Tổng đơi nạp: <b>
                                <asp:Label ID="lblTotalWaiting" runat="server" Text="0"></asp:Label>
                            </b>
                        </div>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpTop" runat="server" CssClass="form-control drpTop">
                                <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                                <asp:ListItem Selected="True" Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 bản ghi" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="txtUsers" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="txtStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Tình trạng:</asp:ListItem>
                                <asp:ListItem Value="1">Đã hoàn thành</asp:ListItem>
                                <asp:ListItem Value="0">Chưa hoàn thành</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="txtTelco" runat="server" CssClass="form-control">
                                <asp:ListItem Selected="True" Value="">Telco:</asp:ListItem>
                                <asp:ListItem Value="vtt">VTT</asp:ListItem>
                                <asp:ListItem Value="vms">VMS</asp:ListItem>
                                <asp:ListItem Value="vnp">VNP</asp:ListItem>
                                <asp:ListItem Value="gosu">GOSU</asp:ListItem>
                                <asp:ListItem Value="zing">ZING</asp:ListItem>
                                <asp:ListItem Value="garena">GARENA</asp:ListItem>
                                <asp:ListItem Value="vtc">VTC</asp:ListItem>
                                <asp:ListItem Value="dzo">DZO</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Order No</span>
                                <asp:TextBox ID="txtOrderNo" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Time</span>
                                <asp:TextBox ID="txtCreatTime" runat="server" CssClass="form-control txtCreatTime"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <%--<th>Id</th>--%>
                                    <th>OrderNo</th>
                                    <th>UserName</th>
                                    <th>CreatedTime</th>
                                    <th>Telco</th>
                                    <th>Trans/Total</th>
                                    <th>Amount/Total</th>
                                    <th>Queue</th>
                                    <th>Success</th>
                                    <th>Ignore</th>
                                    <th>Lock</th>
                                    <th>Disable</th>
                                    <th>LastTime</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server" OnItemCommand="rptList_OnItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupList %>?id=<%#Eval("OrderNo") %>"><%#Eval("OrderNo") %></a></td>
                                            <td><%#Eval("UserName")%></td>
                                            <td><%#Eval("CreatedTime")%></td>
                                            <td><%#Eval("Telco")%></td>
                                            <td><%#Eval("TotalTranSuccess")%> / <%#Eval("TotalTrans")%></td>
                                            <td><%#Convert.ToInt64(Eval("TotalAmountSuccess")).ToString("N0").Replace(",", ".")%> / <%#Convert.ToInt64(Eval("TotalAmount")).ToString("N0").Replace(",", ".")%></td>
                                            <td><a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupList %>?id=<%#Eval("OrderNo") %>&s=1"><%#Eval("Queue")%></a></td>
                                            <td><a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupList %>?id=<%#Eval("OrderNo") %>&s=3"><%#Eval("Success")%></a></td>
                                            <td><a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupList %>?id=<%#Eval("OrderNo") %>&s=-1"><%#Eval("Ignore")%></a></td>
                                            <td><a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupList %>?id=<%#Eval("OrderNo") %>&s=-2"><%#Eval("Lock")%></a></td>
                                            <td><a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupList %>?id=<%#Eval("OrderNo") %>&s=0"><%#Eval("Disable")%></a></td>
                                            <td><%#Eval("LastTime")%></td>
                                            <td>
                                                <a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupList %>?id=<%#Eval("OrderNo") %>">Xem</a> |  <a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupEditOrder %>?o=<%#Eval("OrderNo") %>&u=<%#Eval("UserId")%>&ur=<%=HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString())%>&ic=0">Sửa</a>
                                                | <asp:LinkButton runat="server" ID="ExportExcel" CommandName="Export" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'>Export</asp:LinkButton>
                                                | <asp:LinkButton runat="server" ID="ExportExcelTrans" CommandName="ExportTrans" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'>Export Trans</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <!-- /.box-body -->
                </div>
                <!-- /.box -->
            </div>
            <!-- /.col -->

            <!-- /.row -->
        </div>
    </section>
    <!-- /.content -->
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript">
        $(function () {
            $(function () {
                $('.txtCreatTime').datetimepicker();
            });
        });
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

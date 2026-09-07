<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.Search.aspx.cs" Inherits="Pages_topup_search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Topup
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Tra cứu" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Tra cứu</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Tra cứu log giao dịch" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpTop" runat="server" CssClass="form-control">
                                <asp:ListItem Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                                <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">RequestNo</span>
                                <asp:TextBox ID="txtRequestNo" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Mobile</span>
                                <asp:TextBox ID="txtMobile" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">CardSerial</span>
                                <asp:TextBox ID="txtCardSerial" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">CardCode</span>
                                <asp:TextBox ID="txtCardCode" Text="" runat="server" CssClass="form-control"></asp:TextBox>
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
                    <div class="box-body  no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>Id</th>
                                    <th>IdOrder</th>
                                    <th>TranId</th>
                                    <th>Engine</th>
                                    <th>UserName</th>
                                    <th>OrderNo</th>
                                    <th>FullName</th>
                                    <th>Telco</th>
                                    <th>Mobile</th>
                                    <th>Card Serial</th>
                                    <th>Card Code</th>
                                    <th>Amount</th>
                                    <th>AmountUser</th>
                                    <th>CreateTime</th>
                                    <th>LastTime</th>
                                    <th>Status</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Id")%></td>
                                            <td><%#Eval("IdOrder")%></td>
                                            <td><%#Eval("TransactionID")%></td>
                                            <td><%#Eval("Core")%></td>
                                            <td><%#Eval("UserName")%></td>
                                            <td><a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupList %>?id=<%#Eval("OrderNo") %>"><%#Eval("OrderNo") %></a></td>
                                            <td><%#Eval("FullName")%></td>
                                            <td><%#Eval("Telco")%></td>
                                            <td><a href="<%#SearchUrl(Eval("Mobile").ToString(),Eval("IdOrder").ToString())%>"><%#Eval("Mobile")%></a></td>
                                            <td><%#Eval("CardSerial")%></td>
                                            <td><%#Eval("CardCode")%></td>
                                            <td><%#Eval("Amount")%></td>
                                            <td><%#Eval("AmountUser")%></td>
                                            <td><%#Eval("CreateTime")%></td>
                                            <td><%#Eval("LastTime")%></td>
                                            <td><%#Eval("Status")%></td>
                                            <td>
                                                <a href="<%#DetailUrl(Eval("Id").ToString()) %>">xem</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                    </div>
                    <!-- /.box-body -->
                </div>
                <!-- /.box -->
            </div>
            <!-- /.col -->
        </div>
        <!-- /.row -->
    </section>
    <!-- /.content -->
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript">
        $(function () {
            $(function () {
                $('.txtCreatTime').datetimepicker();
            });
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

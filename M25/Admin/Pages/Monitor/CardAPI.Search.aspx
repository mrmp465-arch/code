<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardAPI.Search.aspx.cs" Inherits="Pages_Monitor_CardAPI_Search" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Thẻ cào
            <small>
                <asp:Label ID="lblTtitle" runat="server" Text="Tra cứu giao dịch nạp thẻ" CssClass="title"></asp:Label>
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
                            <asp:Label runat="server" Text="Tra cứu giao dịch nạp thẻ" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpTop" runat="server" CssClass="form-control">
                                <asp:ListItem Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                                <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">UserName</span>
                                <asp:TextBox ID="txtAccountName" Text="" runat="server" CssClass="form-control"></asp:TextBox>
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
                                <span class="input-group-addon">RefCode</span>
                                <asp:TextBox ID="txtRefCode" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Time</span>
                                <asp:TextBox ID="txtCreatTime" runat="server" CssClass="form-control txtCreatTime"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <% if (AppUtils.IsPartner || AppUtils.IsAdmin)
                                        { %>
                                    <th>Partner</th>
                                    <% } %>
                                    <th>RefCode</th>
                                    <th>AccountName</th>
                                    <th>CardSerial</th>
                                    <th>CardCode</th>
                                    <th>Amount</th>
                                    <th>Amount User</th>
                                    <th>CardType</th>
                                    <% if (AppUtils.IsProvider || AppUtils.IsAdmin)
                                        {%>
                                    <th>Provider</th>
                                    <%}%>
                                    <th>CreatTime</th>
                                    <th>Status</th>
                                    <th>Description</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("TransactionID")%></td>
                                            <% if (AppUtils.IsPartner || AppUtils.IsAdmin)
                                                { %>
                                            <td><%#CheckPartner(Eval("PartnerCode")) %></td>
                                            <% } %>
                                            <td><%#Eval("RequestNo") %></td>
                                            <td><%#Eval("AccountName") %></td>
                                            <td><%#Eval("CardSerial") %></td>
                                            <td><%#Eval("CardCode") %></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Convert.ToInt64(Eval("AmountUser")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Eval("CardType") %></td>
                                            <% if (AppUtils.IsProvider || AppUtils.IsAdmin)
                                                {%>
                                            <td><%#Eval("Provider") %></td>
                                            <%}%>
                                            <td><%#Eval("CreatTime") %></td>
                                            <td><%#Eval("Status") %></td>
                                            <td><a href="<%#DetailUrl(Eval("TransactionID").ToString()) %>">Xem</a>&nbsp; <asp:LinkButton   id="lnCallback" runat="server" OnCommand="Callback_Command" CommandName="Callback" CommandArgument='<%#Eval("TransactionID")%>' Visible='<%# Eval("Status").ToString() == "1" || Eval("Status").ToString() == "2" %>' >Callback </asp:LinkButton> </td>
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
        </div>
        <!-- /.row -->
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
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

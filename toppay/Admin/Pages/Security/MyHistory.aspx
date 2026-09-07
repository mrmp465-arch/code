<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="MyHistory.aspx.cs" Inherits="Pages_Security_MyHistory" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1><%= Resources.Pay.Account%>
            <small><%= Resources.Pay.History%>
            </small>
        </h1>

    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">

                <div class="box">

                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control" ID="drpTop" runat="server">


                                <asp:ListItem Text="50 row" Value="50" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="100 row" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 row" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 row" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpUser" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2" style="display: none">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.Partner%></span>
                                <asp:TextBox CssClass="form-control" ID="txtPartnerCode" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.Keyword%></span>
                                <asp:TextBox CssClass="form-control" ID="txtNote" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;"><%= Resources.Pay.TransactionType%></span>
                                <asp:DropDownList ID="drpType" runat="server" CssClass="form-control ">

                                    <%-- <asp:ListItem Text="Loại" Value="-1"> </asp:ListItem>
                                    <asp:ListItem Text="Cộng tiền" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Trừ tiền" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Rút tiền" Value="3"></asp:ListItem>--%>
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




                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                            <asp:Button ID="btExcel" runat="server" CssClass="btn btn-info" OnClick="ExportTran2_Click" Text="Export Excel"></asp:Button>
                        </div>

                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div class="table-responsive">
                            <div id="dgrid" class="dataTables_wrapper form-inline" role="grid">
                                <table class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th style="width: 10px;">#</th>
                                            <%--<th>UserName</th>--%>
                                            <th><%= Resources.Pay.Partner%></th>
                                            <th>ID</th>
                                            <th><%= Resources.Pay.Money%></th>
                                            <th><%= Resources.Pay.Time%></th>
                                            <th><%= Resources.Pay.Note%></th>
                                            <th><%= Resources.Pay.BalanceBefore%></th>
                                            <th><%= Resources.Pay.BalanceAfter%></th>

                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptList" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td></td>

                                                    <td><%#Eval("UserName") %></td>
                                                    <%--<td><%#Eval("PartnerCode")%></td>--%>
                                                    <td><%#Eval("RefCode")%></td>
                                                    <td><%#InsertCommaMark(Eval("Amount").ToString(),int.Parse(Eval("Type").ToString()),Eval("Note").ToString())%></td>
                                                    <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}") %></td>

                                                    <td><%#Eval("Note")%></td>


                                                    <td><%#Convert.ToInt64(Eval("BalanceBefore")).ToString("#,#").Replace(",", ".") %></td>
                                                    <td><%#Convert.ToInt64(Eval("Balance")).ToString("#,#").Replace(",", ".") %></td>

                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=txtBeginTime.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });
            $('#<%=txtEndTime.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });

        });
        $(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true, "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>
    <style>
        .blue_txt {
            color: #337ab7;
        }

        .red_txt {
            color: red;
        }

        .label {
            width: 76px !important;
            padding: 3px;
            display: block
        }

        .txtCreatTime {
            width: 138px !important;
            padding: 3px !important;
        }
        .table-responsive {
    overflow-x: unset;
}
    </style>
</asp:Content>

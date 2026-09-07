<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="User.Report.aspx.cs" Inherits="Pages_PayGate_User_Report" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Tài khoản
            <small>Báo cáo đối tác cuối ngày
            </small>
        </h1>

    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">

                <div class="box">

                    <div class="box-tools" style="margin-top: 10px;">
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Tài khoản</span>
                                <asp:DropDownList ID="ddlUser" runat="server" CssClass="form-control select2">

                                    <%-- <asp:ListItem Text="Loại" Value="-1"> </asp:ListItem>
                                    <asp:ListItem Text="Cộng tiền" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Trừ tiền" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Rút tiền" Value="3"></asp:ListItem>--%>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <% } %>



                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px">Từ ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtFromDate" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px">Đến ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtCreatTime" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
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
                                    <th>Tài khoản</th>
                                    <th>Ngày</th>
                                    <th> Bank In</th>
                                    <th> Bank Out</th>
                                    <th>Thẻ</th>
                                    <%--<th>Total Card In</th>
                                    <th>Total Card Out</th>--%>
                                    
                                    <th>Nạp số dư</th>
                                    <th>Rút số dư</th>
                                    <th>Số dư đầu</th>
                                    <th>Số dư cuối</th>

                                    <th>Độ lệch</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>

                                            <td><%#Eval("UserName") %></td>
                                            <td><%#formatDay(Eval("Day").ToString())%></td>
                                            <td><%#Convert.ToInt64(Eval("TotalBankIn")).ToString("#,#").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalBankOut")).ToString("#,#").Replace(".", ",") %></td>
                                             <td><%#Convert.ToInt64(Eval("TotalCardIn")).ToString("#,#").Replace(".", ",") %></td>
                                           <%-- <td><%#Convert.ToInt64(Eval("TotalCardIn")).ToString("#,#").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalCardOut")).ToString("#,#").Replace(".", ",") %></td>--%>
                                            <td><%#Convert.ToInt64(Eval("TotalRecharge")).ToString("#,#").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalCash")).ToString("#,#").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceBefore")).ToString("#,#").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceAfter")).ToString("#,#").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("#,#").Replace(".", ",") %></td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
 </div> </div>

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
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>

    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />
    <script type="text/javascript">


        $(document).ready(function () {
            $('#<%=txtFromDate.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });
            $('#<%=txtCreatTime.ClientID %>').datetimepicker({
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
        .label {
            width: 76px !important;
            padding: 3px;
            display: block
        }

        .txtCreatTime {
            width: 138px !important;
            padding: 3px !important;
        }
    </style>

    <style>
        .blue_txt {
            color: #337ab7;
        }

        .red_txt {
            color: red;
        }
                .table-responsive {
    overflow-x: unset;
}
    </style>
</asp:Content>

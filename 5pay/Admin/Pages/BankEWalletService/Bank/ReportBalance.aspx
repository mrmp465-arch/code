<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="ReportBalance.aspx.cs" Inherits="Pages_BankEWalletService_Bank_ReportBalance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Quản trị
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

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">STK/ví </span>
                                <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Loại GD</span>
                                <asp:DropDownList ID="drpType" runat="server" CssClass="form-control ">
                                    <asp:ListItem Text="Loại GD" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Bank" Value="1"></asp:ListItem>
                                   
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Loại</span>
                                <asp:DropDownList ID="drpBankType" runat="server" CssClass="form-control ">
                                    <asp:ListItem Text="Loại" Value=""></asp:ListItem>
                                    <asp:ListItem Text="IN" Value="IN"></asp:ListItem>
                                    <asp:ListItem Text="OUT" Value="OUT"></asp:ListItem>
                                    <asp:ListItem Text="INOUT" Value="INOUT"></asp:ListItem>
                                    <asp:ListItem Text="OUTALL" Value="OUTALL"></asp:ListItem>

                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Trạng thái</span>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control ">

                                    <asp:ListItem Text="Tất cả" Value="-1"></asp:ListItem>
                                    <asp:ListItem Text="Bình thường" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Khóa" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtEndTime" runat="server"></asp:TextBox>
                            </div>
                        </div>




                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4" style="font-weight: bold; font-size: 105%; padding-top: 6px; float: right">
                            <asp:Label runat="server" ID="lblTotal"></asp:Label>
                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>STK/số ví</th>
                                    <th style="width: 10px;">Loại GD</th>
                                    <th>Tên bank</th>
                                    <th>Chủ tài khoản</th>
                                    <th>Loại bank</th>
                                    <th>Trạng thái</th>
                                    <th>Số dư đầu ngày</th>
                                    <th>Số dư cuối ngày</th>
                                    <th>Tổng tiền nạp</th>
                                    <th>Tổng tiền rút</th>
                                    <th>Ngày</th>

                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("BankId") %></td>
                                            <td><%#getType(Eval("Type").ToString())%></td>
                                            <td><%#Eval("BankCode") %></td>
                                            <td><%#Eval("BankName") %></td>
                                            <td><%#Eval("BankType") %></td>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;</td>
                                            <td><%#Convert.ToInt64(Eval("BalanceBefore")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("Balance")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalIn")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("TotalOut")).ToString("N0") %></td>
                                            <td><%#formatDay(Eval("Time").ToString())%></td>


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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('.txtCreatTime').datepicker({
                autoclose: true,
                Accepts: 'bottom'
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
    </style>
</asp:Content>

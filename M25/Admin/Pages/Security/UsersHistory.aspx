<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="UsersHistory.aspx.cs" Inherits="Pages_Security_UsersHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Quản Trị
        <small>Danh sách người dùng
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Người dùng</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
               
                <div class="box">

                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control" ID="drpTop" runat="server">


                                <asp:ListItem Text="50 bản ghi" Value="50" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 bản ghi" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">ParnerCode</span>
                                <asp:TextBox CssClass="form-control" ID="txtPartnerCode" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">Từ khóa</span>
                                <asp:TextBox CssClass="form-control" ID="txtNote" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Loại</span>
                                <asp:DropDownList ID="drpType" runat="server" CssClass="form-control ">

                                    <asp:ListItem Text="Loại" Value="-1"> </asp:ListItem>
                                    <asp:ListItem Text="Cộng tiền" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Trừ tiền" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Rút tiền" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Từ ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtBeginTime" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Đến ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtEndTime" runat="server"></asp:TextBox>
                            </div>
                        </div>




                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>

                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">#</th>
                                    <th>UserName</th>
                                    <th>PartnerCode</th>
                                    <th>Transaction</th>
                                    <th>Số tiền</th>
                                    <th>Thời gian</th>
                                    <th>Ghi chú</th>
                                    <th>Số dư đầu</th>
                                    <th>Số dư sau</th>

                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>

                                            <td><%#Eval("UserName") %></td>
                                            <td><%#Eval("PartnerCode")%></td>
                                            <td><%#Eval("RefCode")%></td>
                                            <td><%#InsertCommaMark(Eval("Amount").ToString(),int.Parse(Eval("Type").ToString()),Eval("Note").ToString())%></td>
                                            <td><%#Eval("CreatedTime")%></td>
                                            <td><%#Eval("Note")%></td>


                                            <td><%#Convert.ToInt64(Eval("BalanceBefore")).ToString("#,#").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("Balance")).ToString("#,#").Replace(".", ",") %></td>

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

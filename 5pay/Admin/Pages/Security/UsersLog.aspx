<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="UsersLog.aspx.cs" Inherits="Pages_Security_UsersLog" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Quản Trị
        <small>Nhật ký người dùng
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Nhật ký Người dùng</li>
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
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpUser" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">

                            <asp:DropDownList ID="drpType" runat="server" CssClass="form-control select2">
                                <asp:ListItem Text="--Hành động--" Value=""> </asp:ListItem>
                                <asp:ListItem Text="Đăng nhập" Value="login"> </asp:ListItem>
                                <asp:ListItem Text="Đổi mật khẩu" Value="changepass"></asp:ListItem>
                                <asp:ListItem Text="Thêm người dùng" Value="useradd"></asp:ListItem>
                                <asp:ListItem Text="Cập nhật người dùng" Value="userupdate"></asp:ListItem>
                                 <asp:ListItem Text="Xóa người dùng" Value="userdelete"></asp:ListItem>
                             <%--   <asp:ListItem Text="Cộng tiền người dùng" Value="usertopup"> </asp:ListItem>
                                <asp:ListItem Text="Trừ tiền người dùng" Value="userdeduct"></asp:ListItem>
                               
                                <asp:ListItem Text="Thêm đối tác" Value="partneradd"></asp:ListItem>

                                <asp:ListItem Text="Xóa đối tác" Value="partnerdelete"> </asp:ListItem>
                                <asp:ListItem Text="Cập nhật đối tác" Value="partnerupdate"></asp:ListItem>--%>
                                <asp:ListItem Text="Cập nhật chiếu khấu" Value="partnereditck"></asp:ListItem>
                                <asp:ListItem Text="Cấu hình hệ thống" Value="systemconfig"></asp:ListItem>
                                <asp:ListItem Text="Cập nhật lệnh nạp bank" Value="bankinupdate"> </asp:ListItem>
                                <asp:ListItem Text="Hủy bankout" Value="bankoutcancel"></asp:ListItem>
                                <asp:ListItem Text="Duyệt tay bankout" Value="bankoutapp"></asp:ListItem>
                                <asp:ListItem Text="Duyệt tự động" Value="bankoutappapi"></asp:ListItem>

                                <asp:ListItem Text="Thêm bank" Value="bankadd"> </asp:ListItem>
                                <asp:ListItem Text="Cập nhật bank" Value="bankupdate"></asp:ListItem>
                                 <asp:ListItem Text="Cập nhật nội dung bank" Value="bankcontentupdate"></asp:ListItem>
                                <asp:ListItem Text="Chuyển tiền bank" Value="banktranfer"></asp:ListItem>
                                 <asp:ListItem Text="Đồng bộ số dư bank" Value="bankupdatebl"></asp:ListItem>
                                <asp:ListItem Text="Xóa bank" Value="bankdelete"></asp:ListItem>
                               <%-- <asp:ListItem Text="Thêm momo" Value="momoadd"> </asp:ListItem>
                                <asp:ListItem Text="Cập nhật momo" Value="momoupdate"></asp:ListItem>
                                 <asp:ListItem Text="Cập nhật nội dung momo" Value="momocontentupdate"></asp:ListItem>
                                <asp:ListItem Text="Xóa momo" Value="momodelete"></asp:ListItem>
                                <asp:ListItem Text="Rút tiền momo" Value="momocash"></asp:ListItem>
                                <asp:ListItem Text="Chuyển tiền momo" Value="momotranfer"></asp:ListItem>--%>
                            </asp:DropDownList>

                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">Từ khóa</span>
                                <asp:TextBox CssClass="form-control" ID="txtNote" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Từ ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtFromDate" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2 ">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Đến ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtCreatTime" runat="server"></asp:TextBox>
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
                                    <th>Hành động</th>
                                    <th>Mô tả</th>
                                 <%--   <th>IP</th>--%>
                                    <th>Thời gian</th>


                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>

                                            <td><%#Eval("UserName") %></td>
                                            <td><%#Eval("ActionName")%></td>
                                            <td><%#Eval("Description")%></td>
                                          <%--  <td><%#Eval("Ip")%></td>--%>
                                            <td><%#Eval("Time", "{0:dd/MM HH:mm:ss}") %>  </td>
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
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
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
</asp:Content>

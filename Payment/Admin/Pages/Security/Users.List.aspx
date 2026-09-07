<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Users.List.aspx.cs" Inherits="Pages_Security_Users_List" %>

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
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Hoạt động</span>
                                <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value="-1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Bình thường" Value="1"> </asp:ListItem>
                                    <asp:ListItem Text="Khóa" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Loại tại khoản</span>
                                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value="-1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Admin" Value="1"> </asp:ListItem>
                                    <asp:ListItem Text="Đối tác" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Săp xếp</span>
                                <asp:DropDownList ID="drpOrder" runat="server" CssClass="form-control select2">

                                    <asp:ListItem Text="Tên" Value="1"> </asp:ListItem>
                                    <asp:ListItem Text="Số dư" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>



                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2" style="float: right; font-weight: bold;">
                            <asp:Label runat="server" ID="lblTotalBalance"></asp:Label>
                        </div>
                    </div>
                <!-- /.box-header -->
                <div class="box-body no-padding">
                     <div style="height: 20px; clear: both;"></div>
                    <table class="table table-striped" id="TableResponsive">
                        <thead>
                            <tr>
                                <th style="width: 10px;">#</th>
                                <th>Tên truy nhập</th>
                                <th>Số dư</th>
                                <th>Loại tài khoản</th>
                                <th>Thời gian tạo</th>
                                <th>Đăng nhập lần cuối</th>
                                
                                <%--<th>Đối ứng</th>--%>
                                <th>Kích hoạt</th>
                                <th>Tác vụ</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptList" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td></td>
                                        <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.UsersEdit %>?id=<%#Eval("UserID") %>"><%#Eval("UserName") %></a></td>
                                         <td><%#Convert.ToInt64(Eval("Balance")).ToString("#,#").Replace(".", ",") %></td>
                                        <td><%#GetUserType(Eval("IsAdmin").ToString(),Eval("IsPartner").ToString())%></td>
                                        <td><%#Eval("CreatedTime")%></td>
                                        <td><%#Eval("LastestTime")%></td>
                                       
                                        <%--<td><%#Convert.ToInt64(Eval("Deposit")).ToString("#,#").Replace(".", ",") %></td>--%>
                                        <td>
                                            <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;</td>
                                        <td>
                                            <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.UsersDelete %>?id=<%#Eval("UserID") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                            <asp:Label ID="lblUserID" runat="server" Visible="false" Text='<%#Eval("UserID") %>'></asp:Label>
                                            |
                                                <a href='<%=Constant.ADMIN_PATH %><%=Resources.Url.UsersHistory %>?name=<%#Eval("UserName") %>'>Lịch sử giao dịch</a>
                                            |
                                                 <a href='<%=Constant.ADMIN_PATH %><%=Resources.Url.UsersTopup %>?name=<%#Eval("UserName") %>'>Cộng tiền</a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <div class="box-footer">
                    <asp:Button ID="btAdd" CssClass="btn btn-info" runat="server" Width="80px" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>
                    <asp:Button ID="btApply" runat="server" Text="Update" CssClass="btn btn-info   pull-right" OnClick="btApply_Click"></asp:Button>
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
    <script type="text/javascript">
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true,
                "autoWidth": false,
                "paging": false,
                "searching": true,
                "info": true,
                "ordering": true,
            });
            
            new $.fn.dataTable.FixedHeader(table);

        });
    </script>
</asp:Content>


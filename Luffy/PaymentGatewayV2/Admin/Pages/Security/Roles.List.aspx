<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Roles.List.aspx.cs" Inherits="Pages_Security_Roles_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Quản trị
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Danh sách Role" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Role</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <!-- /.box-header -->
                    <div class="box-body  no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>Id</th>
                                    <th>Nhóm</th>
                                    <th>Name</th>
                                    <th>Hiển thị Menu</th>
                                    <th>Description</th>
                                    <th>Url</th>

                                    <th>Sắp xếp</th>

                                    <th>Trạng thái</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Id") %></td>
                                            <td><%#Eval("GroupName") %></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.RolesEdit %>?id=<%#Eval("Id") %>"><%#Eval("Name") %></a></td>
                                            <td><%#viewMenu((bool)Eval("IsMenu"),"Hiển thị") %></td>
                                            <td><%#Eval("Description") %></td>
                                            <td><%#Eval("Url") %></td> 
                                            <td><%#Eval("OrderNo") %></td> 
                                            <td><%#viewStatus((int)Eval("Status"),"Sử dụng") %></td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.RolesDelete %>?id=<%#Eval("Id") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a> |
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.RolesEdit %>?id=<%#Eval("Id") %>" title="Xóa">Sửa</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="btAdd" CssClass="btn btn-info" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>
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
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

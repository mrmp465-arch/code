<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Users.List.aspx.cs" Inherits="Pages_Security_Users_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Quản Trị
        <small>Danh sách người dùng
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
            <li class="active">Người dùng</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">#</th>
                                    <th>Tên truy nhập</th>
                                    <th>Họ tên</th>

                                    <th>Thời gian tạo</th>
                                    <th>Đăng nhập lần cuối</th> 
                                      <th>Kích hoạt</th><th>Tác vụ</th> 
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.UsersEdit %>?id=<%#Eval("UserID") %>"><%#Eval("UserName") %></a></td>
                                            <td><%#Eval("FullName") %></td>

                                            <td><%#Eval("CreatedTime")%></td>
                                            <td><%#Eval("LastestTime")%></td>
                                            
                                          
                                             <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;</td>
                                              <td>   
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.UsersDelete %>?id=<%#Eval("UserID") %>"  onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">
                                                Xóa</a>
                                                <asp:Label ID="lblUserID" runat="server" Visible="false" Text='<%#Eval("UserID") %>'></asp:Label>
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


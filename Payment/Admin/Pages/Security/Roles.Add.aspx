<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Roles.Add.aspx.cs" Inherits="Pages_Security_Roles_Add" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Quản trị 
        <small>
            <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.RolesList %>">Danh sách Role</a>
        </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Thêm mới đối tác</h3>
            </div>
            <div class="box-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtName">Name *</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Name *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtDescription">Description </label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" placeholder="Description"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtUrl">Url</label>
                            <asp:TextBox ID="txtUrl" runat="server" CssClass="form-control" placeholder="Url"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtGoup">Nhóm</label>
                            <asp:DropDownList ID="txtGoup" runat="server" OnSelectedIndexChanged="txtGoup_SelectedIndexChanged" CssClass="form-control" AutoPostBack="True">
                                <asp:ListItem Value="0">Dashboard</asp:ListItem>
                              <%--  <asp:ListItem Value="1">Nạp hộ</asp:ListItem>--%>
                                <asp:ListItem Value="2">Gạch thẻ</asp:ListItem>
                                <asp:ListItem Value="3">Mua thẻ</asp:ListItem>
                                <asp:ListItem Value="4">Nhập khoản</asp:ListItem>
                                 <asp:ListItem Value="5">Xuất khoản</asp:ListItem>
                               <%-- <asp:ListItem Value="6">SMS</asp:ListItem>
                                <asp:ListItem Value="7">Kho thẻ</asp:ListItem>--%>
                                 <asp:ListItem Value="8">Bank, Ví</asp:ListItem>
                                <asp:ListItem Value="9">Kết Nối</asp:ListItem>
                                <asp:ListItem Value="10">Cấu hình</asp:ListItem>
                                <asp:ListItem Value="11">Quản Trị</asp:ListItem>
                                <asp:ListItem Value="12">Số dư</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtGoup">Trang Chính</label>
                            <asp:DropDownList ID="txtParentId" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtOrderNo">Xắp xếp</label>
                            <asp:TextBox ID="txtOrderNo" runat="server" CssClass="form-control" placeholder="">0</asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtIsMenu">Hiển thị trên menu</label>
                            <asp:CheckBox ID="txtIsMenu" runat="server" CssClass="form-control" placeholder=""></asp:CheckBox>
                        </div>
                        <div class="form-group">
                            <label for="txtStatus">Trạng thái</label>
                            <asp:DropDownList ID="txtStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Value="1">Sử dụng</asp:ListItem>
                                <asp:ListItem Value="0">Không sử dụng</asp:ListItem>

                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btAdd" runat="server" Text="THÊM" CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
                <asp:Button ID="btCancel" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
            </div>
        </div>

    </section>
</asp:Content>

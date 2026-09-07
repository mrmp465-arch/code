<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Roles.Edit.aspx.cs" Inherits="Pages_Security_Roles_Edit" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Quản trị
        <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.RolesList %>">Danh sách đối tác</a>  </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Cập nhật đối tác</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Thông Tin</a></li>
            </ul>
            <div class="tab-content no-padding">
                <!-- Morris chart - Sales -->
                <div class="chart tab-pane active" id="revenue-chart">
                    <!-- SELECT2 EXAMPLE -->
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
                                    <asp:DropDownList ID="txtGoup" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="txtGoup_SelectedIndexChanged">
                                        <asp:ListItem Value="0">Dashboard</asp:ListItem>
                                        <%--  <asp:ListItem Value="1">Nạp hộ</asp:ListItem>--%>
                                       <%-- <asp:ListItem Value="2">Gạch thẻ</asp:ListItem>
                                        <asp:ListItem Value="3">Mua thẻ</asp:ListItem>--%>
                                        <asp:ListItem Value="4">Nạp bank</asp:ListItem>
                                        <asp:ListItem Value="5">Rút bank</asp:ListItem>
                                        <%-- <asp:ListItem Value="6">SMS</asp:ListItem>
  <asp:ListItem Value="7">Kho thẻ</asp:ListItem>--%>
                                        <asp:ListItem Value="8">Momo Core</asp:ListItem>
                                        <asp:ListItem Value="9">Bank Core</asp:ListItem>
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
                        <asp:Button ID="btUpdate" runat="server" Text="CẬP NHẬT" CssClass="btn btn-info " OnClick="btUpdate_Click"></asp:Button>
                        <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>


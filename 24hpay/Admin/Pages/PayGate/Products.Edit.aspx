<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Products.Edit.aspx.cs" Inherits="Pages_PayGate_Products_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Kết nối
            <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProductList %>">Danh sách Sản phẩm
            </a></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Sản phẩm chi tiết</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Sửa mới Sản phẩm
                    <%if (Type == "7")
                        {%>
                        Thẻ
                    <% }
                        else if (Type == "15")
                        { %> 
                            Mua Thẻ mã thẻ
                    <% }
                        else if (Type == "5")
                        { %> 
                        Topup TK Mobile
                    <% }
                        else if (Type == "18")
                        { %> 
                            Ngân hàng Cash
                    <% }
                        else if (Type == "13")
                        { %>
                        Ngân hàng
                    <% }%>
                </h3>
            </div>

            <div class="box-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtName">Tên Sản phẩm </label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên dịch vụ *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtCode">Mã Sản phẩm</label>
                            <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" placeholder="Code"></asp:TextBox>
                        </div>
                        <div class="checkbox">
                            <label for="txtName">
                                <asp:CheckBox ID="chkIsActive" Checked="true" runat="server"></asp:CheckBox>Kích hoạt
                            </label>
                        </div>
                    </div>
                    <!-- /.col -->
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtType">Loại</label>
                            <asp:DropDownList ID="txtType" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Gạch thẻ" Value="7"></asp:ListItem>
                                <asp:ListItem Text="Mua mã Thẻ" Value="15"></asp:ListItem>
                                <asp:ListItem Text="Topup TK Mobile" Value="5"></asp:ListItem>
                                <asp:ListItem Text="Ngân Hàng" Value="13"></asp:ListItem>
                                <asp:ListItem Text="Ngân hàng Cash" Value="18"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtSubType">SubType</label>
                            <asp:TextBox ID="txtSubType" runat="server" CssClass="form-control SubType" placeholder="SubType"></asp:TextBox>
                        </div>
                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btUpdate" runat="server" Text="CẬP NHẬT" CssClass="btn btn-info " OnClick="btUpdate_Click"></asp:Button>
                <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
        $(function () {
            $(".SubType,.txtQuota,.txtSignatureType").attr("type", "number");
        });
    </script>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Products.Add.aspx.cs" Inherits="Pages_PayGate_Products_Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Kết nối
        <small>
            <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProductList %>">Danh sách Sản phẩm 
            </a>
        </small>
        </h1>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Thêm mới Sản phẩm
                    <%if (Type == "7")
                    {%>
                        Thẻ
                    <% }
                    else if (Type == "14")
                    { %> 
                            Mua Thẻ & Topup
                    <% }
                         else if (Type == "18")
                    { %> 
                            Ngân hàng Cash
                    <% }
                        else if (Type == "13") { %>
                        Ngân hàng
                    <% }%>
                </h3>
            </div>
            <div class="box-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtName">Tên Sản phẩm</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên dịch vụ *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtProductCode">Mã Sản phẩm</label>
                            <asp:TextBox ID="txtProductCode" runat="server" CssClass="form-control" placeholder="ProductCode"></asp:TextBox>
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
                                <asp:ListItem Text="Mua Thẻ & Topup" Value="14"></asp:ListItem>
                                <asp:ListItem Text="Ngân Hàng" Value="13"></asp:ListItem>
                                 <asp:ListItem Text="Ngân hàng Cash" Value="18"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtSubType">SubType</label>
                            <asp:TextBox ID="txtSubType" runat="server" CssClass="form-control" placeholder="SubType"></asp:TextBox>
                        </div>

                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btAdd" runat="server" Text="THÊM" CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
                <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
            </div>
        </div>

    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
        $(function () {
            $(".txtOderNo,.txtQuota,.txtSignatureType").attr("type", "number");
        });
    </script>
</asp:Content>

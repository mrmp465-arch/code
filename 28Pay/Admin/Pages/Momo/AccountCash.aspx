<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="AccountCash.aspx.cs" Inherits="Pages_Momo_AccountCash" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Quản lý đối tác" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Quản lý tài khoản rút tiền</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>MOMO</li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoAccount %>">Tài khoản momo</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoPartner %>" data-toggle="tab">Quản lý đối tác</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerMomo %>">Phân bố đối tác & Tài khoản</a></li>
                <li class="active"><a href="#">Tài khoản rút tiền</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title">Thêm mới tài khoản</h3>
                        </div>

                        <div class="box-body">
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="txtClassName">Ngân hàng</label>
                                        <asp:DropDownList ID="drpBankCode" runat="server" CssClass="form-control select2">
                                            <asp:ListItem Value="" Selected disabled hidden>Chọn ngân hàng</asp:ListItem>
                                            <asp:ListItem Value="ACB">ACB - Ngân hàng TMCP Á Châu</asp:ListItem>
                                            <asp:ListItem Value="BIDV">BIDV - Ngân hàng TMCP Đầu tư và Phát triển Việt Nam</asp:ListItem>
                                            <asp:ListItem Value="MB">MBBank - Ngân hàng TMCP Quân đội</asp:ListItem>
                                            <asp:ListItem Value="TCB">Techcombank - Ngân hàng TMCP Kỹ thương Việt Nam</asp:ListItem>
                                            <asp:ListItem Value="TPB">TPBank - Ngân hàng TMCP Tiên Phong</asp:ListItem>
                                            <asp:ListItem Value="VCB">Vietcombank - Ngân hàng TMCP Ngoại Thương Việt Nam</asp:ListItem>
                                            <asp:ListItem Value="ICB">VietinBank - Ngân hàng TMCP Công thương Việt Nam</asp:ListItem>
                                            <asp:ListItem Value="VPB">VPBank - Ngân hàng TMCP Việt Nam Thịnh Vượng</asp:ListItem>

                                        </asp:DropDownList>
                                    </div>


                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="txtDescription">Số tài khoản*</label>
                                        <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                    </div>
                                </div>

                                <div class="col-md-3">


                                    <div class="form-group">
                                        <label for="txtDescription">Tên tài khoản *</label>
                                        <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                    </div>



                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="txtDescription">Trạng thái</label>
                                        <div class="checkbox">
                                            <label for="cbxIsActive">
                                                <asp:CheckBox ID="chkIsActive" Checked="False" runat="server"></asp:CheckBox>
                                            </label>
                                        </div>
                                    </div>
                                </div>

                                <!-- /.col -->
                            </div>
                            <!-- /.row -->
                        </div>
                        <div class="box-footer">
                            <asp:Button ID="btAdd" CssClass="btn btn-info pull-right" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>
                        </div>

                    </div>
                    <div class="box">
                        <!-- /.box-header -->
                        <div class="box-header with-border">
                            <h3 class="box-title">Danh sách tài khoản</h3>
                        </div>
                        <div class="box-body  no-padding">
                            <div style="height: 20px; clear: both;"></div>
                            <table class="table table-striped" id="TableResponsive">
                                <thead>
                                    <tr>
                                        <th>Stt</th>
                                        <th>BankCode</th>
                                        <th>Số tài khoản</th>
                                        <th>Tên tài khoản</th>

                                        <th>Status</th>
                                        <th>Tác vụ</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptList" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Container.ItemIndex + 1 %></td>

                                                <td><%#Eval("BankCode") %></td>
                                                <td><%#Eval("AccountNumber") %></td>
                                                <td><%#Eval("AccountName") %></td>
                                                <td>
                                                    <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                                </td>

                                                <td>
                                                    <asp:LinkButton ID="lnDelete" runat="server" OnClientClick="return confirm('Bạn có muốn xóa?')" OnCommand="Delete_Command" CommandName="Delete" CommandArgument='<%#Eval("Id")%>'>Xóa </asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                        <div class="box-footer">

                            <asp:Button ID="Button5" runat="server" Text="Cập nhật" CssClass="btn btn-info pull-right " OnClick="btApply_Click"></asp:Button>
                        </div>
                    </div>
                    <!-- /.box-body -->
                    <
        <!-- /.box -->

                    <!-- /.col -->
                </div>
            </div>
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


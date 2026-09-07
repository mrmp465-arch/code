<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Account.Add.aspx.cs" Inherits="Pages_BankEWalletService_Bank_Account_Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="MAlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertBans"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger bg-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>

    <div class="modal  fade bs-example-modal-sm" id="MAlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình Bank
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Tài khoản Bank" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Thêm mới tài khoản Bank</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Bank</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Tài khoản Bank</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankPartner %>">Quản lý đối tác</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerBank %>">Phân bố đối tác & tài khoản</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">

                    <div class="col-md-6">
                        <br />
                        <br />
                        <div class="form-group">
                            <label for="txtClassName">Bank Code *</label>
                            <asp:DropDownList ID="drpBankCode" runat="server" CssClass="form-control" required>
                                <asp:ListItem Text="Chọn Bank:" Value=""></asp:ListItem>
                                <asp:ListItem Text="ACB" Value="ACB"></asp:ListItem>
                                <asp:ListItem Text="VPB" Value="VPB"></asp:ListItem>
                                <asp:ListItem Text="SEAB" Value="SEAB"></asp:ListItem>
                                <asp:ListItem Text="ICB" Value="ICB"></asp:ListItem>
                                <asp:ListItem Text="MB" Value="MB"></asp:ListItem>
                                <asp:ListItem Text="VCB" Value="VCB"></asp:ListItem>
                                <asp:ListItem Text="BIDV" Value="BIDV"></asp:ListItem>
                                <asp:ListItem Text="TIMO" Value="TIMO"></asp:ListItem>
                                <asp:ListItem Text="OCB" Value="OCB"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtClassName">Bank Type *</label>
                            <asp:DropDownList ID="drpBankType" runat="server" CssClass="form-control" required>
                                <asp:ListItem Text="Chọn loại:" Value=""></asp:ListItem>
                                <asp:ListItem Text="Cá nhân" Value="IND"></asp:ListItem>
                                <asp:ListItem Text="Doanh nghiệp" Value="BIZ"></asp:ListItem>

                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtBankName">Tên tài khoản *</label>
                            <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control" placeholder="Tên tài khoản" required></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtName">Bank Id *</label>
                            <asp:TextBox ID="txtBankId" runat="server" CssClass="form-control" placeholder="Số tài khoản" required></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="txtName">Tên đăng nhập *</label>
                            <asp:TextBox ID="txtBankAccount" runat="server" CssClass="form-control" placeholder="Tài khoản đăng nhập App" required></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtBankPass">Mật khẩu *</label>
                            <asp:TextBox ID="txtBankPass" TextMode="Password" runat="server" CssClass="form-control" placeholder="Mật khẩu" required></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtBalanceMaxDay">Số tiền nhận tối đa trong ngày (triệu) *</label>
                            <asp:TextBox ID="txtBalanceMaxDay" runat="server" CssClass="form-control" placeholder="Số tiền nhận tối đa trong ngày" required></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtBalanceMaxMonth">Số tiền nhận tối đa trong tháng (triệu) *</label>
                            <asp:TextBox ID="txtBalanceMaxMonth" runat="server" CssClass="form-control" placeholder="Số tiền nhận tối đa trong tháng" required></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtClassName">Loại  *</label>
                            <asp:DropDownList ID="drpType" runat="server" CssClass="form-control" required>
                                <asp:ListItem Text="INOUT" Value="INOUT"></asp:ListItem>
                                <asp:ListItem Text="IN" Value="IN"></asp:ListItem>
                                <asp:ListItem Text="OUT" Value="OUT"></asp:ListItem>
                                <asp:ListItem Text="OUTALL" Value="OUTALL"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtSolution">Giải pháp  *</label>
                            <asp:DropDownList ID="drpSolution" runat="server" CssClass="form-control" required>
                                <%-- <asp:ListItem Text="Chọn giải pháp:" Value=""></asp:ListItem>--%>
                                <asp:ListItem Text="API" Value="API" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="LD" Value="LD"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtComputer">Computer</label>
                            <asp:TextBox ID="txtComputer" runat="server" CssClass="form-control" placeholder="Tên máy tính chứa Client"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtPhone">Phone</label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Tên điện thoại cài app"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtPhone">CloudPhoneId</label>
                            <asp:TextBox ID="txtCloudPhoneId" runat="server" CssClass="form-control" placeholder="CloudPhoneId"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtPhone">Pin OTP</label>
                            <asp:TextBox ID="txtPinOtp" runat="server" CssClass="form-control" placeholder="Mã PIN của OTP"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtPhone">AppDeviceId</label>
                            <asp:TextBox ID="txtAppDeviceId" runat="server" CssClass="form-control" placeholder="AppDeviceId"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtPhone">Ghi chú</label>
                            <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" placeholder="Ghi chú"></asp:TextBox>
                        </div>
                        <div class="checkbox">
                            <label for="cbxIsActive">
                                <asp:CheckBox ID="chkIsActive" Checked="False" runat="server"></asp:CheckBox>Trạng thái
                            </label>
                        </div>
                        <div class="box-footer">
                            <asp:Button ID="btAdd" runat="server" Text="THÊM" CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
                            <asp:Button ID="btCancel" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
            <!-- /.box -->
        </div>
        <!-- /.col -->
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

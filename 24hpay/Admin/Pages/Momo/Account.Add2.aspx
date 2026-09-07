<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Account.Add2.aspx.cs" Inherits="Pages_Momo_Account_Add2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
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
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Tài khoản momo" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Thêm mới tài khoản momo</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>MOMO</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Tài khoản momo</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoPartner %>">Quản lý kênh</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerMomo %>">Phân bố kênh & tài khoản</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">

                    <div class="col-md-6">
                        <br />
                        <br />
                        <div class="form-group">
                            <label for="txtName">Số điện thoại *</label>
                            <asp:TextBox ID="txtMomoId" runat="server" CssClass="form-control" placeholder="Số điện thoại" required></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtMomoName">Tên tài khoản *</label>
                            <asp:TextBox ID="txtMomoName" runat="server" CssClass="form-control" placeholder="Tên tài khoản" required></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="txtMomoPass">Mật khẩu *</label>
                            <asp:TextBox ID="txtMomoPass" TextMode="Password" runat="server" CssClass="form-control" placeholder="Mật khẩu" required></asp:TextBox>
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
                                <asp:ListItem Text="Chọn giải pháp:" Value=""></asp:ListItem>
                                <asp:ListItem Text="APIV2" Value="APIV2" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="API" Value="API"></asp:ListItem>
                            </asp:DropDownList>
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

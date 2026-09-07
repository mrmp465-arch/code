<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Providers.Edit.Discount.aspx.cs" Inherits="Pages_PayGate_Provider_Edit_Discount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <div class="modal  fade bs-example-modal-sm" id="AlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertBans"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <section class="content-header">
            <h1>Kết nối
            <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderList %>">Danh sách nhà cung cấp
            </a></small>
            </h1>
            <ol class="breadcrumb">
                <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
                <li class="active">Nhà cung cấp chi tiết</li>
            </ol>
        </section>
        <!-- Main content -->
        <section class="content">
            <div class="nav-tabs-custom">
                <ul class="nav nav-tabs pull-left">
                    <li class="pull-left header"><i class="fa fa-inbox"></i>
                        Sửa nhà cung cấp
                        <asp:Label ID="lblProviderCode" runat="server"></asp:Label>
                        -
                        <asp:Label ID="lblProviderId" runat="server"></asp:Label>
                    </li>
                    <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEdit%>?id=<%=lblProviderId.Text%>&code=<%=lblProviderCode.Text%>&tab=info">Thông Tin</a></li>
                    <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEdit%>?id=<%=lblProviderId.Text%>&code=<%=lblProviderCode.Text%>&tab=product">Sản phẩm</a></li>
                    <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEdit%>?id=<%=lblProviderId.Text%>&code=<%=lblProviderCode.Text%>&tab=partner">Đối tác</a></li>
                    <li class="active"><a href="#discount" data-toggle="tab">Chiết khấu</a></li>
                </ul>
                <div class="tab-content no-padding">
                    <div class="chart tab-pane active" id="discount">
                        <div class="box-body">
                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <div class="form-group">
                                    <div class="input-group date">
                                        <div class="input-group-addon">
                                            <i class="fa fa-calendar"></i>
                                        </div>
                                        <asp:TextBox ID="datepicker" runat="server" class="form-control"> </asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-4">
                                <asp:Button ID="btnDiscountView" runat="server" Text="Xem Discount" CssClass="btn btn-primary pull-left" OnClick="btnDiscountView_Click"></asp:Button>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <asp:FileUpload ID="fileUploadExcel" runat="server" CssClass="btn btn-default" />
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-1">
                                <asp:Button ID="btnUploadExcel" runat="server" Text="Import Excel" CssClass="btn btn-primary pull-left" OnClick="btnUploadExcel_Click"></asp:Button>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-3">
                                (Có thể upload cho toàn bộ các mã Partner)
                            </div>
                            <div class="col-sm-12">
                                <div class="box-body table-responsive no-padding">
                                    <table class="table table-condensed">
                                        <tbody>
                                            <tr>
                                                <th>Ngày</th>
                                                <th>Discount</th>
                                                <th>Reward</th>
                                            </tr>
                                            <asp:Repeater ID="rptDiscount" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td><%#Eval("Time") %></td>
                                                        <td>
                                                            <asp:TextBox ID="txtDiscount" runat="server" CssClass="form-control" Text='<%#Eval("Discount") %>'></asp:TextBox></td>
                                                        <td>
                                                            <asp:TextBox ID="txtReward" runat="server" CssClass="form-control" Text='<%#Eval("Reward") %>'></asp:TextBox></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <!-- /.tab-pane -->

                            <!-- /.tab-content -->
                        </div>
                        <div class="box-footer">
                            <asp:Button ID="btnUpdateDiscount" runat="server" Text="CẬP NHẬT" CssClass="btn btn-primary" OnClick="btnUpdateDiscount_Click"></asp:Button>
                        </div>

                    </div>
                </div>
            </div>
        </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $(".txtOrderNo,.txtQuota,.txtSignatureType").attr("type", "number");
        });

        $(function () {
            $(function () {
                //Date picker
                $('#<%=datepicker.ClientID%>').datepicker({
                    orientation: "bottom auto",
                    autoclose: true,
                    format: "mm-yyyy",
                    startView: 1,
                    minViewMode: 1
                });
            });
        });

    </script>
</asp:Content>

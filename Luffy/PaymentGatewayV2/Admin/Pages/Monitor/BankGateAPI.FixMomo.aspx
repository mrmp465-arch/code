<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankGateAPI.FixMomo.aspx.cs" Inherits="Pages_Monitor_BankGateAPI_FixMomo" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
        <h1>Ngân hàng
        <small>Sửa thông tin giao dịch</small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Sửa lỗi</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">

        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Sửa thông tin giao dịch</h3>
            </div>
            <div class="box-body">
                <div class="row">

                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtName">TransactionID *</label>
                            <div class="input-group input-group-sm">
                                <asp:TextBox ID="txtTransactionID" ReadOnly="true" Text="0" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:HiddenField  runat="server" ID="hdPartner" />
                            </div>

                        </div>
                       <div class="form-group">
                            <label for="drpPartner">Đối tác</label>
                             <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtRefCode">MomoId</label>
                            <asp:TextBox ID="txtOrderInfo" runat="server" CssClass="form-control" placeholder="RefCode"></asp:TextBox>
                        </div>
                        
                         <div class="form-group">
                            <label for="txtLogContent">Nội dung</label>
                            <asp:TextBox ID="txOrderNo" runat="server" CssClass="form-control" placeholder="Ghi chú"></asp:TextBox>
                        </div>
                        
                    </div>
                    <div class="col-md-6">
                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-info" OnClick="btUpdate_Click"></asp:Button>
            </div>
        </div>
    </section>
</asp:Content>

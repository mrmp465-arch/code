<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="ChangePassword.aspx.cs" Inherits="Pages_Security_ChangePassword" %>  
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
        <h1>Tài Khoản
        <small>Đổi mật khẩu
        </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">

        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Đổi mật khẩu</h3>
            </div>
            <div class="box-body">
                <div class="row"> 
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtName">Mật khẩu cũ *</label>
                            <asp:TextBox ID="txtPasswordOld" TextMode="Password" runat="server" CssClass="form-control" placeholder="Mật khẩu cũ *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtServiceCode">Mật khẩu mới *</label>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Mật khẩu mới *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtDescription">Nhập lại mật khẩu *</label>
                            <asp:TextBox ID="txtPasswordAgain" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập lại mật khẩu *"></asp:TextBox>
                            
                        </div>
                        <div>

                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="Mật khẩu tối thiểu phải có 6 ký tự"
                                Display="None" ControlToValidate="txtPassword" ValidationExpression=".{6}.*"></asp:RegularExpressionValidator>
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender2"
                                Width="160px" HighlightCssClass="validatorCalloutHighlight" TargetControlID="RegularExpressionValidator2" WarningIconImageUrl="" />
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPassword"
                                ControlToValidate="txtPasswordAgain" Display="None" ErrorMessage="Mật khẩu nhật lại không khớp"></asp:CompareValidator>
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender3"
                                HighlightCssClass="validatorCalloutHighlight" TargetControlID="CompareValidator1" WarningIconImageUrl="" />
                        </div>
                    </div>
                    <!-- /.col -->
                    <div class="col-md-6">
                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btSubmit" runat="server" Text="ĐỔI" CssClass="btn btn-info " OnClick="btSubmit_Click"></asp:Button>
            </div>
        </div>

    </section>
</asp:Content> 
 


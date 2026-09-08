<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Payments.Add.aspx.cs" Inherits="Pages_PayGate_Payments_Add" %>
 
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
        <h1>Dịch Vụ
        <small> 
            <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PaymentsList %>">
                Danh sách dịch vụ 
            </a>
        </small>
        </h1> 
    </section>

    <!-- Main content -->
    <section class="content">

        <div class="box box-primary"> 
             <div class="box-header with-border">
                <h3 class="box-title">Thêm mới dịch vụ</h3>
            </div>
            <div class="box-body">
                <div class="row">

                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtName">Tên dịch vụ *</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên dịch vụ *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtServiceCode">Mã dịch vụ *</label>
                            <asp:TextBox ID="txtServiceCode" runat="server" CssClass="form-control" placeholder="Mã dịch vụ *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtDescription">Chú thích</label>
                            <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" CssClass="form-control" placeholder="Chú thích"></asp:TextBox>
                            <small>Thông tin ghi chú nếu cần thiết</small>
                        </div>



                    </div>
                    <!-- /.col -->
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtClassName">Tên lớp thư viện * </label>
                            <asp:TextBox ID="txtClassName" runat="server" CssClass="form-control" placeholder="Tên lớp  thư viện *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                          <label for="exampleInputFile">File thư viện *</label> 
                              <asp:FileUpload ID="fileUploadClass" runat="server" ></asp:FileUpload> 
                        </div> 
                        <div class="form-group">
                            <label for="txtConfig">Cấu hình </label>
                            <asp:TextBox ID="txtConfig" TextMode="MultiLine" runat="server" CssClass="form-control" placeholder="Cấu hình"></asp:TextBox>
                            <small>Thông tin cấu hình nếu có</small>
                        </div>
                        <div class="checkbox">
                            <label for="cbxIsActive">
                                <asp:CheckBox ID="cbxIsActive" Checked="true" runat="server"></asp:CheckBox>Kích hoạt
                            </label>
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

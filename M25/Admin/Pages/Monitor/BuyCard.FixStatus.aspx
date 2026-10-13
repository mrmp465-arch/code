<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BuyCard.FixStatus.aspx.cs" Inherits="Pages_Monitor_BuyCard_FixStatus" %> 

 
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
        <h1>Mua thẻ & Topup
        <small>Sửa thông tin giao dịch thẻ cào</small>
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
                <h3 class="box-title"><asp:Label ID="lblTtitle" runat="server" Text="Sửa thông tin giao dịch thẻ cào" CssClass="title"></asp:Label></h3>
            </div>
            <div class="box-body">
                <div class="row">

                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtTransactionID">TransactionID *</label>
                            <div class="input-group input-group-sm">
                                <asp:TextBox ID="txtTransactionID" Text="0" runat="server" CssClass="form-control"></asp:TextBox> 
                                    <span class="input-group-btn">
                                     <asp:Button ID="btView" runat="server" CssClass="btn btn-info btn-flat" OnClick="btView_Click" Text="Xem"></asp:Button>
                                    </span>
                              </div>
                           
                            
                        </div> 
                            <div class="form-group">
                            <label for="txtCardSerial">CardSerial</label>
                            <asp:TextBox ID="txtCardSerial" runat="server"  ReadOnly="true" CssClass="form-control" placeholder="CardSerial"></asp:TextBox>
                        </div> 
                        <div class="form-group">
                            <label for="txtCardCode">CardCode</label>
                            <asp:TextBox ID="txtCardCode" runat="server"  ReadOnly="true" CssClass="form-control" placeholder="CardCode"></asp:TextBox>
                        </div>  
                    </div>  
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtStatus">Status</label>
                            <asp:TextBox ID="txtStatus" runat="server"   CssClass="form-control" placeholder="Status"></asp:TextBox> 
                        </div>  
                        <div class="form-group">
                            <label for="LastTime">Amount</label>
                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Amount"></asp:TextBox>
                        </div> 
                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-info " OnClick="btUpdate_Click"></asp:Button> 
            </div>
        </div> 
    </section> 
</asp:Content>
 
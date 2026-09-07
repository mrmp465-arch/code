<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="F2A.aspx.cs" Inherits="Pages_Security_F2A" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertBans"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>

    <div class="modal  fade bs-example-modal-sm" id="MAlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1><%= Resources.Pay.Account%>
            <small><%= Resources.Pay._2FASetup%>
            </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">

        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title"><%= Resources.Pay._2FASetup%></h3>
            </div>
            <div class="box-body">
                <div class="row" id="dvFA" runat="server" visible="false">
                    <div class="col-md-6">
                        <div class="form-group">
                            <div style="text-align: center">
                                <img src="<%=BarcodeImageUrl %>" width="200" alt="" />
                            </div>

                        </div>
                        <div class="form-group">
                            <label for="txtName"><%= Resources.Pay._2FACode%> </label>
                            <asp:TextBox ID="txtOTP" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>


                    </div>
                    <!-- /.col -->
                    <div class="col-md-6">
                        <h2><%= Resources.Pay.Help%></h2>
                        1. <%= Resources.Pay.Help1%><br />
                        2. <%= Resources.Pay.Help2%>
                        <br />
                        3. <%= Resources.Pay.Help3%><br />
                        4. <%= Resources.Pay.Help4%>
                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btSubmit" runat="server" Text="Xác nhận" CssClass="btn btn-info " OnClick="btSubmit_Click"></asp:Button>
            </div>
        </div>
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Cài đặt</h3>
            </div>
            <div class="box-body">
                <div class="row" id="Div1" runat="server" visible="true">
                    <div class="col-md-6">

                        <div class="form-group">
                            <label for="txtName">Số tiền duyệt đơn rút</label>
                            <asp:TextBox ID="txtMinBankAproveAmount" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
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
                <asp:Button ID="Button1" runat="server" Text="Cập nhật" CssClass="btn btn-info " OnClick="btSubmit2_Click"></asp:Button>
            </div>
        </div>
    </section>
</asp:Content>

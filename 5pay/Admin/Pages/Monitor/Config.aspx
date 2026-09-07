<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Config.aspx.cs" Inherits="Pages_Monitor_Config" %>

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
            <div class="alert alert-danger alert-dismissible">
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
        <h1>Cấu hình
        <small>
            <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersList %>">Cấu hình hệ thống</a>
        </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Đóng mở hệ thống</h3>
            </div>
            <div class="box-body">
                <div class="row">

                    <div class="col-md-2">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox ID="ddlBankCashEnable" runat="server"></asp:CheckBox>
                                    <asp:HiddenField ID="hdBankCashEnable" runat="server" />
                                </span>
                                <span class="form-control" style="font-weight: bold">Rút bank</span>

                            </div>

                        </div>


                    </div>


                    <!-- /.col -->
                    <div class="col-md-2">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox ID="ddlBankInEnable" runat="server"></asp:CheckBox>
                                    <asp:HiddenField ID="hdBankInEnable" runat="server" />
                                </span>
                                <span class="form-control" style="font-weight: bold">Nạp bank</span>

                            </div>

                        </div>

                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox ID="ddlMomoCashEnable" runat="server"></asp:CheckBox>

                                </span>
                                <span class="form-control" style="font-weight: bold">Rút số dư</span>

                            </div>

                        </div>


                    </div>
                    <div class="col-md-2" style="display: none">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox ID="ddlMomoInEnable" runat="server"></asp:CheckBox>

                                </span>
                                <span class="form-control" style="font-weight: bold">Nạp momo</span>

                            </div>

                        </div>

                    </div>
                    <div class="col-md-2" style="display: none">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox ID="ddlBankApp" runat="server"></asp:CheckBox>
                                    <asp:HiddenField ID="hdlBankApp" runat="server" />
                                </span>
                                <span class="form-control" style="font-weight: bold">Nạp GPay</span>

                            </div>

                        </div>

                    </div>
                    <div class="col-md-2" style="display: none">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox ID="chkCas" runat="server"></asp:CheckBox>
                                    <asp:HiddenField ID="hdCas" runat="server" />
                                </span>
                                <span class="form-control" style="font-weight: bold">Nạp Cas</span>

                            </div>

                        </div>

                    </div>
                    <div class="col-md-2" style="display: none">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox ID="chkVPB" runat="server"></asp:CheckBox>

                                </span>
                                <span class="form-control" style="font-weight: bold">Ưu tiên VPB</span>

                            </div>

                        </div>

                    </div>
                    <div style="clear: both">
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtBankCodeInMaintain">Danh sách bank bảo trì nạp(ACB,MB,BIDV,VCB)</label>
                            <asp:TextBox ID="txtBankCodeInMaintain" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <div style="clear: both"></div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtBankPrefix">Nội dung chuyển tiền</label>
                            <asp:TextBox ID="txtBankPrefix" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <div style="clear: both"></div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtService">Dịch vụ backup IN(fast)</label>
                            <asp:TextBox ID="txtService" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <div style="clear: both">
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtService">Dịch vụ backup  OUT (fast)</label>
                            <asp:TextBox ID="txtServiceOUT" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <div style="clear: both">
                    </div>
                    <div class="col-md-12">
                        <div class="form-group">
                            <label for="txtBankPrefix">Block Accounts</label>
                            <asp:TextBox ID="txtBlockAccount" runat="server" TextMode="MultiLine" Rows="6" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <div style="clear: both"></div>
                    <%--  <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtBankPrefix">Nội dung notify</label>
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <div style="clear: both"></div>
                    <div class="col-md-2">
                           <asp:Button ID="btSend" runat="server" Text="Gửi thông báo" CssClass="btn btn-info  pull-right" OnClick="btSend_Click"  Visible="false"></asp:Button>
                    </div>--%>
                    <!-- /.col -->
                </div>
                <!-- /.row -->

                <div class="box-footer">
                    <asp:Button ID="btAdd" runat="server" Text="Cập nhật" CssClass="btn btn-info  pull-right" OnClick="btAdd_Click"></asp:Button>

                </div>
            </div>
        </div>
    </section>
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Gửi thông báo</h3>
            </div>
            <div class="box-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtBankPrefix">Nội dung thông báo</label>
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="" TextMode="MultiLine" Rows="6"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox ID="cbxParnterall" CssClass="CheckAllBlock" onclick="checkAll(this);" Checked="true" runat="server"></asp:CheckBox>

                                </span>
                                <span class="form-control">Chọn/Bỏ chọn All</span>

                            </div>

                        </div>
                    </div>
                    <asp:Repeater ID="rptList2" runat="server">
                        <ItemTemplate>
                            <div class="col-md-1">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxParnter" CssClass="CheckAll" Checked="true" runat="server"></asp:CheckBox>
                                            <asp:Label ID="lblPartnerID" runat="server" Visible="false" Text='<%#Eval("PartnerID") %>'></asp:Label>
                                        </span>
                                        <span class="form-control"><%#Eval("Name") %></span>

                                    </div>

                                </div>

                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <!-- /.col -->
                </div>
                <!-- /.row -->

                <div class="box-footer">
                    <asp:Button ID="btSend" runat="server" Text="Gửi thông báo" CssClass="btn btn-info  pull-right" OnClick="btSend_Click" Visible="true"></asp:Button>

                </div>
            </div>
        </div>
    </section>
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Danh sách bank rút</h3>
            </div>
            <div class="box-body">
                <div class="row">
                    <asp:Repeater ID="rptList" runat="server">
                        <ItemTemplate>
                            <div class="col-md-2">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxStatus" Checked='<%#Convert.ToBoolean(Eval("isTransfer")) %>' runat="server"></asp:CheckBox>
                                            <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("id") %>'></asp:Label>
                                        </span>
                                        <span class="form-control"><%#Eval("shortName") %></span>

                                    </div>

                                </div>

                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <!-- /.col -->
                </div>
                <!-- /.row -->

                <div class="box-footer">
                    <asp:Button ID="Button2" runat="server" Text="Cập nhật" CssClass="btn btn-info  pull-right" OnClick="btAdd_Click3"></asp:Button>

                </div>
            </div>
        </div>
    </section>
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Cấu hình khác</h3>
            </div>
            <div class="box-body">
                <div class="row">



                    <div class="col-md-2">
                        <div class="form-group">
                            <label for="txtName">Số tiền rút bank tối thiểu</label>
                            <asp:TextBox ID="txtMinBankCashAmount" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <!-- /.col -->


                    <div class="col-md-2">
                        <div class="form-group">
                            <label for="txtName">Số tiền rút bank tối đa</label>
                            <asp:TextBox ID="txtMaxBankCashAmount" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            <label for="txtName">Số tiền duyệt đơn rút</label>
                            <asp:TextBox ID="txtMinBankAproveAmount" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-2" style="">

                        <div class="form-group">
                            <label for="txtName">Số tiền rút số dư tối thiểu</label>
                            <asp:TextBox ID="txtMinMomoCashAmount" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>

                    </div>
                    <div class="col-md-2">

                        <div class="form-group">
                            <label for="txtName">Số tiền tối đa sửa đơn</label>
                            <asp:TextBox ID="txtMaxMomoCashAmount" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>

                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->

                <div class="box-footer">
                    <asp:Button ID="Button1" runat="server" Text="Cập nhật" CssClass="btn btn-info  pull-right" OnClick="btAdd_Click2"></asp:Button>

                </div>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script>
        function checkAll(obj1) {
            var returnVal = undefined;
            if (obj1.checked == true) {

                returnVal = confirm("Bạn có muốn chọn tất cả?");
                if (returnVal) {
                    $(this).attr("checked", true);
                    $(".CheckAll input").attr("checked", true);
                }
            }
            else {

                returnVal = confirm("Bạn có muốn Hủy chọn tất cả?");
                if (returnVal) {
                    $(this).attr("checked", false);
                    $(".CheckAll input").attr("checked", false);
                }
            }


        }
    </script>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Partners.Add.aspx.cs" Inherits="Pages_PayGate_Partners_Add" ValidateRequest="false" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Kết nối
        <small>
            <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersList %>">Danh sách đối tác</a>
        </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Thêm mới đối tác</h3>
            </div>
            <div class="box-body">
                <div class="row">

                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtName">Tên đối tác *</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên đối tác *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtPartnerCode">Mã đối tác *</label>
                            <asp:TextBox ID="txtPartnerCode" runat="server" CssClass="form-control" placeholder="Mã đối tác *"></asp:TextBox>
                        </div>



                        <div class="form-group" style="display: none">
                            <label for="txtSMSCommand">SMSCommand </label>
                            <asp:TextBox ID="txtSMSCommand" runat="server" CssClass="form-control" placeholder="SMSCommand"></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtSMSUrl">SMSUrl</label>
                            <asp:TextBox ID="txtSMSUrl" runat="server" CssClass="form-control" placeholder="SMSUrl"></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtSMSPlusCommand">SMSPlusCommand</label>
                            <asp:TextBox ID="txtSMSPlusCommand" runat="server" CssClass="form-control" placeholder="SMSPlusUrl"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtSMSPlusCheckUrl">MOMO Channel</label>
                            <asp:DropDownList ID="drlMomoGroup" runat="server" CssClass="form-control ">
                            </asp:DropDownList>

                        </div>
                        <div class="form-group">
                            <label for="txtSMSPlusUrl">Bank Channel</label>
                            <asp:DropDownList ID="drlBankGroup" runat="server" CssClass="form-control ">
                            </asp:DropDownList>

                        </div>
                        <div class="form-group">
                            <label for="txtHotline">Tài khoản đối ứng</label>
                            <asp:DropDownList ID="ddlUser" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                            <%--<asp:TextBox ID="txtHotline" runat="server" CssClass="form-control" placeholder="Tài khoản đối ứng"></asp:TextBox>--%>
                        </div>
                    </div>


                    <!-- /.col -->
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtClassName">Loại chữ ký *</label>
                            <asp:DropDownList ID="drpSignatureType" runat="server" CssClass="form-control">
                                <asp:ListItem Text="MD5" Value="1"></asp:ListItem>
                                <asp:ListItem Text="RSA" Value="2"></asp:ListItem>
                                <asp:ListItem Text="SHA256" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtPrivateKey">PrivateKey *</label>
                            <asp:TextBox ID="txtPrivateKey" Height="72" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="PrivateKey"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtPublicKey">PublicKey *</label>
                            <asp:TextBox ID="txtPublicKey" Height="72" TextMode="MultiLine" runat="server" CssClass="form-control" placeholder="PublicKey"></asp:TextBox>
                        </div>

                        <div class="checkbox">
                            <label for="cbxIsActive">
                                <asp:CheckBox ID="chkIsActive" Checked="true" runat="server"></asp:CheckBox>Trạng thái
                            </label>
                        </div>
                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->

                <div class="box-footer">
                    <asp:Button ID="btAdd" runat="server" Text="THÊM" CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
                    <asp:Button ID="btCancel" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript">
        
        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>

</asp:Content>

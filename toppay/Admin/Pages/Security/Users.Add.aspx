<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Users.Add.aspx.cs" Inherits="Pages_Security_Users_Add" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Tài Khoản
        <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.UsersList %>">Thêm thông tin tài khoản</a>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Thêm người dùng</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>
                    Sửa tài khoản  
                </li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Thông Tin</a></li>
                <%--<li><a href="#sales-chart" data-toggle="tab">Đối tác</a></li>
                <li><a href="#sales-roles" data-toggle="tab">Chức năng quản trị</a></li>--%>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtUserName">Tên truy cập *</label>
                            <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="Tên truy cập *"></asp:TextBox>
                            <small>Tên truy cập để đăng nhập hệ thống.</small>
                        </div>
                        <div class="form-group">
                            <label for="txtFullName">Họ tên *</label>
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Họ tên *"></asp:TextBox>
                            <small>Ví dụ: Nguyễn Văn Bình, Lê Hoàng,...</small>
                        </div>
                        <div class="form-group" style="display:none">
                            <label for="txtFullName">Nhóm *</label>
                            <asp:DropDownList ID="drpOrder" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtPassword">Mật khẩu *</label>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập lại mật khẩu *"></asp:TextBox>
                            <small>Mật khẩu có ít nhất 6 ký tự</small>
                        </div>
                        <div class="form-group">
                            <label for="txtPasswordAgain">Nhập lại mật khẩu *</label>
                            <asp:TextBox ID="txtPasswordAgain" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập lại mật khẩu *"></asp:TextBox>
                            <small>Mật khẩu và mật khẩu nhập lại phải giống nhau</small>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox ID="chkIsActive" Checked="true" runat="server"></asp:CheckBox>
                            <small>Chọn để cho phép người dùng đăng nhập</small>
                        </div>
                        <%if (AppUtils.IsAdmin)
                            {%>
                        <div class="checkbox">
                            <label for="cbxIsAdmin">
                                <asp:RadioButton ID="cbxIsAdmin" Checked="false" runat="server" GroupName="TypeUser"></asp:RadioButton>
                                Tài khoản admin   
                            </label>
                        </div>

                        <div class="checkbox">
                            <label for="cbxIsAdmin">
                                <asp:RadioButton ID="cbxIsPartner" Checked="true" runat="server" GroupName="TypeUser"></asp:RadioButton>
                                Tài khoản merchant
                            </label>
                        </div>
                        <div class="checkbox"  >
                            <label for="cbxIsAdmin">
                                <asp:RadioButton ID="cbxIsTopup" Checked="false" runat="server" GroupName="TypeUser"></asp:RadioButton>
                                Tài khoản support
                            </label>
                        </div>
                        <div class="checkbox" style="display: none">
                            <label for="cbxIsAdmin">
                                <asp:RadioButton ID="cbxIsProvider" Checked="false" runat="server" GroupName="TypeUser"></asp:RadioButton>
                                User Provider   
                            </label>
                        </div>
                        <%}%>
                        <style type="text/css">
                            #msg table {
                                z-index: 100000;
                            }
                        </style>
                        <div id="msg">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Tên truy cập phải có tối thiểu 3 ký tự"
                                Display="None" ControlToValidate="txtUserName" ValidationExpression=".{3}.*"></asp:RegularExpressionValidator>
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender1"
                                Width="160px" HighlightCssClass="validatorCalloutHighlight" TargetControlID="RegularExpressionValidator1" WarningIconImageUrl="" />

                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="Mật khẩu tối thiểu phải có 6 ký tự"
                                Display="None" ControlToValidate="txtPassword" ValidationExpression=".{6}.*"></asp:RegularExpressionValidator>
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender2"
                                Width="160px" HighlightCssClass="validatorCalloutHighlight" TargetControlID="RegularExpressionValidator2" WarningIconImageUrl="" />
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPassword"
                                ControlToValidate="txtPasswordAgain" Display="None" ErrorMessage="Mật khẩu nhật lại không khớp"></asp:CompareValidator>
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender3"
                                HighlightCssClass="validatorCalloutHighlight" TargetControlID="CompareValidator1" WarningIconImageUrl="" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtFullName"
                                Display="None" ErrorMessage="Bạn chưa nhập họ tên!" />
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender5" WarningIconImageUrl=""
                                HighlightCssClass="validatorCalloutHighlight" TargetControlID="RequiredFieldValidator2" />
                        </div>
                    </div>
                    <!-- /.col -->
                    <div class="col-md-6">
                    </div>
                    <!-- /.col -->

                </div>

            </div>
            <div class="box-footer">
                <asp:Button ID="btAdd" runat="server" Text="Thêm Mới " CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
                <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" CausesValidation="false" Width="50px" OnClick="btCancel_Click"></asp:Button>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
        $(function () {
            $(".txtParnerId").attr("type", "number");
            $('.CheckAllBlock').change(function () {
                var returnVal = undefined;
                if ($(this).is(":checked")) {
                    returnVal = confirm("Bạn có muốn chọn tất cả?");
                    if (returnVal) {
                        $(this).attr("checked", true);
                        $(".CheckAll" + $(this).data("value") + " .CheckAll input").attr("checked", true);
                    }
                } else {
                    returnVal = confirm("Bạn có muốn Hủy chọn tất cả?");
                    if (returnVal) {
                        $(this).attr("checked", false);
                        $(".CheckAll" + $(this).data("value") + " .CheckAll input").attr("checked", false);
                    }
                }

            });
        });
    </script>
</asp:Content>




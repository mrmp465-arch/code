<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Users.Edit.aspx.cs" Inherits="Pages_Topup_Users_Edit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Tài Khoản
        <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.TopupUsersList %>">Sửa thông tin tài khoản</a>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Sửa người dùng</li>
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
                <%--<li><a href="#sales-chart" data-toggle="tab">Đối tác</a></li>--%>
                <li><a href="#sales-roles" data-toggle="tab">Chức năng quản trị</a></li>
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
                                <asp:CheckBox ID="chkIsActive" runat="server"></asp:CheckBox>
                                <small>Chọn để cho phép người dùng đăng nhập</small>
                            </div> 
                            
                            <div id="msg"> 
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="Mật khẩu tối thiểu phải có 6 ký tự"
                                    Display="None" ControlToValidate="txtPassword" ValidationExpression=".{6}.*"></asp:RegularExpressionValidator>

                                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender2"
                                    Width="160px" HighlightCssClass="validatorCalloutHighlight" TargetControlID="RegularExpressionValidator2" WarningIconImageUrl="" />


                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPassword"
                                    ControlToValidate="txtPasswordAgain" Display="None" ErrorMessage="Mật khẩu nhật lại không khớp"></asp:CompareValidator>
                                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender3"
                                    HighlightCssClass="validatorCalloutHighlight" TargetControlID="CompareValidator1" WarningIconImageUrl="" /> 

                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtFullName"
                                    Display="None" ErrorMessage="Bạn chưa nhập họ tên!" />
                      
                            </div>
                        </div>
                        <!-- /.col -->
                        <div class="col-md-6">
                        </div> 
                </div>
               <%-- <div class="chart tab-pane" id="sales-chart">
                    <div class="box-body no-padding" style="padding-top: 15px !important">
                        <asp:Repeater ID="rptListPartner" runat="server">
                            <ItemTemplate>
                                <div class="col-md-3 col-sm-6 col-xs-12" style="padding-bottom: 10px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxIsCheckPartner" runat="server" Checked='<%#Eval("IsCheck") %>'></asp:CheckBox>
                                            <asp:HiddenField ID="txtPartnerId" Value='<%#Eval("PartnerId") %>' runat="server"></asp:HiddenField>
                                            <asp:HiddenField ID="txtPartnerCode" Value='<%#Eval("PartnerCode") %>' runat="server"></asp:HiddenField>
                                        </span>
                                        <asp:Label ID="lbNamePartner" CssClass="form-control" runat="server"><%#Eval("Name") %></asp:Label>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>--%>
                  <div class="chart tab-pane" id="sales-roles">
                    <div class="box-body no-padding" style="padding-top: 15px !important"> 
                       <div class="col-md-3 col-sm-12 col-xs-12" style="padding-bottom: 10px; display:table-cell">
                            <asp:Repeater ID="rptListRoles" runat="server">
                                <ItemTemplate>
                                    <%# GetHtmlGroup(Eval("Group"),Eval("GroupName"),Eval("IsCheck") )%>  
                                        <div class="input-group CheckAll<%#Eval("Group") %>" style="padding-bottom:5px;" >
                                            <span class="input-group-addon">
                                                <asp:CheckBox ID="cbxIsCheckRole" runat="server" CssClass="CheckAll" Checked='<%#Eval("IsCheck") %>' ></asp:CheckBox>
                                                <asp:HiddenField ID="txtRoleId" Value='<%#Eval("RoleId") %>' runat="server"></asp:HiddenField>
                                                <asp:HiddenField ID="txtUrl" Value='<%#Eval("Url") %>' runat="server"></asp:HiddenField>
                                            </span>
                                            <asp:Label ID="lbName" CssClass="form-control" runat="server"><%#Eval("Name") %></asp:Label> 
                                        </div> 
                                </ItemTemplate>
                            </asp:Repeater>
                         </div> 
                    </div>
                </div>
            </div>
            <div class="box-footer">
                <asp:Button ID="btUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-primary" OnClick="btUpdate_Click"></asp:Button>
                <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" CausesValidation="false" OnClick="btCancel_Click"></asp:Button>
            </div>
        </div>
    </section>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
        $(function () { 
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

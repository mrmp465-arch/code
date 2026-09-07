<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Users.Edit.aspx.cs" Inherits="Pages_Security_Users_Edit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
    <div class="modal  fade bs-example-modal-sm" id="MAlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1>Tài Khoản
        <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.UsersList %>">Sửa thông tin tài khoản</a>
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
                <% if (IsUserPartner == 1)
                    {%>
                <li><a href="#sales-connect" data-toggle="tab">Kết nối</a></li>
                <%}%>
                <% if (IsUserToup == 1)
                    {%>
                <li><a href="#sales-chart-partner" data-toggle="tab">Đối tác</a></li>

                <%}%>
                <%if ((IsUserAdmin == 1 || AppUtils.UserName == "admin") && RoleUpadte)
                    {%>
                <li><a href="#sales-roles" data-toggle="tab">Chức năng quản trị</a></li>
                <%}%>
            </ul>
            <div class="tab-content no-padding">
                <% if (IsUserPartner == 1)
                    {%>
                <div class="chart tab-pane" id="sales-connect">
                    <div class="col-md-7">
                        <div class="form-group">
                            <label for="txtPartnerCode">PartnerCode</label>
                            <asp:TextBox ID="txtPartnerCode" runat="server" CssClass="form-control textcopy" ReadOnly="true"></asp:TextBox>
                            <div class="input-group-append">
                                <button type="button" class="btn" onclick="copyText(<%=txtPartnerCode.ClientID%>)">
                                    Copy
                                </button>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="txtPartnerCode">PartnerKey</label>
                            <asp:TextBox ID="txtPartnerKey" runat="server" CssClass="form-control textcopy" ReadOnly="true"></asp:TextBox>
                            <div class="input-group-append">
                                <button type="button" class="btn" onclick="copyText(<%=txtPartnerKey.ClientID%>)">
                                    Copy
                                </button>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="txtSMSPlusUrl">GroupId</label>


                            <asp:TextBox ID="txtSMSPlusUrl" runat="server" CssClass="form-control" placeholder="GroupId"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtSMSPlusUrl">Số tiền xác nhận đơn rút</label>


                            <asp:TextBox ID="txtSMSCommand" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtSMSPlusUrl">Số tiền gửi bill tự động</label>


                            <asp:TextBox ID="txtRequestType" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtSMSPlusUrl">Tài khoản đại lý</label>


                            <asp:TextBox ID="txtSMSPlusCommand" runat="server" CssClass="form-control" placeholder="Nhập tên tk đại lý"></asp:TextBox>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="ddlBankCashEnable" runat="server"></asp:CheckBox>

                                        </span>
                                        <span class="form-control" style="font-weight: bold">Rút bank</span>

                                    </div>
                                </div>
                            </div>

                            <div class="col-md-3">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="ddlBankInEnable" runat="server"></asp:CheckBox>

                                        </span>
                                        <span class="form-control" style="font-weight: bold">Nạp bank</span>

                                    </div>

                                </div>
                            </div>


                        </div>
                    </div>

                </div>

                <%}%>
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtUserName">Tên truy cập *</label>
                            <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" ReadOnly="true">Tên truy cập *"></asp:TextBox>
                            <small>Tên truy cập để đăng nhập hệ thống.</small>
                        </div>
                        <div class="form-group">
                            <label for="txtFullName">Họ tên *</label>
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Họ tên *"></asp:TextBox>
                            <small>Ví dụ: Nguyễn Văn Bình, Lê Hoàng,...</small>
                        </div>
                        <div class="form-group" style="display: none">
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
                        <div class="form-group" style="display: none">
                            <label for="txtDeposit">Số dư tài khoản đối ứng *</label>
                            <asp:TextBox ID="txtDeposit" runat="server" CssClass="form-control"></asp:TextBox>

                        </div>
                        <div class="form-group">
                            <label for="txtIp">Login Whitelist Ip</label>
                            <asp:TextBox ID="txtIp" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            <small>Danh sách IP Cách nhau bằng dấu phẩy</small>

                        </div>
                        <div class="form-group">
                            <label for="txtIp">API Whitelist Ip</label>
                            <asp:TextBox ID="txtAPIIP" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            <small>Danh sách IP Cách nhau bằng dấu phẩy</small>

                        </div>
                        <div class="form-group">
                            <label for="txtIp">User xác nhận</label>
                            <asp:TextBox ID="txtUserConfirm" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            <small>Danh sách user Cách nhau bằng dấu phẩy</small>

                        </div>
                        <div class="form-group">
                            <asp:CheckBox ID="chkWithdraw" runat="server"></asp:CheckBox>
                            <small>Rút tiền (tk phụ)</small>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox ID="chkIsActive" runat="server"></asp:CheckBox>
                            <small>Kích hoạt</small>
                        </div>
                        <%if (AppUtils.IsAdmin)
                            {%>
                        <div class="checkbox">
                            <label for="cbxIsAdmin">
                                <asp:RadioButton ID="cbxIsAdmin" Enabled="false" Checked="false" runat="server" GroupName="TypeUser"></asp:RadioButton>
                                Tài khoản Admin   
                            </label>
                        </div>

                        <div class="checkbox">
                            <label for="cbxIsAdmin">
                                <asp:RadioButton ID="cbxIsPartner" Enabled="false" Checked="false" runat="server" GroupName="TypeUser"></asp:RadioButton>
                                Tài khoản merchant
                            </label>
                        </div>
                        <div class="checkbox">
                            <label for="cbxIsAdmin">
                                <asp:RadioButton ID="cbxIsTopup" Enabled="false" Checked="false" runat="server" GroupName="TypeUser"></asp:RadioButton>
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
                        <%if (IsUserAdmin != 1)
                            {%>
                        <div class="row">
                        </div>

                        <div class="form-group">
                            <label for="txtDiscountBANKTRANFER">Chiết khấu bank </label>
                            <asp:HiddenField ID="hdDiscountBANKTRANFER" runat="server" />
                            <asp:TextBox ID="txtDiscountBANKTRANFER" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtDiscountBANKOUTTRANFER">Chiếu khấu bank out </label>
                            <asp:HiddenField ID="hdDiscountBANKOUTTRANFER" runat="server" />
                            <asp:TextBox ID="txtDiscountBANKOUTTRANFER" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtDiscountMOMO">Reward bank </label>
                            <asp:HiddenField ID="hdRewardBANKTRANFER" runat="server" />
                            <asp:TextBox ID="txtRewardBANKTRANFER" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtDiscountMOMOOUT">Reward bank out </label>
                            <asp:HiddenField ID="hdRewardBANKOUTTRANFER" runat="server" />
                            <asp:TextBox ID="txtRewardBANKOUTTRANFER" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>

                        <div class="form-group" style="display: none">
                            <label for="txtDiscountMOMO">Chiết khấu momo </label>
                            <asp:HiddenField ID="hdDiscountMOMO" runat="server" />
                            <asp:TextBox ID="txtDiscountMOMO" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtDiscountMOMOOUT">Chiếu khấu momo out </label>
                            <asp:HiddenField ID="hdDiscountMOMOOUT" runat="server" />
                            <asp:TextBox ID="txtDiscountMOMOOUT" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtDiscountVTT">Reward Momo </label>
                            <asp:TextBox ID="txtRewardMOMO" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtDiscountVTT">Reward Momo out </label>
                            <asp:TextBox ID="txtRewardMOMOOUT" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtDiscountVTT">Chiếu khấu thẻ </label>
                            <asp:HiddenField ID="hdDiscountVTT" runat="server" />
                            <asp:TextBox ID="txtDiscountVTT" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox ID="rckUpdate" runat="server"></asp:CheckBox>
                            <small>Cập nhật chiết khấu</small>
                        </div>


                        <%}%>
                    </div>
                </div>
                <div class="chart tab-pane" id="sales-chart-partner">
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
                </div>
                <div class="chart tab-pane" id="sales-chart-provider">
                    <div class="box-body no-padding" style="padding-top: 15px !important">
                        <asp:Repeater ID="rptListProvider" runat="server">
                            <ItemTemplate>
                                <div class="col-md-3 col-sm-6 col-xs-12" style="padding-bottom: 10px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxIsCheckProvider" runat="server" Checked='<%#Eval("IsCheck") %>'></asp:CheckBox>
                                            <asp:HiddenField ID="txtProviderId" Value='<%#Eval("ProviderId") %>' runat="server" />
                                            <asp:HiddenField ID="txtProviderCode" Value='<%#Eval("ProviderCode") %>' runat="server"></asp:HiddenField>
                                        </span>
                                        <asp:Label ID="lbNameProvider" CssClass="form-control" runat="server"><%#Eval("Name") %></asp:Label>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="chart tab-pane" id="sales-chart-provider3">
                    <div class="box-body no-padding" style="padding-top: 15px !important">
                        <asp:Repeater ID="rptListProvider3" runat="server">
                            <ItemTemplate>
                                <div class="col-md-3 col-sm-6 col-xs-12" style="padding-bottom: 10px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxIsCheckProvider" runat="server" Checked='<%#Eval("IsCheck") %>'></asp:CheckBox>
                                            <asp:HiddenField ID="txtProviderId" Value='<%#Eval("ProviderId") %>' runat="server" />
                                            <asp:HiddenField ID="txtProviderCode" Value='<%#Eval("ProviderCode") %>' runat="server"></asp:HiddenField>
                                        </span>
                                        <asp:Label ID="lbNameProvider2" CssClass="form-control" runat="server"><%#Eval("Name") %></asp:Label>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="chart tab-pane" id="sales-chart-provider2">
                    <div class="box-body no-padding" style="padding-top: 15px !important">
                        <asp:Repeater ID="rptListProvider2" runat="server">
                            <ItemTemplate>
                                <div class="col-md-3 col-sm-6 col-xs-12" style="padding-bottom: 10px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxIsCheckProvider" runat="server" Checked='<%#Eval("IsCheck") %>'></asp:CheckBox>
                                            <asp:HiddenField ID="txtProviderId" Value='<%#Eval("ProviderId") %>' runat="server" />
                                            <asp:HiddenField ID="txtProviderCode" Value='<%#Eval("ProviderCode") %>' runat="server"></asp:HiddenField>
                                        </span>
                                        <asp:Label ID="lbNameProvider2" CssClass="form-control" runat="server"><%#Eval("Name") %></asp:Label>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="chart tab-pane" id="sales-roles">
                    <div class="box-body no-padding" style="padding-top: 15px !important">
                        <div class="col-md-3 col-sm-12 col-xs-12" style="padding-bottom: 10px; display: table-cell">
                            <asp:Repeater ID="rptListRoles" runat="server">
                                <ItemTemplate>
                                    <%# GetHtmlGroup(Eval("Group"),Eval("GroupName"),Eval("IsCheck") )%>
                                    <div class="input-group CheckAll<%#Eval("Group") %>" style="padding-bottom: 5px;">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxIsCheckRole" runat="server" CssClass="CheckAll" Checked='<%#Eval("IsCheck") %>'></asp:CheckBox>
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
                <asp:Button ID="btUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-info " OnClick="btUpdate_Click"></asp:Button>
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
    <script>
        function copyText(id) {
            var str = $(id).val();
            var el = document.createElement('textarea');
            el.value = str;
            el.setAttribute('readonly', '');
            el.style = {
                position: 'absolute',
                left: '-9999px'
            };
            document.body.appendChild(el);
            el.select();
            document.execCommand('copy');
            document.body.removeChild(el);

        }
    </script>
    <style>
        .textcopy {
            width: 86%;
        }

        #dv60, #dv61, #dv62, #dv63 {
            display: none;
        }

        #dv8, #dv9, #dv25, #dv26, #dv91, #dv96 {
            display: none;
        }

        .input-group-append {
            float: right;
            margin-top: -34px;
        }
    </style>
</asp:Content>

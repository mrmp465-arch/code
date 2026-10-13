<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.Edit.aspx.cs" Inherits="Pages_Security_Roles_Edit" ValidateRequest="false" %>

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
        <h1>Quản trị
        <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.TopupAdd %>">Danh sách đối tác</a>  </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Cập nhật dòng đơn hàng</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Thông Tin</a></li>
            </ul>
            <div class="tab-content no-padding">
                <!-- Morris chart - Sales -->
                <div class="chart tab-pane active" id="revenue-chart">
                    <!-- SELECT2 EXAMPLE -->
                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtFullName">Order No*</label>
                                    <asp:TextBox ID="txtOrderNo" runat="server" CssClass="form-control" placeholder="Mã đơn hàng"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtName">Telco *</label>
                                    <asp:DropDownList ID="txtTelco" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">Chọn mạng:</asp:ListItem>
                                        <asp:ListItem Value="vtt">VTT</asp:ListItem>
                                        <asp:ListItem Value="vms">VMS</asp:ListItem>
                                        <asp:ListItem Value="vnp">VNP</asp:ListItem>
                                        <asp:ListItem Value="gosu">GOSU</asp:ListItem>
                                        <asp:ListItem Value="zing">ZING</asp:ListItem>
                                        <asp:ListItem Value="garena">GARENA</asp:ListItem>
                                        <asp:ListItem Value="vtc">VTC</asp:ListItem>
                                        <asp:ListItem Value="dzo">DZO</asp:ListItem>
                                        <asp:ListItem Value="gate">GATE</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtName">Loại hình thuê bao *</label>
                                    <asp:DropDownList ID="txtTopupType" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">Thuê bao:</asp:ListItem>
                                        <asp:ListItem Value="1">Nạp số dư trả trước</asp:ListItem>
                                        <asp:ListItem Value="2">Nạp số dư trả sau</asp:ListItem>
                                        <asp:ListItem Value="3">Nạp cước Internet</asp:ListItem>
                                        <asp:ListItem Value="4">Nạp Smas</asp:ListItem>
                                        <asp:ListItem Value="5">Nạp cước điện thoại cố định</asp:ListItem>
                                        <asp:ListItem Value="6">Nạp Nhà thuốc (PPG)</asp:ListItem>
                                        <asp:ListItem Value="7">Nạp Tiêm Chủng (VNCDC)</asp:ListItem>
                                        <asp:ListItem Value="8">Nạp ShopOne</asp:ListItem>
                                        <asp:ListItem Value="10">Nạp Metro Wan/Leased line</asp:ListItem>
                                        <asp:ListItem Value="9">Nạp Game I</asp:ListItem>
                                        <asp:ListItem Value="11">Nạp Game II</asp:ListItem>
                                        <asp:ListItem Value="12">Nạp Game III</asp:ListItem>
                                        <asp:ListItem Value="13">Nạp Game IV</asp:ListItem>
                                        <asp:ListItem Value="14">Nạp Game V</asp:ListItem>
                                        <asp:ListItem Value="15">Nạp Game VI</asp:ListItem>
                                        <asp:ListItem Value="16">Nạp Game M VII</asp:ListItem>
                                        <asp:ListItem Value="17">Nạp Game M VIII</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtFullName">Tên khách hàng</label>
                                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Tên khách hàng"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtMobile">Mobile/Acount*</label>
                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Số điện thoại hoặc tài khoản internet, truyền hình ... tùy thuộc vào chọn hình thuê bao"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtAmount">Amount*</label>
                                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Số tiền cần thanh toán"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtAccountName">AccountName</label>
                                    <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control" placeholder="Tên đăng nhập nhà mạng"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="txtPassword">Password/Token</label>
                                    <asp:Panel runat="server" ID="phVMS" Visible="False">
                                        <div class="row">
                                            <div class="col-xs-2">
                                                <asp:TextBox ID="txtOTP" class="form-control" runat="server" placeholder="OTP"></asp:TextBox>
                                            </div>
                                            <div class="col-xs-2">
                                                <button type="button" class="btn btn-primary" id="btnGetOTP" onclick="OTPAjax();">Get OTP</button>
                                                <button type="button" class="btn btn-primary" id="btnGetToken" style="display: none;" onclick="TOKENAjax();">Get TOKEN</button>
                                                <i id="loading" class="fa fa-refresh fa-spin" style="display: none;"></i>
                                            </div>
                                            <div class="col-xs-3">
                                                <asp:TextBox ID="txtDeviceId" class="form-control" runat="server" placeholder="DeviceId" Style="display: none;"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group has-error"><span class="help-block" id="tokenhelp"></span></div>
                                    </asp:Panel>

                                </div>
                                <div class="form-group">
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Mật khẩu nhà mạng"></asp:TextBox>
                                </div>

                                <asp:Panel runat="server" ID="phGameM" Visible="False">
                                    <div class="form-group">
                                        <div class="row">
                                            <div class="col-xs-3">
                                                <label for="txtServer">Server*</label>
                                                <asp:DropDownList ID="txtServer" class="form-control" runat="server" OnSelectedIndexChanged="txtServer_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                            </div>
                                            <div class="col-xs-4">
                                                <label for="txtServer">Role*</label>
                                                <asp:DropDownList ID="txtRole" class="form-control" runat="server"></asp:DropDownList>
                                            </div>
                                            <div class="col-xs-4 has-error">
                                                <label for="txtServer"></label>
                                                <asp:Label class="help-block" ID="roleHelp" runat="server"></asp:Label>
                                            </div>
                                        </div>

                                    </div>
                                </asp:Panel>

                               <%-- <div class="form-group">
                                    <label for="txtAmount">AmountMin* tối thiểu cho lần nạp đầu tiên (Chọn 0 là không bắt buộc)</label>
                                    <asp:DropDownList ID="txtFirtAmout" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">AmountMin:</asp:ListItem>
                                        <asp:ListItem Value="0">0</asp:ListItem>
                                        <asp:ListItem Value="10000">10.000</asp:ListItem>
                                        <asp:ListItem Value="20000">20.000</asp:ListItem>
                                        <asp:ListItem Value="50000">50.000</asp:ListItem>
                                        <asp:ListItem Value="100000">100.000</asp:ListItem>
                                        <asp:ListItem Value="200000">200.000</asp:ListItem>
                                        <asp:ListItem Value="300000">300.000</asp:ListItem>
                                        <asp:ListItem Value="500000">500.000</asp:ListItem>
                                        <asp:ListItem Value="1000000">1.000.000</asp:ListItem>
                                    </asp:DropDownList>
                                </div>--%>
                                <div class="form-group">
                                    <label for="txtAmount">AmountMinAll* Mệnh giá thẻ tối thiểu (Chọn 0 là không bắt buộc)</label>
                                    <asp:DropDownList ID="txtAmountAll" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">AmountMinAll:</asp:ListItem>
                                        <asp:ListItem Value="0">0</asp:ListItem>
                                        <asp:ListItem Value="10000">10.000</asp:ListItem>
                                        <asp:ListItem Value="20000">20.000</asp:ListItem>
                                        <asp:ListItem Value="50000">50.000</asp:ListItem>
                                        <asp:ListItem Value="100000">100.000</asp:ListItem>
                                        <asp:ListItem Value="200000">200.000</asp:ListItem>
                                        <asp:ListItem Value="300000">300.000</asp:ListItem>
                                        <asp:ListItem Value="500000">500.000</asp:ListItem>
                                        <asp:ListItem Value="1000000">1.000.000</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                
                                <div class="form-group">
                                    <label for="txtMobile">Mức độ ưu tiên * (P càng nhỏ mức độ ưu tiên càng cao)</label>
                                    <asp:DropDownList ID="txtPriority" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">Priority:</asp:ListItem>
                                       
                                        <asp:ListItem Value="49">P49</asp:ListItem>
                                        <asp:ListItem Value="48">P48</asp:ListItem>
                                        <asp:ListItem Value="47">P47</asp:ListItem>
                                        <asp:ListItem Value="46">P46</asp:ListItem>
                                        <asp:ListItem Value="45">P45</asp:ListItem>
                                        <asp:ListItem Value="44">P44</asp:ListItem>
                                        <asp:ListItem Value="43">P43</asp:ListItem>
                                        <asp:ListItem Value="42">P42</asp:ListItem>
                                        <asp:ListItem Value="41">P41</asp:ListItem>
                                        <asp:ListItem Value="40">P40</asp:ListItem>
                                         <asp:ListItem Value="39">P39</asp:ListItem>
                                        <asp:ListItem Value="38">P38</asp:ListItem>
                                        <asp:ListItem Value="37">P37</asp:ListItem>
                                        <asp:ListItem Value="36">P36</asp:ListItem>
                                        <asp:ListItem Value="35">P35</asp:ListItem>
                                        <asp:ListItem Value="34">P34</asp:ListItem>
                                        <asp:ListItem Value="33">P33</asp:ListItem>
                                        <asp:ListItem Value="32">P32</asp:ListItem>
                                        <asp:ListItem Value="31">P31</asp:ListItem>
                                        <asp:ListItem Value="30">P30</asp:ListItem>
                                        <asp:ListItem Value="29">P29</asp:ListItem>
                                        <asp:ListItem Value="28">P28</asp:ListItem>
                                        <asp:ListItem Value="27">P27</asp:ListItem>
                                        <asp:ListItem Value="26">P26</asp:ListItem>
                                        <asp:ListItem Value="25">P25</asp:ListItem>
                                        <asp:ListItem Value="24">P24</asp:ListItem>
                                        <asp:ListItem Value="23">P23</asp:ListItem>
                                        <asp:ListItem Value="22">P22</asp:ListItem>
                                        <asp:ListItem Value="21">P21</asp:ListItem>
                                        <asp:ListItem Value="20">P20</asp:ListItem>
                                        <asp:ListItem Value="19">P19</asp:ListItem>
                                        <asp:ListItem Value="18">P18</asp:ListItem>
                                        <asp:ListItem Value="17">P17</asp:ListItem>
                                        <asp:ListItem Value="16">P16</asp:ListItem>
                                        <asp:ListItem Value="15">P15</asp:ListItem>
                                        <asp:ListItem Value="14">P14</asp:ListItem>
                                        <asp:ListItem Value="13">P13</asp:ListItem>
                                        <asp:ListItem Value="12">P12</asp:ListItem>
                                        <asp:ListItem Value="11">P11</asp:ListItem>
                                        <asp:ListItem Value="10">P10</asp:ListItem>
                                        <asp:ListItem Value="9">P9</asp:ListItem>
                                        <asp:ListItem Value="8">P8</asp:ListItem>
                                        <asp:ListItem Value="7">P7</asp:ListItem>
                                        <asp:ListItem Value="6">P6</asp:ListItem>
                                        <asp:ListItem Value="5">P5</asp:ListItem>
                                        <asp:ListItem Value="4">P4</asp:ListItem>
                                        <asp:ListItem Value="3">P3</asp:ListItem>
                                        <asp:ListItem Value="2">P2</asp:ListItem>
                                        <asp:ListItem Value="1">P1</asp:ListItem>

                                        
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtStatus">Trạng thái (Hạn chế thay đổi trạng thái hệ thống)* </label>
                                    <asp:DropDownList ID="txtStatus" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="-3">Đợi nạp</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="1">Đợi xử lý</asp:ListItem>
                                        <asp:ListItem Value="2">Đang xử lý (Hệ thống)</asp:ListItem>
                                        <asp:ListItem Value="3">Đã hoàn thành (Hệ thống)</asp:ListItem>
                                        <asp:ListItem Value="0">Không sử dụng</asp:ListItem>
                                        <asp:ListItem Value="-1">Bỏ qua (Hệ thống)</asp:ListItem>
                                        <asp:ListItem Value="-2">Telco khóa (Hệ thống)</asp:ListItem>
                                        <asp:ListItem Value="-4">Telco hết lượt (Hệ thống)</asp:ListItem>
                                        <asp:ListItem Value="-6">Dừng để Review</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtStatus">Tình trạng</label>
                                    <asp:DropDownList ID="txtConfirm" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="0">Chưa chốt</asp:ListItem>
                                        <asp:ListItem Value="1">Đã chốt</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtUssd">Hỗ trợ nạp USSD</label>
                                    <asp:DropDownList ID="txtUssd" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="0">Không</asp:ListItem>
                                        <asp:ListItem Value="1">Có</asp:ListItem>
                                        <asp:ListItem Value="2">Chỉ chạy USSD</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <!-- /.row -->
                    </div>

                    <div class="box-footer">
                        <asp:Button ID="btUpdate" runat="server" Text="CẬP NHẬT" CssClass="btn btn-primary" OnClick="btUpdate_Click"></asp:Button>
                        <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <script type="text/javascript">

        function OTPAjax() {
            var otp = {
                Command: "GETOTP",
                Mobile: $("#<%=txtMobile.ClientID%>").val(),
                DeviceId: $("#<%=txtDeviceId.ClientID%>").val()
            }
            $("#loading").show();
            $.ajax({
                type: "post",
                datatype: "json",
                <%--url: "<%=Constant.ADMIN_PATH %>ServiceHandler/VMSToken.ashx",--%>
                url: "<%=Constant.ADMIN_PATH %>ServiceHandler/MyVMSToken.ashx",
                success: function (data) {
                    if (data.Code == "1") {
                        $("#tokenhelp").text('Điền OTP để lẫy Token');
                        $("#btnGetOTP").hide();
                        $("#btnGetToken").show();
                        $("#<%=txtPassword.ClientID%>").val('');
                    } else {
                        $("#tokenhelp").text(data.Data);
                    }
                    $("#loading").hide();
                },
                data: JSON.stringify(otp)
            });
        }

        function TOKENAjax() {
            var otp = {
                Command: "GETTOKEN",
                Mobile: $("#<%=txtMobile.ClientID%>").val(),
                Otp: $("#<%=txtOTP.ClientID%>").val(),
                DeviceId: $("#<%=txtDeviceId.ClientID%>").val()
            }
            $("#loading").show();
            $.ajax({
                type: "post",
                datatype: "json",
                <%--url: "<%=Constant.ADMIN_PATH %>ServiceHandler/VMSToken.ashx",--%>
                url: "<%=Constant.ADMIN_PATH %>ServiceHandler/MyVMSToken.ashx",
                success: function (data) {
                    if (data.Code == "1") {
                        $("#<%=txtPassword.ClientID%>").val(data.Data);
                    }
                    else {
                        $("#tokenhelp").text(data.Data);
                        if (data.Data == "OTP không đúng") {
                            $("#btnGetOTP").show();
                            $("#btnGetToken").hide();
                        }
                    }
                    $("#loading").hide();
                },
                data: JSON.stringify(otp)
            });
        }
    </script>
</asp:Content>


<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Account.Edit.aspx.cs" Inherits="Pages_Momo_Account_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
    <style type="text/css">
        .radioButtonList input[type="radio"] {
            margin-left: 15px;
            margin-right: 1px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Tài khoản momo" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Cập nhật tài khoản momo</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>MOMO</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Tài khoản momo</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoPartner %>">Quản lý kênh</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerMomo %>">Phân bố kênh & tài khoản</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">
                    <div class="nav-tabs-custom">
                        <!-- Tabs within a box -->
                        <ul class="nav nav-tabs pull-right ui-sortable-handle" id="accountTabs">
                            <li class=""><a href="#balance-info" data-toggle="tab">Số dư tài khoản</a></li>
                            <li class="active"><a href="#account-info" data-toggle="tab">Thông tin tài khoản</a></li>
                            <li class="pull-left header"></li>
                        </ul>
                        <div class="tab-content">
                            <!-- Morris chart - Sales -->
                            <div class="chart tab-pane" id="balance-info">

                                <div class="col-md-3">
                                    <div class="box box-solid">
                                        <div class="box-header with-border">
                                            <h3 class="box-title">
                                                <asp:Label runat="server" ID="lblAccount"></asp:Label></h3>

                                            <% if (Solution == "LD")
                                                {%>
                                            <div class="box-tools">
                                                <button type="button" class="btn" id="btnClientStart" runat="server" onserverclick="btStartClient_Click">
                                                    <i class="fa fa-play"></i>
                                                </button>
                                            </div>
                                            <%}
                                            %>
                                        </div>
                                        <div class="box-body no-padding" style="">
                                            <ul class="nav nav-stacked" id="balanceTabs">
                                                <li class="active"><a href="#detail-form-1" data-toggle="tab"><i class="fa fa-credit-card"></i>&nbsp;Rút tiền về nghân hàng liên kết (tự động)</a></li>
                                                <li><a href="#detail-form-2" data-toggle="tab"><i class="fa fa-credit-card"></i>&nbsp;Rút tiền về ngân hàng khác (tự động)</a></li>
                                                <li><a href="#detail-form-3" data-toggle="tab"><i class="fa fa-credit-card"></i>&nbsp;Khai báo rút tiền bằng tay (khai báo)</a></li>
                                                <li><a href="#detail-form-4" data-toggle="tab"><i class="fa fa-credit-card"></i>&nbsp;Chuyển khoản Momo (tự động)</a></li>
                                            </ul>
                                        </div>
                                        <!-- /.box-body -->
                                    </div>

                                </div>
                                <div class="col-md-9">
                                    <div class="callout callout-warning" id="calloutwaring" runat="server">
                                        <%--<h4>Chú ý quan trọng.</h4>--%>
                                        <p><i>Bạn cần bật thực hiện lệnh khởi động <b>Client</b> bằng cách bấm vào biểu tượng <b>PLAY</b> trước khi thực hiện các thao tác.</i></p>
                                    </div>
                                </div>
                                <div class="tab-content">
                                    <div class="tab-pane active" id="detail-form-1">
                                        <div class="col-md-9">
                                            <div class="callout callout-success" id="calloutform1" runat="server" visible="False">
                                                <p id="resTextForm1" runat="server"><i>Bạn đã rút tiền thành công.</i></p>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtAmount">Số tiền cần rút * (Tối thiểu là 50.000 đ)</label>
                                                <asp:TextBox ID="txtAmountForm1" runat="server" CssClass="form-control" placeholder="Số tiền cần rút" required="true"></asp:TextBox>
                                            </div>
                                            <div class="box-footer">
                                                <asp:Button ID="btnCashForm1" runat="server" Text="Rút tiền" CssClass="btn btn-info" OnClick="btnCashForm1_Click"></asp:Button>
                                                <asp:Button ID="btnCancelForm1" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane" id="detail-form-2">
                                        <div class="col-md-9">
                                            <div class="callout callout-success" id="calloutform2" runat="server" visible="False">
                                                <p id="resTextForm2" runat="server" style="white-space: pre-line"><i>Bạn đã rút tiền thành công.</i></p>
                                            </div>
                                            <div class="form-group">
                                                <label for="ddlBank">Chọn kiểu rút *</label>
                                                <asp:RadioButtonList ID="rbTransferType" runat="server" RepeatDirection="Horizontal" CssClass="radioButtonList" Font-Bold="False" Font-Italic="True">
                                                    <asp:ListItem Text=" QR (max 15M)" Value="QR" Selected="True" />
                                                    <asp:ListItem Text=" SEARCH (max 10M)" Value="SEARCH" />
                                                </asp:RadioButtonList>

                                            </div>
                                            <div class="form-group">
                                                <label for="ddlBank">Chọn ngân hàng *</label>
                                                <asp:DropDownList runat="server" ID="ddlBank" class="form-control select2" Style="width: 100%;" required="true">
                                                    <asp:ListItem Value="" Selected disabled hidden>Chọn ngân hàng</asp:ListItem>
                                                    <asp:ListItem Value="ACB">ACB - Ngân hàng TMCP Á Châu</asp:ListItem>
                                                    <asp:ListItem Value="BIDV">BIDV - Ngân hàng TMCP Đầu tư và Phát triển Việt Nam</asp:ListItem>
                                                    <asp:ListItem Value="MB">MBBank - Ngân hàng TMCP Quân đội</asp:ListItem>
                                                    <asp:ListItem Value="TCB">Techcombank - Ngân hàng TMCP Kỹ thương Việt Nam</asp:ListItem>
                                                    <asp:ListItem Value="TPB">TPBank - Ngân hàng TMCP Tiên Phong</asp:ListItem>
                                                    <asp:ListItem Value="VCB">Vietcombank - Ngân hàng TMCP Ngoại Thương Việt Nam</asp:ListItem>
                                                    <asp:ListItem Value="ICB">VietinBank - Ngân hàng TMCP Công thương Việt Nam</asp:ListItem>
                                                    <asp:ListItem Value="VPB">VPBank - Ngân hàng TMCP Việt Nam Thịnh Vượng</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtBankNumber">Số tài khoản / Số thẻ *</label>
                                                <asp:TextBox ID="txtBankNumber" runat="server" CssClass="form-control" placeholder="Nhập Số thẻ / Số tài khoản" required="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <label for="BankHolderName">Tên chủ Tài khoản / thẻ *</label>
                                                <asp:TextBox ID="txtBankHolderName" runat="server" CssClass="form-control" placeholder="Nhập tên chủ thẻ / tài khoản" required="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtAmountForm2">Số tiền cần rút *</label>
                                                <asp:TextBox ID="txtAmountForm2" runat="server" CssClass="form-control" placeholder="Nhập số tiền cần rút" required="true"></asp:TextBox>
                                            </div>
                                            <div class="box-footer">
                                                <asp:Button ID="btnCashForm2" runat="server" Text="Rút tiền" CssClass="btn btn-info" OnClick="btnCashForm2_Click"></asp:Button>
                                                <asp:Button ID="btnCancelForm2" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane" id="detail-form-3">
                                        <div class="col-md-9">
                                            <div class="form-group">
                                                <label for="txtAmount">Khai báo số tiền đã rút *</label><asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" placeholder="Số tiền đã rút"></asp:TextBox>
                                            </div>
                                            <div class="box-footer">
                                                <asp:Button ID="btnCashForm3" runat="server" Text="Khai báo" CssClass="btn btn-info" OnClick="btnCashForm3_Click"></asp:Button>
                                                <asp:Button ID="btnCancelForm3" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="tab-pane" id="detail-form-4">

                                        <div class="col-md-9">
                                            <div class="callout callout-success" id="calloutForm4" runat="server" visible="False">
                                                <p id="resTextForm4" runat="server"><i>Bạn đã rút tiền thành công.</i></p>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtMomoId">Tài khoản Momo người nhận * (Thực hiện tự động)</label>
                                                <asp:TextBox ID="txtMomoIdForm4" runat="server" CssClass="form-control" placeholder="Momo Id người nhận" required="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtMomoName">Tên người nhận</label>
                                                <asp:TextBox ID="txtMomoNameForm4" runat="server" CssClass="form-control" placeholder="Tên ngưởi nhận" required="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtAmount">Số tiền cần chuyển *</label>
                                                <asp:TextBox ID="txtAmountForm4" runat="server" CssClass="form-control" placeholder="Số tiền cần chuyển" required="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtNote">Nội dung chuyển khoản</label>
                                                <asp:TextBox ID="txtNoteForm4" runat="server" CssClass="form-control" placeholder="Nội dung chuyển khoản"></asp:TextBox>
                                            </div>
                                            <div class="box-footer">
                                                <asp:Button ID="btnCashForm4" runat="server" Text="Chuyển tiền" CssClass="btn btn-info" OnClick="btnCashForm4_Click"></asp:Button>
                                                <asp:Button ID="btnCancelForm4" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="chart tab-pane active" id="account-info">
                                <div class="col-md-8">
                                    <div class="form-group">
                                        <label for="txtName">Số điện thoại *</label>
                                        <asp:TextBox ID="txtMomoId" runat="server" CssClass="form-control" placeholder="Số điện thoại"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtMomoName">Tên tài khoản *</label>
                                        <asp:TextBox ID="txtMomoName" runat="server" CssClass="form-control" placeholder="Tên tài khoản "></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtMomoPass">Mật khẩu *</label>
                                        <asp:TextBox ID="txtMomoPass" TextMode="Password" runat="server" CssClass="form-control" placeholder="Mật khẩu"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtBalanceMaxDay">Số tiền nhận tối đa trong ngày (triệu) *</label>
                                        <asp:TextBox ID="txtBalanceMaxDay" runat="server" CssClass="form-control" placeholder="Số tiền nhận tối đa trong ngày"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtBalanceMaxMonth">Số tiền nhận tối đa trong tháng (triệu) *</label>
                                        <asp:TextBox ID="txtBalanceMaxMonth" runat="server" CssClass="form-control" placeholder="Số tiền nhận tối đa trong tháng"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtClassName">Loại  *</label>
                                        <asp:DropDownList ID="drpType" runat="server" CssClass="form-control select2">
                                            <asp:ListItem Text="IN" Value="IN"></asp:ListItem>
                                            <asp:ListItem Text="OUT" Value="OUT"></asp:ListItem>
                                            <asp:ListItem Text="INOUT" Value="INOUT"></asp:ListItem>
                                            <asp:ListItem Text="OUTALL" Value="OUTALL"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtSolution">Giải pháp  *</label>
                                        <asp:DropDownList ID="drpSolution" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="Chọn giải pháp:" Value=""></asp:ListItem>
                                            <asp:ListItem Text="APIV2" Value="APIV2"></asp:ListItem>
                                            <asp:ListItem Text="API" Value="API"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">

                                        <label for="txtSolution">Trạng thái  *</label>
                                        <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">

                                            <asp:ListItem Text="Kích hoạt" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Chưa kích hoạt" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Bỏ qua" Value="-1"></asp:ListItem>
                                        </asp:DropDownList>

                                    </div>

                                    <div class="box-footer">
                                        <asp:Button ID="btAdd" runat="server" Text="Cập nhật" CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
                                        <asp:Button ID="btCancel" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="box-body">
                                        <div class="callout callout-success" id="divResultSync" runat="server" visible="False">
                                            <p id="syncMessage" runat="server"><i>Bạn đã thực hiện lệnh đồng bộ thành công (Client sẽ thực hiện lệch và đồng bộ sau ít phút).</i></p>
                                        </div>
                                        <blockquote style="border-color: orange" id="blockBalance" runat="server">
                                            <p>Đồng bộ số dư tài khoản.</p>
                                            <h5>&#9733; Số dư Web&nbsp;&nbsp;&nbsp;&nbsp;:
                                                <asp:Label ID="lblBalanceWeb" runat="server"></asp:Label></h5>
                                            <h5>&#9734; Số dư Momo:
                                                <asp:Label ID="lblBalanceMomo" runat="server"></asp:Label></h5>
                                            <asp:Button ID="btnSyncBalance" runat="server" Text="Đồng bộ" CssClass="btn btn-block btn-default btn-flat" OnClick="bntSyncBalance_Click"></asp:Button>
                                        </blockquote>

                                        <blockquote id="loginStatus" runat="server" style="border-color: green">

                                            <p>
                                                Trạng thái đăng nhập
                                                <h5>
                                                    <asp:Label ID="lblDescription" runat="server"></asp:Label><br />


                                                    <i>(Chú ý: Hạn chế việc đăng kí thiết bị mới chỉ dưới 5 lần/day)</i></h5>
                                            </p>


                                            <asp:Panel runat="server" ID="pnLogin" Visible="False">
                                                <asp:Button ID="btnSenOtpSms" runat="server" Text="Đăng thiết bị mới" CssClass="btn btn-block btn-default btn-flat" OnClick="bntSendOTPMsg_Click"></asp:Button>
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="pnOtpLogin" Visible="False">
                                                <asp:TextBox ID="txtOtp" runat="server" CssClass="form-control" placeholder="Nhập mã OTP"></asp:TextBox>
                                                <asp:Button ID="btnOtpLogin" runat="server" Text="Xác nhận" CssClass="btn btn-block btn-default btn-flat" OnClick="bntOtpLogin_Click"></asp:Button>
                                            </asp:Panel>
                                            <asp:Panel runat="server" ID="pnCap" Visible="False">
                                                <h5>(Chú ý : Bạn cần ấn mở lại để hệ thống quét thông tin giao dịch)</h5>
                                                <asp:Button ID="btnOpen" runat="server" Text="Mở lại" CssClass="btn btn-block btn-default btn-flat" OnClick="bntOpen_Click"></asp:Button>
                                            </asp:Panel>
                                        </blockquote>
                                        <blockquote id="profileimg" runat="server" style="border-color: blue" visible="true">
                                            <p>Ảnh chân dung</p>
                                            <h5>Sau khi upload ảnh bạn cần ấn nút "Cập Nhật" phía dưới để lưu thông tin</h5>
                                            <div class="row profileimg">
                                                <div class="col-md-3 no-padding">
                                                    <a href="javascript:;" class="btndeleteimg" onclick="deleteimg1()" title="xóa ảnh">
                                                        <img src="/cmspay/content/deleteicon.png" height="16" /></a>
                                                    <a href="javascript:;" data-fancybox="gallery" class="base64-img">
                                                        <asp:Image ID="img1" runat="server" Width="98%" Height="120" />
                                                    </a>
                                                    <asp:HiddenField runat="server" ID="hdimg1" Value="1" />
                                                    <div style="height: 10px;"></div>
                                                    <asp:FileUpload ID="fileUpload1" accept="image/*" runat="server" />
                                                </div>
                                                <div class="col-md-3 no-padding">
                                                    <a href="javascript:;" class="btndeleteimg" onclick="deleteimg2()" title="xóa ảnh">
                                                        <img src="/cmspay/content/deleteicon.png" height="16" />
                                                    </a>
                                                    <asp:HiddenField runat="server" ID="hdimg2" Value="1" />
                                                    <a href="javascript:;" class="base64-img">
                                                        <asp:Image ID="img2" runat="server" Width="98%" Height="120" /></a>
                                                    <div style="height: 10px;"></div>
                                                    <asp:FileUpload ID="fileUpload2" accept="image/*" runat="server" />

                                                </div>

                                                <div class="col-md-3 no-padding">
                                                    <a href="javascript:;" class="btndeleteimg" onclick="deleteimg3()" title="xóa ảnh">
                                                        <img src="/cmspay/content/deleteicon.png" height="16" /></a>
                                                    <asp:HiddenField runat="server" ID="hdimg3" Value="1" />
                                                    <a href="javascript:;" class="base64-img">
                                                        <asp:Image ID="img3" runat="server" Width="98%" Height="120" />
                                                    </a>
                                                    <div style="height: 10px;"></div>
                                                    <asp:FileUpload ID="fileUpload3" accept="image/*" runat="server" />

                                                </div>
                                                <div class="col-md-3 no-padding">
                                                    <a href="javascript:;" class="btndeleteimg" onclick="deleteimg4()" title="xóa ảnh">
                                                        <img src="/cmspay/content/deleteicon.png" height="16" /></a>
                                                    <asp:HiddenField runat="server" ID="hdimg4" Value="1" />
                                                    <a href="javascript:;" class="base64-img">
                                                        <asp:Image ID="img4" runat="server" Width="98%" Height="120" />
                                                    </a>
                                                    <div style="height: 10px;"></div>
                                                    <asp:FileUpload ID="fileUpload4" accept="image/*" runat="server" />

                                                </div>
                                                <div style="clear: both; height: 20px;"></div>
                                                <div class="col-md-3 no-padding">
                                                    <a href="javascript:;" class="btndeleteimg" onclick="deleteimg5()" title="xóa ảnh">
                                                        <img src="/cmspay/content/deleteicon.png" height="16" /></a>
                                                    <a href="javascript:;" data-fancybox="gallery" class="base64-img">
                                                        <asp:Image ID="img5" runat="server" Width="98%" Height="120" />
                                                    </a>
                                                    <asp:HiddenField runat="server" ID="hdimg5" Value="1" />
                                                    <div style="height: 10px;"></div>
                                                    <asp:FileUpload ID="fileUpload5" accept="image/*" runat="server" />
                                                </div>
                                                <div class="col-md-3 no-padding">
                                                    <a href="javascript:;" class="btndeleteimg" onclick="deleteimg6()" title="xóa ảnh">
                                                        <img src="/cmspay/content/deleteicon.png" height="16" />
                                                    </a>
                                                    <asp:HiddenField runat="server" ID="hdimg6" Value="1" />
                                                    <a href="javascript:;" class="base64-img">
                                                        <asp:Image ID="img6" runat="server" Width="98%" Height="120" /></a>
                                                    <div style="height: 10px;"></div>
                                                    <asp:FileUpload ID="fileUpload6" accept="image/*" runat="server" />

                                                </div>

                                                <div class="col-md-3 no-padding">
                                                    <a href="javascript:;" class="btndeleteimg" onclick="deleteimg7()" title="xóa ảnh">
                                                        <img src="/cmspay/content/deleteicon.png" height="16" /></a>
                                                    <asp:HiddenField runat="server" ID="hdimg7" Value="1" />
                                                    <a href="javascript:;" class="base64-img">
                                                        <asp:Image ID="img7" runat="server" Width="98%" Height="120" />
                                                    </a>
                                                    <div style="height: 10px;"></div>
                                                    <asp:FileUpload ID="fileUpload7" accept="image/*" runat="server" />

                                                </div>
                                                <div class="col-md-3 no-padding">
                                                    <a href="javascript:;" class="btndeleteimg" onclick="deleteimg8()" title="xóa ảnh">
                                                        <img src="/cmspay/content/deleteicon.png" height="16" /></a>
                                                    <asp:HiddenField runat="server" ID="hdimg8" Value="1" />
                                                    <a href="javascript:;" class="base64-img">
                                                        <asp:Image ID="img8" runat="server" Width="98%" Height="120" />
                                                    </a>
                                                    <div style="height: 10px;"></div>
                                                    <asp:FileUpload ID="fileUpload8" accept="image/*" runat="server" />

                                                </div>
                                                <div style="clear: both; height: 10px">
                                                </div>
                                                <div class="form-group" style="text-align: left; font-size: 14px">
                                                    <div class="checkbox">
                                                        <label for="cbxIsActive">
                                                            <asp:CheckBox ID="chkIsActive" Checked="False" runat="server"></asp:CheckBox>Lấy Detech
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                        </blockquote>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <!-- /.box -->
            </div>
            <!-- /.col -->
        </div>

        <!-- /.row -->
    </section>
    <!-- /.content -->
    <asp:HiddenField ID="hidAccountTAB" runat="server" Value="#account-info" />
    <asp:HiddenField ID="hidBalanceTAB" runat="server" Value="#detail-form-1" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">


    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {
            var tabAccount = document.getElementById('<%= hidAccountTAB.ClientID%>').value;
            $('#accountTabs a[href="' + tabAccount + '"]').tab('show');

            var tabBalance = document.getElementById('<%= hidBalanceTAB.ClientID%>').value;
            $('#balanceTabs a[href="' + tabBalance + '"]').tab('show');
        });

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })

        //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        //var target = $(e.target).attr("href") // activated tab
        //alert(target);
        //});

        function validate(params, param) {
            $("#<%=txtAmountForm1.ClientID %>").prop('required', false);

            $("#<%=ddlBank.ClientID %>").prop('required', false);
            $("#<%=txtBankNumber.ClientID %>").prop('required', false);
            $("#<%=txtAmountForm2.ClientID %>").prop('required', false);
            $("#<%=txtBankHolderName.ClientID %>").prop('required', false);

            $("#<%=txtMomoIdForm4.ClientID %>").prop('required', false);
            $("#<%=txtMomoNameForm4.ClientID %>").prop('required', false);
            $("#<%=txtAmountForm4.ClientID %>").prop('required', false);



            if (Array.isArray(params)) {
                for (i = 0; i < params.length; i++) {
                    $(params[i]).prop('required', true);
                }
            }

        }
        function deleteimg1() {
            if (confirm("Bạn có muỗn xóa ảnh?")) {
                $("#<%=img1.ClientID %>").attr("src", "/cmspay/content/noimage.png");
                $("#<%=hdimg1.ClientID %>").val("0");
            }

        }
        function deleteimg2() {
            if (confirm("Bạn có muỗn xóa ảnh?")) {
                $("#<%=img2.ClientID %>").attr("src", "/cmspay/content/noimage.png");
                $("#<%=hdimg2.ClientID %>").val("0");
            }

        }
        function deleteimg3() {
            if (confirm("Bạn có muỗn xóa ảnh?")) {
                $("#<%=img3.ClientID %>").attr("src", "/cmspay/content/noimage.png");
                $("#<%=hdimg3.ClientID %>").val("0");
            }

        }
        function deleteimg4() {
            if (confirm("Bạn có muỗn xóa ảnh?")) {
                $("#<%=img4.ClientID %>").attr("src", "/cmspay/content/noimage.png");
                $("#<%=hdimg4.ClientID %>").val("0");
            }

        }

        function deleteimg5() {
            if (confirm("Bạn có muỗn xóa ảnh?")) {
                $("#<%=img5.ClientID %>").attr("src", "/cmspay/content/noimage.png");
                $("#<%=hdimg5.ClientID %>").val("0");
            }

        }
        function deleteimg6() {
            if (confirm("Bạn có muỗn xóa ảnh?")) {
                $("#<%=img6.ClientID %>").attr("src", "/cmspay/content/noimage.png");
                $("#<%=hdimg6.ClientID %>").val("0");
            }

        }
        function deleteimg7() {
            if (confirm("Bạn có muỗn xóa ảnh?")) {
                $("#<%=img7.ClientID %>").attr("src", "/cmspay/content/noimage.png");
                $("#<%=hdimg7.ClientID %>").val("0");
            }

        }
        function deleteimg8() {
            if (confirm("Bạn có muỗn xóa ảnh?")) {
                $("#<%=img8.ClientID %>").attr("src", "/cmspay/content/noimage.png");
                $("#<%=hdimg8.ClientID %>").val("0");
            }

        }
        $("#<%=img4.ClientID %>").height($("#<%=img4.ClientID %>").width() / 3 * 4);
        $("#<%=img3.ClientID %>").height($("#<%=img3.ClientID %>").width() / 3 * 4);
        $("#<%=img2.ClientID %>").height($("#<%=img2.ClientID %>").width() / 3 * 4);
        $("#<%=img1.ClientID %>").height($("#<%=img1.ClientID %>").width() / 3 * 4);

        $("#<%=img8.ClientID %>").height($("#<%=img8.ClientID %>").width() / 3 * 4);
        $("#<%=img7.ClientID %>").height($("#<%=img7.ClientID %>").width() / 3 * 4);
        $("#<%=img6.ClientID %>").height($("#<%=img6.ClientID %>").width() / 3 * 4);
        $("#<%=img5.ClientID %>").height($("#<%=img5.ClientID %>").width() / 3 * 4);


        //$(document).ready(function () {
        //    $('.base64-img').on('click', function () {
        //        var src = $(this).children('img').first().attr('src');
        //        $('#zoom-img').attr('src', src);
        //        $('#zoom-modal').css('display', 'flex'); // sho
        //    });

        //    $('#zoom-modal').on('click', function () {
        //        $(this).hide();
        //    });
        //});
        $(document).ready(function () {
            $('.base64-img').each(function () {
                var imgSrc = $(this).find('img').attr('src');
                $(this).attr('href', imgSrc);
            });
        });
        $(document).ready(function () {
            $(".base64-img").fancybox({

            });
        });

    </script>
    <style>
        input[type=file] {
            font-size: 10px;
            width: 100%;
            text-overflow: ellipsis;
        }

        .profileimg {
            text-align: center;
            margin-top: 40px;
        }

        .btndeleteimg {
            position: absolute;
            right: 0;
            top: -22px;
        }

        #zoom-modal {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.8);
            justify-content: center; /* căn giữa theo chiều ngang */
            align-items: center; /* căn giữa theo chiều dọc */
            z-index: 9999;
        }

            #zoom-modal img {
                max-width: 80%;
                max-height: 80%;
            }
    </style>
</asp:Content>


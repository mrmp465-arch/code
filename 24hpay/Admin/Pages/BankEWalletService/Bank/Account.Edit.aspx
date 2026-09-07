<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Account.Edit.aspx.cs" Inherits="Pages_BankEWalletService_Bank_Account_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình Bank
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Tài khoản Bank" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Cập nhật tài khoản Bank</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Bank</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Tài khoản Bank</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.ITAccount %>">Tài khoản chứa</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.ITAccount2 %>">Tài khoản riêng</a></li>
                <%--<li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankPartner %>">Quản lý kênh</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerBank %>">Phân bố kênh & tài khoản</a></li>--%>
            </ul>

            <div class="row">
                <div class="col-xs-12">
                    <div class="nav-tabs-custom">
                        <!-- Tabs within a box -->
                        <ul class="nav nav-tabs pull-right ui-sortable-handle" id="accountTabs">
                            <li class=""><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankUploadVideo%>?id=<%=Id%>&bankcode=<%=drpBankCode.SelectedValue%>&bankid=<%=txtBankId.Text%>">Video</a></li>
                            <li class=""><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankUploadImage%>?id=<%=Id%>">Ảnh</a> </li>
                            <% if (RoleTransfer)
                                { %>
                            <li class=""><a href="#balance-info" data-toggle="tab">Chuyển tiền</a></li>
                            <% } %>
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
                                        </div>
                                        <div class="box-body no-padding" style="">
                                            <ul class="nav nav-stacked" id="balanceTabs">
                                                <li class="active"><a href="#detail-form-4" data-toggle="tab"><i class="fa fa-credit-card"></i>&nbsp;Chuyển khoản Bank (tự động)</a></li>
                                            </ul>
                                        </div>
                                        <!-- /.box-body -->
                                    </div>

                                </div>
                                <div class="col-md-9">
                                </div>
                                <div class="tab-content">
                                    <div class="tab-pane active" id="detail-form-4">

                                        <div class="col-md-9" id="divTransfer" runat="server">
                                            <div class="callout callout-success" id="calloutForm4" runat="server" visible="False">
                                                <p id="resTextForm4" runat="server"><i>Bạn đã rút tiền thành công.</i></p>
                                            </div>
                                            <div class="form-group" style="display:none">
                                                <label for="drpBankCode4">Chọn từ tài khoản riêng *</label>

                                                <asp:DropDownList ID="drpBankCode4" runat="server" CssClass="form-control select2">
                                                    <asp:ListItem Text="Chọn từ tài khoản riêng: " Value=""></asp:ListItem>
                                                </asp:DropDownList>

                                            </div>
                                            <div class="form-group">
                                                <label for="drpBankCode2">Chọn từ tài khoản chứa *</label>

                                                <asp:DropDownList ID="drpBankCode2" runat="server" CssClass="form-control select2">
                                                    <asp:ListItem Text="Chọn từ tài khoản chứa: " Value=""></asp:ListItem>
                                                </asp:DropDownList>

                                            </div>
                                            <div class="form-group">
                                                <label for="drpBankCode3">Chọn từ tài khoản out *</label>

                                                <asp:DropDownList ID="drpBankCode3" runat="server" CssClass="form-control select2">
                                                    <asp:ListItem Text="Chọn từ tài khoản out: " Value=""></asp:ListItem>
                                                </asp:DropDownList>

                                            </div>

                                            <div class="form-group">
                                                <label for="drpBankCodeForm4">Ngân hàng người nhận *</label>
                                                <asp:DropDownList ID="drpBankCodeForm4" runat="server" CssClass="form-control" required>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtBankIdForm4">Số tài khoản người nhận *</label>
                                                <asp:TextBox ID="txtBankIdForm4" runat="server" CssClass="form-control" placeholder="Số tài khoản người nhận" required="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <label for="txtBankNameForm4">Tên tài khoản người nhận *</label>
                                                <asp:TextBox ID="txtBankNameForm4" runat="server" CssClass="form-control" placeholder="Tên ngưởi nhận" required="true"></asp:TextBox>
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
                                                <asp:Button ID="btnCashForm4" runat="server" Text="Chuyển tiền" CssClass="btn btn-info" OnClick="btnCashForm4_Click" OnClientClick="return confirm('Bạn có chắc muốn chuyển tiền không?');"></asp:Button>
                                                <asp:Button ID="btnCancelForm4" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="chart tab-pane active" id="account-info">
                                <div class="col-md-8">
                                    <div class="form-group">
                                        <label for="txtClassName">Bank Code *</label>
                                        <asp:DropDownList ID="drpBankCode" runat="server" CssClass="form-control" required>
                                            <asp:ListItem Text="Chọn Bank:" Value=""></asp:ListItem>
                                            <asp:ListItem Text="ACB" Value="ACB"></asp:ListItem>
                                            <asp:ListItem Text="VPB" Value="VPB"></asp:ListItem>
                                            <asp:ListItem Text="SEAB" Value="SEAB"></asp:ListItem>
                                            <asp:ListItem Text="ICB" Value="ICB"></asp:ListItem>
                                            <asp:ListItem Text="MB" Value="MB"></asp:ListItem>
                                            <asp:ListItem Text="VCB" Value="VCB"></asp:ListItem>
                                            <asp:ListItem Text="BIDV" Value="BIDV"></asp:ListItem>
                                            <asp:ListItem Text="TIMO" Value="TIMO"></asp:ListItem>
                                            <asp:ListItem Text="OCB" Value="OCB"></asp:ListItem>
                                            <asp:ListItem Text="TPB" Value="TPB"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtClassName">Bank Type *</label>
                                        <asp:DropDownList ID="drpBankType" runat="server" CssClass="form-control" required>
                                            <asp:ListItem Text="Chọn loại:" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Cá nhân" Value="IND"></asp:ListItem>
                                            <asp:ListItem Text="Doanh nghiệp" Value="BIZ"></asp:ListItem>

                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtBankName">Tên tài khoản *</label>
                                        <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control" placeholder="Tên tài khoản "></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtName">Bank Id *</label>
                                        <asp:TextBox ID="txtBankId" runat="server" CssClass="form-control" placeholder="Số điện thoại"></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtName">Tên đăng nhập *</label>
                                        <asp:TextBox ID="txtBankAccount" runat="server" CssClass="form-control" placeholder="Tài khoản đăng nhập App" required></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtBankPass">Mật khẩu *</label>
                                        <asp:TextBox ID="txtBankPass" TextMode="Password" runat="server" CssClass="form-control" placeholder="Mật khẩu"></asp:TextBox>
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
                                            <asp:ListItem Text="API" Value="API"></asp:ListItem>
                                            <asp:ListItem Text="LD" Value="LD"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtComputer">Computer</label>
                                        <asp:TextBox ID="txtComputer" runat="server" CssClass="form-control" placeholder="Tên máy tính chứa Client"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtPhone">Phone</label>
                                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Tên điện thoại cài app"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtPhone">CloudPhoneId</label>
                                        <asp:TextBox ID="txtCloudPhoneId" runat="server" CssClass="form-control" placeholder="CloudPhoneId"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtPhone">Pin OTP</label>
                                        <asp:TextBox ID="txtPinOtp" runat="server" CssClass="form-control" placeholder="Mã PIN của OTP"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtPhone">AppDeviceId</label>
                                        <asp:TextBox ID="txtAppDeviceId" runat="server" CssClass="form-control" placeholder="AppDeviceId"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtPhone">Ghi chú</label>
                                        <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" placeholder="Ghi chú"></asp:TextBox>
                                    </div>
                                    <div class="checkbox">
                                        <label for="cbxIsActive">
                                            <asp:CheckBox ID="chkIsActive" Checked="False" runat="server"></asp:CheckBox>Trạng thái
                                        </label>
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

                                        <blockquote style="border-color: orange">
                                            <p>Đồng bộ số dư tài khoản.</p>
                                            <h5>&#9733; Số dư Web&nbsp;&nbsp;:
                                                <asp:Label ID="lblBalanceWeb" runat="server"></asp:Label></h5>
                                            <h5>&#9734; Số dư Bank:
                                                <asp:Label ID="lblBalanceBank" runat="server"></asp:Label></h5>
                                            <h5 runat="server" id="DVOTP" visible="False">&#9734; OTP: 
                                                <asp:Label ID="lbOTP" runat="server"></asp:Label></h5>
                                            <asp:Button ID="btnSyncBalance" runat="server" Text="Đồng bộ" CssClass="btn btn-block btn-default btn-flat" OnClick="bntSyncBalance_Click"></asp:Button>
                                        </blockquote>
                                        <blockquote id="loginStatus" runat="server" style="border-color: green">
                                            <asp:Panel runat="server" ID="pnGroupLogin">
                                                <p>
                                                    Trạng thái đăng nhập.
                                                        <h5>
                                                            <asp:Label ID="lblDescription" runat="server"></asp:Label><br />
                                                            <i>(Chú ý: Chỉ thực hiện đăng nhập lại 1 lần duy nhất để tránh khoá tài khoản)</i></h5>
                                                </p>

                                                <asp:Panel runat="server" ID="pnLogin" Visible="False">
                                                    <asp:Button ID="btnReLogin" runat="server" Text="Đăng nhập lại" CssClass="btn btn-block btn-default btn-flat" OnClick="bntReLogin_Click"></asp:Button>
                                                </asp:Panel>
                                                <asp:Panel runat="server" ID="pnOtpLogin" Visible="False">
                                                    <asp:TextBox ID="txtOtp" runat="server" CssClass="form-control" placeholder="Nhập mã OTP"></asp:TextBox>
                                                    <asp:Button ID="btnOtpLogin" runat="server" Text="Xác nhận" CssClass="btn btn-block btn-default btn-flat" OnClick="bntOtpLogin_Click"></asp:Button>
                                                </asp:Panel>

                                            </asp:Panel>
                                            <br />
                                            <br />
                                            <asp:Panel runat="server" ID="pnGroupLogin2">

                                                <p>
                                                    Đăng ký SmartOTP
                                                     <h5>
                                                         <asp:Label ID="lblDescription2" runat="server"></asp:Label><br />
                                                         <i>(Chú ý:  Chỉ thực hiện đăng kí 1 lần khi lên tài khoản, hoặc bị đăng nhập bời thiết bị khác)</i></h5>
                                                </p>

                                                <asp:Panel runat="server" ID="pnLogin2" Visible="False">
                                                    <asp:Button ID="btnReLogin2" runat="server" Text="Đăng ký lại" CssClass="btn btn-block btn-default btn-flat" OnClick="bntReLogin2_Click"></asp:Button>
                                                </asp:Panel>
                                                <asp:Panel runat="server" ID="pnOtpLogin2" Visible="False">
                                                    <asp:TextBox ID="txtOtp2" runat="server" CssClass="form-control" placeholder="Nhập mã OTP"></asp:TextBox>
                                                    <asp:Button ID="btnOtpLogin2" runat="server" Text="Xác nhận" CssClass="btn btn-block btn-default btn-flat" OnClick="bntOtpLogin2_Click"></asp:Button>
                                                </asp:Panel>
                                            </asp:Panel>
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
    <div id="zoom-modal">
        <img id="zoom-img" src="">
    </div>
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


            var hash = window.location.hash;

            if (hash === "#balance-info") {

                $('#accountTabs a[href="#balance-info"]').tab('show');
            }
        });

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })

        //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        //var target = $(e.target).attr("href") // activated tab
        //alert(target);
        //});
        $('#<%= drpBankCode2.ClientID%>').on('change', function () {
            if (this.value != '') {

                var result = this.value.split("-");
                $("#<%= drpBankCodeForm4.ClientID%>").val(result[0]);
                $("#<%= txtBankNameForm4.ClientID%>").val(result[1]);
                $("#<%= txtBankIdForm4.ClientID%>").val(result[2]);
            }

        });
        $('#<%= drpBankCode3.ClientID%>').on('change', function () {
            if (this.value != '') {

                var result = this.value.split("-");
                $("#<%= drpBankCodeForm4.ClientID%>").val(result[0]);
                $("#<%= txtBankNameForm4.ClientID%>").val(result[1]);
                $("#<%= txtBankIdForm4.ClientID%>").val(result[2]);
            }

        });
        $('#<%= drpBankCode4.ClientID%>').on('change', function () {
            if (this.value != '') {

                var result = this.value.split("-");
                $("#<%= drpBankCodeForm4.ClientID%>").val(result[0]);
         $("#<%= txtBankNameForm4.ClientID%>").val(result[1]);
                 $("#<%= txtBankIdForm4.ClientID%>").val(result[2]);
             }

         });
        function validate(params, param) {

            $("#<%=txtBankIdForm4.ClientID %>").prop('required', false);
            $("#<%=txtBankNameForm4.ClientID %>").prop('required', false);
            $("#<%=txtAmountForm4.ClientID %>").prop('required', false);
            $("#<%=drpBankCodeForm4.ClientID %>").prop('required', false);

            //$(param).prop('disabled', true);

            //$(param).click(function () {                
            //    alert(`Now You Postback Start`);
            //});

            if (Array.isArray(params)) {
                for (i = 0; i < params.length; i++) {
                    $(params[i]).prop('required', true);
                }
            }

        }
        this.DocumentHeght = function () {
            return $(document).height();
        };
        this.GetFullHeight = function () {
            return parseInt($(document).scrollTop() + $('html').height());
        };
        this.DocumentWidth = function () { return $(document).width(); };
        this.WindowHeight = function () { return $(window).height(); };
        this.WindowWidth = function () { return $(window).width(); };
        function Loading() {
            //this.UnLoading();
            var html = '<img src="/cmspay/Content/loading2014.gif" height="30px" style="margin-top:-5px" alt="loadding" />';
            $("#<%=lblBalanceBank.ClientID %>").html(html);

        };
        function UnLoading() {
            $('#LoadingContainer').remove();

        };
        if ($("#<%=lblBalanceBank.ClientID %>").html() != "-1") {
            var params = {
                BankCode: $("#<%=drpBankCode.ClientID %>").val(),
                BankId: $("#<%=txtBankId.ClientID %>").val(),

            };
            Loading();
            $.ajax({
                type: 'GET',
                url: '/cmspay/ServiceHandler/GetBankBalance.ashx',
                data: params,
                success: function (data) {
                    /*UnLoading();*/
                    $("#<%=lblBalanceBank.ClientID %>").html(data);

                },
                error: function () {
                    $("#<%=lblBalanceBank.ClientID %>").html("");
                    /*  UnLoading();*/
                }
            });
        }



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

        .select2-container {
            display: block;
            width: 500px !important;
        }
    </style>
</asp:Content>

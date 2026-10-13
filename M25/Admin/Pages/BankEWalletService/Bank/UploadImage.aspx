<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="UploadImage.aspx.cs" Inherits="Pages_BankEWalletService_Bank_UploadImage" %>

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
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankPartner %>">Quản lý kênh</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerBank %>">Phân bố kênh & tài khoản</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">
                    <div class="nav-tabs-custom">
                        <!-- Tabs within a box -->
                        <ul class="nav nav-tabs pull-right ui-sortable-handle" id="accountTabs">
                              <li class=""><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankUploadVideo%>?id=<%=Id%>">Video</a> </li>
                            <li class="active"><a href="#account-Ảnh" data-toggle="tab">Ảnh</a></li>
                            <li class=""><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankAccountEdit%>?id=<%=Id%>#balance-info">Chuyển tiền</a></li>
                            <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankAccountEdit%>?id=<%=Id%>#account-info">Thông tin tài khoản</a></li>
                            <li class="pull-left header"></li>
                        </ul>
                        <div class="tab-content">
                            <div class="chart tab-pane active" id="account-Ảnh">
                                <div class="col-md-3">
                                    <div class="box box-solid">
                                        <div class="box-header with-border">
                                            <h3 class="box-title">
                                                <asp:Label runat="server" ID="lblAccount"></asp:Label></h3>
                                        </div>
                                        <div class="box-body no-padding" style="">
                                            <ul class="nav nav-stacked" id="balanceTabs">
                                                <li class="active"><a href="#detail-form-4" data-toggle="tab"><i class="fa fa-image"></i>&nbsp;Upload ảnh</a></li>
                                            </ul>
                                        </div>
                                        <!-- /.box-body -->
                                    </div>
                                    <br />
                                    <br />
                                    <b>Lưu ý up ảnh cho VPB</b>
                                    <h5>- Ảnh nhìn thẳng: từ 1 đến 4</h5>
                                    <h5>- Ảnh ngẩng mặt: ảnh 5 </h5>
                                    <h5>- Ảnh cúi mặt: ảnh 6 </h5>
                                    <h5>- Ảnh quay trái: ảnh 7 </h5>
                                    <h5>- Ảnh quay phải: ảnh 8 </h5>
                                    <h5>- Kích thước ảnh :1080x1440 </h5>
                                </div>
                                <div class="col-md-9">
                                    <blockquote id="profileimg" runat="server" style="border-color: blue" visible="true">
                                        <p>Ảnh chân dung</p>
                                        <h5>Sau khi upload ảnh bạn cần ấn nút "Cập Nhật" phía dưới để lưu thông tin</h5>
                                        <div class="row profileimg">
                                            <div class="col-md-6">
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
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="row profileimg">
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
                                                </div>
                                            </div>

                                            <div style="clear: both; height: 20px;"></div>

                                        </div>
                                    </blockquote>


                                    <div class="box-footer">
                                        <asp:Button ID="btAdd" runat="server" Text="Cập nhật" CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
                                        &nbsp;
                                         <asp:Button ID="btSave" runat="server" Text="Save DB" CssClass="btn btn-info " OnClick="btSave_Click"></asp:Button>
                                         &nbsp;
                                        <asp:Button ID="btCancel" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
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
    <div id="zoom-modal">
        <img id="zoom-img" src="">
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />
    <script type="text/javascript">

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })

        //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        //var target = $(e.target).attr("href") // activated tab
        //alert(target);
        //});



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
    <style>
        #ctl00_ContentPlaceHolder1_pnGroupLogin2 {
            background: #e8f4ff; /* xanh nhạt */
            border: 1px solid #b6dbff;
            border-left: 5px solid #1890ff; /* viền xanh đậm bên trái */
            padding: 16px 18px;
            margin-bottom: 20px;
            border-radius: 6px;
        }
    </style>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" EnableEventValidation="true" CodeFile="Nap.aspx.cs" Inherits="Nap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- Morris charts -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/morris.js/morris.css">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.2.1/css/all.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Dashboard
        </h1>
        <ol class="breadcrumb">
            <li><a href="Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Dashboard</li>
        </ol>
    </section>
    <asp:Panel ID="PanelContent" runat="server">

        <section class="content">
            <!-- Info boxes -->

            <div class="row">
                <div class="box-body"></div>
            </div>

            <div class="row">
                <!-- /.row -->




                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title">Tạo giao dịch momo</h3>
                        </div>

                        <!-- /.box-header -->
                        <div class="box-body">

                            <div class="form-group row">
                                <%-- <div class="col-xs-3 col-sm-6 col-md-4 col-lg-3">
                                  <asp:DropDownList CssClass="form-control" ID="drpBankCode" runat="server">
                                        <asp:ListItem Text="--Chọn loại bank--" Value="1"></asp:ListItem>
                                      <asp:ListItem Text="VCB" Value="2"></asp:ListItem>
                                  </asp:DropDownList>
                              </div>--%>
                                <div class="col-md-12">
                                    Lưu ý: mỗi một lệnh chỉ được chuyển tiền 1 lần.
                                    <br />
                                    Chuyển thành công cần ấn nút tạo lệnh để lấy thông tin mới. Số tiền tối đa là 5.000.000
                                </div>
                                <br />
                                <div style="clear: both; margin-bottom: 20px;"></div>
                                <div class="col-md-3">

                                    <asp:TextBox ID="txtAmount" type="number" step="5000000" runat="server" CssClass="form-control" placeholder="Số tiền cần nạp" Text="5000000"></asp:TextBox>
                                </div>
                                <label id="textMoneyVND" class="control-label col-md-2" style="margin-top: 8px">
                                    &nbsp;
                                </label>
                                <div class="col-md-2">
                                    <asp:Button ID="btAdd" runat="server" CssClass="btn btn-primary" OnClick="btAdd_Click" Text="Tạo lệnh"></asp:Button>
                                </div>
                            </div>
                            <br />

                            <div class="form-group" style="height: 295px; margin-top: 15px">
                                <div id="dvBankInfo" runat="server" visible="false">
                                    <div class="row">

                                        <div class="col-md-3">
                                            <div style="display: none">
                                                <input class="form-control" readonly="readonly" id="name" value="<%=MomoName %>" />
                                                <input class="form-control" readonly="readonly" id="account" value="<%=MomoId %>" />
                                            </div>
                                            <div style="margin-top: 20px; margin-left: 45px;">
                                                <dt>Tài khoản</dt>
                                                <dd>

                                                    <asp:Label ID="lbAccountNumber" runat="server" Text=""></asp:Label>
                                                    &nbsp;
                                                    <button type="button" class="btn" data-clipboard-target="#account>">
                                                        <i class="fas fa-copy" onclick="copyStringToClipboard('account')"></i>
                                                    </button>
                                                </dd>
                                                <br />
                                                <dt>Tên tài khoản</dt>
                                                <dd>

                                                    <asp:Label ID="lbAccountName" runat="server" Text=""></asp:Label>&nbsp;
                                                    <button type="button" class="btn" data-clipboard-target="#name>">
                                                        <i class="fas fa-copy" onclick="copyStringToClipboard('name')"></i>
                                                    </button>
                                                </dd>
                                                <br />
                                                <dt>Địa chỉ</dt>
                                                <dd>

                                                    <asp:Label ID="lbInfo" runat="server" Text=""></asp:Label>
                                                </dd>
                                                <br />


                                            </div>

                                        </div>
                                        <div class="col-md-4">
                                            <div style="width: 300px; margin-left: 0px;">
                                                <asp:Image ID="qrCode" runat="server" Width="200" />

                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
             
            </div>

        </section>

    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <!-- page script -->
    <!-- Morris.js charts -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/raphael/2.1.0/raphael-min.js"></script>


    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript">
        function copyStringToClipboard(id) {
            var str = document.getElementById(id).value;
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
            var x = document.getElementById("snackbar");
            x.className = "show";
            setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
        }



        $(function () {
            $('.txtCreatTime').datepicker({
                autoclose: true,
                Accepts: 'bottom'
            });
        });



    </script>
    <script type="text/javascript">


        function formatPrice(price) {
            var fixedToSix = (Math.round(price * 1000000) / 1000000);
            return (Math.round(fixedToSix) == fixedToSix + 0.000001 ? fixedToSix + 0.000001 : fixedToSix);
        }
        Number.prototype.formatMoney = function (c, d, t) {
            var n = this,
                c = isNaN(c = Math.abs(c)) ? 2 : c,
                d = d == undefined ? "." : d,
                t = t == undefined ? "," : t,
                s = n < 0 ? "-" : "",
                i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
                j = (j = i.length) > 3 ? j % 3 : 0;
            return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
        };

        var unformat = function (value, decimal) {
            // Recursively unformat arrays:
            //            if (isArray(value)) {
            //                return map(value, function (val) {
            //                    return unformat(val, decimal);
            //                });
            //            }
            // Fails silently (need decent errors):
            value = value || 0;
            // Return the value as-is if it's already a number:
            if (typeof value === "number") return value;
            // Default decimal point is "." but could be set to eg. "," in opts:
            decimal = decimal || ",";
            // Build regex to strip out everything except digits, decimal point and minus sign:
            var regex = new RegExp("[^0-9-" + decimal + "]", ["g"]),
                unformatted = parseFloat(
                    ("" + value)
                        .replace(/\((.*)\)/, "-$1") // replace bracketed values with negatives
                        .replace(regex, '') // strip out any cruft
                        .replace(decimal, ',') // make sure decimal point is standard
                );
            // This will fail silently which may cause trouble, let's wait and see:
            return !isNaN(unformatted) ? unformatted : 0;
        };
        var DocTienBangChu = function (SoTien) {
            var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
            var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");
            var lan = 0;
            var i = 0;
            var so = 0;
            var KetQua = "";
            var tmp = "";
            var ViTri = new Array();
            if (SoTien < 0) return "Số tiền âm !";
            if (SoTien == 0) return "Không";
            if (SoTien > 0) {
                so = SoTien;
            }
            else {
                so = -SoTien;
            }
            if (SoTien > 8999999999999999) {
                //SoTien = 0;
                return "Số quá lớn!";
            }
            ViTri[5] = Math.floor(so / 1000000000000000);
            if (isNaN(ViTri[5]))
                ViTri[5] = "0";
            so = so - parseFloat(ViTri[5].toString()) * 1000000000000000;
            ViTri[4] = Math.floor(so / 1000000000000);
            if (isNaN(ViTri[4]))
                ViTri[4] = "0";
            so = so - parseFloat(ViTri[4].toString()) * 1000000000000;
            ViTri[3] = Math.floor(so / 1000000000);
            if (isNaN(ViTri[3]))
                ViTri[3] = "0";
            so = so - parseFloat(ViTri[3].toString()) * 1000000000;
            ViTri[2] = parseInt(so / 1000000);
            if (isNaN(ViTri[2]))
                ViTri[2] = "0";
            ViTri[1] = parseInt((so % 1000000) / 1000);
            if (isNaN(ViTri[1]))
                ViTri[1] = "0";
            ViTri[0] = parseInt(so % 1000);
            if (isNaN(ViTri[0]))
                ViTri[0] = "0";
            if (ViTri[5] > 0) {
                lan = 5;
            }
            else if (ViTri[4] > 0) {
                lan = 4;
            }
            else if (ViTri[3] > 0) {
                lan = 3;
            }
            else if (ViTri[2] > 0) {
                lan = 2;
            }
            else if (ViTri[1] > 0) {
                lan = 1;
            }
            else {
                lan = 0;
            }
            for (i = lan; i >= 0; i--) {
                tmp = DocSo3ChuSo(ViTri[i]);
                KetQua += tmp;
                if (ViTri[i] > 0) KetQua += Tien[i];
                if ((i > 0) && (tmp.length > 0)) KetQua += ',';//&& (!string.IsNullOrEmpty(tmp))
            }
            if (KetQua.substring(KetQua.length - 1) == ',') {
                KetQua = KetQua.substring(0, KetQua.length - 1);
            }
            KetQua = KetQua.substring(1, 2).toUpperCase() + KetQua.substring(2);
            //KetQua += " GG";
            return KetQua;//.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
        }
        //Hàm chuyển số thành chữ
        var DocSo3ChuSo = function (baso) {
            var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
            var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");
            var tram;
            var chuc;
            var donvi;
            var KetQua = "";
            tram = parseInt(baso / 100);
            chuc = parseInt((baso % 100) / 10);
            donvi = baso % 10;
            if (tram == 0 && chuc == 0 && donvi == 0) return "";
            if (tram != 0) {
                KetQua += ChuSo[tram] + " trăm ";
                if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
            }
            if ((chuc != 0) && (chuc != 1)) {
                KetQua += ChuSo[chuc] + " mươi";
                if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
            }
            if (chuc == 1) KetQua += " mười ";
            switch (donvi) {
                case 1:
                    if ((chuc != 0) && (chuc != 1)) {
                        KetQua += " mốt ";
                    }
                    else {
                        KetQua += ChuSo[donvi];
                    }
                    break;
                case 5:
                    if (chuc == 0) {
                        KetQua += ChuSo[donvi];
                    }
                    else {
                        KetQua += " lăm ";
                    }
                    break;
                default:
                    if (donvi != 0) {
                        KetQua += ChuSo[donvi];
                    }
                    break;
            }
            return KetQua;
        }
    </script>
    <script type="text/javascript">

        var refreshPageInterval = 120;

        setInterval("countDownPageRefresh()", 1000);//1 s gọi 1 lần
        $(document).mouseover(function () {
            funcResetRefreshPageInterval();
        });
        $(window).scroll(function () {
            funcResetRefreshPageInterval();
        });
        window.onkeypress = funcResetRefreshPageInterval;
        function funcResetRefreshPageInterval() {
            refreshPageInterval = 120;
        }
        function countDownPageRefresh() {
            refreshPageInterval = refreshPageInterval - 1;
            //console.log(refreshPageInterval);

            if (refreshPageInterval <= 0) {
                funcResetRefreshPageInterval();

                location.reload();

            }
        }

    </script>

    <style>
        .text-gate {
            color: #e12d2c;
        }

        .text-vtt {
            color: #d2d6de;
        }

        .text-vms {
            color: #d1332b;
        }

        .text-vnp {
            color: #57adee;
        }

        .text-momo {
            color: #ad2b73;
        }

        .text-acb {
            color: #e0f542;
        }

        .text-vcb {
            color: #6a885d;
        }

        .text-bidv {
            color: #7bfc73;
        }

        .text-mb {
            color: #1c1ed2;
        }

        .text-tpb {
            color: #5e2e86;
        }

        .text-icb {
            color: #d52a29;
        }

        #data1 tr:last-child {
            font-weight: bold;
        }

        .nav > li > a {
            padding: 2px 5px;
        }

        @media screen and (max-width: 767px) {
            .table-responsive {
                width: 100%;
                margin-bottom: 15px;
                overflow-y: hidden;
                -ms-overflow-style: -ms-autohiding-scrollbar;
                border: 1px solid #ddd;
            }
        }
    </style>
</asp:Content>


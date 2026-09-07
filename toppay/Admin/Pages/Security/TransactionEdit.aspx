<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="TransactionEdit.aspx.cs" Inherits="Pages_Security_TransactionEdit" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertBans"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertInfoss"></asp:Label>
            </div>
        </div>
    </div>

    <div class="modal  fade bs-example-modal-sm" id="MAlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1><%= Resources.Pay.BalanceFluctuation%>
            <small><%= Resources.Pay.WithdrawalHistory%></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active"><%= Resources.Pay.WithdrawalHistory%></li>
        </ol>
    </section>
    <section class="content">
        <div class="row">
            <div class="col-xs-12">


                <% if (AppUtils.IsAdmin)
                    { %>
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Tạo lệnh rút tiền</h3>
                    </div>

                    <div style="clear: both"></div>



                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtName">Tài khoản</label>


                                    <asp:DropDownList ID="ddlAccount" runat="server" CssClass="form-control">
                                    </asp:DropDownList>




                                </div>
                                <div class="form-group">
                                    <label for="txtName">Loại giao dịch</label>


                                    <asp:DropDownList CssClass="form-control" ID="drlType2" runat="server">

                                        <asp:ListItem Text="VND" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Usdt mua" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Usdt sẵn" Value="3"></asp:ListItem>
                                    </asp:DropDownList>


                                </div>
                                <div class="form-group">

                                    <label for="txtName">Số tiền (VND)</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtAmount2" runat="server" CssClass="form-control" placeholder="" ReadOnly="true"></asp:TextBox>
                                        </div>
                                        <label id="textMoneyVND2" class="control-label col-md-3" style="margin-top: 8px">
                                            &nbsp;
                                        </label>

                                    </div>

                                </div>

                            </div>
                            <div class="col-md-6">
                                <div class="form-group">

                                    <label for="txtName">Số Usdt</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtUsdt" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                        </div>


                                    </div>

                                </div>
                                <div class="form-group">

                                    <label for="txtName">Tỉ giá đầu vào</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtRateIn" runat="server" CssClass="form-control" placeholder="Nhập tỉ giá ví dụ 26500"></asp:TextBox>
                                        </div>


                                    </div>

                                </div>
                                <div class="form-group">

                                    <label for="txtName">Tỉ giá đẩu ra</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtRate" runat="server" CssClass="form-control" placeholder="Nhập tỉ giá ví dụ 26600"></asp:TextBox>
                                        </div>


                                    </div>

                                </div>
                                <div class="form-group">

                                    <label for="txtName">Lợi nhuận</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtFee" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                        </div>

                                        <label id="textMoneyVND3" class="control-label col-md-3" style="margin-top: 8px">
                                            &nbsp;
                                        </label>
                                        <div class="col-md-3" style="font-weight:bold">
                                            <a href="javascript:;" onclick="tinhloinhuan()">Tính lợi nhuận </a>
                                        </div>

                                    </div>
                                </div>

                                <div class="form-group" style="display: none">

                                    <label for="txtName">Ghi chú </label>


                                    <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                </div>
                            </div>

                            <%-- <div style="padding-left: 15px">
                                Lưu ý: sn1: lợi nhuận=30* số usdt, tiền gửi= 0* usdt. Các đối tác khác lợi nhuận=100* usdt, tiền gửi =0
                            </div>--%>

                            <div style="padding-right: 15px">
                                <asp:Button ID="Button1" runat="server" CssClass="btn btn-primary pull-right" OnClick="btAdd_Click2" Text="Duyệt lệnh rút tiền"></asp:Button>

                            </div>



                            <!-- /.col -->
                        </div>
                        <!-- /.row -->
                    </div>
                    <div class="box-footer">
                    </div>
                </div>
                <% } %>
            </div>
        </div>
    </section>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>

    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />
    <script type="text/javascript">

        $(document).ready(function () {
            $(".lstlightbox a").fancybox({

            });
        });


        $(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true, "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>
    <style>
        .label {
            width: 76px !important;
            padding: 3px;
            display: block
        }

        .txtCreatTime {
            width: 138px !important;
            padding: 3px !important;
        }
    </style>
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
    <script>


        $("#<%=txtAmount2.ClientID %>").change(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND2").html(formatMoneyVNDVal);
            }
        });


        $("#<%=txtFee.ClientID %>").change(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND3").html(formatMoneyVNDVal);
            }
        });

        function tinhloinhuan() {
            var ratein = parseInt($("#<%=txtRateIn.ClientID %>").val());
            var rateout = parseInt($("#<%=txtRate.ClientID %>").val());
            var usdt = parseInt($("#<%=txtUsdt.ClientID %>").val());
            var fee = usdt * (rateout - ratein);
            var price = parseInt(fee);
            $("#<%=txtFee.ClientID %>").val(price);
            var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
            $("#textMoneyVND3").html(formatMoneyVNDVal);
        }

<%--        $("#<%=txtUsdt.ClientID %>").change(function () {

            var user = $("#<%=ddlAccount.ClientID %>").val();
            if (user == "sn1" || user == "sn3") {
                var val = $(this).val();
                var fee = val * 30;
                var reward = val * 0;
                $("#<%=txtFee.ClientID %>").val(fee);
                $("#<%=txtReward.ClientID %>").val(reward);

                var price = parseInt(fee);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND3").html(formatMoneyVNDVal);

                var price2 = parseInt(reward);
                var formatMoneyVNDVal2 = price2.formatMoney(0, '', '.') + "";
                $("#textMoneyVND4").html(formatMoneyVNDVal2);

            }
            else {
                var val = $(this).val();
                var fee = val * 100;
                $("#<%=txtFee.ClientID %>").val(fee);


                var price = parseInt(fee);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND3").html(formatMoneyVNDVal);
            }
        });--%>
    </script>
</asp:Content>

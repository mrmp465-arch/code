<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankGateAPI.FixStatus.aspx.cs" Inherits="Pages_Monitor_BankGateAPI_FixStatus" %>


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
            <div class="alert alert-danger alert-dismissible">
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
        <h1>Nạp bank
        <small>Sửa thông tin giao dịch</small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Sửa lỗi</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">

        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Sửa thông tin giao dịch</h3>
            </div>
            <div class="box-body">
                <div class="row">

                    <div class="col-md-6">
                        <div class="form-group" style="display: none">
                            <label for="txtName">TransactionID *</label>
                            <div class="input-group input-group-sm">
                                <asp:TextBox ID="txtTransactionID" Text="0" runat="server" CssClass="form-control"></asp:TextBox>
                                <span class="input-group-btn">
                                    <asp:Button ID="btView" runat="server" CssClass="btn btn-info btn-flat" OnClick="btView_Click" Text="Xem"></asp:Button>
                                </span>
                            </div>


                        </div>

                        <div class="form-group" style="display: none">
                            <label for="txtBankAccountName">BankAccountName</label>
                            <asp:TextBox ID="txtBankAccountName" runat="server" CssClass="form-control" placeholder="BankAccountName"></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtBankAccountNumber">BankAccountNumber</label>
                            <asp:TextBox ID="txtBankAccountNumber" runat="server" CssClass="form-control" placeholder="BankAccountNumber"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="txtAmount">OrderNO</label>
                            <asp:TextBox ID="txtOrderNO" runat="server" CssClass="form-control" placeholder="OrderNo" ReadOnly></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtAmount">Amount</label>

                            <div class="row">
                                <div class="col-md-6">
                                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Amount"></asp:TextBox>
                                </div>
                                <label id="textMoneyVND" class="control-label col-md-3" style="margin-top: 8px">
                                    &nbsp;
                                </label>

                            </div>

                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtStatus">Status</label>
                            <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" placeholder="Status"></asp:TextBox>
                        </div>


                        <div class="form-group">
                            <label for="txtRefCode">RefCode</label>
                            <asp:TextBox ID="txtRefCode" runat="server" CssClass="form-control" placeholder="RefCode" ReadOnly></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="txtRefCode">OrderInfo</label>
                            <asp:TextBox ID="txtOrderInfo" runat="server" CssClass="form-control" placeholder="OrderInfo"></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtRefCode">Mobile</label>
                            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Mobile"></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtRefCode">Đối tác</label>
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group" >
                            <label for="txtLogContent">Tài khoản chuyển</label>
                            <asp:TextBox ID="txtAccountInfo" runat="server" CssClass="form-control" placeholder="Ghi chú"></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtLogContent">Ghi chú</label>
                            <asp:TextBox ID="txtLogContent" runat="server" CssClass="form-control" placeholder="Ghi chú"></asp:TextBox>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="txtLogContent">Ghi chú</label>
                            <asp:TextBox ID="txtLastTime" runat="server" CssClass="form-control" placeholder="Ghi chú"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-6">
                    </div>
                    <!-- /.col -->
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-info" OnClick="btUpdate_Click"></asp:Button>
            </div>
        </div>
    </section>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
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
        $("#<%=txtAmount.ClientID %>").change(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND").html(formatMoneyVNDVal);
            }
        });

        $(document).ready(function () {
            var val = $("#<%=txtAmount.ClientID %>").val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND").html(formatMoneyVNDVal);
            }

        });
    </script>
</asp:Content>

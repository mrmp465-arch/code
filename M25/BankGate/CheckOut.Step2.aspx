<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/HomePage.master" AutoEventWireup="true" CodeFile="CheckOut.Step2.aspx.cs" Inherits="CheckOut_Step2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="wrap">
        <div class="col12 bottom0" style="margin-left: 0;">
            <div class="step2_title"></div>
        </div>
        <div class="row-end"></div>

        <div class="col12" style="margin-left: 0;">
            <div class="nd_muathe">
                <asp:Label ID="lblErrorMessage" CssClass="error_img" runat="server" Text="Label" Visible="false"></asp:Label>
                <span class="title_ctt">Vui lòng lựa chọn phương thức thanh toán</span>
                <div class="col12">
                    <asp:Panel ID="panelVisa" runat="server" Visible="true">
                        <div class="chontt"><strong>Thẻ tín dụng quốc tế</strong></div>
                        <ul class="pay_logo">
                            <li style="width: 141px" class="hlk_SelectBank">
                                <img src="/bankgate/resources/images/logo-pay/visa.jpg" /><input type="radio" class="ratio_deposite" name="rdoBank" value="visa" /><%=GetTotalAmount("visa") %></li>
                            <li style="width: 141px" class="hlk_SelectBank">
                                <img src="/bankgate/resources/images/logo-pay/master.jpg" /><input type="radio" class="ratio_deposite" name="rdoBank" value="master" /><%=GetTotalAmount("visa") %></li>
                            <li style="width: 141px" class="hlk_SelectBank">
                                <img src="/bankgate/resources/images/logo-pay/jcb-bank.png" /><input type="radio" class="ratio_deposite" name="rdoBank" value="jcb" /><%=GetTotalAmount("visa") %></li>
                        </ul>
                        <div class="row-end"></div>
                    </asp:Panel>
                    <div class="chontt">
                        <strong>Ngân hàng</strong>
                    </div>
                    <ul class="pay_logo">
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/vcb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="VCB" /><%=GetTotalAmount("smartlink") %>
                            <%--vietcombank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/tcb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="TCB" /><%=GetTotalAmount("smartlink") %>
                            <%--techcombank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/vibb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="VIB" /><%=GetTotalAmount("smartlink") %>
                            <%--vib Quốc tế--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/abbb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="ABB" /><%=GetTotalAmount("smartlink") %>
                            <%--abbank  An Bình--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/scbb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="STB" /><%=GetTotalAmount("smartlink") %>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/mtb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="MSB" /><%=GetTotalAmount("smartlink") %>
                            <%--maritimebank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/nvb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="NVB" /><%=GetTotalAmount("smartlink") %>
                            <%--Navibank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/vbb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="CTG" /><%=GetTotalAmount("smartlink") %>
                            <%--Vietinbank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/dab.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="DAB" /><%=GetTotalAmount("smartlink") %>
                            <%--DongABank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/hdb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="HDB" /><%=GetTotalAmount("smartlink") %>
                            <%--HDBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/vab.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="VAB" /><%=GetTotalAmount("smartlink") %>
                            <%--vietabank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/vpb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="VPB" /><%=GetTotalAmount("smartlink") %>
                            <%--vpbank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/acbb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="ACB" /><%=GetTotalAmount("smartlink") %>
                            <%--ACB--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/mbb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="MB" /><%=GetTotalAmount("smartlink") %>
                            <%--MBBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/gpb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="GPB" /><%=GetTotalAmount("smartlink") %>
                            <%--GPBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/exb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="EIB" /><%=GetTotalAmount("smartlink") %>
                            <%--Eximbank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/ocb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="OJB" /><%=GetTotalAmount("smartlink") %>
                            <%--OceanBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/bab.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="NASB" /><%=GetTotalAmount("smartlink") %>
                            <%--BacABank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/pdb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="OCB" /><%=GetTotalAmount("smartlink") %>
                            <%--OricomBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/tpb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="TPB" /><%=GetTotalAmount("smartlink") %>
                            <%--TPBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/lpb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="LPB" /><%=GetTotalAmount("smartlink") %>
                            <%--LienVietPostBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/lpb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="SEAB" /><%=GetTotalAmount("smartlink") %>
                            <%--Seabank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/bidv.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="BIDV" /><%=GetTotalAmount("smartlink") %>
                            <%--BIDV--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/ab.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="VARB" /><%=GetTotalAmount("smartlink") %>
                            <%--AgriBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/bvb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="BVB" /><%=GetTotalAmount("smartlink") %>
                            <%--BaoVietBank--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/shb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="SHB" /><%=GetTotalAmount("smartlink") %>
                            <%--SHB--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/klb.png" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="KLB" /><%=GetTotalAmount("smartlink") %>
                            <%--SHB--%>
                        </li>
                        <li class="hlk_SelectBank">
                            <img width="90" height="45" src="/bankgate/resources/images/logo-pay/gb.jpg" />
                            <input type="radio" class="ratio_deposite" name="rdoBank" value="SCB" /><%=GetTotalAmount("smartlink") %>
                            <%--SaigonBank--%>
                        </li>
                    </ul>
                    <div class="row-end"></div>
                    <div class="chontt"><strong>Ví điện tử</strong></div>
                    <ul class="pay_logo">
                        <li style="width: 141px" class="hlk_SelectBank">
                            <img src="/bankgate/resources/images/logo-pay/logo-vtcpay.gif" /><input type="radio" class="ratio_deposite" name="rdoBank" value="vtcpay" /><%=GetTotalAmount("vtcpay") %></li>
                        <asp:Panel ID="panelBaoKim" Visible="false" runat="server">
                            <li style="width: 141px" class="hlk_SelectBank">
                                <img src="/bankgate/resources/images/logo-pay/logo-baokim.gif" /><input type="radio" class="ratio_deposite" name="rdoBank" value="baokim" /><%=GetTotalAmount("baokim") %></li>
                        </asp:Panel>
                        <li style="width: 141px" class="hlk_SelectBank">
                            <img src="/bankgate/resources/images/logo-pay/logo-nganluong.gif" /><input type="radio" class="ratio_deposite" name="rdoBank" value="nganluong" /><%=GetTotalAmount("nganluong") %></li>
                        <li style="width: 141px" class="hlk_SelectBank">
                            <img src="/bankgate/resources/images/logo-pay/logo-sohapay.gif" /><input type="radio" class="ratio_deposite" name="rdoBank" value="sohapay" /><%=GetTotalAmount("sohapay") %></li>
                    </ul>
                    <div class="row-end"></div>
                </div>
                <div class="row-end"></div>
                <span class="title_ctt">Thông tin đơn hàng</span>
                <div class="col3 bottom0" style="margin-left: 0;">
                    <span class="thongtin_txt">Giá trị giao dịch</span>
                    <span class="thongtin_txt">Thông tin giao dịch</span>
                </div>
                <div class="col3">
                    <span class="madonhang">
                        <asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></span>
                    <span class="madonhang">
                        <asp:Label ID="lblOrderInfo" runat="server" Text=""></asp:Label></span>
                </div>
                <div class="row-end"></div>
                <br />
                <div class="col3" style="float: left; padding: 10px 0 0 0;">
                    <a href="#" onclick="history.go(-1);return false;" style="padding: 0 20px 0 10px;"><i class="fa fa-long-arrow-left"></i>Quay lại</a>
                    <%--<a href="#"><i class="fa fa-question"></i>Hướng dẫn</a>--%>
                </div>
                <div class="col6">
                </div>
                <div class="col2" style="float: right;">
                    <asp:LinkButton ID="lbNext" CssClass="btn_tt" Style="float: left;" runat="server" OnClick="lbNext_Click">Thanh toán</asp:LinkButton>
                </div>
                <div class="row-end"></div>
            </div>
            <!--END nd_hotro-->
        </div>
        <!--END col12-->
        <div class="row-end"></div>
    </div>
    <!--END wrap-->
    <div class="row-end"></div>

    <script type="text/javascript">
        $(".hlk_SelectBank").click(function () {
            selectCard(this);
        });

        function selectCard(obj) {

            var o = $(obj).find("input");
            $(".ratio_deposite").attr("checked", false);

            o.attr("checked", true);
        }

        function goBack() {
            window.history.back();
        }
    </script>
</asp:Content>


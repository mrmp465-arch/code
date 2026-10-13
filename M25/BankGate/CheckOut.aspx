<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/HomePage.master" AutoEventWireup="true" CodeFile="CheckOut.aspx.cs" Inherits="CheckOut" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="wrap">
        <div class="col12 bottom0" style="margin-left: 0;">
            <div class="step1_title"></div>
        </div>
        <div class="row-end"></div>

        <div class="col12" style="margin-left: 0;">
            <div class="nd_muathe">
                <asp:Panel ID="Panel1" DefaultButton="lbNext" runat="server">
                <asp:Label ID="lblErrorMessage" CssClass="error_img" runat="server" Text="" Visible="false"></asp:Label>
                <span class="title_ctt">Thông tin người mua</span>
                <div class="col3 bottom0" style="margin-left: 0;">
                    <span class="loai_the">Họ tên (*)</span>
                    <span class="loai_the">Điện thoại (*)</span>
                </div>
                <div class="col3">
                    <asp:TextBox ID="txtFullName" CssClass="sl_the" runat="server"></asp:TextBox>
                    <asp:TextBox ID="txtMobile" CssClass="sl_the" runat="server"></asp:TextBox>
                </div>
                <div class="row-end"></div>
                <%--<div class="col6" style="margin-left: 0;">
                    <p style="text-align: left; padding: 0 0 0 40px;">Bạn cần nhập họ tên, điện thoại để có thể tra cứu chính xác!</p>
                </div>
                <div class="row-end"></div>--%>
                <div class="col3 bottom0" style="margin-left: 0;">
                    <asp:LinkButton ID="lbNext" CssClass="btn_tt" runat="server" OnClick="lbNext_Click">Tiếp</asp:LinkButton>
                </div>
                <div class="col3">
                    <asp:LinkButton ID="lbCancel" runat="server" OnClick="lbCancel_Click">Hủy</asp:LinkButton>
                </div>
                <div class="row-end"></div>
                <span class="title_ctt">Thông tin đơn hàng</span>
                <div class="col3 bottom0" style="margin-left: 0;">
                    <span class="thongtin_txt">Mã giao dịch</span>
                    <span class="thongtin_txt">Thông tin giao dịch</span>
                    <span class="thongtin_txt">Số tiền giao dịch</span>
                </div>
                <div class="col6">
                    <span class="madonhang"><asp:Label ID="lblOrderNo" runat="server" Text=""></asp:Label></span>
                    <span class="madonhang"><asp:Label ID="lblOrderInfo" runat="server" Text=""></asp:Label></span>
                    <span class="madonhang"><asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></span>
                </div>
                <div class="row-end"></div>
                <span class="title_ctt">Thông tin nhà cung cấp dịch vụ</span>
                <div class="col8" style="margin-left: 0;">
                    <p style="text-align: left; padding: 0 0 0 40px;">Công ty Cổ phần Giải trí VGG. Địa chỉ: 23 Láng Hạ, Thành Công, Ba Đình, Hà Nội</p>
                </div>
                <div class="row-end"></div>
                </asp:Panel>
            </div>
            <!--END nd_hotro-->
        </div>
        <!--END col12-->
        <div class="row-end"></div>
    </div>
    <!--END wrap-->
    <div class="row-end"></div>
</asp:Content>


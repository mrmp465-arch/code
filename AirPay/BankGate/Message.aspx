<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/HomePage.master" AutoEventWireup="true" CodeFile="Message.aspx.cs" Inherits="Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="wrap">
        <div class="col12 bottom0" style="margin-left: 0;">
            <div class="step1_title"></div>
        </div>
        <div class="row-end"></div>

        <div class="col12" style="margin-left: 0;">
            <div class="nd_muathe">
                <span class="title_ctt">Thông báo</span>
                <div class="col6" style="margin-left: 0;">
                    <p style="text-align: left; padding: 0 0 0 40px;"><asp:Label ID="lblMessage" runat="server" Text=""></asp:Label></p>
                </div>
                <div class="row-end"></div>
            </div>
            <!--END nd_hotro-->
        </div>
        <!--END col12-->
        <div class="row-end"></div>

    </div>
    <!--END wrap-->
</asp:Content>


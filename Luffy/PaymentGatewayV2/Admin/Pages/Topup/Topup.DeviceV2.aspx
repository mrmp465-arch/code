<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.DeviceV2.aspx.cs" Inherits="Pages_Topup_Topup_DeviceV2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <section class="content-header">
        <h1>USSD
            <small>Quản lý thiết bị</small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Quản lý thiết bị</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Quản lý thiết bị</h3>
                    </div>
                    <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                        { %>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="txtUsers" AutoPostBack="true" OnSelectedIndexChanged="txtUsers_SelectedIndexChanged" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <% } %>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        
                        <div>
                            <iframe width="100%" height="800" style="border: none" src="<%=Url %>"></iframe>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>


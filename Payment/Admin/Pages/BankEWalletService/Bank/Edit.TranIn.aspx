<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Edit.TranIn.aspx.cs" Inherits="Pages_BankEWalletService_Bank_Edit_TranIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Giao dịch bank
            <small>
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.CardAPIMonitor %>">Xem log giao dịch</a>
            </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Xem chi tiết</li>
        </ol>
    </section>
    <section class="content">
        <div class="box box-solid">
            <div class="box-header with-border">
                <i class="fa fa-text-width"></i>
                <h3 class="box-title">
                    <asp:Label ID="lblTtitle" runat="server" Text="Chỉnh sửa code" CssClass="title"></asp:Label></h3>
            </div>
            <!-- /.box-header -->
            <div class="pad margin no-print" runat="server" id="resultDiv">
                <div class="callout callout-success" id="callout" runat="server">


                    <div class="box-body">
                        <pre runat="server" id="contentSpan"></pre>
                    </div>

                </div>
            </div>
            <div class="box-body">
                <dl class="dl-horizontal">
                    <dt>Id</dt>
                    <dd>
                        <asp:Label ID="lblId" runat="server" Text=""></asp:Label></dd>
                    <dt>RefCode</dt>
                    <dd>
                        <asp:Label ID="lblRefCode" runat="server" Text=""></asp:Label></dd>
                    <dt>Partner Code</dt>
                    <dd>
                        <asp:Label ID="lblPartnerCode" runat="server" Text=""></asp:Label></dd>
                    <dt>Command Code</dt>
                    <dd>
                        <asp:Label ID="lblCommandCode" runat="server" Text=""></asp:Label></dd>
                    <dt>Bank TransId</dt>
                    <dd>
                        <asp:Label ID="lblBankTransId" runat="server"></asp:Label></dd>
                    <dt>Bank Code</dt>
                    <dd>
                        <asp:Label ID="lblBankCode" runat="server"></asp:Label></dd>
                    <dt>Tk Dịch Vụ</dt>
                    <dd>
                        <asp:Label ID="lblPartnerBankId" runat="server"></asp:Label></dd>
                    <dt>TK Khách</dt>
                    <dd>
                        <asp:Label ID="lblUserBankId" runat="server"></asp:Label></dd>
                    <dt>Amount</dt>
                    <dd>
                        <asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></dd>
                    <dt>Nội dung</dt>
                    <dd>
                        <asp:Label ID="lblContent" runat="server"></asp:Label></dd>
                    <dt>Code</dt>
                    <dd>
                        <asp:TextBox ID="txtNote" runat="server" class="form-control" placeholder="Nhập code..." Text=""></asp:TextBox></dd>
                    <dt>Create Time</dt>
                    <dd>
                        <asp:Label ID="lblCreatTime" runat="server" Text=""></asp:Label></dd>
                    <dt>Last Time</dt>
                    <dd>
                        <asp:Label ID="lblBankTime" runat="server" Text=""></asp:Label></dd>
                    <dt></dt>
                    <dd></dd>
                    <dt>Status</dt>
                    <dd>
                        <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></dd>
                    <dt>CallbackUrl</dt>
                    <dd>
                        <asp:Label ID="lblCallbackUrl" runat="server" Text=""></asp:Label></dd>
                </dl>

                <div class="col-xs-12">
                    <button type="button" class="btn btn-primary pull-right" style="margin-right: 5px;" runat="server" id="btnEditTran" onserverclick="btnEditTran_Click">
                        <i class="fa fa-check-square-o"></i>Cập nhật (và Callback)
                    </button>
                    <button type="button" class="btn btn-default pull-left" style="margin-right: 5px;" runat="server" id="btnBack" onserverclick="btnBack_Click">
                        <i class="fa fa-backward"></i>Quay lại
                    </button>
                </div>
            </div>

            <!-- /.box-body -->
        </div>

        <%--<section id="brand">
            <h4 class="page-header">Brand Icons</h4>
            <div class="alert alert-info">
                <ul class="margin-bottom-none padding-left-lg">
                    <li>All brand icons are trademarks of their respective owners.</li>
                    <li>The use of these trademarks does not indicate endorsement of the trademark holder by Font
                        Awesome, nor vice versa.
                    </li>
                </ul>
            </div>
        </section>--%>
    </section>

</asp:Content>


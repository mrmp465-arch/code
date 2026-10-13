<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="SMS.Monitor.Detail.aspx.cs" Inherits="Pages_Monitor_SMS_Monitor_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>SMS & SMSPlus
            <small>
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.SMSMonitor %>">Xem log giao dịch nạp thẻ</a>
            </small>
        </h1>
    </section>
    <section class="content">
        <div class="box box-solid">
            <div class="box-header with-border">
                <i class="fa fa-text-width"></i>
                <h3 class="box-title">
                    <asp:Label ID="lblTtitle" runat="server" Text="Chi tiết thông tin giao dịch" CssClass="title"></asp:Label></h3>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <dl class="dl-horizontal">

                      <dt>Id</dt><dd><asp:Label ID="lblId" runat="server" Text=""></asp:Label></dd> 
                        <% if (AppUtils.IsAdmin)
                        { %>
                            <dt>Provider</dt><dd><asp:Label ID="lblProvider" runat="server" Text=""></asp:Label></dd> 
                        <% } %>
                        <dt>Type</dt><dd><asp:Label ID="lblType" runat="server" Text=""></asp:Label></dd> 
                        <dt>SenderId</dt><dd><asp:Label ID="lblSenderId" runat="server" Text=""></asp:Label></dd> 
                        <dt>SenderNumber</dt><dd><asp:Label ID="lblSenderNumber" runat="server" Text=""></asp:Label></dd> 
                        <dt>ReceiverNumber</dt><dd><asp:Label ID="lblReceiverNumber" runat="server" Text=""></asp:Label></dd> 
                        <dt>ReceiverId</dt><dd><asp:Label ID="lblReceiverId" runat="server" Text=""></asp:Label></dd> 
                        <dt>Subject</dt><dd><asp:Label ID="lblSubject" runat="server" Text=""></asp:Label></dd> 
                        <dt>PartnerCode</dt><dd><asp:Label ID="lblPartnerCode" runat="server" Text=""></asp:Label></dd> 
                        <dt>PartnerCommand</dt><dd><asp:Label ID="lblPartnerCommand" runat="server" Text=""></asp:Label></dd> 
                        <dt>Content</dt><dd><asp:Label ID="lblContent" runat="server" Text=""></asp:Label></dd> 
                        <dt>SentTime</dt><dd><asp:Label ID="lblSentTime" runat="server" Text=""></asp:Label></dd> 
                        <dt>ReceivedTime</dt><dd><asp:Label ID="lblReceivedTime" runat="server" Text=""></asp:Label></dd> 
                    <% if (AppUtils.IsAdmin)
                       { %>    
                        <dt>RefTranId</dt><dd><asp:Label ID="lblRefTranId" runat="server" Text=""></asp:Label></dd> 
                     <% } %>
                        <dt>Amount</dt><dd><asp:Label ID="lblAmount" runat="server" Text=""></asp:Label></dd> 
                        <dt>Status</dt><dd><asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></dd> 
                        <dt>Description</dt><dd><asp:Label ID="lblDescription" runat="server" Text=""></asp:Label></dd> 
                        <dt>CreatedTime</dt><dd><asp:Label ID="lblCreatedTime" runat="server" Text=""></asp:Label></dd> 
                        <dt>ModifiedTime</dt><dd><asp:Label ID="lblModifiedTime" runat="server" Text=""></asp:Label></dd>  
                    
                    
                    
                 
                    
                </dl>
            </div>
            <!-- /.box-body -->
        </div>
    </section>

</asp:Content>


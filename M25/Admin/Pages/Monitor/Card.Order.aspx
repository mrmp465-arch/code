<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Card.Order.aspx.cs" Inherits="Pages_Monitor_Card_Order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Mua thẻ
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Danh sách order" CssClass="title"></asp:Label></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Order</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <div class="box-header with-border">
                <div class="col-xs-3 col-sm-6 col-md-2">
                    <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control "  AutoPostBack="false">
                    </asp:DropDownList>
                </div>
                <div class="col-xs-3 col-sm-6 col-md-2">
                    <asp:TextBox ID="txtOrderNo" placeholder="OrderNo" runat="server" CssClass="form-control " ></asp:TextBox>
                </div>
                <div class="col-xs-3 col-sm-6 col-md-2">
                    <asp:Button ID="btnCreate" CssClass="btn btn-info" OnClick="btnCreate_Click" runat="server" Text="Tạo mới" />
                </div>
            </div>
            <div class="box-body no-padding">
                <div style="height: 20px; clear: both;"></div>
                <table class="table table-striped" id="TableResponsive1">
                    <thead>
                        <tr>
                            <th style="width: 10px;">STT </th>
                            <th>Mã Order </th>
                            <th>Đối tác</th>
                            <th>Thời gian tạo</th>
                            <th>Hoàn thành</th>

                            <th>Tác vụ</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptList1" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Container.ItemIndex + 1 %></td>
                                    <td><%#Eval("OrderNo") %></td>
                                    <td><%#Eval("PartnerCode") %></td>
                                    <td><%#Eval("Time") %></td>
                                    <td><%#GetStatus(Eval("Status")) %></td>

                                    <td>
                                        <a href="<%#DetailUrl(Eval("Id").ToString()) %>">Cấu hình |</a>  &nbsp;
                                                <asp:LinkButton ID="lnDelete" runat="server" OnCommand="Delete_Command" CommandName="Delete" CommandArgument='<%#Eval("Id")%>' Visible='<%# Eval("Status").ToString() == "0" %>'>Xóa |</asp:LinkButton> 
                                        &nbsp;
                                               <asp:LinkButton ID="lnConfirm" runat="server" OnCommand="Confirm_Command" CommandName="Confirm" CommandArgument='<%#Eval("Id")%>' Visible='<%# Eval("Status").ToString() == "0" %>'>Hoàn thành |</asp:LinkButton>
                                         <asp:LinkButton ID="lnDownload" runat="server" OnCommand="Download_Command" CommandName="Download" CommandArgument='<%#Eval("Id")%>' >Download</asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
    </section>
    <!-- /.content -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>

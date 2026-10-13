<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Card.OrderDetail.aspx.cs" Inherits="Pages_Monitor_Card_OrderDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Mua thẻ
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Chi tiết order" CssClass="title"></asp:Label>
            <%=OrderNo %></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Order </li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <div class="box-header with-border">
                <div class="col-xs-3 col-sm-6 col-md-2">
                    <asp:DropDownList ID="drpCardType" class="form-control " runat="server">
                        <asp:ListItem Value="">Loại thẻ:</asp:ListItem>
                        <asp:ListItem Value="VTT">Viettel</asp:ListItem>
                        <asp:ListItem Value="VMS">Mobifone</asp:ListItem>
                        <asp:ListItem Value="VNP">Vinaphone</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-xs-3 col-sm-6 col-md-2">
                    <asp:DropDownList ID="drpCardValue" class="form-control " runat="server">
                        <asp:ListItem Value="">Mệnh giá:</asp:ListItem>
                        <asp:ListItem Value="50000">50k</asp:ListItem>
                        <asp:ListItem Value="100000">100k</asp:ListItem>
                        <asp:ListItem Value="200000">200k</asp:ListItem>
                        <asp:ListItem Value="500000">500k</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-xs-3 col-sm-6 col-md-2">
                    <asp:TextBox ID="txtNumberCard" TextMode="Number" PlaceHoder="Số lượng" runat="server" CssClass="form-control "></asp:TextBox>
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

                            <th>Loại thẻ</th>
                            <th>Mệnh giá</th>
                            <th>Số lượng</th>
                            <th>Trạng thái</th>
                            <th>Tác vụ</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptList1" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Container.ItemIndex + 1 %></td>
                                    <td>
                                        <asp:DropDownList  ID="drpCardTypeItem" class="form-control  drpCardTypeItem" runat="server" selectedValue='<%#Eval("CardType")%>' Enabled='<%#Eval("Status").ToString()=="0"%>'>
                                           
                                            <asp:ListItem Value="VTT">Viettel</asp:ListItem>
                                            <asp:ListItem Value="VMS">Mobifone</asp:ListItem>
                                            <asp:ListItem Value="VNP">Vinaphone</asp:ListItem>
                                        </asp:DropDownList></td>
                                    <td> <asp:DropDownList ID="drpCardValueItem" class="form-control  drpCardValueItem" runat="server" selectedValue='<%#Eval("CardValue")%>' Enabled='<%#Eval("Status").ToString()=="0"%>'>
                                        
                                        <asp:ListItem Value="50000">50k</asp:ListItem>
                                        <asp:ListItem Value="100000">100k</asp:ListItem>
                                        <asp:ListItem Value="200000">200k</asp:ListItem>
                                        <asp:ListItem Value="500000">500k</asp:ListItem>
                                    </asp:DropDownList>
                                         <asp:Label ID="lblCardId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                        
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumberCardItem" TextMode="Number" runat="server" CssClass="form-control txtNumberCardItem" Text='<%#Eval("NumberCard") %>' Enabled='<%#Eval("Status").ToString()=="0" && Status==0 %>' placeholder="Số lượng">  </asp:TextBox>
                                    </td>
                                    <td><%#GetStatus(Eval("Status")) %></td>
                                    <td>
                                          <asp:LinkButton ID="lnDelete" runat="server" OnCommand="Delete_Command" CommandName="Delete" CommandArgument='<%#Eval("Id")%>' Visible='<%# Eval("Status").ToString() == "0" && Status==0 %>'>Xóa |</asp:LinkButton> 
                                        &nbsp;
                                               <asp:LinkButton ID="lnConfirm" runat="server" OnCommand="Confirm_Command" CommandName="Confirm" CommandArgument='<%#Eval("Id")%>' Visible='<%# Eval("Status").ToString() == "0" %>'>Lấy thẻ </asp:LinkButton>
                                        &nbsp;
                                               <asp:LinkButton ID="lnDownload" runat="server" OnCommand="Download_Command" CommandName="Download" CommandArgument='<%#Eval("Id")%>' Visible='False' >Download </asp:LinkButton>

                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
             <div class="box-footer">
                        <asp:Button ID="Button1" runat="server" Text="Quay lại" CssClass="btn btn-default pull-right "   OnClick="btApply_Click2"></asp:Button>&nbsp;
                        <asp:Button ID="Button5" runat="server" Text="Cập nhật" CssClass="btn btn-info pull-right " Visible="<%# Status==0 %>" OnClick="btApply_Click1"></asp:Button>
                    </div>
        </div>
    </section>
    <!-- /.content -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <style>
        .txtNumberCardItem
        {
            max-width:80px;
        }
         .drpCardValueItem,.drpCardTypeItem
        {
            max-width:150px;
        }
         .btn-default
         {
             margin-left:15px;
         }
    </style>
</asp:Content>

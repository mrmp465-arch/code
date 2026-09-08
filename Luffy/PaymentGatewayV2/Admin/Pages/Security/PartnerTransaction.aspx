<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="PartnerTransaction.aspx.cs" Inherits="Pages_Security_PartnerTransaction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Biên động số dư
            <small>Lịch sử giao dịch </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Lịch sử giao dịch</li>
        </ol>
    </section>
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label ID="lblTotal" runat="server" CssClass="title">

                            </asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Số tiền rút:</span>
                                <asp:TextBox ID="txtAmount" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Ghi chú:</span>
                                <asp:TextBox ID="txtNote" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                          <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btCreate" runat="server" CssClass="btn btn-primary" OnClick="btAdd_Click" Text="Tạo lệnh rút tiền"></asp:Button>
                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>#Mã rút</th>


                                    <th>Mã đối tác</th>
                                    <th>Số tiền</th>
                                    <th>Thời gian tạo </th>
                                    <th>Thời gian hoàn thành </th>
                                    <th>Ghi chú</th>
                                    <th>Số dư sau giao dịch</th>
                                    <th>Trạng thái</th>

                                    <th>#</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Id")%></td>
                                            <td><%#Eval("PartnerCode")%></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Eval("CreatedTime") %>  </td>
                                            <td><%#Eval("UpdateTime") %>  </td>
                                            <td><%# Eval("Note") %></td>
                                            <td> <asp:Label  id="lbBalance" runat="server"  Visible='<%#Eval("Status").ToString()=="1"%>' Text='<%#Convert.ToInt64(Eval("Balance")).ToString("N0").Replace(",", ".") %>'> </asp:Label> </td>
                                            <td><%# GetStatus(Eval("Status")) %></td>
                                            <td>
                                                <% if (AppUtils.IsAdmin)
                                                    { %>
                                                <asp:LinkButton ID="lnkUpdate" CausesValidation="false" ToolTip="Giải quyết" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("Status").ToString()=="0"%>'
                                                    CommandName="update" OnClientClick="return confirm('Bạn chắc chắn muốn thực hiện?'); "
                                                    runat="server" OnCommand="Update_Command">Giải quyết</asp:LinkButton>

                                                <% } %>
                                                <% if (AppUtils.IsPartner)
                                                    { %>
                                                <asp:LinkButton ID="lnkDelete" CausesValidation="false" ToolTip="Giải quyết" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("Status").ToString()=="0"%>'
                                                    CommandName="delete" OnClientClick="return confirm('Bạn chắc chắn muốn thực hiện?'); "
                                                    runat="server" OnCommand="Delete_Command">Hủy</asp:LinkButton>

                                                <% } %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>

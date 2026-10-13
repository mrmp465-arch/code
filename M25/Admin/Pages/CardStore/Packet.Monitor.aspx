<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Packet.Monitor.aspx.cs" Inherits="Pages_CardStore_Packet_Monitor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Thông kê
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Thống kê kho thẻ" CssClass="title"></asp:Label></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Thông kê</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Thông kê</li>
                <li class="<% if (Request["Type"] == "1" || Request["Type"] == null) {%>active <% }%>"><a href="#gach-the" data-toggle="tab">Packet</a></li>
                <%--<li class="<% if (Request["Type"] == "2" ) {%>active <% }%>"><a href="#mua-the" data-toggle="tab">Mua mã thẻ & Topup</a></%--li>
                <li class="<% if (Request["Type"] == "3" ) {%>active <% }%>"><a href="#ngan-hang" data-toggle="tab">Ngân hàng</a></li>
                <li class="<% if (Request["Type"] == "4" ) {%>active <% }%>"><a href="#sms" data-toggle="tab">SMS</a></li>--%>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-body">
                        <table class="table table-striped" id="TableResponsive" style="clear: both;">
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" placeholder="Status"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtOrderNo" runat="server" CssClass="form-control" placeholder="OrderNo"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtProviderCode" runat="server" CssClass="form-control" placeholder="ProviderCode "></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtCardType" runat="server" CssClass="form-control" placeholder="CardType"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <asp:Button ID="btView" runat="server" CssClass="btn btn-info" Text="Xem" OnClick="btView_Click"></asp:Button>
                                </div>
                            </div>
                        </table>
                    </div>
                </div>

                <div class="chart tab-pane <% if (Request["Type"] == "1" || Request["Type"] == null) {%>active <% }%>" id="gach-the">
                    <div class="box-body no-padding">
                        <table class="table table-striped" id="TableResponsive1">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">Id</th>
                                    <th>Order No</th>
                                    <th>Name</th>
                                    <th>Provider</th>
                                    <th>Card Type</th>
                                    <th>Card Sole/Total Card</th>
                                    <th>Card Value</th>
                                    <th>Expire Date</th>
                                    <th>Card Up</th>
                                    <th>Is Active</th>
                                    <th>Status</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptPacketList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Id") %></td>
                                            <td><%#Eval("OrderNo") %></td>
                                            <td><%#Eval("Name") %></td>
                                            <td><%#Eval("ProviderCode")%></td>
                                            <td><%#Eval("CardType") %></td>
                                            <td><%#Eval("NumberCardSole")%>/<%#Eval("NumberCard") %></td>
                                            <td><%#Convert.ToInt32(Eval("CardValue")).ToString("#,#").Replace(",", ".") %></td>
                                            <td><%#Eval("ExpireDate", "{0:dd/MM/yyyy}")%></td>
                                            <td><%#Eval("NumberCardUp") %></td>
                                            <td><%#Eval("IsActive") %></td>
                                            <td><%#Eval("Status") %></td>
                                            <td>#</td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                    </div>
                    <div class="box-footer">
                         <div class="pull-right">
                            
                            <% if (pages!=null && pages.EndPage > 1){%>
                                    <span style="line-height: 27px;    padding: 0 9px 0 0;    font-weight: bold;"> Trang <%=pages.CurrentPage  %>/<%=pages.TotalPages%></span>                                    <ul class="pagination pagination-sm no-margin pull-right">
                                        <% if (pages.CurrentPage > 1){%>
                                               <li>
								                    <a href="<%=Constant.ADMIN_PATH + Resources.Url.PacketMonitor %>?page=1">First</a>
							                    </li>
							                    <li>
								                    <a href="<%=Constant.ADMIN_PATH + Resources.Url.PacketMonitor %>?page=<%=pages.CurrentPage - 1%>"><</a>
							                    </li>
                                        <% } %>
                                          <% for (var page = pages.StartPage; page <= pages.EndPage; page++){%>
                                              <li class="<%= Getactive(page, pages.CurrentPage)%>">   
								                <a href="<%=Constant.ADMIN_PATH + Resources.Url.PacketMonitor %>?page=<%= page%>"> <%= page%></a>
							                </li> 
                                        <% } %>
                                        <% if (pages.CurrentPage < pages.TotalPages){%>
                                               <li>
								                    <a href="<%=Constant.ADMIN_PATH + Resources.Url.PacketMonitor %>?page=<%=pages.CurrentPage + 1%>">></a>
							                    </li>
							                    <li>
								                    <a href="<%=Constant.ADMIN_PATH + Resources.Url.PacketMonitor %>?page=<%= (pages.TotalPages) %>  ">Previous</a>
							                    </li> 
                                        <% } %>
                                    </ul>
                            <% } %>  
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!-- /.content -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">

        $(document).ready(function () {
            var table = $('#TableResponsive1,#TableResponsive2,#TableResponsive3').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

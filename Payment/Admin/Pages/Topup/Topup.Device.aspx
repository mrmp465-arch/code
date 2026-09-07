<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.Device.aspx.cs" Inherits="Pages_Topup_Topup_Device" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertBans"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>

    <div class="modal  fade bs-example-modal-sm" id="AlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1>USSD
            <small>Quản lý Device và Sim Slot</small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Quản lý Device và Sim Slot</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Quản lý Device và Sim Slot</h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <% if (AppUtils.IsAdmin)
                            {%>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control drpProviders" ID="drpProviders" runat="server">
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                        <%}%>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">

                            <tr>
                                <th style="text-align: left">Id</th>
                                <th>Name</th>
                                <th>ClientId</th>
                                <th>ProviderCode</th>
                                <th>Status</th>
                                <th style="text-align: right; padding-right: 10px;">
                                    <% if (AppUtils.IsAdmin)
                                        {%>
                                    <span class="item1-span1">
                                        <img src="<%=Constant.ADMIN_PATH %>Content/add.jpg" title="Thêm" /></span>
                                    <%}%>
                                </th>
                            </tr>

                            <asp:Repeater ID="rptList" runat="server">
                                <ItemTemplate>
                                    <tr class="item-tr item-tr-<%#Eval("Id")%>">
                                        <td style="text-align: left" class="Id"><%#Eval("Id")%></td>
                                        <td class="Name"><span><a href="#"><%#Eval("Name") %></a></span> </td>
                                        <td class="ClientId"><span><%#  Eval("ClientId").ToString().Substring(0,50)  %>...</span></td>
                                        <td class="ProviderCode"><span><%#  Eval("ProviderCode")  %></span></td>
                                        <td class="Status">
                                            <span><%#  GetStatus((int)Eval("Status"))  %></span>
                                        </td>
                                        <td style="text-align: right;">
                                            <% if (AppUtils.IsAdmin)
                                                {%>
                                            <span class="item1-span-block1">
                                                <span class="item1-span4"><a href="#">Xóa</a></span>
                                            </span>
                                            <%}%>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td colspan="7" style="padding: 5px 0 0 25px;">
                                            <table class="table table-striped">
                                                <tr>
                                                    <th style="width: 20px;"></th>
                                                    <th style="text-align: left">Id</th>
                                                    <th>Slot</th>
                                                    <th>Telco</th>
                                                    <th>Quota</th>
                                                    <th>Amount</th>
                                                    <th>Status</th>
                                                    <th style="text-align: right; padding-right: 10px;">
                                                        <% if (AppUtils.IsAdmin)
                                                            {%>
                                                        <span class="item2-span1">
                                                            <img src="<%=Constant.ADMIN_PATH %>Content/add.jpg" title="Thêm" />
                                                        </span>
                                                        <%}%>
                                                    </th>
                                                </tr>
                                                <asp:Repeater ID="rptListSim" runat="server" DataSource='<%#ListSim(Eval("Id")) %>'>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td style="width: 20px;"><span class="open" style="display: block; width: 20px; padding: 0; text-align: center;"></span></td>
                                                            <td style="text-align: left"><%#Eval("Id") %></td>
                                                            <td class="Slot"><span><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.TopupSimEdit %>?Id=<%#Eval("Id") %>">Slot <%#Eval("Slot") %></a></span> </td>
                                                            <td class="Telco">
                                                                <span><%# Eval("Telco")  %></span>
                                                            </td>
                                                            <td class="Quota">
                                                                <span><%#Convert.ToInt32(Eval("Quota")).ToString("N0").Replace(",", ".") %></span> </td>
                                                            <td class="Amount">
                                                                <span><%#Convert.ToInt32(Eval("Amount")).ToString("N0").Replace(",", ".") %></span> </td>

                                                            <td class="Status">
                                                                <span><%#  GetStatusSim((int)Eval("Status"))  %></span>
                                                            </td>

                                                            <td style="text-align: right;">
                                                                <span class="item2-span-block1">
                                                                    <div>
                                                                        <span class="item1-span3">
                                                                            <asp:LinkButton Name="LinkResetAmount" runat="server" CommandArgument='<%# Eval("Id") %>' OnClick="LinkResetAmount_Click" OnClientClick="return confirm('Bạn có muốn reset Amount về 0?')" title="Reset Amout">Reset Amount</asp:LinkButton>
                                                                        </span>
                                                                        <% if (AppUtils.IsAdmin)
                                                                            {%>
                                                                        | <span class="item2-span4" data-id="<%#Eval("Id") %>"><a href="#">Xóa</a></span>
                                                                        <%}%>
                                                                    </div>

                                                                </span>

                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </td>
                                    </tr>


                                </ItemTemplate>
                            </asp:Repeater>
                        </table>

                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <style type="text/css">
        .item1-span1, .item2-span1, .item3-span1, .item3-span1 {
            cursor: pointer;
        }


        #TableResponsive span.open,
        #TableResponsive span.open2 {
            cursor: pointer;
            font-weight: bold;
            font-size: 18px;
            color: blue;
        }

        .item1-span-block1 span, .item1-span-block2 span, .item1-span-block3 span,
        .item2-span-block1 span, .item2-span-block2 span, .item2-span-block3 span,
        .item3-span-block1 span, .item3-span-block2 span, .item3-span-block3 span {
            cursor: pointer;
        }

            .item1-span-block1 span:hover, .item1-span-block2 span:hover, .item1-span-block3 span:hover,
            .item2-span-block1 span:hover, .item2-span-block2 span:hover, .item2-span-block3 span:hover,
            .item3-span-block1 span:hover, .item3-span-block2 span:hover, .item3-span-block3 span:hover {
                text-decoration: underline;
            }
    </style>
</asp:Content>


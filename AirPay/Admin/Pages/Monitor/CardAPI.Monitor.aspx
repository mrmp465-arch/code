<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardAPI.Monitor.aspx.cs" Inherits="Pages_Monitor_CardAPI_Monitor" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <span id="AlertInfos"></span>
            </div>
        </div>
    </div>
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Thẻ cào
              <small><%= Resources.Pay.ViewLog%> </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active"><%= Resources.Pay.ViewLog%></li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title"><%= Resources.Pay.ViewLog%></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList CssClass="form-control" ID="drpTop" runat="server">
                                <asp:ListItem Text="50 row" Value="50" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="100 row" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 row" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 row" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control" ID="drpStatus" runat="server">
                                <%--  <asp:ListItem Text="Trạng thái" Value="-99" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Thành công" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Đang xử lý" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Thất bại" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="Thẻ bị từ chối" Value="-7"></asp:ListItem>--%>
                            </asp:DropDownList>
                        </div>
                        <% if (AppUtils.IsAdmin || AppUtils.IsTopup || AppUtils.IsPartner)
                            { %>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpCardType" runat="server" CssClass="form-control ">
                                <%--<asp:ListItem Text="Loại thẻ:" Value=""></asp:ListItem>
                                <asp:ListItem Text="Viettel" Value="viettel"></asp:ListItem>
                                <asp:ListItem Text="MobiFone" Value="vms"></asp:ListItem>
                                <asp:ListItem Text="Vinaphone" Value="vnp"></asp:ListItem>--%>
                            </asp:DropDownList>
                        </div>
                        <% if (AppUtils.IsAdmin || AppUtils.IsProvider)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2" style="display: none">
                            <asp:DropDownList ID="drpProvider" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.Keyword%></span>
                                <asp:TextBox CssClass="form-control" ID="txtOrderNo" runat="server" placeholder="Refcode,Pin,Seri"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px"><%= Resources.Pay.From%></span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtFromDate" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px"><%= Resources.Pay.To%></span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtCreatTime" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>#Id</th>
                                    <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                                        { %>
                                    <th><%= Resources.Pay.Partner%></th>
                                    <% } %>


                                    <th>CardSerial</th>
                                    <th>CardCode</th>
                                    <th><%= Resources.Pay.AmountUser%></th>
                                    <th><%= Resources.Pay.RealAmount%></th>
                                    <th><%= Resources.Pay.Fee%></th>

                                   <th><%= Resources.Pay.CardType%></th>

                                    <th><%= Resources.Pay.CreatedTime%></th>
                                    <th><%= Resources.Pay.LastTime%></th>
                                    <th><%= Resources.Pay.Status%></th>
                                    <th>#</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("TransactionID")%></td>
                                            <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                                                { %>
                                            <td><%#Eval("PartnerCode")%></td>
                                            <% } %>



                                            <td><%#Eval("CardSerial") %></td>
                                            <td><%#Eval("CardCode") %></td>
                                            <td><%#Convert.ToInt64(Eval("AmountUser")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Convert.ToInt64(Eval("Fee")).ToString("N0").Replace(".", ",") %></td>

                                            <td><%#Eval("CardType") %></td>

                                            <td><%#Eval("CreatTime") %>  </td>
                                            <td><%#Eval("LastTime") %>  </td>

                                            <td><%#Eval("Status") %></td>
                                            <td><a href="<%#DetailUrl(Eval("TransactionID").ToString()) %>">[<%= Resources.Pay.View%>]</a> |
                                                <asp:LinkButton ID="lnCallback" runat="server" OnCommand="Callback_Command" CommandName="Callback" CommandArgument='<%#Eval("TransactionID")%>' Visible='<%# Eval("Status").ToString() == "1" || Eval("Status").ToString() == "2" %>'>[Callback] </asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <!-- /.box-body -->
                </div>
                <!-- /.box -->
            </div>
            <!-- /.col -->
        </div>
        <!-- /.row -->
    </section>
    <!-- /.content -->
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=txtFromDate.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });
            $('#<%=txtCreatTime.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });

        });
        $(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true, "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>
    <style>
        .label {
            width: 76px !important;
            padding: 3px;
            display: block
        }

        .btcallback {
            display: none;
        }

        .txtCreatTime {
            width: 138px !important;
            padding: 3px !important;
        }
    </style>
</asp:Content>

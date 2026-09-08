<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardAPI.Monitor.aspx.cs" Inherits="Pages_Monitor_CardAPI_Monitor" %>

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
            <small>
                <asp:Label ID="lblTtitle" runat="server" Text="Xem log giao dịch nạp thẻ" CssClass="title"></asp:Label>
            </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Xem log</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Xem log giao dịch nạp thẻ" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpTop" runat="server" CssClass="form-control drpTop">
                                <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                                <asp:ListItem Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 bản ghi" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group DivStatus">
                                <span class="input-group-addon" style="padding: 5px;">Status</span>
                                <asp:TextBox ID="txtStatus" Text="All" runat="server" CssClass="form-control txtStatus" list="browsers" autocomplete="off"></asp:TextBox>
                                <span class="input-group-addon icon" style="padding: 5px; border-left: none;"><i class="fa fa-sort-down"></i></span>
                                <span class="lb">All</span>
                            </div>
                        </div>
                        <style type="text/css">
                            .DivStatus {
                                position: relative;
                            }

                                .DivStatus .lb {
                                    position: absolute;
                                    display: none;
                                    left: 0;
                                    top: 33px;
                                    z-index: 10000;
                                    background: #1e90ff;
                                    border: 1px solid #7b9dd4;
                                    width: 100%;
                                    padding: 3px 0 3px 54px;
                                    color: #FFF;
                                }
                        </style>
                        <script type="text/javascript">  
                            $(document).ready(function () {
                                $(".DivStatus .lb").click(function () {
                                    $(this).css("display", "none");
                                    $(".txtStatus").val("All");
                                });
                                $(".DivStatus .icon").click(function () {
                                    $(".DivStatus .lb").css("display", "block");
                                    $(".txtStatus").select();
                                });
                                $(".txtStatus").focus(function () {
                                    $(this).select();
                                });
                                $(".txtStatus").keydown(function (e) {
                                    if (e.keyCode == 65 || e.keyCode == 76) {
                                        $(".DivStatus .lb").css("display", "block");
                                    }
                                    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190, 189, 109, 65, 76]) !== -1 ||
                                        (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                                        (e.keyCode == 67 && (e.ctrlKey === true || e.metaKey === true)) ||
                                        (e.keyCode == 88 && (e.ctrlKey === true || e.metaKey === true)) ||
                                        (e.keyCode >= 35 && e.keyCode <= 39)) {
                                        return;
                                    }
                                    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                                        e.preventDefault();
                                    }
                                });
                            });
                        </script>

                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control ">
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
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpProvider" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                        </div>
                        <% } %>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Time</span>
                                <asp:TextBox ID="txtCreatTime" runat="server" CssClass="form-control txtCreatTime"></asp:TextBox>
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
                                    <th >#Id</th>
                                    <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                                        { %>
                                    <th>Partner</th>
                                    <% } %>
                                   
                                   <%-- <th>RefCode</th>--%>
                                    <th class="show-mo">Status</th>
                                   
                                   <%-- <th>AccountName</th>--%>
                                    <th>CardSerial</th>
                                    <th>CardCode</th>
                                    <th>Amount</th>
                                    <th>AmountUser</th>
                                    <th>CardType</th>
                                    <% if (AppUtils.IsAdmin || AppUtils.IsProvider)
                                        {%>
                                    <th class="show-pc">Provider</th>
                                    <%}%>
                                    <th>CreatTime</th>
                                    <th>LastTime</th>
                                    <th >Status</th>
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
                                            
                                              
                                            <td class="show-mo"><%#Eval("Status") %></td>
                                            <%--<td><%#Eval("RequestNo") %></td>--%>
                                            
                                          <%--  <td><%#Eval("AccountName") %></td>--%>
                                            <td><%#Eval("CardSerial") %></td>
                                            <td><%#Eval("CardCode") %></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Convert.ToInt64(Eval("AmountUser")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Eval("CardType") %></td>
                                            <% if (AppUtils.IsAdmin || AppUtils.IsProvider)
                                                {%>
                                            <td class="show-pc"><%#Eval("Provider") %></td>
                                            <%}%>
                                            <td><%#Eval("CreatTime") %>  </td>
                                             <td><%#Eval("LastTime") %>  </td>

                                            <td ><%#Eval("Status") %></td>
                                            <td><a href="<%#DetailUrl(Eval("TransactionID").ToString()) %>">xem</a></td>
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
    <script type="text/javascript">
        $(function () {
            $(function () {
                $('.txtCreatTime').datetimepicker();
            });
        });
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
    <style>
        .show-mo {
            display: none;
        }
             .show-pc
        {
            display:block;
        }
        @media screen and (max-width: 767px) {
            .show-mo {
            display: block;
        }
             .show-pc
        {
            display:none;
        }
        }

    </style>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="BankCash.Monitor.aspx.cs" Inherits="Pages_Monitor_BankCash_Monitor" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
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
            <div class="alert alert-danger bg-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>

    <div class="modal  fade bs-example-modal-sm" id="MAlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1><%= Resources.Pay.Cash%>
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
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-1">
                            <asp:TextBox CssClass="form-control" ID="txtTransactionID" placeholder="ID" runat="server"></asp:TextBox>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-2">

                            <asp:DropDownList CssClass="form-control" ID="drpStatus" runat="server">
                                <%-- <asp:ListItem Text="Trạng thái" Value="-99" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Thành công" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Đợi duyệt" Value="-2"></asp:ListItem>
                                <asp:ListItem Text="Đang xử lý" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Thất bại" Value="-1"></asp:ListItem>--%>
                            </asp:DropDownList>

                        </div>

                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner || AppUtils.IsTopup)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2 ">
                            </asp:DropDownList>
                        </div>
                        <% } %>

                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2" style="display: none">
                            <asp:DropDownList ID="drpProvider" runat="server" CssClass="form-control ">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <%--<div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control" ID="drpService" runat="server">
                            </asp:DropDownList>
                        </div>--%>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">BankCode</span>
                                <asp:TextBox CssClass="form-control" ID="txtBankCode" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.RefCode%></span>
                                <asp:TextBox CssClass="form-control" ID="txtRefCode" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">Tk nhận</span>
                                <asp:TextBox CssClass="form-control" ID="txtMobile" runat="server"></asp:TextBox>
                            </div>
                        </div>
                         <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
     <div class="input-group">
         <span class="input-group-addon">Tk chuyển</span>
         <asp:TextBox CssClass="form-control" ID="txtMobile2" runat="server"></asp:TextBox>
     </div>
 </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon"><%= Resources.Pay.CashAmount%></span>
                                <asp:TextBox CssClass="form-control" ID="txtAmount" runat="server"></asp:TextBox>
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
                         <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox runat="server" ID="cbAuto" Checked="False" />
                                </span>
                                <span class="form-control">Auto Load</span>
                            </div>
                            <!-- /input-group -->

                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                            <asp:Button ID="btExcel" runat="server" CssClass="btn btn-info" OnClick="ExportTran2_Click" Text="Export Excel"></asp:Button>
                            <asp:Button ID="btCallbankk" runat="server" CssClass="btn btn-info btcallback" OnClick="Callbackall_Click" OnClientClick="return confirm('Bạn có muốn thực hiện?')" Text="CallbackAll"></asp:Button>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 " style="font-weight: bold; font-size: 105%; padding-top: 6px; float: right">
                            <asp:Label runat="server" ID="lblTotal"></asp:Label>
                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <% if (AppUtils.IsAdmin)
                            {%>
                        <div class="pull-left" style="padding: 10px">
                             <asp:Button ID="btApp2" runat="server" OnClientClick="return CheckApp2()" CssClass="btn btn-success" OnClick="btApp2Click" Text="Đơn đúng" Visible="true"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;

                        </div>

                        <div class="pull-right" style="padding: 10px" runat="server" id="dvApp">
                           
                           <asp:Button ID="btCancel" runat="server" OnClientClick="return CheckCanel()" CssClass="btn btn-default " OnClick="btCancelClick" Text="Hủy đơn"></asp:Button>
                        </div>
                        <% } %>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>
                                        <asp:CheckBox ID="cbxStatus" runat="server" CssClass="CheckAllBlock" onclick="checkAll(this);"></asp:CheckBox>

                                    </th>
                                    <th>ID</th>

                                    <% if (AppUtils.IsAdmin || AppUtils.IsPartner || AppUtils.IsTopup)
                                        { %>
                                    <th>Partner</th>
                                    <% } %>

                                    <th><%= Resources.Pay.RefCode%></th>
                                    <th><%= Resources.Pay.AccountReceive%></th>
                                    <th><%= Resources.Pay.AccountTranfer%></th>
                                    <th><%= Resources.Pay.RealAmount%></th>
                                    <th><%= Resources.Pay.Fee%></th>
                                    <%--  <% if (AppUtils.UserName.Contains("admin"))
                                        { %>
                                    <th>Hoa hồng</th>
                                    <%}%>--%>
                                    <th><%= Resources.Pay.CreatedTime%></th>
                                    <th><%= Resources.Pay.LastTime%></th>
                                    <th>Time(s)</th>
                                    <th><%= Resources.Pay.OrderInfo%></th>
                                    <% if (AppUtils.IsAdmin)
                                        {%>
                                    <th>Người duyệt</th>
                                    <%}%>

                                    <th>Status</th>

                                    <th><%= Resources.Pay.Operation%></th>

                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>

                                            <td>

                                                <asp:CheckBox ID="cbxStatus" runat="server" CssClass="CheckAll"></asp:CheckBox>&nbsp;
                                                    <asp:Label ID="ID" runat="server" Visible="false" Text='<%#Eval("TransactionID") %>'></asp:Label>

                                            </td>
                                            <td><%#Eval("TransactionID")%></td>

                                            <% if (AppUtils.IsAdmin || AppUtils.IsPartner || AppUtils.IsTopup)
                                                { %>
                                            <td><%#Eval("PartnerCode")%></td>
                                            <% } %>

                                            <td><%#Eval("RefCode") %></td>
                                            <td><%= Resources.Pay.BankCode%>:<b><%#Eval("BankCode") %></b>
                                                <br />
                                                <%= Resources.Pay.AccountNumber%>: <b><%#Eval("BankAccountNumber") %> </b>
                                                <br />
                                                <%= Resources.Pay.AccountName%>: <b><%#Eval("BankAccountName")%></b>
                                                <%-- <asp:Panel ID="pnNote" runat="server" Visible='<%# Eval("Note").ToString() != ""  %>'>

                                                    <%= Resources.Pay.Content%>:  <b><%#Eval("Note")%></b>
                                                </asp:Panel>--%>

                                            </td>
                                            <td><%#Eval("Mobile") %></td>
                                            <td <%#GetAmountStyle(Eval("Amount"))%>> <%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(".", ",") %></td>
                                            <td><%#Convert.ToInt64(Eval("Fee")).ToString("N0").Replace(".", ",") %></td>
                                            <%-- <% if (AppUtils.UserName.Contains("admin"))
                                                { %>
 <td><%#Convert.ToInt64(Eval("Reward")).ToString("N0").Replace(".", ",") %></td>
 <% } %>--%>
                                            <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}") %></td>
                                            <td><%#Eval("LastTime", "{0:dd/MM HH:mm:ss}") %></td>
                                            <td>
                                                <%# 
                                            (Eval("LastTime") != DBNull.Value && Eval("CreatedTime") != DBNull.Value) 
                                            ? ((DateTime)Eval("LastTime") - (DateTime)Eval("CreatedTime")).TotalSeconds.ToString("N0") 
                                            : ""
                                                %>
                                            </td>
                                            <td><%#Eval("OrderInfo") %></td>
                                            <% if (AppUtils.IsAdmin)
                                                {%>
                                            <td><%#Eval("ApproveUser") %></td>
                                            <%}%>


                                            <td style="width: 100px"><%#GetStatusExtra(Eval("Status"),AppUtils.IsAdmin) %></td>

                                            <td style="font-size: 105%">
                                                <a href="<%=Constant.ADMIN_PATH + Resources.Url.BankCashMonitorDetail%>?id=<%#Eval("TransactionID") %>">[<%= Resources.Pay.View%>]</a>


                                                <% if (AppUtils.IsAdmin)
                                                    { %>
                                                |
                                           <%--     <asp:LinkButton ID="lnCallback" runat="server" OnCommand="Callback_Command" CommandName="Callback" OnClientClick="return confirm('Bạn có muốn thực hiện?')" CommandArgument='<%#Eval("TransactionID")%>' Visible='<%# Eval("Status").ToString() == "0"||Eval("Status").ToString() == "-3"||Eval("Status").ToString() == "-4"  %>'> [Đơn đúng] </asp:LinkButton>--%>
                                                 <asp:HyperLink
                                                    ID="lnbill"
                                                    CssClass="lightbox"
                                                    NavigateUrl='<%# "https://info.666app.info/Pages/Detail.aspx?orderNo=" + Eval("TransactionID") %>'
                                                    runat="server"
                                                    Visible='<%# Eval("Status").ToString() == "1" %>'>
                                                 [Xem bill]
                                                </asp:HyperLink>


                                                <% } %></td>

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

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>

    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
<link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />

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
     <script type="text/javascript">

         var refreshPageInterval = 30;

         setInterval("countDownPageRefresh()", 1000);//1 s gọi 1 lần
         $(document).mouseover(function () {
             funcResetRefreshPageInterval();
         });
         $(window).scroll(function () {
             funcResetRefreshPageInterval();
         });
         window.onkeypress = funcResetRefreshPageInterval;
         function funcResetRefreshPageInterval() {
             refreshPageInterval = 60;
         }
         function countDownPageRefresh() {
             refreshPageInterval = refreshPageInterval - 1;
             //console.log(refreshPageInterval);

             if (refreshPageInterval <= 0) {
                 funcResetRefreshPageInterval();

                 if ($('#<%= cbAuto.ClientID %>').is(':checked')) {
                     document.getElementById('<%= btView.ClientID %>').click();
                 }

             }
         }

     </script>
    <script>
        $(function () {
            $(".lightbox").fancybox({
                type: "iframe",
                width: "86%",
                height: "96%",
                fitToView: true
            });
        });
</script>
    <script type="text/javascript">
        function checkAll(obj1) {
            var returnVal = undefined;
            if (obj1.checked == true) {

                returnVal = confirm("Bạn có muốn chọn tất cả?");
                if (returnVal) {
                    $(this).attr("checked", true);
                    $(".CheckAll input").attr("checked", true);
                }
            }
            else {

                returnVal = confirm("Bạn có muốn Hủy chọn tất cả?");
                if (returnVal) {
                    $(this).attr("checked", false);
                    $(".CheckAll input").attr("checked", false);
                }
            }


        }
        function CheckCanel() {
            var check = 0;
            $('.CheckAll input').each(function (i, e) {

                if ($(e).prop("checked") == true) {
                    check = 1;

                }

            });

            if (check == 0) {

                alert('Vui lòng chọn đơn để hủy');
                return false;
            }
            if (confirm("Bạn có muốn hủy đơn?")) {
                return true;
            }
            return false;
        }
        function CheckApp() {
            var check = 0;
            $('.CheckAll input').each(function (i, e) {

                if ($(e).prop("checked") == true) {
                    check = 1;

                }

            });

            if (check == 0) {

                alert('Vui lòng chọn đơn để duyệt tay');
                return false;
            }
            if (confirm("Bạn có muốn duyệt tay các đơn đã chọn?")) {
                return true;
            }
            return false;
        }
        function CheckApp2() {
            var check = 0;
            $('.CheckAll input').each(function (i, e) {

                if ($(e).prop("checked") == true) {
                    check = 1;

                }

            });

            if (check == 0) {

                alert('Vui lòng chọn đơn để cập nhật');
                return false;
            }
            if (confirm("Bạn có muốn cập nhật đơn?")) {
                return true;
            }
            return false;
        }
        $(document).ready(function () {
            $(".lstlightbox a").fancybox({

            });
        });
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

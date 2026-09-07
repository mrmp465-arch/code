<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="BankCash.App2.aspx.cs" Inherits="Pages_Monitor_BankCash_App2" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="MAlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
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
        <h1>Xuất khoản
            <small>Duyệt đơn</small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Duyệt đơn</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Duyệt đơn</h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList CssClass="form-control" ID="drpTop" runat="server">


                                <asp:ListItem Text="50 row" Value="50"></asp:ListItem>
                                <asp:ListItem Text="100 row" Value="100" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="500 row" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 row" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-1">
                            <asp:TextBox CssClass="form-control" ID="txtTransactionID" placeholder="ID" runat="server"></asp:TextBox>
                        </div>


                        <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2 ">
                            </asp:DropDownList>
                        </div>
                        <% } %>



                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-2">
                            <div class="input-group">
                                <span class="input-group-addon">RefCode</span>
                                <asp:TextBox CssClass="form-control" ID="txtRefCode" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px">Từ ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtFromDate" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px">Đến ngày</span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtCreatTime" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>

                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4 " style="font-weight: bold; font-size: 105%; padding-top: 6px; float: right">
                            <asp:Label runat="server" ID="lblTotal"></asp:Label>
                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>

                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>

                                    <th>STT</th>
                                    <th>ID</th>

                                    <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                                        { %>
                                    <th>Partner</th>
                                    <% } %>

                                    <th>RefCode</th>
                                    <th>Số tiền</th>
                                    <th>Tài khoản nhận</th>


                                    <th>Time</th>
                                    <th>Quản trị</th>



                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>

                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><%#Eval("TransactionID")%></td>

                                            <% if (AppUtils.IsAdmin || AppUtils.IsPartner)
                                                { %>
                                            <td><%#Eval("PartnerCode")%></td>
                                            <% } %>

                                            <td><%#Eval("RefCode") %></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%= Resources.Pay.BankCode%>:<b><%#Eval("BankCode") %></b>
                                                <br />
                                                <%= Resources.Pay.AccountNumber%>: <b><%#Eval("BankAccountNumber") %> </b>
                                                <br />
                                                <%= Resources.Pay.AccountName%>: <b><%#Eval("BankAccountName")%></b>


                                            </td>

                                            <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}") %></td>
                                            <td>
                                                <asp:LinkButton
                                                    ID="lnkApp"
                                                    runat="server"
                                                    Text="[Duyệt]"
                                                    OnClientClick='<%# "return showOtpModal(\"" + Eval("TransactionID") + "\");" %>'>
                                                </asp:LinkButton>
                                                &nbsp;| &nbsp;
                                                 <asp:LinkButton
                                                     ID="lnCancel"
                                                     runat="server"
                                                     Text="[Huỷ]"
                                                     OnClientClick='<%# "return showOtpModal2(\"" + Eval("TransactionID") + "\");" %>'>
                                                 </asp:LinkButton>
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
    <!-- Modal nhập OTP -->
    <div class="modal fade" id="otpModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Xác nhận bằng OTP</h5>
                </div>
                <div class="modal-body">
                    <p>Nhập mã F2A để xác nhận duyệt:</p>
                    <input type="text" id="txtOtp" class="form-control" maxlength="6" />
                    <input type="hidden" id="hiddenUserId" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy</button>
                    <button type="button" class="btn btn-danger" onclick="confirmOtp()">Xác nhận</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="otpModal2" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Xác nhận bằng F2A</h5>
                </div>
                <div class="modal-body">
                    <p>Nhập mã F2A để xác nhận huỷ:</p>
                    <input type="text" id="txtOtp2" class="form-control" maxlength="6" />
                    <input type="hidden" id="hiddenUserId2" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy</button>
                    <button type="button" class="btn btn-danger" onclick="confirmOtp2()">Xác nhận</button>
                </div>
            </div>
        </div>
    </div>
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
            //showOtpModal2('2');
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
        function showOtpModal(userId) {
            if (confirm("Bạn có chắc muốn duyệt đơn này không?")) {
                document.getElementById("hiddenUserId").value = userId;
                $('#otpModal').modal()
            }
            return false; // Ngăn postback của LinkButton
        }

        function confirmOtp() {
            var otp = document.getElementById("txtOtp").value;
            var userId = document.getElementById("hiddenUserId").value;

            if (!otp) {
                alert("Vui lòng nhập F2A!");
                return;
            }

            // Gửi OTP về server qua __doPostBack
            __doPostBack('ConfirmOtp', userId + '|' + otp);
        }

        function showOtpModal2(userId) {
            if (confirm("Bạn có chắc muốn huỷ đơn này không?")) {
                document.getElementById("hiddenUserId2").value = userId;

                $('#otpModal2').modal()
            }
            return false; // Ngăn postback của LinkButton
        }

        function confirmOtp2() {
            var otp = document.getElementById("txtOtp2").value;
            var userId = document.getElementById("hiddenUserId2").value;

            if (!otp) {
                alert("Vui lòng nhập F2A!");
                return;
            }

            // Gửi OTP về server qua __doPostBack
            __doPostBack('ConfirmOtp2', userId + '|' + otp);
        }
    </script>
    <style>
        .label {
            width: 76px !important;
            padding: 3px;
            display: block
        }

        .txtCreatTime {
            width: 138px !important;
            padding: 3px !important;
        }
    </style>
</asp:Content>

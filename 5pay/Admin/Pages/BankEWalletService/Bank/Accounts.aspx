<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Accounts.aspx.cs" Inherits="Pages_BankEWalletService_Bank_Accounts" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


    <div class="modal  fade bs-example-modal-sm" id="MAlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss">Cập nhật thành công</asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1>Cấu hình Bank
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Tài khoản Bank" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Tài khoản Bank</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Bank</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Tài khoản Bank</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.ITAccount %>">Tài khoản chứa</a></li>
                <%--  <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankPartner %>">Quản lý kênh</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerBank %>">Phân bố kênh & Tài khoản</a></li>--%>
            </ul>

            <div class="row">
                <div class="col-xs-12">


                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Danh sách tài khoản Bank" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">


                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Tên</span>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Bank ID </span>
                                <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">BankType</span>
                                <asp:DropDownList ID="drpBankType" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Chọn loại:" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Cá nhân" Value="IND"></asp:ListItem>
                                    <asp:ListItem Text="Doanh nghiệp" Value="BIZ"></asp:ListItem>

                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">BankCode</span>
                                <asp:DropDownList ID="drpBank" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="BankCode" Value=""></asp:ListItem>
                                    <asp:ListItem Text="ACB" Value="ACB"></asp:ListItem>
                                    <asp:ListItem Text="VPB" Value="VPB"></asp:ListItem>
                                    <asp:ListItem Text="MB" Value="MB"></asp:ListItem>
                                    <asp:ListItem Text="ICB" Value="ICB"></asp:ListItem>
                                    <asp:ListItem Text="BIDV" Value="BIDV"></asp:ListItem>
                                    <asp:ListItem Text="VCB" Value="VCB"></asp:ListItem>
                                    <asp:ListItem Text="SEAB" Value="SEAB"></asp:ListItem>
                                    <asp:ListItem Text="TIMO" Value="TIMO"></asp:ListItem>
                                    <asp:ListItem Text="OCB" Value="OCB"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Loại</span>
                                <asp:DropDownList ID="drpType" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Loại" Value=""></asp:ListItem>
                                    <asp:ListItem Text="IN" Value="IN"></asp:ListItem>
                                    <asp:ListItem Text="OUT" Value="OUT"></asp:ListItem>
                                    <asp:ListItem Text="INOUT" Value="INOUT"></asp:ListItem>
                                    <asp:ListItem Text="OUTALL" Value="OUTALL"></asp:ListItem>

                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">PC</span>
                                <asp:DropDownList ID="drpComputer" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Chọn PC:" Value="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="PC001" Value="PC001"></asp:ListItem>
                                    <asp:ListItem Text="PC002" Value="PC002"></asp:ListItem>
                                    <asp:ListItem Text="PC003" Value="PC003"></asp:ListItem>
                                    <asp:ListItem Text="PC004" Value="PC004"></asp:ListItem>
                                    <asp:ListItem Text="PC005" Value="PC005"></asp:ListItem>
                                    <asp:ListItem Text="PC006" Value="PC006"></asp:ListItem>
                                    <asp:ListItem Text="PC007" Value="PC007"></asp:ListItem>
                                    <asp:ListItem Text="PC008" Value="PC008"></asp:ListItem>
                                    <asp:ListItem Text="PC009" Value="PC009"></asp:ListItem>
                                    <asp:ListItem Text="PC010" Value="PC010"></asp:ListItem>
                                    <asp:ListItem Text="PC011" Value="PC011"></asp:ListItem>
                                    <asp:ListItem Text="PC012" Value="PC012"></asp:ListItem>
                                    <asp:ListItem Text="PC013" Value="PC013"></asp:ListItem>
                                    <asp:ListItem Text="PC014" Value="PC014"></asp:ListItem>
                                    <asp:ListItem Text="PC015" Value="PC005"></asp:ListItem>
                                    <asp:ListItem Text="PC016" Value="PC016"></asp:ListItem>
                                    <asp:ListItem Text="PC017" Value="PC017"></asp:ListItem>
                                    <asp:ListItem Text="PC018" Value="PC018"></asp:ListItem>
                                    <asp:ListItem Text="PC019" Value="PC019"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Login</span>
                                <asp:DropDownList ID="drpStatusExtra" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Bình thường" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Khóa" Value="-1"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Trạng thái</span>
                                <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value="-1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Kích hoạt" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Chưa kích hoạt" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                            <asp:Button ID="btAdd" CssClass="btn btn-primary" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>
                        </div>
                        <%-- <div style="clear: both"></div>--%>

                        <div class="col-xs-12 col-sm-6 col-md-4" style="font-weight: bold; float: right">
                            <asp:Label runat="server" ID="lblTota2"></asp:Label><br />
                            <asp:Label runat="server" ID="lblTotal"></asp:Label>
                        </div>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body  no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <% if (AppUtils.IsAdmin)
                            {%>
                        <div class="pull-left" style="padding: 10px">
                            <asp:Button ID="btApp2" runat="server" OnClientClick="return CheckApp()" CssClass="btn btn-success" OnClick="btApp2Click" Text="Bật bank" Visible="true"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;

                        </div>

                        <div class="pull-right" style="padding: 10px" runat="server" id="dvApp">

                            <asp:Button ID="btCancel" runat="server" OnClientClick="return CheckCanel()" CssClass="btn btn-default " OnClick="btCancelClick" Text="Tắt bank"></asp:Button>
                        </div>
                        <% } %>
                        <div style="height: 20px; clear: both;"></div>

                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>
                                        <asp:CheckBox ID="cbxStatus" runat="server" CssClass="CheckAllBlock" onclick="checkAll(this);"></asp:CheckBox>

                                    </th>
                                    <th>Stt</th>
                                    <th>Id</th>
                                    <%-- <th>BankType</th>--%>
                                    <th>Bank</th>
                                    <%-- <th>BankAccount</th>--%>
                                    <th>Name</th>
                                    <th>BankId</th>
                                    <th>D.In</th>
                                    <th>M.In</th>
                                    <th>D.Out</th>
                                    <th>M.Out</th>
                                    <th>Balance</th>
                                    <th>M.D.In</th>
                                    <th>M.M.In</th>
                                    <th>Type</th>
                                     <th>Time</th>
                                    <th>QR</th>
                                    <th>In</th>
                                    <th>Out</th>
                                    <th>T.Thái</th>
                                    <%-- <th>Partner</th>--%>
                                    <th>K.Hoạt</th>
                                    <th>PC</th>
                                    <th>AppDeviceId</th>
                                    <th>Ghi chú</th>
                                    <th>T.Vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td>

                                                <asp:CheckBox ID="cbxStatus2" runat="server" CssClass="CheckAll"></asp:CheckBox>&nbsp;
                                             <asp:Label ID="ID" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>

                                            </td>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><%#Eval("Id") %></td>
                                            <%-- <td><%#Eval("BankType") %></td>--%>

                                            <td><%#Eval("BankCode") %></td>
                                            <%--<td><%#Eval("BankAccount") %></td>--%>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankAccountEdit %>?id=<%#Eval("Id") %>" title="<%#Eval("StopScanAt") %>"><%#Eval("BankName") %></a></td>
                                            <td><%#Eval("BankId") %></td>
                                            <%--<td><%#Convert.ToInt32(Eval("BalanceDayIn")).ToString("N0").Replace(",", ".") %></td>--%>
                                            <td><%#Convert.ToInt64(Eval("BalanceDayIn")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceMonthIn")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceDayOut")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceMonthOut")).ToString("N0") %></td>
                                            <td><span class='<%#GetBankClass(Eval("Type"),Eval("Status"),Eval("BalanceTotal"),Eval("BalanceMaxDay"),Eval("BankCode"))%>'><%#Convert.ToInt64(Eval("BalanceTotal")).ToString("N0") %></span></td>
                                            <td><%#Eval("BalanceMaxDay") %></td>
                                            <td><%#Eval("BalanceMaxMonth")%></td>
                                            <td><%#Eval("Type") %></td>
                                              <td><%#GetDate(Eval("CreatedTime")) %></td>
                                            <td class="lstlightbox">
                                                <asp:Panel ID="pnQR" runat="server" Visible='<%# Eval("Type").ToString().Contains("OUT") %>'>
                                                    <a href="<%#GetQR(Eval("BankCode").ToString(),Eval("BankId").ToString(),Eval("BankName").ToString()) %>">
                                                        <img src="/cmspay/qr.png" height="25" />
                                                    </a>
                                                </asp:Panel>
                                            </td>


                                            <td><%#GetStatus(Eval("StatusOverIn"))%></td>
                                            <td><%#GetStatus(Eval("StatusOverOut")) %></td>
                                            <td><%#GetStatusExtra(Eval("StatusExtra")) %></td>
                                            <%--<td><%#Eval("PartnerName")%></td>--%>
                                            <td>

                                                <asp:CheckBox ID="cbxStatus" CssClass="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>' Title='<%#Eval("Id") %>'></asp:CheckBox>
                                                <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                            </td>
                                            <td><%#Eval("PhoneDevice") %>-<%#Eval("Computer") %></td>
                                            <td><%#Eval("AppDeviceId") %></td>
                                             <td><%#Eval("Note") %></td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankAccountDelete %>?id=<%#Eval("Id") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">[Xóa]</a>
                                                <asp:LinkButton ID="lnkUpdateTime" runat="server" OnCommand="UpdateTime_Command" Visible='<%#  Eval("Type").ToString().Contains("IN")   %>' CommandName="Callback" CommandArgument='<%#Eval("Id")%>' OnClientClick="return confirm('Bạn có muốn quét lại?')">[Quét lại] </asp:LinkButton>
                                                &nbsp; &nbsp;<asp:LinkButton ID="lnCallback" runat="server" OnCommand="Reset_Command" Visible='<%#  Eval("StatusOverIn").ToString() != "1" %>' CommandName="Callback" CommandArgument='<%#Eval("Id")%>' OnClientClick="return confirm('Bạn có muốn reset?')">[Reset] </asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <%--<tfoot>
                            <tr>
                                <th rowspan="1" colspan="1">Tổng:</th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1">CSS grade</th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                                <th rowspan="1" colspan="1"></th>
                            </tr>
                            </tfoot>--%>
                        </table>
                    </div>
                    <div class="box-footer">
                    </div>
                    <!-- /.box-body -->
                </div>
                <!-- /.box -->

                <!-- /.col -->
            </div>

        </div>
        <!-- /.row -->
    </section>
    <!-- /.content -->
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true,
                "autoWidth": false,
                "paging": false,
                "searching": false,
                "info": false,
                "ordering": true,
            });
            new $.fn.dataTable.FixedHeader(table);

        });
        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
        $(document).ready(function () {
            $(".lstlightbox a").fancybox({

            });
        });
        this.DocumentHeght = function () {
            return $(document).height();
        };
        this.GetFullHeight = function () {
            return parseInt($(document).scrollTop() + $('html').height());
        };
        this.DocumentWidth = function () { return $(document).width(); };
        this.WindowHeight = function () { return $(window).height(); };
        this.WindowWidth = function () { return $(window).width(); };
        function Loading() {
            var html = '<div id="LoadingContainer"><div  id="Loading" style="display: none; text-align: center; overflow-y: none; vertical-align: middle;"><img src="/cmspay/Content/loading48.gif" alt="loadding" /></div>';
            html += '<div  id="LoadingOverlay"></div>';
            html += '<style> #Loading{	width: 300px;	height: 300px;	z-index: 1400;	position: fixed;	padding: 5px;}#LoadingOverlay{	-moz-opacity: 0.8;	opacity: .80;	filter: alpha(opacity=10);	position: absolute;	z-index: 1200;	top: 0;	left: 0;	width: 100%;	height: 100%;	display: none;	background-color: #fff;}</style></div>';
            $('body').append(html);
            $('#Loading');
            $('#LoadingOverlay').show();
            var leftOffset = (this.WindowWidth() - 300) / 2;
            var topOffset = (this.GetFullHeight() - 300) / 2;
            $('#Loading').css('width', 300);
            $('#Loading').css('height', 300);
            $('#Loading').css('left', leftOffset);
            $('#Loading').css('top', '47%'); //topOffset);
            $('#Loading').show();
            $('#LoadingOverlay').css('height', this.GetFullHeight());

        };
        function UnLoading() {
            $('#LoadingContainer').remove();

        };
        function UpdateBank(id) {

            if (confirm("Bạn có muốn cập nhật")) {
                var params = {
                    id: id
                };
                Loading();
                $.ajax({
                    type: 'GET',
                    url: '/cmspay/ServiceHandler/UpdateBankStatus.ashx',
                    data: params,
                    success: function (data) {
                        UnLoading();
                        $('#MAlertSuccess').modal()
                    },
                    error: function () {
                        UnLoading();
                    }
                });
            }

        }
        $(document).ready(function () {
            $(".cbxStatus").click(function () {
                UpdateBank($(this).attr("title"));
            });
        });

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

                alert('Vui lòng chọn bank để cập nhật');
                return false;
            }
            if (confirm("Bạn có muốn tắt bank?")) {
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

                alert('Vui lòng chọn bank để cập nhật');
                return false;
            }
            if (confirm("Bạn có muốn bật bank?")) {
                return true;
            }
            return false;
        }
    </script>
    <style>
        .bwarning {
            color: red;
            font-weight: bold;
            animation: blinker 1s linear infinite;
        }

        @keyframes blinker {
            50% {
                opacity: 0;
            }
        }

        .table > thead > tr > td, .table > thead > tr > th {
            padding: 8px 2px;
        }
    </style>
</asp:Content>

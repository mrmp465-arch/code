<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Accounts.aspx.cs" Inherits="Pages_Momo_Accounts" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <div class="modal  fade bs-example-modal-sm" id="MAlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss">Cập nhật thành công</asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Tài khoản momo" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Tài khoản momo</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>MOMO</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Tài khoản momo</a></li>
                <%--<li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoPartner %>">Quản lý kênh</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerMomo %>">Phân bố kênh & Tài khoản</a></li>--%>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoAccountCash %>">Tài khoản Bank Hold Momo</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">


                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Danh sách tài khoản momo" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">


                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Tên/sdt</span>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Trang</span>
                                <asp:DropDownList ID="ddlPage" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="All" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="1" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="5" Value="5"></asp:ListItem>

                                    <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                    <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                    <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                    <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                    <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                    <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                    <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                    <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                    <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                    <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                    <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                    <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                    <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                    <asp:ListItem Text="21" Value="21"></asp:ListItem>
                                    <asp:ListItem Text="22" Value="22"></asp:ListItem>
                                    <asp:ListItem Text="23" Value="23"></asp:ListItem>
                                    <asp:ListItem Text="24" Value="24"></asp:ListItem>
                                    <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                    <asp:ListItem Text="26" Value="26"></asp:ListItem>
                                    <asp:ListItem Text="27" Value="27"></asp:ListItem>
                                    <asp:ListItem Text="28" Value="28"></asp:ListItem>
                                    <asp:ListItem Text="29" Value="29"></asp:ListItem>
                                    <asp:ListItem Text="30" Value="30"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpType" runat="server" CssClass="form-control select2">
                                <asp:ListItem Text="Loại" Value=""></asp:ListItem>
                                <asp:ListItem Text="IN" Value="IN"></asp:ListItem>
                                <asp:ListItem Text="OUT" Value="OUT"></asp:ListItem>
                                <asp:ListItem Text="INOUT" Value="INOUT"></asp:ListItem>
                                <asp:ListItem Text="OUTALL" Value="OUTALL"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Login</span>
                                <asp:DropDownList ID="drpStatusExtra" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value="-999"></asp:ListItem>
                                    <asp:ListItem Text="Bình thường" Value="4" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Logged" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="OTPRequired" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Ide" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Error" Value="-1"></asp:ListItem>
                                    <asp:ListItem Text="LoginFailed" Value="-3"></asp:ListItem>
                                    <asp:ListItem Text="AccLocked" Value="-4"></asp:ListItem>
                                    <asp:ListItem Text="OTPOver" Value="-5"></asp:ListItem>
                                    <asp:ListItem Text="FaceOver" Value="-6"></asp:ListItem>
                                    <asp:ListItem Text="FaceNotMatched" Value="-7"></asp:ListItem>
                                    <asp:ListItem Text="MissingKYC" Value="-8"></asp:ListItem>
                                    <asp:ListItem Text="CaptchaRequired" Value="-9"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-12 col-md-1" style="display: none">
                            <div class="form-group">
                                <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2" OnSelectedIndexChanged="drpPartner_SelectedIndexChanged" AutoPostBack="True">
                                    <%-- <asp:ListItem Text="Source" Value=""></asp:ListItem>
                                    <asp:ListItem Text="inhouse" Value="inhouse"></asp:ListItem>
                                    <asp:ListItem Text="mouse" Value="mouse"></asp:ListItem>--%>
                                </asp:DropDownList>
                            </div>
                        </div>


                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Trạng thái</span>
                                <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value="-2"></asp:ListItem>
                                    <asp:ListItem Text="Sẵn sàng" Value="2" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Kích hoạt" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Chưa kích hoạt" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Bỏ qua" Value="-1"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Detect</span>
                                <asp:DropDownList ID="drpDetect" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value="-2"></asp:ListItem>
                                    <asp:ListItem Text="Chưa detect" Value="-1"></asp:ListItem>
                                    <asp:ListItem Text="Đã detect" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Đợi detect" Value="0"></asp:ListItem>

                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2" style="float: left">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Giải pháp</span>
                                <asp:DropDownList ID="drpSolution" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value=""></asp:ListItem>
                                    <asp:ListItem Text="APIV2" Value="APIV2"></asp:ListItem>
                                    <asp:ListItem Text="APIV3" Value="APIV3"></asp:ListItem>
                                    <asp:ListItem Text="APIV4" Value="APIV4"></asp:ListItem>
                                    <asp:ListItem Text="API" Value="API"></asp:ListItem>


                                </asp:DropDownList>
                            </div>
                        </div>
                        <div style="clear: both"></div>
                        <div class="col-xs-12 col-sm-6 col-md-2" style="float: left">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                            &nbsp;
                         <asp:Button ID="btAdd" CssClass="btn btn-primary" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>



                        </div>


                        <div class="col-xs-12 col-sm-6 col-md-4" style="font-weight: bold; float: right">
                            <asp:Label runat="server" ID="lblTotal"></asp:Label>
                        </div>
                    </div>

                    <div style="clear: both"></div>

                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:TextBox ID="txtUser" runat="server" placeholder="Số ví nhận tiền" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btApp" runat="server" OnClientClick="return CheckApp()" CssClass="btn btn-info" OnClick="btAppClick" Text="Chuyển tiền"></asp:Button>
                             &nbsp;&nbsp;
 <asp:Button ID="btApp2" runat="server" OnClientClick="return CheckApp2()" CssClass="btn btn-info" OnClick="btApp2Click" Text="Bật"></asp:Button>
                        </div>
                    </div>

                    <!-- /.box-<div style="clear: both"></div>header -->
                    <div class="box-body  no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>
                                        <asp:CheckBox ID="cbxStatus" runat="server" CssClass="CheckAllBlock" onclick="checkAll(this);"></asp:CheckBox></th>
                                    <th>Stt</th>
                                    <th>Id</th>
                                    <th>Name</th>
                                    <th>Mobile</th>
                                    <th>D.In</th>
                                    <th>M.In</th>
                                    <th>D.Out</th>
                                    <th>M.Out</th>
                                    <th>Balance</th>
                                    <th>D.In(M)</th>
                                    <th>M.In(M)</th>
                                    <%--<th>N.M.Cash</th>--%>
                                    <th>T.M.Cash</th>
                                    <th>Type</th>
                                    <th>In</th>
                                    <th>Out</th>
                                    <th>T.Thái</th>
                                    <th>Source</th>
                                    <th>LoginDate</th>
                                    <th>K.Hoạt</th>
                                    <%-- <th>G.Pháp</th>--%>
                                    <th>T.Vụ</th>

                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus2" runat="server" CssClass="CheckAll"></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="ID" runat="server" Visible="false" Text='<%#Eval("MomoId") %>'></asp:Label>
                                            </td>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><%#Eval("Id") %></td>
                                            <td style="<%#GetSolutionStyle(Eval("ByPass20M")) %>"><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.MomoAccountEdit %>?id=<%#Eval("Id") %>" title="<%#Eval("StopScanAt") %>"><%#Eval("MomoName") %>
                                               
                                            </a></td>
                                            <td><%#Eval("MomoId") %></td>
                                            <%--<td><%#Convert.ToInt32(Eval("BalanceDayIn")).ToString("N0").Replace(",", ".") %></td>--%>
                                            <td><%#Convert.ToInt64(Eval("BalanceDayIn")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceMonthIn")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceDayOut")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceMonthOut")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceTotal")).ToString("N0") %></td>
                                            <td><%#Eval("BalanceMaxDay") %></td>
                                            <td><%#Eval("BalanceMaxMonth")%></td>
                                            <%--<td><%#Eval("CashTimeMonth")%></td>--%>
                                            <td><%#Convert.ToInt64(Eval("CashTotalMonth")).ToString("N0")%></td>
                                            <td><%#Eval("Type") %></td>
                                            <td><%#GetStatus(Eval("StatusOver"))%></td>
                                            <td><%#GetStatus(Eval("StatusOverOut")) %></td>
                                            <td><%#GetStatusExtra(Eval("StatusExtra")) %></td>
                                            <%-- <td><%#Eval("Source")%></td>--%>
                                            <td><%#Eval("Source")%></td>
                                            <td><%#GetDate(Eval("CreatedAt")) %></td>
                                            <td><%#GetStatusActive(Eval("Status")) %> &nbsp;

                                                 <asp:CheckBox Visible='<%# (Eval("Status").ToString() == "1" || Eval("Status").ToString() == "0" ) %> ' ID="cbxStatus" CssClass="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>' Title='<%#Eval("Id") %>'></asp:CheckBox>
                                                <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                                &nbsp;<%--<asp:LinkButton runat="server" Title='<%#Eval("Id") %>' CssClass="lbstatus">Dừng hẳn</asp:LinkButton>--%>

                                                <asp:LinkButton ID="lnkUpdateTime" runat="server" OnCommand="UpdateTime_Command" Visible='true' CommandName="Callback" CommandArgument='<%#Eval("Id")%>' OnClientClick="return confirm('Bạn có muốn quét lại?')">[Quét lại] </asp:LinkButton>
                                                <a onclick='StopBank(<%#Eval("Id") %>)' href="javascript:;" class="lbstatus">[Dừng hẳn]</a>
                                            </td>



                                            <%-- <td>
                                               
                                                <asp:CheckBox ID="cbxStatus" CssClass="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>' Title='<%#Eval("Id") %>'></asp:CheckBox>
                                                <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                            </td>--%>
                                            <%--<td><%#Eval("Solution") %></td>--%>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.MomoAccountDelete %>?id=<%#Eval("Id") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">[Xóa]</a>
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


            <!-- /.row -->
    </section>
    <!-- /.content -->
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
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
                    url: '/cmspay/ServiceHandler/UpdateMomoStatus.ashx',
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
        function StopBank(id) {

            if (confirm("Bạn có muốn dừng hẳn")) {
                var params = {
                    id: id
                };
                Loading();
                $.ajax({
                    type: 'GET',
                    url: '/cmspay/ServiceHandler/StopMomo.ashx',
                    data: params,
                    success: function (data) {
                        UnLoading();
                        //$("button[id$='btView']").click();
                        $("#<%= btView.ClientID %>").click();
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
            //$(".lbstatus").click(function () {
            //    StopBank($(this).attr("title"));
            //});
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

                alert('Vui lòng chọn tài khoản để chuyển tiền');
                return false;
            }
            if (confirm("Bạn có muốn chuyển tiền?")) {
                return true;
            }
            return false;
        }

    </script>
</asp:Content>

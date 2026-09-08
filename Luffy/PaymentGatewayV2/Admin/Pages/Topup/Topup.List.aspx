<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.List.aspx.cs" Inherits="Pages_Topup_Topup_List" %>

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
        <h1>Topup
            <small>
                <asp:Label ID="lblTtitle" runat="server" Text="Danh sách Order" CssClass="title"></asp:Label>
            </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Xem log</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Theo dõi đơn hàng</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Danh sách Order</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.TopupListOrder %>">Order hoạt động</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.TopupListOrderConfirm%>">Order đã chốt</a></li>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpTop" runat="server" CssClass="form-control drpTop">
                                <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                                <asp:ListItem Selected="True" Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 bản ghi" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="txtStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Trạng thái:</asp:ListItem>
                                <asp:ListItem Value="-3">Đợi nạp</asp:ListItem>
                                <asp:ListItem Selected="True" Value="1">Đợi xử lý</asp:ListItem>
                                <asp:ListItem Value="2">Đang xử lý</asp:ListItem>
                                <asp:ListItem Value="3">Đã hoàn thành</asp:ListItem>
                                <asp:ListItem Value="0">Không sử dụng</asp:ListItem>
                                <asp:ListItem Value="-1">Bỏ qua</asp:ListItem>
                                <asp:ListItem Value="-2">Telco khóa</asp:ListItem>
                                <asp:ListItem Value="-4">Telco hết lượt</asp:ListItem>
                                <asp:ListItem Value="-6">Dừng để Review</asp:ListItem>
                            </asp:DropDownList>
                        </div>


                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="txtUsers" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="txtTelco" runat="server" CssClass="form-control">
                                <asp:ListItem Selected="True" Value="">Telco:</asp:ListItem>
                                <asp:ListItem Value="vtt">VTT</asp:ListItem>
                                <asp:ListItem Value="vms">VMS</asp:ListItem>
                                <asp:ListItem Value="vnp">VNP</asp:ListItem>
                                <asp:ListItem Value="gosu">GOSU</asp:ListItem>
                                <asp:ListItem Value="zing">ZING</asp:ListItem>
                                <asp:ListItem Value="garena">GARENA</asp:ListItem>
                                <asp:ListItem Value="vtc">VTC</asp:ListItem>
                                <asp:ListItem Value="dzo">DZO</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="txtTopupType" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Thuê bao:</asp:ListItem>
                                <asp:ListItem Value="1">Nạp số dư trả trước</asp:ListItem>
                                <asp:ListItem Value="2">Nạp số dư trả sau</asp:ListItem>
                                <asp:ListItem Value="3">Nạp cước Internet</asp:ListItem>
                                <asp:ListItem Value="4">Nạp Smas</asp:ListItem>
                                <asp:ListItem Value="5">Nạp cước điện thoại cố định</asp:ListItem>
                                <asp:ListItem Value="6">Nạp Nhà thuốc (PPG)</asp:ListItem>
                                <asp:ListItem Value="7">Nạp Tiêm Chủng (VNCDC)</asp:ListItem>
                                <asp:ListItem Value="8">Nạp ShopOne</asp:ListItem>
                                <asp:ListItem Value="10">Nạp Metro Wan/Leased line</asp:ListItem>
                                <asp:ListItem Value="9">Nạp Game I</asp:ListItem>
                                <asp:ListItem Value="11">Nạp Game II</asp:ListItem>
                                <asp:ListItem Value="12">Nạp Game III</asp:ListItem>
                                <asp:ListItem Value="13">Nạp Game IV</asp:ListItem>
                                <asp:ListItem Value="14">Nạp Game V</asp:ListItem>
                                <asp:ListItem Value="15">Nạp Game VI</asp:ListItem>
                                <asp:ListItem Value="16">Nạp Game M VII</asp:ListItem>
                                <asp:ListItem Value="17">Nạp Game M VIII</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Order No</span>
                                <asp:TextBox ID="txtOrderNo" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Mobile</span>
                                <asp:TextBox ID="txtMobile" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Full Name</span>
                                <asp:TextBox ID="txtFullName" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="txtPriority" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Priority:</asp:ListItem>
                                <asp:ListItem Value="49">P49</asp:ListItem>
                                <asp:ListItem Value="48">P48</asp:ListItem>
                                <asp:ListItem Value="47">P47</asp:ListItem>
                                <asp:ListItem Value="46">P46</asp:ListItem>
                                <asp:ListItem Value="45">P45</asp:ListItem>
                                <asp:ListItem Value="44">P44</asp:ListItem>
                                <asp:ListItem Value="43">P43</asp:ListItem>
                                <asp:ListItem Value="42">P42</asp:ListItem>
                                <asp:ListItem Value="41">P41</asp:ListItem>
                                <asp:ListItem Value="40">P40</asp:ListItem>
                                <asp:ListItem Value="39">P39</asp:ListItem>
                                <asp:ListItem Value="38">P38</asp:ListItem>
                                <asp:ListItem Value="37">P37</asp:ListItem>
                                <asp:ListItem Value="36">P36</asp:ListItem>
                                <asp:ListItem Value="35">P35</asp:ListItem>
                                <asp:ListItem Value="34">P34</asp:ListItem>
                                <asp:ListItem Value="33">P33</asp:ListItem>
                                <asp:ListItem Value="32">P32</asp:ListItem>
                                <asp:ListItem Value="31">P31</asp:ListItem>
                                <asp:ListItem Value="30">P30</asp:ListItem>
                                <asp:ListItem Value="29">P29</asp:ListItem>
                                <asp:ListItem Value="28">P28</asp:ListItem>
                                <asp:ListItem Value="27">P27</asp:ListItem>
                                <asp:ListItem Value="26">P26</asp:ListItem>
                                <asp:ListItem Value="25">P25</asp:ListItem>
                                <asp:ListItem Value="24">P24</asp:ListItem>
                                <asp:ListItem Value="23">P23</asp:ListItem>
                                <asp:ListItem Value="22">P22</asp:ListItem>
                                <asp:ListItem Value="21">P21</asp:ListItem>
                                <asp:ListItem Value="20">P20</asp:ListItem>
                                <asp:ListItem Value="19">P19</asp:ListItem>
                                <asp:ListItem Value="18">P18</asp:ListItem>
                                <asp:ListItem Value="17">P17</asp:ListItem>
                                <asp:ListItem Value="16">P16</asp:ListItem>
                                <asp:ListItem Value="15">P15</asp:ListItem>
                                <asp:ListItem Value="14">P14</asp:ListItem>
                                <asp:ListItem Value="13">P13</asp:ListItem>
                                <asp:ListItem Value="12">P12</asp:ListItem>
                                <asp:ListItem Value="11">P11</asp:ListItem>
                                <asp:ListItem Value="10">P10</asp:ListItem>
                                <asp:ListItem Value="9">P9</asp:ListItem>
                                <asp:ListItem Value="8">P8</asp:ListItem>
                                <asp:ListItem Value="7">P7</asp:ListItem>
                                <asp:ListItem Value="6">P6</asp:ListItem>
                                <asp:ListItem Value="5">P5</asp:ListItem>
                                <asp:ListItem Value="4">P4</asp:ListItem>
                                <asp:ListItem Value="3">P3</asp:ListItem>
                                <asp:ListItem Value="2">P2</asp:ListItem>
                                <asp:ListItem Value="1">P1</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="txtUssd" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Ussd:</asp:ListItem>
                                <asp:ListItem Value="0">Không</asp:ListItem>
                                <asp:ListItem Value="1">Có</asp:ListItem>
                                <asp:ListItem Value="2">Chỉ chạy Ussd</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="txtConfirm" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Tình trạng:</asp:ListItem>
                                <asp:ListItem Value="0" Selected="True">Chưa chốt</asp:ListItem>
                                <asp:ListItem Value="1">Đã chốt</asp:ListItem>
                            </asp:DropDownList>
                        </div>
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
                                    <th>Trans Id</th>
                                    <th>Amount</th>
                                    <th>Amount Success</th>
                                   <%-- <th>Amount Min</th>--%>
                                    <th>Amount MinAll</th>
                                    <th>Full Name</th>
                                    <th>Status</th>
                                    <th>State</th>
                                    <th>Priority</th>
                                    <th>Telco</th>
                                    <th>Mobile</th>
                                    <th>Topup Type</th>
                                    <th>Order No</th>
                                    <th>RequestNo</th>
                                    <th>CreatedTime</th>
                                    <th>LastTime</th>
                                    <th>UserName</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server" OnItemCommand="rptList_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("TransactionID")%></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".")%></td>
                                            <td><%#Convert.ToInt64(Eval("AmountTopupSuccess")).ToString("N0").Replace(",", ".")%></td>
                                            <%--<td><%#Convert.ToInt64(Eval("AmountMin")).ToString("N0").Replace(",", ".")%></td>--%>
                                            <td><%#Convert.ToInt64(Eval("AmountMinAll")).ToString("N0").Replace(",", ".")%></td>
                                            <td><%#Eval("FullName")%></td>
                                            <td><%#StatusDetail(Convert.ToInt32(Eval("Status")))%></td>
                                            <td><%#StateDetail(Convert.ToInt32(Eval("IsConfirm")))%></td>
                                            <td><%#PriorityDetail(Convert.ToInt32(Eval("Priority")))%></td>
                                            <td><%#Eval("Telco")%></td>
                                            <td><a href="<%#SearchUrl(Eval("TransactionID").ToString())%>"><%#Eval("Mobile")%></a></td>
                                            <td><%#TopupTypeDetail(Convert.ToInt32(Eval("TopupType")))%></td>
                                            <td><%#Eval("OrderNo")%></td>
                                            <td><a href="<%#EditlUrl(Eval("TransactionID").ToString()) %>"><%#Eval("RequestNo")%></a></td>
                                            <td><%#Eval("CreatedTime")%></td>
                                            <td><%#Eval("LastTime")%></td>
                                            <td><%#Eval("UserName")%></td>
                                            <td>
                                                <a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupDelete %>?id=<%#Eval("TransactionID") %>&u=<%= HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()) %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                                |
                                                <asp:LinkButton runat="server" ID="ExportExcel" CommandName="Export" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TransactionID") %>'>Export</asp:LinkButton>
                                                <% if (txtStatus.SelectedValue == "-6")
                                                    { %>
                                                | <a href="<%= Constant.ADMIN_PATH %><%= Resources.Url.TopupReview %>?id=<%#Eval("TransactionID") %>&u=<%= HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()) %>" onclick="return confirm('Bạn đã chắc Review chưa, và có muốn mở lại đơn?')" title="Đã Review">Đã Review</a>
                                                <% } %>
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
                <!-- /.col -->
            </div>
            <!-- /.row -->
        </div>
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
</asp:Content>

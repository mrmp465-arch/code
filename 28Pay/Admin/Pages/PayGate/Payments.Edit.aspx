<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Payments.Edit.aspx.cs" Inherits="Pages_PayGate_Payments_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Dịch Vụ
            <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PaymentsList %>">Danh sách dịch vụ 
            </a></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
            <li class="active">Đối tác chi tiết</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Cập nhật dịch vụ</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Thông Tin</a></li>
                <li><a href="#sales-chart" data-toggle="tab">Đối tác</a></li>
            </ul>
            <div class="tab-content no-padding">
                <!-- Morris chart - Sales -->
                <div class="chart tab-pane active" id="revenue-chart">
                    <!-- SELECT2 EXAMPLE -->
                    <div class="box-body">
                        <div class="row">

                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtName">Tên dịch vụ *</label>
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên dịch vụ *"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtServiceCode">Mã dịch vụ *</label>
                                    <asp:TextBox ID="txtServiceCode" runat="server" CssClass="form-control" placeholder="Mã dịch vụ *"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtDescription">Chú thích</label>
                                    <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" CssClass="form-control" placeholder="Chú thích"></asp:TextBox>
                                    <small>Thông tin ghi chú nếu cần thiết</small>
                                </div>
                                <div class="form-group">
                                    <label for="txtClassName">Tên lớp thư viện * </label>
                                    <asp:TextBox ID="txtClassName" runat="server" CssClass="form-control" placeholder="Tên lớp  thư viện *"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="exampleInputFile">File thư viện *</label>
                                    <asp:FileUpload ID="fileUploadClass" runat="server"></asp:FileUpload>
                                </div>
                            </div>
                            <!-- /.col -->
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtName">Cấu hình </label>
                                    <asp:TextBox ID="txtConfig" TextMode="MultiLine" runat="server" CssClass="form-control" placeholder="Cấu hình"></asp:TextBox>
                                    <small>Thông tin cấu hình nếu có</small>
                                </div>
                                <div class="checkbox">
                                    <label for="txtName">
                                        <asp:CheckBox ID="cbxIsActive" Checked="true" runat="server"></asp:CheckBox>Kích hoạt
                                    </label>
                                </div>

                                <div class="form-group">
                                    <label for="txtName">Số lượng lỗi</label>
                                    <asp:TextBox ID="txtErrorCount" runat="server" CssClass="form-control txtErrorCount" placeholder="Số lượng lỗi"></asp:TextBox>
                                </div>



                                <div class="form-group">
                                    <label for="txtName">Thông tin giao dịch cuối</label>
                                    <asp:TextBox ID="txtLastTransactionInfo" TextMode="MultiLine" runat="server" CssClass="form-control" placeholder="Thông tin giao dịch cuối"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtName">Hẹn giờ</label>
                                    <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control txtStartTime" placeholder="Hẹn giờ"></asp:TextBox>
                                </div>
                            </div>
                            <!-- /.col -->
                        </div>
                        <!-- /.row -->
                    </div>

                    <div class="box-footer">
                        <asp:Button ID="btUpdate" runat="server" Text="CẬP NHẬT" CssClass="btn btn-info " OnClick="btUpdate_Click"></asp:Button>
                        <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
                    </div>
                </div>
                <div class="chart tab-pane" id="sales-chart">
                    <div class="box-body no-padding">
                        <table class="table table-condensed">
                            <tbody>
                                <tr>
                                    <th style="width: 10px">#</th>
                                    <th>Đối tác</th>
                                    <th>Quota</th>
                                    <th>Occurs</th>
                                    <th>CommandCode</th>
                                    <th>IP truy cập</th>
                                    <th>Trạng thái</th>
                                </tr>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td>-</td>
                                            <td><%#Eval("Name") %></td>
                                            <td>
                                                <asp:TextBox ID="txtQuota" runat="server" CssClass="form-control"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpOccurs" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="Giờ" Value="4"></asp:ListItem>
                                                    <asp:ListItem Text="Ngày" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Tuần" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Tháng" Value="3"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCommandCode" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtIPAddress" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server"></asp:CheckBox>&nbsp;
                                                <asp:Label ID="lblPartnerID" Visible="false" runat="server" Text='<%#Eval("PartnerID") %>'></asp:Label></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="btApply" runat="server" Text="CẬP NHẬT" CssClass="btn btn-info " OnClick="btApply_Click"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript">
        $(function () {
            $(function () {
                $('.txtStartTime').datetimepicker();
            });
            $(function () {
                $(".txtErrorCount").attr("type", "number");
            });
        });
    </script>
</asp:Content>

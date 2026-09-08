<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Partners.List.aspx.cs" Inherits="Pages_PayGate_Partners_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Kết nối
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Danh sách đối tác" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Đối tác</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <!-- /.box-header -->
                    <div class="box-body  no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>STT</th>
                                    <th>Đối tác</th>
                                    <th>Mã đối tác</th>
                                    <th>Thời gian tạo</th>

                                    <th>Loại chữ ký</th>
                                    <th>Kích hoạt</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersEdit %>?id=<%#Eval("PartnerID") %>&code=<%#Eval("PartnerCode") %>"><%#Eval("Name") %></a></td>
                                            <td><%#Eval("PartnerCode") %></td>
                                            <td><%#Eval("CreatedTime", "{0:dd/MM/yyyy HH:mm}")%></td>

                                            <td><%# SignatureName(Eval("SignatureType").ToString())%></td>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblPartnerID" runat="server" Visible="false" Text='<%#Eval("PartnerID") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersDelete %>?id=<%#Eval("PartnerID") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="btAdd" CssClass="btn btn-info" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>
                        <asp:Button ID="btApply" runat="server" Text="Update" CssClass="btn btn-info   pull-right" OnClick="btApply_Click"></asp:Button>
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
    <script type="text/javascript">
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

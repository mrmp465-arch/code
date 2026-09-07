<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Partners.List.aspx.cs" Debug="true" Inherits="Pages_PayGate_Partners_List" %>

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
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Nhóm</span>
                                <asp:DropDownList ID="drpGroup" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpGroup_SelectedIndexChanged">
                                    <asp:ListItem Text="Tất cả" Value="" Selected="True"></asp:ListItem>
                                   
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Hoạt động</span>
                                <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tất cả" Value="-1" ></asp:ListItem>
                                    <asp:ListItem Text="Hoạt động" Value="1" Selected="True"> </asp:ListItem>
                                    <asp:ListItem Text="Khóa" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-4">
                            <div class="input-group date-group " style="float: left; margin-right: 15px;">
                                <asp:DropDownList ID="drpYear" runat="server" CssClass="input-group-addon"></asp:DropDownList>
                                <asp:DropDownList ID="drpMonth" runat="server" CssClass="input-group-addon"></asp:DropDownList>
                                <asp:DropDownList ID="drpDay" runat="server" CssClass="input-group-addon"></asp:DropDownList>
                            </div>
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                            <style type="text/css">
                                .date-group select {
                                    width: 80px;
                                }
                            </style>
                        </div>
                    </div>
                    <div class="box-body  no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>STT</th>
                                    <th>Đối tác</th>
                                    <th>Mã đối tác</th>
                                    <th>Nhóm</th>

                                    <th>TK đối ứng</th>
                                    <th>Chiết khấu(bank|bankout|rwbank|rwbankout)</th>
                                    <th>Kích hoạt</th>
                                    <%--<th>Hoạt động</th>--%>
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
                                            <td><%#Eval("SMSUrl") %></td>
                                            <%--  <td><%#Eval("CreatedTime", "{0:dd/MM/yyyy HH:mm}")%></td>--%>

                                            <%-- <td><%# SignatureName(Eval("SignatureType").ToString())%></td>--%>
                                            <td><%#Eval("HotLine") %></td>
                                            <td><%#Eval("SMSCommand") %></td>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblPartnerID" runat="server" Visible="false" Text='<%#Eval("PartnerID") %>'></asp:Label>
                                            </td>
                                            <%-- <td><%#CheckActive(Eval("PartnerCode").ToString()) %>--%>
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
                responsive: true,
                "autoWidth": false,
                "paging": false,
                "searching": true,
                "info": true,
                "ordering": true,
            });

            new $.fn.dataTable.FixedHeader(table);

        });
    </script>
</asp:Content>

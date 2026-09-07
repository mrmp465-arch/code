<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Partners.aspx.cs" Inherits="Pages_BankEWalletService_Bank_Partners" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Quản lý kênh" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Quản lý đối tác</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Bank</li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankAccount %>">Tài khoản Bank</a></li>
                <li class="active"><a href="#" data-toggle="tab">Quản lý kênh</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerBank %>">Phân bố kênh & Tài khoản</a></li>

            </ul>

            <div class="row">
                <div class="col-xs-12">


                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Quản lý kênh" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Channel Name</span>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Chanel Code</span>
                                <asp:TextBox ID="txtPartnerCode" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>


                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Tìm"></asp:Button>
                        </div>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body  no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>Stt</th>
                                    <th>Id</th>
                                    <th>Chanel Name</th>
                                    <th>Chanel Code</th>
                                     <th>Description</th>
                                    <th>Callback</th>
                                    <th>Status</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><%#Eval("Id") %></td>
                                            <td><%#Eval("Name") %></td>
                                            <td><%#Eval("Code") %></td>
                                             <td><%#Eval("Description") %></td>
                                            <td>
                                              <asp:TextBox ID="txtCallback" runat="server" CssClass="form-control" style="width:80%;"  placeholder="Nhập callback In đến đối tác" Text='<%#Eval("InCallbackUrl") %>'></asp:TextBox>
                                            </td>

                                            <td>
                                                 <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                            </td>

                                            <td>
                                                <asp:LinkButton ID="lnDelete" runat="server" OnClientClick="return confirm('Bạn có muốn xóa?')" OnCommand="Delete_Command" CommandName="Delete" CommandArgument='<%#Eval("Id")%>'>Xóa </asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <%--<asp:Button ID="btAdd" CssClass="btn btn-info" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>--%>
                        <asp:Button ID="Button5" runat="server" Text="Update" CssClass="btn btn-info pull-right " OnClick="btApply_Click"></asp:Button>
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


<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="ITAccounts.aspx.cs" Inherits="Pages_BankEWalletService_InternalTransfer_ITAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình bank
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Quản lý tài khoản chứa" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Quản lý tài khoản chứa</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Bank</li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankAccount %>">Tài khoản Bank</a></li>

                <li class="active"><a href="#">Tài khoản chứa</a></li>

            </ul>

            <div class="row">
                <div class="col-xs-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title">Thêm mới tài khoản</h3>
                        </div>

                        <div class="box-body">
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="txtClassName">Ngân hàng</label>
                                        <asp:DropDownList ID="drpBankCode" runat="server" CssClass="form-control select2">
                                        </asp:DropDownList>
                                    </div>


                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="txtDescription">Số tài khoản*</label>
                                        <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                    </div>
                                </div>

                                <div class="col-md-3">


                                    <div class="form-group">
                                        <label for="txtDescription">Tên tài khoản *</label>
                                        <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                    </div>



                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="txtDescription">Trạng thái</label>
                                        <div class="checkbox">
                                            <label for="cbxIsActive">
                                                <asp:CheckBox ID="chkIsActive" Checked="True" runat="server"></asp:CheckBox>
                                            </label>
                                        </div>
                                    </div>
                                </div>

                                <!-- /.col -->
                            </div>
                            <!-- /.row -->
                        </div>
                        <div class="box-footer">
                            <asp:Button ID="btAdd" CssClass="btn btn-info pull-right" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>
                        </div>

                    </div>
                    <div class="box">
                        <!-- /.box-header -->
                        <div class="box-header with-border">
                            <h3 class="box-title">Danh sách tài khoản</h3>
                        </div>
                        <div class="box-body  no-padding">
                            <div style="height: 20px; clear: both;"></div>
                            <table class="table table-striped" id="TableResponsive">
                                <thead>
                                    <tr>
                                        <th>Stt</th>
                                        <th>BankCode</th>
                                        <th>Số tài khoản</th>
                                        <th>Tên tài khoản</th>
                                        <th>QR</th>
                                        <th>Status</th>
                                        <th>Tác vụ</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptList" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Container.ItemIndex + 1 %></td>

                                                <td><%#Eval("BankCode") %></td>
                                                <td><%#Eval("BankId") %></td>
                                                <td><%#Eval("BankName") %></td>
                                                <td class="lstlightbox">
                                                    <a href="<%#GetQR(Eval("BankCode").ToString(),Eval("BankId").ToString(),Eval("BankName").ToString()) %>">
                                                        <img src="/cmspay/qr.png" height="35" />
                                                    </a>
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

                            <asp:Button ID="Button5" runat="server" Text="Cập nhật" CssClass="btn btn-info pull-right " OnClick="btApply_Click"></asp:Button>
                        </div>
                    </div>
                    <!-- /.box-body -->
                    <
        <!-- /.box -->

                    <!-- /.col -->
                </div>
            </div>
        </div>
        <!-- /.row -->
    </section>
    <!-- /.content -->
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
        $(document).ready(function () {
            $(".lstlightbox a").fancybox({

            });
        });
    </script>
</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Payments.List.aspx.cs" Inherits="Pages_PayGate_Payments_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Dịch vụ
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Danh sách dịch vụ" CssClass="title"></asp:Label></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
            <li class="active">Dịch vụ</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                         <div style="height:20px; clear:both;"></div>
                        <table class="table table-striped" id="TableResponsive" >
                            <thead>
                                <tr>
                                    <th style="width:10px;">#</th>
                                    <th>Dịch vụ</th>
                                    <th>Mã dịch vụ</th>
                                    <th>Ngày tạo</th>
                                    <th>Cập nhật</th>
                                    <th>Trạng thái</th>
                                    <th>Giao dịch lỗi</th>
                                    <th>Giao dịch cuối</th>
                                    <th>Hẹn giờ</th>
                                    <th>Log</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PaymentsEdit %>?id=<%#Eval("ServiceID") %>"><%#Eval("Name") %></a></td>
                                            <td><%#Eval("ServiceCode") %></td>
                                            <td><%#Eval("CreatedTime", "{0:dd/MM/yyyy}")%></td>
                                            <td><%#Eval("UpdateTime", "{0:dd/MM HH:mm}")%></td>
                                            <td><%#Eval("Status") %></td>
                                            <td><%#Eval("ErrorCount") %></td>
                                            <td><%#Eval("LastTransactionTime", "{0:dd/MM/yyyy}")%></td>
                                            <td><%#Eval("StartTime", "{0:dd/MM HH:mm}")%></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PaymentsLog %>?id=<%#Eval("ServiceID") %>">Log</a></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="btAdd" runat="server" Text="Thêm mới" CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
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
                 ,"autoWidth": false,"paging": false, "searching": false,"info": false,"ordering": false}); 
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Payments.Log.aspx.cs" Inherits="Pages_PayGate_Payments_Log" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Dịch vụ
        <small>
             <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PaymentsList %>">
                Danh sách dịch vụ 
            </a>

        </small>
        </h1> 
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <!-- /.box-header -->
                    <div class="box-header with-border">
                      <h3 class="box-title">Log cập nhật dịch vụ</h3>
                    </div>
                    <div class="box-body no-padding">
                         <div style="height:20px; clear:both;"></div>
                        <table class="table table-striped" id="TableResponsive" >
                            <thead>
                                <tr>
                                    <th>Dịch vụ</th>
                                    <th>LogType</th>
                                    <th>DataSize</th>
                                    <th>Chú thích</th>
                                    <th>Người cập nhật</th>
                                    <th>Thời gian</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PaymentsLog %>?id=<%#Eval("ServiceID") %>"><%#Eval("ServiceID") %></a></td>
                                            <td><%#Eval("LogType") %></td>
                                            <td><%#Eval("DataSize") %></td>
                                            <td><%#Eval("Description") %></td>
                                            <td><%#Eval("UserName") %></td>
                                            <td><%#Eval("LogTime", "{0:dd/MM/yy HH:mm}")%></td>
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


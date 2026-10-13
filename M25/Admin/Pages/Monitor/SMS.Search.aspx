<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="SMS.Search.aspx.cs" Inherits="Pages_Monitor_SMS_Search" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>SMS & SMSPlus
            <small>
                <asp:Label ID="lblTtitle" runat="server" Text="Tra cứu giao dịch nạp thẻ" CssClass="title"></asp:Label>
            </small>
        </h1>
        <ol class="breadcrumb">
                <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
                <li class="active">Tra cứu</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Tra cứu giao dịch nạp thẻ" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpTop" runat="server" CssClass="form-control">
                                <asp:ListItem Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                                <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">ReceiverNumber</span>
                                <asp:TextBox ID="txtSubject" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">SenderNumber</span>
                                <asp:TextBox ID="txtSenderNumber" Text="" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Time</span>
                                <asp:TextBox ID="txtCreatTime" runat="server" CssClass="form-control txtCreatTime"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                             <thead>
                                <tr>
                                    <th># Id</th>
                                    <th>Partner</th>
                                    <th>ReceiverNumber</th>
                                    <th>SenderNumber</th> 
                                    <th>Content</th>
                                    <th>Amount</th>
                                    <% if (AppUtils.IsAdmin){%>
                                        <th>Provider</th>
                                    <%}%>
                                    <th>CreatedTime</th>
                                    <th>Status</th>
                                    <th>Description</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Id")%></td>
                                            <td><%#Eval("PartnerCode")%></td>
                                            <td><%#Eval("ReceiverNumber")%></td> 
                                            <td><%#Eval("SenderNumber")%></td> 
                                            <td><%#Eval("Content")%></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".") %></td>
                                          
                                            <% if (AppUtils.IsAdmin)
                                               {%>
                                                <td><%#Eval("Provider")%></td>
                                            <%}%>
                                            <td><%#Eval("CreatedTime")%></td>
                                            <td><%#Eval("Status")%></td>
                                            <td><a href="<%#DetailUrl(Eval("Id").ToString()) %>">xem</a></td>
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
            var table = $('#TableResponsive').DataTable({
                responsive: true
             , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
</asp:Content>


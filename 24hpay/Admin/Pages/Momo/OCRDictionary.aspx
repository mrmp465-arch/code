<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="OCRDictionary.aspx.cs" Inherits="Pages_Momo_OCRDictionary" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Quản lý OCRDictionary các trường hợp nhận dạng sai" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Xem log</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Danh sách log giao dịch" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">

                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpTop" runat="server" CssClass="form-control select2">
                                <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                                <asp:ListItem Text="100 bản ghi" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 bản ghi" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">

                            <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control select2">
                                <asp:ListItem Text="Trạng Thái:" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="Đã duyệt" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Chưa duyệt" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Source</span>
                                <asp:TextBox ID="txtSource" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Destination</span>
                                <asp:TextBox ID="txtDestination" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
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
                                    <th>Source</th>
                                    <th>Destination</th>
                                    <th>Số lần hỗ trợ</th>
                                    <th>LastTime</th>
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
                                            <td><%#Eval("Source") %></td>
                                            <td><%#Eval("Destination") %></td>
                                            <td><%#Eval("Count") %></td>
                                            <td><%#Eval("UpdateTime", "{0:dd/MM HH:mm:ss}")%></td>
                                            <td><%#GetStatus(Eval("Status")) %></td>
                                            <td>
                                                <%--<asp:HyperLink NavigateUrl='<%#DeleteTranInUrl(Eval("Id").ToString()) %>' runat="server"> Xóa </asp:HyperLink>--%>
                                                <div class="btn-group">
                                                    <button type="button" class="btn btn-default btn-flat btn-xs">Tác vụ</button>
                                                    <button type="button" class="btn btn-default btn-flat btn-xs dropdown-toggle" data-toggle="dropdown">
                                                        <span class="caret"></span>
                                                        <span class="sr-only">Toggle Dropdown</span>
                                                    </button>
                                                    <ul class="dropdown-menu" role="menu">
                                                        <li>
                                                            <asp:LinkButton ID="btnDel" runat="server" OnClientClick="return ConfirmDelete(this)" data-id='<%# Eval("Id")%>'>Loại bỏ</asp:LinkButton></li>
                                                        <li>
                                                            <asp:LinkButton ID="btnAccept" runat="server" OnClick="btnAccept_Click" CommandArgument='<%# Eval("Id")%>'>Duyệt thay ORC</asp:LinkButton></li>
                                                    </ul>
                                                </div>
                                            </td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:HiddenField ID="myHiddenField" runat="server" ClientIDMode="Static" />
                        <asp:Button ID="btAdd" CssClass="btn btn-info" runat="server" Text="Thêm mới"></asp:Button>
                    </div>
                    <!-- /.box-body -->
                </div>
                <!-- /.box -->

                <!-- /.col -->
            </div>
        </div>
        <!-- /.row -->
        <div class="modal fade" id="modal-default">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Cảnh báo !</h4>
                    </div>
                    <div class="modal-body">
                        <p>Bạn có chắc chắn muốn loại bỏ từ khóa này với ID (<strong><span id="currentItem"></span></strong>) ?</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default pull-left" data-dismiss="modal">Đóng</button>
                        <asp:Button  ID="btnDelete" runat="server" OnClick="btnDel_Click" class="btn btn-primary" Text="Đồng ý"></asp:Button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
    </section>
    <!-- /.content -->
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2({
                minimumResultsForSearch: -1
            });
        })

        function ConfirmDelete(cnt) {
            var doc = document.getElementById("<%=myHiddenField.ClientID%>");            
            doc.value = cnt.getAttribute("data-id");
            var itemRef = document.getElementById("currentItem");
            itemRef.innerHTML = doc.value
            $('#modal-default').modal(); // initialized with defaults
            return false;
        }

    </script>
</asp:Content>

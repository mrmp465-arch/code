<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Partner.Add.aspx.cs" Inherits="Pages_Momo_Partner_Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Thêm mới kênh" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Thêm mới đối tác</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>MOMO</li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoAccount %>">Tài khoản momo</a></li>
                <li class="active"><a href="#" data-toggle="tab">Quản lý kênh</a></li>
                 <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerMomo %>">Phân bố kênh & tài khoản</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">
                   
                            <div class="box-header with-border">
                                <h3 class="box-title">
                                    <asp:Label runat="server" Text="Quản lý kênh" CssClass="title"></asp:Label></h3>
                            </div>
                            <div class="box-tools" style="margin-top: 10px;">


                                <div class="col-xs-12 col-sm-12 col-md-6">
                                    <div class="form-group">
                                        <label for="txtName">Chọn kênh</label>
                                        <asp:DropDownList ID="drpPartner" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpPartner_SelectedIndexChanged" CssClass="form-control drpTop">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtName">Channel Name</label>
                                        <asp:TextBox ID="txtPartnerName" runat="server" CssClass="form-control" placeholder=" " ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtName">Channel Code</label>
                                        <asp:TextBox ID="txtPartnerCode" runat="server" CssClass="form-control" placeholder=" " ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtName">CallbackUrl</label>
                                        <asp:TextBox ID="txtCallbackUrl" runat="server" CssClass="form-control" placeholder=" "></asp:TextBox>
                                    </div>
                                </div>


                            </div>
                            <div style="clear:both"></div>
                            
                           

                            <div class="box-footer">
                                <asp:Button ID="btAdd" CssClass="btn btn-info" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>
                                <asp:Button ID="btCancel" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
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

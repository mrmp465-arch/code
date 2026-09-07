<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PartnerMomo.aspx.cs" MasterPageFile="~/Layout/Layout.master" Inherits="Pages_Momo_PartnerMomo" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Phân bổ kênh &tài khoản" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Phân bổ kênh &tài khoản</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>MOMO</li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoAccount %>">Tài khoản momo</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.MomoPartner %>">Quản lý kênh</a></li>
                <li class="active"><a href="#" data-toggle="tab">Phân bổ kênh & Tài khoản</a></li>

            </ul>

            <div class="row">
                <div class="col-xs-12">

                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Phân bổ kênh & tài khoản" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">


                        <%-- <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Partner Code</span>
                                <asp:TextBox ID="txtPartnerCode" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>--%>

                        <div class="col-xs-12 col-sm-12 col-md-2">
                            <div class="form-group">
                                <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2" OnSelectedIndexChanged="drpPartner_SelectedIndexChanged" AutoPostBack="True">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Momo Account</span>
                                <asp:TextBox ID="txtMomoId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                             <asp:Button ID="btAdd" CssClass="btn btn-primary" runat="server" OnClick="btAdd_Click" Text="Thêm mới"></asp:Button>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-3" style="font-weight:bold">
                             <asp:Label runat="server" ID="lblAcountInfo" Visible="false"></asp:Label>
                        </div>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body  no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>STT</th>
                                    <th>ID</th>
                                    <th>P.Name</th>
                                    <th>P.Code</th>
                                    <th>M.Mobile</th>
                                    <th>M.Name</th>
                                    <th>D.In</th>
                                    <th>M.In</th>
                                    <th>D.Out</th>
                                    <th>M.Out</th>
                                    <th>Balance</th>
                                    <th>Type</th>
                                    <th>In</th>
                                    <th>Out</th>
                                    <th>T.Thái</th>
                                    <th>K.Hoạt</th>
                                    <th>OrderNo</th>
                                    <th>G.Pháp</th>
                                    <th>T.Vụ</th>
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
                                            <td><%#Eval("MomoMobile") %></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.MomoAccountEdit %>?id=<%#Eval("MomoId") %>"><%#Eval("MomoName") %></a></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceDayIn")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceMonthIn")).ToString("N0")%></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceDayOut")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceMonthOut")).ToString("N0") %></td>
                                            <td><%#Convert.ToInt64(Eval("BalanceTotal")).ToString("N0") %></td>
                                            <td><%#Eval("Type") %></td>
                                            <td><%#GetStatus(Eval("StatusOver"))%></td>
                                            <td><%#GetStatus(Eval("StatusOverOut")) %></td>
                                            <td><%#GetStatusExtra(Eval("StatusExtra")) %></td>
                                            <td>
                                                 
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtOrderNo" placeholder="Nhập thứ tự ưu tiên" runat="server" CssClass="form-control txtOrderNo" Text='<%#Eval("OrderNo") %>'></asp:TextBox>
                                            </td>
                                            <td><%#Eval("Solution") %></td>
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
                       
                        <asp:Button ID="Button5" runat="server" Text="Update" CssClass="btn btn-info pull-right " OnClick="btApply_Click"></asp:Button>
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
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": true
            });
            new $.fn.dataTable.FixedHeader(table);
        });

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2({
                //minimumResultsForSearch: -1
            });

        })

    </script>
</asp:Content>

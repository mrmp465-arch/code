<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Layout/Layout.master"  CodeFile="PartnerBank.Add.aspx.cs" Inherits="Pages_BankEWalletService_Bank__Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
      <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger bg-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>

    <!-- Content Header (Page header) -->
   <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Phân bổ đối tác & tài khoản" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Phân bổ đối tác &tài khoản</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Bank</li>
               
              
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankAccount %>">Tài khoản Bank</a></li>
                 <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankPartner %>">Quản lý kênh</a></li>
                <li  class="active" ><a href="#"  data-toggle="tab">Phân bổ kênh &tài khoản</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">
                   
                            <div class="box-header with-border">
                                <h3 class="box-title">
                                    <asp:Label runat="server" Text="Phân bổ kênh &tài khoản" CssClass="title"></asp:Label></h3>
                            </div>
                            <div class="box-tools" style="margin-top: 10px;">


                                <div class="col-xs-12 col-sm-12 col-md-5">
                                    <div class="form-group">
                                        <label for="txtName">Chọn kênh *</label>
                                        <asp:DropDownList ID="drpPartner" runat="server"  CssClass="form-control drpTop">
                                        </asp:DropDownList>
                                    </div>
                                   
                                </div>


                            </div>
                            <div style="clear:both"></div>
                            <div class="box-tools" style="margin-top: 10px;">


                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="input-group">
                                        <span class="input-group-addon" style="padding: 5px;">Tên</span>
                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="input-group">
                                        <span class="input-group-addon" style="padding: 5px;">Bank Id</span>
                                        <asp:TextBox ID="txtBankId" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-6 col-md-1">
                                    <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Tìm Account"></asp:Button>
                                </div>
                            </div>
                            <!-- /.box-header -->
                            <div class="box-body  no-padding">
                                <div style="height: 20px; clear: both;"></div>
                                <table class="table table-striped" id="TableResponsive">
                                    <thead>
                                        <tr>
                                            <th>STT</th>
                                            <th>Id</th>
                                            <th>BankCode</th>
                                            <th>Name</th>
                                            <th>BankId</th>
                                            <th>BalanceDayIn</th>
                                            <th>BalanceMonthIn</th>
                                            <th>BalanceDayOut</th>
                                            <th>BalanceMonthOut</th>
                                            <th>BalanceTotal</th>
                                            <th>Type</th>
                                            <th>Chọn</th>

                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptList" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Container.ItemIndex + 1 %></td>
                                                    <td><%#Eval("Id") %></td>
                                                    <td><%#Eval("BankCode") %></td>
                                                    <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankAccountEdit %>?id=<%#Eval("Id") %>" ><%#Eval("BankName") %></a></td>
                                                    <td><%#Eval("BankId") %></td>
                                                    <td><%#Convert.ToInt64(Eval("BalanceDayIn")).ToString("N0").Replace(",", ".") %></td>
                                                    <td><%#Convert.ToInt64(Eval("BalanceMonthIn")).ToString("N0").Replace(",", ".") %></td>
                                                    <td><%#Convert.ToInt64(Eval("BalanceDayOut")).ToString("N0").Replace(",", ".") %></td>
                                                    <td><%#Convert.ToInt64(Eval("BalanceMonthOut")).ToString("N0").Replace(",", ".") %></td>
                                                    <td><%#Convert.ToInt64(Eval("BalanceTotal")).ToString("N0").Replace(",", ".") %></td>
                                                    <td><%#Eval("Type") %></td>
                                                    <td>
                                                        <asp:CheckBox ID="cbxStatus" runat="server"></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                                    </td>

                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>

                            <div class="box-footer">
                                <asp:Button ID="btAdd" CssClass="btn btn-info" runat="server" OnClick="btAdd_Click" Text="Thêm"></asp:Button>
                                <asp:Button ID="btCancel" runat="server" Text="Hủy" CausesValidation="false" CssClass="btn btn-default pull-right" OnClick="btCancel_Click"></asp:Button>
                            </div>
                            <!-- /.box-body -->
                      <!-- /.col -->
                </div>
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

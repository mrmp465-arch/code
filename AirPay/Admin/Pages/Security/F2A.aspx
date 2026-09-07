<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" MaintainScrollPositionOnPostback="true" CodeFile="F2A.aspx.cs" Inherits="Pages_Security_F2A" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertBans"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>

    <div class="modal  fade bs-example-modal-sm" id="MAlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1><%= Resources.Pay.Account%>
            <small><%= Resources.Pay._2FASetup%>
            </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>
                    Cài đặt
                </li>

                <li class="active"><a href="#revenue-chart" data-toggle="tab">Bảo mật 2 lớp</a></li>
                <% if (AppUtils.IsPartner)
                    {%>
                <li><a href="#bank" data-toggle="tab">Tài khoản rút tiền</a></li>
                <%}%>
            </ul>
        </div>
        <div class="tab-content no-padding">
            <div class="chart tab-pane active" id="revenue-chart">
                <div class="row">
                    <div class="col-xs-12">
                        <div class="box box-primary">
                            <div class="box-header with-border">
                                <h3 class="box-title"><%= Resources.Pay._2FASetup%></h3>
                            </div>
                            <div class="box-body">
                                <div class="row" id="dvFA" runat="server" visible="false">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <div style="text-align: center">
                                                <img src="<%=BarcodeImageUrl %>" width="200" alt="" />
                                            </div>

                                        </div>
                                        <div class="form-group">
                                            <label for="txtName"><%= Resources.Pay._2FACode%> </label>
                                            <asp:TextBox ID="txtOTP" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                        </div>


                                    </div>
                                    <!-- /.col -->
                                    <div class="col-md-6">
                                        <h2><%= Resources.Pay.Help%></h2>
                                        1. <%= Resources.Pay.Help1%><br />
                                        2. <%= Resources.Pay.Help2%>
                                        <br />
                                        3. <%= Resources.Pay.Help3%><br />
                                        4. <%= Resources.Pay.Help4%>
                                    </div>
                                    <!-- /.col -->
                                </div>
                                <!-- /.row -->
                            </div>
                            <div class="box-footer">
                                <asp:Button ID="btSubmit" runat="server" Text="Xác nhận" CssClass="btn btn-info " OnClick="btSubmit_Click"></asp:Button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="chart tab-pane " id="bank">
                <asp:Panel ID="PanelContent" runat="server">
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
                                                    <asp:ListItem Value="">Chọn bank</asp:ListItem>
                                                    <asp:ListItem Value="trc20">usdt-trc20</asp:ListItem>
                                                    <asp:ListItem Value="bep20">usdt-bep20</asp:ListItem>
                                                    <asp:ListItem Value="erc20">usdt-erc20</asp:ListItem>
                                                    <asp:ListItem Value="VietcomBank">(VCB) - NH TMCP Ngoai Thuong VN (VietcomBank)</asp:ListItem>
                                                    <asp:ListItem Value="Vietinbank">(VTB) - NH TMCP Cong Thuong VN (Vietinbank)</asp:ListItem>
                                                    <asp:ListItem Value="Techcombank">(TCB) - NH TMCP Ky Thuong VN (Techcombank)</asp:ListItem>
                                                    <asp:ListItem Value="BIDV">(BIDV) - NH TMCP Dau Tu va Phat Trien VN (BIDV)</asp:ListItem>
                                                    <asp:ListItem Value="AgriBank">(AGR) - NH Nông nghiệp và Phát triển Nông thôn (AgriBank)</asp:ListItem>
                                                    <asp:ListItem Value="Sacombank">(STB) - NH TMCP Sai Gon Thuong Tin (Sacombank)</asp:ListItem>
                                                    <asp:ListItem Value="ACB">(ACB) - NH TMCP A Chau (ACB)</asp:ListItem>
                                                    <asp:ListItem Value="MB">(MBB) - NH TMCP Quan Doi (MB)</asp:ListItem>
                                                    <asp:ListItem Value="TPBank">(TPB) - NH TMCP Tien Phong (TPBank)</asp:ListItem>
                                                    <asp:ListItem Value="ShinhanBank">(SHIB) - NH TNHH MTV Shinhan VN (ShinhanBank)</asp:ListItem>
                                                    <asp:ListItem Value="VIB">(VIB) - NH TMCP Quoc Te VN (VIB)</asp:ListItem>
                                                    <asp:ListItem Value="VP Bank">(VPB) - NH TMCP Viet Nam Thinh Vuong (VP Bank)</asp:ListItem>
                                                    <asp:ListItem Value="SHB">(SHB) - NH TMCP Sai Gon Ha Noi (SHB)</asp:ListItem>
                                                    <asp:ListItem Value="OCB">(OCB) - NH TMCP Phuong Dong (OCB)</asp:ListItem>
                                                    <asp:ListItem Value="Eximbank">(EIB) - NH TMCP Xuat Nhap khau VN (Eximbank)</asp:ListItem>
                                                    <asp:ListItem Value="BaoVietBank">(BVB) - NH TMCP Bao Viet (BaoVietBank)</asp:ListItem>
                                                    <asp:ListItem Value="VCCB">(VCCB) - NH TMCP Ban Viet (Viet Capital Bank)</asp:ListItem>
                                                    <asp:ListItem Value="SCB">(SCB) - NH TMCP Sai Gon (SCB)</asp:ListItem>
                                                    <asp:ListItem Value="VRB">(VRB) - NH Lien Doanh Viet Nga (VRB)</asp:ListItem>
                                                    <asp:ListItem Value="ABB">(ABB) - NH TMCP An Binh (ABBank)</asp:ListItem>
                                                    <asp:ListItem Value="PVcombank">(PVB) - NH TMCP Dai Chung VN (PVcombank)</asp:ListItem>
                                                    <asp:ListItem Value="OceanBank">(OJB) - NH TM TNHH MTV Dai Duong (OceanBank)</asp:ListItem>
                                                    <asp:ListItem Value="NamABank">(NAB) - NH TMCP Nam A (NamABank)</asp:ListItem>
                                                    <asp:ListItem Value="HDB">(HDB) - NH TMCP Phat Trien TP HCM (HDBank)</asp:ListItem>
                                                    <asp:ListItem Value="VietBank">(VB) - NH TMCP Viet Nam Thuong Tin (VietBank)</asp:ListItem>
                                                    <asp:ListItem Value="PGBank">(PGB) - NH TMCP Xang Dau Petrolimex (PG Bank)</asp:ListItem>
                                                    <asp:ListItem Value="COB">(COB) - NH Hop Tac (Co op Bank)</asp:ListItem>
                                                    <asp:ListItem Value="NCB">(NCB) - NH TMCP Quoc Dan (NCB)</asp:ListItem>
                                                    <asp:ListItem Value="IDB">(IDB) - NH TNHH Indovina (Indovina Bank)</asp:ListItem>
                                                    <asp:ListItem Value="VIKKI">Vikki Digital Bank</asp:ListItem>
                                                    <asp:ListItem Value="GPBank">(GPB) - NH TM TNHH MTV Dau Khi Toan Cau (GPBank)</asp:ListItem>
                                                    <asp:ListItem Value="BacABank">(BAB) - NH TMCP Bac A (BacABank)</asp:ListItem>
                                                    <asp:ListItem Value="VietABank">(VAB) - NH TMCP Viet A (VietABank)</asp:ListItem>
                                                    <asp:ListItem Value="Saigonbank">(SGB) - NH TMCP Sai Gon Cong Thuong (Saigonbank)</asp:ListItem>
                                                    <asp:ListItem Value="MSB">(MSB) - NH TMCP Hang Hai VN (Maritime Bank)</asp:ListItem>
                                                    <asp:ListItem Value="LPBank">LPBank - Ngan Hang Phat Loc Viet Nam</asp:ListItem>
                                                    <asp:ListItem Value="KienLongBank">(KLB) - NH TMCP Kien Long (KienLongBank)</asp:ListItem>
                                                    <asp:ListItem Value="Wooribank">(WRB) - NH Wooribank</asp:ListItem>
                                                    <asp:ListItem Value="SeABank">(SAB) - NH TMCP Dong Nam A(SeABank)</asp:ListItem>
                                                    <asp:ListItem Value="Standard">(SC) - Standard Chartered</asp:ListItem>
                                                    <asp:ListItem Value="Liobank">Lio Bank</asp:ListItem>
                                                    <asp:ListItem Value="Cake">Cake</asp:ListItem>
                                                    <asp:ListItem Value="Ubank">Ubank</asp:ListItem>
                                                    <asp:ListItem Value="Citibank">Citibank</asp:ListItem>
                                                    <asp:ListItem Value="Timo">Timo</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>


                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <label for="txtDescription">Số tài khoản/ví *</label>
                                                <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-md-3">


                                            <div class="form-group">
                                                <label for="txtDescription">Tên tài khoản </label>
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
                                                <%-- <th>Stt</th>--%>
                                                <th>BankCode</th>
                                                <th>Số tài khoản</th>
                                                <th>Tên tài khoản</th>
                                                <th>Thứ tự</th>
                                                <%-- <th>IP</th>--%>
                                                <th>Time</th>
                                                <th>Status</th>
                                                <th>Tác vụ</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="rptList" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <%-- <td><%# Container.ItemIndex + 1 %></td>--%>

                                                        <td><%#Eval("BankCode") %></td>
                                                        <td><%#Eval("AccountNumber") %></td>
                                                        <td><%#Eval("AccountName") %></td>
                                                        <td>
                                                            <asp:TextBox ID="txtOrderNo" placeholder="Nhập thứ tự " Width="50" runat="server" CssClass="form-control txtOrderNo" Text='<%# DataBinder.Eval(Container.DataItem, "Number") %>'></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                     <asp:Label ID="lblId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                                        </td>
                                                        <%-- <td><%#Eval("IP") %></td>--%>
                                                        <td><%#GetDate(Eval("Time")) %></td>

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

                            <!-- /.box -->

                            <!-- /.col -->
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>

        <asp:HiddenField ID="hdCurrentTab" runat="server" />
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->


    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script type="text/javascript">


        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
        $(function () {

            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                $('#<%= hdCurrentTab.ClientID %>').val($(e.target).attr('href'));
            });

            var currentTab = $('#<%= hdCurrentTab.ClientID %>').val();
            if (currentTab) {
                $('a[href="' + currentTab + '"]').tab('show');
            }

        });
    </script>
    <style>
        .select2-container {
            display: block;
            width: 300px !important;
        }
    </style>
</asp:Content>

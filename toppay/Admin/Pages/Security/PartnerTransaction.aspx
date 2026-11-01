<%@ Page Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="PartnerTransaction.aspx.cs" Inherits="Pages_Security_PartnerTransaction" %>

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
                <asp:Label runat="server" ID="AlertInfoss"></asp:Label>
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
        <h1><%= Resources.Pay.BalanceFluctuation%>
            <small><%= Resources.Pay.WithdrawalHistory%></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active"><%= Resources.Pay.WithdrawalHistory%></li>
        </ol>
    </section>
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <% if (!AppUtils.IsAdmin)
                    { %>
                <div class="box" id="dvRut" runat="server">
                    <div class="box-header with-border">
                        <h3 class="box-title"><%= Resources.Pay.CreateWithdrawalOrder%></h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">

                        <div class="col-xs-12 col-sm-6 col-md-4" id="dvUserInfo" runat="server" visible="false" style="font-size: 110%; font-weight: bold">
                            <asp:Label ID="lblTotal" runat="server" CssClass="title">

                            </asp:Label>
                            &nbsp;<a href="<%=UrlHistory %>" target="_blank"><%= Resources.Pay.History%></a>
                        </div>

                    </div>
                    <div style="clear: both"></div>



                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-6" runat="server" id="divAccount">
                                <div class="form-group">
                                    <label for="txtClassName"><%= Resources.Pay.ChoseAccount%> </label>
                                    <asp:DropDownList ID="drpBankCode2" runat="server" CssClass="form-control select2">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div style="clear: both"></div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtClassName"><%= Resources.Pay.BankCode%></label>
                                    <asp:DropDownList ID="drpBankCode" runat="server" CssClass="form-control select2">
                                        <asp:ListItem Value="Bank"></asp:ListItem>
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
                                <div class="form-group">
                                    <label for="txtDescription"><%= Resources.Pay.AccountName%>*</label>
                                    <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                </div>

                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtDescription"><%= Resources.Pay.AccountNumber%>*</label>
                                    <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                </div>

                                <div class="form-group">
                                    <label for="txtName"><%= Resources.Pay.Money%> *</label>
                                    <div class="row">
                                        <div class="col-md-4">
                                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                        </div>
                                        <label id="textMoneyVND" class="control-label col-md-4" style="margin-top: 8px">
                                            &nbsp;
                                        </label>
                                    </div>

                                </div>



                            </div>

                            <!-- /.col -->
                        </div>
                        <!-- /.row -->
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="btCreate" runat="server" CssClass="btn btn-primary pull-right" OnClientClick="return confirm('Are you sure?');" OnClick="btAdd_Click" Text="Tạo lệnh rút tiền"></asp:Button>

                    </div>
                </div>
                <% } %>

                <% if (AppUtils.IsAdmin)
                    { %>
                <div class="box" id="dvRutAdmin" runat="server">
                    <div class="box-header with-border">
                        <h3 class="box-title">Tạo lệnh rút tiền</h3>
                    </div>

                    <div style="clear: both"></div>



                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtName">Tài khoản</label>


                                    <asp:DropDownList ID="ddlAccount" runat="server" CssClass="form-control select2">
                                    </asp:DropDownList>




                                </div>
                                <div class="form-group">
                                    <label for="txtName">Loại giao dịch</label>


                                    <asp:DropDownList CssClass="form-control" ID="drlType2" runat="server">
                                        <asp:ListItem Text="-Loại giao dịch--" Value="0" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="VND" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Usdt" Value="2"></asp:ListItem>

                                    </asp:DropDownList>




                                </div>
                                <div class="form-group">

                                    <label for="txtName">Số tiền (VND)</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtAmount2" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                        </div>
                                        <label id="textMoneyVND2" class="control-label col-md-3" style="margin-top: 8px">
                                            &nbsp;
                                        </label>

                                    </div>

                                </div>



                            </div>
                            <div class="col-md-6">
                                <div class="form-group">

                                    <label for="txtName">Số Usdt</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtUsdt" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                        </div>


                                    </div>

                                </div>
                                <div class="form-group" style="display: none">

                                    <label for="txtName">Tỉ giá đầu vào</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtRateIn" runat="server" Text="0" CssClass="form-control" placeholder="Nhập tỉ giá ví dụ 26500"></asp:TextBox>
                                        </div>


                                    </div>

                                </div>
                                <div class="form-group" style="display: none">

                                    <label for="txtName">Tỉ giá đầu ra</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtRate" runat="server" Text="0" CssClass="form-control" placeholder="Nhập tỉ giá ví dụ 26600"></asp:TextBox>
                                        </div>


                                    </div>
                                </div>

                                <div class="form-group" style="display: none">



                                    <label for="txtName">Lợi nhuận</label>

                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtFee" Text="0" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                        </div>

                                        <label id="textMoneyVND3" class="control-label col-md-3" style="margin-top: 8px">
                                            &nbsp;
                                        </label>
                                        <div class="col-md-3" style="font-weight: bold">
                                            <a href="javascript:;" onclick="tinhloinhuan()">Tính lợi nhuận </a>
                                        </div>

                                    </div>
                                </div>
                                <div class="form-group">

                                    <label for="txtName">Ghi chú </label>


                                    <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                </div>
                            </div>





                        </div>
                        <div style="padding-left: 0; font-weight: bold">
                            <asp:Label runat="server" ID="lbReport2"></asp:Label>
                        </div>

                        <div style="padding-right: 15px">
                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-primary pull-right" OnClientClick="return confirm('Are you sure?');" OnClick="btAdd_Click2" Text="Tạo lệnh rút tiền"></asp:Button>

                        </div>



                        <!-- /.col -->
                    </div>
                    <!-- /.row -->

                    <div class="box-footer">
                    </div>
                </div>
                <% } %>


                <% if (AppUtils.UserName == "admin")
                    { %>
                <div class="box" style="display: none">
                    <div class="box-header with-border">
                        <h3 class="box-title">Tạo lệnh QR</h3>
                    </div>

                    <div style="clear: both"></div>



                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-7">

                                <label for="txtName" class="col-md-3" style="margin-top: 8px; padding: 0 5px">Nội dung</label>

                                <div class="col-md-9">
                                    <asp:TextBox runat="server" ID="MyBox" CssClass="form-control" TextMode="MultiLine" Rows="10" />

                                </div>
                            </div>
                            <div class="col-md-5">

                                <label for="txtName" class="col-md-3" style="margin-top: 8px">QR</label>

                                <div class="col-md-9">
                                    <asp:Image ID="imgqr" runat="server" Visible="false" Height="200" />
                                </div>





                            </div>
                            <div style="clear: both; height: 10px"></div>
                            <div class="col-md-12">
                                <asp:Button ID="Button2" runat="server" CssClass="btn btn-primary pull-right" OnClick="btAdd_QR" Text="Tạo QR"></asp:Button>

                            </div>


                            <!-- /.col -->
                        </div>
                        <!-- /.row -->
                    </div>
                    <div class="box-footer">
                    </div>
                </div>
                <% } %>
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title"><%= Resources.Pay.ListWithdrawalOrders%></h3>
                    </div>

                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList CssClass="form-control" ID="drpTop" runat="server">
                                <asp:ListItem Text="50 row" Value="50" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="100 row" Value="100"></asp:ListItem>
                                <asp:ListItem Text="500 row" Value="500"></asp:ListItem>
                                <asp:ListItem Text="1000 row" Value="1000"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <% if (AppUtils.IsAdmin)
                            { %>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control" ID="drpType" runat="server">
                                <asp:ListItem Text="-Loại--" Value="0" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="VND" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Usdt" Value="2"></asp:ListItem>

                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <div class="col-xs-12 col-sm-6 col-md-2">

                            <asp:DropDownList CssClass="form-control" ID="drpStatus" runat="server">
                                <%--  <asp:ListItem Text="Trạng thái" Value="-99" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Hoàn thành" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Chờ duyệt" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Hủy" Value="-1"></asp:ListItem>--%>
                            </asp:DropDownList>

                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px"><%= Resources.Pay.From%></span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtFromDate" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 2px"><%= Resources.Pay.To%></span>
                                <asp:TextBox CssClass="form-control txtCreatTime" ID="txtCreatTime" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6" style="font-weight: bold; font-size: 105%; padding-top: 6px; float: right">
                            <asp:Label runat="server" ID="lblTotalReport"></asp:Label>
                        </div>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>#ID</th>

                                    <% if (AppUtils.IsAdmin)
                                        { %>

                                    <th>Tài khoản</th>
                                    <% } %>
                                    <th><%= Resources.Pay.Money%></th>
                                    <th><%= Resources.Pay.TransferInfo%></th>

                                    <th><%= Resources.Pay.Creationtime%></th>
                                    <th><%= Resources.Pay.Completiontime%> </th>


                                    <th><%= Resources.Pay.Status%></th>
                                    <% if (AppUtils.IsAdmin)
                                        { %>

                                    <th>QR</th>
                                    <th>Loại</th>
                                    <th>Usdt</th>



                                    <th>Người duyệt</th>
                                    <th>Duyệt</th>
                                    <% } %>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("Id")%></td>
                                            <% if (AppUtils.IsAdmin)
                                                { %>
                                            <td><%#Eval("UserName")%></td>
                                            <% } %>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(".", ",") %></td>
                                            <td><%# Eval("Note") %></td>

                                            <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}") %>  </td>
                                            <td><%#Eval("UpdateTime", "{0:dd/MM HH:mm:ss}") %>  </td>




                                            <td><%# GetStatus(Eval("Status")) %></td>
                                            <% if (AppUtils.IsAdmin)
                                                { %>
                                            <td class="lstlightbox">
                                                <asp:Panel ID="pnNote" runat="server" Visible='<%# Eval("BankInfo").ToString() != "" && Eval("Status").ToString() == "0" %>'>

                                                    <a href="<%#Eval("BankInfo").ToString() %>">
                                                        <img src="/cmspay/qr.png" height="35" />
                                                    </a>
                                                </asp:Panel>
                                            </td>
                                            <td><%# GetType(Eval("Type")) %></td>
                                            <td><%#Convert.ToInt64(Eval("Usdt")).ToString("N0").Replace(".", ",") %></td>



                                            <td><%# Eval("admin") %></td>
                                            <td>
                                                <% if (RoleApp)
                                                { %>
                                                <asp:LinkButton ID="lnkUpdate" CausesValidation="false" ToolTip="Duyệt" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("Status").ToString()=="0"%>'
                                                    CommandName="update" OnClientClick="return confirm('Bạn chắc chắn muốn thực hiện?'); "
                                                    runat="server" OnCommand="Update_Command">Duyệt</asp:LinkButton>
                                                &nbsp; |&nbsp;
                                                 <asp:HyperLink NavigateUrl='<%# FixtUrl(Eval("Id").ToString()) %>' Visible='<%# Eval("Status").ToString() == "5" %>' runat="server"> Cập nhật </asp:HyperLink>
                                                &nbsp; |&nbsp;
                                                <asp:LinkButton ID="lnkDelete" CausesValidation="false" ToolTip="Hủy" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("Status").ToString()=="0"%>'
                                                    CommandName="delete" OnClientClick="return confirm('Bạn chắc chắn muốn thực hiện?'); "
                                                    runat="server" OnCommand="Delete_Command">Hủy</asp:LinkButton>


                                                <% } %>

                                            </td>
                                            <% } %>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>

    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />
    <script type="text/javascript">

        $(document).ready(function () {
            $(".lstlightbox a").fancybox({

            });
        });
        $(document).ready(function () {
            $('#<%=txtFromDate.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });
            $('#<%=txtCreatTime.ClientID %>').datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss'
            });

        });

        $(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true, "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });

        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>
    <style>
        .label {
            width: 76px !important;
            padding: 3px;
            display: block
        }

        .txtCreatTime {
            width: 138px !important;
            padding: 3px !important;
        }
    </style>
    <script type="text/javascript">


        function formatPrice(price) {
            var fixedToSix = (Math.round(price * 1000000) / 1000000);
            return (Math.round(fixedToSix) == fixedToSix + 0.000001 ? fixedToSix + 0.000001 : fixedToSix);
        }
        Number.prototype.formatMoney = function (c, d, t) {
            var n = this,
                c = isNaN(c = Math.abs(c)) ? 2 : c,
                d = d == undefined ? "." : d,
                t = t == undefined ? "," : t,
                s = n < 0 ? "-" : "",
                i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
                j = (j = i.length) > 3 ? j % 3 : 0;
            return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
        };

        var unformat = function (value, decimal) {
            // Recursively unformat arrays:
            //            if (isArray(value)) {
            //                return map(value, function (val) {
            //                    return unformat(val, decimal);
            //                });
            //            }
            // Fails silently (need decent errors):
            value = value || 0;
            // Return the value as-is if it's already a number:
            if (typeof value === "number") return value;
            // Default decimal point is "." but could be set to eg. "," in opts:
            decimal = decimal || ",";
            // Build regex to strip out everything except digits, decimal point and minus sign:
            var regex = new RegExp("[^0-9-" + decimal + "]", ["g"]),
                unformatted = parseFloat(
                    ("" + value)
                        .replace(/\((.*)\)/, "-$1") // replace bracketed values with negatives
                        .replace(regex, '') // strip out any cruft
                        .replace(decimal, ',') // make sure decimal point is standard
                );
            // This will fail silently which may cause trouble, let's wait and see:
            return !isNaN(unformatted) ? unformatted : 0;
        };
        var DocTienBangChu = function (SoTien) {
            var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
            var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");
            var lan = 0;
            var i = 0;
            var so = 0;
            var KetQua = "";
            var tmp = "";
            var ViTri = new Array();
            if (SoTien < 0) return "Số tiền âm !";
            if (SoTien == 0) return "Không";
            if (SoTien > 0) {
                so = SoTien;
            }
            else {
                so = -SoTien;
            }
            if (SoTien > 8999999999999999) {
                //SoTien = 0;
                return "Số quá lớn!";
            }
            ViTri[5] = Math.floor(so / 1000000000000000);
            if (isNaN(ViTri[5]))
                ViTri[5] = "0";
            so = so - parseFloat(ViTri[5].toString()) * 1000000000000000;
            ViTri[4] = Math.floor(so / 1000000000000);
            if (isNaN(ViTri[4]))
                ViTri[4] = "0";
            so = so - parseFloat(ViTri[4].toString()) * 1000000000000;
            ViTri[3] = Math.floor(so / 1000000000);
            if (isNaN(ViTri[3]))
                ViTri[3] = "0";
            so = so - parseFloat(ViTri[3].toString()) * 1000000000;
            ViTri[2] = parseInt(so / 1000000);
            if (isNaN(ViTri[2]))
                ViTri[2] = "0";
            ViTri[1] = parseInt((so % 1000000) / 1000);
            if (isNaN(ViTri[1]))
                ViTri[1] = "0";
            ViTri[0] = parseInt(so % 1000);
            if (isNaN(ViTri[0]))
                ViTri[0] = "0";
            if (ViTri[5] > 0) {
                lan = 5;
            }
            else if (ViTri[4] > 0) {
                lan = 4;
            }
            else if (ViTri[3] > 0) {
                lan = 3;
            }
            else if (ViTri[2] > 0) {
                lan = 2;
            }
            else if (ViTri[1] > 0) {
                lan = 1;
            }
            else {
                lan = 0;
            }
            for (i = lan; i >= 0; i--) {
                tmp = DocSo3ChuSo(ViTri[i]);
                KetQua += tmp;
                if (ViTri[i] > 0) KetQua += Tien[i];
                if ((i > 0) && (tmp.length > 0)) KetQua += ',';//&& (!string.IsNullOrEmpty(tmp))
            }
            if (KetQua.substring(KetQua.length - 1) == ',') {
                KetQua = KetQua.substring(0, KetQua.length - 1);
            }
            KetQua = KetQua.substring(1, 2).toUpperCase() + KetQua.substring(2);
            //KetQua += " GG";
            return KetQua;//.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
        }
        //Hàm chuyển số thành chữ
        var DocSo3ChuSo = function (baso) {
            var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
            var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");
            var tram;
            var chuc;
            var donvi;
            var KetQua = "";
            tram = parseInt(baso / 100);
            chuc = parseInt((baso % 100) / 10);
            donvi = baso % 10;
            if (tram == 0 && chuc == 0 && donvi == 0) return "";
            if (tram != 0) {
                KetQua += ChuSo[tram] + " trăm ";
                if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
            }
            if ((chuc != 0) && (chuc != 1)) {
                KetQua += ChuSo[chuc] + " mươi";
                if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
            }
            if (chuc == 1) KetQua += " mười ";
            switch (donvi) {
                case 1:
                    if ((chuc != 0) && (chuc != 1)) {
                        KetQua += " mốt ";
                    }
                    else {
                        KetQua += ChuSo[donvi];
                    }
                    break;
                case 5:
                    if (chuc == 0) {
                        KetQua += ChuSo[donvi];
                    }
                    else {
                        KetQua += " lăm ";
                    }
                    break;
                default:
                    if (donvi != 0) {
                        KetQua += ChuSo[donvi];
                    }
                    break;
            }
            return KetQua;
        }
    </script>
    <script>
        $('#<%= drpBankCode2.ClientID%>').on('change', function () {
            if (this.value != '') {

                var result = this.value.split("-");
                $("#<%= drpBankCode.ClientID %>")
                    .val(result[0])
                    .trigger("change.select2");
                $("#<%= txtAccountName.ClientID%>").val(result[1]);
                $("#<%= txtAccountNumber.ClientID%>").val(result[2]);
            }

        });
        $("#<%=txtAmount.ClientID %>").keyup(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                //var textMoneyVND = DocTienBangChu(price) + " đồng";
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND").html(formatMoneyVNDVal);
            }
        }).blur(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                //var textMoneyVND = DocTienBangChu(price) + " đồng";
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND").html(formatMoneyVNDVal);
            }
        });
        $("#<%=txtAmount.ClientID %>").change(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND").html(formatMoneyVNDVal);
            }
        });


        $("#<%=txtAmount2.ClientID %>").change(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND2").html(formatMoneyVNDVal);
            }
        });


        $("#<%=txtFee.ClientID %>").change(function () {
            var val = $(this).val();
            if (val) {

                val = unformat(val, 0);
                $(this).val(val);

                var price = parseInt(val);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND3").html(formatMoneyVNDVal);
            }
        });

        function tinhloinhuan() {
            var ratein = parseInt($("#<%=txtRateIn.ClientID %>").val());
            var rateout = parseInt($("#<%=txtRate.ClientID %>").val());
            var usdt = parseInt($("#<%=txtUsdt.ClientID %>").val());
            var fee = usdt * (rateout - ratein);
            var price = parseInt(fee);
            $("#<%=txtFee.ClientID %>").val(price);
            var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
            $("#textMoneyVND3").html(formatMoneyVNDVal);
        }
        <%--$("#<%=txtUsdt.ClientID %>").change(function () {

            var user = $("#<%=ddlAccount.ClientID %>").val();
            if (user == "sn1" || user == "sn3") {
                var val = $(this).val();
                var fee = val * 30;
                var reward = val * 0;
                $("#<%=txtFee.ClientID %>").val(fee);
                $("#<%=txtReward.ClientID %>").val(reward);

                var price = parseInt(fee);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND3").html(formatMoneyVNDVal);

                var price2 = parseInt(reward);
                var formatMoneyVNDVal2 = price2.formatMoney(0, '', '.') + "";
                $("#textMoneyVND4").html(formatMoneyVNDVal2);

            }
            else {
                var val = $(this).val();
                var fee = val * 100;
                $("#<%=txtFee.ClientID %>").val(fee);


                var price = parseInt(fee);
                var formatMoneyVNDVal = price.formatMoney(0, '', '.') + "";
                $("#textMoneyVND3").html(formatMoneyVNDVal);
            }
        });--%>
    </script>
</asp:Content>

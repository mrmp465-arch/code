<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Transfer.aspx.cs" MasterPageFile="~/Layout/Layout.master" Inherits="Pages_BankEWalletService_InternalTransfer_Transfer" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i><%= Resources.Pay.Alert%>!</h4>
                <asp:Label runat="server" ID="AlertInfoss"></asp:Label>
            </div>
        </div>
    </div>
    <!-- Content Header (Page header) -->
    <script type="text/javascript">
        var exportThisWithParameter = (function () {
            var uri = 'data:application/vnd.ms-excel;base64,',
                template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"  xmlns="http://www.w3.org/TR/REC-html40"><head> <!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets> <x:ExcelWorksheet><x:Name>{worksheet}</x:Name> <x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions> </x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook> </xml><![endif]--></head><body> <table>{table}</table></body></html>',
                base64 = function (s) {
                    return window.btoa(unescape(encodeURIComponent(s)))
                },
                format = function (s, c) {
                    return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; })
                }
            return function (tableID, excelName) {
                tableID = document.getElementById(tableID)
                var ctx = { worksheet: excelName || 'Worksheet', table: tableID.innerHTML.replace(/<td/g, "<td style='text-align:center;vertical-align:middle;'").replace(/<th/g, "<th style='text-align:center;vertical-align:middle;'") }
                window.location.href = uri + base64(format(template, ctx))
            }
        })()
    </script>
    <section class="content-header">
        <h1>Gom tiền
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Quản lý tài khoản chứa" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Lệnh gom tiền</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Bank</li>

                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.ITransaction %>">Lệnh gom tiền</a></li>
                <li class="active"><a href="#">Lệnh chuyển tiền</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.ITAccount %>">Tài khoản chứa</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.ITAccountOutside %>">Tài khoản ngoài</a></li>
            </ul>

        </div>
        <!-- /.row -->
        <div class="row">
            <div class="col-xs-12">

                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Tạo lệnh chuyển tiền</h3>
                    </div>

                    <div style="clear: both"></div>



                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtClassName">Tài khoản chuyển</label>
                                    <asp:DropDownList ID="drpPartnerBankCode2" runat="server" CssClass="form-control select2">
                                        <asp:ListItem Text="Tài khoản chuyển:" Value=""></asp:ListItem>

                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtClassName">Ngân hàng nhận</label>
                                    <asp:DropDownList ID="drpBankCode2" runat="server" CssClass="form-control select2">
                                        <asp:ListItem Value="Bank"></asp:ListItem>
                                        
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
                                        <asp:ListItem Value="BVBank">BVBank - Ngan Hang Ban Viet Bank</asp:ListItem>
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
                                        <asp:ListItem Value="DongA">(DAB) - NH TMCP Dong A (DongA Bank)</asp:ListItem>
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
                                    <label for="txtClassName">STK nhận</label>
                                    <asp:TextBox ID="txtBankId" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                </div>
                                <div class="form-group">
                                    <label for="txtClassName">Tên TK nhận</label>
                                    <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>

                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtClassName">Số tiền</label>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtAmount2" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>
                                        </div>
                                        <label id="textMoneyVND2" class="control-label col-md-2" style="margin-top: 8px">
                                            &nbsp;
                                        </label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="txtClassName">Nội dung chuyển</label>
                                    <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" placeholder=""></asp:TextBox>


                                </div>
                            </div>

                        </div>

                    </div>
                    <div class="box-footer">
                        <asp:Button ID="Button1" runat="server" CssClass="btn btn-primary pull-right" OnClick="btAdd_Click2" Text="Tạo lệnh"></asp:Button>

                    </div>
                </div>

                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">
                            <asp:Label runat="server" Text="Danh sách log giao dịch" CssClass="title"></asp:Label></h3>
                    </div>
                    <div class="box-body">

                        <div class="box-tools" style="margin-top: 10px;">


                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <asp:DropDownList ID="drpTop" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="50 bản ghi" Value="50"></asp:ListItem>
                                    <asp:ListItem Text="100 bản ghi" Value="100"></asp:ListItem>
                                    <asp:ListItem Text="500 bản ghi" Value="500"></asp:ListItem>
                                    <asp:ListItem Text="1000 bản ghi" Value="1000"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon" style="padding: 5px;">Id</span>
                                    <asp:TextBox ID="txtId" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <asp:DropDownList ID="drpPartnerBankCode" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Text="Tài khoản chuyển:" Value=""></asp:ListItem>



                                </asp:DropDownList>
                            </div>

                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon" style="padding: 5px;">Status</span>
                                    <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control select2">
                                        <asp:ListItem Text="Trạng Thái:" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Đã duyệt" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Khởi tạo" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Đợi duyệt" Value="1"></asp:ListItem>
                                    </asp:DropDownList>

                                </div>
                            </div>



                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon" style="padding: 5px;">Nội dung CK</span>
                                    <asp:TextBox ID="txtComment" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon" style="padding: 5px;">Số tiền</span>
                                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon" style="padding: 2px">Từ ngày</span>
                                    <asp:TextBox CssClass="form-control txtCreatTime" ID="txtFromDate" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6 col-md-2">
                                <div class="input-group">
                                    <span class="input-group-addon" style="padding: 2px">Đến ngày</span>
                                    <asp:TextBox CssClass="form-control txtCreatTime" ID="txtCreatTime" runat="server"></asp:TextBox>
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
                                        <th>STT</th>
                                        <th>Id</th>

                                        <th>TK chuyển</th>
                                        <th>TK nhận</th>


                                        <th>Số tiền</th>


                                        <th>Nội dung</th>
                                        <th>QRCode</th>
                                        <th>CreatedTime</th>
                                        <th>LastTime</th>
                                        <th>Người tạo</th>
                                        <th>Người duyệt</th>
                                        <th>Trạng thái</th>
                                        <th>Tác vụ</th>
                                    </tr>

                                </thead>

                                <tbody>
                                    <asp:Repeater ID="rptList" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Container.ItemIndex + 1 %></td>
                                                <td><%#Eval("Id") %></td>


                                                <td><%#Eval("PartnerBankCode")%>- <%#Eval("PartnerBankName") %>- <%#Eval("PartnerBankId") %></td>
                                                <td><%#Eval("BankCode") %>- <%#Eval("BankName") %>- <%#Eval("BankId") %></td>


                                                <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(".", ",") %></td>

                                                <td>ck1 <%#Eval("Id") %></td>
                                                <td class="lstlightbox">
                                                    <asp:Panel ID="pnQR" runat="server" Visible='<%#Eval("Status").ToString()=="0"%>'>
                                                        <a href="<%#GetQR(Eval("BankCode").ToString(),Eval("BankId").ToString(),Eval("Id").ToString(),Eval("Amount").ToString()) %>">
                                                            <img src="<%#GetQR(Eval("BankCode").ToString(),Eval("BankId").ToString(),Eval("Description").ToString(),Eval("Amount").ToString()) %>>" height="25" />
                                                        </a>
                                                    </asp:Panel>
                                                </td>
                                                <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}" )%></td>
                                                <td><%#Eval("UpdateTime", "{0:dd/MM HH:mm:ss}")%></td>
                                                <td><%#Eval("UserName") %></td>
                                                <td><%#Eval("AppName") %></td>
                                                <td><%# GetStatus(Eval("Status")) %></td>
                                                <td>

                                                    <% if ((AppUtils.IsAdmin && RoleApp))
                                                        {
                                                    %>
                                                    <asp:LinkButton ID="lnkUpdate" CausesValidation="false" ToolTip="Duyệt" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("Status").ToString()=="1"%>'
                                                        CommandName="update" OnClientClick="return confirm('Bạn chắc chắn muốn duyệt?'); "
                                                        runat="server" OnCommand="App_Command">Duyệt</asp:LinkButton>
                                                    &nbsp; |&nbsp;
                                                  
                                                 <asp:LinkButton ID="lnkDelete" CausesValidation="false" ToolTip="Hủy" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("Status").ToString()=="1"%>'
                                                     CommandName="delete" OnClientClick="return confirm('Bạn chắc chắn muốn huỷ?'); "
                                                     runat="server" OnCommand="Delete_Command">Hủy</asp:LinkButton>
                                                    <%} %>

                                                    <asp:LinkButton ID="LinkButton1" CausesValidation="false" ToolTip="Cập nhật" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("Status").ToString()=="0" && AppUtils.UserName ==Eval("UserName").ToString() %>'
                                                        CommandName="update" OnClientClick="return confirm('Bạn chắc chắn muốn cập nhtaajt?'); "
                                                        runat="server" OnCommand="Update_Command">Cập nhật</asp:LinkButton>
                                                    &nbsp; |&nbsp;
   
                                                  <asp:LinkButton ID="LinkButton2" CausesValidation="false" ToolTip="Hủy" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("Status").ToString()=="0" && AppUtils.UserName ==Eval("UserName").ToString( )%>'
                                                      CommandName="delete" OnClientClick="return confirm('Bạn chắc chắn muốn huỷ?'); "
                                                      runat="server" OnCommand="Delete_Command">Hủy</asp:LinkButton>



                                                </td>

                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                        <div class="box-footer">
                            <input type="button" class="btn btn-info pull-right" value="export Excel" onclick="exportThisWithParameter('TableResponsive', 'DoiSoat')" />
                        </div>
                        <!-- /.box-body -->
                    </div>
                    <!-- /.box -->

                    <!-- /.col -->
                </div>
            </div>
    </section>
    <!-- /.content -->
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

</script>
</asp:Content>

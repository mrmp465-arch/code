<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Partners.Edit.Discount.aspx.cs" Inherits="Pages_PayGate_Partners_Edit_Discount" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header)  -->
    <div class="modal  fade bs-example-modal-sm" id="AlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertBans"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1>Kết nối
        <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersList %>">Danh sách đối tác</a>  </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Cập nhật đối tác
                    <asp:Label ID="lblPartner" runat="server"></asp:Label>
                    -
                    <asp:Label ID="lblPartnerId" runat="server"></asp:Label>
                </li>
                <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersEdit%>?id=<%=lblPartnerId.Text%>&code=<%=lblPartner.Text%>&type=info">Thông Tin</a></li>
                <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersEdit%>?id=<%=lblPartnerId.Text%>&code=<%=lblPartner.Text%>&type=service">Dịch vụ</a></li>
                <li class="active"><a href="#discount" data-toggle="tab">Chiết khấu</a></li>
            </ul>
            <div class="tab-content no-padding">
                <!-- Morris chart - Sales -->
                <div class="chart tab-pane active" id="discount">
                    <div class="nav-tabs-custom">
                        <ul class="nav nav-tabs pull-right">
                            <li class=""><a href="#tabSms" data-toggle="tab" aria-expanded="false">SMS</a></li>
                            <li class=""><a href="#tabBank" data-toggle="tab" aria-expanded="false">Ngân hàng</a></li>
                            <li class=""><a href="#tabBuyCard" data-toggle="tab" aria-expanded="false">Mua thẻ</a></li>
                            <li class="active"><a href="#tabCard" data-toggle="tab" aria-expanded="true">Gạch thẻ</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane" id="tabSms">
                                <b>SMS</b>
                            </div>
                            <div class="tab-pane" id="tabBank">
                                <b>Bank</b>
                            </div>
                            <div class="tab-pane" id="tabBuyCard">
                                <b>Buy Card</b>
                            </div>

                            <div class="tab-pane active" id="tabCard">
                                <div class="box-tools">
                                    <div class="col-xs-12 col-sm-6 col-md-2">
                                        <div class="form-group">
                                            <div class="input-group date">
                                                <div class="input-group-addon">
                                                    <i class="fa fa-calendar"></i>
                                                </div>
                                                <asp:TextBox ID="datepicker" runat="server" class="form-control"> </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-4">
                                        <asp:Button ID="btnDiscountView" runat="server" Text="Xem Discount" CssClass="btn btn-primary pull-left" OnClick="btnDiscountView_Click"></asp:Button>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-2">
                                        <asp:FileUpload ID="fileUploadExcel" runat="server" CssClass="btn btn-default" />
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-1">
                                        <asp:Button ID="btnUploadExcel" runat="server" Text="Import Excel" CssClass="btn btn-primary pull-left" OnClick="btnUploadExcel_Click"></asp:Button>
                                    </div>
                                    <div class="col-xs-12 col-sm-6 col-md-3">
                                        (Có thể upload cho toàn bộ các mã Partner)
                                    </div>
                                </div>
                            </div>
                            <div class="box-body">
                                <div class="col-sm-12">
                                    <div class="box-body table-responsive no-padding">
                                        <table class="table table-condensed">
                                            <tbody>
                                                <tr>
                                                    <th>Ngày</th>

                                                    <th>D. BANK</th>
                                                    <th>R. BANK</th>
                                                    <th>D. MOMO</th>
                                                    <th>R. MOMO</th>

                                                    <th>D. BANKOUT</th>
                                                    <th>R. BANKOUT</th>
                                                    <th>D. MOMOOUT</th>
                                                    <th>R. MOMOOUT</th>
                                                    <th>D. CARDOUT</th>
                                                    <th>R. CARDOUT</th>
                                                    <th>D. VTT</th>
                                                    <th>R. VTT</th>
                                                    <th>D. VNP</th>
                                                    <th>R. VNP</th>
                                                    <th>D. VMS</th>
                                                    <th>R. VMS</th>

                                                    <th>D. GATE</th>
                                                   <%-- <th>R. GATE</th>--%>
                                                </tr>
                                                <asp:Repeater ID="rptDiscount" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td><%#Eval("Time") %></td>

                                                            <td style="display: none">
                                                                <asp:TextBox ID="txtDiscountZing" runat="server" CssClass="form-control" Text='<%#Eval("DiscountZING") %>'></asp:TextBox>
                                                            </td>

                                                            <td style="display: none">
                                                                <asp:TextBox ID="txtRewardZING" runat="server" CssClass="form-control" Text='<%#Eval("RewardZING") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtDiscountBankTranfer" runat="server" CssClass="form-control" Text='<%#Eval("DiscountBANKTRANFER") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtRewardBankTranfer" runat="server" CssClass="form-control" Text='<%#Eval("RewardBANKTRANFER") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtDiscountMomo" runat="server" CssClass="form-control" Text='<%#Eval("DiscountMOMO") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtRewardMomo" runat="server" CssClass="form-control" Text='<%#Eval("RewardMOMO") %>'></asp:TextBox>
                                                            </td>


                                                            <td>
                                                                <asp:TextBox ID="txtDiscountBit" runat="server" CssClass="form-control" Text='<%#Eval("DiscountBANKOUTTRANFER") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtRewardBit" runat="server" CssClass="form-control" Text='<%#Eval("RewardBANKOUTTRANFER") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtDiscountDzo" runat="server" CssClass="form-control" Text='<%#Eval("DiscountMOMOOUT") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtRewardDzo" runat="server" CssClass="form-control" Text='<%#Eval("RewardMOMOOUT") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" Text='<%#Eval("DiscountVTTOUT") %>'></asp:TextBox>
                                                            </td>
                                                            <td >
                                                                <asp:TextBox ID="txtRewardGosu" runat="server" CssClass="form-control" Text='<%#Eval("RewardVTTOUT") %>'></asp:TextBox>
                                                            </td>

                                                            <td>
                                                                <asp:TextBox ID="txtDiscountVTT" runat="server" CssClass="form-control" Text='<%#Eval("DiscountVTT") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtRewardVTT" runat="server" CssClass="form-control" Text='<%#Eval("RewardVTT") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtDiscountVNP" runat="server" CssClass="form-control" Text='<%#Eval("DiscountVNP") %>'></asp:TextBox>
                                                            </td>
                                                             <td >
                                                                <asp:TextBox ID="txtRewardVNP" runat="server" CssClass="form-control" Text='<%#Eval("RewardVNP") %>'></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtDiscountVMS" runat="server" CssClass="form-control" Text='<%#Eval("DiscountVMS") %>'></asp:TextBox>
                                                            </td>
                                                             <td >
                                                                <asp:TextBox ID="txtRewardVMS" runat="server" CssClass="form-control" Text='<%#Eval("RewardVMS") %>'></asp:TextBox>
                                                            </td>

                                                            <td>
                                                                <asp:TextBox ID="txtDiscountGate" runat="server" CssClass="form-control" Text='<%#Eval("DiscountGATE") %>'></asp:TextBox>
                                                            </td>
                                                             <td style="display: none">
                                                                <asp:TextBox ID="txtRewardGate" runat="server" CssClass="form-control" Text='<%#Eval("RewardGATE") %>'></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <!-- /.tab-pane -->

                                <!-- /.tab-content -->
                            </div>


                        </div>
                        <div class="box-footer">


                            <asp:Button ID="btnUpdateDiscount" runat="server" Text="CẬP NHẬT" CssClass="btn btn-primary" OnClick="btnUpdateDiscount_Click"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <style type="text/css">
        .LastIndex {
            display: none;
        }
    </style>
    <script type="text/javascript">  
        $(document).ready(function () {
            $(".openclass").click(function () {
                if (!$(this).hasClass("open")) {
                    $(this).addClass("open").html("-");
                    $(".LastIndex" + $(this).data("id")).css("display", "table-row");
                } else {
                    $(".LastIndex" + $(this).data("id")).css("display", "none");
                    $(this).removeClass("open").html("+");
                }
            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css" />
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $(function () {
                //Date picker
                $('#<%=datepicker.ClientID%>').datepicker({
                    orientation: "bottom auto",
                    autoclose: true,
                    format: "mm-yyyy",
                    startView: 1,
                    minViewMode: 1
                });
            });
        });

    </script>
    <style>
        .table-responsive {
            width: 100%;
            overflow-x: auto;
            overflow-y: hidden;
            -webkit-overflow-scrolling: touch;
            -ms-overflow-style: -ms-autohiding-scrollbar;
        }

            .table-responsive input {
                width: 68px;
            }
    </style>
</asp:Content>

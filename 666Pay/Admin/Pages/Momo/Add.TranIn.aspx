<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Add.TranIn.aspx.cs" Inherits="Pages_Monitor_Add_TranIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Giao dịch Momo
            <small>
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.CardAPIMonitor %>">Xem log giao dịch</a>
            </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Xem chi tiết</li>
        </ol>
    </section>
    <section class="content">
        <div class="box box-solid">
            <div class="box-header with-border">
                <i class="fa fa-text-width"></i>
                <h3 class="box-title">
                    <asp:Label ID="lblTtitle" runat="server" Text="Kiểm tra và thêm thông tin giao dịch In" CssClass="title"></asp:Label></h3>
            </div>
            <!-- /.box-header -->
            <div class="pad margin no-print" runat="server" id="resultDiv">
                <div class="callout callout-success" id="callout" runat="server">

                    <div class="box-body">
                        <pre runat="server" id="contentSpan"></pre>
                    </div>
                    <div class="input-group">
                        <span class="input-group-btn">
                            <button type="button" class="btn btn-primary pull-right" runat="server" id="btnAcceptedTran" onserverclick="btnAcceptedTran_Click">Thêm giao dịch</button>
                        </span>
                    </div>
                </div>
            </div>

            <div class="box-body">

                <div class="form-group">
                    <label>Momo Transaction Id</label>
                    <input class="form-control" placeholder="Enter Momo Transaction Id" runat="Server" id="MomoTransactionId"></input>
                </div>
                <div class="form-group">
                    <label>MomoId Khách hàng</label>
                    <input class="form-control" placeholder="Enter  Momo User Id" runat="Server" id="MomoUserId"></input>
                </div>
                <div class="form-group">
                    <label>MomoId Dịch vụ</label>
                    <input class="form-control" placeholder="Enter Momo Partner Id" runat="Server" id="MomoPartnerId"></input>
                </div>

                <div class="form-group">
                    
                    <button type="button" class="btn btn-primary pull-right" runat="server" id="btnCheckTran" onserverclick="btnCheckTran_Click">
                        <i class="fa fa-check-square-o"></i> Gọi Momo Client kiểm tra
                    </button>
                    <button type="button" class="btn btn-default pull-left" runat="server" id="btnBack" onserverclick="btnBack_Click">
                        <i class="fa fa-backward"></i> Quay lại
                    </button>
                </div>
            </div>

            <!-- /.box-body -->
        </div>

        <%--<section id="brand">
            <h4 class="page-header">Brand Icons</h4>
            <div class="alert alert-info">
                <ul class="margin-bottom-none padding-left-lg">
                    <li>All brand icons are trademarks of their respective owners.</li>
                    <li>The use of these trademarks does not indicate endorsement of the trademark holder by Font
                        Awesome, nor vice versa.
                    </li>
                </ul>
            </div>
        </section>--%>
    </section>

</asp:Content>


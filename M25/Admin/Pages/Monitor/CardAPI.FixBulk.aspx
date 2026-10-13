<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardAPI.FixBulk.aspx.cs" Inherits="Pages_Monitor_CardAPI_FixBulk" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
        <h1>Thẻ Cào
        <small>Sửa thông tin giao dịch thẻ cào</small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Sửa lỗi</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Sửa lỗi</li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixStatus %>">Sửa đơn</a></li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Sửa theo Bulk</a></li>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-body">
                        <asp:Panel runat="server" ID="panelTools">

                            <div class="row" style="margin-top: 10px;">
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtTransactionID" runat="server" CssClass="form-control" placeholder="TransactionID"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtCardSerial" runat="server" CssClass="form-control" placeholder="CardSerial"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Amount"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <div class="form-group">
                                        <asp:HiddenField ID="txtId" runat="server"></asp:HiddenField>
                                        <asp:Button ID="btnAdd" runat="server" Text="Cập nhập" CssClass="btn btn-info " OnClick="btnAdd_Click"></asp:Button>
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-xs-12 col-sm-6 col-md-3">
                                    <img src="<%=Constant.ADMIN_PATH  %>Content/MauExcel.png" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <a href="<%=Constant.ADMIN_PATH  %>content/FileMau.xlsx"><strong>Tải file excel mẫu</strong></a>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <asp:FileUpload ID="FileUploadExcel" CssClass="form-control" runat="server" />
                                </div>
                                <div class="col-xs-12 col-sm-6 col-md-2">
                                    <asp:Button ID="Button1" runat="server" Text="Tải lên" CssClass="btn btn-info " OnClick="btnUpload_Click"></asp:Button>
                                </div>
                            </div>

                        </asp:Panel>

                        <table class="table table-striped" id="TableResponsive" style="margin-top: 20px; clear: both;">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">#</th>
                                    <th>TransactionID</th>
                                    <th>CardSerial</th>
                                    <th>Amount</th>
                                    <th>Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><%#Eval("[TransactionID]") %></td>

                                            <td><%#Eval("[CardSerial]")%></td>
                                            <td><%#Eval("[Amount]")%></td>
                                            <td>
                                                <%#Eval("[status]")%></td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk %>?id=<%#Eval("Id") %>" title="Sửa">sửa</a> | 
                                                <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk %>?id=<%#Eval("Id") %>&type=del" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                                <asp:Label ID="lblUserID" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:Label>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <!-- /.row -->
                        <%-- --%>
                        <div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="btnHistory" runat="server" Text="Lịch sử sửa lỗi" CssClass="btn btn-info " OnClick="btHistory_Click"></asp:Button>
                        <asp:Button ID="btUpdate" runat="server" Text="Sửa lỗi" CssClass="btn btn-info pull-right" OnClick="btFixBulk_Click"></asp:Button>
                        <div class="pull-right">
                            
                            <% if (pages!=null && pages.EndPage > 1){%>
                                    <span style="line-height: 27px;    padding: 0 9px 0 0;    font-weight: bold;"> Trang <%=pages.CurrentPage  %>/<%=pages.TotalPages%></span>                                    <ul class="pagination pagination-sm no-margin pull-right">
                                        <% if (pages.CurrentPage > 1){%>
                                               <li>
								                    <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk %>?type=his&page=1">First</a>
							                    </li>
							                    <li>
								                    <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk %>?type=his&page=<%=pages.CurrentPage - 1%>"><</a>
							                    </li>
                                        <% } %>
                                          <% for (var page = pages.StartPage; page <= pages.EndPage; page++){%>
                                              <li class="<%= Getactive(page, pages.CurrentPage)%>">   
								                <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk %>?type=his&page=<%= page%>"> <%= page%></a>
							                </li> 
                                        <% } %>
                                        <% if (pages.CurrentPage < pages.TotalPages){%>
                                               <li>
								                    <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk %>?type=his&page=<%=pages.CurrentPage + 1%>">></a>
							                    </li>
							                    <li>
								                    <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk %>?type=his&page=<%= (pages.TotalPages) %>  ">Previous</a>
							                    </li> 
                                        <% } %>
                                    </ul>
                            <% } %>  
                        </div>
                    </div>
                </div>
                <div class="chart tab-pane " id="sales-chart">
                </div>
            </div>
        </div>
    </section>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <style type="text/css">
        .pagination .page_disabled{
       z-index: 3;
        color: #fff;
        cursor: default;
        background-color: #337ab7;
        border-color: #337ab7;
    }
    </style>
</asp:Content>

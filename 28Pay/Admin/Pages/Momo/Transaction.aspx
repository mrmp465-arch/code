<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Transaction.aspx.cs" Inherits="Pages_Momo_Transaction" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ajaxToolkit:FilteredTextBoxExtender ID="ftbeId" runat="server" TargetControlID="txtId" ValidChars="1234567890" />
    <ajaxToolkit:FilteredTextBoxExtender ID="ftbeStatus" runat="server" TargetControlID="txtStatus" ValidChars="-1-2-3-4-5-6-7-8-901234567890" />
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình momo
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Quản lý log giao dịch" CssClass="title"></asp:Label>
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
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Id</span>
                                <asp:TextBox ID="txtId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>

                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpPartner" runat="server" CssClass="form-control select2">
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpType" runat="server" CssClass="form-control select2">
                                <asp:ListItem Text="Loại:" Value=""></asp:ListItem>
                                <asp:ListItem Text="IN" Value="IN"></asp:ListItem>
                                <asp:ListItem Text="OUT" Value="OUT"></asp:ListItem>
                                <asp:ListItem Text="CASH" Value="CASH"></asp:ListItem>
                                <asp:ListItem Text="TRANSFER" Value="TRANSFER"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Status</span>
                                <%-- <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control select2">
                                <asp:ListItem Text="Trạng Thái:" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="Thành công" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Khác" Value="0"></asp:ListItem>
                            </asp:DropDownList>--%>
                                <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">RefCode</span>
                                <asp:TextBox ID="txtRefCode" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">TK Khách</span>
                                <asp:TextBox ID="txtMomoId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">TK Hệ thống</span>
                                <asp:TextBox ID="txtPartnerMomoId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">MomoTransId</span>
                                <asp:TextBox ID="txMomoTransId" runat="server" CssClass="form-control"></asp:TextBox>
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
                                <span class="input-group-addon" style="padding: 5px;">Time</span>
                                <asp:TextBox ID="txtCreatTime" runat="server" CssClass="form-control txtCreatTime"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox runat="server" ID="cbByPass10k" Checked="True" />
                                </span>
                                <span class="form-control">>= 10K</span>
                            </div>
                            <!-- /input-group -->
                        </div>

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox runat="server" ID="cbAuto" Checked="False" />
                                </span>
                                <span class="form-control">Auto Load</span>
                            </div>
                            <!-- /input-group -->

                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                    </div>
                    <div style="clear: both"></div>
                    <!-- /.box-header -->
                    <div class="box-body  no-padding">
                        <div class="col-sm-6" style="font-weight: bold">
                            <asp:Label runat="server" ID="lblTotal"></asp:Label>
                        </div>
                        <div class="col-sm-6">
                            <button type="button" class="btn btn-primary pull-right" runat="server" id="btnAcceptedTran" onserverclick="AddTranIn_Click">Kiểm tra giao dịch In</button>
                        </div>
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>STT</th>
                                    <th>Id</th>
                                    <th>PartnerCode</th>
                                    <th>RefCode</th>
                                    <th>Loại</th>
                                    <th>Tài khoản khách</th>
                                    <th>Tài khoản hệ thống</th>
                                    <th>MomoTransId</th>
                                    <th>Amount</th>
                                    <th>CreatedTime</th>
                                    <th>LastTime</th>
                                    <th>Nội dung ck</th>
                                     <th>Code</th>
                                    <%--<th>Des</th>--%>
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
                                            <td><%#Eval("PartnerCode") %></td>
                                            <td><%#Eval("RefCode") %></td>
                                            <td><%#Eval("CommandCode") %></td>
                                            <td><%#Eval("MomoId") %></td>
                                            <td><%#Eval("PartnerMomoId") %></td>
                                            <td><%#Eval("MomoTransId") %></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(",", ".") %></td>
                                            <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}" )%></td>
                                            <td><%#Eval("UpdateTime", "{0:dd/MM HH:mm:ss}")%></td>
                                            <td><%#Eval("Comment") %></td>
                                             <td><%#GetCodeStatus(Eval("CommandCode").ToString(),Eval("Comment").ToString(),Eval("OrderNo").ToString()) %></td>
                                            <%--<td><%#Eval("Description") %></td>--%>
                                            <td><%#Eval("Status") %></td>
                                            <td>
                                                <asp:HyperLink NavigateUrl='<%#CheckTranOutUrl(Eval("Id").ToString()) %>' Visible='<%# (Eval("CommandCode").ToString() == "OUT" || Eval("CommandCode").ToString() == "TRANSFER" || Eval("CommandCode").ToString() == "CASH") %>' runat="server"> [Kiểm tra] </asp:HyperLink>
                                                <asp:HyperLink NavigateUrl='<%#EditTranInUrl(Eval("Id").ToString()) %>' Visible='<%#  Eval("CommandCode").ToString() == "IN" %>' runat="server"> [Sửa Code] </asp:HyperLink>
                                                &nbsp;<asp:LinkButton ID="lnCallback" runat="server" OnCommand="Callback_Command" CommandName="Callback" CommandArgument='<%#Eval("Id")%>' Visible='False' OnClientClick="return confirm('Bạn có muốn thực hiện?')">[Callback] </asp:LinkButton>
                                                                                &nbsp;<asp:LinkButton ID="lnUpdateCash" runat="server" OnCommand="UpdateCash_Command" CommandName="UpdateCash" CommandArgument='<%#Eval("Id")%>' Visible='<%#Eval("CommandCode").ToString() == "CASH" && Eval("Status").ToString() == "-1" %>' OnClientClick="return confirm('Bạn có muốn thực hiện?')">[Cập nhật đúng] </asp:LinkButton>
                                            </td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- /.box-body -->
                </div>
                <!-- /.box -->

                <!-- /.col -->
            </div>
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

        $(function () {
            $("#<%= drpPartner.ClientID %>").select2()
        })


    </script>
    <script type="text/javascript">

        var refreshPageInterval = 30;

        setInterval("countDownPageRefresh()", 1000);//1 s gọi 1 lần
        $(document).mouseover(function () {
            funcResetRefreshPageInterval();
        });
        $(window).scroll(function () {
            funcResetRefreshPageInterval();
        });
        window.onkeypress = funcResetRefreshPageInterval;
        function funcResetRefreshPageInterval() {
            refreshPageInterval = 30;
        }
        function countDownPageRefresh() {
            refreshPageInterval = refreshPageInterval - 1;
            //console.log(refreshPageInterval);

            if (refreshPageInterval <= 0) {
                funcResetRefreshPageInterval();

                if ($('#<%= cbAuto.ClientID %>').is(':checked')) {
                    $('#<%= btView.ClientID %>').onclick();
                }

            }
        }

    </script>
    <style>
        .bwarning {
            color: red;
            animation: blinker 1s linear infinite;
        }

        @keyframes blinker {
            50% {
                opacity: 0;
            }
        }
    </style>
</asp:Content>

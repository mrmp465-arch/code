<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="Transaction.aspx.cs" Inherits="Pages_BankEWalletService_Bank_Transaction" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ajaxToolkit:FilteredTextBoxExtender ID="ftbeId" runat="server" TargetControlID="txtId" ValidChars="1234567890" />
    <ajaxToolkit:FilteredTextBoxExtender ID="ftbeStatus" runat="server" TargetControlID="txtStatus" ValidChars="-1-2-3-4-5-6-7-8-901234567890" />
    <%--<script type="text/javascript">
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
    </script>--%>
    <script src="https://cdn.jsdelivr.net/npm/xlsx@0.18.5/dist/xlsx.full.min.js"></script>


    <script>
        //function exportTableToExcel(tableID, fileName,) {
        //    var table = document.getElementById(tableID);
        //    var hiddenColumns = [2,3,4,,11,14,15,16]
        //    // Chuyển bảng HTML sang worksheet
        //    var ws = XLSX.utils.table_to_sheet(table);

        //    // Ẩn cột (theo chỉ số)
        //    ws['!cols'] = ws['!cols'] || [];
        //    hiddenColumns.forEach(index => {
        //        ws['!cols'][index] = { hidden: true };
        //    });

        //    // Tạo workbook và ghi file
        //    var wb = XLSX.utils.book_new();
        //    XLSX.utils.book_append_sheet(wb, ws, "Sheet1");
        //    XLSX.writeFile(wb, fileName + ".xlsx");
        //}
        function exportTableToExcel(tableID, fileName) {

            var removeColumns = [2, 3, 4, 6, 11, 14, 15, 16]
            var table = document.getElementById(tableID);

            // B1: HTML → Sheet
            var ws = XLSX.utils.table_to_sheet(table);

            // B2: Sheet → AOA
            var data = XLSX.utils.sheet_to_json(ws, { header: 1 });

            // B3: Xoá cột
            var newData = data.map(row =>
                row.filter((_, colIndex) => !removeColumns.includes(colIndex))
            );

            // B4: AOA → Sheet
            var newSheet = XLSX.utils.aoa_to_sheet(newData);

            // B5: AUTO WIDTH
            var colWidths = [];
            var colCount = newData[0].length;

            for (let col = 0; col < colCount; col++) {
                let maxLen = 0;
                newData.forEach(row => {
                    let cell = row[col] ? row[col].toString() : "";
                    if (cell.length > maxLen) maxLen = cell.length;
                });

                colWidths.push({ wch: maxLen }); // +2 cho thoáng
            }

            newSheet['!cols'] = colWidths;

            // B6: Xuất file
            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, newSheet, "Sheet1");
            XLSX.writeFile(wb, fileName + ".xlsx");
        }
    </script>

    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình Bank
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Quản lý log giao dịch" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            Tra
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
                            <asp:DropDownList ID="drpPartnerBankCode" runat="server" CssClass="form-control select2">
                                <asp:ListItem Text="Bank Sys:" Value=""></asp:ListItem>
                                <asp:ListItem Text="ACB" Value="ACB"></asp:ListItem>
                                <asp:ListItem Text="VPB" Value="VPB"></asp:ListItem>

                                <asp:ListItem Text="BIDV" Value="BIDV"></asp:ListItem>
                                <asp:ListItem Text="MB" Value="MB"></asp:ListItem>
                                <asp:ListItem Text="VCB" Value="VCB"></asp:ListItem>
                                <asp:ListItem Text="SEAB" Value="SEAB"></asp:ListItem>
                                <asp:ListItem Text="ICB" Value="ICB"></asp:ListItem>
                                <asp:ListItem Text="TIMO" Value="TIMO"></asp:ListItem>
                                <asp:ListItem Text="OCB" Value="OCB"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:DropDownList ID="drpType" runat="server" CssClass="form-control select2">
                                <asp:ListItem Text="Loại:" Value=""></asp:ListItem>
                                <asp:ListItem Text="IN" Value="IN"></asp:ListItem>
                                <asp:ListItem Text="OUT" Value="OUT"></asp:ListItem>
                                <%-- <asp:ListItem Text="CASH" Value="CASH"></asp:ListItem>--%>
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
                                <asp:TextBox ID="txtBankId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">TK Hệ thống</span>
                                <asp:TextBox ID="txtPartnerBankId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">BankTransId</span>
                                <asp:TextBox ID="txBankTransId" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group1">

                                <asp:TextBox ID="txtComment" runat="server" CssClass="form-control" placeholder="Nội dung CK"></asp:TextBox>
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

                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Min Amount</span>
                                <asp:TextBox ID="txtMinAmount" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon" style="padding: 5px;">Max Amount</span>
                                <asp:TextBox ID="txtMaxAmount" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <asp:CheckBox runat="server" ID="cbComentNull" Checked="False" />
                                </span>
                                <span class="form-control">Sai nội dung</span>
                            </div>
                            <!-- /input-group -->
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-primary" OnClick="btView_Click" Text="Xem"></asp:Button>
                            &nbsp;
        <asp:Button ID="btExcel" runat="server" CssClass="btn btn-info" OnClick="ExportTran2_Click" Text="Export Excel"></asp:Button>
                        </div>
                        <%-- <div style="display:none;">
                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-primary" OnClick="btView2_Click" Text="Xem"></asp:Button>
                        </div>--%>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body  no-padding">
                        <%--<div class="col-sm-12">
                            <button type="button" class="btn btn-primary pull-right" runat="server" id="btnAcceptedTran" onserverclick="AddTranIn_Click">Kiểm tra giao dịch In</button>
                        </div>--%>
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">
                            <thead>
                                <tr>
                                    <th>STT</th>
                                    <th>Id</th>
                                    <th>RefCode</th>
                                    <th>SysCode</th>
                                    <th>Loại</th>
                                    <th>TK hệ thống</th>
                                    <th>TK khách</th>

                                    <th>BankTransId</th>
                                    <th>Amount</th>
                                    <th>CreatedTime</th>
                                    <th>LastTime</th>
                                    <th>Time(s)</th>
                                    <th>Nội dung</th>
                                    <th>Code</th>
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

                                            <td><%#Eval("RefCode") %></td>
                                            <td><%#Eval("PartnerCode") %></td>
                                            <td><%#Eval("CommandCode") %></td>
                                            <td><%#Eval("PartnerBankCode") %>- <%#Eval("PartnerBankId") %></td>
                                            <td><%#Eval("BankCode") %>- <%#Eval("BankId") %></td>

                                            <td><%#Eval("BankTransId") %></td>
                                            <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(".", ",") %></td>
                                            <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}" )%></td>
                                            <td><%#Eval("UpdateTime", "{0:dd/MM HH:mm:ss}")%></td>
                                            <td>
                                                <%# 
                                                (Eval("UpdateTime") != DBNull.Value && Eval("CreatedTime") != DBNull.Value) 
                                                ? ((DateTime)Eval("UpdateTime") - (DateTime)Eval("CreatedTime")).TotalSeconds.ToString("N0") 
                                                : ""
                                                %>
                                            </td>
                                            <td><%#Eval("CommentOrg") %></td>
                                            <td><%#GetCodeStatus(Eval("CommandCode").ToString(),Eval("PartnerCode").ToString(),Eval("Comment").ToString(),Eval("CommentOrg").ToString()) %></td>
                                            <td><%#Eval("Status") %></td>
                                            <td>
                                                <asp:HyperLink NavigateUrl='<%#CheckTranOutUrl(Eval("Id").ToString()) %>' Visible='<%# Eval("Status").ToString() != "1"   &&  (Eval("CommandCode").ToString() == "OUT" || Eval("CommandCode").ToString() == "TRANSFER") %>' runat="server"> [Kiểm tra]  </asp:HyperLink>
                                                <asp:HyperLink NavigateUrl='<%#EditTranInUrl(Eval("Id").ToString()) %>' Visible='<%#   Eval("CommandCode").ToString() == "IN" && Eval("Comment").ToString() != "hoantien" && !Eval("Comment").ToString().Contains("huybo") %>' runat="server"> [Sửa Code] </asp:HyperLink>
                                                <asp:HyperLink
                                                    ID="lnbill"
                                                    CssClass="lightbox"
                                                    NavigateUrl='<%# "https://info.5apps.info/Pages/Detail2.aspx?orderNo=" + Eval("Id") %>'
                                                    runat="server"
                                                    Visible='<%# Eval("Status").ToString() == "1" && Eval("CommandCode").ToString()=="TRANSFER" %>'>
                                                 Xem bill
                                                </asp:HyperLink>
                                            </td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <%-- <div class="box-footer">
                        <input type="button" class="btn btn-info pull-right" value="export Excel" onclick="exportTableToExcel('TableResponsive', 'giaodich')" />
                    </div>--%>
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
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />

    <script type="text/javascript">
        $(document).ready(function () {
            var table = $('#TableResponsive').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
        $(document).ready(function () {

            $(document).ready(function () {
                $('#<%=txtFromDate.ClientID %>').datetimepicker({
                    format: 'DD-MM-YYYY HH:mm:ss'
                });
                $('#<%=txtCreatTime.ClientID %>').datetimepicker({
                    format: 'DD-MM-YYYY HH:mm:ss'
                });

            });

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
    <style>
        td, th {
            padding-right: 1px !important
        }
    </style>
    <script type="text/javascript">

        var refreshPageInterval = 60;

        setInterval("countDownPageRefresh()", 1000);//1 s gọi 1 lần
        $(document).mouseover(function () {
            funcResetRefreshPageInterval();
        });
        $(window).scroll(function () {
            funcResetRefreshPageInterval();
        });
        window.onkeypress = funcResetRefreshPageInterval;
        function funcResetRefreshPageInterval() {
            refreshPageInterval = 60;
        }
        function countDownPageRefresh() {
            refreshPageInterval = refreshPageInterval - 1;
            //console.log(refreshPageInterval);

            if (refreshPageInterval <= 0) {
                funcResetRefreshPageInterval();

                location.reload();
            }
        }

    </script>
    <script>
        $(function () {
            $(".lightbox").fancybox({
                type: "iframe",
                width: "80%",
                height: "89%",
                fitToView: true
            });
        });
    </script>
</asp:Content>

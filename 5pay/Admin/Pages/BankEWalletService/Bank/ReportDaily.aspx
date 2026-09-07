<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Layout/Layout.master" CodeFile="ReportDaily.aspx.cs" Inherits="Pages_BankEWalletService_Bank_ReportDaily" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

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

            var removeColumns = []
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
    <section class="content-header">
        <h1>Bank
            <small>Báo cáo giao dịch </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Báo cáo</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">

        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Báo Cáo</li>

                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankReport %>">Theo thời gian</a></li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Theo khoảng thời gian</a></li>

            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-tools" style="margin-top: 10px;">

                        <div class="col-xs-12 col-sm-4 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">Từ ngày </span>
                                <asp:TextBox ID="txtBeginTime1" runat="server" CssClass="form-control txtBeginTime"></asp:TextBox>
                            </div>

                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">Tới</span>
                                <asp:TextBox ID="txtEndTime1" runat="server" CssClass="form-control txtEndTime"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-4 col-md-2">
                            <asp:Button ID="btView1" runat="server" CssClass="btn btn-info " OnClick="btView_Click1" Text="Xem"></asp:Button>
                        </div>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <div class="table-responsive">
                            <div id="dgrid" class="dataTables_wrapper form-inline" role="grid">
                                <table class="table table-striped" id="TableResponsive">
                                    <thead>
                                        <tr>
                                            <td style="width: 10px;">#</td>
                                            <th>Time</th>
                                            <th>In</th>
                                            <th>Out</th>

                                            <th>Tranfer </th>


                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptList" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td></td>
                                                    <td><%#Eval("Time")%></td>
                                                    <td><%#Convert.ToInt64(Eval("TotalAmountIn")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalIn")) %>)</td>
                                                    <td><%#Convert.ToInt64(Eval("TotalAmountOut")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalOut")) %>)</td>

                                                    <td><%#Convert.ToInt64(Eval("TotalAmountTranfer")).ToString("#,#").Replace(".", ",") %> (<%#Convert.ToInt32(Eval("TotalTranfer")) %>)</td>

                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <tr style="font-weight: bold">
                                            <td colspan="2">Tổng</td>
                                            <td>

                                                <asp:Label ID="lblTotalIn" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblTotalOut" runat="server" Text=""></asp:Label>
                                            </td>

                                            <td>

                                                <asp:Label ID="lblTotalTranfer" runat="server" Text=""></asp:Label>
                                            </td>

                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <input type="button" class="btn btn-info pull-right" value="export Excel" onclick="exportTableToExcel('TableResponsive', 'reportbank')" />
                    </div>
                    <!-- /.box-body -->
                </div>

            </div>
        </div>

        <!-- /.row -->
    </section>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <style type="text/css">
        table.table-bordered.dataTable tbody th, table.table-bordered.dataTable tbody td {
            vertical-align: middle !important;
            text-align: center;
        }

        table tr td, table tr th {
            vertical-align: middle !important;
            text-align: center;
        }

        table .right {
            text-align: right;
        }
    </style>
    <!-- bootstrap datepicker -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css">
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.txtBeginTime').datepicker({
                autoclose: true,
                Accepts: 'bottom'
            });
            $('.txtEndTime').datepicker({
                autoclose: true,
                horizontal: 'right',
                vertical: 'bottom'

            });

        });
    </script>
</asp:Content>

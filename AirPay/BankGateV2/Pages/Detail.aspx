<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="BankGateV2.Pages.Detail" %>

<!doctype html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Chi tiết giao dịch</title>

    <style>
        :root {
            --page-bg: #eef1f5;
            --acb-bg: #f2f3f5; /* xám nhạt giống ACB */
            --text: #333;
            --muted: #666;
            --line: #d8d8d8;
        }

        * {
            box-sizing: border-box
        }

        body {
            margin: 0;
            background: var(--page-bg);
            color: var(--text);
        }

        /* container */
        .wrap {
            max-width: 1100px;
            margin: 20px auto;
            padding: 0 16px 40px;
        }

        /* ================== ACB (giống web) ================== */
        .acb-wrapper {
            background: var(--acb-bg);
            padding: 28px 36px;
            border-radius: 10px;
            margin-bottom: 28px;
            font-family: "Segoe UI", Arial, sans-serif;
            color: #444;
        }

        .acb-header {
            text-align: center;
            font-weight: 600;
            font-size: 20px;
            color: #2e5fa4;
            margin-bottom: 18px;
            letter-spacing: .2px;
        }

        .acb-section {
            padding: 12px 0;
        }

            .acb-section + .acb-section {
                border-top: 1px solid var(--line);
            }

        .acb-grid {
            display: grid;
            grid-template-columns: 240px 1fr;
            gap: 8px 18px;
            font-size: 14px;
            line-height: 1.45;
        }

            .acb-grid .label {
                color: #666;
            }

            .acb-grid .value {
                color: #333;
            }

        .mono {
            font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;
            font-size: 13.2px;
        }

        .strong {
            font-weight: 600;
        }

        @media(max-width:800px) {
            .acb-grid {
                grid-template-columns: 1fr;
            }
        }

        /* ================== VPBANK (giống email) ================== */
        .vpb-card {
            background: #fff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 4px 14px rgba(0,0,0,.06);
        }

        /* Banner full ảnh (logo + gradient nằm trong ảnh) */
        .vpb-banner {
            height: 120px; /* chỉnh theo ảnh bạn */
            background-image: url("https://momo.opaps.info/Resources/images/logovpb.png?vs=2");
            background-size: cover;
            background-position: center left;
            background-repeat: no-repeat;
        }

        /* body VPB */
        .vpb-body {
            padding: 18px 26px 26px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 13px;
            color: #333;
        }

        /* phần thư */
        .vpb-letter {
            font-size: 13px;
            line-height: 1.6;
            color: #333;
            margin-bottom: 8px;
        }

            .vpb-letter .row {
                display: grid;
                grid-template-columns: 180px 1fr;
                gap: 10px;
                margin: 6px 0;
            }

            .vpb-letter .k {
                color: #333;
            }

            .vpb-letter .v {
                font-weight: 700;
            }

            .vpb-letter .en-ital {
                font-style: italic;
                color: #555;
                margin: 2px 0 10px;
            }

        /* bảng info */
        .vpb-info {
            margin-top: 10px;
        }

        .vpb-table {
            width: 100%;
            border-collapse: collapse;
        }

            .vpb-table td {
                vertical-align: top;
                padding: 8px 8px;
            }

            .vpb-table .vi-label {
                font-weight: 600;
                font-size: 13px;
                color: #222;
            }

            .vpb-table .en-label {
                margin-top: 4px;
                font-style: italic;
                font-size: 12px;
                color: #666;
            }

            .vpb-table .val {
                font-size: 13px;
                color: #333;
                word-break: break-word;
            }

                .vpb-table .val.mono {
                    font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;
                    font-size: 12.8px;
                }

            .vpb-table .spacer {
                width: 24px;
            }

        .vpb-divider {
            border-top: 1px solid #e5e7eb;
            margin-top: 12px;
        }

        /* responsive */
        @media (max-width: 860px) {
            .vpb-letter .row {
                grid-template-columns: 1fr;
            }

            .vpb-table, .vpb-table tbody, .vpb-table tr, .vpb-table td {
                display: block;
                width: 100%;
            }

                .vpb-table td.spacer {
                    display: none;
                }

                .vpb-table td {
                    padding: 10px 0;
                    border-bottom: 1px dashed #e5e7eb;
                }
        }
    </style>
</head>

<body>
    <div class="wrap">
        <asp:Panel ID="pn_hide_info" runat="server" Visible="false">
            <div class="row">
                <div class="p-5">
                    <h1>Không có thông tin giao dịch!</h1>
                </div>
            </div>
        </asp:Panel>
        <div id="bill">
            <!-- ================= ACB ================= -->
            <div class="acb-wrapper" runat="server" id="dvACB" visible="false">
                <div class="acb-header">Chi tiết giao dịch</div>

                <div class="acb-section">
                    <div class="acb-grid">
                        <div class="label">Số</div>
                        <div class="value">&nbsp;</div>

                        <div class="label">Ngày lập</div>
                        <div class="value"><%= BankInfo.LastTime.ToString("dd/MM/yyyy") %></div>

                        <div class="label">Trạng thái</div>
                        <div class="value">GD đã hoàn tất</div>

                        <div class="label">Tên đơn vị trả tiền</div>
                        <div class="value"><%=BankInfo.Mobile.Split('-')[2] %></div>

                        <div class="label">Tài khoản số</div>
                        <div class="value"><%=BankInfo.Mobile.Split('-')[1] %></div>

                        <div class="label">Tại ngân hàng</div>
                        <div class="value">ACB - PGD KIM DONG</div>
                    </div>
                </div>

                <div class="acb-section">
                    <div class="acb-grid">
                        <div class="label">Tên đơn vị nhận tiền</div>
                        <div class="value"><%= BankInfo.BankAccountName %></div>

                        <div class="label">Số CMND / Passport</div>
                        <div class="value">&nbsp;</div>

                        <div class="label">Ngày cấp</div>
                        <div class="value">&nbsp;</div>

                        <div class="label">Nơi cấp</div>
                        <div class="value">&nbsp;</div>

                        <div class="label">Tài khoản số</div>
                        <div class="value"><%= BankInfo.BankAccountNumber %></div>

                        <div class="label">Tại ngân hàng</div>
                        <div class="value"><%= BankInfo.BankCode %></div>
                    </div>
                </div>

                <div class="acb-section">
                    <div class="acb-grid">
                        <div class="label">Số tiền</div>
                        <div class="value"><%= Convert.ToInt64(BankInfo.Amount).ToString("#,#").Replace(".", ",") %></div>

                        <div class="label">Số tiền bằng chữ</div>
                        <div class="value"><%=DocTien( Convert.ToInt32(BankInfo.Amount))%></div>

                        <div class="label">Nội dung chuyển khoản</div>
                        <div class="value mono"><%= BankInfo.Note %>-<%= BankInfo.LastTime.ToString("ddMMyy-HH:mm:ss") %> </div>
                    </div>
                </div>
            </div>

            <!-- ================= VPBANK ================= -->
            <div class="vpb-card" runat="server" id="dvVPB" visible="false">
                <div class="vpb-banner"></div>

                <div class="vpb-body">
                    <div class="vpb-letter">
                        <div class="row">
                            <div class="k">Kính gửi Khách hàng:</div>
                            <div class="v"><%=BankInfo.Mobile.Split('-')[2] %></div>
                        </div>

                        <div class="en-ital">Dear Mr/Ms</div>

                        <div style="margin: 6px 0 10px;">
                            Ngân hàng Việt Nam Thịnh Vượng - VPBank xin trân trọng thông báo thông tin giao dịch Quý khách vừa thực hiện thành công qua dịch vụ ngân hàng số toàn năng của VPBank như sau:
                        </div>

                        <div class="en-ital" style="margin-top: 0;">
                            Vietnam Prosperity Joint Stock Commercial Bank; VPBank is pleased to inform that your transaction has been successfully implemented through Online Banking with details as below:
                        </div>
                    </div>

                    <div class="vpb-info">
                        <table class="vpb-table" aria-label="Chi tiết giao dịch VPBank">
                            <tr>
                                <td style="width: 32%;">
                                    <div class="vi-label">Mã giao dịch:</div>
                                    <div class="en-label">Transaction code</div>
                                </td>
                                <td style="width: 30%;">
                                    <div class="val mono"><%= BankInfo.OrderInfo %></div>
                                </td>

                                <td class="spacer"></td>

                                <td style="width: 22%;">
                                    <div class="vi-label">Ngày, giờ giao dịch:</div>
                                    <div class="en-label">Transaction date, time</div>
                                </td>
                                <td style="width: 16%;">
                                    <div class="val"><%= BankInfo.LastTime.ToString("dd/MM/yyyy HH:mm:ss") %></div>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <div class="vi-label">Tài khoản trích nợ</div>
                                    <div class="en-label">Debit Account</div>
                                </td>
                                <td>
                                    <div class="val mono"><%=BankInfo.Mobile.Split('-')[1] %></div>
                                </td>

                                <td class="spacer"></td>

                                <td>
                                    <div class="vi-label">Số tiền trích nợ:</div>
                                    <div class="en-label">Debit Amount</div>
                                </td>
                                <td>
                                    <div class="val"><%= Convert.ToInt64(BankInfo.Amount).ToString("#,#").Replace(".", ",") %> VND</div>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <div class="vi-label">Tài khoản ghi có:</div>
                                    <div class="en-label">Credit Account</div>
                                </td>
                                <td>
                                    <div class="val mono"><%= BankInfo.BankAccountNumber %></div>
                                </td>

                                <td class="spacer"></td>

                                <td>
                                    <div class="vi-label">Số tiền ghi có:</div>
                                    <div class="en-label">Credit Amount</div>
                                </td>
                                <td>
                                    <div class="val"><%= Convert.ToInt64(BankInfo.Amount).ToString("#,#").Replace(".", ",") %> VND</div>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <div class="vi-label">Tên người hưởng:</div>
                                    <div class="en-label">Beneficiary Name</div>
                                </td>
                                <td>
                                    <div class="val"><%= BankInfo.BankAccountName %></div>
                                </td>

                                <td class="spacer"></td>
                                <td>
                                    <div class="vi-label">&nbsp;</div>
                                    <div class="en-label">&nbsp;</div>
                                </td>
                                <td>
                                    <div class="val">&nbsp;</div>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <div class="vi-label">Loại phí:</div>
                                    <div class="en-label">Charge Code</div>
                                </td>
                                <td>
                                    <div class="val">Phí người chuyển trả</div>
                                    <div class="en-label" style="font-style: italic; color: #333; margin-top: 6px;">Exclude</div>
                                </td>

                                <td class="spacer"></td>

                                <td>
                                    <div class="vi-label">Số tiền phí:</div>
                                    <div class="en-label">Fee Amount</div>
                                </td>
                                <td>
                                    <div class="val">0 VND</div>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <div class="vi-label">Nội dung chuyển tiền:</div>
                                    <div class="en-label">Details of Payment</div>
                                </td>
                                <td colspan="4">
                                    <div class="val"><%= BankInfo.Note %></div>
                                </td>
                            </tr>
                        </table>

                        <div class="vpb-divider"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>

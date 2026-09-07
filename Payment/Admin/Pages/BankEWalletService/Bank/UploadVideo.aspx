<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UploadVideo.aspx.cs" MasterPageFile="~/Layout/Layout.master" Inherits="Pages_BankEWalletService_Bank_UploadVideo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/css/select2.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Cấu hình Bank
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Tài khoản Bank" CssClass="title"></asp:Label>
        </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Cập nhật tài khoản Bank</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Bank</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Tài khoản Bank</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.BankPartner %>">Quản lý kênh</a></li>
                <li><a href="<%=Constant.ADMIN_PATH + Resources.Url.PartnerBank %>">Phân bố kênh & tài khoản</a></li>
            </ul>

            <div class="row">
                <div class="col-xs-12">
                    <div class="nav-tabs-custom">
                        <!-- Tabs within a box -->
                        <ul class="nav nav-tabs pull-right ui-sortable-handle" id="accountTabs">

                            <li class="active"><a href="#account-video" data-toggle="tab">Video</a></li>
                            <li class=""><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankUploadImage%>?id=<%=lblId.Text%>">Ảnh</a> </li>
                            <li class=""><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankAccountEdit%>?id=<%=lblId.Text%>#balance-info">Chuyển tiền</a></li>
                            <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.BankAccountEdit%>?id=<%=lblId.Text%>#account-info">Thông tin tài khoản</a></li>
                            <li class="pull-left header"></li>
                        </ul>
                        <div class="tab-content">
                            <div class="chart tab-pane active" id="account-video">
                                <div class="box">

                                    <h2>
                                        <asp:Label runat="server" ID="lblBankInfo"></asp:Label></h2>
                                    <div style="display: none">
                                        <asp:Label runat="server" ID="lblId" Visible="true"></asp:Label></div>

                                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                    <br />
                                    <br />

                                    <!-- Upload nhiều file -->
                                    <asp:FileUpload ID="fuVideos" runat="server" AllowMultiple="true" />

                                    <asp:Button ID="btnUpload" runat="server"
                                        Text="Upload"
                                        CssClass="btn btn-upload"
                                        OnClick="btnUpload_Click" />

                                    <!-- Danh sách file -->
                                    <asp:Repeater ID="rptVideos" runat="server">
                                        <HeaderTemplate>
                                            <table>
                                                <tr>
                                                    <th>#</th>
                                                    <th>Tên file</th>
                                                    <%--<th>Preview</th>--%>
                                                    <th>Kích thước</th>
                                                    <th>Ngày</th>
                                                    <th>Xoá</th>
                                                </tr>
                                        </HeaderTemplate>

                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Container.ItemIndex + 1 %></td>

                                                <td><%# Eval("FileName") %></td>

                                                <%-- <td>
                                                    <video controls>
                                                        <source src='<%# Eval("FileUrl") %>' />
                                                        Trình duyệt không hỗ trợ video
                                                    </video>
                                                </td>--%>

                                                <td><%# Eval("FileSizeText") %></td>

                                                <td><%# Eval("LastWriteTimeText") %></td>

                                                <td>
                                                    <button type="button"
                                                        class="btn btn-delete"
                                                        onclick='deleteVideo("<%# Eval("FileName") %>")'>
                                                        Xoá
                                                    </button>
                                                </td>
                                            </tr>
                                        </ItemTemplate>

                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>

                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <!-- /.box -->
            </div>
            <!-- /.col -->
        </div>

        <!-- /.row -->
    </section>
    <!-- /.content -->

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <script type="text/javascript" src="<%=Constant.ADMIN_PATH %>Content/bower_components/select2/dist/js/select2.full.min.js"></script>
    <script src="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.pack.js"></script>
    <link href="<%=Constant.ADMIN_PATH %>Content/fancybox2.1.5/jquery.fancybox.css" rel="stylesheet" />


    <script type="text/javascript">
        function deleteVideo(fileName) {
            if (!confirm('Bạn có chắc muốn xoá file này không?')) return;

            var xhr = new XMLHttpRequest();
            xhr.open("POST", "/cmspay/ServiceHandler/DeleteVideo.ashx?bankid=<%=bankId %>&bankcode=<%=bankcode %>", true);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        var res = xhr.responseText;
                        if (res === "1") {
                            alert("Xoá file thành công");
                            location.reload();
                        } else {
                            alert("Xoá file thất bại: " + res);
                        }
                    } else {
                        alert("Có lỗi khi gọi server");
                    }
                }
            };

            xhr.send("fileName=" + encodeURIComponent(fileName));
        }
    </script>
    <style>
        .box {
            max-width: 900px;
            margin: 0 auto;
        }

        .message {
            padding: 10px;
            margin-bottom: 15px;
            border-radius: 4px;
            background: #f3f3f3;
        }

        .success {
            color: green;
        }

        .error {
            color: red;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

            table th, table td {
                border: 1px solid #ddd;
                padding: 8px;
            }

            table th {
                background: #f5f5f5;
            }

        .btn {
            padding: 6px 12px;
            cursor: pointer;
        }

        .btn-delete {
            color: white;
            background: #d9534f;
            border: none;
            border-radius: 4px;
        }

        .btn-upload {
            color: white;
            background: #0275d8;
            border: none;
            border-radius: 4px;
        }

        .video-link {
            text-decoration: none;
        }
    </style>
</asp:Content>

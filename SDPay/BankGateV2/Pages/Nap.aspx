<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Nap.aspx.cs" Inherits="BankGateV2.Pages.Nap" %>


<head>
    <title>Thông tin thanh toán</title>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700" />
    <!--end::Fonts-->
    <!--begin::Page Custom Styles(used by this page)-->
    <link rel="stylesheet" href="/Content/bower_components/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.2.1/css/all.min.css">
    <link rel="stylesheet" href="/Content/dist/css/AdminLTE.min.css">
    <link rel="stylesheet" href="/Content/dist/css/skins/_all-skins.min.css">

    <script src="/Content/bower_components/jquery/dist/jquery.min.js"></script>

    <!--end::Layout Themes-->
</head>
<body>
    <form id="form1" runat="server">

        <div class="content-wrapper" style="margin-left: 0px !important">
            <div class="container" style="padding: 20px 0;">

                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title">Tạo giao dịch thanh toán</h3>
                        </div>

                        <!-- /.box-header -->
                        <div class="box-body">

                            <div class="form-group row">

                                <div class="col-md-12">
                                    Lưu ý: mỗi một lệnh chỉ được chuyển tiền 1 lần.
                  <br />

                                </div>
                                <br />
                                <div style="clear: both; margin-bottom: 20px;"></div>

                                <div class="col-md-3">
                                    <asp:TextBox ID="txtAmount3" runat="server" CssClass="form-control"></asp:TextBox>

                                </div>

                                <div class="col-md-2" style="margin-left: 100px">
                                    <asp:Button ID="btAdd" runat="server" CssClass="btn btn-primary" OnClick="btAdd_Click" Text="Tạo lệnh"></asp:Button>
                                </div>
                            </div>
                            <br />

                            <div class="form-group" style="height: 200px; margin-top: 15px">
                                <div id="dvBankInfo" runat="server" visible="false">
                                    <div class="row">

                                        <div class="col-md-3">
                                            <div style="display: none">
                                                <input class="form-control" readonly="readonly" id="name" value="<%=MomoName %>" />
                                                <input class="form-control" readonly="readonly" id="account" value="<%=MomoId %>" />
                                            </div>
                                            <div style="margin-top: 20px; margin-left: 45px;">
                                                <dt>Ngân hàng</dt>
                                                <dd>

                                                    <asp:Label ID="Label1" runat="server" Text="MB Bank"></asp:Label>
                                                    &nbsp;
                                 
                                                </dd>
                                                <br />
                                                <dt>Tài khoản</dt>
                                                <dd>

                                                    <asp:Label ID="lbAccountNumber" runat="server" Text=""></asp:Label>
                                                    &nbsp;
                                 
                                                </dd>
                                                <br />
                                                <dt>Tên tài khoản</dt>
                                                <dd>

                                                    <asp:Label ID="lbAccountName" runat="server" Text=""></asp:Label>&nbsp;
                                 
                                                </dd>
                                                <br />
                                                <dt>Nội dung</dt>
                                                <dd>

                                                    <asp:Label ID="lbContent" runat="server" Text=""></asp:Label>&nbsp;
                                 
                                                </dd>
                                                <br />


                                            </div>

                                        </div>
                                        <div class="col-md-4">
                                            <div style="width: 300px; margin-left: 0px;">
                                                <asp:Image ID="qrCode" runat="server" Width="250" />

                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title">Danh sách giao dịch bank</h3>
                        </div>
                        <!-- /.box-header -->
                        <div class="box-body">
                            <div class="row">
                                <div class="col-xs-12">
                                    <div class="box">
                                        <div class="box-body no-padding">
                                            <div style="clear: both"></div>
                                            <div class="table-responsive">
                                                <table class="table table-striped" id="data3">
                                                    <thead>
                                                        <tr>
                                                            <th>Tài khoản nhận</th>
                                                            <th>Nội dung chuyển tiền</th>
                                                            <th>Số tiền</th>
                                                            <th>Số tiền thực nhận</th>
                                                            <th>CreatedTime</th>
                                                            <th>LastTime</th>
                                                            <th>Trạng thái</th>


                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater ID="rpBanklog" runat="server">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td><%#Eval("BankCode") %> - <%#Eval("BankAccountNumber") %> - <%#Eval("BankAccountName") %></td>
                                                                    <td><%#Eval("OrderNo") %></td>
                                                                    <td><%#Convert.ToInt64(Eval("Amount")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Convert.ToInt64(Eval("TotalAmount")).ToString("N0").Replace(".", ",") %></td>
                                                                    <td><%#Eval("CreatedTime", "{0:dd/MM HH:mm:ss}") %></td>
                                                                    <td><%#Eval("LastTime", "{0:dd/MM HH:mm:ss}") %></td>

                                                                    <td style="width: 80px"><%#GetStatusExtra(Eval("Status")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="clear: both"></div>
            </div>
        </div>
    </form>

    <script type="text/javascript">
        function copyStringToClipboard(id) {
            var str = document.getElementById(id).value;
            var el = document.createElement('textarea');
            el.value = str;
            el.setAttribute('readonly', '');
            el.style = {
                position: 'absolute',
                left: '-9999px'
            };
            document.body.appendChild(el);
            el.select();
            document.execCommand('copy');
            document.body.removeChild(el);
            var x = document.getElementById("snackbar");
            x.className = "show";
            setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
        }







    </script>




</body>

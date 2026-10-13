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
        <div class="container">
            <div class="col-md-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Tạo giao dịch momo</h3>
                    </div>

                    <!-- /.box-header -->
                    <div class="box-body">

                        <div class="form-group row">
                            <%-- <div class="col-xs-3 col-sm-6 col-md-4 col-lg-3">
                <asp:DropDownList CssClass="form-control" ID="drpBankCode" runat="server">
                      <asp:ListItem Text="--Chọn loại bank--" Value="1"></asp:ListItem>
                    <asp:ListItem Text="VCB" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </div>--%>
                            <div class="col-md-12">
                                Lưu ý: mỗi một lệnh chỉ được chuyển tiền 1 lần.
                  <br />
                                Chuyển thành công cần ấn nút tạo lệnh để lấy thông tin mới. Số tiền tối đa là 5.000.000
                            </div>
                            <br />
                            <div style="clear: both; margin-bottom: 20px;"></div>

                            <%--  <div class="col-md-3">
                  <asp:TextBox ID="txtAmount3" runat="server" CssClass="form-control"></asp:TextBox>
                 
              </div>--%>

                            <div class="col-md-2" style="margin-left: 100px">
                                <asp:Button ID="btAdd" runat="server" CssClass="btn btn-primary" OnClick="btAdd_Click" Text="Tạo lệnh"></asp:Button>
                            </div>
                        </div>
                        <br />

                        <div class="form-group" style="height: 295px; margin-top: 15px">
                            <div id="dvBankInfo" runat="server" visible="false">
                                <div class="row">

                                    <div class="col-md-3">
                                        <div style="display: none">
                                            <input class="form-control" readonly="readonly" id="name" value="<%=MomoName %>" />
                                            <input class="form-control" readonly="readonly" id="account" value="<%=MomoId %>" />
                                        </div>
                                        <div style="margin-top: 20px; margin-left: 45px;">
                                            <dt>Tài khoản</dt>
                                            <dd>

                                                <asp:Label ID="lbAccountNumber" runat="server" Text=""></asp:Label>
                                                &nbsp;
                                  <button type="button" class="btn" data-clipboard-target="#account>">
                                      <i class="fas fa-copy" onclick="copyStringToClipboard('account')"></i>
                                  </button>
                                            </dd>
                                            <br />
                                            <dt>Tên tài khoản</dt>
                                            <dd>

                                                <asp:Label ID="lbAccountName" runat="server" Text=""></asp:Label>&nbsp;
                                  <button type="button" class="btn" data-clipboard-target="#name>">
                                      <i class="fas fa-copy" onclick="copyStringToClipboard('name')"></i>
                                  </button>
                                            </dd>
                                            <br />
                                            <dt>Địa chỉ</dt>
                                            <dd>

                                                <asp:Label ID="lbInfo" runat="server" Text=""></asp:Label>
                                            </dd>
                                            <br />


                                        </div>

                                    </div>
                                    <div class="col-md-4">
                                        <div style="width: 300px; margin-left: 0px;">
                                            <asp:Image ID="qrCode" runat="server" Width="200" />

                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
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

    <script type="text/javascript">

        var refreshPageInterval = 120;

        setInterval("countDownPageRefresh()", 1000);//1 s gọi 1 lần
        $(document).mouseover(function () {
            funcResetRefreshPageInterval();
        });
        $(window).scroll(function () {
            funcResetRefreshPageInterval();
        });
        window.onkeypress = funcResetRefreshPageInterval;
        function funcResetRefreshPageInterval() {
            refreshPageInterval = 120;
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


</body>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="BankGateV2.Pages.Login" %>


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
                            <h3 class="box-title">Đăng nhập</h3>
                        </div>

                        <!-- /.box-header -->
                        <div class="login-box-body">
                            <p class="login-box-msg">Login</p>

                            <div class="form-group has-feedback">

                                <asp:TextBox ID="txtUserName" autocomplete="off" runat="server" CssClass="form-control" placeholder="Tên đăng nhập"></asp:TextBox>
                                <span class="glyphicon glyphicon-envelope form-control-feedback"></span>
                            </div>
                            <div class="form-group has-feedback">
                                <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" CssClass="form-control" placeholder="Mật khẩu"></asp:TextBox>
                                <span class="glyphicon glyphicon-lock form-control-feedback"></span>
                            </div>
                          
                            <div>

                                <asp:Panel ID="PanelMessage" runat="server" Visible="false">
                                    <asp:Label ID="lblMessage" runat="server" Text="Message if login failed"></asp:Label>
                                </asp:Panel>
                            </div>
                            <div class="row" style="padding: 20PX 0 25PX;">
                               
                                <!-- /.col -->
                                <div class="col-xs-4">
                                    <asp:Button ID="btSignIn" runat="server" CssClass="btn btn-warning btn-block btn-flat" Text="Đăng nhập" OnClick="btSignIn_Click"></asp:Button>
                                </div>
                                <!-- /.col -->
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




</body>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SignIn.aspx.cs" Inherits="Pages_Security_SignIn" %>


<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>TopPay| Login</title>
    <!-- Tell the browser to be responsive to screen width -->
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport">
    <!-- Bootstrap 3.3.7 -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap/dist/css/bootstrap.min.css">
    <!-- Font Awesome -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/font-awesome/css/font-awesome.min.css">
    <!-- Ionicons -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/bower_components/Ionicons/css/ionicons.min.css">
    <!-- Theme style -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/AdminLTE.min.css">
    <!-- iCheck -->
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/plugins/iCheck/square/blue.css">
    <link rel='shortcut icon' type='image/x-icon' href='/cmspay/favicon.ico' />
    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
  <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
  <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
  <![endif]-->

    <!-- Google Font -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600,700,300italic,400italic,600italic">
</head>
<body class="hold-transition login-page">
    <form id="form1" runat="server">
        <div class="login-box">
            <div class="login-logo">
                <a href="<%=Constant.ADMIN_PATH %>"><b>TopPay</b>CMS</a>
            </div>
            <!-- /.login-logo -->
            <div class="login-box-body">
                <p class="login-box-msg">Login</p>

                <div class="form-group has-feedback">

                    <asp:TextBox ID="txtUserName" autocomplete="off" runat="server" CssClass="form-control" placeholder="Account Name"></asp:TextBox>
                    <span class="glyphicon glyphicon-envelope form-control-feedback"></span>
                </div>
                <div class="form-group has-feedback">
                    <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" CssClass="form-control" placeholder="Password"></asp:TextBox>
                    <span class="glyphicon glyphicon-lock form-control-feedback"></span>
                </div>
                <div class="form-group has-feedback">

                    <asp:TextBox ID="txtOTP" autocomplete="off" runat="server" CssClass="form-control" placeholder="2FA Code (For Account Security Settings ) "></asp:TextBox>
                    <span class="glyphicon glyphicon-phone form-control-feedback"></span>
                </div>
                <div>

                    <asp:Panel ID="PanelMessage" runat="server" Visible="false">
                        <asp:Label ID="lblMessage" runat="server" Text="Message if login failed"></asp:Label>
                    </asp:Panel>
                </div>
                <div class="row" style="padding: 20PX 0 25PX;">
                    <div class="col-xs-8">
                        <div class="checkbox icheck">
                            <label>
                                <input type="checkbox">
                                Remember login
                            </label>
                        </div>
                    </div>
                    <!-- /.col -->
                    <div class="col-xs-4">
                        <asp:Button ID="btSignIn" runat="server" CssClass="btn btn-primary btn-block btn-flat" Text="Login" OnClick="btSignIn_Click"></asp:Button>
                    </div>
                    <!-- /.col -->
                </div>


            </div>
            <!-- /.login-box-body -->
        </div>
        <!-- /.login-box -->

        <!-- jQuery 3 -->
        <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/jquery/dist/jquery.min.js"></script>
        <!-- Bootstrap 3.3.7 -->
        <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
        <!-- iCheck -->
        <script src="<%=Constant.ADMIN_PATH %>Content/plugins/iCheck/icheck.min.js"></script>
        <script>
            $(function () {
                $('input').iCheck({
                    checkboxClass: 'icheckbox_square-blue',
                    radioClass: 'iradio_square-blue',
                    increaseArea: '20%' // optional
                });
            });
        </script>
        <style>
            #lblMessage
            {
                font-size:110%;
                font-weight:bold;
                color:red;
            }
        </style>
    </form>
</body>
</html>



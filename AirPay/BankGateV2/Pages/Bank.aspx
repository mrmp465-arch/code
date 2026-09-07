<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bank.aspx.cs" Inherits="BankGateV2.Pages.Bank" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Thông tin thanh toán</title>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700" />
    <!--end::Fonts-->
    <!--begin::Page Custom Styles(used by this page)-->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.2.1/css/all.min.css">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css" integrity="sha384-rbsA2VBKQhggwzxH7pPCaAqO46MgnOM80zW1RWuH61DGLwZJEdK2Kadq2F9CUG65" crossorigin="anonymous">

    <link href="sb_admin.css" rel="stylesheet" />
    <link rel="shortcut icon" type="image/x-icon" href="/favicon.ico?vs=20" />
    <style>
        .btn {
            color: #8a0054;
            font-size: 30px;
        }

        #snackbar {
            visibility: hidden;
            min-width: 250px;
            margin-left: -125px;
            background-color: #333;
            color: #fff;
            text-align: center;
            border-radius: 2px;
            padding: 16px;
            position: fixed;
            z-index: 1;
            left: 50%;
            bottom: 30px;
            font-size: 17px;
        }

            #snackbar.show {
                visibility: visible;
                -webkit-animation: fadein 0.5s, fadeout 0.5s 2.5s;
                animation: fadein 0.5s, fadeout 0.5s 2.5s;
            }

        @-webkit-keyframes fadein {
            from {
                bottom: 0;
                opacity: 0;
            }

            to {
                bottom: 30px;
                opacity: 1;
            }
        }

        @keyframes fadein {
            from {
                bottom: 0;
                opacity: 0;
            }

            to {
                bottom: 30px;
                opacity: 1;
            }
        }

        @-webkit-keyframes fadeout {
            from {
                bottom: 30px;
                opacity: 1;
            }

            to {
                bottom: 0;
                opacity: 0;
            }
        }

        @keyframes fadeout {
            from {
                bottom: 30px;
                opacity: 1;
            }

            to {
                bottom: 0;
                opacity: 0;
            }
        }

        #qrcode img {
            display: inline !important;
        }

        body, input, a, button {
            font-size: 25px;
        }

        .form-control {
            font-size: 25px;
            color: #000;
        }

        label {
            color: #000;
        }

        h1 {
            font-size: 40px;
        }


        @media screen and (max-width: 485px) {
            body, input, a, button {
                font-size: 25px !important;
            }

            .form-control {
                font-size: 25px;
            }

            h3 {
                font-size: 25px;
            }

            #qrcode img {
                width: 88% !important;
            }
        }
    </style>
    <!--end::Layout Themes-->
</head>
<body>
    <div id="snackbar">Copy thành công...</div>
    <div class="">

        <!-- Outer Row -->
        <div class="">

            <div class="container" style="padding: 15px 15px;">

                <div class="">
                    <div class="card-body p-0">
                        <!-- Nested Row within Card Body -->
                        <asp:Panel ID="pn_hide_info" runat="server" Visible="false">
                            <div class="row">
                                <div class="p-5">
                                    <h1>Giao dịch hết hiệu lực!</h1>
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pnSuccess" runat="server" Visible="false">
                            <div class="row">
                                <div class="">
                                    <h1 style="color: #00a65a">Giao dịch thành công</h1>
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pn_show_info" runat="server">
                            <div id="show_info" class="row" runat="server">
                                <div class="col-lg-5 " style="padding-top: 1rem; text-align: center;">
                                    <div>
                                        <image src="/airpay.jpg?vs=21" style="width: 30%" />
                                    </div>
                                    <div class="qrcode" id="qrcode">
                                        <img src="<%=QRCodeBase64 %>" style="width: 100%" />
                                    </div>

                                    <!--<img id="qr_image" src="" />-->
                                    <h3 id="countdown" style="color: crimson;"></h3>

                                </div>
                                <div class="col-lg-7" style="padding-top: 1rem; text-align: center;">

                                    <div class="">

                                        <div class="form-group" style="text-align: left;">
                                            <div>
                                                <label>Số tài khoản <%=BankCode %> nhận tiền</label>
                                                <button class="btn" style="float: right; padding-right: 0px;" onclick="copyStringToClipboard('account')">Copy</button>
                                            </div>
                                            <div class="input-group">

                                                <input class="form-control" readonly="readonly" id="account" value="<%=MomoId %>" />
                                                <div class="input-group-append">
                                                    <button type="button" class="btn" data-clipboard-target="#account">
                                                        <i class="fas fa-copy" onclick="copyStringToClipboard('account')"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group" style="text-align: left;">
                                            <div>
                                                <label>Tên tài khoản nhận tiền</label>
                                                <button class="btn" style="float: right; padding-right: 0px;" onclick="copyStringToClipboard('name')">Copy</button>
                                            </div>
                                            <div class="input-group">

                                                <input class="form-control" readonly="readonly" id="name" value="<%=MomoName %>" />
                                                <div class="input-group-append">
                                                    <button type="button" class="btn" data-clipboard-target="#name">
                                                        <i class="fas fa-copy" onclick="copyStringToClipboard('name')"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group" style="text-align: left;">
                                            <div>
                                                <label>Số tiền</label>
                                                <button class="btn" style="float: right; padding-right: 0px;" onclick="copyStringToClipboard('amount')">Copy</button>
                                            </div>
                                            <div class="input-group">

                                                <input class="form-control" readonly="readonly" id="amount" value="<%=Amount %>" />
                                                <div class="input-group-append">
                                                    <button type="button" class="btn" data-clipboard-target="#amount">
                                                        <i class="fas fa-copy" onclick="copyStringToClipboard('amount')"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group" style="text-align: left;">
                                            <div>
                                                <label>Nội dung</label>
                                                <button class="btn" style="float: right; padding-right: 0px;" onclick="copyStringToClipboard('message')">Copy</button>
                                            </div>
                                            <div class="input-group">

                                                <input class="form-control" readonly="readonly" id="message" value="<%=OrderNo %>" />
                                                <div class="input-group-append">
                                                    <button type="button" class="btn" data-clipboard-target="#message">
                                                        <i class="fas fa-copy" onclick="copyStringToClipboard('message')"></i>
                                                    </button>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12" style="padding-top: 1rem; text-align: center;">
                                    <span style="color: red;">Mỗi mã QRCode và nội dung chỉ được sử dụng 1 lần<br />
                                        Vui lòng nhập đúng nội dung, nếu sai sẽ không được cộng điểm</span>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>

        </div>

    </div>



</body>
<!--end::Body-->
<script src="https://cdnjs.cloudflare.com/ajax/libs/qrcodejs/1.0.0/qrcode.min.js">
</script>
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
<script>
    // Set the date we're counting down to
    var countDownDate = new Date("<%=ExpTime %>").getTime();
    var dateNow = new Date("<%=CurrentTime %>").getTime();
    var index = 0;
    // Update the count down every 1 second
    var x = setInterval(function () {

        // Get todays date and time
        //var now = new Date().getTime();
        var now = dateNow + index;
        index += 1000;
        // Find the distance between now and the count down date
        var distance = countDownDate - now;

        // Time calculations for days, hours, minutes and seconds
        //var days = Math.floor(distance / (1000 * 60 * 60 * 24));
        //var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        if (minutes != -1) {
            if (minutes <= 9) minutes = '0' + minutes;
            if (seconds <= 9) seconds = '0' + seconds;
            //dispTime = minutes + ':' + seconds;
        }
        var clocklocation = document.getElementById('countdown');
        clocklocation.innerHTML = minutes + ':' + seconds;
        //clearInterval(x);
        // Display the result in the element with id="demo"

        // If the count down is finished, write some text

        if (Math.floor(distance / 1000) < 0) {
            clocklocation.innerHTML = '00:00';
            clearInterval(x);
            //saveGame2();
            location.reload();
            return;
        }
    }, 1000);
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
</html>

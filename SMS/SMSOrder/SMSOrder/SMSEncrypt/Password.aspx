<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Password.aspx.cs" Inherits="SMSEncrypt.Password" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <script src="/Scripts/jquery-3.3.1.min.js"></script>
    <script src="/Scripts/crypto.js"></script>
</head>
<body>
   <script >
        function getUrlParameter(sParam) {
            var sPageURL = window.location.search.substring(1),
                sURLVariables = sPageURL.split('&'),
                sParameterName,
                i;

            for (i = 0; i < sURLVariables.length; i++) {
                sParameterName = sURLVariables[i].split('=');

                if (sParameterName[0] === sParam) {
                    return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
                }
            }
        }
</script>
<script>
   
    function getPassword()
    {
        var password = getUrlParameter('password');
        console.log(password);

        //var salt = getUrlParameter('salt');
        //console.log(salt);

        var v1 = getUrlParameter('v1');
        console.log(v1);

        var v2 = getUrlParameter('v2');
        console.log(v2);


        var passwordMd5 = CryptoJS.MD5(password);
        var passwordKey = CryptoJS.SHA256(CryptoJS.SHA256(passwordMd5 + v1) + v2);
        var encryptedPassword = CryptoJS.AES.encrypt(passwordMd5, passwordKey, { mode: CryptoJS.mode.ECB, padding: CryptoJS.pad.NoPadding });
        encryptedPassword = CryptoJS.enc.Base64.parse(encryptedPassword.toString()).toString(CryptoJS.enc.Hex);
        console.log(encryptedPassword);
        $("#encryptedPassword").html(encryptedPassword);
        //document.body=encryptedPassword
        document.title = encryptedPassword;
    }
</script>

<div id="encryptedPassword"></div>
     <asp:Label ID="Label1" runat="server"></asp:Label> 
      <asp:ScriptManager ID="ScriptManager1" runat="server" /> 
</body>
</html>

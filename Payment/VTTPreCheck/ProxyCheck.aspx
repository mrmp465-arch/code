<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProxyCheck.aspx.cs" Inherits="VTTPreCheck.ProxyCheck" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<%--<script type="text/javascript">
    <!--
    function scrollTextBoxDown(elementId)
    {
        var textRange = document.getElementById(elementId).createTextRange();
        textRange.collapse(false);
        textRange.select();
    }
// -->
</script>--%>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="txtResult" runat="server" Height="342px" TextMode="MultiLine" Width="973px"></asp:TextBox>
        </div>
        <asp:Button ID="btnStart" runat="server" OnClick="btnStart_Click" Text="Start" style="height: 26px" />
    </form>
</body>

</html>

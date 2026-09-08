<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="Layout_Header" %>
<%--<header id="top">
    <div class="container_12 clearfix">
	    <div id="logo" class="grid_6">
		    <!-- replace with your website title or logo -->
		    <a id="site-title" href="dashboard.html">Admin</a>
		    <a id="view-site" href="#">View Site</a>
	    </div>
	    <div id="userinfo" class="grid_6">
		    Xin chào, <a href="#"></a>
	    </div>
    </div>
</header>--%>

<li class="dropdown user user-menu">
    <a href="#" class="dropdown-toggle" data-toggle="dropdown">
        <img src="<%=Constant.ADMIN_PATH %>Content/dist/img/luffy.png" class="user-image" alt="User Image">
        <span class="hidden-xs">
            <asp:Label ID="lblName" runat="server" Text=""></asp:Label></span>
    </a>
    <ul class="dropdown-menu">
        <!-- User image -->
        <li class="user-header">
            <img src="<%=Constant.ADMIN_PATH %>Content/dist/img/luffy.png" class="img-circle" alt="User Image">

            <%--<p>
                Alexander Pierce - Web Developer
                 
                <small>Member since Nov. 2012</small>
            </p>--%>
        </li>
        <!-- Menu Footer-->
        <li class="user-footer">
            <div class="pull-left">
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ChangePassword %>" class="btn btn-default btn-flat">Đổi Mật Khẩu</a>
            </div>
            <div class="pull-right">
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.SignOut %>" class="btn btn-default btn-flat">Thoát</a>
            </div>
        </li>
    </ul>
</li> 
 
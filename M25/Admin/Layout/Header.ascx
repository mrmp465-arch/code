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
        <img src="<%=Constant.ADMIN_PATH %>Content/dist/img/avatar5.png" class="user-image" alt="User Image">
        <span class="hidden-xs">
            <asp:Label ID="lblName" runat="server" Text=""></asp:Label></span>
    </a>
    <ul class="dropdown-menu">
        <!-- User image -->
        <li class="user-header">
            <img src="<%=Constant.ADMIN_PATH %>Content/dist/img/avatar5.png" class="img-circle" alt="User Image">

            <%--<p>
                Alexander Pierce - Web Developer
                 
                <small>Member since Nov. 2012</small>
            </p>--%>
        </li>
        <!-- Menu Footer-->
        <% if (Lang == "vi-vn")
    { %>
        <li class="user-footer">
            <div class="pull-left">
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ChangePassword %>" class="btn btn-default btn-flat">Đổi Mật Khẩu</a>
            </div>
            <div class="pull-right">
                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.SignOut %>" class="btn btn-default btn-flat">Thoát</a>
            </div>
        </li>
        <%}else {  %>
          <li class="user-footer">
      <div class="pull-left">
          <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ChangePassword %>" class="btn btn-default btn-flat">更改密码</a>
      </div>
      <div class="pull-right">
          <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.SignOut %>" class="btn btn-default btn-flat">出口</a>
      </div>
  </li>
        <% } %>
    </ul>
</li>
<% if (Lang == "vi-vn")
    { %>
<li>
    <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.Lang %>?lang=en-us"><img src="<%=Constant.ADMIN_PATH %>Content/dist/img/china.png?vs=02" height="18" /></a>
</li>
<%}
else {  %>
<li>
    <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.Lang %>?lang=vi-vn"><img src="<%=Constant.ADMIN_PATH %>Content/dist/img/vietnam.png?vs=02" height="18" /></a>
</li>
<% } %>
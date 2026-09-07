<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TopMenu.ascx.cs" Inherits="Layout_TopMenu" %>

<style type="text/css">
  .sidebar-menu .fa-icon-1:before{content:"\f1ea"}  .sidebar-menu .fa-icon-2:before{content:"\f09d"}.sidebar-menu .fa-icon-3:before{content:"\f1ea"}.sidebar-menu .fa-icon-4:before{content:"\f0d6"}.sidebar-menu .fa-icon-5:before{content:"\f0d6"}.sidebar-menu .fa-icon-6:before{content:"\f1d7"}.sidebar-menu .fa-icon-7:before{content:"\f02d"}.sidebar-menu .fa-icon-8:before{content:"\f19c"}.sidebar-menu .fa-icon-9:before{content:"\f013"}.sidebar-menu .fa-icon-10:before{content:"\f0c0"}.fa-icon-11:before{content:"\f0c9"}.fa-icon-12:before{content:"\f283"}
</style>
<ul class="sidebar-menu" data-widget="tree">
    <li class="header">Directional</li> 
    <%  foreach (var item in lisUrl){%> 
            <li class="treeview <%= GetUrl2(item.list, "menu-open") %>">          
                <a href="#">
                    <i class="fa  fa-icon-<%=item.item.Group %>"></i><span><%=GetGroupName(Lang,item.item.GroupName) %></span>
                    <span class="pull-right-container">
                        <i class="fa fa-angle-left pull-right"></i>
                    </span>
                </a>
                <ul class="treeview-menu" <%= GetUrl2(item.list,    "style='display: block;'") %>>
                    <%  foreach (var item2 in item.listMenu){%>
                        <li class="<%= GetUrl(lstUrlResources,item2.Url, item2.Id,"active") %>"><a href="<%=Constant.ADMIN_PATH %><%=item2.Url %>"><i class="fa fa-circle-o"></i><%= GetMenuName(Lang,item2.Name)  %></a></li>
                    <%} %> 
                </ul> 
            </li>
    <%} %> 
</ul>

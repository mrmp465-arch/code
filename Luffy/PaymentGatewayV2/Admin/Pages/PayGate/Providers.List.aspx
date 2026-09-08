<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Providers.List.aspx.cs" Inherits="Pages_PayGate_Provider_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Kết nối
        <small>
            <asp:Label ID="lblTtitle" runat="server" Text="Danh sách nhà cung cấp" CssClass="title"></asp:Label></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Nhà cung cấp</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Nhà cung cấp</li>
                <li class="<% if (Request["Type"] == "7" || Request["Type"] == null)
                    {%>active <% }%>"><a href="#gach-the" data-toggle="tab">Gạch thẻ</a></li>
                <li class="<% if (Request["Type"] == "14")
                    {%>active <% }%>"><a href="#mua-the" data-toggle="tab">Mua mã thẻ & Topup</a></li>
                <li class="<% if (Request["Type"] == "13")
                    {%>active <% }%>"><a href="#ngan-hang" data-toggle="tab">Ngân hàng</a></li>
                <li class="<% if (Request["Type"] == "18")
                    {%>active <% }%>"><a href="#bankcash" data-toggle="tab">Ngân hàng Cash</a></li>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane <% if (Request["Type"] == "7" || Request["Type"] == null)
                    {%>active <% }%>" id="gach-the">
                    <div  class="box-header with-border <% if (Request["Type"] == "7" || Request["Type"] == null)
                    {%>show <% }%>">
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpCardType" runat="server" CssClass="form-control " OnTextChanged="drpCardType_TextChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control " OnTextChanged="drpStatus_TextChanged" AutoPostBack="true">
                                <asp:ListItem Value="-1">--Trạng thái--</asp:ListItem>
                                <asp:ListItem Value="1" >Active</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive1">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">STT </th>
                                    <th>Tên nhà cung cấp </th>
                                    <th>Mã nhà cung cấp</th>
                                    <th>SignatureType</th>
                                    <th>ProductCode</th>
                                    <th>Quota</th>
                                    <th>Occurs</th>
                                    <th>Mức độ ưu tiên</th>
                                    <th>Kích hoạt</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEdit %>?id=<%#Eval("ProviderId") %>"><%#Eval("Name") %></a></td>
                                            <td><%#Eval("ProviderCode") %></td>
                                            <td><%#Eval("SignatureType") %></td>
                                            <td><%#Eval("ProductCode") %></td>
                                            <td>
                                                <asp:TextBox ID="txtQuota" placeholder="Nhập giới hạn doanh số" runat="server" CssClass="form-control txtQuota" Text='<%# Convert.ToInt64(DataBinder.Eval(Container.DataItem, "Quota").ToString()) / 1000 %>'></asp:TextBox> </td>
                                            <td><%#GetOccurs(Eval("Occurs").ToString())%> </td>
                                            <td>
                                                <asp:TextBox ID="txtOrderNo" placeholder="Nhập thứ tự ưu tiên" runat="server" CssClass="form-control txtOrderNo" Text='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'></asp:TextBox>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblProviderId" runat="server" Visible="false" Text='<%#Eval("ProviderId") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProvidersDelete %>?id=<%#Eval("ProviderId") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="Button4" runat="server" Text="Thêm mới" CssClass="btn btn-info " OnClick="btAdd_Click1"></asp:Button>
                        <asp:Button ID="Button5" runat="server" Text="Update" CssClass="btn btn-info pull-right " OnClick="btApply_Click1"></asp:Button>
                    </div>
                </div>
                <div class="chart tab-pane <% if (Request["Type"] == "14")
                    {%>active <% }%>" id="mua-the">
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive2">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">ID </th>
                                    <th>Tên nhà cung cấp </th>
                                    <th>Mã nhà cung cấp</th>
                                    <th>SignatureType</th>
                                    <th>ProductCode</th>
                                    <th>Quota</th>
                                    <th>Occurs</th>
                                    <th>Mức độ ưu tiên</th>
                                    <th>Kích hoạt</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList2" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEdit %>?id=<%#Eval("ProviderId") %>"><%#Eval("Name") %></a></td>
                                            <td><%#Eval("ProviderCode") %></td>
                                            <td><%#Eval("SignatureType") %></td>
                                            <td><%#Eval("ProductCode") %></td>
                                            <td>
                                                <asp:TextBox ID="txtQuota" placeholder="Nhập giới hạn doanh số" runat="server" CssClass="form-control txtQuota" Text='<%# Convert.ToInt64(DataBinder.Eval(Container.DataItem, "Quota").ToString()) / 1000000 %>'></asp:TextBox> </td>
                                            <td><%#GetOccurs(Eval("Occurs").ToString())%> </td>
                                            <td>
                                                <asp:TextBox ID="txtOrderNo" placeholder="Nhập thứ tự ưu tiên" runat="server" CssClass="form-control txtOrderNo" Text='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'></asp:TextBox>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblProviderId" runat="server" Visible="false" Text='<%#Eval("ProviderId") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProvidersDelete %>?id=<%#Eval("ProviderId") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="Button2" runat="server" Text="Thêm mới" CssClass="btn btn-info " OnClick="btAdd_Click2"></asp:Button>
                        <asp:Button ID="Button3" runat="server" Text="Update" CssClass="btn btn-info pull-right " OnClick="btApply_Click2"></asp:Button>
                    </div>
                </div>

                <div class="chart tab-pane <% if (Request["Type"] == "13")
                    {%>active <% }%>" id="ngan-hang">
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive3">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">ID </th>
                                    <th>Tên nhà cung cấp </th>
                                    <th>Mã nhà cung cấp</th>
                                    <th>SignatureType</th>
                                    <th>ProductCode</th>
                                    <th>Quota</th>
                                    <th>Occurs</th>
                                    <th>Mức độ ưu tiên</th>
                                    <th>Kích hoạt</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList3" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEdit %>?id=<%#Eval("ProviderId") %>"><%#Eval("Name") %></a></td>
                                            <td><%#Eval("ProviderCode") %></td>
                                            <td><%#Eval("SignatureType") %></td>
                                            <td><%#Eval("ProductCode") %></td>
                                            <td>
                                                <asp:TextBox ID="txtQuota" placeholder="Nhập giới hạn doanh số" runat="server" CssClass="form-control txtQuota" Text='<%# Convert.ToInt64(DataBinder.Eval(Container.DataItem, "Quota").ToString()) / 1000 %>'></asp:TextBox> </td>
                                            <td><%#GetOccurs(Eval("Occurs").ToString())%> </td>
                                            <td>
                                                <asp:TextBox ID="txtOrderNo" placeholder="Nhập thứ tự ưu tiên" runat="server" CssClass="form-control txtOrderNo" Text='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'></asp:TextBox>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblProviderId" runat="server" Visible="false" Text='<%#Eval("ProviderId") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProvidersDelete %>?id=<%#Eval("ProviderId") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="btAdd" runat="server" Text="Thêm mới" CssClass="btn btn-info " OnClick="btAdd_Click3"></asp:Button>
                        <asp:Button ID="Button1" runat="server" Text="Update" CssClass="btn btn-info pull-right " OnClick="btApply_Click3"></asp:Button>
                    </div>
                </div>
                <div class="chart tab-pane <% if (Request["Type"] == "18")
                    {%>active <% }%>" id="bankcash">
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive5">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">ID </th>
                                    <th>Tên nhà cung cấp </th>
                                    <th>Mã nhà cung cấp</th>
                                    <th>SignatureType</th>
                                    <th>ProductCode</th>
                                    <th>Quota</th>
                                    <th>Occurs</th>
                                    <th>Mức độ ưu tiên</th>
                                    <th>Kích hoạt</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList5" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEdit %>?id=<%#Eval("ProviderId") %>"><%#Eval("Name") %></a></td>
                                            <td><%#Eval("ProviderCode") %></td>
                                            <td><%#Eval("SignatureType") %></td>
                                            <td><%#Eval("ProductCode") %></td>
                                            <td>
                                                <asp:TextBox ID="txtQuota" placeholder="Nhập giới hạn doanh số" runat="server" CssClass="form-control txtQuota" Text='<%# Convert.ToInt64(DataBinder.Eval(Container.DataItem, "Quota").ToString()) / 1000 %>'></asp:TextBox> </td>
                                            <td><%#GetOccurs(Eval("Occurs").ToString())%> </td>
                                            <td>
                                                <asp:TextBox ID="txtOrderNo" placeholder="Nhập thứ tự ưu tiên" runat="server" CssClass="form-control txtOrderNo" Text='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'></asp:TextBox>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblProviderId" runat="server" Visible="false" Text='<%#Eval("ProviderId") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProvidersDelete %>?id=<%#Eval("ProviderId") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="Button8" runat="server" Text="Thêm mới" CssClass="btn btn-info " OnClick="btAdd_Click5"></asp:Button>
                        <asp:Button ID="Button9" runat="server" Text="Update" CssClass="btn btn-info pull-right " OnClick="btApply_Click5"></asp:Button>
                    </div>
                </div>
                <div class="chart tab-pane <% if (Request["Type"] == "17")
                    {%>active <% }%>" id="sms">
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive4">
                            <thead>
                                <tr>
                                    <th style="width: 10px;">ID </th>
                                    <th>Tên nhà cung cấp </th>
                                    <th>Mã nhà cung cấp</th>
                                    <th>SignatureType</th>
                                    <th>ProductCode</th>
                                    <th>Quota</th>
                                    <th>Occurs</th>
                                    <th>Mức độ ưu tiên</th>
                                    <th>Kích hoạt</th>
                                    <th>Tác vụ</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptList4" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td></td>
                                            <td><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEdit %>?id=<%#Eval("ProviderId") %>"><%#Eval("Name") %></a></td>
                                            <td><%#Eval("ProviderCode") %></td>
                                            <td><%#Eval("SignatureType") %></td>
                                            <td><%#Eval("ProductCode") %></td>
                                            <td>
                                                <asp:TextBox ID="txtQuota" placeholder="Nhập giới hạn doanh số" runat="server" CssClass="form-control txtQuota" Text='<%# Convert.ToInt64(DataBinder.Eval(Container.DataItem, "Quota").ToString()) / 1000000 %>'></asp:TextBox> </td>
                                            <td><%#GetOccurs(Eval("Occurs").ToString())%> </td>
                                            <td>
                                                <asp:TextBox ID="txtOrderNo" placeholder="Nhập thứ tự ưu tiên" runat="server" CssClass="form-control txtOrderNo" Text='<%# DataBinder.Eval(Container.DataItem, "OrderNo") %>'></asp:TextBox>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server" Checked='<%#Convert.ToBoolean(Eval("Status")) %>'></asp:CheckBox>&nbsp;
                                                 <asp:Label ID="lblProviderId" runat="server" Visible="false" Text='<%#Eval("ProviderId") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProvidersDelete %>?id=<%#Eval("ProviderId") %>" onclick="return confirm('Bạn có muốn xóa?')" title="Xóa">Xóa</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="Button6" runat="server" Text="Thêm mới" CssClass="btn btn-info " OnClick="btAdd_Click4"></asp:Button>
                        <asp:Button ID="Button7" runat="server" Text="Update" CssClass="btn btn-info pull-right " OnClick="btApply_Click4"></asp:Button>
                    </div>
                </div>

            </div>
        </div>
    </section>
    <!-- /.content -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
        $(function () {
            $(".txtOrderNo").attr("type", "number");
            $(".txtQuota").attr("type", "number");
        });
        $(document).ready(function () {
            var table = $('#TableResponsive1,#TableResponsive2,#TableResponsive3,#TableResponsive4,#TableResponsive5').DataTable({
                responsive: true
                , "autoWidth": false, "paging": false, "searching": false, "info": false, "ordering": false
            });
            new $.fn.dataTable.FixedHeader(table);
        });
    </script>
    <style>
        .txtOrderNo {
            max-width: 70px;
        }
        .box-header
        {
            display:none;
        }

        .show{
            display:block;
        }
    </style>
</asp:Content>

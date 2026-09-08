<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Partners.Edit.aspx.cs" Inherits="Pages_PayGate_Partners_Edit" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header)  -->
    <section class="content-header">
        <h1>Kết nối
        <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersList %>">Danh sách đối tác</a>  </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Cập nhật đối tác
                    <asp:Label ID="lblPartner" runat="server"></asp:Label>
                    -
                    <asp:Label ID="lblPartnerId" runat="server"></asp:Label>
                </li>
                <li class="<% if (Request["type"] == "info" || Request["type"] == null)
                    {%>active <% }%>"><a href="#revenue-chart" data-toggle="tab">Thông Tin</a></li>
                <li class="<% if (Request["type"] == "service")
                    {%>active <% }%>"><a href="#sales-chart" data-toggle="tab">Dịch vụ</a></li>
                <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.PartnersEditDiscount%>?id=<%=lblPartnerId.Text%>&code=<%=lblPartner.Text%>">Chiết khấu</a></li>
            </ul>
            <div class="tab-content no-padding">
                <!-- Morris chart - Sales -->
                <div class="chart tab-pane <% if (Request["Type"] == "info" || Request["Type"] == null)
                    {%>active <% }%>"
                    id="revenue-chart">
                    <!-- SELECT2 EXAMPLE -->
                    <div class="box-body">
                        <div class="row">

                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtName">Tên đối tác *</label>
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên đối tác *"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtPartnerCode">Mã đối tác *</label>
                                    <asp:TextBox ID="txtPartnerCode" runat="server" CssClass="form-control" placeholder="Mã đối tác *"></asp:TextBox>
                                </div>


                                <div>
                                    <div class="form-group">
                                        <label for="txtSMSCommand">SMSCommand </label>
                                        <asp:TextBox ID="txtSMSCommand" runat="server" CssClass="form-control" placeholder="SMSCommand"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtSMSUrl">SMSUrl</label>
                                        <asp:TextBox ID="txtSMSUrl" runat="server" CssClass="form-control" placeholder="SMSUrl"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtSMSPlusCommand">SMSPlusCommand</label>
                                        <asp:TextBox ID="txtSMSPlusCommand" runat="server" CssClass="form-control" placeholder="SMSPlusCommand"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtSMSPlusCheckUrl">SMSPlusCheckUrl</label>
                                        <asp:TextBox ID="txtSMSPlusCheckUrl" runat="server" CssClass="form-control" placeholder="SMSPlusCheckUrl"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtSMSPlusUrl">Momo Callback</label>
                                        <asp:TextBox ID="txtSMSPlusUrl" runat="server" CssClass="form-control" placeholder="Momo Callback"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtHotline">Hotline</label>
                                        <asp:TextBox ID="txtHotline" runat="server" CssClass="form-control" placeholder="Hotline"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <!-- /.col -->
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="drpSignatureType">Loại chữ ký *</label>
                                    <asp:DropDownList ID="drpSignatureType" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="MD5" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="RSA" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="SHA256" Value="3"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtClassName">PrivateKey *</label>
                                    <asp:TextBox ID="txtPrivateKey" runat="server" TextMode="MultiLine" Height="90" CssClass="form-control" placeholder="PrivateKey *"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtPublicKey">PublicKey *</label>
                                    <asp:TextBox ID="txtPublicKey" runat="server" TextMode="MultiLine" Height="90" CssClass="form-control" placeholder="PublicKey *"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtPublicKey">Provider</label>
                                    <div class="row">
                                        <div class="col-md-4">
                                            <asp:DropDownList ID="drpCardType" runat="server" CssClass="form-control " OnTextChanged="drpCardType_TextChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-8">
                                            <asp:Label runat="server" ID="lblProvider" />
                                        </div>
                                    </div>

                                    
                                </div>
                                <div class="checkbox">
                                    <label for="txtName">
                                        <asp:CheckBox ID="chkIsActive" Checked="true" runat="server"></asp:CheckBox>Trạng thái
                                    </label>
                                </div>
                            </div>
                            <!-- /.col -->
                        </div>
                        <!-- /.row -->
                    </div>

                    <div class="box-footer">
                        <asp:Button ID="btUpdate" runat="server" Text="CẬP NHẬT" CssClass="btn btn-info " OnClick="btUpdate_Click"></asp:Button>
                        <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
                    </div>
                </div>
                <div class="chart tab-pane <% if (Request["Type"] == "service")
                    {%>active <% }%>"
                    id="sales-chart">
                    <div class="box-body no-padding">
                        <table class="table table-condensed">
                            <tbody>
                                <tr>
                                    <th style="text-align: center; font-weight: bold;">#</th>
                                    <th>Dịch vụ</th>
                                    <th>Quota</th>
                                    <th>Occurs</th>
                                    <th>CommandCode</th>
                                    <th>IP truy cập</th>
                                    <th>Trạng thái</th>
                                </tr>
                                <asp:Repeater ID="rptList" runat="server">
                                    <ItemTemplate>

                                        <tr>
                                            <td class="openclass" data-id="<%#Eval("ServiceID") %>" style="text-align: center; font-weight: bold; cursor: pointer">+</td>
                                            <td><%#Eval("Name") %></td>
                                            <td>
                                                <asp:TextBox ID="txtQuota" runat="server" CssClass="form-control">0</asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpOccurs" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="Giờ" Value="4"></asp:ListItem>
                                                    <asp:ListItem Text="Ngày" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Tuần" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Tháng" Value="3"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCommandCode" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtIPAddress" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:CheckBox ID="cbxStatus" runat="server"></asp:CheckBox>&nbsp;<asp:Label ID="lblServiceID" Visible="false" runat="server" Text='<%#Eval("ServiceID") %>'></asp:Label></td>
                                        </tr>
                                        <tr class="LastIndex LastIndex<%#Eval("ServiceID") %>" style="width: 100%; min-width: 300px">
                                            <td></td>
                                            <td colspan="7">Lựa chọn Provider:
                                               
                                                <div style="margin-left: -15px; margin-top: 10px;">
                                                    <asp:Repeater ID="rptProList" runat="server" DataSource='<%#ListProviders(Eval("ServiceID")) %>'>
                                                        <ItemTemplate>
                                                            <div class="col-xs-4 col-md-2" style="margin-bottom: 10px;">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">
                                                                        <asp:CheckBox ID="cbxProStatus" runat="server"></asp:CheckBox>
                                                                        <asp:HiddenField ID="txtProCode" Value='<%#Eval("ProviderCode") %>' runat="server"></asp:HiddenField>
                                                                        <asp:HiddenField ID="txtProId" Value='<%#Eval("ProviderId") %>' runat="server"></asp:HiddenField>
                                                                    </span>
                                                                    <asp:Label ID="lblName" CssClass="form-control" runat="server"><%#Eval("Name") %></asp:Label>

                                                                </div>
                                                            </div>

                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <asp:Button ID="btApply" runat="server" Text="CẬP NHẬT" CssClass="btn btn-info " OnClick="btApply_Click"></asp:Button>
                    </div>
                </div>

            </div>
        </div>
    </section>
    <style type="text/css">
        .LastIndex {
            display: none;
        }
    </style>
    <script type="text/javascript">  
        $(document).ready(function () {
            $(".openclass").click(function () {
                if (!$(this).hasClass("open")) {
                    $(this).addClass("open").html("-");
                    $(".LastIndex" + $(this).data("id")).css("display", "table-row");
                } else {
                    $(".LastIndex" + $(this).data("id")).css("display", "none");
                    $(this).removeClass("open").html("+");
                }
            });
        });

    </script>
</asp:Content>


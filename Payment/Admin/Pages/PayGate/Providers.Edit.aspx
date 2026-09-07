<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Providers.Edit.aspx.cs" Inherits="Pages_PayGate_Provider_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>Kết nối
            <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderList %>">Danh sách nhà cung cấp
            </a></small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Nhà cung cấp chi tiết</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>
                    Sửa nhà cung cấp <asp:Label ID="lblProviderCode" runat="server"></asp:Label> - <asp:Label ID="lblProviderId" runat="server"></asp:Label>
                    <%-- <%if (Type == "1")
                         {%>
                        Thẻ
                    <% }
                        else if (Type == "1")
                        { %> 
                            Mua Thẻ & Topup
                    <% }
                        else if (Type == "1")
                        { %>
                        SMS
                    <% }%> --%>
                </li>
                <li class="<% if (Request["tab"] == "info" || Request["tab"] == null) {%>active <% }%>"><a href="#revenue-chart" data-toggle="tab">Thông Tin</a></li>
                <li class="<% if (Request["tab"] == "product") {%>active <% }%>"><a href="#sales-chart" data-toggle="tab">Sản phẩm</a></li>
                <li class="<% if (Request["tab"] == "partner") {%>active <% }%>"><a href="#tabPartners" data-toggle="tab">Đối tác</a></li>
                <li><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderEditDiscount%>?id=<%=lblProviderId.Text%>&code=<%=lblProviderCode.Text%>">Chiết khấu</a></li>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane <% if (Request["tab"] == "info"||Request["tab"] == null) {%>active <% }%>" id="revenue-chart">
                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtName">Tên nhà cung cấp </label>
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên dịch vụ *"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtProviderCode">Mã nhà cung cấp</label>
                                    <asp:TextBox ID="txtProviderCode" runat="server" CssClass="form-control" placeholder="ProviderCode"></asp:TextBox>
                                </div>
                               
                                <div class="form-group">
                                    <label for="PrivateKey">PrivateKey</label>
                                    <asp:TextBox ID="txtPrivateKey" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="PrivateKey"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtPublicKey">PublicKey</label>
                                    <asp:TextBox ID="txtPublicKey" runat="server" TextMode="MultiLine" CssClass="form-control txtPublicKey" placeholder="PublicKey"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtPublicKey">Mệnh giá</label>
                                    <asp:TextBox ID="txtAmount" runat="server"  CssClass="form-control txtAmount" placeholder="Mệnh giá"></asp:TextBox>
                                </div>
                            </div>
                            <!-- /.col -->
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtOrderNo">Mức độ ưu tiên</label>
                                    <asp:TextBox ID="txtOrderNo" runat="server" CssClass="form-control txtOrderNo" placeholder="OrderNo"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtQuota">Quota (giá trị 0 không giới hạn)</label>
                                    <asp:TextBox ID="txtQuota" runat="server" CssClass="form-control txtQuota" placeholder="Quota"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtType">Loại</label>
                                    <asp:DropDownList ID="txtType" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Thẻ" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="Mua Thẻ & Topup" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="Ngân Hàng" Value="13"></asp:ListItem>
                                         <asp:ListItem Text="Ngân Hàng Cash" Value="18"></asp:ListItem>
                                        <asp:ListItem Text="SMS" Value="17"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtOccurs">Occurs</label>
                                    <asp:DropDownList ID="drpOccurs" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Giờ" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="Ngày" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Tuần" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Tháng" Value="3"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group" style="display:none">
                                    <label for="txtGSMUrl">GSM Url</label>
                                        <asp:TextBox ID="txtGSMUrl" runat="server" CssClass="form-control txtGSMUrl" placeholder="GSM Url"></asp:TextBox>
                                    
                                </div>
                                 <div class="form-group">
                                    <label for="txtSignatureType">Maintain</label>
                                    <asp:TextBox ID="txtSignatureType" runat="server" CssClass="form-control" placeholder="SignatureType"></asp:TextBox>
                                </div>
                                <div class="checkbox">
                                    <label for="txtName">
                                        <asp:CheckBox ID="chkIsActive" Checked="true" runat="server"></asp:CheckBox>Kích hoạt
                                    </label>
                                </div>
                            </div>
                            <!-- /.col -->
                        </div>
                        <!-- /.row -->
                    </div>

                </div>
                <div class="chart tab-pane <% if (Request["tab"] == "product") {%>active <% }%>" id="sales-chart">
                    <div class="box-body no-padding" style="padding-top: 15px !important">
                        <asp:Repeater ID="rptList" runat="server">
                            <ItemTemplate> 
                                <div class="col-md-3 col-sm-6 col-xs-12" style="padding-bottom:10px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxProCode" runat="server" Checked='<%# GetCheckCardType(Eval("Code")+"") %>'></asp:CheckBox> 
                                                <asp:HiddenField ID="txtProCode" Value='<%#Eval("code") %>' runat="server"></asp:HiddenField>
                                        </span>
                                         <asp:Label  ID="HiddenField1"    CssClass="form-control"  runat="server"><%#Eval("Name") %></asp:Label> 

                                    </div> 
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="chart tab-pane <% if (Request["tab"] == "partner") {%>active <% }%>" id="tabPartners">
                    <div class="box-body no-padding" style="padding-top: 15px !important">
                        <asp:Repeater ID="rptListPartner" runat="server">
                            <ItemTemplate>
                                <div class="col-md-3 col-sm-6 col-xs-12" style="padding-bottom: 10px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxIsCheckPartner" runat="server" Checked='<%#Eval("IsCheck") %>'></asp:CheckBox>
                                            <asp:HiddenField ID="txtPartnerId" Value='<%#Eval("PartnerId") %>' runat="server"></asp:HiddenField>
                                            <asp:HiddenField ID="txtPartnerCode" Value='<%#Eval("PartnerCode") %>' runat="server"></asp:HiddenField>
                                        </span>
                                        <asp:Label ID="lbNamePartner" CssClass="form-control" runat="server"><%#Eval("Name") %></asp:Label>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
            <div class="box-footer">
                <asp:Button ID="btUpdate" runat="server" Text="CẬP NHẬT" CssClass="btn btn-info " OnClick="btUpdate_Click"></asp:Button>
                <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
            </div>
    </section> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    
    <script type="text/javascript">
        $(function () {
            $(".txtOrderNo,.txtQuota,.txtSignatureType").attr("type", "number");
        });
    </script>
</asp:Content>

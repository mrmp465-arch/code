<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Providers.Add.aspx.cs" Inherits="Pages_PayGate_Provider_Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content-header">
        <h1>Kết nối
        <small>
            <a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.ProviderList %>">Danh sách nhà cung cấp 
            </a>
        </small>
        </h1>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>
                    Sửa nhà cung cấp  
                     <%if (Type == "7")
                        {%>
                        Gạch Thẻ
                    <% }
                        else if (Type == "15")
                        { %> 
                            Mua mã thẻ
                    <% }
                        else if (Type == "5")
                        { %> 
                        Topup TK Mobile
                    <% }
                        else if (Type == "18")
                        { %> 
                        Ngân hàng Cash
                    <% }
                        else if (Type == "13")
                        { %>
                        Ngân hàng
                    <% }%>
                </li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Thông Tin</a></li>
                <li><a href="#sales-chart" data-toggle="tab">Dịch vụ</a></li>
            </ul>
            <div class="tab-content no-padding">
                <div class="chart tab-pane active" id="revenue-chart">
                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtName">Tên nhà cung cấp</label>
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên dịch vụ *"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtProviderCode">Mã nhà cung cấp</label>
                                    <asp:TextBox ID="txtProviderCode" runat="server" CssClass="form-control" placeholder="ProviderCode"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtSignatureType">SignatureType</label>
                                    <asp:TextBox ID="txtSignatureType" runat="server" CssClass="form-control txtSignatureType" placeholder="SignatureType"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="PrivateKey">PrivateKey</label>
                                    <asp:TextBox ID="txtPrivateKey" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="PrivateKey"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtPublicKey">PublicKey</label>
                                    <asp:TextBox ID="txtPublicKey" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="PublicKey"></asp:TextBox>
                                </div>
                            </div>
                            <!-- /.col -->
                            <div class="col-md-6">

                                <div class="form-group">
                                    <label for="txtOderNo">Mức độ ưu tiên</label>
                                    <asp:TextBox ID="txtOderNo" runat="server" CssClass="form-control txtOderNo" placeholder="OderNo"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtQuota">Quota (giá trị 0 không giới hạn)</label>
                                    <asp:TextBox ID="txtQuota" runat="server" CssClass="form-control txtQuota" placeholder="Quota"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtType">Loại</label>
                                    <asp:DropDownList ID="txtType" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Gạch thẻ" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="Mua mã Thẻ" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="Topup TK Mobile" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Ngân Hàng" Value="13"></asp:ListItem>
                                        <asp:ListItem Text="Ngân Hàng Cash" Value="18"></asp:ListItem>
                                        <asp:ListItem Text="SMS" Value="17"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtOccurs">Occurs</label>
                                    <asp:DropDownList ID="drpOccurs" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Ngày" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Tuần" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Tháng" Value="3"></asp:ListItem>
                                    </asp:DropDownList>
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
                <div class="chart tab-pane" id="sales-chart">
                    <div class="box-body no-padding" style="padding-top: 15px !important">
                        <asp:Repeater ID="rptList" runat="server">
                            <ItemTemplate>
                                <div class="col-md-3 col-sm-6 col-xs-12" style="padding-bottom: 10px;">
                                    <div class="input-group">
                                        <span class="input-group-addon">
                                            <asp:CheckBox ID="cbxProCode" runat="server" Checked='false'></asp:CheckBox>
                                            <asp:HiddenField ID="txtProCode" Value='<%#Eval("code") %>' runat="server"></asp:HiddenField>
                                        </span>
                                        <asp:Label ID="HiddenField1" CssClass="form-control" runat="server"><%#Eval("Name") %></asp:Label>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
        <div class="box-footer">
            <asp:Button ID="btAdd" runat="server" Text="THÊM" CssClass="btn btn-info " OnClick="btAdd_Click"></asp:Button>
            <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
        $(function () {
            $(".txtOderNo,.txtQuota,.txtSignatureType").attr("type", "number");
        });
    </script>
</asp:Content>

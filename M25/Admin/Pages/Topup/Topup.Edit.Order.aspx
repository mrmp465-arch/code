<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.Edit.Order.aspx.cs" Inherits="Pages_Security_Roles_Edit_Order" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="modal  fade bs-example-modal-sm" id="AlertBan" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-danger alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertBans"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertInfo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-info"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertInfos"></asp:Label>
            </div>
        </div>
    </div>
    <div class="modal  fade bs-example-modal-sm" id="AlertSuccess" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-sm" role="document">
            <div class="alert alert-success alert-dismissible">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i>Thông báo!</h4>
                <asp:Label runat="server" ID="AlertSuccesss"></asp:Label>
            </div>
        </div>
    </div>
    <section class="content-header">
        <h1>Quản trị
        <small><a href="<%=Constant.ADMIN_PATH %><%=Resources.Url.TopupAdd %>">Danh sách đối tác</a>  </small>
        </h1>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="nav-tabs-custom">
            <!-- Tabs within a box -->
            <ul class="nav nav-tabs pull-left">
                <li class="pull-left header"><i class="fa fa-inbox"></i>Cập nhật mức độ ưu tiên đơn hàng</li>
                <li class="active"><a href="#revenue-chart" data-toggle="tab">Thông Tin</a></li>
            </ul>
            <div class="tab-content no-padding">
                <!-- Morris chart - Sales -->
                <div class="chart tab-pane active" id="revenue-chart">
                    <!-- SELECT2 EXAMPLE -->
                    <div class="box-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="txtFullName">Order No*</label>
                                    <asp:TextBox ID="txtOrderNo" runat="server" CssClass="form-control" placeholder="Mã đơn hàng"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label for="txtStatus">Trạng thái (Hạn chế thay đổi trạng thái hệ thống)*  (Không ảnh hưởng đến trạng thái những đơn "Không sử dụng")</label>
                                    <asp:DropDownList ID="txtStatus" runat="server" CssClass="form-control">
                                        <asp:ListItem Selected="True" Value="">Trạng thái:</asp:ListItem>
                                        <asp:ListItem Value="-3">Đợi nạp</asp:ListItem>
                                        <asp:ListItem Value="1">Đợi xử lý</asp:ListItem>
                                        <asp:ListItem Value="2">Đang xử lý (Hệ thống)</asp:ListItem>
                                        <asp:ListItem Value="3">Đã hoàn thành (Hệ thống)</asp:ListItem>
                                        <asp:ListItem Value="0">Không sử dụng</asp:ListItem>
                                        <asp:ListItem Value="-1">Bỏ qua (Hệ thống)</asp:ListItem>
                                        <asp:ListItem Value="-2">Telco khóa (Hệ thống)</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtMobile">Mức độ ưu tiên * (P càng nhỏ mức độ ưu tiên càng cao)</label>
                                    <asp:DropDownList ID="txtPriority" runat="server" CssClass="form-control" >
                                        <asp:ListItem Value="">Priority:</asp:ListItem>
                                         <asp:ListItem Value="49">P49</asp:ListItem>
                                        <asp:ListItem Value="48">P48</asp:ListItem>
                                        <asp:ListItem Value="47">P47</asp:ListItem>
                                        <asp:ListItem Value="46">P46</asp:ListItem>
                                        <asp:ListItem Value="45">P45</asp:ListItem>
                                        <asp:ListItem Value="44">P44</asp:ListItem>
                                        <asp:ListItem Value="43">P43</asp:ListItem>
                                        <asp:ListItem Value="42">P42</asp:ListItem>
                                        <asp:ListItem Value="41">P41</asp:ListItem>
                                        <asp:ListItem Value="40">P40</asp:ListItem>
                                         <asp:ListItem Value="39">P39</asp:ListItem>
                                        <asp:ListItem Value="38">P38</asp:ListItem>
                                        <asp:ListItem Value="37">P37</asp:ListItem>
                                        <asp:ListItem Value="36">P36</asp:ListItem>
                                        <asp:ListItem Value="35">P35</asp:ListItem>
                                        <asp:ListItem Value="34">P34</asp:ListItem>
                                        <asp:ListItem Value="33">P33</asp:ListItem>
                                        <asp:ListItem Value="32">P32</asp:ListItem>
                                        <asp:ListItem Value="31">P31</asp:ListItem>
                                        <asp:ListItem Value="30">P30</asp:ListItem>
                                        <asp:ListItem Value="29">P29</asp:ListItem>
                                        <asp:ListItem Value="28">P28</asp:ListItem>
                                        <asp:ListItem Value="27">P27</asp:ListItem>
                                        <asp:ListItem Value="26">P26</asp:ListItem>
                                        <asp:ListItem Value="25">P25</asp:ListItem>
                                        <asp:ListItem Value="24">P24</asp:ListItem>
                                        <asp:ListItem Value="23">P23</asp:ListItem>
                                        <asp:ListItem Value="22">P22</asp:ListItem>
                                        <asp:ListItem Value="21">P21</asp:ListItem>
                                        <asp:ListItem Value="20">P20</asp:ListItem>
                                        <asp:ListItem Value="19">P19</asp:ListItem>
                                        <asp:ListItem Value="18">P18</asp:ListItem>
                                        <asp:ListItem Value="17">P17</asp:ListItem>
                                        <asp:ListItem Value="16">P16</asp:ListItem>
                                        <asp:ListItem Value="15">P15</asp:ListItem>
                                        <asp:ListItem Value="14">P14</asp:ListItem>
                                        <asp:ListItem Value="13">P13</asp:ListItem>
                                        <asp:ListItem Value="12">P12</asp:ListItem>
                                        <asp:ListItem Value="11">P11</asp:ListItem>
                                        <asp:ListItem Value="10">P10</asp:ListItem>
                                        <asp:ListItem Value="9">P9</asp:ListItem>
                                        <asp:ListItem Value="8">P8</asp:ListItem>
                                        <asp:ListItem Value="7">P7</asp:ListItem>
                                        <asp:ListItem Value="6">P6</asp:ListItem>
                                        <asp:ListItem Value="5">P5</asp:ListItem>
                                        <asp:ListItem Value="4">P4</asp:ListItem>
                                        <asp:ListItem Value="3">P3</asp:ListItem>
                                        <asp:ListItem Value="2">P2</asp:ListItem>
                                        <asp:ListItem Value="1">P1</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtMobile">Tình trạng*</label>
                                    <asp:DropDownList ID="txtConfirm" runat="server" CssClass="form-control" >
                                        <asp:ListItem Selected="True" Value="">Tình trạng:</asp:ListItem>
                                        <asp:ListItem Value="0">Chưa chốt</asp:ListItem>
                                        <asp:ListItem Value="1">Đã chốt</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label for="txtNote">Chú ý: Thực hiện thao tác này sẽ áp dụng cho toàn bộ đơn hàng</label>
                                </div>
                                
                            </div>
                        </div>
                        <!-- /.row -->
                    </div>

                    <div class="box-footer">
                        <asp:Button ID="btUpdate" runat="server" Text="CẬP NHẬT" CssClass="btn btn-primary" OnClick="btUpdate_Click"></asp:Button>
                        <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>


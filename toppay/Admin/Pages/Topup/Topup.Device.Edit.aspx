<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="Topup.Device.Edit.aspx.cs" Inherits="Pages_PayGate_Topup_Device_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <h1>USSD
            <small>Sửa Sim Slot
            </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Sửa Sim Slot</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title"><%=DeviceName%></h3>
            </div>

            <div class="box-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="ddlDevice">Device</label>
                            <asp:DropDownList ID="ddlDevice" runat="server" CssClass="form-control" placeholder="Thiết bị *"></asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtSim">Sim num</label>
                            <asp:TextBox ID="txtSim" runat="server" CssClass="form-control" placeholder="Số Sim *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="ddlSlot">Slot</label>
                            <asp:DropDownList ID="ddlSlot" runat="server" CssClass="form-control" placeholder="Khe cắm Sim *">
                                <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtTelco">Telco</label>
                            <asp:DropDownList ID="ddlTelco" runat="server" CssClass="form-control" placeholder="Thiết bị *">
                                <asp:ListItem Text="Viettel" Value="vtt"></asp:ListItem>
                                <asp:ListItem Text="Mobifone" Value="vms"></asp:ListItem>
                                <asp:ListItem Text="Vinaphone" Value="vnp"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtQouta">Qouta</label>
                            <asp:TextBox ID="txtQouta" runat="server" CssClass="form-control" placeholder="Giới hạn nạp tiền vào SIM *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtAmout">Amout</label>
                            <asp:TextBox ID="txtAmout" runat="server" CssClass="form-control" placeholder="Số tiền đã nạp sau lần Reset gần nhất *"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="ddlStatus">Status</label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" placeholder="Trạng thái Slot*">
                                <asp:ListItem Text="Actived" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Disabled" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Locked" Value="-2"></asp:ListItem>
                                <asp:ListItem Text="Over Quota" Value="-1"></asp:ListItem>
                            </asp:DropDownList>

                        </div>
                    </div>
                </div>
                <!-- /.row -->
            </div>
            <div class="box-footer">
                <asp:Button ID="btUpdate" runat="server" Text="CẬP NHẬT" CssClass="btn btn-info " OnClick="btUpdate_Click"></asp:Button>
                <asp:Button ID="btCancel" runat="server" Text="Hủy" CssClass="btn btn-default pull-right" Width="50px" OnClick="btCancel_Click"></asp:Button>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <script type="text/javascript">
        $(function () {
            $(".txtQuota,.txtAmout").attr("type", "number");
        });
    </script>
</asp:Content>

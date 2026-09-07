<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="SystemConfig.aspx.cs" Inherits="Pages_SystemConfig_SystemConfig" ValidateRequest="false" %>

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
                <li class="pull-left header"><i class="fa fa-inbox"></i>Cấu hình hệ thống
                </li>
                <li class="active"><a href="#sales-chart" data-toggle="tab">Hệ thống tự động</a></li>

            </ul>
            <div class="tab-content no-padding">
                <!-- Morris chart - Sales -->
                <div class="chart tab-pane active"
                    id="sales-chart">
                    <div class="box-body no-padding">
                        <table class="table table-condensed">
                            <tbody>
                                <tr>
                                    <th style="text-align: center; font-weight: bold;">#</th>
                                    <th>Dịch vụ</th>
                                    <th>Mô tả </th>
                                    <th>Trạng thái</th>
                                </tr>
                            <asp:Repeater ID="rptList" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="openclass" data-id="<%#Eval("Id") %>" style="text-align: center; font-weight: bold; cursor: pointer">+</td>
                                    <td><%#Eval("Name") %></td>
                                    <td><%#Eval("Description") %></td>
                                    <td><asp:CheckBox ID="cbxStatus" runat="server" />
                                        <asp:Label ID="lblServiceID" Visible="false" runat="server" Text='<%#Eval("Id") %>'></asp:Label>
                                        <asp:Label ID="LblServiceCode" Visible="false" runat="server" Text='<%#Eval("Code") %>'></asp:Label>
                                        <asp:Label ID="lblListFeature" Visible="false" runat="server" Text='<%#Eval("Feature") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr class="LastIndex LastIndex<%#Eval("Id") %>" style="width: 100%; min-width: 300px">
                                    <td></td>
                                    <td colspan="7">Tắt bật các tính năng:
                                               
                                                <div style="margin-left: -15px; margin-top: 10px;">
                                                    <asp:Repeater ID="rptProList" runat="server" DataSource='<%#listFeature(Eval("Id")) %>'>
                                                        <ItemTemplate>
                                                            <div class="col-xs-4 col-md-2" style="margin-bottom: 10px;">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">
                                                                        <asp:CheckBox ID="cbxProStatus" runat="server"></asp:CheckBox>
                                                                        <asp:HiddenField ID="txtProCode" Value='<%#Eval("Code") %>' runat="server"></asp:HiddenField>
                                                                        <asp:HiddenField ID="txtProId" Value='<%#Eval("Id") %>' runat="server"></asp:HiddenField>
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


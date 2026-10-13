<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Layout.master" AutoEventWireup="true" CodeFile="CardReseller.Orders.aspx.cs" Inherits="Pages_CardStore_CardReseller_Orders" %>

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
        <h1>Quản lý kho thẻ
            <small>Quản lý kho, import thẻ </small>
        </h1>
        <ol class="breadcrumb">
            <li><a href="<%=Constant.ADMIN_PATH %>Default.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
            <li class="active">Xem log</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box">
                    <div class="box-header with-border">
                        <h3 class="box-title">Xem log giao dịch</h3>
                    </div>
                    <div class="box-tools" style="margin-top: 10px;">
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="input-group">
                                <span class="input-group-addon">CardSerial</span>
                                 <asp:TextBox ID="txtCardSerial" Text="" runat="server" CssClass="txtCardSerial form-control"></asp:TextBox>
                            </div> 
                        </div> 
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control drpProviders" ID="drpProviders" runat="server">
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2">
                            <asp:DropDownList CssClass="form-control drpStatus" ID="drpStatus" runat="server">
                                <asp:ListItem Text="Trạng thái:" Value=""></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="0" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-1">
                            <asp:Button ID="btView" runat="server" CssClass="btn btn-info" OnClick="btView_Click" Text="Xem"></asp:Button>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-2"> 
                            <a href="<%=Constant.ADMIN_PATH  %>content/CardStore.txt"><strong>Tải file  mẫu</strong></a>
                        </div>
                        <div>  
                            <input type="hidden" runat="server" id="txtPackId" class="txtPackId" value="" /> 
                            <input type="hidden" runat="server" id="txtProviderCode" class="txtProviderCode" value="" />
                            <input type="hidden" runat="server" id="txtCardType" class="txtCardType" value="" />
                            <input type="hidden" runat="server" id="txtCardValue" class="txtCardValue" value="" />
                            <input type="hidden" runat="server" id="txtIsActive" class="txtIsActive" value="" />
                            <input type="hidden" runat="server" id="txtExpireDate" class="txtExpireDate" value="" />
                            <input type="hidden" runat="server" id="txtNo" class="txtNo" value="" />
                            <input type="hidden" runat="server" id="txtOrderId" class="txtOrderId" value="" /> 
                            <input type="hidden" runat="server" id="txtIsUpload" class="txtIsUpload" value="false" /> 
                            <asp:FileUpload ID="FileUploadExcel" CssClass="FileUploadExcel" runat="server" />
                            <asp:Button ID="Button1" runat="server" Text="Tải lên" CssClass=" btnUpload_Click" OnClick="btnUpload_Click"></asp:Button>
                        </div>
                      
                    </div> 
                    <!-- /.box-header -->
                    <div class="box-body no-padding">
                        <div style="height: 20px; clear: both;"></div>
                        <table class="table table-striped" id="TableResponsive">

                            <tr>
                                <th style="width: 20px;"></th>
                                <th style="text-align: left">ID</th>
                                <th>No</th>
                                <th>Name</th>
                                <th>Provider</th>
                                <th>Status</th>
                                <th style="text-align: right; padding-right: 10px;"><span class="item1-span1">
                                    <img src="<%=Constant.ADMIN_PATH %>Content/add.jpg" title="Thêm"/>
                                </span></th>
                            </tr>
                          
                            <asp:Repeater ID="rptList" runat="server">
                                <ItemTemplate>
                                    <tr class="item-tr item-tr-<%#Eval("Id")%>" data-no="<%# Eval("No") == "" ? "0": Eval("No")  %>" data-id="<%#Eval("Id") %>" data-provider="<%#Eval("ProviderCode") %>" data-open="false" data-status="<%#Eval("Status") %>" data-page="1" >
                                        <td style="width: 20px;"><span class="open" style="display: block; width: 20px; padding: 0; text-align: center;">+</span></td>
                                        <td style="text-align: left" class="Id"><%#Eval("Id")%></td>
                                        <td ><%#Eval("No") %></td>
                                        <td class="Name">
                                            <input type='text' style="display: none;" value="<%#Eval("Name") %>" /><span><%#Eval("Name") %></span> </td>
                                        <td class="ProviderCode">
                                            <select style="display: none">
                                                <% if (lstProviders != null)
                                                    {
                                                        foreach (var item in lstProviders)
                                                        {
                                                            %>
                                                            <option data-value="<%#Eval("ProviderCode") %>" value="<%=item.ProviderCode %>"><%=item.ProviderCode%></option>
                                                            <% 
                                                        }
                                                    }  %>
                                            </select>
                                            <span><%#  Eval("ProviderCode")  %></span>
                                        </td>
                                        <td class="Status">
                                            <select style="display: none">
                                                <option value="0" data-value="<%#Eval("Status") %>">0</option>
                                                <option value="1" data-value="<%#Eval("Status") %>">1</option>                                              
                                            </select>
                                            <span><%#  GetStatus((int)Eval("Status"))  %></span>
                                        </td>
                                        <td style="text-align: right;">
                                            <span class="item1-span-block1">
                                                <span class="item1-span3" data-id="<%#Eval("Id") %>">Sửa</span> | <span class="item1-span4" data-id="<%#Eval("Id") %>">Xóa</span>
                                            </span>
                                            <span class="item1-span-block2" style="display: none">
                                                <span class="item1-span6" data-id="<%#Eval("Id") %>">Cập nhập</span>  |     <span class="item1-span5" data-id="<%#Eval("Id") %>">Hủy</span>
                                            </span>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>

                    </div>
                    <div class="box-footer">
                           <% if (pages!=null && pages.EndPage > 1){%>
                                <span style="line-height: 27px;    padding: 0 9px 0 0;    font-weight: bold;"> Trang <%=pages.CurrentPage  %>/<%=pages.TotalPages%></span>                                    <ul class="pagination pagination-sm no-margin pull-right">
                                    <% if (pages.CurrentPage > 1){%>
                                            <li>
								                <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardResellerOrders %>?page=1">First</a>
							                </li>
							                <li>
								                <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardResellerOrders %>?page=<%=pages.CurrentPage - 1%>"><</a>
							                </li>
                                    <% } %>
                                        <% for (var page = pages.StartPage; page <= pages.EndPage; page++){%>
                                            <li class="<%= GetactivePage(page, pages.CurrentPage)%>">   
								            <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardResellerOrders %>?page=<%= page%>"> <%= page%></a>
							            </li> 
                                    <% } %>
                                    <% if (pages.CurrentPage < pages.TotalPages){%>
                                            <li>
								                <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardResellerOrders %>?page=<%=pages.CurrentPage + 1%>">></a>
							                </li>
							                <li>
								                <a href="<%=Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk %>?page=<%= (pages.TotalPages) %>  ">Previous</a>
							                </li> 
                                    <% } %>
                                </ul>
                        <% } %>   
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server"> 
    <script type="text/javascript">
        var selecthtmlOrder = ''; 
        <% if (lstProviders != null){foreach (var item in lstProviders){%>
        selecthtmlOrder += ' <option data-value="<%#Eval("ProviderCode") %>" value="<%=item.ProviderCode %>"><%=item.ProviderCode%></option>';
        <% }}  %>
        selecthtmlOrder = '<select >' + selecthtmlOrder+' </select>  ';
            var htmltrOrder = ''
                + '<tr class="item-tr item-tr-0" data-open="false" data-page="1">'
                + '<td style="width: 20px;"><span class="open" style="display: block; width: 20px; padding: 0; text-align: center;">-</span></td>'
                + '<td style="text-align: left" class="Id">0</td>'
                + '<td class="No"> <input type="text" readonly value="" /><span></span></td>'
                + '<td class="Name"> <input type="text" value="" /><span></span> </td>'
                + '<td class="ProviderCode">' + selecthtmlOrder + ' <span></span></td>'
                + '<td class="Status"><select><option value="0" data-value="0">0</option><option value="1" data-value="0">1</option></select><span></span></td>'
                + '<td style="text-align: right;"><span class="item1-span-block1" style="display: none" ><span class="item1-span3" data-id="0">Sửa</span> | <span class="item1-span4" data-id="">Xóa</span></span>'
                + '<span class="item1-span-block2" style="display: none"><span class="item1-span6" data-id="0">Cập nhập</span> | <span class="item1-span5" data-id="0">Hủy</span></span>'
                + '<span class="item1-span-block3" ><span class="item1-span7" data-id="0">Thêm</span> | <span class="item1-span8" data-id="">Hủy</span></span></td>'
                + '</tr>'; 
    </script> 
    <script type="text/javascript">
            (function(a){if("undefined"!=typeof a.fn.lc_switch)return!1;a.fn.lc_switch=function(d,f){a.fn.lcs_destroy=function(){a(this).each(function(){a(this).parents(".lcs_wrap").children().not("input").remove();a(this).unwrap()});return!0};a.fn.lcs_on=function(){a(this).each(function(){var b=a(this).parents(".lcs_wrap"),c=b.find("input");"function"==typeof a.fn.prop?b.find("input").prop("checked",!0):b.find("input").attr("checked",!0);b.find("input").trigger("lcs-on");b.find("input").trigger("lcs-statuschange");
    b.find(".lcs_switch").removeClass("lcs_off").addClass("lcs_on");if(b.find(".lcs_switch").hasClass("lcs_radio_switch")){var d=c.attr("name");b.parents("form").find("input[name="+d+"]").not(c).lcs_off()}});return!0};a.fn.lcs_off=function(){a(this).each(function(){var b=a(this).parents(".lcs_wrap");"function"==typeof a.fn.prop?b.find("input").prop("checked",!1):b.find("input").attr("checked",!1);b.find("input").trigger("lcs-off");b.find("input").trigger("lcs-statuschange");b.find(".lcs_switch").removeClass("lcs_on").addClass("lcs_off")});
    return!0};return this.each(function(){if(!a(this).parent().hasClass("lcs_wrap")){var b="undefined"==typeof d?"True":d,c="undefined"==typeof f?"False":f,b=b?'<div class="lcs_label lcs_label_on">'+b+"</div>":"",c=c?'<div class="lcs_label lcs_label_off">'+c+"</div>":"",g=a(this).is(":disabled")?!0:!1,e=a(this).is(":checked")?!0:!1,e=""+(e?" lcs_on":" lcs_off");g&&(e+=" lcs_disabled");b='<div class="lcs_switch '+e+'"><div class="lcs_cursor"></div>'+b+c+"</div>";!a(this).is(":input")||"checkbox"!=a(this).attr("type")&&
    "radio"!=a(this).attr("type")||(a(this).wrap('<div class="lcs_wrap"></div>'),a(this).parent().append(b),a(this).parent().find(".lcs_switch").addClass("lcs_"+a(this).attr("type")+"_switch"))}})};a(document).ready(function(){a(document).delegate(".lcs_switch:not(.lcs_disabled)","click tap",function(d){a(this).hasClass("lcs_on")?a(this).hasClass("lcs_radio_switch")||a(this).lcs_off():a(this).lcs_on()});a(document).delegate(".lcs_wrap input","change",function(){a(this).is(":checked")?a(this).lcs_on():
    a(this).lcs_off()})})})(jQuery);
    </script>
    <script src="<%=Constant.ADMIN_PATH %>Content/bower_components/moment/min/moment.min.js"></script> 
    <link rel="stylesheet" href="<%=Constant.ADMIN_PATH %>Content/dist/css/bootstrap-datetimepicker.css" /> 
    <script src="<%=Constant.ADMIN_PATH %>Content/dist/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript"> 
        function OrderAjax(OrderId, Type, Name, Provider, Status, No, thistr) {
            $.ajax({
                url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerOrders %>?OrderId=" + OrderId + "&type=" + Type + "&Name=" + Name + "&Provider=" + Provider + "&Status=" + Status + "&OrderNo=" + No
            }).done(function (data) {
                 if (data.Type == "delete") {
                    if (data.Result == "True") {
                        $(".item-tr-" + OrderId).remove();
                    }
                } else if (data.Type == "update") {
                    if (data.Result == "True") {
                        blockOrders(OrderId, "none", "block");
                    }
                } else if (data.Type == "add") {
                    if (data.Result == "True") { 
                        thistr.addClass("item-tr-" + data.Id).removeClass("item-tr-0");  
                        blockOrders(data.Id, "none", "block",true);                       
                        thistr.find(".No span").html(data.No);   
                        if (isEmpty(data.No))
                        {
                            thistr.attr("data-no", "0");
                        }
                        else
                        {
                            thistr.attr("data-no", data.No);
                        }   

                        thistr.attr("data-provider", data.Provider);
                        thistr.attr("data-id", data.Id);
                        thistr.attr("data-status", data.Status);   
                        thistr.find(".Id").html(data.Id);
                        thistr.find(".open").html("+");
                        thistr.find(".No input").css("display", "none");
                        thistr.find(".No span").css("display", "block");
                        thistr.find(".item1-span3,.item1-span4,.item1-span5,.item1-span6").attr("data-id", data.Id); 
                        btnClick();
                    }
                }
            });
        } 
        function blockOrders(OrderId, display1, display2,IsAdd) {
            $(".item-tr-" + OrderId + " .Name input").css("display", display1);
            $(".item-tr-" + OrderId + " .ProviderCode select").css("display", display1);
            $(".item-tr-" + OrderId + " .Status select").css("display", display1);
            $("select option").each(function (index) {
                if ($(this).val() == $(this).data("value")) {
                    $(this).attr("selected", "selected");
                }
            }); 
            $(".item-tr-" + OrderId + " .Name span").css("display", display2);
            $(".item-tr-" + OrderId + " .ProviderCode span").css("display", display2);
            $(".item-tr-" + OrderId + " .Status span").css("display", display2);

            $("#TableResponsive .item-tr-" + OrderId + " .item1-span-block1").css("display", display2);
            $("#TableResponsive .item-tr-" + OrderId + " .item1-span-block2").css("display", display1);
            if (IsAdd) {
                $("#TableResponsive .item-tr-" + OrderId + " .item1-span-block3").css("display", display1);
            } 
        }
        $(function () {
            //$('input,textarea').attr('autocomplete', 'off');
            $("#TableResponsive .item1-span1").off("click", item1span1).on("click", item1span1);
            btnClick();  
            Unit();
        });


        function item1span1() { 
            var thistr = $(this).parent().parent();
            thistr.after(htmltrOrder); 
            $.ajax({
                url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerOrders %>?type=gencode"
            }).done(function (data) {
                if (data.Type == "gencode") { 
                    $(".item-tr-0:first .No input").val(data.No);
                    $(".item-tr-0:first .No input").html(data.No);
                }
            });  
            
            $("#TableResponsive .item1-span7").off("click", item1span7).on("click", item1span7);
            $("#TableResponsive .item1-span8").off("click", item1span8).on("click", item1span8);
        }
        function item1span7(){
            var thistr = $(this).parent().parent().parent();
            var No = thistr.find(".No input").val();
            var Name = thistr.find(".Name input").val();
            var Provider = thistr.find(".ProviderCode select :selected");
            var Status = thistr.find(".Status select :selected");

              


            thistr.find(".Name span").html(Name)
            thistr.find(".ProviderCode span").html(Provider.text());
            thistr.find(".Status span").html(Status.text());
            
          OrderAjax(0, "add", Name, Provider.val(), Status.val(), No, thistr);
        }
        function item1span8() {
            $(this).parent().parent().parent().remove();
        }
        function Unit() {
            var OrderId = $(".txtOrderId").val();   
            if (!isEmpty(OrderId)) {
                $("#TableResponsive .item-tr-" + OrderId + " span.open").click(); 
            }
        }
        function isEmpty(val) {
            return (val === undefined || val == null || val.length <= 0) ? true : false;
        }
        function drpProviders() {
            return $('select.drpProviders option:selected').val();
        } 
        function btnClick() {
            $("#TableResponsive .item1-span3").off("click", item1span3).on("click", item1span3);
            $("#TableResponsive .item1-span4").off("click", item1span4).on("click", item1span4);
            $("#TableResponsive .item1-span5").off("click", item1span5).on("click", item1span5);
            $("#TableResponsive .item1-span6").off("click", item1span6).on("click", item1span6);
            $("#TableResponsive .item-tr span.open").off("click", openChild).on("click", openChild);
        }

        ////
        function openChild() { 
            var thisSpan = $(this);
            var thistr = $(this).parent().parent();
            var OrderNo = thistr.data("no");
            var OrderId = thistr.data("id");
            var Provider = thistr.data("provider");
            var Status = thistr.data("status");
            var page = thistr.data("page");
            var PackId = "";
            <% if (btIsView){%>
                PackId = $(".txtPackId").val();   
            <%} %>
             

            if (thistr.data("open") == false) {
                $.ajax({
                    url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerPacket %>?OrderNo=" + OrderNo + "&OrderId=" + OrderId + "&Provider=" + Provider + "&Status=" + Status + "&page=" + page + "&PackId=" + PackId
                    }).done(function (data) {
                        thistr.after(data);
                        thistr.data("open", true);
                        thisSpan.html("-");
                    });
            } else {
                thistr.data("open", false); 
                $("." + OrderNo).remove(); 
                thisSpan.html("+");
            } 
         }
        function item1span4() {
            var thistr2 = $(this);
            var OrderId = thistr2.data("id");  
            var OrderNo = $(".item-tr-" + OrderId).data("no");
            $.ajax({
                url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerOrders %>?OrderId=" + OrderId + "&type=count"  
             }).done(function (data) {
                 if (data.Type == "count") {
                     if (data.Result == "True") {
                         var Msg = 'Bạn có muốn Xóa "' + OrderNo+'"';
                         if (data.Pack > 0) {
                             Msg += '\nGồm ' + data.Pack+' Packet' ;
                         }
                         if (data.Card > 0) {
                             Msg += '\nGồm ' + data.Card +' Card' ;
                         }
                         var r = confirm(Msg);
                         if (r == true) {
                             OrderAjax(OrderId, "delete")
                             $("." + OrderNo).remove();;
                         }
                     }
                 }  
             });  
        }
        function item1span3() {
            var thistr2 = $(this);
            var OrderId = thistr2.data("id");
            blockOrders(OrderId, "block", "none");
        }
        function item1span5() {
            var thistr2 = $(this);
            var OrderId = thistr2.data("id");
            blockOrders(OrderId, "none", "block");
        }
        function item1span6() {
            var thistr2 = $(this);
            var OrderId = thistr2.data("id");
            var OrderNo = $(".item-tr-" + OrderId).data("no");
            var Name = $(".item-tr-" + OrderId + " .Name input").val();
            var Provider = $(".item-tr-" + OrderId + " .ProviderCode select :selected");
            var Status = $(".item-tr-" + OrderId + " .Status select :selected");

            $.ajax({
                url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerOrders %>?OrderId=" + OrderId + "&type=count&Provider=" + Provider.val() 
            }).done(function (data) {
                if (data.Type == "count") {
                    if (data.Result == "True") {
                        var Msg = 'Bạn có muốn Cập Nhập "' + OrderNo + '"';
                        if (data.Pack > 0) {
                            Msg += '\nGồm ' + data.Pack + ' Packet';
                        }
                        //if (data.Card > 0) {
                        //    Msg += '\n gồm ' + data.Card + ' Card';
                        //}
                        var r = confirm(Msg);
                        if (r == true) {

                            $(".item-tr-" + OrderId + " .Name span").html(Name)
                            $(".item-tr-" + OrderId + " .ProviderCode span").html(Provider.text());
                            $(".item-tr-" + OrderId + " .Status span").html(Status.text());
                            OrderAjax(OrderId, "update", Name, Provider.val(), Status.val());
                            $(".item-tr-" + OrderId).data("provider", Provider.val()).data("status", Status.val());
                            if ($(".item-tr-" + OrderId).data("open")) {
                                $("#TableResponsive .item-tr-" + OrderId + " span.open").click();
                                $("#TableResponsive .item-tr-" + OrderId + " span.open").click();
                            }
                        }
                    } else {
                        $(".item-tr-" + OrderId + " .Name span").html(Name)
                        $(".item-tr-" + OrderId + " .ProviderCode span").html(Provider.text());
                        $(".item-tr-" + OrderId + " .Status span").html(Status.text());
                        OrderAjax(OrderId, "update", Name, Provider.val(), Status.val());
                        if ($(".item-tr-" + OrderId).data("open")) {
                            $("#TableResponsive .item-tr-" + OrderId + " span.open").click();
                            $("#TableResponsive .item-tr-" + OrderId + " span.open").click();
                        }
                    }
                }
            });   
        }


        ///////////////Packet Js//////////////

        function aLink() { 
                var OrderId = $(this).data("orderid");
                var page = $(this).data("page");
                $(".item-tr-" + OrderId).data("page", page);
                $("#TableResponsive .item-tr-" + OrderId + " span.open").click();
                $("#TableResponsive .item-tr-" + OrderId + " span.open").click(); 
        }
        function aLink2() { 
            var PackId = $(this).data("packid");
            var page = $(this).data("page");
            $(".item-tr2-" + PackId).data("page", page);  
            $("#TableResponsive .item-tr2-" + PackId + " span.open2").click();
            $("#TableResponsive .item-tr2-" + PackId + " span.open2").click();
        }
        function item2span1() {   
            var orderId = $(this).data("orderid");
            var orderNo = $(this).data("orderno");
            var provider = $(this).data("provider");
            var status = $(this).data("status");
            var thistr = $(this).parent().parent(); 
            thistr.after(htmltrPack);
            $('.lcs_checkPack').lc_switch();
            $('body').undelegate('.lcs_checkPack', 'lcs-statuschange', lcs_checkPack).delegate('.lcs_checkPack', 'lcs-statuschange', lcs_checkPack); 
            $('.' + orderNo + ' .ExpireDate input').datetimepicker({ viewMode: 'years', format: 'L' });
            $('.' + orderNo + ' .Provider select').val(provider).attr('disabled', true);
            //$('.' + orderNo + ' .Status select').val(status).attr('disabled', true);
          
            $("#TableResponsive .item2-span8").off("click", item2span8).on("click", item2span8);
            $("#TableResponsive .item2-span7").attr("data-orderid", orderId).attr("data-orderno", orderNo).attr("data-provider", provider);
            $("#TableResponsive .item2-span7").off("click", item2span7).on("click", item2span7);
        }
        function item2span8() {
            $(this).parent().parent().parent().remove();
        }
        function item2span7() {
            var orderId = $(this).data("orderid");
            var orderNo = $(this).data("orderno");
            var provider = $(this).data("provider");

            var thistr = $(this).parent().parent().parent();
            var Name = thistr.find(".Name input").val();
            var NumberCard = thistr.find(".NumberCard input").val();
            var NumberCardSole = thistr.find(".NumberCardSole input").val();
            var NumberCardUp = thistr.find(".NumberCardUp input").val();
            var ExpireDate = thistr.find(".ExpireDate input").val();
            var CardValue = thistr.find(".CardValue input").val();
            var CardType = thistr.find(".CardType select :selected");
            var Status = thistr.find(".Status select :selected");
            var Provider = thistr.find(".Provider select :selected"); 
            var IsActive = thistr.find(".lcs_checkPack").is(':checked');
            
            thistr.find(".Name span").html(Name);
            thistr.find(".NumberCard span").html(NumberCard);
            thistr.find(".NumberCardSole span").html(NumberCardSole);
            thistr.find(".NumberCardUp span").html(NumberCardUp);
            thistr.find(".ExpireDate span").html(ExpireDate);
            thistr.find(".CardValue span").html(CardValue);
            thistr.find(".CardType span").html(CardType.text());
            thistr.find(".Status span").html(Status.text());
            thistr.find(".Provider span").html(Provider.text()); 

            PartetAjax(0, "add", IsActive, Status.val(), Name, NumberCard, NumberCardSole, NumberCardUp, ExpireDate, CardValue, CardType.val(),  provider,  orderId, orderNo, thistr);
        }
        function PacketBtnClick() {
            $("#TableResponsive .item2-span1").off("click", item2span1).on("click", item2span1);
            $(".uploadClass").off("click", uploadClass).on("click", uploadClass); 
            $(".FileUploadExcel").off("change", FileUploadExcel).on("change", FileUploadExcel);
        }

        function uploadClass() {   
            $(".txtProviderCode").val($(this).data("providercode"));
            $(".txtCardType").val($(this).data("cardtype"));
            $(".txtCardValue").val($(this).data("cardvalue"));
            $(".txtExpireDate").val($(this).data("expiredate"));
            $(".txtIsActive").val($(this).data("isactive")); 
            $(".txtPackId").val($(this).data("id"));
            $(".txtOrderId").val($(this).data("orderid"));
            $(".FileUploadExcel").click();
            $(".progress" + $(this).data("id")).css("display", "block");
        }
        function FileUploadExcel() { 
             $(".btnUpload_Click").click();  
        }
        $("#TableResponsive .item2-span3").off("click", item2span3).on("click", item2span3);

        function PartetAjax(PackId, Type, IsActive, Status, Name, NumberCard, NumberCardSole, NumberCardUp,ExpireDate, CardValue, CardType, ProviderCode, OrderId, OrderNo, thistr) { 
            $.ajax({
                url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerPacket  %>?PackId=" + PackId + "&type=" + Type + "&Name=" + Name + "&NumberCard=" + NumberCard + "&NumberCardSole=" + NumberCardSole + "&NumberCardUp=" + NumberCardUp + "&ExpireDate=" + ExpireDate + "&CardValue=" + CardValue + "&CardType=" + CardType + "&Status=" + Status + "&IsActive=" + IsActive + "&ProviderCode=" + ProviderCode + "&OrderId=" + OrderId + "&OrderNo=" + OrderNo
            }).done(function (data) {
                if (data.Type == "delete") {
                    if (data.Result == "True") {
                        $(".item-tr2-" + PackId).remove();
                    }
                } else if (data.Type == "update") {
                    $(".item-tr2-" + data.Id).find(".uploadClass") 
                        .attr("data-providercode", ProviderCode)
                        .attr("data-cardtype", CardType)
                        .attr("data-cardvalue", CardValue)
                        .attr("data-expiredate", ExpireDate) ;
                    if (data.Result == "True") {
                        blockPack(PackId, "none", "block");
                    }
                } else if (data.Type == "updatestatus") {
                    if (data.Result == "True") { 
                        $(".item-tr2-" + data.Id).find(".uploadClass") 
                            .attr("data-isactive", IsActive);
                        if ($(".item-tr2-" + data.Id).data("open")) {
                            $("#TableResponsive .item-tr2-" + data.Id + " span.open2").click();
                            $("#TableResponsive .item-tr2-" + data.Id + " span.open2").click();
                        }
                    }
                } else if (data.Type == "add") {
                    if (data.Result == "True") {                         
                        thistr.find(".uploadClass")
                            .attr("data-id", data.Id)
                            .attr("data-orderid", data.OrderId)
                            .attr("data-providercode", data.ProviderCode)
                            .attr("data-cardtype", data.CardType)
                            .attr("data-cardvalue", data.CardValue)
                            .attr("data-expiredate", data.ExpireDate)
                            .attr("data-isactive", data.IsActive);


                        thistr.addClass("item-tr2-" + data.Id).removeClass("item-tr2-0");
                        blockPack(data.Id, "none", "block",true);
                        thistr.find(".No span").html(data.No); 
                        thistr.attr("data-id", data.Id);

                        thistr.find(".IsActive input").val(data.Id);
                        thistr.find(".Id").html(data.Id);
                        thistr.find(".open").html("+");
                        thistr.find(".No input").css("display", "none");
                        thistr.find(".No span").css("display", "block");
                        thistr.find(".item2-span3,.item2-span4,.item2-span5,.item2-span6").attr("data-id", data.Id);
                        btnClickPack();
                        PacketBtnClick();
                    }
                }
            });
        }
        function blockPack(PackId, display1, display2, IsAdd) {
            $(".item-tr2-" + PackId + " .Name input" + ",.item-tr2-" + PackId + " .Provider select,.item-tr2-" + PackId + " .NumberCard input" + ",.item-tr2-" + PackId + " .ExpireDate input" + ",.item-tr2-" + PackId + " .CardValue input" + ",.item-tr2-" + ",.item-tr2-" + PackId + " .ProviderCode select" + ",.item-tr2-" + PackId + " .CardType select" + ",.item-tr2-" + PackId + " .Status select" + ",.item-tr2-" + PackId + " .IsActive select").css("display", display1);
            $("select option").each(function (index) {
                if ($(this).val() == $(this).data("value")) { $(this).attr("selected", "selected"); }
            });

            $(".item-tr2-" + PackId + " .Name span" + ",.item-tr2-" + PackId + " .Provider span,.item-tr2-" + PackId + " .NumberCard span" + ",.item-tr2-" + PackId + " .ExpireDate span" + ",.item-tr2-" + PackId + " .CardValue span" + ",.item-tr2-" + PackId + " .ProviderCode span" + ",.item-tr2-" + PackId + " .CardType span" + ",.item-tr2-" + PackId + " .Status span" + ",.item-tr2-" + PackId + " .IsActive span" ).css("display", display2);

            $("#TableResponsive .item-tr2-" + PackId + " .item2-span-block1").css("display", display2);
            $("#TableResponsive .item-tr2-" + PackId + " .item2-span-block2").css("display", display1);
            if (IsAdd) {
                $("#TableResponsive .item-tr2-" + PackId + " .item2-span-block3").css("display", display1);
            }
        }  
        function UnitPack() {
            var PackId = $(".txtPackId").val();
            if (!isEmpty(PackId)) {
                $("#TableResponsive .item-tr2-" + PackId + " span.open2").click();
            }
        }
        function btnClickPack() {
            $("#TableResponsive .item2-span3").off("click", item2span3).on("click", item2span3);
            $("#TableResponsive .item2-span4").off("click", item2span4).on("click", item2span4);
            $("#TableResponsive .item2-span5").off("click", item2span5).on("click", item2span5);
            $("#TableResponsive .item2-span6").off("click", item2span6).on("click", item2span6);
            $("#TableResponsive .item-tr2 span.open2").off("click", openChildPack).on("click", openChildPack);
        }

        function openChildPack() {
            var thisSpan = $(this);
            var thistr2 = $(this).parent().parent();
            var PacketId = thistr2.data("id");
            var page = thistr2.data("page");
            var CardSerial= $(".txtCardSerial").val();
            var CardCode = "";
            if (thistr2.data("open") == false) { 
                $.ajax({
                    url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerStore %>?PacketId=" + PacketId + "&Provider=" + drpProviders() + "&page=" + page + "&CardSerial=" + CardSerial + "&CardCode=" + CardCode
                }).done(function (data) {
                    thistr2.after(data);
                    thistr2.data("open", true);
                });
                thisSpan.html("-");
            } else {
                thistr2.data("open", false);
                $(".Packet" + PacketId).remove();
                thisSpan.html("+");
            }
        }
        function openPackAjax(url, html) {
           
        }
        function item2span3() {
            var thistr2 = $(this);
            var PackId = thistr2.data("id");
            $('.item-tr2-' + PackId + ' .ExpireDate input').datetimepicker({ viewMode: 'years', format: 'L' });
            $('.item-tr2-' + PackId + ' .ProviderCode select').attr('disabled', true);
            //$('.item-tr2-' + PackId + ' .Status select').attr('disabled', true); 
            blockPack(PackId, "block", "none");
        }
        function item2span4() {
            var thistr2 = $(this);
            var PackId = thistr2.data("id");
            var PackName = $(".item-tr2-" + PackId + " .Name input").val(); 
             $.ajax({
                url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerPacket %>?PackId=" + PackId + "&type=count"
             }).done(function (data) {
                 if (data.Type == "count") {
                     if (data.Result == "True") {
                         var Msg = 'Bạn có muốn Xóa "' + PackName + '"'; 
                         if (data.Card > 0) {
                             Msg += '\nGồm ' + data.Card + ' Card';
                         }
                         var r = confirm(Msg);
                         if (r == true) {
                             PartetAjax(PackId, "delete");
                             $(".Packet" + PackId).remove();
                         }
                     }
                 }
             });   
        }
        function item2span5() {
            var thistr2 = $(this);
            var PackId = thistr2.data("id");
            blockPack(PackId, "none", "block");
        }
        function item2span6() {
            var thistr2 = $(this);
            var PackId = thistr2.data("id"); 

            var PackName = $(".item-tr2-" + PackId + " .Name input").val();
            var Name = $(".item-tr2-" + PackId + " .Name input").val();
            var ProviderCode = $(".item-tr2-" + PackId + " .ProviderCode select :selected").val();
            var NumberCard = $(".item-tr2-" + PackId + " .NumberCard input").val();
            var NumberCardSole = $(".item-tr2-" + PackId + " .NumberCardSole input").val();
            var NumberCardUp = $(".item-tr2-" + PackId + " .NumberCardUp input").val();
            var ExpireDate = $(".item-tr2-" + PackId + " .ExpireDate input").val();
            var CardValue = $(".item-tr2-" + PackId + " .CardValue input").val();
            var CardType = $(".item-tr2-" + PackId + " .CardType select :selected");
            var Status = $(".item-tr2-" + PackId + " .Status select :selected");
            var IsActive = $(".item-tr2-" + PackId + " .IsActive select :selected");

            $.ajax({
                url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerPacket %>?PackId=" + PackId + "&type=count&ExpireDate=" + ExpireDate + "&CardValue=" + CardValue + "&CardType=" + CardType.val() + "&IsActive=" + IsActive.val() + "&ProviderCode=" + ProviderCode
            }).done(function (data) {
                if (data.Type == "count") {
                    if (data.Result == "True") {
                        var Msg = 'Bạn có muốn Cập Nhập "' + PackName + '"';
                        if (data.Pack > 0) {
                            Msg += '\nGồm ' + data.Pack + ' Packet';
                        }
                        if (data.Card > 0) {
                            Msg += '\nGồm ' + data.Card + ' Card';
                        }
                        var r = confirm(Msg);
                        if (r == true) {
                            

                            $(".item-tr2-" + PackId + " .Name span").html(Name);
                            $(".item-tr2-" + PackId + " .ProviderCode span").html(ProviderCode);
                            $(".item-tr2-" + PackId + " .NumberCard span").html(NumberCard);
                            $(".item-tr2-" + PackId + " .NumberCardSole span").html(NumberCardSole);
                            $(".item-tr2-" + PackId + " .NumberCardUp span").html(NumberCardUp);
                            $(".item-tr2-" + PackId + " .ExpireDate span").html(ExpireDate);
                            $(".item-tr2-" + PackId + " .CardValue span").html(CardValue);
                            $(".item-tr2-" + PackId + " .CardType span").html(CardType.text());
                            $(".item-tr2-" + PackId + " .Status span").html(Status.text());
                            $(".item-tr2-" + PackId + " .IsActive span").html(IsActive.text());
                            PartetAjax(PackId, "update", IsActive.val(), Status.val(), Name, NumberCard, NumberCardSole, NumberCardUp,ExpireDate, CardValue, CardType.val(),  ProviderCode);
                            if ($(".item-tr2-" + PackId).data("open")) {
                                $("#TableResponsive .item-tr2-" + PackId + " span.open2").click();
                                $("#TableResponsive .item-tr2-" + PackId + " span.open2").click();
                            }
                        }
                    } else {  
                        $(".item-tr2-" + PackId + " .Name span").html(Name);
                        $(".item-tr2-" + PackId + " .ProviderCode span").html(ProviderCode);
                        $(".item-tr2-" + PackId + " .NumberCard span").html(NumberCard);
                        $(".item-tr2-" + PackId + " .NumberCardSole span").html(NumberCardSole);
                        $(".item-tr2-" + PackId + " .NumberCardUp span").html(NumberCardUp);
                        $(".item-tr2-" + PackId + " .ExpireDate span").html(ExpireDate);
                        $(".item-tr2-" + PackId + " .CardValue span").html(CardValue);
                        $(".item-tr2-" + PackId + " .CardType span").html(CardType.text());
                        $(".item-tr2-" + PackId + " .Status span").html(Status.text());
                        $(".item-tr2-" + PackId + " .IsActive span").html(IsActive.text());
                        PartetAjax(PackId, "update", IsActive.val(), Status.val(), Name, NumberCard, NumberCardSole, NumberCardUp, ExpireDate, CardValue, CardType.val(), ProviderCode);
                        if ($(".item-tr2-" + PackId).data("open")) {
                            $("#TableResponsive .item-tr2-" + PackId + " span.open2").click();
                            $("#TableResponsive .item-tr2-" + PackId + " span.open2").click();
                        } 
                    }
                }
            });    
        }  
        var IsChange = true;
        function lcs_checkPack() { 
            
            var thisCheck = $(this);
            var PackId = $(this).val();
            var IsActive = $(this).is(':checked'); 
            if (IsChange) {
                IsChange = false;
                $.ajax({
                    url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerPacket %>?PackId=" + PackId + "&type=count"
                }).done(function (data) {
                    if (data.Type == "count") {
                        if (data.Result == "True") {
                            var Msg = 'Bạn có muốn thay đổi Packet này không!';
                            if (data.Pack > 0) {
                                Msg += '\nGồm ' + data.Pack + ' Packet';
                            }
                            if (data.Card > 0) {
                                Msg += '\nGồm ' + data.Card + ' Card';
                            }
                            var r = confirm(Msg);
                            if (r == true) {
                                PartetAjax(PackId, "updatestatus", IsActive);
                                IsChange = true;
                            } else { 
                                thisCheck.click(); 
                                setTimeout(function () {
                                    IsChange = true;
                                }, 1000); 
                            }
                        }
                    }
                });
            }
            
        }

            
        /// -------- CardStore JS --------

        function CardStoreBtnClick() {
            $("#TableResponsive .item3-span3").off("click", item3span3).on("click", item3span3); 
            $("#TableResponsive .item3-span4").off("click", item3span4).on("click", item3span4);
            $("#TableResponsive .item3-span5").off("click", item3span5).on("click", item3span5);
            $("#TableResponsive .item3-span6").off("click", item3span6).on("click", item3span6);
        }
         
        function item3span3() {
            var thistr2 = $(this);
            var StoreId = thistr2.data("id");
            blockCardStore(StoreId, "block", "none"); 
        }
        function item3span4(){
            var thistr2 = $(this);
            var StoreId = thistr2.data("id");
            var r = confirm("Bạn có muốn xóa!");
            if (r == true) {
                CardStore(StoreId, "delete");
            }
        }
        function item3span5() {
            var thistr2 = $(this);
            var StoreId = thistr2.data("id");
            blockCardStore(StoreId, "none", "block");
        } 
        function item3span6() {
            var thistr2 = $(this);
            var StoreId = thistr2.data("id");  
            var CardSerial = $(".item-tr3-" + StoreId + " .CardSerial input").val();
            var CardCode = $(".item-tr3-" + StoreId + " .CardCode input").val(); 
            var IsSold = $(".item-tr3-" + StoreId + " .IsSold select :selected");
            var IsActive = $(".item-tr3-" + StoreId + " .IsActive select :selected");

            $(".item-tr3-" + StoreId + " .CardSerial span").html(CardSerial);
            $(".item-tr3-" + StoreId + " .CardCode span").html(CardCode); 
            $(".item-tr3-" + StoreId + " .IsSold span").html(IsSold.text());
            $(".item-tr3-" + StoreId + " .IsActive span").html(IsActive.text());
            CardStore(StoreId, "update", IsActive.val(),CardSerial, CardCode, IsSold.val());
        }


        function CardStore(StoreId, Type, IsActive,CardSerial, CardCode, IsSold) {
            $.ajax({
                url: "<%=Constant.ADMIN_PATH %><%=Resources.Url.CardResellerStore %>?StoreId=" + StoreId + "&type=" + Type + "&CardSerial=" + CardSerial + "&CardCode=" + CardCode + "&IsSold=" + IsSold + "&IsActive=" + IsActive  
            }).done(function (data) {
                if (data.Type == "delete") {
                    if (data.Result == "True") {
                        $(".item-tr3-" + StoreId).remove();
                    }
                } else if (data.Type == "update") { 
                    if (data.Result == "True") {

                        blockCardStore(data.Id, "none", "block", true); 
                    }
                } else if (data.Type == "updatestatus") { 
                    if (data.Result == "True") {  
                    }
                }
            });
        }
        function blockCardStore(StoreId, display1, display2) { 
            $(".item-tr3-" + StoreId + " .IsSold select").css("display", display1);
            $(".item-tr3-" + StoreId + " .IsActive select").css("display", display1);
            $("select option").each(function (index) {
                if ($(this).val() == $(this).data("value")) {
                    $(this).attr("selected", "selected");
                }
            }); 
            $(".item-tr3-" + StoreId + " .IsSold span").css("display", display2);
            $(".item-tr3-" + StoreId + " .IsActive span").css("display", display2);
            $("#TableResponsive .item-tr3-" + StoreId + " .item3-span-block1").css("display", display2);
            $("#TableResponsive .item-tr3-" + StoreId + " .item3-span-block2").css("display", display1);
        }
        function lcs_checkStore() { 
            CardStore($(this).val(), "updatestatus", $(this).is(':checked') );
        }
    </script>
    <style type="text/css">
        .item1-span1,.item2-span1,.item3-span1,.item3-span1{
            cursor: pointer;
        } 
        .FileUploadExcel, .btnUpload_Click {
            display: none !important;
        } 
        #TableResponsive span.open,
        #TableResponsive span.open2 {
            cursor: pointer;
            font-weight: bold;
            font-size: 18px;
            color: blue;
        } 
        .item1-span-block1 span, .item1-span-block2 span, .item1-span-block3 span,
        .item2-span-block1 span, .item2-span-block2 span, .item2-span-block3 span,
        .item3-span-block1 span, .item3-span-block2 span, .item3-span-block3 span {
            cursor: pointer;
        } 
        .item1-span-block1 span:hover, .item1-span-block2 span:hover,.item1-span-block3 span:hover,
        .item2-span-block1 span:hover, .item2-span-block2 span:hover,.item2-span-block3 span:hover,
        .item3-span-block1 span:hover, .item3-span-block2 span:hover, .item3-span-block3 span:hover {
            text-decoration: underline;
        }
    .lcs_wrap {	display: inline-block;		direction: ltr;	height: 28px;   vertical-align: middle;}
.lcs_wrap input {	display: none;	}
.lcs_switch {	display: inline-block;		position: relative;	width: 68px;	height: 23px;	border-radius: 30px;	background: #ddd;	overflow: hidden;	cursor: pointer;		-webkit-transition: all .2s ease-in-out;  	-ms-transition: 	all .2s ease-in-out; 	transition: 		all .2s ease-in-out; }
.lcs_cursor {	display: inline-block;	position: absolute;	top: 3px;		width: 18px;	height: 18px;	border-radius: 100%;	background: #fff;	box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.2), 0 3px 4px 0 rgba(0, 0, 0, 0.1);	z-index: 10;		-webkit-transition: all .2s linear;  	-ms-transition: 	all .2s linear; 	transition: 		all .2s linear; }
.lcs_label {	font-family: "Trebuchet MS", Helvetica, sans-serif;    font-size: 12px;	letter-spacing: 1px;	line-height: 15px;	color: #fff;	font-weight: bold;	position: absolute;	width: 33px;	top: 5px;	overflow: hidden;	text-align: center;	opacity: 0;		-webkit-transition: all .2s ease-in-out .1s;  	-ms-transition: 	all .2s ease-in-out .1s;   	transition: 		all .2s ease-in-out .1s;   }
.lcs_label.lcs_label_on {	left: -70px;	z-index: 6;	}
.lcs_label.lcs_label_off {	right: -70px;	z-index: 5;	}
/* on */
.lcs_switch.lcs_on {	background: #75b936;    box-shadow: 0 0 2px #579022 inset;}
.lcs_switch.lcs_on .lcs_cursor {	left: 48px;}.lcs_switch.lcs_on .lcs_label_on {	left: 10px;		opacity: 1;}
/* off */
.lcs_switch.lcs_off {	background: #b2b2b2;	box-shadow: 0px 0px 2px #a4a4a4 inset; 	}
.lcs_switch.lcs_off .lcs_cursor {	left: 3px;}
.lcs_switch.lcs_off .lcs_label_off {	right: 10px;	opacity: 1;	}
/* disabled */
.lcs_switch.lcs_disabled {	opacity: 0.65;	filter: alpha(opacity=65);		cursor: default;}
    </style> 
</asp:Content>
 
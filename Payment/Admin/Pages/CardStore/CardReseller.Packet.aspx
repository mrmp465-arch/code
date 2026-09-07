<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardReseller.Packet.aspx.cs" Inherits="Pages_CardStore_CardReseller_Packet" %>
<%
    string Url = Constant.ADMIN_PATH + Resources.Url.CardResellerPacket;
    string param = "?OrderId="+OrderId+"&OrderNo=" + OrderNo + "&Provider=" + Provider+"&Status="+Status;
    %>
<tr class="<%=OrderNo %>">
    <td colspan="7" style="padding: 5px 0 0 25px;">
        <table class="table table-striped">
            <tr>
                <th style="width: 20px;"></th>
                <th style="text-align: left">PackId</th>
                <th>Name</th>
                <th>Provider</th>
                <th>Loại Thẻ</th>
                <th>SL tồn cho phép</th>
                <th>SL đã up</th>
                <th>SL bán</th>
                <th>SL tồn</th>
                <th>Mệnh giá</th>
                <th>Hạn dùng</th>
                <th>IsActive (All Card)</th>
                <th>Status</th>
                <th style="text-align: right; padding-right: 10px;"><span class="item2-span1" data-orderid="<%=OrderId %>"  data-status="<%=Status %>" data-provider="<%=Provider %>" data-orderno="<%=OrderNo %>">
                    <img src="<%=Constant.ADMIN_PATH %>Content/add.jpg" title="Thêm" />
                </span>
                </th>
            </tr>
            <asp:repeater id="rptList" runat="server">
                <ItemTemplate>
            <tr class="item-tr2 item-tr2-<%#Eval("Id") %>" data-id="<%#Eval("Id") %>" data-open="false" data-page="1">
                <td style="width: 20px;"><span class="open2" style="display: block; width: 20px; padding: 0; text-align: center;">+</span></td>
                <th style="text-align: left"><%#Eval("Id") %></td>
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
                <td class="CardType">
                    <select style="display: none">
                        <% if (lstProducts != null)
                            {
                                foreach (var item in lstProducts)
                                {
                                    %>
                                    <option data-value="<%#Eval("CardType") %>" value="<%=item.Code %>"><%=item.Name%></option>
                                    <% 
                                }
                            }  %>
                    </select>
                    <span><%# Eval("CardType")  %></span>
                </td>
                <td class="NumberCard">
                    <input type='text' style="display: none; width: 70px;" value="<%#Eval("NumberCard") %>" /><span><%#Eval("NumberCard") %></span> </td>
                <td class="NumberCardUp">
                    <%--<input type='text' style="display: none; width: 70px;" value="<%#Eval("NumberCardUp") %>" />--%>
                    <span><%#Eval("NumberCardUp") %></span> </td>
                <td class="NumberCardSole">
                    <%--<input type='text' style="display: none; width: 70px;" value="<%#Eval("NumberCardSole") %>" />--%>
                    <span><%#Eval("NumberCardSole") %></span> </td>
                <td class="NumberInventory">
                    <%--<input type='text' style="display: none; width: 70px;" value="<%#Eval("NumberInventory") %>" />--%>
                    <span><%#Eval("NumberInventory") %></span> </td>
                <td class="CardValue">
                    <input type='text' style="display: none; width: 70px;" value="<%#Eval("CardValue") %>" /><span><%#Eval("CardValue") %></span> </td>
                <td class="ExpireDate">
                    <input type='text' style="display: none;width: 70px;" value="<%#Eval("ExpireDate", "{0:MM/dd/yyyy}") %>" /><span><%#Eval("ExpireDate", "{0:MM/dd/yyyy}") %></span> </td>
                <td class="IsActive">  
                    <input type="checkbox" value="<%#Eval("Id") %>" name="check-1" <%# (bool)Eval("IsActive") == true ? "checked": ""  %> class="lcs_checkPack" autocomplete="off" /> 
                </td>
                <td class="Status">
                    <select style="display: none">
                        <option value="0" data-value="<%#Eval("Status") %>">Nhập tay</option>
                        <option value="1" data-value="<%#Eval("Status") %>">Tự động</option>
                        <option value="2" data-value="<%#Eval("Status") %>">Hết thẻ</option>
                    </select>
                    <span><%#  GetStatus((int)Eval("Status"))  %></span>
                </td>
                <td style="text-align: right;">
                    <span class="item2-span-block1">
                        <div>
                             <span class="uploadClass" data-id="<%#Eval("Id") %>" data-OrderId="<%#Eval("OrderId") %>"    data-ProviderCode="<%#Eval("ProviderCode") %>"    data-CardType="<%#Eval("CardType") %>"    data-CardValue="<%#Eval("CardValue") %>"    data-ExpireDate="<%#Eval("ExpireDate") %>"    data-IsActive="<%#Eval("IsActive") %>"  >Import </span> 
                        | <span class="item2-span3" data-id="<%#Eval("Id") %>">Sửa</span> | <span class="item2-span4" data-id="<%#Eval("Id") %>">Xóa</span> 
                        </div>
                        <div class="progress progress-xs active progress<%#Eval("Id") %>" style="display:none;">
                            <div class="progress-bar progress-bar-primary progress-bar-striped" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                <span class="sr-only">Đang Import thẻ !!</span>
                            </div>
                        </div>
                       
                    </span>
                    <span class="item2-span-block2" style="display: none">
                        <span class="item2-span6" data-id="<%#Eval("Id") %>">Cập nhập</span>  |     <span class="item2-span5" data-id="<%#Eval("Id") %>">Hủy</span>
                    </span>
                </td>
            </tr>
            </ItemTemplate>
            </asp:repeater>
        </table>
        <div>
               <% if (pages!=null && pages.EndPage > 1){%>
                    <span style="line-height: 27px;    padding: 0 9px 0 0;    font-weight: bold;"> Trang <%=pages.CurrentPage  %>/<%=pages.TotalPages%></span>                                    <ul class="pagination pagination-sm no-margin pull-right">
                        <% if (pages.CurrentPage > 1){%>
                                <li>
								    <a class="link" data-orderid="<%=OrderId %>" data-page="1" href="javascript:void(0)">First</a>
							    </li>
							    <li>
								    <a class="link"  data-orderid="<%=OrderId %>" data-page="<%=pages.CurrentPage - 1%>"  href="javascript:void(0)"><</a>
							    </li>
                        <% } %>
                            <% for (var page = pages.StartPage; page <= pages.EndPage; page++){%>
                                <li class="<%= GetactivePage(page, pages.CurrentPage)%>">   
								<a class="link"  data-orderid="<%=OrderId %>" data-page="<%=page %>"  href="javascript:void(0)"> <%= page%></a>
							</li> 
                        <% } %>
                        <% if (pages.CurrentPage < pages.TotalPages){%>
                                <li>
								    <a class="link"  data-orderid="<%=OrderId %>" data-page="<%=pages.CurrentPage + 1 %>" href="javascript:void(0)">></a>
							    </li>
							    <li>
								    <a class="link"  data-orderid="<%=OrderId %>" data-page="<%=pages.TotalPages %>" href="javascript:void(0)">Previous</a>
							    </li> 
                        <% } %>
                    </ul>
            <% } %>   
        </div>
        <div>
    <script type="text/javascript">
    var selecthtmCardType = '';
        <% if (lstProducts != null)
    {
        foreach (var item in lstProducts)
        {%>
    selecthtmCardType += ' <option data-value="" value="<%=item.Code %>"><%=item.Name%></option>';
        <% }
    }  %>
    selecthtmCardType = '<select >' + selecthtmCardType + ' </select>  ';

    var selecthtmlProvider = '';
    <% if (lstProviders != null){foreach (var item in lstProviders){%>
    selecthtmlProvider += ' <option data-value="<%#Eval("ProviderCode") %>" value="<%=item.ProviderCode %>"><%=item.ProviderCode%></option>';
    <% }}  %>
    selecthtmlProvider = '<select >' + selecthtmlProvider + ' </select>  ';

    var htmltrPack = ''
        + '<tr class=" item-tr2 item-tr2-0" data-Id="0" data-open="false" data-page="1">'
        + '<td style="width:20px;"><span class="open2" style="    display: block;    width: 20px;    padding: 0;    text-align: center;">+</span></td>'
        + '<th style="text-align:left" class="Id"> </td>  '
        + '<td class="Name"><input type="text" value="" /><span> </span> </td>'
        + '<td class="Provider">' + selecthtmlProvider +'<span> </span> </td>'
        + '<td class="CardType">' + selecthtmCardType + '<span></span></td>'
        + '<td class="NumberCard"><input type="text" value="0" style=" width:70px;" /><span></span></td>'
        //+ '<td class="NumberCardUp"><input type="text" value="0" style=" width:70px;" /><span></span></td>'
        //+ '<td class="NumberCardSole"><input type="text" value="0" style=" width:70px;" /><span></span></td>'
        + '<td class="CardValue"><input type="text"  value="0"  style=" width:70px;"  /><span></span></td>'
        + '<td class="ExpireDate"><input type="text" style=" width:70px;"  value="<%= DateTime.Now.ToString("MM/dd/yyyy")%>" /><span></span></td>'
        + '<td class="IsActive"><input type="checkbox" name="check-1" class="lcs_checkPack" autocomplete="off" /> </td>'
        + '<td class="Status"><select ><option value="0" >Nhập tay</option><option value="1">Tự động</option></select><span> </span></td>'
        + '<td style="text-align:right;">'
        + '<span class="item2-span-block1" style= "display:none"> <span class="uploadClass">Import </span> | <span class="item2-span3" data-id="0" >Sửa</span> | <span class="item2-span4" data-id="0">Xóa</span></span> '
        + '<span class="item2-span-block2" style= "display:none"><span class="item2-span6" data-id="0">Cập nhập</span> | <span class="item2-span5" data-id="0" >Hủy</span></span>'
        + '<span class="item2-span-block3" ><span class="item2-span7" data-id="0">Thêm</span> | <span class="item2-span8" data-id="">Hủy</span></span>'
        + '</td></tr>'; 
    </script>
<script type="text/javascript"> 
    $(function () {
        PacketBtnClick();
        btnClickPack();
        UnitPack();  
        $("a.link").off("click", aLink).on("click", aLink);
        $('.lcs_checkPack').lc_switch(); 

        $('body').undelegate('.lcs_checkPack', 'lcs-statuschange', lcs_checkPack).delegate('.lcs_checkPack', 'lcs-statuschange', lcs_checkPack); 


        //// triggered each time a field is checked
        //$('body').delegate('.lcs_checkPack', 'lcs-on', function () {
        //    console.log('field is checked');
        //});


        //// triggered each time a is unchecked
        //$('body').delegate('.lcs_checkPack', 'lcs-off', function () {
        //    console.log('field is unchecked');
        //});
    }); 
</script> 
</div>
    </td>
</tr> 

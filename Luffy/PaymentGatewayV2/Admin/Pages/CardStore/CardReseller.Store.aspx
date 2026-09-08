<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="CardReseller.Store.aspx.cs" Inherits="Pages_CardStore_CardReseller_Store" %> 
<tr class="Packet<%=PacketId %>">  
    <td colspan="13" style="padding:5px 0 0 45px;">
        <table class="table table-striped">
            <tr >   
                <th style="text-align:left"> StoreId</th> 
                <th>CardType</th>
                <th>CardSerial</th>
                <th>CardCode</th> 
                <th>Value</th> 
                <th>IsSold</th>
                <th>IsActive</th>  
                <th></th>
            </tr>
            <asp:repeater id="rptList" runat="server">
            <ItemTemplate>
                <tr class="<%#PacketId %> item-tr3 item-tr3-<%#Eval("Id") %>" data-open="false">   
                    <td style="text-align:left"><%#Eval("Id") %></td>  
                    <td class="CardType"><input type='text' style="display:none;"  value="<%#Eval("CardType") %>" /><span><%#Eval("CardType") %></span> </td>  
                    <td class="CardSerial"><input type='text' style="display:none;"  value="<%#Eval("CardSerial") %>" /><span><%#Eval("CardSerial") %></span> </td>  
                    <td class="CardCode"><span>**************</span> </td> 
                    <td class="CardValue"><span> <%#Eval("CardValue") %></span> </td>  
                    <td class="IsSold"> 
                        <select style="display:none" > 
                             <option value="False" data-value="<%#Eval("IsActive") %>">False</option>
                             <option value="True" data-value="<%#Eval("IsActive") %>">True</option> 
                        </select> 
                        <span><%# GetSold((bool)Eval("IsSold")) %></span>  
                    </td>  
                    <td class="IsActive">
                            <input type="checkbox" name="check-1" <%# (bool)Eval("IsActive") == true ? "checked": ""  %> value="<%#Eval("Id") %>" class="lcs_checkStore" autocomplete="off" /> 
                    </td>  
                    <td style="text-align:right;">
                        <span class="item3-span-block1">
                              <span class="item3-span3" data-id="<%#Eval("Id") %>" >Sửa</span> | <span  class="item3-span4" data-id="<%#Eval("Id") %>">Xóa</span> 
                        </span>
                        <span class="item3-span-block2" style="display:none" >
                         <span class="item3-span6" data-id="<%#Eval("Id") %>">Cập nhập</span>  | <span class="item3-span5" data-id="<%#Eval("Id") %>" >Hủy</span> 
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
								    <a class="link2" data-packid="<%=packetId %>" data-page="1" href="javascript:void(0)">First</a>
							    </li>
							    <li>
								    <a class="link2"  data-packid="<%=packetId %>" data-page="<%=pages.CurrentPage - 1%>"  href="javascript:void(0)"><</a>
							    </li>
                        <% } %>
                            <% for (var page = pages.StartPage; page <= pages.EndPage; page++){%>
                                <li class="<%= GetactivePage(page, pages.CurrentPage)%>">   
								<a class="link2"  data-packid="<%=packetId %>" data-page="<%=page %>"  href="javascript:void(0)"> <%= page%></a>
							</li> 
                        <% } %>
                        <% if (pages.CurrentPage < pages.TotalPages){%>
                                <li>
								    <a class="link2"  data-packid="<%=packetId %>" data-page="<%=pages.CurrentPage + 1 %>" href="javascript:void(0)">></a>
							    </li>
							    <li>
								    <a class="link2"  data-packid="<%=packetId %>" data-page="<%=pages.TotalPages %>" href="javascript:void(0)">Previous</a>
							    </li> 
                        <% } %>
                    </ul>
            <% } %>   
        </div>
    </td>
</tr>
<input type="checkbox" class="" />
<script type="text/javascript"> 
    $(function () {   
        CardStoreBtnClick();
        $("a.link2").off("click", aLink2).on("click", aLink2);
        $('.lcs_checkStore').lc_switch();
        $('body').undelegate('.lcs_checkStore', 'lcs-statuschange', lcs_checkStore).delegate('.lcs_checkStore', 'lcs-statuschange', lcs_checkStore); 
    });
 
</script>

 
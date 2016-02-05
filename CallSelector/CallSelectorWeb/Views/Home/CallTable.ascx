<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CallSelectorWeb.Models.HomeCallTable>" %> 

    <% HttpContext.Current.Response.Cache.SetNoStore(); %>

    <a id="home_controller_toggle" href="javascript:void(0)" class=" <%=(("1".Equals(Model.ShowImage))?"toggled":"untoggled") %>">
    
    </a> 
    <div id="home_controller_image">
    </div>
    
    <% System.Data.DataTable tab =  Model.Table;
       Dictionary<string, CallSelectorLib.IWebFileConfig> dict = Model.dicSenderConfig;
       TimeSpan totalTime = new TimeSpan();
       
       bool printtimeInterval = tab.Rows.Count > 0;
       DateTime prevTime = (printtimeInterval?((DateTime)tab.Rows[0]["date_start"]).Date:DateTime.Now);
       foreach (System.Data.DataRow row in  tab.Rows)
       {
           CallSelectorLib.IWebFileConfig cfg = null;
           bool hasFile = dict.TryGetValue((String) row["sender_mail"], out cfg);
           string fileName = (hasFile)
                                 ? (
                                       cfg.DirectoryForAudioFiles() +
                                       System.IO.Path.DirectorySeparatorChar +
                                       row["file_name"])
                                 : "no_file";
           fileName = fileName.Replace(System.IO.Path.DirectorySeparatorChar, '/'); //we are on the web now
           totalTime += (TimeSpan)row["date_interval"];
           if(printtimeInterval)
           {
               printtimeInterval = prevTime.Equals(((DateTime) row["date_start"]).Date);
               prevTime = ((DateTime) row["date_start"]).Date;
           }
       }


    %>
    
    <table class="fixed" id="phone_call_id">
        <tr>
                    <th class="home_operator_mail">Оператор</th>
                    <th class="home_phone_number">Номер</th>
                    <th class="home_call_date">Дата и время 
                        <%= ((printtimeInterval)?( " ( " + ((DateTime)tab.Rows[tab.Rows.Count - 1]["date_start"]).TimeOfDay + " - " + 
                                                           ((DateTime)tab.Rows[0                 ]["date_start"]).TimeOfDay + " )"):"")%> 
                    </th>
                    <th class="home_call_time">Продолжительность ( <%= totalTime.ToString()%> )</th>
                    <th class="home_phone_sleep">Перерыв</th>
                    <th class="home_call_mp3">MP3</th>
                    <th class="home_table_id"># ( <%= tab.Rows.Count %> )</th>
        </tr>

        <% System.Data.DataTable table =  Model.Table;
           Dictionary<string, CallSelectorLib.IWebFileConfig> dic = Model.dicSenderConfig;
           int iRow = table.Rows.Count;
           DateTime prev = (table.Rows.Count > 0)?((DateTime) table.Rows[0]["date_start"]).Add((TimeSpan) table.Rows[0]["date_interval"]):DateTime.Now;
           foreach (System.Data.DataRow row in  table.Rows)
           {
               DateTime cur = ((DateTime) row["date_start"]).Add((TimeSpan) row["date_interval"]);
               TimeSpan sleepInterval = (prev > cur)?(prev - cur):TimeSpan.Zero;
               prev = ((DateTime) row["date_start"]);
               CallSelectorLib.IWebFileConfig cfg = null;
               bool hasFile = dic.TryGetValue((String)row["sender_mail"], out cfg);
               string fileName = (hasFile) ? (
                  "/Home/GetFile?fileName=" + cfg.DirectoryForAudioFiles() +
                  System.IO.Path.DirectorySeparatorChar +
                  row["file_name"]) : "no_file";
               fileName = fileName.Replace(System.IO.Path.DirectorySeparatorChar, '/');//we are on the web now
                   %>
        <tr>

            <td class="home_operator_mail">
                <a id="operator_<%= iRow %>" class="filterInline" href="javascript:void(0)">
                   <%= Html.Encode(row["operator_mail"])%> 
                </a> 
            </td>
            <td class="home_phone_number">
                 <a id="phone_number_<%= iRow %>"  class="filterInline"  href="javascript:void(0)">
                    <%= Html.Encode(row["phone"])%>  
                 </a> 
            </td>
            <td class="home_call_date"> <%= Html.Encode(row["date_start"])%> </td>
            <td class="home_call_time"> <%= Html.Encode(row["date_interval"])%> </td>
            <td class="home_call_sleep"> <%= Html.Encode(sleepInterval)%> </td>
            <td class="home_call_mp3" id="flashcontent">
            <div id="div<%= iRow %>">
                <a id="lnk<%= iRow %>" href="javascript:void(0)" class="player_link">
                    >>>
                </a> 
            </div>
            
            <input type="hidden" name="div_player_link" id="o<%= iRow %>" value='
                <a id="lnk<%= iRow %>" href="javascript:void(0)" class="player_link">
                    >>>
                </a>'/>
            
                <input type="hidden" name="div_player" id="obj<%= iRow %>" value='<div id="objx<%= iRow %>"><object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" width="100%" height="20"
                 codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab">
                    <param name="movie" value="../../Scripts/ThirdParty/singlemp3player.swf?file=<%= Server.UrlEncode(fileName) %>&showDownload=true&autoStart=true" />
                    <param name="wmode" value="transparent" />
                    <embed wmode="transparent" width="100%" height="20" src="../../Scripts/ThirdParty/singlemp3player.swf?file=<%= Server.UrlEncode(fileName) %>&showDownload=true&autoStart=true"
                        type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />
            </object></div>' />
            </td>
            <td class="home_table_id"><%= iRow-- %></td>
        </tr>
        <% } %>
        
        <%= Html.HiddenFor(model => model.RequestForm)%>
    </table> 


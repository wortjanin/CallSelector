<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallSelectorWeb.Models.HomeIndexFilter>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Система учёта звонков - Таблица звонков
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../../Scripts/Index/common.js"></script>
    <script type="text/javascript">

        $(document).ready(function() {
            if (null == $.datepicker) { // ??

                ui_datepicker($, { // Default regional settings
                    clearText: 'Очистить', // Display text for clear link
                    clearStatus: 'Очистить тек. дату', // Status text for clear link
                    closeText: 'Закрыть', // Display text for close link
                    closeStatus: 'Закрыть без сохр.', // Status text for close link
                    prevText: '&#x3c;Пред.', // Display text for previous month link
                    prevStatus: 'Пред. мес.', // Status text for previous month link
                    nextText: 'След.&#x3e;', // Display text for next month link
                    nextStatus: 'След. мес.', // Status text for next month link
                    currentText: 'Текущ.', // Display text for current month link
                    currentStatus: 'Текущ. мес.', // Status text for current month link
                    monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
		        	'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'], // Names of months for drop-down and formatting
                    monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн', 'Июл', 'Авг', 'Сен', 'Окт', 'Нбр', 'Дек'], // For formatting
                    monthStatus: 'Выбрать другой месяц', // Status text for selecting a month
                    yearStatus: 'Выбрать другой год', // Status text for selecting a year
                    weekHeader: 'Неделя', // Header for the week of the year column
                    weekStatus: 'Неделя', // Status text for the week of the year column
                    dayNames: ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'], // For formatting
                    dayNamesShort: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'], // For formatting
                    dayNamesMin: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'], // Column headings for days starting at Sunday
                    dayStatus: 'Выбрать первый день недели', // Status text for the day of the week selection
                    dateStatus: 'Выбрать день, месяц, год', // Status text for the date selection
                    dateFormat: 'dd.mm.yy', // See format options on parseDate
                    firstDay: 1, // The first day of the week, Sun = 0, Mon = 1, ...
                    initStatus: 'Выбрать дату', // Initial Status text on opening
                    isRTL: false // True if right-to-left language, false if left-to-right
                });
            }
            setupDatepicker(false);

            setClicks();
        });
    </script>
    
    <h2> Таблица звонков </h2>

    <%  Html.EnableClientValidation(); %>    
    <% 
        using (Ajax.BeginForm(new AjaxOptions
        {   
            HttpMethod = "POST",          
            UpdateTargetId = "CallTableContent",
            InsertionMode = InsertionMode.Replace,
            OnSuccess = "success",
            OnComplete = "OnComplete",
            OnFailure = "failure",
            Url = "Home/CallTable"
        })){ %>
        
        <table class="fixed">
                <tr>
                    <th class="home_operator_mail">Оператор</th>
                    <th class="home_phone_number">Номер</th>
                    <th class="home_call_date">Дата</th>
                    <th class="home_call_time">Продолжительность</th>
                    <th class="home_phone_sleep">Перерыв</th>
                    <th class="home_call_mp3">&nbsp;</th>
                    <th class="home_table_id">&nbsp;</th>
                </tr>
        
                <tr id="FilterInput">
                    <td class="home_operator_mail">                           
                          <!-- %= Html.EditorFor(model => model.OperatorMail) %><br/>  
                          <= Html.ValidationMessageFor(model => model.OperatorMail)% -->
                        <select id="OperatorMail" name="OperatorMail">
                            <option value=""></option>
                              <%
                                  foreach (System.Data.DataRow row in Model.HomeCallTable.Operators.Rows)
                                  {
                                      var cell = row["mail"].ToString();
                                      %>  <option value="<%=cell%>"><%=cell%></option>  <%
                                  }
                              %>
                    	</select>

                    </td>
                    <td class="home_phone_number"> 
                          <%= Html.EditorFor(model => model.PhoneNumber) %><br/>  
                          <%= Html.ValidationMessageFor(model => model.PhoneNumber)%>&nbsp; 
                    </td>
                    <td class="home_call_date">  
                          <%= Html.EditorFor(model => model.DateMin) %><br/>  
                          <%= Html.ValidationMessageFor(model => model.DateMin)%>&nbsp;<br/>
                          
                          <%= Html.EditorFor(model => model.DateMax) %><br/>
                          <%= Html.ValidationMessageFor(model => model.DateMax)%>&nbsp;<br/>
                    </td>
                    <td class="home_call_time"> 
                          <%= Html.TextBoxFor(model => model.TimeMin) %><br/>  
                          <%= Html.ValidationMessageFor(model => model.TimeMin)%>&nbsp;<br/>
                          
                          <%= Html.TextBoxFor(model => model.TimeMax)%><br/>
                          <%= Html.ValidationMessageFor(model => model.TimeMax)%>&nbsp; 
                    </td>
                    <td class="home_phone_sleep">&nbsp;</td>
                    <td class="home_call_mp3">
                        <input type="submit" id="ButtonFilter"  class="button" value="Найти" />
                        <br/>
                        <br/>
                        <br/><input type="button" id="ButtonClear" class="button" value="Очистить" />
                    </td>
                    <td class="home_table_id">
                        &nbsp; 
                    </td>
                </tr>
         </table>
         <div class="error" id="server_responce"></div>
         
         <%= Html.HiddenFor(model => model.HomeCallTable.ShowImage)%>
         
         <!-- input type="hidden" name="ShowImage" value="0"/ -->
     <%} %>
     <%= Html.ValidationSummary(true)  %>

    <div id="CallTableContent">
        <% Html.RenderPartial("CallTable", Model.HomeCallTable); %>         
    </div>
</asp:Content>

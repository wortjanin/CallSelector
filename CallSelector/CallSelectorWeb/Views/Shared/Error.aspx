<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Система учёта звонков - Ошибка
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">       

    <h2>
        Произошла ошибка
    </h2>

	<% if (null != Model && null != Model.Exception)
    { %>
		<p>
		  Контроллер: <%= Model.ControllerName %>
		</p>
		<p>
		  Событие: <%= Model.ActionName %>
		</p>
		<p>
		  Сообщение: <%= Model.Exception.Message%>
		</p>
		<p>
		  Стэк трейс:
		  </p>
		<pre class="error">
		   <%= Model.Exception.StackTrace%>
		</pre> 
		
	<% } %>
</asp:Content>
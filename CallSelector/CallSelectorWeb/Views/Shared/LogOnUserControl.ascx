<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        Привет <b><%= Html.Encode(Page.User.Identity.Name) %></b>!
        [ <%= Html.ActionLink("Выход здесь", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
       [ <%=  Html.ActionLink("Вход", "LogOn", "Account")%> ]
<%
    }
%>

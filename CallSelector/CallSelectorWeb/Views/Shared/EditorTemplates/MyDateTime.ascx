<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CallSelectorWeb.Common.Models.MyDateTime>" %>

<%= Html.TextBox("", null != Model && Model.DateTime.HasValue
    ? Html.Encode(Model.Value) 
    : "", new { @class = "datePicker" })%>

<input  type="hidden" 
        id="MyDateTime_ascx_DateTimeFormat" 
        value="<%= Model.jDateFormat %>" />


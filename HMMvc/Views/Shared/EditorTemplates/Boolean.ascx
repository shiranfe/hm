<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
    private List<SelectListItem> TriStateValues
    {
        get
        {
            return new List<SelectListItem> {
                new SelectListItem { Text = "Not Set",
                                     Value = String.Empty,
                                     Selected = !Value.HasValue },
                new SelectListItem { Text = "True",
                                     Value = "true",
                                     Selected = Value.HasValue && Value.Value },
                new SelectListItem { Text = "False",
                                     Value = "false",
                                     Selected = Value.HasValue && !Value.Value },
            };
        }
    }
    private bool? Value
    {
        get
        {
            bool? value = null;
            if (ViewData.Model != null)
            {
                value = Convert.ToBoolean(ViewData.Model,
                                          System.Globalization.CultureInfo.InvariantCulture);
            }
            return value;
        }
    }
</script>
<%
    var placeholder = string.Empty;
    if (ViewData.ModelMetadata.Watermark != null)
    {
        placeholder = ViewData.ModelMetadata.Watermark;
    }  
%>
<% if (ViewData.ModelMetadata.IsNullableValueType)
   { %>

<%= Html.DropDownList("", TriStateValues, new { placeholder = placeholder, @class = "TextBox_small" })%>
<% }
   else
   { %>
<%= Html.CheckBox("", Value ?? false, new {placeholder = placeholder, @class = "check-box" })%>
<% } %>
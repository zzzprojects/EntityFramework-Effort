<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Effort.Example.Models.CategoryListModel>" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
	Categories
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <h2>List of all catergories</h2>

    <ul>

    <% for (int i = 0; i < this.Model.Categories.Count; i++) { %>
           
        <li>
           
            <%: Html.ActionLink(this.Model.Categories[i].CategoryName, "Details", new { id = this.Model.Categories[i].CategoryID })%>

        </li>


    <%   } %>

    </ul>

</asp:Content>

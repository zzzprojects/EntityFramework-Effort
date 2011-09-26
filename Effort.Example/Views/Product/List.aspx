<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MMDB.EntityFrameworkProvider.TddTest.Models.ProductListModel>" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
	Products
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <h2>List of all products</h2>

    <ul>

    <% for (int i = 0; i < this.Model.Products.Count; i++) { %>
           
        <li>
           
            <%: Html.ActionLink(this.Model.Products[i].ProductName, "Details", new { id = this.Model.Products[i].ProductID }) %>

        </li>


    <%   } %>

    </ul>

</asp:Content>

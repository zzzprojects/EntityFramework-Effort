<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Effort.Example.Models.Products>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ViewPage1
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Model.ProductName %></h2>

    <fieldset>
        <legend>Details</legend>
        
        <div class="display-label">QuantityPerUnit</div>
        <div class="display-field"><%: Model.QuantityPerUnit %></div>
        
        <div class="display-label">UnitPrice</div>
        <div class="display-field"><%: String.Format("{0:F}", Model.UnitPrice) %></div>
        
        <div class="display-label">UnitsInStock</div>
        <div class="display-field"><%: Model.UnitsInStock %></div>
        
        <div class="display-label">UnitsOnOrder</div>
        <div class="display-field"><%: Model.UnitsOnOrder %></div>
        
        <div class="display-label">ReorderLevel</div>
        <div class="display-field"><%: Model.ReorderLevel %></div>
        
        <div class="display-label">Discontinued</div>
        <div class="display-field"><%: Model.Discontinued %></div>
        
    </fieldset>
    <p>

        <%: Html.ActionLink("Back to List", "List") %>
    </p>

</asp:Content>


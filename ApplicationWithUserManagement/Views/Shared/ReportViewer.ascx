<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.ascx.cs" Inherits="SoniReports.Views.Shared.ReportViewer" %>
<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>

<form id="form1" runat="server">
<div style="Height:720px;Width:800px">
    <asp:ScriptManager ID="scriptManager" runat="server" ScriptMode="Release" EnablePartialRendering="false" />
    <rsweb:ReportViewer Width="100%" Height="100%" ID="reportViewer" ShowPrintButton="true" KeepSessionAlive="true" runat="server" AsyncRendering="false" ProcessingMode="Remote">
         <ServerReport />
    </rsweb:ReportViewer>
</div>
</form>
using System;
using System.Web.Mvc;
using SoniReports.ViewModels;

namespace SoniReports.Views.Shared
{
    public partial class ReportViewer :  ViewUserControl
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            // Required for report events to be handled properly.
            //reportViewer.ServerReport.ReportServerCredentials = new ReportServerCredentials();
            Context.Handler = Page;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var model = (ReportViewModel)Model;
                //reportViewer.ServerReport.ReportServerCredentials = model.ServerCredentials;
                //ReportParameter[] RptParameters = model.parameters;

                reportViewer.ServerReport.ReportPath = model.ReportPath;
                reportViewer.ServerReport.ReportServerUrl = model.ReportServerURL;

                //if (RptParameters.Count() > 0) this.reportViewer.ServerReport.SetParameters(RptParameters);
              
                reportViewer.ServerReport.Refresh();

            }            
        }
    }
}
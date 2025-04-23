using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Web.UI;

namespace Paperwork
{
    public partial class PrintMMB : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                TextBoxStartDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1).ToShortDateString();
                TextBoxEndDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1)).ToShortDateString();

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = GetGlobalSettingString("SSRS_BaseFolder");
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                UpdateReportParams();
            }
            else
            {
                Page.Validate();
            }
        }

        protected void UpdateReportParams()
        {

            //prevent invalid dates from crashing the report
            if (DateTime.TryParse(TextBoxStartDate.Text, out DateTime testDate) == false)
                return;
            if (DateTime.TryParse(TextBoxEndDate.Text, out DateTime testDate2) == false)
                return;

            ReportParameter paramProgYear = new ReportParameter("ProgramYear", Session["ProgramYear"].ToString());
            ReportParameter paramDistID = new ReportParameter("DistrictID", Session["CurrentDistrict"].ToString());
            ReportParameter paramStartDate = new ReportParameter("MMBDateStart", TextBoxStartDate.Text);
            ReportParameter paramEndDate = new ReportParameter("MMBDateEnd", TextBoxEndDate.Text);
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramStartDate, paramEndDate });
            }
            catch { }
            ReportViewer1.ServerReport.Refresh();
            ReportViewer1.ShowPrintButton = true;
            //ReportViewer1.ShowParameterPrompts = true;
        }

        //When month is changed - reopen the calendar
        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

    }
}
using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI;

namespace Paperwork
{
    public partial class PrintSpFnLedger : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;

                //Current Month Start
                TextBoxStartDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(0).ToShortDateString();
                //Current Month End
                TextBoxEndDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1)).ToShortDateString();

                UpdateReportParams();
            }
            else
            {
                Page.Validate();
            }
        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void UpdateReportParams()
        {

            ReportParameter paramDistID = new ReportParameter("DistrictID", Session["CurrentDistrict"].ToString());
            ReportParameter paramProgYear = new ReportParameter("ProgramYear", Session["ProgramYear"].ToString());
            ReportParameter paramStartDate = new ReportParameter("StartingDate", TextBoxStartDate.Text);
            ReportParameter paramEndDate = new ReportParameter("EndingDate", TextBoxEndDate.Text);
            ReportParameter paramRecords = new ReportParameter("RecordSelection", DDLRecords.SelectedValue);


            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramStartDate, paramEndDate, paramRecords });
            }
            catch { }
            ReportViewer1.ServerReport.Refresh();
            ReportViewer1.ShowPrintButton = true;
        }

        protected void DDLRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
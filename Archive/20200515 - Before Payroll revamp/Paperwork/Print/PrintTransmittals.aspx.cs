using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Web.UI;

namespace Paperwork
{
    public partial class PrintTransmittals : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                //TextBoxStartDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1).ToShortDateString();
                //TextBoxEndDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1)).ToShortDateString();
                //Tom wants the default dates to be the next friday
                //Friday=5
                DateTime NextFriday = DateTime.Now.AddDays(5 - (int)DateTime.Now.DayOfWeek);

                TextBoxStartDate.Text = NextFriday.ToShortDateString();
                TextBoxEndDate.Text = NextFriday.ToShortDateString();

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;

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

            ReportParameter paramDistID = new ReportParameter("DistrictID", Session["CurrentDistrict"].ToString());
            ReportParameter paramStartDate = new ReportParameter("PostDateBeginning", TextBoxStartDate.Text);
            ReportParameter paramEndDate = new ReportParameter("PostDateEnding", TextBoxEndDate.Text);
            ReportParameter paramGroupByPostDate = new ReportParameter("GroupByPostDate", CBGroupByPostDate.Checked ? "true" : "false");
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramDistID, paramStartDate, paramEndDate, paramGroupByPostDate });
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

        protected void CBGroupByPostDate_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

    }
}
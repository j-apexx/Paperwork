using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI;

namespace Paperwork
{
    public partial class PrintDetailedProfitFB : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["ProgramYear"] == null )
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                else
                {
                    if (DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }

                DDLDistricts.DataBind();

                if (DDLDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (DDLDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DDLDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                    else
                        DDLDistricts.SelectedIndex = 0;
                }


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

            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramStartDate = new ReportParameter("StartingDate", TextBoxStartDate.Text);
            ReportParameter paramEndDate = new ReportParameter("EndingDate", TextBoxEndDate.Text);
            ReportParameter paramDistID = new ReportParameter("DistrictID", DDLDistricts.SelectedValue);

            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramStartDate, paramEndDate, paramDistID });
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

        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            string storeDist = DDLDistricts.SelectedValue;
            DDLDistricts.Items.Clear();
            DDLDistricts.DataBind();

            //maintain selected dist if possible
            if (DDLDistricts.Items.FindByValue(storeDist) != null)
                DDLDistricts.SelectedValue = storeDist;
            else if(DDLDistricts.Items.Count>0)
                DDLDistricts.SelectedIndex = 0;

            UpdateReportParams();
        }

    }
}
using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI;

namespace Paperwork
{
    public partial class PrintCPMPLReport : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || string.IsNullOrEmpty(Session["CurrentDistrict"].ToString()) )
                Response.Redirect("~/Default.aspx");

            switch (Session["AccessLevel"].ToString().ToUpper())
            {
                case "ADMIN":
                case "OFFICE":
                case "RM":
                    break;
                default:
                    //Denied
                    Response.Redirect("~/Default.aspx");
                    break;
            }

            if (!IsPostBack)
            {
                if (Session["CPMInvoiceDate"] != null && DateTime.TryParse(Session["CPMInvoiceDate"].ToString(), out DateTime outInvoiceDate))
                {
                    TextBoxStartDate.Text = (new DateTime(outInvoiceDate.Year, outInvoiceDate.Month, 1)).ToShortDateString();
                    TextBoxEndDate.Text = (new DateTime(outInvoiceDate.Year, outInvoiceDate.Month, 1).AddMonths(1).AddDays(-1)).ToShortDateString();
                }
                else
                {
                    TextBoxStartDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).ToShortDateString();
                    TextBoxEndDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1)).ToShortDateString();
                }
                
                DDLProgramYears.DataBind();
                if(DDLProgramYears.Items.Count==0)
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
                if (Session["CPMDistID"] != null)
                {
                    if (DDLDistricts.Items.FindByValue(Session["CPMDistID"].ToString()) != null)
                        DDLDistricts.SelectedValue = Session["CPMDistID"].ToString();
                    else
                        DDLDistricts.SelectedIndex = 0;
                }

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = GetGlobalSettingString("SSRS_BaseFolder");
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                UpdateReportParams();

                Session.Remove("CPMDistID");
                Session.Remove("CPMInvoiceDate");
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
            ReportParameter paramDistID = new ReportParameter("DistrictID", DDLDistricts.SelectedValue);
            ReportParameter paramStartDate = new ReportParameter("StartDate", TextBoxStartDate.Text);
            ReportParameter paramEndDate = new ReportParameter("EndDate", TextBoxEndDate.Text);
            ReportParameter paramGuarantee = new ReportParameter("DisplayGuarantee", CBShowGuarantee.Checked.ToString());
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramStartDate, paramEndDate, paramGuarantee });
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

        protected void CBShowGuarantee_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
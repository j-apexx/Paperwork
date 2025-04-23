using Microsoft.Exchange.WebServices.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintReimbVerification : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            //District Session ID not set
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/Default.aspx");

            //District Session ID not set
            if (Session["CurrentDistrict"] == null || string.IsNullOrEmpty(Session["CurrentDistrict"].ToString()))
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

            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;


            if (!IsPostBack)
            {
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

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

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                DDLProgramYears_SelectedIndexChanged(null, null);

                if (Session["CurrentYearMonth"] == null || string.IsNullOrEmpty(Session["CurrentYearMonth"].ToString()))
                {
                    TextBoxYearmonth.Text = DateTime.Now.AddMonths(-1).Year.ToString() + "-" + DateTime.Now.AddMonths(-1).Month.ToString().PadLeft(2, '0');
                }
                else
                {
                    TextBoxYearmonth.Text = Session["CurrentYearMonth"].ToString();
                }
                TextBoxYearmonth_TextChanged(TextBoxYearmonth, EventArgs.Empty);
            }
            else
            {
                Page.Validate();
            }
        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.Parameters["@ProgramYear"].Value = DDLProgramYears.SelectedValue;
        }

        protected void UpdateReportParams()
        {
            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramDist = new ReportParameter("DistrictID", DropDownListDistricts.SelectedValue);
            ReportParameter paramDistYearMonth = new ReportParameter("YearMonth", TextBoxYearmonth.Text);

            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDist, paramDistYearMonth });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }

            ReportViewer1.ShowPrintButton = true;
            //ReportViewer1.ShowParameterPrompts = true;
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            //When program year changes, districts could change as well
            //Dists are now based on ProgramYear and must be rebound
            DropDownListDistricts.DataBind();
            if (DropDownListDistricts.Items.Count == 0)
                Response.Redirect("~/default.aspx");
            if (Session["CurrentDistrict"] != null)
            {
                if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                    DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                else
                    DropDownListDistricts_SelectedIndexChanged(null, null);
            }


            UpdateReportParams();
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void TextBoxYearmonth_TextChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
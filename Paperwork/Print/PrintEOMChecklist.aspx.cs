using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintEOMChecklist : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ProgramYear not set
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/Default.aspx");

            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DDLDistricts.DataSource = SqlDataSourceMyDistsMstr;
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

            }
            else
            {
                Page.Validate();
            }
        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (DDLProgramYears.Items.Count == 0)
                e.Command.Parameters["@ProgramYear"].Value = "";
            else
                e.Command.Parameters["@ProgramYear"].Value = DDLProgramYears.SelectedValue;
        }

        protected void DDLInvYearMonths_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            string storedDist = DDLDistricts.SelectedValue;
            //District List is Year Based and needs to be rebound
            DDLDistricts.DataBind();
            if (DDLDistricts.Items.Count == 0)
                Response.Redirect("~/default.aspx");

            if (DDLDistricts.SelectedIndex == 0 && Session["CurrentDistrict"] != null)
            {
                if (DDLDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                    DDLDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                else
                    DDLDistricts.SelectedIndex = 0;
            }

            if (storedDist != null && storedDist != string.Empty && (Session["CurrentDistrict"] != null && storedDist != Session["CurrentDistrict"].ToString()))
            {
                if (DDLDistricts.Items.FindByValue(storedDist) != null)
                    DDLDistricts.SelectedValue = storedDist;
            }


            DDLInvYearMonths.DataBind();
            if (DDLInvYearMonths.Items.Count >= 1)
            {
                switch (DateTime.Now.Month)
                {
                    case 7:
                    case 8:
                        DDLInvYearMonths.SelectedIndex = 0;
                        break;
                    case 9:
                        DDLInvYearMonths.SelectedIndex = 1;
                        break;
                    case 10:
                        DDLInvYearMonths.SelectedIndex = 2;
                        break;
                    case 11:
                        DDLInvYearMonths.SelectedIndex = 3;
                        break;
                    case 12:
                        DDLInvYearMonths.SelectedIndex = 4;
                        break;
                    case 1:
                        DDLInvYearMonths.SelectedIndex = 5;
                        break;
                    case 2:
                        DDLInvYearMonths.SelectedIndex = 6;
                        break;
                    case 3:
                        DDLInvYearMonths.SelectedIndex = 7;
                        break;
                    case 4:
                        DDLInvYearMonths.SelectedIndex = 8;
                        break;
                    case 5:
                        DDLInvYearMonths.SelectedIndex = 9;
                        break;
                    case 6:
                        DDLInvYearMonths.SelectedIndex = 10;
                        break;

                }
            }

            if (Session["CurrentYearMonth"] != null && Session["CurrentYearMonth"].ToString().Length > 0)
            {
                if (DDLInvYearMonths.Items.FindByValue(Session["CurrentYearMonth"].ToString()) != null)
                    DDLInvYearMonths.SelectedValue = Session["CurrentYearMonth"].ToString();
            }


            UpdateReportParams();
        }

        protected void UpdateReportParams()
        {

            //prevent invalid dates from crashing the report
            if (DDLInvYearMonths.Items.Count == 0)
                return;

            ReportParameter paramDistID = new ReportParameter("DistrictID", DDLDistricts.SelectedValue);
            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramInvYearMonth = new ReportParameter("YearMonth", DDLInvYearMonths.SelectedValue);

            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramInvYearMonth });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }

            ReportViewer1.ShowPrintButton = true;
        }


    }
}
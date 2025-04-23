using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintFMSBudget : Paperwork_Global
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

            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DDLDistricts.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;
            //How to Filter
            //SqlDataSourceMyDistsMstr.FilterExpression = "AcctType IN (1,2)";

            if (!IsPostBack)
            {
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

                DDLBudgetYears.DataBind();
                if (DDLBudgetYears.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                else
                {
                    if (Session["BudgetYear"]!= null && DDLBudgetYears.Items.FindByValue(Session["BudgetYear"].ToString()) != null)
                        DDLBudgetYears.SelectedValue = Session["BudgetYear"].ToString();
                    else
                        DDLBudgetYears.SelectedIndex = 0;
                }

                DDLDistricts.DataBind();

                if (DDLDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (DDLDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DDLDistricts.SelectedValue = Session["CurrentDistrict"].ToString();

                }

                if (Session["BudgetVersion"] != null)
                    TextBoxBudgetVersion.Text = Session["BudgetVersion"].ToString();
                else
                    TextBoxBudgetVersion.Text = "1";


                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer2.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;
                ReportViewer2.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer2.ServerReport.ReportPath;

            }
            else
            {
                Page.Validate();
            }
        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (Session["ProgramYear"]==null)
                e.Command.Parameters["@ProgramYear"].Value = "";
            else
                e.Command.Parameters["@ProgramYear"].Value = Session["ProgramYear"].ToString();

        }

        protected void UpdateReportParams()
        {
            ReportParameter paramBdgtYear = new ReportParameter("BudgetYear", DDLBudgetYears.SelectedValue);
            ReportParameter paramDistrict = new ReportParameter("DistrictID", DDLDistricts.SelectedValue);
            ReportParameter paramBudgetVersion = new ReportParameter("BudgetVersion", TextBoxBudgetVersion.Text);

            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramBdgtYear, paramDistrict, paramBudgetVersion });
                ReportViewer1.ServerReport.Refresh();

                ReportViewer2.ServerReport.SetParameters(new ReportParameter[] { paramBdgtYear, paramDistrict, paramBudgetVersion });
                ReportViewer2.ServerReport.Refresh();
            }
            catch { }

            ReportViewer1.ShowPrintButton = true;
        }

        //When month is changed - reopen the calendar
        protected void TextBoxVersion_TextChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLBudgetYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
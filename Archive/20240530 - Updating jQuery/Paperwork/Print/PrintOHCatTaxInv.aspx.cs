using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintOHCatTaxInv : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            //Prog Year not set
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

        protected void UpdateReportParams()
        {

            //prevent invalid dates from crashing the report
            if (DateTime.TryParse(DDLInvoices.SelectedValue, out DateTime testDate) == false)
                return;

            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramDistrict = new ReportParameter("DistrictID", DDLDistricts.SelectedValue);
            ReportParameter paramPostDate = new ReportParameter("PostDate", DDLInvoices.SelectedValue);
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistrict, paramPostDate });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }

            ReportViewer1.ShowPrintButton = true;
            //ReportViewer1.ShowParameterPrompts = true;
        }

        //When month is changed - reopen the calendar
        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox tbDate = (TextBox)sender;
            NormalizeSimplifiedDate(ref tbDate);
            this.Validate();

            UpdateReportParams();
        }

        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDLInvoices.DataBind();


            //UpdateReportParams();
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

            DDLDistricts_SelectedIndexChanged(sender, e);

            //UpdateReportParams();
        }

        protected void DDLInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintOperatingStatistics : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["ProgramYear"] == null)
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

                TextBoxStartDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1).ToShortDateString();
                TextBoxEndDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1)).ToShortDateString();

                //LBDists.DataBind();
                //CBSelectAll_CheckedChanged(null,null);

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
            ReportParameter paramVPs = new ReportParameter("VP");
            ReportParameter paramCPMOnly = new ReportParameter("CPMOnly", CB_CPMOnly.Checked ? "True" : "False");
            ReportParameter paramStateFilter = new ReportParameter("StateFilter", DDLStateFilter.SelectedValue);
            ReportParameter paramCEP = new ReportParameter("CEPFilter", DDLCEP.SelectedValue);
            ReportParameter paramDistFilter = new ReportParameter("Districts");
            foreach (ListItem li in CheckBoxListVPs.Items)
            {
                if (li.Selected)
                    paramVPs.Values.Add(li.Value);
            }

            if (DDLDistricts.SelectedValue != "" && DDLDistricts.SelectedValue != "%")
            {
                paramDistFilter.Values.Add(DDLDistricts.SelectedValue);

                //report will not run if no VP is selected. Select them all to allow the specific dist to appear
                if(paramVPs.Values.Count==0)
                {
                    foreach (ListItem li in CheckBoxListVPs.Items)
                    {
                        paramVPs.Values.Add(li.Value);
                    }
                }
            }
            else
            {
                foreach (ListItem li in DDLDistricts.Items)
                {
                    if (li.Value == "" || li.Value == "%")
                        continue;
                    paramDistFilter.Values.Add(li.Value);
                }
            }

            try
            {

                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramStartDate, paramEndDate, paramVPs, paramCPMOnly, paramStateFilter, paramCEP, paramDistFilter });
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

            UpdateReportParams();
        }

        protected void CheckBoxListVPs_SelectedIndexChanged(object sender, EventArgs e)
        {
            //UpdateReportParams();
        }

        protected void ISync_Click(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void CB_CPMOnly_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void CBSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            //if (CBSelectAll.Checked)
            //{
            //    LBDists.Enabled = false;
            //    foreach (ListItem li in LBDists.Items)
            //    {
            //        li.Selected= true;
            //    }
            //}
            //else
            //{
            //    LBDists.Enabled = true;
            //}
        }
        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (DDLProgramYears.Items.Count == 0)
                e.Command.Parameters["@ProgramYear"].Value = "";
            else
                e.Command.Parameters["@ProgramYear"].Value = DDLProgramYears.SelectedValue;
        }
    }
}
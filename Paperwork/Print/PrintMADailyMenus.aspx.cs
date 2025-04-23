using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintMADailyMenus : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

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
                    if (Session["PrintProgramYear"]!=null && DDLProgramYears.Items.FindByValue(Session["PrintProgramYear"].ToString()) != null)
                    {
                        DDLProgramYears.SelectedValue = Session["PrintProgramYear"].ToString();
                        Session.Remove("PrintProgramYear");
                    }
                    else if (Session["ProgramYear"] != null && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                    {
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                        Session.Remove("PrintProgramYear");
                    }
                    else if (DDLProgramYears.Items.Count>=1)
                        DDLProgramYears.SelectedIndex = 0;
                }

                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;

                DropDownListDistricts.DataBind();
                if (DropDownListDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                    else if (DropDownListDistricts.Items.Count>=1)
                        DropDownListDistricts.SelectedIndex = 0;
                }

                DropDownListSchools.DataBind();
                if (Session["PrintSchoolNum"]!=null)
                {
                    if (DropDownListSchools.Items.FindByValue(Session["PrintSchoolNum"].ToString()) != null)
                        DropDownListSchools.SelectedValue = Session["PrintSchoolNum"].ToString();
                    else if (DropDownListSchools.Items.Count>=1)
                        DropDownListSchools.SelectedIndex = 0;
                }

                DDLCategories.DataBind();
                if (DDLCategories.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["PrintMenuCategory"] != null)
                {

                    if (DDLCategories.Items.FindByValue(Session["PrintMenuCategory"].ToString()) != null)
                    {
                        DDLCategories.SelectedValue = Session["PrintMenuCategory"].ToString();
                        Session.Remove("PrintMenuCategory");
                    }
                    else if (DDLCategories.Items.Count>=1)
                        DDLCategories.SelectedIndex = 0;
                }

                TextBoxDayStart.Text = "1";
                TextBoxDayEnd.Text = "5";

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

        protected void ISync_Click(object sender, EventArgs e)
        {
            UpdateReportParams();
        }



        protected void UpdateReportParams()
        {


            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramDist = new ReportParameter("DistrictID", DropDownListDistricts.SelectedValue);
            ReportParameter paramMenuCat = new ReportParameter("MenuCategory", DDLCategories.SelectedValue);
            ReportParameter paramSchool = new ReportParameter("SchoolNum", DropDownListSchools.SelectedValue);
            ValidateTextboxDay(TextBoxDayStart);
            ReportParameter paramStartDay = new ReportParameter("BeginDay", TextBoxDayStart.Text);
            ValidateTextboxDay(TextBoxDayEnd);
            ReportParameter paramEndDay = new ReportParameter("EndDay", TextBoxDayEnd.Text);
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDist, paramMenuCat, paramSchool, paramStartDay, paramEndDay });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }

            ReportViewer1.ShowPrintButton = true;
            //ReportViewer1.ShowParameterPrompts = true;

        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }

            UpdateReportParams();

        }

        protected void DDLCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DropDownListSchools_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void TextBoxDay_TextChanged(object sender, EventArgs e)
        {
            TextBox tbDay = (TextBox)sender;
            ValidateTextboxDay(tbDay);

        }

        protected void ValidateTextboxDay(TextBox tbDay)
        {
            if (tbDay==null) return;

            if (int.TryParse(tbDay.Text, out int startDay) == false)
                tbDay.Text = "1";
        }
    }
}
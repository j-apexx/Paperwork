using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintMenuAveragesOverall : Paperwork_Global
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

                //12/2/2024 - Allowing all users to access this report
                //switch (Session["AccessLevel"].ToString().ToUpper())
                //{
                //    case "ADMIN":
                //    case "OFFICE":
                //    case "RM":
                //        break;
                //    default:
                //        //Denied and not explicitly granted access
                //        if (accessExplicitlyGranted == false)
                //            Response.Redirect("~/Default.aspx");
                //        break;
                //}

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
                    else if (Session["ProgramYear"]!=null && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                    {
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                        Session.Remove("PrintProgramYear");
                    }
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }

                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                CheckBoxListDistricts.DataSource = SqlDataSourceMyDistsMstr;

                CheckBoxListDistricts.DataBind();
                if (CheckBoxListDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (CheckBoxListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        CheckBoxListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
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

        protected void BtnSelectDists_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#myModal').modal('show');", true);
        }



        protected void UpdateReportParams()
        {


            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramDist = new ReportParameter("DistrictID");
            foreach (ListItem li in CheckBoxListDistricts.Items)
            {
                if (li.Selected)
                    paramDist.Values.Add(li.Value);
            }
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDist });
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

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
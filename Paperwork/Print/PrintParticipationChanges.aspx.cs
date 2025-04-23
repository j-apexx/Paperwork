using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintParticipationChanges : Paperwork_Global
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

        protected void UpdateReportParams()
        {
            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramCEPChange = new ReportParameter("CEPChange", DDLRecordType.SelectedValue);
            ReportParameter paramDistFilter = new ReportParameter("DistrictID");
            if(DDLIncludeDists.SelectedValue=="0")//all
            {
                SqlDataReader rdr;
                SqlDataSourceDists.DataBind();
                rdr = (SqlDataReader)SqlDataSourceDists.Select(DataSourceSelectArguments.Empty);
                if (rdr != null && rdr.Read())
                {
                    paramDistFilter.Values.Add(rdr["DistrictID"].ToString());
                    while (rdr.Read())
                    {
                        paramDistFilter.Values.Add(rdr["DistrictID"].ToString());
                    }
                    rdr.Close();
                }
            }
            else//User specific
            {
                SqlDataReader rdr;
                SqlDataSourceMyRMDistricts.DataBind();
                rdr = (SqlDataReader)SqlDataSourceMyRMDistricts.Select(DataSourceSelectArguments.Empty);
                if (rdr != null && rdr.Read())
                {
                    paramDistFilter.Values.Add(rdr["DistrictID"].ToString());
                    while(rdr.Read())
                    {
                        paramDistFilter.Values.Add(rdr["DistrictID"].ToString());
                    }
                    rdr.Close();
                }
                else
                {
                   paramDistFilter.Values.Add("ZZZ");
                }
            }

            try
            {

                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramCEPChange, paramDistFilter});
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }

            ReportViewer1.ShowPrintButton = true;
            //ReportViewer1.ShowParameterPrompts = true;
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;

            UpdateReportParams();
        }

        protected void ISync_Click(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

    }
}
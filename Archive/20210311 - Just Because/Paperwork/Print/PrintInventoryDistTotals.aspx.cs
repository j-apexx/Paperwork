using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintInventoryDistTotals : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

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


            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramStartDate = new ReportParameter("StartingDate", DDLStartInventory.SelectedValue);
            ReportParameter paramEndDate = new ReportParameter("EndingDate", DDLEndInventory.SelectedValue);
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramStartDate, paramEndDate });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }
            
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
            DDLStartInventory.DataBind();
            DDLEndInventory.DataBind();

            int currentMonth = DateTime.Now.Month;
            if(DDLEndInventory.Items.Count>0)
            {
                string monthVal = "";
                if (currentMonth == 7)
                    monthVal = "*OPEN*";
                else
                    monthVal = "-" + (currentMonth - 1).ToString().PadLeft(2, '0');

                foreach(ListItem li in DDLEndInventory.Items)
                {
                    if(li.Value.IndexOf(monthVal)>0)
                    {
                        DDLEndInventory.SelectedValue = li.Value;
                        break;
                    }
                }
            }

            if(DDLStartInventory.Items.Count >= DDLStartInventory.SelectedIndex+1)
            {
                if (DDLEndInventory.SelectedIndex == 0)
                    DDLStartInventory.SelectedIndex = 0;
                else
                    DDLStartInventory.SelectedIndex = DDLEndInventory.SelectedIndex - 1;
            }

            UpdateReportParams();
        }

        protected void DDLInventory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
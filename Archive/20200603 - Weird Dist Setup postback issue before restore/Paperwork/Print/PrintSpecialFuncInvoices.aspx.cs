using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintSpecialFuncInvoices : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                DDLInvoices.DataBind();
                if (DDLInvoices.Items.Count > 0)
                {
                    if (Session["CurrentInvoice"] != null && !string.IsNullOrEmpty(Session["CurrentInvoice"].ToString()))
                    {
                        try
                        {
                            DDLInvoices.SelectedValue = Session["CurrentInvoice"].ToString();
                        }
                        catch
                        {
                            DDLInvoices.SelectedIndex = 0;
                        }
                    }
                    else
                        DDLInvoices.SelectedIndex = 0;
                }

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;

                UpdateReportParams();
            }
            else
            {
                Page.Validate();
            }
        }

        protected void UpdateReportParams()
        {
            ReportParameter paramProgYear = new ReportParameter("ProgramYear", Session["ProgramYear"].ToString());
            ReportParameter paramDistID = new ReportParameter("DistrictID", Session["CurrentDistrict"].ToString());
            ReportParameter paramInvNum = new ReportParameter("InvoiceNumber", DDLInvoices.SelectedValue);
            ReportParameter paramMiscOnly = new ReportParameter("OnlyPrintMisc", CBPrintOnlyMisc.Checked?"true":"false");
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramInvNum, paramMiscOnly });
            }
            catch { }
            ReportViewer1.ServerReport.Refresh();
            ReportViewer1.ShowPrintButton = true;
            //ReportViewer1.ShowParameterPrompts = true;
        }


        protected void DDLInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void CBPrintOnlyMisc_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
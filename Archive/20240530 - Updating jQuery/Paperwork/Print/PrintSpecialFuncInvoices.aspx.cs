using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI;

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
                    if (Session["CurrentProgramYear"] != null)
                    {
                        if (DDLProgramYears.Items.FindByValue(Session["CurrentProgramYear"].ToString()) != null)
                            DDLProgramYears.SelectedValue = Session["CurrentProgramYear"].ToString();
                        else
                            DDLProgramYears.SelectedIndex = 0;
                    }
                    else if (Session["ProgramYear"] != null)
                    {
                        if (DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                            DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                        else
                            DDLProgramYears.SelectedIndex = 0;
                    }
                    else
                        Response.Redirect("~/default.aspx");
                }

                Session.Remove("CurrentProgramYear");

                DDLInvoices.DataBind();
                if (DDLInvoices.Items.Count > 0)
                {
                    if (Session["CurrentInvoice"] != null && !string.IsNullOrEmpty(Session["CurrentInvoice"].ToString()) && Session["CurrentInvoice"].ToString().IndexOf(',')==-1)
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
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                UpdateReportParams();

            }
            else
            {
                Page.Validate();
            }
        }

        protected void UpdateReportParams()
        {
            string[] Invoices = Array.Empty<string>();
            if (DDLInvoices.SelectedValue != "-")
            {
                Array.Resize(ref Invoices, 1);
                Invoices[0] = DDLInvoices.SelectedValue;
            }
            else if(DDLInvoices.SelectedValue == "-" && Session["CurrentInvoice"] != null && !string.IsNullOrEmpty(Session["CurrentInvoice"].ToString()) && Session["CurrentInvoice"].ToString().IndexOf(',') > -1)
            {
                Invoices = Session["CurrentInvoice"].ToString().Split(',');
            }

            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramDistID = new ReportParameter("DistrictID", Session["CurrentDistrict"].ToString());
            ReportParameter paramMiscOnly = new ReportParameter("OnlyPrintMisc", CBPrintOnlyMisc.Checked ? "true" : "false");
            ReportParameter paramInvNum = new ReportParameter("InvoiceNumber");
            foreach (string Invoice in Invoices)
            {
                if(string.IsNullOrEmpty(Invoice)==false)
                {
                    paramInvNum.Values.Add(Invoice);
                }
            }


            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramInvNum, paramMiscOnly });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }


            ReportViewer1.ShowPrintButton = true;
        }

        protected void AutoPrint()
        {
            //ReportViewer1.ServerReport.Render("PDF");

            //Warning[] warnings;
            //string[] streamIds;
            //string mimeType = string.Empty;
            //string encoding = string.Empty;
            //string extension = string.Empty;
            //string deviceInfo = "<DeviceInfo>"
            //                    + "<OutputFormat>EMF</OutputFormat>"
            //                    + "<PageWidth>8.5in</PageWidth>"
            //                    + "<PageHeight>11in</PageHeight>"
            //                    + "<MarginTop>0.25in</MarginTop>"
            //                    + "<MarginLeft>0.25in</MarginLeft>"
            //                    + "<MarginRight>0.25in</MarginRight>"
            //                    + "<MarginBottom>0.25in</MarginBottom>"
            //                    + "</DeviceInfo>";
            //byte[] bytes = ReportViewer1.ServerReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamIds, out warnings);
            //Context.Response.Buffer = true;
            //Context.Response.Clear();
            //Context.Response.ContentType = mimeType;
            ////Context.Response.AddHeader("content-disposition", "attachment; filename=SpFnInvoice.pdf" );
            //Context.Response.BinaryWrite(bytes);
            //Context.Response.Flush();
            ////Context.ApplicationInstance.CompleteRequest();
            ////Context.Response.Close();
            //try
            //{
            //    Context.Response.End();
            //}
            //catch (System.Threading.ThreadAbortException err)
            //{
            //    System.Threading.Thread.ResetAbort();
            //}
            //catch (Exception err)
            //{
            //}
        }

        protected void DDLInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void CBPrintOnlyMisc_CheckedChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            DDLInvoices.DataBind();
            UpdateReportParams();
        }
    }
}
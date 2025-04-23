using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintCostPerMealInvoice : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
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
                
                if (Session["CPMInvoiceNumber"] != null && Session["CPMInvoiceNumber"].ToString() != "")
                    TextBoxInvoiceNumber.Text = Session["CPMInvoiceNumber"].ToString();
                //else
                //    TextBoxStatementDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).ToShortDateString();

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

            //prevent invalid dates from crashing the report
            //if (DateTime.TryParse(TextBoxStatementDate.Text, out DateTime testDate) == false)
            //    return;


            ReportParameter paramProgYear = new ReportParameter("ProgramYear", Session["ProgramYear"].ToString());
            ReportParameter paramInvNum = new ReportParameter("InvoiceNumber", TextBoxInvoiceNumber.Text==""? null : TextBoxInvoiceNumber.Text);
            ReportParameter paramStatementDate = new ReportParameter("StatementDate", TextBoxStatementDate.Text==""? null : TextBoxStatementDate.Text);
            ReportParameter paramDistID = new ReportParameter("DistrictID", DDLDistrict.SelectedValue);
            ReportParameter paramShowPrepay = new ReportParameter("ShowPrepay", DDLPrepay.SelectedValue);
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramInvNum, paramStatementDate, paramDistID, paramShowPrepay });
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

            //if invoice number is entered, display only that invoice, otherwise display all by date
            TextBox tb = (TextBox)sender;
            if (tb.ID == "TextBoxInvoiceNumber")
                TextBoxStatementDate.Text = "";
            else if (tb.ID == "TextBoxStatementDate")
                TextBoxInvoiceNumber.Text = "";

            tb.Focus();

            UpdateReportParams();
        }

        protected void DDLDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLPrepay_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
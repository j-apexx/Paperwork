using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Optimization;
using System.Web.UI.WebControls;

namespace Paperwork
{


    public partial class ExportRMEOM : Paperwork_Global
    {
        protected static string ExportPath = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");
            
            VerifyAccess(true);

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
                DDLProgramYears.DataBind();

                TextBoxBeginningDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(0).ToShortDateString();
                TextBoxEndingDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1)).ToShortDateString();

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;
            }



        }

        protected void BtnExportToFile_Click(object sender, EventArgs e)
        {
            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;
            TextBoxStatus.Text = "Beginning export...";
            TextBoxStatus.Rows++;
            UpdatePanelOpexWrapper.Update();


            //prevent invalid dates from crashing the report
            if (DateTime.TryParse(TextBoxBeginningDate.Text, out DateTime startDate) == false)
                return;
            if (DateTime.TryParse(TextBoxEndingDate.Text, out DateTime endDate) == false)
                return;

            if (DropDownListRMs.SelectedValue == "0")
                return;

            //ReportParameter paramProgYear = new ReportParameter("ProgramYear", Session["ProgramYear"].ToString());
            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramStartDate = new ReportParameter("StartingDate", TextBoxBeginningDate.Text);
            ReportParameter paramEndDate = new ReportParameter("EndingDate", TextBoxEndingDate.Text);
            ReportParameter paramRM = new ReportParameter("RMNumber", DropDownListRMs.SelectedValue);

            string RMName = DropDownListRMs.SelectedItem.Text;
            RMName = RMName.Replace(' ', '_');
            string MonthStr = endDate.ToString("MMM");
            string YearStr = endDate.Year.ToString();

            try
            {

                
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramStartDate, paramEndDate, paramRM });
                ReportViewer1.ServerReport.Refresh();

                if (paramRM.Values[0] != " ")
                {
                    string mimetype;
                    string encoding;
                    string fileext;
                    string[] streams;
                    Warning[] warnings;

                    //byte[] excelFile = ReportViewer1.ServerReport.Render("EXCEL");//EXCELOPENXML - should be xlsx, but causes error message about content
                    byte[] excelFile = ReportViewer1.ServerReport.Render("EXCELOPENXML", null, out mimetype, out encoding, out fileext, out streams, out warnings);
                    //HttpContext.Current.
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = mimetype;//"application/octet-stream"
                    Response.AddHeader("content-disposition", "attachment ; filename=RMMonthlyReport_"+ RMName + "_" + MonthStr + "_" + YearStr + "." + fileext + ";");
                    Response.AddHeader("content-length", excelFile.Length.ToString());

                    try
                    {
                        Response.BinaryWrite(excelFile);
                        Response.Flush();
                        Response.End();
                    }
                    catch (System.Threading.ThreadAbortException err)
                    {
                        System.Threading.Thread.ResetAbort();
                    }
                    catch (Exception err)
                    {
                    }

                    TextBoxStatus.ForeColor = System.Drawing.Color.Green;
                    TextBoxStatus.Text += "Export complete.";
                    TextBoxStatus.Rows++;
                    UpdatePanelOpexWrapper.Update();

                }
            }
            catch (ReportViewerException ex)
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                TextBoxStatus.Text += Environment.NewLine + ReportViewer1.ServerReport.ReportPath;
                TextBoxStatus.Rows++;
                UpdatePanelOpexWrapper.Update();
            }
            catch (HttpException ex)
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                UpdatePanelOpexWrapper.Update();
            }
            catch (Exception ex) 
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                UpdatePanelOpexWrapper.Update();
            }

            

        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {

            TextBox tbDate = (TextBox)sender;
            NormalizeSimplifiedDate(ref tbDate);
            this.Validate();

        }
    }
}
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web;

namespace Paperwork
{


    public partial class ExportFBPassThru : Paperwork_Global
    {
        protected static string ExportPath = "";

        protected void Page_Load(object sender, EventArgs e)
        {

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
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

                DDLProgramYears.DataBind();

                TextBoxBeginningDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1).ToShortDateString();
                TextBoxEndingDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(0).AddDays(-1)).ToShortDateString();

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

            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramStartDate = new ReportParameter("StartingDate", TextBoxBeginningDate.Text);
            ReportParameter paramEndDate = new ReportParameter("EndingDate", TextBoxEndingDate.Text);
            ReportParameter paramDistID = new ReportParameter("DistrictID");

            //paramDistID.Values[0] = null;

            string MonthStr = endDate.ToString("MMM");
            string YearStr = endDate.Year.ToString();

            try
            {

                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramStartDate, paramEndDate, paramDistID });
                ReportViewer1.ServerReport.Refresh();

                string mimetype;
                string encoding;
                string fileext;
                string[] streams;
                Warning[] warnings;

                //byte[] excelFile = ReportViewer1.ServerReport.Render("EXCELOPENXML");
                byte[] excelFile = ReportViewer1.ServerReport.Render("EXCELOPENXML", null, out mimetype, out encoding, out fileext, out streams, out warnings);

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimetype;//"application/octet-stream"
                Response.AddHeader("content-disposition", "attachment ; filename=FBPassThru_" + MonthStr + "_" + YearStr + "." + fileext + ";");
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



    }
}
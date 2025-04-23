using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

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
                string SSRSBaseFolder = GetGlobalSettingString("SSRS_BaseFolder");
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
                    byte[] excelFile = ReportViewer1.ServerReport.Render("EXCELOPENXML");

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=RMMonthlyReport_"+ RMName + "_" + MonthStr + "_" + YearStr + ".xlsx;");
                    HttpContext.Current.Response.AddHeader("Content-Length", excelFile.Length.ToString());
                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    HttpContext.Current.Response.BinaryWrite(excelFile);
                    try
                    {
                        HttpContext.Current.Response.End();
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

    }
}
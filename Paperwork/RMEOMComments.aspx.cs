using Microsoft.Exchange.WebServices.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class RMEOMComments : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

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

            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;

            if (!IsPostBack)
            {
                VerifyAccess(true);
                if (Session["AccessLevel"] == null)
                    Response.Redirect("~/Default.aspx");

                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

                //Set the programyear
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

                DDLProgramYears_SelectedIndexChanged(null, null);

                TextBoxFYearmonth.Text = DateTime.Now.AddMonths(-1).Year.ToString() + "-" + DateTime.Now.AddMonths(-1).Month.ToString().PadLeft(2, '0');

                TextBoxFYearmonth_TextChanged(TextBoxFYearmonth, EventArgs.Empty);

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                HFSSRSBaseFolder.DataBind();
                HFSSRSBaseFolder.Value = SSRSBaseFolder;
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                DropDownListDistricts.DataBind();

                UpdateReportParams();

            }
            else
            {
                VerifyAccess(false);
            }

        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.Parameters["@ProgramYear"].Value = DDLProgramYears.SelectedValue;
        }

        protected void TextBoxFYearmonth_TextChanged(object sender, EventArgs e)
        {
            int Year = 0;
            int Month = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            TextBox TextBoxFYearmonth = (TextBox)sender;

            try
            {
                startDate = DateTime.Parse(HFStartDate.Value);
                endDate = DateTime.Parse(HFEndDate.Value);
            }
            catch (Exception ex)//session values not set?
            {
                Response.Redirect("~/default.aspx");
            }


            //If we cannot parse the year and month - set to current year month
            if (TextBoxFYearmonth.Text.Length < 7 || int.TryParse(TextBoxFYearmonth.Text.Substring(0, 4), out Year) == false || int.TryParse(TextBoxFYearmonth.Text.Substring(5, 2), out Month) == false)
            {
                TextBoxFYearmonth.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
                Year = DateTime.Now.Year;
                Month = DateTime.Now.Month;
            }

            DateTime testDate = new DateTime(Year, Month, 1);
            //if date falls before the beginning or after the end of the program year - set to limits
            if (testDate < startDate)
            {
                TextBoxFYearmonth.Text = startDate.Year.ToString() + "-" + startDate.Month.ToString().PadLeft(2, '0');
                Year = startDate.Year;
                Month = startDate.Month;
            }
            else if (testDate > endDate)
            {
                TextBoxFYearmonth.Text = endDate.Year.ToString() + "-" + endDate.Month.ToString().PadLeft(2, '0');
                Year = endDate.Year;
                Month = endDate.Month;
            }

            Session["CurrentYearMonth"] = TextBoxFYearmonth.Text;
            //FormViewRMEOMComments.DataBind();
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
                this.Master.MstrHdrUpdatePanel.Update();

                //Response.Redirect(Request.RawUrl);//Request.RawUrl provides the current page URL
                UpdateReportParams();
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            //Update the program year start and end date hidden fields. They are used to constrain the date lookup.
            SqlDataSourceProgramYears.DataBind();

            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int recordIndex = dvSql.Find(DDLProgramYears.SelectedValue);
                if (recordIndex != -1)
                {
                    DataRowView drvSql = dvSql[recordIndex];
                    HFStartDate.Value = drvSql["StartDate"].ToString();
                    HFEndDate.Value = drvSql["EndDate"].ToString();

                }
                else
                {
                    HFStartDate.Value = "";
                    HFEndDate.Value = "";
                }
            }
            else
            {
                //if no program years could be populated - redirect home
                Response.Redirect("~/default.aspx");
            }



            //When program year changes, districts could change as well
            //Dists are now based on ProgramYear and must be rebound
            DropDownListDistricts.DataBind();
            if (DropDownListDistricts.Items.Count == 0)
                Response.Redirect("~/default.aspx");
            if (Session["CurrentDistrict"] != null)
            {
                if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                    DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                else
                    DropDownListDistricts_SelectedIndexChanged(null, null);
            }

        }

        protected void SaveCommentslist()
        {
            HiddenField HFCommentsSaved = (HiddenField)FindControlRecursive(FormViewRMEOMComments, "HFCommentsSaved");
            if (HFCommentsSaved == null || HFCommentsSaved.Value == "1")
            {
                return;
            }

            //KEY VALUES
            String KeyPYear = FormViewRMEOMComments.DataKey.Values["ProgramYear"].ToString();
            String KeyDistID = FormViewRMEOMComments.DataKey.Values["DistrictID"].ToString();
            String YearMonth = FormViewRMEOMComments.DataKey.Values["YearMonth"].ToString();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryChecklistExists = "SELECT COUNT(*) FROM [RMEOMComments] " +
                    " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth ";

            using (SqlCommand cmd = new SqlCommand(queryChecklistExists, dbConnection))
            {
                #region KeyVals

                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = KeyPYear;
                dbParam_ProgYear.DbType = DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
                dbParam_DistID.DbType = DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_YearMo = new SqlParameter();
                dbParam_YearMo.ParameterName = "@YearMonth";
                dbParam_YearMo.Value = YearMonth;
                dbParam_YearMo.DbType = DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_YearMo);

                #endregion

                int existingRecordCount = 0;

                try
                {
                    dbConnection.Open();
                    Object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out existingRecordCount);
                    }

                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "A database error occurred while checking to see if record existed.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Unknown Error", MessageType.Error, "An error occurred while checking to see if record existed.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

                string InsertUpdateQuery = "";

                if (existingRecordCount == 0)
                {
                    InsertUpdateQuery = "INSERT INTO [RMEOMComments] (ProgramYear, DistrictID, YearMonth,[CommentsParticipation],[CommentsFoodCost],[CommentsLabor],[CommentsClientPL],[CommentsNISide],[CommentsActionPlan],[NI_SideYTD_Budget],[NI_SideYTD_Actual] ) " +
                        " VALUES (@ProgramYear, @DistrictID, @YearMonth, @CommentsParticipation,@CommentsFoodCost,@CommentsLabor,@CommentsClientPL,@CommentsNISide,@CommentsActionPlan,@NI_SideYTD_Budget,@NI_SideYTD_Actual)";
                }
                else
                {
                    InsertUpdateQuery = "UPDATE [RMEOMComments] SET CommentsParticipation=@CommentsParticipation, CommentsFoodCost=@CommentsFoodCost, CommentsLabor=@CommentsLabor " +
                        ", CommentsClientPL=@CommentsClientPL, CommentsNISide=@CommentsNISide, CommentsActionPlan=@CommentsActionPlan, [NI_SideYTD_Budget]=@NI_SideYTD_Budget, [NI_SideYTD_Actual]=@NI_SideYTD_Actual " +
                        " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth ";
                }

                #region FieldValues
                TextBox TextBoxParticipations = (TextBox)FindControlRecursive(FormViewRMEOMComments, "TextBoxParticipations");
                TextBox TextBoxFoodCost = (TextBox)FindControlRecursive(FormViewRMEOMComments, "TextBoxFoodCost");
                TextBox TextBoxLabor = (TextBox)FindControlRecursive(FormViewRMEOMComments, "TextBoxLabor");
                TextBox TextBoxClientPL = (TextBox)FindControlRecursive(FormViewRMEOMComments, "TextBoxClientPL");
                TextBox TextBoxNISide = (TextBox)FindControlRecursive(FormViewRMEOMComments, "TextBoxNISide");
                TextBox TextBoxActionPlan = (TextBox)FindControlRecursive(FormViewRMEOMComments, "TextBoxActionPlan");
                TextBox TextBoxNISideB = (TextBox)FindControlRecursive(FormViewRMEOMComments, "TextBoxNISideB");
                TextBox TextBoxNISideA = (TextBox)FindControlRecursive(FormViewRMEOMComments, "TextBoxNISideA");
                #endregion


                SqlConnection dbConnection2 = new SqlConnection(connectionString);
                using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection2))
                {
                    #region KeyVals
                    SqlParameter dbParam_ProgYear2 = new SqlParameter();
                    dbParam_ProgYear2.ParameterName = "@ProgramYear";
                    dbParam_ProgYear2.Value = KeyPYear;
                    dbParam_ProgYear2.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_ProgYear2);

                    SqlParameter dbParam_DistID2 = new SqlParameter();
                    dbParam_DistID2.ParameterName = "@DistrictID";
                    dbParam_DistID2.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
                    dbParam_DistID2.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_DistID2);

                    SqlParameter dbParam_YearMo2 = new SqlParameter();
                    dbParam_YearMo2.ParameterName = "@YearMonth";
                    dbParam_YearMo2.Value = YearMonth;
                    dbParam_YearMo2.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_YearMo2);
                    #endregion
                    #region Comments
                    SqlParameter dbParam_partic = new SqlParameter();
                    dbParam_partic.ParameterName = "@CommentsParticipation";
                    dbParam_partic.Value = TextBoxParticipations.Text;
                    dbParam_partic.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_partic);

                    SqlParameter dbParam_foodCost = new SqlParameter();
                    dbParam_foodCost.ParameterName = "@CommentsFoodCost";
                    dbParam_foodCost.Value = TextBoxFoodCost.Text;
                    dbParam_foodCost.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_foodCost);

                    SqlParameter dbParam_labor = new SqlParameter();
                    dbParam_labor.ParameterName = "@CommentsLabor";
                    dbParam_labor.Value = TextBoxLabor.Text;
                    dbParam_labor.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_labor);

                    SqlParameter dbParam_clientPL = new SqlParameter();
                    dbParam_clientPL.ParameterName = "@CommentsClientPL";
                    dbParam_clientPL.Value = TextBoxClientPL.Text;
                    dbParam_clientPL.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_clientPL);

                    SqlParameter dbParam_niSide = new SqlParameter();
                    dbParam_niSide.ParameterName = "@CommentsNISide";
                    dbParam_niSide.Value = TextBoxNISide.Text;
                    dbParam_niSide.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_niSide);

                    SqlParameter dbParam_actionPlan = new SqlParameter();
                    dbParam_actionPlan.ParameterName = "@CommentsActionPlan";
                    dbParam_actionPlan.Value = TextBoxActionPlan.Text;
                    dbParam_actionPlan.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_actionPlan);

                    Decimal.TryParse(TextBoxNISideB.Text, out decimal decNISideB);
                    TextBoxNISideB.Text = decNISideB.ToString("F2");
                    SqlParameter dbParam_NISideB = new SqlParameter();
                    dbParam_NISideB.ParameterName = "@NI_SideYTD_Budget";
                    dbParam_NISideB.Value = decNISideB;
                    dbParam_NISideB.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_NISideB);

                    Decimal.TryParse(TextBoxNISideA.Text, out decimal decNISideA);
                    TextBoxNISideA.Text = decNISideA.ToString("F2");
                    SqlParameter dbParam_NISideA = new SqlParameter();
                    dbParam_NISideA.ParameterName = "@NI_SideYTD_Actual";
                    dbParam_NISideA.Value = decNISideA;
                    dbParam_NISideA.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_NISideA);
                    #endregion

                    try
                    {
                        dbConnection2.Open();
                        int recordcount = 0;
                        recordcount = cmd2.ExecuteNonQuery();

                        if (recordcount > 0)
                        {
                            ShowMessageIcon("Saved", MessageType.Success);

                        }
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "An error occurred during comments save.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "An error occurred during comments save.", ex.Message);
                    }
                    finally
                    {
                        dbConnection2.Close();
                    }
                }

            }

            HFCommentsSaved.Value = "1";
        }

        protected void ChecklistItemChanged(object sender, EventArgs e)
        {
            SaveCommentslist();
        }

        protected void FormViewRMEOMComments_PreRender(object sender, EventArgs e)
        {
            //FormViewRMEOMComments.DataBind();
            HiddenField HFCommentsSaved = (HiddenField)FindControlRecursive(FormViewRMEOMComments, "HFCommentsSaved");

            if (HFCommentsSaved != null)
                HFCommentsSaved.Value = "0";
        }

        protected void FormViewRMEOMComments_DataBound(object sender, EventArgs e)
        {
            string Text = TextBoxFYearmonth.Text;
        }

        protected void UpdateReportParams()
        {
            if (int.TryParse(TextBoxFYearmonth.Text.Substring(0, 4), out int DateYear) == false)
                return;
            if (int.TryParse(TextBoxFYearmonth.Text.Substring(5, 2), out int DateMonth) == false)
                return;
            DateTime startDate = new DateTime(DateYear, DateMonth,1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramRM = new ReportParameter("RMNumber","0");
            ReportParameter paramStartDate = new ReportParameter("StartingDate", startDate.ToShortDateString());
            ReportParameter paramEndDate = new ReportParameter("EndingDate", endDate.ToShortDateString());
            ReportParameter paramDist = new ReportParameter("DistrictID", DropDownListDistricts.SelectedValue);
            ReportParameter paramHideComments = new ReportParameter("HideComments", "false");
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramRM, paramStartDate, paramEndDate,paramDist,paramHideComments });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }

            ReportViewer1.ShowPrintButton = true;
            //ReportViewer1.ShowParameterPrompts = true;
        }

        protected void iSave_Click(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
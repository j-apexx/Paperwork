using Microsoft.Exchange.WebServices.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class ReimbVerification : Paperwork_Global
    {
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



            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;

            if (!IsPostBack)
            {

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



                TextBoxYearmonth.Text = DateTime.Now.AddMonths(-1).Year.ToString() + "-" + DateTime.Now.AddMonths(-1).Month.ToString().PadLeft(2, '0');
                TextBoxYearmonth_TextChanged(TextBoxYearmonth, EventArgs.Empty);



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

        protected void TextBoxYearmonth_TextChanged(object sender, EventArgs e)
        {
            int Year = 0;
            int Month = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            TextBox TextBoxYearmonth = (TextBox)sender;

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
            if (TextBoxYearmonth.Text.Length < 7 || int.TryParse(TextBoxYearmonth.Text.Substring(0, 4), out Year) == false || int.TryParse(TextBoxYearmonth.Text.Substring(5, 2), out Month) == false)
            {
                TextBoxYearmonth.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
                Year = DateTime.Now.Year;
                Month = DateTime.Now.Month;
            }

            DateTime testDate = new DateTime(Year, Month, 1);
            //if date falls before the beginning or after the end of the program year - set to limits
            if (testDate < startDate)
            {
                TextBoxYearmonth.Text = startDate.Year.ToString() + "-" + startDate.Month.ToString().PadLeft(2, '0');
                Year = startDate.Year;
                Month = startDate.Month;
            }
            else if (testDate > endDate)
            {
                TextBoxYearmonth.Text = endDate.Year.ToString() + "-" + endDate.Month.ToString().PadLeft(2, '0');
                Year = endDate.Year;
                Month = endDate.Month;
            }

            //Update the Current Month hidden fields used for Recap Totals
            if (int.TryParse(TextBoxYearmonth.Text.Substring(0, 4), out Year) == true && int.TryParse(TextBoxYearmonth.Text.Substring(5, 2), out Month) == true)
            {
                DateTime MonthStartDate = new DateTime(Year, Month, 1);
                HFMonthStart.Value = MonthStartDate.ToShortDateString();
                HFMonthEnd.Value = MonthStartDate.AddMonths(1).AddDays(-1).ToShortDateString();
            }


            Session["CurrentYearMonth"] = TextBoxYearmonth.Text;

        }

        /// <summary>
        /// Special Handling for the Post Date YearMonth
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TextBoxPostToYearMonth_TextChanged(object sender, EventArgs e)
        {
            int Year = 0;
            int Month = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            TextBox TextBoxPostToYearMonth = (TextBox)sender;

            try
            {
                //Post date needs to be equal to or after current month
                startDate = DateTime.Parse(HFMonthStart.Value);
                endDate = DateTime.Parse(HFEndDate.Value);
            }
            catch (Exception ex)//session values not set?
            {
                Response.Redirect("~/default.aspx");
            }


            //If we cannot parse the year and month - set to current year month
            if (TextBoxPostToYearMonth.Text.Length < 7 || int.TryParse(TextBoxPostToYearMonth.Text.Substring(0, 4), out Year) == false || int.TryParse(TextBoxPostToYearMonth.Text.Substring(5, 2), out Month) == false)
            {
                TextBoxPostToYearMonth.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
                Year = DateTime.Now.Year;
                Month = DateTime.Now.Month;
            }

            DateTime testDate = new DateTime(Year, Month, 1);
            //if date falls before the beginning or after the end of the program year - set to limits
            if (testDate < startDate)
            {
                TextBoxPostToYearMonth.Text = startDate.Year.ToString() + "-" + startDate.Month.ToString().PadLeft(2, '0');
                Year = startDate.Year;
                Month = startDate.Month;
            }
            else if (testDate > endDate)
            {
                TextBoxPostToYearMonth.Text = endDate.Year.ToString() + "-" + endDate.Month.ToString().PadLeft(2, '0');
                Year = endDate.Year;
                Month = endDate.Month;
            }
            SaveReimbursement();
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
                    HFBudgetYear.Value = drvSql["BudgetYear"].ToString();

                }
                else
                {
                    HFStartDate.Value = "";
                    HFEndDate.Value = "";
                    HFBudgetYear.Value = "";
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

        protected void TextBoxReimbursements_TextChanged(object sender, EventArgs e)
        {
            SaveReimbursement();
        }

        protected void SaveReimbursement()
        {
            HiddenField HFReimbursementSaved = (HiddenField)FindControlRecursive(FormViewReimbVerification, "HFReimbursementSaved");
            if (HFReimbursementSaved == null || HFReimbursementSaved.Value == "1")
            {
                return;
            }

            //KEY VALUES
            String KeyPYear = FormViewReimbVerification.DataKey.Values["ProgramYear"].ToString();
            String KeyDistID = FormViewReimbVerification.DataKey.Values["DistrictID"].ToString();
            String YearMonth = FormViewReimbVerification.DataKey.Values["YearMonth"].ToString();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryChecklistExists = "SELECT COUNT(*) FROM [ReimbursementVerification] " +
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
                    InsertUpdateQuery = "INSERT INTO [ReimbursementVerification] ([ProgramYear],[DistrictID],[YearMonth],[ActualFederalReimb],[ActualStateReimb]," +
                        "[ActualCACFPReimb],[ReimbursementAdjustment],[PostToYearMonth],[ReimbAdjustComments]) " +
                        "VALUES (@ProgramYear,@DistrictID,@YearMonth,@ActualFederalReimb,@ActualStateReimb," +
                        "@ActualCACFPReimb,@ReimbursementAdjustment,@PostToYearMonth,@ReimbAdjustComments)";
                }
                else
                {
                    InsertUpdateQuery = "UPDATE [ReimbursementVerification] SET [ActualFederalReimb]=@ActualFederalReimb, [ActualStateReimb]=@ActualStateReimb," +
                        "[ActualCACFPReimb]=@ActualCACFPReimb, [ReimbursementAdjustment]=@ReimbursementAdjustment, [PostToYearMonth]=@PostToYearMonth, [ReimbAdjustComments]=@ReimbAdjustComments " +
                        "WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [YearMonth]=@YearMonth";
                }


                #region FieldValues
                TextBox TextBoxReimbFederal = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxReimbFederal");
                TextBox TextBoxReimbState = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxReimbState");
                TextBox TextBoxReimbCACFP = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxReimbCACFP");
                TextBox TextBoxReimbAdjustment = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxReimbAdjustment");
                TextBox TextBoxPostToYearMonth = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxPostToYearMonth");
                TextBox TextBoxComments = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxComments");
                Label LabelActualReimbursements = (Label)FindControlRecursive(FormViewReimbVerification, "LabelActualReimbursements");
                Label LabelPaperworkReimbursements = (Label)FindControlRecursive(FormViewReimbVerification, "LabelPaperworkReimbursements");
                Label LabelDifference = (Label)FindControlRecursive(FormViewReimbVerification, "LabelDifference");
                #endregion

                if (TextBoxReimbFederal == null || TextBoxReimbState == null || TextBoxReimbCACFP == null || TextBoxReimbAdjustment == null || TextBoxPostToYearMonth == null || TextBoxComments==null ||
                    LabelActualReimbursements==null || LabelPaperworkReimbursements==null || LabelDifference==null)
                    return;

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
                    #region FieldVals
                    SqlParameter dbParam_FedReimb = new SqlParameter();
                    dbParam_FedReimb.ParameterName = "@ActualFederalReimb";
                    Decimal.TryParse(TextBoxReimbFederal.Text, out Decimal decReimbFed);
                    dbParam_FedReimb.Value = decReimbFed;
                    dbParam_FedReimb.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_FedReimb);

                    SqlParameter dbParam_StateReimb = new SqlParameter();
                    dbParam_StateReimb.ParameterName = "@ActualStateReimb";
                    Decimal.TryParse(TextBoxReimbState.Text, out Decimal decReimbState);
                    dbParam_StateReimb.Value = decReimbState;
                    dbParam_StateReimb.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_StateReimb);

                    SqlParameter dbParam_CACFPReimb = new SqlParameter();
                    dbParam_CACFPReimb.ParameterName = "@ActualCACFPReimb";
                    Decimal.TryParse(TextBoxReimbCACFP.Text, out Decimal decReimbCACFP);
                    dbParam_CACFPReimb.Value = decReimbCACFP;
                    dbParam_CACFPReimb.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CACFPReimb);

                    SqlParameter dbParam_ReimbAdj = new SqlParameter();
                    dbParam_ReimbAdj.ParameterName = "@ReimbursementAdjustment";
                    Decimal.TryParse(TextBoxReimbAdjustment.Text, out Decimal decReimbAdj);
                    dbParam_ReimbAdj.Value = Decimal.Round( decReimbAdj, 2, MidpointRounding.AwayFromZero);
                    dbParam_ReimbAdj.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ReimbAdj);

                    //Validate the posting date
                    bool invalidPostDate = false;
                    string[] year_monthArray = TextBoxYearmonth.Text.Split('-');
                    if (year_monthArray.Length != 2)
                        invalidPostDate = true;
                    string[] year_monthPostArray = TextBoxPostToYearMonth.Text.Split('-');
                    if (year_monthArray.Length != 2)
                    {
                        invalidPostDate = true;
                        TextBoxPostToYearMonth_TextChanged(TextBoxPostToYearMonth, null);
                    }
                    if (invalidPostDate)
                    {
                        ShowDialog("Error", MessageType.Error, "Invalid Post To period.", "");
                        return;
                    }
                    

                        SqlParameter dbParam_YearMoPost = new SqlParameter();
                    dbParam_YearMoPost.ParameterName = "@PostToYearMonth";
                    dbParam_YearMoPost.Value = TextBoxPostToYearMonth.Text;
                    dbParam_YearMoPost.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_YearMoPost);

                    SqlParameter dbParam_comments = new SqlParameter();
                    dbParam_comments.ParameterName = "@ReimbAdjustComments";
                    dbParam_comments.Value = TextBoxComments.Text;
                    dbParam_comments.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_comments);
                    #endregion

                    try
                    {
                        dbConnection2.Open();
                        int recordcount = 0;
                        recordcount = cmd2.ExecuteNonQuery();

                        if (recordcount > 0)
                        {
                            ShowMessageIcon("Saved", MessageType.Success);

                            Decimal decTotalActual = (decReimbFed + decReimbState + decReimbCACFP);
                            LabelActualReimbursements.Text = decTotalActual.ToString("F2");

                            Decimal.TryParse(LabelPaperworkReimbursements.Text, out Decimal decReimbPaperwork);
                            LabelDifference.Text = (decTotalActual - decReimbPaperwork).ToString("F2");

                        }
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "An error occurred during reimbursement save.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "An error occurred during reimbursement save.", ex.Message);
                    }
                    finally
                    {
                        dbConnection2.Close();
                    }

                }


            }

            HFReimbursementSaved.Value = "1";

        }

        protected void FormViewReimbVerification_PreRender(object sender, EventArgs e)
        {
            HiddenField HFReimbursementSaved = (HiddenField)FindControlRecursive(FormViewReimbVerification, "HFReimbursementSaved");

            if (HFReimbursementSaved != null)
                HFReimbursementSaved.Value = "0";
        }

        protected void FormViewReimbVerification_DataBound(object sender, EventArgs e)
        {
            if (int.TryParse(TextBoxYearmonth.Text.Substring(0, 4), out int Year) == true && int.TryParse(TextBoxYearmonth.Text.Substring(5, 2), out int Month) == true)
            {
                DateTime MonthStartDate = new DateTime(Year, Month, 1);
                TextBox TextBoxReimbFederal = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxReimbFederal");
                TextBox TextBoxReimbState = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxReimbState");
                TextBox TextBoxReimbCACFP = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxReimbCACFP");
                TextBox TextBoxReimbAdjustment = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxReimbAdjustment");
                TextBox TextBoxPostToYearMonth = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxPostToYearMonth");
                TextBox TextBoxComments = (TextBox)FindControlRecursive(FormViewReimbVerification, "TextBoxComments");
                if (TextBoxReimbFederal != null && TextBoxReimbState != null && TextBoxReimbCACFP != null && TextBoxReimbAdjustment != null && TextBoxPostToYearMonth != null && TextBoxComments != null)
                {


                    if (IsDistrictEnforcingPeriodClosures(DropDownListDistricts.SelectedValue) && IsDateOpenForEntry(MonthStartDate, DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, "ReimbVerification") == false)
                    {
                        TextBoxReimbFederal.Enabled = false;
                        TextBoxReimbState.Enabled = false;
                        TextBoxReimbCACFP.Enabled = false;
                        TextBoxReimbAdjustment.Enabled = false;
                        TextBoxPostToYearMonth.Enabled = false;
                        TextBoxComments.Enabled = false;
                    }
                    else
                    {
                        TextBoxReimbFederal.Enabled = true;
                        TextBoxReimbState.Enabled = true;
                        TextBoxReimbCACFP.Enabled = true;
                        TextBoxReimbAdjustment.Enabled = true;
                        TextBoxPostToYearMonth.Enabled = true;
                        TextBoxComments.Enabled = true;
                    }
                }


            }







            Label LabelPaperworkReimbursements = (Label)FindControlRecursive(FormViewReimbVerification, "LabelPaperworkReimbursements");
            Label LabelActualReimbursements = (Label)FindControlRecursive(FormViewReimbVerification, "LabelActualReimbursements");
            Label LabelDifference = (Label)FindControlRecursive(FormViewReimbVerification, "LabelDifference");
            if (LabelPaperworkReimbursements == null || LabelActualReimbursements==null || LabelDifference==null)
                return;

            SqlDataSourceRecapTotals.DataBind();
            SqlDataReader rdr = (SqlDataReader)SqlDataSourceRecapTotals.Select(DataSourceSelectArguments.Empty);
            if (rdr != null && rdr.Read())
            {
                Decimal.TryParse(rdr["TotalReim"].ToString(), out Decimal decReim);
                LabelPaperworkReimbursements.Text = ((Decimal)rdr["TotalReim"]).ToString("F2");

                Decimal.TryParse(LabelActualReimbursements.Text, out Decimal decActualReim);

                LabelDifference.Text = (decActualReim - decReim).ToString("F2");

            }

        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            //Just to ensure the seesion is set
            Session["CurrentYearMonth"] = TextBoxYearmonth.Text;
            Response.Redirect("~/Print/PrintReimbVerification.aspx");
        }
    }
}
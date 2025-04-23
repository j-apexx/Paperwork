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
    public partial class EOMChecklist : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

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

                if (Session["CurrentYearMonth"] != null && !string.IsNullOrEmpty(Session["CurrentYearMonth"].ToString()))
                    TextBoxFYearmonth.Text = Session["CurrentYearMonth"].ToString();
                else
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

        protected void SaveChecklist()
        {
            HiddenField HFChecklistSaved = (HiddenField)FindControlRecursive(FormViewEOMChecklist, "HFChecklistSaved");
            if (HFChecklistSaved == null || HFChecklistSaved.Value == "1")
            {
                return;
            }

            //KEY VALUES
            String KeyPYear = FormViewEOMChecklist.DataKey.Values["ProgramYear"].ToString();
            String KeyDistID = FormViewEOMChecklist.DataKey.Values["DistrictID"].ToString();
            String YearMonth = FormViewEOMChecklist.DataKey.Values["YearMonth"].ToString();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryChecklistExists = "SELECT COUNT(*) FROM [EOMCheckList] " +
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
                    InsertUpdateQuery = "INSERT INTO [EOMCheckList] (ProgramYear, DistrictID, YearMonth, InventoryDate, NOIAmount, CasesNA, CasesBB, CasesDOD, CasesK12, CasesPL, CBPurchTotalEntered" +
                        ", CBUSDAComplete, PurchTotal, CBExtInventoryPrinted, CBSpecFuncLedger, CBIndirectCosts, CBReviewedTransOCosts, POSBalance, CBSOC1, CBOnlinePayments" +
                        ", CBOpDaysVerified, CBSchoolInfoUpdated, CBDistPayrollHrsEntered, CBHealthInsCaptured, CBReviewBdgtReport, CBNegAdjusts, NegAdjustsComment, CBVendDepOSales" +
                        ", CBEditChecksMatch, FoodCostComment, PaperCleanComment, CBUSDAInvMatches, ReviewedUsageComment, TargetPurchasedComment" +
                        ", HoursWagesComment, BottomLineComment, LastEditedUser, LastEditedDate, CBSFLedgerYTDMatchesOR, ValueNA, ValueBB, ValueDOD, ValueK12, ValuePL, TRACSBldgTotals" +
                        ", CBNotificationsComplete, CasesNAYTD, CasesBBYTD, CasesDODYTD, CasesK12YTD, CasesPLYTD, ValueNAYTD, ValueBBYTD, ValueDODYTD, ValueK12YTD, ValuePLYTD, USDADiffComment" +
                        ", POSOverUnderCurrent, POSOverUnderYTD, CBCompareTracsStatement, CBCompletedClaimRec, ParticipationComments)" +
                        " VALUES (@ProgramYear, @DistrictID, @YearMonth, @InventoryDate, @NOIAmount, @CasesNA, @CasesBB, @CasesDOD, @CasesK12, @CasesPL, @CBPurchTotalEntered" +
                        ", @CBUSDAComplete, @PurchTotal, @CBExtInventoryPrinted, @CBSpecFuncLedger, @CBIndirectCosts, @CBReviewedTransOCosts, @POSBalance, @CBSOC1, @CBOnlinePayments" +
                        ", @CBOpDaysVerified, @CBSchoolInfoUpdated, @CBDistPayrollHrsEntered, @CBHealthInsCaptured, @CBReviewBdgtReport, @CBNegAdjusts, @NegAdjustsComment, @CBVendDepOSales" +
                        ", @CBEditChecksMatch, @FoodCostComment, @PaperCleanComment, @CBUSDAInvMatches, @ReviewedUsageComment, @TargetPurchasedComment" +
                        ", @HoursWagesComment, @BottomLineComment, @LastEditedUser, GETDATE(), @CBSFLedgerYTDMatchesOR, @ValueNA, @ValueBB, @ValueDOD, @ValueK12, @ValuePL, @TRACSBldgTotals" +
                        ", @CBNotificationsComplete, @CasesNAYTD, @CasesBBYTD, @CasesDODYTD, @CasesK12YTD, @CasesPLYTD, @ValueNAYTD, @ValueBBYTD, @ValueDODYTD, @ValueK12YTD, @ValuePLYTD, @USDADiffComment" +
                        ", @POSOverUnderCurrent, @POSOverUnderYTD, @CBCompareTracsStatement, @CBCompletedClaimRec, @ParticipationComments)";
                }
                else
                {
                    InsertUpdateQuery = "UPDATE [EOMCheckList] SET InventoryDate=@InventoryDate, NOIAmount=@NOIAmount, CasesNA=@CasesNA, CasesBB=@CasesBB, CasesDOD=@CasesDOD, CasesK12=@CasesK12, CasesPL=@CasesPL" +
                        ", CBPurchTotalEntered=@CBPurchTotalEntered, CBUSDAComplete=@CBUSDAComplete, PurchTotal=@PurchTotal, CBExtInventoryPrinted=@CBExtInventoryPrinted, CBSpecFuncLedger=@CBSpecFuncLedger" +
                        ", CBIndirectCosts=@CBIndirectCosts, CBReviewedTransOCosts=@CBReviewedTransOCosts, POSBalance=@POSBalance, CBSOC1=@CBSOC1, CBOnlinePayments=@CBOnlinePayments" +
                        ", CBReviewBdgtReport=@CBReviewBdgtReport, CBNegAdjusts=@CBNegAdjusts, NegAdjustsComment=@NegAdjustsComment, CBVendDepOSales=@CBVendDepOSales, CBEditChecksMatch=@CBEditChecksMatch" +
                        ", CBOpDaysVerified=@CBOpDaysVerified, CBSchoolInfoUpdated=@CBSchoolInfoUpdated, CBDistPayrollHrsEntered=@CBDistPayrollHrsEntered, CBHealthInsCaptured=@CBHealthInsCaptured" +
                        ", FoodCostComment=@FoodCostComment, PaperCleanComment=@PaperCleanComment, CBUSDAInvMatches=@CBUSDAInvMatches" +
                        ", ReviewedUsageComment=@ReviewedUsageComment, TargetPurchasedComment=@TargetPurchasedComment, HoursWagesComment=@HoursWagesComment, BottomLineComment=@BottomLineComment" +
                        ", LastEditedUser=@LastEditedUser, LastEditedDate=GETDATE(), CBSFLedgerYTDMatchesOR=@CBSFLedgerYTDMatchesOR" +
                        ", ValueNA=@ValueNA, ValueBB=@ValueBB, ValueDOD=@ValueDOD, ValueK12=@ValueK12, ValuePL=@ValuePL, TRACSBldgTotals=@TRACSBldgTotals" +
                        ", CBNotificationsComplete=@CBNotificationsComplete, CasesNAYTD=@CasesNAYTD, CasesBBYTD=@CasesBBYTD, CasesDODYTD=@CasesDODYTD, CasesK12YTD=@CasesK12YTD, CasesPLYTD=@CasesPLYTD" +
                        ", ValueNAYTD=@ValueNAYTD, ValueBBYTD=@ValueBBYTD, ValueDODYTD=@ValueDODYTD, ValueK12YTD=@ValueK12YTD, ValuePLYTD=@ValuePLYTD, USDADiffComment=@USDADiffComment " +
                        ", POSOverUnderCurrent=@POSOverUnderCurrent, POSOverUnderYTD=@POSOverUnderYTD, CBCompareTracsStatement=@CBCompareTracsStatement, CBCompletedClaimRec=@CBCompletedClaimRec " +
                        ", ParticipationComments=@ParticipationComments " +
                        " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth ";
                }

                #region FieldValues
                TextBox TextBoxDateOfInventory = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxDateOfInventory");
                TextBox TextBoxCasesNA = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesNA");
                TextBox TextBoxCasesK12 = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesK12");
                TextBox TextBoxCasesDOD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesDOD");
                TextBox TextBoxCasesPL = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesPL");
                TextBox TextBoxCasesBB = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesBB");
                TextBox TextBoxValueNA = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValueNA");
                TextBox TextBoxValueK12 = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValueK12");
                TextBox TextBoxValueDOD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValueDOD");
                TextBox TextBoxValuePL = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValuePL");
                TextBox TextBoxValueBB = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValueBB");

                TextBox TextBoxCasesNAYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesNAYTD");
                TextBox TextBoxCasesK12YTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesK12YTD");
                TextBox TextBoxCasesDODYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesDODYTD");
                TextBox TextBoxCasesPLYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesPLYTD");
                TextBox TextBoxCasesBBYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxCasesBBYTD");
                TextBox TextBoxValueNAYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValueNAYTD");
                TextBox TextBoxValueK12YTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValueK12YTD");
                TextBox TextBoxValueDODYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValueDODYTD");
                TextBox TextBoxValuePLYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValuePLYTD");
                TextBox TextBoxValueBBYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxValueBBYTD");


                TextBox TextBoxUSDAComment = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAComment");
                TextBox TextBoxNOIAmount = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxNOIAmount");
                CheckBox CBPurchTotalEntered = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBPurchTotalEntered");
                CheckBox CBUSDAComplete = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBUSDAComplete");
                TextBox TextBoxPurchAmount = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPurchAmount");
                CheckBox CBExtInventoryPrinted = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBExtInventoryPrinted");
                CheckBox CBSpecFuncLedger = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBSpecFuncLedger");
                CheckBox CBIndirectCosts = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBIndirectCosts");
                CheckBox CBReviewedTransOCosts = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBReviewedTransOCosts");
                TextBox TextBoxPOSBalance = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPOSBalance");
                CheckBox CBSOC1 = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBSOC1");
                CheckBox CBOnlinePayments = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBOnlinePayments");
                CheckBox CBReviewBdgtReport = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBReviewBdgtReport");
                CheckBox CBNegAdjusts = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBNegAdjusts");
                TextBox TextBoxNegAdjustsComment = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxNegAdjustsComment");
                TextBox TextBoxAdjustments = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxAdjustments");
                CheckBox CBVendDepOSales = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBVendDepOSales");
                CheckBox CBEditChecksMatch = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBEditChecksMatch");
                CheckBox CBOpDaysVerified = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBOpDaysVerified");
                CheckBox CBSchoolInfoUpdated = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBSchoolInfoUpdated");
                CheckBox CBDistPayrollHrsEntered = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBDistPayrollHrsEntered");
                CheckBox CBHealthInsCaptured = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBHealthInsCaptured");
                TextBox TextBoxFoodCostComment = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxFoodCostComment");
                TextBox TextBoxPaperCleanComment = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPaperCleanComment");
                CheckBox CBUSDAInvMatches = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBUSDAInvMatches");
                TextBox TextBoxReviewedUsageComment = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxReviewedUsageComment");
                TextBox TextBoxTargetPurchasedComment = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxTargetPurchasedComment");
                TextBox TextBoxHoursWagesComment = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxHoursWagesComment");
                TextBox TextBoxBottomLineComment = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxBottomLineComment");
                CheckBox CheckBoxSFMatchOR = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CheckBoxSFMatchOR");
                TextBox TextBoxTRACSBldg = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxTRACSBldg");
                CheckBox CBNotifComplete = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBNotifComplete");

                //NEW 2/22/2023
                TextBox TextBoxPOSOverUnderCurrent = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPOSOverUnderCurrent");
                TextBox TextBoxPOSOverUnderYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPOSOverUnderYTD");
                CheckBox CBCompareTracsStatement = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBCompareTracsStatement");
                CheckBox CBCompletedClaimRec = (CheckBox)FindControlRecursive(FormViewEOMChecklist, "CBCompletedClaimRec");

                //New 4/1/2024
                TextBox TextBoxParticipationComments = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxParticipationComments");

                #endregion

                if (decimal.TryParse(TextBoxAdjustments.Text, out decimal AdjValue) == false)
                    AdjValue = 0m;

                if (TextBoxAdjustments != null && TextBoxNegAdjustsComment != null)
                {
                    if (AdjValue != 0M && TextBoxNegAdjustsComment.Text == string.Empty)
                    {
                        ShowDialog("Required", MessageType.Warning, "Recap Adjustment comment is required when Adjustment is not zero.", "Adjustment amount for the month is " + TextBoxAdjustments.Text);
                        TextBoxNegAdjustsComment.Focus();
                        return;
                    }
                }




                SqlConnection dbConnection2 = new SqlConnection(connectionString);
                using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection2))
                {
                    #region KeyVals
                    SqlParameter dbParam_ProgYear2 = new SqlParameter();
                    dbParam_ProgYear2.ParameterName = "@ProgramYear";
                    dbParam_ProgYear2.Value = KeyPYear;
                    dbParam_ProgYear2.DbType = DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_ProgYear2);

                    SqlParameter dbParam_DistID2 = new SqlParameter();
                    dbParam_DistID2.ParameterName = "@DistrictID";
                    dbParam_DistID2.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
                    dbParam_DistID2.DbType = DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_DistID2);

                    SqlParameter dbParam_YearMo2 = new SqlParameter();
                    dbParam_YearMo2.ParameterName = "@YearMonth";
                    dbParam_YearMo2.Value = YearMonth;
                    dbParam_YearMo2.DbType = DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_YearMo2);
                    #endregion
                    #region Inventory Vals
                    DateTime InvDate;
                    if (DateTime.TryParse(TextBoxDateOfInventory.Text, out InvDate) == false)
                        InvDate = new DateTime(1900, 1, 1);

                    SqlParameter dbParam_InvDate = new SqlParameter();
                    dbParam_InvDate.ParameterName = "@InventoryDate";
                    dbParam_InvDate.Value = InvDate;
                    dbParam_InvDate.DbType = DbType.Date;
                    cmd2.Parameters.Add(dbParam_InvDate);
                    /* PERIOD*/
                    SqlParameter dbParam_CasesNA = new SqlParameter();
                    dbParam_CasesNA.ParameterName = "@CasesNA";
                    dbParam_CasesNA.Value = TextBoxCasesNA.Text;
                    dbParam_CasesNA.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesNA);

                    SqlParameter dbParam_CasesK12 = new SqlParameter();
                    dbParam_CasesK12.ParameterName = "@CasesK12";
                    dbParam_CasesK12.Value = TextBoxCasesK12.Text;
                    dbParam_CasesK12.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesK12);

                    SqlParameter dbParam_CasesDOD = new SqlParameter();
                    dbParam_CasesDOD.ParameterName = "@CasesDOD";
                    dbParam_CasesDOD.Value = TextBoxCasesDOD.Text;
                    dbParam_CasesDOD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesDOD);

                    SqlParameter dbParam_CasesPL = new SqlParameter();
                    dbParam_CasesPL.ParameterName = "@CasesPL";
                    dbParam_CasesPL.Value = TextBoxCasesPL.Text;
                    dbParam_CasesPL.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesPL);

                    SqlParameter dbParam_CasesBB = new SqlParameter();
                    dbParam_CasesBB.ParameterName = "@CasesBB";
                    dbParam_CasesBB.Value = TextBoxCasesBB.Text;
                    dbParam_CasesBB.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesBB);

                    SqlParameter dbParam_ValueNA = new SqlParameter();
                    dbParam_ValueNA.ParameterName = "@ValueNA";
                    dbParam_ValueNA.Value = TextBoxValueNA.Text.Replace("$", "");
                    dbParam_ValueNA.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValueNA);

                    SqlParameter dbParam_ValueK12 = new SqlParameter();
                    dbParam_ValueK12.ParameterName = "@ValueK12";
                    dbParam_ValueK12.Value = TextBoxValueK12.Text.Replace("$", "");
                    dbParam_ValueK12.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValueK12);

                    SqlParameter dbParam_ValueDOD = new SqlParameter();
                    dbParam_ValueDOD.ParameterName = "@ValueDOD";
                    dbParam_ValueDOD.Value = TextBoxValueDOD.Text.Replace("$", "");
                    dbParam_ValueDOD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValueDOD);

                    SqlParameter dbParam_ValuePL = new SqlParameter();
                    dbParam_ValuePL.ParameterName = "@ValuePL";
                    dbParam_ValuePL.Value = TextBoxValuePL.Text.Replace("$", "");
                    dbParam_ValuePL.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValuePL);

                    SqlParameter dbParam_ValueBB = new SqlParameter();
                    dbParam_ValueBB.ParameterName = "@ValueBB";
                    dbParam_ValueBB.Value = TextBoxValueBB.Text.Replace("$", "");
                    dbParam_ValueBB.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValueBB);

                    /* YTD */
                    SqlParameter dbParam_CasesNAYTD = new SqlParameter();
                    dbParam_CasesNAYTD.ParameterName = "@CasesNAYTD";
                    dbParam_CasesNAYTD.Value = TextBoxCasesNAYTD.Text;
                    dbParam_CasesNAYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesNAYTD);

                    SqlParameter dbParam_CasesK12YTD = new SqlParameter();
                    dbParam_CasesK12YTD.ParameterName = "@CasesK12YTD";
                    dbParam_CasesK12YTD.Value = TextBoxCasesK12YTD.Text;
                    dbParam_CasesK12YTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesK12YTD);

                    SqlParameter dbParam_CasesDODYTD = new SqlParameter();
                    dbParam_CasesDODYTD.ParameterName = "@CasesDODYTD";
                    dbParam_CasesDODYTD.Value = TextBoxCasesDODYTD.Text;
                    dbParam_CasesDODYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesDODYTD);

                    SqlParameter dbParam_CasesPLYTD = new SqlParameter();
                    dbParam_CasesPLYTD.ParameterName = "@CasesPLYTD";
                    dbParam_CasesPLYTD.Value = TextBoxCasesPLYTD.Text;
                    dbParam_CasesPLYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesPLYTD);

                    SqlParameter dbParam_CasesBBYTD = new SqlParameter();
                    dbParam_CasesBBYTD.ParameterName = "@CasesBBYTD";
                    dbParam_CasesBBYTD.Value = TextBoxCasesBBYTD.Text;
                    dbParam_CasesBBYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CasesBBYTD);

                    SqlParameter dbParam_ValueNAYTD = new SqlParameter();
                    dbParam_ValueNAYTD.ParameterName = "@ValueNAYTD";
                    dbParam_ValueNAYTD.Value = TextBoxValueNAYTD.Text.Replace("$", "");
                    dbParam_ValueNAYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValueNAYTD);

                    SqlParameter dbParam_ValueK12YTD = new SqlParameter();
                    dbParam_ValueK12YTD.ParameterName = "@ValueK12YTD";
                    dbParam_ValueK12YTD.Value = TextBoxValueK12YTD.Text.Replace("$", "");
                    dbParam_ValueK12YTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValueK12YTD);

                    SqlParameter dbParam_ValueDODYTD = new SqlParameter();
                    dbParam_ValueDODYTD.ParameterName = "@ValueDODYTD";
                    dbParam_ValueDODYTD.Value = TextBoxValueDODYTD.Text.Replace("$", "");
                    dbParam_ValueDODYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValueDODYTD);

                    SqlParameter dbParam_ValuePLYTD = new SqlParameter();
                    dbParam_ValuePLYTD.ParameterName = "@ValuePLYTD";
                    dbParam_ValuePLYTD.Value = TextBoxValuePLYTD.Text.Replace("$", "");
                    dbParam_ValuePLYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValuePLYTD);

                    SqlParameter dbParam_ValueBBYTD = new SqlParameter();
                    dbParam_ValueBBYTD.ParameterName = "@ValueBBYTD";
                    dbParam_ValueBBYTD.Value = TextBoxValueBBYTD.Text.Replace("$", "");
                    dbParam_ValueBBYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_ValueBBYTD);

                    SqlParameter dbParam_usdaDiffCmnt = new SqlParameter();
                    dbParam_usdaDiffCmnt.ParameterName = "@USDADiffComment";
                    dbParam_usdaDiffCmnt.Value = TextBoxUSDAComment.Text;
                    dbParam_usdaDiffCmnt.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_usdaDiffCmnt);

                    SqlParameter dbParam_NOI = new SqlParameter();
                    dbParam_NOI.ParameterName = "@NOIAmount";
                    dbParam_NOI.Value = TextBoxNOIAmount.Text;
                    dbParam_NOI.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_NOI);

                    SqlParameter dbParam_CBPurch = new SqlParameter();
                    dbParam_CBPurch.ParameterName = "@CBPurchTotalEntered";
                    dbParam_CBPurch.Value = CBPurchTotalEntered.Checked;
                    dbParam_CBPurch.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBPurch);

                    SqlParameter dbParam_CBUSDA = new SqlParameter();
                    dbParam_CBUSDA.ParameterName = "@CBUSDAComplete";
                    dbParam_CBUSDA.Value = CBUSDAComplete.Checked;
                    dbParam_CBUSDA.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBUSDA);

                    SqlParameter dbParam_PrchAmt = new SqlParameter();
                    dbParam_PrchAmt.ParameterName = "@PurchTotal";
                    dbParam_PrchAmt.Value = TextBoxPurchAmount.Text;
                    dbParam_PrchAmt.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_PrchAmt);

                    SqlParameter dbParam_TRACSBldg = new SqlParameter();
                    dbParam_TRACSBldg.ParameterName = "@TRACSBldgTotals";
                    dbParam_TRACSBldg.Value = TextBoxTRACSBldg.Text;
                    dbParam_TRACSBldg.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_TRACSBldg);

                    SqlParameter dbParam_CBInvPrinted = new SqlParameter();
                    dbParam_CBInvPrinted.ParameterName = "@CBExtInventoryPrinted";
                    dbParam_CBInvPrinted.Value = CBExtInventoryPrinted.Checked;
                    dbParam_CBInvPrinted.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBInvPrinted);
                    #endregion
                    #region Special Function Vals
                    SqlParameter dbParam_CBSFLedger = new SqlParameter();
                    dbParam_CBSFLedger.ParameterName = "@CBSpecFuncLedger";
                    dbParam_CBSFLedger.Value = CBSpecFuncLedger.Checked;
                    dbParam_CBSFLedger.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBSFLedger);

                    SqlParameter dbParam_CBSFLedgerYTD = new SqlParameter();
                    dbParam_CBSFLedgerYTD.ParameterName = "@CBSFLedgerYTDMatchesOR";
                    dbParam_CBSFLedgerYTD.Value = CheckBoxSFMatchOR.Checked;
                    dbParam_CBSFLedgerYTD.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBSFLedgerYTD);
                    #endregion
                    #region Other Vals
                    SqlParameter dbParam_CBIndCosts = new SqlParameter();
                    dbParam_CBIndCosts.ParameterName = "@CBIndirectCosts";
                    dbParam_CBIndCosts.Value = CBIndirectCosts.Checked;
                    dbParam_CBIndCosts.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBIndCosts);

                    SqlParameter dbParam_RevwOcosts = new SqlParameter();
                    dbParam_RevwOcosts.ParameterName = "@CBReviewedTransOCosts";
                    dbParam_RevwOcosts.Value = CBReviewedTransOCosts.Checked;
                    dbParam_RevwOcosts.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_RevwOcosts);

                    SqlParameter dbParam_POSBal = new SqlParameter();
                    dbParam_POSBal.ParameterName = "@POSBalance";
                    dbParam_POSBal.Value = TextBoxPOSBalance.Text;
                    dbParam_POSBal.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_POSBal);

                    SqlParameter dbParam_CBSOC1 = new SqlParameter();
                    dbParam_CBSOC1.ParameterName = "@CBSOC1";
                    dbParam_CBSOC1.Value = CBSOC1.Checked;
                    dbParam_CBSOC1.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBSOC1);

                    SqlParameter dbParam_CBNotifComplete = new SqlParameter();
                    dbParam_CBNotifComplete.ParameterName = "@CBNotificationsComplete";
                    dbParam_CBNotifComplete.Value = CBNotifComplete.Checked;
                    dbParam_CBNotifComplete.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBNotifComplete);

                    SqlParameter dbParam_POSOverUnderCurrent = new SqlParameter();
                    dbParam_POSOverUnderCurrent.ParameterName = "@POSOverUnderCurrent";
                    dbParam_POSOverUnderCurrent.Value = TextBoxPOSOverUnderCurrent.Text;
                    dbParam_POSOverUnderCurrent.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_POSOverUnderCurrent);

                    SqlParameter dbParam_POSOverUnderYTD = new SqlParameter();
                    dbParam_POSOverUnderYTD.ParameterName = "@POSOverUnderYTD";
                    dbParam_POSOverUnderYTD.Value = TextBoxPOSOverUnderYTD.Text;
                    dbParam_POSOverUnderYTD.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_POSOverUnderYTD);

                    SqlParameter dbParam_CBCompareTracsStatement = new SqlParameter();
                    dbParam_CBCompareTracsStatement.ParameterName = "@CBCompareTracsStatement";
                    dbParam_CBCompareTracsStatement.Value = CBCompareTracsStatement.Checked;
                    dbParam_CBCompareTracsStatement.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBCompareTracsStatement);
                    #endregion
                    #region Sales Vals
                    SqlParameter dbParam_CBOnlinePayments = new SqlParameter();
                    dbParam_CBOnlinePayments.ParameterName = "@CBOnlinePayments";
                    dbParam_CBOnlinePayments.Value = CBOnlinePayments.Checked;
                    dbParam_CBOnlinePayments.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBOnlinePayments);

                    SqlParameter dbParam_CBReviewBdgtReport = new SqlParameter();
                    dbParam_CBReviewBdgtReport.ParameterName = "@CBReviewBdgtReport";
                    dbParam_CBReviewBdgtReport.Value = CBReviewBdgtReport.Checked;
                    dbParam_CBReviewBdgtReport.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBReviewBdgtReport);

                    SqlParameter dbParam_CBNegAdjusts = new SqlParameter();
                    dbParam_CBNegAdjusts.ParameterName = "@CBNegAdjusts";
                    dbParam_CBNegAdjusts.Value = CBNegAdjusts.Checked;
                    dbParam_CBNegAdjusts.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBNegAdjusts);

                    SqlParameter dbParam_NegAdjustsComment = new SqlParameter();
                    dbParam_NegAdjustsComment.ParameterName = "@NegAdjustsComment";
                    dbParam_NegAdjustsComment.Value = TextBoxNegAdjustsComment.Text;
                    dbParam_NegAdjustsComment.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_NegAdjustsComment);

                    SqlParameter dbParam_CBVendDepOSales = new SqlParameter();
                    dbParam_CBVendDepOSales.ParameterName = "@CBVendDepOSales";
                    dbParam_CBVendDepOSales.Value = CBVendDepOSales.Checked;
                    dbParam_CBVendDepOSales.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBVendDepOSales);

                    SqlParameter dbParam_ParticComment = new SqlParameter();
                    dbParam_ParticComment.ParameterName = "@ParticipationComments";
                    dbParam_ParticComment.Value = TextBoxParticipationComments.Text;
                    dbParam_ParticComment.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_ParticComment);
                    #endregion
                    #region Edit Checks Vals
                    SqlParameter dbParam_CBEditChecksMatch = new SqlParameter();
                    dbParam_CBEditChecksMatch.ParameterName = "@CBEditChecksMatch";
                    dbParam_CBEditChecksMatch.Value = CBEditChecksMatch.Checked;
                    dbParam_CBEditChecksMatch.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBEditChecksMatch);

                    SqlParameter dbParam_CBOpDaysVerified = new SqlParameter();
                    dbParam_CBOpDaysVerified.ParameterName = "@CBOpDaysVerified";
                    dbParam_CBOpDaysVerified.Value = CBOpDaysVerified.Checked;
                    dbParam_CBOpDaysVerified.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBOpDaysVerified);

                    SqlParameter dbParam_CBSchoolInfoUpdated = new SqlParameter();
                    dbParam_CBSchoolInfoUpdated.ParameterName = "@CBSchoolInfoUpdated";
                    dbParam_CBSchoolInfoUpdated.Value = CBSchoolInfoUpdated.Checked;
                    dbParam_CBSchoolInfoUpdated.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBSchoolInfoUpdated);

                    SqlParameter dbParam_CBCompletedClaimRec = new SqlParameter();
                    dbParam_CBCompletedClaimRec.ParameterName = "@CBCompletedClaimRec";
                    dbParam_CBCompletedClaimRec.Value = CBCompletedClaimRec.Checked;
                    dbParam_CBCompletedClaimRec.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBCompletedClaimRec);

                    
                    #endregion
                    #region Payroll Vals
                    SqlParameter dbParam_CBDistPayrollHrsEntered = new SqlParameter();
                    dbParam_CBDistPayrollHrsEntered.ParameterName = "@CBDistPayrollHrsEntered";
                    dbParam_CBDistPayrollHrsEntered.Value = CBDistPayrollHrsEntered.Checked;
                    dbParam_CBDistPayrollHrsEntered.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBDistPayrollHrsEntered);

                    SqlParameter dbParam_CBHealthInsCaptured = new SqlParameter();
                    dbParam_CBHealthInsCaptured.ParameterName = "@CBHealthInsCaptured";
                    dbParam_CBHealthInsCaptured.Value = CBHealthInsCaptured.Checked;
                    dbParam_CBHealthInsCaptured.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBHealthInsCaptured);
                    #endregion
                    #region Operating Report Vals
                    SqlParameter dbParam_FoodCostComment = new SqlParameter();
                    dbParam_FoodCostComment.ParameterName = "@FoodCostComment";
                    dbParam_FoodCostComment.Value = TextBoxFoodCostComment.Text;
                    dbParam_FoodCostComment.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_FoodCostComment);

                    SqlParameter dbParam_PaperCleanComment = new SqlParameter();
                    dbParam_PaperCleanComment.ParameterName = "@PaperCleanComment";
                    dbParam_PaperCleanComment.Value = TextBoxPaperCleanComment.Text;
                    dbParam_PaperCleanComment.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_PaperCleanComment);

                    SqlParameter dbParam_CBUSDAInvMatches = new SqlParameter();
                    dbParam_CBUSDAInvMatches.ParameterName = "@CBUSDAInvMatches";
                    dbParam_CBUSDAInvMatches.Value = CBUSDAInvMatches.Checked;
                    dbParam_CBUSDAInvMatches.DbType = DbType.Byte;
                    cmd2.Parameters.Add(dbParam_CBUSDAInvMatches);

                    SqlParameter dbParam_ReviewedUsageComment = new SqlParameter();
                    dbParam_ReviewedUsageComment.ParameterName = "@ReviewedUsageComment";
                    dbParam_ReviewedUsageComment.Value = TextBoxReviewedUsageComment.Text;
                    dbParam_ReviewedUsageComment.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_ReviewedUsageComment);

                    SqlParameter dbParam_TargetPurchasedComment = new SqlParameter();
                    dbParam_TargetPurchasedComment.ParameterName = "@TargetPurchasedComment";
                    dbParam_TargetPurchasedComment.Value = TextBoxTargetPurchasedComment.Text;
                    dbParam_TargetPurchasedComment.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_TargetPurchasedComment);

                    SqlParameter dbParam_HoursWagesComment = new SqlParameter();
                    dbParam_HoursWagesComment.ParameterName = "@HoursWagesComment";
                    dbParam_HoursWagesComment.Value = TextBoxHoursWagesComment.Text;
                    dbParam_HoursWagesComment.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_HoursWagesComment);
                    #endregion
                    #region Profit Statement Vals
                    SqlParameter dbParam_BottomLineComment = new SqlParameter();
                    dbParam_BottomLineComment.ParameterName = "@BottomLineComment";
                    dbParam_BottomLineComment.Value = TextBoxBottomLineComment.Text;
                    dbParam_BottomLineComment.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_BottomLineComment);
                    #endregion
                    #region misc vals
                    SqlParameter dbParam_LastEditedUser = new SqlParameter();
                    dbParam_LastEditedUser.ParameterName = "@LastEditedUser";
                    dbParam_LastEditedUser.Value = Context.User.Identity.Name.ToUpper();
                    dbParam_LastEditedUser.DbType = DbType.String;
                    cmd2.Parameters.Add(dbParam_LastEditedUser);
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
                        ShowDialog("SQL Error", MessageType.Error, "An error occurred during checklist save.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "An error occurred during checklist save.", ex.Message);
                    }
                    finally
                    {
                        dbConnection2.Close();
                    }
                }
            }

            HFChecklistSaved.Value = "1";
            //Call Jquery which populates the totals
            //RegisterDOMReadyScript("Page Load Recalculations", "RecalcCasesTotals();");
        }


        protected void ChecklistItemChanged(object sender, EventArgs e)
        {
            SaveChecklist();
        }

        protected void ChecklistSchoolItemChanged(object sender, EventArgs e)
        {
            GridViewRow gvr = ((GridViewRow)((Control)sender).Parent/*Cell*/.Parent);

            if (gvr != null)
            {
                GridView GridViewSchoolRecords = (GridView)gvr.NamingContainer;
                if (GridViewSchoolRecords == null)
                    return;

                //GridViewSchoolRecords.DataKeys[gvr.DataItemIndex].Values["ProgramYear"]
                //KEY VALUES
                String KeyPYear = GridViewSchoolRecords.DataKeys[gvr.DataItemIndex].Values["ProgramYear"].ToString();
                String KeyDistID = GridViewSchoolRecords.DataKeys[gvr.DataItemIndex].Values["DistrictID"].ToString();
                String YearMonth = GridViewSchoolRecords.DataKeys[gvr.DataItemIndex].Values["YearMonth"].ToString();
                Int16 SchoolNum = (Int16)GridViewSchoolRecords.DataKeys[gvr.DataItemIndex].Values["SchoolNum"];

                HiddenField HFRowSaved = (HiddenField)FindControlRecursive(gvr, "HFRowSaved");
                TextBox TextBoxSnapshotDate = (TextBox)FindControlRecursive(gvr, "TextBoxSnapshotDate");
                CheckBox CBUpdatedPricing = (CheckBox)FindControlRecursive(gvr, "CBUpdatedPricing");
                TextBox TextBoxSchoolInventoryDate = (TextBox)FindControlRecursive(gvr, "TextBoxSchoolInventoryDate");
                TextBox TextBoxSnapshotName = (TextBox)FindControlRecursive(gvr, "TextBoxSnapshotName");
                TextBox TextBoxSnapshotTotal = (TextBox)FindControlRecursive(gvr, "TextBoxSnapshotTotal");

                if (HFRowSaved == null || TextBoxSnapshotDate == null || CBUpdatedPricing == null || TextBoxSchoolInventoryDate == null || TextBoxSnapshotName==null || TextBoxSnapshotTotal==null)
                    return;

                if (HFRowSaved.Value == "1")
                    return;

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
                SqlConnection dbConnection = new SqlConnection(connectionString);

                // Need to check to see if the record exists, then run the Insert or Update statement
                // If UPDATE - need to check to see if they are allowed to update
                string queryChecklistExists = "SELECT COUNT(*) FROM [EOMCheckListSchools] " +
                        " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth AND SchoolNum=@SchoolNum ";

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

                    SqlParameter dbParam_ScNum = new SqlParameter();
                    dbParam_ScNum.ParameterName = "@SchoolNum";
                    dbParam_ScNum.Value = SchoolNum;
                    dbParam_ScNum.DbType = DbType.Int16;
                    cmd.Parameters.Add(dbParam_ScNum);

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
                        ShowDialog("SQL Error", MessageType.Error, "A database error occurred while checking to see if school record existed.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Unknown Error", MessageType.Error, "An error occurred while checking to see if school record existed.", ex.Message);
                    }
                    finally
                    {
                        dbConnection.Close();
                    }

                    string InsertUpdateQuery = "";

                    if (existingRecordCount == 0)
                    {
                        InsertUpdateQuery = "INSERT INTO [EOMChecklistSchools] (ProgramYear, DistrictID, YearMonth, SchoolNum, SnapshotDate, CBUpdatedPricing, DateOfPhysicalInventory, SnapshotName, SnapshotTotal) " +
                            " VALUES (@ProgramYear, @DistrictID, @YearMonth, @SchoolNum, @SnapshotDate, @CBUpdatedPricing, @DateOfPhysicalInventory, @SnapshotName, @SnapshotTotal) ";
                    }
                    else
                    {
                        InsertUpdateQuery = "UPDATE [EOMChecklistSchools] SET SnapshotDate=@SnapshotDate, CBUpdatedPricing=@CBUpdatedPricing, DateOfPhysicalInventory=@DateOfPhysicalInventory, SnapshotName=@SnapshotName, SnapshotTotal=@SnapshotTotal " +
                            " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth AND SchoolNum=@SchoolNum ";
                    }

                    SqlConnection dbConnection2 = new SqlConnection(connectionString);
                    using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection2))
                    {
                        SqlParameter dbParam_ProgYear2 = new SqlParameter();
                        dbParam_ProgYear2.ParameterName = "@ProgramYear";
                        dbParam_ProgYear2.Value = KeyPYear;
                        dbParam_ProgYear2.DbType = DbType.String;
                        cmd2.Parameters.Add(dbParam_ProgYear2);

                        SqlParameter dbParam_DistID2 = new SqlParameter();
                        dbParam_DistID2.ParameterName = "@DistrictID";
                        dbParam_DistID2.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
                        dbParam_DistID2.DbType = DbType.StringFixedLength;
                        cmd2.Parameters.Add(dbParam_DistID2);

                        SqlParameter dbParam_YearMo2 = new SqlParameter();
                        dbParam_YearMo2.ParameterName = "@YearMonth";
                        dbParam_YearMo2.Value = YearMonth;
                        dbParam_YearMo2.DbType = DbType.StringFixedLength;
                        cmd2.Parameters.Add(dbParam_YearMo2);

                        SqlParameter dbParam_ScNum2 = new SqlParameter();
                        dbParam_ScNum2.ParameterName = "@SchoolNum";
                        dbParam_ScNum2.Value = SchoolNum;
                        dbParam_ScNum2.DbType = DbType.Int16;
                        cmd2.Parameters.Add(dbParam_ScNum2);

                        if (DateTime.TryParse(TextBoxSnapshotDate.Text, out DateTime sDate) == false)
                            sDate = new DateTime(1900, 1, 1);

                        SqlParameter dbParam_SnapDate = new SqlParameter();
                        dbParam_SnapDate.ParameterName = "@SnapshotDate";
                        dbParam_SnapDate.Value = sDate.ToShortDateString();
                        dbParam_SnapDate.DbType = DbType.Date;
                        cmd2.Parameters.Add(dbParam_SnapDate);

                        SqlParameter dbParam_PricingUp = new SqlParameter();
                        dbParam_PricingUp.ParameterName = "@CBUpdatedPricing";
                        dbParam_PricingUp.Value = CBUpdatedPricing.Checked;
                        dbParam_PricingUp.DbType = DbType.Byte;
                        cmd2.Parameters.Add(dbParam_PricingUp);

                        if (DateTime.TryParse(TextBoxSchoolInventoryDate.Text, out DateTime sInvDate) == false)
                            sInvDate = new DateTime(1900, 1, 1);

                        SqlParameter dbParam_SchoolInvDate = new SqlParameter();
                        dbParam_SchoolInvDate.ParameterName = "@DateOfPhysicalInventory";
                        dbParam_SchoolInvDate.Value = sInvDate.ToShortDateString();
                        dbParam_SchoolInvDate.DbType = DbType.Date;
                        cmd2.Parameters.Add(dbParam_SchoolInvDate);

                        SqlParameter dbParam_SnapName = new SqlParameter();
                        dbParam_SnapName.ParameterName = "@SnapshotName";
                        dbParam_SnapName.Value = TextBoxSnapshotName.Text;
                        dbParam_SnapName.DbType = DbType.String;
                        cmd2.Parameters.Add(dbParam_SnapName);

                        if (Decimal.TryParse(TextBoxSnapshotTotal.Text, out decimal decSnapshotTotal) == false)
                            decSnapshotTotal = 0M;

                        TextBoxSnapshotTotal.Text = decSnapshotTotal.ToString("F2");

                        SqlParameter dbParam_SnapTotal = new SqlParameter();
                        dbParam_SnapTotal.ParameterName = "@SnapshotTotal";
                        dbParam_SnapTotal.Value = decSnapshotTotal;
                        dbParam_SnapTotal.DbType = DbType.Decimal;
                        cmd2.Parameters.Add(dbParam_SnapTotal);

                        try
                        {
                            dbConnection2.Open();
                            int recordcount = 0;
                            recordcount = cmd2.ExecuteNonQuery();

                            if (recordcount > 0)
                            {
                                //ShowMessageIcon("Saved", MessageType.Success);

                            }
                        }
                        catch (SqlException ex)
                        {
                            ShowDialog("SQL Error", MessageType.Error, "An error occurred during checklist save.", ex.Message);
                        }
                        catch (Exception ex)
                        {
                            ShowDialog("Error", MessageType.Error, "An error occurred during checklist save.", ex.Message);
                        }
                        finally
                        {
                            dbConnection2.Close();
                        }
                    }

                }

                HFRowSaved.Value = "1";
                UpdateGridViewSchoolRecordsTotals();
            }
        }

        protected void UpdateGridViewSchoolRecordsTotals()
        {
            GridView GridViewSchoolRecords = (GridView)FindControlRecursive(FormViewEOMChecklist, "GridViewSchoolRecords");
            if (GridViewSchoolRecords == null)
                return;

            decimal decSnapTotal=0M;

            //KEY VALUES
            String KeyPYear = DDLProgramYears.SelectedValue;
            String KeyDistID = DropDownListDistricts.SelectedValue;
            String YearMonth = TextBoxFYearmonth.Text;

            TextBox TextBoxSnapshotTotalFooter = (TextBox)FindControlRecursive(GridViewSchoolRecords, "TextBoxSnapshotTotalFooter");
            if (TextBoxSnapshotTotalFooter == null)
                return;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryChecklistExists = "SELECT SUM(SnapshotTotal) SumSnapshot FROM [EOMCheckListSchools] " +
                    " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth ";

            using (SqlCommand cmd = new SqlCommand(queryChecklistExists, dbConnection))
            {
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

                try
                {
                    dbConnection.Open();
                    Object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        decimal.TryParse(obj.ToString(), out decSnapTotal);
                    }

                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "A database error occurred while checking to see if school record existed.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Unknown Error", MessageType.Error, "An error occurred while checking to see if school record existed.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

            TextBoxSnapshotTotalFooter.Text = decSnapTotal.ToString("F2");
        }

            protected void FormViewEOMChecklist_PreRender(object sender, EventArgs e)
        {
            HiddenField HFChecklistSaved = (HiddenField)FindControlRecursive(FormViewEOMChecklist, "HFChecklistSaved");

            if (HFChecklistSaved != null)
                HFChecklistSaved.Value = "0";
        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox dateTextBox = (TextBox)sender;

            // If just digits are typed - convert to standard date
            NormalizeSimplifiedDate(ref dateTextBox);

            //run the changed procedure
            ChecklistItemChanged(sender, e);
        }

        protected void FormViewEOMChecklist_DataBound(object sender, EventArgs e)
        {
            TextBox TextBoxDateOfInventory = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxDateOfInventory");
            if (TextBoxDateOfInventory != null && TextBoxDateOfInventory.Text == "1/1/1900")
                TextBoxDateOfInventory.Text = string.Empty;

            //Populate pulled in USDA Period values
            TextBox TextBoxUSDACasesNA = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesNA");
            TextBox TextBoxUSDACasesK12 = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesK12");
            TextBox TextBoxUSDACasesDOD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesDOD");
            TextBox TextBoxUSDACasesPL = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesPL");
            TextBox TextBoxUSDACasesBB = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesBB");
            TextBox TextBoxUSDAValueNA = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValueNA");
            TextBox TextBoxUSDAValueK12 = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValueK12");
            TextBox TextBoxUSDAValuePL = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValuePL");
            TextBox TextBoxUSDAValueDOD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValueDOD");
            TextBox TextBoxUSDAValueBB = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValueBB");
            if (TextBoxUSDACasesNA != null && TextBoxUSDACasesK12 != null && TextBoxUSDACasesDOD != null && TextBoxUSDACasesPL != null && TextBoxUSDACasesBB != null &&
                TextBoxUSDAValueNA != null && TextBoxUSDAValueK12 != null && TextBoxUSDAValuePL != null && TextBoxUSDAValueBB != null && TextBoxUSDAValueDOD != null)
            {
                TextBoxUSDACasesNA.Text = "0.00";
                TextBoxUSDACasesK12.Text = "0.00";
                TextBoxUSDACasesDOD.Text = "0.00";
                TextBoxUSDACasesPL.Text = "0.00";
                TextBoxUSDACasesBB.Text = "0.00";

                TextBoxUSDAValueNA.Text = "0.00";
                TextBoxUSDAValueK12.Text = "0.00";
                TextBoxUSDAValuePL.Text = "0.00";
                TextBoxUSDAValueBB.Text = "0.00";
                TextBoxUSDAValueDOD.Text = "0.00";


                SqlDataSourceUSDAInventory.DataBind();
                SqlDataReader rdr = (SqlDataReader)SqlDataSourceUSDAInventory.Select(DataSourceSelectArguments.Empty);
                if (rdr != null)
                {
                    while (rdr.Read())
                    {
                        string itemClass = rdr["ItemClass"].ToString().Trim();
                        switch (itemClass)
                        {
                            case "BB":
                                TextBoxUSDACasesBB.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                TextBoxUSDAValueBB.Text = ((decimal)rdr["TotalCasesReceivedValue"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                            case "DOD":
                                //TextBoxUSDACasesDOD.Text = rdr["TotalCasesReceived"].ToString();
                                TextBoxUSDAValueDOD.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                            case "K12":
                                TextBoxUSDACasesK12.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                TextBoxUSDAValueK12.Text = ((decimal)rdr["TotalCasesReceivedValue"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                            case "PL":
                                TextBoxUSDACasesPL.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                TextBoxUSDAValuePL.Text = ((decimal)rdr["TotalCasesReceivedValue"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                            default:
                                TextBoxUSDACasesNA.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                TextBoxUSDAValueNA.Text = ((decimal)rdr["TotalCasesReceivedValue"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                        }
                    }
                }
            }

            //POPULATE USDA YTD VALUES
            TextBox TextBoxUSDACasesNAYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesNAYTD");
            TextBox TextBoxUSDACasesK12YTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesK12YTD");
            TextBox TextBoxUSDACasesDODYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesDODYTD");
            TextBox TextBoxUSDACasesPLYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesPLYTD");
            TextBox TextBoxUSDACasesBBYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDACasesBBYTD");
            TextBox TextBoxUSDAValueNAYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValueNAYTD");
            TextBox TextBoxUSDAValueK12YTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValueK12YTD");
            TextBox TextBoxUSDAValuePLYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValuePLYTD");
            TextBox TextBoxUSDAValueDODYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValueDODYTD");
            TextBox TextBoxUSDAValueBBYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAValueBBYTD");
            if (TextBoxUSDACasesNAYTD != null && TextBoxUSDACasesK12YTD != null && TextBoxUSDACasesDODYTD != null && TextBoxUSDACasesPLYTD != null && TextBoxUSDACasesBBYTD != null &&
                TextBoxUSDAValueNAYTD != null && TextBoxUSDAValueK12YTD != null && TextBoxUSDAValuePLYTD != null && TextBoxUSDAValueBBYTD != null && TextBoxUSDAValueDODYTD != null)
            {
                TextBoxUSDACasesNAYTD.Text = "0.00";
                TextBoxUSDACasesK12YTD.Text = "0.00";
                TextBoxUSDACasesDODYTD.Text = "0.00";
                TextBoxUSDACasesPLYTD.Text = "0.00";
                TextBoxUSDACasesBBYTD.Text = "0.00";

                TextBoxUSDAValueNAYTD.Text = "0.00";
                TextBoxUSDAValueK12YTD.Text = "0.00";
                TextBoxUSDAValuePLYTD.Text = "0.00";
                TextBoxUSDAValueBBYTD.Text = "0.00";
                TextBoxUSDAValueDODYTD.Text = "0.00";


                SqlDataSourceUSDAInvYTD.DataBind();
                SqlDataReader rdr = (SqlDataReader)SqlDataSourceUSDAInvYTD.Select(DataSourceSelectArguments.Empty);
                if (rdr != null)
                {
                    while (rdr.Read())
                    {
                        string itemClass = rdr["ItemClass"].ToString().Trim();
                        switch (itemClass)
                        {
                            case "BB":
                                TextBoxUSDACasesBBYTD.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                TextBoxUSDAValueBBYTD.Text = ((decimal)rdr["TotalCasesReceivedValue"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                            case "DOD":
                                //TextBoxUSDACasesDODYTD.Text = rdr["TotalCasesReceived"].ToString();
                                TextBoxUSDAValueDODYTD.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);//rdr["TotalCasesReceivedValue"].ToString();
                                break;
                            case "K12":
                                TextBoxUSDACasesK12YTD.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                TextBoxUSDAValueK12YTD.Text = ((decimal)rdr["TotalCasesReceivedValue"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                            case "PL":
                                TextBoxUSDACasesPLYTD.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                TextBoxUSDAValuePLYTD.Text = ((decimal)rdr["TotalCasesReceivedValue"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                            default:
                                TextBoxUSDACasesNAYTD.Text = ((decimal)rdr["TotalCasesReceived"]).ToString("F2", CultureInfo.CurrentCulture);
                                TextBoxUSDAValueNAYTD.Text = ((decimal)rdr["TotalCasesReceivedValue"]).ToString("F2", CultureInfo.CurrentCulture);
                                break;
                        }
                    }
                }
            }


            //Operating - BDGT amounts
            TextBox TextBoxFoodCostBDGT = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxFoodCostBDGT");
            TextBox TextBoxPprClnBDGT = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPprClnBDGT");
            TextBox TextBoxTNGHoursBdgt = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxTNGHoursBdgt");
            TextBox TextBoxTNGWagesBdgt = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxTNGWagesBdgt");
            TextBox TextBoxDistHoursBdgt = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxDistHoursBdgt");
            TextBox TextBoxDistWagesBdgt = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxDistWagesBdgt");
            TextBox TextBoxUSDABdgt = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDABdgt");
            decimal decNutrProdHours = 0M;
            decimal decDistProdHours = 0M;
            decimal decNutrNonProdHours = 0M;
            decimal decDistNonProdHours = 0M;
            decimal decNutrSpFnHours = 0M;
            decimal decDistSpFnHours = 0M;
            decimal decDistLaborDay = 0M;
            decimal decNUTRLaborDay = 0M;
            decimal decProfitBudget = 0M;
            decimal decDaysOfServiceLunch = 0M;
            decimal decDailyPL = 0M;
            TextBox TextBoxProfitBudget1 = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxProfitBudget1");
            TextBox TextBoxProfitBudget2 = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxProfitBudget2");
            TextBox TextBoxServiceDays1 = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxServiceDays1");
            TextBox TextBoxServiceDays2 = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxServiceDays2");
            TextBox TextBoxDaysMonth = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxDaysMonth");
            TextBox TextBoxDaysYtd = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxDaysYtd");
            TextBox TextBoxBudgetPeriod = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxBudgetPeriod");
            TextBox TextBoxBudgetYtd = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxBudgetYtd");

            decimal decBrkDailyCountBud = 0M;
            decimal decLunDailyCountBud = 0M;
            decimal decAlaDailyBud = 0M;


            
            if (TextBoxFoodCostBDGT != null && TextBoxPprClnBDGT != null && TextBoxUSDABdgt != null)
            {
                SqlDataSourceBDGT.DataBind();
                //DateTime startDate = new DateTime(int.Parse(TextBoxFYearmonth.Text.Substring(0, 4)), int.Parse(TextBoxFYearmonth.Text.Substring(5, 2)), 1);
                //DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                SqlDataReader rdr = (SqlDataReader)SqlDataSourceBDGT.Select(DataSourceSelectArguments.Empty);
                if (rdr != null && rdr.Read())
                {
                    decimal.TryParse(rdr["FoodCPM"].ToString(), out decimal decFoodCPM);
                    decimal.TryParse(rdr["PaperCleanCPM"].ToString(), out decimal decPaperCleanCPM);
                    TextBoxFoodCostBDGT.Text = decFoodCPM.ToString("F3");
                    TextBoxPprClnBDGT.Text = decPaperCleanCPM.ToString("F3");

                    decimal.TryParse(rdr["NUTRHoursDayTotal"].ToString(), out decNutrProdHours);
                    decimal.TryParse(rdr["SFAHoursDayTotal"].ToString(), out decDistProdHours);
                    decimal.TryParse(rdr["NUTRNPHoursDayTotal"].ToString(), out decNutrNonProdHours);
                    decimal.TryParse(rdr["SFANPHoursDayTotal"].ToString(), out decDistNonProdHours);
                    decimal.TryParse(rdr["SPFNNutrHrsPerDay"].ToString(), out decNutrSpFnHours);
                    decimal.TryParse(rdr["SPFNDistHrsPerDay"].ToString(), out decDistSpFnHours);
                    decimal.TryParse(rdr["SFAWagesDayTotal"].ToString(), out decDistLaborDay);
                    decimal.TryParse(rdr["NUTRWagesDayTotal"].ToString(), out decNUTRLaborDay);

                    decimal.TryParse(rdr["LessCommodityUseAmt"].ToString(), out decimal decUsdaBdgt);
                    TextBoxUSDABdgt.Text = decUsdaBdgt.ToString("F3");

                    decimal.TryParse(rdr["ProfitBudget"].ToString(), out decProfitBudget);
                    decimal.TryParse(rdr["DaysOfSvcLunch"].ToString(), out decDaysOfServiceLunch);

                    TextBoxProfitBudget1.Text = decProfitBudget.ToString("N2");
                    TextBoxProfitBudget2.Text = decProfitBudget.ToString("N2");
                    TextBoxServiceDays1.Text = decDaysOfServiceLunch.ToString("F0");
                    TextBoxServiceDays2.Text = decDaysOfServiceLunch.ToString("F0");

                    if (decDaysOfServiceLunch != 0)
                        decDailyPL = (decProfitBudget / decDaysOfServiceLunch);


                    decimal.TryParse(rdr["DailyBPaidCount"].ToString(), out decimal decPaidBkftCount);
                    decimal.TryParse(rdr["DailyBReducedCount"].ToString(), out decimal decReducedBkftCount);
                    decimal.TryParse(rdr["DailyBFreeCount"].ToString(), out decimal decFreeBkftCount);
                    decBrkDailyCountBud = decPaidBkftCount + decReducedBkftCount + decFreeBkftCount;

                    decimal.TryParse(rdr["DailyLPaidCount"].ToString(), out decimal decPaidLunCount);
                    decimal.TryParse(rdr["DailyLReducedCount"].ToString(), out decimal decReducedLunCount);
                    decimal.TryParse(rdr["DailyLFreeCount"].ToString(), out decimal decFreeLunCount);
                    decLunDailyCountBud = decPaidLunCount + decReducedLunCount + decFreeLunCount;

                    decimal.TryParse(rdr["DailyAla"].ToString(), out decAlaDailyBud);


                }
                else
                {
                    TextBoxFoodCostBDGT.Text = "0.000";
                    TextBoxPprClnBDGT.Text = "0.000";
                    TextBoxTNGHoursBdgt.Text = "0.00";
                    TextBoxTNGWagesBdgt.Text = "0.00";
                    TextBoxDistHoursBdgt.Text = "0.00";
                    TextBoxDistWagesBdgt.Text = "0.00";
                }
            }

            TextBox TextBoxFoodCostActualM = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxFoodCostActualM");
            TextBox TextBoxFoodCostActualY = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxFoodCostActualY");
            TextBox TextBoxPaperCleanActualM = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPaperCleanActualM");
            TextBox TextBoxPaperCleanActualY = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPaperCleanActualY");
            Label LabelMeals = (Label)FindControlRecursive(FormViewEOMChecklist, "LabelMeals");
            Label LabelNumDays = (Label)FindControlRecursive(FormViewEOMChecklist, "LabelNumDays");
            Label LabelFoodPaperClean = (Label)FindControlRecursive(FormViewEOMChecklist, "LabelFoodPaperClean");
            Label LabelTotalTargetPurchInv = (Label)FindControlRecursive(FormViewEOMChecklist, "LabelTotalTargetPurchInv");
            TextBox TextBoxTNGHoursActual = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxTNGHoursActual");
            TextBox TextBoxDistHoursActual = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxDistHoursActual");
            TextBox TextBoxTNGWagesActual = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxTNGWagesActual");
            TextBox TextBoxDistWagesActual = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxDistWagesActual");
            HiddenField HFSFA_PR = (HiddenField)FindControlRecursive(FormViewEOMChecklist, "HFSFA_PR");
            decimal decTtlMeals = 0M;
            decimal decTtlMealsYtd = 0M;
            decimal decTtlLunches = 0M;
            decimal decTtlLunchesYtd = 0M;
            decimal decTtlBreakfasts = 0M;
            decimal decTtlBreakfastsYtd = 0M;
            decimal decTtlAla = 0M;
            decimal decTtlAlaYtd = 0M;
            decimal decLunchDays = 0M;
            decimal decLunchDaysYtd = 0M;
            decimal decBreakfastDays = 0M;
            decimal decBreakfastDaysYtd = 0M;
            TextBox TextBoxUSDAActual = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAActual");
            TextBox TextBoxPLCurrent = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPLCurrent");
            TextBox TextBoxPLYtd = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPLYtd");
            TextBox TextBoxUSDAYTD = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxUSDAYTD");
            TextBox TextBoxPurchAmount = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxPurchAmount");

            //Sales Adjustments
            TextBox TextBoxAdjustments = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxAdjustments");
            Label LabelCommentRequired = (Label)FindControlRecursive(FormViewEOMChecklist, "LabelCommentRequired");

            //New Participation 4/2024
            TextBox TextBoxBrMonth = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxBrMonth");
            TextBox TextBoxLnMonth = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxLnMonth");
            TextBox TextBoxAlaMonth = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxAlaMonth");

            TextBox TextBoxBrMonthBud = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxBrMonthBud");
            TextBox TextBoxLnMonthBud = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxLnMonthBud");
            TextBox TextBoxAlaMonthBud = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxAlaMonthBud");

            TextBox TextBoxBrYtd = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxBrYtd");
            TextBox TextBoxLnYtd = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxLnYtd");
            TextBox TextBoxAlaYtd = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxAlaYtd");

            TextBox TextBoxBrYtdBud = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxBrYtdBud");
            TextBox TextBoxLnYtdBud = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxLnYtdBud");
            TextBox TextBoxAlaYtdBud = (TextBox)FindControlRecursive(FormViewEOMChecklist, "TextBoxAlaYtdBud");

            if (TextBoxFoodCostActualM != null && TextBoxFoodCostActualY != null && TextBoxPaperCleanActualM != null && TextBoxPaperCleanActualY != null &&
                LabelMeals != null && LabelNumDays != null && LabelFoodPaperClean != null && LabelTotalTargetPurchInv != null &&
                TextBoxTNGHoursActual != null && TextBoxDistHoursActual != null && TextBoxTNGWagesActual != null && TextBoxDistWagesActual != null && TextBoxUSDAActual != null &&
                TextBoxPLCurrent != null && TextBoxPLYtd != null && TextBoxUSDAYTD != null && TextBoxPurchAmount != null &&
                TextBoxAdjustments != null && LabelCommentRequired != null && HFSFA_PR!=null && TextBoxBrMonth!=null && TextBoxLnMonth!=null && TextBoxAlaMonth!=null && 
                TextBoxBrYtd!=null && TextBoxLnYtd!=null && TextBoxAlaYtd!=null)
            {
                SqlDataOperating.DataBind();
                DateTime startDate = new DateTime(int.Parse(TextBoxFYearmonth.Text.Substring(0, 4)), int.Parse(TextBoxFYearmonth.Text.Substring(5, 2)), 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                SqlDataOperating.SelectParameters["StartingDate"].DefaultValue = startDate.ToShortDateString();
                SqlDataOperating.SelectParameters["EndingDate"].DefaultValue = endDate.ToShortDateString();


                SqlDataReader rdr = (SqlDataReader)SqlDataOperating.Select(DataSourceSelectArguments.Empty);
                if (rdr != null && rdr.Read())
                {
                    decimal.TryParse(rdr["TotalFoodUsage"].ToString(), out decimal decTtlFoodUsage);
                    decimal.TryParse(rdr["TotalFoodUsageYtd"].ToString(), out decimal decTtlFoodUsageYtd);

                    decimal.TryParse(rdr["PaperUsage"].ToString(), out decimal decPaperUsage);
                    decimal.TryParse(rdr["PaperUsageYtd"].ToString(), out decimal decPaperUsageYtd);

                    decimal.TryParse(rdr["CleaningUsage"].ToString(), out decimal decCleaningUsage);
                    decimal.TryParse(rdr["CleaningUsageYtd"].ToString(), out decimal decCleaningUsageYtd);

                    decimal.TryParse(rdr["TotalMeals"].ToString(), out decTtlMeals);
                    decimal.TryParse(rdr["TotalMealsYtd"].ToString(), out decTtlMealsYtd);

                    decimal.TryParse(rdr["PaidLunch"].ToString(), out decimal decPaidLunch);
                    decimal.TryParse(rdr["ReducedLunch"].ToString(), out decimal decReducedLunch);
                    decimal.TryParse(rdr["FreeLunch"].ToString(), out decimal decFreeLunch);
                    decTtlLunches = decPaidLunch + decReducedLunch + decFreeLunch;

                    decimal.TryParse(rdr["PaidLunchYtd"].ToString(), out decimal decPaidLunchYtd);
                    decimal.TryParse(rdr["ReducedLunchYtd"].ToString(), out decimal decReducedLunchYtd);
                    decimal.TryParse(rdr["FreeLunchYtd"].ToString(), out decimal decFreeLunchYtd);
                    decTtlLunchesYtd = decPaidLunchYtd + decReducedLunchYtd + decFreeLunchYtd;

                    //Food Cost / PaperClean Cost
                    TextBoxFoodCostActualM.Text = (decTtlMeals == 0M ? 0M : (decTtlFoodUsage / decTtlMeals)).ToString("F3");
                    TextBoxPaperCleanActualM.Text = (decTtlMeals == 0M ? 0M : ((decPaperUsage + decCleaningUsage) / decTtlMeals)).ToString("F3");

                    TextBoxFoodCostActualY.Text = (decTtlMealsYtd == 0M ? 0M : (decTtlFoodUsageYtd / decTtlMealsYtd)).ToString("F3");
                    TextBoxPaperCleanActualY.Text = (decTtlMealsYtd == 0M ? 0M : ((decPaperUsageYtd + decCleaningUsageYtd) / decTtlMealsYtd)).ToString("F3");

                    decimal.TryParse(rdr["LunchDays"].ToString(), out decLunchDays);
                    decimal.TryParse(rdr["LunchDaysYtd"].ToString(), out decLunchDaysYtd);

                    LabelMeals.Text = Decimal.Round(decTtlMeals, 0).ToString();
                    LabelNumDays.Text = decLunchDays.ToString();

                    //Purchased inventory section
                    decimal TotalTargetPurchInv = 0M;
                    decimal TotalFoodPprClnCPM = 0M;
                    if (decTtlMeals != 0M)
                    {
                        TotalFoodPprClnCPM = (decTtlFoodUsage + decPaperUsage + decCleaningUsage) / decTtlMeals;
                    }
                    LabelFoodPaperClean.Text = TotalFoodPprClnCPM.ToString("N3");

                    if (decLunchDays != 0M)
                    {
                        TotalTargetPurchInv = (decTtlMeals / decLunchDays) * (TotalFoodPprClnCPM) * 8;
                    }
                    LabelTotalTargetPurchInv.Text = TotalTargetPurchInv.ToString("N3");

                    decimal.TryParse(rdr["InvGrocClose"].ToString(), out decimal PurchGroc);
                    decimal.TryParse(rdr["InvPaperClose"].ToString(), out decimal PurchPaper);
                    decimal.TryParse(rdr["InvMilkClose"].ToString(), out decimal PurchMilk);
                    decimal.TryParse(rdr["InvCleaningClose"].ToString(), out decimal PurchClean);
                    //decimal.TryParse(rdr["OtherPurchases"].ToString(), out decimal PurchOther);

                    TextBoxPurchAmount.Text = (PurchGroc + PurchPaper + PurchMilk + PurchClean).ToString("N2");

                    //Labor Section
                    decimal.TryParse(rdr["TNGHours"].ToString(), out decimal decTNGHours);
                    decimal.TryParse(rdr["DistTotalHours"].ToString(), out decimal decDistHours);
                    TextBoxTNGHoursActual.Text = decTNGHours.ToString("N2");
                    TextBoxDistHoursActual.Text = decDistHours.ToString("N2");

                    decimal.TryParse(rdr["DistPay"].ToString(), out decimal decDistPay);
                    decimal.TryParse(rdr["DistOtherPay"].ToString(), out decimal decDistOtherPay);
                    decimal.TryParse(rdr["DistHealth"].ToString(), out decimal decDistHealth);

                    //New Sep 2023 - added Dist Pay Rel override
                    decimal.TryParse(rdr["DistPayRelPct"].ToString(), out decimal decDistPayRel);
                    HFSFA_PR.Value = decDistPayRel.ToString();

                    decimal DistWages = decDistPay + decDistOtherPay + decDistHealth + ((decDistPay + decDistOtherPay) * decDistPayRel);
                    TextBoxDistWagesActual.Text = DistWages.ToString("N2");

                    decimal.TryParse(rdr["TNGGross"].ToString(), out decimal decTNGGross);
                    decimal.TryParse(rdr["TNGFica"].ToString(), out decimal decTNGFica);
                    decimal.TryParse(rdr["TNGStateUC"].ToString(), out decimal decTNGStateUC);
                    decimal.TryParse(rdr["TNGFUTA"].ToString(), out decimal decTNGFUTA);
                    decimal.TryParse(rdr["TNGWComp"].ToString(), out decimal decTNGWComp);
                    decimal.TryParse(rdr["TNGPayRel"].ToString(), out decimal decTNGPayRel);
                    decimal.TryParse(rdr["TNGOHSersAmt"].ToString(), out decimal decTNGOHSersAmt);
                    decimal.TryParse(rdr["TNGUniforms"].ToString(), out decimal decTNGUniforms);
                    decimal.TryParse(rdr["TNGPayAccrual"].ToString(), out decimal decTNGPayAccrual);
                    decimal.TryParse(rdr["TNGTaxAccrual"].ToString(), out decimal decTNGTaxAccrual);
                    decimal.TryParse(rdr["TNGHealth"].ToString(), out decimal decTNGHealth);
                    TextBoxTNGWagesActual.Text = (decTNGGross + decTNGFica + decTNGStateUC + decTNGFUTA + decTNGWComp + decTNGPayRel + decTNGOHSersAmt +
                        decTNGUniforms + decTNGPayAccrual + decTNGTaxAccrual + decTNGHealth).ToString("N2");

                    decimal.TryParse(rdr["TotalUSDAUsage"].ToString(), out decimal decActualUSDAUsage);
                    decimal.TryParse(rdr["TotalUSDAUsageYtd"].ToString(), out decimal decActualUSDAUsageYtd);
                    //if (decTtlMeals != 0M)
                    //    TextBoxUSDAActual.Text = (decActualUSDAUsage/decTtlMeals).ToString("F3");
                    if (decTtlLunches != 0M)
                        TextBoxUSDAActual.Text = (decActualUSDAUsage / decTtlLunches).ToString("F3");
                    if (decTtlLunchesYtd != 0M)
                        TextBoxUSDAYTD.Text = (decActualUSDAUsageYtd / decTtlLunchesYtd).ToString("F3");

                    decimal.TryParse(rdr["TotalDifference"].ToString(), out decimal decDiff);
                    decimal.TryParse(rdr["TotalDifferenceYtd"].ToString(), out decimal decDiffYtd);
                    if (decDiff != 0M)
                        TextBoxPLCurrent.Text = decDiff.ToString("N2");
                    if (decDiffYtd != 0M)
                        TextBoxPLYtd.Text = decDiffYtd.ToString("N2");

                    //Sales Adjustments

                    if (TextBoxAdjustments != null && LabelCommentRequired != null)
                    {
                        TextBoxAdjustments.Text = Decimal.Round(((Decimal)rdr["TotalAdjusts"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                        if (Decimal.Round(((Decimal)rdr["TotalAdjusts"]), 2, MidpointRounding.AwayFromZero) == 0M)
                            LabelCommentRequired.Visible = false;
                        else
                            LabelCommentRequired.Visible = true;


                    }

                    //Participation

                    decimal.TryParse(rdr["PaidBkft"].ToString(), out decimal decPaidBkft);
                    decimal.TryParse(rdr["ReducedBkft"].ToString(), out decimal decReducedBkft);
                    decimal.TryParse(rdr["FreeBkft"].ToString(), out decimal decFreeBkft);
                    decTtlBreakfasts = decPaidBkft + decReducedBkft + decFreeBkft;

                    decimal.TryParse(rdr["PaidBkftYtd"].ToString(), out decimal decPaidBkftYtd);
                    decimal.TryParse(rdr["ReducedBkftYtd"].ToString(), out decimal decReducedBkftYtd);
                    decimal.TryParse(rdr["FreeBkftYtd"].ToString(), out decimal decFreeBkftYtd);
                    decTtlBreakfastsYtd = decPaidBkftYtd + decReducedBkftYtd + decFreeBkftYtd;

                    decimal.TryParse(rdr["Ala"].ToString(), out decimal decAla);
                    decimal.TryParse(rdr["AlaYtd"].ToString(), out decimal decAlaYtd);

                    decimal.TryParse(rdr["BreakfastDays"].ToString(), out decBreakfastDays);
                    decimal.TryParse(rdr["BreakfastDaysYtd"].ToString(), out decBreakfastDaysYtd);


                    TextBoxBrMonth.Text = decTtlBreakfasts.ToString("F0");
                    TextBoxLnMonth.Text = decTtlLunches.ToString("F0");
                    TextBoxAlaMonth.Text = decAla.ToString("F0");

                    TextBoxBrYtd.Text = decTtlBreakfastsYtd.ToString("F0");
                    TextBoxLnYtd.Text = decTtlLunchesYtd.ToString("F0");
                    TextBoxAlaYtd.Text = decAlaYtd.ToString("F0");

                }
                else
                {
                    TextBoxFoodCostActualM.Text = "0.000";
                    TextBoxFoodCostActualY.Text = "0.000";
                    TextBoxPaperCleanActualM.Text = "0.000";
                    TextBoxPaperCleanActualY.Text = "0.000";

                    LabelMeals.Text = "0";
                    LabelNumDays.Text = "0";
                    LabelFoodPaperClean.Text = "0.000";
                    LabelTotalTargetPurchInv.Text = "0.000";

                    TextBoxTNGHoursActual.Text = "0.00";
                    TextBoxDistHoursActual.Text = "0.00";
                    TextBoxDistWagesActual.Text = "0.00";
                    TextBoxTNGWagesActual.Text = "0.00";

                    TextBoxPLCurrent.Text = "0.00";
                    TextBoxPLYtd.Text = "0.00";
                    TextBoxPurchAmount.Text = "0.00";

                    //Sales Adjustments
                    TextBoxAdjustments.Text = "0.00";
                    LabelCommentRequired.Visible = false;

                    HFSFA_PR.Value = "0";
                }
            }

            //BDGT fields round 2
            if (TextBoxTNGHoursBdgt != null && TextBoxTNGWagesBdgt != null && TextBoxDistHoursBdgt != null && TextBoxDistWagesBdgt != null &&
                TextBoxDaysMonth != null && TextBoxDaysYtd != null && TextBoxBudgetPeriod != null && TextBoxBudgetYtd != null &&
                TextBoxBrYtdBud != null && TextBoxLnYtdBud != null && TextBoxAlaYtdBud != null &&
                TextBoxBrMonthBud != null && TextBoxLnMonthBud != null && TextBoxAlaMonthBud != null)
            {
                TextBoxTNGHoursBdgt.Text = ((decNutrProdHours + decNutrNonProdHours + decNutrSpFnHours) * decLunchDays).ToString("N2");
                TextBoxDistHoursBdgt.Text = ((decDistProdHours + decDistNonProdHours + decDistSpFnHours) * decLunchDays).ToString("N2");
                TextBoxTNGWagesBdgt.Text = (decNUTRLaborDay * decLunchDays).ToString("N2");
                TextBoxDistWagesBdgt.Text = (decDistLaborDay * decLunchDays).ToString("N2");

                TextBoxDaysMonth.Text = decLunchDays.ToString(("F0"));
                TextBoxDaysYtd.Text = decLunchDaysYtd.ToString(("F0"));

                TextBoxBudgetPeriod.Text = (decDailyPL * decLunchDays).ToString("N2");
                TextBoxBudgetYtd.Text = (decDailyPL * decLunchDaysYtd).ToString("N2");


                TextBoxBrMonthBud.Text = (decBrkDailyCountBud * decBreakfastDays).ToString("F0");
                TextBoxLnMonthBud.Text = (decLunDailyCountBud * decLunchDays).ToString("F0");
                TextBoxAlaMonthBud.Text = (decAlaDailyBud * decLunchDays).ToString("F0");

                TextBoxBrYtdBud.Text = (decBrkDailyCountBud * decBreakfastDaysYtd).ToString("F0");
                TextBoxLnYtdBud.Text = (decLunDailyCountBud * decLunchDaysYtd).ToString("F0");
                TextBoxAlaYtdBud.Text = (decAlaDailyBud * decLunchDaysYtd).ToString("F0");


            }

            HiddenField HFBatchEmailed = (HiddenField)FindControlRecursive(FormViewEOMChecklist, "HFBatchEmailed");
            HiddenField HFBatchEmailedDateTime = (HiddenField)FindControlRecursive(FormViewEOMChecklist, "HFBatchEmailedDateTime");
            HiddenField HFSourceRMEmail = (HiddenField)FindControlRecursive(FormViewEOMChecklist, "HFSourceRMEmail");
            HiddenField HFSourceRMName = (HiddenField)FindControlRecursive(FormViewEOMChecklist, "HFSourceRMName");

            if (HFBatchEmailed != null && HFBatchEmailedDateTime != null && HFSourceRMEmail != null && HFSourceRMName != null)
            {
                HFRMEmail.Value = HFSourceRMEmail.Value;
                LabelRM.Text = "RM: " + HFSourceRMName.Value;

                if (HFBatchEmailed.Value.ToLower() == "true")
                {
                    LabelReportsSent.Text = "Report Batch sent " + HFBatchEmailedDateTime.Value;
                    LabelReportsSent.Visible = true;
                }
                else
                {
                    LabelReportsSent.Visible = false;
                }

            }

        }

        decimal sumSnapshotTotal = 0M;

        protected void GridViewSchoolRecords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow gvr = (GridViewRow)e.Row;

            if(e.Row.RowType==DataControlRowType.Header)
            {
                sumSnapshotTotal = 0M;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                TextBox TextBoxSnapshotDate = (TextBox)FindControlRecursive(gvr, "TextBoxSnapshotDate");
                if (TextBoxSnapshotDate != null && TextBoxSnapshotDate.Text == "1/1/1900")
                    TextBoxSnapshotDate.Text = string.Empty;

                TextBox TextBoxSchoolInventoryDate = (TextBox)FindControlRecursive(gvr, "TextBoxSchoolInventoryDate");
                if (TextBoxSchoolInventoryDate != null && TextBoxSchoolInventoryDate.Text == "1/1/1900")
                    TextBoxSchoolInventoryDate.Text = string.Empty;

                TextBox TextBoxSnapshotTotal = (TextBox)e.Row.FindControl("TextBoxSnapshotTotal");
                if(TextBoxSnapshotTotal != null && !string.IsNullOrEmpty(TextBoxSnapshotTotal.Text))
                {
                    if (decimal.TryParse(TextBoxSnapshotTotal.Text, out decimal decCurrTotal) == false)
                        decCurrTotal = 0M;
                    sumSnapshotTotal += decCurrTotal;
                }
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                TextBox TextBoxSnapshotTotalFooter = (TextBox)e.Row.FindControl("TextBoxSnapshotTotalFooter");
                if(TextBoxSnapshotTotalFooter != null )
                {
                    TextBoxSnapshotTotalFooter.Text = sumSnapshotTotal.ToString("F2");
                }
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string emailServer = "";

            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view = (DataView)SqlDataSourceEmailSettings.Select(args);
            DataTable table = view.ToTable();

            emailServer = table.Rows[0]["emailServer"].ToString();
            Boolean emailUseEWS = table.Rows[0]["emailUseEWS"].ToString() == "0" ? false : true;
            Boolean emailUseSSL = table.Rows[0]["emailUseSSL"].ToString() == "0" ? false : true;
            string emailFrom = table.Rows[0]["emailFromAddress"].ToString();
            //string emailTo = table.Rows[0]["NewVendorRequests"].ToString();

            HiddenField HFSourceRMEmail = (HiddenField)FindControlRecursive(FormViewEOMChecklist, "HFSourceRMEmail");
            if (HFSourceRMEmail == null)
            {
                ShowDialog("Error", MessageType.Error, "Could not find the email address field. Email cancelled.", "");
                return;
            }
            if (HFSourceRMEmail.Value == "")
            {
                ShowDialog("No Email", MessageType.Warning, "No email address is setup for the RM. Email cancelled.", "");
                return;
            }

            string emailTo = HFSourceRMEmail.Value;
            int emailPort = (int)table.Rows[0]["emailPort"];
            string emailDomain = table.Rows[0]["emailAuthDomain"].ToString();
            string emailUsername = table.Rows[0]["emailAuthUser"].ToString();
            string emailPassword = table.Rows[0]["emailAuthPassword"].ToString();

            string subject = "Paperwork Web - EOM Reports";

            //Email body
            StringBuilder sb = new StringBuilder();
            sb.Append(" * *Automated email from the Paperwork Web EOM Checklist form. Do not reply to this email.* *\r\n");
            sb.Append("\r\n");
            sb.Append("Attached are the EOM reports for district: "); sb.Append(DropDownListDistricts.SelectedItem.Text); sb.Append(".\r\n");
            sb.Append("EOM report batch requested by user:  "); sb.Append(Session["CurrentUser"].ToString()); sb.Append("\r\n");

            string content = sb.ToString();

            if (emailUseEWS)
            {
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
                string uriPrefix = "";
                string ewsString = "";
                if (emailUseSSL)
                    uriPrefix = @"https://";
                else
                    uriPrefix = @"http://";

                if (emailUseEWS)
                    ewsString = @"/EWS/Exchange.asmx";

                Uri serverURI = new Uri(@uriPrefix + @emailServer + @ewsString);
                service.Url = serverURI;

                if (service.Url.ToString() != String.Empty)
                {
                    if (emailUsername == String.Empty)
                        service.UseDefaultCredentials = true;
                    else if (emailDomain == String.Empty)
                    {
                        ExchangeCredentials ec = new NetworkCredential(emailUsername, emailPassword);
                        service.Credentials = ec;
                    }
                    else
                    {
                        ExchangeCredentials ec = new NetworkCredential(emailUsername, emailPassword, emailDomain);
                        service.Credentials = ec;
                    }

                    EmailMessage message = new EmailMessage(service);

                    String[] addresses;

                    addresses = emailTo.Split(',');

                    foreach (string address in addresses)
                    {
                        EmailAddress To = new EmailAddress(address);
                        message.ToRecipients.Add(To);
                    }
                    message.Subject = subject;
                    message.Importance = Importance.High;
                    MessageBody body = new MessageBody();

                    body.BodyType = BodyType.Text;

                    body.Text += content;

                    message.Body = body;
                    message.From = new EmailAddress(emailFrom);

                    //Collection of memorystreams which will hold the attachments
                    var msList = new List<MemoryStream>();
                    var bwList = new List<BinaryWriter>();

                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string filenameExtension;
                    string reportName;

                    //Nutrition is not using EWS, but this might work to add attachments is switch to down the road.
                    //if (CBSalesSchool.Checked)
                    //{
                    //    reportName = "SalesTotals";
                    //    ReportViewer1.ServerReport.ReportPath = HFSSRSBaseFolder.Value + reportName;
                    //    ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
                    //    ReportParameter paramDistID = new ReportParameter("DistrictID", DropDownListDistricts.SelectedValue);
                    //    DateTime startDate = new DateTime(int.Parse(TextBoxFYearmonth.Text.Substring(0, 4)), int.Parse(TextBoxFYearmonth.Text.Substring(5, 2)), 1);
                    //    DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                    //    ReportParameter paramStartDate = new ReportParameter("RecapDate", startDate.ToShortDateString());
                    //    ReportParameter paramEndDate = new ReportParameter("RecapDate2", endDate.ToShortDateString());
                    //    ReportParameter paramRevisions = new ReportParameter("ShowRevisions", "true");//report runs with this checked by default
                    //    ReportParameter paramComments = new ReportParameter("ShowComments", "true");//report runs with this checked by default
                    //    try
                    //    {
                    //        ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramStartDate, paramEndDate, paramRevisions, paramComments });
                    //        ReportViewer1.ServerReport.Refresh();
                    //    }
                    //    catch { }


                    //    byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    //    MemoryStream ms1 = new System.IO.MemoryStream(bytes);
                    //    msList.Add(ms1);
                    //    BinaryWriter bw1 = new BinaryWriter(ms1);
                    //    bwList.Add(bw1);
                    //    bw1.Write(bytes);
                    //    bw1.Flush();

                    //    ms1.Position = 0;

                    //    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                    //    //System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms1, ct);
                    //    //attach.ContentDisposition.FileName = reportName + ".pdf";

                    //    message.Attachments.AddFileAttachment(reportName, ms1);

                    //}

                    try
                    {
                        FolderId draftID = new FolderId(WellKnownFolderName.Drafts, emailFrom);
                        //message.SendAndSaveCopy(sentID);
                        message.Save(draftID);
                        message.Send();
                    }
                    catch (Microsoft.Exchange.WebServices.Data.ServiceRequestException ex)
                    {
                        ShowDialog("Error", MessageType.Error, "Exchange Webservices Error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                    finally
                    {
                        //Session["VendorRequestMsg"] = "Vendor request successfully sent.";
                        //Response.Redirect("~/Setup/SetupDistrictVendors");
                    }
                }//service URL is not empty
            }//using EWS
            else
            {
                MailAddress Fm = new MailAddress(emailFrom, emailFrom);
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = Fm;

                String[] addresses;
                addresses = emailTo.Split(',');
                foreach (string address in addresses)
                {
                    MailAddress To = new MailAddress(address);
                    mailMessage.To.Add(To);
                }
                mailMessage.Subject = subject;
                mailMessage.Priority = System.Net.Mail.MailPriority.Normal;
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = content;

                SmtpClient client = new SmtpClient();
                client.Host = emailServer;
                client.Port = emailPort;

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 30000;
                client.EnableSsl = emailUseSSL;

                if (emailUsername == String.Empty && emailPassword == String.Empty)
                    client.UseDefaultCredentials = true;
                else if (emailDomain != String.Empty)
                {
                    client.UseDefaultCredentials = false;
                    NetworkCredential nc = new NetworkCredential(emailUsername, emailPassword, emailDomain);
                    client.Credentials = nc;
                }
                else
                {
                    client.UseDefaultCredentials = false;
                    NetworkCredential nc = new NetworkCredential(emailUsername, emailPassword);
                    client.Credentials = nc;
                }

                //Collection of memorystreams which will hold the attachments
                var msList = new List<MemoryStream>();
                var bwList = new List<BinaryWriter>();

                //Attachments
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                string reportName;

                if (CBEOMChecklist.Checked)
                {
                    reportName = "EOM Checklist";
                    ReportViewer1.ServerReport.ReportPath = HFSSRSBaseFolder.Value + reportName;
                    ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
                    ReportParameter paramDistID = new ReportParameter("DistrictID", DropDownListDistricts.SelectedValue);
                    ReportParameter paramyrMnth = new ReportParameter("YearMonth", TextBoxFYearmonth.Text);

                    try
                    {
                        ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramyrMnth });
                        ReportViewer1.ServerReport.Refresh();
                    }
                    catch { }


                    byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    MemoryStream ms1 = new System.IO.MemoryStream(bytes);
                    msList.Add(ms1);
                    BinaryWriter bw1 = new BinaryWriter(ms1);
                    bwList.Add(bw1);
                    bw1.Write(bytes);
                    bw1.Flush();

                    ms1.Position = 0;

                    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                    System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms1, ct);
                    attach.ContentDisposition.FileName = reportName + ".pdf";

                    mailMessage.Attachments.Add(attach);
                }


                if (CBOperatingReport.Checked)
                {
                    reportName = "Operating Report";
                    ReportViewer1.ServerReport.ReportPath = HFSSRSBaseFolder.Value + reportName;
                    ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
                    ReportParameter paramDistID = new ReportParameter("DistrictID", DropDownListDistricts.SelectedValue);
                    DateTime startDate = new DateTime(int.Parse(TextBoxFYearmonth.Text.Substring(0, 4)), int.Parse(TextBoxFYearmonth.Text.Substring(5, 2)), 1);
                    DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                    ReportParameter paramStartDate = new ReportParameter("StartingDate", startDate.ToShortDateString());
                    ReportParameter paramEndDate = new ReportParameter("EndingDate", endDate.ToShortDateString());
                    try
                    {
                        ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramStartDate, paramEndDate });
                        ReportViewer1.ServerReport.Refresh();
                    }
                    catch { }


                    byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    MemoryStream ms1 = new System.IO.MemoryStream(bytes);
                    msList.Add(ms1);
                    BinaryWriter bw1 = new BinaryWriter(ms1);
                    bwList.Add(bw1);
                    bw1.Write(bytes);
                    bw1.Flush();

                    ms1.Position = 0;

                    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                    System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms1, ct);
                    attach.ContentDisposition.FileName = reportName + ".pdf";

                    mailMessage.Attachments.Add(attach);

                }

                try
                {
                    //Check to see if anything was marked and attached
                    if (msList.Count > 0)
                    {
                        client.Send(mailMessage);

                        //Mail sent - update record as sent
                        setEOMBatchSent();
                    }

                    //Update the Formview - which updates the report sent label at the bottom
                    FormViewEOMChecklist.DataBind();

                }
                catch (SmtpFailedRecipientsException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SMTP Failed Recipients Error occurred.", ex.Message);
                }
                catch (SmtpException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SMTP Error occurred.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An Error occurred.", ex.Message);
                }
                finally
                {
                    foreach (var bw in bwList)
                    {
                        bw.Dispose();
                    }
                    foreach (var ms in msList)
                    {
                        ms.Dispose();
                    }


                    //if(bw != null)
                    //    bw.Dispose();
                    //if (ms != null)
                    //{
                    //    ms.Close();
                    //    ms.Dispose();
                    //}

                    //Session["VendorRequestMsg"] = "Vendor request successfully sent.";
                    //Response.Redirect("~/Setup/SetupDistrictVendors");
                    if (mailMessage != null)
                        mailMessage.Dispose();

                    if (client != null)
                        client.Dispose();
                }

            }

        }


        protected void setEOMBatchSent()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "UPDATE [EOMChecklist] SET LastEditedUser=@LastEditedUser, LastEditedDate=GETDATE(), EOMReportBatchEmailed=1, EOMReportBatchDateTime=GETDATE()" +
                " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth";

            using (IDbCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYr = new SqlParameter();
                dbParam_progYr.ParameterName = "@ProgramYear";
                dbParam_progYr.Value = DDLProgramYears.SelectedValue;
                dbParam_progYr.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_progYr);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DropDownListDistricts.SelectedValue;
                dbParam_distID.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_YrMonth = new SqlParameter();
                dbParam_YrMonth.ParameterName = "@YearMonth";
                dbParam_YrMonth.Value = TextBoxFYearmonth.Text;
                dbParam_YrMonth.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_YrMonth);

                System.Data.IDataParameter dbParam_uSERID = new System.Data.SqlClient.SqlParameter();
                dbParam_uSERID.ParameterName = "@LastEditedUser";
                dbParam_uSERID.Value = Context.User.Identity.Name.ToUpper();
                dbParam_uSERID.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_uSERID);

                dbConnection.Open();
                try
                {
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    //Silent Catch - not a critical procedure
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void GridViewSchoolRecords_PreRender(object sender, EventArgs e)
        {
            //Prior to rendering - reset any saved state to unsaved to allow additional changes to the current row
            GridView GridViewSchoolRecords = (GridView)sender;
            foreach (GridViewRow dataRow in GridViewSchoolRecords.Rows)
            {
                HiddenField HFRowSaved = (HiddenField)FindControlRecursive(dataRow, "HFRowSaved");
                if (HFRowSaved == null)
                    return;

                HFRowSaved.Value = "0";

            }
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            //Create empty record if needed to allow printing of blank sheet
            SaveChecklist();

            Session["CurrentYearMonth"] = TextBoxFYearmonth.Text;

            Response.Redirect("~/Print/PrintEOMChecklist.aspx");
        }
    }
}
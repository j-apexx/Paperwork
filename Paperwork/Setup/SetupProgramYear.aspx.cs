using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupProgramYear : Paperwork_Global
    {
        private string InsertStatusString = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            switch (Session["AccessLevel"].ToString().ToUpper())
            {
                case "ADMIN":
                    break;
                default:
                    //Denied
                    Response.Redirect("~/Default.aspx");
                    break;
            }

            //Check for custom security
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            bool accessExplicitlyGranted = false;
            bool accessExplicitlyDenied = false;
            CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

            if (accessExplicitlyDenied == true)
                Response.Redirect("~/Default.aspx");
        }

        protected void CheckBoxEntryAllowed_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);
            string rowKey = gvr.Cells[0].Text;

            //Updates all other program years
            //Effective 6/25/2021, Tom needed the capability to allow more than 1 year open, especially during the transition period
            if (((CheckBox)sender).Checked == true)
            {
                //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

                //string queryRemoveOtherActives = "UPDATE PROGRAMYEAR SET AllowDataEntry=0 WHERE UPPER(ProgramYear) != @ProgramYear ";

                //using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
                //{
                //    dbConnection.Open();
                //    SqlCommand cmdUpdateProgramYear = new SqlCommand(queryRemoveOtherActives, dbConnection);

                //    SqlParameter dbParam_pRgYear = new SqlParameter();
                //    dbParam_pRgYear.ParameterName = "@ProgramYear";
                //    dbParam_pRgYear.Value = rowKey;
                //    dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                //    cmdUpdateProgramYear.Parameters.Add(dbParam_pRgYear);

                //    cmdUpdateProgramYear.ExecuteNonQuery();
                //    cmdUpdateProgramYear.Dispose();
                //}
            }

            //Now update the individual row
            string querySetActive = "UPDATE PROGRAMYEAR SET AllowDataEntry=" + checkFieldStr + " WHERE UPPER(ProgramYear) = @ProgramYear ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                SqlCommand cmdUpdateProgramYear = new SqlCommand(querySetActive, dbConnection);

                SqlParameter dbParam_pRgYear = new SqlParameter();
                dbParam_pRgYear.ParameterName = "@ProgramYear";
                dbParam_pRgYear.Value = rowKey;
                dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                cmdUpdateProgramYear.Parameters.Add(dbParam_pRgYear);

                cmdUpdateProgramYear.ExecuteNonQuery();
                cmdUpdateProgramYear.Dispose();
            }

            if(((CheckBox)sender).Checked)
                CreateLogEntry("Allow Data Entry Clicked for ProgramYear " + rowKey);
            else
                CreateLogEntry("Allow Data Entry Unclicked for ProgramYear " + rowKey);

            ((GridView)gvr.Parent.Parent).DataBind();
        }

        /// <summary>
        /// Updates the month closed checkbox records.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CheckBoxEntryMonthClosed_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);
            string rowKey = gvr.Cells[0].Text;
            string startDateStr = gvr.Cells[1].Text;
            string endDateStr = gvr.Cells[2].Text;
            string columnName = ((CheckBox)sender).ID;

            DateTime pyStartDate = DateTime.Parse(startDateStr);
            DateTime pyEndDate = DateTime.Parse(endDateStr);

            //Now update the individual row
            string querySetActive = "UPDATE PROGRAMYEAR SET " + columnName + "=" + checkFieldStr + " WHERE UPPER(ProgramYear) = @ProgramYear ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                using (SqlCommand cmdUpdatePrgYear = new SqlCommand(querySetActive, dbConnection))
                {
                    SqlParameter dbParam_pRgYear = new SqlParameter();
                    dbParam_pRgYear.ParameterName = "@ProgramYear";
                    dbParam_pRgYear.Value = rowKey;
                    dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmdUpdatePrgYear.Parameters.Add(dbParam_pRgYear);

                    int recordCount = 0;
                    recordCount = cmdUpdatePrgYear.ExecuteNonQuery();
                    cmdUpdatePrgYear.Dispose();

                    if (recordCount > 0)
                    {

                        int monthColumnIndex = 0;
                        string monthString = "";
                        switch (columnName)
                        {
                            case "Close01":
                                monthColumnIndex = 6;
                                monthString = "January";
                                break;
                            case "Close02":
                                monthColumnIndex = 7;
                                monthString = "February";
                                break;
                            case "Close03":
                                monthColumnIndex = 8;
                                monthString = "March";
                                break;
                            case "Close04":
                                monthColumnIndex = 9;
                                monthString = "April";
                                break;
                            case "Close05":
                                monthColumnIndex = 10;
                                monthString = "May";
                                break;
                            case "Close06":
                                monthColumnIndex = 11;
                                monthString = "June";
                                break;
                            case "Close07":
                                monthColumnIndex = 0;
                                monthString = "July";
                                break;
                            case "Close08":
                                monthColumnIndex = 1;
                                monthString = "August";
                                break;
                            case "Close09":
                                monthColumnIndex = 2;
                                monthString = "September";
                                break;
                            case "Close10":
                                monthColumnIndex = 3;
                                monthString = "October";
                                break;
                            case "Close11":
                                monthColumnIndex = 4;
                                monthString = "November";
                                break;
                            case "Close12":
                                monthColumnIndex = 5;
                                monthString = "December";
                                break;
                            default:
                                break;
                        }

                        if (((CheckBox)sender).Checked)
                            CreateLogEntry(monthString+" Closed-ProgramYear " + rowKey);
                        else
                            CreateLogEntry(monthString+" Opened-ProgramYear " + rowKey);

                        //**The following was too basic - need to see earliest or latest month closed and update cutoffs based on opening or closing
                        //Based on column index - add necessary months to the Program Year start date and set to first of month
                        DateTime StartDateRange = new DateTime(pyStartDate.AddMonths(monthColumnIndex).Year, pyStartDate.AddMonths(monthColumnIndex).Month, 1);
                        //End range is set to the last date of the month sepcified by start date range
                        DateTime EndDateRange = StartDateRange.AddMonths(1).AddDays(-1);

                        int earliestClose = -1;
                        int earliestOpen = -1;

                        for (int i = 7; i <= 12; i++)
                        {
                            CheckBox cbMonth = (CheckBox)FindControlRecursive(gvr, "Close" + i.ToString().PadLeft(2, '0'));
                            if (cbMonth.Checked)
                            {
                                if (earliestClose == -1)
                                    earliestClose = i;
                            }
                            else
                            {
                                if (earliestOpen == -1)
                                    earliestOpen = i;
                            }
                        }
                        for (int i = 1; i <= 6; i++)
                        {
                            CheckBox cbMonth = (CheckBox)FindControlRecursive(gvr, "Close" + i.ToString().PadLeft(2, '0'));
                            if (cbMonth.Checked)
                            {
                                if (earliestClose == -1)
                                    earliestClose = i;
                            }
                            else
                            {
                                if (earliestOpen == -1)
                                    earliestOpen = i;
                            }
                        }

                        int indexAdjustedEOpen = 0;
                        if (earliestOpen == -1)
                            indexAdjustedEOpen = 0;
                        else if (earliestOpen >= 7)
                            indexAdjustedEOpen = earliestOpen - 7;
                        else
                            indexAdjustedEOpen = earliestOpen + 5;

                        //If we are closing a month check to see that all previous months are closed
                        if (((CheckBox)sender).Checked)
                        {
                            //if there isn't an open
                            if (earliestOpen == -1)
                            {
                                StartDateRange = new DateTime(pyEndDate.Year, pyEndDate.Month, 1);
                                EndDateRange = pyEndDate;
                            }
                            else
                            {
                                //Closing sets cutoff to end if date range. So adjusted range needs to be the prior month
                                indexAdjustedEOpen--;

                                //Based on column index - add necessary months to the Program Year start date and set to first of month
                                StartDateRange = new DateTime(pyStartDate.AddMonths(indexAdjustedEOpen).Year, pyStartDate.AddMonths(indexAdjustedEOpen).Month, 1);
                                //End range is set to the last date of the month sepcified by start date range
                                EndDateRange = StartDateRange.AddMonths(1).AddDays(-1);
                            }
                        }
                        //When opening a month, we need to set the cutoff date prior to the 1st of the opened month
                        else
                        {
                            //Based on column index - add necessary months to the Program Year start date and set to first of month
                            StartDateRange = new DateTime(pyStartDate.AddMonths(indexAdjustedEOpen).Year, pyStartDate.AddMonths(indexAdjustedEOpen).Month, 1);
                            //End range is set to the last date of the month sepcified by start date range
                            EndDateRange = StartDateRange.AddMonths(1).AddDays(-1);
                        }


                        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;//, className, Context.User.Identity.Name.ToUpper()

                        //NEW April & May 2022 - cutoffs for transactions
                        UpdateTransactionCutoff(((CheckBox)sender).Checked, StartDateRange, EndDateRange, "Purchases", rowKey, "%", className, Context.User.Identity.Name.ToUpper());
                        UpdateTransactionCutoff(((CheckBox)sender).Checked, StartDateRange, EndDateRange, "SalesEntry", rowKey, "%", className, Context.User.Identity.Name.ToUpper());
                    }
                }
            }

            ((GridView)gvr.Parent.Parent).DataBind();
        }

        protected void DDLEditBudgetYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlBudYear = ((DropDownList)sender);
            GridViewRow gvr = ((GridViewRow)ddlBudYear.Parent/*Cell*/.Parent);
            string rowKey = gvr.Cells[0].Text;

            //Now update the individual row
            string querySetActive = "UPDATE PROGRAMYEAR SET BudgetYear=@BudgetYear WHERE UPPER(ProgramYear) = @ProgramYear ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdUpdatePrgYear = new SqlCommand(querySetActive, dbConnection))
                    {
                        SqlParameter dbParam_pRgYear = new SqlParameter();
                        dbParam_pRgYear.ParameterName = "@ProgramYear";
                        dbParam_pRgYear.Value = rowKey;
                        dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdUpdatePrgYear.Parameters.Add(dbParam_pRgYear);

                        SqlParameter dbParam_bdgtYear = new SqlParameter();
                        dbParam_bdgtYear.ParameterName = "@BudgetYear";
                        dbParam_bdgtYear.Value = ddlBudYear.SelectedValue;
                        dbParam_bdgtYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdUpdatePrgYear.Parameters.Add(dbParam_bdgtYear);

                        int recordCount=cmdUpdatePrgYear.ExecuteNonQuery();
                        if(recordCount > 0)
                            CreateLogEntry("Budget Year Changed to " + ddlBudYear.SelectedValue + " for ProgramYear " + rowKey);    
                    }
                }
                catch (Exception ex) { }

            }

            ((GridView)gvr.Parent.Parent).DataBind();
        }

        /// <summary>
        /// After the new program year is created - initialize the related records.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SqlDataSourceProgramYears_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            string NewProgramYear = e.Command.Parameters["@ProgramYear"].Value.ToString();
            DateTime StartDate = DateTime.Parse(e.Command.Parameters["@StartDate"].Value.ToString());
            DateTime EndDate = DateTime.Parse(e.Command.Parameters["@EndDate"].Value.ToString());
            string NewBudgetYear = e.Command.Parameters["@BudgetYear"].Value.ToString();
            string PrevProgramYear = RetrievePrevProgYear(StartDate);//Using Start Date of new year to find the program year which just ended

            if (e.AffectedRows > 0)
            {
                InsertStatusString = "<strong>New Program Year " + NewProgramYear + " was created.</strong>";

                if (InitializeTaxRates(NewProgramYear, StartDate, PrevProgramYear))
                    InsertStatusString += "<br>-New Tax Rates record was created.";

                //Import the global Purchase Categories
                if (InitGlobalPurchCats(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-Purchasing Other Categories were copied forward.";

                int recordcount = 0;

                if (NewBudgetYear != "")
                {
                    recordcount = ImportCategoryRecords(NewProgramYear, NewBudgetYear);
                    if (recordcount > 0)
                        InsertStatusString += "<br>-District Purchasing Other Categories were synchronized with Budgets.";
                }

                int recordcountNonBudgetDists = 0;
                recordcountNonBudgetDists = InitializeNonBudgetedDistCategoryRecords(NewProgramYear);
                if (recordcountNonBudgetDists > 0)
                    InsertStatusString += "<br>-District Purchasing Other Categories were initialized for non-budgeted districts.";

                //Nov 2024 - disabling the copy forward of the Milk Delivery Info. Sean wants them to set it up from scratch.
                //if (InitMilkDelivInfo(NewProgramYear, PrevProgramYear))
                //    InsertStatusString += "<br>-Milk Delivery Info records were copied forward.";

                if (InitDistPayCodes(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-District Paycode records were copied forward.";

                if (InitDistYearInfo(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-District Year Info records were copied forward.";

                if (InitDistYearContacts(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-District Year Contacts were copied forward.";

                if (InitDistSchoolYearSetup(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-District School Year Setups were copied forward.";

                if (InitOtherCostCats(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-Indirect Cost Categories were copied forward.";

                if(InitMenuCostItemsSetup(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-Menu Cost Analysis Items were copied forward.";

                if (NewBudgetYear != "" && isSFSP() == false)
                    if (InitializeAddOnFees(NewProgramYear, NewBudgetYear, StartDate, EndDate))
                    {
                        InsertStatusString += "<br>-AddOn Fees were created from Budget Totals.";
                    }

                //New July 2024 - Copy forward the Paperwork Other Settings
                if(InitPaperworkOtherSettings(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-Paperwork Other Settings were copied forward.";

                ShowDialog("Info", MessageType.Info, InsertStatusString, "");
            }
        }

        private bool InitPaperworkOtherSettings(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [PaperworkOtherSettings] ([ProgramYear],[SalesReportNonProgramRate]) " +
                " SELECT @NewProgYear,[SalesReportNonProgramRate] " +
                " FROM [PaperworkOtherSettings] WHERE ProgramYear=@PrevProgYear ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating Paperwork Other Settings records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating Paperwork Other Settings records. ", ex.Message);
                }

            }

            return RecordsCopied;

        }


        private bool InitGlobalPurchCats(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [OtherPurchaseCategories] ([ProgramYear],[OtherCategoryID],[OtherCategoryDesc],[OtherCatExpCode]) " +
                " SELECT @NewProgYear,[OtherCategoryID],[OtherCategoryDesc],[OtherCatExpCode] " +
                " FROM [OtherPurchaseCategories] WHERE ProgramYear=@PrevProgYear ";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while copying Purchase Other Cat. records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while copying Purchase Other Cat. records. ", ex.Message);
                }

            }

            return RecordsCopied;
        }

        private bool InitOtherCostCats(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [OtherSalesCats] ([ProgramYear],[DistrictID],[OtherSalesID],[OtherSalesDesc]) " +
                " SELECT @NewProgYear,[DistrictID],[OtherSalesID],[OtherSalesDesc] " +
                " FROM [OtherSalesCats] WHERE ProgramYear=@PrevProgYear AND DistrictID NOT IN (SELECT DistrictID FROM [District] WHERE INACTIVE=1) ";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating District Indirect Cost records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating District Indirect Cost records. ", ex.Message);
                }

            }

            return RecordsCopied;
        }

        private bool InitDistSchoolYearSetup(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [DistrictSchoolYearSetup] ([ProgramYear],[DistrictID],[SchoolNum],[BfastElPrice],[BfastJrPrice],[BfastSrPrice],[BfastRedPrice]," +
                "[LunchElPrice],[LunchJrPrice],[LunchSrPrice],[LunchTier1Price],[LunchTier2Price],[LunchRedPrice],[BfastPaidRate],[BfastRedRate],[BfastFreeRate],[LunchPaidRate]," +
                "[LunchRedRate],[LunchFreeRate],[OSnackPaidPrice],[OSnackRedPrice],[OSnackPaidRate],[OSnackRedRate],[OSnackFreeRate],[AF],[Enrollment],[EligiblePaid]," +
                "[EligibleFree],[EligibleReduced],[BreakfastIncent],[Breakfasts],[Tiers],[OtherSales],[Learners],[Snacks],[SchoolCEPFreePct],[SchoolCEPPaidPct],[PartialCEPPartic],[UseCustomCEPPct]," +
                "[Dinners],[DinnerRate],[AdultLunchPrice]) " +
                " SELECT @NewProgYear,[DistrictID],[SchoolNum],[BfastElPrice],[BfastJrPrice],[BfastSrPrice],[BfastRedPrice]," +
                " [LunchElPrice],[LunchJrPrice],[LunchSrPrice],[LunchTier1Price],[LunchTier2Price],[LunchRedPrice],[BfastPaidRate],[BfastRedRate],[BfastFreeRate],[LunchPaidRate]," +
                " [LunchRedRate],[LunchFreeRate],[OSnackPaidPrice],[OSnackRedPrice],[OSnackPaidRate],[OSnackRedRate],[OSnackFreeRate],[AF],[Enrollment],[EligiblePaid]," +
                " [EligibleFree],[EligibleReduced],[BreakfastIncent],[Breakfasts],[Tiers],[OtherSales],[Learners],[Snacks],[SchoolCEPFreePct],[SchoolCEPPaidPct],[PartialCEPPartic],[UseCustomCEPPct]," +
                " [Dinners],[DinnerRate],[AdultLunchPrice]" +
                " FROM [DistrictSchoolYearSetup] WHERE ProgramYear=@PrevProgYear AND DistrictID NOT IN (SELECT DistrictID FROM [District] WHERE INACTIVE=1) " +
                //AND only Active Schools
                " AND [DistrictID]+CAST([SchoolNum] AS VARCHAR(MAX)) IN (SELECT [DistrictID]+CAST([SchoolNum] AS VARCHAR(MAX)) FROM [dbo].[DistrictSchools] WHERE Inactive=0)";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating District School Year Setup records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating District School Year Setup records. ", ex.Message);
                }

            }

            return RecordsCopied;
        }

        private bool InitDistYearContacts(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [DistrictYearContacts] ([ProgramYear],[DistrictID],[Title],[FirstName],[LastName],[Email],[Position],[Note],[Building]," +
                "[Address],[City],[State],[Zip],[WorkPhone],[CellPhone],[DateAdded],[DateModified]) " +
                " SELECT @NewProgYear,[DistrictID],[Title],[FirstName],[LastName],[Email],[Position],[Note],[Building]," +
                " [Address],[City],[State],[Zip],[WorkPhone],[CellPhone],[DateAdded],[DateModified]" +
                " FROM [DistrictYearContacts] WHERE ProgramYear=@PrevProgYear AND DistrictID NOT IN (SELECT DistrictID FROM [District] WHERE INACTIVE=1) ";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating District Contacts records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating District Contacts records. ", ex.Message);
                }

            }

            return RecordsCopied;
        }

        private bool InitDistYearInfo(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [DistrictYearInfo] ([ProgramYear],[DistrictID],[AcctType],[GM],[RMCode],[RMName],[FSDName],[FSDAddress],[FSDCity],[FSDState]," +
                "[FSDZip],[FSDEmail],[FSDPhone],[FSDCell],[BusinessMgr],[Superintendent],[RMNumber],[AsstFSDName],[AsstFSDPhone],[DistCEPPaidPct],[DistCEPFreePct],[FFVDistrict],[NewOrRebid]) " +
                " SELECT @NewProgYear,[DistrictID],[AcctType],[GM],[RMCode],[RMName],[FSDName],[FSDAddress],[FSDCity],[FSDState]," +
                " [FSDZip],[FSDEmail],[FSDPhone],[FSDCell],[BusinessMgr],[Superintendent],[RMNumber],[AsstFSDName],[AsstFSDPhone],[DistCEPPaidPct],[DistCEPFreePct],[FFVDistrict],[NewOrRebid] " +
                "FROM [DistrictYearInfo] WHERE ProgramYear=@PrevProgYear AND DistrictID NOT IN (SELECT DistrictID FROM [District] WHERE INACTIVE=1) ";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating District Paycode records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating District Paycode records. ", ex.Message);
                }

            }

            return RecordsCopied;
        }

        private bool InitDistPayCodes(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [DistrictPaycodes] ([ProgramYear],[DistrictID],[PayCode],[PayCodeDesc],[HoursType],[RateTier]) " +
                " SELECT @NewProgYear,[DistrictID],[PayCode],[PayCodeDesc],[HoursType],[RateTier] " +
                " FROM [DistrictPaycodes] WHERE ProgramYear=@PrevProgYear AND DistrictID NOT IN (SELECT DistrictID FROM [District] WHERE INACTIVE=1) ";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating District Paycode records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating District Paycode records. ", ex.Message);
                }

            }

            return RecordsCopied;
        }

        private bool InitMilkDelivInfo(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [DistrictMilkDeliveryInfo] ([ProgramYear],[DistrictID],[SchoolNum],[MMBArea],[Monday],[Tuesday],[Wednesday],[Thursday],[Friday],[JuiceCases],[TeaCases],[WalkInCooler]," +
                "[EightCaseCoolers],[TwelveCaseCoolers],[SixteenCaseCoolers],[PALicense],[VendorID],[MMBSheet],[AverageCaseCount],[MilkDiscounts],[OtherDiscounts],[EquipLeases],[EquipOwner],[LeasePayments]) " +
                " SELECT @NewProgYear,[DistrictID],[SchoolNum],[MMBArea],[Monday],[Tuesday],[Wednesday],[Thursday],[Friday],[JuiceCases],[TeaCases],[WalkInCooler],[EightCaseCoolers],[TwelveCaseCoolers]," +
                "[SixteenCaseCoolers],[PALicense],[VendorID],[MMBSheet],[AverageCaseCount],[MilkDiscounts],[OtherDiscounts],[EquipLeases],[EquipOwner],[LeasePayments] " +
                "FROM [DistrictMilkDeliveryInfo] WHERE ProgramYear=@PrevProgYear AND DistrictID NOT IN (SELECT DistrictID FROM [District] WHERE INACTIVE=1) ";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating Milk Delivery Info records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating Milk Delivery Info records. ", ex.Message);
                }

            }

            return RecordsCopied;

        }

        private bool InitMenuCostItemsSetup(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [MenuCostItemsSetup] ([ProgramYear],[DistrictID],[ItemCategoryID],[ItemID],[ItemDescription],[ItemCost]) " +
                " SELECT @NewProgYear,[DistrictID],[ItemCategoryID],[ItemID],[ItemDescription],[ItemCost] " +
                "FROM [MenuCostItemsSetup] WHERE ProgramYear=@PrevProgYear AND DistrictID NOT IN (SELECT DistrictID FROM [District] WHERE INACTIVE=1) ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateRecords = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = prevProgramYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateRecords.Parameters.Add(dbParam_PrevProgYear);

                        if (cmdCreateRecords.ExecuteNonQuery() > 0)
                            RecordsCopied = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating Menu Cost Item records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating Menu Cost Item records. ", ex.Message);
                }

            }

            return RecordsCopied;

        }

        private string RetrievePrevProgYear(DateTime startDate)
        {
            string PrevProgYear = "";
            string QueryString = "SELECT TOP 1 ProgramYear FROM [ProgramYear] WHERE EndDate<@StartDate ORDER BY EndDate DESC";
            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdGetPrgYear = new SqlCommand(QueryString, dbConnection))
                    {
                        SqlParameter dbParam_startDate = new SqlParameter();
                        dbParam_startDate.ParameterName = "@StartDate";
                        dbParam_startDate.Value = startDate;
                        dbParam_startDate.DbType = System.Data.DbType.DateTime;
                        cmdGetPrgYear.Parameters.Add(dbParam_startDate);

                        object ProgYearObj = cmdGetPrgYear.ExecuteScalar();
                        if (ProgYearObj != null)
                            PrevProgYear = ProgYearObj.ToString();
                        else
                        {
                            ShowDialog("Progam Year Error", MessageType.Error, "Previous Program Year could not be retrieved. ", "");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred during previous program year retrieval. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred during previous program year retrieval. ", ex.Message);
                }
            }

            return PrevProgYear;

        }

        private bool InitializeTaxRates(string newProgramYear, DateTime StartDate, string PrevProgYear)
        {
            bool TaxRatesCreated = false;

            //Create new TaxRates Record
            string queryInsert = "INSERT INTO [TaxRates] ([ProgramYear],[TaxYear],[PayProc],[WComp],[OtherPayRelated],[OH_SERS],[OH_WComp],[SC_SalesTax]) " +
                " SELECT @NewProgYear,@TaxYear,[PayProc],[WComp],[OtherPayRelated],[OH_SERS],[OH_WComp],[SC_SalesTax] FROM [TaxRates] WHERE ProgramYear=@PrevProgYear";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdCreateTaxRates = new SqlCommand(queryInsert, dbConnection))
                    {
                        SqlParameter dbParam_NewProgYear = new SqlParameter();
                        dbParam_NewProgYear.ParameterName = "@NewProgYear";
                        dbParam_NewProgYear.Value = newProgramYear;
                        dbParam_NewProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateTaxRates.Parameters.Add(dbParam_NewProgYear);

                        SqlParameter dbParam_PrevProgYear = new SqlParameter();
                        dbParam_PrevProgYear.ParameterName = "@PrevProgYear";
                        dbParam_PrevProgYear.Value = PrevProgYear;
                        dbParam_PrevProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdCreateTaxRates.Parameters.Add(dbParam_PrevProgYear);

                        SqlParameter dbParam_TaxYear = new SqlParameter();
                        dbParam_TaxYear.ParameterName = "@TaxYear";
                        dbParam_TaxYear.Value = StartDate.Year;
                        dbParam_TaxYear.DbType = System.Data.DbType.Int32;
                        cmdCreateTaxRates.Parameters.Add(dbParam_TaxYear);

                        if (cmdCreateTaxRates.ExecuteNonQuery() > 0)
                            TaxRatesCreated = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while creating TaxRates record. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while creating TaxRates record. ", ex.Message);
                }

            }

            return TaxRatesCreated;
        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox TextBoxDate = (TextBox)sender;
            NormalizeSimplifiedDate(ref TextBoxDate);

            if (TextBoxDate.ID == "TextBoxStartDate")
            {
                TextBox TextBoxEndDate = (TextBox)FindControlRecursive(DetailsViewInsertProgramYear, "TextBoxEndDate");
                TextBoxEndDate.Focus();
            }

            if (TextBoxDate.ID == "TextBoxEndDate")
            {
                DropDownList DDLBudgetYears = (DropDownList)FindControlRecursive(DetailsViewInsertProgramYear, "DDLBudgetYears");
                DDLBudgetYears.Focus();
            }
        }

        protected bool InitializeAddOnFees(string newProgramYear, string BudgetYear, DateTime StartDate, DateTime EndDate)
        {
            bool AddOnFeesCreated = false;

            //Create new TaxRates Record
            string queryBDGTTotals = "SELECT BidNumber, AccountingNUTR, TechFeePlusSoftware, RecruitReplace, Liability, Training, WellnessFeeNUTR, CACFPAdminFeeTotal FROM [vwBDGT_Totals] " +
                " WHERE BudgetYear=@BudgetYear AND BudgetType=2 AND ACTIVE=1 AND (AccountingNUTR+TechFeePlusSoftware+RecruitReplace+Liability+Training+WellnessFeeNUTR+CACFPAdminFeeTotal)<>0 ";


            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdReadBDGTFees = new SqlCommand(queryBDGTTotals, dbConnection))
                    {
                        SqlParameter dbParam_BDGTYear = new SqlParameter();
                        dbParam_BDGTYear.ParameterName = "@BudgetYear";
                        dbParam_BDGTYear.Value = BudgetYear;
                        dbParam_BDGTYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdReadBDGTFees.Parameters.Add(dbParam_BDGTYear);

                        SqlDataReader rdr = cmdReadBDGTFees.ExecuteReader();

                        while (rdr.Read())
                        {
                            Decimal.TryParse(rdr["AccountingNUTR"].ToString(), out Decimal outAccountingNUTR);
                            Decimal.TryParse(rdr["TechFeePlusSoftware"].ToString(), out Decimal outTechFeePlusSoftware);
                            Decimal.TryParse(rdr["RecruitReplace"].ToString(), out Decimal outRecruitReplace);
                            Decimal.TryParse(rdr["Liability"].ToString(), out Decimal outLiability);
                            Decimal.TryParse(rdr["Training"].ToString(), out Decimal outTraining);
                            Decimal.TryParse(rdr["WellnessFeeNUTR"].ToString(), out Decimal outWellnessNUTR);
                            Decimal.TryParse(rdr["CACFPAdminFeeTotal"].ToString(), out Decimal outCACFPAdminFeeTotal);
                            String DistrictID = rdr["BidNumber"].ToString();
                            short SchoolNum = GetMinSchool(DistrictID);
                            if (SchoolNum == -1)
                            {
                                ShowDialog("Error", MessageType.Error, "Add-on Fees import failed - no Active Schools found for District " + DistrictID, ". Manually import add-on fees for this district after fixing.");
                                continue;
                            }

                            if (outAccountingNUTR != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outAccountingNUTR, StartDate, EndDate, "NUT", "InvProc", "ACCT", SchoolNum);
                            }

                            if (outTechFeePlusSoftware != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outTechFeePlusSoftware, StartDate, EndDate, "NUT", "TechFee", "COMTK", SchoolNum);
                            }

                            if (outRecruitReplace != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outRecruitReplace, StartDate, EndDate, "NUT", "RecruitFee", "EMPRC", SchoolNum);
                            }

                            if (outLiability != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outLiability, StartDate, EndDate, "NUT", "LiabFee", "INSLI", SchoolNum);
                            }

                            if (outTraining != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outTraining, StartDate, EndDate, "NUT", "Training", "TRNG", SchoolNum);
                            }

                            if (outWellnessNUTR != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outWellnessNUTR, StartDate, EndDate, "NUT", "Wellness", "WELNS", SchoolNum);
                            }

                            if (outCACFPAdminFeeTotal != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outCACFPAdminFeeTotal, StartDate, EndDate, "NUT", "CACFP", "CATER", SchoolNum);
                            }


                        }

                        AddOnFeesCreated = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while Initializing Add-On Fees records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while Initializing Add-On Fees records. ", ex.Message);
                }

            }

            return AddOnFeesCreated;
        }

        /// <summary>
        /// Creates Transmittal records for Add-on fees based on the budgets
        /// </summary>
        /// <param name="ProgramYear"></param>
        /// <param name="districtID"></param>
        /// <param name="AmountToDistribute"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="VendorID"></param>
        /// <param name="InvoiceNum"></param>
        /// <param name="OtherCat"></param>
        /// <param name="SchoolNum"></param>
        private void CreateMonthlyPurchEntries(string ProgramYear, string districtID, decimal AmountToDistribute, DateTime startDate, DateTime endDate, string VendorID, string InvoiceNum, string OtherCat, short SchoolNum)
        {

            int NumMonths = (endDate.Month + 1 - startDate.Month) + ((endDate.Year - startDate.Year) * 12);

            TextBox tbStatus = new TextBox();

            if (NumMonths <= 0)
                return;

            decimal monthlyInstallment = decimal.Round((AmountToDistribute / NumMonths), 2, MidpointRounding.AwayFromZero);

            for (int i = 0; i < NumMonths; i++)
            {
                DateTime currentMonthDate = startDate.AddMonths(i);
                DateTime postingDate = GetFinalPostingDateForMonth(currentMonthDate.Month, currentMonthDate.Year);
                PurchasesInsertRecord(districtID, postingDate, postingDate, VendorID, InvoiceNum, 0M, 0M, 0M, monthlyInstallment, OtherCat, "", 0M, SchoolNum, ProgramYear, ref tbStatus, true, 1, "PROGRAMYEARADDONFEESINIT", "School Year");
            }

            if (tbStatus != null)
                tbStatus.Dispose();

        }

        protected void CreateLogEntry(string LogBasicString)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

            StringBuilder sb = new StringBuilder();

            sb.Append(LogBasicString);

            string fullLogEntry = sb.ToString();

            CreateGlobalLogEntry(className, fullLogEntry);
        }

    }

}

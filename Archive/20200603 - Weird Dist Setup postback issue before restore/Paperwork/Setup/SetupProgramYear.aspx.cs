using Antlr.Runtime;
using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupProgramYear : Paperwork_Global
    {
        protected string InsertStatusString="";

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
        }

        protected void CheckBoxEntryAllowed_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);
            string rowKey = gvr.Cells[0].Text;

            if (((CheckBox)sender).Checked == true)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

                string queryRemoveOtherActives = "UPDATE PROGRAMYEAR SET AllowDataEntry=0 WHERE UPPER(ProgramYear) != @ProgramYear ";

                using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
                {
                    dbConnection.Open();
                    SqlCommand cmdUpdateProgramYear = new SqlCommand(queryRemoveOtherActives, dbConnection);

                    SqlParameter dbParam_pRgYear = new SqlParameter();
                    dbParam_pRgYear.ParameterName = "@ProgramYear";
                    dbParam_pRgYear.Value = rowKey;
                    dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmdUpdateProgramYear.Parameters.Add(dbParam_pRgYear);

                    cmdUpdateProgramYear.ExecuteNonQuery();
                    cmdUpdateProgramYear.Dispose();
                }
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

            ((GridView)gvr.Parent.Parent).DataBind();
        }

        protected void CheckBoxEntryMonthClosed_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);
            string rowKey = gvr.Cells[0].Text;
            string columnName = ((CheckBox)sender).ID;

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

                    cmdUpdatePrgYear.ExecuteNonQuery();
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

                        cmdUpdatePrgYear.ExecuteNonQuery();
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
                if(InitGlobalPurchCats(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-Purchasing Other Categories were copied forward.";

                int recordcount = 0;
                recordcount=ImportCategoryRecords(NewProgramYear, NewBudgetYear);
                if (recordcount>0)
                    InsertStatusString += "<br>-District Purchasing Other Categories were synchronized with Budgets.";

                if(InitMilkDelivInfo(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-Milk Delivery Info records were copied forward.";

                if(InitDistPayCodes(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-District Paycode records were copied forward.";

                if(InitDistYearInfo(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-District Year Info records were copied forward.";

                if (InitDistYearContacts(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-District Year Contacts were copied forward.";

                if (InitDistSchoolYearSetup(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-District School Year Setups were copied forward.";

                if(InitOtherCostCats(NewProgramYear, PrevProgramYear))
                    InsertStatusString += "<br>-Indirect Cost Categories were copied forward.";

                if(InitializeAddOnFees(NewProgramYear, NewBudgetYear, StartDate, EndDate))
                {
                    InsertStatusString += "<br>-AddOn Fees were created from Budget Totals.";
                }

                ShowDialog("Info", MessageType.Info, InsertStatusString, "");
            }
        }

        private bool InitGlobalPurchCats(string newProgramYear, string prevProgramYear)
        {
            bool RecordsCopied = false;

            //Create new Records
            string queryInsert = "INSERT INTO [OtherPurchaseCategories] ([ProgramYear],[OtherCategoryID],[OtherCategoryDesc]) " +
                " SELECT @NewProgYear,[OtherCategoryID],[OtherCategoryDesc] " +
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
                "[EligibleFree],[EligibleReduced],[BreakfastIncent],[Breakfasts],[Tiers],[OtherSales],[Learners],[Snacks],[SchoolCEPFreePct],[SchoolCEPPaidPct],[PartialCEPPartic],[UseCustomCEPPct]) " +
                " SELECT @NewProgYear,[DistrictID],[SchoolNum],[BfastElPrice],[BfastJrPrice],[BfastSrPrice],[BfastRedPrice]," +
                " [LunchElPrice],[LunchJrPrice],[LunchSrPrice],[LunchTier1Price],[LunchTier2Price],[LunchRedPrice],[BfastPaidRate],[BfastRedRate],[BfastFreeRate],[LunchPaidRate]," +
                " [LunchRedRate],[LunchFreeRate],[OSnackPaidPrice],[OSnackRedPrice],[OSnackPaidRate],[OSnackRedRate],[OSnackFreeRate],[AF],[Enrollment],[EligiblePaid]," +
                " [EligibleFree],[EligibleReduced],[BreakfastIncent],[Breakfasts],[Tiers],[OtherSales],[Learners],[Snacks],[SchoolCEPFreePct],[SchoolCEPPaidPct],[PartialCEPPartic],[UseCustomCEPPct]" +
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
                "[FSDZip],[FSDEmail],[FSDPhone],[FSDCell],[BusinessMgr],[Superintendent],[RMNumber],[AsstFSDName],[AsstFSDPhone],[DistCEPPaidPct],[DistCEPFreePct]) " +
                " SELECT @NewProgYear,[DistrictID],[AcctType],[GM],[RMCode],[RMName],[FSDName],[FSDAddress],[FSDCity],[FSDState]," +
                " [FSDZip],[FSDEmail],[FSDPhone],[FSDCell],[BusinessMgr],[Superintendent],[RMNumber],[AsstFSDName],[AsstFSDPhone],[DistCEPPaidPct],[DistCEPFreePct]" +
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
                            ShowDialog("Progam Year Error", MessageType.Error, "Previous Program Year could not be retrieved. " , "");
                        }
                    }
                }
                catch (SqlException ex) 
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred during previous program year retrieval. " , ex.Message);
                }
                catch(Exception ex)
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
            string queryInsert = "INSERT INTO [TaxRates] ([ProgramYear],[TaxYear],[PayProc],[WComp],[OtherPayRelated]) " +
                " SELECT @NewProgYear,@TaxYear,[PayProc],[WComp],[OtherPayRelated] FROM [TaxRates] WHERE ProgramYear=@PrevProgYear";
            

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

            if(TextBoxDate.ID== "TextBoxStartDate")
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

                            if (outAccountingNUTR != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outAccountingNUTR, StartDate, EndDate, "NUT", "InvProc", "ACCT", SchoolNum);
                            }

                            if(outTechFeePlusSoftware != 0M)
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

        private void CreateMonthlyPurchEntries(string ProgramYear, string districtID, decimal AmountToDistribute, DateTime startDate, DateTime endDate, string VendorID, string InvoiceNum, string OtherCat, short SchoolNum)
        {

            int NumMonths = (endDate.Month + 1 - startDate.Month) + ((endDate.Year - startDate.Year) * 12);
            TextBox tbStatus = new TextBox();
            
            if (NumMonths <= 0)
                return;

            decimal monthlyInstallment = decimal.Round( (AmountToDistribute / NumMonths), 2);

            for(int i=0; i<NumMonths; i++)
            {
                DateTime currentMonthDate = startDate.AddMonths(i);
                DateTime postingDate = GetFinalPostingDateForMonth(currentMonthDate.Month, currentMonthDate.Year);
                PurchasesInsertRecord(districtID, postingDate, postingDate, VendorID, InvoiceNum, 0M, 0M, 0M, monthlyInstallment, OtherCat, "", 0M, SchoolNum, ProgramYear, ref tbStatus, true, 1);
            }
        }

        /// <summary>
        /// Retrieve the minimum school number based on the schoolnumber field. Find Active school if possible, otherwise return first found.
        /// </summary>
        /// <param name="DistrictID">District ID string</param>
        /// <returns>School Number ID</returns>
        private short GetMinSchool(string DistrictID)
        {

            short outSchoolNum = 0;
            string querySchool = "SELECT SchoolNum, Inactive FROM [DistrictSchools] WHERE DistrictID=@DistrictID ORDER BY SchoolNum ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdReadSchools = new SqlCommand(querySchool, dbConnection))
                    {
                        SqlParameter dbParam_DistID = new SqlParameter();
                        dbParam_DistID.ParameterName = "@DistrictID";
                        dbParam_DistID.Value = DistrictID;
                        dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                        cmdReadSchools.Parameters.Add(dbParam_DistID);

                        SqlDataReader rdr = cmdReadSchools.ExecuteReader();

                        if (rdr.Read() && short.TryParse(rdr["SchoolNum"].ToString(), out short sclNum))
                        {
                            outSchoolNum = sclNum;

                            //if inactive, try to find an active one
                            if (rdr["Inactive"].ToString() == "1")
                            {
                                while (rdr.Read())
                                {
                                    if (rdr["Inactive"].ToString() == "0" && short.TryParse(rdr["SchoolNum"].ToString(), out short sclNumActive))
                                    {
                                        outSchoolNum = sclNumActive;
                                        break;//break out of while loop and close down reader
                                    }
                                }
                            }
                        }
                    }
                        
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while retrieving schools. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while retrieving schools. ", ex.Message);
                }
            }

            return outSchoolNum;
        }

    }

}

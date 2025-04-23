using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class LockTransactions : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            switch (Session["AccessLevel"].ToString().ToUpper())
            {
                case "ADMIN":
                case "OFFICE":
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

                TextboxStartDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1).ToShortDateString();
                TextboxEndDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1)).ToShortDateString();

                DDLProgramYears.DataBind();
                if (Session["ProgramYear"]!= null && DDLProgramYears.Items.Count > 0 && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                    DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                DDLProgramYears_SelectedIndexChanged(DDLProgramYears, EventArgs.Empty);

            }


        }

        protected void ILock_Click(object sender, EventArgs e)
        {
            DateTime StartDate;
            DateTime EndDate;
            if (DateTime.TryParse(TextboxStartDate.Text, out StartDate) == false)
                return;

            if (DateTime.TryParse(TextboxEndDate.Text, out EndDate) == false)
                return;

            if (DDLDistricts.SelectedValue == "%" && HFConfirmed.Value == "0")
            {
                return;
            }

            CreateLogEntry("Lock Clicked");

            UpdateTrxRecords(true, StartDate, EndDate);

        }

        protected void IUnlock_Click(object sender, EventArgs e)
        {
            DateTime StartDate;
            DateTime EndDate;
            if (DateTime.TryParse(TextboxStartDate.Text, out StartDate) == false)
                return;

            if (DateTime.TryParse(TextboxEndDate.Text, out EndDate) == false)
                return;

            if (DDLDistricts.SelectedValue == "%" && HFConfirmed.Value == "0")
            {
                return;
            }

            CreateLogEntry("Unlock Clicked");

            UpdateTrxRecords(false, StartDate, EndDate);
        }

        protected void CreateLogEntry(string LogBasicString)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

            StringBuilder sb = new StringBuilder();

            sb.Append(LogBasicString);
            sb.Append(" | ProgramYear:" + DDLProgramYears.SelectedValue);
            sb.Append(" | RM:" + DDLRMs.SelectedItem.Text);
            sb.Append(" | District:" + DDLDistricts.SelectedItem.Text);
            sb.Append(" | StartDate:" + TextboxStartDate.Text);
            sb.Append(" | EndDate:" + TextboxEndDate.Text);
            if(CheckBoxPurchases.Checked)
                sb.Append(" | Purchases Checked" );
            if (CheckBoxRecaps.Checked)
                sb.Append(" | Sales Checked");
            if (CheckBoxDistPayroll.Checked)
                sb.Append(" | Dist Payroll Checked");
            if (CheckBoxMMB.Checked)
                sb.Append(" | MMB Sheets Checked");
            if (CheckBoxInv.Checked)
                sb.Append(" | Inventory Checked");
            if (CheckBoxOtherFood.Checked)
                sb.Append(" | Other Food Checked");
            if (CheckBoxSpFunc.Checked)
                sb.Append(" | Sp. Fn. Checked");
            if (CheckBoxDays.Checked)
                sb.Append(" | Days Checked");
            if (CheckBoxNotifications.Checked)
                sb.Append(" | Notifications Checked");

            string fullLogEntry = sb.ToString();

            CreateGlobalLogEntry(className, fullLogEntry);
        }

        protected void UpdateTrxRecords(bool Lock, DateTime StartDateRange, DateTime EndDateRange)
        {

            string LockBitString;
            string ImportedString;
            if (Lock)
                LockBitString = "1";
            else
            {
                LockBitString = "0";
                ImportedString = "0";
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string queryLockRecords = "";

            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;//, className, Context.User.Identity.Name.ToUpper()

            if (CheckBoxPurchases.Checked)
            {
                if (Lock)
                {
                    queryLockRecords += "UPDATE [Purchases] SET Locked=" + LockBitString + " WHERE PostDate>=@StartDate AND PostDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID ";
                    if (DDLRMs.SelectedIndex == 0)
                        queryLockRecords += "LIKE @DistrictID; ";
                    else
                        queryLockRecords += "IN (" + LabelDistricts.Text + "); ";
                }
                else
                {
                    queryLockRecords += "UPDATE [Purchases] SET Locked=" + LockBitString + ", Imported=0 WHERE PostDate>=@StartDate AND PostDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID ";
                    if (DDLRMs.SelectedIndex == 0)
                        queryLockRecords += "LIKE @DistrictID; ";
                    else
                        queryLockRecords += "IN (" + LabelDistricts.Text + "); ";
                }

                if (DDLRMs.SelectedIndex == 0)
                    UpdateTransactionCutoff(Lock, StartDateRange, EndDateRange, "Purchases", DDLProgramYears.SelectedValue, DDLDistricts.SelectedValue, className, Context.User.Identity.Name.ToUpper());
                else
                {
                    string[] dists = LabelDistricts.Text.Split(',');
                    if(dists.Length>0)
                    {
                        foreach(string dist in dists)
                        {
                            string distStripped = dist.Replace("'", "");
                            UpdateTransactionCutoff(Lock, StartDateRange, EndDateRange, "Purchases", DDLProgramYears.SelectedValue, distStripped.Trim(' '), className, Context.User.Identity.Name.ToUpper());
                        }
                    }

                }
                

            }
            if (CheckBoxRecaps.Checked)
            {
                queryLockRecords += "UPDATE [RecapEntry] SET Locked=" + LockBitString + " WHERE RecapDate>=@StartDate AND RecapDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID ";
                if (DDLRMs.SelectedIndex == 0)
                    queryLockRecords += "LIKE @DistrictID; ";
                else
                    queryLockRecords += "IN (" + LabelDistricts.Text + "); ";

                if (DDLRMs.SelectedIndex == 0)
                    UpdateTransactionCutoff(Lock, StartDateRange, EndDateRange, "SalesEntry", DDLProgramYears.SelectedValue, DDLDistricts.SelectedValue, className, Context.User.Identity.Name.ToUpper());
                else
                {
                    string[] dists = LabelDistricts.Text.Split(',');
                    if (dists.Length > 0)
                    {
                        foreach (string dist in dists)
                        {
                            string distStripped = dist.Replace("'", "");
                            UpdateTransactionCutoff(Lock, StartDateRange, EndDateRange, "SalesEntry", DDLProgramYears.SelectedValue, distStripped.Trim(' '), className, Context.User.Identity.Name.ToUpper());
                        }
                    }

                }
            }

            if (CheckBoxDistPayroll.Checked)
            {
                queryLockRecords += "UPDATE [Payroll] SET Locked=" + LockBitString + " WHERE PayDate>=@StartDate AND PayDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID ";
                if (DDLRMs.SelectedIndex == 0)
                    queryLockRecords += "LIKE @DistrictID; ";
                else
                    queryLockRecords += "IN (" + LabelDistricts.Text + "); ";
            }

            if (CheckBoxMMB.Checked)
            {
                queryLockRecords += "UPDATE [MMBTransactions] SET Locked=" + LockBitString + " WHERE MMBDate>=@StartDate AND MMBDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID ";
                if (DDLRMs.SelectedIndex == 0)
                    queryLockRecords += "LIKE @DistrictID; ";
                else
                    queryLockRecords += "IN (" + LabelDistricts.Text + "); ";
            }

            if (CheckBoxInv.Checked)
            {
                queryLockRecords += "UPDATE [Inventory] SET Locked=" + LockBitString + " WHERE YearMonth>=@YearMonthStart AND YearMonth<=@YearMonthEnd AND ProgramYear=@ProgramYear AND DistrictID ";
                if (DDLRMs.SelectedIndex == 0)
                    queryLockRecords += "LIKE @DistrictID; ";
                else
                    queryLockRecords += "IN (" + LabelDistricts.Text + "); ";
            }

            if (CheckBoxOtherFood.Checked)
            {
                queryLockRecords += "UPDATE [OtherSales] SET Locked=" + LockBitString + " WHERE OtherSalesDate>=@StartDate AND OtherSalesDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID ";
                if (DDLRMs.SelectedIndex == 0)
                    queryLockRecords += "LIKE @DistrictID; ";
                else
                    queryLockRecords += "IN (" + LabelDistricts.Text + "); ";
            }

            if (CheckBoxSpFunc.Checked)
            {
                queryLockRecords += "UPDATE [SPFNInvoiceHDR] SET Locked=" + LockBitString + " WHERE BillingDate>=@StartDate AND BillingDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID ";
                if (DDLRMs.SelectedIndex == 0)
                    queryLockRecords += "LIKE @DistrictID; ";
                else
                    queryLockRecords += "IN (" + LabelDistricts.Text + "); ";
            }

            if (CheckBoxDays.Checked)
            {
                //Updated for new RM option
                LockUnlockDaysRecords(Lock, StartDateRange, EndDateRange);
            }

            if (CheckBoxNotifications.Checked)
            {
                queryLockRecords += "UPDATE [Notifications] SET Locked=" + LockBitString + " WHERE NotificationDate>=@StartDate AND NotificationDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID ";
                if (DDLRMs.SelectedIndex == 0)
                    queryLockRecords += "LIKE @DistrictID; ";
                else
                    queryLockRecords += "IN (" + LabelDistricts.Text + "); ";
            }


            if (string.IsNullOrEmpty(queryLockRecords))
                return;

            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                SqlCommand cmdLockRecords = new SqlCommand(queryLockRecords, dbConnection);

                SqlParameter dbParam_startDate = new SqlParameter();
                dbParam_startDate.ParameterName = "@StartDate";
                dbParam_startDate.Value = StartDateRange.ToShortDateString();
                dbParam_startDate.DbType = System.Data.DbType.Date;
                cmdLockRecords.Parameters.Add(dbParam_startDate);

                SqlParameter dbParam_endDate = new SqlParameter();
                dbParam_endDate.ParameterName = "@EndDate";
                dbParam_endDate.Value = EndDateRange.ToShortDateString();
                dbParam_endDate.DbType = System.Data.DbType.Date;
                cmdLockRecords.Parameters.Add(dbParam_endDate);

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                dbParam_progYear.DbType = System.Data.DbType.StringFixedLength;
                cmdLockRecords.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_dist = new SqlParameter();
                dbParam_dist.ParameterName = "@DistrictID";
                dbParam_dist.Value = DDLDistricts.SelectedValue;
                dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                cmdLockRecords.Parameters.Add(dbParam_dist);

                #region InventoryParameters

                string InvYearMonthStart;
                string InvYearMonthEnd;

                if (StartDateRange <= DateTime.Parse(HFProgramYearStartDate.Value))
                    InvYearMonthStart = "*OPEN*";
                else
                    InvYearMonthStart = StartDateRange.Year.ToString() + "-" + StartDateRange.Month.ToString().PadLeft(2, '0');

                SqlParameter dbParam_invYearMonthStart = new SqlParameter();
                dbParam_invYearMonthStart.ParameterName = "@YearMonthStart";
                dbParam_invYearMonthStart.Value = InvYearMonthStart;
                dbParam_invYearMonthStart.DbType = System.Data.DbType.String;
                cmdLockRecords.Parameters.Add(dbParam_invYearMonthStart);

                //if end range is the beginning of the programyear, only close the OPEN counts
                if (EndDateRange <= DateTime.Parse(HFProgramYearStartDate.Value))
                    InvYearMonthEnd = "*OPEN*";
                else if (EndDateRange >= DateTime.Parse(HFProgramYearEndDate.Value))
                    InvYearMonthEnd = "ZZZZZZZ";//All values should be included - even SFSP's special CLOSE yearmonth
                else
                    InvYearMonthEnd = EndDateRange.Year.ToString() + "-" + EndDateRange.Month.ToString().PadLeft(2, '0');

                SqlParameter dbParam_invYearMonthEnd = new SqlParameter();
                dbParam_invYearMonthEnd.ParameterName = "@YearMonthEnd";
                dbParam_invYearMonthEnd.Value = InvYearMonthEnd;
                dbParam_invYearMonthEnd.DbType = System.Data.DbType.String;
                cmdLockRecords.Parameters.Add(dbParam_invYearMonthEnd);

                #endregion

                try
                {
                    int recordcount = cmdLockRecords.ExecuteNonQuery();

                    if (Lock)
                        ShowMessage("Transactions Locked.", MessageType.Success);
                    else
                        ShowMessage("Transactions Unlocked.", MessageType.Success);
                }
                catch (SqlException ex)
                {
                    if (Lock)
                        ShowMessage("Error locking transactions." + Environment.NewLine + ex.Message, MessageType.Error);
                    else
                        ShowMessage("Error unlocking transactions." + Environment.NewLine + ex.Message, MessageType.Error);
                }
                catch
                { }
                cmdLockRecords.Dispose();
            }
        }

        /// <summary>
        /// Created special routine because we need to create missing days to lock them, otherwise just mark the records as locked
        /// </summary>
        private void LockUnlockDaysRecords(bool Lock, DateTime StartDateRange, DateTime EndDateRange)
        {
            DateTime CurrentDate;

            if (StartDateRange > EndDateRange)
                return;

            CurrentDate = StartDateRange;

            while (CurrentDate <= EndDateRange)
            {
                int existingRecordCount = 0;

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

                //Not checking for records for all districts - just update existing
                //If a single district is selected - we will create all missing days as 0's and lock them
                if (DDLDistricts.SelectedValue != "%" && DDLRMs.SelectedValue=="")
                {

                    string checkForDateRecord = "SELECT count(*) FROM [Days] WHERE ProgramYear=@ProgramYear AND ServingDate=@ServingDate AND DistrictID=@DistrictID  ";

                    SqlConnection dbConnection = new SqlConnection(connectionString);
                    using (SqlCommand cmd = new SqlCommand(checkForDateRecord, dbConnection))
                    {
                        dbConnection.Open();
                        SqlParameter dbParam_ProgYear = new SqlParameter();
                        dbParam_ProgYear.ParameterName = "@ProgramYear";
                        dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
                        dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmd.Parameters.Add(dbParam_ProgYear);

                        SqlParameter dbParam_dist = new SqlParameter();
                        dbParam_dist.ParameterName = "@DistrictID";
                        dbParam_dist.Value = DDLDistricts.SelectedValue;
                        dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                        cmd.Parameters.Add(dbParam_dist);

                        SqlParameter dbParam_srvDate = new SqlParameter();
                        dbParam_srvDate.ParameterName = "@ServingDate";
                        dbParam_srvDate.Value = CurrentDate;
                        dbParam_srvDate.DbType = System.Data.DbType.Date;
                        cmd.Parameters.Add(dbParam_srvDate);

                        try
                        {
                            object obj = cmd.ExecuteScalar();
                            if (obj != null)
                            {
                                int.TryParse(obj.ToString(), out existingRecordCount);
                            }
                        }
                        catch (SqlException ex)
                        {
                            //RegisterDOMReadyScript("error message", "alert('Error saving the school header info (eligibility, enrollment...).');");
                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {
                            dbConnection.Close();
                        }
                    }
                }
                else
                    existingRecordCount = 1;//All districts selected - only update the existing records

                string updateCreateDayRecord = "";
                if (existingRecordCount == 0)
                {
                    //Insert
                    updateCreateDayRecord = "INSERT INTO [Days] ([ProgramYear],[DistrictID],[ServingDate],[BreakfastDay],[LunchDay],[Overridden],[OverriddenBy],[Locked],[BSSO],[LSSO]) " +
                        " VALUES(@ProgramYear, @DistrictID, @ServingDate, 0,0,0,'',@Locked,0,0)";
                }
                else
                {
                    //Update
                    //Note - this query is applicable for single district selection and all districts so I must use LIKE
                    updateCreateDayRecord = "UPDATE [Days] SET Locked=@Locked " +
                        " WHERE ProgramYear=@ProgramYear AND ServingDate=@ServingDate AND DistrictID ";
                    if (DDLRMs.SelectedIndex == 0)
                        updateCreateDayRecord += "LIKE @DistrictID; ";
                    else
                        updateCreateDayRecord += "IN (" + LabelDistricts.Text + "); ";

                }

                SqlConnection dbConnection2 = new SqlConnection(connectionString);
                using (SqlCommand cmd2 = new SqlCommand(updateCreateDayRecord, dbConnection2))
                {
                    dbConnection2.Open();

                    SqlParameter dbParam_ProgYear = new SqlParameter();
                    dbParam_ProgYear.ParameterName = "@ProgramYear";
                    dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
                    dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_ProgYear);

                    SqlParameter dbParam_dist = new SqlParameter();
                    dbParam_dist.ParameterName = "@DistrictID";
                    dbParam_dist.Value = DDLDistricts.SelectedValue;
                    dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_dist);

                    SqlParameter dbParam_srvDate = new SqlParameter();
                    dbParam_srvDate.ParameterName = "@ServingDate";
                    dbParam_srvDate.Value = CurrentDate;
                    dbParam_srvDate.DbType = System.Data.DbType.Date;
                    cmd2.Parameters.Add(dbParam_srvDate);

                    SqlParameter dbParam_locked = new SqlParameter();
                    dbParam_locked.ParameterName = "@Locked";
                    dbParam_locked.Value = Lock;
                    dbParam_locked.DbType = System.Data.DbType.Byte;
                    cmd2.Parameters.Add(dbParam_locked);

                    try
                    {
                        object obj = cmd2.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        //RegisterDOMReadyScript("error message", "alert('Error saving the school header info (eligibility, enrollment...).');");
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        dbConnection2.Close();
                    }
                }

                //finally
                CurrentDate = CurrentDate.Date.AddDays(1);
            }

        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;


            //SqlDataSourceInvYear.DataBind();
            //DataView dvSql = (DataView)SqlDataSourceInvYear.Select(DataSourceSelectArguments.Empty);
            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int rowInt = dvSql.Find(DDLProgramYears.SelectedValue);
                DataRowView drvSql = dvSql[rowInt];

                DateTime pyStart = ((DateTime)drvSql["StartDate"]);
                DateTime pyEnd = ((DateTime)drvSql["EndDate"]);

                HFProgramYearStartDate.Value = pyStart.ToShortDateString();
                HFProgramYearEndDate.Value = pyEnd.ToShortDateString();

                DateTime.TryParse(TextboxStartDate.Text, out DateTime curentStartDate);
                DateTime.TryParse(TextboxEndDate.Text, out DateTime curentEndDate);

                if (curentStartDate > pyEnd)
                    curentStartDate = pyEnd;
                if (curentEndDate > pyEnd)
                    curentEndDate = pyEnd;

                if (curentStartDate < pyStart)
                    curentStartDate = pyStart;
                if (curentEndDate < pyStart)
                    curentEndDate = pyStart;


                TextboxStartDate.Text = curentStartDate.ToShortDateString();
                TextboxEndDate.Text = curentEndDate.ToShortDateString();

            }

        }

        protected void TextboxDate_TextChanged(object sender, EventArgs e)
        {
            DateTime.TryParse(HFProgramYearStartDate.Value, out DateTime pyStart);
            DateTime.TryParse(HFProgramYearEndDate.Value, out DateTime pyEnd);
            DateTime.TryParse(TextboxStartDate.Text, out DateTime curentStartDate);
            DateTime.TryParse(TextboxEndDate.Text, out DateTime curentEndDate);

            if (curentStartDate > pyEnd)
                curentStartDate = pyEnd;
            if (curentEndDate > pyEnd)
                curentEndDate = pyEnd;

            if (curentStartDate < pyStart)
                curentStartDate = pyStart;
            if (curentEndDate < pyStart)
                curentEndDate = pyStart;

            if (((TextBox)sender).ID == "TextboxStartDate" && curentStartDate > curentEndDate)
                curentEndDate = curentStartDate;

            if (((TextBox)sender).ID == "TextboxEndDate" && curentEndDate < curentStartDate)
                curentStartDate = curentEndDate;


            TextboxStartDate.Text = curentStartDate.ToShortDateString();
            TextboxEndDate.Text = curentEndDate.ToShortDateString();
        }

        protected void DDLRMs_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList DDLRMs = (DropDownList)sender;
            if (DDLRMs == null)
                return;

            if (DDLRMs.SelectedValue.ToString()=="")
            {
                DDLDistricts.Enabled = true;
                DDLDistricts.Visible = true;
                DDLDistricts.SelectedIndex= 0;
                LabelDistricts.Text = "";
            }
            else
            {
                DDLDistricts.Enabled = false;
                DDLDistricts.Visible = false;
                DDLDistricts.SelectedIndex = 0;
                LabelDistricts.Text = DDLRMs.SelectedValue.ToString();
            }
        }

        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDLDistricts.SelectedValue.ToString() == "%")
                DDLRMs.Visible = true;
            else
                DDLRMs.Visible = false;
        }
    }
}
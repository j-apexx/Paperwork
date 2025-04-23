using System;
using System.Data;
using System.Data.SqlClient;
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
                TextboxStartDate.Text= (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1).ToShortDateString();
                TextboxEndDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1)).ToShortDateString();

                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count > 0 && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                    DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                DDLProgramYears_SelectedIndexChanged(DDLProgramYears, EventArgs.Empty);

            }

            //HFProgramYearStartDate.Value = ((DateTime)Session["ProgramYearStartDate"]).ToShortDateString();
            //HFProgramYearEndDate.Value = ((DateTime)Session["ProgramYearEndDate"]).ToShortDateString();

        }

        protected void ILock_Click(object sender, EventArgs e)
        {
            DateTime StartDate;
            DateTime EndDate;
            if (DateTime.TryParse(TextboxStartDate.Text, out StartDate) == false)
                return;

            if (DateTime.TryParse(TextboxEndDate.Text, out EndDate) == false)
                return;

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

            UpdateTrxRecords(false, StartDate, EndDate);
        }

        protected void UpdateTrxRecords(bool Lock, DateTime StartDateRange, DateTime EndDateRange)
        {

            byte LockBit;
            if (Lock)
                LockBit = 1;
            else
                LockBit = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string queryLockRecords = "";

            if (CheckBoxDistPayroll.Checked)
                queryLockRecords += "UPDATE [Purchases] SET Locked=" + LockBit.ToString() + " WHERE PostDate>=@StartDate AND PostDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID; ";

            if (CheckBoxRecaps.Checked)
                queryLockRecords += "UPDATE [RecapEntry] SET Locked=" + LockBit.ToString() + " WHERE RecapDate>=@StartDate AND RecapDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID; ";

            if (CheckBoxDistPayroll.Checked)
                queryLockRecords += "UPDATE [Payroll] SET Locked=" + LockBit.ToString() + " WHERE PayDate>=@StartDate AND PayDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID; ";

            if (CheckBoxMMB.Checked)
                queryLockRecords += "UPDATE [MMBTransactions] SET Locked=" + LockBit.ToString() + " WHERE MMBDate>=@StartDate AND MMBDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID; ";

            if (CheckBoxInv.Checked)
                queryLockRecords += "UPDATE [Inventory] SET Locked=" + LockBit.ToString() + " WHERE YearMonth>=@YearMonthStart AND YearMonth<=@YearMonthEnd AND ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID; ";

            if (CheckBoxOtherFood.Checked)
                queryLockRecords += "UPDATE [OtherSales] SET Locked=" + LockBit.ToString() + " WHERE OtherSalesDate>=@StartDate AND OtherSalesDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID; ";

            if (CheckBoxSpFunc.Checked)
                queryLockRecords += "UPDATE [SPFNInvoiceHDR] SET Locked=" + LockBit.ToString() + " WHERE BillingDate>=@StartDate AND BillingDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID; ";

            if (CheckBoxDays.Checked)
                LockUnlockDaysRecords(Lock, StartDateRange, EndDateRange);

            if (CheckBoxNotifications.Checked)
                queryLockRecords += "UPDATE [Notifications] SET Locked=" + LockBit.ToString() + " WHERE NotificationDate>=@StartDate AND NotificationDate<=@EndDate AND ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID; ";


            //Others from the access program - Days & Notifications

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

                string InvYearMonthStart;
                string InvYearMonthEnd;

                if (StartDateRange < DateTime.Parse(HFProgramYearStartDate.Value))
                    InvYearMonthStart = "*OPEN*";
                else
                    InvYearMonthStart = StartDateRange.Year.ToString() + "-" + StartDateRange.Month.ToString().PadLeft(2, '0');

                SqlParameter dbParam_invYearMonthStart = new SqlParameter();
                dbParam_invYearMonthStart.ParameterName = "@YearMonthStart";
                dbParam_invYearMonthStart.Value = InvYearMonthStart;
                dbParam_invYearMonthStart.DbType = System.Data.DbType.String;
                cmdLockRecords.Parameters.Add(dbParam_invYearMonthStart);

                if (EndDateRange < DateTime.Parse(HFProgramYearStartDate.Value))
                    InvYearMonthEnd = "*OPEN*";
                else
                    InvYearMonthEnd = EndDateRange.Year.ToString() + "-" + EndDateRange.Month.ToString().PadLeft(2, '0');

                SqlParameter dbParam_invYearMonthEnd = new SqlParameter();
                dbParam_invYearMonthEnd.ParameterName = "@YearMonthEnd";
                dbParam_invYearMonthEnd.Value = InvYearMonthEnd;
                dbParam_invYearMonthEnd.DbType = System.Data.DbType.String;
                cmdLockRecords.Parameters.Add(dbParam_invYearMonthEnd);

                try
                {
                    int recordcount = cmdLockRecords.ExecuteNonQuery();

                    if(Lock)
                        ShowMessage("Transactions Locked.", MessageType.Success);
                    else
                        ShowMessage("Transactions Unlocked.", MessageType.Success);
                }
                catch (SqlException ex)
                {
                    if(Lock)
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

            while(CurrentDate <= EndDateRange)
            {
                int existingRecordCount = 0;

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

                //Not checking for records for all districts - just update existing
                //If a single district is selected - we will create all missing days as 0's and lock them
                if (DDLDistricts.SelectedValue != "%")
                {

                    string checkForDateRecord = "SELECT count(*) FROM [Days] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND ServingDate=@ServingDate ";
                    
                    SqlConnection dbConnection = new SqlConnection(connectionString);
                    using (SqlCommand cmd = new SqlCommand(checkForDateRecord, dbConnection))
                    {
                        dbConnection.Open();
                        SqlParameter dbParam_ProgYear = new SqlParameter();
                        dbParam_ProgYear.ParameterName = "@ProgramYear";
                        dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
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
                if(existingRecordCount==0)
                {
                    //Insert
                    updateCreateDayRecord = "INSERT INTO [Days] ([ProgramYear],[DistrictID],[ServingDate],[BreakfastDay],[LunchDay],[Overridden],[OverriddenBy],[Locked]) " +
                        " VALUES(@ProgramYear, @DistrictID, @ServingDate, 0,0,0,'',@Locked)";
                }
                else
                {
                    //Update
                    //Note - this query is applicable for single district selection and all districts so I must use LIKE
                    updateCreateDayRecord = "UPDATE [Days] SET Locked=@Locked " +
                        " WHERE (ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID AND ServingDate=@ServingDate)";
                }

                SqlConnection dbConnection2 = new SqlConnection(connectionString);
                using (SqlCommand cmd2 = new SqlCommand(updateCreateDayRecord, dbConnection2))
                {
                    dbConnection2.Open();

                    SqlParameter dbParam_ProgYear = new SqlParameter();
                    dbParam_ProgYear.ParameterName = "@ProgramYear";
                    dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
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
                CurrentDate= CurrentDate.Date.AddDays(1);
            }

        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update the hidden fields which contain the start and end dates.


            //SqlDataSourceInvYear.DataBind();
            //DataView dvSql = (DataView)SqlDataSourceInvYear.Select(DataSourceSelectArguments.Empty);
            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int rowInt = dvSql.Find(DDLProgramYears.SelectedValue);
                DataRowView drvSql = dvSql[rowInt];
                HFProgramYearStartDate.Value = ((DateTime)drvSql["StartDate"]).ToShortDateString();
                HFProgramYearEndDate.Value=((DateTime)drvSql["EndDate"]).ToShortDateString();
            }

        }
    }
}
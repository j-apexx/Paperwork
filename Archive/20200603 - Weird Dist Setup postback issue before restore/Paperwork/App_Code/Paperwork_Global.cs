using System;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Script.Serialization;

namespace Paperwork
{
    public class Paperwork_Global: Page
    {

        /// <summary>
        /// Checks to ensure security is set for the site - force check revalidates security when access sensitive pages
        /// </summary>
        /// <param name="forceCheck">Revalidate security</param>
        protected void VerifyAccess(bool forceReCheck)
        {
            Session["CurrentUser"] = Context.User.Identity.Name.ToUpper();

            if (Session["AccessLevel"] == null || forceReCheck)
            {
                SetAccessLevel();
            }

            if (Session["AccessLevel"].ToString().ToUpper() == "UNAUTHORIZED")
                Response.Redirect("~/Unauthorized.aspx");

            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        /// <summary>
        /// Checks the database User table against the current windows user. If found, the access level is saved into the session variable
        /// </summary>
        protected void SetAccessLevel()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.IDbConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT AccessLevel FROM [UserAccess] WHERE UPPER(UserID)=@UserID; UPDATE [UserAccess] SET LastActivity=GETDATE() WHERE UPPER(UserID)=@UserID";

            using (IDbCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                System.Data.IDataParameter dbParam_uSERID = new System.Data.SqlClient.SqlParameter();
                dbParam_uSERID.ParameterName = "@UserID";
                dbParam_uSERID.Value = Context.User.Identity.Name.ToUpper();
                dbParam_uSERID.DbType = System.Data.DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_uSERID);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object accessLev = dbCommand.ExecuteScalar();
                    if (accessLev == null)
                        Session["AccessLevel"] = "Unauthorized";
                    else if ((int)accessLev == 0)
                        Session["AccessLevel"] = "User";
                    else if ((int)accessLev == 5)
                        Session["AccessLevel"] = "RM";
                    else if ((int)accessLev == 10)
                        Session["AccessLevel"] = "Office";
                    else if ((int)accessLev == 99)
                        Session["AccessLevel"] = "Admin";
                    else
                        Session["AccessLevel"] = "Unauthorized";
                }
                catch (Exception)
                {
                    Session["AccessLevel"] = "Unauthorized";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

        }

        /// <summary>
        /// Check access to a folder for the exports
        /// </summary>
        /// <param name="folderPath">Folder Path string</param>
        /// <returns>Boolean indicating access</returns>
        protected bool hasWriteAccessToFolder(string folderPath, out string ErrorInfo)
        {
            ErrorInfo = string.Empty;
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch(DirectoryNotFoundException)
            {
                ErrorInfo = "Directory Not Found.";
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                ErrorInfo = "Access Denied.";
                return false;
            }
        }

        //************ Purchasing ********************//
        /// <summary>
        /// Creates a record with the passed in values in the Purchases table
        /// </summary>
        /// <param name="distID">Disctrict ID</param>
        /// <param name="invDate">Invoice Date</param>
        /// <param name="postDate">Post Date</param>
        /// <param name="vendID">Vendor ID</param>
        /// <param name="invNum">Invoice Number</param>
        /// <param name="groc">Grocery Amount</param>
        /// <param name="paper">Paper Amount</param>
        /// <param name="clean">Cleaning Amount</param>
        /// <param name="other">Other Amount</param>
        /// <param name="otherCat">Other Category</param>
        /// <param name="comments">Comments</param>
        /// <param name="milk">Milk Amount</param>
        /// <param name="schoolNum">School Num</param>
        /// <param name="programYear">Program Year</param>
        /// <param name="TextBoxStatus">referece to the Status TextBox</param>
        /// <returns>Boolean indicating if the record was created</returns>
        protected Boolean PurchasesInsertRecord(String distID, DateTime invDate, DateTime postDate, String vendID, String invNum, Decimal groc, Decimal paper, Decimal clean, Decimal other,
            String otherCat, String comments, Decimal milk, Int16 schoolNum, String programYear, ref TextBox _TextBoxStatus, Boolean VerboseErrors, Byte Locked)
        {
            Boolean RecordCreated = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [Purchases] ([DistrictID],[InvoiceDate],[PostDate],[VendorID],[InvoiceNumber],[Groceries],[Paper],[Cleaning],[Other] " +
                                        " ,[OtherCategoryID],[Imported],[ImportDate],[PurchaseComments],[Milk],[SchoolNum],[ProgramYear],[Locked]) "
                                        + " VALUES(@DistrictID,@InvoiceDate,@PostDate,@VendorID,@InvoiceNumber,@Groceries,@Paper,@Cleaning,@Other " +
                                        " ,@OtherCategoryID,0,'1/1/1900',@PurchaseComments,@Milk,@SchoolNum,@ProgramYear,@Locked )";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_dist = new SqlParameter();
                dbParam_dist.ParameterName = "@DistrictID";
                dbParam_dist.Value = distID;
                dbParam_dist.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_dist);

                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = invDate;
                dbParam_InvDate.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_PostDate = new SqlParameter();
                dbParam_PostDate.ParameterName = "@PostDate";
                dbParam_PostDate.Value = postDate;
                dbParam_PostDate.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_PostDate);

                SqlParameter dbParam_vendID = new SqlParameter();
                dbParam_vendID.ParameterName = "@VendorID";
                dbParam_vendID.Value = vendID;
                dbParam_vendID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_vendID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = invNum;
                dbParam_InvNum.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_groc = new SqlParameter();
                dbParam_groc.ParameterName = "@Groceries";
                dbParam_groc.Value = groc;
                dbParam_groc.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_groc);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = paper;
                dbParam_Paper.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Cleaning";
                dbParam_Clean.Value = clean;
                dbParam_Clean.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = other;
                dbParam_Other.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_othCat = new SqlParameter();
                dbParam_othCat.ParameterName = "@OtherCategoryID";
                dbParam_othCat.Value = otherCat;
                dbParam_othCat.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_othCat);

                SqlParameter dbParam_comments = new SqlParameter();
                dbParam_comments.ParameterName = "@PurchaseComments";
                dbParam_comments.Value = comments;
                dbParam_comments.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_comments);

                SqlParameter dbParam_milk = new SqlParameter();
                dbParam_milk.ParameterName = "@Milk";
                dbParam_milk.Value = milk;
                dbParam_milk.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_milk);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = schoolNum;
                dbParam_schoolNum.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = programYear;
                dbParam_progYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_Locked = new SqlParameter();
                dbParam_Locked.ParameterName = "@Locked";
                dbParam_Locked.Value = Locked;
                dbParam_Locked.DbType = System.Data.DbType.Byte;
                cmd.Parameters.Add(dbParam_Locked);

                try
                {
                    dbConnection.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                        RecordCreated = true;
                    else
                        RecordCreated = false;

                }
                catch(SqlException ex)
                {
                    RecordCreated = false;

                    if (VerboseErrors)
                    {
                        _TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        _TextBoxStatus.Rows++;
                        _TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    RecordCreated = false;

                    if (VerboseErrors)
                    {
                        _TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        _TextBoxStatus.Rows++;
                        _TextBoxStatus.Rows++;
                    }
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return RecordCreated;
        }

        /// <summary>
        /// Checks configured Other Categories for Purchases against the BDGTS database and adds them is a District is budgeted to use
        /// </summary>
        /// <param name="ProgramYear">Program Year</param>
        /// <returns></returns>
        protected int ImportCategoryRecords(string ProgramYear, string BudgetYear)
        {
            int RecordsAffected = 0;

            string ConnStringBDGTS = System.Configuration.ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "INSERT INTO [DistrictAllowedOtherCats]( [ProgramYear],[DistrictID],[OtherCategoryID]) " +
                " SELECT '" + ProgramYear + "' , MyTable.BidNumber , MyTable.CATEGORIES FROM OPENROWSET('SQLNCLI','" + ConnStringBDGTS + "', 'EXEC [dbo].[Papwerworkweb_DistAllowedOCats] @BudgetYear = N''" + BudgetYear + "'' WITH RESULT SETS " +
                "((" +
                    " BidNumber nvarchar(5), CATEGORIES nvarchar(5) " +
                ") " +
                ") ') MyTable WHERE '" + ProgramYear + "' + MyTable.BidNumber + MyTable.CATEGORIES NOT IN (SELECT ProgramYear + DistrictID + OtherCategoryID FROM [DistrictAllowedOtherCats]); ";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                dbConnection.Open();
                try
                {
                    RecordsAffected = cmd.ExecuteNonQuery();
                }
                catch(SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while synchronizing Purchase Category records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Exception occurred while synchronizing Purchase Category records. ", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return RecordsAffected;
        }

        /// <summary>
        /// Checks configured Other Categories for Purchases against the BDGTS database and removes them is a District is not budgeted to use
        /// </summary>
        /// <param name="ProgramYear">Program Year</param>
        /// <returns></returns>
        protected int RemoveCategoryRecords(string ProgramYear, string BudgetYear)
        {
            int RecordsAffected = 0;

            string ConnStringBDGTS = System.Configuration.ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            //REMOVE RECORDS 
            string queryString = " DELETE FROM [DistrictAllowedOtherCats] " +
            " WHERE ProgramYear='" + ProgramYear + "' AND ProgramYear + DistrictID + OtherCategoryID NOT IN( " +
            " SELECT '" + ProgramYear + "' + MyTable.BidNumber + MyTable.CATEGORIES FROM  OPENROWSET('SQLNCLI','" + ConnStringBDGTS + "', 'EXEC [dbo].[Papwerworkweb_DistAllowedOCats] @BudgetYear = N''" + BudgetYear + "'' WITH RESULT SETS " +
            "(( " +
            " BidNumber nvarchar(5), CATEGORIES nvarchar(5) " +
            ")) ') MyTable )";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                dbConnection.Open();
                try
                {
                    RecordsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("ERROR", "alert('An error was encountered while removing unneeded Purchase Other Categories.');");
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return RecordsAffected;
        }

        /// <summary>
        /// Allows you to specify the root or containing control, and the routine will search for and return
        /// a reference to the desired control.
        /// </summary>
        /// <param name="rootControl">The control which contains the control you are looking for.</param>
        /// <param name="controlID">String containing the name of the control you are looking for.</param>
        /// <returns></returns>
        protected Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn =
                    FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }

        protected string GetLastDistrictID()
        {
            string OutDistID = "";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.IDbConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT LastDistrictID FROM UserAccess WHERE UPPER(UserID)=@UserID";

            using (System.Data.IDbCommand dbCommand = new System.Data.SqlClient.SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                System.Data.IDataParameter dbParam_uSERID = new System.Data.SqlClient.SqlParameter();
                dbParam_uSERID.ParameterName = "@UserID";
                dbParam_uSERID.Value = Context.User.Identity.Name.ToUpper();
                dbParam_uSERID.DbType = System.Data.DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_uSERID);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object lastComm = dbCommand.ExecuteScalar();
                    if (lastComm != null)
                        OutDistID = ((string)lastComm).ToUpper();
                }
                catch (Exception)
                {
                    OutDistID = "";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return OutDistID;
        }

        protected void SetLastDistrictID(string dID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.IDbConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "UPDATE UserAccess SET LastDistrictID=@LastDistrictID WHERE UPPER(UserID)=@UserID";

            using (IDbCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                System.Data.IDataParameter dbParam_distID = new System.Data.SqlClient.SqlParameter();
                dbParam_distID.ParameterName = "@LastDistrictID";
                dbParam_distID.Value = dID.ToUpper();
                dbParam_distID.DbType = System.Data.DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_distID);

                System.Data.IDataParameter dbParam_uSERID = new System.Data.SqlClient.SqlParameter();
                dbParam_uSERID.ParameterName = "@UserID";
                dbParam_uSERID.Value = Context.User.Identity.Name.ToUpper();
                dbParam_uSERID.DbType = System.Data.DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_uSERID);

                dbConnection.Open();
                try
                {
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {

                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected string GetGlobalSettingString(string SettingName)
        {
            string OutSSRSUrl = "";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT ["+ SettingName + "] FROM [PaperworkGlobalSettings] WHERE [Index]=0";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                dbConnection.Open();
                try
                {
                    object urlString = dbCommand.ExecuteScalar();
                    if (urlString != null)
                        OutSSRSUrl = ((string)urlString).ToUpper();
                }
                catch (Exception ex)
                {
                    OutSSRSUrl = "";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return OutSSRSUrl;
        }

        protected void UpdateDaysTable(DateTime ServingDate, String DistID)
        {
            int existingRecordCount = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
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
                dbParam_dist.Value = DistID;
                dbParam_dist.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_dist);

                SqlParameter dbParam_srvDate = new SqlParameter();
                dbParam_srvDate.ParameterName = "@ServingDate";
                dbParam_srvDate.Value = ServingDate;
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
                    return;
                }
                catch (Exception ex)
                {
                    return;
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            string updateCreateDayRecord = "";
            if (existingRecordCount == 0)//Create new if needed
            {
                //Insert
                updateCreateDayRecord = "INSERT INTO [Days] ([ProgramYear], [DistrictID], [ServingDate], [BreakfastDay], [LunchDay]) " +
                    " SELECT @ProgramYear, @DistrictID, @RecapDate,  " +
                    " ISNULL(MAX([BreakfastDay]),0) BreakfastDay, ISNULL(MAX([LunchDay]),0) LunchDay " +
                    " FROM [dbo].[RecapEntry] " +
                    " WHERE [ProgramYear]=@ProgramYear AND [DistrictID] = @DistrictID AND [RecapDate]=@RecapDate";
    }
            else
            {
                //UPDATE
                updateCreateDayRecord = "UPDATE [Days] SET [BreakfastDay]=ISNULL(BreakfastDayRecap,0), [LunchDay]=ISNULL(LunchDayRecap,0) " +
                    " FROM [Days] " +
                    " LEFT OUTER JOIN (SELECT ProgramYear, DistrictID, RecapDate, " +
                    " ISNULL(MAX([BreakfastDay]), 0) BreakfastDayRecap, ISNULL(MAX([LunchDay]),0) LunchDayRecap " +
                    " FROM [RecapEntry] GROUP BY ProgramYear, DistrictID, RecapDate" +
                    " ) Recaps ON [Days].ProgramYear=Recaps.ProgramYear AND Days.DistrictID=Recaps.DistrictID AND Days.ServingDate=Recaps.RecapDate " +
                    " WHERE Days.[ProgramYear]=@ProgramYear AND Days.[DistrictID]=@DistrictID AND Days.[ServingDate]=@RecapDate AND Locked=0 AND Overridden = 0 ";
                    //Only allowed to update if not locked and not already overridden
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
                dbParam_dist.Value = DistID;
                dbParam_dist.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_dist);

                SqlParameter dbParam_srvDate = new SqlParameter();
                dbParam_srvDate.ParameterName = "@RecapDate";
                dbParam_srvDate.Value = ServingDate;
                dbParam_srvDate.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_srvDate);

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


        }

        //******** Date Related Functions *************//
        protected bool IsDateOpenForEntry(DateTime checkDate)
        {
            bool trxEntryAllowed = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string queryCheckDate = "SELECT Count(*) FROM [ProgramYear] " +
                " WHERE StartDate<=@CheckDate AND EndDate>=@CheckDate AND [Close" + checkDate.Month.ToString().PadLeft(2, '0') + "] = 0 AND AllowDataEntry = 1";
            using (SqlCommand cmd = new SqlCommand(queryCheckDate, sqlConnection))
            {
                SqlParameter dbParam_cHeckDate = new SqlParameter();
                dbParam_cHeckDate.ParameterName = "@CheckDate";
                dbParam_cHeckDate.Value = checkDate;
                dbParam_cHeckDate.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_cHeckDate);

                sqlConnection.Open();
                int openCount = 0;
                openCount = (int)cmd.ExecuteScalar();

                if (openCount > 0)
                    trxEntryAllowed = true;
                else
                    trxEntryAllowed = false;
            }


            return trxEntryAllowed;
        }

        /// <summary>
        /// Check to see if the District is enforcing period closures
        /// </summary>
        /// <param name="DistrictID">District ID</param>
        /// <returns>Boolean indicating if closures are being enforced</returns>
        protected bool IsDistrictEnforcingPeriodClosures(string DistrictID)
        {
            bool districtEnforcesPeriods = true;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.IDbConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT [EnforcePeriodClosures] FROM [District] WHERE DistrictID=@DistrictID";

            using (System.Data.IDbCommand dbCommand = new System.Data.SqlClient.SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                System.Data.IDataParameter dbParam_distID = new System.Data.SqlClient.SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DistrictID;
                dbParam_distID.DbType = System.Data.DbType.String;
                dbCommand.Parameters.Add(dbParam_distID);

                dbConnection.Open();
                try
                {
                    object enabled = dbCommand.ExecuteScalar();

                    if (enabled == null)
                        districtEnforcesPeriods = true;
                    else if ((byte)enabled == 0)//not enforce
                        districtEnforcesPeriods = false;
                    else if ((byte)enabled == 1)//enforced
                        districtEnforcesPeriods = true;
                }
                catch (Exception)
                {
                    districtEnforcesPeriods = true;
                }
                finally
                {
                    dbConnection.Close();
                }
            }
            return districtEnforcesPeriods;
        }

        /// <summary>
        /// Checks the transaction entry date to ensure it falls within the current program year. This also confirms any weekend dates which are selected
        /// </summary>
        /// <param name="TrxDate">Textbox field containing a date</param>
        protected void ValidateTrxEntryDate(ref TextBox TrxDate, Boolean WarnWeekends)
        {
            // if the program year session variables are not set, just return
            if (Session["ProgramYearStartDate"] == null || Session["ProgramYearEndDate"] == null)
            {
                return;
            }
            DateTime UseDate;
            if (DateTime.TryParse(TrxDate.Text, out UseDate))
            {

                if (UseDate >= (DateTime)Session["ProgramYearStartDate"] && UseDate <= (DateTime)Session["ProgramYearEndDate"])
                { /*Do nothing */}
                else if (UseDate < (DateTime)Session["ProgramYearStartDate"])
                {
                    UseDate = (DateTime)Session["ProgramYearStartDate"];
                    TrxDate.Text = UseDate.ToShortDateString();
                    RegisterDOMReadyScript("Date Error", "alert('The date falls before the beginning of the Program Year. Date has been changed to the start of the program year.');");
                }
                else if (UseDate > (DateTime)Session["ProgramYearEndDate"])
                {
                    UseDate = (DateTime)Session["ProgramYearEndDate"];
                    TrxDate.Text = UseDate.ToShortDateString();
                    RegisterDOMReadyScript("Date Error", "alert('The date falls after the end of the Program Year. Date has been changed to the last day of the program year.');");
                }

                if (WarnWeekends == true && (UseDate.DayOfWeek == DayOfWeek.Saturday || UseDate.DayOfWeek == DayOfWeek.Sunday))
                {
                    RegisterDOMReadyScript("Date Issue", "alert('The date is not a weekday - is it correct?');$('[id *= TrxDate]').addClass('highlight');");
                }
                else
                    RegisterDOMReadyScript("Date Issue", "$('[id *= TrxDate]').removeClass('highlight');");
            }
        }

        /// <summary>
        /// Allows users to enter all digits for date fields and changes to standard date format
        /// </summary>
        /// <param name="dateTextBox">Reference to a Date TextBox</param>
        protected void NormalizeSimplifiedDate(ref TextBox dateTextBox)
        {
            //check to see if it's all digits
            if (Regex.IsMatch(dateTextBox.Text, @"^\d+$"))
            {
                //Day only - use current month & year
                if (dateTextBox.Text.Length == 1)
                    dateTextBox.Text = DateTime.Now.Month.ToString() + "/" + "0" + dateTextBox.Text + "/" + DateTime.Now.Year.ToString();
                else if (dateTextBox.Text.Length == 2)
                    dateTextBox.Text = DateTime.Now.Month.ToString() + "/" +  dateTextBox.Text + "/" + DateTime.Now.Year.ToString();
                //Day and Month - use current year
                else if (dateTextBox.Text.Length == 4)
                    dateTextBox.Text = dateTextBox.Text.Substring(0, 2) + "/" + dateTextBox.Text.Substring(2, 2) + "/" + DateTime.Now.Year.ToString();
                //2-digit year
                else if (dateTextBox.Text.Length == 6)
                    dateTextBox.Text = dateTextBox.Text.Substring(0, 2) + "/" + dateTextBox.Text.Substring(2, 2) + "/20" + dateTextBox.Text.Substring(4, 2);
                //4-digit year
                else if (dateTextBox.Text.Length == 8)
                    dateTextBox.Text = dateTextBox.Text.Substring(0, 2) + "/" + dateTextBox.Text.Substring(2, 2) + "/" + dateTextBox.Text.Substring(4, 4);
            }
        }

        protected DateTime GetFinalPostingDateForMonth(int MonthNum, int Year)
        {
            DateTime returnDate;
            DateTime EOMDate = new DateTime(Year, MonthNum, 1).AddMonths(1).AddDays(-1);

            if (EOMDate.DayOfWeek < DayOfWeek.Friday || EOMDate.DayOfWeek == DayOfWeek.Saturday || MonthNum == 12)
            {
                //Exception - December needs to be before Christmas
                if(MonthNum==12)
                    returnDate = new DateTime(EOMDate.Year, 12, 24);//Set to Christmas Eve and work backwards
                else
                    returnDate = EOMDate;

                while (returnDate.DayOfWeek != DayOfWeek.Friday)
                {
                    returnDate = returnDate.AddDays(-1);
                }

            }
            else
            {
                returnDate = EOMDate;
            }

            return returnDate;
        }

        /// <summary>
        /// Makes a GridView row clickable
        /// </summary>
        /// <param name="sender">GridView</param>
        /// <param name="e"></param>
        protected void GridviewClickableRowDatabound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            GridView gv = (GridView)sender;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Note for this code to work properly - the page with the Selectable GridView must have the following in the header:
                // EnableEventValidation="false"
                // Due to Master page - the unique id generated base ctl01 is different then the actual ctl00
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv, "Select$" + e.Row.RowIndex);
                e.Row.ToolTip = "Click to select this row.";
            }
        }

        //***** JQUERY RELATED SCRIPTS *****//

        /// <summary>
        /// Runs a JQuery or Javascript function after the webpage DOM has loaded.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="script"></param>
        public void RegisterDOMReadyScript(string key, string script)
        {
            string enclosed = EncloseOnDOMReadyEvent(script);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), key, enclosed, true);
        }

        /// <summary>
        /// Works with RegisterDOMReadyScript to build the page script to be executed
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string EncloseOnDOMReadyEvent(string str)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("function r(f){/in/.test(document.readyState)?setTimeout('r('+f+')',9):f()} r(function(){")
                .Append(str)
                .Append("});");
            return sb.ToString();
        }

        public enum MessageType { Success, Error, Info, Warning };
        //ShowMessage allows you to interface with ShowMessage function in the jquery section of the page. Must be included.
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        //ShowMessagebyName allows you to interface with ShowMessagebyName function in the jquery section of the page. Allows you to specify a specific alert block. Jquery code Must be included.
        protected void ShowMessageByName(string Message, MessageType type, string AlertBlock, int Duration)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessageByName('" + Message + "','" + type + "','" + AlertBlock + "','" + Duration.ToString() + "');", true);
        }

        //ShowMessageIcon allows you to interface with ShowMessageIcon function in the jquery section of the page. Must be included. Shortened version of ShowMessage
        protected void ShowMessageIcon(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessageIcon('" + Message + "','" + type + "');", true);
        }

        /// <summary>
        /// Procedure streamlines the use of JQuery UI Dialog boxes
        /// </summary>
        /// <param name="Title">Title bar string</param>
        /// <param name="type">MessageType for background color</param>
        /// <param name="Synopsis">Basic dialog message string</param>
        /// <param name="Details">Detailed dialog message string</param>
        protected void ShowDialog(string Title, MessageType type, string Synopsis, string Details)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            StringBuilder sb = new StringBuilder();
            sb.Append(Synopsis);
            if(string.IsNullOrEmpty(Details) == false)
            {
                sb.Append("<br /><strong>Details:</strong> ");
                sb.Append(jss.Serialize(Details));
            }

            string ErrorDetails = jss.Serialize(Details);
            RegisterDOMReadyScript("Popup", "ShowPopup('" + Title + "', '" + sb.ToString() + " ', '" + type + "');");
        }

    }

}
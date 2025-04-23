using Microsoft.Ajax.Utilities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupSchools : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                VerifyAccess(true);
            }
            else
            {
                VerifyAccess(false);
            }

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if(!IsPostBack)
            {
                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;

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

                //Compare records against the budget for missing schools
                ImportMissingBDGTSchools();
            }
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                Response.Redirect("~/Setup/SetupSchools.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void CB_Inactive_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);
            GridView GVSchools = ((GridView)gvr.Parent.Parent);
            string schoolNum = GVSchools.DataKeys[gvr.RowIndex]["SchoolNum"].ToString();
            string distID = GVSchools.DataKeys[gvr.RowIndex]["DistrictID"].ToString(); 

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string queryUpdateSchool = "UPDATE [DistrictSchools] SET Inactive=@Inactive WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum ";

            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                SqlCommand cmdUpdateInv = new SqlCommand(queryUpdateSchool, dbConnection);

                SqlParameter dbParam_inactive = new SqlParameter();
                dbParam_inactive.ParameterName = "@Inactive";
                dbParam_inactive.Value = checkFieldStr;
                dbParam_inactive.DbType = System.Data.DbType.Byte;
                cmdUpdateInv.Parameters.Add(dbParam_inactive);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = distID;
                dbParam_distID.DbType = System.Data.DbType.StringFixedLength;
                cmdUpdateInv.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNumber = new SqlParameter();
                dbParam_schoolNumber.ParameterName = "@SchoolNum";
                dbParam_schoolNumber.Value = schoolNum;
                dbParam_schoolNumber.DbType = System.Data.DbType.Int16;
                cmdUpdateInv.Parameters.Add(dbParam_schoolNumber);

                cmdUpdateInv.ExecuteNonQuery();
                cmdUpdateInv.Dispose();
            }

            //if ord change pending, run the save before databinding
            if (HFOrdChangePending.Value == "1")
                ISaveOrder_Click(null, null);

            //update the Gridview and redraw it and the Details View
            GVSchools.DataBind();
        }

        /// <summary>
        /// Creates matching school in the budget program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SqlDataSourceSchools_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e == null || e.AffectedRows == 0)
            {
                if(e.Exception != null)
                {
                    if(e.Exception.Message.IndexOf("duplicate")>=0)
                        ShowDialog("Error", MessageType.Error, "School could not be created in Paperwork.", "Duplicate found.");
                    else
                        ShowDialog("Error", MessageType.Error, "School was not created in Paperwork.", e.Exception.Message);
                    e.ExceptionHandled = true;
                }
                return;
            }

            //Using BaseFolder to check for training company
            //Do not want to create new schools in GP when using the training database
            string SSRSBaseFolder = GetGlobalSettingString("SSRS_BaseFolder");

            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING") > 0)
            {
                return;
            }

            if (Session["BudgetYear"] == null || string.IsNullOrEmpty(Session["BudgetYear"].ToString()))
                return;//cannot check for or insert schools without valid budget info

            //Due to the need to insert schools on the Paperwork side - we need to also ensure the same school is created in Budgets
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryInsertBDGTSchool = "INSERT INTO [BDGT_Sites] ([BudgetYear],[BidNumber],[ORD],[BDGTSiteName],[BDGTSiteAddress],[GradeLevels],[DDL_PrepType],[NumServingPeriods],[TimesBfast],[TimesLunch],[TimesPMSnack],[CBMealBFast]" +
                                            ",[CBMealLunch],[CBOfferVServeB],[CBOfferVServeL],[CBAlaBfast],[CBAlaLunch],[CBAdultBfast],[CBAdultLunch],[CBAfterSchoolSnack],[CBMilkKPreK],[DaysOfSvcSite],[DDL_SiteType]) " +
                                            " VALUES(@BudgetYear, @BidNumber, @ORD, @BDGTSiteName, '' ,'' ,0 ,0 ,'' ,'' ,'' ,0 " +
                                            " ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,@DDL_SiteType)";

            using (SqlCommand cmd = new SqlCommand(queryInsertBDGTSchool, dbConnection))
            {
                SqlParameter dbParam_BudYear = new SqlParameter();
                dbParam_BudYear.ParameterName = "@BudgetYear";
                dbParam_BudYear.Value = Session["BudgetYear"].ToString();
                dbParam_BudYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_BudYear);

                SqlParameter dbParam_bid = new SqlParameter();
                dbParam_bid.ParameterName = "@BidNumber";
                dbParam_bid.Value = DropDownListDistricts.SelectedValue;
                dbParam_bid.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_bid);

                SqlParameter dbParam_SchlNum = new SqlParameter();
                dbParam_SchlNum.ParameterName = "@ORD";
                dbParam_SchlNum.Value = e.Command.Parameters["@SchoolNum"].Value.ToString();
                dbParam_SchlNum.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_SchlNum);

                SqlParameter dbParam_siteName = new SqlParameter();
                dbParam_siteName.ParameterName = "@BDGTSiteName";
                dbParam_siteName.Value = e.Command.Parameters["@SchoolName"].Value.ToString();
                dbParam_siteName.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_siteName);

                SqlParameter dbParam_siteType = new SqlParameter();
                dbParam_siteType.ParameterName = "@DDL_SiteType";
                dbParam_siteType.Value = e.Command.Parameters["@SchoolType"].Value.ToString();
                dbParam_siteType.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_siteType);

                int recordCount = 0;

                try
                {
                    dbConnection.Open();
                    Object obj = cmd.ExecuteNonQuery();
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out recordCount);
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "School was created in Paperwork, but could not be created in the Budget Program.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        /// <summary>
        /// Updates school in the budget program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SqlDataSourceSchools_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e == null || e.AffectedRows == 0)
                return;

            if (Session["BudgetYear"] == null || string.IsNullOrEmpty(Session["BudgetYear"].ToString()))
                return;//cannot check for or insert schools without valid budget info

            //Due to the need to insert schools on the Paperwork side - we need to also ensure the same school is created in Budgets
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryUpdateBDGTSchool = "UPDATE [BDGT_Sites] SET [BDGTSiteName]=@BDGTSiteName, [DDL_SiteType]=@DDL_SiteType " +
                                " WHERE [BudgetYear]=@BudgetYear AND [BidNumber]=@BidNumber AND [ORD]= @ORD";

            using (SqlCommand cmd = new SqlCommand(queryUpdateBDGTSchool, dbConnection))
            {
                SqlParameter dbParam_BudYear = new SqlParameter();
                dbParam_BudYear.ParameterName = "@BudgetYear";
                dbParam_BudYear.Value = Session["BudgetYear"].ToString();
                dbParam_BudYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_BudYear);

                SqlParameter dbParam_bid = new SqlParameter();
                dbParam_bid.ParameterName = "@BidNumber";
                dbParam_bid.Value = DropDownListDistricts.SelectedValue;
                dbParam_bid.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_bid);

                SqlParameter dbParam_SchlNum = new SqlParameter();
                dbParam_SchlNum.ParameterName = "@ORD";
                dbParam_SchlNum.Value = e.Command.Parameters["@SchoolNum"].Value.ToString();
                dbParam_SchlNum.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_SchlNum);

                SqlParameter dbParam_siteName = new SqlParameter();
                dbParam_siteName.ParameterName = "@BDGTSiteName";
                dbParam_siteName.Value = e.Command.Parameters["@SchoolName"].Value.ToString();
                dbParam_siteName.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_siteName);

                SqlParameter dbParam_siteType = new SqlParameter();
                dbParam_siteType.ParameterName = "@DDL_SiteType";
                dbParam_siteType.Value = e.Command.Parameters["@SchoolType"].Value.ToString();
                dbParam_siteType.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_siteType);

                int recordCount = 0;

                try
                {
                    dbConnection.Open();
                    Object obj = cmd.ExecuteNonQuery();
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out recordCount);
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "School was updated in Paperwork, but could not be updated in the Budget Program.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }

        }

        /// <summary>
        /// Imports missing schools into PaperworkWeb from the Budgets Program
        /// </summary>
        protected void ImportMissingBDGTSchools()
        {
            if (Session["BudgetYear"] == null || string.IsNullOrEmpty(Session["BudgetYear"].ToString()))
                return;//cannot check for or insert schools without valid budget info

            //Due to the need to insert schools on the Paperwork side - we need to also ensure the same school is created in Budgets
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryInsertBDGTSchool = "SELECT * FROM [BDGT_Sites] " +
                                            " WHERE [BudgetYear]=@BudgetYear AND [BidNumber]=@BidNumber  ";

            using (SqlCommand cmd = new SqlCommand(queryInsertBDGTSchool, dbConnection))
            {
                SqlParameter dbParam_BudYear = new SqlParameter();
                dbParam_BudYear.ParameterName = "@BudgetYear";
                dbParam_BudYear.Value = Session["BudgetYear"].ToString();
                dbParam_BudYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_BudYear);

                SqlParameter dbParam_bid = new SqlParameter();
                dbParam_bid.ParameterName = "@BidNumber";
                dbParam_bid.Value = DropDownListDistricts.SelectedValue;
                dbParam_bid.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_bid);

                try
                {
                    dbConnection.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        string schoolName = rdr["BDGTSiteName"].ToString().Trim();
                        Int32 schoolNum = (Int32)rdr["ORD"];
                        Int16 schoolType = (Int16)rdr["DDL_SiteType"];
                        VerifySchool(schoolNum, schoolType, schoolName);
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "School was created in Paperwork, but could not be created in the Budget Program.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

        }

        /// <summary>
        /// Checks to see if the school exists in PaperworkWeb. If so, it validates the name and type, and updates PaperworkWeb if different.
        /// </summary>
        /// <param name="schoolNum">School Number key</param>
        /// <param name="schoolType">School Type</param>
        /// <param name="schoolName">School Name</param>
        private void VerifySchool(Int32 schoolNum, short schoolType, string schoolName)
        {
            if (DropDownListDistricts.Items.Count == 0 || DropDownListDistricts.SelectedIndex == -1)
                return;

            //Due to the need to insert schools on the Paperwork side - we need to also ensure the same school is created in Budgets
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string querySelectSchool = "SELECT * FROM [DistrictSchools] " +
                                       " WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum  ";

            using (SqlCommand cmd = new SqlCommand(querySelectSchool, dbConnection))
            {
                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DropDownListDistricts.SelectedValue;
                dbParam_distID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = schoolNum;
                dbParam_schoolNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_schoolNum);

                try
                {
                    dbConnection.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())//school num found - validate name / type
                    {
                        string PPWschoolName = rdr["SchoolName"].ToString();
                        Int16 PPWschoolType = (Int16)rdr["SchoolType"];

                        if(PPWschoolName!=schoolName || PPWschoolType!=schoolType)
                        {
                            SqlConnection dbConnectionUpdatePPW = new SqlConnection(connectionString);
                            string queryUpdatePPWSchool = "UPDATE [DistrictSchools] SET SchoolName=@SchoolName, SchoolType=@SchoolType " +
                                       " WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum ";

                            using (SqlCommand cmdUpdatePPW = new SqlCommand(queryUpdatePPWSchool, dbConnectionUpdatePPW))
                            {
                                SqlParameter dbParam_distID3 = new SqlParameter();
                                dbParam_distID3.ParameterName = "@DistrictID";
                                dbParam_distID3.Value = DropDownListDistricts.SelectedValue;
                                dbParam_distID3.DbType = System.Data.DbType.StringFixedLength;
                                cmdUpdatePPW.Parameters.Add(dbParam_distID3);

                                SqlParameter dbParam_schoolNum3 = new SqlParameter();
                                dbParam_schoolNum3.ParameterName = "@SchoolNum";
                                dbParam_schoolNum3.Value = schoolNum;
                                dbParam_schoolNum3.DbType = System.Data.DbType.Int32;
                                cmdUpdatePPW.Parameters.Add(dbParam_schoolNum3);

                                SqlParameter dbParam_schoolName3 = new SqlParameter();
                                dbParam_schoolName3.ParameterName = "@SchoolName";
                                dbParam_schoolName3.Value = schoolName.Trim();
                                dbParam_schoolName3.DbType = System.Data.DbType.StringFixedLength;
                                cmdUpdatePPW.Parameters.Add(dbParam_schoolName3);

                                SqlParameter dbParam_schoolType3 = new SqlParameter();
                                dbParam_schoolType3.ParameterName = "@SchoolType";
                                dbParam_schoolType3.Value = schoolType;
                                dbParam_schoolType3.DbType = System.Data.DbType.Int16;
                                cmdUpdatePPW.Parameters.Add(dbParam_schoolType3);

                                using (dbConnectionUpdatePPW)
                                {
                                    try
                                    {
                                        dbConnectionUpdatePPW.Open();
                                        int rowsaffected = cmdUpdatePPW.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    { }
                                    catch (Exception ex)
                                    { }
                                }
                            }
                        }
                    }
                    else//need to create school in Paperwork
                    {
                        SqlConnection dbConnectionInsertPPW = new SqlConnection(connectionString);
                        string queryInsertPPWSchool = "INSERT INTO [DistrictSchools] (DistrictID, SchoolNum, SchoolName, Inactive, SchoolType) " +
                                       " VALUES(@DistrictID, @SchoolNum, @SchoolName, 0, @SchoolType)  ";
                        using (SqlCommand cmdInsertPPW = new SqlCommand(queryInsertPPWSchool, dbConnectionInsertPPW))
                        {
                            SqlParameter dbParam_distID2 = new SqlParameter();
                            dbParam_distID2.ParameterName = "@DistrictID";
                            dbParam_distID2.Value = DropDownListDistricts.SelectedValue;
                            dbParam_distID2.DbType = System.Data.DbType.StringFixedLength;
                            cmdInsertPPW.Parameters.Add(dbParam_distID2);

                            SqlParameter dbParam_schoolNum2 = new SqlParameter();
                            dbParam_schoolNum2.ParameterName = "@SchoolNum";
                            dbParam_schoolNum2.Value = schoolNum;
                            dbParam_schoolNum2.DbType = System.Data.DbType.Int32;
                            cmdInsertPPW.Parameters.Add(dbParam_schoolNum2);

                            SqlParameter dbParam_schoolName2 = new SqlParameter();
                            dbParam_schoolName2.ParameterName = "@SchoolName";
                            dbParam_schoolName2.Value = schoolName.Trim();
                            dbParam_schoolName2.DbType = System.Data.DbType.StringFixedLength;
                            cmdInsertPPW.Parameters.Add(dbParam_schoolName2);

                            SqlParameter dbParam_schoolType2 = new SqlParameter();
                            dbParam_schoolType2.ParameterName = "@SchoolType";
                            dbParam_schoolType2.Value = schoolType;
                            dbParam_schoolType2.DbType = System.Data.DbType.Int16;
                            cmdInsertPPW.Parameters.Add(dbParam_schoolType2);

                            using (dbConnectionInsertPPW)
                            {
                                try
                                {
                                    dbConnectionInsertPPW.Open();
                                    int rowsaffected = cmdInsertPPW.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                { }
                                catch (Exception ex)
                                { }
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('School was created in Paperwork, but could not be created in the Budget Program. Details:' " + ex.Message + ");");
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void SqlDataSourceSchools_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //if ord change pending, run the save before inserting & databinding
            if (HFOrdChangePending.Value == "1")
                ISaveOrder_Click(null, null);

            if (e.Command.Parameters["@SchoolType"] != null && e.Command.Parameters["@SchoolType"].Value.ToString() == "0")
                e.Command.Parameters["@SchoolType"].Value = 1;
        }

        protected void GridViewSchools_RowEditing(object sender, GridViewEditEventArgs e)
        {
            
            //if ord change pending, run the save before editing 
            if (HFOrdChangePending.Value == "1")
            {
                GridView gvSchools = (GridView)sender;
                Int16 storedSchoolNum = (Int16)(gvSchools.DataKeys[e.NewEditIndex].Values["SchoolNum"]);

                ISaveOrder_Click(null, null);

                //Find the new location of the requested record after databind from change Order
                int newIndex = 0;
                foreach (GridViewRow gvr in gvSchools.Rows)
                {
                    DataKey schoolKey = gvSchools.DataKeys[gvr.DataItemIndex];
                    if (schoolKey != null && ((Int16)schoolKey["SchoolNum"]) == storedSchoolNum)
                    {
                        newIndex = gvr.DataItemIndex;
                        break;
                    }
                }

                //Update the edit location
                e.NewEditIndex = newIndex;
            }


            
        }

        protected void ISaveOrder_Click(object sender, EventArgs e)
        {
            if (Request.Form["HFSchoolNum"] == null)
                return;

            //if save already happened - return
            if (HFOrdChangePending.Value != "1")
                return;

            //need to have the hidden field
            //this grabs all the hidden field values in the order they are displayed in the page
            string[] schools = (from p in Request.Form["HFSchoolNum"].Split(',')
                               select p).ToArray();

            int newOrd = 100;
            foreach (string school in schools)
            {
                Int16.TryParse(school, out Int16 schoolInt);
                UpdateSchoolOrd(DropDownListDistricts.SelectedValue, schoolInt, newOrd);
                //UpdateSchoolOrd(cID, route, newOrd);
                newOrd += 100;
            }

            GridViewSchools.DataBind();

            HFOrdChangePending.Value = "0";
        }

        /// <summary>
        /// Updates the HDR Display Order with the supplied new number
        /// </summary>
        /// <param name="commissaryID">Commissary ID string</param>
        /// <param name="routeID">Route ID string</param>
        /// <param name="newOrd">New Ord integer</param>
        private void UpdateSchoolOrd(string DistrictID, Int16 SchoolNum, int NewOrd)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryStringRenumber = "UPDATE [DistrictSchools] SET [SortOrder]=@NewOrd WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum ";

            SqlCommand dbCommandPre = new SqlCommand();
            dbCommandPre.CommandText = queryStringRenumber;
            dbCommandPre.Connection = dbConnection;

            SqlParameter dbParam_newORD = new SqlParameter();
            dbParam_newORD.ParameterName = "@NewOrd";
            dbParam_newORD.Value = NewOrd;
            dbParam_newORD.DbType = System.Data.DbType.Int32;
            dbCommandPre.Parameters.Add(dbParam_newORD);

            SqlParameter dbParam_dIDPre = new SqlParameter();
            dbParam_dIDPre.ParameterName = "@DistrictID";
            dbParam_dIDPre.Value = DistrictID;
            dbParam_dIDPre.DbType = System.Data.DbType.StringFixedLength;
            dbCommandPre.Parameters.Add(dbParam_dIDPre);

            SqlParameter dbParam_schoolNum = new SqlParameter();
            dbParam_schoolNum.ParameterName = "@SchoolNum";
            dbParam_schoolNum.Value = SchoolNum;
            dbParam_schoolNum.DbType = System.Data.DbType.StringFixedLength;
            dbCommandPre.Parameters.Add(dbParam_schoolNum);

            try
            {
                dbConnection.Open();
                dbCommandPre.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShowDialog("Error", MessageType.Error, "Error while updating order.", ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }


        }

        protected string CannotDeleteReasons;

        /// <summary>
        /// This procedure will check applicable transaction tables in paperwork and related budgeting tables in the Budget program
        /// to determine if a school can be deleted.
        /// </summary>
        /// <param name="DistrictID">District ID string</param>
        /// <param name="SchoolNum">School number</param>
        /// <returns>Boolean indicating if the school can be deleted. This will only be true if there are no transaction or budget records associated.</returns>
        private bool CanSchoolBeDeleted(string DistrictID, Int16 SchoolNum)
        {
            bool canDelete = false;

            CannotDeleteReasons = string.Empty;

            bool SiteCounts = HasBDGTSiteCounts(DistrictID, SchoolNum);
            bool MMBTrxs = HasMMBTrx(DistrictID, SchoolNum);
            bool PayrollRecs = HasPayrollTrx(DistrictID, SchoolNum);
            bool PurchaseRecs = HasPurchTrx(DistrictID, SchoolNum);
            bool RecapRecs = HasRecaps(DistrictID, SchoolNum);

            if (SiteCounts == false && MMBTrxs == false && PayrollRecs == false && PurchaseRecs == false && RecapRecs == false)
            {
                canDelete = true;
            }

            return canDelete;
        }

        protected bool HasBDGTSiteCounts(string DistrictID, Int16 SchoolNum)
        {
            bool ThereAreSiteCounts = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryInsertBDGTSchool = "SELECT COUNT(*) FROM [BDGT_CBS_SiteCounts] " +
                                            " WHERE [BidNumber]=@BidNumber AND [ORD]=@ORD  ";

            using (SqlCommand cmd = new SqlCommand(queryInsertBDGTSchool, dbConnection))
            {
                SqlParameter dbParam_bid = new SqlParameter();
                dbParam_bid.ParameterName = "@BidNumber";
                dbParam_bid.Value = DistrictID;
                dbParam_bid.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_bid);

                SqlParameter dbParam_SchoolOrd = new SqlParameter();
                dbParam_SchoolOrd.ParameterName = "@ORD";
                dbParam_SchoolOrd.Value = SchoolNum;
                dbParam_SchoolOrd.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_SchoolOrd);

                try
                {
                    dbConnection.Open();

                    Object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out int schoolCount);

                        if (schoolCount > 0)
                        {
                            ThereAreSiteCounts = true;
                            CannotDeleteReasons += "<li>Budgeted Site Counts exist.</li>";
                        }
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error encountered checking budgets for site counts.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

            return ThereAreSiteCounts;

        }

        protected bool HasMMBTrx(string DistrictID, Int16 SchoolNum)
        {
            bool ThereAreMMBTrx = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryInsertBDGTSchool = "SELECT COUNT(*) FROM [MMBTransactions] " +
                                            " WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum  ";

            using (SqlCommand cmd = new SqlCommand(queryInsertBDGTSchool, dbConnection))
            {
                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchoolNum = new SqlParameter();
                dbParam_SchoolNum.ParameterName = "@SchoolNum";
                dbParam_SchoolNum.Value = SchoolNum;
                dbParam_SchoolNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_SchoolNum);

                try
                {
                    dbConnection.Open();

                    Object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out int schoolCount);

                        if (schoolCount > 0)
                        {
                            ThereAreMMBTrx = true;
                            CannotDeleteReasons += "<li>MMB Transactions exist.</li>";
                        }
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error encountered retrieving MMB Transaction records.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

            return ThereAreMMBTrx;

        }

        protected bool HasPayrollTrx(string DistrictID, Int16 SchoolNum)
        {
            bool ThereArePayrollTrx = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryInsertBDGTSchool = "SELECT COUNT(*) FROM [Payroll] " +
                                            " WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum  ";

            using (SqlCommand cmd = new SqlCommand(queryInsertBDGTSchool, dbConnection))
            {
                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchoolNum = new SqlParameter();
                dbParam_SchoolNum.ParameterName = "@SchoolNum";
                dbParam_SchoolNum.Value = SchoolNum;
                dbParam_SchoolNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_SchoolNum);

                try
                {
                    dbConnection.Open();

                    Object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out int schoolCount);

                        if (schoolCount > 0)
                        {
                            ThereArePayrollTrx = true;
                            CannotDeleteReasons += "<li>Payroll Records exist.</li>";
                        }
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error encountered retrieving Payroll records.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

            return ThereArePayrollTrx;

        }

        protected bool HasPurchTrx(string DistrictID, Int16 SchoolNum)
        {
            bool ThereArePurchaseTrx = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryInsertBDGTSchool = "SELECT COUNT(*) FROM [Purchases] " +
                                            " WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum  ";

            using (SqlCommand cmd = new SqlCommand(queryInsertBDGTSchool, dbConnection))
            {
                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchoolNum = new SqlParameter();
                dbParam_SchoolNum.ParameterName = "@SchoolNum";
                dbParam_SchoolNum.Value = SchoolNum;
                dbParam_SchoolNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_SchoolNum);

                try
                {
                    dbConnection.Open();

                    Object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out int schoolCount);

                        if (schoolCount > 0)
                        {
                            ThereArePurchaseTrx = true;
                            CannotDeleteReasons += "<li>Transmittal Records exist.</li>";
                        }
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error encountered retrieving Transmittal records.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

            return ThereArePurchaseTrx;

        }

        protected bool HasRecaps(string DistrictID, Int16 SchoolNum)
        {
            bool ThereAreRecaps = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryInsertBDGTSchool = "SELECT COUNT(*) FROM [RecapEntry] " +
                                            " WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum  ";

            using (SqlCommand cmd = new SqlCommand(queryInsertBDGTSchool, dbConnection))
            {
                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchoolNum = new SqlParameter();
                dbParam_SchoolNum.ParameterName = "@SchoolNum";
                dbParam_SchoolNum.Value = SchoolNum;
                dbParam_SchoolNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_SchoolNum);

                try
                {
                    dbConnection.Open();

                    Object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out int schoolCount);

                        if (schoolCount > 0)
                        {
                            ThereAreRecaps = true;
                            CannotDeleteReasons += "<li>Sales Records exist.</li>";
                        }
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error encountered retrieving Sales records.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

            return ThereAreRecaps;

        }

        protected void GridViewSchools_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridView gvSchools = (GridView)sender;
            Int16 currentSchoolNum = (Int16)(gvSchools.DataKeys[e.RowIndex].Values["SchoolNum"]);
            String currentDistrict = (string)(gvSchools.DataKeys[e.RowIndex].Values["DistrictID"]);

            if(CanSchoolBeDeleted(currentDistrict, currentSchoolNum)==false)
            {
                e.Cancel = true;
                ShowDialog("Cannot Delete", MessageType.Error, CannotDeleteReasons, "");
            }

        }

        protected void SqlDataSourceSchools_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e == null || e.AffectedRows == 0)
                return;

            if (Session["BudgetYear"] == null || string.IsNullOrEmpty(Session["BudgetYear"].ToString()))
                return;//cannot check for or insert schools without valid budget info

            //Due to the need to insert schools on the Paperwork side - we need to also ensure the same school is created in Budgets
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryInsertBDGTSchool = "DELETE [BDGT_Sites] WHERE [BudgetYear]=@BudgetYear AND [BidNumber]=@BidNumber AND [ORD]=@ORD";

            using (SqlCommand cmd = new SqlCommand(queryInsertBDGTSchool, dbConnection))
            {
                try
                {

                    SqlParameter dbParam_BudYear = new SqlParameter();
                    dbParam_BudYear.ParameterName = "@BudgetYear";
                    dbParam_BudYear.Value = Session["BudgetYear"].ToString();
                    dbParam_BudYear.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_BudYear);

                    SqlParameter dbParam_bid = new SqlParameter();
                    dbParam_bid.ParameterName = "@BidNumber";
                    dbParam_bid.Value = DropDownListDistricts.SelectedValue;
                    dbParam_bid.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_bid);

                    SqlParameter dbParam_SchlNum = new SqlParameter();
                    dbParam_SchlNum.ParameterName = "@ORD";
                    dbParam_SchlNum.Value = e.Command.Parameters["@SchoolNum"].Value.ToString();
                    dbParam_SchlNum.DbType = System.Data.DbType.Int16;
                    cmd.Parameters.Add(dbParam_SchlNum);

                    int recordCount = 0;


                    dbConnection.Open();
                    Object obj = cmd.ExecuteNonQuery();
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out recordCount);
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "School could not be deleted in the budget program.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }

        }
    }
}
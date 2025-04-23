using Microsoft.Ajax.Utilities;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public class Paperwork_Global : Page
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

        protected bool isSFSP()
        {
            string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"].ToUpper();
            bool isSFSP = false;

            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBSFSP") > 0)
            {
                isSFSP = true;
            }

            return isSFSP;
        }

        protected bool isTOOLBOX()
        {
            string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"].ToUpper();
            bool isToolbox = false;

#if DEBUG
            isToolbox = true;
#else
            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKTOOLBOX") > 0)
            {
                isToolbox = true;
            }
#endif

            return isToolbox;
        }

        protected bool isTRAINING()
        {
            string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"].ToUpper();
            bool isTRAINING = false;

            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING") > 0)
            {
                isTRAINING = true;
            }

            return isTRAINING;
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
        /// Checks to see if access has been explicitly granted or denied for a specific security object. Explicit denials will override grants.
        /// </summary>
        /// <param name="SecurityObject"></param>
        /// <param name="explicitGrant"></param>
        /// <param name="explicitDeny"></param>
        protected void CheckCustomAccess(string SecurityObject, ref bool explicitGrant, ref bool explicitDeny)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT AccessAllowed, AccessDenied FROM [UserAccessCustomSecurity] WHERE UPPER(UserID)=@UserID AND UPPER(SecurityObject)=@SecurityObject;";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_uSERID = new SqlParameter();
                dbParam_uSERID.ParameterName = "@UserID";
                dbParam_uSERID.Value = Context.User.Identity.Name.ToUpper();
                dbParam_uSERID.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_uSERID);

                SqlParameter dbParam_securityObject = new SqlParameter();
                dbParam_securityObject.ParameterName = "@SecurityObject";
                dbParam_securityObject.Value = SecurityObject.ToUpper();
                dbParam_securityObject.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_securityObject);

                dbConnection.Open();
                try
                {
                    SqlDataReader dataReader = dbCommand.ExecuteReader();
                    if (dataReader.Read())
                    {
                        if ((bool)dataReader["AccessAllowed"] == true)
                        {
                            explicitGrant = true;
                        }
                        if ((bool)dataReader["AccessDenied"] == true)
                        {
                            explicitGrant = false;
                            explicitDeny = true;//deny overrides grants
                        }

                    }

                    if (dataReader != null)
                        dataReader.Close();
                }
                catch (Exception ex)
                {
                    Session["AccessLevel"] = "Unauthorized";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

        }

        public static void UpdateMasterMenuStructure (SiteMaster Master)
        {
            if (Master == null)
                return;

            string CurrentUser = "";
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;

            if (Master.Session["CurrentUser"] == null)
                return;
            else
                CurrentUser = Master.Session["CurrentUser"].ToString().ToUpper(cultureInfo);

            //Cannot call isSFSP directly. Need the following code to help with SFSP specific menu settings
            string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"].ToUpper();
            bool isSFSP = false;
            bool isToolbox = false;
            bool isTraining = false;
            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBSFSP") > 0)
                isSFSP = true;
            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKTOOLBOX") > 0)
                isToolbox = true;
            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING") > 0)
                isTraining = true;



            #region Transactions
            string sectionID = "Transactions/*";
            Master.link_PurchasesVisibility = DisplayInMenu(CurrentUser, sectionID, "Purchases", Master.link_PurchasesVisibility);
            Master.link_SalesEntryVisibility =  DisplayInMenu(CurrentUser, sectionID, "SalesEntry", Master.link_SalesEntryVisibility);
            Master.link_DistrictPayrollEntryVisibility = DisplayInMenu(CurrentUser, sectionID, "DistrictPayrollEntry", Master.link_DistrictPayrollEntryVisibility);
            Master.link_InventoryEntryVisibility = DisplayInMenu(CurrentUser, sectionID, "InventoryEntry", Master.link_InventoryEntryVisibility);
            Master.link_InventoryVerificationSFSPVisibility = DisplayInMenu(CurrentUser, sectionID, "InventoryVerificationSFSP", Master.link_InventoryVerificationSFSPVisibility);
            Master.link_OtherSalesVisibility = DisplayInMenu(CurrentUser, sectionID, "OtherSales", Master.link_OtherSalesVisibility);
            Master.link_SpecialFunctionsEntryVisibility = DisplayInMenu(CurrentUser, sectionID, "SpecialFunctionsEntry", Master.link_SpecialFunctionsEntryVisibility);
            Master.link_DaysVisibility = DisplayInMenu(CurrentUser, sectionID, "Days", Master.link_DaysVisibility);
            Master.link_NotificationsVisibility = DisplayInMenu(CurrentUser, sectionID, "Notifications", Master.link_NotificationsVisibility);
            Master.link_EOMChecklistVisibility = DisplayInMenu(CurrentUser, sectionID, "EOMChecklist", Master.link_EOMChecklistVisibility);
            Master.link_MenuAnalysisEntryVisibility = DisplayInMenu(CurrentUser, sectionID, "MenuAnalysisEntry", Master.link_MenuAnalysisEntryVisibility);
            #endregion
            #region Reports
            sectionID = "Reports/*";
            Master.link_PrintBudgetReportVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintBudgetReport", Master.link_PrintBudgetReportVisibility);
            #region/* Closing Reports */
            Master.link_PrintOperatingReportVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintOperatingReport", Master.link_PrintOperatingReportVisibility);
            Master.link_PrintOperatingStatisticsVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintOperatingStatistics", Master.link_PrintOperatingStatisticsVisibility);
            Master.link_PrintCompanyFoodCostVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintCompanyFoodCost", Master.link_PrintCompConsolMenuVisibility);
            Master.link_PrintCompConsolMenuVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintCompanyConsolMenu", Master.link_PrintCompConsolMenuVisibility);
            Master.link_PrintUSDACoverVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintUSDACover", Master.link_PrintUSDACoverVisibility);
            Master.link_PrintDetailedProfitCPMVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintDetailedProfitCPM", Master.link_PrintDetailedProfitCPMVisibility);
            Master.link_PrintDetailedProfitFBVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintDetailedProfitFB", Master.link_PrintDetailedProfitFBVisibility);
            Master.link_PrintEOMChecklistVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintEOMChecklist", Master.link_PrintEOMChecklistVisibility);
            Master.link_PrintInventoryDistTotalsVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintInventoryDistTotals", Master.link_PrintInventoryDistTotalsVisibility);
            Master.link_PrintInventoryVerificationVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintInventoryVerification", Master.link_PrintInventoryVerificationVisibility);
            Master.link_PrintTargetInventoryVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintTargetInventory", Master.link_PrintTargetInventoryVisibility);
            Master.link_PrintParticipationChangesVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintParticipationChanges", Master.link_PrintParticipationChangesVisibility);
            Master.link_PrintPurchasesSummaryVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintPurchasesSummary", Master.link_PrintPurchasesSummaryVisibility);
            Master.link_PrintReinhartCreditsVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintReinhartCredits", Master.link_PrintReinhartCreditsVisibility);
            Master.link_PrintUsageSummaryVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintUsageSummary", Master.link_PrintUsageSummaryVisibility);
            Master.link_PrintUSDACPMVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintUSDACPM", Master.link_PrintUSDACPMVisibility);
            #endregion
            #region/* Edit Check Reports */
            Master.link_PrintEditCheckBySchoolVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintEditCheckBySchool", Master.link_PrintEditCheckBySchoolVisibility);
            Master.link_PrintEditCheckSummaryVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintEditCheckSummary", Master.link_PrintEditCheckSummaryVisibility);
            Master.link_PrintCN6Visibility = DisplayInMenu(CurrentUser, sectionID, "PrintCN6", Master.link_PrintCN6Visibility);
            Master.link_PrintCN7Visibility = DisplayInMenu(CurrentUser, sectionID, "PrintCN7", Master.link_PrintCN7Visibility);
            #endregion
            #region /* Menu Analysis Reports */
            Master.link_PrintMenuAveragesVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintMenuAverages", Master.link_PrintMenuAveragesVisibility);
            Master.link_PrintMenuAveragesOverallVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintMenuAveragesOverall", Master.link_PrintMenuAveragesOverallVisibility);
            Master.link_PrintMADailyMenusVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintMADailyMenus", Master.link_PrintMADailyMenusVisibility);
            #endregion
            Master.link_PrintLunchClaimVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintLunchClaim", Master.link_PrintLunchClaimVisibility);
            Master.link_PrintNotificationsVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintNotifications", Master.link_PrintNotificationsVisibility);
            #region/* Payroll Reports */
            Master.link_PrintPayrollVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintPayroll", Master.link_PrintPayrollVisibility);
            Master.link_PrintPayrollHealthVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintPayrollHealth", Master.link_PrintPayrollHealthVisibility);
            Master.link_PrintPayrollDetailVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintPayrollDetail", Master.link_PrintPayrollDetailVisibility);
            #endregion
            #region/* RMEOM Reports */
            Master.link_PrintRMEOMVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintRMEOM", Master.link_PrintRMEOMVisibility);
            Master.link_RMEOMCommentsVisibility = DisplayInMenu(CurrentUser, sectionID, "RMEOMComments", Master.link_RMEOMCommentsVisibility);
            #endregion
            #region/* Sales Reports */
            Master.link_PrintSalesVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSales", Master.link_PrintSalesVisibility);
            Master.link_PrintSalesRevisionsVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSalesRevisions", Master.link_PrintSalesRevisionsVisibility);
            Master.link_PrintSalesRevisionsDetailVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSalesRevisionsDetail", Master.link_PrintSalesRevisionsDetailVisibility);
            Master.link_PrintSalesDistrictVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSalesDistrict", Master.link_PrintSalesDistrictVisibility);
            Master.link_PrintSnackSalesVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSnackSales", Master.link_PrintSnackSalesVisibility);
            Master.link_PrintSalesAdultLunchVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSalesAdultLunch", Master.link_PrintSalesAdultLunchVisibility);
            Master.link_PrintSalesDinnerVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSalesDinner", Master.link_PrintSalesDinnerVisibility);
            Master.link_PrintSalesNutrForLifeVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSalesNutrForLife", Master.link_PrintSalesNutrForLifeVisibility);
            #endregion
            #region/* Special Functions Reports */
            Master.link_PrintSpecialFuncInvoicesVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSpecialFuncInvoices", Master.link_PrintSpecialFuncInvoicesVisibility);
            Master.link_PrintSpFnLedgerVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSpFnLedger", Master.link_PrintSpFnLedgerVisibility);
            Master.div_PrintSpFnReportsGroupVisibility = (Master.link_PrintSpecialFuncInvoicesVisibility || Master.link_PrintSpFnLedgerVisibility);
            #endregion
            #region/* Transmittal Reports */
            Master.link_PrintAddOnFeesInvoiceVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintAddOnFeesInvoice", Master.link_PrintAddOnFeesInvoiceVisibility);
            Master.link_PrintTransmittalsVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintTransmittals", Master.link_PrintTransmittalsVisibility);
            Master.link_PrintTransmittalsOtherVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintTransmittalsOther", Master.link_PrintTransmittalsOtherVisibility);
            Master.link_PrintOHCatTaxInvVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintOHCatTaxInv", Master.link_PrintOHCatTaxInvVisibility);
            #endregion
            /* SFSP Reports */
            Master.link_PrintSummarySFSPDistrictsVisibility = DisplayInMenu(CurrentUser, sectionID, "PrintSummarySFSPDists", Master.link_PrintSummarySFSPDistrictsVisibility);

            #endregion
            #region Setups
            sectionID = "Setup/*";
            Master.link_SetupDistrictInfoVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupDistrictInfo", Master.link_SetupDistrictInfoVisibility);
            Master.link_SetupSchoolsVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupSchools", Master.link_SetupSchoolsVisibility);
            Master.link_SetupSchoolInfoVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupSchoolInfo", Master.link_SetupSchoolInfoVisibility);
            Master.link_SetupDistrictVendorsVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupDistrictVendors", Master.link_SetupDistrictVendorsVisibility);
            Master.link_SetupPaycodesVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupPaycodes", Master.link_SetupPaycodesVisibility);
            Master.link_SetupEmployeesVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupEmployees", Master.link_SetupEmployeesVisibility);
            Master.link_SetupMenuAnalysisItemsVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupMenuAnalysisItems", Master.link_SetupMenuAnalysisItemsVisibility);
            Master.link_SetupMilkDeliveryVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupMilkDelivery", Master.link_SetupMilkDeliveryVisibility);
            Master.link_SetupOtherSalesVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupOtherSales", Master.link_SetupOtherSalesVisibility);
            Master.link_SetupSpFnInvoicesVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupSpFnInvoices", Master.link_SetupSpFnInvoicesVisibility);
            Master.link_SetupSpFnOrgsVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupSpFnOrganizations", Master.link_SetupSpFnOrgsVisibility);
            Master.link_SetupTransOptsVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupTransmittalOptions", Master.link_SetupTransOptsVisibility);
            Master.link_BudgetEntryFMSVisibility = DisplayInMenu(CurrentUser, sectionID, "BudgetEntryFMS", Master.link_BudgetEntryFMSVisibility);
            Master.link_SetupBudgetYearsFMSVisibility = DisplayInMenu(CurrentUser, sectionID, "SetupBudgetYearsFMS", Master.link_SetupBudgetYearsFMSVisibility);
            #endregion
            #region Utilities
            sectionID = "Utilities/*";
            #region/*Exports*/
            Master.link_ExportFBPassThruVisibility = DisplayInMenu(CurrentUser, sectionID, "ExportFB", Master.link_ExportFBPassThruVisibility);
            Master.link_ExportInvChangeVisibility = DisplayInMenu(CurrentUser, sectionID, "ExportInvChange", Master.link_ExportInvChangeVisibility);
            Master.link_PayablesExportVisibility = DisplayInMenu(CurrentUser, sectionID, "PayablesExport", Master.link_PayablesExportVisibility);
            #endregion
            #region/*Imports*/
            Master.link_ImportPOSUsrVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportPOS", Master.link_ImportPOSUsrVisibility);
            Master.link_ImportPOSMgtVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportPOS", Master.link_ImportPOSMgtVisibility);
            Master.link_ImportAddonFeesVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportAddonFees", Master.link_ImportAddonFeesVisibility);
            Master.link_ImportExtraChefVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportExtraChef", Master.link_ImportExtraChefVisibility);
            Master.link_ImportIndirectCostsVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportIndirectCosts", Master.link_ImportIndirectCostsVisibility);
            Master.link_ImportInsuranceVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportInsurance", Master.link_ImportInsuranceVisibility);
            Master.link_ImportPayrollVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportPayroll", Master.link_ImportPayrollVisibility);
            Master.link_ImportReinhartCreditsVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportReinhartCredits", Master.link_ImportReinhartCreditsVisibility);
            Master.link_ImportReinhartVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportReinhart", Master.link_ImportReinhartVisibility);
            Master.link_ImportGallikerVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportGalliker", Master.link_ImportGallikerVisibility);
            Master.link_ImportMorabitoVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportMorabito", Master.link_ImportMorabitoVisibility);
            Master.link_ImportTransmittalsGenericVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportTransmittalsGeneric", Master.link_ImportTransmittalsGenericVisibility);
            Master.link_ImportSpFnPayVisibility = DisplayInMenu(CurrentUser, sectionID, "ImportSpecialFunctionPay", Master.link_ImportSpFnPayVisibility);
            #endregion

            Master.link_InvoiceCreditsVisibility = DisplayInMenu(CurrentUser, sectionID, "InvoiceCredits", Master.link_InvoiceCreditsVisibility);
            Master.link_RMAdjustmentsVisibility = DisplayInMenu(CurrentUser, sectionID, "RMAdjustments", Master.link_RMAdjustmentsVisibility);
            Master.link_FBPrepaysVisibility = DisplayInMenu(CurrentUser, sectionID, "FBPrepays", Master.link_FBPrepaysVisibility);
            Master.link_LockTransactionsVisibility = DisplayInMenu(CurrentUser, sectionID, "LockTransactions", Master.link_LockTransactionsVisibility);
            Master.link_PayrollAccrualsVisibility = DisplayInMenu(CurrentUser, sectionID, "PayrollAccruals", Master.link_PayrollAccrualsVisibility);
            Master.link_InvAdjVisibility = DisplayInMenu(CurrentUser, sectionID, "InvAdjustments", Master.link_InvAdjVisibility);
            Master.link_CEPRecalcUtilVisibility = DisplayInMenu(CurrentUser, sectionID, "CEPRecalcUtil", Master.link_CEPRecalcUtilVisibility);
            #endregion

            //Special Handling for SFSP
            if (isSFSP)
            {
                /* Hide the following links in SFSP (Not Needed) */
                Master.link_SpecialFunctionsEntryVisibility = false;
                Master.link_PrintSpecialFuncInvoicesVisibility = false;
                Master.link_PrintSpFnLedgerVisibility = false;
                Master.div_PrintSpFnReportsGroupVisibility = false;
                Master.link_SetupSpFnInvoicesVisibility = false;
                Master.link_SetupSpFnOrgsVisibility = false;
                Master.link_MenuAnalysisReportsGroupVisibility = false;

                Master.link_EOMChecklistVisibility = false;
                Master.link_PrintEOMChecklistVisibility = false;

                Master.link_MenuAnalysisEntryVisibility = false;
                Master.link_SetupMenuAnalysisItemsVisibility = false;
            }
            else
            {
                /* Hide the following links in non-SFSP sites */
                Master.link_PrintSummarySFSPDistrictsVisibility= false;
                Master.link_InventoryVerificationSFSPVisibility= false;
                Master.link_PrintInventoryVerificationVisibility = false;
            }

            //Special Handling for Toolbox
            if(isToolbox)
            { 
                //Transactions
                Master.link_SalesEntryVisibility = false;
                Master.link_SpecialFunctionsEntryVisibility = false;
                Master.link_DaysVisibility = false;
                Master.link_EOMChecklistVisibility = false;
                Master.link_MenuAnalysisEntryVisibility = false;
                //Reports
                Master.link_ClosingReportsGroupVisibility = false;
                Master.link_EditChecksReportsGroupVisibility = false;
                Master.link_PrintLunchClaimVisibility = false;
                Master.link_MenuAnalysisReportsGroupVisibility = false;
                Master.link_RMEOMGroupVisibility = false;
                Master.link_SalesReportsGroupVisibility = false;
                Master.div_PrintSpFnReportsGroupVisibility = false;
                Master.link_SchoolInvoicingReportsVisibility = false;
                //setups
                Master.link_SetupMenuAnalysisItemsVisibility = false;
                Master.link_SetupMilkDeliveryVisibility = false;
                Master.link_SetupSpFnInvoicesVisibility = false;
                Master.link_SetupSpFnOrgsVisibility = false;
            }

            if(isToolbox || isTraining)
            {
                Master.link_SetupBudgetYearsFMSVisibility = true;
                Master.link_BudgetEntryFMSVisibility = true;
            }
            else
            {
                Master.link_SetupBudgetYearsFMSVisibility = false;
                Master.link_BudgetEntryFMSVisibility = false;
            }
        }

        /// <summary>
        /// Procedure to determine if a specific item should display in the menus according to user settings and User Custom Security
        /// </summary>
        /// <param name="userID">String containing User ID</param>
        /// <param name="menuSection">String group name of menu</param>
        /// <param name="SecurityObject">String name of security object</param>
        /// <returns></returns>
        protected static bool DisplayInMenu(string userID, string menuSection, string SecurityObject, bool currentDisplayStatus)
        {
            bool DisplayMenuItem = false;
            bool grantSectionAccess=false;
            bool denySectionAccess=false;
            bool grantObjectAccess=false;
            bool denyObjectAccess=false;
            bool recordFound = false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT SecurityObject, AccessAllowed, AccessDenied FROM [UserAccessCustomSecurity] WHERE UPPER(UserID)=@UserID AND (UPPER(SecurityObject)=@SecurityObject OR UPPER(SecurityObject)=@menuSection) " +
                "UNION ALL " +
                "SELECT @SecurityObject, CAST(1 AS BIT) 'AccessAllowed', CAST(0 AS BIT) 'AccessDenied' FROM [UserAccess] WHERE UPPER(UserID)=@UserID AND AccessLevel=99; ";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                CultureInfo cultureInfo= CultureInfo.InvariantCulture;

                SqlParameter dbParam_uSERID = new SqlParameter();
                dbParam_uSERID.ParameterName = "@UserID";
                dbParam_uSERID.Value = userID.ToUpper(cultureInfo);
                dbParam_uSERID.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_uSERID);

                SqlParameter dbParam_securityObject = new SqlParameter();
                dbParam_securityObject.ParameterName = "@SecurityObject";
                dbParam_securityObject.Value = SecurityObject.ToUpper(cultureInfo);
                dbParam_securityObject.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_securityObject);

                SqlParameter dbParam_menuSection = new SqlParameter();
                dbParam_menuSection.ParameterName = "@menuSection";
                dbParam_menuSection.Value = menuSection.ToUpper(cultureInfo);
                dbParam_menuSection.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_menuSection);

                dbConnection.Open();
                try
                {
                    SqlDataReader dataReader = dbCommand.ExecuteReader();
                    while (dataReader.Read())
                    {
                        recordFound = true;
                        if (dataReader["SecurityObject"].ToString().ToUpper(cultureInfo).Trim()== menuSection.ToUpper(cultureInfo).Trim())
                        {
                            grantSectionAccess = (bool)dataReader["AccessAllowed"];
                            denySectionAccess = (bool)dataReader["AccessDenied"];
                            continue;
                        }
                        if (dataReader["SecurityObject"].ToString().ToUpper(cultureInfo).Trim() == SecurityObject.ToUpper(cultureInfo).Trim())
                        {
                            grantObjectAccess = (bool)dataReader["AccessAllowed"];
                            denyObjectAccess = (bool)dataReader["AccessDenied"];
                            continue;
                        }
                    }

                    if (dataReader != null)
                        dataReader.Close();
                }
                catch (Exception ex)
                {
                    
                }
                finally
                {
                    dbConnection.Close();
                }
            }


            if (recordFound == false)
            {
                //If no record found - do not override the menu display status
                DisplayMenuItem = currentDisplayStatus;
            }
            else
            {
                //Object security assignments override section assignments
                //if object explicitly denied - remove menu item
                if (denyObjectAccess == true)
                    DisplayMenuItem = false;
                else if (grantObjectAccess == true)
                    DisplayMenuItem = true;
                //if object not explicitly denied, but section is explicitly denied remove menu item
                else if (denySectionAccess == true)
                    DisplayMenuItem = false;
                else if (denyObjectAccess == false && grantObjectAccess == false && recordFound == true)//record created in security table, but both grant and deny false - be pessimistic
                    DisplayMenuItem = false;
                else
                    DisplayMenuItem = true;
            }

            /*  
             *  SPECIAL HANDLING FOR MENU ANALYSIS TESTING  
             *  Only grant access if explicitly granted for test group
             */
            //Remove this special shim on 5/7/2024
            //if (SecurityObject=="SetupMenuAnalysisItems" || SecurityObject=="MenuAnalysisEntry")
            //{
            //    if(grantObjectAccess == true )
            //        DisplayMenuItem = true;
            //    else
            //        DisplayMenuItem = false;
            //}

            return DisplayMenuItem;
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
            catch (DirectoryNotFoundException)
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
            String otherCat, String comments, Decimal milk, Int16 schoolNum, String programYear, ref TextBox _TextBoxStatus, Boolean VerboseErrors, Byte Locked, String UserLastMod, String ProgType)
        {
            Boolean RecordCreated = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [Purchases] ([DistrictID],[InvoiceDate],[PostDate],[VendorID],[InvoiceNumber],[Groceries],[Paper],[Cleaning],[Other] " +
                                        " ,[OtherCategoryID],[PurchaseComments],[Milk],[SchoolNum],[ProgramYear],[Locked],[ImpToPaperwork],[ImpToPaperworkDate],[UserLastModified],[ProgramType]) "
                                        + " VALUES(@DistrictID,@InvoiceDate,@PostDate,@VendorID,@InvoiceNumber,@Groceries,@Paper,@Cleaning,@Other " +
                                        " ,@OtherCategoryID,@PurchaseComments,@Milk,@SchoolNum,@ProgramYear,@Locked, 1, GETDATE(), @UserLastModified, @ProgramType )";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_dist = new SqlParameter();
                dbParam_dist.ParameterName = "@DistrictID";
                dbParam_dist.Value = distID;
                dbParam_dist.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_dist);

                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = invDate;
                dbParam_InvDate.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_PostDate = new SqlParameter();
                dbParam_PostDate.ParameterName = "@PostDate";
                dbParam_PostDate.Value = postDate;
                dbParam_PostDate.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_PostDate);

                SqlParameter dbParam_vendID = new SqlParameter();
                dbParam_vendID.ParameterName = "@VendorID";
                dbParam_vendID.Value = vendID;
                dbParam_vendID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_vendID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = invNum;
                dbParam_InvNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_groc = new SqlParameter();
                dbParam_groc.ParameterName = "@Groceries";
                dbParam_groc.Value = groc;
                dbParam_groc.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_groc);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = paper;
                dbParam_Paper.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Cleaning";
                dbParam_Clean.Value = clean;
                dbParam_Clean.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = other;
                dbParam_Other.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_othCat = new SqlParameter();
                dbParam_othCat.ParameterName = "@OtherCategoryID";
                dbParam_othCat.Value = otherCat;
                dbParam_othCat.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_othCat);

                SqlParameter dbParam_comments = new SqlParameter();
                dbParam_comments.ParameterName = "@PurchaseComments";
                dbParam_comments.Value = comments;
                dbParam_comments.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_comments);

                SqlParameter dbParam_milk = new SqlParameter();
                dbParam_milk.ParameterName = "@Milk";
                dbParam_milk.Value = milk;
                dbParam_milk.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_milk);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = schoolNum;
                dbParam_schoolNum.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = programYear;
                dbParam_progYear.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_Locked = new SqlParameter();
                dbParam_Locked.ParameterName = "@Locked";
                dbParam_Locked.Value = Locked;
                dbParam_Locked.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_Locked);

                SqlParameter dbParam_userLastMod = new SqlParameter();
                dbParam_userLastMod.ParameterName = "@UserLastModified";
                dbParam_userLastMod.Value = UserLastMod;
                dbParam_userLastMod.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_userLastMod);

                SqlParameter dbParam_progType = new SqlParameter();
                dbParam_progType.ParameterName = "@ProgramType";
                dbParam_progType.Value = ProgType;
                dbParam_progType.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_progType);

                try
                {
                    dbConnection.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                        RecordCreated = true;
                    else
                        RecordCreated = false;

                }
                catch (SqlException ex)
                {
                    RecordCreated = false;

                    if (VerboseErrors && _TextBoxStatus != null)
                    {
                        _TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        _TextBoxStatus.Rows++;
                        _TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    RecordCreated = false;

                    if (VerboseErrors && _TextBoxStatus != null)
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
        /// Creates a record with the passed in values in the Purchases table
        /// </summary>
        /// <param name="commID">Commissary ID</param>
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
        /// <param name="TextBoxStatus">reference to the Status TextBox</param>
        /// <param name="VerboseErrors">Return verbose errors or not</param>
        /// <param name="UserLastMod">Last User to Modify</param>
        /// <param name="milk">Milk Amount</param>
        /// <returns>Boolean indicating if the record was created</returns>
        protected Boolean CommissaryPurchasesInsertRecord(String commID, DateTime invDate, DateTime postDate, String vendID, String invNum, Decimal groc, Decimal paper, Decimal clean, Decimal other,
            String otherCat, String comments, ref TextBox _TextBoxStatus, Boolean VerboseErrors, String UserLastMod, Decimal milk, Byte Locked)
        {
            Boolean RecordCreated = false;

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [Purchases] ([CommissaryID],[InvoiceDate],[PostDate],[VendorID],[InvoiceNumber],[Groceries],[Paper],[Cleaning],[Other] "
                                        + " ,[OtherCategoryID],[PurchaseComments],[ImpToCommissaryweb],[ImpToCommissarywebDate],[UserLastModified],[Milk],[Locked]) "
                                        + " VALUES(@CommissaryID,@InvoiceDate,@PostDate,@VendorID,@InvoiceNumber,@Groceries,@Paper,@Cleaning,@Other "
                                        + " ,@OtherCategoryID,@PurchaseComments, 1, GETDATE(),@UserLastModified,@Milk,@Locked )";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_comm = new SqlParameter();
                dbParam_comm.ParameterName = "@CommissaryID";
                dbParam_comm.Value = commID;
                dbParam_comm.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_comm);

                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = invDate;
                dbParam_InvDate.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_PostDate = new SqlParameter();
                dbParam_PostDate.ParameterName = "@PostDate";
                dbParam_PostDate.Value = postDate;
                dbParam_PostDate.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_PostDate);

                SqlParameter dbParam_vendID = new SqlParameter();
                dbParam_vendID.ParameterName = "@VendorID";
                dbParam_vendID.Value = vendID;
                dbParam_vendID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_vendID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = invNum;
                dbParam_InvNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_groc = new SqlParameter();
                dbParam_groc.ParameterName = "@Groceries";
                dbParam_groc.Value = groc;
                dbParam_groc.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_groc);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = paper;
                dbParam_Paper.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Cleaning";
                dbParam_Clean.Value = clean;
                dbParam_Clean.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = other;
                dbParam_Other.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_othCat = new SqlParameter();
                dbParam_othCat.ParameterName = "@OtherCategoryID";
                dbParam_othCat.Value = otherCat;
                dbParam_othCat.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_othCat);

                SqlParameter dbParam_comments = new SqlParameter();
                dbParam_comments.ParameterName = "@PurchaseComments";
                dbParam_comments.Value = comments;
                dbParam_comments.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_comments);

                SqlParameter dbParam_UserLastMod = new SqlParameter();
                dbParam_UserLastMod.ParameterName = "@UserLastModified";
                dbParam_UserLastMod.Value = UserLastMod;
                dbParam_UserLastMod.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UserLastMod);

                SqlParameter dbParam_Milk = new SqlParameter();
                dbParam_Milk.ParameterName = "@Milk";
                dbParam_Milk.Value = milk;
                dbParam_Milk.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Milk);

                SqlParameter dbParam_Locked = new SqlParameter();
                dbParam_Locked.ParameterName = "@Locked";
                dbParam_Locked.Value = Locked;
                dbParam_Locked.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_Locked);

                try
                {
                    dbConnection.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                        RecordCreated = true;
                    else
                        RecordCreated = false;

                }
                catch (SqlException ex)
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
        protected Boolean ToolboxPurchasesInsertRecord(String distID, DateTime invDate, DateTime postDate, String vendID, String invNum, Decimal groc, Decimal paper, Decimal clean, Decimal other,
            String otherCat, String comments, Decimal milk, Int16 schoolNum, String programYear, ref TextBox _TextBoxStatus, Boolean VerboseErrors, Byte Locked, String UserLastMod, String ProgType)
        {
            Boolean RecordCreated = false;

            if(System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [Purchases] ([DistrictID],[InvoiceDate],[PostDate],[VendorID],[InvoiceNumber],[Groceries],[Paper],[Cleaning],[Other] " +
                                        " ,[OtherCategoryID],[PurchaseComments],[Milk],[SchoolNum],[ProgramYear],[Locked],[ImpToPaperwork],[ImpToPaperworkDate],[UserLastModified],[ProgramType]) "
                                        + " VALUES(@DistrictID,@InvoiceDate,@PostDate,@VendorID,@InvoiceNumber,@Groceries,@Paper,@Cleaning,@Other " +
                                        " ,@OtherCategoryID,@PurchaseComments,@Milk,@SchoolNum,@ProgramYear,@Locked, 1, GETDATE(), @UserLastModified, @ProgramType )";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_dist = new SqlParameter();
                dbParam_dist.ParameterName = "@DistrictID";
                dbParam_dist.Value = distID;
                dbParam_dist.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_dist);

                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = invDate;
                dbParam_InvDate.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_PostDate = new SqlParameter();
                dbParam_PostDate.ParameterName = "@PostDate";
                dbParam_PostDate.Value = postDate;
                dbParam_PostDate.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_PostDate);

                SqlParameter dbParam_vendID = new SqlParameter();
                dbParam_vendID.ParameterName = "@VendorID";
                dbParam_vendID.Value = vendID;
                dbParam_vendID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_vendID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = invNum;
                dbParam_InvNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_groc = new SqlParameter();
                dbParam_groc.ParameterName = "@Groceries";
                dbParam_groc.Value = groc;
                dbParam_groc.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_groc);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = paper;
                dbParam_Paper.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Cleaning";
                dbParam_Clean.Value = clean;
                dbParam_Clean.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = other;
                dbParam_Other.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_othCat = new SqlParameter();
                dbParam_othCat.ParameterName = "@OtherCategoryID";
                dbParam_othCat.Value = otherCat;
                dbParam_othCat.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_othCat);

                SqlParameter dbParam_comments = new SqlParameter();
                dbParam_comments.ParameterName = "@PurchaseComments";
                dbParam_comments.Value = comments;
                dbParam_comments.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_comments);

                SqlParameter dbParam_milk = new SqlParameter();
                dbParam_milk.ParameterName = "@Milk";
                dbParam_milk.Value = milk;
                dbParam_milk.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_milk);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = schoolNum;
                dbParam_schoolNum.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = programYear;
                dbParam_progYear.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_Locked = new SqlParameter();
                dbParam_Locked.ParameterName = "@Locked";
                dbParam_Locked.Value = Locked;
                dbParam_Locked.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_Locked);

                SqlParameter dbParam_userLastMod = new SqlParameter();
                dbParam_userLastMod.ParameterName = "@UserLastModified";
                dbParam_userLastMod.Value = UserLastMod;
                dbParam_userLastMod.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_userLastMod);

                SqlParameter dbParam_progType = new SqlParameter();
                dbParam_progType.ParameterName = "@ProgramType";
                dbParam_progType.Value = ProgType;
                dbParam_progType.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_progType);

                try
                {
                    dbConnection.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                        RecordCreated = true;
                    else
                        RecordCreated = false;

                }
                catch (SqlException ex)
                {
                    RecordCreated = false;

                    if (VerboseErrors && _TextBoxStatus != null)
                    {
                        _TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Toolbox: " + ex.Message;
                        _TextBoxStatus.Rows++;
                        _TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    RecordCreated = false;

                    if (VerboseErrors && _TextBoxStatus != null)
                    {
                        _TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Toolbox: " + ex.Message;
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


        protected Boolean PurchasesDoesInvExist(String programYear, String distID, String vendID, String invNum)
        {
            Boolean InvoiceExists = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string InvoiceQuery;
            InvoiceQuery = "SELECT COUNT(*) FROM [Purchases] " +
                "WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@InvoiceNumber AND VendorID=@VendorID ";
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            using (SqlCommand cmd = new SqlCommand(InvoiceQuery, dbConnection))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = programYear;// Session["ProgramYear"].ToString();
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = distID;
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_InvoiceNum = new SqlParameter();
                dbParam_InvoiceNum.ParameterName = "@InvoiceNumber";
                dbParam_InvoiceNum.Value = invNum;
                dbParam_InvoiceNum.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_InvoiceNum);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = vendID;
                dbParam_VendorID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_VendorID);
                #endregion

                try
                {
                    dbConnection.Open();
                    object recordcount = cmd.ExecuteScalar();
                    if (recordcount != null && (int)recordcount > 0)
                        InvoiceExists = true;
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "SQL Error checking for duplicate Invoices.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error checking for duplicate Invoices.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


            return InvoiceExists;
        }

        protected Boolean PurchasesDoesVendorExist(String distID, String vendID)
        {
            Boolean VendorExists = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string InvoiceQuery;
            InvoiceQuery = "SELECT COUNT(*) FROM [DistrictVendors] " +
                "WHERE DistrictID=@DistrictID AND VendorID=@VendorID ";
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            using (SqlCommand cmd = new SqlCommand(InvoiceQuery, dbConnection))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            {
                #region KeyVals
                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = distID;
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = vendID;
                dbParam_VendorID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_VendorID);
                #endregion

                try
                {
                    dbConnection.Open();
                    object recordcount = cmd.ExecuteScalar();
                    if (recordcount != null && (int)recordcount > 0)
                        VendorExists = true;
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "SQL Error checking for district vendor.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error checking for district vendor.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


            return VendorExists;
        }

        protected Boolean CommissaryPurchasesDoesInvExist(String commID, String vendID, String invNum)
        {
            Boolean InvoiceExists = false;

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string InvoiceQuery;
            InvoiceQuery = "SELECT COUNT(*) FROM [Purchases] " +
                "WHERE CommissaryID=@CommissaryID AND InvoiceNumber=@InvoiceNumber AND VendorID=@VendorID ";
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            using (SqlCommand cmd = new SqlCommand(InvoiceQuery, dbConnection))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            {
                #region KeyVals
                SqlParameter dbParam_CommID2 = new SqlParameter();
                dbParam_CommID2.ParameterName = "@CommissaryID";
                dbParam_CommID2.Value = commID;
                dbParam_CommID2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_CommID2);

                SqlParameter dbParam_InvoiceNum = new SqlParameter();
                dbParam_InvoiceNum.ParameterName = "@InvoiceNumber";
                dbParam_InvoiceNum.Value = invNum;
                dbParam_InvoiceNum.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_InvoiceNum);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = vendID;
                dbParam_VendorID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_VendorID);
                #endregion

                try
                {
                    dbConnection.Open();
                    object recordcount = cmd.ExecuteScalar();
                    if (recordcount != null && (int)recordcount > 0)
                        InvoiceExists = true;
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "SQL Error checking for duplicate Invoices.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error checking for duplicate Invoices.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


            return InvoiceExists;
        }

        protected Boolean ToolboxPurchasesDoesInvExist(String programYear, String distID, String vendID, String invNum)
        {
            Boolean InvoiceExists = false;

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string InvoiceQuery;
            InvoiceQuery = "SELECT COUNT(*) FROM [Purchases] " +
                "WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@InvoiceNumber AND VendorID=@VendorID ";
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            using (SqlCommand cmd = new SqlCommand(InvoiceQuery, dbConnection))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = programYear;// Session["ProgramYear"].ToString();
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = distID;
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_InvoiceNum = new SqlParameter();
                dbParam_InvoiceNum.ParameterName = "@InvoiceNumber";
                dbParam_InvoiceNum.Value = invNum;
                dbParam_InvoiceNum.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_InvoiceNum);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = vendID;
                dbParam_VendorID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_VendorID);
                #endregion

                try
                {
                    dbConnection.Open();
                    object recordcount = cmd.ExecuteScalar();
                    if (recordcount != null && (int)recordcount > 0)
                        InvoiceExists = true;
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "[TOOLBOX] SQL Error checking for duplicate Invoices.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "[TOOLBOX] Error checking for duplicate Invoices.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


            return InvoiceExists;
        }

        protected Boolean ToolboxPurchasesDoesVendorExist(String distID, String vendID)
        {
            Boolean VendorExists = false;

            if(System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string InvoiceQuery;
            InvoiceQuery = "SELECT COUNT(*) FROM [DistrictVendors] " +
                "WHERE DistrictID=@DistrictID AND VendorID=@VendorID ";
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            using (SqlCommand cmd = new SqlCommand(InvoiceQuery, dbConnection))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            {
                #region KeyVals
                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = distID;
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = vendID;
                dbParam_VendorID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_VendorID);
                #endregion

                try
                {
                    dbConnection.Open();
                    object recordcount = cmd.ExecuteScalar();
                    if (recordcount != null && (int)recordcount > 0)
                        VendorExists = true;
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "[TOOLBOX] SQL Error checking for district vendor.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "[TOOLBOX] Error checking for district vendor.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


            return VendorExists;
        }




        /// <summary>
        /// On the occassion where a district does not do a budget (Okaloosa for example) allow all Other categories to be used for the current program year
        /// </summary>
        /// <param name="ProgramYear"></param>
        /// <returns>Records Affected count</returns>
        protected int InitializeNonBudgetedDistCategoryRecords(string ProgramYear)
        {
            int RecordsAffected = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "INSERT INTO [DistrictAllowedOtherCats] (ProgramYear, DistrictID, OtherCategoryID)" +
                " SELECT @ProgramYear, D.DistrictID, OPC.OtherCategoryID " +
                " FROM OtherPurchaseCategories OPC " +
                " , District D " + //Intentional cartesian
                " LEFT OUTER JOIN (SELECT * FROM DistrictAllowedOtherCats WHERE ProgramYear=@ProgramYear) DAOC ON D.DistrictID=DAOC.DistrictID " +
                " WHERE DAOC.ProgramYear IS NULL AND Inactive=0 AND OPC.ProgramYear=@ProgramYear " +
                " GROUP BY D.DistrictID, OPC.OtherCategoryID " +
                " HAVING COUNT(DAOC.OtherCategoryID)=0 ";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = ProgramYear;
                dbParam_progYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_progYear);

                dbConnection.Open();
                try
                {
                    RecordsAffected = cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while initializing non-budgeted district Purchase Category records. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Exception occurred while initializing non-budgeted district Purchase Category records. ", ex.Message);
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
            if (rootControl != null)
            {
                if (rootControl.ID == controlID) return rootControl;


                foreach (Control controlToSearch in rootControl.Controls)
                {
                    Control controlToReturn =
                        FindControlRecursive(controlToSearch, controlID);
                    if (controlToReturn != null) return controlToReturn;
                }

            }
            return null;
        }

        /// <summary>
        /// Allows you to specify a parent control type to look for. The routine will continue climbing up the parent heierarchy until it finds the first parent of the desired type
        /// </summary>
        /// <param name="rootControl"></param>
        /// <param name="controlType"></param>
        /// <returns></returns>
        protected static Control FindParentControlOfType(Control rootControl, string controlType)
        {
            if (rootControl.GetType().Name == controlType) return rootControl;

            Control CurrentControl = rootControl;

            while (CurrentControl.Parent != null)
            {
                if (CurrentControl.Parent.GetType().Name == controlType)
                    return CurrentControl.Parent;
                else
                    CurrentControl = CurrentControl.Parent;//Set current control to parent and prepare to go up one more level
            }

            //Finally - control of type was not found, return null
            return null;
        }

        /// <summary>
        /// Find the control that caused the postback
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Control GetPostBackControl(Page page)
        {
            /* WARNING - while this does work to get the postback control, using it to focus will break any scroll tweaks using JQuery */
            Control control = null;

            string ctrlname = page.Request.Params.Get("__EVENTTARGET");
            if (ctrlname != null && ctrlname != String.Empty)
            {
                control = page.FindControl(ctrlname);
            }
            else
            {
                foreach (string ctl in page.Request.Form)
                {
                    Control c = page.FindControl(ctl);
                    if (c is System.Web.UI.WebControls.Button ||
                             c is System.Web.UI.WebControls.ImageButton)
                    {
                        control = c;
                        break;
                    }
                }
            }
            return control;
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

        protected string GetBudgetYear(string ProgramYear)
        {
            string outBudgetYear = "";
            string querySchool = "SELECT BudgetYear FROM [ProgramYear] WHERE ProgramYear=@ProgramYear ";

            using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(querySchool, dbConnection))
                    {
                        SqlParameter dbParam_progYear = new SqlParameter();
                        dbParam_progYear.ParameterName = "@ProgramYear";
                        dbParam_progYear.Value = ProgramYear;
                        dbParam_progYear.DbType = DbType.String;
                        cmd.Parameters.Add(dbParam_progYear);


                            object objBudgetYr = cmd.ExecuteScalar();
                            if (objBudgetYr != null)
                                outBudgetYear = objBudgetYr.ToString();
                    }

                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while reading budget year from program year table. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while reading budget year from program year table. ", ex.Message);
                }
            }

            return outBudgetYear;
        }


        /// <summary>
        /// Creates a log entry in the LogFile table. Current User and DateTime stamp will be used.
        /// </summary>
        /// <param name="LogEntity">Page or object initiating the log entry</param>
        /// <param name="LogEntry">Synopsis for the log entry</param>
        protected void CreateGlobalLogEntry(string LogEntity, string LogEntry)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "INSERT INTO [LogFile]([UserID],[LogEntity],[LogDateTime],[LogEntry])" +
                " SELECT @UserID, @LogEntity, @LogDateTime, @LogEntry";


            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                System.Data.IDataParameter dbParam_uSERID = new System.Data.SqlClient.SqlParameter();
                dbParam_uSERID.ParameterName = "@UserID";
                dbParam_uSERID.Value = Context.User.Identity.Name.ToUpper();
                dbParam_uSERID.DbType = System.Data.DbType.String;
                dbCommand.Parameters.Add(dbParam_uSERID);

                System.Data.IDataParameter dbParam_logEntity = new System.Data.SqlClient.SqlParameter();
                dbParam_logEntity.ParameterName = "@LogEntity";
                dbParam_logEntity.Value = LogEntity.ToUpper();
                dbParam_logEntity.DbType = System.Data.DbType.String;
                dbCommand.Parameters.Add(dbParam_logEntity);

                System.Data.IDataParameter dbParam_dateTime = new System.Data.SqlClient.SqlParameter();
                dbParam_dateTime.ParameterName = "@LogDateTime";
                dbParam_dateTime.Value = DateTime.Now;
                dbParam_dateTime.DbType = System.Data.DbType.DateTime;
                dbCommand.Parameters.Add(dbParam_dateTime);

                System.Data.IDataParameter dbParam_logEntry = new System.Data.SqlClient.SqlParameter();
                dbParam_logEntry.ParameterName = "@LogEntry";
                dbParam_logEntry.Value = LogEntry;
                dbParam_logEntry.DbType = System.Data.DbType.String;
                dbCommand.Parameters.Add(dbParam_logEntry);

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
            string OutGlobalSetting = "";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT [" + SettingName + "] FROM [PaperworkGlobalSettings] WHERE [Index]=0";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                dbConnection.Open();
                try
                {
                    object SettingObject = dbCommand.ExecuteScalar();
                    if (SettingObject != null)
                        OutGlobalSetting = ((string)SettingObject).ToUpper();
                }
                catch (Exception ex)
                {
                    OutGlobalSetting = "";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return OutGlobalSetting;
        }

        protected void UpdateDaysTable(DateTime ServingDate, String DistID, string ProgYear)
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
                dbParam_ProgYear.Value = ProgYear;
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
                // 3/11/2021 - modified the update to prevent both regular days and SSO days from being checked at the same time.
                updateCreateDayRecord = "UPDATE [Days] SET Days.[BreakfastDay]= IIF(Days.BSSO=1 or Days.LSSO=1, 0, ISNULL(BreakfastDayRecap,0)), " +
                    " Days.[LunchDay]= IIF(Days.BSSO=1 or Days.LSSO=1, 0, ISNULL(LunchDayRecap,0)), " +
                    " Days.[BSSO]= IIF(Days.BSSO=1 or Days.LSSO=1,ISNULL(BreakfastDayRecap,0), 0 ), " +
                    " Days.[LSSO]= IIF(Days.BSSO=1 or Days.LSSO=1,ISNULL(LunchDayRecap,0), 0 )" +
                    " FROM [Days] " +
                    " LEFT OUTER JOIN (SELECT ProgramYear, DistrictID, RecapDate, " +
                    " ISNULL(MAX([BreakfastDay]), 0) BreakfastDayRecap, ISNULL(MAX([LunchDay]),0) LunchDayRecap " +
                    " FROM [RecapEntry] GROUP BY ProgramYear, DistrictID, RecapDate" +
                    " ) Recaps ON [Days].ProgramYear=Recaps.ProgramYear AND Days.DistrictID=Recaps.DistrictID AND Days.ServingDate=Recaps.RecapDate " +
                    " WHERE Days.[ProgramYear]=@ProgramYear AND Days.[DistrictID]=@DistrictID AND Days.[ServingDate]=@RecapDate AND Days.Locked=0 AND Overridden = 0 ";
                //Only allowed to update if not locked and not already overridden
            }

            SqlConnection dbConnection2 = new SqlConnection(connectionString);
            using (SqlCommand cmd2 = new SqlCommand(updateCreateDayRecord, dbConnection2))
            {
                dbConnection2.Open();

                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = ProgYear;
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

        /// <summary>
        /// Retrieve the minimum school number based on the schoolnumber field. Find Active school if possible, otherwise return first found.
        /// </summary>
        /// <param name="DistrictID">District ID string</param>
        /// <returns>School Number ID</returns>
        protected Int16 GetMinSchool(string DistrictID)
        {

            Int16 outSchoolNum = -1;
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
                        dbParam_DistID.DbType = DbType.StringFixedLength;
                        cmdReadSchools.Parameters.Add(dbParam_DistID);

                        SqlDataReader rdr = cmdReadSchools.ExecuteReader();

                        if (rdr.Read() == true)
                        {
                            outSchoolNum = (Int16)rdr["SchoolNum"];
                            //if (Int16.TryParse(rdr["SchoolNum"].ToString(), out Int16 sclNum) == false)
                            //    sclNum = 0;

                            //outSchoolNum = sclNum;

                            //if inactive, try to find an active one
                            if (rdr["Inactive"].ToString() == "1")
                            {
                                while (rdr.Read())
                                {
                                    if (rdr["Inactive"].ToString() == "0")
                                    {
                                        outSchoolNum = (Int16)rdr["SchoolNum"];
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

        /// <summary>
        /// Checks to see if the school exists in PaperworkWeb. 
        /// </summary>
        /// <param name="schoolNum">School Number key</param>
        /// <param name="DistrictID">District ID</param>
        protected bool SchoolExists(Int32 SchoolNum, string DistrictID)
        {
            bool schoolExists = false;

            if (DistrictID == "")
                return false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string querySelectSchool = "SELECT COUNT(*) FROM [DistrictSchools] " +
                                       "WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum AND INACTIVE=0  ";

            using (SqlCommand cmd = new SqlCommand(querySelectSchool, dbConnection))
            {
                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DistrictID;
                dbParam_distID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = SchoolNum;
                dbParam_schoolNum.DbType = DbType.Int32;
                cmd.Parameters.Add(dbParam_schoolNum);

                try
                {
                    dbConnection.Open();

                    object objCount = cmd.ExecuteScalar();
                    if (objCount != null && int.Parse(objCount.ToString()) > 0)
                        schoolExists = true;
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error checking for school record.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
            return schoolExists;
        }

        /// <summary>
        /// Checks to see if the school exists in PaperworkWeb. 
        /// </summary>
        /// <param name="schoolNum">School Number key</param>
        /// <param name="DistrictID">District ID</param>
        protected bool SchoolSetupRecordExists(Int32 SchoolNum, string DistrictID, string ProgramYear)
        {
            bool schoolSetupExists = false;

            if (string.IsNullOrEmpty(DistrictID))
                return false;

            if (string.IsNullOrEmpty(ProgramYear))
                return false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string querySelectSchool = "SELECT COUNT(*) FROM [DistrictSchoolYearSetup] " +
                                       "WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum AND [ProgramYear]=@ProgramYear  ";

            using (SqlCommand cmd = new SqlCommand(querySelectSchool, dbConnection))
            {
                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DistrictID;
                dbParam_distID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = SchoolNum;
                dbParam_schoolNum.DbType = DbType.Int32;
                cmd.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_progYr = new SqlParameter();
                dbParam_progYr.ParameterName = "@ProgramYear";
                dbParam_progYr.Value = ProgramYear;
                dbParam_progYr.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_progYr);

                try
                {
                    dbConnection.Open();

                    object objCount = cmd.ExecuteScalar();
                    if (objCount != null && int.Parse(objCount.ToString()) > 0)
                        schoolSetupExists = true;
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error checking for school year setup record.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
            return schoolSetupExists;
        }
        /// <summary>
        /// Checks to see if the passed in school is currently a CEP school for the current program year.
        /// </summary>
        /// <param name="SchoolNum">School Number</param>
        /// <param name="DistrictID">District ID</param>
        /// <param name="ProgramYear">Program Year</param>
        /// <returns>Boolean indicating if school is participant</returns>
        protected bool isSchoolCurrentlyCEP(Int32 SchoolNum, string DistrictID, string ProgramYear)
        {
            bool isCepSchool = false;

            if (string.IsNullOrEmpty(DistrictID))
                return isCepSchool;

            if (string.IsNullOrEmpty(ProgramYear))
                return isCepSchool;

            if(SchoolExists(SchoolNum, DistrictID) == false)
                return isCepSchool;

            string BudgetYear = GetBudgetYear(ProgramYear);

            //ShowDialog("Budget Year", MessageType.Info, "Budget Year", BudgetYear);

            bool CEPDistrict = false;
            int CEPCoverage = 0;
            BDGTGetCEPFields(DistrictID, BudgetYear, out CEPDistrict, out CEPCoverage);

            bool OverrideBdgtsCEP = false;
            bool PPWCEPDistrict = false;
            int PPWCEPCoverage = 0;
            //Check for new paperwork override for CEP
            PPWGetCEPFields(DistrictID, ProgramYear, out OverrideBdgtsCEP, out PPWCEPDistrict, out PPWCEPCoverage);
            if(OverrideBdgtsCEP)
            {
                CEPDistrict = PPWCEPDistrict;
                CEPCoverage = PPWCEPCoverage;
            }

            //ShowDialog("Bdgt Fields Results", MessageType.Info, "CEP District: " + CEPDistrict.ToString() + " CEP Coverage: " + (CEPCoverage==0?"District-Wide":"Partial"),"");

            if(CEPDistrict==false)
                return isCepSchool;
            else if(CEPDistrict==true && CEPCoverage==0/*DistrictWide*/)
            {
                isCepSchool = true;
                return isCepSchool;
            }

            //otherwise, check the school setup
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string querySelectSchool = "SELECT PartialCEPPartic FROM [DistrictSchoolYearSetup] " +
                           "WHERE [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum AND [ProgramYear]=@ProgramYear  ";

            using (SqlCommand cmd = new SqlCommand(querySelectSchool, dbConnection))
            {
                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DistrictID;
                dbParam_distID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNum = new SqlParameter();
                dbParam_schoolNum.ParameterName = "@SchoolNum";
                dbParam_schoolNum.Value = SchoolNum;
                dbParam_schoolNum.DbType = DbType.Int32;
                cmd.Parameters.Add(dbParam_schoolNum);

                SqlParameter dbParam_progYr = new SqlParameter();
                dbParam_progYr.ParameterName = "@ProgramYear";
                dbParam_progYr.Value = ProgramYear;
                dbParam_progYr.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_progYr);

                try
                {
                    dbConnection.Open();

                    object objPartialCEPParticipant = cmd.ExecuteScalar();
                    if (objPartialCEPParticipant != null && (bool)objPartialCEPParticipant)
                        isCepSchool = true;
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error checking for school year setup record.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return isCepSchool;
        }

        protected void PPWGetCEPFields(string DistrictID, string ProgramYear,out bool OverrideBdgtsCEP, out bool isCEP, out int cepCoverage)
        {
            //default values
            cepCoverage = 0;
            isCEP = false;
            OverrideBdgtsCEP = false;

            if (isSFSP() || isTOOLBOX())
                return;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT [OverridBdgtCEP], [PaperworkCEP], [PaperworkCEPCoverage] FROM [DistrictYearInfo] WHERE [DistrictID] = @DistrictID AND [ProgramYear] = @ProgramYear";

            using (SqlCommand cmd = new SqlCommand(queryString, dbConnection))
            {
                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DistrictID;
                dbParam_distID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_progYr = new SqlParameter();
                dbParam_progYr.ParameterName = "@ProgramYear";
                dbParam_progYr.Value = ProgramYear;
                dbParam_progYr.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_progYr);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read() == true)
                    {
                        OverrideBdgtsCEP = (bool)rdr["OverridBdgtCEP"];

                        if (OverrideBdgtsCEP == true)
                        {
                            int.TryParse(rdr["PaperworkCEPCoverage"].ToString(), out cepCoverage);
                            isCEP = rdr["PaperworkCEP"].ToString() == "1" ? true : false;
                        }
                    }
                }
                catch (SqlException ex)
                {
#if DEBUG
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while reading Paperwork Budget Override CEP Records. ", ex.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    ShowDialog("Error", MessageType.Error, "Exception occurred while reading Paperwork Budget Override CEP Records. ", ex.Message);
#endif
                }
                finally
                {
                    dbConnection.Close();
                }
            }

        }


        #region BDGT Procedures

        protected void BDGTGetCEPFields(string DistrictID, string BudgetYear, out bool isCEP, out int cepCoverage)
        {
            //default values
            cepCoverage = 0;
            isCEP = false;
            if (isSFSP() || isTOOLBOX())
                return;

            string ConnStringBDGTS = ConfigurationManager.ConnectionStrings["connBDGTS"].ConnectionString;
            //string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(ConnStringBDGTS);

            string queryString = "SELECT [CEP], [CEPCoverage] FROM [vwBDGT_Totals] WHERE [BidNumber] = @BidNumber AND BudgetYear = @BudgetYear";

            using (SqlCommand cmd = new SqlCommand(queryString, dbConnection))
            {
                SqlParameter dbParam_bidNum = new SqlParameter();
                dbParam_bidNum.ParameterName = "@BidNumber";
                dbParam_bidNum.Value = DistrictID;
                dbParam_bidNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_bidNum);

                SqlParameter dbParam_bdgtYr = new SqlParameter();
                dbParam_bdgtYr.ParameterName = "@BudgetYear";
                dbParam_bdgtYr.Value = BudgetYear;
                dbParam_bdgtYr.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_bdgtYr);

                
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read() == true)
                    {
                        int.TryParse(rdr["CEPCoverage"].ToString(), out cepCoverage);
                        isCEP = rdr["CEP"].ToString()=="1"?true:false;
                    }
                }
                catch (SqlException ex)
                {
#if DEBUG
                        ShowDialog("Error", MessageType.Error, "SQL Exception occurred while reading Budget CEP Records. ", ex.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                        ShowDialog("Error", MessageType.Error, "Exception occurred while reading Budget CEP Records. ", ex.Message);
#endif
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            //    if ((CEPDistrict && CEPCoverage == 0 && DDLRevisions.SelectedValue == "0") || (CEPDistrict && CEPCoverage == 1 && HFSchoolCEPParticipant.Value == "True" && DDLRevisions.SelectedValue == "0"))

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
                " SELECT '" + ProgramYear + "' , MyTable.BidNumber , MyTable.CATEGORIES FROM OPENROWSET('SQLNCLI','" + ConnStringBDGTS + "', 'EXEC [dbo].[Paperworkweb_DistAllowedOCats] @BudgetYear = N''" + BudgetYear + "'' WITH RESULT SETS " +
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
                catch (SqlException ex)
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
            " SELECT '" + ProgramYear + "' + MyTable.BidNumber + MyTable.CATEGORIES FROM  OPENROWSET('SQLNCLI','" + ConnStringBDGTS + "', 'EXEC [dbo].[Paperworkweb_DistAllowedOCats] @BudgetYear = N''" + BudgetYear + "'' WITH RESULT SETS " +
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


        #endregion
        //******** Date Related Functions *************//
        #region Date Functions
        protected bool IsDateOpenForEntry(DateTime checkDate, string ProgramYear, string DistrictID, string Module)
        {
            bool trxEntryAllowed = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string queryCheckDate = "SELECT Count(*) FROM [ProgramYear] PY " +
                " LEFT OUTER JOIN [TransactionClosures] TC ON PY.ProgramYear=TC.ProgramYear AND @DistrictID=TC.DistrictID AND @Module=TransactionModule" +
                " WHERE PY.StartDate<=@CheckDate AND PY.EndDate>=@CheckDate AND PY.AllowDataEntry = 1 AND PY.ProgramYear=@ProgramYear" +
                " AND ((TC.ClosedThroughDate IS NOT NULL AND TC.ClosedThroughDate<@CheckDate) OR (TC.ClosedThroughDate IS NULL AND PY.[Close" + checkDate.Month.ToString().PadLeft(2, '0') + "] = 0))";

            using (SqlCommand cmd = new SqlCommand(queryCheckDate, sqlConnection))
            {
                SqlParameter dbParam_cHeckDate = new SqlParameter();
                dbParam_cHeckDate.ParameterName = "@CheckDate";
                dbParam_cHeckDate.Value = checkDate;
                dbParam_cHeckDate.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_cHeckDate);

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = ProgramYear;
                dbParam_progYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_DistrictID = new SqlParameter();
                dbParam_DistrictID.ParameterName = "@DistrictID";
                dbParam_DistrictID.Value = DistrictID;
                dbParam_DistrictID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistrictID);

                SqlParameter dbParam_module = new SqlParameter();
                dbParam_module.ParameterName = "@Module";
                dbParam_module.Value = Module;
                dbParam_module.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_module);

                sqlConnection.Open();
                int openCount = 0;
                openCount = (int)cmd.ExecuteScalar();

                if (openCount > 0)
                    trxEntryAllowed = true;
                else
                    trxEntryAllowed = false;
            }

            if (sqlConnection != null)
                sqlConnection.Dispose();


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
                    else if ((byte)enabled == 0)//not enforced
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
        protected void ValidateTrxEntryDate(ref TextBox TrxDate, Boolean WarnWeekends, string PYStartDate, string PYEndDate)
        {
            // if the program year variables are not set, just return
            if (string.IsNullOrEmpty(PYStartDate))
            { return; }

            if (string.IsNullOrEmpty(PYEndDate))
            { return; }

            //** NOTE - due to strange compiler error - need to move DateTime declaration separate from the tryparse method
            DateTime DatePYStartDate;
            if (DateTime.TryParse(PYStartDate, out DatePYStartDate) == false)
            { return; }

            DateTime DatePYEndDate;
            if (DateTime.TryParse(PYEndDate, out DatePYEndDate) == false)
            { return; }

            DateTime UseDate;
            if (DateTime.TryParse(TrxDate.Text, out UseDate) == false)
            {
                //date could not be parsed - set to today
                UseDate = DateTime.Now.Date;
                TrxDate.Text = UseDate.ToShortDateString();
            }

            if (UseDate >= DatePYStartDate && UseDate <= DatePYEndDate)
            {
                //update the date field in case it could be parsed, but may not be usable by jquery date function
                TrxDate.Text = UseDate.ToShortDateString();
            }
            else if (UseDate < DatePYStartDate)
            {
                UseDate = DatePYStartDate;
                TrxDate.Text = UseDate.ToShortDateString();
                ShowDialog("Date Error", MessageType.Error, "The date falls before the beginning of the Program Year. Date has been changed to the start of the program year.", "");
            }
            else if (UseDate > DatePYEndDate)
            {
                UseDate = DatePYEndDate;
                TrxDate.Text = UseDate.ToShortDateString();
                ShowDialog("Date Error", MessageType.Error, "The date falls after the end of the Program Year. Date has been changed to the last day of the program year.", "");
            }

            if (WarnWeekends == true && (UseDate.DayOfWeek == DayOfWeek.Saturday || UseDate.DayOfWeek == DayOfWeek.Sunday))
            {
                string JqueryPost = "$('[id$=" + TrxDate.ID + "]').addClass('highlight');";
                ShowDialogPlus("Date Warning", MessageType.Warning, "The date is not a weekday - is it correct?", "", JqueryPost);
            }
            else
                RegisterDOMReadyScript("Date Unhighlight", "$('[id$= " + TrxDate.ID + "]').removeClass('highlight');");

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
                    dateTextBox.Text = DateTime.Now.Month.ToString() + "/" + dateTextBox.Text + "/" + DateTime.Now.Year.ToString();
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
                if (MonthNum == 12)
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
        /// New April 2022 - Prevent new transactions from being entered before a locking date
        /// </summary>
        /// <param name="Lock"></param>
        /// <param name="StartDateRange"></param>
        /// <param name="EndDateRange"></param>
        protected void UpdateTransactionCutoff(bool Lock, DateTime StartDateRange, DateTime EndDateRange, string Module, string ProgramYear, string DistID, string SourcePage, string SourceUser)
        {
            DateTime CutoffDate;

            if (StartDateRange > EndDateRange)
                return;

            //if locking set the cutoff=end date, otherwise set to 1 day prior to start date.
            if (Lock)
                CutoffDate = EndDateRange;
            else
                CutoffDate = StartDateRange.AddDays(-1);

            //only create records for configured modules. If string not passed in, exit
            if (string.IsNullOrEmpty(Module))
                return;


            int existingRecordCount = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            if (DistID != "%")
            {

                string checkForRecord = "SELECT count(*) FROM [TransactionClosures] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND TransactionModule=@TransactionModule ";

                SqlConnection dbConnection = new SqlConnection(connectionString);
                using (SqlCommand cmd = new SqlCommand(checkForRecord, dbConnection))
                {
                    dbConnection.Open();
                    SqlParameter dbParam_ProgYear = new SqlParameter();
                    dbParam_ProgYear.ParameterName = "@ProgramYear";
                    dbParam_ProgYear.Value = ProgramYear;
                    dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_ProgYear);

                    SqlParameter dbParam_dist = new SqlParameter();
                    dbParam_dist.ParameterName = "@DistrictID";
                    dbParam_dist.Value = DistID;
                    dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_dist);

                    SqlParameter dbParam_module = new SqlParameter();
                    dbParam_module.ParameterName = "@TransactionModule";
                    dbParam_module.Value = Module;
                    dbParam_module.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_module);

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
                        //RegisterDOMReadyScript("error message", "alert('Error ...');");
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
                existingRecordCount = 1;//All districts selected 

            string updateCreateDayRecord = "";
            if (existingRecordCount == 0)
            {
                //Insert
                updateCreateDayRecord = "INSERT INTO [TransactionClosures] ([ProgramYear],[DistrictID],[TransactionModule],[ClosedThroughDate],[SourcePage],[SourceUser]) " +
                    " VALUES(@ProgramYear, @DistrictID, @TransactionModule, @ClosedThroughDate, @SourcePage, @SourceUser)";
            }
            else
            {
                //Update
                //Note - this query is applicable for single district selection and all districts so I must use LIKE
                //It will also create any missing district records

                //If locking only change the date if greater than the existing record date
                if (Lock)
                {
                    updateCreateDayRecord = "UPDATE [TransactionClosures] SET ClosedThroughDate=@ClosedThroughDate, [SourcePage]=@SourcePage, [SourceUser]=@SourceUser " +
                    " WHERE (ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID AND TransactionModule=@TransactionModule AND ClosedThroughDate<@ClosedThroughDate);";
                }
                else //Unlocking - only change the date if new one is lesser than existing record
                {
                    updateCreateDayRecord = "UPDATE [TransactionClosures] SET ClosedThroughDate=@ClosedThroughDate, [SourcePage]=@SourcePage, [SourceUser]=@SourceUser " +
                    " WHERE (ProgramYear=@ProgramYear AND DistrictID LIKE @DistrictID AND TransactionModule=@TransactionModule AND ClosedThroughDate>@ClosedThroughDate);";
                }

                updateCreateDayRecord += "INSERT INTO [TransactionClosures] ([ProgramYear],[DistrictID],[TransactionModule],[ClosedThroughDate],[SourcePage],[SourceUser]) " +
                    "SELECT @ProgramYear, DistrictID, @TransactionModule, @ClosedThroughDate, @SourcePage, @SourceUser FROM District WHERE DistrictID LIKE @DistrictID AND DistrictID NOT IN " +
                    "(SELECT DistrictID FROM [TransactionClosures] WHERE ProgramYear=@ProgramYear AND TransactionModule=@TransactionModule)";
            }

            SqlConnection dbConnection2 = new SqlConnection(connectionString);
            using (SqlCommand cmd2 = new SqlCommand(updateCreateDayRecord, dbConnection2))
            {
                dbConnection2.Open();

                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = ProgramYear;
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_dist = new SqlParameter();
                dbParam_dist.ParameterName = "@DistrictID";
                dbParam_dist.Value = DistID;
                dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_dist);

                SqlParameter dbParam_module = new SqlParameter();
                dbParam_module.ParameterName = "@TransactionModule";
                dbParam_module.Value = Module;
                dbParam_module.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_module);

                SqlParameter dbParam_closeThroughDate = new SqlParameter();
                dbParam_closeThroughDate.ParameterName = "@ClosedThroughDate";
                dbParam_closeThroughDate.Value = CutoffDate;
                dbParam_closeThroughDate.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_closeThroughDate);

                SqlParameter dbParam_sourcePage = new SqlParameter();
                dbParam_sourcePage.ParameterName = "@SourcePage";
                dbParam_sourcePage.Value = SourcePage;
                dbParam_sourcePage.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_sourcePage);

                SqlParameter dbParam_sourceUser = new SqlParameter();
                dbParam_sourceUser.ParameterName = "@SourceUser";
                dbParam_sourceUser.Value = SourceUser;
                dbParam_sourceUser.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_sourceUser);

                try
                {
                    object obj = cmd2.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "SQL error while updating the transaction cutoff date for module " + Module, ex.Message);
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
        #endregion

        #region Page Utility Procedures

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

        //from: https://stackoverflow.com/questions/4344408/disable-submit-button-in-case-of-multiple-click-c

        /// <summary>
        /// Disables the Button immediately after clicking to prevent procedures from running again with a double-click
        /// </summary>
        /// <param name="_Button"></param>
        protected static void PreventMultipleClicks(ref WebControl _Button)
        {
            if (_Button == null)
                return;

            _Button.Attributes.Add("onclick", "this.disabled=true;" + _Button.Page.ClientScript.GetPostBackEventReference(_Button, String.Empty).ToString());
        }

        /// <summary>
        /// Disables the Button immediately after clicking to prevent procedures from running again with a double-click
        /// </summary>
        /// <param name="_Button"></param>
        protected static void PreventMultipleClicks(ref Button _Button)
        {
            if (_Button == null)
                return;

            _Button.Attributes.Add("onclick", "this.disabled=true;" + _Button.Page.ClientScript.GetPostBackEventReference(_Button, String.Empty).ToString());
        }
        protected static void PreventMultipleClicks(ref LinkButton _Button)
        {
            if (_Button == null)
                return;

            _Button.Attributes.Add("onclick", "this.disabled=true;" + _Button.Page.ClientScript.GetPostBackEventReference(_Button, String.Empty).ToString());
        }


        #endregion

        #region JQUERY RELATED SCRIPTS

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
        //ShowMessage allows you to interface with ShowMessage function in the jquery section of the page.
        //This block of jquery must be included.
        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        //ShowMessagebyName allows you to interface with ShowMessagebyName function in the jquery section of the page. Allows you to specify a specific alert block. Jquery code Must be included.
        protected void ShowMessageByName(string Message, MessageType type, string AlertBlock, int Duration)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessageByName('" + Message + "','" + type + "','" + AlertBlock + "','" + Duration.ToString() + "');", true);
        }

        //ShowMessageIcon allows you to interface with ShowMessageIcon function in the jquery section of the page.
        //Must include a div on the page with id alert_container. Shortened version of ShowMessage
        //<div class="messagealert" id="alert_container" style="padding-top:10px;"></div>
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

            //New line chars need to be removed to work with jQuery popup
            string cleanedSynopsis = Synopsis.Replace("/r/n", "");
            string cleanedDetails = Details.Replace("/r/n", "");

            sb.Append(jss.Serialize(cleanedSynopsis));
            if (string.IsNullOrEmpty(cleanedDetails) == false)
            {
                sb.Append("<br /><strong>Details:</strong> ");
                sb.Append(jss.Serialize(cleanedDetails));
            }

            RegisterDOMReadyScript("Popup", "ShowPopup('" + Title + "', '" + @sb.ToString().Replace('\"',' ') + " ', '" + type + "');");
        }

        /// <summary>
        /// Procedure streamlines the use of JQuery UI Dialog boxes. Plus runs additional jQuery after the dialog.
        /// </summary>
        /// <param name="Title">Title bar string</param>
        /// <param name="type">MessageType for background color</param>
        /// <param name="Synopsis">Basic dialog message string</param>
        /// <param name="Details">Detailed dialog message string</param>
        /// <param name="ExtraJqueryScripts">Additional JQuery to run after the dialog is displayed</param>
        protected void ShowDialogPlus(string Title, MessageType type, string Synopsis, string Details, string ExtraJqueryScripts)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            StringBuilder sb = new StringBuilder();

            //New line chars need to be removed to work with jQuery popup
            string cleanedSynopsis = Synopsis.Replace("/r/n", "");
            string cleanedDetails = Details.Replace("/r/n", "");

            sb.Append(jss.Serialize(cleanedSynopsis));
            if (string.IsNullOrEmpty(cleanedDetails) == false)
            {
                sb.Append("<br /><strong>Details:</strong> ");
                sb.Append(jss.Serialize(cleanedDetails));
            }

            RegisterDOMReadyScript("Popup", "ShowPopup('" + Title + "', '" + @sb.ToString().Replace('\"', ' ') + " ', '" + type + "');" + ExtraJqueryScripts);
        }

        #endregion

        protected string GetContentTypeFromExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                case ".docx":
                    return "application/msword";
                case ".xls":
                case ".xlsx":
                    return "application/vnd.ms-excel";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".txt":
                    return "text/plain";
                default:
                    return null; // Unknown file type
            }
        }

    }

    public class RecapEntry
    {
        private int schoolNum;
        private DateTime recapDate;
        private string districtID;// Primary Key
        private int revision;// Primary Key (revision is used in conjunction with Edit Check Date)
                             // A revision will have a Recap Date in a future period which is open and editable while the EditCheckDate will refer to the previous closed recap date
                             // Revision number just indicates the future date has multiple Recaps associated for posting purposes. The original recap will show that it has a revision
                             // which will display the future posting date as it's revision
        private string programYear;// Primary Key
        private DateTime editCheckDate;// Original Recap date. If not revised, will match the Recap Date, otherwise will be a date in the past when compared to RecapDate
        private byte breakfastDay;
        private byte lunchDay;
        private int bPaidElem;
        private int bPaidJr;
        private int bPaidSr;
        private int bReduced;
        private int bFree;
        private int bPaidLrn;
        private int bReducedLrn;
        private int bFreeLrn;
        private decimal bStuAla;
        private decimal bAdultAla;
        private decimal bAdvanced;
        private decimal bRedeemed;
        private decimal bRepaid;
        private decimal bCharges;
        private decimal bDeposit;
        private int lPaidElem;
        private int lPaidJr;
        private int lPaidSr;
        private int lTier1;
        private int lTier2;
        private int lReduced;
        private int lReducedTier1;
        private int lFree;
        private int lFreeTier1;
        private int lFreeTier2;
        private int lPaidLrn;
        private int lReducedLrn;
        private int lFreeLrn;
        private decimal lStuAla;
        private decimal lAdultAla;
        private decimal lOtherAla1;
        private decimal lVendAla;
        private decimal lAdvanced;
        private decimal lRedeemed;
        private decimal lRepaid;
        private decimal lCharges;
        private int oPaidSnack;
        private int oReducedSnack;
        private int oFreeSnack;
        private decimal specialFunctionSale;
        private decimal specialFunctionColl;
        private decimal specialFunctionComm;
        private decimal uSDASale;
        private decimal uSDAColl;
        private decimal otherSale1;
        private decimal otherColl1;
        private decimal otherSale2;
        private decimal otherColl2;
        private decimal onlineAdvance;
        private decimal adjustment;
        private decimal deposit;
        private string editCheckComments;
        private string recapComments;
        private decimal r_AF;
        private int r_Enrollment;
        private int r_EligiblePaid;
        private int r_EligibleFree;
        private int r_EligibleReduced;
        private decimal r_BreakfastIncent;
        private byte locked;
        private int otherSale1Cat;
        private int otherColl1Cat;
        private int otherSale2Cat;
        private int otherColl2Cat;
        private byte cepPartic;



        /// <summary>
        /// /////////////////////////////////////////////////////////
        /// </summary>



        public RecapEntry()
        {
            schoolNum = 0;
            recapDate = new DateTime();// Primary Key
            districtID = "";// Primary Key
            revision = 0;// Primary Key (revision is used in conjunction with Edit Check Date)
                         //                    // A revision will have a Recap Date in a future period which is open and editable while the EditCheckDate will refer to the previous closed recap date
                         //                    // Revision number just indicates the future date has multiple Recaps associated for posting purposes. The original recap will show that it has a revision
                         //                    // which will display the future posting date as it's revision
            programYear = "";// Primary Key
            editCheckDate = new DateTime();// Original Recap date. If not revised, will match the Recap Date, otherwise will be a date in the past when compared to RecapDate
            breakfastDay = (byte)0;
            lunchDay = (byte)0;
            bPaidElem = 0;
            bPaidJr = 0;
            bPaidSr = 0;
            bReduced = 0;
            bFree = 0;
            bPaidLrn = 0;
            bReducedLrn = 0;
            bFreeLrn = 0;
            bStuAla = 0;
            bAdultAla = 0M;
            bAdvanced = 0M;
            bRedeemed = 0M;
            bRepaid = 0M;
            bCharges = 0M;
            bDeposit = 0M;
            lPaidElem = 0;
            lPaidJr = 0;
            lPaidSr = 0;
            lTier1 = 0;
            lTier2 = 0;
            lReduced = 0;
            lReducedTier1 = 0;
            lFree = 0;
            lFreeTier1 = 0;
            lFreeTier2 = 0;
            lPaidLrn = 0;
            lReducedLrn = 0;
            lFreeLrn = 0;
            lStuAla = 0M;
            lAdultAla = 0M;
            lOtherAla1 = 0M;
            lVendAla = 0M;
            lAdvanced = 0M;
            lRedeemed = 0M;
            lRepaid = 0M;
            lCharges = 0M;
            oPaidSnack = 0;
            oReducedSnack = 0;
            oFreeSnack = 0;
            specialFunctionSale = 0M;
            specialFunctionColl = 0M;
            specialFunctionComm = 0M;
            uSDASale = 0M;
            uSDAColl = 0M;
            otherSale1 = 0M;
            otherColl1 = 0M;
            otherSale2 = 0M;
            otherColl2 = 0M;
            onlineAdvance = 0M;
            adjustment = 0M;
            deposit = 0M;
            editCheckComments = "";
            recapComments = "";
            r_AF = 0M;
            r_Enrollment = 0;
            r_EligiblePaid = 0;
            r_EligibleFree = 0;
            r_EligibleReduced = 0;
            r_BreakfastIncent = 0M;
            locked = (byte)0;
            otherSale1Cat = 0;
            otherColl1Cat = 0;
            otherSale2Cat = 0;
            otherColl2Cat = 0;
            cepPartic = (byte)0;
        }

        public int SchoolNum { get { return schoolNum; } set { schoolNum = value; } }
        public DateTime RecapDate { get { return recapDate; } set { recapDate = value; } }
        public string DistrictID { get { return districtID; } set { districtID = value; } }
        public int Revision { get { return revision; } set { revision = value; } }
        public string ProgramYear { get { return programYear; } set { programYear = value; } }
        public DateTime EditCheckDate { get { return editCheckDate; } set { editCheckDate = value; } }
        public byte BreakfastDay { get { return breakfastDay; } set { breakfastDay = value; } }
        public byte LunchDay { get { return lunchDay; } set { lunchDay = value; } }
        public int BPaidElem { get { return bPaidElem; } set { bPaidElem = value; } }
        public int BPaidJr { get { return bPaidJr; } set { bPaidJr = value; } }
        public int BPaidSr { get { return bPaidSr; } set { bPaidSr = value; } }
        public int BReduced { get { return bReduced; } set { bReduced = value; } }
        public int BFree { get { return bFree; } set { bFree = value; } }
        public int BPaidLrn { get { return bPaidLrn; } set { bPaidLrn = value; } }
        public int BReducedLrn { get { return bReducedLrn; } set { bReducedLrn = value; } }
        public int BFreeLrn { get { return bFreeLrn; } set { bFreeLrn = value; } }
        public decimal BStuAla { get { return bStuAla; } set { bStuAla = value; } }
        public decimal BAdultAla { get { return bAdultAla; } set { bAdultAla = value; } }
        public decimal BAdvanced { get { return bAdvanced; } set { bAdvanced = value; } }
        public decimal BRedeemed { get { return bRedeemed; } set { bRedeemed = value; } }
        public decimal BRepaid { get { return bRepaid; } set { bRepaid = value; } }
        public decimal BCharges { get { return bCharges; } set { bCharges = value; } }
        public decimal BDeposit { get { return bDeposit; } set { bDeposit = value; } }
        public int LPaidElem { get { return lPaidElem; } set { lPaidElem = value; } }
        public int LPaidJr { get { return lPaidJr; } set { lPaidJr = value; } }
        public int LPaidSr { get { return lPaidSr; } set { lPaidSr = value; } }
        public int LTier1 { get { return lTier1; } set { lTier1 = value; } }
        public int LTier2 { get { return lTier2; } set { lTier2 = value; } }
        public int LReduced { get { return lReduced; } set { lReduced = value; } }
        public int LReducedTier1 { get { return lReducedTier1; } set { lReducedTier1 = value; } }
        public int LFree { get { return lFree; } set { lFree = value; } }
        public int LFreeTier1 { get { return lFreeTier1; } set { lFreeTier1 = value; } }
        public int LFreeTier2 { get { return lFreeTier2; } set { lFreeTier2 = value; } }
        public int LPaidLrn { get { return lPaidLrn; } set { lPaidLrn = value; } }
        public int LReducedLrn { get { return lReducedLrn; } set { lReducedLrn = value; } }
        public int LFreeLrn { get { return lFreeLrn; } set { lFreeLrn = value; } }
        public decimal LStuAla { get { return lStuAla; } set { lStuAla = value; } }
        public decimal LAdultAla { get { return lAdultAla; } set { lAdultAla = value; } }
        public decimal LOtherAla1 { get { return lOtherAla1; } set { lOtherAla1 = value; } }
        public decimal LVendAla { get { return lVendAla; } set { lVendAla = value; } }
        public decimal LAdvanced { get { return lAdvanced; } set { lAdvanced = value; } }
        public decimal LRedeemed { get { return lRedeemed; } set { lRedeemed = value; } }
        public decimal LRepaid { get { return lRepaid; } set { lRepaid = value; } }
        public decimal LCharges { get { return lCharges; } set { lCharges = value; } }
        public int OPaidSnack { get { return oPaidSnack; } set { oPaidSnack = value; } }
        public int OReducedSnack { get { return oReducedSnack; } set { oReducedSnack = value; } }
        public int OFreeSnack { get { return oFreeSnack; } set { oFreeSnack = value; } }
        public decimal SpecialFunctionSale { get { return specialFunctionSale; } set { specialFunctionSale = value; } }
        public decimal SpecialFunctionColl { get { return specialFunctionColl; } set { specialFunctionColl = value; } }
        public decimal SpecialFunctionComm { get { return specialFunctionComm; } set { specialFunctionComm = value; } }
        public decimal USDASale { get { return uSDASale; } set { uSDASale = value; } }
        public decimal USDAColl { get { return uSDAColl; } set { uSDAColl = value; } }
        public decimal OtherSale1 { get { return otherSale1; } set { otherSale1 = value; } }
        public decimal OtherColl1 { get { return otherColl1; } set { otherColl1 = value; } }
        public decimal OtherSale2 { get { return otherSale2; } set { otherSale2 = value; } }
        public decimal OtherColl2 { get { return otherColl2; } set { otherColl2 = value; } }
        public decimal OnlineAdvance { get { return onlineAdvance; } set { onlineAdvance = value; } }
        public decimal Adjustment { get { return adjustment; } set { adjustment = value; } }
        public decimal Deposit { get { return deposit; } set { deposit = value; } }
        public string EditCheckComments { get { return editCheckComments; } set { editCheckComments = value; } }
        public string RecapComments { get { return recapComments; } set { recapComments = value; } }
        public decimal R_AF { get { return r_AF; } set { r_AF = value; } }
        public int R_Enrollment { get { return r_Enrollment; } set { r_Enrollment = value; } }
        public int R_EligiblePaid { get { return r_EligiblePaid; } set { r_EligiblePaid = value; } }
        public int R_EligibleFree { get { return r_EligibleFree; } set { r_EligibleFree = value; } }
        public int R_EligibleReduced { get { return r_EligibleReduced; } set { r_EligibleReduced = value; } }
        public decimal R_BreakfastIncent { get { return r_BreakfastIncent; } set { r_BreakfastIncent = value; } }
        public byte Locked { get { return locked; } set { locked = value; } }
        public int OtherSale1Cat { get { return otherSale1Cat; } set { otherSale1Cat = value; } }
        public int OtherColl1Cat { get { return otherColl1Cat; } set { otherColl1Cat = value; } }
        public int OtherSale2Cat { get { return otherSale2Cat; } set { otherSale2Cat = value; } }
        public int OtherColl2Cat { get { return otherColl2Cat; } set { otherColl2Cat = value; } }
        public byte CEPPartic { get { return cepPartic; } set { cepPartic = value; } }


    }
}
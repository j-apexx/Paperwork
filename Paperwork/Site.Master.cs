using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Paperwork
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;
        private bool isToolbox = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CurrentDistrictName"] == null || Session["CurrentDistrictName"].ToString() == string.Empty)
                {
                    CurrDistrict.Text = "*";
                }
                else
                    CurrDistrict.Text = Session["CurrentDistrictName"].ToString();

                //Using BaseFolder to check for training company
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"].ToUpper();

                if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING") > 0)
                {
                    LabelPPWTrainSFSPIndicator.Text = "PPW TRAIN";
                    logo.Visible = true;
                    logoFMS.Visible = false;
                    link_SpecialFunctionsEntry.HRef = "SpecialFunctionsEntryTEST.aspx";
                    link_SalesEntry.HRef = "SalesEntryTEST.aspx";
                    link_PrintOperatingReport_TEST.Visible = true;
                }
                else if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBSFSP") > 0)
                {
                    LabelPPWTrainSFSPIndicator.Text = "SFSP";
                    logo.Visible = true;
                    logoFMS.Visible = false;
                }
                else if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKTOOLBOX") > 0)
                {
                    LabelPPWTrainSFSPIndicator.Text = "TOOLBOX";
                    logo.Visible = false;
                    logoFMS.Visible = true;
                    isToolbox = true;
                }
                else
                {
                    LabelPPWTrainSFSPIndicator.Text = "Paperwork";
                    logo.Visible = true;
                    logoFMS.Visible = false;
                }


                //Security Settings
                if (Session["AccessLevel"] != null)
                {
                    switch (Session["AccessLevel"].ToString().ToUpper())
                    {
                        case "ADMIN":
                            MstrAdminSetup.Visible = true;
                            MstrAdminOfficeUtils.Visible = true;
                            MstrUserOnlyUtils.Visible = false;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                            link_SchoolInvoicingReportsGroup.Visible = true;
                            link_PrintOperatingStatistics.Visible = true;
                            link_PrintCompanyFoodCost.Visible = true;
                            link_PrintCompConsolMenu.Visible = true;
                            link_RMEOMReportsGroup.Visible = true;
                            link_PrintNotifications.Visible = true;
                            link_PrintInventoryDistTotals.Visible = true;
                            link_PrintParticipationChanges.Visible = true;
                            link_PrintPurchasesSummary.Visible = true;
                            link_PrintUsageSummary.Visible = true;
                            link_PrintUSDACPM.Visible = true;
                            link_PrintPayrollDetail.Visible = true;
                            link_ImportTransmittalsGeneric.Visible = true;
                            link_PrintTargetInventory.Visible = true;
                            link_SetupContracts.Visible = true;
                            //link_PrintMenuAvgsOverall.Visible = true;
                            link_ImportSpFnPay.Visible = true;
                            link_CEPRecalcUtil.Visible = true;
                            break;
                        case "OFFICE":
                            MstrAdminSetup.Visible = false;
                            MstrAdminOfficeUtils.Visible = true;
                            MstrUserOnlyUtils.Visible = false;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                            link_SchoolInvoicingReportsGroup.Visible = true;
                            link_PrintOperatingStatistics.Visible = true;
                            link_PrintCompanyFoodCost.Visible = true;
                            link_PrintCompConsolMenu.Visible = true;
                            link_RMEOMReportsGroup.Visible = true;
                            link_PrintNotifications.Visible = true;
                            link_PrintInventoryDistTotals.Visible = true;
                            link_PrintParticipationChanges.Visible = true;
                            link_PrintPurchasesSummary.Visible = true;
                            link_PrintUsageSummary.Visible = true;
                            link_PrintUSDACPM.Visible = true;
                            link_PrintPayrollDetail.Visible = true;
                            link_ImportTransmittalsGeneric.Visible = false;
                            link_PrintTargetInventory.Visible = true;
                            //link_PrintMenuAvgsOverall.Visible = true;
                            link_ImportSpFnPay.Visible = true;
                            link_CEPRecalcUtil.Visible = false;
                            link_SetupContracts.Visible = true;
                            break;
                        case "RM":
                            MstrAdminSetup.Visible = false;
                            MstrAdminOfficeUtils.Visible = false;
                            MstrUserOnlyUtils.Visible = true;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                            link_SchoolInvoicingReportsGroup.Visible = true;
                            link_PrintOperatingStatistics.Visible = true;
                            link_PrintCompanyFoodCost.Visible = true;
                            link_PrintCompConsolMenu.Visible = true;
                            link_RMEOMReportsGroup.Visible = true;
                            link_PrintNotifications.Visible = true;
                            link_PrintInventoryDistTotals.Visible = true;
                            link_PrintParticipationChanges.Visible = true;
                            link_PrintPurchasesSummary.Visible = false;
                            link_PrintUsageSummary.Visible = false;
                            link_PrintUSDACPM.Visible = false;
                            link_PrintPayrollDetail.Visible = true;
                            link_ImportTransmittalsGeneric.Visible = false;
                            link_PrintTargetInventory.Visible = true;
                            //link_PrintMenuAvgsOverall.Visible = true;
                            link_ImportSpFnPay.Visible = false;
                            link_CEPRecalcUtil.Visible = false;
                            link_SetupContracts.Visible = true;
                            break;
                        default:
                            MstrAdminSetup.Visible = false;
                            MstrAdminOfficeUtils.Visible = false;
                            MstrUserOnlyUtils.Visible = true;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                            link_SchoolInvoicingReportsGroup.Visible = false;
                            link_PrintOperatingStatistics.Visible = false;
                            link_PrintCompanyFoodCost.Visible = false;
                            link_PrintCompConsolMenu.Visible = false;
                            link_RMEOMReportsGroup.Visible = false;
                            link_PrintNotifications.Visible = false;
                            link_PrintInventoryDistTotals.Visible = false;
                            link_PrintParticipationChanges.Visible = false;
                            link_PrintPurchasesSummary.Visible = false;
                            link_PrintUsageSummary.Visible = false;
                            link_PrintUSDACPM.Visible = true;
                            link_PrintPayrollDetail.Visible = false;
                            link_ImportTransmittalsGeneric.Visible = false;
                            link_PrintTargetInventory.Visible = false;
                            //link_PrintMenuAvgsOverall.Visible = false;
                            link_ImportSpFnPay.Visible = false;
                            link_CEPRecalcUtil.Visible = false;
                            link_SetupContracts.Visible = false;
                            break;

                    }

                }


                Paperwork_Global.UpdateMasterMenuStructure(this);

                //Toolbox specific changes
                if(isToolbox)
                {
                    labelSetupSchoolsLink.Text="Setup Buildings";
                    labelSetupSchoolInfoLink.Text = "Setup Building Info";
                    link_BudgetEntryFMS.Visible = true;
                    link_SetupBudgetYearsFMS.Visible = true;
                }
                else
                {
                    labelSetupSchoolsLink.Text = "Setup Schools";
                    labelSetupSchoolInfoLink.Text = "Setup School Info";
                    link_BudgetEntryFMS.Visible = false;
                    link_SetupBudgetYearsFMS.Visible = false;
                }

            }




                
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        public string MasterDistrictLabel
        {
            get
            {
                return CurrDistrict.Text;
            }
            set
            {
                CurrDistrict.Text = value;
            }
        }

        //Unified Districts list
        public SqlDataSource SQLMasterDistricts
        {
            get { return SqlDataSourceMyDistrictsMstr; }
        }

        //Unified Districts list Updated to only show Active and Inactive with Trx in Year
        public SqlDataSource SQLMasterDistrictsPY
        {
            get { return SqlDataSourceMyDistrictsMstrPY; }
        }

        public UpdatePanel MstrHdrUpdatePanel
        {
            get { return UpdatePanelMstrHdr; }
        }

        /* EXPOSE MASTER PAGE MENU LINKS VISIBILITY FOR CUSTOM SECURITY */
        /* IMPORTANT!!! - NEED TO UPDATE THE SECURITY CHECK IN Paperwork_Global WHEN ADDING HERE */
        #region Transactions
        public bool link_PurchasesVisibility
        {
            get { return link_Purchases.Visible; }
            set { link_Purchases.Visible=value; }
        }
        public bool link_SalesEntryVisibility
        {
            get { return link_SalesEntry.Visible; }
            set { link_SalesEntry.Visible = value; }
        }
        public bool link_DistrictPayrollEntryVisibility
        {
            get { return link_DistrictPayrollEntry.Visible; }
            set { link_DistrictPayrollEntry.Visible = value; }
        }
        public bool link_InventoryEntryVisibility
        {
            get { return link_InventoryEntry.Visible; }
            set { link_InventoryEntry.Visible = value; }
        }
        public bool link_InventoryVerificationSFSPVisibility
        {
            get { return link_InventoryVerificationSFSP.Visible; }
            set { link_InventoryVerificationSFSP.Visible = value; }
        }
        public bool link_OtherSalesVisibility
        {
            get { return link_OtherSales.Visible; }
            set { link_OtherSales.Visible = value; }
        }
        public bool link_SpecialFunctionsEntryVisibility
        {
            get { return link_SpecialFunctionsEntry.Visible; }
            set { link_SpecialFunctionsEntry.Visible = value; }
        }
        public bool link_DaysVisibility
        {
            get { return link_Days.Visible; }
            set { link_Days.Visible = value; }
        }
        public bool link_NotificationsVisibility
        {
            get { return link_Notifications.Visible; }
            set { link_Notifications.Visible = value; }
        }
        public bool link_EOMChecklistVisibility
        {
            get { return link_EOMChecklist.Visible; }
            set { link_EOMChecklist.Visible = value; }
        }
        public bool link_MenuAnalysisEntryVisibility
        {
            get { return link_MenuAnalysisEntry.Visible; }
            set { link_MenuAnalysisEntry.Visible = value; }
        }
        #endregion
        #region Reports
        public bool link_PrintBudgetReportVisibility
        {
            get { return link_PrintBudgetReport.Visible; }
            set { link_PrintBudgetReport.Visible = value; }
        }
        #region Closing Reports
        public bool link_ClosingReportsGroupVisibility
        {
            get { return link_ClosingReportsGroup.Visible; }
            set { link_ClosingReportsGroup.Visible = value; }
        }
        public bool link_PrintOperatingReportVisibility
        {
            get { return link_PrintOperatingReport.Visible; }
            set { link_PrintOperatingReport.Visible = value; }
        }
        public bool link_PrintOperatingStatisticsVisibility
        {
            get { return link_PrintOperatingStatistics.Visible; }
            set { link_PrintOperatingStatistics.Visible = value; }
        }
        public bool link_PrintCompanyFoodCostVisibility
        {
            get { return link_PrintCompanyFoodCost.Visible; }
            set { link_PrintCompanyFoodCost.Visible = value; }
        }
        public bool link_PrintCompConsolMenuVisibility
        {
            get { return link_PrintCompConsolMenu.Visible; }
            set { link_PrintCompConsolMenu.Visible = value; }
        }
        public bool link_PrintUSDACoverVisibility
        {
            get { return link_PrintUSDACover.Visible; }
            set { link_PrintUSDACover.Visible = value; }
        }
        public bool link_PrintDetailedProfitCPMVisibility
        {
            get { return link_PrintDetailedProfitCPM.Visible; }
            set { link_PrintDetailedProfitCPM.Visible = value; }
        }
        public bool link_PrintDetailedProfitFBVisibility
        {
            get { return link_PrintDetailedProfitFB.Visible; }
            set { link_PrintDetailedProfitFB.Visible = value; }
        }
        public bool link_PrintEOMChecklistVisibility
        {
            get { return link_PrintEOMChecklist.Visible; }
            set { link_PrintEOMChecklist.Visible = value; }
        }
        public bool link_PrintInventoryDistTotalsVisibility
        {
            get { return link_PrintInventoryDistTotals.Visible; }
            set { link_PrintInventoryDistTotals.Visible = value; }
        }
        public bool link_PrintInventoryVerificationVisibility
        {
            get { return link_PrintInventoryVerification.Visible; }
            set { link_PrintInventoryVerification.Visible = value; }
        }
        public bool link_PrintTargetInventoryVisibility
        {
            get { return link_PrintTargetInventory.Visible; }
            set { link_PrintTargetInventory.Visible = value; }
        }
        public bool link_PrintParticipationChangesVisibility
        {
            get { return link_PrintParticipationChanges.Visible; }
            set { link_PrintParticipationChanges.Visible = value; }
        }
        public bool link_PrintPurchasesSummaryVisibility
        {
            get { return link_PrintPurchasesSummary.Visible; }
            set { link_PrintPurchasesSummary.Visible = value; }
        }
        public bool link_PrintReinhartCreditsVisibility
        {
            get { return link_PrintReinhartCredits.Visible; }
            set { link_PrintReinhartCredits.Visible = value; }
        }
        public bool link_PrintUsageSummaryVisibility
        {
            get { return link_PrintUsageSummary.Visible; }
            set { link_PrintUsageSummary.Visible = value; }
        }
        public bool link_PrintUSDACPMVisibility
        {
            get { return link_PrintUSDACPM.Visible; }
            set { link_PrintUSDACPM.Visible = value; }
        }
        #endregion
        #region Edit Check Reports
        public bool link_EditChecksReportsGroupVisibility
        {
            get { return link_EditChecksReportsGroup.Visible; }
            set { link_EditChecksReportsGroup.Visible = value; }
        }
        public bool link_PrintEditCheckBySchoolVisibility
        {
            get { return link_PrintEditCheckBySchool.Visible; }
            set { link_PrintEditCheckBySchool.Visible = value; }
        }
        public bool link_PrintEditCheckSummaryVisibility
        {
            get { return link_PrintEditCheckSummary.Visible; }
            set { link_PrintEditCheckSummary.Visible = value; }
        }
        public bool link_PrintCN6Visibility
        {
            get { return link_PrintCN6.Visible; }
            set { link_PrintCN6.Visible = value; }
        }
        public bool link_PrintCN7Visibility
        {
            get { return link_PrintCN7.Visible; }
            set { link_PrintCN7.Visible = value; }
        }
        #endregion
        #region Menu Analysis Reports
        public bool link_MenuAnalysisReportsGroupVisibility
        {
            get { return link_MenuAnalysisReportsGroup.Visible; }
            set { link_MenuAnalysisReportsGroup.Visible = value; }
        }
        public bool link_PrintMenuAveragesVisibility
        {
            get { return link_PrintMenuAvgs.Visible; }
            set { link_PrintMenuAvgs.Visible = value; }
        }
        public bool link_PrintMenuAveragesOverallVisibility
        {
            get { return link_PrintMenuAvgsOverall.Visible; }
            set { link_PrintMenuAvgsOverall.Visible = value; }
        }
        public bool link_PrintMADailyMenusVisibility
        {
            get { return link_PrintMADailyMenus.Visible; }
            set { link_PrintMADailyMenus.Visible = value; }
        }
        #endregion
        public bool link_PrintLunchClaimVisibility
        {
            get { return link_PrintLunchClaim.Visible; }
            set { link_PrintLunchClaim.Visible = value; }
        }
        public bool link_PrintNotificationsVisibility
        {
            get { return link_PrintNotifications.Visible; }
            set { link_PrintNotifications.Visible = value; }
        }
        #region Payroll Reports
        public bool link_PrintPayrollVisibility
        {
            get { return link_PrintPayroll.Visible; }
            set { link_PrintPayroll.Visible = value; }
        }
        public bool link_PrintPayrollHealthVisibility
        {
            get { return link_PrintPayrollHealth.Visible; }
            set { link_PrintPayrollHealth.Visible = value; }
        }
        public bool link_PrintPayrollDetailVisibility
        {
            get { return link_PrintPayrollDetail.Visible; }
            set { link_PrintPayrollDetail.Visible = value; }
        }
        #endregion
        #region RMEOM Reports
        public bool link_RMEOMGroupVisibility
        {
            get { return link_RMEOMReportsGroup.Visible; }
            set { link_RMEOMReportsGroup.Visible = value; }
        }
        public bool link_PrintRMEOMVisibility
        {
            get { return link_PrintRMEOM.Visible; }
            set { link_PrintRMEOM.Visible = value; }
        }
        public bool link_RMEOMCommentsVisibility
        {
            get { return link_RMEOMComments.Visible; }
            set { link_RMEOMComments.Visible = value; }
        }
        #endregion
        #region Sales Reports
        public bool link_SalesReportsGroupVisibility
        {
            get { return link_SalesReportsGroup.Visible; }
            set { link_SalesReportsGroup.Visible = value; }
        }
        public bool link_PrintSalesVisibility
        {
            get { return link_PrintSales.Visible; }
            set { link_PrintSales.Visible = value; }
        }
        public bool link_PrintSalesRevisionsVisibility
        {
            get { return link_PrintSalesRevisions.Visible; }
            set { link_PrintSalesRevisions.Visible = value; }
        }
        public bool link_PrintSalesRevisionsDetailVisibility
        {
            get { return link_PrintSalesRevisionsDetail.Visible; }
            set { link_PrintSalesRevisionsDetail.Visible = value; }
        }
        public bool link_PrintSalesDistrictVisibility
        {
            get { return link_PrintSalesDistrict.Visible; }
            set { link_PrintSalesDistrict.Visible = value; }
        }
        public bool link_PrintSnackSalesVisibility
        {
            get { return link_PrintSnackSales.Visible; }
            set { link_PrintSnackSales.Visible = value; }
        }
        public bool link_PrintSalesAdultLunchVisibility
        {
            get { return link_PrintSalesAdultLunch.Visible; }
            set { link_PrintSalesAdultLunch.Visible = value; }
        }
        public bool link_PrintSalesNutrForLifeVisibility
        {
            get { return link_PrintSalesNutrForLife.Visible; }
            set { link_PrintSalesNutrForLife.Visible = value; }
        }
        public bool link_PrintSalesDinnerVisibility
        {
            get { return link_PrintSalesDinner.Visible; }
            set { link_PrintSalesDinner.Visible = value; }
        }
        #endregion
        #region Special Function Reports
        public bool link_PrintSpecialFuncInvoicesVisibility
        {
            get { return link_PrintSpecialFuncInvoices.Visible; }
            set { link_PrintSpecialFuncInvoices.Visible = value; }
        }
        public bool link_PrintSpFnLedgerVisibility
        {
            get { return link_PrintSpFnLedger.Visible; }
            set { link_PrintSpFnLedger.Visible = value; }
        }
        public bool div_PrintSpFnReportsGroupVisibility
        {
            get { return link_SpecialFunctionsReportsGroup.Visible; }
            set { link_SpecialFunctionsReportsGroup.Visible = value; }
        }
        #endregion
        #region Transmittal Reports
        public bool link_PrintAddOnFeesInvoiceVisibility
        {
            get { return link_PrintAddOnFeesInvoice.Visible; }
            set { link_PrintAddOnFeesInvoice.Visible = value; }
        }
        public bool link_PrintTransmittalsVisibility
        {
            get { return link_PrintTransmittals.Visible; }
            set { link_PrintTransmittals.Visible = value; }
        }
        public bool link_PrintTransmittalsOtherVisibility
        {
            get { return link_PrintTransmittalsOther.Visible; }
            set { link_PrintTransmittalsOther.Visible = value; }
        }
        public bool link_PrintOHCatTaxInvVisibility
        {
            get { return link_PrintOHCatTaxInv.Visible; }
            set { link_PrintOHCatTaxInv.Visible = value; }
        }
        #endregion

        public bool link_PrintSummarySFSPDistrictsVisibility
        {
            get { return link_PrintSFSPDistsSummary.Visible; }
            set { link_PrintSFSPDistsSummary.Visible = value; }
        }

        public bool link_SchoolInvoicingReportsVisibility
        {
            get { return link_SchoolInvoicingReportsGroup.Visible; }
            set { link_SchoolInvoicingReportsGroup.Visible = value; }
        }

        #endregion
        #region Setups
        public bool link_SetupDistrictInfoVisibility
        {
            get { return link_SetupDistrictInfo.Visible; }
            set { link_SetupDistrictInfo.Visible = value; }
        }
        public bool link_SetupContractsVisibility
        {
            get { return link_SetupContracts.Visible; }
            set { link_SetupContracts.Visible = value; }
        }
        public bool link_SetupSchoolsVisibility
        {
            get { return link_SetupSchools.Visible; }
            set { link_SetupSchools.Visible = value; }
        }
        public bool link_SetupSchoolInfoVisibility
        {
            get { return link_SetupSchoolInfo.Visible; }
            set { link_SetupSchoolInfo.Visible = value; }
        }
        public bool link_SetupDistrictVendorsVisibility
        {
            get { return link_SetupDistrictVendors.Visible; }
            set { link_SetupDistrictVendors.Visible = value; }
        }
        public bool link_SetupPaycodesVisibility
        {
            get { return link_SetupPaycodes.Visible; }
            set { link_SetupPaycodes.Visible = value; }
        }
        public bool link_SetupEmployeesVisibility
        {
            get { return link_SetupEmployees.Visible; }
            set { link_SetupEmployees.Visible = value; }
        }
        public bool link_SetupMenuAnalysisItemsVisibility
        {
            get { return link_SetupMenuAnalysisItems.Visible; }
            set { link_SetupMenuAnalysisItems.Visible = value; }
        }
        public bool link_SetupMilkDeliveryVisibility
        {
            get { return link_SetupMilkDelivery.Visible; }
            set { link_SetupMilkDelivery.Visible = value; }
        }
        public bool link_SetupOtherSalesVisibility
        {
            get { return link_SetupOtherSales.Visible; }
            set { link_SetupOtherSales.Visible = value; }
        }
        public bool link_SetupSpFnInvoicesVisibility
        {
            get { return link_SetupSpFnInvoices.Visible; }
            set { link_SetupSpFnInvoices.Visible = value; }
        }
        public bool link_SetupSpFnOrgsVisibility
        {
            get { return link_SetupSpFnOrgs.Visible; }
            set { link_SetupSpFnOrgs.Visible = value; }
        }
        public bool link_SetupTransOptsVisibility
        {
            get { return link_SetupTransOpts.Visible; }
            set { link_SetupTransOpts.Visible = value; }
        }
        public bool link_SetupBudgetYearsFMSVisibility
        {
            get { return link_SetupBudgetYearsFMS.Visible; }
            set { link_SetupBudgetYearsFMS.Visible = value; }
        }
        public bool link_BudgetEntryFMSVisibility
        {
            get { return link_BudgetEntryFMS.Visible; }
            set { link_BudgetEntryFMS.Visible = value; }
        }
        #endregion
        #region Utilities
        #region User Only Utilities
        public bool link_ImportPOSUsrVisibility
            {
                get { return link_ImportPOSUsr.Visible; }
                set { link_ImportPOSUsr.Visible = value; }
            }
            #endregion
            #region Mgmt & Office Utilities
            #region Exports
            public bool link_ExportFBPassThruVisibility
            {
                get { return link_ExportFBPassThru.Visible; }
                set { link_ExportFBPassThru.Visible = value; }
            }
            public bool link_ExportInvChangeVisibility
            {
                get { return link_ExportInvChange.Visible; }
                set { link_ExportInvChange.Visible = value; }
            }
            public bool link_PayablesExportVisibility
            {
                get { return link_PayablesExport.Visible; }
                set { link_PayablesExport.Visible = value; }
            }
            #endregion
            #region Imports
            public bool link_ImportAddonFeesVisibility
            {
                get { return link_ImportAddonFees.Visible; }
                set { link_ImportAddonFees.Visible = value; }
            }
            public bool link_ImportExtraChefVisibility
            {
                get { return link_ImportExtraChef.Visible; }
                set { link_ImportExtraChef.Visible = value; }
            }
            public bool link_ImportIndirectCostsVisibility
            {
                get { return link_ImportIndirectCosts.Visible; }
                set { link_ImportIndirectCosts.Visible = value; }
            }
            public bool link_ImportInsuranceVisibility
            {
                get { return link_ImportInsurance.Visible; }
                set { link_ImportInsurance.Visible = value; }
            }
            public bool link_ImportPayrollVisibility
            {
                get { return link_ImportPayroll.Visible; }
                set { link_ImportPayroll.Visible = value; }
            }
            public bool link_ImportPOSMgtVisibility
            {
                get { return link_ImportPOS.Visible; }
                set { link_ImportPOS.Visible = value; }
            }
            public bool link_ImportReinhartCreditsVisibility
            {
                get { return link_ImportReinhartCredits.Visible; }
                set { link_ImportReinhartCredits.Visible = value; }
            }
            public bool link_ImportReinhartVisibility
            {
                get { return link_ImportReinhart.Visible; }
                set { link_ImportReinhart.Visible = value; }
            }
            public bool link_ImportGallikerVisibility
            {
                get { return link_ImportGalliker.Visible; }
                set { link_ImportGalliker.Visible = value; }
            }
            public bool link_ImportMorabitoVisibility
            {
                get { return link_ImportMorabito.Visible; }
                set { link_ImportMorabito.Visible = value; }
            }
            public bool link_ImportTransmittalsGenericVisibility
            {
                get { return link_ImportTransmittalsGeneric.Visible; }
                set { link_ImportTransmittalsGeneric.Visible = value; }
            }
            public bool link_ImportSpFnPayVisibility
            {
                get { return link_ImportSpFnPay.Visible; }
                set { link_ImportSpFnPay.Visible = value; }
            }
            public bool link_CEPRecalcUtilVisibility
            {
                get { return link_CEPRecalcUtil.Visible; }
                set { link_CEPRecalcUtil.Visible = value; }
            }
        #endregion
        #region Invoice Updates
        public bool link_InvoiceCreditsVisibility
                {
                    get { return link_InvoiceCredits.Visible; }
                    set { link_InvoiceCredits.Visible = value; }
                }
                public bool link_RMAdjustmentsVisibility
                {
                    get { return link_RMAdjustments.Visible; }
                    set { link_RMAdjustments.Visible = value; }
                }
                public bool link_FBPrepaysVisibility
                {
                    get { return link_FBPrepays.Visible; }
                    set { link_FBPrepays.Visible = value; }
                }
                public bool link_InvAdjVisibility
                {
                    get { return link_InvAdj.Visible; }
                    set { link_InvAdj.Visible = value; }
                }
                #endregion
            public bool link_LockTransactionsVisibility
            {
                get { return link_LockTransactions.Visible; }
                set { link_LockTransactions.Visible = value; }
            }
            public bool link_PayrollAccrualsVisibility
            {
                get { return link_PayrollAccruals.Visible; }
                set { link_PayrollAccruals.Visible = value; }
            }
            #endregion
        #endregion

    }
}
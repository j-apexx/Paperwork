using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class _Default : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            SqlDataSourceInvYear.DataBind();
            DataView dvSql = (DataView)SqlDataSourceInvYear.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                DataRowView drvSql = dvSql[0];
                LabelInventoryYear.Text = drvSql["ProgramYear"].ToString().TrimEnd(' ');

                //only set if user has not already selected a working PY
                if (Session["ProgramYear"]==null)
                    Session["ProgramYear"] = LabelInventoryYear.Text.TrimEnd(' ');

            }
            else
            {
                LabelInventoryYear.Text = "**Not Open**";
                Session.Remove("ProgramYear");
            }

            if (!IsPostBack)
            {
                //Using BaseFolder to check for alternate versions including training company, SFSP and Toolbox
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"].ToUpper();

                if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING") > 0)
                {
                    LiteralHDRGeneralPaperwork.Visible = true;
                    LiteralHDRGeneralToolbox.Visible = false;
                    LabelTrainIndicator.Text = "Training";
                    LiteralTrainingMessage.Visible = true;

                    //using Session variable to popup message only on first access
                    if (Session["CurrentDistrict"] == null)
                        ShowDialog("--Training Only--", MessageType.Error, "** Do not use this site to enter live data! **", "");
                }
                else if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBSFSP") > 0)
                {
                    LiteralHDRGeneralPaperwork.Visible = true;
                    LiteralHDRGeneralToolbox.Visible = false;
                    LabelTrainIndicator.Text = "SFSP";
                    LiteralSFSPMessage.Visible = true;
                }
                else if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKTOOLBOX") > 0)
                {
                    LiteralHDRGeneralPaperwork.Visible = false;
                    LiteralHDRGeneralToolbox.Visible = true; 
                    LabelTrainIndicator.Visible = false;
                    LiteralGeneralPaperwork.Visible = false;
                    LiteralToolboxMessage.Visible = true;
                }

                //Set the programyear
                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count >=1)
                {
                    if (Session["ProgramYear"]!= null && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }

                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
                SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;

                DropDownListDistricts.DataBind();

                if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                {
                    string lastDist = GetLastDistrictID();
                    if (lastDist != String.Empty)
                    {
                        ListItem li = DropDownListDistricts.Items.FindByValue(lastDist);
                        if (li != null)
                            li.Selected = true;
                    }

                    DropDownListDistricts_SelectedIndexChanged(null, null);
                }
                else
                {
                    if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                    else
                        DropDownListDistricts_SelectedIndexChanged(null, null);
                }
            }

            if (Session["AccessLevel"] != null)
            {

                switch (Session["AccessLevel"].ToString().ToUpper())
                {
                    case "ADMIN":
                        DivAdminSetup.Visible = true;
                        DivAdminOfficeUtilsSub.Visible = true;
                        DivUserOnlyUtils.Visible = false;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                        DivMgmtReports.Visible = true;
                        link_PrintOperatingStatistics.Visible = true;
                        link_PrintCompanyFoodCost.Visible = true;
                        link_PrintCompConsolMenu.Visible = true;
                        link_PrintRMEOM.Visible = true;
                        link_RMEOMComments.Visible = true;
                        link_PrintNotifications.Visible = true;
                        link_PrintInventoryDistTotals.Visible = true;
                        link_PrintPurchasesSummary.Visible = true;
                        link_PrintParticipationChanges.Visible = true;
                        link_PrintUsageSummary.Visible = true;
                        link_PrintUSDACPM.Visible = true;
                        link_PrintPayrollDetail.Visible = true;
                        link_SetupTransOpts.Visible = true;
                        link_CEPRecalcUtil.Visible = true;
                        link_SetupContracts.Visible = true;
                        //link_PrintMenuAvgOverall.Visible = true;
                        if (isSFSP())
                        {
                            link_PrintSFSPDistsSummary.Visible = true;
                            link_InventoryVerificationSFSP.Visible = true;
                            link_PrintInventoryVerification.Visible = true;
                        }
                        else
                        {
                            link_PrintSFSPDistsSummary.Visible = false;
                            link_InventoryVerificationSFSP.Visible = false;
                            link_PrintInventoryVerification.Visible = false;
                        }
                        link_ImportTransmittalsGeneric.Visible = true;
                        link_ImportSpFnPay.Visible = true;
                        break;
                    case "OFFICE":
                        DivAdminSetup.Visible = false;
                        DivAdminOfficeUtilsSub.Visible = true;
                        DivUserOnlyUtils.Visible = false;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                        DivMgmtReports.Visible = true;
                        link_PrintOperatingStatistics.Visible = true; 
                        link_PrintCompanyFoodCost.Visible = true;
                        link_PrintCompConsolMenu.Visible = true;
                        link_PrintRMEOM.Visible = true;
                        link_RMEOMComments.Visible = true;
                        link_PrintNotifications.Visible = true;
                        link_PrintInventoryDistTotals.Visible = true;
                        link_PrintPurchasesSummary.Visible = true;
                        link_PrintParticipationChanges.Visible = true;
                        link_PrintUsageSummary.Visible = true;
                        link_PrintUSDACPM.Visible = true;
                        link_PrintPayrollDetail.Visible = true;
                        link_SetupTransOpts.Visible = true;
                        link_CEPRecalcUtil.Visible = false;
                        link_SetupContracts.Visible = true;
                        //link_PrintMenuAvgOverall.Visible = true;
                        if (isSFSP())
                        {
                            link_PrintSFSPDistsSummary.Visible = true;
                            link_InventoryVerificationSFSP.Visible = true;
                            link_PrintInventoryVerification.Visible = true;
                        }
                        else
                        {
                            link_PrintSFSPDistsSummary.Visible = false;
                            link_InventoryVerificationSFSP.Visible = false;
                            link_PrintInventoryVerification.Visible = false;
                        }
                        link_ImportTransmittalsGeneric.Visible = false;
                        link_ImportSpFnPay.Visible = true;
                        break;
                    case "RM":
                        DivAdminSetup.Visible = false;
                        DivAdminOfficeUtilsSub.Visible = false;
                        DivUserOnlyUtils.Visible = true;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                        DivMgmtReports.Visible = true;
                        link_PrintOperatingStatistics.Visible = true;
                        link_PrintCompanyFoodCost.Visible = true;
                        link_PrintCompConsolMenu.Visible = true;
                        link_PrintRMEOM.Visible = true;
                        link_RMEOMComments.Visible = true;
                        link_PrintNotifications.Visible = true;
                        link_PrintInventoryDistTotals.Visible = true;
                        link_PrintPurchasesSummary.Visible = false;
                        link_PrintParticipationChanges.Visible = true;
                        link_PrintUsageSummary.Visible = false;
                        link_PrintUSDACPM.Visible = false;
                        link_PrintPayrollDetail.Visible = true;
                        link_PrintSFSPDistsSummary.Visible = false;
                        link_SetupTransOpts.Visible = false;
                        link_CEPRecalcUtil.Visible = false;
                        link_SetupContracts.Visible = true;
                        //link_PrintMenuAvgOverall.Visible = true;
                        if (isSFSP())
                        {
                            link_InventoryVerificationSFSP.Visible = true;
                            link_PrintInventoryVerification.Visible = true;
                        }
                        else
                        {
                            link_InventoryVerificationSFSP.Visible = false;
                            link_PrintInventoryVerification.Visible = false;
                        }
                        link_ImportTransmittalsGeneric.Visible = false;
                        link_ImportSpFnPay.Visible = false;
                        break;
                    default:
                        DivAdminSetup.Visible = false;
                        DivAdminOfficeUtilsSub.Visible = false;
                        DivUserOnlyUtils.Visible = true;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                        DivMgmtReports.Visible = false;
                        link_PrintOperatingStatistics.Visible = false; 
                        link_PrintCompanyFoodCost.Visible = false;
                        link_PrintCompConsolMenu.Visible = false;
                        link_PrintRMEOM.Visible = false;
                        link_RMEOMComments.Visible = false;
                        link_PrintNotifications.Visible = false;
                        link_PrintInventoryDistTotals.Visible = false;
                        link_PrintPurchasesSummary.Visible = false;
                        link_PrintParticipationChanges.Visible = false;
                        link_PrintUsageSummary.Visible = false;
                        link_PrintUSDACPM.Visible = false;
                        link_PrintPayrollDetail.Visible = false;
                        link_PrintSFSPDistsSummary.Visible = false;
                        link_SetupTransOpts.Visible = false;
                        link_CEPRecalcUtil.Visible = false;
                        link_SetupContracts.Visible = false;
                        //link_PrintMenuAvgOverall.Visible = false;
                        if (isSFSP())
                        {
                            link_InventoryVerificationSFSP.Visible = true;
                            link_PrintInventoryVerification.Visible = true;
                        }
                        else
                        {
                            link_InventoryVerificationSFSP.Visible = false;
                            link_PrintInventoryVerification.Visible = false;
                        }
                        link_ImportTransmittalsGeneric.Visible = false;
                        link_ImportSpFnPay.Visible = false;
                        break;
                }

            }

            //April 2023 SFSP Links to hide
            if(isSFSP())
            {
                link_SpecialFunctionsEntry.Visible = false;
                link_PrintSpecialFuncInvoices.Visible = false;
                link_PrintSpFnLedger.Visible = false;
                link_SetupSpFnOrgs.Visible = false;
                link_SetupSpFnInvoices.Visible = false;
                DivSpecialFunctionReports.Visible = false;

                link_EOMChecklist.Visible = false;
                link_PrintEOMChecklist.Visible=false;

                //utilities
                link_ImportReinhartCredits.Visible = false;
                link_ImportReinhart.Visible= false;
                link_ImportSpFnPay.Visible = false;
            }

            //SEP 2024 - this is just for the training site to allow the New Special Functions Entry page to be accessed
            //MAR 2025 - Commenting this out, this was just supposed to be temporary for Sarah Testing.
            //if(isTRAINING())
            //{
            //    link_SpecialFunctionsEntry.HRef = "SpecialFunctionsEntryTEST.aspx";
            //    link_SalesEntry.HRef = "SalesEntryTEST.aspx";
            //    link_PrintOperatingReport_TEST.Visible = true;
            //}

#if !DEBUG
            if(isTOOLBOX() || isTRAINING())
            {
                link_SetupBudgetYearFMS.Visible = true;
                link_BudgetEntryFMS.Visible = true;
            }
            else
            {
                link_SetupBudgetYearFMS.Visible = false;
                link_BudgetEntryFMS.Visible = false;
            }
#endif


        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            //Due to order of page lifecycle - Page Load runs first, then Master Page Load updates the menu structure, then this will run and copy the Master Page Menu structure
            //Set Default page link visibility to mirror the Site.Master menu structure link visibility
            #region TRANSACTIONS
            link_Purchases.Visible = Master.link_PurchasesVisibility;
            link_SalesEntry.Visible = Master.link_SalesEntryVisibility;
            link_DistrictPayrollEntry.Visible = Master.link_DistrictPayrollEntryVisibility;
            link_InventoryEntry.Visible = Master.link_InventoryEntryVisibility;
            link_InventoryVerificationSFSP.Visible = Master.link_InventoryVerificationSFSPVisibility;
            link_OtherSales.Visible = Master.link_OtherSalesVisibility;
            link_SpecialFunctionsEntry.Visible = Master.link_SpecialFunctionsEntryVisibility;
            link_Days.Visible = Master.link_DaysVisibility;
            link_Notifications.Visible = Master.link_NotificationsVisibility;
            link_EOMChecklist.Visible = Master.link_EOMChecklistVisibility;
            link_MenuAnalysisEntry.Visible = Master.link_MenuAnalysisEntryVisibility;
            #endregion
            #region REPORTS
            /* REPORTS */
            link_PrintBudgetReport.Visible = Master.link_PrintBudgetReportVisibility;
            /* Closing Reports */
            link_PrintOperatingReport.Visible = Master.link_PrintOperatingReportVisibility;
            link_PrintOperatingStatistics.Visible = Master.link_PrintOperatingStatisticsVisibility;
            link_PrintCompanyFoodCost.Visible=Master.link_PrintCompanyFoodCostVisibility;
            link_PrintCompConsolMenu.Visible = Master.link_PrintCompConsolMenuVisibility;
            link_PrintUSDACover.Visible = Master.link_PrintUSDACoverVisibility;
            link_PrintDetailedProfitCPM.Visible = Master.link_PrintDetailedProfitCPMVisibility;
            link_PrintDetailedProfitFB.Visible = Master.link_PrintDetailedProfitFBVisibility;
            link_PrintEOMChecklist.Visible = Master.link_PrintEOMChecklistVisibility;
            link_PrintInventoryDistTotals.Visible = Master.link_PrintInventoryDistTotalsVisibility;
            link_PrintInventoryVerification.Visible = Master.link_PrintInventoryVerificationVisibility;
            link_PrintTargetInventory.Visible = Master.link_PrintTargetInventoryVisibility;
            link_PrintPurchasesSummary.Visible = Master.link_PrintPurchasesSummaryVisibility;
            link_PrintParticipationChanges.Visible = Master.link_PrintParticipationChangesVisibility;
            link_PrintReinhartCredits.Visible = Master.link_PrintReinhartCreditsVisibility;
            link_PrintUsageSummary.Visible = Master.link_PrintUsageSummaryVisibility;
            link_PrintUSDACPM.Visible = Master.link_PrintUSDACPMVisibility;
            if(link_PrintOperatingReport.Visible==false && link_PrintOperatingStatistics.Visible==false && link_PrintCompanyFoodCost.Visible==false && link_PrintUSDACover.Visible==false &&
                link_PrintDetailedProfitCPM.Visible==false && link_PrintDetailedProfitFB.Visible==false && link_PrintEOMChecklist.Visible==false && link_PrintInventoryDistTotals.Visible==false &&
                link_PrintPurchasesSummary.Visible==false && link_PrintReinhartCredits.Visible==false && link_PrintUsageSummary.Visible==false&& link_PrintUSDACPM.Visible==false && link_PrintCompConsolMenu.Visible==false) 
                DivClosingReports.Visible = false;
            DivClosingReports.Visible = Master.link_ClosingReportsGroupVisibility;

            /* Edit Check Reports */
            link_PrintEditCheckBySchool.Visible = Master.link_PrintEditCheckBySchoolVisibility;
            link_PrintEditCheckSummary.Visible = Master.link_PrintEditCheckSummaryVisibility;
            link_PrintCN6.Visible = Master.link_PrintCN6Visibility;
            link_PrintCN7.Visible = Master.link_PrintCN7Visibility;
            if(link_PrintEditCheckBySchool.Visible==false && link_PrintEditCheckSummary.Visible== false && link_PrintCN6.Visible== false && link_PrintCN7.Visible==false)
                DivEditCheckReports.Visible = false;
            DivEditCheckReports.Visible=Master.link_EditChecksReportsGroupVisibility;

            /* Menu Analysis */
            link_PrintMenuAvg.Visible = Master.link_PrintMenuAveragesVisibility;
            link_PrintMenuAvgOverall.Visible = Master.link_PrintMenuAveragesOverallVisibility;
            link_PrintMADailyMenus.Visible = Master.link_PrintMADailyMenusVisibility;
            if (link_PrintMenuAvg.Visible== false && link_PrintMenuAvgOverall.Visible==false && link_PrintMADailyMenus.Visible==false)
                DivMenuAnalysisReports.Visible = false;
            DivMenuAnalysisReports.Visible=Master.link_MenuAnalysisReportsGroupVisibility;

            link_PrintLunchClaim.Visible = Master.link_PrintLunchClaimVisibility;
            link_PrintNotifications.Visible= Master.link_PrintNotificationsVisibility;
            /* Payroll Reports */
            link_PrintPayroll.Visible= Master.link_PrintPayrollVisibility;
            link_PrintPayrollHealth.Visible= Master.link_PrintPayrollHealthVisibility;
            link_PrintPayrollDetail.Visible = Master.link_PrintPayrollDetailVisibility;
            if (link_PrintPayroll.Visible== false && link_PrintPayrollHealth.Visible== false && link_PrintPayrollDetail.Visible==false)
                DivPayrollReports.Visible = false;

            /* RMEOM Reports */
            link_PrintRMEOM.Visible= Master.link_PrintRMEOMVisibility;
            link_RMEOMComments.Visible = Master.link_RMEOMCommentsVisibility;

            /* Sales Reports */
            link_PrintSales.Visible = Master.link_PrintSalesVisibility;
            link_PrintSalesRevisions.Visible= Master.link_PrintSalesRevisionsVisibility;
            link_PrintSalesRevisionsDetail.Visible=Master.link_PrintSalesRevisionsDetailVisibility;
            link_PrintSalesDistrict.Visible=Master.link_PrintSalesDistrictVisibility;
            link_PrintSnackSales.Visible=Master.link_PrintSnackSalesVisibility;
            link_PrintSalesAdultLunch.Visible = Master.link_PrintSalesAdultLunchVisibility;
            link_PrintSalesDinner.Visible = Master.link_PrintSalesDinnerVisibility;
            link_PrintSalesNutrForLife.Visible = Master.link_PrintSalesNutrForLifeVisibility;
            if (link_PrintSales.Visible== false && link_PrintSalesRevisions.Visible== false && link_PrintSalesRevisionsDetail.Visible== false && link_PrintSalesDistrict.Visible== false &&
                link_PrintSnackSales.Visible== false && link_PrintSalesAdultLunch.Visible== false && link_PrintSalesDinner.Visible== false && link_PrintSalesNutrForLife.Visible == false)
                DivSalesReports.Visible = false;
            DivSalesReports.Visible = Master.link_SalesReportsGroupVisibility;

            /* Special Functions Reports */
            link_PrintSpecialFuncInvoices.Visible = Master.link_PrintSpecialFuncInvoicesVisibility;
            link_PrintSpFnLedger.Visible=Master.link_PrintSpFnLedgerVisibility;
            if(link_PrintSpecialFuncInvoices.Visible== false && link_PrintSpFnLedger.Visible==false)
                DivSpecialFunctionReports.Visible = false;
            DivSpecialFunctionReports.Visible = Master.div_PrintSpFnReportsGroupVisibility;

            /* Transmittal Reports */
            link_PrintAddOnFeesInvoice.Visible = Master.link_PrintAddOnFeesInvoiceVisibility;
            link_PrintTransmittals.Visible=Master.link_PrintTransmittalsVisibility;
            link_PrintTransmittalsOther.Visible = Master.link_PrintTransmittalsOtherVisibility;
            link_PrintOHCatTaxInv.Visible = Master.link_PrintOHCatTaxInvVisibility;
            if (link_PrintAddOnFeesInvoice.Visible== false && link_PrintTransmittals.Visible== false && link_PrintTransmittalsOther.Visible==false && link_PrintOHCatTaxInv.Visible)
                DivTransmittalReports.Visible = false;

            /* SFSP Reports */
            link_PrintSFSPDistsSummary.Visible = Master.link_PrintSummarySFSPDistrictsVisibility;

            /* School Invoicing */
            DivMgmtReports.Visible = Master.link_SchoolInvoicingReportsVisibility;

            #endregion
            #region SETUPS
            link_SetupDistrictInfo.Visible=Master.link_SetupDistrictInfoVisibility;
            link_SetupContracts.Visible=Master.link_SetupContractsVisibility;
            link_SetupSchools.Visible=Master.link_SetupSchoolsVisibility;
            if(isTOOLBOX())
                labelSetupSchoolsLink.Text = "Setup Buildings";
            else
                labelSetupSchoolsLink.Text = "Setup Schools";
            link_SetupSchoolInfo.Visible= Master.link_SetupSchoolInfoVisibility;
            if(isTOOLBOX())
                labelSetupSchoolInfoLink.Text = "Setup Building Info";
            else
                labelSetupSchoolInfoLink.Text = "Setup School Info";
            link_SetupDistrictVendors.Visible = Master.link_SetupDistrictVendorsVisibility;
            link_SetupPaycodes.Visible=Master.link_SetupPaycodesVisibility;
            link_SetupEmployees.Visible=Master.link_SetupEmployeesVisibility;
            link_SetupMenuAnalysisItems.Visible = Master.link_SetupMenuAnalysisItemsVisibility;
            link_SetupMilkDelivery.Visible=Master.link_SetupMilkDeliveryVisibility;
            link_SetupOtherSales.Visible=Master.link_SetupOtherSalesVisibility;
            link_SetupSpFnInvoices.Visible = Master.link_SetupSpFnInvoicesVisibility;
            link_SetupSpFnOrgs.Visible = Master.link_SetupSpFnOrgsVisibility;
            link_SetupTransOpts.Visible = Master.link_SetupTransOptsVisibility;
            link_BudgetEntryFMS.Visible = Master.link_BudgetEntryFMSVisibility;
            link_SetupBudgetYearFMS.Visible = Master.link_SetupBudgetYearsFMSVisibility;
            #endregion
            #region UTILITIES
            link_ImportPOSUsr.Visible=Master.link_ImportPOSUsrVisibility;
            link_ExportFBPassThru.Visible = Master.link_ExportFBPassThruVisibility;
            link_ExportInvChange.Visible=Master.link_ExportInvChangeVisibility;
            link_PayablesExport.Visible=Master.link_PayablesExportVisibility;
            link_ImportAddonFees.Visible=Master.link_ImportAddonFeesVisibility;
            link_ImportExtraChef.Visible=Master.link_ImportExtraChefVisibility;
            link_ImportIndirectCosts.Visible=Master.link_ImportIndirectCostsVisibility;
            link_ImportInsurance.Visible=Master.link_ImportInsuranceVisibility;
            link_ImportPayroll.Visible=Master.link_ImportPayrollVisibility;
            link_ImportPOSMgt.Visible=Master.link_ImportPOSMgtVisibility;
            link_ImportReinhartCredits.Visible = Master.link_ImportReinhartCreditsVisibility;
            link_ImportReinhart.Visible = Master.link_ImportReinhartVisibility;
            link_ImportGalliker.Visible = Master.link_ImportGallikerVisibility;
            link_ImportMorabito.Visible = Master.link_ImportMorabitoVisibility;
            link_InvoiceCredits.Visible=Master.link_InvoiceCreditsVisibility;
            link_RMAdjustments.Visible = Master.link_RMAdjustmentsVisibility;
            link_FBPrepays.Visible=Master.link_FBPrepaysVisibility;
            link_InvAdj.Visible = Master.link_InvAdjVisibility;
            link_LockTransactions.Visible=Master.link_LockTransactionsVisibility;
            link_PayrollAccruals.Visible = Master.link_PayrollAccrualsVisibility;
            link_ImportTransmittalsGeneric.Visible = Master.link_ImportTransmittalsGenericVisibility;
            link_ImportSpFnPay.Visible = Master.link_ImportSpFnPayVisibility;
            #endregion

        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (Session["ProgramYear"] == null)
                e.Command.Parameters["@ProgramYear"].Value = "";
            else
                e.Command.Parameters["@ProgramYear"].Value = Session["ProgramYear"].ToString();
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
            }
            else
            {
                Session.Remove("CurrentDistrict");
                Session.Remove("CurrentDistrictName");
                this.Master.MasterDistrictLabel = " No District Selected ";

            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
        }
    }
}
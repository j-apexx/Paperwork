using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class BudgetEntryFMS : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            //Set Dists data source and select event before calling DDLProgramYears_SelectedIndexChanged
            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;

            if (!IsPostBack)
            {
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

                DropDownListDistricts.DataBind();

                if (DropDownListDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();

                }

                GridViewBudgetHdrs.DataBind();
                if (GridViewBudgetHdrs.Rows.Count > 0 && GridViewBudgetHdrs.SelectedIndex==-1)
                {
                    GridViewBudgetHdrs.SelectRow(0);
                    GridViewBudgetHdrs.SelectedIndex = 0;
                }

            }
        }

        protected void DDLBudgetYears_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
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
                this.Master.MstrHdrUpdatePanel.Update();
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void ISave_Click(object sender, EventArgs e)
        {
            FormViewBudget.UpdateItem(false);
        }

        protected void IDelete_Click(object sender, EventArgs e)
        {
            if(IDelete.Enabled)
                FormViewBudget.DeleteItem();
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            Session["BudgetYear"] = HFBudgetYear.Value;
            Session["BudgetVersion"] = HFBudgetVersion.Value;
            //Response.Redirect("~/Print/PrintDeliverySheet.aspx");
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('../Print/PrintFMSBudget.aspx' ,'_blank');", true);

        }

        protected void FormViewBudget_DataBound(object sender, EventArgs e)
        {
            //Used to access the data object directly
            FormView frm = (FormView)sender;
            DataRowView drv = null;
            if (frm != null)
            {
                drv = (DataRowView)frm.DataItem;
            }

            if (drv == null)
            {
                frm.Enabled = false;
                IDelete.Enabled = false;
                IDelete.ForeColor = System.Drawing.Color.Gray;
                //ISave.Enabled = false;
                //ISave.ForeColor = System.Drawing.Color.Gray;
                IClone.Enabled = false;
                IClone.ForeColor = System.Drawing.Color.Gray;
                ILock.Enabled = false;
                ILock.ForeColor = System.Drawing.Color.Gray;
                IUnlock.Visible = false;

                return;
            }
            else
            {
                bool BudgetEditsAllowed = BudgetYearAllowsEdits(drv["BudgetYear"].ToString());
                bool BudgetLocked = (bool)drv["LockedBudget"];

                if(BudgetLocked)
                {
                    ILock.Visible = false;
                    if(Session["AccessLevel"].ToString().ToUpper()=="ADMIN")
                        IUnlock.Visible = true;
                    else
                        IUnlock.Visible = false;
                }
                else
                {
                    ILock.Visible = true;
                    IUnlock.Visible = false;
                }

                if (BudgetEditsAllowed && BudgetLocked==false)
                {
                    frm.Enabled = true;
                    IDelete.Enabled = true;
                    IDelete.ForeColor = System.Drawing.Color.Red;
                    //ISave.Enabled = true;
                    //ISave.ForeColor = System.Drawing.Color.Green;
                    ILock.Enabled = true;
                    ILock.ForeColor = System.Drawing.Color.DarkGoldenrod;
                    IUnlock.Enabled = true;
                    IUnlock.ForeColor = System.Drawing.Color.DarkGoldenrod;
                }
                else
                {
                    frm.Enabled = false;
                    IDelete.Enabled = false;
                    IDelete.ForeColor = System.Drawing.Color.Gray;
                    IDelete.Visible = false;
                    //ISave.Enabled = false;
                    //ISave.ForeColor = System.Drawing.Color.Gray;
                    if (BudgetEditsAllowed && Session["AccessLevel"].ToString().ToUpper() == "ADMIN")
                    {
                        ILock.Enabled = true;
                        ILock.ForeColor = System.Drawing.Color.DarkGoldenrod;
                        IUnlock.Enabled = true;
                        IUnlock.ForeColor = System.Drawing.Color.DarkGoldenrod;
                    }
                    else
                    {
                        ILock.Enabled = false;
                        ILock.ForeColor = System.Drawing.Color.Gray;
                        IUnlock.Enabled = false;
                        IUnlock.ForeColor = System.Drawing.Color.Gray;
                    }
                }

                IClone.Enabled = true;
                IClone.ForeColor = System.Drawing.Color.Empty;

                RegisterDOMReadyScript("RecalcAll", "RecalcAll();");
            }
        }

        protected void GridViewBudgetHdrs_PageIndexChanged(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;
            gv.DataBind();
            gv.SelectRow(0);
        }

        protected void GridViewBudgetHdrs_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            RowHighlight(gv);

            //Update the displayed record in the lower panel
            if (gv.SelectedIndex != -1 && (gv.SelectedIndex + 1) <= gv.DataKeys.Count)
            {
                HFBudgetYear.Value = gv.DataKeys[(gv.SelectedIndex)].Values["BudgetYear"].ToString();
                HFDistrictID.Value = gv.DataKeys[(gv.SelectedIndex)].Values["DistrictID"].ToString();
                HFBudgetVersion.Value = gv.DataKeys[(gv.SelectedIndex)].Values["BudgetVersion"].ToString();

                LabelBudgetYearSrc.Text = HFBudgetYear.Value;
                LabelBudgetVersionSrc.Text = HFBudgetVersion.Value;

                //Clear out the Hourly Employee Stored Values for new records
                HFHourlyEmpName.Value = string.Empty;
                HFHourlySchoolNum.Value = string.Empty;
                HFHourlyJobClass.Value = string.Empty;
                HFHourlySchedule.Value = string.Empty;
                HFHourlyRate.Value = string.Empty;
                HFHourlyHours.Value = string.Empty;
                HFHourlyDays.Value = string.Empty;
                HFHourlyPlanType.Value = string.Empty;
                HFHourlyHealth.Value = string.Empty;
                HFHourlyDental.Value = string.Empty;
                HFHourlyDisability.Value = string.Empty;
                HFHourlyVision.Value = string.Empty;
                HFHourlyAnnuity.Value = string.Empty;
                HFHourlyRetire.Value = string.Empty;
                HFHourlyLife.Value = string.Empty;
                HFHourlyOther.Value = string.Empty;


                FormViewBudget.DataBind();

            }
            else
            {
                HFBudgetYear.Value = string.Empty;
                HFDistrictID.Value = string.Empty;
                HFBudgetVersion.Value = string.Empty;

                LabelBudgetYearSrc.Text = HFBudgetYear.Value;
                LabelBudgetVersionSrc.Text = HFBudgetVersion.Value;
            }
        }

        /// <summary>
        /// For some reason, when deleting the _SelectedIndexChanged runs twice, and forgets to keep the row highlight. Below reapplies the row highlight without
        /// changing the HFTransferNumber for the formview
        /// </summary>
        /// <param name="sender"></param>
        protected void RowHighlight(GridView sender)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            //Highlight the selected row
            foreach (GridViewRow row in gv.Rows)
            {
                if (row.RowIndex == gv.SelectedIndex)
                {
                    row.ToolTip = string.Empty;
                    row.BorderColor = Color.Green;
                    row.BorderWidth = 2;
                    row.BorderStyle = BorderStyle.Solid;

                    //Latest Firefox update does not display row borders
                    foreach (TableCell tc in row.Cells)
                    {
                        tc.CssClass += " blueHighighter";
                    }
                }
                else
                {
                    row.ToolTip = "Click to select this row.";
                    row.BorderColor = Color.Empty;
                    row.BorderWidth = 0;
                    row.ForeColor = Color.Empty;

                    //Latest Firefox update does not display row borders
                    foreach (TableCell tc in row.Cells)
                    {
                        tc.CssClass = tc.CssClass.Replace(" blueHighighter", "");
                    }
                }
            }
        }

        protected void FormViewBudget_PreRender(object sender, EventArgs e)
        {
            if (GridViewBudgetHdrs.SelectedIndex > -1)
                RowHighlight(GridViewBudgetHdrs);
        }

        protected void LinkButtonNewBudget_Click(object sender, EventArgs e)
        {
            int budgetVersion = 0;
            if(DDLBudgetYearsAdd.SelectedValue == "")
            {
                ShowDialog("Error", MessageType.Error, "Please select a budget year to create a new budget.", "");
                return;
            }

            if(BudgetYearAllowsNew(DDLBudgetYearsAdd.SelectedValue) ==false)
            {
                ShowDialog("Error", MessageType.Error, "The selected budget year does not allow new budgets to be created.", "");
                return;
            }

            if (CreateNewBudget(DDLBudgetYearsAdd.SelectedValue, Session["CurrentDistrict"].ToString(), out budgetVersion) == false)
            {
                ShowDialog("Error", MessageType.Error, "The new budget could not be created. Process cancelled.", "");
                return;
            }
            else
            {

                GridViewBudgetHdrs.DataBind();

                bool transferfound = false;

                //Update the selected transfer - select first if it was deleted
                for (int i = 0; i < GridViewBudgetHdrs.DataKeys.Count; i++)
                {
                    if (GridViewBudgetHdrs.DataKeys[i].Values["BudgetVersion"].ToString() == budgetVersion.ToString())
                    {
                        GridViewBudgetHdrs.SelectRow(i);
                        transferfound = true;
                        break;
                    }
                }

                if (transferfound == false)
                {
                    if (GridViewBudgetHdrs.Rows.Count > 0)
                    {
                        GridViewBudgetHdrs.SelectRow(0);
                    }
                }

                //GridViewTransfers_SelectedIndexChanged(GridViewTransfers, EventArgs.Empty);

                ShowMessageIcon("Budget Version " + budgetVersion.ToString() + " Created", MessageType.Success);
            }

        }

        private bool CreateNewBudget(string BudgetYear, string DistrictID, out int budgetVersion)
        {
            bool transferGenerated = false;
            budgetVersion = 0;

            string sqlGenerateNewBudget = "DECLARE @NewBudgetVersion INT; SET @NewBudgetVersion=(SELECT ISNULL(MAX(BudgetVersion),0)+1 FROM [FMSBudgetHeader] WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID);" +
                "INSERT INTO [FMSBudgetHeader] ([BudgetYear],[DistrictID],[BudgetVersion],[Active],[BudgetDescription],[StartingDate],[EndingDate],[LockedBudget],[ContractID]) " +
                "SELECT [BudgetYear],@DistrictID,@NewBudgetVersion,0,'',[StartingDate],[EndingDate],0,'' FROM [FMSBudgetYear_Setup] WHERE BudgetYear=@BudgetYear;" +
                //" VALUES(@BudgetYear,@DistrictID,@NewBudgetVersion,0,'','1/1/1900','1/1/1900',0,'');" +
                "INSERT INTO [FMSBudgetExpenses] ([BudgetYear],[DistrictID],[BudgetVersion],[ManagementFee],[AdminFee]) VALUES(@BudgetYear,@DistrictID,@NewBudgetVersion,0,0);" +
                "INSERT INTO [FMSBudgetDirectCosts] ([BudgetYear],[DistrictID],[BudgetVersion],[BackgroundCheckNUTR],[CarTruckRentNUTR],[CarTruckRentDIST],[CarTruckMileageDIST],[CarTruckMaintNUTR],[CarTruckMaintDIST]" +
                ",[SoftwareUpgradeNUTR],[TechnologyFeeDIST],[CourierNUTR],[EmployeeMealsNUTR],[EmpRecruitmentNUTR],[EmpRecruitmentDIST],[EmpReplacementNUTR],[EmpReplacementDIST]" +
                ",[IndirectLaborNUTR],[IndirectLaborDIST],[LiabilityEquipNUTR],[LiabilityInsurNUTR],[VehicleInsuranceNUTR],[LicenseFeeNUTR],[OfficeExpenseNUTR],[OfficeExpenseDIST]" +
                ",[PrintingNUTR],[PrintingDIST],[PerformanceBondNUTR],[PostageNUTR],[PostageDIST],[StaffTrainingNUTR],[TrainingNUTR],[TrainingDIST],[CommNUTR],[LaundryNUTR]" +
                ",[LaundryDIST],[UniformsNUTR],[UniformsDIST],[FreightDelivNUTR]) VALUES (@BudgetYear,@DistrictID,@NewBudgetVersion,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0);" +
                "INSERT INTO [FMSBudgetLaborHourly]([BudgetYear],[DistrictID],[BudgetVersion],[FICAPCT],[FUTAPCT],[FUTADLR],[SUIPCT],[SUIDLR],[RelatedPayrollPCT],[WCompPct],[OtherLaborDlr],[HourlyLaborNotes]) " +
                "VALUES(@BudgetYear,@DistrictID,@NewBudgetVersion,0,0,0,0,0,0,0,0,'');" +
                "INSERT INTO [FMSBudgetLaborMgmt] ([BudgetYear],[DistrictID],[BudgetVersion],[FICAPCT],[FUTAPCT],[FUTADLR],[SUIPCT],[SUIDLR],[RelatedPayrollPCT],[WCompPCT],[OtherLaborDLR],[MgmtLaborNotes],[PctBudgetIncrease]) " +
                "VALUES(@BudgetYear,@DistrictID,@NewBudgetVersion,0,0,0,0,0,0,0,0,'',0);" +
                "SELECT @NewBudgetVersion;";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmdHdr = new SqlCommand(sqlGenerateNewBudget, dbConnection))
                {
                    SqlParameter dbParam_budgetYear = new SqlParameter();
                    dbParam_budgetYear.ParameterName = "@BudgetYear";
                    dbParam_budgetYear.Value = BudgetYear;
                    dbParam_budgetYear.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_budgetYear);

                    SqlParameter dbParam_distID = new SqlParameter();
                    dbParam_distID.ParameterName = "@DistrictID";
                    dbParam_distID.Value = DistrictID;
                    dbParam_distID.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_distID);

                    try
                    {
                        object objBudgetVers = cmdHdr.ExecuteScalar();

                        if (objBudgetVers != null)
                        {
                            if (int.TryParse(objBudgetVers.ToString(), out budgetVersion) == true && budgetVersion > 0)
                                transferGenerated = true;
                        }


                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("Error", MessageType.Error, "A new budget could not be created. Process cancelled. ", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "A new budget could not be created. Process cancelled. ", ex.Message);
                    }
                }
            }

            return transferGenerated;

        }

        private bool CopyBudget(string srcBudgetYear, string srcDistrictID, int srcBudgetVersion, string destBudgetYear, out int newBudgetVersion)
        {
            bool budgetCopied = false;
            newBudgetVersion = 0;

            if (BudgetYearAllowsNew(destBudgetYear) == false)
            {
                ShowDialog("Error", MessageType.Error, "The destination budget year does not allow new budgets to be created.", "");
                return false;
            }

            string sqlGenerateNewBudget = "DECLARE @NewBudgetVersion INT; SET @NewBudgetVersion=(SELECT ISNULL(MAX(BudgetVersion),0)+1 FROM [FMSBudgetHeader] WHERE BudgetYear=@BudgetYearDest AND DistrictID=@DistrictID);" +

                "INSERT INTO [FMSBudgetHeader] ([BudgetYear],[DistrictID],[BudgetVersion],[Active],[BudgetDescription],[StartingDate],[EndingDate],[LockedBudget],[ContractID]) " +
                " SELECT @BudgetYearDest,[DistrictID],@NewBudgetVersion,0 [Active],[BudgetDescription],[StartingDate],[EndingDate],0 [LockedBudget],[ContractID] FROM [FMSBudgetHeader] WHERE [BudgetYear]=@BudgetYearSrc AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersionSrc;" +

                "INSERT INTO [FMSBudgetExpenses] ([BudgetYear],[DistrictID],[BudgetVersion],[ManagementFee],[AdminFee]) " +
                " SELECT @BudgetYearDest,[DistrictID],@NewBudgetVersion,[ManagementFee],[AdminFee] FROM [FMSBudgetExpenses] WHERE [BudgetYear]=@BudgetYearSrc AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersionSrc;" +

                "INSERT INTO [FMSBudgetDirectCosts] ([BudgetYear],[DistrictID],[BudgetVersion],[BackgroundCheckNUTR],[CarTruckRentNUTR],[CarTruckRentDIST],[CarTruckMileageDIST],[CarTruckMaintNUTR],[CarTruckMaintDIST]" +
                ",[SoftwareUpgradeNUTR],[TechnologyFeeDIST],[CourierNUTR],[EmployeeMealsNUTR],[EmpRecruitmentNUTR],[EmpRecruitmentDIST],[EmpReplacementNUTR],[EmpReplacementDIST]" +
                ",[IndirectLaborNUTR],[IndirectLaborDIST],[LiabilityEquipNUTR],[LiabilityInsurNUTR],[VehicleInsuranceNUTR],[LicenseFeeNUTR],[OfficeExpenseNUTR],[OfficeExpenseDIST]" +
                ",[PrintingNUTR],[PrintingDIST],[PerformanceBondNUTR],[PostageNUTR],[PostageDIST],[StaffTrainingNUTR],[TrainingNUTR],[TrainingDIST],[CommNUTR],[LaundryNUTR]" +
                ",[LaundryDIST],[UniformsNUTR],[UniformsDIST],[FreightDelivNUTR]) " +
                " SELECT @BudgetYearDest,[DistrictID],@NewBudgetVersion,[BackgroundCheckNUTR],[CarTruckRentNUTR],[CarTruckRentDIST],[CarTruckMileageDIST],[CarTruckMaintNUTR],[CarTruckMaintDIST]"+
                ",[SoftwareUpgradeNUTR],[TechnologyFeeDIST],[CourierNUTR],[EmployeeMealsNUTR],[EmpRecruitmentNUTR],[EmpRecruitmentDIST],[EmpReplacementNUTR],[EmpReplacementDIST]"+
                ",[IndirectLaborNUTR],[IndirectLaborDIST],[LiabilityEquipNUTR],[LiabilityInsurNUTR],[VehicleInsuranceNUTR],[LicenseFeeNUTR],[OfficeExpenseNUTR],[OfficeExpenseDIST]"+
                ",[PrintingNUTR],[PrintingDIST],[PerformanceBondNUTR],[PostageNUTR],[PostageDIST],[StaffTrainingNUTR],[TrainingNUTR],[TrainingDIST],[CommNUTR],[LaundryNUTR]"+
                ",[LaundryDIST],[UniformsNUTR],[UniformsDIST],[FreightDelivNUTR] FROM [FMSBudgetDirectCosts] WHERE [BudgetYear]=@BudgetYearSrc AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersionSrc;" +

                "INSERT INTO [FMSBudgetLaborHourly]([BudgetYear],[DistrictID],[BudgetVersion],[FICAPCT],[FUTAPCT],[FUTADLR],[SUIPCT],[SUIDLR],[RelatedPayrollPCT],[WCompPct],[OtherLaborDlr],[HourlyLaborNotes]) " +
                "SELECT @BudgetYearDest,[DistrictID],@NewBudgetVersion,[FICAPCT],[FUTAPCT],[FUTADLR],[SUIPCT],[SUIDLR],[RelatedPayrollPCT],[WCompPct],[OtherLaborDlr],[HourlyLaborNotes] " +
                "FROM [FMSBudgetLaborHourly] WHERE [BudgetYear]=@BudgetYearSrc AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersionSrc;" +

                "INSERT INTO [FMSBudgetLaborHourlyEmp]([BudgetYear],[DistrictID],[BudgetVersion],[ORD],[SchoolNum],[EmpName],[Job],[EmpSchedule],[HourlyRate]" +
                ",[HoursDay],[DaysPaid],[PlanType],[Health],[Dental],[Disability],[Vision],[Annuity],[Retire],[Life],[OtherBenefit]) " +
                "SELECT @BudgetYearDest,[DistrictID],@NewBudgetVersion,[ORD],[SchoolNum],[EmpName],[Job],[EmpSchedule],[HourlyRate]" +
                ",[HoursDay],[DaysPaid],[PlanType],[Health],[Dental],[Disability],[Vision],[Annuity],[Retire],[Life],[OtherBenefit] FROM [FMSBudgetLaborHourlyEmp]" +
                "WHERE [BudgetYear]=@BudgetYearSrc AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersionSrc;" +

                "INSERT INTO [FMSBudgetLaborMgmt] ([BudgetYear],[DistrictID],[BudgetVersion],[FICAPCT],[FUTAPCT],[FUTADLR],[SUIPCT],[SUIDLR],[RelatedPayrollPCT],[WCompPCT],[OtherLaborDLR],[MgmtLaborNotes],[PctBudgetIncrease]) " +
                "SELECT @BudgetYearDest,[DistrictID],@NewBudgetVersion,[FICAPCT],[FUTAPCT],[FUTADLR],[SUIPCT],[SUIDLR],[RelatedPayrollPCT],[WCompPCT],[OtherLaborDLR],[MgmtLaborNotes],[PctBudgetIncrease]" +
                "FROM [FMSBudgetLaborMgmt] WHERE [BudgetYear]=@BudgetYearSrc AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersionSrc;" +

                "INSERT INTO [FMSBudgetLaborMgmtEmp]([BudgetYear],[DistrictID],[BudgetVersion],[ORD],[ManagerName],[Job],[CurrentSalary],[CurrentBonus],[CurrentHealthBuyout],[WorkWeeks],[PlanType],[Health]" +
                ",[Life],[Dental],[Vision],[OtherBenefit],[Ben401k],[ExnpTravel],[ExpnDues],[ExpnEducation],[ExpnSubscriptions]) " +
                "SELECT @BudgetYearDest,[DistrictID],@NewBudgetVersion,[ORD],[ManagerName],[Job],[CurrentSalary],[CurrentBonus],[CurrentHealthBuyout],[WorkWeeks],[PlanType],[Health]" +
                ",[Life],[Dental],[Vision],[OtherBenefit],[Ben401k],[ExnpTravel],[ExpnDues],[ExpnEducation],[ExpnSubscriptions] FROM [FMSBudgetLaborMgmtEmp] " +
                "WHERE [BudgetYear]=@BudgetYearSrc AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersionSrc;" +

                "SELECT @NewBudgetVersion;";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmdHdr = new SqlCommand(sqlGenerateNewBudget, dbConnection))
                {
                    SqlParameter dbParam_budgetYearSrc = new SqlParameter();
                    dbParam_budgetYearSrc.ParameterName = "@BudgetYearSrc";
                    dbParam_budgetYearSrc.Value = srcBudgetYear;
                    dbParam_budgetYearSrc.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_budgetYearSrc);

                    SqlParameter dbParam_distID = new SqlParameter();
                    dbParam_distID.ParameterName = "@DistrictID";
                    dbParam_distID.Value = srcDistrictID;
                    dbParam_distID.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_distID);

                    SqlParameter dbParam_budgetVersionSrc = new SqlParameter();
                    dbParam_budgetVersionSrc.ParameterName = "@BudgetVersionSrc";
                    dbParam_budgetVersionSrc.Value = srcBudgetVersion;
                    dbParam_budgetVersionSrc.DbType = System.Data.DbType.Int32;
                    cmdHdr.Parameters.Add(dbParam_budgetVersionSrc);

                    SqlParameter dbParam_budgetYearDest = new SqlParameter();
                    dbParam_budgetYearDest.ParameterName = "@BudgetYearDest";
                    dbParam_budgetYearDest.Value = destBudgetYear;
                    dbParam_budgetYearDest.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_budgetYearDest);

                    try
                    {
                        object objBudgetVers = cmdHdr.ExecuteScalar();

                        if (objBudgetVers != null)
                        {
                            if (int.TryParse(objBudgetVers.ToString(), out newBudgetVersion) == true && newBudgetVersion > 0)
                                budgetCopied = true;
                        }


                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("Error", MessageType.Error, "The budget could not be copied. Process cancelled. ", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "The budget could not be copied. Process cancelled. ", ex.Message);
                    }
                }
            }

            return budgetCopied;

        }

        protected void GridViewBudgetHdrs_DataBound(object sender, EventArgs e)
        {
            if(GridViewBudgetHdrs.Rows.Count==0)
            {
                FormViewBudget.Visible = false;
                FormViewBudget_DataBound(FormViewBudget, EventArgs.Empty);
            }
            else
            {
                FormViewBudget.Visible = true;
                if(GridViewBudgetHdrs.SelectedIndex==-1)
                {
                    GridViewBudgetHdrs.SelectRow(0);
                    GridViewBudgetHdrs.SelectedIndex = 0;
                    GridViewBudgetHdrs_SelectedIndexChanged(GridViewBudgetHdrs, EventArgs.Empty);
                }
                else
                    GridViewBudgetHdrs_SelectedIndexChanged(GridViewBudgetHdrs, EventArgs.Empty);
            }
        }

        protected void SqlDataSourceSelectedBudget_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e!=null && e.AffectedRows > 0)
            {
                GridViewBudgetHdrs.DataBind();
                ShowMessageIcon("Saved", MessageType.Success);
            }
        }

        protected void SqlDataSourceSelectedBudget_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if(e!=null && e.AffectedRows>0)
            {
                GridViewBudgetHdrs.DataBind();

                if (GridViewBudgetHdrs.Rows.Count > 0 && GridViewBudgetHdrs.SelectedIndex== GridViewBudgetHdrs.Rows.Count)
                {
                    GridViewBudgetHdrs.SelectRow(GridViewBudgetHdrs.Rows.Count-1);
                    GridViewBudgetHdrs.SelectedIndex = (GridViewBudgetHdrs.Rows.Count - 1);
                    GridViewBudgetHdrs_SelectedIndexChanged(GridViewBudgetHdrs, EventArgs.Empty);
                }
            }
        }



        protected void gvHourlyEmps_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView gvHourlyEmps = (GridView)sender;
            if (gvHourlyEmps == null)
                return;



            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    if (SqlDataSourceHourlyEmps.Insert() > 0)
                    {
                        //gvHourlyEmps.DataBind();
                        FormViewBudget.DataBind();
                        //Don't databind here - it causes the footer to disappear when retaining values. This databind is done in the SqlDataSourceHourlyEmps_Inserted event


                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate key") >= 0)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "Duplicate record.", "");
                    }
                    else
                    {
                        ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }
            }
            else if(e.CommandName == "Delete")
            {
                gvHourlyEmps.DataBind();
            }
        }

        protected void SqlDataSourceHourlyEmps_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                //ShowMessageIcon("Deleted", MessageType.Success);
                FormViewBudget.DataBind();
            }
            else if (e.Exception != null)
            {
                ShowDialog("Error", MessageType.Error, "An error occurred.", e.Exception.Message);
            }
        }

        protected void SqlDataSourceHourlyEmps_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null || FormViewBudget == null)
                return;

            GridView gvHourlyEmps = (GridView)FormViewBudget.FindControl("gvHourlyEmps");

            DropDownList DDLSchoolsF = (DropDownList)gvHourlyEmps.FooterRow.FindControl("DDLSchoolsF");
            TextBox TextBoxEmployeeF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxEmployeeF");
            TextBox TextBoxJobF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxJobF");
            TextBox TextBoxScheduleF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxScheduleF");
            TextBox TextBoxRateF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxRateF");
            TextBox TextBoxHoursDayF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxHoursDayF");
            TextBox TextBoxDaysPaidF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxDaysPaidF");
            DropDownList DDLPlanTypeF = (DropDownList)gvHourlyEmps.FooterRow.FindControl("DDLPlanTypeF");
            TextBox TextBoxHealthF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxHealthF");
            TextBox TextBoxDentalF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxDentalF");
            TextBox TextBoxDisabilityF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxDisabilityF");
            TextBox TextBoxVisionF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxVisionF");
            TextBox TextBoxAnnuityF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxAnnuityF");
            TextBox TextBoxLifeF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxLifeF");
            TextBox TextBoxRetireF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxRetireF");
            TextBox TextBoxOtherF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxOtherF");

            if (DDLSchoolsF == null || TextBoxEmployeeF == null || TextBoxJobF == null || TextBoxScheduleF == null || TextBoxRateF == null || TextBoxHoursDayF == null || TextBoxDaysPaidF == null
                || DDLPlanTypeF == null || TextBoxHealthF == null || TextBoxDentalF == null || TextBoxDisabilityF == null || TextBoxVisionF == null || TextBoxAnnuityF == null || TextBoxLifeF == null
                || TextBoxRetireF == null || TextBoxOtherF == null)
                return;

            //validate numerics
            decimal.TryParse(TextBoxRateF.Text, out decimal rate);
            e.Command.Parameters["@HourlyRate"].Value = rate;

            decimal.TryParse(TextBoxHoursDayF.Text, out decimal hoursDay);
            e.Command.Parameters["@HoursDay"].Value = hoursDay;

            int.TryParse(TextBoxDaysPaidF.Text, out int daysPaid);
            e.Command.Parameters["@DaysPaid"].Value = daysPaid;

            decimal.TryParse(TextBoxHealthF.Text, out decimal health);
            e.Command.Parameters["@Health"].Value = health;

            decimal.TryParse(TextBoxDentalF.Text, out decimal dental);
            e.Command.Parameters["@Dental"].Value = dental;

            decimal.TryParse(TextBoxDisabilityF.Text, out decimal disability);
            e.Command.Parameters["@Disability"].Value = disability;

            decimal.TryParse(TextBoxVisionF.Text, out decimal vision);
            e.Command.Parameters["@Vision"].Value = vision;

            decimal.TryParse(TextBoxAnnuityF.Text, out decimal annuity);
            e.Command.Parameters["@Annuity"].Value = annuity;

            decimal.TryParse(TextBoxLifeF.Text, out decimal life);
            e.Command.Parameters["@Life"].Value = life;

            decimal.TryParse(TextBoxRetireF.Text, out decimal retire);
            e.Command.Parameters["@Retire"].Value = retire;

            decimal.TryParse(TextBoxOtherF.Text, out decimal other);
            e.Command.Parameters["@OtherBenefit"].Value = other;

            e.Command.Parameters["@SchoolNum"].Value = DDLSchoolsF.SelectedValue;
            e.Command.Parameters["@EmpName"].Value = TextBoxEmployeeF.Text;
            e.Command.Parameters["@Job"].Value = TextBoxJobF.Text;
            e.Command.Parameters["@EmpSchedule"].Value = TextBoxScheduleF.Text;
            e.Command.Parameters["@PlanType"].Value = DDLPlanTypeF.SelectedValue;

            
        }

        protected void SqlDataSourceHourlyEmps_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e != null)
            {
                if (e.AffectedRows > 0 )
                {
                    GridView gvHourlyEmps = (GridView)FormViewBudget.FindControl("gvHourlyEmps");
                    if (gvHourlyEmps != null)
                    {
                       
                        ShowMessageIcon("Saved", MessageType.Success);

                        CheckBox CBRetainFooterEntries = (CheckBox)FormViewBudget.FindControl("CBRetainFooterEntries");
                        if (CBRetainFooterEntries != null && CBRetainFooterEntries.Checked)
                        {
                            HFHourlyEmpName.Value = e.Command.Parameters["@EmpName"].Value.ToString();
                            HFHourlySchoolNum.Value = e.Command.Parameters["@SchoolNum"].Value.ToString();
                            HFHourlyJobClass.Value = e.Command.Parameters["@Job"].Value.ToString();
                            HFHourlySchedule.Value = e.Command.Parameters["@EmpSchedule"].Value.ToString();
                            HFHourlyRate.Value = e.Command.Parameters["@HourlyRate"].Value.ToString();
                            HFHourlyHours.Value = e.Command.Parameters["@HoursDay"].Value.ToString();
                            HFHourlyDays.Value = e.Command.Parameters["@DaysPaid"].Value.ToString();
                            HFHourlyPlanType.Value = e.Command.Parameters["@PlanType"].Value.ToString();
                            HFHourlyHealth.Value = e.Command.Parameters["@Health"].Value.ToString();
                            HFHourlyDental.Value = e.Command.Parameters["@Dental"].Value.ToString();
                            HFHourlyDisability.Value = e.Command.Parameters["@Disability"].Value.ToString();
                            HFHourlyVision.Value = e.Command.Parameters["@Vision"].Value.ToString();
                            HFHourlyAnnuity.Value = e.Command.Parameters["@Annuity"].Value.ToString();
                            HFHourlyRetire.Value = e.Command.Parameters["@Retire"].Value.ToString();
                            HFHourlyLife.Value = e.Command.Parameters["@Life"].Value.ToString();
                            HFHourlyOther.Value = e.Command.Parameters["@OtherBenefit"].Value.ToString();
                        }
                        else
                        {
                            //Clear out the Hourly Employee Stored Values for new records
                            HFHourlyEmpName.Value = string.Empty;
                            HFHourlySchoolNum.Value = string.Empty;
                            HFHourlyJobClass.Value = string.Empty;
                            HFHourlySchedule.Value = string.Empty;
                            HFHourlyRate.Value = string.Empty;
                            HFHourlyHours.Value = string.Empty;
                            HFHourlyDays.Value = string.Empty;
                            HFHourlyPlanType.Value = string.Empty;
                            HFHourlyHealth.Value = string.Empty;
                            HFHourlyDental.Value = string.Empty;
                            HFHourlyDisability.Value = string.Empty;
                            HFHourlyVision.Value = string.Empty;
                            HFHourlyAnnuity.Value = string.Empty;
                            HFHourlyRetire.Value = string.Empty;
                            HFHourlyLife.Value = string.Empty;
                            HFHourlyOther.Value = string.Empty;
                        }

                        TextBox TextBoxEmployeeF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxEmployeeF");
                        if (TextBoxEmployeeF != null)
                        {
                            //TextBoxEmployeeF.Focus();
                            RegisterDOMReadyScript("FocusScrollY", "$('[id*=TextBoxEmployeeF]').focus();var y = $('#divHourlyEmps').scrollY;alert(y);");
                            //$('[id*=TextBoxEmployeeF]').focus({ preventScroll: true });var y = $('#divHourlyEmps').scrollY;$('#divHourlyEmps').scrollTo(0,y);
                        }
                    }
                }
                else if (e.Exception != null)
                {
                    ShowMessageIcon("Error", MessageType.Error);
                }
            }
        }

        protected void TextBoxHourlyHDR_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SqlDataSourceHourlyHdr.Update() > 0)
                {
                    FormViewBudget.DataBind();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.IndexOf("duplicate key") >= 0)
                {
                    ShowDialog("SQL Error", MessageType.Error, "Duplicate record.", "");
                }
                else
                {
                    ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                }
            }
            catch (Exception ex)
            {
                ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
            }
        }

        protected void SqlDataSourceHourlyHdr_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null || FormViewBudget == null)
                return;

            GridView gvHourlyEmps = (GridView)FormViewBudget.FindControl("gvHourlyEmps");

            TextBox TextBoxHdrFICAPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrFICAPct");
            TextBox TextBoxHdrFUTAPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrFUTAPct");
            TextBox TextBoxHdrFUTADlr = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrFUTADlr");
            TextBox TextBoxHdrSUIPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrSUIPct");
            TextBox TextBoxHdrSUIDlr = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrSUIDlr");
            TextBox TextBoxHdrRelatedPayrollPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrRelatedPayrollPct");
            TextBox TextBoxHdrWCompPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrWCompPct");
            TextBox TextBoxOtherLaborDlr = (TextBox)FormViewBudget.FindControl("TextBoxOtherLaborDlr");
            TextBox TextBoxHourlyLaborNotes = (TextBox)FormViewBudget.FindControl("TextBoxHourlyLaborNotes");


            if (TextBoxHdrFICAPct == null || TextBoxHdrFUTAPct == null || TextBoxHdrFUTADlr == null || TextBoxHdrSUIPct == null || TextBoxHdrSUIDlr == null || TextBoxHdrRelatedPayrollPct == null || TextBoxHdrWCompPct == null
                || TextBoxOtherLaborDlr == null || TextBoxHourlyLaborNotes == null)
                return;

            //validate numerics
            decimal.TryParse(TextBoxHdrFICAPct.Text, out decimal fica);
            e.Command.Parameters["@FICAPCT"].Value = fica * 0.01M;

            decimal.TryParse(TextBoxHdrFUTAPct.Text, out decimal futa);
            e.Command.Parameters["@FUTAPCT"].Value = futa * 0.01M;

            int.TryParse(TextBoxHdrFUTADlr.Text, out int futaDlr);
            e.Command.Parameters["@FUTADLR"].Value = futaDlr;

            decimal.TryParse(TextBoxHdrSUIPct.Text, out decimal sui);
            e.Command.Parameters["@SUIPCT"].Value = sui * 0.01M;

            decimal.TryParse(TextBoxHdrSUIDlr.Text, out decimal suiDlr);
            e.Command.Parameters["@SUIDLR"].Value = suiDlr;

            decimal.TryParse(TextBoxHdrRelatedPayrollPct.Text, out decimal payRel);
            e.Command.Parameters["@RelatedPayrollPCT"].Value = payRel * 0.01M;

            decimal.TryParse(TextBoxHdrWCompPct.Text, out decimal wcomp);
            e.Command.Parameters["@WCompPct"].Value = wcomp * 0.01M;

            decimal.TryParse(TextBoxOtherLaborDlr.Text, out decimal otherLabor);
            e.Command.Parameters["@OtherLaborDLR"].Value = otherLabor;

            e.Command.Parameters["@HourlyLaborNotes"].Value = TextBoxHourlyLaborNotes.Text;

        }


        protected void gvHourlyEmps_PreRender(object sender, EventArgs e)
        {

            GridView gvHourlyEmps = (GridView)sender;
            if (gvHourlyEmps == null)
                return;

            //Header Values
            TextBox TextBoxHdrFICAPct = new TextBox();
            TextBox TextBoxHdrFUTAPct = new TextBox();
            TextBox TextBoxHdrFUTADlr = new TextBox();
            TextBox TextBoxHdrSUIPct = new TextBox();
            TextBox TextBoxHdrSUIDlr = new TextBox();
            TextBox TextBoxHdrRelatedPayrollPct = new TextBox();
            TextBox TextBoxHdrWCompPct = new TextBox();
            if (gvHourlyEmps.HeaderRow != null)
            {
                TextBoxHdrFICAPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrFICAPct");
                TextBoxHdrFUTAPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrFUTAPct");
                TextBoxHdrFUTADlr = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrFUTADlr");
                TextBoxHdrSUIPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrSUIPct");
                TextBoxHdrSUIDlr = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrSUIDlr");
                TextBoxHdrRelatedPayrollPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrRelatedPayrollPct");
                TextBoxHdrWCompPct = (TextBox)gvHourlyEmps.HeaderRow.FindControl("TextBoxHdrWCompPct");

                if (TextBoxHdrFICAPct == null || TextBoxHdrFUTAPct == null || TextBoxHdrFUTADlr == null || TextBoxHdrSUIPct == null || TextBoxHdrSUIDlr == null || TextBoxHdrRelatedPayrollPct == null || TextBoxHdrWCompPct == null)
                {
                    return;
                }
            }

            //Footer Totals
            Label LabelHoursDayTotal = new Label();
            Label LabelWagesTotal = new Label();
            Label LabelFICATotal = new Label();
            Label LabelFUTATotal = new Label();
            Label LabelSUITotal = new Label();
            Label LabelPayRelTotal = new Label();
            Label LabelWCompTotal = new Label();
            Label LabelFringesTotal = new Label();
                
            Label LabelHealthTotal = new Label();
            Label LabelDentalTotal = new Label();
            Label LabelDisabilityTotal = new Label();
            Label LabelVisionTotal = new Label();
            Label LabelAnnuityTotal = new Label();
            Label LabelLifeTotal = new Label();
            Label LabelRetireTotal = new Label();
            Label LabelOtherTotal = new Label();
            Label LabelTtlBensTotal = new Label();
            Label LabelGrandTtlTotal = new Label();

            if (gvHourlyEmps.FooterRow != null)
            {
                
                LabelHoursDayTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelHoursDayTotal");
                LabelWagesTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelWagesTotal");
                LabelFICATotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelFICATotal");
                LabelFUTATotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelFUTATotal");
                LabelSUITotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelSUITotal");
                LabelPayRelTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelPayRelTotal");
                LabelWCompTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelWCompTotal");
                LabelFringesTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelFringesTotal");

                LabelHealthTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelHealthTotal");
                LabelDentalTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelDentalTotal");
                LabelDisabilityTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelDisabilityTotal");
                LabelVisionTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelVisionTotal");
                LabelAnnuityTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelAnnuityTotal");
                LabelLifeTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelLifeTotal");
                LabelRetireTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelRetireTotal");
                LabelOtherTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelOtherTotal");
                LabelTtlBensTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelTtlBensTotal");
                LabelGrandTtlTotal = (Label)gvHourlyEmps.FooterRow.FindControl("LabelGrandTtlTotal");

                if (LabelHoursDayTotal == null || LabelWagesTotal == null || LabelFICATotal == null || LabelFUTATotal == null || LabelSUITotal == null
                    || LabelPayRelTotal == null || LabelWCompTotal == null || LabelFringesTotal == null || LabelHealthTotal == null || LabelDentalTotal == null
                    || LabelDisabilityTotal == null || LabelVisionTotal == null || LabelAnnuityTotal == null || LabelLifeTotal == null || LabelRetireTotal == null
                    || LabelOtherTotal == null || LabelTtlBensTotal == null || LabelGrandTtlTotal == null)
                {
                    return;
                }
            }

            //Bottom Values Outside of the gridview
            TextBox TextBoxOtherLaborDlr = (TextBox)FormViewBudget.FindControl("TextBoxOtherLaborDlr");
            TextBox TextBoxHourlyLaborNotes = (TextBox)FormViewBudget.FindControl("TextBoxHourlyLaborNotes");
            if(TextBoxOtherLaborDlr==null || TextBoxHourlyLaborNotes==null)
            {
                return;
            }

            SqlDataReader rdr;
            SqlDataSourceHourlyHdr.DataBind();
            rdr = (SqlDataReader)SqlDataSourceHourlyHdr.Select(DataSourceSelectArguments.Empty);

            if (rdr != null && rdr.Read())
            {
                //Header Values
                TextBoxHdrFICAPct.Text = ((decimal)rdr["FICAPCT"]* 100M).ToString("F4");
                TextBoxHdrFUTAPct.Text = ((decimal)rdr["FUTAPCT"] * 100M).ToString("F4");
                TextBoxHdrFUTADlr.Text = ((decimal)rdr["FUTADLR"]).ToString("F0");
                TextBoxHdrSUIPct.Text = ((decimal)rdr["SUIPCT"] * 100M).ToString("F4");
                TextBoxHdrSUIDlr.Text = ((decimal)rdr["SUIDLR"]).ToString("F0");
                TextBoxHdrRelatedPayrollPct.Text = ((decimal)rdr["RelatedPayrollPCT"] * 100M).ToString("F4");
                TextBoxHdrWCompPct.Text = ((decimal)rdr["WCompPCT"] * 100M).ToString("F4");

                LabelHoursDayTotal.Text = ((decimal)rdr["TotalHoursDay"]).ToString("F2");
                LabelWagesTotal.Text = ((decimal)rdr["TotalWages"]).ToString("C2");
                LabelFICATotal.Text = ((decimal)rdr["TotalFICA"]).ToString("C2");
                LabelFUTATotal.Text = ((decimal)rdr["TotalFUTA"]).ToString("C2");
                LabelSUITotal.Text = ((decimal)rdr["TotalSUI"]).ToString("C2");
                LabelPayRelTotal.Text = ((decimal)rdr["TotalPayRel"]).ToString("C2");
                LabelWCompTotal.Text = ((decimal)rdr["TotalWComp"]).ToString("C2");
                LabelFringesTotal.Text = ((decimal)rdr["GrandTotalFringes"]).ToString("C2");

                LabelHealthTotal.Text = ((decimal)rdr["TotalHealth"]).ToString("C2");
                LabelDentalTotal.Text = ((decimal)rdr["TotalDental"]).ToString("C2");
                LabelDisabilityTotal.Text = ((decimal)rdr["TotalDisability"]).ToString("C2");
                LabelVisionTotal.Text = ((decimal)rdr["TotalVision"]).ToString("C2");
                LabelAnnuityTotal.Text = ((decimal)rdr["TotalAnnuity"]).ToString("C2");
                LabelLifeTotal.Text = ((decimal)rdr["TotalLife"]).ToString("C2");
                LabelRetireTotal.Text = ((decimal)rdr["TotalRetire"]).ToString("C2");
                LabelOtherTotal.Text = ((decimal)rdr["TotalOtherBenefit"]).ToString("C2");
                LabelTtlBensTotal.Text = ((decimal)rdr["GrandTotalBenefits"]).ToString("C2");
                LabelGrandTtlTotal.Text = (((decimal)rdr["TotalWages"])+ ((decimal)rdr["GrandTotalFringes"])+((decimal)rdr["GrandTotalBenefits"])).ToString("C2");


                //Bottom Values
                TextBoxOtherLaborDlr.Text = ((decimal)rdr["OtherLaborDLR"]).ToString();
                TextBoxHourlyLaborNotes.Text = rdr["HourlyLaborNotes"].ToString();

                rdr.Close();
            }
            else
            {
                TextBoxHdrFICAPct.Text = "0.0000";
                TextBoxHdrFUTAPct.Text = "0.0000";
                TextBoxHdrFUTADlr.Text= "0";
                TextBoxHdrSUIPct.Text = "0.0000";
                TextBoxHdrSUIDlr.Text = "0";
                TextBoxHdrRelatedPayrollPct.Text = "0.0000";
                TextBoxHdrWCompPct.Text = "0.0000";

                LabelHoursDayTotal.Text = "0.00";

                TextBoxOtherLaborDlr.Text = "0.00";
                TextBoxHourlyLaborNotes.Text = string.Empty;
            }
        }

        protected void DDLSchools_DataBound(object sender, EventArgs e)
        {
            DropDownList DDLSchools = (DropDownList)sender;
            GridView gvgvHourlyEmps = (GridView)FormViewBudget.FindControl("gvHourlyEmps");
            if(DDLSchools==null || gvgvHourlyEmps==null)
            {
                return;
            }

            //if the vendor record is not available anymore, or was inactivated - try to  find it an add it to the dropdown
            HiddenField HFSchoolNum = (HiddenField)FindControlRecursive(gvgvHourlyEmps, "HFSchoolNum");

            if (HFSchoolNum == null)
                return;

            HFSchoolNum.DataBind();

            if (HFSchoolNum.Value != string.Empty)
            {
                if (DDLSchools.Items.FindByValue(HFSchoolNum.Value) == null)
                {
                    //add the item to the list
                    DDLSchools.Items.Add(new ListItem("*Unknown Building ID:" + HFSchoolNum.Value, HFSchoolNum.Value));
                }
            }

            DDLSchools.SelectedValue = HFSchoolNum.Value;

        }

        protected void SqlDataSourceHourlyEmps_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null || FormViewBudget == null)
                return;

            GridView gvHourlyEmps = (GridView)FormViewBudget.FindControl("gvHourlyEmps");
            
            GridViewRow gvr = (GridViewRow)gvHourlyEmps.Rows[gvHourlyEmps.EditIndex];

            //Key Values
            e.Command.Parameters["@BudgetYear"].Value = gvHourlyEmps.DataKeys[gvHourlyEmps.EditIndex].Values["BudgetYear"];
            e.Command.Parameters["@DistrictID"].Value = gvHourlyEmps.DataKeys[gvHourlyEmps.EditIndex].Values["DistrictID"];
            e.Command.Parameters["@BudgetVersion"].Value = gvHourlyEmps.DataKeys[gvHourlyEmps.EditIndex].Values["BudgetVersion"];
            e.Command.Parameters["@ORD"].Value = gvHourlyEmps.DataKeys[gvHourlyEmps.EditIndex].Values["ORD"];
            
            DropDownList DDLSchools = (DropDownList)gvr.FindControl("DDLSchools");
            e.Command.Parameters["@SchoolNum"].Value = DDLSchools.SelectedValue;

            TextBox TextBoxEmployee = (TextBox)gvr.FindControl("TextBoxEmployee");
            e.Command.Parameters["@EmpName"].Value = TextBoxEmployee.Text;

            TextBox TextBoxJob = (TextBox)gvr.FindControl("TextBoxJob");
            e.Command.Parameters["@Job"].Value = TextBoxJob.Text;

            TextBox TextBoxSchedule = (TextBox)gvr.FindControl("TextBoxSchedule");
            e.Command.Parameters["@EmpSchedule"].Value = TextBoxSchedule.Text;

            TextBox TextBoxRate = (TextBox)gvr.FindControl("TextBoxRate");
            decimal.TryParse(TextBoxRate.Text, out decimal rate);
            e.Command.Parameters["@HourlyRate"].Value = rate;

            TextBox TextBoxHoursDay = (TextBox)gvr.FindControl("TextBoxHoursDay");
            decimal.TryParse(TextBoxHoursDay.Text, out decimal hoursDay);
            e.Command.Parameters["@HoursDay"].Value = hoursDay;

            TextBox TextBoxDaysPaid = (TextBox)gvr.FindControl("TextBoxDaysPaid");
            int.TryParse(TextBoxDaysPaid.Text, out int daysPaid);
            e.Command.Parameters["@DaysPaid"].Value = daysPaid;

            DropDownList DDLPlanType = (DropDownList)gvr.FindControl("DDLPlanType");
            e.Command.Parameters["@PlanType"].Value = DDLPlanType.SelectedValue;

            TextBox TextBoxHealth = (TextBox)gvr.FindControl("TextBoxHealth");
            decimal.TryParse(TextBoxHealth.Text, out decimal health);
            e.Command.Parameters["@Health"].Value = health;

            TextBox TextBoxDental = (TextBox)gvr.FindControl("TextBoxDental");
            decimal.TryParse(TextBoxDental.Text, out decimal dental);
            e.Command.Parameters["@Dental"].Value = dental;

            TextBox TextBoxDisability = (TextBox)gvr.FindControl("TextBoxDisability");
            decimal.TryParse(TextBoxDisability.Text, out decimal disability);
            e.Command.Parameters["@Disability"].Value = disability;

            TextBox TextBoxVision = (TextBox)gvr.FindControl("TextBoxVision");
            decimal.TryParse(TextBoxVision.Text, out decimal vision);
            e.Command.Parameters["@Vision"].Value = vision;

            TextBox TextBoxAnnuity = (TextBox)gvr.FindControl("TextBoxAnnuity");
            decimal.TryParse(TextBoxAnnuity.Text, out decimal annuity);
            e.Command.Parameters["@Annuity"].Value = annuity;

            TextBox TextBoxLife = (TextBox)gvr.FindControl("TextBoxLife");
            decimal.TryParse(TextBoxLife.Text, out decimal life);
            e.Command.Parameters["@Life"].Value = life;

            TextBox TextBoxRetire = (TextBox)gvr.FindControl("TextBoxRetire");
            decimal.TryParse(TextBoxRetire.Text, out decimal retire);
            e.Command.Parameters["@Retire"].Value = retire;

            TextBox TextBoxOther = (TextBox)gvr.FindControl("TextBoxOther");
            decimal.TryParse(TextBoxOther.Text, out decimal other);
            e.Command.Parameters["@OtherBenefit"].Value = other;



        }

        protected void gvHourlyEmps_ControlChanged(object sender, EventArgs e)
        {
            Control gvHourlyEmpsControl = (Control)sender;
            if (gvHourlyEmpsControl == null)
                return;
            
            GridViewRow gvr = (GridViewRow)FindParentControlOfType(gvHourlyEmpsControl, "GridViewRow");

            if (gvr == null)
                return;

            GridView gvHourlyEmps = (GridView)FormViewBudget.FindControl("gvHourlyEmps");
            if (gvHourlyEmps == null)
                return;

            gvHourlyEmps.EditIndex = gvr.RowIndex;

            if(gvHourlyEmps.EditIndex>=0)
            {
                try
                {
                    if(SqlDataSourceHourlyEmps.Update()>0)
                    {
                        FormViewBudget.DataBind();
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }
            }
        }

        protected void ButtonCreateCopy_Click(object sender, EventArgs e)
        {
            if(DDLDestBudgetYear.SelectedValue=="")
            {
                ShowDialog("Error", MessageType.Error, "Please select a destination budget year to copy to.", "");
                return;
            }
            if(BudgetYearAllowsNew(DDLDestBudgetYear.SelectedValue) ==false)
            {
                ShowDialog("Error", MessageType.Error, "The destination budget year does not allow new budgets.", "");
                return;
            }

            int newBudgetVersion = 0;
            if(int.TryParse(HFBudgetVersion.Value, out int srcBudgetVersion)==false)
            {
                return;
            }
            if(CopyBudget(HFBudgetYear.Value,HFDistrictID.Value, srcBudgetVersion, DDLDestBudgetYear.SelectedValue, out newBudgetVersion))
            {
                DDLBudgetYears.SelectedValue=DDLDestBudgetYear.SelectedValue;
                HFBudgetYear.Value = DDLDestBudgetYear.SelectedValue;
                HFBudgetVersion.Value = newBudgetVersion.ToString();
                GridViewBudgetHdrs.DataBind();

                for (int i = 0; i < GridViewBudgetHdrs.DataKeys.Count; i++)
                {
                    if (GridViewBudgetHdrs.DataKeys[i].Values["BudgetVersion"].ToString() == newBudgetVersion.ToString() && GridViewBudgetHdrs.DataKeys[i].Values["BudgetYear"].ToString() == HFBudgetYear.Value && GridViewBudgetHdrs.DataKeys[i].Values["DistrictID"].ToString() == HFDistrictID.Value)
                    {
                        GridViewBudgetHdrs.SelectedIndex = i;
                        GridViewBudgetHdrs_SelectedIndexChanged(GridViewBudgetHdrs, EventArgs.Empty);
                        break;
                    }
                }
            }
        }

        protected void IClone_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#<%= LabelBudgetYearSrc.ClientID %>').val('" + HFBudgetYear.Value + "'); $('#myModal').modal('show');", true);
        }

        protected void SqlDataSourceSelectedBudget_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            //need to removed the dollar signs before this will update.
            if (e == null)
                return;
            decimal.TryParse(e.Command.Parameters["@ManagementFee"].Value.ToString().Replace("$", String.Empty), out decimal MFee);
            e.Command.Parameters["@ManagementFee"].Value = MFee;
            decimal.TryParse(e.Command.Parameters["@AdminFee"].Value.ToString().Replace("$", String.Empty), out decimal AFee);
            e.Command.Parameters["@AdminFee"].Value = AFee;

        }

        protected void SqlDataSourceMgmtEmps_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null || FormViewBudget == null)
                return;

            GridView gvMgmtEmps = (GridView)FormViewBudget.FindControl("gvMgmtEmps");
            if(gvMgmtEmps == null)
                return;

            TextBox TextBoxManagerF = (TextBox)gvMgmtEmps.FooterRow.FindControl("TextBoxManagerF");
            TextBox TextBoxJobF = (TextBox)gvMgmtEmps.FooterRow.FindControl("TextBoxJobF");
            TextBox TextBoxCurrSalaryF = (TextBox)gvMgmtEmps.FooterRow.FindControl("TextBoxCurrSalaryF");
            TextBox TextBoxWorkWeeksF = (TextBox)gvMgmtEmps.FooterRow.FindControl("TextBoxWorkWeeksF");
            TextBox TextBoxCurrBonusF = (TextBox)gvMgmtEmps.FooterRow.FindControl("TextBoxCurrBonusF");
            TextBox TextBoxCurrHealthBuyoutF = (TextBox)gvMgmtEmps.FooterRow.FindControl("TextBoxCurrHealthBuyoutF");

            if (TextBoxManagerF == null || TextBoxJobF == null || TextBoxCurrSalaryF == null || TextBoxWorkWeeksF == null || TextBoxCurrBonusF == null || TextBoxCurrHealthBuyoutF == null )
                return;

            //validate numerics
            decimal.TryParse(TextBoxCurrSalaryF.Text, out decimal currSalary);
            e.Command.Parameters["@CurrentSalary"].Value = currSalary;

            int.TryParse(TextBoxWorkWeeksF.Text, out int workWeeks);
            if(workWeeks<1 || workWeeks>52)
                workWeeks = 52;
            e.Command.Parameters["@WorkWeeks"].Value = workWeeks;

            decimal.TryParse(TextBoxCurrBonusF.Text, out decimal currBonus);
            e.Command.Parameters["@CurrentBonus"].Value = currBonus;

            decimal.TryParse(TextBoxCurrHealthBuyoutF.Text, out decimal currHlthBuyout);
            e.Command.Parameters["@CurrentHealthBuyout"].Value = currHlthBuyout;

            e.Command.Parameters["@ManagerName"].Value = TextBoxManagerF.Text;
            e.Command.Parameters["@Job"].Value = TextBoxJobF.Text;

        }

        protected void SqlDataSourceMgmtEmps_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {

        }

        protected void SqlDataSourceMgmtEmps_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null || FormViewBudget == null)
                return;

            GridView gvMgmtEmps = (GridView)FormViewBudget.FindControl("gvMgmtEmps");
            GridView gvMgmtEmpsFringe = (GridView)FormViewBudget.FindControl("gvMgmtEmpsFringe");
            GridView gvMgmtEmpsBenefits = (GridView)FormViewBudget.FindControl("gvMgmtEmpsBenefits");
            GridView gvMgmtEmpsTotals = (GridView)FormViewBudget.FindControl("gvMgmtEmpsTotals");
            GridView gvMgmtExpenses = (GridView)FormViewBudget.FindControl("gvMgmtExpenses");

            if (gvMgmtEmps == null || gvMgmtEmpsFringe == null || gvMgmtEmpsBenefits == null || gvMgmtEmpsTotals == null || gvMgmtExpenses == null)
                return;

            GridViewRow gvr = gvMgmtEmps.Rows[gvMgmtEmps.EditIndex];
            GridViewRow gvrFringe = gvMgmtEmpsFringe.Rows[gvMgmtEmpsFringe.EditIndex];
            GridViewRow gvrBenefits = gvMgmtEmpsBenefits.Rows[gvMgmtEmpsBenefits.EditIndex];
            GridViewRow gvrMgmtEmpsTotals = gvMgmtEmpsTotals.Rows[gvMgmtEmpsTotals.EditIndex];
            GridViewRow gvrMgmtExpenses = gvMgmtExpenses.Rows[gvMgmtExpenses.EditIndex];
            if (gvr == null || gvrFringe == null || gvrBenefits == null || gvrMgmtEmpsTotals == null || gvrMgmtExpenses == null)
                return;

            //Key Values
            e.Command.Parameters["@BudgetYear"].Value = gvMgmtEmps.DataKeys[gvMgmtEmps.EditIndex].Values["BudgetYear"];
            e.Command.Parameters["@DistrictID"].Value = gvMgmtEmps.DataKeys[gvMgmtEmps.EditIndex].Values["DistrictID"];
            e.Command.Parameters["@BudgetVersion"].Value = gvMgmtEmps.DataKeys[gvMgmtEmps.EditIndex].Values["BudgetVersion"];
            e.Command.Parameters["@ORD"].Value = gvMgmtEmps.DataKeys[gvMgmtEmps.EditIndex].Values["ORD"];

            //Manager setup and wages gridview
            TextBox TextBoxManager = (TextBox)gvr.FindControl("TextBoxManager");
            e.Command.Parameters["@ManagerName"].Value = TextBoxManager.Text;
            TextBox TextBoxJob = (TextBox)gvr.FindControl("TextBoxJob");
            e.Command.Parameters["@Job"].Value = TextBoxJob.Text;
            TextBox TextBoxCurrSalary = (TextBox)gvr.FindControl("TextBoxCurrSalary");
            decimal.TryParse(TextBoxCurrSalary.Text, out decimal currSalary);
            e.Command.Parameters["@CurrentSalary"].Value = currSalary;
            TextBox TextBoxWorkWeeks = (TextBox)gvr.FindControl("TextBoxWorkWeeks");
            int.TryParse(TextBoxWorkWeeks.Text, out int workWeeks);
            if (workWeeks < 1 || workWeeks > 52)
                workWeeks = 52;
            e.Command.Parameters["@WorkWeeks"].Value = workWeeks;
            TextBox TextBoxCurrBonus = (TextBox)gvr.FindControl("TextBoxCurrBonus");
            decimal.TryParse(TextBoxCurrBonus.Text, out decimal currBonus);
            e.Command.Parameters["@CurrentBonus"].Value = currBonus;
            TextBox TextBoxCurrHealthBuyout = (TextBox)gvr.FindControl("TextBoxCurrHealthBuyout");
            decimal.TryParse(TextBoxCurrHealthBuyout.Text, out decimal currHlthBuyout);
            e.Command.Parameters["@CurrentHealthBuyout"].Value = currHlthBuyout;
            TextBox TextBoxCellPhone = (TextBox)gvr.FindControl("TextBoxCellPhone");
            decimal.TryParse(TextBoxCellPhone.Text, out decimal cellBenefit);
            e.Command.Parameters["@CellBenefit"].Value = cellBenefit;
            TextBox TextBoxFitness = (TextBox)gvr.FindControl("TextBoxFitness");
            decimal.TryParse(TextBoxFitness.Text, out decimal fitnessBenefit);
            e.Command.Parameters["@FitnessCredit"].Value = fitnessBenefit;

            //Manager Benefits Gridview
            DropDownList DDLPlanType = (DropDownList)gvrBenefits.FindControl("DDLPlanType");
            e.Command.Parameters["@PlanType"].Value = DDLPlanType.SelectedValue;
            TextBox TextBoxHealth = (TextBox)gvrBenefits.FindControl("TextBoxHealth");
            decimal.TryParse(TextBoxHealth.Text, out decimal health);
            e.Command.Parameters["@Health"].Value = health;
            TextBox TextBoxLife = (TextBox)gvrBenefits.FindControl("TextBoxLife");
            decimal.TryParse(TextBoxLife.Text, out decimal life);
            e.Command.Parameters["@Life"].Value = life;
            TextBox TextBoxDental = (TextBox)gvrBenefits.FindControl("TextBoxDental");
            decimal.TryParse(TextBoxDental.Text, out decimal dental);
            e.Command.Parameters["@Dental"].Value = dental;
            TextBox TextBoxVision = (TextBox)gvrBenefits.FindControl("TextBoxVision");
            decimal.TryParse(TextBoxVision.Text, out decimal vision);
            e.Command.Parameters["@Vision"].Value = vision;
            TextBox TextBoxOther = (TextBox)gvrBenefits.FindControl("TextBoxOther");
            decimal.TryParse(TextBoxOther.Text, out decimal other);
            e.Command.Parameters["@OtherBenefit"].Value = other;

            //Manager Totals Gridview
            TextBox TextBox401K = (TextBox)gvrMgmtEmpsTotals.FindControl("TextBox401K");
            decimal.TryParse(TextBox401K.Text, out decimal ben401k);
            e.Command.Parameters["@Ben401k"].Value = ben401k;

            //Manager Expenses Gridview
            TextBox TextBoxTravel = (TextBox)gvrMgmtExpenses.FindControl("TextBoxTravel");
            decimal.TryParse(TextBoxTravel.Text, out decimal travel);
            e.Command.Parameters["@ExpnTravel"].Value = travel;
            TextBox TextBoxDues = (TextBox)gvrMgmtExpenses.FindControl("TextBoxDues");
            decimal.TryParse(TextBoxDues.Text, out decimal dues);
            e.Command.Parameters["@ExpnDues"].Value = dues;
            TextBox TextBoxEducation = (TextBox)gvrMgmtExpenses.FindControl("TextBoxEducation");
            decimal.TryParse(TextBoxEducation.Text, out decimal education);
            e.Command.Parameters["@ExpnEducation"].Value = education;
            TextBox TextBoxSubs = (TextBox)gvrMgmtExpenses.FindControl("TextBoxSubs");
            decimal.TryParse(TextBoxSubs.Text, out decimal subs);
            e.Command.Parameters["@ExpnSubscriptions"].Value = subs;

        }

        protected void gvMgmtEmps_ControlChanged(object sender, EventArgs e)
        {
            Control gvMgmtEmpsControl = (Control)sender;
            if (gvMgmtEmpsControl == null)
                return;

            GridViewRow gvr = (GridViewRow)FindParentControlOfType(gvMgmtEmpsControl, "GridViewRow");

            if (gvr == null)
                return;


            GridView gvMgmtEmps = (GridView)FormViewBudget.FindControl("gvMgmtEmps");
            GridView gvMgmtEmpsFringe = (GridView)FormViewBudget.FindControl("gvMgmtEmpsFringe");
            GridView gvMgmtEmpsBenefits = (GridView)FormViewBudget.FindControl("gvMgmtEmpsBenefits");
            GridView gvMgmtEmpsTotals = (GridView)FormViewBudget.FindControl("gvMgmtEmpsTotals");
            GridView gvMgmtExpenses = (GridView)FormViewBudget.FindControl("gvMgmtExpenses");
            if (gvMgmtEmps == null || gvMgmtEmpsFringe == null || gvMgmtEmpsBenefits == null || gvMgmtEmpsTotals == null || gvMgmtExpenses == null)
                return;

            //Edit Index tells the update which row to update.
            gvMgmtEmps.EditIndex = gvr.RowIndex;
            gvMgmtEmpsFringe.EditIndex = gvr.RowIndex;
            gvMgmtEmpsBenefits.EditIndex = gvr.RowIndex;
            gvMgmtEmpsTotals.EditIndex = gvr.RowIndex;
            gvMgmtExpenses.EditIndex = gvr.RowIndex;

            if (gvr.RowIndex >= 0)
            {
                try
                {
                    if (SqlDataSourceMgmtEmps.Update() > 0)
                    {
                        FormViewBudget.DataBind();
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }
            }
        }

        protected void gvHourlyEmps_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e == null)
                return;

            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ORD"].ToString() == "0")
                e.Row.Visible = false;

        }

        protected void gvMgmtEmps_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e == null)
                return;

            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ORD"].ToString() == "0")
                e.Row.Visible = false;
        }

        protected void gvMgmtEmps_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView gvMgmtEmps = (GridView)sender;
            if (gvMgmtEmps == null)
                return;

            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    if (SqlDataSourceMgmtEmps.Insert() > 0)
                    {
                        FormViewBudget.DataBind();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate key") >= 0)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "Duplicate record.", "");
                    }
                    else
                    {
                        ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }
            }
            else if (e.CommandName == "Delete")
            {
                gvMgmtEmps.DataBind();
            }
        }

        protected void SqlDataSourceMgmtHdr_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null || FormViewBudget == null)
                return;

            GridView gvMgmtEmps = (GridView)FormViewBudget.FindControl("gvMgmtEmps");
            GridView gvMgmtEmpsFringe = (GridView)FormViewBudget.FindControl("gvMgmtEmpsFringe");
            if (gvMgmtEmps==null || gvMgmtEmpsFringe==null)
            {
                return;
            }

            TextBox TextBoxMgmtIncrease = (TextBox)FormViewBudget.FindControl("TextBoxMgmtIncrease");
            TextBox TextBoxOtherMgrDlr = (TextBox)FormViewBudget.FindControl("TextBoxOtherMgrDlr");
            TextBox TextBoxMgmtNotes = (TextBox)FormViewBudget.FindControl("TextBoxMgmtNotes");
            if (TextBoxMgmtIncrease==null || TextBoxOtherMgrDlr == null || TextBoxMgmtNotes == null)
            {
                return;
            }

            decimal.TryParse(TextBoxMgmtIncrease.Text, out decimal mgmtIncrease);
            e.Command.Parameters["@PctBudgetIncrease"].Value = mgmtIncrease * 0.01M;

            decimal.TryParse(TextBoxOtherMgrDlr.Text, out decimal otherMgrDlr);
            e.Command.Parameters["@OtherLaborDLR"].Value = otherMgrDlr;

            e.Command.Parameters["@MgmtLaborNotes"].Value = TextBoxMgmtNotes.Text;

            TextBox TextBoxHdrFICAPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrFICAPct");
            TextBox TextBoxHdrFUTAPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrFUTAPct");
            TextBox TextBoxHdrFUTADlr = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrFUTADlr");
            TextBox TextBoxHdrSUIPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrSUIPct");
            TextBox TextBoxHdrSUIDlr = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrSUIDlr");
            TextBox TextBoxHdrPayrollRelatedPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrPayrollRelatedPct");
            TextBox TextBoxHdrWCompPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrWCompPct");


            //TextBox TextBoxOtherLaborDlr = (TextBox)FormViewBudget.FindControl("TextBoxOtherLaborDlr");
            //TextBox TextBoxHourlyLaborNotes = (TextBox)FormViewBudget.FindControl("TextBoxHourlyLaborNotes");


            if (TextBoxHdrFICAPct == null || TextBoxHdrFUTAPct == null || TextBoxHdrFUTADlr == null || TextBoxHdrSUIPct == null || TextBoxHdrSUIDlr == null || TextBoxHdrPayrollRelatedPct == null || TextBoxHdrWCompPct == null)
                return;

            //if( TextBoxOtherLaborDlr == null || TextBoxHourlyLaborNotes == null)
            //    return;

            //validate numerics
            decimal.TryParse(TextBoxHdrFICAPct.Text, out decimal fica);
            e.Command.Parameters["@FICAPCT"].Value = fica * 0.01M;

            decimal.TryParse(TextBoxHdrFUTAPct.Text, out decimal futa);
            e.Command.Parameters["@FUTAPCT"].Value = futa * 0.01M;

            int.TryParse(TextBoxHdrFUTADlr.Text, out int futaDlr);
            e.Command.Parameters["@FUTADLR"].Value = futaDlr;

            decimal.TryParse(TextBoxHdrSUIPct.Text, out decimal sui);
            e.Command.Parameters["@SUIPCT"].Value = sui * 0.01M;

            decimal.TryParse(TextBoxHdrSUIDlr.Text, out decimal suiDlr);
            e.Command.Parameters["@SUIDLR"].Value = suiDlr;

            decimal.TryParse(TextBoxHdrPayrollRelatedPct.Text, out decimal payRel);
            e.Command.Parameters["@RelatedPayrollPCT"].Value = payRel * 0.01M;

            decimal.TryParse(TextBoxHdrWCompPct.Text, out decimal wcomp);
            e.Command.Parameters["@WCompPct"].Value = wcomp * 0.01M;

            //decimal.TryParse(TextBoxOtherLaborDlr.Text, out decimal otherLabor);
            //e.Command.Parameters["@OtherLaborDLR"].Value = otherLabor;

            //e.Command.Parameters["@HourlyLaborNotes"].Value = TextBoxHourlyLaborNotes.Text;

        }

        protected void TextBoxMgmtHdr_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SqlDataSourceMgmtHdr.Update() > 0)
                {
                    FormViewBudget.DataBind();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.IndexOf("duplicate key") >= 0)
                {
                    ShowDialog("SQL Error", MessageType.Error, "Duplicate record.", "");
                }
                else
                {
                    ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                }
            }
            catch (Exception ex)
            {
                ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
            }

        }

        protected void gvMgmtEmps_PreRender(object sender, EventArgs e)
        {
            GridView gvMgmtEmps = (GridView)sender;
            if (gvMgmtEmps == null)
                return;

            //Bind the subsidiary gridviews so we can assign values there as well
            GridView gvMgmtEmpsFringe = (GridView)FormViewBudget.FindControl("gvMgmtEmpsFringe");
            if (gvMgmtEmpsFringe == null)
                return;
            if (gvMgmtEmpsFringe.HeaderRow == null)
                gvMgmtEmpsFringe.DataBind();

            GridView gvMgmtEmpsBenefits = (GridView)FormViewBudget.FindControl("gvMgmtEmpsBenefits");
            if (gvMgmtEmpsBenefits == null)
                return;

            GridView gvMgmtEmpsTotals = (GridView)FormViewBudget.FindControl("gvMgmtEmpsTotals");
            if (gvMgmtEmpsTotals == null)
                return;

            GridView gvMgmtExpenses = (GridView)FormViewBudget.FindControl("gvMgmtExpenses");
            if (gvMgmtExpenses == null)
                return;

            //Fields outside GridViews
            TextBox TextBoxMgmtIncrease = (TextBox)FormViewBudget.FindControl("TextBoxMgmtIncrease");
            Label LabelMgrSalBens = (Label)FormViewBudget.FindControl("LabelMgrSalBens");
            Label LabelExpenses = (Label)FormViewBudget.FindControl("LabelExpenses");
            TextBox TextBoxOtherMgrDlr = (TextBox)FormViewBudget.FindControl("TextBoxOtherMgrDlr");
            Label LabelMgmtGrandTotalCosts = (Label)FormViewBudget.FindControl("LabelMgmtGrandTotalCosts");
            TextBox TextBoxMgmtNotes = (TextBox)FormViewBudget.FindControl("TextBoxMgmtNotes");
            if (TextBoxMgmtIncrease == null || LabelMgrSalBens==null || LabelExpenses==null || TextBoxOtherMgrDlr==null || LabelMgmtGrandTotalCosts==null || TextBoxMgmtNotes==null)
            {
                return;
            }

            //gvMgmtEmps Labels
            Label LabelSubtotalCellPhone = new Label();
            Label LabelSubtotalFitness = new Label();
            Label LabelSubtotalSalary = new Label();
            Label LabelSubtotalBonus = new Label();
            Label LabelSubtotalHealthBuyout = new Label();
            Label LabelTotalWages = new Label();

            //gvMgmtEmps Footer Values
            if (gvMgmtEmps.FooterRow != null)
            {
                LabelSubtotalCellPhone = (Label)gvMgmtEmps.FooterRow.FindControl("LabelSubtotalCellPhone");
                LabelSubtotalFitness = (Label)gvMgmtEmps.FooterRow.FindControl("LabelSubtotalFitness");
                LabelSubtotalSalary = (Label)gvMgmtEmps.FooterRow.FindControl("LabelSubtotalSalary");
                LabelSubtotalBonus = (Label)gvMgmtEmps.FooterRow.FindControl("LabelSubtotalBonus");
                LabelSubtotalHealthBuyout = (Label)gvMgmtEmps.FooterRow.FindControl("LabelSubtotalHealthBuyout");
                LabelTotalWages = (Label)gvMgmtEmps.FooterRow.FindControl("LabelGrandTotalWages");

                if (LabelSubtotalCellPhone == null || LabelSubtotalFitness == null || LabelSubtotalSalary == null || LabelSubtotalBonus == null || LabelSubtotalHealthBuyout == null || LabelTotalWages == null)
                {
                    return;
                }
            }

            //gvMgmtEmpsFringe Mandatory values
            //Headers
            TextBox TextBoxHdrFICAPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrFICAPct");
            TextBox TextBoxHdrFUTAPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrFUTAPct");
            TextBox TextBoxHdrFUTADlr = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrFUTADlr");
            TextBox TextBoxHdrSUIPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrSUIPct");
            TextBox TextBoxHdrSUIDlr = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrSUIDlr");
            TextBox TextBoxHdrPayrollRelatedPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrPayrollRelatedPct");
            TextBox TextBoxHdrWCompPct = (TextBox)gvMgmtEmpsFringe.HeaderRow.FindControl("TextBoxHdrWCompPct");
            if (TextBoxHdrFICAPct == null || TextBoxHdrFUTAPct == null || TextBoxHdrFUTADlr == null || TextBoxHdrSUIPct == null || TextBoxHdrSUIDlr == null || TextBoxHdrPayrollRelatedPct == null || TextBoxHdrWCompPct == null)
                return;
            //Footers
            Label LabelTotalFica = (Label)gvMgmtEmpsFringe.FooterRow.FindControl("LabelTotalFica");
            Label LabelTotalFuta = (Label)gvMgmtEmpsFringe.FooterRow.FindControl("LabelTotalFuta");
            Label LabelTotalPayrollRelated = (Label)gvMgmtEmpsFringe.FooterRow.FindControl("LabelTotalPayrollRelated");
            Label LabelTotalSUI = (Label)gvMgmtEmpsFringe.FooterRow.FindControl("LabelTotalSUI");
            Label LabelTotalWComp = (Label)gvMgmtEmpsFringe.FooterRow.FindControl("LabelTotalWComp");
            Label LabelTotalMandatory = (Label)gvMgmtEmpsFringe.FooterRow.FindControl("LabelTotalMandatory");
            if (LabelTotalFica == null || LabelTotalFuta == null || LabelTotalPayrollRelated == null || LabelTotalSUI == null || LabelTotalWComp == null || LabelTotalMandatory == null)
                return;

            //gvMgmtEmpsBenefits values
            //Footers
            Label LabelHealthTotal = (Label)gvMgmtEmpsBenefits.FooterRow.FindControl("LabelHealthTotal");
            Label LabelLifeTotal = (Label)gvMgmtEmpsBenefits.FooterRow.FindControl("LabelLifeTotal");
            Label LabelDentalTotal = (Label)gvMgmtEmpsBenefits.FooterRow.FindControl("LabelDentalTotal");
            Label LabelVisionTotal = (Label)gvMgmtEmpsBenefits.FooterRow.FindControl("LabelVisionTotal");
            Label LabelOtherBenefitTotal = (Label)gvMgmtEmpsBenefits.FooterRow.FindControl("LabelOtherBenefitTotal");
            Label LabelTotalBenefits = (Label)gvMgmtEmpsBenefits.FooterRow.FindControl("LabelTotalBenefits");
            if (LabelHealthTotal == null || LabelLifeTotal == null || LabelDentalTotal == null || LabelVisionTotal == null || LabelOtherBenefitTotal == null || LabelTotalBenefits == null )
                return;

            //gvMgmtEmpsTotals values
            //Footers
            Label LabelGrandTotalWages = (Label)gvMgmtEmpsTotals.FooterRow.FindControl("LabelGrandTotalWages");
            Label LabelGrandTotalMandatory = (Label)gvMgmtEmpsTotals.FooterRow.FindControl("LabelGrandTotalMandatory");
            Label LabelGrandTotalBenefits = (Label)gvMgmtEmpsTotals.FooterRow.FindControl("LabelGrandTotalBenefits");
            Label Label401kTotal = (Label)gvMgmtEmpsTotals.FooterRow.FindControl("Label401kTotal");
            Label LabelGrandTotalFringeBens = (Label)gvMgmtEmpsTotals.FooterRow.FindControl("LabelGrandTotalFringeBens");

            if (LabelGrandTotalWages == null || LabelGrandTotalMandatory == null || LabelGrandTotalBenefits == null || Label401kTotal == null || LabelGrandTotalFringeBens == null)
                return;

            //gvMgmtExpenses values
            //Footers
            Label LabelTravelTotal = (Label)gvMgmtExpenses.FooterRow.FindControl("LabelTravelTotal");
            Label LabelDuesTotal = (Label)gvMgmtExpenses.FooterRow.FindControl("LabelDuesTotal");
            Label LabelEducationTotal = (Label)gvMgmtExpenses.FooterRow.FindControl("LabelEducationTotal");
            Label LabelSubsTotal = (Label)gvMgmtExpenses.FooterRow.FindControl("LabelSubsTotal");
            Label LabelExpensesTotal = (Label)gvMgmtExpenses.FooterRow.FindControl("LabelExpensesTotal");
            if (LabelTravelTotal == null || LabelDuesTotal == null || LabelEducationTotal == null || LabelSubsTotal == null || LabelExpensesTotal == null)
                return;


            SqlDataReader rdr;
            SqlDataSourceMgmtHdr.DataBind();
            rdr = (SqlDataReader)SqlDataSourceMgmtHdr.Select(DataSourceSelectArguments.Empty);

            if (rdr != null && rdr.Read())
            {
                TextBoxMgmtIncrease.Text = ((decimal)rdr["PctBudgetIncrease"] * 100M).ToString("F2");
                LabelMgrSalBens.Text = ((decimal)rdr["SubtotalWageMandBens"]).ToString("C2");
                LabelExpenses.Text = ((decimal)rdr["GrandTotalExpenses"]).ToString("C2");
                TextBoxOtherMgrDlr.Text = ((decimal)rdr["OtherLaborDLR"]).ToString("F2");
                LabelMgmtGrandTotalCosts.Text = ((decimal)rdr["GrandTotalMgmtCost"]).ToString("C2");
                TextBoxMgmtNotes.Text = rdr["MgmtLaborNotes"].ToString();

                //gvMgmtEmps Footer Totals
                LabelSubtotalCellPhone.Text = ((decimal)rdr["TotalCellBenefit"]).ToString("C2");
                LabelSubtotalFitness.Text = ((decimal)rdr["TotalFitnessCredit"]).ToString("C2");
                LabelSubtotalSalary.Text = ((decimal)rdr["TotalNewSalary"]).ToString("C2");
                LabelSubtotalBonus.Text = ((decimal)rdr["TotalBonus"]).ToString("C2");
                LabelSubtotalHealthBuyout.Text = ((decimal)rdr["TotalHealthBuyout"]).ToString("C2");
                LabelTotalWages.Text = ((decimal)rdr["TotalWages"]).ToString("C2");

                //gvMgmtEmpsFringe Header Values
                TextBoxHdrFICAPct.Text = ((decimal)rdr["FICAPCT"] * 100M).ToString("F4");
                TextBoxHdrFUTAPct.Text = ((decimal)rdr["FUTAPCT"] * 100M).ToString("F4");
                TextBoxHdrFUTADlr.Text = ((decimal)rdr["FUTADLR"]).ToString("F0");
                TextBoxHdrSUIPct.Text = ((decimal)rdr["SUIPCT"] * 100M).ToString("F4");
                TextBoxHdrSUIDlr.Text = ((decimal)rdr["SUIDLR"]).ToString("F0");
                TextBoxHdrPayrollRelatedPct.Text = ((decimal)rdr["RelatedPayrollPCT"] * 100M).ToString("F4");
                TextBoxHdrWCompPct.Text = ((decimal)rdr["WCompPCT"] * 100M).ToString("F4");

                //gvMgmtEmpsFringe Footer Values
                LabelTotalFica.Text = ((decimal)rdr["TotalFica"]).ToString("C2");
                LabelTotalFuta.Text = ((decimal)rdr["TotalFuta"]).ToString("C2");
                LabelTotalPayrollRelated.Text = ((decimal)rdr["TotalPayrel"]).ToString("C2");
                LabelTotalSUI.Text = ((decimal)rdr["TotalSui"]).ToString("C2");
                LabelTotalWComp.Text = ((decimal)rdr["TotalWComp"]).ToString("C2");
                LabelTotalMandatory.Text = ((decimal)rdr["GrandTotalMandatory"]).ToString("C2");

                //gvMgmtEmpsBenefits Footer Values
                LabelHealthTotal.Text = ((decimal)rdr["TotalHealth"]).ToString("C2");
                LabelLifeTotal.Text = ((decimal)rdr["TotalLife"]).ToString("C2");
                LabelDentalTotal.Text = ((decimal)rdr["TotalDental"]).ToString("C2");
                LabelVisionTotal.Text = ((decimal)rdr["TotalVision"]).ToString("C2");
                LabelOtherBenefitTotal.Text = ((decimal)rdr["TotalOtherBenefit"]).ToString("C2");
                LabelTotalBenefits.Text = ((decimal)rdr["GrandTotalBenefits"]).ToString("C2");

                //gvMgmtEmpsTotals Footer Totals
                LabelGrandTotalWages.Text = ((decimal)rdr["TotalWages"]).ToString("C2");
                LabelGrandTotalMandatory.Text = ((decimal)rdr["GrandTotalMandatory"]).ToString("C2");
                LabelGrandTotalBenefits.Text = ((decimal)rdr["GrandTotalBenefits"]).ToString("C2");
                Label401kTotal.Text = ((decimal)rdr["Total401k"]).ToString("C2");
                LabelGrandTotalFringeBens.Text = ((decimal)rdr["GrandTotalFringeBens"]).ToString("C2");

                //gvMgmtExpenses Footer Values
                LabelTravelTotal.Text = ((decimal)rdr["TotalTravel"]).ToString("C2");
                LabelDuesTotal.Text = ((decimal)rdr["TotalDues"]).ToString("C2");
                LabelEducationTotal.Text = ((decimal)rdr["TotalEducation"]).ToString("C2");
                LabelSubsTotal.Text = ((decimal)rdr["TotalSubscriptions"]).ToString("C2");
                LabelExpensesTotal.Text = ((decimal)rdr["GrandTotalExpenses"]).ToString("C2");





                rdr.Close();
            }
            else
            {
                TextBoxMgmtIncrease.Text = "0.00";
                LabelMgrSalBens.Text = "$0.00";
                LabelExpenses.Text = "$0.00";
                TextBoxOtherMgrDlr.Text = "0.00";
                LabelMgmtGrandTotalCosts.Text = "$0.00";
                TextBoxMgmtNotes.Text = string.Empty;

                //gvMgmtEmps Footer Totals
                LabelSubtotalSalary.Text = "$0.00";
                LabelSubtotalBonus.Text = "$0.00";
                LabelSubtotalHealthBuyout.Text = "$0.00";
                LabelTotalWages.Text = "$0.00";

                //gvMgmtEmpsFringe Header Values
                TextBoxHdrFICAPct.Text = "0.0000";
                TextBoxHdrFUTAPct.Text = "0.0000";
                TextBoxHdrFUTADlr.Text = "0";
                TextBoxHdrSUIPct.Text = "0.0000";
                TextBoxHdrSUIDlr.Text = "0";
                TextBoxHdrPayrollRelatedPct.Text = "0.0000";
                TextBoxHdrWCompPct.Text = "0.0000";

                //gvMgmtEmpsFringe Footer Values
                LabelTotalFica.Text = "$0.00";
                LabelTotalFuta.Text = "$0.00";
                LabelTotalPayrollRelated.Text = "$0.00";
                LabelTotalSUI.Text = "$0.00";
                LabelTotalWComp.Text = "$0.00";
                LabelTotalMandatory.Text = "$0.00";

                //gvMgmtEmpsBenefits Footer Values
                LabelHealthTotal.Text = "$0.00";
                LabelLifeTotal.Text = "$0.00";
                LabelDentalTotal.Text = "$0.00";
                LabelVisionTotal.Text = "$0.00";
                LabelOtherBenefitTotal.Text = "$0.00";
                LabelTotalBenefits.Text = "$0.00";

                //gvMgmtEmpsTotals Footer Totals
                LabelGrandTotalWages.Text = "$0.00";
                LabelGrandTotalMandatory.Text = "$0.00";
                LabelGrandTotalBenefits.Text = "$0.00";
                Label401kTotal.Text = "$0.00";
                LabelGrandTotalFringeBens.Text = "$0.00";

                //gvMgmtExpenses Footer Values
                LabelTravelTotal.Text = "$0.00";
                LabelDuesTotal.Text = "$0.00";
                LabelEducationTotal.Text = "$0.00";
                LabelSubsTotal.Text = "$0.00";
                LabelExpensesTotal.Text = "$0.00";
            }

        }

        protected void ToggleBudgetLock(string budgetYear, string districtID, int budgetVersion)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string queryCheckDate = "UPDATE [FMSBudgetHeader] SET LockedBudget=IIF(LockedBudget=1,0,1) " +
                                    "WHERE BudgetYear = @BudgetYear AND DistrictID = @DistrictID AND BudgetVersion = @BudgetVersion";

            using (SqlCommand cmd = new SqlCommand(queryCheckDate, sqlConnection))
            {
                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@BudgetYear";
                dbParam_progYear.Value = budgetYear;
                dbParam_progYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = districtID;
                dbParam_distID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_version = new SqlParameter();
                dbParam_version.ParameterName = "@BudgetVersion";
                dbParam_version.Value = budgetVersion;
                dbParam_version.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_version);

                try
                {
                    sqlConnection.Open();
                    int recordCount = 0;
                    recordCount = cmd.ExecuteNonQuery();
                }
                catch(SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }

            }

            if (sqlConnection != null)
                sqlConnection.Dispose();

        }

        protected bool BudgetYearAllowsEdits(string BudgetYear)
        {
            bool budgetEditsAllowed = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string queryCheckDate = "SELECT COUNT(*) FROM [FMSBudgetYear_Setup] " +
                " WHERE BudgetYear=@BudgetYear AND CB_AllowEditExisting=1";

            using (SqlCommand cmd = new SqlCommand(queryCheckDate, sqlConnection))
            {
                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@BudgetYear";
                dbParam_progYear.Value = BudgetYear;
                dbParam_progYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_progYear);

                sqlConnection.Open();
                int openCount = 0;
                openCount = (int)cmd.ExecuteScalar();

                if (openCount > 0)
                    budgetEditsAllowed = true;
                else
                    budgetEditsAllowed = false;
            }

            if (sqlConnection != null)
                sqlConnection.Dispose();


            return budgetEditsAllowed;
        }

        protected bool BudgetYearAllowsNew(string BudgetYear)
        {
            bool newBudgetsAllowed = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string queryCheckDate = "SELECT COUNT(*) FROM [FMSBudgetYear_Setup] " +
                " WHERE BUdgetYear=@BudgetYear AND CB_AllowNewBudgets=1";

            using (SqlCommand cmd = new SqlCommand(queryCheckDate, sqlConnection))
            {
                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@BudgetYear";
                dbParam_progYear.Value = BudgetYear;
                dbParam_progYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_progYear);

                sqlConnection.Open();
                int openCount = 0;
                openCount = (int)cmd.ExecuteScalar();

                if (openCount > 0)
                    newBudgetsAllowed = true;
                else
                    newBudgetsAllowed = false;
            }

            if (sqlConnection != null)
                sqlConnection.Dispose();


            return newBudgetsAllowed;
        }

        protected void ILockUnlock_Click(object sender, EventArgs e)
        {
            ToggleBudgetLock(HFBudgetYear.Value, HFDistrictID.Value, int.Parse(HFBudgetVersion.Value));
            FormViewBudget.DataBind();
        }

        protected void gvHourlyEmps_DataBound(object sender, EventArgs e)
        {
            CheckBox CBRetainFooterEntries = (CheckBox)FormViewBudget.FindControl("CBRetainFooterEntries");
            if (CBRetainFooterEntries != null && CBRetainFooterEntries.Checked)
            {

                GridView gvHourlyEmps = (GridView)sender;
                TextBox TextBoxEmployeeF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxEmployeeF");
                if (TextBoxEmployeeF != null)
                    TextBoxEmployeeF.Text = HFHourlyEmpName.Value;

                //Reapply last values to speed data entry
                DropDownList DDLSchoolsF = (DropDownList)gvHourlyEmps.FooterRow.FindControl("DDLSchoolsF");
                if (DDLSchoolsF != null && !string.IsNullOrEmpty(HFHourlySchoolNum.Value))
                    DDLSchoolsF.SelectedValue = HFHourlySchoolNum.Value;

                TextBox TextBoxJobF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxJobF");
                if (TextBoxJobF != null)
                    TextBoxJobF.Text = HFHourlyJobClass.Value;

                TextBox TextBoxScheduleF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxScheduleF");
                if (TextBoxScheduleF != null)
                    TextBoxScheduleF.Text = HFHourlySchedule.Value;

                TextBox TextBoxRateF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxRateF");
                if (TextBoxRateF != null)
                    TextBoxRateF.Text = HFHourlyRate.Value;

                TextBox TextBoxHoursDayF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxHoursDayF");
                if (TextBoxHoursDayF != null)
                    TextBoxHoursDayF.Text = HFHourlyHours.Value;

                TextBox TextBoxDaysPaidF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxDaysPaidF");
                if (TextBoxDaysPaidF != null)
                    TextBoxDaysPaidF.Text = HFHourlyDays.Value;

                DropDownList DDLPlanTypeF = (DropDownList)gvHourlyEmps.FooterRow.FindControl("DDLPlanTypeF");
                if (DDLPlanTypeF != null && !string.IsNullOrEmpty(HFHourlyPlanType.Value))
                    DDLPlanTypeF.SelectedValue = HFHourlyPlanType.Value;

                TextBox TextBoxHealthF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxHealthF");
                if (TextBoxHealthF != null)
                    TextBoxHealthF.Text = HFHourlyHealth.Value;

                TextBox TextBoxDentalF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxDentalF");
                if (TextBoxDentalF != null)
                    TextBoxDentalF.Text = HFHourlyDental.Value;

                TextBox TextBoxDisabilityF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxDisabilityF");
                if (TextBoxDisabilityF != null)
                    TextBoxDisabilityF.Text = HFHourlyDisability.Value;

                TextBox TextBoxVisionF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxVisionF");
                if (TextBoxVisionF != null)
                    TextBoxVisionF.Text = HFHourlyVision.Value;

                TextBox TextBoxAnnuityF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxAnnuityF");
                if (TextBoxAnnuityF != null)
                    TextBoxAnnuityF.Text = HFHourlyAnnuity.Value;

                TextBox TextBoxLifeF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxLifeF");
                if (TextBoxLifeF != null)
                    TextBoxLifeF.Text = HFHourlyLife.Value;

                TextBox TextBoxRetireF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxRetireF");
                if (TextBoxRetireF != null)
                    TextBoxRetireF.Text = HFHourlyRetire.Value;

                TextBox TextBoxOtherF = (TextBox)gvHourlyEmps.FooterRow.FindControl("TextBoxOtherF");
                if (TextBoxOtherF != null)
                    TextBoxOtherF.Text = HFHourlyOther.Value;
            }

        }

        protected void buttonLookupContract_Click(object sender, EventArgs e)
        {
            LinkButton linkButton = (LinkButton)sender;
            if (string.IsNullOrEmpty(HFBudgetVersion.Value) || string.IsNullOrEmpty(HFBudgetYear.Value) || string.IsNullOrEmpty(HFDistrictID.Value))
            {
                ShowDialog("Error", MessageType.Error, "Please select a budget first.", "");
                return;
            }

            TextBoxSearchContracts.Text = string.Empty;//Reset the search box
            GridViewContracts.DataBind();

            TextBox TextBoxContractID = (TextBox)FindControlRecursive(FormViewBudget, "TextBoxContractID");

            bool contractFound = false;
            if (TextBoxContractID!=null && !string.IsNullOrEmpty(TextBoxContractID.Text))
            {
                foreach (GridViewRow row in GridViewContracts.Rows)
                {
                    if (row.Cells[0].Text == TextBoxContractID.Text)
                    {
                        GridViewContracts.SelectedIndex = row.RowIndex;
                        hfSelectedIndex.Value = (GridViewContracts.SelectedIndex + 1).ToString();
                        contractFound = true;
                        break;
                    }
                }
            }

            UpdatePanelModal.Update();

            if (contractFound)
                RegisterDOMReadyScript("modalBox", "$('#myModalContracts').modal('show');higlightSelectedRow();");
            else
                RegisterDOMReadyScript("modalBox", "$('#myModalContracts').modal('show');");
        }

        protected void IClearContract_Click(object sender, EventArgs e)
        {
            TextBox TextBoxContractID = (TextBox)FormViewBudget.FindControl("TextBoxContractID");
            TextBox TextBoxContractDesc = (TextBox)FormViewBudget.FindControl("TextBoxContractDesc");
            if(TextBoxContractID==null || TextBoxContractDesc==null)
                return;

            TextBoxContractID.Text = string.Empty;
            TextBoxContractDesc.Text = string.Empty;

            FormViewBudget.UpdateItem(false);
        }

        protected void GridViewContracts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gv = (GridView)sender;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv, "Select$" + e.Row.RowIndex);
                e.Row.ToolTip = "Click to select this row.";
            }
        }

        protected void GridViewContracts_SelectedIndexChanged(object sender, EventArgs e)
        {
            //the postback process which locks in the selected row, forgets the highlight formatting. this recreates it
            //need to add one since the html fields count the headers as number zero
            //note: EnableEventValidation="false" needs to be added to page for this to fire
            hfSelectedIndex.Value = (GridViewContracts.SelectedIndex + 1).ToString();
            
            RegisterDOMReadyScript("pop", "$('#myModalContracts').modal('hide');$('.modal-backdrop').remove();$('body').removeClass('modal-open');");
            //Needed to remove the modal-backdrop which darkens the page
            //Also needed to remove the modal-open class which prevents scrolling.
            //Initially I added this manual postback which fixed the scroll issue, but caused the page to jump:   __doPostBack('', '');

            btnCloseContractModal_Click(null, null);
        }

        protected void GridViewContracts_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Datasource select script was established during modal open. On sorting, the page has forgotten the select script
        }

        protected void btnCloseContractModal_Click(object sender, EventArgs e)
        {
            TextBox TextBoxContractID = (TextBox)FindControlRecursive(FormViewBudget, "TextBoxContractID");
            if(TextBoxContractID == null)
                return;

            if (GridViewContracts.SelectedIndex == -1 || GridViewContracts.DataKeys.Count == 0 || GridViewContracts.DataKeys.Count < GridViewContracts.SelectedIndex + 1)
            {
                TextBoxContractID.Text = string.Empty;
            }
            else
            {
                TextBoxContractID.Text = GridViewContracts.DataKeys[GridViewContracts.SelectedIndex].Values["ContractID"].ToString();
            }

            FormViewBudget.UpdateItem(false);
            UpdatePanel1.Update();

        }

        protected void TextBoxHdr_TextChanged(object sender, EventArgs e)
        {
            FormViewBudget.UpdateItem(false);
        }

        protected void CheckBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            FormViewBudget.UpdateItem(false);
        }
    }
}
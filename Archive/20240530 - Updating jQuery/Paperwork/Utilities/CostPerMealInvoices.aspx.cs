using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class CPMInvoices : Paperwork_Global
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

                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count > 0 && Session["ProgramYear"] != null && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                    DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                DDLProgramYears_SelectedIndexChanged(DDLProgramYears, EventArgs.Empty);

                if (Session["CurrentDistrict"] != null && Session["CurrentDistrict"].ToString() != string.Empty)
                {
                    DropDownListNewInvDist.DataBind();
                    DropDownListNewInvDist.SelectedValue = Session["CurrentDistrict"].ToString();
                }

                if (isSFSP())
                {
                    LinkButtonSpFn.Visible = false;
                }


            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            //Update the hidden fields which contain the start and end dates.
            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int rowInt = dvSql.Find(DDLProgramYears.SelectedValue);
                DataRowView drvSql = dvSql[rowInt];
                HFProgYearStartDate.Value = ((DateTime)drvSql["StartDate"]).ToShortDateString();
                HFProgYearEndDate.Value = ((DateTime)drvSql["EndDate"]).ToShortDateString();
                HFBudgetYear.Value = drvSql["BudgetYear"].ToString();
            }
            UpdatePanelNewInv.Update();
        }

        protected void ICreate_Click(object sender, EventArgs e)
        {
            try
            {
                SqlDataSourceInvoiceList.Insert();
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

        protected void GridViewInvoiceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            if (gv.SelectedIndex == -1 && gv.Rows.Count > 0)
                gv.SelectedIndex = 0;


            if (gv.SelectedIndex != -1 && (gv.SelectedIndex + 1) <= gv.DataKeys.Count)
            {
                //Set the hidden value to the selected invoicenumber then update the displayed invoices
                HFInvoiceNumber.Value = gv.DataKeys[(gv.SelectedIndex)].Values["InvoiceNumber"].ToString();

                HiddenField HFInvoiceRowLocked = (HiddenField)FindControlRecursive(gv.SelectedRow, "HFInvoiceRowLocked");
                if (HFInvoiceRowLocked != null)
                    HFInvoiceLocked.Value = HFInvoiceRowLocked.Value;
                else
                    HFInvoiceLocked.Value = "False";

                if (HFInvoiceLocked.Value == "False")
                {
                    ISave.Enabled = true;
                    ISave.ForeColor = Color.Green;
                }
                else
                {
                    ISave.Enabled = false;
                    ISave.ForeColor = Color.Gray;
                }
            }
            else
            {
                HFInvoiceNumber.Value = string.Empty;
            }

            FormViewCPMInvoice.DataBind();
            UpdatePanelEditInvoice.Update();
        }

        protected void GridViewInvoiceList_PreRender(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            // Do not databind here - will reset any selected index

            if (gv.Rows.Count > 0 && gv.SelectedIndex == -1)
            {
                gv.SelectedIndex = 0;
                GridViewInvoiceList_SelectedIndexChanged(GridViewInvoiceList, EventArgs.Empty);
            }

            //If no row selected - upate any displayed invoice
            if (gv.DataKeys.Count == 0)
            {
                HFInvoiceNumber.Value = "";
                FormViewCPMInvoice.DataBind();
                UpdatePanelEditInvoice.Update();
            }



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

            UpdatePanelExistingInv.Update();
        }

        protected void GridViewInvoiceList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Make the row clickable
            GridviewClickableRowDatabound(sender, e);
        }

        protected void GridViewInvoiceList_PageIndexChanged(object sender, EventArgs e)
        {
            if (sender != null)
            {
                GridView gv = (GridView)sender;
                gv.DataBind();
                gv.SelectRow(0);
            }
        }

        protected void ISave_Click(object sender, EventArgs e)
        {
            SqlDataSourceInvDetails.Update();
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            Session["CPMInvoiceNumber"] = HFInvoiceNumber.Value;
            Response.Redirect("~/Print/PrintInvoiceCostPerMeal.aspx");
        }

        protected void IPrintPL_Click(object sender, EventArgs e)
        {
            HiddenField HFInvoiceDistrictID = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFInvoiceDistrictID");
            if (HFInvoiceDistrictID != null)
                Session["CPMDistID"] = HFInvoiceDistrictID.Value;
            TextBox TextBoxDate = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxDate");
            if (TextBoxDate != null)
                Session["CPMInvoiceDate"] = TextBoxDate.Text;


            Response.Redirect("~/Print/PrintCPMPLReport.aspx");
        }

        protected void GridViewInvoiceList_Sorted(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            gv.DataBind();

            for (int i = 0; i < gv.DataKeys.Count; i++)
            {
                if (gv.DataKeys[i].Values["InvoiceNumber"].ToString() == HFInvoiceNumber.Value)
                {
                    gv.SelectedIndex = i;
                    break;
                }
            }
        }

        protected Boolean GetRecapsTotals(string ProgramYear, string DistrictID, DateTime StartingDate, DateTime EndingDate, out int BreakfastCount, out int LunchCount, out decimal AlaTotal, out int SnackCount, out int DinnerCount)
        {
            Boolean success = false;
            BreakfastCount = 0;
            LunchCount = 0;
            AlaTotal = 0M;
            SnackCount = 0;
            DinnerCount = 0;

            string selectQuery = "SELECT SUM([BPaidElem]) + SUM([BPaidJr]) + SUM([BPaidSr]) + SUM([BReduced]) + SUM([BFree]) + SUM(BPaidLrn) + SUM(BReducedLrn) + SUM(BFreeLrn) BreakfastTotal, " +
                                " SUM(LPaidElem)+SUM(LPaidJr)+SUM(LPaidSr)+SUM(LTier1)+SUM(LTier2)+SUM(LReduced)+SUM(LReducedTier1)+SUM(LFree)+SUM(LFreeTier1)+SUM(LFreeTier2)+SUM(LPaidLrn)+SUM(LReducedLrn)+SUM(LFreeLrn) LunchTotal, " +
                                " SUM(LStuAla)+SUM(LAdultAla)+SUM(LAdult*SchoolSetup.AdultLunchPrice)+SUM(LOtherAla1)+SUM(LVendAla)+SUM(BStuAla)+SUM(BAdultAla) AlaTotal," +
                                " +SUM(OPaidSnack)+SUM(OReducedSnack) SnackTotal, SUM(Dinner) DinnerTotal" +
                                " FROM [dbo].[RecapEntry] " +
                                " INNER JOIN dbo.DistrictSchoolYearSetup AS SchoolSetup ON dbo.RecapEntry.DistrictID = SchoolSetup.DistrictID AND dbo.RecapEntry.SchoolNum = SchoolSetup.SchoolNum AND dbo.RecapEntry.ProgramYear = SchoolSetup.ProgramYear " +
                                " WHERE [RecapEntry].ProgramYear = @ProgramYear AND [RecapEntry].DistrictID = @DistrictID AND [RecapEntry].RecapDate>= @StartingDate AND [RecapEntry].RecapDate<= @EndingDate ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdReadRecords = new SqlCommand(selectQuery, dbConnection))
                    {
                        SqlParameter dbParam_ProgYear = new SqlParameter();
                        dbParam_ProgYear.ParameterName = "@ProgramYear";
                        dbParam_ProgYear.Value = ProgramYear;
                        dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdReadRecords.Parameters.Add(dbParam_ProgYear);

                        SqlParameter dbParam_DistID = new SqlParameter();
                        dbParam_DistID.ParameterName = "@DistrictID";
                        dbParam_DistID.Value = DistrictID;
                        dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                        cmdReadRecords.Parameters.Add(dbParam_DistID);

                        SqlParameter dbParam_StartDate = new SqlParameter();
                        dbParam_StartDate.ParameterName = "@StartingDate";
                        dbParam_StartDate.Value = StartingDate;
                        dbParam_StartDate.DbType = System.Data.DbType.DateTime;
                        cmdReadRecords.Parameters.Add(dbParam_StartDate);

                        SqlParameter dbParam_EndDate = new SqlParameter();
                        dbParam_EndDate.ParameterName = "@EndingDate";
                        dbParam_EndDate.Value = EndingDate;
                        dbParam_EndDate.DbType = System.Data.DbType.DateTime;
                        cmdReadRecords.Parameters.Add(dbParam_EndDate);

                        SqlDataReader rdr = cmdReadRecords.ExecuteReader();
                        if (rdr.Read())
                        {
                            if (int.TryParse(rdr["BreakfastTotal"].ToString(), out BreakfastCount) == false) { }
                            if (int.TryParse(rdr["LunchTotal"].ToString(), out LunchCount) == false) { }
                            if (decimal.TryParse(rdr["AlaTotal"].ToString(), out AlaTotal) == false) { }
                            if (int.TryParse(rdr["SnackTotal"].ToString(), out SnackCount) == false) { }
                            if (int.TryParse(rdr["DinnerTotal"].ToString(), out DinnerCount) == false) { }

                        }
                        success = true;
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Exception occurred while reading Sales Totals. ", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "General Exception occurred while reading Sales Totals. ", ex.Message);
                }
            }

            return success;
        }

        protected void TextBoxDateField_TextChanged(object sender, EventArgs e)
        {
            TextBox tbDateField = (TextBox)sender;

            if (tbDateField == null)
                return;

            NormalizeSimplifiedDate(ref tbDateField);

            //Assist with control focus after postback
            if (tbDateField.ID == "TextBoxNewInvPerStart")
            {
                RegisterDOMReadyScript("Cal", "$('[id*=TextBoxNewInvPerEnd]').datepicker({changeMonth: true,changeYear: true}); ");
                TextBoxNewInvPerEnd.Focus();
                if (TextBoxNewInvPerEnd.Text == "")
                    TextBoxNewInvPerEnd.Text = TextBoxNewInvPerStart.Text;

            }
            else if (tbDateField.ID == "TextBoxNewInvPerEnd")
                DropDownListNewInvDist.Focus();
        }

        protected void SqlDataSourceInvoiceList_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            int BreakfastCount = 0;
            int LunchCount = 0;
            int SnackCount = 0;
            int DinnerCount = 0;
            decimal AlaTotal = 0M;
            decimal CPM = 0M;
            decimal MEFAC = 0M;
            decimal MEFBr = 0M;
            decimal MEFSn = 0M;
            decimal MEFDn = 0M;
            bool OverrideMEs = false;

            if (DateTime.TryParse(TextBoxNewInvPerStart.Text, out DateTime StartDate) == false)
            {
                if (e != null)
                    e.Cancel = true;
                return;
            }
            if (DateTime.TryParse(TextBoxNewInvPerEnd.Text, out DateTime EndDate) == false)
            {
                if (e != null)
                    e.Cancel = true;
                return;
            }

            GetRecapsTotals(DDLProgramYears.SelectedValue, DropDownListNewInvDist.SelectedValue, StartDate, EndDate, out BreakfastCount, out LunchCount, out AlaTotal, out SnackCount, out DinnerCount);

            if (e != null)
            {
                e.Command.Parameters["@BreakfastTotal"].Value = BreakfastCount;
                e.Command.Parameters["@LunchTotal"].Value = LunchCount;
                e.Command.Parameters["@AlaSales"].Value = AlaTotal;
                e.Command.Parameters["@SnackTotal"].Value = SnackCount;
                e.Command.Parameters["@DinnerTotal"].Value = DinnerCount;
            }

            //Check for CPM Billing Rate override in Dist Year Setup
            DataView dvSqlDist = (DataView)SqlDataSourceDistYearInfo.Select(DataSourceSelectArguments.Empty);
            if (dvSqlDist.Count > 0)
            {
                DataRowView drvSqlDist = dvSqlDist[0];//set to first row from Data View
                CPM = (decimal)drvSqlDist["CPMBillingRate"];

                OverrideMEs = (bool)drvSqlDist["CBOverrideMERates"];

                //If a value is entered - use it, otherwise if zero during budget run it will be updated with budget amounts (except SFSP)
                if (OverrideMEs)
                {
                    MEFAC = (decimal)drvSqlDist["SpecifiedMEquivRate"];
                    MEFBr = (decimal)drvSqlDist["SpecifiedMEBfast"];
                    MEFSn = (decimal)drvSqlDist["SpecifiedMESnack"];
                    MEFDn = (decimal)drvSqlDist["SpecifiedMEDinner"];
                }
            }

            DataView dvSqlBDGT = new DataView();
            DataView dvSqlBDGTSFSP = new DataView();

            if (isSFSP())
                dvSqlBDGTSFSP = (DataView)SqlDataSourceBDGTSFSP.Select(DataSourceSelectArguments.Empty);

            dvSqlBDGT = (DataView)SqlDataSourceBDGT.Select(DataSourceSelectArguments.Empty);

            if (dvSqlBDGT.Count > 0)
            {
                DataRowView drvSql = dvSqlBDGT[0];//set to first row from Data View

                if (MEFAC==0M)
                    MEFAC = (decimal)drvSql["MealEquivalentFactor"];

                //Get Cost Per Meal from budgets (if override not set)
                if (CPM == 0M && isSFSP()==false)
                    CPM = (decimal)drvSql["FoodCPM"];
            }

            if(isSFSP() && dvSqlBDGTSFSP.Count>0)
            {
                DataRowView drvSql = dvSqlBDGTSFSP[0];//set to first row from Data View
                //Get Cost Per Meal from budgets (if override not set)
                if (CPM == 0M && isSFSP() == true)
                    CPM = (decimal)drvSql["FoodCPM"];
            }

            e.Command.Parameters["@CPMRate"].Value = CPM;
            e.Command.Parameters["@ACEquivRate"].Value = MEFAC;
            e.Command.Parameters["@BrEquivRate"].Value = MEFBr;
            e.Command.Parameters["@SnEquivRate"].Value = MEFSn;
            e.Command.Parameters["@DnEquivRate"].Value = MEFDn;

            e.Command.Parameters["@ShowOverrideMERates"].Value = OverrideMEs;

        }

        protected void SqlDataSourceInvoiceList_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            RegisterDOMReadyScript("Toggle", "$('#cardCollapse1').removeClass('show');");
            int.TryParse(e.Command.Parameters["@InvoiceIdent"].Value.ToString(), out int NewInvoice);

            GridViewInvoiceList.DataBind();

            //Find and select the new invoice
            if (NewInvoice != 0)
            {
                for (int i = 0; i < GridViewInvoiceList.Rows.Count; i++)
                {
                    if (GridViewInvoiceList.DataKeys[i].Values["InvoiceNumber"].ToString() == NewInvoice.ToString())
                    {
                        GridViewInvoiceList.SelectedIndex = i;
                        GridViewInvoiceList_SelectedIndexChanged(GridViewInvoiceList, EventArgs.Empty);
                        break;
                    }
                }
            }

            UpdatePanelExistingInv.Update();


        }

        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePanelExistingInv.Update();
        }

        protected void GridViewInvoiceList_DataBound(object sender, EventArgs e)
        {

        }

        protected void SqlDataSourceInvoiceList_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            GridViewInvoiceList.SelectedIndex = -1;
        }

        protected void UpdateSpFnDateRange(object sender, EventArgs e)
        {
            if (DateTime.TryParse(TextBoxSpFnStartDate.Text, out DateTime startDate) == false)
                return;
            if (DateTime.TryParse(TextBoxSpFnEndDate.Text, out DateTime endDate) == false)
                return;

            GridviewSpecialFunctions.DataBind();

            decimal SelectedTotal = TotalSpFnGridview();
            LabelTotal.Text = SelectedTotal.ToString("F2");
        }


        protected decimal TotalSpFnGridview()
        {
            Decimal SpFnTotal = 0M;

            foreach (GridViewRow gvr in GridviewSpecialFunctions.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    Label LabelInvoiceTotal = (Label)FindControlRecursive(gvr, "LabelInvoiceTotal");
                    CheckBox CBSelect = (CheckBox)FindControlRecursive(gvr, "CBSelect");
                    if (LabelInvoiceTotal != null && CBSelect != null && CBSelect.Checked)
                    {
                        //string totalString = LabelInvoiceTotal.Text;//remove the "$" symbol
                        //string totalString = LabelInvoiceTotal.Text.Remove(  0, 1);//remove the "$" symbol
                        if (Decimal.TryParse(LabelInvoiceTotal.Text, out Decimal outTotal))
                            SpFnTotal += outTotal;
                    }
                }
            }

            return SpFnTotal;
        }

        protected void CBSelect_CheckedChanged(object sender, EventArgs e)
        {
            decimal SelectedTotal = TotalSpFnGridview();
            LabelTotal.Text = SelectedTotal.ToString("F2");
        }


        protected void BtnProcessSpFn_Click(object sender, EventArgs e)
        {
            TextBox TextBoxSpFnSales = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxSpFnSales");
            //string totalString = LabelTotal.Text.Remove(  0, 1);//remove the "$" symbol
            if (Decimal.TryParse(LabelTotal.Text, out Decimal outSelectedTotal) == false)
                outSelectedTotal = 0M;

            if (TextBoxSpFnSales != null)
            {
                if (rlOptions.SelectedValue == "ADD")
                {
                    string SpFnTotal = TextBoxSpFnSales.Text;
                    if (Decimal.TryParse(SpFnTotal, out Decimal outSpFnTotal) == false)
                        outSpFnTotal = 0M;

                    TextBoxSpFnSales.Text = (outSelectedTotal + outSpFnTotal).ToString("F2", CultureInfo.CurrentCulture);
                }
                else
                {
                    TextBoxSpFnSales.Text = (outSelectedTotal).ToString("F2", CultureInfo.CurrentCulture);
                }


            }



            RegisterDOMReadyScript("Pop", "$('#SpecialFunctionsImportModal').modal('hide');RecalcAll();");

            UpdatePanelEditInvoice.Update();
        }

        protected void LinkButtonSpFn_Click(object sender, EventArgs e)
        {
            //Modal opens for Special Functions Import
            //Refresh the Special Functions grid and prepopulate the dates

            TextBox TextBoxPeriodStart = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxPeriodStart");
            HiddenField HFInvoiceDistrictID = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFInvoiceDistrictID");

            HFSPFNDistrictID.Value = HFInvoiceDistrictID.Value;

            DateTime StartDate;

            if (TextBoxPeriodStart == null)
                StartDate = DateTime.Now;
            else if (DateTime.TryParse(TextBoxPeriodStart.Text, out StartDate) == false)
                StartDate = DateTime.Now;

            DateTime MonthStart = new DateTime(StartDate.Year, StartDate.Month, 1);
            DateTime MonthEnd = MonthStart.AddMonths(1).AddDays(-1);


            if (TextBoxSpFnStartDate.Text == "")
                TextBoxSpFnStartDate.Text = MonthStart.ToShortDateString();
            if (TextBoxSpFnEndDate.Text == "")
                TextBoxSpFnEndDate.Text = MonthEnd.ToShortDateString();

            //GridviewSpecialFunctions.DataBind();
            UpdateSpFnDateRange(null, null);

            RegisterDOMReadyScript("show", "$('#SpecialFunctionsImportModal').modal('show');RecalcAll();");
            OverallPanel.Update();
        }

        protected void SqlDataSourceInvDetails_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            HiddenField HFInvoiceProgYear = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFInvoiceProgYear");
            HiddenField HFInvoiceDistrictID = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFInvoiceDistrictID");
            TextBox TextBoxInvoice = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxInvoice");
            TextBox TextBoxPeriodStart = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxPeriodStart");
            TextBox TextBoxPeriodEnd = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxPeriodEnd");
            TextBox TextBoxBfastCount = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxBfastCount");
            TextBox TextBoxLunchCount = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxLunchCount");
            TextBox TextBoxAlaSales = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxAlaSales");
            TextBox TextBoxSpFnSales = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxSpFnSales");
            TextBox TextBoxUSDACommUsage = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxUSDACommUsage");
            TextBox TextBoxUSDACommRcvd = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxUSDACommRcvd");
            TextBox TextBoxSnackCount = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxSnackCount");
            TextBox TextBoxDinnerCount = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxDinnerCount");

            //PL Amounts
            TextBox TextBoxIncome = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxIncome");
            TextBox TextBoxLabor = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxLabor");
            TextBox TextBoxCosts = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxCosts");

            if (HFInvoiceProgYear == null || HFInvoiceDistrictID == null || TextBoxInvoice == null || TextBoxPeriodStart == null || TextBoxPeriodEnd == null || TextBoxBfastCount == null
                || TextBoxLunchCount == null || TextBoxAlaSales == null || TextBoxSpFnSales == null || TextBoxUSDACommUsage == null || TextBoxUSDACommRcvd == null ||
                TextBoxSnackCount == null || TextBoxDinnerCount == null)
            {
                if (e != null)
                    e.Cancel = true;
                return;
            }

            if (!DateTime.TryParse(TextBoxPeriodStart.Text, out DateTime outStartDate))
            {
                if (e != null)
                    e.Cancel = true;
                return;
            }
            if (!DateTime.TryParse(TextBoxPeriodEnd.Text, out DateTime outEndDate))
            {
                if (e != null)
                    e.Cancel = true;
                return;
            }

            if (e != null)
            {
                e.Command.Parameters["@ProgramYear"].Value = HFInvoiceProgYear.Value;
                e.Command.Parameters["@DistrictID"].Value = HFInvoiceDistrictID.Value;
                e.Command.Parameters["@InvoiceNumber"].Value = TextBoxInvoice.Text;
                e.Command.Parameters["@PeriodStart"].Value = outStartDate;
                e.Command.Parameters["@PeriodEnd"].Value = outEndDate;

                if (int.TryParse(TextBoxBfastCount.Text, out int outBfastCount) == false) { }
                e.Command.Parameters["@BreakfastTotal"].Value = outBfastCount;

                if (int.TryParse(TextBoxLunchCount.Text, out int outLunchCount) == false) { }
                e.Command.Parameters["@LunchTotal"].Value = outLunchCount;

                if (decimal.TryParse(TextBoxAlaSales.Text, out decimal outAlaSales) == false) { }
                e.Command.Parameters["@AlaSales"].Value = outAlaSales;

                if (decimal.TryParse(TextBoxSpFnSales.Text, out decimal outSpFnSales) == false) { }
                e.Command.Parameters["@SpecialFunctionSales"].Value = outSpFnSales;

                if (decimal.TryParse(TextBoxUSDACommUsage.Text, out decimal outCommUsage) == false) { }
                e.Command.Parameters["@USDACommUsage"].Value = outCommUsage;

                if (decimal.TryParse(TextBoxUSDACommRcvd.Text, out decimal outCommRcvd) == false) { }
                e.Command.Parameters["@USDACommRcvd"].Value = outCommRcvd;

                if (decimal.TryParse(TextBoxIncome.Text, out decimal outPLIncome) == false) { }
                e.Command.Parameters["@PLIncome1"].Value = outPLIncome;

                if (decimal.TryParse(TextBoxLabor.Text, out decimal outPLLabor) == false) { }
                e.Command.Parameters["@PLLabor2"].Value = outPLLabor;

                if (decimal.TryParse(TextBoxCosts.Text, out decimal outPLCost) == false) { }
                e.Command.Parameters["@PLCost2"].Value = outPLCost;

                if (int.TryParse(TextBoxSnackCount.Text, out int outSnackCount) == false) { }
                e.Command.Parameters["@SnackTotal"].Value = outSnackCount;

                if (int.TryParse(TextBoxDinnerCount.Text, out int outDinnerCount) == false) { }
                e.Command.Parameters["@DinnerTotal"].Value = outDinnerCount;
            }
        }

        protected void SqlDataSourceInvDetails_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e != null)
            {
                if (e.AffectedRows > 0)
                {
                    ShowMessageByName("Saved", MessageType.Success, "alert_container_InvoiceHdr", 3000);
                }
                else if (e.ExceptionHandled == false)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred during invoice save.", e.Exception.Message);
                }
            }
        }

        protected void TextboxYearMonth_TextChanged(object sender, EventArgs e)
        {
            int Year = 0;
            int Month = 0;
            TextBox TextBoxFYearmonth = (TextBox)sender;

            if (TextBoxFYearmonth == null)
                return;

            //If we cannot parse the year and month - set to current year month
            if (TextBoxFYearmonth.Text.Length < 6 || int.TryParse(TextBoxFYearmonth.Text.Substring(0, 4), out Year) == false || int.TryParse(TextBoxFYearmonth.Text.Substring(5, 2), out Month) == false)
            {
                TextBoxFYearmonth.Text = DateTime.Now.Year.ToString(CultureInfo.CurrentCulture) + "-" + DateTime.Now.Month.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0');
                Year = DateTime.Now.Year;
                Month = DateTime.Now.Month;
            }
        }

        protected void CBSelectInv_CheckedChanged(object sender, EventArgs e)
        {
            decimal RcvdTotal = 0M;
            decimal UsageTotal = 0M;
            TotalInvGridview(ref RcvdTotal, ref UsageTotal);
            LabelTotalRcvd.Text = RcvdTotal.ToString("F2");
            LabelTotalUsage.Text = UsageTotal.ToString("F2");
        }

        protected void LinkButtonInventory_Click(object sender, EventArgs e)
        {
            //Modal opens for Inventory Import
            //Refresh the USDA grid and prepopulate the dates

            TextBox TextBoxPeriodEnd = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxPeriodEnd");
            HiddenField HFInvoiceDistrictID = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFInvoiceDistrictID");

            HFInvDistrictID.Value = HFInvoiceDistrictID.Value;

            DateTime EndDate;

            if (TextBoxPeriodEnd == null)
                EndDate = DateTime.Now;
            else if (DateTime.TryParse(TextBoxPeriodEnd.Text, out EndDate) == false)
                EndDate = DateTime.Now;

            DateTime MonthStart = new DateTime(EndDate.Year, EndDate.Month, 1);

            TextBoxYearmonth.Text = MonthStart.Year.ToString() + "-" + MonthStart.Month.ToString().PadLeft(2, '0');

            GridviewInventory.DataBind();

            CBSelectInv_CheckedChanged(null, null);//simple way to update the modal totals

            RegisterDOMReadyScript("show", "$('#InventoryImportModal').modal('show');RecalcAll();");
            OverallPanel.Update();
        }

        protected void TotalInvGridview(ref decimal rcvdTotal, ref decimal usageTotal)
        {
            rcvdTotal = 0M;
            usageTotal = 0M;

            foreach (GridViewRow gvr in GridviewInventory.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    Label LabelRcvdTotal = (Label)FindControlRecursive(gvr, "LabelRcvdTotal");
                    Label LabelUsageTotal = (Label)FindControlRecursive(gvr, "LabelUsageTotal");
                    CheckBox CBSelect = (CheckBox)FindControlRecursive(gvr, "CBSelect");
                    if (LabelUsageTotal != null && LabelRcvdTotal != null && CBSelect != null && CBSelect.Checked)
                    {
                        if (Decimal.TryParse(LabelRcvdTotal.Text, out Decimal outTotalRcvd))
                            rcvdTotal += outTotalRcvd;

                        if (Decimal.TryParse(LabelUsageTotal.Text, out Decimal outTotalUsed))
                            usageTotal += outTotalUsed;
                    }
                }
            }

        }

        protected void GridviewInventory_DataBound(object sender, EventArgs e)
        {
            CBSelectInv_CheckedChanged(null, null);//simple way to update the modal totals
        }

        protected void BtnProcessInv_Click(object sender, EventArgs e)
        {
            TextBox TextBoxUSDACommUsage = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxUSDACommUsage");
            TextBox TextBoxUSDACommRcvd = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxUSDACommRcvd");
            Label LabelUSDACommRcvd = (Label)FindControlRecursive(FormViewCPMInvoice, "LabelUSDACommRcvd");


            if (decimal.TryParse(LabelTotalRcvd.Text, out decimal outTotalRcvd) == false)
                outTotalRcvd = 0M;
            if (decimal.TryParse(LabelTotalUsage.Text, out decimal outTotalUsed) == false)
                outTotalUsed = 0M;

            if (TextBoxUSDACommUsage != null && TextBoxUSDACommRcvd != null && LabelUSDACommRcvd != null)
            {
                if (rlInvOptions.SelectedValue == "ADD")
                {
                    if (decimal.TryParse(TextBoxUSDACommUsage.Text, out decimal outCurrUsage) == false)
                        outCurrUsage = 0M;
                    TextBoxUSDACommUsage.Text = (outTotalUsed + outCurrUsage).ToString("F2", CultureInfo.CurrentCulture);

                    if (decimal.TryParse(TextBoxUSDACommRcvd.Text, out decimal outCurrRcvd) == false)
                        outCurrRcvd = 0M;
                    TextBoxUSDACommRcvd.Text = (outTotalRcvd + outCurrRcvd).ToString("F2", CultureInfo.CurrentCulture);
                    LabelUSDACommRcvd.Text = TextBoxUSDACommRcvd.Text;
                }
                else
                {
                    TextBoxUSDACommUsage.Text = outTotalUsed.ToString("F2", CultureInfo.CurrentCulture);
                    TextBoxUSDACommRcvd.Text = outTotalRcvd.ToString("F2", CultureInfo.CurrentCulture);
                    LabelUSDACommRcvd.Text = outTotalRcvd.ToString("F2", CultureInfo.CurrentCulture);
                }
            }

            RegisterDOMReadyScript("Pop", "$('#InventoryImportModal').modal('hide');RecalcAll();");

            UpdatePanelEditInvoice.Update();
        }

        protected void FormViewCPMInvoice_DataBound(object sender, EventArgs e)
        {
            //Call Jquery which populates the totals
            RegisterDOMReadyScript("Page Load Recalculations", "RecalcAll();");

            HiddenField HFShowMEOverrides = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFShowMEOverrides");
            HtmlGenericControl DivSnackRow = (HtmlGenericControl)FindControlRecursive(FormViewCPMInvoice, "DivSnackRow");//new 7/28/2022
            HtmlGenericControl DivDinnerRow = (HtmlGenericControl)FindControlRecursive(FormViewCPMInvoice, "DivDinnerRow");//new 7/28/2022

            if (HFShowMEOverrides != null && DivSnackRow != null && DivDinnerRow != null)
            {
                if (HFShowMEOverrides.Value == "True")
                { 
                    DivSnackRow.Visible = true;
                    DivDinnerRow.Visible = true;
                }
                else
                {
                    DivSnackRow.Visible = false;
                    DivDinnerRow.Visible = false;
                }
            }

            if (isSFSP())
            {
                HtmlGenericControl divSpecFuncSales = (HtmlGenericControl)FindControlRecursive(FormViewCPMInvoice, "divSpecFuncSales");
                if (divSpecFuncSales != null)
                    divSpecFuncSales.Visible = false;
            }

        }

        protected void LinkButtonAddOtherSales_Click(object sender, EventArgs e)
        {
            if (HFInvoiceNumber == null || String.IsNullOrEmpty(HFInvoiceNumber.Value))
                return;
            if (string.IsNullOrEmpty(TextBoxOSalesDesc.Text) || string.IsNullOrEmpty(TextBoxOSalesAmount.Text))
                return;

            SqlDataSourceInvoiceLines.Insert();

        }

        protected void SqlDataSourceInvoiceLines_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            e.Command.Parameters["@OtherSalesDesc"].Value = TextBoxOSalesDesc.Text;

            decimal.TryParse(TextBoxOSalesAmount.Text, out decimal OutOSalesAmt);
            e.Command.Parameters["@OtherSalesAmount"].Value = OutOSalesAmt;
        }

        protected void SqlDataSourceInvoiceLines_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            TextBoxOSalesDesc.Text = string.Empty;
            TextBoxOSalesAmount.Text = "0.00";

            if (e.AffectedRows > 0)
            {
                FormViewCPMInvoice.DataBind();
            }
        }

        protected void GridViewOtherSales_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Call Jquery which populates the totals
            RegisterDOMReadyScript("Page Load Recalculations", "RecalcAll();");
        }

        protected void GridViewOtherSales_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //Call Jquery which populates the totals
            RegisterDOMReadyScript("Page Load Recalculations", "RecalcAll();");
        }

        protected void GridViewOtherSales_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            //Call Jquery which populates the totals
            RegisterDOMReadyScript("Page Load Recalculations", "RecalcAll();");
            FormViewCPMInvoice.DataBind();

        }

        protected void GridViewOtherSales_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            //Call Jquery which populates the totals
            RegisterDOMReadyScript("Page Load Recalculations", "RecalcAll();");
            FormViewCPMInvoice.DataBind();
        }

        protected void LinkButtonMeals_Click(object sender, EventArgs e)
        {
            int BreakfastCount = 0;
            int LunchCount = 0;
            decimal AlaTotal = 0M;
            int SnackCount = 0;
            int DinnerCount = 0;

            HiddenField HFInvoiceProgYear = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFInvoiceProgYear");
            HiddenField HFInvoiceDistrictID = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFInvoiceDistrictID");

            TextBox TextBoxPeriodStart = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxPeriodStart");
            TextBox TextBoxPeriodEnd = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxPeriodEnd");
            TextBox TextBoxBfastCount = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxBfastCount");
            TextBox TextBoxLunchCount = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxLunchCount");
            TextBox TextBoxAlaSales = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxAlaSales");
            TextBox TextBoxSnackCount = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxSnackCount");
            TextBox TextBoxDinnerCount = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxDinnerCount");


            if (DateTime.TryParse(TextBoxPeriodStart.Text, out DateTime StartDate) == false)
                return;
            if (DateTime.TryParse(TextBoxPeriodEnd.Text, out DateTime EndDate) == false)
                return;

            GetRecapsTotals(HFInvoiceProgYear.Value, HFInvoiceDistrictID.Value, StartDate, EndDate, out BreakfastCount, out LunchCount, out AlaTotal, out SnackCount, out DinnerCount);

            TextBoxBfastCount.Text = BreakfastCount.ToString();
            TextBoxLunchCount.Text = LunchCount.ToString();
            TextBoxAlaSales.Text = AlaTotal.ToString("F2");
            TextBoxSnackCount.Text = SnackCount.ToString();
            TextBoxDinnerCount.Text = DinnerCount.ToString();


            //RegisterDOMReadyScript("Pop", "$('#InventoryImportModal').modal('hide');RecalcAll();");
            RegisterDOMReadyScript("Pop", "RecalcAll();");

            UpdatePanelEditInvoice.Update();
        }

        #region CovidImport

        protected void CBSelect_CovidCheckedChanged(object sender, EventArgs e)
        {
            decimal SelectedTotal = TotalCovidGridview();
            LabelCovidTotal.Text = SelectedTotal.ToString("F2");
        }

        protected decimal TotalCovidGridview()
        {
            Decimal CovidTotal = 0M;

            foreach (GridViewRow gvr in GridviewCovidPurch.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    Label LabelInvoiceTotal = (Label)FindControlRecursive(gvr, "LabelInvoiceTotal");
                    CheckBox CBSelect = (CheckBox)FindControlRecursive(gvr, "CBSelect");
                    if (LabelInvoiceTotal != null && CBSelect != null && CBSelect.Checked)
                    {
                        //string totalString = LabelInvoiceTotal.Text.Remove(  0, 1);//remove the "$" symbol
                        if (Decimal.TryParse(LabelInvoiceTotal.Text, out Decimal outTotal))
                            CovidTotal += outTotal;
                    }
                }
            }

            return CovidTotal;
        }

        protected void LinkButtonCovidPurch_Click(object sender, EventArgs e)
        {
            //Modal opens for Covid Purchases Import
            //Refresh the Covid Purchases grid and prepopulate the dates

            TextBox TextBoxPeriodStart = (TextBox)FindControlRecursive(FormViewCPMInvoice, "TextBoxPeriodStart");
            HiddenField HFInvoiceDistrictID = (HiddenField)FindControlRecursive(FormViewCPMInvoice, "HFInvoiceDistrictID");

            HFCovidDistrictID.Value = HFInvoiceDistrictID.Value;

            DateTime StartDate;

            if (TextBoxPeriodStart == null)
                StartDate = DateTime.Now;
            else if (DateTime.TryParse(TextBoxPeriodStart.Text, out StartDate) == false)
                StartDate = DateTime.Now;

            DateTime MonthStart = new DateTime(StartDate.Year, StartDate.Month, 1);
            DateTime MonthEnd = MonthStart.AddMonths(1).AddDays(-1);


            if (TextBoxCovidStartDate.Text == "")
                TextBoxCovidStartDate.Text = MonthStart.ToShortDateString();
            if (TextBoxCovidEndDate.Text == "")
                TextBoxCovidEndDate.Text = MonthEnd.ToShortDateString();

            UpdateCovidDateRange(null, null);

            RegisterDOMReadyScript("show", "$('#CovidImportModal').modal('show');RecalcAll();");
            OverallPanel.Update();
        }

        protected void UpdateCovidDateRange(object sender, EventArgs e)
        {
            if (DateTime.TryParse(TextBoxCovidStartDate.Text, out DateTime startDate) == false)
                return;
            if (DateTime.TryParse(TextBoxCovidEndDate.Text, out DateTime endDate) == false)
                return;

            GridviewCovidPurch.DataBind();

            decimal SelectedTotal = TotalCovidGridview();
            LabelCovidTotal.Text = SelectedTotal.ToString("F2");
        }

        protected void LinkButtonProcessCovid_Click(object sender, EventArgs e)
        {

            SqlDataSourceInvoiceLines.Inserting += SqlDataSourceInvoiceLines_CovidInsert;
            SqlDataSourceInvoiceLines.Insert();
            SqlDataSourceInvoiceLines.Inserting -= SqlDataSourceInvoiceLines_CovidInsert;

            RegisterDOMReadyScript("Pop", "$('#CovidImportModal').modal('hide');RecalcAll();");

            UpdatePanelEditInvoice.Update();

        }

        private void SqlDataSourceInvoiceLines_CovidInsert(object sender, SqlDataSourceCommandEventArgs e)
        {
            e.Command.Parameters["@OtherSalesAmount"].Value = LabelCovidTotal.Text;//.Remove(  0, 1)remove the "$" symbol
            e.Command.Parameters["@OtherSalesDesc"].Value = "Covid Purchases";
        }

        #endregion


    }
}
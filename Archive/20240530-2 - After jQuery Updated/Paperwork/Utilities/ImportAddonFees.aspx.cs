using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class ImportAddonFees : Paperwork_Global
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



                DDLDistricts.DataBind();
                if (DDLDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (DDLDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DDLDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                    else
                        DDLDistricts.SelectedIndex = 0;
                }
            }
            else
            {

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
                HFProgramYearStartDate.Value = ((DateTime)drvSql["StartDate"]).ToShortDateString();
                HFProgramYearEndDate.Value = ((DateTime)drvSql["EndDate"]).ToShortDateString();
                LabelBudgetYear.Text = drvSql["BudgetYear"].ToString();
            }

            //If Budget Year is not setup
            if (string.IsNullOrEmpty(LabelBudgetYear.Text))
            {
                LabelBudgetYear.Text = "* Budget Year has not be setup for selected Program Year *";
                LabelBudgetYear.ForeColor = System.Drawing.Color.Red;
                BtnProcess.Enabled = false;
            }
            else
            {
                LabelBudgetYear.ForeColor = System.Drawing.Color.Black;
                BtnProcess.Enabled = true;
            }

        }

        protected void BtnProcess_Click(object sender, EventArgs e)
        {
            TextBoxResults.Text = string.Empty;
            TextBoxResults.Rows = 0;
            TextBoxResults.ForeColor = System.Drawing.Color.Gray;
            TextBoxResults.Text = "Beginning import of Add-on Fees...";
            TextBoxResults.Rows++;
            TextBoxResults.Text += Environment.NewLine + "-------------------------------------";
            TextBoxResults.Rows++;

            foreach (GridViewRow gvr in GridViewDistBDGTTotals.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    string distID = GridViewDistBDGTTotals.DataKeys[gvr.RowIndex].Values["BidNumber"].ToString();

                    StringBuilder sb = new StringBuilder();

                    sb.Append("District ID:");
                    sb.Append(distID);

                    if (DateTime.TryParse(HFProgramYearStartDate.Value, out DateTime outPYStart) == false)
                        return;
                    if (DateTime.TryParse(HFProgramYearEndDate.Value, out DateTime outPYEnd) == false)
                        return;

                    //Create the Add-on Fee
                    InitializeAddOnFees(DDLProgramYears.SelectedValue, LabelBudgetYear.Text, distID, outPYStart, outPYEnd);


                    TextBoxResults.Text += Environment.NewLine + sb.ToString();
                    TextBoxResults.Rows++;

                }
            }

            TextBoxResults.Text += Environment.NewLine + "DONE";
            TextBoxResults.Rows++;
        }

        protected bool InitializeAddOnFees(string newProgramYear, string BudgetYear, string BidNumber, DateTime StartDate, DateTime EndDate)
        {
            bool AddOnFeesCreated = false;

            //Create new TaxRates Record
            string queryBDGTTotals = SqlDataSourceAddonFees.SelectCommand;

            //"SELECT BidNumber, AccountingNUTR, TechFeePlusSoftware, RecruitReplace, Liability, Training, WellnessFeeNUTR, CACFPAdminFeeTotal FROM [vwBDGT_Totals] " +
            //" WHERE BudgetYear=@BudgetYear AND BidNumber=@BidNumber AND BudgetType=2 AND ACTIVE=1 AND (AccountingNUTR+TechFeePlusSoftware+RecruitReplace+Liability+Training+WellnessFeeNUTR+CACFPAdminFeeTotal)<>0 ";


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

                        SqlParameter dbParam_BidNumber = new SqlParameter();
                        dbParam_BidNumber.ParameterName = "@BidNumber";
                        dbParam_BidNumber.Value = BidNumber;
                        dbParam_BidNumber.DbType = System.Data.DbType.StringFixedLength;
                        cmdReadBDGTFees.Parameters.Add(dbParam_BidNumber);

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

                            int[] BillingPeriods = new int[12];
                            int.TryParse(rdr["BillingJul"].ToString(), out BillingPeriods[0]);
                            int.TryParse(rdr["BillingAug"].ToString(), out BillingPeriods[1]);
                            int.TryParse(rdr["BillingSep"].ToString(), out BillingPeriods[2]);
                            int.TryParse(rdr["BillingOct"].ToString(), out BillingPeriods[3]);
                            int.TryParse(rdr["BillingNov"].ToString(), out BillingPeriods[4]);
                            int.TryParse(rdr["BillingDec"].ToString(), out BillingPeriods[5]);
                            int.TryParse(rdr["BillingJan"].ToString(), out BillingPeriods[6]);
                            int.TryParse(rdr["BillingFeb"].ToString(), out BillingPeriods[7]);
                            int.TryParse(rdr["BillingMar"].ToString(), out BillingPeriods[8]);
                            int.TryParse(rdr["BillingApr"].ToString(), out BillingPeriods[9]);
                            int.TryParse(rdr["BillingMay"].ToString(), out BillingPeriods[10]);
                            int.TryParse(rdr["BillingJun"].ToString(), out BillingPeriods[11]);

                            if (outAccountingNUTR != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outAccountingNUTR, StartDate, EndDate, "NUT", "InvProc", "ACCT", SchoolNum, BillingPeriods);
                            }

                            if (outTechFeePlusSoftware != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outTechFeePlusSoftware, StartDate, EndDate, "NUT", "TechFee", "COMTK", SchoolNum, BillingPeriods);
                            }

                            if (outRecruitReplace != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outRecruitReplace, StartDate, EndDate, "NUT", "RecruitFee", "EMPRC", SchoolNum, BillingPeriods);
                            }

                            if (outLiability != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outLiability, StartDate, EndDate, "NUT", "LiabFee", "INSLI", SchoolNum, BillingPeriods);
                            }

                            if (outTraining != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outTraining, StartDate, EndDate, "NUT", "Training", "TRNG", SchoolNum, BillingPeriods);
                            }

                            if (outWellnessNUTR != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outWellnessNUTR, StartDate, EndDate, "NUT", "Wellness", "WELNS", SchoolNum, BillingPeriods);
                            }

                            if (outCACFPAdminFeeTotal != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outCACFPAdminFeeTotal, StartDate, EndDate, "NUT", "CACFP", "CATER", SchoolNum, BillingPeriods);
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

            decimal monthlyInstallment = decimal.Round((AmountToDistribute / NumMonths), 2, MidpointRounding.AwayFromZero);

            for (int i = 0; i < NumMonths; i++)
            {
                DateTime currentMonthDate = startDate.AddMonths(i);
                DateTime postingDate = GetFinalPostingDateForMonth(currentMonthDate.Month, currentMonthDate.Year);
                PurchasesInsertRecord(districtID, postingDate, postingDate, VendorID, InvoiceNum, 0M, 0M, 0M, monthlyInstallment, OtherCat, "", 0M, SchoolNum, ProgramYear, ref tbStatus, true, 1, "", "School Year");
            }

            if (tbStatus != null)
                tbStatus.Dispose();
        }

        private void CreateMonthlyPurchEntries(string ProgramYear, string districtID, decimal AmountToDistribute, DateTime startDate, DateTime endDate, string VendorID, string InvoiceNum, string OtherCat, short SchoolNum, int[] BillingPeriods)
        {

            int NumPeriods = 0;
            foreach (int i in BillingPeriods)
            {
                NumPeriods += i;
            }
            TextBox tbStatus = new TextBox();

            if (NumPeriods <= 0)
                return;

            int monthInt = 0;
            foreach (int installmentMultiple in BillingPeriods)
            {
                decimal monthlyInstallment = decimal.Round(((AmountToDistribute / NumPeriods) * installmentMultiple), 2, MidpointRounding.AwayFromZero);
                //skip ahead for $0 installments
                if (monthlyInstallment == 0M)
                {
                    monthInt++;//need to increment the month before continuing
                    continue;
                }

                DateTime currentMonthDate = startDate.AddMonths(monthInt++);//increments the month after AddMonths runs
                DateTime postingDate = GetFinalPostingDateForMonth(currentMonthDate.Month, currentMonthDate.Year);
                PurchasesInsertRecord(districtID, postingDate, postingDate, VendorID, InvoiceNum, 0M, 0M, 0M, monthlyInstallment, OtherCat, "", 0M, SchoolNum, ProgramYear, ref tbStatus, true, 1, "", "School Year");
            }

            if (tbStatus != null)
                tbStatus.Dispose();

        }


        protected void SqlDataSourceDistricts_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            DDLDistricts.Items.Clear();
        }

        protected void GridViewDistBDGTTotals_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null)
            {
                string findDist = ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["BidNumber"].ToString();
                //If dist is not in the list of districts - hide the record
                if (DDLDistricts.Items.FindByValue(findDist) == null)
                    e.Row.Visible = false;
            }
        }

        protected void CBActiveOnly_CheckedChanged(object sender, EventArgs e)
        {
            ListItem storedLI = DDLDistricts.SelectedItem;
            DDLDistricts.DataBind();
            ListItem newLI = DDLDistricts.Items.FindByValue(storedLI.Value);
            if (newLI != null)
                newLI.Selected = true;
            else
                DDLDistricts.SelectedIndex = 0;

            GridViewDistBDGTTotals.DataBind();
        }
    }
}
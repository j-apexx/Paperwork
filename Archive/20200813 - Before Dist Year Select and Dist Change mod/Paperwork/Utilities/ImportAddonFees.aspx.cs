using System;
using System.Data;
using System.Data.SqlClient;
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
                        DDLDistricts.SelectedIndex=0;
                }
            }
            else
            {

            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            if( string.IsNullOrEmpty(LabelBudgetYear.Text))
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

            foreach(GridViewRow gvr in GridViewDistBDGTTotals.Rows)
            {
                if(gvr.RowType == DataControlRowType.DataRow)
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
            string queryBDGTTotals = "SELECT BidNumber, AccountingNUTR, TechFeePlusSoftware, RecruitReplace, Liability, Training, WellnessFeeNUTR, CACFPAdminFeeTotal FROM [vwBDGT_Totals] " +
                " WHERE BudgetYear=@BudgetYear AND BidNumber=@BidNumber AND BudgetType=2 AND ACTIVE=1 AND (AccountingNUTR+TechFeePlusSoftware+RecruitReplace+Liability+Training+WellnessFeeNUTR+CACFPAdminFeeTotal)<>0 ";


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

                            if (outAccountingNUTR != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outAccountingNUTR, StartDate, EndDate, "NUT", "InvProc", "ACCT", SchoolNum);
                            }

                            if (outTechFeePlusSoftware != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outTechFeePlusSoftware, StartDate, EndDate, "NUT", "TechFee", "COMTK", SchoolNum);
                            }

                            if (outRecruitReplace != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outRecruitReplace, StartDate, EndDate, "NUT", "RecruitFee", "EMPRC", SchoolNum);
                            }

                            if (outLiability != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outLiability, StartDate, EndDate, "NUT", "LiabFee", "INSLI", SchoolNum);
                            }

                            if (outTraining != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outTraining, StartDate, EndDate, "NUT", "Training", "TRNG", SchoolNum);
                            }

                            if (outWellnessNUTR != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outWellnessNUTR, StartDate, EndDate, "NUT", "Wellness", "WELNS", SchoolNum);
                            }

                            if (outCACFPAdminFeeTotal != 0M)
                            {
                                CreateMonthlyPurchEntries(newProgramYear, DistrictID, outCACFPAdminFeeTotal, StartDate, EndDate, "NUT", "CACFP", "CATER", SchoolNum);
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

            decimal monthlyInstallment = decimal.Round((AmountToDistribute / NumMonths), 2);

            for (int i = 0; i < NumMonths; i++)
            {
                DateTime currentMonthDate = startDate.AddMonths(i);
                DateTime postingDate = GetFinalPostingDateForMonth(currentMonthDate.Month, currentMonthDate.Year);
                PurchasesInsertRecord(districtID, postingDate, postingDate, VendorID, InvoiceNum, 0M, 0M, 0M, monthlyInstallment, OtherCat, "", 0M, SchoolNum, ProgramYear, ref tbStatus, true, 1);
            }
        }

        /// <summary>
        /// Retrieve the minimum school number based on the schoolnumber field. Find Active school if possible, otherwise return first found.
        /// </summary>
        /// <param name="DistrictID">District ID string</param>
        /// <returns>School Number ID</returns>
        private short GetMinSchool(string DistrictID)
        {

            short outSchoolNum = 0;
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
                        dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                        cmdReadSchools.Parameters.Add(dbParam_DistID);

                        SqlDataReader rdr = cmdReadSchools.ExecuteReader();

                        if (rdr.Read() && short.TryParse(rdr["SchoolNum"].ToString(), out short sclNum))
                        {
                            outSchoolNum = sclNum;

                            //if inactive, try to find an active one
                            if (rdr["Inactive"].ToString() == "1")
                            {
                                while (rdr.Read())
                                {
                                    if (rdr["Inactive"].ToString() == "0" && short.TryParse(rdr["SchoolNum"].ToString(), out short sclNumActive))
                                    {
                                        outSchoolNum = sclNumActive;
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



        protected void SqlDataSourceDistricts_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            DDLDistricts.Items.Clear();
        }

        protected void GridViewDistBDGTTotals_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null )
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
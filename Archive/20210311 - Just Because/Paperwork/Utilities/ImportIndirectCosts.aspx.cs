using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class ImportIndirectCosts : Paperwork_Global
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
            TextBoxResults.Text = "Beginning import of Indirect Costs...";
            TextBoxResults.Rows++;
            TextBoxResults.Text += Environment.NewLine + "-------------------------------------";
            TextBoxResults.Rows++;

            foreach(GridViewRow gvr in GridViewDistBDGTTotals.Rows)
            {
                if(gvr.RowType == DataControlRowType.DataRow)
                {
                    string distID = GridViewDistBDGTTotals.DataKeys[gvr.RowIndex].Values["DistrictID"].ToString();
                    decimal.TryParse(GridViewDistBDGTTotals.DataKeys[gvr.RowIndex].Values["DistrictTotal"].ToString(), out decimal distTotal);
                    StringBuilder sb = new StringBuilder();

                    sb.Append("District ID:");
                    sb.Append(distID);
                    
                    //TextBoxResults.Text += Environment.NewLine + distID + " : " + Math.Round( distTotal, 2).ToString("F2");
                    //TextBoxResults.Rows++;

                    //Create the Indirect Cost in setup if it doesn't exist
                    if(CreateIndirectSetup(distID))
                    {
                        sb.Append(", Created Setup Item");
                    }
                    else
                    {
                        sb.Append(", Could not create Setup Item**");
                        continue;
                    }

                    //Create the monthly Indirect Costs
                    if(CreateMonthlyIndSales(distID, distTotal))
                    {
                        sb.Append(", Created monthly indirect sales trxs.");
                    }
                    else
                    {
                        sb.Append(", Error creating monthly indirect sales trxs**.");
                    }

                    TextBoxResults.Text += Environment.NewLine + sb.ToString();
                    TextBoxResults.Rows++;

                }
            }

            TextBoxResults.Text += Environment.NewLine + "DONE";
            TextBoxResults.Rows++;
        }

        private bool CreateIndirectSetup(string distID)
        {
            bool setupCreated = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            string checkForIndSetup = "SELECT COUNT(*) FROM [OtherSalesCats] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND OtherSalesDesc=@OtherSalesDesc";
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand dbCommand = new SqlCommand(checkForIndSetup, dbConnection))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        SqlParameter dbParam_progYear = new SqlParameter();
                        dbParam_progYear.ParameterName = "@ProgramYear";
                        dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                        dbParam_progYear.DbType = System.Data.DbType.String;
                        dbCommand.Parameters.Add(dbParam_progYear);

                        SqlParameter dbParam_distID = new SqlParameter();
                        dbParam_distID.ParameterName = "@DistrictID";
                        dbParam_distID.Value = distID;
                        dbParam_distID.DbType = System.Data.DbType.String;
                        dbCommand.Parameters.Add(dbParam_distID);

                        SqlParameter dbParam_OSalesDesc = new SqlParameter();
                        dbParam_OSalesDesc.ParameterName = "@OtherSalesDesc";
                        dbParam_OSalesDesc.Value = TextBoxIndirectCosts.Text;
                        dbParam_OSalesDesc.DbType = System.Data.DbType.String;
                        dbCommand.Parameters.Add(dbParam_OSalesDesc);

                        dbConnection.Open();
                        Object oscCount = dbCommand.ExecuteScalar();
                        int.TryParse(oscCount.ToString(), out int oscInt);

                        //Setup record is already created - return true indicating as such
                        if (oscInt > 0)
                            return true;

                    }
                }

                using (SqlConnection dbConnection2 = new SqlConnection(connectionString))
                {
                    string createIndSetup = "INSERT INTO [OtherSalesCats] (ProgramYear, DistrictID, OtherSalesID, OtherSalesDesc) " +
                        "SELECT @ProgramYear, @DistrictID, " +
                        " (SELECT MAX(OtherSalesID+1) FROM [OtherSalesCats] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID)" +
                        ",@OtherSalesDesc ";
                    using (SqlCommand dbCommand2 = new SqlCommand(createIndSetup, dbConnection2))
                    {
                        dbCommand2.CommandType = CommandType.Text;

                        SqlParameter dbParam_progYear = new SqlParameter();
                        dbParam_progYear.ParameterName = "@ProgramYear";
                        dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                        dbParam_progYear.DbType = System.Data.DbType.String;
                        dbCommand2.Parameters.Add(dbParam_progYear);

                        SqlParameter dbParam_distID = new SqlParameter();
                        dbParam_distID.ParameterName = "@DistrictID";
                        dbParam_distID.Value = distID;
                        dbParam_distID.DbType = System.Data.DbType.String;
                        dbCommand2.Parameters.Add(dbParam_distID);

                        SqlParameter dbParam_OSalesDesc = new SqlParameter();
                        dbParam_OSalesDesc.ParameterName = "@OtherSalesDesc";
                        dbParam_OSalesDesc.Value = TextBoxIndirectCosts.Text;
                        dbParam_OSalesDesc.DbType = System.Data.DbType.String;
                        dbCommand2.Parameters.Add(dbParam_OSalesDesc);

                        int results = 0;
                        dbConnection2.Open();
                        results = dbCommand2.ExecuteNonQuery();
                        if (results > 0)
                            setupCreated = true;

                    }
                }
            }
            catch (SqlException ex)
            {
                RegisterDOMReadyScript("Error", "alert('SQL error: " + ex.Message + "');");
            }
            catch (Exception ex)
            {
                RegisterDOMReadyScript("Error", "alert('Error: " + ex.Message + "');");
            }

            return setupCreated;
        }

        private bool CreateMonthlyIndSales(string distID, decimal distTotal)
        {
            bool createdMonthlySales = false;
            decimal[] distAmounts = new decimal [12];
            decimal runningTotal = 0M;
            int recordCount = 0;

            if (DateTime.TryParse(HFProgramYearStartDate.Value, out DateTime ProgYrStartDate) == false)
                return false;

            if (DateTime.TryParse(HFProgramYearEndDate.Value, out DateTime ProgYrEndDate) == false)
                return false;

            //Assign monthly amounts
            for (int i=0; i<12;i++)
            {
                distAmounts[i] = Math.Round(distTotal / 12, 2);
                runningTotal += distAmounts[i];
            }

            //Account for any difference in rounding
            if (runningTotal != distTotal)
                distAmounts[11] += (distTotal - runningTotal);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            string createOtherSalesTrx = "INSERT INTO [OtherSales] (ProgramYear, DistrictID, OtherSalesDate, OtherSalesID, OtherSalesAmount, Locked) " +
                " VALUES(@ProgramYear, @DistrictID, @OtherSalesDate, " +
                " (SELECT OtherSalesID FROM [OtherSalesCats] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND OtherSalesDesc=@OtherSalesDesc)" +
                ", @OtherSalesAmount, 0)";

            try
            {
                using (SqlConnection dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Open();

                    //Assign monthly amounts
                    for (int i = 0; i < 12; i++)
                    {

                        using (SqlCommand dbCommand = new SqlCommand(createOtherSalesTrx, dbConnection))
                        {
                            dbCommand.CommandType = CommandType.Text;

                            dbCommand.Parameters.Clear();

                            SqlParameter dbParam_progYear = new SqlParameter();
                            dbParam_progYear.ParameterName = "@ProgramYear";
                            dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                            dbParam_progYear.DbType = System.Data.DbType.String;
                            dbCommand.Parameters.Add(dbParam_progYear);

                            SqlParameter dbParam_distID = new SqlParameter();
                            dbParam_distID.ParameterName = "@DistrictID";
                            dbParam_distID.Value = distID;
                            dbParam_distID.DbType = System.Data.DbType.String;
                            dbCommand.Parameters.Add(dbParam_distID);

                            SqlParameter dbParam_OSalesDesc = new SqlParameter();
                            dbParam_OSalesDesc.ParameterName = "@OtherSalesDesc";
                            dbParam_OSalesDesc.Value = TextBoxIndirectCosts.Text;
                            dbParam_OSalesDesc.DbType = System.Data.DbType.String;
                            dbCommand.Parameters.Add(dbParam_OSalesDesc);

                            SqlParameter dbParam_OSalesAmt = new SqlParameter();
                            dbParam_OSalesAmt.ParameterName = "@OtherSalesAmount";
                            dbParam_OSalesAmt.Value = distAmounts[i];
                            dbParam_OSalesAmt.DbType = System.Data.DbType.Decimal;
                            dbCommand.Parameters.Add(dbParam_OSalesAmt);

                            DateTime MonthEnd = new DateTime();

                            // Here's how I'm using the array with dates
                            // 0=Jul, 1=Aug, 2=Sep,...11=Jun
                            switch (i)
                            {
                                case 0://July
                                    MonthEnd = new DateTime(ProgYrStartDate.Year, 7, 31);
                                    break;
                                case 1://August
                                    MonthEnd = new DateTime(ProgYrStartDate.Year, 8, 31);
                                    break;
                                case 2://Sep
                                    MonthEnd = new DateTime(ProgYrStartDate.Year, 9, 30);
                                    break;
                                case 3://Oct
                                    MonthEnd = new DateTime(ProgYrStartDate.Year, 10, 31);
                                    break;
                                case 4://Nov
                                    MonthEnd = new DateTime(ProgYrStartDate.Year, 11, 30);
                                    break;
                                case 5://Dec
                                    MonthEnd = new DateTime(ProgYrStartDate.Year, 12, 31);
                                    break;
                                case 6://Jan
                                    MonthEnd = new DateTime(ProgYrEndDate.Year, 1, 31);
                                    break;
                                case 7://Feb
                                    MonthEnd = new DateTime(ProgYrEndDate.Year, 3, 1).AddDays(-1);//special handling for February Leap / Non-Leap years
                                    break;
                                case 8://Mar
                                    MonthEnd = new DateTime(ProgYrEndDate.Year, 3, 31);
                                    break;
                                case 9://Apr
                                    MonthEnd = new DateTime(ProgYrEndDate.Year, 4, 30);
                                    break;
                                case 10://May
                                    MonthEnd = new DateTime(ProgYrEndDate.Year, 5, 31);
                                    break;
                                case 11://Jun
                                    MonthEnd = new DateTime(ProgYrEndDate.Year, 6, 30);
                                    break;
                                default:
                                    continue;//if one of the cases are not matched, something happened - skip to the next iteration if there is one.
                                    break;
                            }

                            SqlParameter dbParam_OSalesDate = new SqlParameter();
                            dbParam_OSalesDate.ParameterName = "@OtherSalesDate";
                            dbParam_OSalesDate.Value = MonthEnd;
                            dbParam_OSalesDate.DbType = System.Data.DbType.DateTime;
                            dbCommand.Parameters.Add(dbParam_OSalesDate);

                            
                            Object oscCount = dbCommand.ExecuteScalar();
                            //int.TryParse(oscCount.ToString(), out int oscInt);
                            recordCount++;
                            if (recordCount == 12)
                                createdMonthlySales = true;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                RegisterDOMReadyScript("Error", "alert('SQL error: " + ex.Message + "');");
            }
            catch (Exception ex)
            {
                RegisterDOMReadyScript("Error", "alert('Error: " + ex.Message + "');");
            }

            

            return createdMonthlySales;
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
                string findDist = ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString();
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
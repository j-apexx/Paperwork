using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace Paperwork
{


    public partial class PayrollAccruals : Paperwork_Global
    {
        protected static string ExportPath = "";

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
        }

        protected void BtnUpdateAccruals_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;
            TextBoxStatus.Text = "Beginning update of Payroll Accruals...";
            TextBoxStatus.Rows++;

            if(AccrualDaysAreValid())
            {
                UpdatePayAndTaxAccruals();
            }

        }

        protected void UpdatePayAndTaxAccruals()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            string updateAccrualQuery = "UpdatePaytaxAccruals";
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand dbCommand = new SqlCommand(updateAccrualQuery, dbConnection))
                    {
                        dbCommand.CommandType = CommandType.StoredProcedure;

                        SqlParameter dbParam_startingDate = new SqlParameter();
                        dbParam_startingDate.ParameterName = "@BDt";
                        dbParam_startingDate.Value = TextBoxBeginningDate.Text;
                        dbParam_startingDate.DbType = System.Data.DbType.DateTime;
                        dbCommand.Parameters.Add(dbParam_startingDate);

                        SqlParameter dbParam_endingDate = new SqlParameter();
                        dbParam_endingDate.ParameterName = "@EDt";
                        dbParam_endingDate.Value = TextBoxEndingDate.Text;
                        dbParam_endingDate.DbType = System.Data.DbType.DateTime;
                        dbCommand.Parameters.Add(dbParam_endingDate);

                        int AccDays;
                        int.TryParse(TextBoxAccrualDaysCount.Text, out AccDays);

                        SqlParameter dbParam_AccrualOverride = new SqlParameter();
                        dbParam_AccrualOverride.ParameterName = "@AccrualDaysOverride";
                        dbParam_AccrualOverride.Value = AccDays;
                        dbParam_AccrualOverride.DbType = System.Data.DbType.Int32;
                        dbCommand.Parameters.Add(dbParam_AccrualOverride);

                        int outRowsUpdated = 0;

                        SqlParameter dbParam_rowsUpdated = new SqlParameter();
                        dbParam_rowsUpdated.ParameterName = "@RowsUpdated";
                        dbParam_rowsUpdated.DbType = System.Data.DbType.Int32;
                        dbParam_rowsUpdated.Direction = ParameterDirection.Output;
                        dbCommand.Parameters.Add(dbParam_rowsUpdated);

                        dbConnection.Open();
                        dbCommand.ExecuteNonQuery();

                        outRowsUpdated = (Int32)dbParam_rowsUpdated.Value;

                        TextBoxStatus.ForeColor = System.Drawing.Color.Green;
                        TextBoxStatus.Text += Environment.NewLine + "DONE" + 
                                              Environment.NewLine + outRowsUpdated.ToString() + " rows were updated.";
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                        
                    }
                }
            }
            catch (SqlException ex)
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                TextBoxStatus.Rows++;
            }
            catch (Exception ex)
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                TextBoxStatus.Rows++;
            }
            finally
            {
            }

        }

        /// <summary>
        /// Retrieves the Accrual days for the date range and checks to see if greater than 9
        /// </summary>
        /// <returns>Boolean indicating that Accrual Days was retrieved and is within the limit</returns>
        protected bool AccrualDaysAreValid()
        {
            Boolean AccrualDaysAreValid = false;

            int AccDays = retrieveAccrualDays();

            if (AccDays == null)
                return false;

            if (AccDays > 9)
            {
                RegisterDOMReadyScript("The number of accrual days is too large!", "alert('Check to see that all payrolls for the month have been imported before adding the accruals!');");
                AccrualDaysAreValid = false;
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + "The number of accrual days is too large!" +
                                    Environment.NewLine + "Check to see that all payrolls for the month have been imported before adding the accruals!";
                TextBoxStatus.Rows++;
                TextBoxStatus.Rows++;
            }
            else
                AccrualDaysAreValid = true;


            return AccrualDaysAreValid;
        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;

            //if both dates are entered - calculate the accrual days and populate the box
            if (TextBoxBeginningDate.Text!=string.Empty && TextBoxEndingDate.Text != string.Empty) 
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    int accDays = retrieveAccrualDays();
                    if (accDays == null)
                        accDays = 0;
                    TextBoxAccrualDaysCount.Text = accDays.ToString();
                }
            }
        }

        protected int retrieveAccrualDays()
        {
            int AccDays = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string AccrualDaysQuery = "SELECT Min(Accrual.AccruDays) AS MinOfAccruDays " +
                "FROM(SELECT DistrictID, " +
                    "DateDiff(D, PayTax.EndDate, EOMONTH(@EDt)) - (2 * (DatePart(WW, EOMONTH(@EDt)) - DatePart(WW,[EndDate]))) AS AccruDays " +
                    "FROM PayTax WHERE PayTax.EndDate Between @BDt And @EDt " +
                ") Accrual ";
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand dbCommand = new SqlCommand(AccrualDaysQuery, dbConnection))
                    {
                        SqlParameter dbParam_startingDate = new SqlParameter();
                        dbParam_startingDate.ParameterName = "@BDt";
                        dbParam_startingDate.Value = TextBoxBeginningDate.Text;
                        dbParam_startingDate.DbType = System.Data.DbType.DateTime;
                        dbCommand.Parameters.Add(dbParam_startingDate);

                        SqlParameter dbParam_endingDate = new SqlParameter();
                        dbParam_endingDate.ParameterName = "@EDt";
                        dbParam_endingDate.Value = TextBoxEndingDate.Text;
                        dbParam_endingDate.DbType = System.Data.DbType.DateTime;
                        dbCommand.Parameters.Add(dbParam_endingDate);

                        dbConnection.Open();
                        Object objAccDays = dbCommand.ExecuteScalar();
                        if (objAccDays != null)
                        {
                            if(int.TryParse(objAccDays.ToString(), out AccDays)==false)
                            {
                                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                                TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Accrual days could not be parsed for the given range.";
                                TextBoxStatus.Rows++;
                                TextBoxStatus.Rows++;
                            }

                        }
                        else
                        {
                            TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                            TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Accrual days could not be retrieved for the given range.";
                            TextBoxStatus.Rows++;
                            TextBoxStatus.Rows++;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                TextBoxStatus.Rows++;
            }
            catch (Exception ex)
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                TextBoxStatus.Rows++;
            }
            finally
            {
            }

            return AccDays;
        }
    }
}
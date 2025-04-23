using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportInsurance : Paperwork_Global
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

                InsDateFilter.Text = DateTime.Now.ToShortDateString();
                TextBoxInsuranceDate.Text = DateTime.Now.ToShortDateString();

                //Set the programyear
                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                else
                {
                    if (DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;

            DateTime checkTheDate;

            if (DateTime.TryParse(TextBoxInsuranceDate.Text, out checkTheDate) == false)
            {
                ShowDialog("Invalid Date", MessageType.Error, "Insurance Date is invalid.", "");
                return;
            }

            if (FileUpload1.PostedFile != null && !string.IsNullOrEmpty(FileUpload1.PostedFile.FileName))
            {
                try
                {
                    string path = string.Concat(Server.MapPath("~/UploadFile/" + FileUpload1.FileName));
                    FileUpload1.SaveAs(path);

                    //first line - no newline constant
                    TextBoxStatus.Text += "-File " + FileUpload1.FileName.ToLower() + " uploaded.";
                    TextBoxStatus.Rows++;

                    TextBoxStatus.Text += Environment.NewLine + "-Reading data.";
                    TextBoxStatus.Rows++;

                    //Read the contents of Tab Delimited file.
                    string csvData = File.ReadAllText(path);

                    Boolean importErrors = false;

                    int recordCount = 0;
                    int duplicateRecordCount = 0;
                    int zeroDollarRecordCount = 0;

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            Boolean DuplicateRecord = false;
                            Boolean SkippedDistrictRecord = false;
                            Boolean SkippedZeroDollarRecord = false;

                            if (row.IndexOf("Unit") >= 0 || row.IndexOf("\t\t") >= 0)
                            {
                                //skip header row, do not increment recordcount
                                //skip footer row which has double tab and total
                            }
                            else if (CreateImportRecord(row, out DuplicateRecord, out SkippedDistrictRecord, out SkippedZeroDollarRecord) == false)
                            {
                                recordCount++;

                                if (DuplicateRecord)
                                    duplicateRecordCount++;
                                else if (SkippedDistrictRecord)
                                {
                                    TextBoxStatus.Text += Environment.NewLine + "SKIPPED:  " + row.Replace("\r", "");
                                    TextBoxStatus.Rows++;
                                }
                                else
                                {
                                    //record creation failed - need to decide what to do
                                    TextBoxStatus.Text += Environment.NewLine + "-Import failed at the following record:";
                                    TextBoxStatus.Rows++;
                                    TextBoxStatus.Text += Environment.NewLine + "  " + row.Replace("\r", "");
                                    TextBoxStatus.Rows++;

                                    importErrors = true;
                                    break;//exit out of the foreach loop of records
                                }
                            }
                            else
                            {
                                recordCount++;
                                if (SkippedZeroDollarRecord)
                                    zeroDollarRecordCount++;
                            }


                        }
                    }

                    //Duplicate notification
                    if (duplicateRecordCount > 0)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-" + duplicateRecordCount.ToString() + " duplicate record" + (duplicateRecordCount == 1 ? " was" : "s were") + " detected & skipped.";
                        TextBoxStatus.Rows++;
                    }

                    //Zero dollar records
                    if (zeroDollarRecordCount > 0)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-" + zeroDollarRecordCount.ToString() + " record" + (zeroDollarRecordCount == 1 ? " was" : "s were") + " skipped which had $0 amounts.";
                        TextBoxStatus.Rows++;
                    }

                    //No Errors
                    if (importErrors == false)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + recordCount.ToString() + " record" + (recordCount == 1 ? "" : "s") + " from the import file. Import complete.";
                        TextBoxStatus.Rows++;
                        TextBoxStatus.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + recordCount.ToString() + " record" + (recordCount == 1 ? "" : "s") + " from the import file. **Errors caused the import to end**.";
                        TextBoxStatus.Rows++;
                        TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                    }

                    try
                    {
                        File.Delete(path);
                    }
                    catch { }//silent catch

                    //refresh the imported record grid
                    GridViewPayInsRecords.DataBind();

                }
                catch (InvalidOperationException ex)
                {
                    TextBoxStatus.Text += Environment.NewLine + "-File upload and import did not complete (Invalid Operation Exception).";
                    TextBoxStatus.Rows++;
                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    TextBoxStatus.Text += Environment.NewLine + "-File upload and import did not complete.";
                    TextBoxStatus.Rows++;
                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
            }
        }

        /// <summary>
        /// Parse the row string and create the Import Table record
        /// </summary>
        /// <param name="row">Row string with comma separated values</param>
        /// <returns>Boolean indicating if there were errors</returns>
        private Boolean CreateImportRecord(string row, out Boolean Duplicate, out Boolean SkippedDistrict, out Boolean SkippZeroDollar)
        {
            Boolean RecordCreated = false;
            Duplicate = false;
            SkippedDistrict = false;
            SkippZeroDollar = false;

            DateTime InsuranceDate;
            Decimal InsuranceTotal = 0M;
            String DistrictID = "";


            //Tab file - split with the tab character
            string[] rowArray = row.Split('\t');

            if (rowArray.Length == 1)
            {
                rowArray = row.Split(',');
            }

            //  Spec: Import Insurance Spec
            //	• Date1
            //	• Unit
            //	• Amount
            //  YrID

            //9/22/2021 - new file format is omitting the insurance date. Now provided field to specify
            if (DateTime.TryParse(TextBoxInsuranceDate.Text, out InsuranceDate) == false)
                return false;

            if (rowArray.Length == 4)
            {

                //if (DateTime.TryParse(rowArray[0], out InsuranceDate) == false)
                //    return false;

                DistrictID = rowArray[1].PadLeft(3, '0');

                if (decimal.TryParse(rowArray[2], out InsuranceTotal) == false)
                    return false;
            }
            else if (rowArray.Length == 3)//date is missing
            {
                //if (DateTime.TryParse(rowArray[0], out InsuranceDate) == false)
                //    return false;

                DistrictID = rowArray[0].PadLeft(3, '0');

                if (decimal.TryParse(rowArray[1], out InsuranceTotal) == false)
                    return false;
            }

            //if the district is special - summer (00113S, 00226R), skip
            if (DistrictID.Length > 3)
            {
                //per Tom - skip the S (SFSP) but include the R's
                if (DistrictID.IndexOf('R') > 0)
                {
                    DistrictID = DistrictID.Replace("R", "").TrimStart('0').PadLeft(3, '0');//Remove R, Remove leading zeros, pad with zeros as needed to create 3 char string
                }
                else
                {
                    SkippedDistrict = true;
                    return false;
                }

            }

            if (InsuranceTotal == 0M)
            {
                SkippZeroDollar = true;
                return true;
            }

            InsDateFilter.Text = InsuranceDate.ToShortDateString();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues;

            if (RecordExists(DDLProgramYears.SelectedValue, DistrictID, InsuranceDate))
                queryImportValues = "UPDATE [PayIns] SET [Amount]=[Amount] + @Amount WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [InsuranceDate]=@InvoiceDate ";
            else
                queryImportValues = "INSERT INTO [PayIns] ([ProgramYear],[DistrictID],[InsuranceDate],[Amount]) "
                                        + " VALUES(@ProgramYear,@DistrictID,@InvoiceDate,@Amount)";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
                dbParam_ProgYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = InsuranceDate;
                dbParam_InvDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_Total = new SqlParameter();
                dbParam_Total.ParameterName = "@Amount";
                dbParam_Total.Value = InsuranceTotal;
                dbParam_Total.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Total);

                try
                {
                    dbConnection.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        RecordCreated = true;
                    }
                    else
                        RecordCreated = false;

                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate") > 0)//Duplicate record 
                    {
                        /*do nothing - just skip and handle error*/
                        RecordCreated = false;
                        Duplicate = true;
                    }
                    else
                    {
                        RecordCreated = false;

                        if (CB_VerboseErrors.Checked)
                        {
                            TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                            TextBoxStatus.Rows++;
                            TextBoxStatus.Rows++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    RecordCreated = false;

                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return RecordCreated;
        }

        private bool RecordExists(string progYear, string districtID, DateTime insuranceDate)
        {
            bool recordExists = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryRecordExists = "Select count(*) FROM [PayIns] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [InsuranceDate]=@InvoiceDate";

            using (SqlCommand cmd = new SqlCommand(queryRecordExists, dbConnection))
            {
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = progYear;
                dbParam_ProgYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = districtID;
                dbParam_DistID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = insuranceDate;
                dbParam_InvDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_InvDate);

                try
                {
                    dbConnection.Open();
                    object objRecCount = cmd.ExecuteScalar();
                    if (objRecCount != null)
                    {
                        if (int.TryParse(objRecCount.ToString(), out int count) == true && count > 0)
                            recordExists = true;
                    }
                }
                catch (SqlException ex)
                {
                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
                finally
                {
                    dbConnection.Close();
                }


            }


            return recordExists;
        }

        protected void BtnImportFile_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#myModal').modal('show');", true);
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
        }
    }
}
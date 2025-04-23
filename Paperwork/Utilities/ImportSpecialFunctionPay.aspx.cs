using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportSpecialFunctionPay : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/default.aspx");

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

        protected void BtnImportFile_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#myModal').modal('show');", true);
        }

        protected void btnUpload_ClickCSV(object sender, EventArgs e)
        {
            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;

            if (FileUpload1.PostedFile != null)
            {
                try
                {

                    string path = string.Concat(Server.MapPath("~/UploadFile/" + FileUpload1.FileName));
                    FileUpload1.SaveAs(path);

                    TextBoxStatus.Text += Environment.NewLine + "-File " + FileUpload1.FileName.ToLower() + " uploaded.";
                    TextBoxStatus.Rows++;

                    TextBoxStatus.Text += Environment.NewLine + "-Reading data.";
                    TextBoxStatus.Rows++;

                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();

                    //Read the contents of CSV file.
                    string csvData = File.ReadAllText(path);

                    Boolean importErrors = false;

                    int recordCount = 0;
                    int duplicateRecordCount = 0;

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            //Skip the first record
                            if (recordCount > 0)
                            {
                                if (recordCount == 1)
                                {
                                    string rowCleaned = row.Replace("\"", string.Empty);
                                    string[] rowArray = rowCleaned.Split(',');
                                    int RowArrayLength = rowArray.Length;

                                }


                                if (CreateImportRecord(row) == false)
                                {
                                    //record creation failed - need to decide what to do
                                    TextBoxStatus.Text += Environment.NewLine + "-Import failed at the following record:";
                                    TextBoxStatus.Rows++;
                                    TextBoxStatus.Text += Environment.NewLine + "  " + row;
                                    TextBoxStatus.Rows++;

                                    importErrors = true;
                                    break;//exit out of the foreach loop of records
                                }
                                else
                                    recordCount++;
                            }
                            else
                                recordCount++;

                        }
                    }
                    sw.Stop();

                    //No Errors
                    if (importErrors == false)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + (recordCount - 1).ToString() + " records from the import file. Import complete.";
                        TextBoxStatus.Rows++;

                        TextBoxStatus.Text += Environment.NewLine + "-Elapsed Time " + String.Format(@"Time : {0:mm\:ss\.ff}", sw.Elapsed) + " seconds.";
                        TextBoxStatus.Rows++;


                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + (recordCount - 1).ToString() + " records from the import file. **Errors caused the import to end**.";
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Text += Environment.NewLine + "-Elapsed Time " + String.Format(@"Time : {0:mm\:ss\.ff}", sw.Elapsed) + " seconds.";
                        TextBoxStatus.Rows++;
                        TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                    }

                    GridViewSpFnPay.DataBind();

                    try
                    {
                        File.Delete(path);
                    }
                    catch { }//silent catch

                }
                finally { }
            }
        }

        private Boolean CreateImportRecord(string row)
        {
            Boolean RecordCreated = false;

            #region variables
            String FirstName = String.Empty;
            String LastName = String.Empty;
            String DistrictID;
            DateTime PayDate;
            Decimal PayAmount = 0M;
            #endregion

            string rowCleaned = row.Replace("\"", string.Empty);

            string[] rowArray = rowCleaned.Split(',');

            int RowArrayLength = rowArray.Length;

            if (RowArrayLength != 5 )
            {
                RecordCreated = false;
                if (CB_VerboseErrors.Checked)
                {
                    TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Number of columns in the import file does not match the number expected. Import will terminate.";
                    TextBoxStatus.Rows++;
                    TextBoxStatus.Rows++;
                }
                return RecordCreated;
            }

            
            try
            {   
                #region Assign Variable Values from row
                LastName = rowArray[0].ToUpper();
                FirstName = rowArray[1].ToUpper();
                DistrictID = rowArray[2];
                if(DistrictID.Length > 3)
                    DistrictID = DistrictID.Substring(DistrictID.Length - 3, 3);
                if (DateTime.TryParse(rowArray[3], out PayDate) == false)
                    PayDate = DateTime.Parse("1/1/1900");
                if (Decimal.TryParse(rowArray[4].Replace("$","").Replace("(", "-").Replace(")", "").Replace("/r",""), out PayAmount) == false) PayAmount = 0M;

                if(PayDate.ToShortDateString()=="1/1/1900" || PayAmount == 0M)
                {
                    RecordCreated = false;
                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Current row date could not be read or amount was zero.";
                        TextBoxStatus.Text += Environment.NewLine + rowCleaned;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                    return RecordCreated;
                }



                #endregion


                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
                SqlConnection dbConnection = new SqlConnection(connectionString);

                string queryImportValues = "INSERT INTO [SPFNPay] ([ProgramYear],[DistrictID],[PayDate],[SpFnAmount],[LastName],[FirstName],[ImportedDate],[ImportedUser])" +
                                            " VALUES(@ProgramYear,@DistrictID,@PayDate,@SpFnAmount,@LastName,@FirstName,CAST(GETDATE() AS DATE),@ImportedUser)";

                using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
                {
                    #region SQL Parameter creation and assignment

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

                    SqlParameter dbParam_PayDate = new SqlParameter();
                    dbParam_PayDate.ParameterName = "@PayDate";
                    dbParam_PayDate.Value = PayDate;
                    dbParam_PayDate.DbType = System.Data.DbType.Date;
                    cmd.Parameters.Add(dbParam_PayDate);

                    SqlParameter dbParam_SpFnAmt = new SqlParameter();
                    dbParam_SpFnAmt.ParameterName = "@SpFnAmount";
                    dbParam_SpFnAmt.Value = PayAmount;
                    dbParam_SpFnAmt.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_SpFnAmt);

                    SqlParameter dbParam_LastName = new SqlParameter();
                    dbParam_LastName.ParameterName = "@LastName";
                    dbParam_LastName.Value = LastName;
                    dbParam_LastName.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(dbParam_LastName);

                    SqlParameter dbParam_FirstName = new SqlParameter();
                    dbParam_FirstName.ParameterName = "@FirstName";
                    dbParam_FirstName.Value = FirstName;
                    dbParam_FirstName.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(dbParam_FirstName);

                    SqlParameter dbParam_ImpUser = new SqlParameter();
                    dbParam_ImpUser.ParameterName = "@ImportedUser";
                    dbParam_ImpUser.Value = Context.User.Identity.Name.ToUpper();
                    dbParam_ImpUser.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(dbParam_ImpUser);

                    #endregion

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

                        if (CB_VerboseErrors.Checked)
                        {
                            TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                            TextBoxStatus.Rows++;
                            TextBoxStatus.Rows++;
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

            }
            catch (IndexOutOfRangeException ex)
            {
                RecordCreated = false;
                if (CB_VerboseErrors.Checked)
                {
                    TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                    TextBoxStatus.Rows++;
                    TextBoxStatus.Rows++;
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

            return RecordCreated;
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
        }

    }
}
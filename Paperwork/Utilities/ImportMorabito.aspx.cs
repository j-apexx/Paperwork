using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportMorabito : Paperwork_Global
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

                DDLProgramYears_SelectedIndexChanged(null, null);
            }
        }

        /// <summary>
        /// Opens the modal import window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnImportFile_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#myModal').modal('show');", true);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            PreventMultipleClicks(ref ButtonUpld);

            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;

            if (FileUpload1.PostedFile != null && FileUpload1.PostedFile.FileName != string.Empty)
            {
                try
                {
                    //clear out the new temp table [IMPORT_PURCHASES]
                    ClearPURCHASESImportTable(HFVendorID.Value);
                    if(!isTRAINING())
                        CommissaryClearPURCHASESImportTable(HFVendorID.Value);

                    TextBoxStatus.Text += "-Temp tables cleared.";
                    TextBoxStatus.Rows++;


                    string path = string.Concat(Server.MapPath("~/UploadFile/" + FileUpload1.FileName));
                    FileUpload1.SaveAs(path);


                    TextBoxStatus.Text += Environment.NewLine + "-File " + FileUpload1.FileName.ToLower() + " uploaded.";
                    TextBoxStatus.Rows++;

                    TextBoxStatus.Text += Environment.NewLine + "-Reading data.";
                    TextBoxStatus.Rows++;

                    Boolean importErrors = false;

                    if (FileUpload1.PostedFile.FileName.ToUpper().Contains(".XLS"))
                    {
                        //Read contents of the XLSX file and create records in both Paperwork & CommissaryWeb
                        importErrors = ReadExcelFile(path);
                    }

                    //No Errors
                    if (importErrors == false)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + RecordsProcessed.ToString() + " Records from the import file.";
                        TextBoxStatus.Rows++;
                    }

                    try
                    {
                        File.Delete(path);
                    }
                    catch { }//silent catch

                    //Update the DistrictID and SchoolNums in [IMPORT_PURCHASES] Paperwork
                    TextBoxStatus.Text += Environment.NewLine + "-Updating the Paperwork Districts and SchoolNums in the import table.";
                    TextBoxStatus.Rows++;
                    UpdateImportDivisionsAndSchools(HFVendorID.Value);
                    AddMissingVendor(HFVendorID.Value);


                    if (!isTRAINING())
                    {
                        //Update the CommissaryID in [IMPORT_PURCHASES] CommissaryWeb
                        TextBoxStatus.Text += Environment.NewLine + "-Updating the CommisaryWeb Commissaries in the import table.";
                        TextBoxStatus.Rows++;
                        CommissaryUpdateImportDivisions(HFVendorID.Value);
                        CommissaryAddMissingVendor(HFVendorID.Value);
                    }

                    //Validate School and Districts
                    //Per Tom - no need to stop import for unmapped schools as files can contain commissaries - informational only
                    //Updates the textbox with the informational details
                    CheckForMissingDivSchools(HFVendorID.Value);

                    //****** If we are going to integrate with the Commissaries - do it here **
                    // Not needed to run this check for missing divs

                    //New 6/28/2021 - removing the non-program year records
                    int removedRecords = 0;
                    int removedCommissaryRecords = 0;

                    RemoveNonProgramYearRecords(HFVendorID.Value, out removedRecords);
                    if (!isTRAINING())
                    {
                        RemoveNonProgYearRecordsComm(HFVendorID.Value, out removedCommissaryRecords);
                    }

                    if ((removedRecords) > 0)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Skipped " + removedRecords.ToString() + " Paperwork invoices which fell outside program year dates.";
                        TextBoxStatus.Rows++;
                    }

                    if (!isTRAINING())
                    {
                        if ((removedCommissaryRecords) > 0)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Skipped " + (removedCommissaryRecords).ToString() + " Commissary invoices which fell outside program year dates.";
                            TextBoxStatus.Rows++;
                        }
                    }


                    //Finally - Create the Purch records
                    Int32 CountPurchRecords = 0;
                    Int32 CountDuplicatesSkipped = 0;


                    if (CreatePurchaseRecords(out CountPurchRecords, out CountDuplicatesSkipped) == true)
                    {
                        //if successful clear out the temp table
                        //Otherwise it may be useful for troubleshooting
                        ClearPURCHASESImportTable(HFVendorID.Value);

                        TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountPurchRecords.ToString() + " Paperwork Transmittal records were created";
                        TextBoxStatus.Rows++;

                        if(CountDuplicatesSkipped>0)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-                 " + CountDuplicatesSkipped.ToString() +
                                (CountDuplicatesSkipped==1? "Duplicate Invoice record was skipped" : " Duplicate Invoice records were skipped");
                            TextBoxStatus.Rows++;
                        }

                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountPurchRecords.ToString() + " Paperwork Transmittal records were created";
                        TextBoxStatus.Rows++;

                        if (CountDuplicatesSkipped > 0)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-                 " + CountDuplicatesSkipped.ToString() +
                                (CountDuplicatesSkipped == 1 ? "Duplicate Invoice record was skipped" : " Duplicate Invoice records were skipped");
                            TextBoxStatus.Rows++;
                        }
                    }

                    Int32 CountCommissaryPurchRecords = 0;
                    Int32 CountCommissaryDuplicatesSkipped = 0;

                    if (!isTRAINING())
                    {
                        if (CommissaryCreatePurchaseRecords(out CountCommissaryPurchRecords, out CountCommissaryDuplicatesSkipped) == true)
                        {
                            //if successful clear out the temp table in Commissary DB
                            //Otherwise it may be useful for troubleshooting
                            CommissaryClearPURCHASESImportTable(HFVendorID.Value);

                            TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountCommissaryPurchRecords.ToString() + " Commissary Transmittal records were created";
                            TextBoxStatus.Rows++;

                            if (CountCommissaryDuplicatesSkipped > 0)
                            {
                                TextBoxStatus.Text += Environment.NewLine + "-                 " + CountCommissaryDuplicatesSkipped.ToString() +
                                    (CountCommissaryDuplicatesSkipped == 1 ? "Duplicate Commissary Invoice record was skipped" : " Duplicate Commissary Invoice records were skipped");
                                TextBoxStatus.Rows++;
                            }

                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountCommissaryPurchRecords.ToString() + " Commissary Transmittal records were created";
                            TextBoxStatus.Rows++;

                            if (CountCommissaryDuplicatesSkipped > 0)
                            {
                                TextBoxStatus.Text += Environment.NewLine + "-                 " + CountCommissaryDuplicatesSkipped.ToString() +
                                    (CountCommissaryDuplicatesSkipped == 1 ? "Duplicate Commissary Invoice record was skipped" : " Duplicate Commissary Invoice records were skipped");
                                TextBoxStatus.Rows++;
                            }
                        }
                    }


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

        private bool ReadExcelFile(string _FileName)
        {
            bool spreadsheetHadErrors = false;
            string xLconnString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", _FileName);

            string SheetName = getFirstSheetName(xLconnString);

            var adapter = new OleDbDataAdapter("SELECT * FROM [" + SheetName + "]", xLconnString);
            var ds = new DataSet();

            adapter.Fill(ds, "BillingRecords");
            DataTable data = ds.Tables["BillingRecords"];

            int rowCount = data.Rows.Count;

            spreadsheetHadErrors = ParseBillingRecords(ref data);

            if (ds != null)
                ds.Dispose();

            if (adapter != null)
                adapter.Dispose();

            return spreadsheetHadErrors;
        }

        /// <summary>
        /// Checks the excel file schema and retrieves the first sheet title
        /// </summary>
        /// <returns>String containing the name of the first spreadsheet title</returns>
        private string getFirstSheetName(string xLconnString)
        {
            OleDbConnection objConn = null;
            DataTable dt = null;

            try
            {
                // Create connection object by using the connection string.
                objConn = new OleDbConnection(xLconnString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table containg the schema guid.
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                // Loop through all of the sheets if you want too...
                //for (int j = 0; j < excelSheets.Length; j++)
                //{
                //    // Query each excel sheet.
                //}

                // For now I'm just interested in the first one...
                return excelSheets[0];
            }
            catch (Exception ex)
            {
                ShowDialog("Error", MessageType.Error, "Error while reading excel file sheet name.", ex.Message);

                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }

        }

        private int RecordsProcessed = 0;

        private bool isPCN = false;
        private bool isNUTR = false;
        private bool ParseBillingRecords(ref DataTable data)
        {
            bool spreadsheetHadErrors = false;
            isPCN = false;
            isNUTR = false;

            foreach (DataRow row in data.Rows)
            {
                try
                {
                    Decimal InvoiceAmount = 0M;
                    DateTime InvoiceDate = new DateTime();
                    string InvoiceNumber = "";
                    Decimal FoodAmount = 0M;
                    Decimal PaperAmount = 0M;
                    Decimal CleanAmount = 0M;
                    Decimal OtherAmount = 0M;
                    Decimal MilkAmount = 0M;
                    string Unit = "";
                    int SchoolNum = 0;
                    string VendorCustomerNumber = "";
                    string VendorID = HFVendorID.Value;

                    //will be using import config at some point for this, setting manually for now
                    int colInvoiceAmount = 4;
                    int colInvoiceDate = 3;
                    int colInvoiceNumber = 0;
                    int colFoodAmount = 4;
                    int colPaperAmount = -1;
                    int colCleanAmount = -1;
                    int colOtherAmount = -1;
                    int colMilkAmount = -1;
                    int colUnit = -1;
                    int colSchoolNum = -1;
                    int colVendorCustomerNum = 1;

                    if(colInvoiceAmount>=0)
                    {
                        if (Decimal.TryParse(row.ItemArray[colInvoiceAmount].ToString(), out InvoiceAmount) == false || InvoiceAmount == 0M) continue;
                    }
                    if(colInvoiceDate>=0)
                    {
                        if (DateTime.TryParse(row.ItemArray[colInvoiceDate].ToString(), out InvoiceDate) == false) continue;
                    }
                    if(colInvoiceNumber>=0)
                    {
                        InvoiceNumber = row.ItemArray[colInvoiceNumber].ToString();
                    }
                    if(colFoodAmount>=0)
                    {
                        if (Decimal.TryParse(row.ItemArray[colFoodAmount].ToString(), out FoodAmount) == false ) continue;
                    }
                    if(colPaperAmount>=0)
                    {
                        if (Decimal.TryParse(row.ItemArray[colPaperAmount].ToString(), out PaperAmount)==false ) continue;
                    }
                    if(colCleanAmount>=0)
                    {
                        if (Decimal.TryParse(row.ItemArray[colCleanAmount].ToString(), out CleanAmount) == false ) continue;
                    }
                    if(colOtherAmount>=0)
                    {
                        if (Decimal.TryParse(row.ItemArray[colOtherAmount].ToString(), out OtherAmount) == false )continue;
                    }
                    if(colMilkAmount>=0)
                    {
                        if (Decimal.TryParse(row.ItemArray[colMilkAmount].ToString(), out MilkAmount) == false ) continue;
                    }
                    if(colUnit>=0)
                    {
                        Unit = row.ItemArray[colUnit].ToString();
                    }
                    if(colSchoolNum>=0)
                    {
                        if(int.TryParse(row.ItemArray[colSchoolNum].ToString(), out SchoolNum)==false) continue;
                    }
                    if(colVendorCustomerNum>=0)
                    {
                        VendorCustomerNumber = row.ItemArray[colVendorCustomerNum].ToString();
                    }

                    if (string.IsNullOrEmpty(InvoiceNumber)) continue;

                    RecordsProcessed++;

                    CreateImportRecord(InvoiceDate, InvoiceNumber, VendorCustomerNumber, InvoiceAmount, FoodAmount, PaperAmount, CleanAmount, OtherAmount, MilkAmount, Unit, SchoolNum, VendorID);
                    if (!isTRAINING())
                        CommissaryCreateImportRecord(InvoiceDate, InvoiceNumber, VendorCustomerNumber, InvoiceAmount, FoodAmount, PaperAmount, CleanAmount, OtherAmount, MilkAmount, Unit, SchoolNum, VendorID);

                }
                catch (Exception ex)
                {
                    TextBoxStatus.Text += Environment.NewLine + "-Error parsing record data in record " + RecordsProcessed.ToString() + " .";
                    TextBoxStatus.Rows++;
                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;

                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                    }

                    spreadsheetHadErrors = true;
                }
            }


            return spreadsheetHadErrors;
        }



        private void RemoveNonProgYearRecordsComm(string VendorID, out int removedCommissaryRecords)
        {
            removedCommissaryRecords = 0;

            if (DateTime.TryParse(HFProgYearStartDate.Value, out DateTime PyStartDate) == false)
                return;
            if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PyEndDate) == false)
                return;

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryRemoveNonPYTrxs = "DELETE FROM [IMPORT_PURCHASES] WHERE [CommissaryID]!='' AND ( [InvoiceDate]<@ProgramYearStartDate OR [InvoiceDate]>@ProgramYearEndDate) AND VendorID=@VendorID ";

            using (SqlCommand cmd = new SqlCommand(queryRemoveNonPYTrxs, dbConnection))
            {
                SqlParameter dbParam_pyStart = new SqlParameter();
                dbParam_pyStart.ParameterName = "@ProgramYearStartDate";
                dbParam_pyStart.Value = PyStartDate;
                dbParam_pyStart.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_pyStart);

                SqlParameter dbParam_pyEnd = new SqlParameter();
                dbParam_pyEnd.ParameterName = "@ProgramYearEndDate";
                dbParam_pyEnd.Value = PyEndDate;
                dbParam_pyEnd.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_pyEnd);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = VendorID;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    removedCommissaryRecords = cmd.ExecuteNonQuery();
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
        }

        private void RemoveNonProgramYearRecords(string VendorID, out int removedRecords)
        {
            removedRecords = 0;

            if (DateTime.TryParse(HFProgYearStartDate.Value, out DateTime PyStartDate) == false)
                return;
            if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PyEndDate) == false)
                return;


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryRemoveNonPYTrxs = "DELETE FROM [IMPORT_PURCHASES] WHERE DistrictID!='' AND SchoolNum!=-1 AND ([InvoiceDate]<@ProgramYearStartDate OR [InvoiceDate]>@ProgramYearEndDate) AND VendorID=@VendorID ";

            using (SqlCommand cmd = new SqlCommand(queryRemoveNonPYTrxs, dbConnection))
            {
                SqlParameter dbParam_pyStart = new SqlParameter();
                dbParam_pyStart.ParameterName = "@ProgramYearStartDate";
                dbParam_pyStart.Value = PyStartDate;
                dbParam_pyStart.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_pyStart);

                SqlParameter dbParam_pyEnd = new SqlParameter();
                dbParam_pyEnd.ParameterName = "@ProgramYearEndDate";
                dbParam_pyEnd.Value = PyEndDate;
                dbParam_pyEnd.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_pyEnd);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = VendorID;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    removedRecords = cmd.ExecuteNonQuery();
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
        }

        /// <summary>
        /// Create the record in the temp purchases import table
        /// </summary>
        /// <param name="InvDate"></param>
        /// <param name="InvNum"></param>
        /// <param name="VendorCustNum"></param>
        /// <param name="InvAmount"></param>
        /// <param name="FoodAmount"></param>
        /// <param name="PaperAmount"></param>
        /// <param name="CleanAmount"></param>
        /// <param name="OtherAmount"></param>
        /// <param name="MilkAmount"></param>
        /// <param name="DistrictID"></param>
        /// <param name="SchoolNum"></param>
        /// <param name="VendorID"></param>
        /// <returns>Boolean indicating record was created</returns>
        private Boolean CreateImportRecord(DateTime InvDate, string InvNum, string VendorCustNum,  Decimal InvAmount, Decimal FoodAmount, Decimal PaperAmount, Decimal CleanAmount, Decimal OtherAmount, Decimal MilkAmount, string DistrictID,  int SchoolNum, string VendorID)
        {
            Boolean RecordCreated = false;

            #region PostDate
            DateTime UsePostDate = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(TextBoxPostingDate.Text, out DateTime PostDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxMonthEndDate.Text, out DateTime PostDateLater) == false)
                return false;

            if (InvDate > PostDate)
                UsePostDate = PostDateLater;
            else
                UsePostDate = PostDate;
            #endregion

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [IMPORT_PURCHASES] ([InvoiceDate],[PostDate],[InvNum],[VendorCustNum],[InvTotal],[Food],[Paper],[Clean],[Other],[Milk],[DistrictID],[SchoolNum],[VendorID]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@VendorCustNum,@InvTotal,@Food,@Paper,@Clean,@Other,@Milk,@DistrictID,@SchoolNum,@VendorID)";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = InvDate;
                dbParam_InvDate.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_PostDate = new SqlParameter();
                dbParam_PostDate.ParameterName = "@PostDate";
                dbParam_PostDate.Value = UsePostDate;
                dbParam_PostDate.DbType = DbType.DateTime;
                cmd.Parameters.Add(dbParam_PostDate);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvNum";
                dbParam_InvNum.Value = InvNum;
                dbParam_InvNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_CustNum = new SqlParameter();
                dbParam_CustNum.ParameterName = "@VendorCustNum";
                dbParam_CustNum.Value = VendorCustNum;
                dbParam_CustNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_CustNum);

                SqlParameter dbParam_InvTotal = new SqlParameter();
                dbParam_InvTotal.ParameterName = "@InvTotal";
                dbParam_InvTotal.Value = InvAmount;
                dbParam_InvTotal.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_InvTotal);

                SqlParameter dbParam_Food = new SqlParameter();
                dbParam_Food.ParameterName = "@Food";
                dbParam_Food.Value = FoodAmount;
                dbParam_Food.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Food);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = PaperAmount;
                dbParam_Paper.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Clean";
                dbParam_Clean.Value = CleanAmount;
                dbParam_Clean.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = OtherAmount;
                dbParam_Other.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_Milk = new SqlParameter();
                dbParam_Milk.ParameterName = "@Milk";
                dbParam_Milk.Value = MilkAmount;
                dbParam_Milk.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Milk);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchoolNum = new SqlParameter();
                dbParam_SchoolNum.ParameterName = "@SchoolNum";
                dbParam_SchoolNum.Value = SchoolNum;
                dbParam_SchoolNum.DbType = DbType.Int32;
                cmd.Parameters.Add(dbParam_SchoolNum);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = VendorID;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

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
                    //if (ex.Number == 2627)//Duplicate record 
                    //{
                    //    /*do nothing - just skip and handle error*/
                    //    RecordCreated = true;
                    //}
                    //else
                    //{
                        RecordCreated = false;

                        if (CB_VerboseErrors.Checked)
                        {
                            TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                            TextBoxStatus.Rows++;
                            TextBoxStatus.Rows++;
                        }
                    //}
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

        /// <summary>
        /// Clear out the Purchases Import SQL table for the specified vendor ID
        /// </summary>
        private void ClearPURCHASESImportTable(string VendID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "DELETE [IMPORT_PURCHASES] WHERE [VendorID]=@VendorID";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_VndID = new SqlParameter();
                dbParam_VndID.ParameterName = "@VendorID";
                dbParam_VndID.Value = VendID;
                dbParam_VndID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VndID);

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        /// <summary>
        /// Update the temporary import records with the proper DistrictID and SchoolNum
        /// </summary>
        private void UpdateImportDivisionsAndSchools(string VendorID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string VendorCustTable = "";
            string VendCustField = "";
            switch (VendorID)
            {
                case "MORBAK":
                    VendorCustTable = "[MorabitoCustomerNumbers]";
                    VendCustField = "[MorabitoCustNum]";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(VendorCustTable) || string.IsNullOrEmpty(VendCustField))
                return;

            string queryImportValues = "UPDATE [IMPORT_PURCHASES] SET DistrictID=VndCst.DistrictID, SchoolNum=VndCst.SchoolNum " +
                " FROM [IMPORT_PURCHASES] I " +
                " INNER JOIN "+ VendorCustTable + " VndCst ON VndCst."+ VendCustField + "=I.VendorCustNum" +
                " WHERE I.VendorID=@VendorID ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = VendorID;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL error during Dist and school mapping.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error during Dist and school mapping.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        /// <summary>
        /// Checks the import table for any records without a district or schoolnum. Returns boolean indicating if records are missing school/div
        /// </summary>
        /// <returns>Boolean indicating if there are any records without district or school specified</returns>
        protected Boolean CheckForMissingDivSchools(string VendorID)
        {
            Boolean missingDivSchools = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT DISTINCT 'CustNum:' + RTRIM(VendorCustNum) FROM IMPORT_PURCHASES WHERE DistrictID='' OR SchoolNum=0 AND VendorID=@VendorID";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = VendorID;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (missingDivSchools == false)
                        {
                            missingDivSchools = true;
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: The following Vendor Customer Numbers could not be resolved to a District ID and School Number.";
                            TextBoxStatus.Rows++;
                        }

                        TextBoxStatus.Text += Environment.NewLine + rdr[0].ToString();
                        TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    missingDivSchools = true;

                    TextBoxStatus.Text += Environment.NewLine + "-An error occurred during District and School validation.";
                    TextBoxStatus.Rows++;
                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;
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

            return missingDivSchools;
        }

        private void AddMissingVendor(string VendorID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = " INSERT INTO [DistrictVendors]([DistrictID],[VENDORID],[VendorExpenseCode]) " +
                " SELECT DISTINCT DistrictID, @VendorID, '' FROM [IMPORT_PURCHASES] " +
                " WHERE RTRIM(DistrictID) + @VendorID NOT IN (SELECT RTRIM(DistrictID)+RTRIM(VENDORID) FROM DistrictVendors) AND DistrictID!='' AND [IMPORT_PURCHASES].[VendorID]=@VendorID;";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_vndrID = new SqlParameter();
                dbParam_vndrID.ParameterName = "@VendorID";
                dbParam_vndrID.Value = VendorID;
                dbParam_vndrID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_vndrID);

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }


        /// <summary>
        /// Cycles through the import table records and creates records in the Purchases table 
        /// </summary>
        /// <returns></returns>
        protected Boolean CreatePurchaseRecords(out Int32 recordCount, out Int32 duplicatesSkipped)
        {
            Boolean success = false;
            recordCount = 0;
            duplicatesSkipped = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT * FROM IMPORT_PURCHASES WHERE DistrictID!='' AND SchoolNum!=-1 AND VendorID=@VendorID ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = HFVendorID.Value;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string _vndrID = HFVendorID.Value;

                        Decimal _milk = 0M;
                        Decimal _food = 0M;
                        Decimal _paper = 0M;
                        Decimal _clean = 0M;
                        Decimal _other = 0M;

                        _milk = (Decimal)rdr["Milk"];
                        _food = (Decimal)rdr["Food"];
                        _paper = (Decimal)rdr["Paper"];
                        _clean = (Decimal)rdr["Clean"];
                        _other = (Decimal)rdr["Other"];

                        if (PurchasesDoesInvExist(DDLProgramYears.SelectedValue, rdr["DistrictID"].ToString(), _vndrID, rdr["InvNum"].ToString()) == true)
                        {
                            duplicatesSkipped++;
                            continue;//read the next record
                        }

                        if (PurchasesDoesVendorExist(rdr["DistrictID"].ToString(), _vndrID) == false)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: District " + rdr["DistrictID"].ToString() + " does not have vendor: " + _vndrID + " setup. The transmittal was still created, and the vendor will need to be added for this district or corrected on the transmittal.";
                            TextBoxStatus.Rows++;
                        }

                        if (PurchasesInsertRecord(rdr["DistrictID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), _food
                            , _paper, _clean, _other, "NONE"/*OtherCat*/, ""/*Comments*/, _milk, (Int16)rdr["SchoolNum"], DDLProgramYears.SelectedValue
                            , ref TextBoxStatus, CB_VerboseErrors.Checked, 0, HFVendorID.Value + "IMPORTER", "School Year") == true)
                        {
                            recordCount++;
                        }

                    }

                    success = true;
                }
                catch (SqlException ex)
                {
                    success = false;

                    TextBoxStatus.Text += Environment.NewLine + "-An SQL error occurred while creating Purchasing records.";
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
                    success = false;

                    TextBoxStatus.Text += Environment.NewLine + "-An error occurred while creating Purchasing records.";
                    TextBoxStatus.Rows++;
                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;
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

            return success;
        }

        #region Commissary Procedures

        /// <summary>
        /// Create the record in the temp purchases import table in CommissaryWeb
        /// </summary>
        /// <param name="InvDate"></param>
        /// <param name="InvNum"></param>
        /// <param name="VendorCustNum"></param>
        /// <param name="InvAmount"></param>
        /// <param name="FoodAmount"></param>
        /// <param name="PaperAmount"></param>
        /// <param name="CleanAmount"></param>
        /// <param name="OtherAmount"></param>
        /// <param name="MilkAmount"></param>
        /// <param name="CommissaryID"></param>
        /// <param name="SchoolNum"></param>
        /// <param name="VendorID"></param>
        /// <returns>Boolean indicating record was created</returns>
        private Boolean CommissaryCreateImportRecord(DateTime InvDate, string InvNum, string VendorCustNum, Decimal InvAmount, Decimal FoodAmount, Decimal PaperAmount, Decimal CleanAmount, Decimal OtherAmount, Decimal MilkAmount, string CommissaryID, int SchoolNum, string VendorID)
        {
            Boolean RecordCreated = false;

            #region
            DateTime UsePostDate = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(TextBoxPostingDate.Text, out DateTime PostDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxMonthEndDate.Text, out DateTime PostDateLater) == false)
                return false;

            if (InvDate > PostDate)
                UsePostDate = PostDateLater;
            else
                UsePostDate = PostDate;
            #endregion
            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [IMPORT_PURCHASES] ([InvoiceDate],[PostDate],[InvNum],[VendorCustNum],[InvTotal],[Food],[Paper],[Clean],[Other],[Milk],[CommissaryID],[SchoolNum],[VendorID]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@VendorCustNum,@InvTotal,@Food,@Paper,@Clean,@Other,@Milk,@CommissaryID,@SchoolNum,@VendorID)";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = InvDate;
                dbParam_InvDate.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_PostDate = new SqlParameter();
                dbParam_PostDate.ParameterName = "@PostDate";
                dbParam_PostDate.Value = UsePostDate;
                dbParam_PostDate.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_PostDate);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvNum";
                dbParam_InvNum.Value = InvNum;
                dbParam_InvNum.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_CustNum = new SqlParameter();
                dbParam_CustNum.ParameterName = "@VendorCustNum";
                dbParam_CustNum.Value = VendorCustNum;
                dbParam_CustNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_CustNum);

                SqlParameter dbParam_InvTotal = new SqlParameter();
                dbParam_InvTotal.ParameterName = "@InvTotal";
                dbParam_InvTotal.Value = InvAmount;
                dbParam_InvTotal.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_InvTotal);

                SqlParameter dbParam_Food = new SqlParameter();
                dbParam_Food.ParameterName = "@Food";
                dbParam_Food.Value = FoodAmount;
                dbParam_Food.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Food);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = PaperAmount;
                dbParam_Paper.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Clean";
                dbParam_Clean.Value = CleanAmount;
                dbParam_Clean.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = OtherAmount;
                dbParam_Other.DbType = DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_Milk = new SqlParameter();
                dbParam_Milk.ParameterName = "@Milk";
                dbParam_Milk.Value = MilkAmount;
                dbParam_Milk.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Milk);

                SqlParameter dbParam_CommID = new SqlParameter();
                dbParam_CommID.ParameterName = "@CommissaryID";
                dbParam_CommID.Value = CommissaryID;
                dbParam_CommID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_CommID);

                SqlParameter dbParam_SchoolNum = new SqlParameter();
                dbParam_SchoolNum.ParameterName = "@SchoolNum";
                dbParam_SchoolNum.Value = SchoolNum;
                dbParam_SchoolNum.DbType = DbType.Int32;
                cmd.Parameters.Add(dbParam_SchoolNum);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = VendorID;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

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
                    //if (ex.Number == 2627)//Duplicate record - Tom said there may be duplicates in a file
                    //{
                    //    /*do nothing - just skip and handle error*/
                    //    RecordCreated = true;
                    //}
                    //else
                    //{
                        RecordCreated = false;

                        if (CB_VerboseErrors.Checked)
                        {
                            TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                            TextBoxStatus.Rows++;
                            TextBoxStatus.Rows++;
                        }
                    //}
                }
                catch (Exception ex)
                {
                    //RegisterDOMReadyScript("error message", "alert('Error saving the import record');");
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

        /// <summary>
        /// Update the temporary import records with the proper CommissaryID
        /// </summary>
        private void CommissaryUpdateImportDivisions(string VendorID)
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string VendorCustTable = "";
            string VendCustField = "";
            switch (VendorID)
            {
                case "MORBAK":
                    VendorCustTable = "[MorabitoCustomerNumbers]";
                    VendCustField = "[MorabitoCustNum]";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(VendorCustTable) || string.IsNullOrEmpty(VendCustField))
                return;

            string queryImportValues = "UPDATE [IMPORT_PURCHASES] SET CommissaryID=VndCst.CommissaryID " +
                " FROM [IMPORT_PURCHASES] I " +
                " INNER JOIN " + VendorCustTable + " VndCst ON VndCst." + VendCustField + "=I.VendorCustNum" +
                " WHERE I.VendorID=@VendorID ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = VendorID;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL error during Commissary mapping.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error during Commissary mapping.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        private void CommissaryAddMissingVendor(string VendorID)
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = " INSERT INTO [dbo].[CommissaryVendors]([CommissaryID],[VENDORID],[VendorExpenseCode]) " +
                " SELECT DISTINCT COMMISSARYID, @VendorID, '' FROM [dbo].[IMPORT_PURCHASES] " +
                " WHERE RTRIM(CommissaryID) + @VendorID NOT IN (SELECT RTRIM(COMMISSARYID)+RTRIM(VENDORID) FROM CommissaryVendors) AND CommissaryID!='' AND [IMPORT_PURCHASES].[VendorID]=@VendorID; ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_vndrID = new SqlParameter();
                dbParam_vndrID.ParameterName = "@VendorID";
                dbParam_vndrID.Value = VendorID;
                dbParam_vndrID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_vndrID);

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        /// <summary>
        /// Cycles through the import table records and creates records in the Commissary Purchases table 
        /// </summary>
        /// <returns></returns>
        protected Boolean CommissaryCreatePurchaseRecords(out Int32 recordCount, out Int32 duplicatesSkipped)
        {
            Boolean success = false;
            recordCount = 0;
            duplicatesSkipped = 0;

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT * FROM [IMPORT_PURCHASES] WHERE CommissaryID!='' AND VendorID=@VendorID ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = HFVendorID.Value;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string _vndrID = HFVendorID.Value; 
                        string _otherCat = ""; // 	OCIndex - if USDA Flag=1 then 37 else (if Other>0, then 24 else 1)

                        Decimal _milk = 0M;
                        Decimal _food = 0M;
                        Decimal _paper = 0M;
                        Decimal _clean = 0M;
                        Decimal _other = 0M;

                        _milk = (Decimal)rdr["Milk"];
                        _food = (Decimal)rdr["Food"];
                        _paper = (Decimal)rdr["Paper"];
                        _clean = (Decimal)rdr["Clean"];
                        _other = (Decimal)rdr["Other"];

                        if (CommissaryPurchasesDoesInvExist(rdr["CommissaryID"].ToString(), _vndrID, rdr["InvNum"].ToString()) == true)
                        {
                            duplicatesSkipped++;
                            continue;//read the next record
                        }


                        //If vendor id is blank, skip
                        //if (_vndrID != String.Empty)
                        //{
                            if (CommissaryPurchasesInsertRecord(rdr["CommissaryID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), _food
                                , _paper, _clean, _other, "NONE", "", ref TextBoxStatus, CB_VerboseErrors.Checked, HFVendorID.Value + "IMPORTER", _milk, 0) == true)
                            {
                                recordCount++;
                            }
                        //}
                        //else
                        //{
                        //    TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: Could not create record for Commissary " + rdr["CommissaryID"].ToString() + ", could not identify import vendor.";
                        //    TextBoxStatus.Rows++;
                        //}
                    }

                    success = true;
                }
                catch (SqlException ex)
                {
                    success = false;

                    TextBoxStatus.Text += Environment.NewLine + "-An SQL error occurred while creating Purchasing records.";
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
                    success = false;

                    TextBoxStatus.Text += Environment.NewLine + "-An error occurred while creating Purchasing records.";
                    TextBoxStatus.Rows++;
                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;
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

            return success;
        }

        /// <summary>
        /// Clear out the Purchases Import SQL table
        /// </summary>
        private void CommissaryClearPURCHASESImportTable(string VendID)
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "DELETE [IMPORT_PURCHASES] WHERE [VendorID]=@VendorID";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_VndID = new SqlParameter();
                dbParam_VndID.ParameterName = "@VendorID";
                dbParam_VndID.Value = VendID;
                dbParam_VndID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VndID);

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        #endregion

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            SqlDataSourceProgramYears.DataBind();

            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int recordIndex = dvSql.Find(DDLProgramYears.SelectedValue);
                if (recordIndex != -1)
                {
                    DataRowView drvSql = dvSql[recordIndex];
                    HFProgYearStartDate.Value = drvSql["StartDate"].ToString();
                    HFProgYearEndDate.Value = drvSql["EndDate"].ToString();


                }
                else
                {
                    HFProgYearStartDate.Value = "";
                    HFProgYearEndDate.Value = "";
                }
            }
            else
            {
                HFProgYearStartDate.Value = "";
                HFProgYearEndDate.Value = "";
            }

            TextBoxMonthEndDate.Text = string.Empty;
            TextBoxPostingDate.Text = string.Empty;
        }
    }
}
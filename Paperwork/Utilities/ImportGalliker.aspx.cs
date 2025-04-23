using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportGalliker : Paperwork_Global
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
                    //clear out the temp table [IMPORT_GALLIKER]
                    ClearGALLIKERImportTable();
                    if(!isTRAINING())
                        CommissaryClearGALLIKERImportTable();

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

                    //Update the DistrictID and SchoolNums in [IMPORT_GALLIKER] Paperwork
                    TextBoxStatus.Text += Environment.NewLine + "-Updating the Paperwork Districts and SchoolNums in the import table.";
                    TextBoxStatus.Rows++;
                    UpdateImportDivisionsAndSchools();
                    AddMissingVendor();


                    if (!isTRAINING())
                    {
                        //Update the CommissaryID in [IMPORT_GALLIKER] CommissaryWeb
                        TextBoxStatus.Text += Environment.NewLine + "-Updating the CommisaryWeb Commissaries in the import table.";
                        TextBoxStatus.Rows++;
                        CommissaryUpdateImportDivisions();
                        CommissaryAddMissingVendor();
                    }

                    //Validate School and Districts
                    //Per Tom - no need to stop import for unmapped schools as files can contain commissaries - informational only
                    //Updates the textbox with the informational details
                    CheckForMissingDivSchools();

                    //****** If we are going to integrate with the Commissaries - do it here **
                    // Not needed to run this check for missing divs

                    //New 6/28/2021 - removing the non-program year records
                    int removedRecords = 0;
                    int removedCommissaryRecords = 0;

                    RemoveNonProgramYearRecords(out removedRecords);
                    if (!isTRAINING())
                    {
                        RemoveNonProgYearRecordsComm(out removedCommissaryRecords);
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
                        ClearGALLIKERImportTable();

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
                            CommissaryClearGALLIKERImportTable();

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
            //changed Jun 2023
            //if (data.Columns[4].ColumnName== "PCN - Billing Recap" || data.Columns[5].ColumnName == "PCN - Billing Recap" || data.Columns[6].ColumnName == "PCN - Billing Recap")
            //    isPCN = true;
            //else if (data.Columns[4].ColumnName== "Nutrition - Billing Recap" || data.Columns[5].ColumnName == "Nutrition - Billing Recap" || data.Columns[6].ColumnName == "Nutrition - Billing Recap")
            //    isNUTR= true;

            //Changes Oct 2023
            if (data.Columns[4].ColumnName.Trim(' ') == "PCN" || data.Columns[5].ColumnName.Trim(' ') == "PCN" || data.Columns[6].ColumnName.Trim(' ') == "PCN")
                isPCN = true;
            else if (data.Columns[4].ColumnName.Trim(' ') == "NUT" || data.Columns[5].ColumnName.Trim(' ') == "NUT" || data.Columns[6].ColumnName.Trim(' ') == "NUT")
                isNUTR = true;

            int colDocDate = -1;
            int colSalesAmt = -1;
            int colBranchAcct = -1;
            int colReference = -1;

            //PCN and NUTR sheets are slightly different. Set the column indexes based on company.
            //UPDATE June 2023 - now sheet columns are the same
            //if(isPCN)
            //{
            //    colDocDate = 1;
            //    colSalesAmt = 2;
            //    colBranchAcct = 3;
            //    colReference = 5;
            //}
            //else 
            if(isNUTR || isPCN)
            {
                colDocDate = 0;
                colSalesAmt = 1;
                colBranchAcct = 2;
                colReference = 4;
            }
            else
            {
                spreadsheetHadErrors = true;
                ShowDialog("Unidentified Sheet", MessageType.Error, "Import could not identify which company the sheet contains based on the header line.", "");
                return true;
            }

            foreach (DataRow row in data.Rows)
            {
                try
                {
                    DateTime InvoiceDate = new DateTime();
                    Decimal InvoiceAmount = 0M;
                    int BranchAccount = 0;
                    string InvoiceNumber = "";

                    if (DateTime.TryParse(row.ItemArray[colDocDate].ToString(), out InvoiceDate) == false)
                        continue;

                    if (Decimal.TryParse(row.ItemArray[colSalesAmt].ToString(), out InvoiceAmount) == false || InvoiceAmount==0M)
                        continue;

                    if (int.TryParse(row.ItemArray[colBranchAcct].ToString(), out BranchAccount)==false || BranchAccount==0M) 
                        continue;

                    InvoiceNumber = row.ItemArray[colReference].ToString();

                    if (string.IsNullOrEmpty(InvoiceNumber))
                        continue;

                    RecordsProcessed++;

                    CreateImportRecord(InvoiceDate, InvoiceAmount, BranchAccount, InvoiceNumber);
                    if (!isTRAINING())
                        CommissaryCreateImportRecord(InvoiceDate, InvoiceAmount, BranchAccount, InvoiceNumber);

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



        private void RemoveNonProgYearRecordsComm(out int removedCommissaryRecords)
        {
            removedCommissaryRecords = 0;

            if (DateTime.TryParse(HFProgYearStartDate.Value, out DateTime PyStartDate) == false)
                return;
            if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PyEndDate) == false)
                return;

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryRemoveNonPYTrxs = "DELETE FROM [IMPORT_GALLIKER] WHERE [CommissaryID]!='' AND ( [InvoiceDate]<@ProgramYearStartDate OR [InvoiceDate]>@ProgramYearEndDate) ";

            using (SqlCommand cmd = new SqlCommand(queryRemoveNonPYTrxs, dbConnection))
            {
                SqlParameter dbParam_pyStart = new SqlParameter();
                dbParam_pyStart.ParameterName = "@ProgramYearStartDate";
                dbParam_pyStart.Value = PyStartDate;
                dbParam_pyStart.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_pyStart);

                SqlParameter dbParam_pyEnd = new SqlParameter();
                dbParam_pyEnd.ParameterName = "@ProgramYearEndDate";
                dbParam_pyEnd.Value = PyEndDate;
                dbParam_pyEnd.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_pyEnd);

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

        private void RemoveNonProgramYearRecords(out int removedRecords)
        {
            removedRecords = 0;

            if (DateTime.TryParse(HFProgYearStartDate.Value, out DateTime PyStartDate) == false)
                return;
            if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PyEndDate) == false)
                return;


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryRemoveNonPYTrxs = "DELETE FROM [IMPORT_GALLIKER] WHERE DistrictID!='' AND SchoolNum!=-1 AND ([InvoiceDate]<@ProgramYearStartDate OR [InvoiceDate]>@ProgramYearEndDate) ";

            using (SqlCommand cmd = new SqlCommand(queryRemoveNonPYTrxs, dbConnection))
            {
                SqlParameter dbParam_pyStart = new SqlParameter();
                dbParam_pyStart.ParameterName = "@ProgramYearStartDate";
                dbParam_pyStart.Value = PyStartDate;
                dbParam_pyStart.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_pyStart);

                SqlParameter dbParam_pyEnd = new SqlParameter();
                dbParam_pyEnd.ParameterName = "@ProgramYearEndDate";
                dbParam_pyEnd.Value = PyEndDate;
                dbParam_pyEnd.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_pyEnd);

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
        /// Parse the row string and create the Import Table record
        /// </summary>
        /// <param name="row">Row string with comma separated values</param>
        /// <returns>Boolean indicating if there were errors</returns>
        private Boolean CreateImportRecord(DateTime InvDate, Decimal InvAmount, int BranchAct, string InvNum)
        {
            Boolean RecordCreated = false;

            DateTime UsePostDate = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(TextBoxPostingDate.Text, out DateTime PostDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxMonthEndDate.Text, out DateTime PostDateLater) == false)
                return false;

            if (InvDate > PostDate)
                UsePostDate = PostDateLater;
            else
                UsePostDate = PostDate;


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [IMPORT_GALLIKER] ([InvoiceDate],[PostDate],[InvNum],[GLKRCustNum],[InvTotal],[Milk],[DistrictID],[SchoolNum]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@GLKRCustNum,@InvTotal,@Milk,'', 0 )";

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
                dbParam_CustNum.ParameterName = "@GLKRCustNum";
                dbParam_CustNum.Value = BranchAct;
                dbParam_CustNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_CustNum);

                SqlParameter dbParam_InvTotal = new SqlParameter();
                dbParam_InvTotal.ParameterName = "@InvTotal";
                dbParam_InvTotal.Value = InvAmount;
                dbParam_InvTotal.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_InvTotal);

                SqlParameter dbParam_Milk = new SqlParameter();
                dbParam_Milk.ParameterName = "@Milk";
                dbParam_Milk.Value = InvAmount;
                dbParam_Milk.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Milk);

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
                    if (ex.Number == 2627)//Duplicate record - Tom said there may be duplicates in a file
                    {
                        /*do nothing - just skip and handle error*/
                        RecordCreated = true;
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
        /// Clear out the Galliker Import SQL table
        /// </summary>
        private void ClearGALLIKERImportTable()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "TRUNCATE TABLE [IMPORT_GALLIKER]";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
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
        private void UpdateImportDivisionsAndSchools()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = " UPDATE [IMPORT_GALLIKER] SET DistrictID=G.DistrictID, SchoolNum=G.SchoolNum " +
                " FROM [IMPORT_GALLIKER] I " +
                " INNER JOIN [GLKRCustomerNumbers] G ON G.GLKRCustNum=I.GLKRCustNum; ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
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
        /// Checks the import table for any records without a district or schoolnum. Returns boolean indicating if records are missing school/div
        /// </summary>
        /// <returns>Boolean indicating if there are any records without district or school specified</returns>
        protected Boolean CheckForMissingDivSchools()
        {
            Boolean missingDivSchools = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT DISTINCT 'CustNum:' + RTRIM(CAST(GLKRCustNum AS VARCHAR(10))) FROM IMPORT_GALLIKER WHERE DistrictID='' OR SchoolNum=0";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (missingDivSchools == false)
                        {
                            missingDivSchools = true;
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: The following Galliker Customer Numbers could not be resolved to a District ID and School Number.";
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

        private void AddMissingVendor()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string vndrID = "";

            if (isPCN)
                vndrID = "PCNM";
            else if (isNUTR)
                vndrID = "GALL";
            else
                return;

            string queryImportValues = " INSERT INTO [dbo].[DistrictVendors]([DistrictID],[VENDORID],[VendorExpenseCode]) " +
                " SELECT DISTINCT DistrictID, @VendorID, '' FROM [dbo].[IMPORT_GALLIKER] " +
                " WHERE RTRIM(DistrictID) + @VendorID NOT IN (SELECT RTRIM(DistrictID)+RTRIM(VENDORID) FROM DistrictVendors) AND DistrictID!='';";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_vndrID = new SqlParameter();
                dbParam_vndrID.ParameterName = "@VendorID";
                dbParam_vndrID.Value = vndrID;
                dbParam_vndrID.DbType = System.Data.DbType.String;
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

            string queryImportValues = "SELECT * FROM IMPORT_GALLIKER WHERE DistrictID!='' AND SchoolNum!=-1 ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string _vndrID = "";

                        Decimal _milk = 0M;

                        _milk = (Decimal)rdr["Milk"];
                        if (isPCN)
                            _vndrID = "PCNM";
                        else if (isNUTR)
                            _vndrID = "GALL";

                        if (PurchasesDoesInvExist(DDLProgramYears.SelectedValue, rdr["DistrictID"].ToString(), _vndrID, rdr["InvNum"].ToString()) == true)
                        {
                            duplicatesSkipped++;
                            continue;//read the next record
                        }


                        //If the District does not have the Reinhart vendor setup - do not create the records
                        if (_vndrID != String.Empty)
                        {
                            if (PurchasesDoesVendorExist(rdr["DistrictID"].ToString(), _vndrID) == false)
                            {
                                TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: District " + rdr["DistrictID"].ToString() + " does not have vendor: " + _vndrID + " setup. The transmittal was still created, and the vendor will need to be added for this district or corrected on the transmittal.";
                                TextBoxStatus.Rows++;
                            }

                            if (PurchasesInsertRecord(rdr["DistrictID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), 0M/*food*/
                                , 0M/*paper*/, 0M/*clean*/, 0M/*other*/, "NONE"/*OtherCat*/, ""/*Comments*/, _milk, (Int16)rdr["SchoolNum"], DDLProgramYears.SelectedValue
                                , ref TextBoxStatus, CB_VerboseErrors.Checked, 0, "GALLIKERIMPORTER","School Year") == true)
                            {
                                recordCount++;
                            }
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: District " + rdr["DistrictID"].ToString() + " does not have a Galliker Milk vendor setup and was skipped.";
                            TextBoxStatus.Rows++;
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

        private string getVendorID(string DistrictID)
        {
            string ReturnVendorID = string.Empty;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT VendorID FROM DistrictVendors Where DistrictID=@DistrictID AND VendorID IN ('GALL')";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DistrictID;
                dbParam_distID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_distID);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    //Set return value to first record regardless of value
                    if (rdr.Read())
                    {
                        ReturnVendorID = rdr[0].ToString();
                    }

                    if (rdr != null)
                        rdr.Close();

                }
                catch (SqlException ex)
                {

                    //if (CB_VerboseErrors.Checked)
                    //{
                    //    TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                    //    TextBoxStatus.Rows++;
                    //    TextBoxStatus.Rows++;
                    //}
                }
                catch (Exception ex)
                {

                    //if (CB_VerboseErrors.Checked)
                    //{
                    //    TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                    //    TextBoxStatus.Rows++;
                    //    TextBoxStatus.Rows++;
                    //}
                }
                finally
                {
                    dbConnection.Close();
                }

            }

            return ReturnVendorID;
        }

        #region Commissary Procedures

        /// <summary>
        /// Parse the row string and create the Import Table record
        /// </summary>
        /// <param name="row">Row string with comma separated values</param>
        /// <returns>Boolean indicating if there were errors</returns>
        private Boolean CommissaryCreateImportRecord(DateTime InvDate, Decimal InvAmount, int BranchAct, string InvNum)
        {
            Boolean RecordCreated = false;

            DateTime UsePostDate = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(TextBoxPostingDate.Text, out DateTime PostDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxMonthEndDate.Text, out DateTime PostDateLater) == false)
                return false;

            if (InvDate > PostDate)
                UsePostDate = PostDateLater;
            else
                UsePostDate = PostDate;

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return false; }


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [IMPORT_GALLIKER] ([InvoiceDate],[PostDate],[InvNum],[GLKRCustNum],[InvTotal],[Milk],[CommissaryID],[SchoolNum]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@GLKRCustNum,@InvTotal,@Milk,'', 0 )";

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
                dbParam_CustNum.ParameterName = "@GLKRCustNum";
                dbParam_CustNum.Value = BranchAct;
                dbParam_CustNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_CustNum);

                SqlParameter dbParam_InvTotal = new SqlParameter();
                dbParam_InvTotal.ParameterName = "@InvTotal";
                dbParam_InvTotal.Value = InvAmount;
                dbParam_InvTotal.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_InvTotal);

                SqlParameter dbParam_Milk = new SqlParameter();
                dbParam_Milk.ParameterName = "@Milk";
                dbParam_Milk.Value = InvAmount;
                dbParam_Milk.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Milk);

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
                    if (ex.Number == 2627)//Duplicate record - Tom said there may be duplicates in a file
                    {
                        /*do nothing - just skip and handle error*/
                        RecordCreated = true;
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
        private void CommissaryUpdateImportDivisions()
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = " UPDATE [IMPORT_GALLIKER] SET CommissaryID=G.CommissaryID " +
                " FROM [IMPORT_GALLIKER] I " +
                " INNER JOIN [GLKRCustomerNumbers] G ON G.GLKRCustNum=I.GLKRCustNum; ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
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

        private void CommissaryAddMissingVendor()
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string vndrID = "";

            if (isPCN)
                vndrID = "PCNM";
            else if (isNUTR)
                vndrID = "GALL";
            else
                return;

            string queryImportValues = " INSERT INTO [dbo].[CommissaryVendors]([CommissaryID],[VENDORID],[VendorExpenseCode]) " +
                " SELECT DISTINCT COMMISSARYID, @VendorID, '' FROM [dbo].[IMPORT_GALLIKER] " +
                " WHERE RTRIM(CommissaryID) + @VendorID NOT IN (SELECT RTRIM(COMMISSARYID)+RTRIM(VENDORID) FROM CommissaryVendors) AND CommissaryID!=''; ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_vndrID = new SqlParameter();
                dbParam_vndrID.ParameterName = "@VendorID";
                dbParam_vndrID.Value = vndrID;
                dbParam_vndrID.DbType = System.Data.DbType.String;
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

            string queryImportValues = "SELECT * FROM [IMPORT_GALLIKER] WHERE CommissaryID!='' ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string _vndrID = "";//	VendorNum: IF USDAFLAG=1 then retrieve VendorNum for the unit where VendorID='REIUSD' for unit. Otherwise - retrieve vendornum for the other VendorID's
                        string _otherCat = ""; // 	OCIndex - if USDA Flag=1 then 37 else (if Other>0, then 24 else 1)

                        Decimal _milk = 0M;

                        if (isPCN)
                            _vndrID = "PCNM";
                        else if (isNUTR)
                            _vndrID = "GALL";

                        _milk = (Decimal)rdr["Milk"];

                        if (CommissaryPurchasesDoesInvExist(rdr["CommissaryID"].ToString(), _vndrID, rdr["InvNum"].ToString()) == true)
                        {
                            duplicatesSkipped++;
                            continue;//read the next record
                        }


                        //If vendor id is blank, skip
                        if (_vndrID != String.Empty)
                        {
                            if (CommissaryPurchasesInsertRecord(rdr["CommissaryID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), 0M/*food*/
                                , 0M/*paper*/, 0M/*clean*/, 0M/*other*/, "NONE", "", ref TextBoxStatus, CB_VerboseErrors.Checked, "GALLIKERIMPORTER", _milk, 0) == true)
                            {
                                recordCount++;
                            }
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: Could not create record for Commissary " + rdr["CommissaryID"].ToString() + ", could not identify import vendor.";
                            TextBoxStatus.Rows++;
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

        /// <summary>
        /// Clear out the Galliker Import SQL table
        /// </summary>
        private void CommissaryClearGALLIKERImportTable()
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "TRUNCATE TABLE [IMPORT_GALLIKER]";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
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
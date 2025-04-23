using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class ImportTransmittalsGeneric : Paperwork_Global
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

            if (Session["ProgramYear"]==null)
                Response.Redirect("~/Default.aspx");

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

                CommissaryRowOther.Visible = false;
                ToolboxRowOther.Visible = false;

                DDLProgramYears_SelectedIndexChanged(null, null);

                ShowHideCommissaryAndToolboxSettings();

            }
        }

        /// <summary>
        /// Opens the modal import window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnImportFile_Click(object sender, EventArgs e)
        {
            if (ValidateConfigSettings() == false)
                return;

            if (!CBPostDateFromImport.Checked && (string.IsNullOrEmpty(TextBoxPostDateSpecified.Text) || DateTime.TryParse(TextBoxPostDateSpecified.Text, out DateTime postDate) == false))
            {
                ShowDialog("Error", MessageType.Error, "Get Post Date from import is not checked and Post Date specified is missong or invalid.", "");
                TextBoxPostDateSpecified.Focus();
                return;
            }
            if (string.IsNullOrEmpty(TextBoxMonthEndDate.Text))
                TextBoxMonthEndDate.Text = TextBoxPostDateSpecified.Text;


            if (Page.IsValid)
                Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#myModal').modal('show');", true);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            PreventMultipleClicks(ref ButtonUpld);

            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;

            bool commWebConfigured = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null ? false : true;
            bool toolboxConfigured = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null ? false : true;


            if (FileUpload1.PostedFile != null && FileUpload1.PostedFile.FileName != string.Empty)
            {
                try
                {
                    //clear out the temp table [IMPORT_PURCHASES]
                    if(String.IsNullOrEmpty(TextBoxVendorIDSpecified.Text))
                    {
                        ShowDialog("Error", MessageType.Error, "Cannot run import without Vendor specified. Aborting process.", "");
                        return;
                    }
                    ClearPURCHASESImportTable(TextBoxVendorIDSpecified.Text);

                    if (commWebConfigured)
                    {
                        if (!isTRAINING() && CBContainsCommissaryRecords.Checked)
                            CommissaryClearPURCHASESImportTable(TextBoxVendorIDSpecified.Text);
                    }

                    if (toolboxConfigured)
                    {
                        if (!isTRAINING() && CBContainsToolboxRecords.Checked)
                            ToolboxClearPURCHASESImportTable(TextBoxVendorIDSpecified.Text);
                    }

                    TextBoxStatus.Text += "-Temp tables cleared.";
                    TextBoxStatus.Rows++;


                    string path = string.Concat(Server.MapPath("~/UploadFile/" + FileUpload1.FileName));
                    FileUpload1.SaveAs(path);


                    TextBoxStatus.Text += Environment.NewLine + "-File " + FileUpload1.FileName.ToLower() + " uploaded.";
                    TextBoxStatus.Rows++;

                    TextBoxStatus.Text += Environment.NewLine + "-Reading data.";
                    TextBoxStatus.Rows++;

                    Boolean importErrors = false;

                    DataTable importDT = new DataTable();

                    if (DDLFileType.SelectedValue == "CSV")
                    {
                        bool csvHadErrors = false;

                        //Read the contents of CSV file.
                        string csvData = File.ReadAllText(path);

                        //importDT = GetDataTableFromCsv(path, CBFileHasHeaders.Checked);
                        importDT = CreateDataTableFromCsv(csvData);

                        importErrors = ParseBillingRecords(ref importDT);

                    }
                    else if(DDLFileType.SelectedValue == "XLSX" && FileUpload1.PostedFile.FileName.ToUpper().Contains(".XLS"))
                    {
                        //Read contents of the XLSX file and create records in both Paperwork & CommissaryWeb
                        importDT = ReadExcelFile(path);

                        importErrors = ParseBillingRecords(ref importDT);
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
                    if (CBUseVendCustMap.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Updating the Paperwork Districts and SchoolNums.";
                        TextBoxStatus.Rows++;
                        UpdateImportDivisionsAndSchools(TextBoxVendorIDSpecified.Text);
                    }
                    AddMissingVendor(TextBoxVendorIDSpecified.Text);

                    if (commWebConfigured && !isTRAINING() && CBUseVendCustMap.Checked && CBContainsCommissaryRecords.Checked)
                    {
                        //Update the CommissaryID in [IMPORT_PURCHASES] CommissaryWeb
                        TextBoxStatus.Text += Environment.NewLine + "-Updating the CommisaryWeb Commissaries.";
                        TextBoxStatus.Rows++;
                        CommissaryUpdateImportDivisions(TextBoxVendorIDSpecified.Text);
                    }

                    if (toolboxConfigured && !isTRAINING() && CBUseVendCustMap.Checked && CBContainsToolboxRecords.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Updating the Toolbox Districts and Building Numbers.";
                        TextBoxStatus.Rows++;
                        ToolboxUpdateImportDivisionsAndSchools(TextBoxVendorIDSpecified.Text);
                    }

                    //Validate School and Districts
                    //Per Tom - no need to stop import for unmapped schools as files can contain commissaries - informational only
                    //Updates the textbox with the informational details
                    CheckForMissingDivSchools(TextBoxVendorIDSpecified.Text);

                    //****** If we are going to integrate with the Commissaries - do it here **
                    // Not needed to run this check for missing divs
                    // CommissaryCheckForMissingDivs();

                    //New 6/28/2021 - removing the non-program year records
                    int removedRecords = 0;
                    int removedCommissaryRecords = 0;
                    int removedToolboxRecords = 0;

                    RemoveNonProgramYearRecords(TextBoxVendorIDSpecified.Text, out removedRecords);
                    if (commWebConfigured && !isTRAINING() && CBContainsCommissaryRecords.Checked)
                    {
                        CommissaryRemoveNonProgYearRecords(TextBoxVendorIDSpecified.Text, out removedCommissaryRecords);
                    }
                    if (toolboxConfigured && !isTRAINING() && CBContainsToolboxRecords.Checked)
                    {
                        ToolboxRemoveNonProgramYearRecords(TextBoxVendorIDSpecified.Text, out removedToolboxRecords);
                    }


                    if ((removedRecords) > 0)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Skipped " + removedRecords.ToString() + " Paperwork invoices which fell outside program year dates.";
                        TextBoxStatus.Rows++;
                    }

                    if (commWebConfigured && !isTRAINING() && CBContainsCommissaryRecords.Checked)
                    {
                        if ((removedCommissaryRecords) > 0)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Skipped " + (removedCommissaryRecords).ToString() + " Commissary invoices which fell outside program year dates.";
                            TextBoxStatus.Rows++;
                        }
                    }

                    if (toolboxConfigured && !isTRAINING() && CBContainsToolboxRecords.Checked)
                    {
                        if ((removedToolboxRecords) > 0)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Skipped " + (removedToolboxRecords).ToString() + " Toolbox invoices which fell outside program year dates.";
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
                        ClearPURCHASESImportTable(TextBoxVendorIDSpecified.Text);

                        TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountPurchRecords.ToString() + " Paperwork Transmittal records were created";
                        TextBoxStatus.Rows++;

                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountPurchRecords.ToString() + " Paperwork Transmittal records were created";
                        TextBoxStatus.Rows++;
                    }

                    Int32 CountCommissaryPurchRecords = 0;
                    Int32 CountCommissaryDuplicatesSkipped = 0;

                    if (commWebConfigured && !isTRAINING() && CBContainsCommissaryRecords.Checked)
                    {
                        if (CommissaryCreatePurchaseRecords(out CountCommissaryPurchRecords, out CountCommissaryDuplicatesSkipped) == true)
                        {
                            //if successful clear out the temp table in Commissary DB
                            //Otherwise it may be useful for troubleshooting
                            CommissaryClearPURCHASESImportTable(TextBoxVendorIDSpecified.Text);

                            TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountCommissaryPurchRecords.ToString() + " Commissary Purchase records were created";
                            TextBoxStatus.Rows++;

                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountCommissaryPurchRecords.ToString() + " Commissary Purchase records were created";
                            TextBoxStatus.Rows++;
                        }
                    }

                    Int32 CountToolboxPurchRecords = 0;
                    Int32 CountToolboxDuplicatesSkipped = 0;

                    if (toolboxConfigured && !isTRAINING() && CBContainsToolboxRecords.Checked)
                    {
                        if (ToolboxCreatePurchaseRecords(out CountToolboxPurchRecords, out CountToolboxDuplicatesSkipped) == true)
                        {
                            //if successful clear out the temp table in Toolbox DB
                            //Otherwise it may be useful for troubleshooting
                            ToolboxClearPURCHASESImportTable(TextBoxVendorIDSpecified.Text);

                            TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountToolboxPurchRecords.ToString() + " Toolbox Purchase records were created";
                            TextBoxStatus.Rows++;

                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountToolboxPurchRecords.ToString() + " Toolbox Purchase records were created";
                            TextBoxStatus.Rows++;
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
        
        protected DataTable CreateDataTableFromCsv(string csvContents)
        {
            DataTable csvDT = new DataTable();
            csvDT.TableName = "Transmittals";
            int rowsSkipped = 0;
            //Execute a loop over the rows.
            foreach (string row in csvContents.Split('\n'))
            {
                if(CBFileHasHeaders.Checked && rowsSkipped==0) 
                {
                    rowsSkipped++;
                    continue;
                }

                string[] rowArray = row.Split(',');

                //ensure datatable has enough columns
                if (!string.IsNullOrEmpty(row) && csvDT.Columns.Count == 0)
                {
                    for (int i = 0; i < rowArray.Length; i++)
                    {
                        csvDT.Columns.Add();
                    }
                }
                else if(!string.IsNullOrEmpty(row) && csvDT.Columns.Count< rowArray.Length)
                {
                    int colCountDiff = rowArray.Length - csvDT.Columns.Count;
                    for (int i = 0; i < colCountDiff; i++)
                    {
                        csvDT.Columns.Add();
                    }
                }

                if (!string.IsNullOrEmpty(row))
                    csvDT.Rows.Add(rowArray);
            }


            return csvDT;
        }

        private DataTable ReadExcelFile(string _FileName)
        {
            bool spreadsheetHadErrors = false;
            string xLconnString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", _FileName);

            string SheetName = getFirstSheetName(xLconnString);

            var adapter = new OleDbDataAdapter("SELECT * FROM [" + SheetName + "]", xLconnString);
            var ds = new DataSet();

            adapter.Fill(ds, "BillingRecords");
            DataTable data = ds.Tables["BillingRecords"];

            //int rowCount = data.Rows.Count;

            //spreadsheetHadErrors = ParseBillingRecords(ref data);

            if (ds != null)
                ds.Dispose();

            if (adapter != null)
                adapter.Dispose();

            return data;
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
        private bool ParseBillingRecords(ref DataTable data)
        {
            bool datatableImportHadErrors = false;

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
                    string strOtherCat = "";
                    string strOtherCatComm = "";
                    string strOtherCatToolbox = "";
                    string strComments = "";
                    string strProgType = "";
                    string VendorID = TextBoxVendorIDSpecified.Text;

                    //column mappings
                    int colInvoiceDate = -1;
                    int colInvoiceNumber = -1;
                    int colFoodAmount = -1;
                    int colPaperAmount = -1;
                    int colCleanAmount = -1;
                    int colOtherAmount = -1;
                    int colMilkAmount = -1;
                    int colUnit = -1;
                    int colSchoolNum = -1;
                    int colVendorCustomerNum = -1;
                    int colOtherCat = -1;
                    int colComments = -1;
                    int colProgType = -1;

                    if (CBUseVendCustMap.Checked && int.TryParse(TextBoxVendCustCol.Text, out int intColVendorCustomerNum))
                    {
                        colVendorCustomerNum = intColVendorCustomerNum - 1;
                        if (colVendorCustomerNum >= 0)
                        {
                            VendorCustomerNumber = row.ItemArray[colVendorCustomerNum].ToString();
                        }
                    }
                    else if(!CBUseVendCustMap.Checked)
                    {
                        if (CBDistFromImport.Checked && int.TryParse(TextBoxDistCol.Text, out int intColDist))
                        {
                            colUnit = intColDist - 1;
                            if (colUnit >= 0)
                            {
                                Unit = row.ItemArray[colUnit].ToString();
                            }
                        }
                        else if(!CBDistFromImport.Checked)
                        {
                            Unit = TextBoxDistSpecified.Text;
                        }

                        if (CBSchoolNumFromImport.Checked && int.TryParse(TextBoxSchoolNumCol.Text, out int intColSchool))
                        {
                            colSchoolNum = intColSchool - 1;
                            if (colSchoolNum >= 0)
                            {
                                if (int.TryParse(row.ItemArray[colSchoolNum].ToString(), out SchoolNum) == false) continue;
                            }
                        }
                        else if(!CBSchoolNumFromImport.Checked)
                        {
                            if(CBUseDefaultSchool.Checked && !string.IsNullOrEmpty(Unit))
                            {
                                SchoolNum = GetMinSchool(Unit);
                            }
                            else if(!CBUseDefaultSchool.Checked)
                            {
                                if (int.TryParse(TextBoxSchoolNumSpecified.Text, out SchoolNum) == false) continue;
                            }
                        }
                    }

                    if (CBInvDateFromImport.Checked && int.TryParse(TextBoxInvDateCol.Text, out int intColInvoiceDate))
                    {
                        colInvoiceDate = intColInvoiceDate - 1;
                        if (colInvoiceDate >= 0)
                        {
                            if (DateTime.TryParse(row.ItemArray[colInvoiceDate].ToString(), out InvoiceDate) == false) continue;
                        }
                    }
                    else if(!CBInvDateFromImport.Checked)
                    {
                        if (DateTime.TryParse(TextBoxInvoiceDateSpecified.Text, out InvoiceDate) == false) continue;
                    }

                    if (CBInvNumFromImport.Checked && int.TryParse(TextBoxInvNumCol.Text, out int intColInvoiceNumber))
                    {
                        colInvoiceNumber = intColInvoiceNumber - 1;
                        if (colInvoiceNumber >= 0)
                        {
                            InvoiceNumber = row.ItemArray[colInvoiceNumber].ToString();
                        }
                    }
                    else if(!CBInvNumFromImport.Checked)
                    {
                        InvoiceNumber = TextBoxInvNumSpecified.Text;
                    }

                    if (CBFoodAmtFromImport.Checked && int.TryParse(TextBoxFoodAmtCol.Text, out int intColFoodAmount))
                    {
                        colFoodAmount = intColFoodAmount - 1;
                        if (colFoodAmount >= 0)
                        {
                            if (Decimal.TryParse(row.ItemArray[colFoodAmount].ToString(), out FoodAmount) == false) continue;
                        }
                    }
                    else if(!CBFoodAmtFromImport.Checked)
                    {
                        if (Decimal.TryParse(TextBoxFoodAmtSpecified.Text, out FoodAmount) == false) continue;
                    }

                    if (CBPaperAmtFromImport.Checked && int.TryParse(TextBoxPaperAmtCol.Text, out int intColPaperAmount))
                    {
                        colPaperAmount = intColPaperAmount - 1;
                        if (colPaperAmount >= 0)
                        {
                            if (Decimal.TryParse(row.ItemArray[colPaperAmount].ToString(), out PaperAmount) == false) continue;
                        }
                    }
                    else if(!CBPaperAmtFromImport.Checked)
                    {
                        if (Decimal.TryParse(TextBoxPaperAmtSpecified.Text, out PaperAmount) == false) continue;
                    }

                    if (CBCleaningAmtFromImport.Checked && int.TryParse(TextBoxCleanAmtCol.Text, out int intColCleanAmount))
                    {
                        colCleanAmount = intColCleanAmount - 1;
                        if (colCleanAmount >= 0)
                        {
                            if (Decimal.TryParse(row.ItemArray[colCleanAmount].ToString(), out CleanAmount) == false) continue;
                        }
                    }
                    else if(!CBCleaningAmtFromImport.Checked)
                    {
                        if (Decimal.TryParse(TextBoxCleanAmtSpecified.Text, out CleanAmount) == false) continue;
                    }

                    if (CBOtherAmtFromImport.Checked && int.TryParse(TextBoxOtherAmtCol.Text, out int intColOtherAmount))
                    {
                        colOtherAmount = intColOtherAmount - 1;
                        if (colOtherAmount >= 0)
                        {
                            if (Decimal.TryParse(row.ItemArray[colOtherAmount].ToString(), out OtherAmount) == false) continue;
                        }
                    }
                    else if(!CBOtherAmtFromImport.Checked)
                    {
                        if (Decimal.TryParse(TextBoxOtherAmtSpecified.Text, out OtherAmount) == false) continue;
                    }

                    if (CBMilkAmtFromImport.Checked && int.TryParse(TextBoxMilkAmtCol.Text, out int intColMilkAmount))
                    {
                        colMilkAmount = intColMilkAmount - 1;
                        if (colMilkAmount >= 0)
                        {
                            if (Decimal.TryParse(row.ItemArray[colMilkAmount].ToString(), out MilkAmount) == false) continue;
                        }
                    }
                    else if(!CBMilkAmtFromImport.Checked)
                    {
                        if (Decimal.TryParse(TextBoxMilkAmtSpecified.Text, out MilkAmount) == false) continue;
                    }

                    if (CBOtherCatFromImport.Checked && int.TryParse(TextBoxOtherCatCol.Text, out int intColCat))
                    {
                        colOtherCat = intColCat - 1;
                        if (colOtherCat >= 0)
                        {
                            strOtherCat = row.ItemArray[colOtherCat].ToString();
                            strOtherCatComm = strOtherCat;
                            strOtherCatToolbox = strOtherCat;
                        }
                    }
                    else if(!CBOtherCatFromImport.Checked)
                    {
                        strOtherCat = ddlOtherCats.SelectedValue;
                        strOtherCatComm = ddlOtherCatsComm.SelectedValue;
                        strOtherCatToolbox = ddlOtherCatsToolbox.SelectedValue;
                    }

                    if (CBPurchCommntsFromImport.Checked && int.TryParse(TextBoxPurchCmmntCol.Text, out int intColPurchCommnts))
                    {
                        colComments = intColPurchCommnts - 1;
                        if (colComments >= 0)
                        {
                            strComments = row.ItemArray[colComments].ToString();
                        }
                    }
                    else if(!CBPurchCommntsFromImport.Checked)
                    {
                        strComments = TextBoxPurchCmmntSpecified.Text;
                    }

                    if (CBProgramTypeFromImport.Checked && int.TryParse(TextBoxProgTypeCol.Text, out int intColProgramType))
                    {
                        colProgType = intColProgramType - 1;
                        if (colProgType >= 0)
                        {
                            strProgType = row.ItemArray[colProgType].ToString();
                        }
                    }
                    else if(!CBProgramTypeFromImport.Checked)
                    {
                        strProgType = DDLProgramType.SelectedValue;
                    }
                 
                    


                    InvoiceAmount = FoodAmount + MilkAmount + PaperAmount + CleanAmount + OtherAmount;

                    RecordsProcessed++;

                    if (string.IsNullOrEmpty(InvoiceNumber) || (CBUseVendCustMap.Checked && string.IsNullOrEmpty(VendorCustomerNumber)) || InvoiceAmount==0M) continue;
                    

                    CreateImportRecord(InvoiceDate, InvoiceNumber, VendorCustomerNumber, InvoiceAmount, FoodAmount, PaperAmount, CleanAmount, OtherAmount, MilkAmount, Unit, SchoolNum, VendorID, strOtherCat, strComments, strProgType);
                    if (!isTRAINING() && CBContainsCommissaryRecords.Checked)
                        CommissaryCreateImportRecord(InvoiceDate, InvoiceNumber, VendorCustomerNumber, InvoiceAmount, FoodAmount, PaperAmount, CleanAmount, OtherAmount, MilkAmount, Unit, SchoolNum, VendorID, strOtherCatComm, strComments);

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

                    datatableImportHadErrors = true;
                }
            }


            return datatableImportHadErrors;
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
        private Boolean CreateImportRecord(DateTime InvDate, string InvNum, string VendorCustNum, Decimal InvAmount, Decimal FoodAmount, Decimal PaperAmount, Decimal CleanAmount, 
            Decimal OtherAmount, Decimal MilkAmount, string DistrictID, int SchoolNum, string VendorID, string OtherCat, string Comments, string ProgType)
        {
            Boolean RecordCreated = false;

            #region PostDate
            DateTime UsePostDate = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(TextBoxPostDateSpecified.Text, out DateTime PostDate) == false)
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

            string queryImportValues = "INSERT INTO [IMPORT_PURCHASES] ([InvoiceDate],[PostDate],[InvNum],[VendorCustNum],[InvTotal],[Food],[Paper],[Clean],[Other],[Milk],[DistrictID],[SchoolNum],[VendorID],[OtherCat],[PurchComments],[ProgType]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@VendorCustNum,@InvTotal,@Food,@Paper,@Clean,@Other,@Milk,@DistrictID,@SchoolNum,@VendorID,@OtherCat,@PurchComments,@ProgType)";

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

                SqlParameter dbParam_OtherCat = new SqlParameter();
                dbParam_OtherCat.ParameterName = "@OtherCat";
                dbParam_OtherCat.Value = OtherCat;
                dbParam_OtherCat.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_OtherCat);

                SqlParameter dbParam_Comments = new SqlParameter();
                dbParam_Comments.ParameterName = "@PurchComments";
                dbParam_Comments.Value = Comments;
                dbParam_Comments.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_Comments);

                SqlParameter dbParam_ProgType = new SqlParameter();
                dbParam_ProgType.ParameterName = "@ProgType";
                dbParam_ProgType.Value = ProgType;
                dbParam_ProgType.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_ProgType);

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
        /// Update the temporary import records with the proper DistrictID and SchoolNum
        /// </summary>
        private void UpdateImportDivisionsAndSchools(string VendorID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "UPDATE [IMPORT_PURCHASES] SET DistrictID=VndCst.DistrictID, SchoolNum=IIF(CB_UseDefaultSchool=1, ISNULL(dbo.GetMinSchool(VndCst.DistrictID),0), VndCst.SchoolNum) " +
            " FROM [IMPORT_PURCHASES] I " +
            " INNER JOIN [VendorCustomerMappings] VndCst ON VndCst.Customernumber=I.VendorCustNum AND VndCst.VendorID=I.VendorID " +
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
                    if (rdr != null)
                        rdr.Close();
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
                dbParam_VendorID.Value = TextBoxVendorIDSpecified.Text;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string _vndrID = rdr["VendorID"].ToString();

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

                        string progType = rdr["ProgType"].ToString();
                        string purchComments = rdr["PurchComments"].ToString();
                        string otherCat = string.IsNullOrEmpty(rdr["OtherCat"].ToString().Trim()) ? "NONE": rdr["OtherCat"].ToString().Trim();

                        byte LockRecord = (byte)(CBLockRecords.Checked ? 1 : 0);

                        //If the District does not have the vendor setup - do not create the records
                        if (_vndrID != String.Empty)
                        {
                            if (PurchasesInsertRecord(rdr["DistrictID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), _food
                                , _paper, _clean, _other, otherCat, purchComments, _milk, (Int16)rdr["SchoolNum"], DDLProgramYears.SelectedValue
                                , ref TextBoxStatus, CB_VerboseErrors.Checked, LockRecord, TextBoxVendorIDSpecified.Text + "IMPORTER", progType) == true)
                            {
                                recordCount++;
                            }
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: District " + rdr["DistrictID"].ToString() + " does not have the vendor setup and was skipped.";
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

        private string getReiVendorID(string DistrictID, string PreferredREIVndr)
        {
            string ReturnVendorID = string.Empty;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT VendorID FROM DistrictVendors Where DistrictID=@DistrictID AND VendorID IN ('R','REI204','REIV','REIT')";

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

                    //Then, if the previously set value doesn't match the RFSDivision from the file - see if the District is setup with the match
                    //otherwise, we'll just pass back the Reinhart VendorID they are configured for
                    while (rdr.Read() && ReturnVendorID != PreferredREIVndr)
                    {
                        if (rdr[0].ToString() == PreferredREIVndr)
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


        #region Commissary Procedures

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

            string queryImportValues = "SELECT * FROM [IMPORT_PURCHASES] WHERE CommissaryID IN (SELECT CommissaryID FROM Commissaries) AND VendorID=@VendorID ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = TextBoxVendorIDSpecified.Text;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string _vndrID = TextBoxVendorIDSpecified.Text;
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

                        byte LockRecord = (byte)(CBLockRecords.Checked ? 1 : 0);

                        string purchComments = rdr["PurchComments"].ToString();
                        string otherCat = string.IsNullOrEmpty(rdr["OtherCat"].ToString().Trim()) ? "NONE" : rdr["OtherCat"].ToString().Trim();


                        if (CommissaryPurchasesInsertRecord(rdr["CommissaryID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), _food
                                , _paper, _clean, _other, otherCat, purchComments, ref TextBoxStatus, CB_VerboseErrors.Checked, _vndrID + "IMPORTER", _milk, LockRecord) == true)
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

        /// <summary>
        /// Update the temporary import records with the proper CommissaryID
        /// </summary>
        private void CommissaryUpdateImportDivisions(string VendorID)
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "UPDATE [IMPORT_PURCHASES] SET CommissaryID=VndCst.CommissaryID " +
                " FROM [IMPORT_PURCHASES] I " +
                " INNER JOIN [VendorCustomerMappings] VndCst ON VndCst.CustomerNumber=I.VendorCustNum AND VndCst.VendorID=I.VendorID " +
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

        private void CommissaryRemoveNonProgYearRecords(string VendorID, out int removedCommissaryRecords)
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
        private Boolean CommissaryCreateImportRecord(DateTime InvDate, string InvNum, string VendorCustNum, Decimal InvAmount, Decimal FoodAmount, Decimal PaperAmount,
            Decimal CleanAmount, Decimal OtherAmount, Decimal MilkAmount, string CommissaryID, int SchoolNum, string VendorID, string OtherCat, string Comments)
        {
            Boolean RecordCreated = false;

            #region
            DateTime UsePostDate = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(TextBoxPostDateSpecified.Text, out DateTime PostDate) == false)
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

            string queryImportValues = "INSERT INTO [IMPORT_PURCHASES] ([InvoiceDate],[PostDate],[InvNum],[VendorCustNum],[InvTotal],[Food],[Paper],[Clean],[Other],[Milk],[CommissaryID],[SchoolNum],[VendorID],[OtherCat],[PurchComments]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@VendorCustNum,@InvTotal,@Food,@Paper,@Clean,@Other,@Milk,@CommissaryID,@SchoolNum,@VendorID,@OtherCat,@PurchComments)";

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

                SqlParameter dbParam_OtherCat = new SqlParameter();
                dbParam_OtherCat.ParameterName = "@OtherCat";
                dbParam_OtherCat.Value = OtherCat;
                dbParam_OtherCat.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_OtherCat);

                SqlParameter dbParam_Comments = new SqlParameter();
                dbParam_Comments.ParameterName = "@PurchComments";
                dbParam_Comments.Value = Comments;
                dbParam_Comments.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_Comments);

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
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "COMMISSARY:" + ex.Message;
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


        #endregion

        #region Toolbox Procedures
        /// <summary>
        /// Cycles through the import table records and creates records in the Purchases table 
        /// </summary>
        /// <returns></returns>
        protected Boolean ToolboxCreatePurchaseRecords(out Int32 recordCount, out Int32 duplicatesSkipped)
        {
            Boolean success = false;
            recordCount = 0;
            duplicatesSkipped = 0;

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT * FROM IMPORT_PURCHASES WHERE DistrictID!='' AND SchoolNum!=-1 AND VendorID=@VendorID ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = TextBoxVendorIDSpecified.Text;
                dbParam_VendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_VendorID);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string _vndrID = rdr["VendorID"].ToString();

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

                        if (ToolboxPurchasesDoesInvExist(DDLProgramYears.SelectedValue, rdr["DistrictID"].ToString(), _vndrID, rdr["InvNum"].ToString()) == true)
                        {
                            duplicatesSkipped++;
                            continue;//read the next record
                        }

                        if (ToolboxPurchasesDoesVendorExist(rdr["DistrictID"].ToString(), _vndrID) == false)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-TOOLBOX INFO ONLY: District " + rdr["DistrictID"].ToString() + " does not have vendor: " + _vndrID + " setup. The transmittal was still created, and the vendor will need to be added for this district or corrected on the transmittal.";
                            TextBoxStatus.Rows++;
                        }

                        string progType = rdr["ProgType"].ToString();
                        string purchComments = rdr["PurchComments"].ToString();
                        string otherCat = string.IsNullOrEmpty(rdr["OtherCat"].ToString().Trim()) ? "NONE" : rdr["OtherCat"].ToString().Trim();

                        byte LockRecord = (byte)(CBLockRecords.Checked ? 1 : 0);

                        //If the District does not have the Reinhart vendor setup - do not create the records
                        if (_vndrID != String.Empty)
                        {
                            if (PurchasesInsertRecord(rdr["DistrictID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), _food
                                , _paper, _clean, _other, otherCat, purchComments, _milk, (Int16)rdr["SchoolNum"], DDLProgramYears.SelectedValue
                                , ref TextBoxStatus, CB_VerboseErrors.Checked, LockRecord, TextBoxVendorIDSpecified.Text + "IMPORTER", progType) == true)
                            {
                                recordCount++;
                            }
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-TOOLBOX INFO ONLY: District " + rdr["DistrictID"].ToString() + " does not have a vendor setup and was skipped.";
                            TextBoxStatus.Rows++;
                        }
                    }

                    success = true;
                }
                catch (SqlException ex)
                {
                    success = false;

                    TextBoxStatus.Text += Environment.NewLine + "-[TOOLBOX] An SQL error occurred while creating Purchasing records.";
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

                    TextBoxStatus.Text += Environment.NewLine + "-[TOOLBOX] An error occurred while creating Purchasing records.";
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
        /// Clear out the Purchases Import SQL table for the specified vendor ID
        /// </summary>
        private void ToolboxClearPURCHASESImportTable(string VendID)
        {
            if(System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"].ConnectionString;
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
        private void ToolboxUpdateImportDivisionsAndSchools(string VendorID)
        {
            if(System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "UPDATE [IMPORT_PURCHASES] SET DistrictID=VndCst.DistrictID, SchoolNum=IIF(CB_UseDefaultSchool=1, ISNULL(dbo.GetMinSchool(VndCst.DistrictID),0), VndCst.SchoolNum) " +
            " FROM [IMPORT_PURCHASES] I " +
            " INNER JOIN [VendorCustomerMappings] VndCst ON VndCst.Customernumber=I.VendorCustNum AND VndCst.VendorID=I.VendorID " +
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
                    ShowDialog("Error", MessageType.Error, "[TOOLBOX] SQL error during Dist and school mapping.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "[TOOLBOX] Error during Dist and school mapping.", ex.Message);
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
        private Boolean ToolboxCreateImportRecord(DateTime InvDate, string InvNum, string VendorCustNum, Decimal InvAmount, Decimal FoodAmount, Decimal PaperAmount, Decimal CleanAmount,
            Decimal OtherAmount, Decimal MilkAmount, string DistrictID, int SchoolNum, string VendorID, string OtherCat, string Comments, string ProgType)
        {
            Boolean RecordCreated = false;

            #region PostDate
            DateTime UsePostDate = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(TextBoxPostDateSpecified.Text, out DateTime PostDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxMonthEndDate.Text, out DateTime PostDateLater) == false)
                return false;

            if (InvDate > PostDate)
                UsePostDate = PostDateLater;
            else
                UsePostDate = PostDate;
            #endregion

            if (System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null) { return false; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [IMPORT_PURCHASES] ([InvoiceDate],[PostDate],[InvNum],[VendorCustNum],[InvTotal],[Food],[Paper],[Clean],[Other],[Milk],[DistrictID],[SchoolNum],[VendorID],[OtherCat],[PurchComments],[ProgType]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@VendorCustNum,@InvTotal,@Food,@Paper,@Clean,@Other,@Milk,@DistrictID,@SchoolNum,@VendorID,@OtherCat,@PurchComments,@ProgType)";

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

                SqlParameter dbParam_OtherCat = new SqlParameter();
                dbParam_OtherCat.ParameterName = "@OtherCat";
                dbParam_OtherCat.Value = OtherCat;
                dbParam_OtherCat.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_OtherCat);

                SqlParameter dbParam_Comments = new SqlParameter();
                dbParam_Comments.ParameterName = "@PurchComments";
                dbParam_Comments.Value = Comments;
                dbParam_Comments.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_Comments);

                SqlParameter dbParam_ProgType = new SqlParameter();
                dbParam_ProgType.ParameterName = "@ProgType";
                dbParam_ProgType.Value = ProgType;
                dbParam_ProgType.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_ProgType);

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
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Toolbox: " + ex.Message;
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
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Toolbox: " + ex.Message;
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

        private void ToolboxRemoveNonProgramYearRecords(string VendorID, out int removedRecords)
        {
            removedRecords = 0;

            if (DateTime.TryParse(HFProgYearStartDate.Value, out DateTime PyStartDate) == false)
                return;
            if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PyEndDate) == false)
                return;

            if(System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null) { return; }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"].ConnectionString;
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
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Toolbox: " + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + "Toolbox: " + ex.Message;
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
            TextBoxPostDateSpecified.Text = string.Empty;
        }
        protected void DDLProgramType_DataBound(object sender, EventArgs e)
        {
            DropDownList DDLProgramType = (DropDownList)sender;
            if (DDLProgramType == null)
                return;
            if (DDLProgramType.Items.Count > 0)
            {
                ListItem liSchoolYear = DDLProgramType.Items.FindByValue("School Year");
                if (liSchoolYear != null)
                    DDLProgramType.SelectedValue = liSchoolYear.Value;

            }

        }

        #region ShowHideImportSettings
        protected void CBCleaningAmtFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBCleaningAmtFromImport.Checked)
            {
                LabelCleanAmtCol.Visible = true;
                TextBoxCleanAmtCol.Visible = true;
                LabelCleanAmtSpecified.Visible = false;
                TextBoxCleanAmtSpecified.Visible = false;
            }
            else
            {
                LabelCleanAmtCol.Visible = false;
                TextBoxCleanAmtCol.Visible = false;
                LabelCleanAmtSpecified.Visible = true;
                TextBoxCleanAmtSpecified.Visible = true;
            }
        }

        protected void CBDistFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBDistFromImport.Checked)
            {
                LabelDistCol.Visible = true;
                TextBoxDistCol.Visible = true;
                LabelDistSpecified.Visible = false;
                TextBoxDistSpecified.Visible = false;
            }
            else
            {
                LabelDistCol.Visible = false;
                TextBoxDistCol.Visible = false;
                LabelDistSpecified.Visible = true;
                TextBoxDistSpecified.Visible = true;
            }
        }

        protected void CBFoodAmtFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBFoodAmtFromImport.Checked)
            {
                LabelFoodAmtCol.Visible = true;
                TextBoxFoodAmtCol.Visible = true;
                LabelFoodAmtSpecified.Visible = false;
                TextBoxFoodAmtSpecified.Visible = false;
            }
            else
            {
                LabelFoodAmtCol.Visible = false;
                TextBoxFoodAmtCol.Visible = false;
                LabelFoodAmtSpecified.Visible = true;
                TextBoxFoodAmtSpecified.Visible = true;
            }
        }

        protected void CBInvDateFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBInvDateFromImport.Checked)
            {
                LabelInvDateCol.Visible = true;
                TextBoxInvDateCol.Visible = true;
                LabelInvDateSpecified.Visible = false;
                TextBoxInvoiceDateSpecified.Visible = false;
            }
            else
            {
                LabelInvDateCol.Visible = false;
                TextBoxInvDateCol.Visible = false;
                LabelInvDateSpecified.Visible = true;
                TextBoxInvoiceDateSpecified.Visible = true;
            }
        }

        protected void CBInvNumFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBInvNumFromImport.Checked)
            {
                LabelInvNumCol.Visible = true;
                TextBoxInvNumCol.Visible = true;
                LabelInvNumSpecified.Visible = false;
                TextBoxInvNumSpecified.Visible = false;
            }
            else
            {
                LabelInvNumCol.Visible = false;
                TextBoxInvNumCol.Visible = false;
                LabelInvNumSpecified.Visible = true;
                TextBoxInvNumSpecified.Visible = true;
            }
        }

        protected void CBMilkAmtFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBMilkAmtFromImport.Checked)
            {
                LabelMilkAmtCol.Visible = true;
                TextBoxMilkAmtCol.Visible = true;
                LabelMilkAmtSpecified.Visible = false;
                TextBoxMilkAmtSpecified.Visible = false;
            }
            else
            {
                LabelMilkAmtCol.Visible = false;
                TextBoxMilkAmtCol.Visible = false;
                LabelMilkAmtSpecified.Visible = true;
                TextBoxMilkAmtSpecified.Visible = true;
            }
        }

        protected void CBOtherAmtFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBOtherAmtFromImport.Checked)
            {
                LabelOtherAmtCol.Visible = true;
                TextBoxOtherAmtCol.Visible = true;
                LabelOtherAmtSpecified.Visible = false;
                TextBoxOtherAmtSpecified.Visible = false;
            }
            else
            {
                LabelOtherAmtCol.Visible = false;
                TextBoxOtherAmtCol.Visible = false;
                LabelOtherAmtSpecified.Visible = true;
                TextBoxOtherAmtSpecified.Visible = true;
            }
        }

        protected void CBOtherCatFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBOtherCatFromImport.Checked)
            {
                LabelOtherCatCol.Visible = true;
                TextBoxOtherCatCol.Visible = true;
                LabelOtherCatSpecified.Visible = false;
                ddlOtherCats.Visible = false;
            }
            else
            {
                LabelOtherCatCol.Visible = false;
                TextBoxOtherCatCol.Visible = false;
                LabelOtherCatSpecified.Visible = true;
                ddlOtherCats.Visible = true;
            }
        }

        protected void CBPaperAmtFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBPaperAmtFromImport.Checked)
            {
                LabelPaperAmtCol.Visible = true;
                TextBoxPaperAmtCol.Visible = true;
                LabelPaperAmtSpecified.Visible = false;
                TextBoxPaperAmtSpecified.Visible = false;
            }
            else
            {
                LabelPaperAmtCol.Visible = false;
                TextBoxPaperAmtCol.Visible = false;
                LabelPaperAmtSpecified.Visible = true;
                TextBoxPaperAmtSpecified.Visible = true;
            }
        }

        protected void CBProgramTypeFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBProgramTypeFromImport.Checked)
            {
                LabelProgTypeCol.Visible = true;
                TextBoxProgTypeCol.Visible = true;
                LabelProgTypeSpecified.Visible = false;
                DDLProgramType.Visible = false;
            }
            else
            {
                LabelProgTypeCol.Visible = false;
                TextBoxProgTypeCol.Visible = false;
                LabelProgTypeSpecified.Visible = true;
                DDLProgramType.Visible = true;
            }
        }

        protected void CBPostDateFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBPostDateFromImport.Checked)
            {
                LabelPostDateCol.Visible = true;
                TextBoxPostDateCol.Visible = true;
                LabelPostDateSpecified.Visible = false;
                TextBoxPostDateSpecified.Visible = false;
            }
            else
            {
                LabelPostDateCol.Visible = false;
                TextBoxPostDateCol.Visible = false;
                LabelPostDateSpecified.Visible = true;
                TextBoxPostDateSpecified.Visible = true;
            }
        }

        protected void CBPurchCommntsFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBPurchCommntsFromImport.Checked)
            {
                LabelPurchCmmntCol.Visible = true;
                TextBoxPurchCmmntCol.Visible = true;
                LabelPurchCmmntSpecified.Visible = false;
                TextBoxPurchCmmntSpecified.Visible = false;
            }
            else
            {
                LabelPurchCmmntCol.Visible = false;
                TextBoxPurchCmmntCol.Visible = false;
                LabelPurchCmmntSpecified.Visible = true;
                TextBoxPurchCmmntSpecified.Visible = true;
            }
        }

        protected void CBSchoolNum_CheckedChanged(object sender, EventArgs e)
        {
            if (CBSchoolNumFromImport.Checked)
            {
                LabelSchoolNumCol.Visible = true;
                TextBoxSchoolNumCol.Visible = true;
                LabelSchoolNumSpecified.Visible = false;
                TextBoxSchoolNumSpecified.Visible = false;
                CBUseDefaultSchool.Visible = false;
                LiteralSchoolNumSpecified.Visible = false;
            }
            else
            {
                LabelSchoolNumCol.Visible = false;
                TextBoxSchoolNumCol.Visible = false;
                CBUseDefaultSchool.Visible = true;
                if (CBUseDefaultSchool.Checked)
                {
                    LabelSchoolNumSpecified.Visible = false;
                    TextBoxSchoolNumSpecified.Visible = false;
                    LiteralSchoolNumSpecified.Visible = false;
                }
                else
                {
                    LabelSchoolNumSpecified.Visible = true;
                    TextBoxSchoolNumSpecified.Visible = true;
                    LiteralSchoolNumSpecified.Visible = true;
                }
            }
        }

        protected void CBUseVendCustMap_CheckedChanged(object sender, EventArgs e)
        {
            if (CBUseVendCustMap.Checked)
            {
                LabelVendCustCol.Visible = true;
                TextBoxVendCustCol.Visible = true;
                divDistrict.Visible = false;
                divSchool.Visible = false;
            }
            else
            {
                LabelVendCustCol.Visible = false;
                TextBoxVendCustCol.Visible = false;
                divDistrict.Visible = true;
                divSchool.Visible = true;
            }
        }

        protected void CBVendorFromImport_CheckedChanged(object sender, EventArgs e)
        {
            if (CBVendorFromImport.Checked)
            {
                LabelVendorIDCol.Visible = true;
                TextBoxVendorIDCol.Visible = true;
                LabelVendorIDSpecified.Visible = false;
                TextBoxVendorIDSpecified.Visible = false;
            }
            else
            {
                LabelVendorIDCol.Visible = false;
                TextBoxVendorIDCol.Visible = false;
                LabelVendorIDSpecified.Visible = true;
                TextBoxVendorIDSpecified.Visible = true;
            }
        }

        protected void CBContainsCommissaryRecords_CheckedChanged(object sender, EventArgs e)
        {
            if (CBContainsCommissaryRecords.Checked && (CBOtherAmtFromImport.Checked || LabelOtherAmtSpecified.Text != "0"))
            {
                CommissaryRowOther.Visible = true;
            }
            else
            {
                CommissaryRowOther.Visible = false;
            }
        }
        protected void CBContainsToolboxRecords_CheckedChanged(object sender, EventArgs e)
        {
            if (CBContainsToolboxRecords.Checked && (CBOtherAmtFromImport.Checked || LabelOtherAmtSpecified.Text != "0"))
            {
                ToolboxRowOther.Visible = true;
            }
            else
            {
                ToolboxRowOther.Visible = false;
            }
        }

        #endregion

        #region Settings

        protected void IUpdate_Click(object sender, EventArgs e)
        {
            if (DDLConfigs.Items.Count == 0 || string.IsNullOrEmpty(DDLConfigs.SelectedValue))
                return;

            SaveUpdateConfigSettings(DDLConfigs.SelectedValue);
        }

        protected void ButtonSaveName_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(TextBoxConfigName.Text)==false)
            {
                SaveUpdateConfigSettings(TextBoxConfigName.Text);
            }

            DDLConfigs.DataBind();
            try
            {
                DDLConfigs.SelectedValue = TextBoxConfigName.Text;
            }
            catch { }

        }

        protected void ISaveAs_Click(object sender, EventArgs e)
        {
            //Open the popup to request config name
            TextBoxConfigName.Text = "";
            Page.ClientScript.RegisterStartupScript(GetType(), "modalBoxConfig", "$('#myModalConfigName').modal('show');", true);
            TextBoxConfigName.Focus();
        }

        protected void iDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DDLConfigs.SelectedValue) == false)
            {
                DeleteConfigSettings(DDLConfigs.SelectedValue);
            }
            Page.Response.Redirect(Page.Request.Url.ToString(), false);
        }

        protected void ShowHideCommissaryAndToolboxSettings()
        {

            bool commWebConfigured = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"] == null ? false : true;
            bool toolboxConfigured = System.Configuration.ConfigurationManager.ConnectionStrings["connToolbox"] == null ? false : true;

            if (commWebConfigured == false)
            {
                CBContainsCommissaryRecords.Enabled = false;
                CBContainsCommissaryRecords.Checked = false;
                CommissaryRow.Visible = false;
            }
            else
                CommissaryRow.Visible = true;

            if (toolboxConfigured == false)
            {
                CBUseVendCustMap.Enabled = false;
                CBUseVendCustMap.Checked = false;
                ToolboxRow.Visible = false;
            }
            else
                ToolboxRow.Visible = true;
        }

        protected void SaveUpdateConfigSettings(string ConfigName)
        {
            if(string.IsNullOrEmpty(ConfigName))
            {
                ShowDialog("Error", MessageType.Error, "Import Configuration name was empty.", "");
                return;
            }

            if (ValidateConfigSettings() == false)
                return;

            ShowHideCommissaryAndToolboxSettings();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "";

            if (DoesConfigExist(ConfigName))
                queryImportValues =
                    "UPDATE [PurchasesImportSettings] SET [CB_UseVendCustMapping]=@CB_UseVendCustMapping, [CB_GetDistFromImport]=@CB_GetDistFromImport, " +
                    "[CB_GetSchoolFromImport]=@CB_GetSchoolFromImport, [CB_GetInvoiceDateFromImport]=@CB_GetInvoiceDateFromImport, [CB_GetPostDateFromImport]=@CB_GetPostDateFromImport, " +
                    "[CB_GetVendorFromImport]=@CB_GetVendorFromImport, [CB_GetInvoiceNumFromImport]=@CB_GetInvoiceNumFromImport, [CB_GetFoodAmtFromImport]=@CB_GetFoodAmtFromImport, " +
                    "[CB_GetMilkAmtFromImport]=@CB_GetMilkAmtFromImport, [CB_GetPaperAmtFromImport]=@CB_GetPaperAmtFromImport, [CB_GetCleanAmtFromImport]=@CB_GetCleanAmtFromImport, " +
                    "[CB_GetOtherAmtFromimport]=@CB_GetOtherAmtFromimport, [CB_GetOtherCatFromImport]=@CB_GetOtherCatFromImport, [CB_GetCommentsFromImport]=@CB_GetCommentsFromImport, " +
                    "[CB_LockRecords]=@CB_LockRecords, [CB_GetProgTypeFromImport]=@CB_GetProgTypeFromImport, [CB_ImportHasHeaders]=@CB_ImportHasHeaders, [ColmapCustomerNumber]=@ColmapCustomerNumber, " +
                    "[ColmapDistrict]=@ColmapDistrict, [ColmapSchoolNum]=@ColmapSchoolNum, [ColmapInvoiceDate]=@ColmapInvoiceDate, [ColmapPostDate]=@ColmapPostDate, [ColmapVendorID]=@ColmapVendorID, " +
                    "[ColmapInvoiceNum]=@ColmapInvoiceNum, [ColmapFood]=@ColmapFood, [ColmapMilk]=@ColmapMilk, [ColmapPaper]=@ColmapPaper, [ColmapClean]=@ColmapClean, [ColmapOther]=@ColmapOther, " +
                    "[ColmapOtherCat]=@ColmapOtherCat, [ColmapComments]=@ColmapComments, [ColmapProgType]=@ColmapProgType, [FileType]=@FileType, [UseValue_District]=@UseValue_District, " +
                    "[UseValue_SchoolNum]=@UseValue_SchoolNum, [UseValue_InvoiceDate]=@UseValue_InvoiceDate, [UseValue_VendorID]=@UseValue_VendorID, [UseValue_InvoiceNum]=@UseValue_InvoiceNum, " +
                    "[UseValue_FoodAmt]=@UseValue_FoodAmt, [UseValue_MilkAmt]=@UseValue_MilkAmt, [UseValue_PaperAmt]=@UseValue_PaperAmt, [UseValue_CleanAmt]=@UseValue_CleanAmt, " +
                    "[UseValue_OtherAmt]=@UseValue_OtherAmt, [UseValue_OtherCat]=@UseValue_OtherCat, [UseValue_Comments]=@UseValue_Comments, [UseValue_ProgType]=@UseValue_ProgType," +
                    "[CB_UseDefaultSchool]=@CB_UseDefaultSchool,[CB_ContainsCommissaryRecords]=@CB_ContainsCommissaryRecords,[CB_ContainsToolboxRecords]=@CB_ContainsToolboxRecords," +
                    "[UseValue_OtherCatComm]=@UseValue_OtherCatComm, [UseValue_OtherCatToolbox]=@UseValue_OtherCatToolbox " +
                    " WHERE [ImportConfigurationName]=@ImportConfigurationName ";
            else
                queryImportValues =
                    "INSERT INTO [PurchasesImportSettings] ([ImportConfigurationName]" +
                    ",[CB_UseVendCustMapping],[CB_GetDistFromImport],[CB_GetSchoolFromImport],[CB_GetInvoiceDateFromImport],[CB_GetPostDateFromImport],[CB_GetVendorFromImport] " +
                    ",[CB_GetInvoiceNumFromImport],[CB_GetFoodAmtFromImport],[CB_GetMilkAmtFromImport],[CB_GetPaperAmtFromImport],[CB_GetCleanAmtFromImport] " +
                    ",[CB_GetOtherAmtFromimport],[CB_GetOtherCatFromImport],[CB_GetCommentsFromImport],[CB_LockRecords],[CB_GetProgTypeFromImport] " +
                    ",[CB_ImportHasHeaders],[ColmapCustomerNumber],[ColmapDistrict],[ColmapSchoolNum],[ColmapInvoiceDate],[ColmapPostDate],[ColmapVendorID] " +
                    ",[ColmapInvoiceNum],[ColmapFood],[ColmapMilk],[ColmapPaper],[ColmapClean],[ColmapOther],[ColmapOtherCat],[ColmapComments] " +
                    ",[ColmapProgType],[FileType],[UseValue_District],[UseValue_SchoolNum],[UseValue_InvoiceDate],[UseValue_VendorID],[UseValue_InvoiceNum] " +
                    ",[UseValue_FoodAmt],[UseValue_MilkAmt],[UseValue_PaperAmt],[UseValue_CleanAmt],[UseValue_OtherAmt],[UseValue_OtherCat],[UseValue_Comments],[UseValue_ProgType]" +
                    ",[CB_UseDefaultSchool],[CB_ContainsCommissaryRecords],[CB_ContainsToolboxRecords],[UseValue_OtherCatComm],[UseValue_OtherCatToolbox]) " +
                    "VALUES(@ImportConfigurationName" +
                    ",@CB_UseVendCustMapping,@CB_GetDistFromImport,@CB_GetSchoolFromImport,@CB_GetInvoiceDateFromImport,@CB_GetPostDateFromImport,@CB_GetVendorFromImport " +
                    ",@CB_GetInvoiceNumFromImport,@CB_GetFoodAmtFromImport,@CB_GetMilkAmtFromImport,@CB_GetPaperAmtFromImport,@CB_GetCleanAmtFromImport " +
                    ",@CB_GetOtherAmtFromimport,@CB_GetOtherCatFromImport,@CB_GetCommentsFromImport,@CB_LockRecords,@CB_GetProgTypeFromImport " +
                    ",@CB_ImportHasHeaders,@ColmapCustomerNumber,@ColmapDistrict,@ColmapSchoolNum,@ColmapInvoiceDate,@ColmapPostDate,@ColmapVendorID " +
                    ",@ColmapInvoiceNum,@ColmapFood,@ColmapMilk,@ColmapPaper,@ColmapClean,@ColmapOther,@ColmapOtherCat,@ColmapComments " +
                    ",@ColmapProgType,@FileType,@UseValue_District,@UseValue_SchoolNum,@UseValue_InvoiceDate,@UseValue_VendorID,@UseValue_InvoiceNum " +
                    ",@UseValue_FoodAmt,@UseValue_MilkAmt,@UseValue_PaperAmt,@UseValue_CleanAmt,@UseValue_OtherAmt,@UseValue_OtherCat,@UseValue_Comments,@UseValue_ProgType" +
                    ",@CB_UseDefaultSchool,@CB_ContainsCommissaryRecords,@CB_ContainsToolboxRecords,@UseValue_OtherCatComm,@UseValue_OtherCatToolbox) ";


            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                #region KeyVal
                SqlParameter dbParam_ConfigName = new SqlParameter();
                dbParam_ConfigName.ParameterName = "@ImportConfigurationName";
                dbParam_ConfigName.Value = ConfigName;
                dbParam_ConfigName.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_ConfigName);
                #endregion
                #region CheckBoxes

                SqlParameter dbParam_CBUseVendCustMap = new SqlParameter();
                dbParam_CBUseVendCustMap.ParameterName = "@CB_UseVendCustMapping";
                dbParam_CBUseVendCustMap.Value = CBUseVendCustMap.Checked;
                dbParam_CBUseVendCustMap.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBUseVendCustMap);

                SqlParameter dbParam_CBDistFromImport = new SqlParameter();
                dbParam_CBDistFromImport.ParameterName = "@CB_GetDistFromImport";
                dbParam_CBDistFromImport.Value = CBDistFromImport.Checked;
                dbParam_CBDistFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBDistFromImport);

                SqlParameter dbParam_CBSchoolFromImport = new SqlParameter();
                dbParam_CBSchoolFromImport.ParameterName = "@CB_GetSchoolFromImport";
                dbParam_CBSchoolFromImport.Value = CBSchoolNumFromImport.Checked;
                dbParam_CBSchoolFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBSchoolFromImport);

                SqlParameter dbParam_CBUseDefaultSchool = new SqlParameter();
                dbParam_CBUseDefaultSchool.ParameterName = "@CB_UseDefaultSchool";
                dbParam_CBUseDefaultSchool.Value = CBUseDefaultSchool.Checked;
                dbParam_CBUseDefaultSchool.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBUseDefaultSchool);

                SqlParameter dbParam_CBInvDateFromimport = new SqlParameter();
                dbParam_CBInvDateFromimport.ParameterName = "@CB_GetInvoiceDateFromImport";
                dbParam_CBInvDateFromimport.Value = CBInvDateFromImport.Checked;
                dbParam_CBInvDateFromimport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBInvDateFromimport);

                SqlParameter dbParam_CBPostDateFromImport = new SqlParameter();
                dbParam_CBPostDateFromImport.ParameterName = "@CB_GetPostDateFromImport";
                dbParam_CBPostDateFromImport.Value = CBPostDateFromImport.Checked;
                dbParam_CBPostDateFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBPostDateFromImport);

                SqlParameter dbParam_CBVendFromImport = new SqlParameter();
                dbParam_CBVendFromImport.ParameterName = "@CB_GetVendorFromImport";
                dbParam_CBVendFromImport.Value = CBVendorFromImport.Checked;
                dbParam_CBVendFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBVendFromImport);

                SqlParameter dbParam_CBInvNumFromImport = new SqlParameter();
                dbParam_CBInvNumFromImport.ParameterName = "@CB_GetInvoiceNumFromImport";
                dbParam_CBInvNumFromImport.Value = CBInvNumFromImport.Checked;
                dbParam_CBInvNumFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBInvNumFromImport);

                SqlParameter dbParam_CBFoodFromimport = new SqlParameter();
                dbParam_CBFoodFromimport.ParameterName = "@CB_GetFoodAmtFromImport";
                dbParam_CBFoodFromimport.Value = CBFoodAmtFromImport.Checked;
                dbParam_CBFoodFromimport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBFoodFromimport);

                SqlParameter dbParam_CBMilkFromImport = new SqlParameter();
                dbParam_CBMilkFromImport.ParameterName = "@CB_GetMilkAmtFromImport";
                dbParam_CBMilkFromImport.Value = CBMilkAmtFromImport.Checked;
                dbParam_CBMilkFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBMilkFromImport);

                SqlParameter dbParam_CBPaperFromImport = new SqlParameter();
                dbParam_CBPaperFromImport.ParameterName = "@CB_GetPaperAmtFromImport";
                dbParam_CBPaperFromImport.Value = CBPaperAmtFromImport.Checked;
                dbParam_CBPaperFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBPaperFromImport);

                SqlParameter dbParam_CBCleanFromImport = new SqlParameter();
                dbParam_CBCleanFromImport.ParameterName = "@CB_GetCleanAmtFromImport";
                dbParam_CBCleanFromImport.Value = CBCleaningAmtFromImport.Checked;
                dbParam_CBCleanFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBCleanFromImport);

                SqlParameter dbParam_CBOtherFromImport = new SqlParameter();
                dbParam_CBOtherFromImport.ParameterName = "@CB_GetOtherAmtFromimport";
                dbParam_CBOtherFromImport.Value = CBOtherAmtFromImport.Checked;
                dbParam_CBOtherFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBOtherFromImport);

                SqlParameter dbParam_CBOtherCatFromImport = new SqlParameter();
                dbParam_CBOtherCatFromImport.ParameterName = "@CB_GetOtherCatFromImport";
                dbParam_CBOtherCatFromImport.Value = CBOtherCatFromImport.Checked;
                dbParam_CBOtherCatFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBOtherCatFromImport);

                SqlParameter dbParam_CBCommentsFromImport = new SqlParameter();
                dbParam_CBCommentsFromImport.ParameterName = "@CB_GetCommentsFromImport";
                dbParam_CBCommentsFromImport.Value = CBPurchCommntsFromImport.Checked;
                dbParam_CBCommentsFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBCommentsFromImport);

                SqlParameter dbParam_CBLockRecords = new SqlParameter();
                dbParam_CBLockRecords.ParameterName = "@CB_LockRecords";
                dbParam_CBLockRecords.Value = CBLockRecords.Checked;
                dbParam_CBLockRecords.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBLockRecords);

                SqlParameter dbParam_CBProgTypeFromImport = new SqlParameter();
                dbParam_CBProgTypeFromImport.ParameterName = "@CB_GetProgTypeFromImport";
                dbParam_CBProgTypeFromImport.Value = CBProgramTypeFromImport.Checked;
                dbParam_CBProgTypeFromImport.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBProgTypeFromImport);

                SqlParameter dbParam_CBImportHasHeaders = new SqlParameter();
                dbParam_CBImportHasHeaders.ParameterName = "@CB_ImportHasHeaders";
                dbParam_CBImportHasHeaders.Value = CBFileHasHeaders.Checked;
                dbParam_CBImportHasHeaders.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBImportHasHeaders);

                SqlParameter dbParam_CBContainsCommRecords = new SqlParameter();
                dbParam_CBContainsCommRecords.ParameterName = "@CB_ContainsCommissaryRecords";
                dbParam_CBContainsCommRecords.Value = CBContainsCommissaryRecords.Checked;
                dbParam_CBContainsCommRecords.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBContainsCommRecords);

                SqlParameter dbParam_CBContainsToolboxRecords = new SqlParameter();
                dbParam_CBContainsToolboxRecords.ParameterName = "@CB_ContainsToolboxRecords";
                dbParam_CBContainsToolboxRecords.Value = CBContainsToolboxRecords.Checked;
                dbParam_CBContainsToolboxRecords.DbType = DbType.Byte;
                cmd.Parameters.Add(dbParam_CBContainsToolboxRecords);

                #endregion
                #region ColMaps

                SqlParameter dbParam_ColmapCustNumber = new SqlParameter();
                dbParam_ColmapCustNumber.ParameterName = "@ColmapCustomerNumber";
                int.TryParse(TextBoxVendCustCol.Text, out int intColmapCustNum);
                dbParam_ColmapCustNumber.Value = intColmapCustNum;
                dbParam_ColmapCustNumber.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapCustNumber);

                SqlParameter dbParam_ColmapDist = new SqlParameter();
                dbParam_ColmapDist.ParameterName = "@ColmapDistrict";
                int.TryParse(TextBoxDistCol.Text, out int intColmapDist);
                dbParam_ColmapDist.Value = intColmapDist ;
                dbParam_ColmapDist.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapDist);

                SqlParameter dbParam_ColmapSchoolnum = new SqlParameter();
                dbParam_ColmapSchoolnum.ParameterName = "@ColmapSchoolNum";
                int.TryParse(TextBoxSchoolNumCol.Text, out int intColmapSchoolNum);
                dbParam_ColmapSchoolnum.Value = intColmapSchoolNum ;
                dbParam_ColmapSchoolnum.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapSchoolnum);

                SqlParameter dbParam_ColmapInvDate = new SqlParameter();
                dbParam_ColmapInvDate.ParameterName = "@ColmapInvoiceDate";
                int.TryParse(TextBoxInvDateCol.Text, out int intColmapInvDate);
                dbParam_ColmapInvDate.Value = intColmapInvDate ;
                dbParam_ColmapInvDate.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapInvDate);

                SqlParameter dbParam_ColmapPostDate = new SqlParameter();
                dbParam_ColmapPostDate.ParameterName = "@ColmapPostDate";
                int.TryParse(TextBoxPostDateCol.Text, out int intColmapPostDate);
                dbParam_ColmapPostDate.Value = intColmapPostDate ;
                dbParam_ColmapPostDate.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapPostDate);

                SqlParameter dbParam_ColmapVendorID = new SqlParameter();
                dbParam_ColmapVendorID.ParameterName = "@ColmapVendorID";
                int.TryParse(TextBoxVendorIDCol.Text, out int intColmapVendID);
                dbParam_ColmapVendorID.Value = intColmapVendID ;
                dbParam_ColmapVendorID.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapVendorID);

                SqlParameter dbParam_ColmapInvNum = new SqlParameter();
                dbParam_ColmapInvNum.ParameterName = "@ColmapInvoiceNum";
                int.TryParse(TextBoxInvNumCol.Text, out int intColmapInvNum);
                dbParam_ColmapInvNum.Value = intColmapInvNum ;
                dbParam_ColmapInvNum.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapInvNum);

                SqlParameter dbParam_ColmapFood = new SqlParameter();
                dbParam_ColmapFood.ParameterName = "@ColmapFood";
                int.TryParse(TextBoxFoodAmtCol.Text, out int intColmapFood);
                dbParam_ColmapFood.Value = intColmapFood;
                dbParam_ColmapFood.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapFood);

                SqlParameter dbParam_ColmapMilk = new SqlParameter();
                dbParam_ColmapMilk.ParameterName = "@ColmapMilk";
                int.TryParse(TextBoxMilkAmtCol.Text, out int intColmapMilk);
                dbParam_ColmapMilk.Value = intColmapMilk;
                dbParam_ColmapMilk.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapMilk);

                SqlParameter dbParam_ColmapPaper = new SqlParameter();
                dbParam_ColmapPaper.ParameterName = "@ColmapPaper";
                int.TryParse(TextBoxPaperAmtCol.Text, out int intColmapPaper);
                dbParam_ColmapPaper.Value = intColmapPaper;
                dbParam_ColmapPaper.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapPaper);

                SqlParameter dbParam_ColmapClean = new SqlParameter();
                dbParam_ColmapClean.ParameterName = "@ColmapClean";
                int.TryParse(TextBoxCleanAmtCol.Text, out int intColmapClean);
                dbParam_ColmapClean.Value = intColmapClean;
                dbParam_ColmapClean.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapClean);

                SqlParameter dbParam_ColmapOther = new SqlParameter();
                dbParam_ColmapOther.ParameterName = "@ColmapOther";
                int.TryParse(TextBoxOtherAmtCol.Text, out int intColmapOther);
                dbParam_ColmapOther.Value = intColmapOther;
                dbParam_ColmapOther.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapOther);

                SqlParameter dbParam_ColmapOtherCat = new SqlParameter();
                dbParam_ColmapOtherCat.ParameterName = "@ColmapOtherCat";
                int.TryParse(TextBoxOtherCatCol.Text, out int intColmapOtherCat);
                dbParam_ColmapOtherCat.Value = intColmapOtherCat;
                dbParam_ColmapOtherCat.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapOtherCat);

                SqlParameter dbParam_ColmapComments = new SqlParameter();
                dbParam_ColmapComments.ParameterName = "@ColmapComments";
                int.TryParse(TextBoxPurchCmmntCol.Text, out int intColmapComments);
                dbParam_ColmapComments.Value = intColmapComments;
                dbParam_ColmapComments.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapComments);

                SqlParameter dbParam_ColmapProgType = new SqlParameter();
                dbParam_ColmapProgType.ParameterName = "@ColmapProgType";
                int.TryParse(TextBoxProgTypeCol.Text, out int intColmapProgType);
                dbParam_ColmapProgType.Value = intColmapProgType;
                dbParam_ColmapProgType.DbType = DbType.Int16;
                cmd.Parameters.Add(dbParam_ColmapProgType);

                SqlParameter dbParam_FileType = new SqlParameter();
                dbParam_FileType.ParameterName = "@FileType";
                dbParam_FileType.Value = DDLFileType.SelectedValue;
                dbParam_FileType.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_FileType);

                #endregion
                #region ExplicitValues
                SqlParameter dbParam_UseValueDist = new SqlParameter();
                dbParam_UseValueDist.ParameterName = "@UseValue_District";
                dbParam_UseValueDist.Value = TextBoxDistSpecified.Text;
                dbParam_UseValueDist.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueDist);

                SqlParameter dbParam_UseValueSchoolNum = new SqlParameter();
                dbParam_UseValueSchoolNum.ParameterName = "@UseValue_SchoolNum";
                dbParam_UseValueSchoolNum.Value = TextBoxSchoolNumSpecified.Text;
                dbParam_UseValueSchoolNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueSchoolNum);

                SqlParameter dbParam_UseValueInvDate = new SqlParameter();
                dbParam_UseValueInvDate.ParameterName = "@UseValue_InvoiceDate";
                dbParam_UseValueInvDate.Value = TextBoxInvoiceDateSpecified.Text;
                dbParam_UseValueInvDate.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueInvDate);

                SqlParameter dbParam_UseValueVendorID = new SqlParameter();
                dbParam_UseValueVendorID.ParameterName = "@UseValue_VendorID";
                dbParam_UseValueVendorID.Value = TextBoxVendorIDSpecified.Text;
                dbParam_UseValueVendorID.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueVendorID);

                SqlParameter dbParam_UseValueInvoiceNum = new SqlParameter();
                dbParam_UseValueInvoiceNum.ParameterName = "@UseValue_InvoiceNum";
                dbParam_UseValueInvoiceNum.Value = TextBoxInvNumSpecified.Text;
                dbParam_UseValueInvoiceNum.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueInvoiceNum);

                SqlParameter dbParam_UseValueFood = new SqlParameter();
                dbParam_UseValueFood.ParameterName = "@UseValue_FoodAmt";
                dbParam_UseValueFood.Value = TextBoxFoodAmtSpecified.Text;
                dbParam_UseValueFood.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueFood);

                SqlParameter dbParam_UseValueMilk = new SqlParameter();
                dbParam_UseValueMilk.ParameterName = "@UseValue_MilkAmt";
                dbParam_UseValueMilk.Value = TextBoxMilkAmtSpecified.Text;
                dbParam_UseValueMilk.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueMilk);

                SqlParameter dbParam_UseValuePaper = new SqlParameter();
                dbParam_UseValuePaper.ParameterName = "@UseValue_PaperAmt";
                dbParam_UseValuePaper.Value = TextBoxPaperAmtSpecified.Text;
                dbParam_UseValuePaper.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValuePaper);

                SqlParameter dbParam_UseValueClean = new SqlParameter();
                dbParam_UseValueClean.ParameterName = "@UseValue_CleanAmt";
                dbParam_UseValueClean.Value = TextBoxCleanAmtSpecified.Text;
                dbParam_UseValueClean.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueClean);

                SqlParameter dbParam_UseValueOther = new SqlParameter();
                dbParam_UseValueOther.ParameterName = "@UseValue_OtherAmt";
                dbParam_UseValueOther.Value = TextBoxOtherAmtSpecified.Text;
                dbParam_UseValueOther.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueOther);

                SqlParameter dbParam_UseValueOtherCat = new SqlParameter();
                dbParam_UseValueOtherCat.ParameterName = "@UseValue_OtherCat";
                dbParam_UseValueOtherCat.Value = ddlOtherCats.SelectedValue;
                dbParam_UseValueOtherCat.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueOtherCat);

                SqlParameter dbParam_UseValueComments = new SqlParameter();
                dbParam_UseValueComments.ParameterName = "@UseValue_Comments";
                dbParam_UseValueComments.Value = TextBoxPurchCmmntSpecified.Text;
                dbParam_UseValueComments.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueComments);

                SqlParameter dbParam_UseValueProgType = new SqlParameter();
                dbParam_UseValueProgType.ParameterName = "@UseValue_ProgType";
                dbParam_UseValueProgType.Value = DDLProgramType.SelectedValue;
                dbParam_UseValueProgType.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueProgType);

                SqlParameter dbParam_UseValueOtherCatComm = new SqlParameter();
                dbParam_UseValueOtherCatComm.ParameterName = "@UseValue_OtherCatComm";
                dbParam_UseValueOtherCatComm.Value = ddlOtherCatsComm.SelectedValue;
                dbParam_UseValueOtherCatComm.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueOtherCatComm);

                SqlParameter dbParam_UseValueOtherCatToolbox = new SqlParameter();
                dbParam_UseValueOtherCatToolbox.ParameterName = "@UseValue_OtherCatToolbox";
                dbParam_UseValueOtherCatToolbox.Value = ddlOtherCatsToolbox.SelectedValue;
                dbParam_UseValueOtherCatToolbox.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_UseValueOtherCatToolbox);
                #endregion

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL error while saving configuration settings.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error while saving configuration settings.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void DeleteConfigSettings(string ConfigName)
        {
            if (string.IsNullOrEmpty(ConfigName))
            {
                return;
            }


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "";

            if (DoesConfigExist(ConfigName))
                queryImportValues =
                    "DELETE [PurchasesImportSettings] " +
                    " WHERE [ImportConfigurationName]=@ImportConfigurationName ";
            else
                return;


            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                #region KeyVal
                SqlParameter dbParam_ConfigName = new SqlParameter();
                dbParam_ConfigName.ParameterName = "@ImportConfigurationName";
                dbParam_ConfigName.Value = ConfigName;
                dbParam_ConfigName.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_ConfigName);
                #endregion

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL error while deleting configuration settings.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error while deleting configuration settings.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }

        }

        private bool ValidateConfigSettings()
        {
            bool SettingsAreValid = true;

            if (CBUseVendCustMap.Checked && (string.IsNullOrEmpty(TextBoxVendCustCol.Text) || int.TryParse(TextBoxVendCustCol.Text, out int CustCol)==false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Use Vendor-Customer mapping is checked but column is invalid", "");
                return SettingsAreValid;
            }
            if (CBDistFromImport.Checked && (string.IsNullOrEmpty(TextBoxDistCol.Text) || int.TryParse(TextBoxDistCol.Text, out int DistCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get District ID from import is checked but column is invalid", "");
                return SettingsAreValid;
            }
            if (!CBUseVendCustMap.Checked && !CBDistFromImport.Checked && string.IsNullOrEmpty(TextBoxDistSpecified.Text))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get District ID from import is not checked but and District ID was not provided", "");
                return SettingsAreValid;
            }
            if (CBSchoolNumFromImport.Checked && !CBUseDefaultSchool.Checked &&  (string.IsNullOrEmpty(TextBoxSchoolNumCol.Text) || int.TryParse(TextBoxSchoolNumCol.Text, out int SchoolCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get School Num from import is checked but column is invalid and Use Default school is not checked.", "");
                return SettingsAreValid;
            }
            if(!CBUseVendCustMap.Checked && !CBSchoolNumFromImport.Checked && !CBUseDefaultSchool.Checked && string.IsNullOrEmpty(TextBoxSchoolNumSpecified.Text))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get School Num from import is not checked, Use Default school is not checked and school num was not provided.", "");
                return SettingsAreValid;
            }
            if (CBInvDateFromImport.Checked  && (string.IsNullOrEmpty(TextBoxInvDateCol.Text) || int.TryParse(TextBoxInvDateCol.Text, out int InvDateCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Invoice Date from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if(!CBInvDateFromImport.Checked && (string.IsNullOrEmpty(TextBoxInvoiceDateSpecified.Text) || DateTime.TryParse(TextBoxInvoiceDateSpecified.Text, out DateTime invDate)==false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Invoice Date from import is not checked and invoice date specified is invalid.", "");
                return SettingsAreValid;
            }
            if (CBPostDateFromImport.Checked && (string.IsNullOrEmpty(TextBoxPostDateCol.Text) || int.TryParse(TextBoxPostDateCol.Text, out int PostDateCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Post Date from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if (CBVendorFromImport.Checked && (string.IsNullOrEmpty(TextBoxVendorIDCol.Text) || int.TryParse(TextBoxVendorIDCol.Text, out int VendorCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Vendor ID from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if(!CBVendorFromImport.Checked && string.IsNullOrEmpty(TextBoxVendorIDSpecified.Text))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Vendor ID from import is not checked but Vendor ID is missing.", "");
                return SettingsAreValid;
            }
            if (CBInvNumFromImport.Checked && (string.IsNullOrEmpty(TextBoxInvNumCol.Text) || int.TryParse(TextBoxInvNumCol.Text, out int InvNumCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Invoice Number from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if(!CBInvNumFromImport.Checked && string.IsNullOrEmpty(TextBoxInvNumSpecified.Text))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Invoice Number from import is not checked but Invoice Number entered is invalid.", "");
                return SettingsAreValid;
            }
            if (CBFoodAmtFromImport.Checked && (string.IsNullOrEmpty(TextBoxFoodAmtCol.Text) || int.TryParse(TextBoxFoodAmtCol.Text, out int FoodCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Food Amount from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if (CBMilkAmtFromImport.Checked && (string.IsNullOrEmpty(TextBoxMilkAmtCol.Text) || int.TryParse(TextBoxMilkAmtCol.Text, out int MilkCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Milk Amount from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if (CBPaperAmtFromImport.Checked && (string.IsNullOrEmpty(TextBoxPaperAmtCol.Text) || int.TryParse(TextBoxPaperAmtCol.Text, out int PaperCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Paper Amount from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if (CBCleaningAmtFromImport.Checked && (string.IsNullOrEmpty(TextBoxCleanAmtCol.Text) || int.TryParse(TextBoxCleanAmtCol.Text, out int CleanCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Cleaning Amount from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if (CBOtherAmtFromImport.Checked && (string.IsNullOrEmpty(TextBoxOtherAmtCol.Text) || int.TryParse(TextBoxOtherAmtCol.Text, out int OtherCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Other Amount from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            decimal.TryParse(TextBoxFoodAmtSpecified.Text, out decimal decFood);
            decimal.TryParse(TextBoxMilkAmtSpecified.Text, out decimal decMilk);
            decimal.TryParse(TextBoxPaperAmtSpecified.Text, out decimal decPaper);
            decimal.TryParse(TextBoxCleanAmtSpecified.Text, out decimal decClean);
            decimal.TryParse(TextBoxOtherAmtSpecified.Text, out decimal decOther);
            if (!CBFoodAmtFromImport.Checked && !CBMilkAmtFromImport.Checked && !CBPaperAmtFromImport.Checked && !CBCleaningAmtFromImport.Checked && !CBOtherAmtFromImport.Checked &&
                ((decFood + decMilk + decClean + decPaper + decOther)==0M))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get no amounts are being retrieved from the import and all specified amounts add up to zero.", "");
                return SettingsAreValid;
            }
            if (CBOtherCatFromImport.Checked && (string.IsNullOrEmpty(TextBoxOtherCatCol.Text) || int.TryParse(TextBoxOtherCatCol.Text, out int OtherCatCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Other Category from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if (CBPurchCommntsFromImport.Checked && (string.IsNullOrEmpty(TextBoxPurchCmmntCol.Text) || int.TryParse(TextBoxPurchCmmntCol.Text, out int CmmntCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Comments from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }
            if (CBProgramTypeFromImport.Checked && (string.IsNullOrEmpty(TextBoxProgTypeCol.Text) || int.TryParse(TextBoxProgTypeCol.Text, out int ProgCol) == false))
            {
                SettingsAreValid = false;
                ShowDialog("Error", MessageType.Error, "Get Program Type from import is checked but column is invalid.", "");
                return SettingsAreValid;
            }

            ShowHideCommissaryAndToolboxSettings();

            return SettingsAreValid;
        }

        protected bool DoesConfigExist(string ConfigName)
        {
            bool exists = false;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);
            string queryConfig = "SELECT COUNT(*) FROM [PurchasesImportSettings] WHERE [ImportConfigurationName]=@ImportConfigurationName";
            using (SqlCommand cmd = new SqlCommand(queryConfig, dbConnection))
            {
                #region KeyVal
                SqlParameter dbParam_ConfigName = new SqlParameter();
                dbParam_ConfigName.ParameterName = "@ImportConfigurationName";
                dbParam_ConfigName.Value = ConfigName;
                dbParam_ConfigName.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_ConfigName);
                #endregion

                try
                {


                    dbConnection.Open();
                    object retObj = cmd.ExecuteScalar();
                    if(retObj != null)
                    {
                        if((int)retObj > 0) { exists = true; }
                    }
                    
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL error while checking for configuration.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error while checking for configuration.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }


            }

            return exists;
        }

        protected void ApplyConfigSettings(string ConfigName)
        {
            ShowHideCommissaryAndToolboxSettings();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);
            string queryConfig = "SELECT * FROM [PurchasesImportSettings] WHERE [ImportConfigurationName]=@ImportConfigurationName";
            using (SqlCommand cmd = new SqlCommand(queryConfig, dbConnection))
            {
                #region KeyVal
                SqlParameter dbParam_ConfigName = new SqlParameter();
                dbParam_ConfigName.ParameterName = "@ImportConfigurationName";
                dbParam_ConfigName.Value = ConfigName;
                dbParam_ConfigName.DbType = DbType.String;
                cmd.Parameters.Add(dbParam_ConfigName);
                #endregion

                if (ConfigName == string.Empty)
                {
                    ResetSettings();
                }
                else
                {
                    try
                    {
                        dbConnection.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                #region Checkboxes
                                CBUseVendCustMap.Checked = (bool)rdr["CB_UseVendCustMapping"]; CBUseVendCustMap_CheckedChanged(null, null);
                                CBDistFromImport.Checked = (bool)rdr["CB_GetDistFromImport"]; CBDistFromImport_CheckedChanged(null, null);
                                CBSchoolNumFromImport.Checked = (bool)rdr["CB_GetSchoolFromImport"]; CBSchoolNum_CheckedChanged(null, null);
                                CBUseDefaultSchool.Checked = (bool)rdr["CB_UseDefaultSchool"];
                                CBInvDateFromImport.Checked = (bool)rdr["CB_GetInvoiceDateFromImport"]; CBInvDateFromImport_CheckedChanged(null, null);
                                CBPostDateFromImport.Checked = (bool)rdr["CB_GetPostDateFromImport"]; CBPostDateFromImport_CheckedChanged(null, null);
                                CBVendorFromImport.Checked = (bool)rdr["CB_GetVendorFromImport"]; CBVendorFromImport_CheckedChanged(null, null);
                                CBInvNumFromImport.Checked = (bool)rdr["CB_GetInvoiceNumFromImport"]; CBInvNumFromImport_CheckedChanged(null, null);
                                CBFoodAmtFromImport.Checked = (bool)rdr["CB_GetFoodAmtFromImport"]; CBFoodAmtFromImport_CheckedChanged(null, null);
                                CBMilkAmtFromImport.Checked = (bool)rdr["CB_GetMilkAmtFromImport"]; CBMilkAmtFromImport_CheckedChanged(null, null);
                                CBPaperAmtFromImport.Checked = (bool)rdr["CB_GetPaperAmtFromImport"]; CBPaperAmtFromImport_CheckedChanged(null, null);
                                CBCleaningAmtFromImport.Checked = (bool)rdr["CB_GetCleanAmtFromImport"]; CBCleaningAmtFromImport_CheckedChanged(null, null);
                                CBOtherAmtFromImport.Checked = (bool)rdr["CB_GetOtherAmtFromImport"]; CBOtherAmtFromImport_CheckedChanged(null, null);
                                CBOtherCatFromImport.Checked = (bool)rdr["CB_GetOtherCatFromImport"]; CBOtherCatFromImport_CheckedChanged(null, null);
                                CBPurchCommntsFromImport.Checked = (bool)rdr["CB_GetCommentsFromImport"]; CBPurchCommntsFromImport_CheckedChanged(null, null);
                                CBLockRecords.Checked = (bool)rdr["CB_LockRecords"];
                                CBProgramTypeFromImport.Checked = (bool)rdr["CB_GetProgTypeFromImport"]; CBProgramTypeFromImport_CheckedChanged(null, null);
                                CBFileHasHeaders.Checked = (bool)rdr["CB_ImportHasHeaders"];
                                CBContainsCommissaryRecords.Checked = (bool)rdr["CB_ContainsCommissaryRecords"]; CBContainsCommissaryRecords_CheckedChanged(null, null);
                                CBContainsToolboxRecords.Checked = (bool)rdr["CB_ContainsToolboxRecords"]; CBContainsToolboxRecords_CheckedChanged(null, null);
                                #endregion
                                #region ColMaps
                                TextBoxVendCustCol.Text = ((Int16)rdr["ColmapCustomerNumber"]) > 0 ? ((Int16)rdr["ColmapCustomerNumber"]).ToString() : string.Empty;
                                TextBoxDistCol.Text = ((Int16)rdr["ColmapDistrict"]) > 0 ? ((Int16)rdr["ColmapDistrict"]).ToString() : string.Empty;
                                TextBoxSchoolNumCol.Text = ((Int16)rdr["ColmapSchoolNum"]) > 0 ? ((Int16)rdr["ColmapSchoolNum"]).ToString() : string.Empty;
                                TextBoxInvDateCol.Text = ((Int16)rdr["ColmapInvoiceDate"]) > 0 ? ((Int16)rdr["ColmapInvoiceDate"]).ToString() : string.Empty;
                                TextBoxPostDateCol.Text = ((Int16)rdr["ColmapPostDate"]) > 0 ? ((Int16)rdr["ColmapPostDate"]).ToString() : string.Empty;
                                TextBoxVendorIDCol.Text = ((Int16)rdr["ColmapVendorID"]) > 0 ? ((Int16)rdr["ColmapVendorID"]).ToString() : string.Empty;
                                TextBoxInvNumCol.Text = ((Int16)rdr["ColmapInvoiceNum"]) > 0 ? ((Int16)rdr["ColmapInvoiceNum"]).ToString() : string.Empty;
                                TextBoxFoodAmtCol.Text = ((Int16)rdr["ColmapFood"]) > 0 ? ((Int16)rdr["ColmapFood"]).ToString() : string.Empty;
                                TextBoxMilkAmtCol.Text = ((Int16)rdr["ColmapMilk"]) > 0 ? ((Int16)rdr["ColmapMilk"]).ToString() : string.Empty;
                                TextBoxPaperAmtCol.Text = ((Int16)rdr["ColmapPaper"]) > 0 ? ((Int16)rdr["ColmapPaper"]).ToString() : string.Empty;
                                TextBoxCleanAmtCol.Text = ((Int16)rdr["ColmapClean"]) > 0 ? ((Int16)rdr["ColmapClean"]).ToString() : string.Empty;
                                TextBoxOtherAmtCol.Text = ((Int16)rdr["ColmapOther"]) > 0 ? ((Int16)rdr["ColmapOther"]).ToString() : string.Empty;
                                TextBoxOtherCatCol.Text = ((Int16)rdr["ColmapOtherCat"]) > 0 ? ((Int16)rdr["ColmapOtherCat"]).ToString() : string.Empty;
                                TextBoxPurchCmmntCol.Text = ((Int16)rdr["ColmapComments"]) > 0 ? ((Int16)rdr["ColmapComments"]).ToString() : string.Empty;
                                TextBoxProgTypeCol.Text = ((Int16)rdr["ColmapProgType"]) > 0 ? ((Int16)rdr["ColmapProgType"]).ToString() : string.Empty;
                                #endregion
                                #region Values
                                TextBoxDistSpecified.Text = rdr["UseValue_District"].ToString();
                                TextBoxSchoolNumSpecified.Text = rdr["UseValue_SchoolNum"].ToString();
                                DateTime emptyDate = new DateTime(1900, 1, 1);
                                TextBoxInvoiceDateSpecified.Text = ((DateTime)rdr["UseValue_InvoiceDate"]) == emptyDate ? "" : ((DateTime)rdr["UseValue_InvoiceDate"]).ToShortDateString();
                                TextBoxVendorIDSpecified.Text = rdr["UseValue_VendorID"].ToString();
                                TextBoxInvNumSpecified.Text = rdr["UseValue_InvoiceNum"].ToString();
                                TextBoxFoodAmtSpecified.Text = rdr["UseValue_FoodAmt"].ToString();
                                TextBoxMilkAmtSpecified.Text = rdr["UseValue_MilkAmt"].ToString();
                                TextBoxPaperAmtSpecified.Text = rdr["UseValue_PaperAmt"].ToString();
                                TextBoxCleanAmtSpecified.Text = rdr["UseValue_CleanAmt"].ToString();
                                TextBoxOtherAmtSpecified.Text = rdr["UseValue_OtherAmt"].ToString();
                                try { ddlOtherCats.SelectedValue = rdr["UseValue_OtherCat"].ToString(); } catch { ddlOtherCats.SelectedIndex = 0; }
                                TextBoxPurchCmmntSpecified.Text = rdr["UseValue_Comments"].ToString();
                                try { DDLProgramType.SelectedValue = rdr["UseValue_ProgType"].ToString(); } catch { }
                                try { DDLFileType.SelectedValue = rdr["FileType"].ToString(); } catch { }
                                try { ddlOtherCatsComm.SelectedValue = rdr["UseValue_OtherCatComm"].ToString(); } catch { ddlOtherCatsComm.SelectedIndex = 0; }
                                try { ddlOtherCatsToolbox.SelectedValue = rdr["UseValue_OtherCatToolbox"].ToString(); } catch { ddlOtherCatsToolbox.SelectedIndex = 0; }
                                #endregion

                            }
                            else
                            {
                                ResetSettings();
                            }
                        }

                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("Error", MessageType.Error, "SQL error while retrieving configuration.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "Error while retrieving configuration.", ex.Message);
                    }
                    finally
                    {
                        dbConnection.Close();
                    }
                }


            }
        }

        protected void ResetSettings()
        {
            #region Checkboxes
            CBUseVendCustMap.Checked = false;
            CBDistFromImport.Checked = false;
            CBSchoolNumFromImport.Checked = false;
            CBUseDefaultSchool.Checked = false;
            CBInvDateFromImport.Checked = false;
            CBPostDateFromImport.Checked = false;
            CBVendorFromImport.Checked = false;
            CBInvNumFromImport.Checked = false;
            CBFoodAmtFromImport.Checked = false;
            CBMilkAmtFromImport.Checked = false;
            CBPaperAmtFromImport.Checked = false;
            CBCleaningAmtFromImport.Checked = false;
            CBOtherAmtFromImport.Checked = false;
            CBOtherCatFromImport.Checked = false;
            CBPurchCommntsFromImport.Checked = false;
            CBLockRecords.Checked = false;
            CBProgramTypeFromImport.Checked = false;
            CBFileHasHeaders.Checked = false;
            CBContainsCommissaryRecords.Checked = false;
            CBContainsToolboxRecords.Checked = false;
            #endregion
            #region ColMaps
            TextBoxVendCustCol.Text = string.Empty;
            TextBoxDistCol.Text = string.Empty;
            TextBoxSchoolNumCol.Text = string.Empty;
            TextBoxInvDateCol.Text = string.Empty;
            TextBoxPostDateCol.Text = string.Empty;
            TextBoxVendorIDCol.Text = string.Empty;
            TextBoxInvNumCol.Text = string.Empty;
            TextBoxFoodAmtCol.Text = string.Empty;
            TextBoxMilkAmtCol.Text = string.Empty;
            TextBoxPaperAmtCol.Text = string.Empty;
            TextBoxCleanAmtCol.Text = string.Empty;
            TextBoxOtherAmtCol.Text = string.Empty;
            TextBoxOtherCatCol.Text = string.Empty;
            TextBoxPurchCmmntCol.Text = string.Empty;
            TextBoxProgTypeCol.Text = string.Empty;
            #endregion
            #region Values
            TextBoxDistSpecified.Text = string.Empty;
            TextBoxSchoolNumSpecified.Text = string.Empty;
            DateTime emptyDate = new DateTime(1900, 1, 1);
            TextBoxInvoiceDateSpecified.Text = string.Empty;
            TextBoxVendorIDSpecified.Text = string.Empty;
            TextBoxInvNumSpecified.Text = string.Empty;
            TextBoxFoodAmtSpecified.Text = "0";
            TextBoxMilkAmtSpecified.Text = "0";
            TextBoxPaperAmtSpecified.Text = "0";
            TextBoxCleanAmtSpecified.Text = "0";
            TextBoxOtherAmtSpecified.Text = "0";
            try { ddlOtherCats.SelectedIndex = 0; } catch { }
            TextBoxPurchCmmntSpecified.Text = string.Empty;
            try { DDLProgramType.SelectedIndex = 0; } catch { }
            try { DDLFileType.SelectedIndex = 0; } catch { }
            try { ddlOtherCatsComm.SelectedIndex = 0; } catch { }
            try { ddlOtherCatsToolbox.SelectedIndex = 0; } catch { }
            #endregion

        }

        protected void DDLConfigs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyConfigSettings(DDLConfigs.SelectedValue);
        }

        #endregion

        
    }
}
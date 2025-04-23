using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportExtraChef : Paperwork_Global
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

            if (FileUpload1.PostedFile != null && !string.IsNullOrEmpty(FileUpload1.PostedFile.FileName))
            {
                try
                {
                    //clear out the temp table [IMPORT_XTRACHEF]
                    ClearXTRACHEFImportTable();
                    //New 4/8/2022 - adding Commissary Capabilities
                    CommissaryClearXTRACHEFImportTable();

                    TextBoxStatus.Text += "-Temp tables cleared.";
                    TextBoxStatus.Rows++;


                    string path = string.Concat(Server.MapPath("~/UploadFile/" + FileUpload1.FileName));
                    FileUpload1.SaveAs(path);


                    TextBoxStatus.Text += Environment.NewLine + "-File " + FileUpload1.FileName.ToLower() + " uploaded.";
                    TextBoxStatus.Rows++;

                    TextBoxStatus.Text += Environment.NewLine + "-Reading data.";
                    TextBoxStatus.Rows++;

                    //Read the contents of CSV file.
                    string csvData = File.ReadAllText(path);

                    Boolean importErrors = false;

                    int recordCount = 0;
                    int recordErrorCount = 0;

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {

                        //skip row headers
                        if (row.IndexOf("Batch Number") > -1)
                            continue;

                        if (!string.IsNullOrEmpty(row))
                        {
                            string ErrorHint = "";

                            //Parse the import row and creates the import record in [IMPORT_XTRACHEF] which holds temporary records
                            if (CreateImportRecord(row, ref ErrorHint) == false)
                            {
                                //record creation failed - report issues
                                if (recordErrorCount == 0)
                                {
                                    TextBoxStatus.Text += Environment.NewLine + "-[PAPERWORK]     Import failed at the following record(s):";
                                    TextBoxStatus.Rows++;
                                }

                                TextBoxStatus.Text += Environment.NewLine + "  " + row;
                                TextBoxStatus.Rows += 3;//add 3 lines since the records are long and wrap
                                if (string.IsNullOrEmpty(ErrorHint) == false)
                                {
                                    TextBoxStatus.Text += Environment.NewLine + "  Reason: " + ErrorHint;
                                    TextBoxStatus.Rows++;
                                }

                                importErrors = true;
                                recordErrorCount++;

                                //9/10/2021 - Tom asked to continue even when errors are encountered
                                //break;//exit out of the foreach loop of records
                            }
                            else
                                recordCount++;

                            if (CommissaryCreateImportRecord(row, ref ErrorHint) == false)
                            {
                                //record creation failed - report issues
                                if (recordErrorCount == 0)
                                {
                                    TextBoxStatus.Text += Environment.NewLine + "-[COMMISSARYWEB]     Import failed at the following record(s):";
                                    TextBoxStatus.Rows++;
                                }

                                TextBoxStatus.Text += Environment.NewLine + "  " + row;
                                TextBoxStatus.Rows += 3;//add 3 lines since the records are long and wrap
                                if (string.IsNullOrEmpty(ErrorHint) == false)
                                {
                                    TextBoxStatus.Text += Environment.NewLine + "  Reason: " + ErrorHint;
                                    TextBoxStatus.Rows++;
                                }

                                //importErrors = true;
                                //recordErrorCount++;
                            }

                        }
                    }

                    TextBoxStatus.Text += Environment.NewLine + "-Read " + recordCount.ToString() + " Records from the import file.";
                    TextBoxStatus.Rows++;

                    //Errors
                    if (importErrors == true)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "--" + recordErrorCount.ToString() + " Error Records were encountered during import. These will either need to be fixed in the source file and reimported, or entered manually.";
                        TextBoxStatus.Rows++;
                    }

                    try
                    {
                        File.Delete(path);
                    }
                    catch { }//silent catch


                    //Finally - Create the Purch records

                    //PAPERWORK PROCESSING
                    Int32 CountPurchRecordsPaperwork = 0;
                    Int32 SkippedRecordsPaperwork = 0;
                    if (CreatePurchaseRecords(out CountPurchRecordsPaperwork, out SkippedRecordsPaperwork) == true)
                    {
                        //if successful clear out the temp table
                        //Otherwise it may be useful for troubleshooting
                        ClearXTRACHEFImportTable();

                        TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountPurchRecordsPaperwork.ToString(CultureInfo.CurrentCulture) + " Paperwork Transmittal record" + (CountPurchRecordsPaperwork == 1 ? " was" : "s were") + " created";
                        TextBoxStatus.Rows++;

                        if (SkippedRecordsPaperwork > 0)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Duplicate Found. " + SkippedRecordsPaperwork.ToString(CultureInfo.CurrentCulture) + " Paperwork Transmittal" + (SkippedRecordsPaperwork == 1 ? "" : "s") + " already existed and " + (SkippedRecordsPaperwork == 1 ? "was" : "were") + " skipped.";
                            TextBoxStatus.Rows++;
                        }

                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountPurchRecordsPaperwork.ToString(CultureInfo.CurrentCulture) + " Paperwork Transmittal record" + (CountPurchRecordsPaperwork == 1 ? " was" : "s were") + " created";
                        TextBoxStatus.Rows++;
                    }

                    //COMMISSARY PROCESSING
                    Int32 CountPurchRecordsCommissary = 0;
                    Int32 SkippedRecordsCommissary = 0;
                    if (CommissaryCreatePurchaseRecords(out CountPurchRecordsCommissary, out SkippedRecordsCommissary) == true)
                    {
                        //if successful clear out the temp table
                        //Otherwise it may be useful for troubleshooting
                        CommissaryClearXTRACHEFImportTable();

                        TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountPurchRecordsCommissary.ToString(CultureInfo.CurrentCulture) + " Commissary Transmittal record" + (CountPurchRecordsCommissary == 1 ? " was" : "s were") + " created";
                        TextBoxStatus.Rows++;

                        if (SkippedRecordsCommissary > 0)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Duplicate Found. " + SkippedRecordsCommissary.ToString(CultureInfo.CurrentCulture) + " Commissary Transmittal" + (SkippedRecordsCommissary == 1 ? "" : "s") + " already existed and " + (SkippedRecordsCommissary == 1 ? "was" : "were") + " skipped.";
                            TextBoxStatus.Rows++;
                        }

                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountPurchRecordsCommissary.ToString(CultureInfo.CurrentCulture) + " Commissary Transmittal record" + (CountPurchRecordsCommissary == 1 ? " was" : "s were") + " created";
                        TextBoxStatus.Rows++;
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

        private void ClearXTRACHEFImportTable()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "TRUNCATE TABLE [IMPORT_XTRACHEF]";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {

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

        private void CommissaryClearXTRACHEFImportTable()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "TRUNCATE TABLE [IMPORT_XTRACHEF]";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {

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

        private static string[] DataRowParser(string csvRow)
        {
            string csv = csvRow;

            TextFieldParser parser = new TextFieldParser(new StringReader(csv));

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            string[] fields = Array.Empty<string>();//new string [0];

            while (!parser.EndOfData)
            {
                fields = parser.ReadFields();
            }

            parser.Close();

            return fields;
        }

        /// <summary>
        /// Parse the row string and create the Import Table record
        /// </summary>
        /// <param name="row">Row string with comma separated values</param>
        /// <returns>Boolean indicating if there were errors</returns>
        private Boolean CreateImportRecord(string row, ref string Errorhint)
        {
            Boolean RecordCreated = false;

            DateTime InvoiceDate;
            DateTime PostDate;
            DateTime PostDateLater;
            DateTime UsePostDate;
            String InvNum;
            Decimal InvoiceTotal;
            Decimal LineTotal;
            Decimal Food = 0M;
            Decimal Paper = 0M;
            Decimal Clean = 0M;
            Decimal Other = 0M;
            Decimal Milk = 0M;
            String DistrictID;
            String acctCode = "";
            String VendorID = "";
            short SchlNum = 0;
            string OtherCategory = "NONE";

            //CSV file has columns encapsulated in quotes.
            //string[] stringSeparators = new string[] { "\",\""};
            //string[] rowArray = row.Split(stringSeparators, StringSplitOptions.None);
            ////if rows are not encapsulated in quotes - try without quotes
            //if(rowArray.Length==1)
            //    rowArray = row.Split(',');

            string[] rowArray2 = DataRowParser(row);

            if (DateTime.TryParse(rowArray2[11].Replace("\"", ""), out InvoiceDate) == false)
            {
                Errorhint = "Bad invoice date.";
                return false;
            }

            if (DateTime.TryParse(TextBoxPostingDate.Text, out PostDate) == false)
            {
                Errorhint = "Bad Post Date.";
                return false;
            }

            if (DateTime.TryParse(TextBoxMonthEndDate.Text, out PostDateLater) == false)
            {
                Errorhint = "Bad Month End Date";
                return false;
            }

            if (InvoiceDate > PostDate)
                UsePostDate = PostDateLater;
            else
                UsePostDate = PostDate;



            InvNum = rowArray2[10].Replace("\"", "");
            if (string.IsNullOrEmpty(InvNum))
            {
                Errorhint = "No Invoice number.";
                return false;
            }

            if (Decimal.TryParse(rowArray2[14].Replace("\"", ""), out InvoiceTotal) == false)
            {
                Errorhint = "Error with invoice total.";
                return false;
            }

            if (Decimal.TryParse(rowArray2[19].Replace("\"", ""), out LineTotal) == false)
            {
                Errorhint = "Error with line total.";
                return false;
            }

            string docType = rowArray2[6].Replace("\"", "").Trim(' ').ToUpper();
            switch (docType)
            {
                case "C":
                    LineTotal *= -1;
                    break;
                default:
                    break;
            }

            DistrictID = rowArray2[4].Replace("\"", "");
            if (DistrictID.Length > 3)
                DistrictID = String.Empty;
            else if (DistrictID.Length > 0 && DistrictID.Length < 3)
                DistrictID = DistrictID.PadLeft(3, '0');//in case 1 or 2 digit districts aren't padded with zeros

            if (string.IsNullOrEmpty(DistrictID))
            {
                Errorhint = "Missing Location Code.";
                return false;
            }


            acctCode = rowArray2[15].Replace("\"", "");

            switch (acctCode)
            {
                case "5000":
                    Food = LineTotal;
                    break;
                case "5002":
                    Paper = LineTotal;
                    break;
                case "5003":
                    Clean = LineTotal;
                    break;
                //case "5005":
                //    Other = LineTotal;
                //    break;
                case "5006":
                    Milk = LineTotal;
                    break;
                default://all other
                    Other = LineTotal;
                    //Check to ensure district uses acctCode
                    if (OtherCatAcctExists(DistrictID, acctCode, out OtherCategory) == false)
                    {
                        //11/2/2021 - not rejecting records anymore with wrong other cat. Just using NONE

                        //Errorhint = "Line GL code could not be read or matched.";
                        //return false;
                    }


                    break;
            }


            VendorID = rowArray2[7].Replace("\"", "");
            if (string.IsNullOrEmpty(VendorID))
            {
                Errorhint = "Missing Vendor Code.";
                return false;
            }


            SchlNum = GetMinSchool(DistrictID);
            if (SchlNum == -1)
            {
                Errorhint = "Could not find a school for district:" + DistrictID + " .";
                // return false; 
                // Will create the record with -1, the purchase importer will skip -1 records
                // needed since there are now districts with buildings numbered 0
            }

            string queryImportValues = "";

            //Check to see if the invoice already exists (Invoice / DistrictID) and update instead of insert
            if (DoesVendorInvoiceExistForDistrict(DistrictID, VendorID, InvNum))
            {
                queryImportValues = "UPDATE [IMPORT_XTRACHEF] SET [Food]=[Food]+@Food, [Paper]=[Paper]+@Paper, [Clean]=[Clean]+@Clean, [Other]=[Other]+@Other, [Milk]=[Milk]+@Milk," +
                                " OtherCatID= IIF(OtherCatID='NONE', @OtherCategoryID, OtherCatID) " //only modify if original value is NONE. This will populate with the first non-NONE value
                            + " WHERE [DistrictID]=@DistrictID AND [VendorID]=@VendorID AND [InvNum]=@InvNum ";
            }
            else
            {
                queryImportValues = "INSERT INTO [IMPORT_XTRACHEF] ([InvoiceDate],[PostDate],[InvNum],[InvTotal],[Food],[Paper],[Clean],[Other],[Milk],[DistrictID],[SchoolNum],[VendorID],[OtherCatID]) "
                            + " VALUES(@InvoiceDate,@PostDate,@InvNum,@InvTotal,@Food,@Paper,@Clean,@Other,@Milk,@DistrictID,@SchoolNum,@VendorID,@OtherCategoryID)";
            }



            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);



            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = InvoiceDate;
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

                SqlParameter dbParam_InvTotal = new SqlParameter();
                dbParam_InvTotal.ParameterName = "@InvTotal";
                dbParam_InvTotal.Value = InvoiceTotal;
                dbParam_InvTotal.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_InvTotal);

                SqlParameter dbParam_Food = new SqlParameter();
                dbParam_Food.ParameterName = "@Food";
                dbParam_Food.Value = Food;
                dbParam_Food.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Food);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = Paper;
                dbParam_Paper.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Clean";
                dbParam_Clean.Value = Clean;
                dbParam_Clean.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = Other;
                dbParam_Other.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_Milk = new SqlParameter();
                dbParam_Milk.ParameterName = "@Milk";
                dbParam_Milk.Value = Milk;
                dbParam_Milk.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Milk);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchlNum = new SqlParameter();
                dbParam_SchlNum.ParameterName = "@SchoolNum";
                dbParam_SchlNum.Value = SchlNum;
                dbParam_SchlNum.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_SchlNum);

                SqlParameter dbParam_VendID = new SqlParameter();
                dbParam_VendID.ParameterName = "@VendorID";
                dbParam_VendID.Value = VendorID;
                dbParam_VendID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_VendID);

                SqlParameter dbParam_otherCatID = new SqlParameter();
                dbParam_otherCatID.ParameterName = "@OtherCategoryID";
                dbParam_otherCatID.Value = OtherCategory;
                dbParam_otherCatID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_otherCatID);

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
        /// Parse the row string and create the Import Table record
        /// Commissary Version
        /// </summary>
        /// <param name="row">Row string with comma separated values</param>
        /// <returns>Boolean indicating if there were errors</returns>
        private Boolean CommissaryCreateImportRecord(string row, ref string Errorhint)
        {
            Boolean RecordCreated = false;

            DateTime InvoiceDate;
            DateTime PostDate;
            DateTime PostDateLater;
            DateTime UsePostDate;
            String InvNum;
            Decimal InvoiceTotal;
            Decimal LineTotal;
            Decimal Food = 0M;
            Decimal Paper = 0M;
            Decimal Clean = 0M;
            Decimal Other = 0M;
            Decimal Milk = 0M;
            String CommissaryID;
            String acctCode = "";
            String VendorID = "";
            string OtherCategory = "NONE";

            //CSV file has columns encapsulated in quotes.
            //string[] stringSeparators = new string[] { "\",\""};
            //string[] rowArray = row.Split(stringSeparators, StringSplitOptions.None);
            ////if rows are not encapsulated in quotes - try without quotes
            //if(rowArray.Length==1)
            //    rowArray = row.Split(',');

            string[] rowArray2 = DataRowParser(row);

            if (DateTime.TryParse(rowArray2[11].Replace("\"", ""), out InvoiceDate) == false)
            {
                Errorhint = "Bad invoice date.";
                return false;
            }

            if (DateTime.TryParse(TextBoxPostingDate.Text, out PostDate) == false)
            {
                Errorhint = "Bad Post Date.";
                return false;
            }

            UsePostDate = PostDate;


            InvNum = rowArray2[10].Replace("\"", "");
            if (string.IsNullOrEmpty(InvNum))
            {
                Errorhint = "No Invoice number.";
                return false;
            }

            if (Decimal.TryParse(rowArray2[14].Replace("\"", ""), out InvoiceTotal) == false)
            {
                Errorhint = "Error with invoice total.";
                return false;
            }

            if (Decimal.TryParse(rowArray2[19].Replace("\"", ""), out LineTotal) == false)
            {
                Errorhint = "Error with line total.";
                return false;
            }

            string docType = rowArray2[6].Replace("\"", "").Trim(' ').ToUpper();
            switch (docType)
            {
                case "C":
                    LineTotal *= -1;
                    break;
                default:
                    break;
            }

            CommissaryID = rowArray2[4].Replace("\"", "");
            if (CommissaryID.Length > 3)
                CommissaryID = String.Empty;
            else if (CommissaryID.Length > 0 && CommissaryID.Length < 3)
                CommissaryID = CommissaryID.PadLeft(3, '0');//in case 1 or 2 digit commissaries aren't padded with zeros

            if (string.IsNullOrEmpty(CommissaryID))
            {
                Errorhint = "Missing Location Code.";
                return false;
            }


            acctCode = rowArray2[15].Replace("\"", "");

            switch (acctCode)
            {
                case "5000":
                    Food = LineTotal;
                    break;
                case "5002":
                    Paper = LineTotal;
                    break;
                case "5003":
                    Clean = LineTotal;
                    break;
                //case "5005":
                //    Other = LineTotal;
                //    break;
                case "5006":
                    Milk = LineTotal;
                    break;
                default://all other
                    Other = LineTotal;
                    //Set the Other Category ID
                    CommissaryOtherCatAcctExists(acctCode, out OtherCategory);
                    break;
            }


            VendorID = rowArray2[7].Replace("\"", "");
            if (string.IsNullOrEmpty(VendorID))
            {
                Errorhint = "Missing Vendor Code.";
                return false;
            }


            string queryImportValues = "";

            //Check to see if the invoice already exists (Invoice / DistrictID) and update instead of insert
            if (CommissaryDoesVendorInvoiceExist(CommissaryID, VendorID, InvNum))
            {
                queryImportValues = "UPDATE [IMPORT_XTRACHEF] SET [Food]=[Food]+@Food, [Paper]=[Paper]+@Paper, [Clean]=[Clean]+@Clean, [Other]=[Other]+@Other, [Milk]=[Milk]+@Milk," +
                                " OtherCatID= IIF(OtherCatID='NONE', @OtherCategoryID, OtherCatID) " + //only modify if original value is NONE. This will populate with the first non-NONE value
                                " WHERE [CommissaryID]=@CommissaryID AND [VendorID]=@VendorID AND [InvNum]=@InvNum ";
            }
            else
            {
                queryImportValues = "INSERT INTO [IMPORT_XTRACHEF] ([InvoiceDate],[PostDate],[InvNum],[InvTotal],[Food],[Paper],[Clean],[Other],[Milk],[CommissaryID],[VendorID],[OtherCatID]) "
                            + " VALUES(@InvoiceDate,@PostDate,@InvNum,@InvTotal,@Food,@Paper,@Clean,@Other,@Milk,@CommissaryID,@VendorID,@OtherCategoryID)";
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = InvoiceDate;
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

                SqlParameter dbParam_InvTotal = new SqlParameter();
                dbParam_InvTotal.ParameterName = "@InvTotal";
                dbParam_InvTotal.Value = InvoiceTotal;
                dbParam_InvTotal.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_InvTotal);

                SqlParameter dbParam_Food = new SqlParameter();
                dbParam_Food.ParameterName = "@Food";
                dbParam_Food.Value = Food;
                dbParam_Food.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Food);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = Paper;
                dbParam_Paper.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Clean";
                dbParam_Clean.Value = Clean;
                dbParam_Clean.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = Other;
                dbParam_Other.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_Milk = new SqlParameter();
                dbParam_Milk.ParameterName = "@Milk";
                dbParam_Milk.Value = Milk;
                dbParam_Milk.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Milk);

                SqlParameter dbParam_CommID = new SqlParameter();
                dbParam_CommID.ParameterName = "@CommissaryID";
                dbParam_CommID.Value = CommissaryID;
                dbParam_CommID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_CommID);

                SqlParameter dbParam_VendID = new SqlParameter();
                dbParam_VendID.ParameterName = "@VendorID";
                dbParam_VendID.Value = VendorID;
                dbParam_VendID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_VendID);

                SqlParameter dbParam_otherCatID = new SqlParameter();
                dbParam_otherCatID.ParameterName = "@OtherCategoryID";
                dbParam_otherCatID.Value = OtherCategory;
                dbParam_otherCatID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_otherCatID);

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


        private bool OtherCatAcctExists(string districtID, string acctCode, out string OUTOtherCode)
        {
            bool ExpenseAccountConfigured = false;
            OUTOtherCode = "NONE";//default value

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            string queryForExpCode = "Select OtherCategoryID FROM [OtherPurchaseCategories] WHERE [ProgramYear]=@ProgramYear AND OtherCategoryID NOT IN ('STORE','USDAP') " + // 11/3/2021 - Multiple codes setup for acct 5004. Need to force to USDAD if coded.
                "AND [OtherCategoryID] IN (SELECT [OtherCategoryID] FROM [DistrictAllowedOtherCats] WHERE ProgramYear = @ProgramYear AND DistrictID = @DistrictID AND OtherCatExpCode = @OtherCatExpCode)";


            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryForExpCode;
                cmd.Connection = conn;

                SqlParameter dbParam_YearID = new System.Data.SqlClient.SqlParameter();
                dbParam_YearID.ParameterName = "@ProgramYear";
                dbParam_YearID.Value = DDLProgramYears.SelectedValue;
                dbParam_YearID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_YearID);

                SqlParameter dbParam_distID = new System.Data.SqlClient.SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = districtID;
                dbParam_distID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_acctCode = new System.Data.SqlClient.SqlParameter();
                dbParam_acctCode.ParameterName = "@OtherCatExpCode";
                dbParam_acctCode.Value = acctCode;
                dbParam_acctCode.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_acctCode);

                conn.Open();
                try
                {
                    object otherCatObj = cmd.ExecuteScalar();

                    if (otherCatObj != null)
                    {
                        ExpenseAccountConfigured = true;
                        OUTOtherCode = otherCatObj.ToString();
                    }
                    else
                        ExpenseAccountConfigured = false;


                }
                catch (Exception)
                {
                    ExpenseAccountConfigured = false;
                }
                finally
                {
                    conn.Close();
                }
            }

            return ExpenseAccountConfigured;
        }

        private bool CommissaryOtherCatAcctExists(string acctCode, out string OUTOtherCode)
        {
            bool ExpenseAccountConfigured = false;
            OUTOtherCode = "NONE";//default value

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            string queryForExpCode = "Select OtherCategoryID FROM [OtherPurchaseCategories] WHERE OtherCatExpCode=@OtherCatExpCode ";

            using (SqlCommand cmd = new SqlCommand())
            {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                cmd.CommandText = queryForExpCode;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                cmd.Connection = conn;

                SqlParameter dbParam_acctCode = new System.Data.SqlClient.SqlParameter();
                dbParam_acctCode.ParameterName = "@OtherCatExpCode";
                dbParam_acctCode.Value = acctCode;
                dbParam_acctCode.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_acctCode);

                conn.Open();
                try
                {
                    object otherCatObj = cmd.ExecuteScalar();

                    if (otherCatObj != null)
                    {
                        ExpenseAccountConfigured = true;
                        OUTOtherCode = otherCatObj.ToString();
                    }
                    else
                        ExpenseAccountConfigured = false;


                }
                catch (Exception)
                {
                    ExpenseAccountConfigured = false;
                }
                finally
                {
                    conn.Close();
                }
            }

            return ExpenseAccountConfigured;
        }


        private bool DoesVendorInvoiceExistForDistrict(string districtID, string vendorID, string invNum)
        {
            bool InvoiceExists = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            string queryCheckForInvoice = "SELECT COUNT(*) FROM [IMPORT_XTRACHEF] WHERE DistrictID=@DistrictID AND VendorID=@VendorID AND InvNum=@InvNum";


            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryCheckForInvoice;
                cmd.Connection = conn;

                SqlParameter dbParam_distID = new System.Data.SqlClient.SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = districtID;
                dbParam_distID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_vendID = new System.Data.SqlClient.SqlParameter();
                dbParam_vendID.ParameterName = "@VendorID";
                dbParam_vendID.Value = vendorID;
                dbParam_vendID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_vendID);

                SqlParameter dbParam_invNum = new System.Data.SqlClient.SqlParameter();
                dbParam_invNum.ParameterName = "@InvNum";
                dbParam_invNum.Value = invNum;
                dbParam_invNum.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_invNum);

                conn.Open();
                try
                {
                    object countObj = cmd.ExecuteScalar();

                    if (int.TryParse(countObj.ToString(), out int intCount) == true)
                    {
                        if (intCount > 0)
                        {
                            InvoiceExists = true;
                        }
                    }
                    else
                        InvoiceExists = false;


                }
                catch (Exception)
                {

                }
                finally
                {
                    conn.Close();
                }
            }

            return InvoiceExists;
        }

        private bool CommissaryDoesVendorInvoiceExist(string commissaryID, string vendorID, string invNum)
        {
            bool InvoiceExists = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            string queryCheckForInvoice = "SELECT COUNT(*) FROM [IMPORT_XTRACHEF] WHERE CommissaryID=@CommissaryID AND VendorID=@VendorID AND InvNum=@InvNum";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryCheckForInvoice;
                cmd.Connection = conn;

                SqlParameter dbParam_distID = new System.Data.SqlClient.SqlParameter();
                dbParam_distID.ParameterName = "@CommissaryID";
                dbParam_distID.Value = commissaryID;
                dbParam_distID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_vendID = new System.Data.SqlClient.SqlParameter();
                dbParam_vendID.ParameterName = "@VendorID";
                dbParam_vendID.Value = vendorID;
                dbParam_vendID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_vendID);

                SqlParameter dbParam_invNum = new System.Data.SqlClient.SqlParameter();
                dbParam_invNum.ParameterName = "@InvNum";
                dbParam_invNum.Value = invNum;
                dbParam_invNum.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_invNum);

                conn.Open();
                try
                {
                    object countObj = cmd.ExecuteScalar();

                    if (int.TryParse(countObj.ToString(), out int intCount) == true)
                    {
                        if (intCount > 0)
                        {
                            InvoiceExists = true;
                        }
                    }
                    else
                        InvoiceExists = false;


                }
                catch (Exception)
                {

                }
                finally
                {
                    conn.Close();
                }
            }

            return InvoiceExists;
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

            string queryImportValues = "SELECT * FROM IMPORT_XTRACHEF WHERE DistrictID!='' AND SchoolNum!=-1 ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //If the checkbox is marked to skip duplicates, run the check and skip as needed.
                        if (CBSkipDuplicates.Checked)
                        {
                            if (PurchasesDoesInvExist(DDLProgramYears.SelectedValue, rdr["DistrictID"].ToString(), rdr["VendorID"].ToString(), rdr["InvNum"].ToString()) == true)
                            {
                                duplicatesSkipped++;
                                continue;//read the next record
                            }
                        }

                        //If vendor is missing - do not create the records
                        if (string.IsNullOrEmpty(rdr["VendorID"].ToString()) == false)
                        {
                            if(PurchasesDoesVendorExist(rdr["DistrictID"].ToString(), rdr["VendorID"].ToString()) ==false)
                            {
                                TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: District " + rdr["DistrictID"].ToString() + " does not have a vendor: " + rdr["VendorID"].ToString() + " setup. The transmittal was still created, and the vendor will need to be added for this district or corrected on the transmittal.";
                                TextBoxStatus.Rows++;
                            }

                            if (PurchasesInsertRecord(rdr["DistrictID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], rdr["VendorID"].ToString(), rdr["InvNum"].ToString(), (Decimal)rdr["Food"]
                                , (Decimal)rdr["Paper"], (Decimal)rdr["Clean"], (Decimal)rdr["Other"], rdr["OtherCatID"].ToString(), "", (Decimal)rdr["Milk"], (Int16)rdr["SchoolNum"], DDLProgramYears.SelectedValue
                                , ref TextBoxStatus, CB_VerboseErrors.Checked, 0, "EXTRACHEFIMPORTER", "School Year") == true)
                            {
                                recordCount++;
                            }
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: District " + rdr["DistrictID"].ToString() + " had a record with a blank vendor and was skipped.";
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
        /// Cycles through the import table records and creates records in the Commissary Purchases table 
        /// </summary>
        /// <returns>Boolean indicating success</returns>
        protected Boolean CommissaryCreatePurchaseRecords(out Int32 recordCount, out Int32 duplicatesSkipped)
        {
            Boolean success = false;
            recordCount = 0;
            duplicatesSkipped = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            //Filter to existing commissaries
            string queryImportValues = "SELECT * FROM IMPORT_XTRACHEF WHERE CommissaryID IN (SELECT CommissaryID FROM [Commissaries]) ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //If the checkbox is marked to skip duplicates, run the check and skip as needed.
                        if (CBSkipDuplicates.Checked)
                        {
                            if (CommissaryPurchasesDoesInvExist(rdr["CommissaryID"].ToString(), rdr["VendorID"].ToString(), rdr["InvNum"].ToString()) == true)
                            {
                                duplicatesSkipped++;
                                continue;//read the next record
                            }
                        }

                        //If vendor is missing - do not create the records
                        if (string.IsNullOrEmpty(rdr["VendorID"].ToString()) == false)
                        {
                            if (CommissaryPurchasesInsertRecord(rdr["CommissaryID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], rdr["VendorID"].ToString(), rdr["InvNum"].ToString(), (Decimal)rdr["Food"]
                                , (Decimal)rdr["Paper"], (Decimal)rdr["Clean"], (Decimal)rdr["Other"], rdr["OtherCatID"].ToString(), "", ref TextBoxStatus, CB_VerboseErrors.Checked, "EXTRACHEFIMPORTER", (Decimal)rdr["Milk"], 0) == true)
                            {
                                recordCount++;
                            }
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: Commissary " + rdr["CommissaryID"].ToString() + " does not have a vendor setup and was skipped.";
                            TextBoxStatus.Rows++;
                        }
                    }

                    success = true;
                }
                catch (SqlException ex)
                {
                    success = false;

                    TextBoxStatus.Text += Environment.NewLine + "-An SQL error occurred while creating Commissary Purchasing records.";
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

                    TextBoxStatus.Text += Environment.NewLine + "-An error occurred while creating Commissary Purchasing records.";
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
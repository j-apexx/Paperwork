using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportReinhart : Paperwork_Global
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

            if(!IsPostBack)
            {
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
            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;

            if (FileUpload1.PostedFile != null && FileUpload1.PostedFile.FileName!=string.Empty)
            {
                try
                {
                    //clear out the temp table [IMPORT_RFS]
                    ClearRFSImportTable();
                    CommissaryClearRFSImportTable();

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

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {

                            //Parse the import row and creates the import record in [IMPORT_RFS] which holds temporary records
                            if (CreateImportRecord(row) == false)
                            {
                                //record creation failed - need to decide what to do
                                TextBoxStatus.Text += Environment.NewLine + "-[PAPERWORK]     Import failed at the following record:";
                                TextBoxStatus.Rows++;
                                TextBoxStatus.Text += Environment.NewLine + "  " + row;
                                TextBoxStatus.Rows++;

                                importErrors = true;
                                break;//exit out of the foreach loop of records
                            }
                            else
                                recordCount++;

                            //Create the record in the Commissary Database
                            if (CommissaryCreateImportRecord(row) == false)
                            {
                                //record creation failed - need to decide what to do
                                TextBoxStatus.Text += Environment.NewLine + "-[COMMISSARYWEB] Import failed at the following record:";
                                TextBoxStatus.Rows++;
                                TextBoxStatus.Text += Environment.NewLine + "  " + row;
                                TextBoxStatus.Rows++;

                                break;//exit out of the foreach loop of records
                            }

                        }
                    }

                    //No Errors
                    if (importErrors == false)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + recordCount.ToString() + " Records from the import file.";
                        TextBoxStatus.Rows++;
                    }

                    try
                    {
                        File.Delete(path);
                    }
                    catch { }//silent catch

                    //Update the DistrictID and SchoolNums in [IMPORT_RFS] Paperwork
                    TextBoxStatus.Text += Environment.NewLine + "-Updating the Paperwork Districts and SchoolNums.";
                    TextBoxStatus.Rows++;
                    UpdateImportDivisionsAndSchools();

                    //Update the CommissaryID in [IMPORT_RFS] CommissaryWeb
                    TextBoxStatus.Text += Environment.NewLine + "-Updating the CommisaryWeb Commissaries.";
                    TextBoxStatus.Rows++;
                    CommissaryUpdateImportDivisions();



                    //Validate School and Districts
                    //Per Tom - no need to stop import for unmapped schools as files can contain commissaries - informational only
                    //Updates the textbox with the informational details
                    CheckForMissingDivSchools();

                    //****** If we are going to integrate with the Commissaries - do it here **
                    // Not needed to run this check for missing divs
                    // CommissaryCheckForMissingDivs();

                    //New 6/28/2021 - removing the non-program year records
                    int removedRecords = 0;
                    int removedCommissaryRecords = 0;

                    RemoveNonProgramYearRecords(out removedRecords);
                    RemoveNonProgYearRecordsComm(out removedCommissaryRecords);

                    if((removedRecords) >0)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Skipped " + removedRecords.ToString() + " Paperwork invoices which fell outside program year dates.";
                        TextBoxStatus.Rows++;
                    }

                    if ((removedCommissaryRecords) > 0)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Skipped " + (removedCommissaryRecords).ToString() + " Commissary invoices which fell outside program year dates.";
                        TextBoxStatus.Rows++;
                    }


                    //Finally - Create the Purch records
                    Int32 CountPurchRecords = 0;
                    
                    
                    if(CreatePurchaseRecords(out CountPurchRecords) ==true)
                    {
                        //if successful clear out the temp table
                        //Otherwise it may be useful for troubleshooting
                        ClearRFSImportTable();

                        TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountPurchRecords.ToString() + " Paperwork Transmittal records were created";
                        TextBoxStatus.Rows++;

                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountPurchRecords.ToString() + " Paperwork Transmittal records were created";
                        TextBoxStatus.Rows++;
                    }

                    Int32 CountCommissaryPurchRecords = 0;

                    if (CommissaryCreatePurchaseRecords(out CountCommissaryPurchRecords) == true)
                    {
                        //if successful clear out the temp table in Commissary DB
                        //Otherwise it may be useful for troubleshooting
                        CommissaryClearRFSImportTable();

                        TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountCommissaryPurchRecords.ToString() + " Commissary Purchase records were created";
                        TextBoxStatus.Rows++;

                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountCommissaryPurchRecords.ToString() + " Commissary Purchase records were created";
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

        private void RemoveNonProgYearRecordsComm(out int removedCommissaryRecords)
        {
            removedCommissaryRecords = 0;

            if (DateTime.TryParse(HFProgYearStartDate.Value, out DateTime PyStartDate) == false)
                return;
            if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PyEndDate) == false)
                return;


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryRemoveNonPYTrxs = "DELETE FROM [IMPORT_RFS] WHERE [CommissaryID]!='' AND ( [InvoiceDate]<@ProgramYearStartDate OR [InvoiceDate]>@ProgramYearEndDate) ";

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
                    removedCommissaryRecords=cmd.ExecuteNonQuery();
                }
                catch(SqlException ex)
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

            string queryRemoveNonPYTrxs = "DELETE FROM [IMPORT_RFS] WHERE DistrictID!='' AND SchoolNum!=0 AND ([InvoiceDate]<@ProgramYearStartDate OR [InvoiceDate]>@ProgramYearEndDate) ";

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
        private Boolean CreateImportRecord(string row)
        {
            Boolean RecordCreated = false;

            DateTime InvoiceDate;
            DateTime PostDate;
            DateTime PostDateLater;
            DateTime UsePostDate;
            String InvNum;
            Int32 RFSCustomerNum;
            Int16 USDAFlag;
            Decimal InvoiceTotal;
            Decimal Food;
            Decimal Paper;
            Decimal Clean;
            Decimal Other;
            Decimal Promo;
            Decimal NOI;
            String RFSDivision;
            String DistrictID;
            Decimal SalesTax;

            string[] rowArray = row.Split(',');

            if (DateTime.TryParse(rowArray[0], out InvoiceDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxPostingDate.Text, out PostDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxMonthEndDate.Text, out PostDateLater) == false)
                return false;

            if (InvoiceDate > PostDate)
                UsePostDate = PostDateLater;
            else
                UsePostDate = PostDate;


            InvNum = rowArray[1];
            if (string.IsNullOrEmpty(InvNum))
                return false;

            if(Int32.TryParse(rowArray[2], out RFSCustomerNum) == false)
                return false;

            if (Int16.TryParse(rowArray[3], out USDAFlag) == false)
                return false;

            if (Decimal.TryParse(rowArray[4], out InvoiceTotal) == false)
                return false;

            if (Decimal.TryParse(rowArray[5], out Food) == false)
                return false;

            if (Decimal.TryParse(rowArray[6], out Paper) == false)
                return false;

            if (Decimal.TryParse(rowArray[7], out Clean) == false)
                return false;

            if (Decimal.TryParse(rowArray[8], out Other) == false)
                return false;

            if (Decimal.TryParse(rowArray[9], out Promo) == false)
                return false;

            if (Decimal.TryParse(rowArray[10], out NOI) == false)
                return false;

            RFSDivision = rowArray[11];
            if (string.IsNullOrEmpty(RFSDivision))
                return false;

            DistrictID = rowArray[12];
            if (DistrictID.Length > 3)
                DistrictID = String.Empty;

            if (Decimal.TryParse(rowArray[13], out SalesTax) == false)
                return false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [IMPORT_RFS] ([InvoiceDate],[PostDate],[InvNum],[RFSCustNum],[USDAFlag],[InvTotal],[Food],[Paper],[Clean], "
                                        + " [Other],[Promo],[NOI],[RFSDivision],[SalesTax],[DistrictID],[SchoolNum]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@RFSCustNum,@USDAFlag,@InvTotal,@Food,@Paper,@Clean,"
                                        + " @Other,@Promo,@NOI,@RFSDivision,@SalesTax,'', 0 )";

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

                SqlParameter dbParam_CustNum = new SqlParameter();
                dbParam_CustNum.ParameterName = "@RFSCustNum";
                dbParam_CustNum.Value = RFSCustomerNum;
                dbParam_CustNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_CustNum);

                SqlParameter dbParam_USDAFLAG = new SqlParameter();
                dbParam_USDAFLAG.ParameterName = "@USDAFlag";
                dbParam_USDAFLAG.Value = USDAFlag;
                dbParam_USDAFLAG.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_USDAFLAG);

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

                SqlParameter dbParam_Promo = new SqlParameter();
                dbParam_Promo.ParameterName = "@Promo";
                dbParam_Promo.Value = Promo;
                dbParam_Promo.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Promo);

                SqlParameter dbParam_NOI = new SqlParameter();
                dbParam_NOI.ParameterName = "@NOI";
                dbParam_NOI.Value = NOI;
                dbParam_NOI.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_NOI);

                SqlParameter dbParam_RFSDiv = new SqlParameter();
                dbParam_RFSDiv.ParameterName = "@RFSDivision";
                dbParam_RFSDiv.Value = RFSDivision;
                dbParam_RFSDiv.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_RFSDiv);

                SqlParameter dbParam_SalesTax = new SqlParameter();
                dbParam_SalesTax.ParameterName = "@SalesTax";
                dbParam_SalesTax.Value = SalesTax;
                dbParam_SalesTax.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_SalesTax);

                try
                {
                    dbConnection.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                        RecordCreated = true;
                    else
                        RecordCreated = false;

                }
                catch(SqlException ex)
                {
                    if(ex.Number == 2627)//Duplicate record - Tom said there may be duplicates in a file
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
        /// Clear out the RFSImport SQL table
        /// </summary>
        private void ClearRFSImportTable()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "TRUNCATE TABLE [IMPORT_RFS]";

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
            // +
                
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = " UPDATE [IMPORT_RFS] SET DistrictID=R.DistrictID , SchoolNum=R.SchoolNum " +
                " FROM [IMPORT_RFS] I " +
                " INNER JOIN [RFSCustomerNumbers] R ON R.RFSCustNum = I.RFSCustNum AND R.Division = I.RFSDivision; ";

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

            string queryImportValues = "SELECT DISTINCT 'Division:'+ RFSDivision + ', ' + 'CustNum:' + RTRIM(CAST(RFSCustNum AS VARCHAR(10))) FROM IMPORT_RFS WHERE DistrictID='' OR SchoolNum=0";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        if(missingDivSchools==false)
                        {
                            missingDivSchools = true;
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: The following RFS Customer Numbers could not be resolved to a District ID and School Number.";
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

        /// <summary>
        /// Cycles through the import table records and creates records in the Purchases table 
        /// </summary>
        /// <returns></returns>
        protected Boolean CreatePurchaseRecords(out Int32 recordCount)
        {
            Boolean success = false;
            recordCount = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT * FROM IMPORT_RFS WHERE DistrictID!='' AND SchoolNum!=0 ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string _vndrID="";//	○ VendorNum: IF USDAFLAG=1 then retrieve VendorNum for the unit where VendorID='REIUSD' for unitOtherwise - retrieve vendornum for the other VendorID's
                        string _otherCat = ""; // 	○ OCIndex - if USDA Flag=1 then 37 else (if Other>0, then 24 else 1)

                        Decimal _food = 0M;
                        Decimal _other = 0M;

                        if ((Int16)rdr["USDAFlag"] == 0)
                        {
                            _food = (Decimal)rdr["Food"];
                            _other = (Decimal)rdr["Other"];
                            _vndrID = getReiVendorID(rdr["DistrictID"].ToString(), rdr["RFSDivision"].ToString());

                            if (_other > 0)
                                _otherCat = "SMALL";//SMALLWARES
                            else
                                _otherCat = "NONE";//NONE
                        }
                        else
                        {
                            _other= (Decimal)rdr["Food"];
                            _food = 0M;
                            _vndrID = "REIUSD";//Reinhart USDA VendorID
                            _otherCat = "USDAD";//USDA DELIVERY
                        }

                        //If the District does not have the Reinhart vendor setup - do not create the records
                        if (_vndrID != String.Empty)
                        {
                            if(PurchasesInsertRecord(rdr["DistrictID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), _food
                                , (Decimal)rdr["Paper"], (Decimal)rdr["Clean"], _other, _otherCat, "", 0M, (Int16)rdr["SchoolNum"], DDLProgramYears.SelectedValue
                                , ref TextBoxStatus, CB_VerboseErrors.Checked, 0)==true)
                            {
                                recordCount++;
                            }
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: District " + rdr["DistrictID"].ToString() + " does not have a Reinhart vendor setup and was skipped.";
                            TextBoxStatus.Rows++;
                        }
                    }

                    success = true;
                }
                catch(SqlException ex)
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
                    if(rdr.Read())
                    {
                        ReturnVendorID = rdr[0].ToString();
                    }

                    //Then, if the previously set value doesn't match the RFSDivision from the file - see if the District is setup with the match
                    //otherwise, we'll just pass back the Reinhart VendorID they are configured for
                    while(rdr.Read() && ReturnVendorID != PreferredREIVndr)
                    {
                        if(rdr[0].ToString()== PreferredREIVndr)
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
        private Boolean CommissaryCreateImportRecord(string row)
        {
            Boolean RecordCreated = false;

            DateTime InvoiceDate;
            DateTime PostDate;
            DateTime PostDateLater;
            DateTime UsePostDate;
            String InvNum;
            Int32 RFSCustomerNum;
            Int16 USDAFlag;
            Decimal InvoiceTotal;
            Decimal Food;
            Decimal Paper;
            Decimal Clean;
            Decimal Other;
            Decimal Promo;
            Decimal NOI;
            String RFSDivision;
            String CommissaryID;
            Decimal SalesTax;

            string[] rowArray = row.Split(',');

            if (DateTime.TryParse(rowArray[0], out InvoiceDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxPostingDate.Text, out PostDate) == false)
                return false;

            if (DateTime.TryParse(TextBoxMonthEndDate.Text, out PostDateLater) == false)
                return false;

            if (InvoiceDate > PostDate)
                UsePostDate = PostDateLater;
            else
                UsePostDate = PostDate;


            InvNum = rowArray[1];
            if (string.IsNullOrEmpty(InvNum))
                return false;

            if (Int32.TryParse(rowArray[2], out RFSCustomerNum) == false)
                return false;

            if (Int16.TryParse(rowArray[3], out USDAFlag) == false)
                return false;

            if (Decimal.TryParse(rowArray[4], out InvoiceTotal) == false)
                return false;

            if (Decimal.TryParse(rowArray[5], out Food) == false)
                return false;

            if (Decimal.TryParse(rowArray[6], out Paper) == false)
                return false;

            if (Decimal.TryParse(rowArray[7], out Clean) == false)
                return false;

            if (Decimal.TryParse(rowArray[8], out Other) == false)
                return false;

            if (Decimal.TryParse(rowArray[9], out Promo) == false)
                return false;

            if (Decimal.TryParse(rowArray[10], out NOI) == false)
                return false;

            RFSDivision = rowArray[11];
            if (string.IsNullOrEmpty(RFSDivision))
                return false;

            CommissaryID = rowArray[12];
            if (CommissaryID.Length > 3)
                CommissaryID = String.Empty;

            if (Decimal.TryParse(rowArray[13], out SalesTax) == false)
                return false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [IMPORT_RFS] ([InvoiceDate],[PostDate],[InvNum],[RFSCustNum],[USDAFlag],[InvTotal],[Food],[Paper],[Clean], "
                                        + " [Other],[Promo],[NOI],[RFSDivision],[SalesTax],[CommissaryID],[SchoolNum]) "
                                        + " VALUES(@InvoiceDate,@PostDate,@InvNum,@RFSCustNum,@USDAFlag,@InvTotal,@Food,@Paper,@Clean,"
                                        + " @Other,@Promo,@NOI,@RFSDivision,@SalesTax,'', 0 )";

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

                SqlParameter dbParam_CustNum = new SqlParameter();
                dbParam_CustNum.ParameterName = "@RFSCustNum";
                dbParam_CustNum.Value = RFSCustomerNum;
                dbParam_CustNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_CustNum);

                SqlParameter dbParam_USDAFLAG = new SqlParameter();
                dbParam_USDAFLAG.ParameterName = "@USDAFlag";
                dbParam_USDAFLAG.Value = USDAFlag;
                dbParam_USDAFLAG.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_USDAFLAG);

                SqlParameter dbParam_InvTotal = new SqlParameter();
                dbParam_InvTotal.ParameterName = "@InvTotal";
                dbParam_InvTotal.Value = InvoiceTotal;
                dbParam_InvTotal.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_InvTotal);

                SqlParameter dbParam_Food = new SqlParameter();
                dbParam_Food.ParameterName = "@Food";
                dbParam_Food.Value = (Food + Paper + Clean);//3/12/2021 - Tom wanted Paper & Cleaning combined into Food
                dbParam_Food.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Food);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = 0M;// Paper;
                dbParam_Paper.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Clean";
                dbParam_Clean.Value = 0M;// Clean;
                dbParam_Clean.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = Other;
                dbParam_Other.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_Promo = new SqlParameter();
                dbParam_Promo.ParameterName = "@Promo";
                dbParam_Promo.Value = Promo;
                dbParam_Promo.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Promo);

                SqlParameter dbParam_NOI = new SqlParameter();
                dbParam_NOI.ParameterName = "@NOI";
                dbParam_NOI.Value = NOI;
                dbParam_NOI.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_NOI);

                SqlParameter dbParam_RFSDiv = new SqlParameter();
                dbParam_RFSDiv.ParameterName = "@RFSDivision";
                dbParam_RFSDiv.Value = RFSDivision;
                dbParam_RFSDiv.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_RFSDiv);

                SqlParameter dbParam_SalesTax = new SqlParameter();
                dbParam_SalesTax.ParameterName = "@SalesTax";
                dbParam_SalesTax.Value = SalesTax;
                dbParam_SalesTax.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_SalesTax);

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
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = " UPDATE [IMPORT_RFS] SET CommissaryID=R.CommissaryID " +
                " FROM [IMPORT_RFS] I " +
                " INNER JOIN [RFSCustomerNumbers] R ON R.RFSCustNum = I.RFSCustNum AND R.Division = I.RFSDivision; ";

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
        /// Checks the import table for any records without a Commissary . Returns boolean indicating if records are missing school/div
        /// </summary>
        /// <returns>Boolean indicating if there are any records without commissary specified</returns>
        protected Boolean CommissaryCheckForMissingDivs()
        {
            Boolean missingDivs = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT DISTINCT 'Division:'+ RFSDivision + ', ' + 'CustNum:' + RTRIM(CAST(RFSCustNum AS VARCHAR(10))) FROM IMPORT_RFS WHERE CommissaryID='' ";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (missingDivs == false)
                        {
                            missingDivs = true;
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: The following RFS Customer Numbers could not be resolved to a Commissary ID.";
                            TextBoxStatus.Rows++;
                        }

                        TextBoxStatus.Text += Environment.NewLine + rdr[0].ToString();
                        TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    missingDivs = true;

                    TextBoxStatus.Text += Environment.NewLine + "-An error occurred during Commissary validation.";
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

            return missingDivs;
        }

        /// <summary>
        /// Cycles through the import table records and creates records in the Commissary Purchases table 
        /// </summary>
        /// <returns></returns>
        protected Boolean CommissaryCreatePurchaseRecords(out Int32 recordCount)
        {
            Boolean success = false;
            recordCount = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT * FROM IMPORT_RFS WHERE CommissaryID!='' ";

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

                        Decimal _food = 0M;
                        Decimal _other = 0M;

                        _vndrID = CommissarygetReiVendorID(rdr["CommissaryID"].ToString(), rdr["RFSDivision"].ToString());

                        if ((Int16)rdr["USDAFlag"] == 0)
                        {
                            _food = (Decimal)rdr["Food"];
                            _other = (Decimal)rdr["Other"];


                            if (_other > 0)
                                _otherCat = "MINOR";//MINOR EQUIPMENT
                            else
                                _otherCat = "NONE";//NONE
                        }
                        else
                        {
                            _other = (Decimal)rdr["Food"] + (Decimal)rdr["Other"];
                            _food = 0M;
                            _otherCat = "NONE";//NONE
                        }

                        //If the Commissary does not have the Reinhart vendor setup - do not create the records
                        if (_vndrID != String.Empty)
                        {
                            if (CommissaryPurchasesInsertRecord(rdr["CommissaryID"].ToString(), (DateTime)rdr["InvoiceDate"], (DateTime)rdr["PostDate"], _vndrID, rdr["InvNum"].ToString(), _food
                                , (Decimal)rdr["Paper"], (Decimal)rdr["Clean"], _other, _otherCat, "", CB_VerboseErrors.Checked) == true)
                            {
                                recordCount++;
                            }
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-INFO ONLY: Commissary " + rdr["CommissaryID"].ToString() + " does not have a Reinhart vendor setup and was skipped.";
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

        private string CommissarygetReiVendorID(string CommissaryID, string PreferredREIVndr)
        {
            string ReturnVendorID = string.Empty;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "SELECT VendorID FROM CommissaryVendors Where CommissaryID=@CommissaryID AND VendorID IN ('R','REI204','REIV','REIT','REIN')";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_commID = new SqlParameter();
                dbParam_commID.ParameterName = "@CommissaryID";
                dbParam_commID.Value = CommissaryID;
                dbParam_commID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_commID);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    //Set return value to first record regardless of value
                    if (rdr.Read())
                    {
                        ReturnVendorID = rdr[0].ToString();
                    }

                    //Then, if the previously set value doesn't match the RFSDivision from the file - see if the Commissary is setup with the match
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
        /// Creates a record with the passed in values in the Purchases table
        /// </summary>
        /// <param name="commID">Commissary ID</param>
        /// <param name="invDate">Invoice Date</param>
        /// <param name="postDate">Post Date</param>
        /// <param name="vendID">Vendor ID</param>
        /// <param name="invNum">Invoice Number</param>
        /// <param name="groc">Grocery Amount</param>
        /// <param name="paper">Paper Amount</param>
        /// <param name="clean">Cleaning Amount</param>
        /// <param name="other">Other Amount</param>
        /// <param name="otherCat">Other Category</param>
        /// <param name="comments">Comments</param>
        /// <param name="TextBoxStatus">reference to the Status TextBox</param>
        /// <param name="VerboseErrors">Return verbose errors or not</param>
        /// <returns>Boolean indicating if the record was created</returns>
        protected Boolean CommissaryPurchasesInsertRecord(String commID, DateTime invDate, DateTime postDate, String vendID, String invNum, Decimal groc, Decimal paper, Decimal clean, Decimal other,
            String otherCat, String comments, Boolean VerboseErrors)
        {
            Boolean RecordCreated = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [Purchases] ([CommissaryID],[InvoiceDate],[PostDate],[VendorID],[InvoiceNumber],[Groceries],[Paper],[Cleaning],[Other] " 
                                        + " ,[OtherCategoryID],[PurchaseComments],[ImpToCommissaryweb],[ImpToCommissarywebDate]) "
                                        + " VALUES(@CommissaryID,@InvoiceDate,@PostDate,@VendorID,@InvoiceNumber,@Groceries,@Paper,@Cleaning,@Other " 
                                        + " ,@OtherCategoryID,@PurchaseComments, 1, GETDATE() )";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_comm = new SqlParameter();
                dbParam_comm.ParameterName = "@CommissaryID";
                dbParam_comm.Value = commID;
                dbParam_comm.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_comm);

                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = invDate;
                dbParam_InvDate.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_PostDate = new SqlParameter();
                dbParam_PostDate.ParameterName = "@PostDate";
                dbParam_PostDate.Value = postDate;
                dbParam_PostDate.DbType = System.Data.DbType.DateTime;
                cmd.Parameters.Add(dbParam_PostDate);

                SqlParameter dbParam_vendID = new SqlParameter();
                dbParam_vendID.ParameterName = "@VendorID";
                dbParam_vendID.Value = vendID;
                dbParam_vendID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_vendID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = invNum;
                dbParam_InvNum.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_groc = new SqlParameter();
                dbParam_groc.ParameterName = "@Groceries";
                dbParam_groc.Value = groc;
                dbParam_groc.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_groc);

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@Paper";
                dbParam_Paper.Value = paper;
                dbParam_Paper.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Clean = new SqlParameter();
                dbParam_Clean.ParameterName = "@Cleaning";
                dbParam_Clean.Value = clean;
                dbParam_Clean.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Clean);

                SqlParameter dbParam_Other = new SqlParameter();
                dbParam_Other.ParameterName = "@Other";
                dbParam_Other.Value = other;
                dbParam_Other.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Other);

                SqlParameter dbParam_othCat = new SqlParameter();
                dbParam_othCat.ParameterName = "@OtherCategoryID";
                dbParam_othCat.Value = otherCat;
                dbParam_othCat.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_othCat);

                SqlParameter dbParam_comments = new SqlParameter();
                dbParam_comments.ParameterName = "@PurchaseComments";
                dbParam_comments.Value = comments;
                dbParam_comments.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_comments);

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

                    if (VerboseErrors)
                    {
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
                catch (Exception ex)
                {
                    RecordCreated = false;

                    if (VerboseErrors)
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
        /// Clear out the RFSImport SQL table
        /// </summary>
        private void CommissaryClearRFSImportTable()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connCommissaries"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "TRUNCATE TABLE [IMPORT_RFS]";

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
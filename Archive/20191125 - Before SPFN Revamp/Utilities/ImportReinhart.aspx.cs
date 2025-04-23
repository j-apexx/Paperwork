using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportReinhart : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"].ToString() != "Admin")
                Response.Redirect("~/Default.aspx");
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
                    //clear out the temp table
                    ClearRFSImportTable();
                    TextBoxStatus.Text += "-Temp table cleared.";
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
                            //TextBoxStatus.Text += Environment.NewLine + " [ROW] " + row;
                            //TextBoxStatus.Rows++;

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

                    //Update the DistrictID and SchoolNums
                    TextBoxStatus.Text += Environment.NewLine + "-Updating the Districts and SchoolNums.";
                    TextBoxStatus.Rows++;
                    UpdateImportDivisionsAndSchools();

                    //Validate School and Districts
                    //Per Tom - no need to stop import for unmapped schools as files can contain commissaries - informational only
                    CheckForMissingDivSchools();

                    //Finally - Create the Purch records
                    Int32 CountPurchRecords = 0;
                    if(CreatePurchaseRecords(out CountPurchRecords) ==true)
                    {
                        //if successful clear out the temp table
                        //Otherwise it may be useful for troubleshooting
                        ClearRFSImportTable();

                        TextBoxStatus.Text += Environment.NewLine + "-Import Complete. " + CountPurchRecords.ToString() + " Purchase records were created";
                        TextBoxStatus.Rows++;

                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Import encountered issues. " + CountPurchRecords.ToString() + " Purchase records were created";
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
            if (InvNum == string.Empty)
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
            if (RFSDivision == string.Empty)
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
                                , (Decimal)rdr["Paper"], (Decimal)rdr["Clean"], _other, _otherCat, "", 0M, (Int16)rdr["SchoolNum"], Session["ProgramYear"].ToString()
                                , ref TextBoxStatus, CB_VerboseErrors.Checked)==true)
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

        protected void BtnImportFile_Click(object sender, EventArgs e)
        {
            if(Page.IsValid)
                Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#myModal').modal('show');", true);
        }
    }
}
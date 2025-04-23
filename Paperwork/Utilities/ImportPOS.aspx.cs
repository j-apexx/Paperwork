using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web.UI;


namespace Paperwork
{
    public partial class ImportPOS : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            if (Session["CurrentDistrict"] == null)
                Response.Redirect("~/default.aspx");

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

        private int RecordsProcessed = 0;
        private int DuplicateRecords = 0;

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;

            RecordsProcessed = 0;
            DuplicateRecords = 0;

            if (FileUpload1.PostedFile != null && !string.IsNullOrEmpty(FileUpload1.PostedFile.FileName) /*&& FileUpload1.PostedFile.FileName.ToUpper().Contains("XLSX")*/)
            {
                if (FileUpload1.PostedFile.FileName.ToUpper().Contains(".XLS") || FileUpload1.PostedFile.FileName.ToUpper().Contains(".CSV"))
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

                        Boolean importErrors = false;

                        if (FileUpload1.PostedFile.FileName.ToUpper().Contains(".XLS"))
                        {
                            //Read contents of the XLSX file.
                            importErrors = ReadExcelFile(path);
                        }
                        else if (FileUpload1.PostedFile.FileName.ToUpper().Contains(".CSV"))
                        {
                            importErrors = ReadCSVFile(path);
                        }



                        //No Errors
                        if (importErrors == false)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Read " + RecordsProcessed.ToString() + " records from the import file. Import complete.";
                            TextBoxStatus.Rows++;
                            TextBoxStatus.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Read " + RecordsProcessed.ToString() + " records from the import file. **Errors caused the import to end**.";
                            TextBoxStatus.Rows++;
                            TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                        }

                        if (DuplicateRecords > 0)
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-" + DuplicateRecords.ToString() + " duplicate records were encountered and skipped.";
                            TextBoxStatus.Rows++;
                        }

                        try
                        {
                            File.Delete(path);
                        }
                        catch { }//silent catch

                        //refresh the imported record grid
                        //GridViewPayInsRecords.DataBind();

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
                else
                {
                    TextBoxStatus.Text += Environment.NewLine + "-File was not expected filetype.";
                    TextBoxStatus.Rows++;
                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private bool ReadCSVFile(string _FileName)
        {
            bool csvFileHadErrors = false;

            FileInfo file = new FileInfo(_FileName);

            string csvConnString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='text;HDR=Yes;FMT=Delimited(,)';", file.DirectoryName);
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM [" + file.Name + "]", csvConnString);

            using (DataSet ds = new DataSet())
            {
                adapter.Fill(ds, "POSRecords");
                DataTable data = ds.Tables["POSRecords"];
                int rowCount = data.Rows.Count;

                csvFileHadErrors = ParsePOSRecords(ref data);
            }

            if (adapter != null)
                adapter.Dispose();

            return csvFileHadErrors;
        }

        private bool ReadExcelFile(string _FileName)
        {
            bool spreadsheetHadErrors = false;
            string xLconnString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", _FileName);

            string SheetName = getFirstSheetName(xLconnString);

            var adapter = new OleDbDataAdapter("SELECT * FROM [" + SheetName + "]", xLconnString);
            var ds = new DataSet();

            adapter.Fill(ds, "POSRecords");
            DataTable data = ds.Tables["POSRecords"];

            int rowCount = data.Rows.Count;

            spreadsheetHadErrors = ParsePOSRecords(ref data);

            if (ds != null)
                ds.Dispose();

            if (adapter != null)
                adapter.Dispose();

            return spreadsheetHadErrors;
        }

        private bool ParsePOSRecords(ref DataTable data)
        {
            bool spreadsheetHadErrors = false;

            if (data.Columns[0].ColumnName == "SclNum" && data.Columns[1].ColumnName == "Date" &&
            data.Columns[2].ColumnName == "Unit" && data.Columns[3].ColumnName == "Revision" &&
            data.Columns[4].ColumnName == "YrID" && data.Columns[5].ColumnName == "EditCkDate" &&
            data.Columns[6].ColumnName == "BkDay" && data.Columns[7].ColumnName == "LnDay" &&
            data.Columns[8].ColumnName == "PdElBk" && data.Columns[9].ColumnName == "PdJrBk" &&
            data.Columns[10].ColumnName == "PdSrBk" && data.Columns[11].ColumnName == "PdBI" &&
            data.Columns[12].ColumnName == "RdBk" && data.Columns[13].ColumnName == "RdBI" &&
            data.Columns[14].ColumnName == "FrBk" && data.Columns[15].ColumnName == "FrBI" &&
            data.Columns[16].ColumnName == "StACBk" && data.Columns[17].ColumnName == "AdACBk" &&
            data.Columns[18].ColumnName == "PdElLn" && data.Columns[19].ColumnName == "PdJrLn" &&
            data.Columns[20].ColumnName == "PdSrLn" && data.Columns[21].ColumnName == "Tr1" &&
            data.Columns[22].ColumnName == "Tr2" && data.Columns[23].ColumnName == "PdLL" &&
            data.Columns[24].ColumnName == "RdLn" && data.Columns[25].ColumnName == "RdTr1" &&
            data.Columns[26].ColumnName == "RdLL" && data.Columns[27].ColumnName == "FrLn" &&
            data.Columns[28].ColumnName == "FrTr1" && data.Columns[29].ColumnName == "FrTr2" &&
            data.Columns[30].ColumnName == "FrLL" && data.Columns[31].ColumnName == "StACLn" &&
            data.Columns[32].ColumnName == "AdACLn" && data.Columns[33].ColumnName == "OAla1" &&
            data.Columns[34].ColumnName == "OAla2" && data.Columns[35].ColumnName == "BkCash" &&
            data.Columns[36].ColumnName == "LnCash" && data.Columns[37].ColumnName == "Adj" &&
            data.Columns[38].ColumnName == "Spfn" && data.Columns[39].ColumnName == "SpFnCl" &&
            data.Columns[40].ColumnName == "Vending" && data.Columns[41].ColumnName == "USDA" &&
            data.Columns[42].ColumnName == "USDACI" && data.Columns[43].ColumnName == "Other1" &&
            data.Columns[44].ColumnName == "Other1CI" && data.Columns[45].ColumnName == "Other2" &&
            data.Columns[46].ColumnName == "Other2CI" && data.Columns[47].ColumnName == "OnLineAdv" &&  /* ONLINEADVANCE */
            data.Columns[48].ColumnName == "AdSlBk" && data.Columns[49].ColumnName == "AdRdBk" &&
            data.Columns[50].ColumnName == "ChMdBk" && data.Columns[51].ColumnName == "ChRpBk" &&
            data.Columns[52].ColumnName == "AdSlLn" && data.Columns[53].ColumnName == "AdRdLn" && /* ADV SALES LUNCH */
            data.Columns[54].ColumnName == "ChMdLn" && data.Columns[55].ColumnName == "ChRpLn" &&
            data.Columns[56].ColumnName == "FrElig" && data.Columns[57].ColumnName == "RdElig" &&
            data.Columns[58].ColumnName == "PdElig" && data.Columns[59].ColumnName == "BkIncent" &&
            data.Columns[60].ColumnName == "Enroll" && data.Columns[61].ColumnName == "AF" &&
            data.Columns[62].ColumnName == "PdSnack" && data.Columns[63].ColumnName == "RdSnack" &&
            data.Columns[64].ColumnName == "FrSnack" && data.Columns[65].ColumnName == "EditCkComments" &&
            data.Columns[66].ColumnName == "RecapComments" && data.Columns[67].ColumnName == "Record Lock" &&
            data.Columns[68].ColumnName == "Date Added" && data.Columns[69].ColumnName == "PdCACFP" &&
            data.Columns[70].ColumnName == "RdCACFP" && data.Columns[71].ColumnName == "FrCACFP"
            )
            {
                foreach (DataRow row in data.Rows)
                {
                    try
                    {
                        RecordsProcessed++;

                        RecapEntry rcRecord = new RecapEntry();

                        if (int.TryParse(row.ItemArray[0].ToString(), out int schNum))
                            rcRecord.SchoolNum = schNum;

                        if (DateTime.TryParse(row.ItemArray[1].ToString(), out DateTime rdate))
                            rcRecord.RecapDate = rdate;

                        rcRecord.DistrictID = row.ItemArray[2].ToString().PadLeft(3, '0');
                        if (int.TryParse(row.ItemArray[3].ToString(), out int rev))
                            rcRecord.Revision = rev;

                        rcRecord.ProgramYear = DDLProgramYears.SelectedValue;

                        if (DateTime.TryParse(row.ItemArray[5].ToString(), out DateTime edCheckDate))
                            rcRecord.EditCheckDate = edCheckDate;

                        //Excel lists as false, csv lists as zero
                        rcRecord.BreakfastDay = (row.ItemArray[6].ToString().ToUpper() == "FALSE" || row.ItemArray[6].ToString() == "0") ? (byte)0 : (byte)1;
                        rcRecord.LunchDay = (row.ItemArray[7].ToString().ToUpper() == "FALSE" || row.ItemArray[7].ToString() == "0") ? (byte)0 : (byte)1;


                        if (int.TryParse(row.ItemArray[8].ToString(), out int bpelem))
                            rcRecord.BPaidElem = bpelem;
                        if (int.TryParse(row.ItemArray[9].ToString(), out int bpjr))
                            rcRecord.BPaidJr = bpjr;
                        if (int.TryParse(row.ItemArray[10].ToString(), out int bpsr))
                            rcRecord.BPaidSr = bpsr;
                        if (int.TryParse(row.ItemArray[11].ToString(), out int bpl))
                            rcRecord.BPaidLrn = bpl;
                        if (int.TryParse(row.ItemArray[12].ToString(), out int bred))
                            rcRecord.BReduced = bred;
                        if (int.TryParse(row.ItemArray[13].ToString(), out int bredl))
                            rcRecord.BReducedLrn = bredl;
                        if (int.TryParse(row.ItemArray[14].ToString(), out int bfree))
                            rcRecord.BFree = bfree;
                        if (int.TryParse(row.ItemArray[15].ToString(), out int bfreel))
                            rcRecord.BFreeLrn = bfreel;
                        if (decimal.TryParse(row.ItemArray[16].ToString(), out decimal bstuala))
                            rcRecord.BStuAla = bstuala;
                        if (decimal.TryParse(row.ItemArray[17].ToString(), out decimal baala))
                            rcRecord.BAdultAla = baala;

                        /* LUNCH */
                        if (int.TryParse(row.ItemArray[18].ToString(), out int lpaidelem))
                            rcRecord.LPaidElem = lpaidelem;
                        if (int.TryParse(row.ItemArray[19].ToString(), out int lpaidjr))
                            rcRecord.LPaidJr = lpaidjr;
                        if (int.TryParse(row.ItemArray[20].ToString(), out int lpaidsr))
                            rcRecord.LPaidSr = lpaidsr;
                        if (int.TryParse(row.ItemArray[21].ToString(), out int ltier1))
                            rcRecord.LTier1 = ltier1;
                        if (int.TryParse(row.ItemArray[22].ToString(), out int ltier2))
                            rcRecord.LTier2 = ltier2;
                        if (int.TryParse(row.ItemArray[23].ToString(), out int lpaidlrn))
                            rcRecord.LPaidLrn = lpaidlrn;
                        if (int.TryParse(row.ItemArray[24].ToString(), out int lreduced))
                            rcRecord.LReduced = lreduced;
                        if (int.TryParse(row.ItemArray[25].ToString(), out int lredt1))
                            rcRecord.LReducedTier1 = lredt1;
                        if (int.TryParse(row.ItemArray[26].ToString(), out int lredlearn))
                            rcRecord.LReducedLrn = lredlearn;
                        if (int.TryParse(row.ItemArray[27].ToString(), out int lfree))
                            rcRecord.LFree = lfree;
                        if (int.TryParse(row.ItemArray[28].ToString(), out int lfreet1))
                            rcRecord.LFreeTier1 = lfreet1;
                        if (int.TryParse(row.ItemArray[29].ToString(), out int lfreet2))
                            rcRecord.LFreeTier2 = lfreet2;
                        if (int.TryParse(row.ItemArray[30].ToString(), out int lfreelrn))
                            rcRecord.LFreeLrn = lfreelrn;
                        if (decimal.TryParse(row.ItemArray[31].ToString(), out decimal lstuala))
                            rcRecord.LStuAla = lstuala;
                        if (decimal.TryParse(row.ItemArray[32].ToString(), out decimal ladultala))
                            rcRecord.LAdultAla = ladultala;

                        /* Other */
                        if (decimal.TryParse(row.ItemArray[33].ToString(), out decimal lotherala1))
                            rcRecord.LOtherAla1 = lotherala1;
                        if (decimal.TryParse(row.ItemArray[34].ToString(), out decimal lvendala))
                            rcRecord.LVendAla = lvendala;
                        if (decimal.TryParse(row.ItemArray[35].ToString(), out decimal bdeposit))
                            rcRecord.BDeposit = bdeposit;
                        if (decimal.TryParse(row.ItemArray[36].ToString(), out decimal deposit))
                            rcRecord.Deposit = deposit;
                        if (decimal.TryParse(row.ItemArray[37].ToString(), out decimal adj))
                            rcRecord.Adjustment = adj;
                        if (decimal.TryParse(row.ItemArray[38].ToString(), out decimal sfsale))
                            rcRecord.SpecialFunctionSale = sfsale;
                        if (decimal.TryParse(row.ItemArray[39].ToString(), out decimal sfcoll))
                            rcRecord.SpecialFunctionColl = sfcoll;

                        if (decimal.TryParse(row.ItemArray[40].ToString(), out decimal sfcomm))
                            rcRecord.SpecialFunctionComm = sfcomm;
                        if (decimal.TryParse(row.ItemArray[41].ToString(), out decimal usdasale))
                            rcRecord.USDASale = usdasale;
                        if (decimal.TryParse(row.ItemArray[42].ToString(), out decimal usdacoll))
                            rcRecord.USDAColl = usdacoll;
                        if (decimal.TryParse(row.ItemArray[43].ToString(), out decimal osale1))
                            rcRecord.OtherSale1 = osale1;
                        if (decimal.TryParse(row.ItemArray[44].ToString(), out decimal ocoll1))
                            rcRecord.OtherColl1 = ocoll1;
                        if (decimal.TryParse(row.ItemArray[45].ToString(), out decimal osale2))
                            rcRecord.OtherSale2 = osale2;
                        if (decimal.TryParse(row.ItemArray[46].ToString(), out decimal ocoll2))
                            rcRecord.OtherColl2 = ocoll2;
                        if (decimal.TryParse(row.ItemArray[47].ToString(), out decimal onadvance)) /* ONLUNEADVANCE */
                            rcRecord.OnlineAdvance = onadvance;
                        if (decimal.TryParse(row.ItemArray[48].ToString(), out decimal badvanced))
                            rcRecord.BAdvanced = badvanced;
                        if (decimal.TryParse(row.ItemArray[49].ToString(), out decimal bredeemed))
                            rcRecord.BRedeemed = bredeemed;
                        if (decimal.TryParse(row.ItemArray[50].ToString(), out decimal bcharges))
                            rcRecord.BCharges = bcharges;
                        if (decimal.TryParse(row.ItemArray[51].ToString(), out decimal brepaid))
                            rcRecord.BRepaid = brepaid;
                        if (decimal.TryParse(row.ItemArray[52].ToString(), out decimal ladvanced))  /* Lunch Advanced */
                            rcRecord.LAdvanced = ladvanced;
                        if (decimal.TryParse(row.ItemArray[53].ToString(), out decimal lredeemed))
                            rcRecord.LRedeemed = lredeemed;
                        if (decimal.TryParse(row.ItemArray[54].ToString(), out decimal lcharges))
                            rcRecord.LCharges = lcharges;
                        if (decimal.TryParse(row.ItemArray[55].ToString(), out decimal lrepaid))
                            rcRecord.LRepaid = lrepaid;

                        if (int.TryParse(row.ItemArray[56].ToString(), out int r_eligfree))
                            rcRecord.R_EligibleFree = r_eligfree;
                        if (int.TryParse(row.ItemArray[57].ToString(), out int r_eligred))
                            rcRecord.R_EligibleReduced = r_eligred;
                        if (int.TryParse(row.ItemArray[58].ToString(), out int r_eligpaid))
                            rcRecord.R_EligiblePaid = r_eligpaid;
                        if (decimal.TryParse(row.ItemArray[59].ToString(), out decimal r_bfastincent))
                            rcRecord.R_BreakfastIncent = r_bfastincent / 100M;//values are 0,2,4 need to divide by 100
                        if (int.TryParse(row.ItemArray[60].ToString(), out int r_enroll))
                            rcRecord.R_Enrollment = r_enroll;
                        if (decimal.TryParse(row.ItemArray[61].ToString(), out decimal r_af))
                            rcRecord.R_AF = r_af;
                        if (int.TryParse(row.ItemArray[62].ToString(), out int opaidsnack))
                            rcRecord.OPaidSnack = opaidsnack;
                        if (int.TryParse(row.ItemArray[63].ToString(), out int oredsnack))
                            rcRecord.OReducedSnack = oredsnack;
                        if (int.TryParse(row.ItemArray[64].ToString(), out int ofreesnack))
                            rcRecord.OFreeSnack = ofreesnack;

                        rcRecord.EditCheckComments = row.ItemArray[65].ToString();
                        rcRecord.RecapComments = row.ItemArray[66].ToString();

                        CreateRecord(rcRecord);

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
            }
            else
            {
                TextBoxStatus.Text += Environment.NewLine + "-File column names did not match the expected layout.";
                TextBoxStatus.Rows++;
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;

                spreadsheetHadErrors = true;
            }


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


        /// <summary>
        /// Create the Sales Table record
        /// </summary>
        private Boolean CreateRecord(RecapEntry re)
        {
            Boolean RecordCreated = false;

            // Reject the record if its district does not match the current district
            if (re.DistrictID != Session["CurrentDistrict"].ToString())
                return RecordCreated;

            if (SchoolExists(re.SchoolNum, re.DistrictID) == false)
            {
                TextBoxStatus.Text += Environment.NewLine + "-School does not exist for this district. Dist:" + re.DistrictID + " School number:" + re.SchoolNum.ToString();
                TextBoxStatus.Rows++;

                return RecordCreated;
            }

            if (SchoolSetupRecordExists(re.SchoolNum, re.DistrictID, re.ProgramYear) == false)
            {
                TextBoxStatus.Text += Environment.NewLine + "-School Setup record does not exist for this district. Dist:" + re.DistrictID + " School number:" + re.SchoolNum.ToString();
                TextBoxStatus.Rows++;

                return RecordCreated;
            }

            //Check the date
            if (IsDistrictEnforcingPeriodClosures(re.DistrictID) == true && IsDateOpenForEntry(re.RecapDate, DDLProgramYears.SelectedValue, re.DistrictID, "SalesEntry") == false)
            {
                TextBoxStatus.Text += Environment.NewLine + "-Record falls in closed period and was skipped. " + re.RecapDate.ToShortDateString();
                TextBoxStatus.Rows++;
                return RecordCreated;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string InsertUpdateQuery = "INSERT INTO [RecapEntry] (ProgramYear, DistrictID, SchoolNum, RecapDate, BreakfastDay, LunchDay, BPaidElem, BPaidJr, BPaidSr "
                + " ,BReduced, BFree, BPaidLrn, BReducedLrn, BFreeLrn, BStuAla, BAdultAla, BAdvanced, BRedeemed, BRepaid, BCharges, BDeposit "
                + " ,LPaidElem, LPaidJr, LPaidSr, LTier1, LTier2, LReduced, LReducedTier1, LFree, LFreeTier1, LFreeTier2, LPaidLrn, LReducedLrn "
                + " ,LFreeLrn, LStuAla, LAdultAla, LOtherAla1, LVendAla, LAdvanced, LRedeemed, LRepaid, LCharges "
                + " ,OPaidSnack ,OReducedSnack ,OFreeSnack ,SpecialFunctionSale ,SpecialFunctionColl ,SpecialFunctionComm "
                + " ,USDASale ,USDAColl ,OtherSale1 ,OtherColl1 ,OtherSale2 ,OtherColl2 "
                + " ,OnlineAdvance ,Adjustment ,Deposit ,OtherSale1Cat ,OtherColl1Cat "
                + " ,EditCheckComments ,RecapComments ,OtherSale2Cat ,OtherColl2Cat "
                + " ,R_AF, R_Enrollment, R_BreakfastIncent, R_EligiblePaid, R_EligibleReduced, R_EligibleFree, Revision, EditCheckDate, CEPPartic, CreatedDate, LastModifiedUser, LastModifiedDate ) "
                + " VALUES (@ProgramYear, @DistrictID, @SchoolNum, @RecapDate, @BreakfastDay, @LunchDay, @BPaidElem, @BPaidJr, @BPaidSr, @BReduced, @BFree, @BPaidLrn "
                + " ,@BReducedLrn, @BFreeLrn, @BStuAla, @BAdultAla, @BAdvanced, @BRedeemed, @BRepaid, @BCharges, @BDeposit "
                + " ,@LPaidElem, @LPaidJr, @LPaidSr, @LTier1, @LTier2, @LReduced, @LReducedTier1, @LFree, @LFreeTier1, @LFreeTier2, @LPaidLrn, @LReducedLrn "
                + " ,@LFreeLrn, @LStuAla, @LAdultAla, @LOtherAla1, @LVendAla, @LAdvanced, @LRedeemed, @LRepaid, @LCharges "
                + " ,@OPaidSnack ,@OReducedSnack ,@OFreeSnack ,@SpecialFunctionSale ,@SpecialFunctionColl ,@SpecialFunctionComm "
                + " ,@USDASale ,@USDAColl ,@OtherSale1 ,@OtherColl1 ,@OtherSale2 ,@OtherColl2 "
                + " ,@OnlineAdvance ,@Adjustment ,@Deposit ,@OtherSale1Cat ,@OtherColl1Cat "
                + " ,@EditCheckComments ,@RecapComments ,@OtherSale2Cat ,@OtherColl2Cat "
                + " ,@R_AF, @R_Enrollment, @R_BreakfastIncent, @R_EligiblePaid, @R_EligibleReduced, @R_EligibleFree, @Revision, @EditCheckDate, @CEPPartic, GETDATE(), @LastModifiedUser, GETDATE() )";


            using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection))
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = re.ProgramYear;
                dbParam_ProgYear.DbType = DbType.String;
                cmd2.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = re.DistrictID;
                dbParam_DistID.DbType = DbType.String;
                cmd2.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchlNum2 = new SqlParameter();
                dbParam_SchlNum2.ParameterName = "@SchoolNum";
                dbParam_SchlNum2.Value = re.SchoolNum;
                dbParam_SchlNum2.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_SchlNum2);

                SqlParameter dbParam_RecapDate2 = new SqlParameter();
                dbParam_RecapDate2.ParameterName = "@RecapDate";
                dbParam_RecapDate2.Value = re.RecapDate;
                dbParam_RecapDate2.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_RecapDate2);

                //Only used in Inserts
                SqlParameter dbParam_EditCheckDate = new SqlParameter();
                dbParam_EditCheckDate.ParameterName = "@EditCheckDate";
                dbParam_EditCheckDate.DbType = System.Data.DbType.Date;
                dbParam_EditCheckDate.Value = re.EditCheckDate;// TrxDate.Text;
                cmd2.Parameters.Add(dbParam_EditCheckDate);

                SqlParameter dbParam_Revision2 = new SqlParameter();
                dbParam_Revision2.ParameterName = "@Revision";
                dbParam_Revision2.Value = re.Revision;
                dbParam_Revision2.DbType = DbType.Int32;
                cmd2.Parameters.Add(dbParam_Revision2);
                #endregion

                #region Checkboxes
                //Updateable Values
                SqlParameter dbParam_bDay = new SqlParameter();
                dbParam_bDay.ParameterName = "@BreakfastDay";
                dbParam_bDay.Value = re.BreakfastDay;
                dbParam_bDay.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_bDay);

                SqlParameter dbParam_lDay = new SqlParameter();
                dbParam_lDay.ParameterName = "@LunchDay";
                dbParam_lDay.Value = re.LunchDay;
                dbParam_lDay.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_lDay);
                #endregion

                #region Breakfast
                SqlParameter dbParam_bPaidElem = new SqlParameter();
                dbParam_bPaidElem.ParameterName = "@BPaidElem";
                dbParam_bPaidElem.Value = re.BPaidElem;
                dbParam_bPaidElem.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_bPaidElem);

                SqlParameter dbParam_bPaidJr = new SqlParameter();
                dbParam_bPaidJr.ParameterName = "@BPaidJr";
                dbParam_bPaidJr.Value = re.BPaidJr;
                dbParam_bPaidJr.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_bPaidJr);

                SqlParameter dbParam_bPaidSr = new SqlParameter();
                dbParam_bPaidSr.ParameterName = "@BPaidSr";
                dbParam_bPaidSr.Value = re.BPaidSr;
                dbParam_bPaidSr.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_bPaidSr);

                SqlParameter dbParam_bReduced = new SqlParameter();
                dbParam_bReduced.ParameterName = "@BReduced";
                dbParam_bReduced.Value = re.BReduced;
                dbParam_bReduced.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_bReduced);

                SqlParameter dbParam_bFree = new SqlParameter();
                dbParam_bFree.ParameterName = "@BFree";
                dbParam_bFree.Value = re.BFree;
                dbParam_bFree.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_bFree);

                SqlParameter dbParam_bPaidLrn = new SqlParameter();
                dbParam_bPaidLrn.ParameterName = "@BPaidLrn";
                dbParam_bPaidLrn.Value = re.BPaidLrn;
                dbParam_bPaidLrn.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_bPaidLrn);

                SqlParameter dbParam_bReducedLrn = new SqlParameter();
                dbParam_bReducedLrn.ParameterName = "@BReducedLrn";
                dbParam_bReducedLrn.Value = re.BReducedLrn;
                dbParam_bReducedLrn.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_bReducedLrn);

                SqlParameter dbParam_bFreeLrn = new SqlParameter();
                dbParam_bFreeLrn.ParameterName = "@BFreeLrn";
                dbParam_bFreeLrn.Value = re.BFreeLrn;
                dbParam_bFreeLrn.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_bFreeLrn);

                SqlParameter dbParam_bStuAla = new SqlParameter();
                dbParam_bStuAla.ParameterName = "@BStuAla";
                dbParam_bStuAla.Value = re.BStuAla;
                dbParam_bStuAla.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_bStuAla);

                SqlParameter dbParam_bAdultAla = new SqlParameter();
                dbParam_bAdultAla.ParameterName = "@BAdultAla";
                dbParam_bAdultAla.Value = re.BAdultAla;
                dbParam_bAdultAla.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_bAdultAla);

                SqlParameter dbParam_bAdvanced = new SqlParameter();
                dbParam_bAdvanced.ParameterName = "@BAdvanced";
                dbParam_bAdvanced.Value = re.BAdvanced;
                dbParam_bAdvanced.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_bAdvanced);

                SqlParameter dbParam_bRedeemed = new SqlParameter();
                dbParam_bRedeemed.ParameterName = "@BRedeemed";
                dbParam_bRedeemed.Value = re.BRedeemed * -1M;
                dbParam_bRedeemed.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_bRedeemed);

                SqlParameter dbParam_bRepaid = new SqlParameter();
                dbParam_bRepaid.ParameterName = "@BRepaid";
                dbParam_bRepaid.Value = re.BRepaid;
                dbParam_bRepaid.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_bRepaid);

                SqlParameter dbParam_bCharges = new SqlParameter();
                dbParam_bCharges.ParameterName = "@BCharges";
                dbParam_bCharges.Value = re.BCharges * -1M;
                dbParam_bCharges.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_bCharges);

                SqlParameter dbParam_bDeposit = new SqlParameter();
                dbParam_bDeposit.ParameterName = "@BDeposit";
                dbParam_bDeposit.Value = re.BDeposit;
                dbParam_bDeposit.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_bDeposit);
                #endregion

                #region Lunch
                SqlParameter dbParam_lPaidElem = new SqlParameter();
                dbParam_lPaidElem.ParameterName = "@LPaidElem";
                dbParam_lPaidElem.Value = re.LPaidElem;
                dbParam_lPaidElem.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lPaidElem);

                SqlParameter dbParam_lPaidJr = new SqlParameter();
                dbParam_lPaidJr.ParameterName = "@LPaidJr";
                dbParam_lPaidJr.Value = re.LPaidJr;
                dbParam_lPaidJr.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lPaidJr);

                SqlParameter dbParam_lPaidSr = new SqlParameter();
                dbParam_lPaidSr.ParameterName = "@LPaidSr";
                dbParam_lPaidSr.Value = re.LPaidSr;
                dbParam_lPaidSr.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lPaidSr);

                SqlParameter dbParam_lTier1 = new SqlParameter();
                dbParam_lTier1.ParameterName = "@LTier1";
                dbParam_lTier1.Value = re.LTier1;
                dbParam_lTier1.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lTier1);

                SqlParameter dbParam_lTier2 = new SqlParameter();
                dbParam_lTier2.ParameterName = "@LTier2";
                dbParam_lTier2.Value = re.LTier2;
                dbParam_lTier2.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lTier2);

                SqlParameter dbParam_lReduced = new SqlParameter();
                dbParam_lReduced.ParameterName = "@LReduced";
                dbParam_lReduced.Value = re.LReduced;
                dbParam_lReduced.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lReduced);

                SqlParameter dbParam_lReducedTier1 = new SqlParameter();
                dbParam_lReducedTier1.ParameterName = "@LReducedTier1";
                dbParam_lReducedTier1.Value = re.LReducedTier1;
                dbParam_lReducedTier1.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lReducedTier1);

                SqlParameter dbParam_lFree = new SqlParameter();
                dbParam_lFree.ParameterName = "@LFree";
                dbParam_lFree.Value = re.LFree;
                dbParam_lFree.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lFree);

                SqlParameter dbParam_lFreeTier1 = new SqlParameter();
                dbParam_lFreeTier1.ParameterName = "@LFreeTier1";
                dbParam_lFreeTier1.Value = re.LFreeTier1;
                dbParam_lFreeTier1.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lFreeTier1);

                SqlParameter dbParam_lFreeTier2 = new SqlParameter();
                dbParam_lFreeTier2.ParameterName = "@LFreeTier2";
                dbParam_lFreeTier2.Value = re.LFreeTier2;
                dbParam_lFreeTier2.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lFreeTier2);

                SqlParameter dbParam_lPaidLrn = new SqlParameter();
                dbParam_lPaidLrn.ParameterName = "@LPaidLrn";
                dbParam_lPaidLrn.Value = re.LPaidLrn;
                dbParam_lPaidLrn.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lPaidLrn);

                SqlParameter dbParam_lReducedLrn = new SqlParameter();
                dbParam_lReducedLrn.ParameterName = "@LReducedLrn";
                dbParam_lReducedLrn.Value = re.LReducedLrn;
                dbParam_lReducedLrn.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lReducedLrn);

                SqlParameter dbParam_lFreeLrn = new SqlParameter();
                dbParam_lFreeLrn.ParameterName = "@LFreeLrn";
                dbParam_lFreeLrn.Value = re.LFreeLrn;
                dbParam_lFreeLrn.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_lFreeLrn);

                SqlParameter dbParam_lStuAla = new SqlParameter();
                dbParam_lStuAla.ParameterName = "@LStuAla";
                dbParam_lStuAla.Value = re.LStuAla;
                dbParam_lStuAla.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_lStuAla);

                SqlParameter dbParam_lAdultAla = new SqlParameter();
                dbParam_lAdultAla.ParameterName = "@LAdultAla";
                dbParam_lAdultAla.Value = re.LAdultAla;
                dbParam_lAdultAla.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_lAdultAla);

                SqlParameter dbParam_lOtherAla1 = new SqlParameter();
                dbParam_lOtherAla1.ParameterName = "@LOtherAla1";
                dbParam_lOtherAla1.Value = re.LOtherAla1;
                dbParam_lOtherAla1.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_lOtherAla1);

                SqlParameter dbParam_lVendAla = new SqlParameter();
                dbParam_lVendAla.ParameterName = "@LVendAla";
                dbParam_lVendAla.Value = re.LVendAla;
                dbParam_lVendAla.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_lVendAla);

                SqlParameter dbParam_lAdvanced = new SqlParameter();/* Lunch Advanced */
                dbParam_lAdvanced.ParameterName = "@LAdvanced";
                dbParam_lAdvanced.Value = re.LAdvanced + re.OnlineAdvance;//6/24/2020 - Tom said to add online advance into Lunch Advanced
                dbParam_lAdvanced.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_lAdvanced);

                SqlParameter dbParam_lRedeemed = new SqlParameter();
                dbParam_lRedeemed.ParameterName = "@LRedeemed";
                dbParam_lRedeemed.Value = re.LRedeemed * -1M;
                dbParam_lRedeemed.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_lRedeemed);

                SqlParameter dbParam_lRepaid = new SqlParameter();
                dbParam_lRepaid.ParameterName = "@LRepaid";
                dbParam_lRepaid.Value = re.LRepaid;
                dbParam_lRepaid.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_lRepaid);

                SqlParameter dbParam_lCharges = new SqlParameter();
                dbParam_lCharges.ParameterName = "@LCharges";
                dbParam_lCharges.Value = re.LCharges * -1M;
                dbParam_lCharges.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_lCharges);
                #endregion

                #region Other
                SqlParameter dbParam_oPaidSnack = new SqlParameter();
                dbParam_oPaidSnack.ParameterName = "@OPaidSnack";
                dbParam_oPaidSnack.Value = re.OPaidSnack;
                dbParam_oPaidSnack.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_oPaidSnack);

                SqlParameter dbParam_oReducedSnack = new SqlParameter();
                dbParam_oReducedSnack.ParameterName = "@OReducedSnack";
                dbParam_oReducedSnack.Value = re.OReducedSnack;
                dbParam_oReducedSnack.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_oReducedSnack);

                SqlParameter dbParam_oFreeSnack = new SqlParameter();
                dbParam_oFreeSnack.ParameterName = "@OFreeSnack";
                dbParam_oFreeSnack.Value = re.OFreeSnack;
                dbParam_oFreeSnack.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_oFreeSnack);

                SqlParameter dbParam_oSFSale = new SqlParameter();
                dbParam_oSFSale.ParameterName = "@SpecialFunctionSale";
                dbParam_oSFSale.Value = re.SpecialFunctionSale;
                dbParam_oSFSale.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oSFSale);

                SqlParameter dbParam_oSFColl = new SqlParameter();
                dbParam_oSFColl.ParameterName = "@SpecialFunctionColl";
                dbParam_oSFColl.Value = re.SpecialFunctionColl;
                dbParam_oSFColl.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oSFColl);

                SqlParameter dbParam_oVendComm = new SqlParameter();
                dbParam_oVendComm.ParameterName = "@SpecialFunctionComm";
                dbParam_oVendComm.Value = re.SpecialFunctionComm;
                dbParam_oVendComm.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oVendComm);

                SqlParameter dbParam_oUSDASale = new SqlParameter();
                dbParam_oUSDASale.ParameterName = "@USDASale";
                dbParam_oUSDASale.Value = re.USDASale;
                dbParam_oUSDASale.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oUSDASale);

                SqlParameter dbParam_oUSDAColl = new SqlParameter();
                dbParam_oUSDAColl.ParameterName = "@USDAColl";
                dbParam_oUSDAColl.Value = re.USDAColl;
                dbParam_oUSDAColl.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oUSDAColl);

                SqlParameter dbParam_oSale1 = new SqlParameter();
                dbParam_oSale1.ParameterName = "@OtherSale1";
                dbParam_oSale1.Value = re.OtherSale1;
                dbParam_oSale1.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oSale1);

                SqlParameter dbParam_oColl1 = new SqlParameter();
                dbParam_oColl1.ParameterName = "@OtherColl1";
                dbParam_oColl1.Value = re.OtherColl1;
                dbParam_oColl1.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oColl1);

                SqlParameter dbParam_oSale2 = new SqlParameter();
                dbParam_oSale2.ParameterName = "@OtherSale2";
                dbParam_oSale2.Value = re.OtherSale2;
                dbParam_oSale2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oSale2);

                SqlParameter dbParam_oColl2 = new SqlParameter();
                dbParam_oColl2.ParameterName = "@OtherColl2";
                dbParam_oColl2.Value = re.OtherColl2;
                dbParam_oColl2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oColl2);

                SqlParameter dbParam_oSale1Cat = new SqlParameter();
                dbParam_oSale1Cat.ParameterName = "@OtherSale1Cat";
                dbParam_oSale1Cat.Value = re.OtherSale1Cat;
                dbParam_oSale1Cat.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_oSale1Cat);

                SqlParameter dbParam_oColl1Cat = new SqlParameter();
                dbParam_oColl1Cat.ParameterName = "@OtherColl1Cat";
                dbParam_oColl1Cat.Value = re.OtherColl1Cat;
                dbParam_oColl1Cat.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_oColl1Cat);

                SqlParameter dbParam_oSale2Cat = new SqlParameter();
                dbParam_oSale2Cat.ParameterName = "@OtherSale2Cat";
                dbParam_oSale2Cat.Value = re.OtherSale2Cat;
                dbParam_oSale2Cat.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_oSale2Cat);

                SqlParameter dbParam_oColl2Cat = new SqlParameter();
                dbParam_oColl2Cat.ParameterName = "@OtherColl2Cat";
                dbParam_oColl2Cat.Value = re.OtherColl2Cat;
                dbParam_oColl2Cat.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_oColl2Cat);

                SqlParameter dbParam_oOnlineAdv = new SqlParameter();
                dbParam_oOnlineAdv.ParameterName = "@OnlineAdvance";  /* Online Advanced */
                dbParam_oOnlineAdv.Value = re.OnlineAdvance;
                dbParam_oOnlineAdv.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oOnlineAdv);

                SqlParameter dbParam_oAdjustment = new SqlParameter();
                dbParam_oAdjustment.ParameterName = "@Adjustment";
                dbParam_oAdjustment.Value = re.Adjustment;
                dbParam_oAdjustment.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oAdjustment);

                SqlParameter dbParam_oDeposit = new SqlParameter();
                dbParam_oDeposit.ParameterName = "@Deposit";
                dbParam_oDeposit.Value = re.Deposit;
                dbParam_oDeposit.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_oDeposit);

                SqlParameter dbParam_oEditCheckCmt = new SqlParameter();
                dbParam_oEditCheckCmt.ParameterName = "@EditCheckComments";
                dbParam_oEditCheckCmt.Value = re.EditCheckComments;
                dbParam_oEditCheckCmt.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_oEditCheckCmt);

                SqlParameter dbParam_oRecapCmt = new SqlParameter();
                dbParam_oRecapCmt.ParameterName = "@RecapComments";
                dbParam_oRecapCmt.Value = re.RecapComments;
                dbParam_oRecapCmt.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_oRecapCmt);

                //New 8-2024 for new reporting
                bool isCEP = isSchoolCurrentlyCEP(re.SchoolNum, re.DistrictID, re.ProgramYear);
                SqlParameter dbParam_lCepPartic = new SqlParameter();
                dbParam_lCepPartic.ParameterName = "@CEPPartic";
                dbParam_lCepPartic.Value = isCEP;
                dbParam_lCepPartic.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_lCepPartic);
                #endregion

                #region StoredHdrValues
                SqlParameter dbParam_AF = new SqlParameter();
                dbParam_AF.ParameterName = "@R_AF";
                dbParam_AF.Value = re.R_AF;
                dbParam_AF.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_AF);

                SqlParameter dbParam_Enrollment = new SqlParameter();
                dbParam_Enrollment.ParameterName = "@R_Enrollment";
                dbParam_Enrollment.Value = re.R_Enrollment;
                dbParam_Enrollment.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_Enrollment);

                SqlParameter dbParam_EligPaid = new SqlParameter();
                dbParam_EligPaid.ParameterName = "@R_EligiblePaid";
                dbParam_EligPaid.Value = re.R_EligiblePaid;
                dbParam_EligPaid.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_EligPaid);

                SqlParameter dbParam_EligFree = new SqlParameter();
                dbParam_EligFree.ParameterName = "@R_EligibleFree";
                dbParam_EligFree.Value = re.R_EligibleFree;
                dbParam_EligFree.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_EligFree);

                SqlParameter dbParam_EligRed = new SqlParameter();
                dbParam_EligRed.ParameterName = "@R_EligibleReduced";
                dbParam_EligRed.Value = re.R_EligibleReduced;
                dbParam_EligRed.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_EligRed);

                SqlParameter dbParam_BfIncent = new SqlParameter();
                dbParam_BfIncent.ParameterName = "@R_BreakfastIncent";
                dbParam_BfIncent.Value = re.R_BreakfastIncent;
                dbParam_BfIncent.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfIncent);

                SqlParameter dbParam_User = new SqlParameter();
                dbParam_User.ParameterName = "@LastModifiedUser";
                dbParam_User.Value = "ImportPOS:" + Session["CurrentUser"].ToString();
                dbParam_User.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_User);
                #endregion


                try
                {
                    dbConnection.Open();
                    int recordcount = 0;
                    recordcount = cmd2.ExecuteNonQuery();

                    if (recordcount > 0)
                    {
                        RecordCreated = true;

                        //Update the days table which shows breakfast and lunch days
                        UpdateDaysTable(re.EditCheckDate, re.DistrictID, DDLProgramYears.SelectedValue);

                    }
                    else
                        RecordCreated = false;
                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate") > 0)//Duplicate record 
                    {
                        /*do nothing - just skip and handle error*/
                        RecordCreated = true;
                        DuplicateRecords++;
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
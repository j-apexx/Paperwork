using System;
using System.Configuration;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportPayroll : Paperwork_Global
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
                PayrollDateFilter.Text = DateTime.Now.ToShortDateString();
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
                    //clear out the temp table
                    clearTempPayrollTable();
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
                    int duplicateRecordCount = 0;
                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            //Skip the first record
                            if (recordCount > 0)
                            {

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

                    //No Errors
                    if (importErrors == false)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + (recordCount - 1).ToString() + " records from the import file. Import complete.";
                        TextBoxStatus.Rows++;

                        //summarize
                        if (runSummarizePayrollSP())
                        {
                            TextBoxStatus.Text += Environment.NewLine + "-Summarized records.";
                            TextBoxStatus.Rows++;

                            //clear out the temp table
                            clearTempPayrollTable();

                            TextBoxStatus.Text += Environment.NewLine + "-Complete.";
                            TextBoxStatus.Rows++;
                            TextBoxStatus.ForeColor = System.Drawing.Color.Green;

                            GridViewPayTax.DataBind();
                        }
                        else
                        {
                            TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + (recordCount - 1).ToString() + " records from the import file. **Errors caused the import to end**.";
                        TextBoxStatus.Rows++;
                        TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                    }

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
            String ClientCode;
            String Distribution;
            String CheckNum;
            DateTime PayDate;
            String Adjust;
            String Unit;
            Decimal Gross=0M;
            Decimal Hours = 0M;
            Decimal FUTA = 0M;
            Decimal MEDR = 0M;
            Decimal PASUI = 0M;
            Decimal OHSUI = 0M;
            Decimal MISUI = 0M;
            Decimal FLSUI = 0M;
            Decimal NJSUI = 0M;
            Decimal VASUI = 0M;
            Decimal SSR = 0M;
            Decimal Uniform = 0M;
            Decimal Bereav = 0M;
            Decimal BereavHrs = 0M;
            Decimal Bkft = 0M;
            Decimal BkftHrs = 0M;
            Decimal FFV = 0M;
            Decimal FFVHrs = 0M;
            Decimal Hol = 0M;
            Decimal HolHrs = 0M;
            Decimal Jury = 0M;
            Decimal JuryHrs = 0M;
            Decimal PTO = 0M;
            Decimal PTOHrs = 0M;
            Decimal Meet = 0M;
            Decimal MeetHrs = 0M;
            Decimal Meet1 = 0M;
            Decimal Meet1Hrs = 0M;
            Decimal OTNonBud = 0M;
            Decimal OTNonBudHrs = 0M;
            Decimal Other = 0M;
            Decimal OtherHrs = 0M;
            Decimal OT = 0M;
            Decimal OTHrs=0M; 
            Decimal Rate4 = 0M;
            Decimal Rate4Hrs = 0M;
            Decimal Rate5 = 0M;
            Decimal Rate5Hrs = 0M;
            Decimal Reg = 0M;
            Decimal RegHrs = 0M;
            Decimal Retro = 0M;
            Decimal RetroHrs = 0M;
            Decimal ShiftDif = 0M;
            Decimal ShiftDifHrs = 0M;
            Decimal Spfn = 0M;
            Decimal SpfnHrs = 0M;
            Decimal SummerWN = 0M;
            Decimal SummerWNHrs = 0M;
            Decimal Summer = 0M;
            Decimal SummerHrs = 0M;
            Decimal Train = 0M;
            Decimal TrainHrs = 0M;
            Decimal Unpaid = 0M;
            Decimal UnpaidHrs = 0M;
            Decimal Bonus = 0M;
            Decimal BonusHrs = 0M;
            Decimal Incent = 0M;
            Decimal IncentHrs = 0M;
            Decimal InsBuy = 0M;
            Decimal InsBuyHrs = 0M;
            Decimal Ira = 0M;
            Decimal IraHrs = 0M;
            Decimal OtherDollars = 0M;
            Decimal OtherDollarsHrs = 0M;
            Decimal RegDollars = 0M;
            Decimal RegDollarsHrs = 0M;
            Decimal Retroactive = 0M;
            Decimal RetroactiveHrs = 0M;
            Decimal ShiftDiffDollar = 0M;
            Decimal ShiftDiffDollarHrs = 0M;
            Decimal SpfnDollar = 0M;
            Decimal SpfnDollarHrs = 0M;
            Decimal ThirdPartySick = 0M;
            Decimal ThirdPartySickHrs = 0M;
            Decimal WNOT = 0M;
            Decimal WNOTHrs = 0M;
            Decimal WNVac = 0M;
            Decimal WNVacHrs = 0M;
            Decimal PdTimeOffDollar = 0M;
            Decimal PdTimeOffHrs = 0M;
            Decimal DNUDollars = 0M;
            Decimal DNUHrs = 0M;
            Decimal PittTax = 0M;
            Decimal HSOA = 0M;
            Decimal HSOAHrs = 0M;
            #endregion

            string[] rowArray = row.Split(',');

            if(rowArray.Length!=91)
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

            #region Assign Variable Values from row
            try
            {
                ClientCode = rowArray[0];
                Distribution = rowArray[1];
                CheckNum = rowArray[2];
                if (DateTime.TryParse(rowArray[3], out PayDate) == false)
                    PayDate = DateTime.Parse("1/1/1900");
                else //For record filter
                    PayrollDateFilter.Text = PayDate.ToShortDateString();

                Adjust = rowArray[4];
                Unit = rowArray[5];
                Decimal.TryParse(rowArray[6], out Gross);
                Decimal.TryParse(rowArray[7], out Hours);
                Decimal.TryParse(rowArray[8], out FUTA);
                Decimal.TryParse(rowArray[9], out MEDR);
                Decimal.TryParse(rowArray[10], out PASUI);
                Decimal.TryParse(rowArray[11], out OHSUI);
                Decimal.TryParse(rowArray[12], out MISUI);
                Decimal.TryParse(rowArray[13], out FLSUI);
                Decimal.TryParse(rowArray[14], out NJSUI);
                Decimal.TryParse(rowArray[15], out VASUI);
                Decimal.TryParse(rowArray[16], out SSR);
                Decimal.TryParse(rowArray[17], out Uniform);
                Decimal.TryParse(rowArray[18], out Bereav);
                Decimal.TryParse(rowArray[19], out BereavHrs);
                Decimal.TryParse(rowArray[20], out Bkft);
                Decimal.TryParse(rowArray[21], out BkftHrs);
                Decimal.TryParse(rowArray[22], out FFV);
                Decimal.TryParse(rowArray[23], out FFVHrs);
                Decimal.TryParse(rowArray[24], out Hol);
                Decimal.TryParse(rowArray[25], out HolHrs);
                Decimal.TryParse(rowArray[26], out Jury);
                Decimal.TryParse(rowArray[27], out JuryHrs);
                Decimal.TryParse(rowArray[28], out PTO);
                Decimal.TryParse(rowArray[29], out PTOHrs);
                Decimal.TryParse(rowArray[30], out Meet);
                Decimal.TryParse(rowArray[31], out MeetHrs);
                Decimal.TryParse(rowArray[32], out Meet1);
                Decimal.TryParse(rowArray[33], out Meet1Hrs);
                Decimal.TryParse(rowArray[34], out OTNonBud);
                Decimal.TryParse(rowArray[35], out OTNonBudHrs);
                Decimal.TryParse(rowArray[36], out Other);
                Decimal.TryParse(rowArray[37], out OtherHrs);
                Decimal.TryParse(rowArray[38], out OT);
                Decimal.TryParse(rowArray[39], out OTHrs);
                Decimal.TryParse(rowArray[40], out Rate4);
                Decimal.TryParse(rowArray[41], out Rate4Hrs);
                Decimal.TryParse(rowArray[42], out Rate5);
                Decimal.TryParse(rowArray[43], out Rate5Hrs);
                Decimal.TryParse(rowArray[44], out Reg);
                Decimal.TryParse(rowArray[45], out RegHrs);
                Decimal.TryParse(rowArray[46], out Retro);
                Decimal.TryParse(rowArray[47], out RetroHrs);
                Decimal.TryParse(rowArray[48], out ShiftDif);
                Decimal.TryParse(rowArray[49], out ShiftDifHrs);
                Decimal.TryParse(rowArray[50], out Spfn);
                Decimal.TryParse(rowArray[51], out SpfnHrs);
                Decimal.TryParse(rowArray[52], out SummerWN);
                Decimal.TryParse(rowArray[53], out SummerWNHrs);
                Decimal.TryParse(rowArray[54], out Summer);
                Decimal.TryParse(rowArray[55], out SummerHrs);
                Decimal.TryParse(rowArray[56], out Train);
                Decimal.TryParse(rowArray[57], out TrainHrs);
                Decimal.TryParse(rowArray[58], out Unpaid);
                Decimal.TryParse(rowArray[59], out UnpaidHrs);
                Decimal.TryParse(rowArray[60], out Bonus);
                Decimal.TryParse(rowArray[61], out BonusHrs);
                Decimal.TryParse(rowArray[62], out Incent);
                Decimal.TryParse(rowArray[63], out IncentHrs);
                Decimal.TryParse(rowArray[64], out InsBuy);
                Decimal.TryParse(rowArray[65], out InsBuyHrs);
                Decimal.TryParse(rowArray[66], out Ira);
                Decimal.TryParse(rowArray[67], out IraHrs);
                Decimal.TryParse(rowArray[68], out OtherDollars);
                Decimal.TryParse(rowArray[69], out OtherDollarsHrs);
                Decimal.TryParse(rowArray[70], out RegDollars);
                Decimal.TryParse(rowArray[71], out RegDollarsHrs);
                Decimal.TryParse(rowArray[72], out Retroactive);
                Decimal.TryParse(rowArray[73], out RetroactiveHrs);
                Decimal.TryParse(rowArray[74], out ShiftDiffDollar);
                Decimal.TryParse(rowArray[75], out ShiftDiffDollarHrs);
                Decimal.TryParse(rowArray[76], out SpfnDollar);
                Decimal.TryParse(rowArray[77], out SpfnDollarHrs);
                Decimal.TryParse(rowArray[78], out ThirdPartySick);
                Decimal.TryParse(rowArray[79], out ThirdPartySickHrs);
                Decimal.TryParse(rowArray[80], out WNOT);
                Decimal.TryParse(rowArray[81], out WNOTHrs);
                Decimal.TryParse(rowArray[82], out WNVac);
                Decimal.TryParse(rowArray[83], out WNVacHrs);
                Decimal.TryParse(rowArray[84], out PdTimeOffDollar);
                Decimal.TryParse(rowArray[85], out PdTimeOffHrs);
                Decimal.TryParse(rowArray[86], out DNUDollars);
                Decimal.TryParse(rowArray[87], out DNUHrs);
                Decimal.TryParse(rowArray[88], out PittTax);
                Decimal.TryParse(rowArray[89], out HSOA);
                Decimal.TryParse(rowArray[90], out HSOAHrs);

            #endregion


                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
                SqlConnection dbConnection = new SqlConnection(connectionString);

                string queryImportValues = "INSERT INTO [PaycomTemp] ([Client],[Distribution],[CheckNum],[PayDate],[Adjust],[Unit] " +
                                            " ,[Gross],[Hours],[FUTA],[MEDR],[PASUI],[OHSUI],[MISUI],[FLSUI],[NJSUI],[VASUI],[SSR],[Uniform],[Bereav],[BereavHrs]" +      
                                            ",[Bkft],[BkftHrs],[FFV],[FFVHrs],[Hol],[HolHrs],[Jury],[JuryHrs],[PTO],[PTOHrs],[Meet],[MeetHrs],[Meet1],[Meet1Hrs]" +
                                            ",[OTNonbud],[OTNonbudHrs],[Other],[OtherHrs],[OT],[OTHrs],[Rate4],[Rate4Hrs],[Rate5],[Rate5Hrs],[Reg],[RegHrs]" +
                                            ",[Retro],[RetroHrs],[ShiftDif],[ShiftDifHrs],[Spfn],[SpfnHrs],[SummerWN],[SummerWNHrs],[Summer],[SummerHrs]" +
                                            ",[Train],[TrainHrs],[Unpaid],[UnpaidHrs],[Bonus],[BonusHrs],[Incent],[IncentHrs],[InsBuy],[InsBuyHrs],[Ira],[IraHrs]" +
                                            ",[OtherDollars],[OtherDollarsHrs],[RegDollars],[RegDollarsHrs],[Retrocative],[RetrocativeHrs],[ShiftDiffDollar],[ShiftDiffDollarHrs]" +
                                            ",[SpfnDollars],[SpfnDollarsHrs],[ThirdPartySick],[ThirdPartySickHrs],[WNOT],[WNOTHrs],[WNVac],[WNVacHrs]" +
                                            ",[PdTimeOffDollar],[PdTimeOffHrs],[DNUDollars],[DNUHrs],[PittTax],[HSOA],[HSOAHrs] )" +
                                            " VALUES(@Client,@Distribution,@CheckNum,@PayDate,@Adjust,@Unit," +
                                            "@Gross,@Hours,@FUTA,@MEDR,@PASUI,@OHSUI,@MISUI,@FLSUI,@NJSUI,@VASUI,@SSR,@Uniform,@Bereav,@BereavHrs," +      
                                            "@Bkft,@BkftHrs,@FFV,@FFVHrs,@Hol,@HolHrs,@Jury,@JuryHrs,@PTO,@PTOHrs,@Meet,@MeetHrs,@Meet1,@Meet1Hrs," +
                                            "@OTNonbud,@OTNonbudHrs,@Other,@OtherHrs,@OT,@OTHrs,@Rate4,@Rate4Hrs,@Rate5,@Rate5Hrs,@Reg,@RegHrs," +
                                            "@Retro,@RetroHrs,@ShiftDif,@ShiftDifHrs,@Spfn,@SpfnHrs,@SummerWN,@SummerWNHrs,@Summer,@SummerHrs," +
                                            "@Train,@TrainHrs,@Unpaid,@UnpaidHrs,@Bonus,@BonusHrs,@Incent,@IncentHrs,@InsBuy,@InsBuyHrs,@Ira,@IraHrs," +
                                            "@OtherDollars,@OtherDollarsHrs,@RegDollars,@RegDollarsHrs,@Retrocative,@RetrocativeHrs,@ShiftDiffDollar,@ShiftDiffDollarHrs," +
                                            "@SpfnDollars,@SpfnDollarsHrs,@ThirdPartySick,@ThirdPartySickHrs,@WNOT,@WNOTHrs,@WNVac,@WNVacHrs," +
                                            "@PdTimeOffDollar,@PdTimeOffHrs,@DNUDollars,@DNUHrs,@PittTax,@HSOA,@HSOAHrs)";

                using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
                {
                    #region SQL Parameter creation and assignment
                    SqlParameter dbParam_Client = new SqlParameter();
                    dbParam_Client.ParameterName = "@Client";
                    dbParam_Client.Value = ClientCode;
                    dbParam_Client.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(dbParam_Client);

                    SqlParameter dbParam_Distr = new SqlParameter();
                    dbParam_Distr.ParameterName = "@Distribution";
                    dbParam_Distr.Value = Distribution;
                    dbParam_Distr.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(dbParam_Distr);

                    SqlParameter dbParam_CheckNum = new SqlParameter();
                    dbParam_CheckNum.ParameterName = "@CheckNum";
                    dbParam_CheckNum.Value = CheckNum;
                    dbParam_CheckNum.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(dbParam_CheckNum);

                    SqlParameter dbParam_PayDate = new SqlParameter();
                    dbParam_PayDate.ParameterName = "@PayDate";
                    dbParam_PayDate.Value = PayDate;
                    dbParam_PayDate.DbType = System.Data.DbType.DateTime;
                    cmd.Parameters.Add(dbParam_PayDate);

                    SqlParameter dbParam_Adjust = new SqlParameter();
                    dbParam_Adjust.ParameterName = "@Adjust";
                    dbParam_Adjust.Value = Adjust;
                    dbParam_Adjust.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(dbParam_Adjust);

                    SqlParameter dbParam_Unit = new SqlParameter();
                    dbParam_Unit.ParameterName = "@Unit";
                    dbParam_Unit.Value = Unit;
                    dbParam_Unit.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(dbParam_Unit);

                    SqlParameter dbParam_Gross = new SqlParameter();
                    dbParam_Gross.ParameterName = "@Gross";
                    dbParam_Gross.Value = Gross;
                    dbParam_Gross.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Gross);

                    SqlParameter dbParam_Hours = new SqlParameter();
                    dbParam_Hours.ParameterName = "@Hours";
                    dbParam_Hours.Value = Hours;
                    dbParam_Hours.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Hours);

                    SqlParameter dbParam_FUTA = new SqlParameter();
                    dbParam_FUTA.ParameterName = "@FUTA";
                    dbParam_FUTA.Value = FUTA;
                    dbParam_FUTA.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_FUTA);

                    SqlParameter dbParam_MEDR = new SqlParameter();
                    dbParam_MEDR.ParameterName = "@MEDR";
                    dbParam_MEDR.Value = MEDR;
                    dbParam_MEDR.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_MEDR);

                    SqlParameter dbParam_PASUI = new SqlParameter();
                    dbParam_PASUI.ParameterName = "@PASUI";
                    dbParam_PASUI.Value = PASUI;
                    dbParam_PASUI.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_PASUI);

                    SqlParameter dbParam_OHSUI = new SqlParameter();
                    dbParam_OHSUI.ParameterName = "@OHSUI";
                    dbParam_OHSUI.Value = OHSUI;
                    dbParam_OHSUI.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_OHSUI);

                    SqlParameter dbParam_MISUI = new SqlParameter();
                    dbParam_MISUI.ParameterName = "@MISUI";
                    dbParam_MISUI.Value = MISUI;
                    dbParam_MISUI.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_MISUI);

                    SqlParameter dbParam_FLSUI = new SqlParameter();
                    dbParam_FLSUI.ParameterName = "@FLSUI";
                    dbParam_FLSUI.Value = FLSUI;
                    dbParam_FLSUI.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_FLSUI);

                    SqlParameter dbParam_NJSUI = new SqlParameter();
                    dbParam_NJSUI.ParameterName = "@NJSUI";
                    dbParam_NJSUI.Value = NJSUI;
                    dbParam_NJSUI.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_NJSUI);

                    SqlParameter dbParam_VASUI = new SqlParameter();
                    dbParam_VASUI.ParameterName = "@VASUI";
                    dbParam_VASUI.Value = VASUI;
                    dbParam_VASUI.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_VASUI);

                    SqlParameter dbParam_SSR = new SqlParameter();
                    dbParam_SSR.ParameterName = "@SSR";
                    dbParam_SSR.Value = SSR;
                    dbParam_SSR.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_SSR);

                    SqlParameter dbParam_Uniform = new SqlParameter();
                    dbParam_Uniform.ParameterName = "@Uniform";
                    dbParam_Uniform.Value = Uniform;
                    dbParam_Uniform.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Uniform);

                    SqlParameter dbParam_Bereav = new SqlParameter();
                    dbParam_Bereav.ParameterName = "@Bereav";
                    dbParam_Bereav.Value = Bereav;
                    dbParam_Bereav.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Bereav);

                    SqlParameter dbParam_BereavHrs = new SqlParameter();
                    dbParam_BereavHrs.ParameterName = "@BereavHrs";
                    dbParam_BereavHrs.Value = BereavHrs;
                    dbParam_BereavHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_BereavHrs);

                    SqlParameter dbParam_Bkft = new SqlParameter();
                    dbParam_Bkft.ParameterName = "@Bkft";
                    dbParam_Bkft.Value = Bkft;
                    dbParam_Bkft.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Bkft);

                    SqlParameter dbParam_BkftHrs = new SqlParameter();
                    dbParam_BkftHrs.ParameterName = "@BkftHrs";
                    dbParam_BkftHrs.Value = BkftHrs;
                    dbParam_BkftHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_BkftHrs);

                    SqlParameter dbParam_FFV = new SqlParameter();
                    dbParam_FFV.ParameterName = "@FFV";
                    dbParam_FFV.Value = FFV;
                    dbParam_FFV.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_FFV);

                    SqlParameter dbParam_FFVHrs = new SqlParameter();
                    dbParam_FFVHrs.ParameterName = "@FFVHrs";
                    dbParam_FFVHrs.Value = FFVHrs;
                    dbParam_FFVHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_FFVHrs);

                    SqlParameter dbParam_Hol = new SqlParameter();
                    dbParam_Hol.ParameterName = "@Hol";
                    dbParam_Hol.Value = Hol;
                    dbParam_Hol.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Hol);

                    SqlParameter dbParam_HolHrs = new SqlParameter();
                    dbParam_HolHrs.ParameterName = "@HolHrs";
                    dbParam_HolHrs.Value = HolHrs;
                    dbParam_HolHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_HolHrs);

                    SqlParameter dbParam_Jury = new SqlParameter();
                    dbParam_Jury.ParameterName = "@Jury";
                    dbParam_Jury.Value = Jury;
                    dbParam_Jury.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Jury);

                    SqlParameter dbParam_JuryHrs = new SqlParameter();
                    dbParam_JuryHrs.ParameterName = "@JuryHrs";
                    dbParam_JuryHrs.Value = JuryHrs;
                    dbParam_JuryHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_JuryHrs);

                    SqlParameter dbParam_PTO = new SqlParameter();
                    dbParam_PTO.ParameterName = "@PTO";
                    dbParam_PTO.Value = PTO;
                    dbParam_PTO.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_PTO);

                    SqlParameter dbParam_PTOHrs = new SqlParameter();
                    dbParam_PTOHrs.ParameterName = "@PTOHrs";
                    dbParam_PTOHrs.Value = PTOHrs;
                    dbParam_PTOHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_PTOHrs);

                    SqlParameter dbParam_Meet = new SqlParameter();
                    dbParam_Meet.ParameterName = "@Meet";
                    dbParam_Meet.Value = Meet;
                    dbParam_Meet.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Meet);

                    SqlParameter dbParam_MeetHrs = new SqlParameter();
                    dbParam_MeetHrs.ParameterName = "@MeetHrs";
                    dbParam_MeetHrs.Value = MeetHrs;
                    dbParam_MeetHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_MeetHrs);

                    SqlParameter dbParam_Meet1 = new SqlParameter();
                    dbParam_Meet1.ParameterName = "@Meet1";
                    dbParam_Meet1.Value = Meet1;
                    dbParam_Meet1.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Meet1);

                    SqlParameter dbParam_Meet1Hrs = new SqlParameter();
                    dbParam_Meet1Hrs.ParameterName = "@Meet1Hrs";
                    dbParam_Meet1Hrs.Value = Meet1Hrs;
                    dbParam_Meet1Hrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Meet1Hrs);

                    SqlParameter dbParam_OTNonBud = new SqlParameter();
                    dbParam_OTNonBud.ParameterName = "@OTNonBud";
                    dbParam_OTNonBud.Value = OTNonBud;
                    dbParam_OTNonBud.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_OTNonBud);

                    SqlParameter dbParam_OTNonBudHrs = new SqlParameter();
                    dbParam_OTNonBudHrs.ParameterName = "@OTNonBudHrs";
                    dbParam_OTNonBudHrs.Value = OTNonBudHrs;
                    dbParam_OTNonBudHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_OTNonBudHrs);

                    SqlParameter dbParam_Other = new SqlParameter();
                    dbParam_Other.ParameterName = "@Other";
                    dbParam_Other.Value = Other;
                    dbParam_Other.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Other);

                    SqlParameter dbParam_OtherHrs = new SqlParameter();
                    dbParam_OtherHrs.ParameterName = "@OtherHrs";
                    dbParam_OtherHrs.Value = OtherHrs;
                    dbParam_OtherHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_OtherHrs);

                    SqlParameter dbParam_OT = new SqlParameter();
                    dbParam_OT.ParameterName = "@OT";
                    dbParam_OT.Value = OT;
                    dbParam_OT.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_OT);

                    SqlParameter dbParam_OTHrs = new SqlParameter();
                    dbParam_OTHrs.ParameterName = "@OTHrs";
                    dbParam_OTHrs.Value = OTHrs;
                    dbParam_OTHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_OTHrs);

                    SqlParameter dbParam_Rate4 = new SqlParameter();
                    dbParam_Rate4.ParameterName = "@Rate4";
                    dbParam_Rate4.Value = Rate4;
                    dbParam_Rate4.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Rate4);

                    SqlParameter dbParam_Rate4Hrs = new SqlParameter();
                    dbParam_Rate4Hrs.ParameterName = "@Rate4Hrs";
                    dbParam_Rate4Hrs.Value = Rate4Hrs;
                    dbParam_Rate4Hrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Rate4Hrs);

                    SqlParameter dbParam_Rate5 = new SqlParameter();
                    dbParam_Rate5.ParameterName = "@Rate5";
                    dbParam_Rate5.Value = Rate5;
                    dbParam_Rate5.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Rate5);

                    SqlParameter dbParam_Rate5Hrs = new SqlParameter();
                    dbParam_Rate5Hrs.ParameterName = "@Rate5Hrs";
                    dbParam_Rate5Hrs.Value = Rate5Hrs;
                    dbParam_Rate5Hrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Rate5Hrs);

                    SqlParameter dbParam_Reg = new SqlParameter();
                    dbParam_Reg.ParameterName = "@Reg";
                    dbParam_Reg.Value = Reg;
                    dbParam_Reg.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Reg);

                    SqlParameter dbParam_RegHrs = new SqlParameter();
                    dbParam_RegHrs.ParameterName = "@RegHrs";
                    dbParam_RegHrs.Value = RegHrs;
                    dbParam_RegHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_RegHrs);

                    SqlParameter dbParam_Retro = new SqlParameter();
                    dbParam_Retro.ParameterName = "@Retro";
                    dbParam_Retro.Value = Retro;
                    dbParam_Retro.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Retro);

                    SqlParameter dbParam_RetroHrs = new SqlParameter();
                    dbParam_RetroHrs.ParameterName = "@RetroHrs";
                    dbParam_RetroHrs.Value = RetroHrs;
                    dbParam_RetroHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_RetroHrs);

                    SqlParameter dbParam_ShiftDif = new SqlParameter();
                    dbParam_ShiftDif.ParameterName = "@ShiftDif";
                    dbParam_ShiftDif.Value = ShiftDif;
                    dbParam_ShiftDif.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_ShiftDif);

                    SqlParameter dbParam_ShiftDifHrs = new SqlParameter();
                    dbParam_ShiftDifHrs.ParameterName = "@ShiftDifHrs";
                    dbParam_ShiftDifHrs.Value = ShiftDifHrs;
                    dbParam_ShiftDifHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_ShiftDifHrs);

                    SqlParameter dbParam_Spfn = new SqlParameter();
                    dbParam_Spfn.ParameterName = "@Spfn";
                    dbParam_Spfn.Value = Spfn;
                    dbParam_Spfn.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Spfn);

                    SqlParameter dbParam_SpfnHrs = new SqlParameter();
                    dbParam_SpfnHrs.ParameterName = "@SpfnHrs";
                    dbParam_SpfnHrs.Value = SpfnHrs;
                    dbParam_SpfnHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_SpfnHrs);

                    SqlParameter dbParam_SummerWN = new SqlParameter();
                    dbParam_SummerWN.ParameterName = "@SummerWN";
                    dbParam_SummerWN.Value = SummerWN;
                    dbParam_SummerWN.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_SummerWN);

                    SqlParameter dbParam_SummerWNHrs = new SqlParameter();
                    dbParam_SummerWNHrs.ParameterName = "@SummerWNHrs";
                    dbParam_SummerWNHrs.Value = SummerWNHrs;
                    dbParam_SummerWNHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_SummerWNHrs);

                    SqlParameter dbParam_Summer = new SqlParameter();
                    dbParam_Summer.ParameterName = "@Summer";
                    dbParam_Summer.Value = Summer;
                    dbParam_Summer.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Summer);

                    SqlParameter dbParam_SummerHrs = new SqlParameter();
                    dbParam_SummerHrs.ParameterName = "@SummerHrs";
                    dbParam_SummerHrs.Value = SummerHrs;
                    dbParam_SummerHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_SummerHrs);

                    SqlParameter dbParam_Train = new SqlParameter();
                    dbParam_Train.ParameterName = "@Train";
                    dbParam_Train.Value = Train;
                    dbParam_Train.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Train);

                    SqlParameter dbParam_TrainHrs = new SqlParameter();
                    dbParam_TrainHrs.ParameterName = "@TrainHrs";
                    dbParam_TrainHrs.Value = TrainHrs;
                    dbParam_TrainHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_TrainHrs);

                    SqlParameter dbParam_Unpaid = new SqlParameter();
                    dbParam_Unpaid.ParameterName = "@Unpaid";
                    dbParam_Unpaid.Value = Unpaid;
                    dbParam_Unpaid.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Unpaid);

                    SqlParameter dbParam_UnpaidHrs = new SqlParameter();
                    dbParam_UnpaidHrs.ParameterName = "@UnpaidHrs";
                    dbParam_UnpaidHrs.Value = UnpaidHrs;
                    dbParam_UnpaidHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_UnpaidHrs);

                    SqlParameter dbParam_Bonus = new SqlParameter();
                    dbParam_Bonus.ParameterName = "@Bonus";
                    dbParam_Bonus.Value = Bonus;
                    dbParam_Bonus.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Bonus);

                    SqlParameter dbParam_BonusHrs = new SqlParameter();
                    dbParam_BonusHrs.ParameterName = "@BonusHrs";
                    dbParam_BonusHrs.Value = BonusHrs;
                    dbParam_BonusHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_BonusHrs);

                    SqlParameter dbParam_Incent = new SqlParameter();
                    dbParam_Incent.ParameterName = "@Incent";
                    dbParam_Incent.Value = Incent;
                    dbParam_Incent.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Incent);

                    SqlParameter dbParam_IncentHrs = new SqlParameter();
                    dbParam_IncentHrs.ParameterName = "@IncentHrs";
                    dbParam_IncentHrs.Value = IncentHrs;
                    dbParam_IncentHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_IncentHrs);

                    SqlParameter dbParam_InsBuy = new SqlParameter();
                    dbParam_InsBuy.ParameterName = "@InsBuy";
                    dbParam_InsBuy.Value = InsBuy;
                    dbParam_InsBuy.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_InsBuy);

                    SqlParameter dbParam_InsBuyHrs = new SqlParameter();
                    dbParam_InsBuyHrs.ParameterName = "@InsBuyHrs";
                    dbParam_InsBuyHrs.Value = InsBuyHrs;
                    dbParam_InsBuyHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_InsBuyHrs);

                    SqlParameter dbParam_Ira = new SqlParameter();
                    dbParam_Ira.ParameterName = "@Ira";
                    dbParam_Ira.Value = Ira;
                    dbParam_Ira.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Ira);

                    SqlParameter dbParam_IraHrs = new SqlParameter();
                    dbParam_IraHrs.ParameterName = "@IraHrs";
                    dbParam_IraHrs.Value = IraHrs;
                    dbParam_IraHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_IraHrs);

                    SqlParameter dbParam_OtherDollars = new SqlParameter();
                    dbParam_OtherDollars.ParameterName = "@OtherDollars";
                    dbParam_OtherDollars.Value = OtherDollars;
                    dbParam_OtherDollars.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_OtherDollars);

                    SqlParameter dbParam_OtherDollarsHrs = new SqlParameter();
                    dbParam_OtherDollarsHrs.ParameterName = "@OtherDollarsHrs";
                    dbParam_OtherDollarsHrs.Value = OtherDollarsHrs;
                    dbParam_OtherDollarsHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_OtherDollarsHrs);

                    SqlParameter dbParam_RegDollars = new SqlParameter();
                    dbParam_RegDollars.ParameterName = "@RegDollars";
                    dbParam_RegDollars.Value = RegDollars;
                    dbParam_RegDollars.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_RegDollars);

                    SqlParameter dbParam_RegDollarsHrs = new SqlParameter();
                    dbParam_RegDollarsHrs.ParameterName = "@RegDollarsHrs";
                    dbParam_RegDollarsHrs.Value = RegDollarsHrs;
                    dbParam_RegDollarsHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_RegDollarsHrs);

                    SqlParameter dbParam_Retroactive = new SqlParameter();
                    dbParam_Retroactive.ParameterName = "@Retrocative";
                    dbParam_Retroactive.Value = Retroactive;
                    dbParam_Retroactive.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_Retroactive);

                    SqlParameter dbParam_RetroactiveHrs = new SqlParameter();
                    dbParam_RetroactiveHrs.ParameterName = "@RetrocativeHrs";
                    dbParam_RetroactiveHrs.Value = RetroactiveHrs;
                    dbParam_RetroactiveHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_RetroactiveHrs);

                    SqlParameter dbParam_ShiftDiffDollar = new SqlParameter();
                    dbParam_ShiftDiffDollar.ParameterName = "@ShiftDiffDollar";
                    dbParam_ShiftDiffDollar.Value = ShiftDiffDollar;
                    dbParam_ShiftDiffDollar.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_ShiftDiffDollar);

                    SqlParameter dbParam_ShiftDiffDollarHrs = new SqlParameter();
                    dbParam_ShiftDiffDollarHrs.ParameterName = "@ShiftDiffDollarHrs";
                    dbParam_ShiftDiffDollarHrs.Value = ShiftDiffDollarHrs;
                    dbParam_ShiftDiffDollarHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_ShiftDiffDollarHrs);

                    SqlParameter dbParam_SpfnDollar = new SqlParameter();
                    dbParam_SpfnDollar.ParameterName = "@SpfnDollars";
                    dbParam_SpfnDollar.Value = SpfnDollar;
                    dbParam_SpfnDollar.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_SpfnDollar);

                    SqlParameter dbParam_SpfnDollarHrs = new SqlParameter();
                    dbParam_SpfnDollarHrs.ParameterName = "@SpfnDollarsHrs";
                    dbParam_SpfnDollarHrs.Value = SpfnDollarHrs;
                    dbParam_SpfnDollarHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_SpfnDollarHrs);

                    SqlParameter dbParam_ThirdPartySick = new SqlParameter();
                    dbParam_ThirdPartySick.ParameterName = "@ThirdPartySick";
                    dbParam_ThirdPartySick.Value = ThirdPartySick;
                    dbParam_ThirdPartySick.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_ThirdPartySick);

                    SqlParameter dbParam_ThirdPartySickHrs = new SqlParameter();
                    dbParam_ThirdPartySickHrs.ParameterName = "@ThirdPartySickHrs";
                    dbParam_ThirdPartySickHrs.Value = ThirdPartySickHrs;
                    dbParam_ThirdPartySickHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_ThirdPartySickHrs);

                    SqlParameter dbParam_WNOT = new SqlParameter();
                    dbParam_WNOT.ParameterName = "@WNOT";
                    dbParam_WNOT.Value = WNOT;
                    dbParam_WNOT.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_WNOT);

                    SqlParameter dbParam_WNOTHrs = new SqlParameter();
                    dbParam_WNOTHrs.ParameterName = "@WNOTHrs";
                    dbParam_WNOTHrs.Value = WNOTHrs;
                    dbParam_WNOTHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_WNOTHrs);

                    SqlParameter dbParam_WNVac = new SqlParameter();
                    dbParam_WNVac.ParameterName = "@WNVac";
                    dbParam_WNVac.Value = WNVac;
                    dbParam_WNVac.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_WNVac);

                    SqlParameter dbParam_WNVacHrs = new SqlParameter();
                    dbParam_WNVacHrs.ParameterName = "@WNVacHrs";
                    dbParam_WNVacHrs.Value = WNVacHrs;
                    dbParam_WNVacHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_WNVacHrs);

                    SqlParameter dbParam_PdTimeOffDollar = new SqlParameter();
                    dbParam_PdTimeOffDollar.ParameterName = "@PdTimeOffDollar";
                    dbParam_PdTimeOffDollar.Value = PdTimeOffDollar;
                    dbParam_PdTimeOffDollar.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_PdTimeOffDollar);

                    SqlParameter dbParam_PdTimeOffHrs = new SqlParameter();
                    dbParam_PdTimeOffHrs.ParameterName = "@PdTimeOffHrs";
                    dbParam_PdTimeOffHrs.Value = PdTimeOffHrs;
                    dbParam_PdTimeOffHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_PdTimeOffHrs);

                    SqlParameter dbParam_DNUDollars = new SqlParameter();
                    dbParam_DNUDollars.ParameterName = "@DNUDollars";
                    dbParam_DNUDollars.Value = DNUDollars;
                    dbParam_DNUDollars.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_DNUDollars);

                    SqlParameter dbParam_DNUHrs = new SqlParameter();
                    dbParam_DNUHrs.ParameterName = "@DNUHrs";
                    dbParam_DNUHrs.Value = DNUHrs;
                    dbParam_DNUHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_DNUHrs);

                    SqlParameter dbParam_PittTax = new SqlParameter();
                    dbParam_PittTax.ParameterName = "@PittTax";
                    dbParam_PittTax.Value = PittTax;
                    dbParam_PittTax.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_PittTax);

                    SqlParameter dbParam_HSOA = new SqlParameter();
                    dbParam_HSOA.ParameterName = "@HSOA";
                    dbParam_HSOA.Value = HSOA;
                    dbParam_HSOA.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_HSOA);

                    SqlParameter dbParam_HSOAHrs = new SqlParameter();
                    dbParam_HSOAHrs.ParameterName = "@HSOAHrs";
                    dbParam_HSOAHrs.Value = HSOAHrs;
                    dbParam_HSOAHrs.DbType = System.Data.DbType.Decimal;
                    cmd.Parameters.Add(dbParam_HSOAHrs);
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

        protected void clearTempPayrollTable()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.IDbConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "TRUNCATE TABLE [PaycomTemp]";

            System.Data.IDbCommand dbCommand = new System.Data.SqlClient.SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;


            try
            {
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                TextBoxStatus.Text += Environment.NewLine + "-Error clearing the temp table.";
                TextBoxStatus.Rows++;
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        protected bool runSummarizePayrollSP()
        {
            bool success = false;
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "INSERT INTO PayTax ([ProgramYear],[DistrictID],[EndDate],[StartDate] ,[Gross],[Hours],[ProdHrs],[NonProdHrs],[SpecFunctionHrs],[FUTA] ,[Medicare],[PASUI],[SocSec],[Uniforms],[PayAccrual],[TaxAccrual]) " +
                " SELECT @ProgramYear, RIGHT([DistrictID],3), @EndDate, @StartDate, [SumOfGross],[SumOfHours],[ProdHrs],[NonProdHrs],[SpFnHr],[SumOfFUTA],[SumOfMEDR], " +
                " [sumofPASUI]+[SumofOHSUI]+[SumofMISUI]+[SumofFLSUI]+[SumofNJSUI]+[SumofPittTax] AS SUI," +
                " SumofSSR, [SumofUniform]*-1 AS Uniform, 0 AS PayAccrual, 0 AS TaxAccrual " +
                " FROM [PaycomSumPayPeriod] " +
#if DEBUG
                " WHERE RIGHT([DistrictID],1) <'a' ";
#else
            //LIVE
                " WHERE RIGHT([DistrictID],3) IN(SELECT DISTRICTID FROM DISTRICT) ";
#endif

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.CommandType = System.Data.CommandType.Text;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
                dbParam_ProgYear.DbType = System.Data.DbType.String;
                dbCommand.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_startingDate = new SqlParameter();
                dbParam_startingDate.ParameterName = "@StartDate";
                dbParam_startingDate.Value = TextBoxBeginningDate.Text;
                dbParam_startingDate.DbType = System.Data.DbType.DateTime;
                dbCommand.Parameters.Add(dbParam_startingDate);

                SqlParameter dbParam_endingDate = new SqlParameter();
                dbParam_endingDate.ParameterName = "@EndDate";
                dbParam_endingDate.Value = TextBoxEndingDate.Text;
                dbParam_endingDate.DbType = System.Data.DbType.DateTime;
                dbCommand.Parameters.Add(dbParam_endingDate);


                try
                {
                    dbConnection.Open();
                    dbCommand.ExecuteNonQuery();
                    success = true;
                }
                catch (SqlException ex)
                {
                    TextBoxStatus.Text += Environment.NewLine + "-Sql error while summarizing the Payroll records.";
                    TextBoxStatus.Rows++;
                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                    }

                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                    success = false;
                }
                catch (Exception ex)
                {
                    TextBoxStatus.Text += Environment.NewLine + "-Error while summarizing the Payroll records.";
                    TextBoxStatus.Rows++;
                    if (CB_VerboseErrors.Checked)
                    {
                        TextBoxStatus.Text += Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                    }
                    TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                    success = false;
                }
                finally
                {
                    dbConnection.Close();

                }
            }
            return success;
        }

    }
}
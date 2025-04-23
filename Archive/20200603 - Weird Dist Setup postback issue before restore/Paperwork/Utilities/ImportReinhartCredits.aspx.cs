using System;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class ImportReinhartCredits : Paperwork_Global
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
                HFCounterFilterStart.Value = "0";
                HFCounterFilterEnd.Value = "9223372036854775807";

                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DDLDistricts.DataSource = SqlDataSourceMyDistsMstr;

                DDLDistricts.DataBind();
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;
            HFCounterFilterStart.Value = (GetMaxReinhartCreditCounter()).ToString();

            if (FileUpload1.PostedFile != null && !string.IsNullOrEmpty( FileUpload1.PostedFile.FileName))
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

                    //Read the contents of CSV file.
                    string csvData = File.ReadAllText(path);

                    Boolean importErrors = false;

                    int recordCount = 0;
                    int duplicateRecordCount=0;

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            //TextBoxStatus.Text += Environment.NewLine + " [ROW] " + row;
                            //TextBoxStatus.Rows++;

                            Boolean DuplicateRecord = false;

                            if (CreateImportRecord(row, out DuplicateRecord) == false)
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

                            if (DuplicateRecord)
                                duplicateRecordCount++;

                        }
                    }

                    //Duplicate notification
                    if(duplicateRecordCount>0)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-" + duplicateRecordCount.ToString() + " duplicate records were detected & skipped.";
                        TextBoxStatus.Rows++;
                    }

                    //No Errors
                    if (importErrors == false)
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + recordCount.ToString() + " records from the import file. Import complete.";
                        TextBoxStatus.Rows++;
                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + recordCount.ToString() + " records from the import file. **Errors caused the import to end**.";
                        TextBoxStatus.Rows++;
                    }

                    try
                    {
                        File.Delete(path);
                    }
                    catch { }//silent catch

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

            HFCounterFilterEnd.Value = (GetMaxReinhartCreditCounter()).ToString();
        }

        /// <summary>
        /// Parse the row string and create the Import Table record
        /// </summary>
        /// <param name="row">Row string with comma separated values</param>
        /// <returns>Boolean indicating if there were errors</returns>
        private Boolean CreateImportRecord(string row, out Boolean Duplicate)
        {
            Boolean RecordCreated = false;
            Duplicate = false;

            DateTime InvoiceDate;
            Decimal CreditTotal;
            Decimal Promotional;
            Decimal Competitive;
            Decimal NetOffInvoice;
            Int32 Mth;
            String Type;
            String DistrictID;

            string[] rowArray = row.Split(',');



            DistrictID = rowArray[0];
            if (string.IsNullOrEmpty(DistrictID))
                return false;
            if (DistrictID.Length > 3)
                DistrictID = String.Empty;

            Type = rowArray[1].Trim('"');//need to remove the quotes
            if (string.IsNullOrEmpty(Type))
                return false;

            if(Int32.TryParse(rowArray[2], out Mth) == false)
                return false;

            if (DateTime.TryParse(rowArray[3], out InvoiceDate) == false)
                return false;

            if (Decimal.TryParse(rowArray[4], out CreditTotal) == false)
                return false;

            if (Decimal.TryParse(rowArray[5], out Promotional) == false)
                return false;

            if (Decimal.TryParse(rowArray[6], out Competitive) == false)
                return false;

            if (Decimal.TryParse(rowArray[7], out NetOffInvoice) == false)
                return false;


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [ReinhartCredits] ([ProgramYear],[DistrictID],[InvoiceDate],[Type],[Month],[Total],[Promotional],[Competitive],[NetOffInvoice]) "
                                        + " VALUES(@ProgramYear,RIGHT('000' + @DistrictID,3),@InvoiceDate,@Type,@Month,@Total,@Promotional,@Competitive,@NetOffInvoice)";

            using (SqlCommand cmd = new SqlCommand(queryImportValues, dbConnection))
            {
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
                dbParam_ProgYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_InvDate = new SqlParameter();
                dbParam_InvDate.ParameterName = "@InvoiceDate";
                dbParam_InvDate.Value = InvoiceDate;
                dbParam_InvDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_Type = new SqlParameter();
                dbParam_Type.ParameterName = "@Type";
                dbParam_Type.Value = Type;
                dbParam_Type.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_Type);

                SqlParameter dbParam_Month = new SqlParameter();
                dbParam_Month.ParameterName = "@Month";
                dbParam_Month.Value = Mth;
                dbParam_Month.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_Month);

                SqlParameter dbParam_Total = new SqlParameter();
                dbParam_Total.ParameterName = "@Total";
                dbParam_Total.Value = CreditTotal;
                dbParam_Total.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Total);

                SqlParameter dbParam_Promo = new SqlParameter();
                dbParam_Promo.ParameterName = "@Promotional";
                dbParam_Promo.Value = Promotional;
                dbParam_Promo.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Promo);

                SqlParameter dbParam_Comp = new SqlParameter();
                dbParam_Comp.ParameterName = "@Competitive";
                dbParam_Comp.Value = Competitive;
                dbParam_Comp.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Comp);

                SqlParameter dbParam_NetOff = new SqlParameter();
                dbParam_NetOff.ParameterName = "@NetOffInvoice";
                dbParam_NetOff.Value = NetOffInvoice;
                dbParam_NetOff.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_NetOff);


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
                    if (ex.Message.IndexOf("duplicate")>0 )//Duplicate record 
                    {
                        /*do nothing - just skip and handle error*/
                        RecordCreated = true;
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

        protected void BtnImportFile_Click(object sender, EventArgs e)
        {
            if(Page.IsValid)
                Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#myModal').modal('show');", true);
        }

        protected int GetMaxReinhartCreditCounter()
        {
            int returnCounter = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);
            string counterQuery = "SELECT MAX(Counter) FROM [ReinhartCredits]";

            using (SqlCommand cmd = new SqlCommand(counterQuery, dbConnection))
            {
                try
                {
                    using(dbConnection)
                    {
                        dbConnection.Open();
                        Object retObj = cmd.ExecuteScalar();
                        if (retObj != null)
                            int.TryParse(retObj.ToString(), out returnCounter);
                    }
                    
                }
                catch (SqlException ex)
                {

                }
                catch (Exception ex)
                {

                }

            }

            return returnCounter;

        }
    }
}
using System;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Paperwork
{
    public partial class ImportInsurance : Paperwork_Global
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
                InsDateFilter.Text = DateTime.Now.ToShortDateString();
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;

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

                    //Read the contents of Tab Delimited file.
                    string csvData = File.ReadAllText(path);

                    Boolean importErrors = false;

                    int recordCount = 0;
                    int duplicateRecordCount=0;

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            Boolean DuplicateRecord = false;

                            if(row.IndexOf("Date1")>=0 || row.IndexOf("\t\t") >= 0)
                            {
                                //skip header row, do not increment recordcount
                                //skip footer row which has double tab and total
                            }
                            else if (CreateImportRecord(row, out DuplicateRecord) == false)
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
                        TextBoxStatus.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        TextBoxStatus.Text += Environment.NewLine + "-Read " + recordCount.ToString() + " records from the import file. **Errors caused the import to end**.";
                        TextBoxStatus.Rows++;
                        TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                    }

                    try
                    {
                        File.Delete(path);
                    }
                    catch { }//silent catch

                    //refresh the imported record grid
                    GridViewPayInsRecords.DataBind();

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
        private Boolean CreateImportRecord(string row, out Boolean Duplicate)
        {
            Boolean RecordCreated = false;
            Duplicate = false;

            DateTime InsuranceDate;
            Decimal InsuranceTotal;
            String DistrictID;


            //Tab file - split with the tab character
            string[] rowArray = row.Split('\t');

                //  Spec: Import Insurance Spec
                //	• Date1
                //	• Unit
                //	• Amount
                //  YrID

            if (DateTime.TryParse(rowArray[0], out InsuranceDate) == false)
                return false;

            DistrictID = rowArray[1];

            if (decimal.TryParse(rowArray[2], out InsuranceTotal) == false)
                return false;

            InsDateFilter.Text = InsuranceDate.ToShortDateString();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryImportValues = "INSERT INTO [PayIns] ([ProgramYear],[DistrictID],[InsuranceDate],[Amount]) "
                                        + " VALUES(@ProgramYear,@DistrictID,@InvoiceDate,@Amount)";

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
                dbParam_InvDate.Value = InsuranceDate;
                dbParam_InvDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_InvDate);

                SqlParameter dbParam_Total = new SqlParameter();
                dbParam_Total.ParameterName = "@Amount";
                dbParam_Total.Value = InsuranceTotal;
                dbParam_Total.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_Total);

                try
                {
                    dbConnection.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        RecordCreated = true;
                    }
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
    }
}
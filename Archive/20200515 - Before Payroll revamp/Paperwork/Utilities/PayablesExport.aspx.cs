using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Web;

namespace Paperwork
{


    public partial class PayablesExport : Paperwork_Global
    {
        protected static string ExportPath = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");
            
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
                
                //TextBoxBatchID.Text = "PPWK_" + Session["CurrentDistrict"] + "_" + DateTime.Now.ToString("MMddyy");

                //get export path from global settings
                ExportPath = GetGlobalSettingString("PayablesExportFolder");
                LabelExportPath.Text = ExportPath;
                string ErrorInfo = "";

                if (hasWriteAccessToFolder(ExportPath, out ErrorInfo))
                {
                    LabelOK.Visible = true;
                    LabelBad.Visible = false;
                }
                else
                {
                    LabelOK.Visible = false;
                    LabelBad.Visible = true;
                    LabelBad.ToolTip = ErrorInfo;
                }
            }


            if (Session["AccessLevel"] == null || (Session["AccessLevel"].ToString() != "Admin" && Session["AccessLevel"].ToString() != "Office"))
                Response.Redirect("~/Default.aspx");
        }

        protected void BtnExportToFile_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            TextBoxStatus.Text = string.Empty;
            TextBoxStatus.Rows = 0;
            TextBoxStatus.ForeColor = System.Drawing.Color.Gray;
            TextBoxStatus.Text = "Beginning export...";
            TextBoxStatus.Rows++;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            

            string queryString = "SELECT InvoiceDate, PostDate, P.VendorID, D.DistrictName, " +
                " Groceries Total5000, " +
                " '5000-' + RMCode + '-' + P.DistrictID PMAccNum5000, " +
                " Milk Total5006, " + //Milk
                " '5006-' + RMCode + '-' + P.DistrictID PMAccNum5006, " +
                " Paper Total5002, " +
                " '5002-' + RMCode + '-' + P.DistrictID PMAccNum5002, " +
                " Cleaning Total5003, " +
                " '5003-' + RMCode + '-' + P.DistrictID PMAccNum5003, " +
                " Other Total5005, " +
                " CASE WHEN ISNULL(V.VendorExpenseCode,'')='' THEN '5005' ELSE RTRIM(V.VendorExpenseCode) END " +
                " + '-' + RMCode + '-' + P.DistrictID PMAccNum5005, " +
                " InvoiceNumber + CASE WHEN P.VendorID IN (SELECT VENDORID FROM [Vendors] WHERE VENDNAME LIKE '%REINHART%') " +
                " THEN (SELECT ReinhartInvSuffix FROM [PaperworkGlobalSettings] WHERE [Index]=0) ELSE '' END InvoiceNumber" +
                " FROM Purchases P " +
                " INNER JOIN District D ON P.DistrictID = D.DistrictID " +
                " INNER JOIN DistrictYearInfo DYI ON P.DistrictID = DYI.DistrictID AND P.ProgramYear=DYI.ProgramYear " +
                " LEFT OUTER JOIN DistrictVendors V ON P.DistrictID=V.DistrictID AND P.VendorID=V.VENDORID " +
                " WHERE P.PostDate >= @StartPostDate AND P.PostDate <= @EndPostDate AND P.DistrictID=@DistrictID " +
                " ORDER BY InvoiceDate DESC ";

            try
            {
                using (SqlConnection dbConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand dbCommand = new SqlCommand(queryString, dbConnection))
                    {

                        SqlParameter dbParam_distID = new SqlParameter();
                        dbParam_distID.ParameterName = "@DistrictID";
                        dbParam_distID.Value = Session["CurrentDistrict"].ToString();
                        dbParam_distID.DbType = System.Data.DbType.String;
                        dbCommand.Parameters.Add(dbParam_distID);

                        SqlParameter dbParam_startingPostDate = new SqlParameter();
                        dbParam_startingPostDate.ParameterName = "@StartPostDate";
                        dbParam_startingPostDate.Value = TextBoxBeginningDate.Text;
                        dbParam_startingPostDate.DbType = System.Data.DbType.DateTime;
                        dbCommand.Parameters.Add(dbParam_startingPostDate);

                        SqlParameter dbParam_endingPostDate = new SqlParameter();
                        dbParam_endingPostDate.ParameterName = "@EndPostDate";
                        dbParam_endingPostDate.Value = TextBoxEndingDate.Text;
                        dbParam_endingPostDate.DbType = System.Data.DbType.DateTime;
                        dbCommand.Parameters.Add(dbParam_endingPostDate);

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            sda.SelectCommand = dbCommand;
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                if (dt.Rows.Count > 0)
                                {
                                    //Build the CSV file data as a Comma separated string.
                                    string csv = string.Empty;

                                    foreach (DataRow row in dt.Rows)
                                    {
                                        int colIndex = 0;
                                        foreach (DataColumn column in dt.Columns)
                                        {
                                            colIndex++;
                                            //Add the Data rows.
                                            if (colIndex > 2)
                                                csv += "\"" + row[column.ColumnName].ToString().Replace(",", ";") + "\",";
                                            else
                                                csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';//shouldn't need this .Replace("AM","")
                                        }

                                        //remove the trailing comma
                                        csv = csv.TrimEnd(',');

                                        //Add new line.
                                        csv += "\r\n";
                                    }

                                    string ErrorInfo = "";
                                    if (hasWriteAccessToFolder(ExportPath, out ErrorInfo))
                                    {
                                        try
                                        {
                                            string PayFileName = @ExportPath + Session["CurrentDistrict"].ToString() + ".txt";
                                            FileInfo fi = new FileInfo(PayFileName);

                                            FileStream fs = new FileStream(PayFileName, FileMode.Create);
                                            StreamWriter writer = new StreamWriter(fs);
                                            writer.Write(csv);
                                            writer.Close();

                                            MarkRecordsASImported();

                                            TextBoxStatus.Text += Environment.NewLine + "Saved " + Session["CurrentDistrict"].ToString() + ".txt to " + @ExportPath;
                                            TextBoxStatus.Rows++;
                                        }
                                        catch(IOException ex)
                                        {
                                            TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                                            TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                                            TextBoxStatus.Rows++;
                                            TextBoxStatus.Rows++;
                                        }
                                        catch (Exception ex)
                                        {
                                            TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                                            TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                                            TextBoxStatus.Rows++;
                                            TextBoxStatus.Rows++;
                                        }
                                    }
                                    else
                                    {
                                        //Download the CSV file.
                                        Response.Clear();
                                        Response.Buffer = true;
                                        Response.AddHeader("content-disposition", @"attachment;filename=" + Session["CurrentDistrict"].ToString() + ".txt");
                                        Response.Charset = "";
                                        Response.ContentType = "application/text";
                                        Response.Output.Write(csv);
                                        Response.Flush(); // Sends all currently buffered output to the client.
                                                          //Response.End(); // Apparently this always throws an Exception. Replaced with the following 2 lines of code.
                                        Response.SuppressContent = true; // Gets or sets a value indicating whether to send HTTP content to the client.
                                        HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

                                        MarkRecordsASImported();

                                        //TextBoxStatus update does not work with this method because the Flush call prevents the page from updating
                                    }
                                }
                                else
                                {
                                    TextBoxStatus.Text += Environment.NewLine + "No records found in that date range.";
                                    TextBoxStatus.Rows++;
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                TextBoxStatus.Rows++;
            }
            catch (Exception ex)
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                TextBoxStatus.Rows++;
                TextBoxStatus.Rows++;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Using the same page parameters - mark the purchase records as imported
        /// </summary>
        private void MarkRecordsASImported()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;


            string queryString = "UPDATE Purchases SET IMPORTED=1, ImportDate=GETDATE() " +
                " WHERE Purchases.PostDate >= @StartPostDate AND Purchases.PostDate <= @EndPostDate AND Purchases.DistrictID=@DistrictID ";

            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand dbCommand = new SqlCommand(queryString, dbConnection))
                {

                    SqlParameter dbParam_distID = new SqlParameter();
                    dbParam_distID.ParameterName = "@DistrictID";
                    dbParam_distID.Value = Session["CurrentDistrict"].ToString();
                    dbParam_distID.DbType = System.Data.DbType.String;
                    dbCommand.Parameters.Add(dbParam_distID);

                    SqlParameter dbParam_startingPostDate = new SqlParameter();
                    dbParam_startingPostDate.ParameterName = "@StartPostDate";
                    dbParam_startingPostDate.Value = TextBoxBeginningDate.Text;
                    dbParam_startingPostDate.DbType = System.Data.DbType.DateTime;
                    dbCommand.Parameters.Add(dbParam_startingPostDate);

                    SqlParameter dbParam_endingPostDate = new SqlParameter();
                    dbParam_endingPostDate.ParameterName = "@EndPostDate";
                    dbParam_endingPostDate.Value = TextBoxEndingDate.Text;
                    dbParam_endingPostDate.DbType = System.Data.DbType.DateTime;
                    dbCommand.Parameters.Add(dbParam_endingPostDate);

                    try
                    {
                        dbConnection.Open();
                        dbCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                    catch (Exception ex)
                    {
                        TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
            }
        }
    }
}
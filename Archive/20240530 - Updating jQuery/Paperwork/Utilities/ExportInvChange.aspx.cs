using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;

namespace Paperwork
{


    public partial class ExportInvChange : Paperwork_Global
    {
        protected static string ExportPath = "";

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
                    if (Session["ProgramYear"] != null && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }

                DDLProgramYears_SelectedIndexChanged(null, null);

                DDLStartInventory.DataBind();
                DDLEndInventory.DataBind();

                int currentMonth = DateTime.Now.Month;
                if (DDLEndInventory.Items.Count > 0)
                {
                    string monthVal = "";
                    if (currentMonth == 7)
                        monthVal = "*OPEN*";
                    else
                        monthVal = "-" + (currentMonth - 1).ToString().PadLeft(2, '0');

                    foreach (ListItem li in DDLEndInventory.Items)
                    {
                        if (li.Value.IndexOf(monthVal) > 0)
                        {
                            DDLEndInventory.SelectedValue = li.Value;
                            break;
                        }
                    }
                }

                if (DDLStartInventory.Items.Count >= DDLStartInventory.SelectedIndex + 1)
                {
                    if (DDLEndInventory.SelectedIndex == 0)
                        DDLStartInventory.SelectedIndex = 0;
                    else
                        DDLStartInventory.SelectedIndex = DDLEndInventory.SelectedIndex - 1;
                }

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


            string queryString = "SELECT GLDateStr " +
                    ",'5000-' + RMCode + '-' + DYI.DistrictID FoodAccount " +
                    ",ISNULL(IOPEN.Groceries, 0) - ISNULL(ICLOSE.Groceries, 0) FoodAmount " +
                    ",MonthStr + ' Inventory-Food' FoodLabel " +
                    ",'5006-' + RMCode + '-' + DYI.DistrictID MilkAccount " +
                    ",ISNULL(IOPEN.Milk, 0) - ISNULL(ICLOSE.Milk, 0) MilkAmount " +
                    ",MonthStr + ' Inventory-Milk' FoodLabel " +
                    ",'5002-' + RMCode + '-' + DYI.DistrictID PaperAccount " +
                    ",ISNULL(IOPEN.Paper, 0) - ISNULL(ICLOSE.Paper, 0) PaperAmount " +
                    ",MonthStr + ' Inventory-Paper' FoodLabel " +
                    ",'5003-' + RMCode + '-' + DYI.DistrictID CleaningAccount " +
                    ",ISNULL(IOPEN.Cleaning, 0) - ISNULL(ICLOSE.Cleaning, 0) CleaningAmount " +
                    ",MonthStr + ' Inventory-Cleaning' FoodLabel " +
                "FROM DistrictYearInfo DYI " +
                "INNER JOIN District D ON DYI.DistrictID = D.DistrictID " +
                "INNER JOIN ProgramYear ON DYI.ProgramYear = ProgramYear.ProgramYear " +
                "LEFT OUTER JOIN[Inventory] IOPEN ON DYI.ProgramYear = IOPEN.ProgramYear AND DYI.DistrictID = IOPEN.DistrictID AND @YearMonthStart = IOPEN.YearMonth " +
                "LEFT OUTER JOIN[Inventory] ICLOSE ON DYI.ProgramYear = ICLOSE.ProgramYear AND DYI.DistrictID = ICLOSE.DistrictID AND @YearMonthEnd = ICLOSE.YearMonth " +
                "CROSS APPLY(SELECT CASE @YearMonthEnd WHEN '*OPEN*' THEN ProgramYear.StartDate WHEN 'CLOSE' THEN ProgramYear.EndDate ELSE  DATEFROMPARTS(SUBSTRING(@YearMonthEnd,1,4),SUBSTRING(@YearMonthEnd, 6, 2),1) END CloseDate) CALC " +
                " CROSS APPLY(SELECT DATEADD(DAY, -1, DATEADD(MONTH, 1, CloseDate)) GLDate) CALC2 " +
                " CROSS APPLY(SELECT CONVERT(VARCHAR(3), GLDate, 100) MonthStr, CONVERT(VARCHAR, GLDate, 1) GLDateStr) CALC3 " +
                " WHERE DYI.ProgramYear = @ProgramYear AND AcctType IN(1, 3, 5) AND D.Inactive = 0" +
                " AND NOT ( (ISNULL(IOPEN.Groceries, 0)-ISNULL(ICLOSE.Groceries, 0))=0 AND " +
                "      (ISNULL(IOPEN.Milk, 0)-ISNULL(ICLOSE.Milk, 0))=0 AND" +
                "      (ISNULL(IOPEN.Paper, 0)-ISNULL(ICLOSE.Paper, 0))=0 AND" +
                "      (ISNULL(IOPEN.Cleaning, 0)-ISNULL(ICLOSE.Cleaning, 0))=0) ";

            try
            {
                using (SqlConnection dbConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand dbCommand = new SqlCommand(queryString, dbConnection))
                    {

                        SqlParameter dbParam_programYear = new SqlParameter();
                        dbParam_programYear.ParameterName = "@ProgramYear";
                        dbParam_programYear.Value = DDLProgramYears.SelectedValue;//Session["ProgramYear"].ToString();
                        dbParam_programYear.DbType = DbType.String;
                        dbCommand.Parameters.Add(dbParam_programYear);

                        SqlParameter dbParam_startingPostDate = new SqlParameter();
                        dbParam_startingPostDate.ParameterName = "@YearMonthStart";
                        dbParam_startingPostDate.Value = DDLStartInventory.SelectedValue;
                        dbParam_startingPostDate.DbType = DbType.String;
                        dbCommand.Parameters.Add(dbParam_startingPostDate);

                        SqlParameter dbParam_endingPostDate = new SqlParameter();
                        dbParam_endingPostDate.ParameterName = "@YearMonthEnd";
                        dbParam_endingPostDate.Value = DDLEndInventory.SelectedValue;
                        dbParam_endingPostDate.DbType = DbType.String;
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
                                            //if (colIndex > 2)
                                            csv += "\"" + row[column.ColumnName].ToString().Replace(",", ";") + "\",";
                                            //else
                                            //    csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';//shouldn't need this .Replace("AM","")
                                        }

                                        //remove the trailing comma
                                        csv = csv.TrimEnd(',');

                                        //Add new line.
                                        csv += "\r\n";
                                    }

                                    string ErrorInfo = "";
                                    string fileLabelName = "";
                                    if (DDLEndInventory.SelectedValue == "*OPEN*")
                                        fileLabelName = DDLProgramYears.SelectedValue + "OPEN";
                                    else
                                        fileLabelName = DDLEndInventory.SelectedValue;

                                    if (hasWriteAccessToFolder(ExportPath, out ErrorInfo))
                                    {
                                        try
                                        {


                                            string InventoryFileName = @ExportPath + "INV" + fileLabelName + ".txt";
                                            FileInfo fi = new FileInfo(InventoryFileName);

                                            FileStream fs = new FileStream(InventoryFileName, FileMode.Create);
                                            StreamWriter writer = new StreamWriter(fs);
                                            writer.Write(csv);
                                            writer.Close();


                                            TextBoxStatus.Text += Environment.NewLine + "Saved " + "INV" + fileLabelName + ".txt to " + @ExportPath;
                                            TextBoxStatus.Rows++;
                                        }
                                        catch (IOException ex)
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
                                        Response.AddHeader("content-disposition", @"attachment;filename=" + "INV" + fileLabelName + ".txt");
                                        Response.Charset = "";
                                        Response.ContentType = "application/text";
                                        Response.Output.Write(csv);
                                        Response.Flush(); // Sends all currently buffered output to the client.
                                                          //Response.End(); // Apparently this always throws an Exception. Replaced with the following 2 lines of code.
                                        Response.SuppressContent = true; // Gets or sets a value indicating whether to send HTTP content to the client.
                                        HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

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

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
        }

        protected void DDLStartInventory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int comparison = String.Compare(DDLStartInventory.SelectedValue, DDLEndInventory.SelectedValue, comparisonType: StringComparison.OrdinalIgnoreCase);
            if (comparison > 0)
                DDLEndInventory.SelectedValue = DDLStartInventory.SelectedValue;
        }

        protected void DDLEndInventory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int comparison = String.Compare(DDLStartInventory.SelectedValue, DDLEndInventory.SelectedValue, comparisonType: StringComparison.OrdinalIgnoreCase);
            if (comparison > 0)
                DDLStartInventory.SelectedValue = DDLEndInventory.SelectedValue;
        }

    }
}
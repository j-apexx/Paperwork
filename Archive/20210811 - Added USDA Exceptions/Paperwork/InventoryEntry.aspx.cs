using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class InventoryEntry : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");



            if (!IsPostBack)
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

                //Update HiddenFields which are used to constrain the DatePickers
                DDLProgramYears_SelectedIndexChanged(null, null);

                Session["AllowUSDAEdits"] = IsDistrictAllowedUSDAEdits(Session["CurrentDistrict"].ToString());
            }
        }

        protected void TextboxYearMonth_TextChanged(object sender, EventArgs e)
        {
            int Year = 0;
            int Month = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            TextBox TextBoxFYearmonth = (TextBox)sender;

            try
            {
                startDate = DateTime.Parse(HFStartDate.Value);
                endDate = DateTime.Parse(HFEndDate.Value);
            }
            catch(Exception ex)//session values not set?
            {
                Response.Redirect("~/default.aspx");
            }

            //If we cannot parse the year and month - set to current year month
            if (TextBoxFYearmonth.Text.Length<7 || int.TryParse(TextBoxFYearmonth.Text.Substring(0, 4), out Year) == false || int.TryParse(TextBoxFYearmonth.Text.Substring(5, 2), out Month) == false)
            {
                TextBoxFYearmonth.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
                Year = DateTime.Now.Year;
                Month = DateTime.Now.Month;
            }

            DateTime testDate = new DateTime(Year, Month, 1);
            //if date falls before the beginning or after the end of the program year - set to limits
            if(testDate< startDate)
            {
                TextBoxFYearmonth.Text = startDate.Year.ToString() + "-" + startDate.Month.ToString().PadLeft(2, '0');
                Year = startDate.Year;
                Month = startDate.Month;
            }
            else if(testDate > endDate)
            {
                TextBoxFYearmonth.Text = endDate.Year.ToString() + "-" + endDate.Month.ToString().PadLeft(2, '0');
                Year = endDate.Year;
                Month = endDate.Month;
            }

            //Check for duplication and handle
            Boolean YMFound = false;
            //Check to see if there is already the same YM period
            foreach (GridViewRow gvr in GridViewInventory.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    Label LabelEYearMonth = (Label)FindControlRecursive(gvr, "LabelEYearMonth");
                    Label LabelYearMonth = (Label)FindControlRecursive(gvr, "LabelYearMonth");
                    if (LabelEYearMonth != null && LabelEYearMonth.Text == TextBoxFYearmonth.Text)
                    {
                        YMFound = true;
                        break;
                    }
                    if (LabelYearMonth != null && LabelYearMonth.Text == TextBoxFYearmonth.Text)
                    {
                        YMFound = true;
                        break;
                    }
                }
            }

            if (YMFound)
            {
                RegisterDOMReadyScript("Alert", "alert('An inventory has already been entered for " + TextBoxFYearmonth.Text + ".');");
                TextBoxFYearmonth.Text = "";
                TextBoxFYearmonth.Enabled = true;
            }



        }

        protected void CBOpen_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox CBOpen = (CheckBox)sender;
            TextBox TextBoxFYearmonth = (TextBox)FindControlRecursive(GridViewInventory, "TextBoxFYearmonth");
            //RegularExpressionValidator RegexTrxMonth = (RegularExpressionValidator)FindControlRecursive(GridViewInventory, "RegexTrxMonth");

            if (CBOpen.Checked)
            {
                Boolean OpenFound = false;
                //Check to see if there is already an open period
                foreach(GridViewRow gvr in GridViewInventory.Rows)
                {
                    if(gvr.RowType==DataControlRowType.DataRow)
                    {
                        Label LabelEYearMonth = (Label)FindControlRecursive(gvr, "LabelEYearMonth");
                        Label LabelYearMonth = (Label)FindControlRecursive(gvr, "LabelYearMonth");
                        if (LabelEYearMonth!=null && LabelEYearMonth.Text=="*OPEN*")
                        {
                            OpenFound = true;
                            break;
                        }
                        if (LabelYearMonth != null && LabelYearMonth.Text == "*OPEN*")
                        {
                            OpenFound = true;
                            break;
                        }
                    }
                }

                if(OpenFound)
                {
                    CBOpen.Checked = false;
                    TextBoxFYearmonth.Text = "";
                    TextBoxFYearmonth.Enabled = true;
                    RegisterDOMReadyScript("Alert", "alert('An OPEN inventory has already been entered.');");
                }
                else
                {
                    //Set the YearMonth TextBox to *OPEN*
                    TextBoxFYearmonth.Text = "*OPEN*";
                    TextBoxFYearmonth.Enabled = false;
                }


            }
            else
            {
                //Clear the YearMonth Textbox and allow normal datepicker functionality
                TextBoxFYearmonth.Text = "";
                TextBoxFYearmonth.Enabled = true;
            }
        }

        protected void GridViewInventory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceInventory.Insert();
                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate key") >= 0)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "Duplicate record.", "");
                    }
                    else
                    {
                        ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }
            }
        }

        protected void GridViewInventory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["YearMonth"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceInventory_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null)
                return;

            TextBox TextBoxFYearmonth = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFYearmonth");

            DateTime checkDate;
            if (TextBoxFYearmonth.Text == "*OPEN*")
                DateTime.TryParse(HFStartDate.Value, out checkDate);
            else
            {
                try
                {
                    checkDate = new DateTime(int.Parse(TextBoxFYearmonth.Text.Substring(0, 4)), int.Parse(TextBoxFYearmonth.Text.Substring(5, 2)), 1);
                }
                catch
                {
                    ShowDialog("Error", MessageType.Error, "Error with Year-Month format.", "");
                    return;
                }
            }

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(checkDate, DDLProgramYears.SelectedValue) == false)
            {
                ShowDialog("Warning", MessageType.Warning, "Period has been closed and record cannot be inserted.", "");
                e.Cancel = true;
                return;
            }


            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // Program Year and District are already set from Session or page dropdown
            
            TextBox TextBoxFFood = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFFood");
            TextBox TextBoxFPaper = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFPaper");
            TextBox TextBoxFCleaning = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFCleaning");
            TextBox TextBoxFMilk = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFMilk");
            TextBox TextBoxFUSDAProtOpen = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAProtOpen");
            TextBox TextBoxFUSDAProtRcvd = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAProtRcvd");
            TextBox LabelFUSDAProtIUse = (TextBox)GridViewInventory.FooterRow.FindControl("LabelFUSDAProtIUse");
            TextBox TextBoxFUSDAProtTrns = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAProtTrns");
            TextBox TextBoxFUSDAProtEnd = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAProtEnd");
            TextBox TextBoxFUSDAGrocOpen = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocOpen");
            TextBox TextBoxFUSDAGrocRcvd = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocRcvd");
            TextBox TextBoxFUSDAGrocIUse = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocIUse");
            TextBox TextBoxFUSDAGrocTrns = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocTrns");
            TextBox TextBoxFUSDAGrocEnd = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocEnd");
            TextBox TextBoxFComments = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFComments");
            TextBox TextBoxFUSDAComments = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAComments");

            e.Command.Parameters["@YearMonth"].Value = TextBoxFYearmonth.Text;

            if (Decimal.TryParse(TextBoxFFood.Text, out Decimal outFood))
                e.Command.Parameters["@Groceries"].Value = outFood;
            else
            {
                e.Command.Parameters["@Groceries"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFPaper.Text, out Decimal outPaper))
                e.Command.Parameters["@Paper"].Value = outPaper;
            else
            {
                e.Command.Parameters["@Paper"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFCleaning.Text, out Decimal outClean))
                e.Command.Parameters["@Cleaning"].Value = outClean;
            else
            {
                e.Command.Parameters["@Cleaning"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFMilk.Text, out Decimal outMilk))
                e.Command.Parameters["@Milk"].Value = outMilk;
            else
            {
                e.Command.Parameters["@Milk"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAProtOpen.Text, out Decimal outProtOpen))
                e.Command.Parameters["@USDAProteinOpen"].Value = outProtOpen;
            else
            {
                e.Command.Parameters["@USDAProteinOpen"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAProtRcvd.Text, out Decimal outProtRcvd))
                e.Command.Parameters["@USDAProteinRcvd"].Value = outProtRcvd;
            else
            {
                e.Command.Parameters["@USDAProteinRcvd"].Value = 0M;
            }

            if (Decimal.TryParse(LabelFUSDAProtIUse.Text, out Decimal outProtIUse))
                e.Command.Parameters["@USDAProteinInstUse"].Value = outProtIUse;
            else
            {
                e.Command.Parameters["@USDAProteinInstUse"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAProtTrns.Text, out Decimal outProtTrans))
                e.Command.Parameters["@USDAProteinTrans"].Value = outProtTrans;
            else
            {
                e.Command.Parameters["@USDAProteinTrans"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAProtEnd.Text, out Decimal outProtEnd))
                e.Command.Parameters["@USDAProteinEnding"].Value = outProtEnd;
            else
            {
                e.Command.Parameters["@USDAProteinEnding"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocOpen.Text, out Decimal outGrocOpen))
                e.Command.Parameters["@USDAGroceryOpen"].Value = outGrocOpen;
            else
            {
                e.Command.Parameters["@USDAGroceryOpen"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocRcvd.Text, out Decimal outGrocRcvd))
                e.Command.Parameters["@USDAGroceryRcvd"].Value = outGrocRcvd;
            else
            {
                e.Command.Parameters["@USDAGroceryRcvd"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocIUse.Text, out Decimal outGrocIUse))
                e.Command.Parameters["@USDAGroceryInstUse"].Value = outGrocIUse;
            else
            {
                e.Command.Parameters["@USDAGroceryInstUse"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocTrns.Text, out Decimal outGrocTrans))
                e.Command.Parameters["@USDAGroceryTrans"].Value = outGrocTrans;
            else
            {
                e.Command.Parameters["@USDAGroceryTrans"].Value = 0M;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocEnd.Text, out Decimal outGrocEnd))
                e.Command.Parameters["@USDAGroceryEnding"].Value = outGrocEnd;
            else
            {
                e.Command.Parameters["@USDAGroceryEnding"].Value = 0M;
            }

            e.Command.Parameters["@Comments"].Value = TextBoxFComments.Text;
            e.Command.Parameters["@USDAComments"].Value = TextBoxFUSDAComments.Text;



        }

        protected void SqlDataSourceInventory_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);
            }
            else if (e.Exception != null)
            {
                if (e.Exception.Message.IndexOf("Duplicate") >= 0)
                {
                    ShowMessageIcon("Error-Duplicate", MessageType.Error);
                    e.ExceptionHandled = true;
                }
                else
                {
                    ShowMessageIcon("SQL Error:" + e.Exception.Message, MessageType.Error);
                    e.ExceptionHandled = true;
                }
            }
        }

        protected void SqlDataSourceInventory_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Deleted", MessageType.Success);
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        protected void SqlDataSourceInventory_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Updated", MessageType.Success);
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

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
                    HFStartDate.Value = drvSql["StartDate"].ToString();
                    HFEndDate.Value = drvSql["EndDate"].ToString();


                }
                else
                {
                    HFStartDate.Value = "";
                    HFEndDate.Value = "";
                }
            }
            else
            {
                //if no program years could be populated - redirect home
                Response.Redirect("~/default.aspx");
            }
        }

        protected void SqlDataSourceInventory_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            
            string Yearmonth = e.Command.Parameters["@YearMonth"].Value.ToString();

            DateTime checkDate;
            if (Yearmonth == "*OPEN*")
                DateTime.TryParse(HFStartDate.Value, out checkDate);
            else
            {
                try
                {
                    checkDate = new DateTime(int.Parse(Yearmonth.Substring(0, 4)), int.Parse(Yearmonth.Substring(5, 2)), 1);
                }
                catch
                {
                    e.Cancel=true;
                    return;
                }
            }

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(checkDate, DDLProgramYears.SelectedValue) == false)
            {
                ShowDialog("Warning", MessageType.Warning, "Period has been closed and record cannot be updated.", "");
                e.Cancel = true;
                return;
            }

        }

        protected void SqlDataSourceInventory_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            string Yearmonth = e.Command.Parameters["@YearMonth"].Value.ToString();

            DateTime checkDate;
            if (Yearmonth == "*OPEN*")
                DateTime.TryParse(HFStartDate.Value, out checkDate);
            else
            {
                try
                {
                    checkDate = new DateTime(int.Parse(Yearmonth.Substring(0, 4)), int.Parse(Yearmonth.Substring(5, 2)), 1);
                }
                catch
                {
                    return;
                }
            }

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(checkDate, DDLProgramYears.SelectedValue) == false)
            {
                ShowDialog("Warning", MessageType.Warning, "Period has been closed and record cannot be deleted.", "");
                e.Cancel = true;
                return;
            }
        }

        protected bool IsDistrictAllowedUSDAEdits(string distID)
        {
            bool editsAllowed = false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT AllowInvUSDAEdits FROM [District] WHERE DistrictID=@DistrictID";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = distID;
                dbParam_distID.DbType = DbType.StringFixedLength;
                dbCommand.Parameters.Add(dbParam_distID);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object obj_allowEdits = dbCommand.ExecuteScalar();
                    if (obj_allowEdits == null)
                        editsAllowed = false;
                    else if ((bool)obj_allowEdits == true)
                        editsAllowed = true;
                    else
                        editsAllowed = false;
                }
                catch (Exception ex)
                {
                    editsAllowed = false;
                }
                finally
                {
                    dbConnection.Close();
                }
            }


            return editsAllowed;
        }
    }
}
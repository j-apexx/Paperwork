using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupPurchOtherCats : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            switch (Session["AccessLevel"].ToString().ToUpper())
            {
                case "ADMIN":
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


                DDLProgramYears.DataBind();
                try
                {
                    if (Session["ProgramYear"] != null)
                    {
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                        DDLProgramYears_SelectedIndexChanged(null, null);
                    }
                }
                catch (Exception ex) { }
            }
        }

        protected void SqlDataSourcePurchaseCats_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check to ensure the Category has not been used
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.SqlClient.SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT Count(*) FROM [Purchases] WHERE ProgramYear=@ProgramYear AND OtherCategoryID=@OtherCategoryID ";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = e.Command.Parameters["@ProgramYear"].Value;
                dbParam_ProgYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_PayCode = new SqlParameter();
                dbParam_PayCode.ParameterName = "@OtherCategoryID";
                dbParam_PayCode.Value = e.Command.Parameters["@OtherCategoryID"].Value;
                dbParam_PayCode.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_PayCode);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object categoryUseCount = cmd.ExecuteScalar();
                    if (categoryUseCount != null && (int)categoryUseCount > 0)
                    {
                        e.Cancel = true;
                        RegisterDOMReadyScript("Warning", "alert('Cannot delete this category because it has been used on a purchase entry.');");
                    }
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void GridViewOtherCats_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourcePurchaseCats.Insert();
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

        protected void GridViewOtherCats_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["OtherCategoryID"].ToString() == "")
                    e.Row.Visible = false;
            }
            catch (Exception ex)
            { }
        }

        protected void SqlDataSourcePurchaseCats_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set

            TextBox TextBoxFCatID = (TextBox)GridViewOtherCats.FooterRow.FindControl("TextBoxFCatID");
            TextBox TextBoxFCatDesc = (TextBox)GridViewOtherCats.FooterRow.FindControl("TextBoxFCatDesc");
            TextBox TextBoxFExpCode = (TextBox)GridViewOtherCats.FooterRow.FindControl("TextBoxFExpCode");

            if (DDLProgramYears.Items.Count == 0 || DDLProgramYears.SelectedIndex == -1)
                return;

            if (TextBoxFCatID == null || TextBoxFCatID.Text == string.Empty)
                return;

            if (TextBoxFCatDesc == null || TextBoxFCatDesc.Text == string.Empty)
                return;

            if (TextBoxFExpCode == null)
                return;

            e.Command.Parameters["@OtherCategoryID"].Value = TextBoxFCatID.Text;
            e.Command.Parameters["@OtherCategoryDesc"].Value = TextBoxFCatDesc.Text;
            e.Command.Parameters["@OtherCatExpCode"].Value = TextBoxFExpCode.Text;

        }

        protected void ISync_Click(object sender, EventArgs e)
        {
            if (HFBudgetYear.Value == String.Empty)
            {
                ShowMessageIcon("No Budget Year configured for this Program Year.", MessageType.Warning);
                return;
            }

            int recordsAdded;
            int recordsRemoved;
            int recordsAddedNonBudgeted;
            string messageString = "";

            recordsAdded = ImportCategoryRecords(DDLProgramYears.SelectedValue, HFBudgetYear.Value.TrimEnd(' '));
            recordsRemoved = RemoveCategoryRecords(DDLProgramYears.SelectedValue, HFBudgetYear.Value.TrimEnd(' '));
            recordsAddedNonBudgeted = InitializeNonBudgetedDistCategoryRecords(DDLProgramYears.SelectedValue);

            if (recordsAdded > 0)
                messageString = "Added " + recordsAdded.ToString() + " Category permission records.";
            else
                messageString = "0 Category permission records added.";

            if (recordsRemoved > 0)
                messageString += " Removed " + recordsRemoved.ToString() + " Category permission records.";

            if (recordsAddedNonBudgeted > 0)
                messageString += "Added " + recordsAddedNonBudgeted.ToString() + " Category permission records for non-budgeted districts.";

            ShowMessageIcon(messageString, MessageType.Info);
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            //Populate the BudgetYear hidden field
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.SqlClient.SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT BudgetYear FROM [ProgramYear] WHERE ProgramYear=@ProgramYear";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                SqlParameter param_ProgramYear = new SqlParameter();
                param_ProgramYear.ParameterName = "@ProgramYear";
                param_ProgramYear.Value = DDLProgramYears.SelectedValue;
                param_ProgramYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(param_ProgramYear);

                dbConnection.Open();
                try
                {
                    object budgetObj = cmd.ExecuteScalar();
                    if (budgetObj != null)
                        HFBudgetYear.Value = budgetObj.ToString();
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
    }
}
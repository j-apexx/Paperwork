using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupMenuAnalysisLevels : Paperwork_Global
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
                    }
                }
                catch (Exception ex) { }

            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            SqlDataSourceProgramYears.DataBind();

        }

        protected void TextBoxRange_TextChanged(object sender, EventArgs e)
        {
            TextBox TextBoxRangeField = (TextBox)sender;
            string updateField = "";
            decimal decRangeValue = 0M;
            switch (TextBoxRangeField.ID)
            {
                case "TextBoxMidRangeLower":
                    updateField = "MidrangeLower";
                    decimal.TryParse(TextBoxRangeField.Text.Replace("$", ""), out decRangeValue);
                    break;
                case "TextBoxMidRangeUpper":
                    updateField = "MidrangeUpper";
                    decimal.TryParse(TextBoxRangeField.Text.Replace("$", ""), out decRangeValue);
                    break;
                default:
                    return;
                    break;
            }

            GridViewRow gvr = (GridViewRow)FindParentControlOfType(TextBoxRangeField, "GridViewRow");
            if (gvr == null)
                return;
            int itemCatID = (int)gvMenuAnalysisLevels.DataKeys[gvr.DataItemIndex].Values["ItemCategoryID"];
            Label LabelLowRange = (Label)FindControlRecursive(gvr, "LabelLowRange");


            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString;
            if (doesCostRangeExist(DDLProgramYears.SelectedValue, itemCatID))
                queryString = "UPDATE [MenuCostRangeSetup] SET [" + updateField + "]=@RangeValue WHERE [ProgramYear]=@ProgramYear AND [ItemCategoryID]=@ItemCategoryID";
            else
                queryString = "INSERT INTO [MenuCostRangeSetup] ([ProgramYear],[ItemCategoryID],[MidrangeLower],[MidrangeUpper]) SELECT @ProgramYear,@ItemCategoryID,@RangeValue,@RangeValue";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                dbParam_progYear.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_itemCatID = new SqlParameter();
                dbParam_itemCatID.ParameterName = "@ItemCategoryID";
                dbParam_itemCatID.Value = itemCatID;
                dbParam_itemCatID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemCatID);

                //SqlParameter dbParam_lower = new SqlParameter();
                //dbParam_lower.ParameterName = "@MidrangeLower";
                //dbParam_lower.Value = decMidrangeLower;
                //dbParam_lower.DbType = DbType.Decimal;
                //dbCommand.Parameters.Add(dbParam_lower);

                //SqlParameter dbParam_upper = new SqlParameter();
                //dbParam_upper.ParameterName = "@MidrangeUpper";
                //dbParam_upper.Value = decMidrangeUpper;
                //dbParam_upper.DbType = DbType.Decimal;
                //dbCommand.Parameters.Add(dbParam_upper);

                SqlParameter dbParam_singleRange = new SqlParameter();
                dbParam_singleRange.ParameterName = "@RangeValue";
                dbParam_singleRange.Value = decRangeValue;
                dbParam_singleRange.DbType = DbType.Decimal;
                dbCommand.Parameters.Add(dbParam_singleRange);

                try
                {
                    dbConnection.Open();

                    dbCommand.ExecuteNonQuery();

                    gvMenuAnalysisLevels.DataBind();

                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SQL error occurred while saving range values.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred while saving the range values.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }



        }

        private bool doesCostRangeExist(object progYear, object itemCatID)
        {
            bool recordExists = false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT COUNT(*) FROM [MenuCostRangeSetup] WHERE [ProgramYear]=@ProgramYear AND [ItemCategoryID]=@ItemCategoryID";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_progYear = new SqlParameter();
                dbParam_progYear.ParameterName = "@ProgramYear";
                dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                dbParam_progYear.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_progYear);

                SqlParameter dbParam_itemCatID = new SqlParameter();
                dbParam_itemCatID.ParameterName = "@ItemCategoryID";
                dbParam_itemCatID.Value = itemCatID;
                dbParam_itemCatID.DbType = DbType.Int32;
                dbCommand.Parameters.Add(dbParam_itemCatID);

                try
                {
                    dbConnection.Open();

                    object objRetQty = dbCommand.ExecuteScalar();

                    if (objRetQty == null)
                        recordExists = false;
                    else if (int.Parse(objRetQty.ToString()) > 0)
                        recordExists = true;

                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SQL error occurred while checking for menu cost range setup record.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred while checking for menu cost range setup record.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return recordExists;
        }
    }
}
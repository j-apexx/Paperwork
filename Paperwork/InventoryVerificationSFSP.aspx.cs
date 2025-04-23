using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class InventoryVerificationSFSP : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");



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

                //Update HiddenFields which are used to constrain the DatePickers
                DDLProgramYears_SelectedIndexChanged(null, null);

                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;

                DropDownListDistricts.DataBind();
                if (DropDownListDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                    else
                        DropDownListDistricts_SelectedIndexChanged(null, null);
                }

            }
        }



        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void TextBox_TextChanged(object sender, EventArgs e)
        {
            SaveInventoryVerification();
        }

        protected void SaveInventoryVerification()
        {
            HiddenField HFVerificationSaved = (HiddenField)FindControlRecursive(FormViewInvVerification, "HFVerificationSaved");
            if (HFVerificationSaved == null || HFVerificationSaved.Value == "1")
                return;

            //KEY VALUES
            String KeyPYear = FormViewInvVerification.DataKey.Values["ProgramYear"].ToString();
            String KeyDistID = FormViewInvVerification.DataKey.Values["DistrictID"].ToString();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryVerificationExists = "SELECT COUNT(*) FROM [SFSPInventoryVerification] " +
                    " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID ";
            using (SqlCommand cmd = new SqlCommand(queryVerificationExists, dbConnection))
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = KeyPYear;
                dbParam_ProgYear.DbType = DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
                dbParam_DistID.DbType = DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);
                #endregion

                int existingRecordCount = 0;

                try
                {
                    dbConnection.Open();
                    Object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out existingRecordCount);
                    }

                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "A database error occurred while checking to see if record existed.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Unknown Error", MessageType.Error, "An error occurred while checking to see if record existed.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

                string InsertUpdateQuery = "";

                if (existingRecordCount == 0)
                {
                    InsertUpdateQuery = "INSERT INTO [SFSPInventoryVerification] ([ProgramYear],[DistrictID]" +
                        ",[GroceriesSYEnding],[PaperSYEnding],[CleaningSYEnding],[MilkSYEnding]" +
                        ",[GroceriesSFSPEnding],[PaperSFSPEnding],[CleaningSFSPEnding],[MilkSFSPEnding])" +
                        " VALUES (@ProgramYear,@DistrictID,@GroceriesSYEnding,@PaperSYEnding,@CleaningSYEnding,@MilkSYEnding" +
                        ",@GroceriesSFSPEnding,@PaperSFSPEnding,@CleaningSFSPEnding,@MilkSFSPEnding)";
                }
                else
                {
                    InsertUpdateQuery = "UPDATE [SFSPInventoryVerification] SET [GroceriesSYEnding]=@GroceriesSYEnding" +
                        ",[PaperSYEnding]=@PaperSYEnding,[CleaningSYEnding]=@CleaningSYEnding,[MilkSYEnding]=@MilkSYEnding" +
                        ",[GroceriesSFSPEnding]=@GroceriesSFSPEnding,[PaperSFSPEnding]=@PaperSFSPEnding" +
                        ",[CleaningSFSPEnding]=@CleaningSFSPEnding,[MilkSFSPEnding]=@MilkSFSPEnding " +
                        " WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID";
                }

                #region FieldValues
                TextBox TextBoxFoodSYEnding = (TextBox)FindControlRecursive(FormViewInvVerification, "TextBoxFoodSYEnding");
                TextBox TextBoxMilkSYEnding = (TextBox)FindControlRecursive(FormViewInvVerification, "TextBoxMilkSYEnding");
                TextBox TextBoxPaperSYEnding = (TextBox)FindControlRecursive(FormViewInvVerification, "TextBoxPaperSYEnding");
                TextBox TextBoxCleaningSYEnding = (TextBox)FindControlRecursive(FormViewInvVerification, "TextBoxCleaningSYEnding");

                TextBox TextBoxFoodSFSPEnding = (TextBox)FindControlRecursive(FormViewInvVerification, "TextBoxFoodSFSPEnding");
                TextBox TextBoxMilkSFSPEnding = (TextBox)FindControlRecursive(FormViewInvVerification, "TextBoxMilkSFSPEnding");
                TextBox TextBoxPaperSFSPEnding = (TextBox)FindControlRecursive(FormViewInvVerification, "TextBoxPaperSFSPEnding");
                TextBox TextBoxCleaningSFSPEnding = (TextBox)FindControlRecursive(FormViewInvVerification, "TextBoxCleaningSFSPEnding");
                #endregion

                SqlConnection dbConnection2 = new SqlConnection(connectionString);
                using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection2))
                {
                    #region KeyVals
                    SqlParameter dbParam_ProgYear2 = new SqlParameter();
                    dbParam_ProgYear2.ParameterName = "@ProgramYear";
                    dbParam_ProgYear2.Value = KeyPYear;
                    dbParam_ProgYear2.DbType = DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_ProgYear2);

                    SqlParameter dbParam_DistID2 = new SqlParameter();
                    dbParam_DistID2.ParameterName = "@DistrictID";
                    dbParam_DistID2.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
                    dbParam_DistID2.DbType = DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_DistID2);
                    #endregion
                    #region FieldVals SY
                    SqlParameter dbParam_FoodSY = new SqlParameter();
                    dbParam_FoodSY.ParameterName = "@GroceriesSYEnding";
                    Decimal.TryParse(TextBoxFoodSYEnding.Text, out decimal decFoodSy);
                    dbParam_FoodSY.Value = decFoodSy;
                    dbParam_FoodSY.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_FoodSY);

                    SqlParameter dbParam_PaperSY = new SqlParameter();
                    dbParam_PaperSY.ParameterName = "@PaperSYEnding";
                    Decimal.TryParse(TextBoxPaperSYEnding.Text, out decimal decPaperSy);
                    dbParam_PaperSY.Value = decPaperSy;
                    dbParam_PaperSY.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_PaperSY);

                    SqlParameter dbParam_CleaningSY = new SqlParameter();
                    dbParam_CleaningSY.ParameterName = "@CleaningSYEnding";
                    Decimal.TryParse(TextBoxCleaningSYEnding.Text, out decimal decCleaningSy);
                    dbParam_CleaningSY.Value = decCleaningSy;
                    dbParam_CleaningSY.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CleaningSY);

                    SqlParameter dbParam_MilkSY = new SqlParameter();
                    dbParam_MilkSY.ParameterName = "@MilkSYEnding";
                    Decimal.TryParse(TextBoxMilkSYEnding.Text, out decimal decMilkSy);
                    dbParam_MilkSY.Value = decMilkSy;
                    dbParam_MilkSY.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_MilkSY);
                    #endregion
                    #region FieldVals SFSP
                    SqlParameter dbParam_FoodSFSP = new SqlParameter();
                    dbParam_FoodSFSP.ParameterName = "@GroceriesSFSPEnding";
                    Decimal.TryParse(TextBoxFoodSFSPEnding.Text, out decimal decFoodSFSP);
                    dbParam_FoodSFSP.Value = decFoodSFSP;
                    dbParam_FoodSFSP.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_FoodSFSP);

                    SqlParameter dbParam_PaperSFSP = new SqlParameter();
                    dbParam_PaperSFSP.ParameterName = "@PaperSFSPEnding";
                    Decimal.TryParse(TextBoxPaperSFSPEnding.Text, out decimal decPaperSFSP);
                    dbParam_PaperSFSP.Value = decPaperSFSP;
                    dbParam_PaperSFSP.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_PaperSFSP);

                    SqlParameter dbParam_CleaningSFSP = new SqlParameter();
                    dbParam_CleaningSFSP.ParameterName = "@CleaningSFSPEnding";
                    Decimal.TryParse(TextBoxCleaningSFSPEnding.Text, out decimal decCleaningSFSP);
                    dbParam_CleaningSFSP.Value = decCleaningSFSP;
                    dbParam_CleaningSFSP.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_CleaningSFSP);

                    SqlParameter dbParam_MilkSFSP = new SqlParameter();
                    dbParam_MilkSFSP.ParameterName = "@MilkSFSPEnding";
                    Decimal.TryParse(TextBoxMilkSFSPEnding.Text, out decimal decMilkSFSP);
                    dbParam_MilkSFSP.Value = decMilkSFSP;
                    dbParam_MilkSFSP.DbType = DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_MilkSFSP);
                    #endregion

                    try
                    {
                        dbConnection2.Open();
                        int recordcount = 0;
                        recordcount = cmd2.ExecuteNonQuery();

                        if (recordcount > 0)
                        {
                            ShowMessageIcon("Saved", MessageType.Success);

                        }
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "An error occurred during verification save.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "An error occurred during varification save.", ex.Message);
                    }
                    finally
                    {
                        dbConnection2.Close();
                    }

                }
                HFVerificationSaved.Value = "1";
            }
        }

        protected void FormViewInvVerification_PreRender(object sender, EventArgs e)
        {
            //Reset the Save tracker
            HiddenField HFVerificationSaved = (HiddenField)FindControlRecursive(FormViewInvVerification, "HFVerificationSaved");

            if (HFVerificationSaved != null)
                HFVerificationSaved.Value = "0";
        }

        protected void FormViewInvVerification_DataBound(object sender, EventArgs e)
        {
            RegisterDOMReadyScript("Recalc", "UpdateTotals();");
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Print/PrintInventoryVerification.aspx");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupSchoolInfo : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");
            else
                LabelProgramYear.Text = Session["ProgramYear"].ToString();

            if (!IsPostBack)
            {
                VerifyAccess(true);
            }
            else
            {
                VerifyAccess(false);
            }

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if(!IsPostBack)
            {
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

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                Response.Redirect("~/Setup/SetupSchoolInfo.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        /// <summary>
        /// Prerender runs after the control postback scripts are run. This means all saves have occurred at this point,
        /// Reset the hidden field indicators to allow future changes to be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FormViewSchoolInfo_PreRender(object sender, EventArgs e)
        {
            HiddenField HFDistSchoolInfoSaved = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistSchoolInfoSaved");

            if (HFDistSchoolInfoSaved != null)
                HFDistSchoolInfoSaved.Value = "0";
        }

        protected void SaveDistSchoolInfo()
        {
            HiddenField HFDistSchoolInfoSaved = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistSchoolInfoSaved");
            if (HFDistSchoolInfoSaved.Value == "1")
                return;

            if (DropDownListSchools.Items.Count == 0)
                return;

            if (DropDownListDistricts.Items.Count == 0)
                return;

            #region ReferenceControls
            HiddenField HFSchoolNum = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFSchoolNum");
            TextBox TextBoxAttFactor = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxAttFactor");
            TextBox TextBoxEnrollment = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxEnrollment");
            TextBox TextBoxPaidElig = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxPaidElig");
            TextBox TextBoxReducedElig = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxReducedElig");
            TextBox TextBoxFreeElig = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxFreeElig");

            CheckBox CB_Breakfast = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Breakfast");
            CheckBox CB_Tiers = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Tiers");
            CheckBox CB_OtherSales = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_OtherSales");
            CheckBox CB_Learners = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Learners");
            CheckBox CB_Snacks = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Snacks");

            TextBox TextBoxBfastElPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastElPrice");
            TextBox TextBoxBfastJrPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastJrPrice");
            TextBox TextBoxBfastSrPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastSrPrice");
            TextBox TextBoxBfastRedPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastRedPrice");
            TextBox TextBoxLunchElPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchElPrice");
            TextBox TextBoxLunchJrPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchJrPrice");
            TextBox TextBoxLunchSrPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchSrPrice");
            TextBox TextBoxLunchTier1Price = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchTier1Price");
            TextBox TextBoxLunchTier2Price = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchTier2Price");
            TextBox TextBoxLunchRedPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchRedPrice");
            TextBox TextBoxOSnackPaidPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxOSnackPaidPrice");
            TextBox TextBoxOSnackRedPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxOSnackRedPrice");

            TextBox TextBoxBfastPaidRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastPaidRate");
            TextBox TextBoxBfastRedRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastRedRate");
            TextBox TextBoxBfastFreeRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastFreeRate");
            TextBox TextBoxLunchPaidRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchPaidRate");
            TextBox TextBoxLunchRedRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchRedRate");
            TextBox TextBoxLunchFreeRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchFreeRate");
            TextBox TextBoxOSnackPaidRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxOSnackPaidRate");
            TextBox TextBoxOSnackRedRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxOSnackRedRate");
            TextBox TextBoxOSnackFreeRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxOSnackFreeRate");
            TextBox TextBoxBreakfastIncent = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBreakfastIncent");
            #endregion


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryUpdateSchoolHdr = "SELECT COUNT(*) FROM [DistrictSchoolYearSetup] " +
                                          " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND Schoolnum=@SchoolNum ";

            int existingRecordCount = 0;

            using (SqlCommand cmd = new SqlCommand(queryUpdateSchoolHdr, dbConnection))
            {

                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = Session["CurrentDistrict"].ToString();
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchlNum = new SqlParameter();
                dbParam_SchlNum.ParameterName = "@SchoolNum";
                dbParam_SchlNum.Value = HFSchoolNum.Value;
                dbParam_SchlNum.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_SchlNum);

                #endregion

                

                try
                {
                    dbConnection.Open();
                    Object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out existingRecordCount);
                    }

                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error reading the school info table.');");
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            string InsertUpdateQuery;
            if (existingRecordCount == 0)
            {
                InsertUpdateQuery = "INSERT INTO [DistrictSchoolYearSetup] ( [ProgramYear],[DistrictID],[SchoolNum],[BfastElPrice],[BfastJrPrice],[BfastSrPrice],[BfastRedPrice],[LunchElPrice],[LunchJrPrice],[LunchSrPrice] " +
                    " ,[LunchTier1Price],[LunchTier2Price],[LunchRedPrice],[BfastPaidRate],[BfastRedRate],[BfastFreeRate],[LunchPaidRate],[LunchRedRate],[LunchFreeRate] " +
                    " ,[OSnackPaidPrice],[OSnackRedPrice],[OSnackPaidRate],[OSnackRedRate],[OSnackFreeRate],[AF],[Enrollment],[EligiblePaid],[EligibleFree] " +
                    " ,[EligibleReduced],[BreakfastIncent],[Breakfasts],[Tiers],[OtherSales],[Learners],[Snacks]) " +
                    " VALUES (@ProgramYear,@DistrictID,@SchoolNum,@BfastElPrice,@BfastJrPrice,@BfastSrPrice,@BfastRedPrice,@LunchElPrice,@LunchJrPrice,@LunchSrPrice " +
                    " ,@LunchTier1Price,@LunchTier2Price,@LunchRedPrice,@BfastPaidRate,@BfastRedRate,@BfastFreeRate,@LunchPaidRate,@LunchRedRate,@LunchFreeRate " +
                    " ,@OSnackPaidPrice,@OSnackRedPrice,@OSnackPaidRate,@OSnackRedRate,@OSnackFreeRate,@AF,@Enrollment,@EligiblePaid,@EligibleFree " +
                    " ,@EligibleReduced,@BreakfastIncent,@Breakfasts,@Tiers,@OtherSales,@Learners,@Snacks) ";
            }
            else
            {
                InsertUpdateQuery = "UPDATE [DistrictSchoolYearSetup] SET AF=@AF, Enrollment=@Enrollment, EligiblePaid=@EligiblePaid, EligibleFree=@EligibleFree, "
                    + " EligibleReduced=@EligibleReduced, BreakfastIncent=@BreakfastIncent, Breakfasts=@Breakfasts, Tiers=@Tiers, OtherSales=@OtherSales, Learners=@Learners, Snacks=@Snacks, "
                    + " BfastElPrice=@BfastElPrice, BfastJrPrice=@BfastJrPrice, BfastSrPrice=@BfastSrPrice, BfastRedPrice=@BfastRedPrice, LunchElPrice=@LunchElPrice, LunchJrPrice=@LunchJrPrice,"
                    + " LunchSrPrice=@LunchSrPrice, LunchTier1Price=@LunchTier1Price, LunchTier2Price=@LunchTier2Price, LunchRedPrice=@LunchRedPrice, OSnackPaidPrice=@OSnackPaidPrice, "
                    + " OSnackRedPrice=@OSnackRedPrice, BfastPaidRate=@BfastPaidRate, BfastRedRate=@BfastRedRate, BfastFreeRate=@BfastFreeRate, LunchPaidRate=@LunchPaidRate, "
                    + " LunchRedRate=@LunchRedRate, LunchFreeRate=@LunchFreeRate, OSnackPaidRate=@OSnackPaidRate, OSnackRedRate=@OSnackRedRate, OSnackFreeRate=@OSnackFreeRate "
                    + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND Schoolnum=@SchoolNum ";
            }

            using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection))
            { 

                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = Session["ProgramYear"].ToString();
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = Session["CurrentDistrict"].ToString();
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_SchlNum2 = new SqlParameter();
                dbParam_SchlNum2.ParameterName = "@SchoolNum";
                dbParam_SchlNum2.Value = HFSchoolNum.Value;
                dbParam_SchlNum2.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_SchlNum2);

                #endregion

                //Updateable Values
                #region MainValues
                Decimal.TryParse(TextBoxAttFactor.Text.Trim('%'), out Decimal outAF);

                SqlParameter dbParam_AF = new SqlParameter();
                dbParam_AF.ParameterName = "@AF";
                dbParam_AF.Value = (outAF * 0.01M);
                dbParam_AF.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_AF);

                SqlParameter dbParam_Enrollment = new SqlParameter();
                dbParam_Enrollment.ParameterName = "@Enrollment";
                dbParam_Enrollment.Value = string.IsNullOrEmpty(TextBoxEnrollment.Text) ? "0" : TextBoxEnrollment.Text;
                dbParam_Enrollment.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_Enrollment);

                SqlParameter dbParam_EligPaid = new SqlParameter();
                dbParam_EligPaid.ParameterName = "@EligiblePaid";
                dbParam_EligPaid.Value = string.IsNullOrEmpty(TextBoxPaidElig.Text) ? "0" : TextBoxPaidElig.Text;
                dbParam_EligPaid.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_EligPaid);

                SqlParameter dbParam_EligFree = new SqlParameter();
                dbParam_EligFree.ParameterName = "@EligibleFree";
                dbParam_EligFree.Value = string.IsNullOrEmpty(TextBoxFreeElig.Text) ? "0" : TextBoxFreeElig.Text;
                dbParam_EligFree.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_EligFree);

                SqlParameter dbParam_EligRed = new SqlParameter();
                dbParam_EligRed.ParameterName = "@EligibleReduced";
                dbParam_EligRed.Value = string.IsNullOrEmpty(TextBoxReducedElig.Text) ? "0" : TextBoxReducedElig.Text;
                dbParam_EligRed.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_EligRed);
                #endregion
                #region Checkboxes
                SqlParameter dbParam_Breakfasts = new SqlParameter();
                dbParam_Breakfasts.ParameterName = "@Breakfasts";
                dbParam_Breakfasts.Value = CB_Breakfast.Checked;
                dbParam_Breakfasts.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_Breakfasts);

                SqlParameter dbParam_Tiers = new SqlParameter();
                dbParam_Tiers.ParameterName = "@Tiers";
                dbParam_Tiers.Value = CB_Tiers.Checked;
                dbParam_Tiers.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_Tiers);

                SqlParameter dbParam_OtherSales = new SqlParameter();
                dbParam_OtherSales.ParameterName = "@OtherSales";
                dbParam_OtherSales.Value = CB_OtherSales.Checked;
                dbParam_OtherSales.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_OtherSales);

                SqlParameter dbParam_Learners = new SqlParameter();
                dbParam_Learners.ParameterName = "@Learners";
                dbParam_Learners.Value = CB_Learners.Checked;
                dbParam_Learners.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_Learners);

                SqlParameter dbParam_Snacks = new SqlParameter();
                dbParam_Snacks.ParameterName = "@Snacks";
                dbParam_Snacks.Value = CB_Snacks.Checked;
                dbParam_Snacks.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_Snacks);
                #endregion
                #region Prices
                SqlParameter dbParam_BfastElPrice = new SqlParameter();
                dbParam_BfastElPrice.ParameterName = "@BfastElPrice";
                dbParam_BfastElPrice.Value = string.IsNullOrEmpty(TextBoxBfastElPrice.Text) ? "0" : TextBoxBfastElPrice.Text;
                dbParam_BfastElPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastElPrice);

                SqlParameter dbParam_BfastJrPrice = new SqlParameter();
                dbParam_BfastJrPrice.ParameterName = "@BfastJrPrice";
                dbParam_BfastJrPrice.Value = string.IsNullOrEmpty(TextBoxBfastJrPrice.Text) ? "0" : TextBoxBfastJrPrice.Text;
                dbParam_BfastJrPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastJrPrice);

                SqlParameter dbParam_BfastSrPrice = new SqlParameter();
                dbParam_BfastSrPrice.ParameterName = "@BfastSrPrice";
                dbParam_BfastSrPrice.Value = string.IsNullOrEmpty(TextBoxBfastSrPrice.Text) ? "0" : TextBoxBfastSrPrice.Text;
                dbParam_BfastSrPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastSrPrice);

                SqlParameter dbParam_BfastRedPrice = new SqlParameter();
                dbParam_BfastRedPrice.ParameterName = "@BfastRedPrice";
                dbParam_BfastRedPrice.Value = string.IsNullOrEmpty(TextBoxBfastRedPrice.Text) ? "0" : TextBoxBfastRedPrice.Text;
                dbParam_BfastRedPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastRedPrice);

                SqlParameter dbParam_LunchElPrice = new SqlParameter();
                dbParam_LunchElPrice.ParameterName = "@LunchElPrice";
                dbParam_LunchElPrice.Value = string.IsNullOrEmpty(TextBoxLunchElPrice.Text) ? "0" : TextBoxLunchElPrice.Text;
                dbParam_LunchElPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchElPrice);

                SqlParameter dbParam_LunchJrPrice = new SqlParameter();
                dbParam_LunchJrPrice.ParameterName = "@LunchJrPrice";
                dbParam_LunchJrPrice.Value = string.IsNullOrEmpty(TextBoxLunchJrPrice.Text) ? "0" : TextBoxLunchJrPrice.Text;
                dbParam_LunchJrPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchJrPrice);

                SqlParameter dbParam_LunchSrPrice = new SqlParameter();
                dbParam_LunchSrPrice.ParameterName = "@LunchSrPrice";
                dbParam_LunchSrPrice.Value = string.IsNullOrEmpty(TextBoxLunchSrPrice.Text) ? "0" : TextBoxLunchSrPrice.Text;
                dbParam_LunchSrPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchSrPrice);

                SqlParameter dbParam_LunchTier1Price = new SqlParameter();
                dbParam_LunchTier1Price.ParameterName = "@LunchTier1Price";
                dbParam_LunchTier1Price.Value = string.IsNullOrEmpty(TextBoxLunchTier1Price.Text) ? "0" : TextBoxLunchTier1Price.Text;
                dbParam_LunchTier1Price.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchTier1Price);

                SqlParameter dbParam_LunchTier2Price = new SqlParameter();
                dbParam_LunchTier2Price.ParameterName = "@LunchTier2Price";
                dbParam_LunchTier2Price.Value = string.IsNullOrEmpty(TextBoxLunchTier2Price.Text) ? "0" : TextBoxLunchTier2Price.Text;
                dbParam_LunchTier2Price.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchTier2Price);

                SqlParameter dbParam_LunchRedPrice = new SqlParameter();
                dbParam_LunchRedPrice.ParameterName = "@LunchRedPrice";
                dbParam_LunchRedPrice.Value = string.IsNullOrEmpty(TextBoxLunchRedPrice.Text) ? "0" : TextBoxLunchRedPrice.Text;
                dbParam_LunchRedPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchRedPrice);

                SqlParameter dbParam_OSnackPaidPrice = new SqlParameter();
                dbParam_OSnackPaidPrice.ParameterName = "@OSnackPaidPrice";
                dbParam_OSnackPaidPrice.Value = string.IsNullOrEmpty(TextBoxOSnackPaidPrice.Text) ? "0" : TextBoxOSnackPaidPrice.Text;
                dbParam_OSnackPaidPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_OSnackPaidPrice);

                SqlParameter dbParam_OSnackRedPrice = new SqlParameter();
                dbParam_OSnackRedPrice.ParameterName = "@OSnackRedPrice";
                dbParam_OSnackRedPrice.Value = string.IsNullOrEmpty(TextBoxOSnackRedPrice.Text) ? "0" : TextBoxOSnackRedPrice.Text;
                dbParam_OSnackRedPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_OSnackRedPrice);


                #endregion
                #region Reimbursements
                SqlParameter dbParam_BfastPaidRate = new SqlParameter();
                dbParam_BfastPaidRate.ParameterName = "@BfastPaidRate";
                dbParam_BfastPaidRate.Value = string.IsNullOrEmpty(TextBoxBfastPaidRate.Text) ? "0" : TextBoxBfastPaidRate.Text;
                dbParam_BfastPaidRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastPaidRate);

                SqlParameter dbParam_BfastRedRate = new SqlParameter();
                dbParam_BfastRedRate.ParameterName = "@BfastRedRate";
                dbParam_BfastRedRate.Value = string.IsNullOrEmpty(TextBoxBfastRedRate.Text) ? "0" : TextBoxBfastRedRate.Text;
                dbParam_BfastRedRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastRedRate);

                SqlParameter dbParam_BfastFreeRate = new SqlParameter();
                dbParam_BfastFreeRate.ParameterName = "@BfastFreeRate";
                dbParam_BfastFreeRate.Value = string.IsNullOrEmpty(TextBoxBfastFreeRate.Text) ? "0" : TextBoxBfastFreeRate.Text;
                dbParam_BfastFreeRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastFreeRate);

                SqlParameter dbParam_BfastIncentive = new SqlParameter();
                dbParam_BfastIncentive.ParameterName = "@BreakfastIncent";
                dbParam_BfastIncentive.Value = string.IsNullOrEmpty(TextBoxBreakfastIncent.Text) ? "0" : TextBoxBreakfastIncent.Text;
                dbParam_BfastIncentive.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastIncentive);

                SqlParameter dbParam_LunchPaidRate = new SqlParameter();
                dbParam_LunchPaidRate.ParameterName = "@LunchPaidRate";
                dbParam_LunchPaidRate.Value = string.IsNullOrEmpty(TextBoxLunchPaidRate.Text) ? "0" : TextBoxLunchPaidRate.Text;
                dbParam_LunchPaidRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchPaidRate);

                SqlParameter dbParam_LunchRedRate = new SqlParameter();
                dbParam_LunchRedRate.ParameterName = "@LunchRedRate";
                dbParam_LunchRedRate.Value = string.IsNullOrEmpty(TextBoxLunchRedRate.Text) ? "0" : TextBoxLunchRedRate.Text;
                dbParam_LunchRedRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchRedRate);

                SqlParameter dbParam_LunchFreeRate = new SqlParameter();
                dbParam_LunchFreeRate.ParameterName = "@LunchFreeRate";
                dbParam_LunchFreeRate.Value = string.IsNullOrEmpty(TextBoxLunchFreeRate.Text) ? "0" : TextBoxLunchFreeRate.Text;
                dbParam_LunchFreeRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchFreeRate);

                SqlParameter dbParam_OSnackPaidRate = new SqlParameter();
                dbParam_OSnackPaidRate.ParameterName = "@OSnackPaidRate";
                dbParam_OSnackPaidRate.Value = string.IsNullOrEmpty(TextBoxOSnackPaidRate.Text) ? "0" : TextBoxOSnackPaidRate.Text;
                dbParam_OSnackPaidRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_OSnackPaidRate);

                SqlParameter dbParam_OSnackRedRate = new SqlParameter();
                dbParam_OSnackRedRate.ParameterName = "@OSnackRedRate";
                dbParam_OSnackRedRate.Value = string.IsNullOrEmpty(TextBoxOSnackRedRate.Text) ? "0" : TextBoxOSnackRedRate.Text;
                dbParam_OSnackRedRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_OSnackRedRate);

                SqlParameter dbParam_OSnackFreeRate = new SqlParameter();
                dbParam_OSnackFreeRate.ParameterName = "@OSnackFreeRate";
                dbParam_OSnackFreeRate.Value = string.IsNullOrEmpty(TextBoxOSnackFreeRate.Text) ? "0" : TextBoxOSnackFreeRate.Text;
                dbParam_OSnackFreeRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_OSnackFreeRate);
                #endregion

                try
                {
                    int recordcount = 0;
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error saving the school info.');");
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            //Mark hdr as saved
            HFDistSchoolInfoSaved.Value = "1";

        }

        protected void TextBoxSchoolInfo_TextChanged(object sender, EventArgs e)
        {
            SaveDistSchoolInfo();
        }

        protected void CB_SchInfo_CheckedChanged(object sender, EventArgs e)
        {
            SaveDistSchoolInfo();
        }

        protected void SqlDataSourceSchoolYearSetup_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            //if (e.AffectedRows == 0)
            //{
            //    FormViewSchoolInfo.DefaultMode = FormViewMode.Insert;
            //}
            //else
            //{
            //    FormViewSchoolInfo.DefaultMode = FormViewMode.Edit;
            //}
        }

        protected void FormViewSchoolInfo_DataBound(object sender, EventArgs e)
        {
            FormView FormViewSchoolInfo = (FormView)sender;

            if (FormViewSchoolInfo.DataItemCount==0)
                FormViewSchoolInfo.ChangeMode(FormViewMode.Insert);
            else
                FormViewSchoolInfo.ChangeMode(FormViewMode.Edit);
        }
    }
}
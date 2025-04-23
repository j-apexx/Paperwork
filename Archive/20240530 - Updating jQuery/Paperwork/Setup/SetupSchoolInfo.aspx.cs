using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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

            if (!IsPostBack)
            {
                VerifyAccess(true);

                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");
            }
            else
            {
                VerifyAccess(false);
            }

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

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

                //Update the Budget Year Hidden Field
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

                DropDownListSchools.DataBind();
                if (DropDownListSchools.Items.Count == 0)
                    Response.Redirect("~/Setup/SetupSchools.aspx");
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
            LabelErrors.Text = "";
            HiddenField HFDistSchoolInfoSaved = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistSchoolInfoSaved");
            if (HFDistSchoolInfoSaved.Value == "1")
                return;

            if (DropDownListSchools.Items.Count == 0)
                return;

            if (DropDownListDistricts.Items.Count == 0)
                return;

            #region ReferenceControls
            String KeyPYear = FormViewSchoolInfo.DataKey.Values["ProgramYear"].ToString();
            String KeyDistID = FormViewSchoolInfo.DataKey.Values["DistrictID"].ToString();
            Int16 KeySchlNum = (Int16)FormViewSchoolInfo.DataKey.Values["SchoolNum"];

            //HiddenField HFSchoolNum = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFSchoolNum");
            TextBox TextBoxAttFactor = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxAttFactor");
            TextBox TextBoxEnrollment = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxEnrollment");
            TextBox TextBoxPaidElig = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxPaidElig");
            TextBox TextBoxReducedElig = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxReducedElig");
            TextBox TextBoxFreeElig = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxFreeElig");

            TextBox TextBoxCEPCoverage = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxCEPCoverage");
            CheckBox CheckBoxSchoolCEP = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CheckBoxSchoolCEP");
            CheckBox CB_UseCustomCEPPcts = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_UseCustomCEPPcts");
            TextBox TextBoxSchoolCEPPaidPct = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxSchoolCEPPaidPct");
            TextBox TextBoxSchoolCEPFreePct = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxSchoolCEPFreePct");
            //HiddenField HFDistCEPaidPct = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistCEPaidPct");
            //HiddenField HFDistCEFreePct = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistCEFreePct");
            HiddenField HFCEPDist = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFCEPDist");
            HiddenField HFCEPCoverageInt = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFCEPCoverageInt");



            CheckBox CB_Breakfast = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Breakfast");
            CheckBox CB_Tiers = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Tiers");
            CheckBox CB_OtherSales = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_OtherSales");
            CheckBox CB_Learners = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Learners");
            CheckBox CB_Snacks = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Snacks");
            CheckBox CB_Dinner = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_Dinner");//New 7-2022

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
            TextBox TextBoxAdultLunchPrice = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxAdultLunchPrice");

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
            TextBox TextBoxDinnerRate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxDinnerRate");//New 7-2022

            //New mid-year prices (for Oct 01, 2022 Breakfasts changing to free)
            TextBox TextBoxEffectivePricesDate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxEffectivePricesDate");
            TextBox TextBoxBfastElPrice2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastElPrice2");
            TextBox TextBoxBfastJrPrice2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastJrPrice2");
            TextBox TextBoxBfastSrPrice2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastSrPrice2");
            TextBox TextBoxBfastRedPrice2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastRedPrice2");
            CheckBox CBMidYearPriceChange = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CBMidYearPriceChange");


            //New mid-year reimbursement rates
            TextBox TextBoxEffectiveRatesDate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxEffectiveRatesDate");
            TextBox TextBoxBfastPaidRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastPaidRate2");
            TextBox TextBoxBfastRedRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastRedRate2");
            TextBox TextBoxBfastFreeRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBfastFreeRate2");
            TextBox TextBoxLunchPaidRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchPaidRate2");
            TextBox TextBoxLunchRedRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchRedRate2");
            TextBox TextBoxLunchFreeRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchFreeRate2");
            TextBox TextBoxOSnackPaidRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxOSnackPaidRate2");
            TextBox TextBoxOSnackRedRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxOSnackRedRate2");
            TextBox TextBoxOSnackFreeRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxOSnackFreeRate2");
            CheckBox CBMidYearRateChange = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CBMidYearRateChange");
            TextBox TextBoxDinnerRate2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxDinnerRate2");//New 7-2022

            //New mid-year lunch prices (for Feb 01, 2023 Lunch)
            TextBox TextBoxEffectivePricesDateL = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxEffectivePricesDateL");
            TextBox TextBoxLunchElPrice2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchElPrice2");
            TextBox TextBoxLunchJrPrice2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchJrPrice2");
            TextBox TextBoxLunchSrPrice2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchSrPrice2");
            TextBox TextBoxLunchRedPrice2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxLunchRedPrice2");
            CheckBox CBMidYearPriceChangeL = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CBMidYearPriceChangeL");
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
                dbParam_ProgYear.Value = KeyPYear; //Session["ProgramYear"].ToString();
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = KeyDistID; //Session["CurrentDistrict"].ToString();
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchlNum = new SqlParameter();
                dbParam_SchlNum.ParameterName = "@SchoolNum";
                dbParam_SchlNum.Value = KeySchlNum; //HFSchoolNum.Value;
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
                    if (Session["AccessLevel"].ToString().ToUpper() == "ADMIN")
                        LabelErrors.Text = ex.Message;

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
                    " ,[EligibleReduced],[BreakfastIncent],[Breakfasts],[Tiers],[OtherSales],[Learners],[Snacks] " +
                    " ,[SchoolCEPFreePct],[SchoolCEPPaidPct],[PartialCEPPartic],[UseCustomCEPPct] " +
                    " ,[Reim2EffectiveDate],[BfastPaidRate2],[BfastRedRate2],[BfastFreeRate2],[LunchPaidRate2],[LunchRedRate2],[LunchFreeRate2],[OSnackPaidRate2],[OSnackRedRate2],[OSnackFreeRate2] " +
                    " ,[DinnerRate],[DinnerRate2],[Dinners],[AdultLunchPrice] " +
                    " ,[Price2EffectiveDate],[BfastElPrice2],[BfastJrPrice2],[BfastSrPrice2],[BfastRedPrice2] " +
                    " ,[LPrice2EffectiveDate],[LunchElPrice2],[LunchJrPrice2],[LunchSrPrice2],[LunchRedPrice2]) " +
                    " VALUES (@ProgramYear,@DistrictID,@SchoolNum,@BfastElPrice,@BfastJrPrice,@BfastSrPrice,@BfastRedPrice,@LunchElPrice,@LunchJrPrice,@LunchSrPrice " +
                    " ,@LunchTier1Price,@LunchTier2Price,@LunchRedPrice,@BfastPaidRate,@BfastRedRate,@BfastFreeRate,@LunchPaidRate,@LunchRedRate,@LunchFreeRate " +
                    " ,@OSnackPaidPrice,@OSnackRedPrice,@OSnackPaidRate,@OSnackRedRate,@OSnackFreeRate,@AF,@Enrollment,@EligiblePaid,@EligibleFree " +
                    " ,@EligibleReduced,@BreakfastIncent,@Breakfasts,@Tiers,@OtherSales,@Learners,@Snacks" +
                    " ,@SchoolCEPFreePct,@SchoolCEPPaidPct,@PartialCEPPartic,@UseCustomCEPPct" +
                    " ,@Reim2EffectiveDate,@BfastPaidRate2,@BfastRedRate2,@BfastFreeRate2,@LunchPaidRate2,@LunchRedRate2,@LunchFreeRate2,@OSnackPaidRate2,@OSnackRedRate2,@OSnackFreeRate2 " +
                    " ,@DinnerRate,@DinnerRate2,@Dinners,@AdultLunchPrice " +
                    " ,@Price2EffectiveDate,@BfastElPrice2,@BfastJrPrice2,@BfastSrPrice2,@BfastRedPrice2 " +
                    " ,@LPrice2EffectiveDate,@LunchElPrice2,@LunchJrPrice2,@LunchSrPrice2,@LunchRedPrice2) ";
            }
            else
            {
                InsertUpdateQuery = "UPDATE [DistrictSchoolYearSetup] SET AF=@AF, Enrollment=@Enrollment, EligiblePaid=@EligiblePaid, EligibleFree=@EligibleFree, "
                    + " EligibleReduced=@EligibleReduced, BreakfastIncent=@BreakfastIncent, Breakfasts=@Breakfasts, Tiers=@Tiers, OtherSales=@OtherSales, Learners=@Learners, Snacks=@Snacks, "
                    + " BfastElPrice=@BfastElPrice, BfastJrPrice=@BfastJrPrice, BfastSrPrice=@BfastSrPrice, BfastRedPrice=@BfastRedPrice, LunchElPrice=@LunchElPrice, LunchJrPrice=@LunchJrPrice,"
                    + " LunchSrPrice=@LunchSrPrice, LunchTier1Price=@LunchTier1Price, LunchTier2Price=@LunchTier2Price, LunchRedPrice=@LunchRedPrice, OSnackPaidPrice=@OSnackPaidPrice, "
                    + " OSnackRedPrice=@OSnackRedPrice, BfastPaidRate=@BfastPaidRate, BfastRedRate=@BfastRedRate, BfastFreeRate=@BfastFreeRate, LunchPaidRate=@LunchPaidRate, "
                    + " LunchRedRate=@LunchRedRate, LunchFreeRate=@LunchFreeRate, OSnackPaidRate=@OSnackPaidRate, OSnackRedRate=@OSnackRedRate, OSnackFreeRate=@OSnackFreeRate,"
                    + " SchoolCEPFreePct=@SchoolCEPFreePct,[SchoolCEPPaidPct]=@SchoolCEPPaidPct,[PartialCEPPartic]=@PartialCEPPartic,[UseCustomCEPPct]=@UseCustomCEPPct, "
                    + " Reim2EffectiveDate=@Reim2EffectiveDate,BfastPaidRate2=@BfastPaidRate2,BfastRedRate2=@BfastRedRate2,BfastFreeRate2=@BfastFreeRate2,LunchPaidRate2=@LunchPaidRate2,"
                    + " LunchRedRate2=@LunchRedRate2,LunchFreeRate2=@LunchFreeRate2,OSnackPaidRate2=@OSnackPaidRate2,OSnackRedRate2=@OSnackRedRate2,OSnackFreeRate2=@OSnackFreeRate2 " 
                    + ",DinnerRate=@DinnerRate,DinnerRate2=@DinnerRate2,Dinners=@Dinners,AdultLunchPrice=@AdultLunchPrice " 
                    + ",Price2EffectiveDate=@Price2EffectiveDate,BfastElPrice2=@BfastElPrice2,BfastJrPrice2=@BfastJrPrice2,BfastSrPrice2=@BfastSrPrice2,BfastRedPrice2=@BfastRedPrice2 "
                    + ",LPrice2EffectiveDate=@LPrice2EffectiveDate,LunchElPrice2=@LunchElPrice2,LunchJrPrice2=@LunchJrPrice2,LunchSrPrice2=@LunchSrPrice2,LunchRedPrice2=@LunchRedPrice2 "
                    + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND Schoolnum=@SchoolNum ";
            }

            using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection))
            {

                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = KeyPYear;// Session["ProgramYear"].ToString();
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = KeyDistID;// Session["CurrentDistrict"].ToString();
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_SchlNum2 = new SqlParameter();
                dbParam_SchlNum2.ParameterName = "@SchoolNum";
                dbParam_SchlNum2.Value = KeySchlNum;// HFSchoolNum.Value;
                dbParam_SchlNum2.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_SchlNum2);

                #endregion

                //ProgramYear Dates
                DateTime.TryParse(HFProgYearStartDate.Value, out DateTime pyStartDate);
                DateTime.TryParse(HFProgYearEndDate.Value, out DateTime pyEndDate);


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
                #region CEP

                SqlParameter dbParam_SchCEPPaid = new SqlParameter();
                dbParam_SchCEPPaid.ParameterName = "@SchoolCEPPaidPct";
                dbParam_SchCEPPaid.DbType = System.Data.DbType.Decimal;

                SqlParameter dbParam_SchCEPFree = new SqlParameter();
                dbParam_SchCEPFree.ParameterName = "@SchoolCEPFreePct";
                dbParam_SchCEPFree.DbType = System.Data.DbType.Decimal;

                SqlParameter dbParam_PartialCEPPartic = new SqlParameter();
                dbParam_PartialCEPPartic.ParameterName = "@PartialCEPPartic";
                dbParam_PartialCEPPartic.DbType = System.Data.DbType.Byte;

                SqlParameter dbParam_UseCustomCEP = new SqlParameter();
                dbParam_UseCustomCEP.ParameterName = "@UseCustomCEPPct";
                dbParam_UseCustomCEP.DbType = System.Data.DbType.Byte;

                if (HFCEPDist.Value == "1")
                {

                    if (HFCEPCoverageInt.Value == "0")//District Wide
                        dbParam_PartialCEPPartic.Value = true;
                    else if (HFCEPCoverageInt.Value == "1")//Participation based on checkbox
                        dbParam_PartialCEPPartic.Value = CheckBoxSchoolCEP.Checked;

                    dbParam_UseCustomCEP.Value = CB_UseCustomCEPPcts.Checked;
                    if (CB_UseCustomCEPPcts.Checked)
                    {
                        dbParam_SchCEPPaid.Value = TextBoxSchoolCEPPaidPct.Text;
                        dbParam_SchCEPFree.Value = TextBoxSchoolCEPFreePct.Text;
                    }
                    else//do not store values if not custom. This will eliminate confusion especially if dist values change.
                    {
                        dbParam_SchCEPPaid.Value = 0M;
                        dbParam_SchCEPFree.Value = 0M;
                    }

                }
                else
                {
                    dbParam_SchCEPPaid.Value = 0M;
                    dbParam_SchCEPFree.Value = 0M;
                    dbParam_PartialCEPPartic.Value = false;
                    dbParam_UseCustomCEP.Value = false;
                }

                cmd2.Parameters.Add(dbParam_SchCEPPaid);
                cmd2.Parameters.Add(dbParam_SchCEPFree);
                cmd2.Parameters.Add(dbParam_PartialCEPPartic);
                cmd2.Parameters.Add(dbParam_UseCustomCEP);



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

                SqlParameter dbParam_Dinners = new SqlParameter();
                dbParam_Dinners.ParameterName = "@Dinners";
                dbParam_Dinners.Value = CB_Dinner.Checked;
                dbParam_Dinners.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_Dinners);
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

                SqlParameter dbParam_AdultLunchPrice = new SqlParameter();
                dbParam_AdultLunchPrice.ParameterName = "@AdultLunchPrice";
                dbParam_AdultLunchPrice.Value = string.IsNullOrEmpty(TextBoxAdultLunchPrice.Text) ? "0" : TextBoxAdultLunchPrice.Text;
                dbParam_AdultLunchPrice.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_AdultLunchPrice);


                #endregion
                #region Prices2
                //Breakfasts
                DateTime.TryParse(TextBoxEffectivePricesDate.Text, out DateTime effPricesDate);
                if (effPricesDate >= pyEndDate || effPricesDate <= pyStartDate)
                {
                    effPricesDate = new DateTime(9999, 12, 31);
                    CBMidYearPriceChange.Checked = false;
                    CBPriceRatesShowHide();

                    TextBoxBfastElPrice2.Text = "0";
                    TextBoxBfastJrPrice2.Text = "0";
                    TextBoxBfastSrPrice2.Text = "0";
                    TextBoxBfastRedPrice2.Text = "0";
                }

                SqlParameter dbParam_effPricesDate = new SqlParameter();
                dbParam_effPricesDate.ParameterName = "@Price2EffectiveDate";
                dbParam_effPricesDate.Value = effPricesDate.ToShortDateString();
                dbParam_effPricesDate.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_effPricesDate);

                SqlParameter dbParam_BfastElPrice2 = new SqlParameter();
                dbParam_BfastElPrice2.ParameterName = "@BfastElPrice2";
                dbParam_BfastElPrice2.Value = string.IsNullOrEmpty(TextBoxBfastElPrice2.Text) ? "0" : TextBoxBfastElPrice2.Text;
                dbParam_BfastElPrice2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastElPrice2);

                SqlParameter dbParam_BfastJrPrice2 = new SqlParameter();
                dbParam_BfastJrPrice2.ParameterName = "@BfastJrPrice2";
                dbParam_BfastJrPrice2.Value = string.IsNullOrEmpty(TextBoxBfastJrPrice2.Text) ? "0" : TextBoxBfastJrPrice2.Text;
                dbParam_BfastJrPrice2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastJrPrice2);

                SqlParameter dbParam_BfastSrPrice2 = new SqlParameter();
                dbParam_BfastSrPrice2.ParameterName = "@BfastSrPrice2";
                dbParam_BfastSrPrice2.Value = string.IsNullOrEmpty(TextBoxBfastSrPrice2.Text) ? "0" : TextBoxBfastSrPrice2.Text;
                dbParam_BfastSrPrice2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastSrPrice2);

                SqlParameter dbParam_BfastRedPrice2 = new SqlParameter();
                dbParam_BfastRedPrice2.ParameterName = "@BfastRedPrice2";
                dbParam_BfastRedPrice2.Value = string.IsNullOrEmpty(TextBoxBfastRedPrice2.Text) ? "0" : TextBoxBfastRedPrice2.Text;
                dbParam_BfastRedPrice2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastRedPrice2);
                #endregion
                #region Prices2L
                //New Lunches section
                DateTime.TryParse(TextBoxEffectivePricesDateL.Text, out DateTime effPricesDateL);
                if (effPricesDateL >= pyEndDate || effPricesDateL <= pyStartDate)
                {
                    effPricesDateL = new DateTime(9999, 12, 31);
                    CBMidYearPriceChangeL.Checked = false;
                    CBPriceRatesShowHide();

                    TextBoxLunchElPrice2.Text = "0";
                    TextBoxLunchJrPrice2.Text = "0";
                    TextBoxLunchSrPrice2.Text = "0";
                    TextBoxLunchRedPrice2.Text = "0";
                }

                SqlParameter dbParam_effPricesDateL = new SqlParameter();
                dbParam_effPricesDateL.ParameterName = "@LPrice2EffectiveDate";
                dbParam_effPricesDateL.Value = effPricesDateL.ToShortDateString();
                dbParam_effPricesDateL.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_effPricesDateL);

                SqlParameter dbParam_LunchElPrice2 = new SqlParameter();
                dbParam_LunchElPrice2.ParameterName = "@LunchElPrice2";
                dbParam_LunchElPrice2.Value = string.IsNullOrEmpty(TextBoxLunchElPrice2.Text) ? "0" : TextBoxLunchElPrice2.Text;
                dbParam_LunchElPrice2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchElPrice2);

                SqlParameter dbParam_LunchJrPrice2 = new SqlParameter();
                dbParam_LunchJrPrice2.ParameterName = "@LunchJrPrice2";
                dbParam_LunchJrPrice2.Value = string.IsNullOrEmpty(TextBoxLunchJrPrice2.Text) ? "0" : TextBoxLunchJrPrice2.Text;
                dbParam_LunchJrPrice2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchJrPrice2);

                SqlParameter dbParam_LunchSrPrice2 = new SqlParameter();
                dbParam_LunchSrPrice2.ParameterName = "@LunchSrPrice2";
                dbParam_LunchSrPrice2.Value = string.IsNullOrEmpty(TextBoxLunchSrPrice2.Text) ? "0" : TextBoxLunchSrPrice2.Text;
                dbParam_LunchSrPrice2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchSrPrice2);

                SqlParameter dbParam_LunchRedPrice2 = new SqlParameter();
                dbParam_LunchRedPrice2.ParameterName = "@LunchRedPrice2";
                dbParam_LunchRedPrice2.Value = string.IsNullOrEmpty(TextBoxLunchRedPrice2.Text) ? "0" : TextBoxLunchRedPrice2.Text;
                dbParam_LunchRedPrice2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchRedPrice2);
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

                SqlParameter dbParam_DinnerRate = new SqlParameter();
                dbParam_DinnerRate.ParameterName = "@DinnerRate";
                dbParam_DinnerRate.Value = string.IsNullOrEmpty(TextBoxDinnerRate.Text) ? "0" : TextBoxDinnerRate.Text;
                dbParam_DinnerRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DinnerRate);
                #endregion
                #region Reimbursements2
                DateTime.TryParse(TextBoxEffectiveRatesDate.Text, out DateTime effDate);
                if (effDate >= pyEndDate || effDate <= pyStartDate)
                {
                    effDate = new DateTime(9999, 12, 31);
                    CBMidYearRateChange.Checked = false;
                    CBPriceRatesShowHide();
                    TextBoxBfastPaidRate2.Text = "0";
                    TextBoxBfastRedRate2.Text = "0";
                    TextBoxBfastFreeRate2.Text= "0";
                    TextBoxLunchPaidRate2.Text="0";
                    TextBoxLunchRedRate2.Text = "0";
                    TextBoxLunchFreeRate2.Text = "0";
                    TextBoxOSnackPaidRate2.Text = "0";
                    TextBoxOSnackRedRate2.Text = "0";
                    TextBoxOSnackFreeRate2.Text = "0";
                    TextBoxDinnerRate2.Text = "0";
                }

                SqlParameter dbParam_effDate = new SqlParameter();
                dbParam_effDate.ParameterName = "@Reim2EffectiveDate";
                dbParam_effDate.Value = effDate.ToShortDateString();
                dbParam_effDate.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_effDate);

                SqlParameter dbParam_BfastPaidRate2 = new SqlParameter();
                dbParam_BfastPaidRate2.ParameterName = "@BfastPaidRate2";
                dbParam_BfastPaidRate2.Value = string.IsNullOrEmpty(TextBoxBfastPaidRate2.Text) ? "0" : TextBoxBfastPaidRate2.Text;
                dbParam_BfastPaidRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastPaidRate2);

                SqlParameter dbParam_BfastRedRate2 = new SqlParameter();
                dbParam_BfastRedRate2.ParameterName = "@BfastRedRate2";
                dbParam_BfastRedRate2.Value = string.IsNullOrEmpty(TextBoxBfastRedRate2.Text) ? "0" : TextBoxBfastRedRate2.Text;
                dbParam_BfastRedRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastRedRate2);

                SqlParameter dbParam_BfastFreeRate2 = new SqlParameter();
                dbParam_BfastFreeRate2.ParameterName = "@BfastFreeRate2";
                dbParam_BfastFreeRate2.Value = string.IsNullOrEmpty(TextBoxBfastFreeRate2.Text) ? "0" : TextBoxBfastFreeRate2.Text;
                dbParam_BfastFreeRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BfastFreeRate2);

                SqlParameter dbParam_LunchPaidRate2 = new SqlParameter();
                dbParam_LunchPaidRate2.ParameterName = "@LunchPaidRate2";
                dbParam_LunchPaidRate2.Value = string.IsNullOrEmpty(TextBoxLunchPaidRate2.Text) ? "0" : TextBoxLunchPaidRate2.Text;
                dbParam_LunchPaidRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchPaidRate2);

                SqlParameter dbParam_LunchRedRate2 = new SqlParameter();
                dbParam_LunchRedRate2.ParameterName = "@LunchRedRate2";
                dbParam_LunchRedRate2.Value = string.IsNullOrEmpty(TextBoxLunchRedRate2.Text) ? "0" : TextBoxLunchRedRate2.Text;
                dbParam_LunchRedRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchRedRate2);

                SqlParameter dbParam_LunchFreeRate2 = new SqlParameter();
                dbParam_LunchFreeRate2.ParameterName = "@LunchFreeRate2";
                dbParam_LunchFreeRate2.Value = string.IsNullOrEmpty(TextBoxLunchFreeRate2.Text) ? "0" : TextBoxLunchFreeRate2.Text;
                dbParam_LunchFreeRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchFreeRate2);

                SqlParameter dbParam_OSnackPaidRate2 = new SqlParameter();
                dbParam_OSnackPaidRate2.ParameterName = "@OSnackPaidRate2";
                dbParam_OSnackPaidRate2.Value = string.IsNullOrEmpty(TextBoxOSnackPaidRate2.Text) ? "0" : TextBoxOSnackPaidRate2.Text;
                dbParam_OSnackPaidRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_OSnackPaidRate2);

                SqlParameter dbParam_OSnackRedRate2 = new SqlParameter();
                dbParam_OSnackRedRate2.ParameterName = "@OSnackRedRate2";
                dbParam_OSnackRedRate2.Value = string.IsNullOrEmpty(TextBoxOSnackRedRate2.Text) ? "0" : TextBoxOSnackRedRate2.Text;
                dbParam_OSnackRedRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_OSnackRedRate2);

                SqlParameter dbParam_OSnackFreeRate2 = new SqlParameter();
                dbParam_OSnackFreeRate2.ParameterName = "@OSnackFreeRate2";
                dbParam_OSnackFreeRate2.Value = string.IsNullOrEmpty(TextBoxOSnackFreeRate2.Text) ? "0" : TextBoxOSnackFreeRate2.Text;
                dbParam_OSnackFreeRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_OSnackFreeRate2);

                SqlParameter dbParam_DinnerRate2 = new SqlParameter();
                dbParam_DinnerRate2.ParameterName = "@DinnerRate2";
                dbParam_DinnerRate2.Value = string.IsNullOrEmpty(TextBoxDinnerRate2.Text) ? "0" : TextBoxDinnerRate2.Text;
                dbParam_DinnerRate2.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DinnerRate2);
                #endregion

                try
                {
                    int recordcount = 0;
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();
                    if (recordcount > 0)
                    {
                        ShowMessageIcon("Saved", MessageType.Success);

                    }
                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error saving the school info.');");

                    if (Session["AccessLevel"].ToString().ToUpper() == "ADMIN")
                        LabelErrors.Text = ex.Message;

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

        protected void FormViewSchoolInfo_DataBound(object sender, EventArgs e)
        {
            FormView FormViewSchoolInfo = (FormView)sender;

            if (FormViewSchoolInfo.DataItemCount == 0)
                FormViewSchoolInfo.ChangeMode(FormViewMode.Insert);
            else
                FormViewSchoolInfo.ChangeMode(FormViewMode.Edit);

            //Populate the Budget Values from the Budget Connection and Datasource
            TextBox TextBoxBdgtPaidElemB = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtPaidElemB");
            TextBox TextBoxBdgtPaidJrB = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtPaidJrB");
            TextBox TextBoxBdgtPaidSrB = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtPaidSrB");
            TextBox TextBoxBdgtRedB = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtRedB");
            TextBox TextBoxBdgtFreeB = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtFreeB");
            //Lunch
            TextBox TextBoxBdgtPaidElemL = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtPaidElemL");
            TextBox TextBoxBdgtPaidJrL = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtPaidJrL");
            TextBox TextBoxBdgtPaidSrL = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtPaidSrL");

            TextBox TextBoxBdgtTier1L = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtTier1L");
            TextBox TextBoxBdgtTier2L = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtTier2L");

            TextBox TextBoxBdgtRedL = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtRedL");
            TextBox TextBoxBdgtFreeL = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtFreeL");

            TextBox TextBoxBdgtStuAla = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtStuAla");
            TextBox TextBoxBdgtAdultAla = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtAdultAla");
            TextBox TextBoxBdgtOther1 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtOther1");
            TextBox TextBoxBdgtOther2 = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxBdgtOther2");
            HiddenField HFCEPDist = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFCEPDist");
            HtmlGenericControl SchoolCEP = (HtmlGenericControl)FindControlRecursive(FormViewSchoolInfo, "SchoolCEP");
            TextBox TextBoxCEPCoverage = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxCEPCoverage");
            HiddenField HFCEPCoverageInt = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFCEPCoverageInt");
            CheckBox CheckBoxSchoolCEP = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CheckBoxSchoolCEP");
            CheckBox CB_UseCustomCEPPcts = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CB_UseCustomCEPPcts");
            TextBox TextBoxSchoolCEPPaidPct = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxSchoolCEPPaidPct");
            TextBox TextBoxSchoolCEPFreePct = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxSchoolCEPFreePct");
            HiddenField HFDistCEPaidPct = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistCEPaidPct");
            HiddenField HFDistCEFreePct = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistCEFreePct");

            //Use special dataset for SFSP
            SqlDataReader rdr = null;
            if (!isSFSP())
            {
                SqlDataSourceBDGT.DataBind();
                rdr = (SqlDataReader)SqlDataSourceBDGT.Select(DataSourceSelectArguments.Empty);
            }

            //SqlDataSourceBDGT.DataBind();
            //SqlDataReader rdr = (SqlDataReader)SqlDataSourceBDGT.Select(DataSourceSelectArguments.Empty);
            if (rdr != null && rdr.Read())
            {
                TextBoxBdgtPaidElemB.Text = Decimal.Round(((Decimal)rdr["ElemPaidB_Site"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtPaidJrB.Text = Decimal.Round(((Decimal)rdr["MiddlePaidB_Site"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtPaidSrB.Text = Decimal.Round(((Decimal)rdr["HSPaidB_Site"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtRedB.Text = Decimal.Round(((Decimal)rdr["ReducedBreakfast"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtFreeB.Text = Decimal.Round(((Decimal)rdr["FreeBreakfast"]), 2, MidpointRounding.AwayFromZero).ToString("F2");

                TextBoxBdgtPaidElemL.Text = Decimal.Round(((Decimal)rdr["ElemPaidL_Site"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtPaidJrL.Text = Decimal.Round(((Decimal)rdr["MiddlePaidL_Site"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtPaidSrL.Text = Decimal.Round(((Decimal)rdr["HSPaidL_Site"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtTier1L.Text = Decimal.Round(((Decimal)rdr["PaidTier1L"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtTier2L.Text = Decimal.Round(((Decimal)rdr["PaidTier2L"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtRedL.Text = Decimal.Round(((Decimal)rdr["ReducedLunch"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtFreeL.Text = Decimal.Round(((Decimal)rdr["FreeLunch"]), 2, MidpointRounding.AwayFromZero).ToString("F2");

                TextBoxBdgtStuAla.Text = Decimal.Round(((Decimal)rdr["StudentAla"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtAdultAla.Text = Decimal.Round(((Decimal)rdr["Adult"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtOther1.Text = Decimal.Round(((Decimal)rdr["AlaCarteSalesRevInc_Site"]), 2, MidpointRounding.AwayFromZero).ToString("F2");
                TextBoxBdgtOther2.Text = Decimal.Round(((Decimal)rdr["OtherRevVending_Site"]), 2, MidpointRounding.AwayFromZero).ToString("F2");

                bool CEPDistrict = rdr["CEP"].ToString() == "0" ? false : true;
                if (CEPDistrict)
                {
                    HFCEPDist.Value = "1";
                    SchoolCEP.Visible = true;
                    HFCEPCoverageInt.Value = rdr["CEPCoverage"].ToString();
                    TextBoxCEPCoverage.Text = ((Int16)rdr["CEPCoverage"]) == 0 ? "District Wide" : "Partial";
                    if (((Int16)rdr["CEPCoverage"]) == 0)//District Wide
                    {
                        CheckBoxSchoolCEP.Enabled = false;//DistrictWide, not option
                        CheckBoxSchoolCEP.Checked = true;
                    }
                    else
                    {
                        CheckBoxSchoolCEP.Enabled = true;//Partial, can opt in or out
                        //Checked will map to [PartialCEPPartic]
                    }

                    //CB_UseCustomCEPPcts.DataBind();
                    if (CB_UseCustomCEPPcts.Checked == false)//use District Values
                    {
                        TextBoxSchoolCEPPaidPct.Text = HFDistCEPaidPct.Value;
                        TextBoxSchoolCEPPaidPct.Enabled = false;
                        TextBoxSchoolCEPFreePct.Text = HFDistCEFreePct.Value;
                        TextBoxSchoolCEPFreePct.Enabled = false;
                    }
                    else
                    {
                        //Use the school mapped values from the table
                        TextBoxSchoolCEPPaidPct.Enabled = true;
                        TextBoxSchoolCEPFreePct.Enabled = true;
                    }

                }
                else
                {
                    HFCEPDist.Value = "0";
                    SchoolCEP.Visible = false;
                    TextBoxCEPCoverage.Text = "";
                    HFCEPCoverageInt.Value = "0";
                }
            }
            else if (TextBoxBdgtPaidElemB != null) //zeros
            {
                TextBoxBdgtPaidElemB.Text = (0M).ToString("F2");
                TextBoxBdgtPaidJrB.Text = (0M).ToString("F2");
                TextBoxBdgtPaidSrB.Text = (0M).ToString("F2");
                TextBoxBdgtRedB.Text = (0M).ToString("F2");
                TextBoxBdgtFreeB.Text = (0M).ToString("F2");

                TextBoxBdgtPaidElemL.Text = (0M).ToString("F2");
                TextBoxBdgtPaidJrL.Text = (0M).ToString("F2");
                TextBoxBdgtPaidSrL.Text = (0M).ToString("F2");
                TextBoxBdgtTier1L.Text = (0M).ToString("F2");
                TextBoxBdgtTier2L.Text = (0M).ToString("F2");
                TextBoxBdgtRedL.Text = (0M).ToString("F2");
                TextBoxBdgtFreeL.Text = (0M).ToString("F2");

                TextBoxBdgtStuAla.Text = (0M).ToString("F2");
                TextBoxBdgtAdultAla.Text = (0M).ToString("F2");
                TextBoxBdgtOther1.Text = (0M).ToString("F2");
                TextBoxBdgtOther2.Text = (0M).ToString("F2");

                SchoolCEP.Visible = false;
                HFCEPDist.Value = "0";
            }

            //run the mid-year price and rate change scripts
            //This new procedure shows or hides the rate and price change divs without triggering the save
            CBPriceRatesShowHide();
        }

        protected void CB_UseCustomCEPPcts_CheckedChanged(object sender, EventArgs e)
        {
            TextBox TextBoxSchoolCEPPaidPct = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxSchoolCEPPaidPct");
            TextBox TextBoxSchoolCEPFreePct = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxSchoolCEPFreePct");
            HiddenField HFDistCEPaidPct = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistCEPaidPct");
            HiddenField HFDistCEFreePct = (HiddenField)FindControlRecursive(FormViewSchoolInfo, "HFDistCEFreePct");
            CheckBox CB_UseCustomCEPPcts = (CheckBox)sender;

            if (CB_UseCustomCEPPcts.Checked)
            {
                //Allow changing values
                TextBoxSchoolCEPPaidPct.Enabled = true;
                TextBoxSchoolCEPFreePct.Enabled = true;
            }
            else
            {
                TextBoxSchoolCEPPaidPct.Text = HFDistCEPaidPct.Value;
                TextBoxSchoolCEPPaidPct.Enabled = false;
                TextBoxSchoolCEPFreePct.Text = HFDistCEFreePct.Value;
                TextBoxSchoolCEPFreePct.Enabled = false;
            }

            //mark record as changed for saving
            CB_SchInfo_CheckedChanged(sender, e);
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            SqlDataSourceProgramYears.DataBind();

            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int recordIndex = dvSql.Find(DDLProgramYears.SelectedValue);
                if (recordIndex != -1)
                {
                    DataRowView drvSql = dvSql[recordIndex];
                    HFBudgetYear.Value = drvSql["BudgetYear"].ToString().TrimEnd(' ');
                    HFProgYearStartDate.Value = drvSql["StartDate"].ToString();
                    HFProgYearEndDate.Value = drvSql["EndDate"].ToString();
                }
                else
                {
                    HFBudgetYear.Value = "";
                    HFProgYearStartDate.Value = "";
                    HFProgYearEndDate.Value = "";
                }
            }
            else
            {
                HFBudgetYear.Value = "";
                HFProgYearStartDate.Value = "";
                HFProgYearEndDate.Value = "";
            }


        }

        protected void CBMidYearRateChange_CheckedChanged(object sender, EventArgs e)
        {
            Control DivMidYearRates = (Control)FindControlRecursive(FormViewSchoolInfo, "DivMidYearRates");
            CheckBox CBMidYearRateChange = (CheckBox)sender;
            TextBox TextBoxEffectiveRatesDate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxEffectiveRatesDate");
            if (DivMidYearRates != null && CBMidYearRateChange != null)
            {
                if (CBMidYearRateChange.Checked)
                {
                    DivMidYearRates.Visible = true;

                    //if effective date is the default 12/31/9999 - set to Jan 1 of current program year to make calendar selector easier to change if needed
                    if (TextBoxEffectiveRatesDate != null && TextBoxEffectiveRatesDate.Text == "12/31/9999")
                    {
                        if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PYEndDate))
                        {
                            DateTime defaultEffDate = new DateTime(PYEndDate.Year, 1, 1);
                            TextBoxEffectiveRatesDate.Text = defaultEffDate.ToShortDateString();

                        }
                    }
                }
                else
                {
                    DivMidYearRates.Visible = false;
                    TextBoxEffectiveRatesDate.Text = "12/31/9999";

                }
                //mark record as changed for saving
                CB_SchInfo_CheckedChanged(sender, e);
            }
        }

        protected void CBMidYearPriceChange_CheckedChanged(object sender, EventArgs e)
        {
            Control DivMidYearPrices = (Control)FindControlRecursive(FormViewSchoolInfo, "DivMidYearPrices");
            CheckBox CBMidYearPriceChange = (CheckBox)sender;
            TextBox TextBoxEffectivePricesDate = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxEffectivePricesDate");
            if (DivMidYearPrices != null && CBMidYearPriceChange != null)
            {
                if (CBMidYearPriceChange.Checked)
                {
                    DivMidYearPrices.Visible = true;

                    //if effective date is the default 12/31/9999 - set to Jan 1 of current program year to make calendar selector easier to change if needed
                    if (TextBoxEffectivePricesDate != null && TextBoxEffectivePricesDate.Text == "12/31/9999")
                    {
                        if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PYEndDate))
                        {
                            DateTime defaultEffDate = new DateTime(PYEndDate.Year, 1, 1);
                            TextBoxEffectivePricesDate.Text = defaultEffDate.ToShortDateString();

                        }
                    }
                }
                else
                {
                    DivMidYearPrices.Visible = false;
                    TextBoxEffectivePricesDate.Text = "12/31/9999";

                }
                //mark record as changed for saving
                CB_SchInfo_CheckedChanged(sender, e);
            }
        }

        protected void CBMidYearPriceChangeL_CheckedChanged(object sender, EventArgs e)
        {
            Control DivMidYearPricesL = (Control)FindControlRecursive(FormViewSchoolInfo, "DivMidYearPricesL");
            CheckBox CBMidYearPriceChangeL = (CheckBox)sender;
            TextBox TextBoxEffectivePricesDateL = (TextBox)FindControlRecursive(FormViewSchoolInfo, "TextBoxEffectivePricesDateL");
            if (DivMidYearPricesL != null && CBMidYearPriceChangeL != null)
            {
                if (CBMidYearPriceChangeL.Checked)
                {
                    DivMidYearPricesL.Visible = true;

                    //if effective date is the default 12/31/9999 - set to Jan 1 of current program year to make calendar selector easier to change if needed
                    if (TextBoxEffectivePricesDateL != null && TextBoxEffectivePricesDateL.Text == "12/31/9999")
                    {
                        if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime PYEndDate))
                        {
                            DateTime defaultEffDate = new DateTime(PYEndDate.Year, 1, 1);
                            TextBoxEffectivePricesDateL.Text = defaultEffDate.ToShortDateString();

                        }
                    }
                }
                else
                {
                    DivMidYearPricesL.Visible = false;
                    TextBoxEffectivePricesDateL.Text = "12/31/9999";

                }
                //mark record as changed for saving
                CB_SchInfo_CheckedChanged(sender, e);
            }
        }

        /// <summary>
        /// Created this procedure to show/hide during load process, but not trigger record saves which cause stack overflow during databinding
        /// </summary>
        protected void CBPriceRatesShowHide()
        {
            Control DivMidYearRates = (Control)FindControlRecursive(FormViewSchoolInfo, "DivMidYearRates");
            CheckBox CBMidYearRateChange = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CBMidYearRateChange"); ;
            if (DivMidYearRates != null && CBMidYearRateChange != null)
            {
                if (CBMidYearRateChange.Checked)
                    DivMidYearRates.Visible = true;
                else
                    DivMidYearRates.Visible = false;
            }

            Control DivMidYearPrices = (Control)FindControlRecursive(FormViewSchoolInfo, "DivMidYearPrices");
            CheckBox CBMidYearPriceChange = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CBMidYearPriceChange");
            if (DivMidYearPrices != null && CBMidYearPriceChange != null)
            {
                if (CBMidYearPriceChange.Checked)
                    DivMidYearPrices.Visible = true;
                else
                    DivMidYearPrices.Visible = false;
            }

            Control DivMidYearPricesL = (Control)FindControlRecursive(FormViewSchoolInfo, "DivMidYearPricesL");
            CheckBox CBMidYearPriceChangeL = (CheckBox)FindControlRecursive(FormViewSchoolInfo, "CBMidYearPriceChangeL");
            if (DivMidYearPricesL != null && CBMidYearPriceChangeL != null)
            {
                if (CBMidYearPriceChangeL.Checked)
                    DivMidYearPricesL.Visible = true;
                else
                    DivMidYearPricesL.Visible = false;
            }
        }
    }
}
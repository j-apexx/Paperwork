using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SalesEntry : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");
            else
                LabelProgramYear.Text = Session["ProgramYear"].ToString();

            if (!IsPostBack)
            {
                DateTime UseDate = DateTime.Now.Date;

                if (UseDate >= (DateTime)Session["ProgramYearStartDate"] && UseDate <= (DateTime)Session["ProgramYearEndDate"])
                { /*Do nothing */}
                else if (UseDate < (DateTime)Session["ProgramYearStartDate"])
                    UseDate = (DateTime)Session["ProgramYearStartDate"];
                else if(UseDate > (DateTime)Session["ProgramYearEndDate"])
                    UseDate = (DateTime)Session["ProgramYearEndDate"];

                TrxDate.Text = UseDate.ToShortDateString();

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

                PopulateSchoolDates();
            }
            else
            {

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
                Response.Redirect(Request.RawUrl);//Request.RawUrl provides the current page URL
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void SaveDistSchoolHdrData()
        {
            HiddenField HFDistSchoolHdrSaved = (HiddenField)FindControlRecursive(FormViewSales, "HFDistSchoolHdrSaved");
            if (HFDistSchoolHdrSaved.Value == "1")
                return;

            HiddenField HFSchoolNum = (HiddenField)FindControlRecursive(FormViewSales, "HFSchoolNum");
            TextBox TextBoxAttFactor = (TextBox)FindControlRecursive(FormViewSales, "TextBoxAttFactor");
            TextBox TextBoxEnrollment = (TextBox)FindControlRecursive(FormViewSales, "TextBoxEnrollment");
            TextBox TextBoxBfastIncentive = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBfastIncentive");
            TextBox TextBoxPaidElig = (TextBox)FindControlRecursive(FormViewSales, "TextBoxPaidElig");
            TextBox TextBoxReducedElig = (TextBox)FindControlRecursive(FormViewSales, "TextBoxReducedElig");
            TextBox TextBoxFreeElig = (TextBox)FindControlRecursive(FormViewSales, "TextBoxFreeElig");


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryUpdateSchoolHdr = "UPDATE [DistrictSchoolYearSetup] SET AF=@AF, Enrollment=@Enrollment, EligiblePaid=@EligiblePaid, EligibleFree=@EligibleFree, " +
                                            " EligibleReduced=@EligibleReduced, BreakfastIncent=@BreakfastIncent " +
                                            " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND Schoolnum=@SchoolNum ";

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

                //Updateable Values
                Decimal.TryParse(TextBoxAttFactor.Text.Trim('%'), out Decimal outAF);

                SqlParameter dbParam_AF = new SqlParameter();
                dbParam_AF.ParameterName = "@AF";
                dbParam_AF.Value = outAF * 0.01M;
                dbParam_AF.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_AF);

                SqlParameter dbParam_Enrollment = new SqlParameter();
                dbParam_Enrollment.ParameterName = "@Enrollment";
                dbParam_Enrollment.Value = TextBoxEnrollment.Text;
                dbParam_Enrollment.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_Enrollment);

                SqlParameter dbParam_EligPaid = new SqlParameter();
                dbParam_EligPaid.ParameterName = "@EligiblePaid";
                dbParam_EligPaid.Value = TextBoxPaidElig.Text;
                dbParam_EligPaid.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_EligPaid);

                SqlParameter dbParam_EligFree = new SqlParameter();
                dbParam_EligFree.ParameterName = "@EligibleFree";
                dbParam_EligFree.Value = TextBoxFreeElig.Text;
                dbParam_EligFree.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_EligFree);

                SqlParameter dbParam_EligRed = new SqlParameter();
                dbParam_EligRed.ParameterName = "@EligibleReduced";
                dbParam_EligRed.Value = TextBoxReducedElig.Text;
                dbParam_EligRed.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_EligRed);

                SqlParameter dbParam_BfIncent = new SqlParameter();
                dbParam_BfIncent.ParameterName = "@BreakfastIncent";
                dbParam_BfIncent.Value = TextBoxBfastIncentive.Text;
                dbParam_BfIncent.DbType = System.Data.DbType.Decimal;
                cmd.Parameters.Add(dbParam_BfIncent);

                try
                {
                    dbConnection.Open();
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error saving the school header info (eligibility, enrollment...).');");
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            //Mark hdr as saved
            HFDistSchoolHdrSaved.Value = "1";
        }

        protected void SaveDistSchoolDayData()
        {
            HiddenField HFDistSchoolDaySaved = (HiddenField)FindControlRecursive(FormViewSales, "HFDistSchoolDaySaved");
            if (HFDistSchoolDaySaved.Value == "1")
                return;

            if (DateTime.TryParse(TrxDate.Text, out DateTime UseDate))
            {

                if (UseDate >= (DateTime)Session["ProgramYearStartDate"] && UseDate <= (DateTime)Session["ProgramYearEndDate"])
                { /*Do nothing */
                }
                else
                {
                    RegisterDOMReadyScript("Date Error", "alert('Date falls outside the Program Year. Save Canceled');");
                    return;
                }
            }
            else
            {
                RegisterDOMReadyScript("Date Error", "alert('Error reading date. Save Canceled');");
                return;
            }


            HiddenField HFSchoolNum = (HiddenField)FindControlRecursive(FormViewSales, "HFSchoolNum");


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);


            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryUpdateSchoolHdr = "SELECT COUNT(*) FROM [RecapEntry] " +
                                          " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND Schoolnum=@SchoolNum AND RecapDate=@RecapDate ";

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

                SqlParameter dbParam_RecapDate = new SqlParameter();
                dbParam_RecapDate.ParameterName = "@RecapDate";
                dbParam_RecapDate.Value = TrxDate.Text;
                dbParam_RecapDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_RecapDate);
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
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error saving the school header info (eligibility, enrollment...).');");
                }
                finally
                {
                    dbConnection.Close();
                }

                string InsertUpdateQuery = "";
                if (existingRecordCount == 0)
                {
                    InsertUpdateQuery = "INSERT INTO [RecapEntry] (ProgramYear, DistrictID, SchoolNum, RecapDate, BreakfastDay, LunchDay, BPaidElem, BPaidJr, BPaidSr "
                        + " ,BReduced, BFree, BPaidLrn, BReducedLrn, BFreeLrn, BStuAla, BAdultAla, BAdvanced, BRedeemed, BRepaid, BCharges, BDeposit "
                        + " ,LPaidElem, LPaidJr, LPaidSr, LTier1, LTier2, LReduced, LReducedTier1, LFree, LFreeTier1, LFreeTier2, LPaidLrn, LReducedLrn "
                        + " ,LFreeLrn, LStuAla, LAdultAla, LOtherAla1, LVendAla, LAdvanced, LRedeemed, LRepaid, LCharges "
                        + " ,OPaidSnack ,OReducedSnack ,OFreeSnack ,SpecialFunctionSale ,SpecialFunctionColl ,SpecialFunctionComm "
                        + " ,USDASale ,USDAColl ,OtherSale1 ,OtherColl1 ,OtherSale2 ,OtherColl2 "
                        + " ,OnlineAdvance ,Adjustment ,Deposit ,OtherSale1Cat ,OtherColl1Cat "
                        + " ,EditCheckComments ,RecapComments ,OtherSale2Cat ,OtherColl2Cat "
                        + " ,R_AF, R_Enrollment, R_BreakfastIncent, R_EligiblePaid, R_EligibleReduced, R_EligibleFree ) "
                        + " VALUES (@ProgramYear, @DistrictID, @SchoolNum, @RecapDate, @BreakfastDay, @LunchDay, @BPaidElem, @BPaidJr, @BPaidSr, @BReduced, @BFree, @BPaidLrn "
                        + " ,@BReducedLrn, @BFreeLrn, @BStuAla, @BAdultAla, @BAdvanced, @BRedeemed, @BRepaid, @BCharges, @BDeposit "
                        + " ,@LPaidElem, @LPaidJr, @LPaidSr, @LTier1, @LTier2, @LReduced, @LReducedTier1, @LFree, @LFreeTier1, @LFreeTier2, @LPaidLrn, @LReducedLrn "
                        + " ,@LFreeLrn, @LStuAla, @LAdultAla, @LOtherAla1, @LVendAla, @LAdvanced, @LRedeemed, @LRepaid, @LCharges "
                        + " ,@OPaidSnack ,@OReducedSnack ,@OFreeSnack ,@SpecialFunctionSale ,@SpecialFunctionColl ,@SpecialFunctionComm "
                        + " ,@USDASale ,@USDAColl ,@OtherSale1 ,@OtherColl1 ,@OtherSale2 ,@OtherColl2 "
                        + " ,@OnlineAdvance ,@Adjustment ,@Deposit ,@OtherSale1Cat ,@OtherColl1Cat "
                        + " ,@EditCheckComments ,@RecapComments ,@OtherSale2Cat ,@OtherColl2Cat "
                        + " ,@R_AF, @R_Enrollment, @R_BreakfastIncent, @R_EligiblePaid, @R_EligibleReduced, @R_EligibleFree)";
                }
                else
                {
                    InsertUpdateQuery = "UPDATE [RecapEntry] SET BreakfastDay=@BreakfastDay, LunchDay=@LunchDay, BPaidElem=@BPaidElem, BPaidJr=@BPaidJr, BPaidSr=@BPaidSr "
                        + " ,BReduced=@BReduced, BFree=@BFree, BPaidLrn=@BPaidLrn, BReducedLrn=@BReducedLrn , BFreeLrn=@BFreeLrn, BStuAla=@BStuAla, BAdultAla=@BAdultAla "
                        + " ,BAdvanced=@BAdvanced, BRedeemed=@BRedeemed, BRepaid=@BRepaid, BCharges=@BCharges, BDeposit=@BDeposit "
                        + " ,LPaidElem=@LPaidElem, LPaidJr=@LPaidJr, LPaidSr=@LPaidSr, LTier1=@LTier1, LTier2=@LTier2, LReduced=@LReduced, LReducedTier1=@LReducedTier1 "
                        + " ,LFree=@LFree, LFreeTier1=@LFreeTier1, LFreeTier2=@LFreeTier2, LPaidLrn=@LPaidLrn, LReducedLrn=@LReducedLrn "
                        + " ,LFreeLrn=@LFreeLrn, LStuAla=@LStuAla, LAdultAla=@LAdultAla, LOtherAla1=@LOtherAla1, LVendAla=@LVendAla, LAdvanced=@LAdvanced "
                        + " ,LRedeemed=@LRedeemed, LRepaid=@LRepaid, LCharges=@LCharges ,OPaidSnack=@OPaidSnack ,OReducedSnack=@OReducedSnack "
                        + " ,OFreeSnack=@OFreeSnack ,SpecialFunctionSale=@SpecialFunctionSale ,SpecialFunctionColl=@SpecialFunctionColl ,SpecialFunctionComm=@SpecialFunctionComm "
                        + " ,USDASale=@USDASale ,USDAColl=@USDAColl ,OtherSale1=@OtherSale1 ,OtherColl1=@OtherColl1 ,OtherSale2=@OtherSale2 ,OtherColl2=@OtherColl2 "
                        + " ,OnlineAdvance=@OnlineAdvance ,Adjustment=@Adjustment ,Deposit=@Deposit ,OtherSale1Cat=@OtherSale1Cat ,OtherColl1Cat=@OtherColl1Cat "
                        + " ,EditCheckComments=@EditCheckComments ,RecapComments=@RecapComments ,OtherSale2Cat=@OtherSale2Cat ,OtherColl2Cat=@OtherColl2Cat "
                        + " ,R_AF=@R_AF, R_Enrollment=@R_Enrollment, R_BreakfastIncent=@R_BreakfastIncent "
                        + " ,R_EligiblePaid=@R_EligiblePaid, R_EligibleReduced=@R_EligibleReduced, R_EligibleFree=@R_EligibleFree "
                        + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND RecapDate=@RecapDate ";
                }

                #region StoredHdrValues
                TextBox TextBoxAttFactor = (TextBox)FindControlRecursive(FormViewSales, "TextBoxAttFactor");
                TextBox TextBoxEnrollment = (TextBox)FindControlRecursive(FormViewSales, "TextBoxEnrollment");
                TextBox TextBoxBfastIncentive = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBfastIncentive");
                TextBox TextBoxPaidElig = (TextBox)FindControlRecursive(FormViewSales, "TextBoxPaidElig");
                TextBox TextBoxReducedElig = (TextBox)FindControlRecursive(FormViewSales, "TextBoxReducedElig");
                TextBox TextBoxFreeElig = (TextBox)FindControlRecursive(FormViewSales, "TextBoxFreeElig");
                #endregion
                #region BreakfastControls
                CheckBox CBBreakfast = (CheckBox)FindControlRecursive(FormViewSales, "CBBreakfast");
                CheckBox CheckBoxLunch = (CheckBox)FindControlRecursive(FormViewSales, "CheckBoxLunch");
                TextBox TextBoxBPaidElem = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBPaidElem");
                TextBox TextBoxBPaidJr = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBPaidJr");
                TextBox TextBoxBPaidSr = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBPaidSr");
                TextBox TextBoxBReduced = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBReduced");
                TextBox TextBoxBFree = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBFree");
                TextBox TextBoxBPaidLrn = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBPaidLrn");
                TextBox TextBoxBReducedLrn = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBReducedLrn");
                TextBox TextBoxBFreeLrn = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBFreeLrn");
                TextBox TextBoxBStuAla = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBStuAla");
                TextBox TextBoxBAdltAla = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBAdltAla");
                TextBox TextBoxBAdvanced = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBAdvanced");
                TextBox TextBoxBRedeemed = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBRedeemed");
                TextBox TextBoxBRepaid = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBRepaid");
                TextBox TextBoxBCharges = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBCharges");
                TextBox TextBoxBDeposit = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBDeposit");
                #endregion
                #region LunchControls
                TextBox TextBoxLPaidElem = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLPaidElem");
                TextBox TextBoxLPaidJr = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLPaidJr");
                TextBox TextBoxLPaidSr = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLPaidSr");
                TextBox TextBoxLTier1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLTier1");
                TextBox TextBoxLTier2 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLTier2");
                TextBox TextBoxLReduced = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLReduced");
                TextBox TextBoxLReducedT1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLReducedT1");
                TextBox TextBoxLFree = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLFree");
                TextBox TextBoxLFreeT1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLFreeT1");
                TextBox TextBoxLFreeT2 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLFreeT2");
                TextBox TextBoxLPaidLrn = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLPaidLrn");
                TextBox TextBoxLReducedLrn = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLReducedLrn");
                TextBox TextBoxLFreeLrn = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLFreeLrn");
                TextBox TextBoxLStuAla = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLStuAla");
                TextBox TextBoxLAdltAla = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLAdltAla");
                TextBox TextBoxLOtherAla1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLOtherAla1");
                TextBox TextBoxLVendAla = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLVendAla");
                TextBox TextBoxLAdvanced = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLAdvanced");
                TextBox TextBoxLRedeemed = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLRedeemed");
                TextBox TextBoxLRepaid = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLRepaid");
                TextBox TextBoxLCharges = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLCharges");
                #endregion
                #region OtherControls
                TextBox TextBoxOSnack = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOSnack");
                TextBox TextBoxOReducedSnack = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOReducedSnack");
                TextBox TextBoxOFreeSnack = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOFreeSnack");
                TextBox TextBoxOSFSale = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOSFSale");
                TextBox TextBoxOSFCollect = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOSFCollect");
                TextBox TextBoxOVendComm = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOVendComm");
                TextBox TextBoxUSDASale = (TextBox)FindControlRecursive(FormViewSales, "TextBoxUSDASale");
                TextBox TextBoxUSDAColl = (TextBox)FindControlRecursive(FormViewSales, "TextBoxUSDAColl");
                TextBox TextBoxOSale1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOSale1");
                TextBox TextBoxOColl1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOColl1");
                TextBox TextBoxOSale2 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOSale2");
                TextBox TextBoxOColl2 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOColl2");
                DropDownList DDLSale1Cats = (DropDownList)FindControlRecursive(FormViewSales, "DDLSale1Cats");
                DropDownList DDLColl1Cats = (DropDownList)FindControlRecursive(FormViewSales, "DDLColl1Cats");
                DropDownList DDLSale2Cats = (DropDownList)FindControlRecursive(FormViewSales, "DDLSale2Cats");
                DropDownList DDLColl2Cats = (DropDownList)FindControlRecursive(FormViewSales, "DDLColl2Cats");
                TextBox TextBoxOnlineAdv = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOnlineAdv");
                TextBox TextBoxAdjustment = (TextBox)FindControlRecursive(FormViewSales, "TextBoxAdjustment");
                TextBox TextBoxLDeposit = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLDeposit");
                TextBox TextBoxEditCheckCmt = (TextBox)FindControlRecursive(FormViewSales, "TextBoxEditCheckCmt");
                TextBox TextBoxRecapCmt = (TextBox)FindControlRecursive(FormViewSales, "TextBoxRecapCmt");
                #endregion

                SqlConnection dbConnection2 = new SqlConnection(connectionString);
                using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection2))
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

                    SqlParameter dbParam_RecapDate2 = new SqlParameter();
                    dbParam_RecapDate2.ParameterName = "@RecapDate";
                    dbParam_RecapDate2.Value = TrxDate.Text;
                    dbParam_RecapDate2.DbType = System.Data.DbType.Date;
                    cmd2.Parameters.Add(dbParam_RecapDate2);
                    #endregion

                    #region Checkboxes
                    //Updateable Values
                    SqlParameter dbParam_bDay = new SqlParameter();
                    dbParam_bDay.ParameterName = "@BreakfastDay";
                    dbParam_bDay.Value = CBBreakfast.Checked ? 1 : 0;
                    dbParam_bDay.DbType = System.Data.DbType.Byte;
                    cmd2.Parameters.Add(dbParam_bDay);

                    SqlParameter dbParam_lDay = new SqlParameter();
                    dbParam_lDay.ParameterName = "@LunchDay";
                    dbParam_lDay.Value = CheckBoxLunch.Checked ? 1 : 0;
                    dbParam_lDay.DbType = System.Data.DbType.Byte;
                    cmd2.Parameters.Add(dbParam_lDay);
                    #endregion

                    #region Breakfast
                    SqlParameter dbParam_bPaidElem = new SqlParameter();
                    dbParam_bPaidElem.ParameterName = "@BPaidElem";
                    dbParam_bPaidElem.Value = TextBoxBPaidElem.Text;
                    dbParam_bPaidElem.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_bPaidElem);

                    SqlParameter dbParam_bPaidJr = new SqlParameter();
                    dbParam_bPaidJr.ParameterName = "@BPaidJr";
                    dbParam_bPaidJr.Value = TextBoxBPaidJr.Text;
                    dbParam_bPaidJr.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_bPaidJr);

                    SqlParameter dbParam_bPaidSr = new SqlParameter();
                    dbParam_bPaidSr.ParameterName = "@BPaidSr";
                    dbParam_bPaidSr.Value = TextBoxBPaidSr.Text;
                    dbParam_bPaidSr.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_bPaidSr);

                    SqlParameter dbParam_bReduced = new SqlParameter();
                    dbParam_bReduced.ParameterName = "@BReduced";
                    dbParam_bReduced.Value = TextBoxBReduced.Text;
                    dbParam_bReduced.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_bReduced);

                    SqlParameter dbParam_bFree = new SqlParameter();
                    dbParam_bFree.ParameterName = "@BFree";
                    dbParam_bFree.Value = TextBoxBFree.Text;
                    dbParam_bFree.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_bFree);

                    SqlParameter dbParam_bPaidLrn = new SqlParameter();
                    dbParam_bPaidLrn.ParameterName = "@BPaidLrn";
                    dbParam_bPaidLrn.Value = TextBoxBPaidLrn.Text;
                    dbParam_bPaidLrn.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_bPaidLrn);

                    SqlParameter dbParam_bReducedLrn = new SqlParameter();
                    dbParam_bReducedLrn.ParameterName = "@BReducedLrn";
                    dbParam_bReducedLrn.Value = TextBoxBReducedLrn.Text;
                    dbParam_bReducedLrn.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_bReducedLrn);

                    SqlParameter dbParam_bFreeLrn = new SqlParameter();
                    dbParam_bFreeLrn.ParameterName = "@BFreeLrn";
                    dbParam_bFreeLrn.Value = TextBoxBFreeLrn.Text;
                    dbParam_bFreeLrn.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_bFreeLrn);

                    SqlParameter dbParam_bStuAla = new SqlParameter();
                    dbParam_bStuAla.ParameterName = "@BStuAla";
                    dbParam_bStuAla.Value = TextBoxBStuAla.Text;
                    dbParam_bStuAla.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_bStuAla);

                    SqlParameter dbParam_bAdultAla = new SqlParameter();
                    dbParam_bAdultAla.ParameterName = "@BAdultAla";
                    dbParam_bAdultAla.Value = TextBoxBAdltAla.Text;
                    dbParam_bAdultAla.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_bAdultAla);

                    SqlParameter dbParam_bAdvanced = new SqlParameter();
                    dbParam_bAdvanced.ParameterName = "@BAdvanced";
                    dbParam_bAdvanced.Value = TextBoxBAdvanced.Text;
                    dbParam_bAdvanced.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_bAdvanced);

                    SqlParameter dbParam_bRedeemed = new SqlParameter();
                    dbParam_bRedeemed.ParameterName = "@BRedeemed";
                    dbParam_bRedeemed.Value = TextBoxBRedeemed.Text;
                    dbParam_bRedeemed.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_bRedeemed);

                    SqlParameter dbParam_bRepaid = new SqlParameter();
                    dbParam_bRepaid.ParameterName = "@BRepaid";
                    dbParam_bRepaid.Value = TextBoxBRepaid.Text;
                    dbParam_bRepaid.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_bRepaid);

                    SqlParameter dbParam_bCharges = new SqlParameter();
                    dbParam_bCharges.ParameterName = "@BCharges";
                    dbParam_bCharges.Value = TextBoxBCharges.Text;
                    dbParam_bCharges.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_bCharges);

                    SqlParameter dbParam_bDeposit = new SqlParameter();
                    dbParam_bDeposit.ParameterName = "@BDeposit";
                    dbParam_bDeposit.Value = TextBoxBDeposit.Text;
                    dbParam_bDeposit.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_bDeposit);
                    #endregion
                    #region Lunch
                    SqlParameter dbParam_lPaidElem = new SqlParameter();
                    dbParam_lPaidElem.ParameterName = "@LPaidElem";
                    dbParam_lPaidElem.Value = TextBoxLPaidElem.Text;
                    dbParam_lPaidElem.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lPaidElem);

                    SqlParameter dbParam_lPaidJr = new SqlParameter();
                    dbParam_lPaidJr.ParameterName = "@LPaidJr";
                    dbParam_lPaidJr.Value = TextBoxLPaidJr.Text;
                    dbParam_lPaidJr.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lPaidJr);

                    SqlParameter dbParam_lPaidSr = new SqlParameter();
                    dbParam_lPaidSr.ParameterName = "@LPaidSr";
                    dbParam_lPaidSr.Value = TextBoxLPaidSr.Text;
                    dbParam_lPaidSr.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lPaidSr);

                    SqlParameter dbParam_lTier1 = new SqlParameter();
                    dbParam_lTier1.ParameterName = "@LTier1";
                    dbParam_lTier1.Value = TextBoxLTier1.Text;
                    dbParam_lTier1.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lTier1);

                    SqlParameter dbParam_lTier2 = new SqlParameter();
                    dbParam_lTier2.ParameterName = "@LTier2";
                    dbParam_lTier2.Value = TextBoxLTier2.Text;
                    dbParam_lTier2.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lTier2);

                    SqlParameter dbParam_lReduced = new SqlParameter();
                    dbParam_lReduced.ParameterName = "@LReduced";
                    dbParam_lReduced.Value = TextBoxLReduced.Text;
                    dbParam_lReduced.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lReduced);

                    SqlParameter dbParam_lReducedTier1 = new SqlParameter();
                    dbParam_lReducedTier1.ParameterName = "@LReducedTier1";
                    dbParam_lReducedTier1.Value = TextBoxLReducedT1.Text;
                    dbParam_lReducedTier1.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lReducedTier1);

                    SqlParameter dbParam_lFree = new SqlParameter();
                    dbParam_lFree.ParameterName = "@LFree";
                    dbParam_lFree.Value = TextBoxLFree.Text;
                    dbParam_lFree.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lFree);

                    SqlParameter dbParam_lFreeTier1 = new SqlParameter();
                    dbParam_lFreeTier1.ParameterName = "@LFreeTier1";
                    dbParam_lFreeTier1.Value = TextBoxLFreeT1.Text;
                    dbParam_lFreeTier1.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lFreeTier1);

                    SqlParameter dbParam_lFreeTier2 = new SqlParameter();
                    dbParam_lFreeTier2.ParameterName = "@LFreeTier2";
                    dbParam_lFreeTier2.Value = TextBoxLFreeT2.Text;
                    dbParam_lFreeTier2.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lFreeTier2);

                    SqlParameter dbParam_lPaidLrn = new SqlParameter();
                    dbParam_lPaidLrn.ParameterName = "@LPaidLrn";
                    dbParam_lPaidLrn.Value = TextBoxLPaidLrn.Text;
                    dbParam_lPaidLrn.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lPaidLrn);

                    SqlParameter dbParam_lReducedLrn = new SqlParameter();
                    dbParam_lReducedLrn.ParameterName = "@LReducedLrn";
                    dbParam_lReducedLrn.Value = TextBoxLReducedLrn.Text;
                    dbParam_lReducedLrn.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lReducedLrn);

                    SqlParameter dbParam_lFreeLrn = new SqlParameter();
                    dbParam_lFreeLrn.ParameterName = "@LFreeLrn";
                    dbParam_lFreeLrn.Value = TextBoxLFreeLrn.Text;
                    dbParam_lFreeLrn.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_lFreeLrn);

                    SqlParameter dbParam_lStuAla = new SqlParameter();
                    dbParam_lStuAla.ParameterName = "@LStuAla";
                    dbParam_lStuAla.Value = TextBoxLStuAla.Text;
                    dbParam_lStuAla.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_lStuAla);

                    SqlParameter dbParam_lAdultAla = new SqlParameter();
                    dbParam_lAdultAla.ParameterName = "@LAdultAla";
                    dbParam_lAdultAla.Value = TextBoxLAdltAla.Text;
                    dbParam_lAdultAla.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_lAdultAla);

                    SqlParameter dbParam_lOtherAla1 = new SqlParameter();
                    dbParam_lOtherAla1.ParameterName = "@LOtherAla1";
                    dbParam_lOtherAla1.Value = TextBoxLOtherAla1.Text;
                    dbParam_lOtherAla1.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_lOtherAla1);

                    SqlParameter dbParam_lVendAla = new SqlParameter();
                    dbParam_lVendAla.ParameterName = "@LVendAla";
                    dbParam_lVendAla.Value = TextBoxLVendAla.Text;
                    dbParam_lVendAla.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_lVendAla);

                    SqlParameter dbParam_lAdvanced = new SqlParameter();
                    dbParam_lAdvanced.ParameterName = "@LAdvanced";
                    dbParam_lAdvanced.Value = TextBoxLAdvanced.Text;
                    dbParam_lAdvanced.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_lAdvanced);

                    SqlParameter dbParam_lRedeemed = new SqlParameter();
                    dbParam_lRedeemed.ParameterName = "@LRedeemed";
                    dbParam_lRedeemed.Value = TextBoxLRedeemed.Text;
                    dbParam_lRedeemed.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_lRedeemed);

                    SqlParameter dbParam_lRepaid = new SqlParameter();
                    dbParam_lRepaid.ParameterName = "@LRepaid";
                    dbParam_lRepaid.Value = TextBoxLRepaid.Text;
                    dbParam_lRepaid.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_lRepaid);

                    SqlParameter dbParam_lCharges = new SqlParameter();
                    dbParam_lCharges.ParameterName = "@LCharges";
                    dbParam_lCharges.Value = TextBoxLCharges.Text;
                    dbParam_lCharges.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_lCharges);
                    #endregion
                    #region Other
                    SqlParameter dbParam_oPaidSnack = new SqlParameter();
                    dbParam_oPaidSnack.ParameterName = "@OPaidSnack";
                    dbParam_oPaidSnack.Value = TextBoxOSnack.Text;
                    dbParam_oPaidSnack.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_oPaidSnack);

                    SqlParameter dbParam_oReducedSnack = new SqlParameter();
                    dbParam_oReducedSnack.ParameterName = "@OReducedSnack";
                    dbParam_oReducedSnack.Value = TextBoxOReducedSnack.Text;
                    dbParam_oReducedSnack.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_oReducedSnack);

                    SqlParameter dbParam_oFreeSnack = new SqlParameter();
                    dbParam_oFreeSnack.ParameterName = "@OFreeSnack";
                    dbParam_oFreeSnack.Value = TextBoxOFreeSnack.Text;
                    dbParam_oFreeSnack.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_oFreeSnack);

                    SqlParameter dbParam_oSFSale = new SqlParameter();
                    dbParam_oSFSale.ParameterName = "@SpecialFunctionSale";
                    dbParam_oSFSale.Value = TextBoxOSFSale.Text;
                    dbParam_oSFSale.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oSFSale);

                    SqlParameter dbParam_oSFColl = new SqlParameter();
                    dbParam_oSFColl.ParameterName = "@SpecialFunctionColl";
                    dbParam_oSFColl.Value = TextBoxOSFCollect.Text;
                    dbParam_oSFColl.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oSFColl);

                    SqlParameter dbParam_oVendComm = new SqlParameter();
                    dbParam_oVendComm.ParameterName = "@SpecialFunctionComm";
                    dbParam_oVendComm.Value = TextBoxOVendComm.Text;
                    dbParam_oVendComm.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oVendComm);

                    SqlParameter dbParam_oUSDASale = new SqlParameter();
                    dbParam_oUSDASale.ParameterName = "@USDASale";
                    dbParam_oUSDASale.Value = TextBoxUSDASale.Text;
                    dbParam_oUSDASale.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oUSDASale);

                    SqlParameter dbParam_oUSDAColl = new SqlParameter();
                    dbParam_oUSDAColl.ParameterName = "@USDAColl";
                    dbParam_oUSDAColl.Value = TextBoxUSDAColl.Text;
                    dbParam_oUSDAColl.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oUSDAColl);

                    SqlParameter dbParam_oSale1 = new SqlParameter();
                    dbParam_oSale1.ParameterName = "@OtherSale1";
                    dbParam_oSale1.Value = TextBoxOSale1.Text;
                    dbParam_oSale1.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oSale1);

                    SqlParameter dbParam_oColl1 = new SqlParameter();
                    dbParam_oColl1.ParameterName = "@OtherColl1";
                    dbParam_oColl1.Value = TextBoxOColl1.Text;
                    dbParam_oColl1.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oColl1);

                    SqlParameter dbParam_oSale2 = new SqlParameter();
                    dbParam_oSale2.ParameterName = "@OtherSale2";
                    dbParam_oSale2.Value = TextBoxOSale2.Text;
                    dbParam_oSale2.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oSale2);

                    SqlParameter dbParam_oColl2 = new SqlParameter();
                    dbParam_oColl2.ParameterName = "@OtherColl2";
                    dbParam_oColl2.Value = TextBoxOColl2.Text;
                    dbParam_oColl2.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oColl2);

                    SqlParameter dbParam_oSale1Cat = new SqlParameter();
                    dbParam_oSale1Cat.ParameterName = "@OtherSale1Cat";
                    dbParam_oSale1Cat.Value = DDLSale1Cats.SelectedValue;
                    dbParam_oSale1Cat.DbType = System.Data.DbType.Int16;
                    cmd2.Parameters.Add(dbParam_oSale1Cat);

                    SqlParameter dbParam_oColl1Cat = new SqlParameter();
                    dbParam_oColl1Cat.ParameterName = "@OtherColl1Cat";
                    dbParam_oColl1Cat.Value = DDLColl1Cats.SelectedValue;
                    dbParam_oColl1Cat.DbType = System.Data.DbType.Int16;
                    cmd2.Parameters.Add(dbParam_oColl1Cat);

                    SqlParameter dbParam_oSale2Cat = new SqlParameter();
                    dbParam_oSale2Cat.ParameterName = "@OtherSale2Cat";
                    dbParam_oSale2Cat.Value = DDLSale2Cats.SelectedValue;
                    dbParam_oSale2Cat.DbType = System.Data.DbType.Int16;
                    cmd2.Parameters.Add(dbParam_oSale2Cat);

                    SqlParameter dbParam_oColl2Cat = new SqlParameter();
                    dbParam_oColl2Cat.ParameterName = "@OtherColl2Cat";
                    dbParam_oColl2Cat.Value = DDLColl2Cats.SelectedValue;
                    dbParam_oColl2Cat.DbType = System.Data.DbType.Int16;
                    cmd2.Parameters.Add(dbParam_oColl2Cat);

                    SqlParameter dbParam_oOnlineAdv = new SqlParameter();
                    dbParam_oOnlineAdv.ParameterName = "@OnlineAdvance";
                    dbParam_oOnlineAdv.Value = TextBoxOnlineAdv.Text;
                    dbParam_oOnlineAdv.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oOnlineAdv);

                    SqlParameter dbParam_oAdjustment = new SqlParameter();
                    dbParam_oAdjustment.ParameterName = "@Adjustment";
                    dbParam_oAdjustment.Value = TextBoxAdjustment.Text;
                    dbParam_oAdjustment.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oAdjustment);

                    SqlParameter dbParam_oDeposit = new SqlParameter();
                    dbParam_oDeposit.ParameterName = "@Deposit";
                    dbParam_oDeposit.Value = TextBoxLDeposit.Text;
                    dbParam_oDeposit.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_oDeposit);

                    SqlParameter dbParam_oEditCheckCmt = new SqlParameter();
                    dbParam_oEditCheckCmt.ParameterName = "@EditCheckComments";
                    dbParam_oEditCheckCmt.Value = TextBoxEditCheckCmt.Text;
                    dbParam_oEditCheckCmt.DbType = System.Data.DbType.String;
                    cmd2.Parameters.Add(dbParam_oEditCheckCmt);

                    SqlParameter dbParam_oRecapCmt = new SqlParameter();
                    dbParam_oRecapCmt.ParameterName = "@RecapComments";
                    dbParam_oRecapCmt.Value = TextBoxRecapCmt.Text;
                    dbParam_oRecapCmt.DbType = System.Data.DbType.String;
                    cmd2.Parameters.Add(dbParam_oRecapCmt);
                    #endregion
                    #region StoredHdrValues
                    SqlParameter dbParam_AF = new SqlParameter();
                    dbParam_AF.ParameterName = "@R_AF";
                    dbParam_AF.Value = Decimal.Parse(TextBoxAttFactor.Text.Trim('%')) * 0.01M;
                    dbParam_AF.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_AF);

                    SqlParameter dbParam_Enrollment = new SqlParameter();
                    dbParam_Enrollment.ParameterName = "@R_Enrollment";
                    dbParam_Enrollment.Value = TextBoxEnrollment.Text;
                    dbParam_Enrollment.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_Enrollment);

                    SqlParameter dbParam_EligPaid = new SqlParameter();
                    dbParam_EligPaid.ParameterName = "@R_EligiblePaid";
                    dbParam_EligPaid.Value = TextBoxPaidElig.Text;
                    dbParam_EligPaid.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_EligPaid);

                    SqlParameter dbParam_EligFree = new SqlParameter();
                    dbParam_EligFree.ParameterName = "@R_EligibleFree";
                    dbParam_EligFree.Value = TextBoxFreeElig.Text;
                    dbParam_EligFree.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_EligFree);

                    SqlParameter dbParam_EligRed = new SqlParameter();
                    dbParam_EligRed.ParameterName = "@R_EligibleReduced";
                    dbParam_EligRed.Value = TextBoxReducedElig.Text;
                    dbParam_EligRed.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_EligRed);

                    SqlParameter dbParam_BfIncent = new SqlParameter();
                    dbParam_BfIncent.ParameterName = "@R_BreakfastIncent";
                    dbParam_BfIncent.Value = TextBoxBfastIncentive.Text;
                    dbParam_BfIncent.DbType = System.Data.DbType.Decimal;
                    cmd2.Parameters.Add(dbParam_BfIncent);
                    #endregion

                    try
                    {
                        dbConnection2.Open();
                        cmd2.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        RegisterDOMReadyScript("error message", "alert('Error saving the school sales info.');");
                    }
                    finally
                    {
                        dbConnection2.Close();
                    }
                }
            }

            //Mark hdr as saved
            HFDistSchoolDaySaved.Value = "1";
        }

        protected void TextBoxSchoolHeader_TextChanged(object sender, EventArgs e)
        {
            SaveDistSchoolHdrData();
            //Hdr data is stored in the Sales entry, so the Sales needs to be saved too
            SaveDistSchoolDayData();
        }

        protected void SalesEntryDataChanged(object sender, EventArgs e)
        {
            SaveDistSchoolDayData();
        }

        /// <summary>
        /// Prerender runs after the control postback scripts are run. This means all saves have occurred at this point,
        /// Reset the hidden field indicators to allow future changes to be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FormViewSales_PreRender(object sender, EventArgs e)
        {
            HiddenField HFDistSchoolDaySaved = (HiddenField)FindControlRecursive(FormViewSales, "HFDistSchoolDaySaved");
            HiddenField HFDistSchoolHdrSaved = (HiddenField)FindControlRecursive(FormViewSales, "HFDistSchoolHdrSaved");

            if(HFDistSchoolDaySaved!=null)
                HFDistSchoolDaySaved.Value = "0";
            if(HFDistSchoolHdrSaved!=null)
                HFDistSchoolHdrSaved.Value = "0";

            //When saving the Totals panel is reset. Update the totals because the form view databind is not going to be called
            RegisterDOMReadyScript("Page Load Recalculations", "RecalcAll();");
        }

        protected void FormViewSales_DataBound(object sender, EventArgs e)
        {
            //Call Jquery which populates the totals
            RegisterDOMReadyScript("Page Load Recalculations", "RecalcAll();");
        }

        protected void TrxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox dateTextBox = (TextBox)sender;

            // If just digits are typed - convert to standard date
            NormalizeSimplifiedDate(ref dateTextBox);
            // Ensure the date falls in the Program Year. Validate weekend entries.
            ValidateTrxEntryDate(ref dateTextBox, true);
         
        }

        /// <summary>
        /// The following creates an array of entered dates which is then used to color the calendar with entered days
        /// </summary>
        protected void PopulateSchoolDates()
        {
            //clear out the hidden fields entry
            HFEnteredSchoolDaysArray.Value = string.Empty;
            HFLockedSchoolDaysArray.Value = string.Empty;

            StringBuilder sbEntered = new StringBuilder();
            StringBuilder sbLocked = new StringBuilder();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT DISTINCT RecapDate, Locked FROM [RecapEntry] "
                + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND RecapDate>=@StartDate AND RecapDate<=@EndDate ";


            if (DropDownListDistricts.Items.Count == 0)
                return;

            if (DropDownListSchools.Items.Count == 0)
                return;

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
            dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
            dbCommand.Parameters.Add(dbParam_ProgYear);

            SqlParameter dbParam_DistID = new SqlParameter();
            dbParam_DistID.ParameterName = "@DistrictID";
            dbParam_DistID.Value = DropDownListDistricts.SelectedValue;
            dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
            dbCommand.Parameters.Add(dbParam_DistID);

            SqlParameter dbParam_SchoolNum = new SqlParameter();
            dbParam_SchoolNum.ParameterName = "@SchoolNum";
            dbParam_SchoolNum.Value = DropDownListSchools.SelectedValue;
            dbParam_SchoolNum.DbType = System.Data.DbType.Int16;
            dbCommand.Parameters.Add(dbParam_SchoolNum);

            SqlParameter dbParam_StartDate = new SqlParameter();
            dbParam_StartDate.ParameterName = "@StartDate";
            dbParam_StartDate.Value = (DateTime)Session["ProgramYearStartDate"];
            dbParam_StartDate.DbType = System.Data.DbType.Date;
            dbCommand.Parameters.Add(dbParam_StartDate);

            SqlParameter dbParam_EndDate = new SqlParameter();
            dbParam_EndDate.ParameterName = "@EndDate";
            dbParam_EndDate.Value = (DateTime)Session["ProgramYearEndDate"];
            dbParam_EndDate.DbType = System.Data.DbType.Date;
            dbCommand.Parameters.Add(dbParam_EndDate);

            try
            {
                dbConnection.Open();
                SqlDataReader dataReader = dbCommand.ExecuteReader();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                while(dataReader.Read())
                {
                    if ((bool)dataReader["Locked"] )
                    {
                        if (sbLocked.Length == 0)
                            sbLocked.Append(((DateTime)dataReader["RecapDate"]).ToShortDateString());
                        else
                        {
                            sbLocked.Append(",");
                            sbLocked.Append(((DateTime)dataReader["RecapDate"]).ToShortDateString());
                        }
                    }
                    else
                    {
                        if (sbEntered.Length == 0)
                            sbEntered.Append(((DateTime)dataReader["RecapDate"]).ToShortDateString());
                        else
                        {
                            sbEntered.Append(",");
                            sbEntered.Append(((DateTime)dataReader["RecapDate"]).ToShortDateString());
                        }
                    }

                }


                //if (dataReader.Read())
                //{
                //    //record the first entry
                //    sbEntered.Append(((DateTime)dataReader["RecapDate"]).ToShortDateString());
                //}

                //while (dataReader.Read())
                //{
                //    sbEntered.Append(",");
                //    sbEntered.Append(((DateTime)dataReader["RecapDate"]).ToShortDateString());
                //}

                HFEnteredSchoolDaysArray.Value = sbEntered.ToString();
                HFLockedSchoolDaysArray.Value = sbLocked.ToString();

                if (dataReader != null)
                    dataReader.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                dbConnection.Close();
            }

        }

        protected void DropDownListSchools_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSchoolDates();
        }
    }
}
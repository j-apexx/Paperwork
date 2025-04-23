using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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

            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;//Set the ProgramYear parameter


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

                //Update HiddenFields which are used to constrain the DatePickers and set BudgetYear
                DDLProgramYears_SelectedIndexChanged(null, null);

            }
            else
            {

            }
        }


        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (DDLProgramYears.Items.Count == 0)
                e.Command.Parameters["@ProgramYear"].Value = "";
            else
                e.Command.Parameters["@ProgramYear"].Value = DDLProgramYears.SelectedValue;
        }

        protected void NavNextSchool(object sender, EventArgs e)
        {
            if (DropDownListSchools.Items.Count == 0)
                return;

            if (DropDownListSchools.SelectedIndex == DropDownListSchools.Items.Count - 1)
                DropDownListSchools.SelectedIndex = 0;
            else
                DropDownListSchools.SelectedIndex = (DropDownListSchools.SelectedIndex + 1);

            //Update Calendars, reset revision, focus enrollment
            DropDownListSchools_SelectedIndexChanged(null, null);


        }
        protected void NavPrevSchool(object sender, EventArgs e)
        {
            if (DropDownListSchools.Items.Count == 0)
                return;

            if (DropDownListSchools.SelectedIndex == 0)
                DropDownListSchools.SelectedIndex = DropDownListSchools.Items.Count - 1;
            else
                DropDownListSchools.SelectedIndex = (DropDownListSchools.SelectedIndex - 1);

            //Update Calendars, reset revision, focus enrollment
            DropDownListSchools_SelectedIndexChanged(null, null);
        }

        protected void NavNextDay(object sender, EventArgs e)
        {
            if (DateTime.TryParse(TrxDate.Text, out DateTime CurrentDate) == false)
                return;

            //Update the default user date
            DateTime UseDate = CurrentDate.AddDays(1);

            DateTime stDate;
            DateTime enDate;

            //Constrain the new date to the program year
            if (DateTime.TryParse(HFProgYearStartDate.Value, out stDate) && DateTime.TryParse(HFProgYearEndDate.Value, out enDate))
            {
                if (UseDate >= stDate && UseDate <= enDate)
                { /*Do nothing */}
                else if (UseDate < stDate)
                    UseDate = stDate;
                else if (UseDate > enDate)
                    UseDate = enDate;
            }

            TrxDate.Text = UseDate.ToShortDateString();

            //reset revisions
            HFRevision.Value = "0";
        }
        protected void NavPrevDay(object sender, EventArgs e)
        {
            if (DateTime.TryParse(TrxDate.Text, out DateTime CurrentDate) == false)
                return;

            //Update the default user date
            DateTime UseDate = CurrentDate.AddDays(-1);

            DateTime stDate;
            DateTime enDate;

            //Constrain the new date to the program year
            if (DateTime.TryParse(HFProgYearStartDate.Value, out stDate) && DateTime.TryParse(HFProgYearEndDate.Value, out enDate))
            {
                if (UseDate >= stDate && UseDate <= enDate)
                { /*Do nothing */}
                else if (UseDate < stDate)
                    UseDate = stDate;
                else if (UseDate > enDate)
                    UseDate = enDate;
            }

            TrxDate.Text = UseDate.ToShortDateString();
            
            //reset revisions
            HFRevision.Value = "0";
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                this.Master.MstrHdrUpdatePanel.Update();

                //Response.Redirect(Request.RawUrl);//Request.RawUrl provides the current page URL

                //Update schools and school days
                DropDownListSchools.DataBind();
                if (DropDownListSchools.Items.Count == 0)
                    Response.Redirect("~/Setup/SetupSchools.aspx");

                DropDownListSchools_SelectedIndexChanged(null, null);
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

            //KEY VALUES
            String KeyPYear = FormViewSales.DataKey.Values["ProgramYear"].ToString();
            String KeyDistID = FormViewSales.DataKey.Values["DistrictID"].ToString();

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
                dbParam_ProgYear.Value = KeyPYear;
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
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
                    int recordCount=0;
                    recordCount=cmd.ExecuteNonQuery();
                    //if(recordCount>0)
                    //    ShowMessageIcon("Saved", MessageType.Success);

                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error saving the school header info (eligibility, enrollment...).');");
                    ShowMessageIcon("Error", MessageType.Error);
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
            HiddenField HFEditCheckDate = (HiddenField)FindControlRecursive(FormViewSales, "HFEditCheckDate");
            
            //KEY VALUES
            String KeyPYear = FormViewSales.DataKey.Values["ProgramYear"].ToString();
            String KeyDistID = FormViewSales.DataKey.Values["DistrictID"].ToString();

            //If the delete button was clicked after making some changes, the following flag will be set and this will prevent the save from running unnecessarily
            if (HFDeleting.Value == "1")
                return;


            if (int.TryParse(HFRevision.Value, out int RevisionInt) == false)
                RevisionInt = 0;
            
            //If record has already been saved, simply return. This can be triggered by changes to each field.
            if (HFDistSchoolDaySaved.Value == "1")
                return;

            //If date was changed before saving, the record was being saved to the new date. The following change is meant to correct this
            //if (DateTime.TryParse(TrxDate.Text, out DateTime UseDate))
            if (DateTime.TryParse(HFEditCheckDate.Value, out DateTime UseDate))
            {
                DateTime stDate;
                DateTime enDate;
                if (DateTime.TryParse(HFProgYearStartDate.Value, out stDate) && DateTime.TryParse(HFProgYearEndDate.Value, out enDate))
                {
                    if (UseDate >= stDate && UseDate <= enDate)
                    { /* Date falls within the program year and is okay. Do nothing. */
                    }
                    else
                    {
                        RegisterDOMReadyScript("Date Error", "alert('Date falls outside the Program Year. Save Canceled');");
                        return;
                    }
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
                    " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND Schoolnum=@SchoolNum AND RecapDate=@RecapDate AND Revision=@Revision ";

            using (SqlCommand cmd = new SqlCommand(queryUpdateSchoolHdr, dbConnection))
            {

                #region KeyVals
                //KEY Values
                

                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = KeyPYear; 
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_SchlNum = new SqlParameter();
                dbParam_SchlNum.ParameterName = "@SchoolNum";
                dbParam_SchlNum.Value = HFSchoolNum.Value;
                dbParam_SchlNum.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_SchlNum);

                SqlParameter dbParam_RecapDate = new SqlParameter();
                dbParam_RecapDate.ParameterName = "@RecapDate";
                if (HFRevision.Value == "0")
                    dbParam_RecapDate.Value = HFEditCheckDate.Value;// TrxDate.Text;
                else
                    dbParam_RecapDate.Value = HFRevRecapDate.Value;
                dbParam_RecapDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_RecapDate);

                SqlParameter dbParam_revision = new SqlParameter();
                dbParam_revision.ParameterName = "@Revision";
                dbParam_revision.Value = HFRevision.Value;
                dbParam_revision.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_revision);
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
                        + " ,R_AF, R_Enrollment, R_BreakfastIncent, R_EligiblePaid, R_EligibleReduced, R_EligibleFree, Revision, EditCheckDate ) "
                        + " VALUES (@ProgramYear, @DistrictID, @SchoolNum, @RecapDate, @BreakfastDay, @LunchDay, @BPaidElem, @BPaidJr, @BPaidSr, @BReduced, @BFree, @BPaidLrn "
                        + " ,@BReducedLrn, @BFreeLrn, @BStuAla, @BAdultAla, @BAdvanced, @BRedeemed, @BRepaid, @BCharges, @BDeposit "
                        + " ,@LPaidElem, @LPaidJr, @LPaidSr, @LTier1, @LTier2, @LReduced, @LReducedTier1, @LFree, @LFreeTier1, @LFreeTier2, @LPaidLrn, @LReducedLrn "
                        + " ,@LFreeLrn, @LStuAla, @LAdultAla, @LOtherAla1, @LVendAla, @LAdvanced, @LRedeemed, @LRepaid, @LCharges "
                        + " ,@OPaidSnack ,@OReducedSnack ,@OFreeSnack ,@SpecialFunctionSale ,@SpecialFunctionColl ,@SpecialFunctionComm "
                        + " ,@USDASale ,@USDAColl ,@OtherSale1 ,@OtherColl1 ,@OtherSale2 ,@OtherColl2 "
                        + " ,@OnlineAdvance ,@Adjustment ,@Deposit ,@OtherSale1Cat ,@OtherColl1Cat "
                        + " ,@EditCheckComments ,@RecapComments ,@OtherSale2Cat ,@OtherColl2Cat "
                        + " ,@R_AF, @R_Enrollment, @R_BreakfastIncent, @R_EligiblePaid, @R_EligibleReduced, @R_EligibleFree, @Revision, @EditCheckDate)";
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
                        + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND RecapDate=@RecapDate AND Revision=@Revision ";
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
                TextBox TextBoxBFree1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBFree1");
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
                TextBox TextBoxLFree1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLFree1");
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
                Label LabelRevisionDate = (Label)FindControlRecursive(FormViewSales, "LabelRevisionDate");
                #endregion

                SqlConnection dbConnection2 = new SqlConnection(connectionString);
                using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection2))
                {

                    #region KeyVals
                    //KEY Values
                    SqlParameter dbParam_ProgYear2 = new SqlParameter();
                    dbParam_ProgYear2.ParameterName = "@ProgramYear";
                    dbParam_ProgYear2.Value = KeyPYear; 
                    dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_ProgYear2);

                    SqlParameter dbParam_DistID2 = new SqlParameter();
                    dbParam_DistID2.ParameterName = "@DistrictID";
                    dbParam_DistID2.Value = KeyDistID; //DropDownListDistricts.SelectedValue;
                    dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_DistID2);

                    SqlParameter dbParam_SchlNum2 = new SqlParameter();
                    dbParam_SchlNum2.ParameterName = "@SchoolNum";
                    dbParam_SchlNum2.Value = HFSchoolNum.Value;
                    dbParam_SchlNum2.DbType = System.Data.DbType.Int16;
                    cmd2.Parameters.Add(dbParam_SchlNum2);

                    SqlParameter dbParam_RecapDate2 = new SqlParameter();
                    dbParam_RecapDate2.ParameterName = "@RecapDate";
                    if (HFRevision.Value == "0")
                        dbParam_RecapDate2.Value = HFEditCheckDate.Value;// TrxDate.Text;
                    else
                        dbParam_RecapDate2.Value = HFRevRecapDate.Value;
                    dbParam_RecapDate2.DbType = System.Data.DbType.Date;
                    cmd2.Parameters.Add(dbParam_RecapDate2);

                    //Only used in Inserts
                    SqlParameter dbParam_EditCheckDate = new SqlParameter();
                    dbParam_EditCheckDate.ParameterName = "@EditCheckDate";
                    dbParam_EditCheckDate.DbType = System.Data.DbType.Date;
                    dbParam_EditCheckDate.Value = HFEditCheckDate.Value;// TrxDate.Text;
                    cmd2.Parameters.Add(dbParam_EditCheckDate);

                    SqlParameter dbParam_Revision2 = new SqlParameter();
                    dbParam_Revision2.ParameterName = "@Revision";
                    dbParam_Revision2.Value = HFRevision.Value;
                    dbParam_Revision2.DbType = System.Data.DbType.Int32;
                    cmd2.Parameters.Add(dbParam_Revision2);
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
                    dbParam_bFree.Value = TextBoxBFree1.Text;
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
                    dbParam_lFree.Value = TextBoxLFree1.Text;
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
                        int recordcount = 0;
                        recordcount = cmd2.ExecuteNonQuery();

                        if (recordcount > 0)
                        {
                            UpdateDaysTable(UseDate, DropDownListDistricts.SelectedValue, DDLProgramYears.SelectedValue);

                            //update the datepicker color coding
                            PopulateSchoolDates();
                            PopulateSchoolPostingDates();
                            PopulateSchoolRevisedDate();

                            ShowMessageIcon("Saved", MessageType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessageIcon("Error", MessageType.Error);
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

            //Check to mark if date is a closed period
        }

        protected void SalesEntryDataChanged(object sender, EventArgs e)
        {
            //string callingID = ((WebControl)sender).ID;
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
            //Used to access the data object directly
            FormView frm = (FormView)sender;
            if (frm == null)
                return;
            DataRowView drv = (DataRowView)frm.DataItem;

            //if no school setup record was retrieved - prevent hangs or errors. Hide and disable elements.
            if(drv==null)
            {
                frm.Enabled = false;
                RegisterDOMReadyScript("HideSalesTotals", "$('#SalesTotals').hide();");
                IDelete.Enabled = false;
                IDelete.ForeColor = System.Drawing.Color.Gray;
                ISave.Enabled = false;
                ISave.ForeColor = System.Drawing.Color.Gray;

                LabelError.Text = "** Missing School Info Setup. You must setup school year info before entering Sales. **";
                return;
            }
            else
            {
                frm.Enabled = true;
                RegisterDOMReadyScript("HideSalesTotals", "$('#SalesTotals').show();");
                IDelete.Enabled = true;
                IDelete.ForeColor = System.Drawing.Color.DarkRed;
                ISave.Enabled = true;
                ISave.ForeColor = System.Drawing.Color.Green;
                LabelError.Text = "";
            }

            Boolean newRecord = (Boolean)drv["NEWRECORD"];
            DateTime.TryParse(TrxDate.Text, out DateTime checkEntryDate);
            //If this is not an existing record - check to see if the date falls in a closed period. If closed, prevent creation of the new record
            if (newRecord && IsDateOpenForEntry(checkEntryDate, DDLProgramYears.SelectedValue) ==false)
            {
                frm.Enabled = false;
                RegisterDOMReadyScript("HideSalesTotals", "$('#SalesTotals').hide();");
                IDelete.Enabled = false;
                IDelete.ForeColor = System.Drawing.Color.Gray;
                ISave.Enabled = false;
                ISave.ForeColor = System.Drawing.Color.Gray;

                LabelError.Text = "** Date falls in a closed period. New Sales entries are not allowed. **";
                return;
            }
            else
            {
                frm.Enabled = true;
                RegisterDOMReadyScript("HideSalesTotals", "$('#SalesTotals').show();");

                ISave.Enabled = true;
                ISave.ForeColor = System.Drawing.Color.Green;
                LabelError.Text = "";

                //Certain elements can be changed on a locked record, but need to prevent deletions
                Boolean RecordLocked = false;
                RecordLocked = (Boolean)drv["Locked"];
                if (RecordLocked)
                {
                    IDelete.Enabled = false;
                    IDelete.ForeColor = System.Drawing.Color.Gray;
                }
                else
                {
                    IDelete.Enabled = true;
                    IDelete.ForeColor = System.Drawing.Color.DarkRed;
                }
            }

            HtmlGenericControl CEPSchoolSalesB = (HtmlGenericControl)FindControlRecursive(FormViewSales, "CEPSchoolSalesB");
            HtmlGenericControl CEPSchoolSalesL = (HtmlGenericControl)FindControlRecursive(FormViewSales, "CEPSchoolSalesL");
            HiddenField HFSchoolCEPParticipant = (HiddenField)FindControlRecursive(FormViewSales, "HFSchoolCEPParticipant");
            HiddenField HFUseCustomCEPPct = (HiddenField)FindControlRecursive(FormViewSales, "HFUseCustomCEPPct");
            DropDownList DDLRevisions = (DropDownList)FindControlRecursive(FormViewSales, "DDLRevisions");
            //HiddenField HFPaidPct = (HiddenField)FindControlRecursive(FormViewSales, "HFPaidPct");
            //HiddenField HFFreePct = (HiddenField)FindControlRecursive(FormViewSales, "HFFreePct");
            //HiddenField HFSchoolType = (HiddenField)FindControlRecursive(FormViewSales, "HFSchoolType");

            //Entry regular field references
            TextBox TextBoxBPaidElem = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBPaidElem");
            TextBox TextBoxBPaidJr = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBPaidJr");
            TextBox TextBoxBPaidSr = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBPaidSr");
            TextBox TextBoxBReduced = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBReduced");
            TextBox TextBoxBFree1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxBFree1");

            TextBox TextBoxLPaidElem = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLPaidElem");
            TextBox TextBoxLPaidJr = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLPaidJr");
            TextBox TextBoxLPaidSr = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLPaidSr");
            TextBox TextBoxLReduced = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLReduced");
            TextBox TextBoxLFree1 = (TextBox)FindControlRecursive(FormViewSales, "TextBoxLFree1");



            //Load Budget Data
            SqlDataSourceBDGT.DataBind();
            SqlDataReader rdr = (SqlDataReader)SqlDataSourceBDGT.Select(DataSourceSelectArguments.Empty);
            if (rdr != null && rdr.Read())
            {
                bool CEPDistrict = rdr["CEP"].ToString() == "0" ? false : true;
                Int16 CEPCoverage = ((Int16)rdr["CEPCoverage"]);

                //If (CEP District and DistrictWide) OR (CEP District AND Partial AND School is marked for CEP)
                //6/17/2020 - Tom asked if the recap is a revision - display the fields normally.
                if ((CEPDistrict && CEPCoverage==0 && DDLRevisions.SelectedValue == "0") || (CEPDistrict && CEPCoverage == 1 && HFSchoolCEPParticipant.Value== "True" && DDLRevisions.SelectedValue == "0"))
                {
                    CEPSchoolSalesB.Visible = true;
                    CEPSchoolSalesL.Visible = true;

                    //Take control of the entry fields
                    TextBoxBPaidElem.Enabled = false;
                    TextBoxBPaidJr.Enabled = false;
                    TextBoxBPaidSr.Enabled = false;
                    TextBoxBReduced.Enabled = false;
                    TextBoxBFree1.Enabled = false;

                    TextBoxLPaidElem.Enabled = false;
                    TextBoxLPaidJr.Enabled = false;
                    TextBoxLPaidSr.Enabled = false;
                    TextBoxLReduced.Enabled = false;
                    TextBoxLFree1.Enabled = false;

                    if(HFUseCustomCEPPct.Value=="True")
                    {

                    }
                    else
                    {

                    }

                }
                else
                {
                    CEPSchoolSalesB.Visible = false;
                    CEPSchoolSalesL.Visible = false;
                }
            }
            else
            {
                if(CEPSchoolSalesB!=null)
                    CEPSchoolSalesB.Visible = false;
                if(CEPSchoolSalesL!=null)
                    CEPSchoolSalesL.Visible = false;
            }


            //Call Jquery which populates the totals
            RegisterDOMReadyScript("Page Load Recalculations", "RecalcAll();");
        }

        protected void TrxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox dateTextBox = (TextBox)sender;

            // If just digits are typed - convert to standard date
            NormalizeSimplifiedDate(ref dateTextBox);
            // Ensure the date falls in the Program Year. Validate weekend entries.
            ValidateTrxEntryDate(ref dateTextBox, true, HFProgYearStartDate.Value, HFProgYearEndDate.Value);

            //update the datepicker color coding
            PopulateSchoolDates();
            PopulateSchoolPostingDates();
            PopulateSchoolRevisedDate();

            //reset revisions
            HFRevision.Value = "0";

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

            //NOTE: the serving date is actually the EditCheckDate - not the RecapDate
            //Initially they are the same, but revisions of a past date match the EditCheckDate, and the update numbers post to the RecapDate
            string queryString = "SELECT DISTINCT EditCheckDate, Locked FROM [RecapEntry] "
                + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND EditCheckDate>=@StartDate AND EditCheckDate<=@EndDate ";


            if (DropDownListDistricts.Items.Count == 0)
                return;

            if (DropDownListSchools.Items.Count == 0)
                return;

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
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
            dbParam_StartDate.Value = HFProgYearStartDate.Value;
            dbParam_StartDate.DbType = System.Data.DbType.Date;
            dbCommand.Parameters.Add(dbParam_StartDate);

            SqlParameter dbParam_EndDate = new SqlParameter();
            dbParam_EndDate.ParameterName = "@EndDate";
            dbParam_EndDate.Value = HFProgYearEndDate.Value;
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
                            sbLocked.Append(((DateTime)dataReader["EditCheckDate"]).ToShortDateString());
                        else
                        {
                            sbLocked.Append(",");
                            sbLocked.Append(((DateTime)dataReader["EditCheckDate"]).ToShortDateString());
                        }
                    }
                    else
                    {
                        if (sbEntered.Length == 0)
                            sbEntered.Append(((DateTime)dataReader["EditCheckDate"]).ToShortDateString());
                        else
                        {
                            sbEntered.Append(",");
                            sbEntered.Append(((DateTime)dataReader["EditCheckDate"]).ToShortDateString());
                        }
                    }

                }


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
                if (dbCommand != null)
                    dbCommand.Dispose();
            }

        }

        protected void PopulateSchoolRevisedDate()
        {
            //clear out the hidden fields entry
            HFRevisedSchoolDaysArray.Value = string.Empty;

            StringBuilder sbRevised = new StringBuilder();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            //NOTE: the serving date is actually the EditCheckDate - not the RecapDate
            //Initially they are the same, but revisions of a past date match the EditCheckDate, and the update numbers post to the RecapDate
            string queryString = "SELECT DISTINCT RE.EditCheckDate, EditChkCount " +
            " FROM[RecapEntry] RE "+
            " INNER JOIN(SELECT COUNT(EditCheckDate) EditChkCount, ProgramYear, DistrictID, SchoolNum, EditCheckDate FROM RecapEntry GROUP BY ProgramYear, DistrictID, SchoolNum, EditCheckDate) RECounts "+
            " ON RECounts.ProgramYear = RE.ProgramYear AND RECounts.DistrictID = RE.DistrictID AND RECounts.SchoolNum = RE.SchoolNum AND RECounts.EditCheckDate = RE.EditCheckDate "+
            " WHERE EditChkCount> 1 AND RE.ProgramYear=@ProgramYear AND RE.DistrictID=@DistrictID AND RE.SchoolNum=@SchoolNum AND RE.EditCheckDate>=@StartDate AND RE.EditCheckDate<=@EndDate ";


            if (DropDownListDistricts.Items.Count == 0)
                return;

            if (DropDownListSchools.Items.Count == 0)
                return;

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
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
            dbParam_StartDate.Value = HFProgYearStartDate.Value;
            dbParam_StartDate.DbType = System.Data.DbType.Date;
            dbCommand.Parameters.Add(dbParam_StartDate);

            SqlParameter dbParam_EndDate = new SqlParameter();
            dbParam_EndDate.ParameterName = "@EndDate";
            dbParam_EndDate.Value = HFProgYearEndDate.Value;
            dbParam_EndDate.DbType = System.Data.DbType.Date;
            dbCommand.Parameters.Add(dbParam_EndDate);

            try
            {
                dbConnection.Open();
                SqlDataReader dataReader = dbCommand.ExecuteReader();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    if (sbRevised.Length == 0)
                        sbRevised.Append(((DateTime)dataReader["EditCheckDate"]).ToShortDateString());
                    else
                    {
                        sbRevised.Append(",");
                        sbRevised.Append(((DateTime)dataReader["EditCheckDate"]).ToShortDateString());
                    }
                }


                HFRevisedSchoolDaysArray.Value = sbRevised.ToString();

                if (dataReader != null)
                    dataReader.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                dbConnection.Close();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }

        }

        protected void PopulateSchoolPostingDates()
        {
            //clear out the hidden fields entry
            HFPostingDaysArray.Value = string.Empty;

            StringBuilder sbPosting = new StringBuilder();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            //NOTE: the serving date is actually the EditCheckDate - not the RecapDate
            //Initially they are the same, but revisions of a past date match the EditCheckDate, and the update numbers post to the RecapDate
            string queryString = "SELECT RecapDate FROM [RecapEntry] "
                + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND RecapDate>=@StartDate AND RecapDate<=@EndDate" +
                " AND Locked=0 AND Revision=0 ";


            if (DropDownListDistricts.Items.Count == 0)
                return;

            if (DropDownListSchools.Items.Count == 0)
                return;

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
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
            dbParam_StartDate.Value = HFProgYearStartDate.Value;
            dbParam_StartDate.DbType = System.Data.DbType.Date;
            dbCommand.Parameters.Add(dbParam_StartDate);

            SqlParameter dbParam_EndDate = new SqlParameter();
            dbParam_EndDate.ParameterName = "@EndDate";
            dbParam_EndDate.Value = HFProgYearEndDate.Value;
            dbParam_EndDate.DbType = System.Data.DbType.Date;
            dbCommand.Parameters.Add(dbParam_EndDate);

            try
            {
                dbConnection.Open();
                SqlDataReader dataReader = dbCommand.ExecuteReader();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                        if (sbPosting.Length == 0)
                            sbPosting.Append(((DateTime)dataReader["RecapDate"]).ToShortDateString());
                        else
                        {
                            sbPosting.Append(",");
                            sbPosting.Append(((DateTime)dataReader["RecapDate"]).ToShortDateString());
                        }
                }


                HFPostingDaysArray.Value = sbPosting.ToString();

                if (dataReader != null)
                    dataReader.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                dbConnection.Close();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }

        }


        protected void DropDownListSchools_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSchoolDates();
            PopulateSchoolPostingDates();
            PopulateSchoolRevisedDate();

            //reset revisions
            HFRevision.Value = "0";

            TextBox TextBoxEnrollment = (TextBox)FindControlRecursive(FormViewSales, "TextBoxEnrollment");
            if(TextBoxEnrollment != null)
            {
                TextBoxEnrollment.Focus();
            }
            
        }

        protected void DDLRevisions_SelectedIndexChanged(object sender, EventArgs e)
        {


            DropDownList DDLRevisions = (DropDownList)sender;
            HFRevision.Value = DDLRevisions.SelectedValue;

            //HFRevRecapDate expects a date, so if ORIG is the text, substitute the original EditCheckDate.
            if (DDLRevisions.SelectedItem.Text == "ORIG")
                HFRevRecapDate.Value = TrxDate.Text;
            else
                HFRevRecapDate.Value = DDLRevisions.SelectedItem.Text;

            //DDLRevisions.SelectedIndex = -1;
            FormViewSales.DataBind();
        }

        protected void DDLRevisions_DataBinding(object sender, EventArgs e)
        {
            //For new revisions - need to add the revision being created since it doesn't exist in the table
            //This adds before binding so there will probably be duplication. Need to remove index 0 after binding.
            SqlDataSourceRevisions.DataBind();
            DropDownList DDLRevisions = (DropDownList)sender;
            //DDLRevisions.DataBind();

            if (HFRevision.Value == "")
                HFRevision.Value = "0";

            ListItem li = DDLRevisions.Items.FindByValue(HFRevision.Value);

            if (li == null)
            {
                if (HFRevision.Value == "0")
                    li = new ListItem("ORIG", "0");
                else
                    li = new ListItem(HFRevRecapDate.Value, HFRevision.Value);
                DDLRevisions.Items.Add(li);

                //Selecting was causing a SelectedIndex and SelectedValue mutually exclusive error
                //DDLRevisions.SelectedIndex = DDLRevisions.Items.Count - 1;
            }
        }

        protected void DDLRevisions_PreRender(object sender, EventArgs e)
        {
            DropDownList DDLRevisions = (DropDownList)sender;
            string selectedVal = DDLRevisions.SelectedValue;
            int newIndex = -1;

            //Code to remove the duplicate value - DDL is prepopulated with the new record value which may not exist. If it does exist, it's added again - so need removed.
            for(int i=0; i<DDLRevisions.Items.Count; i++)
            {
                if (i == 0)
                    continue;
                if (DDLRevisions.Items[i].Value == selectedVal)
                {
                    newIndex = i;//select new index
                    DDLRevisions.SelectedIndex = i;
                    if(DDLRevisions.Items[0].Value == DDLRevisions.Items[i].Value)
                        DDLRevisions.Items.RemoveAt(0);
                    break;//exit the loop
                }
            }

        }

        protected void FormViewSales_DataBinding(object sender, EventArgs e)
        {
            //SqlDataSourceRevisions.DataBind();
            //DropDownList DDLRevisions = (DropDownList)FindControlRecursive(FormViewSales, "DDLRevisions");
            //DDLRevisions.DataBind();
        }

        protected void BtnRevisionDialog_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "modalBox", "$('#myModal').modal('show');", true);
        }

        protected void ButtonCreateRevision_Click(object sender, EventArgs e)
        {
            TextBox TextBoxPostingRecapDate = (TextBox)FindControlRecursive(FormViewSales, "TextBoxPostingRecapDate");
            if (TextBoxPostingRecapDate == null)
                return;
            if (string.IsNullOrEmpty(TextBoxPostingRecapDate.Text))
                return;

            if (DateTime.TryParse(TextBoxPostingRecapDate.Text, out DateTime PostDate) == false)
                return;

            //Validate the date is a valid recap and is not locked.
            if(!IsPostingDateValid(PostDate))
            {
                RegisterDOMReadyScript("error message", "alert('The selected date is not valid for creating a revision (closed period or locked posting date).');");
                return;
            }

            //Retrieve the next revision number
            int NewRevision = GetNextRevisionNumber(PostDate);

            if (NewRevision == 0)
                return;

            //Generate and insert the revision record
            bool RevisionCreated = CreateNewRevisionRecord(PostDate, NewRevision);

            //Populate the window with the revision record
            if(RevisionCreated)
            {
                HFRevision.Value = NewRevision.ToString();

                HFRevRecapDate.Value = PostDate.ToShortDateString();

                //DDLRevisions.SelectedIndex = -1;
                FormViewSales.DataBind();
            }

        }

        /// <summary>
        /// Checks the specified date to ensure there is an open and created sales entry.
        /// </summary>
        /// <param name="PostDate"></param>
        /// <returns></returns>
        private bool IsPostingDateValid(DateTime PostDate)
        {
            bool dateIsValid = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT * FROM [RecapEntry] "
                + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND RecapDate=@RecapDate AND Locked=0 AND Revision=0 ";


            if (DropDownListDistricts.Items.Count == 0)
                return false;

            if (DropDownListSchools.Items.Count == 0)
                return false;

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
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

            SqlParameter dbParam_RecapDate = new SqlParameter();
            dbParam_RecapDate.ParameterName = "@RecapDate";
            dbParam_RecapDate.Value = PostDate;
            dbParam_RecapDate.DbType = System.Data.DbType.DateTime;
            dbCommand.Parameters.Add(dbParam_RecapDate);

            try
            {
                dbConnection.Open();
                SqlDataReader dataReader = dbCommand.ExecuteReader();
                //If record exists and Globally the date is in an open entry period
                if(dataReader.Read() && IsDateOpenForEntry(PostDate, DDLProgramYears.SelectedValue))
                {
                    dateIsValid = true;
                }
                else
                    dateIsValid = false;

                if (dataReader != null)
                    dataReader.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                dbConnection.Close();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }


            return dateIsValid;
        }

        private int GetNextRevisionNumber(DateTime PostDate)
        {
            int nextRevision = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT MAX(Revision)+1 NextRevision FROM [RecapEntry] " +
                " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND RecapDate=@RecapDate " +
                " GROUP BY ProgramYear, DistrictID, SchoolNum, RecapDate ";


            if (DropDownListDistricts.Items.Count == 0)
                return 0;

            if (DropDownListSchools.Items.Count == 0)
                return 0;

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
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

            SqlParameter dbParam_RecapDate = new SqlParameter();
            dbParam_RecapDate.ParameterName = "@RecapDate";
            dbParam_RecapDate.Value = PostDate;
            dbParam_RecapDate.DbType = System.Data.DbType.DateTime;
            dbCommand.Parameters.Add(dbParam_RecapDate);

            try
            {
                dbConnection.Open();
                object intObj = dbCommand.ExecuteScalar();

                if (intObj != null)
                    int.TryParse(intObj.ToString(), out nextRevision);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                dbConnection.Close();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }


            return nextRevision;
        }

        private bool CreateNewRevisionRecord(DateTime PostDate, int NewRevisionNum)
        {
            bool recordCreated = false;

            string InsertQuery = "INSERT INTO [RecapEntry] (ProgramYear, DistrictID, SchoolNum, RecapDate, Revision, EditCheckDate ) "
            + " VALUES (@ProgramYear, @DistrictID, @SchoolNum, @RecapDate, @Revision, @EditCheckDate)";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand(InsertQuery, dbConnection))
            {

                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = DDLProgramYears.SelectedValue;
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = DropDownListDistricts.SelectedValue;
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_SchlNum2 = new SqlParameter();
                dbParam_SchlNum2.ParameterName = "@SchoolNum";
                dbParam_SchlNum2.Value = DropDownListSchools.SelectedValue;
                dbParam_SchlNum2.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_SchlNum2);

                // Recap date is in a new, unclosed period
                SqlParameter dbParam_RecapDate2 = new SqlParameter();
                dbParam_RecapDate2.ParameterName = "@RecapDate";
                dbParam_RecapDate2.Value = PostDate;
                dbParam_RecapDate2.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_RecapDate2);

                // Edit Check Date is the original Recap Date which is being corrected
                SqlParameter dbParam_EditCheckDate = new SqlParameter();
                dbParam_EditCheckDate.ParameterName = "@EditCheckDate";
                dbParam_EditCheckDate.DbType = System.Data.DbType.Date;
                dbParam_EditCheckDate.Value = TrxDate.Text;
                cmd.Parameters.Add(dbParam_EditCheckDate);

                SqlParameter dbParam_Revision2 = new SqlParameter();
                dbParam_Revision2.ParameterName = "@Revision";
                dbParam_Revision2.Value = NewRevisionNum;
                dbParam_Revision2.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_Revision2);
                #endregion

                try
                {
                    dbConnection.Open();
                    int recordcount = 0;
                    recordcount = cmd.ExecuteNonQuery();

                    if (recordcount > 0)
                    {
                        recordCreated = true;
                        ShowMessageIcon("Saved", MessageType.Success);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageIcon("Error", MessageType.Error);
                    RegisterDOMReadyScript("error message", "alert('Error creating revision record.');");
                }
                finally
                {
                    dbConnection.Close();
                }

            }



            return recordCreated;
        }

        protected void SqlDataSourceSales_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            string progYear = e.Command.Parameters["@ProgramYear"].ToString();
        }

        protected void BtnSpFnDialog_Click(object sender, EventArgs e)
        {
            //Modal opens for Special Functions Import
            //Refresh the Special Functions grid and prepopulate the dates
            if (DateTime.TryParse(TrxDate.Text, out DateTime StartDate) == false)
            {
                StartDate = DateTime.Now;
            }
            DateTime MonthStart = new DateTime(StartDate.Year, StartDate.Month, 1);
            DateTime MonthEnd = MonthStart.AddMonths(1).AddDays(-1);


            if (TextBoxSpFnStartDate.Text == "")
                TextBoxSpFnStartDate.Text = MonthStart.ToShortDateString();
            if (TextBoxSpFnEndDate.Text == "")
                TextBoxSpFnEndDate.Text = MonthEnd.ToShortDateString();

            //GridviewSpecialFunctions.DataBind();
            UpdateSpFnDateRange(null, null);

            RegisterDOMReadyScript("show", "$('#SpecialFunctionsImportModal').modal('show');");
            OverallPanel.Update();

        }

        protected void UpdateSpFnDateRange(object sender, EventArgs e)
        {
            if (DateTime.TryParse(TextBoxSpFnStartDate.Text, out DateTime startDate) == false)
                return;
            if (DateTime.TryParse(TextBoxSpFnEndDate.Text, out DateTime endDate) == false)
                return;

            GridviewSpecialFunctions.DataBind();

            decimal SelectedTotal = TotalSpFnGridview();
            LabelTotal.Text = SelectedTotal.ToString("F2");

        }

        protected void CBSelect_CheckedChanged(object sender, EventArgs e)
        {
            decimal SelectedTotal = TotalSpFnGridview();
            LabelTotal.Text = SelectedTotal.ToString("F2");
        }

        protected decimal TotalSpFnGridview()
        {
            Decimal SpFnTotal = 0M;

            foreach(GridViewRow gvr in GridviewSpecialFunctions.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow )
                {
                    Label LabelInvoiceTotal = (Label)FindControlRecursive(gvr, "LabelInvoiceTotal");
                    CheckBox CBSelect = (CheckBox)FindControlRecursive(gvr, "CBSelect");
                    if (LabelInvoiceTotal!=null && CBSelect!=null && CBSelect.Checked)
                    {
                        //string totalString = LabelInvoiceTotal.Text.Remove(  0, 1);//remove the "$" symbol
                        if(Decimal.TryParse(LabelInvoiceTotal.Text, out Decimal outTotal))
                            SpFnTotal += outTotal;
                    }
                }
            }

            return SpFnTotal;
        }

        /// <summary>
        /// Processes the Special Functions invoice total import
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnProcessSpFn_Click(object sender, EventArgs e)
        {
            TextBox TextBoxOSFSale = (TextBox)FindControlRecursive(FormViewSales, "TextBoxOSFSale");
            //string totalString = LabelTotal.Text.Remove(  0, 1);//remove the "$" symbol
            if (Decimal.TryParse(LabelTotal.Text, out Decimal outSelectedTotal) == false)
                outSelectedTotal = 0M; ;

            if (TextBoxOSFSale != null)
            {
                if (rlOptions.SelectedValue == "ADD")
                {
                    string SpFnTotal = TextBoxOSFSale.Text;
                    if(Decimal.TryParse(SpFnTotal, out Decimal outSpFnTotal)==false)
                        outSpFnTotal=0M;

                    TextBoxOSFSale.Text = (outSelectedTotal + outSpFnTotal).ToString("F2", CultureInfo.CurrentCulture);
                    SalesEntryDataChanged(null, EventArgs.Empty);

                }
                else
                {
                    TextBoxOSFSale.Text = (outSelectedTotal).ToString("F2", CultureInfo.CurrentCulture);
                    SalesEntryDataChanged(null, EventArgs.Empty);
                }

                MarkSpecialFunctionInvoicesBilled();
            }

            UpdatePanel1.Update();
            
            RegisterDOMReadyScript("Pop", "$('#SpecialFunctionsImportModal').modal('hide');$('[id *= TextBoxOSFSale]').get(0).scrollIntoView();");

        }

        /// <summary>
        /// Update the records which were just imported and mark them as posted.
        /// </summary>
        private void MarkSpecialFunctionInvoicesBilled()
        {
            foreach(GridViewRow gvr in GridviewSpecialFunctions.Rows)
            {
                if(gvr.RowType== DataControlRowType.DataRow)
                {
                    CheckBox CBSelect = (CheckBox)FindControlRecursive(gvr, "CBSelect");
                    HiddenField HFInvoiceNumber = (HiddenField)FindControlRecursive(gvr, "HFInvoiceNumber");


                    if (CBSelect!=null && HFInvoiceNumber!=null && CBSelect.Checked && HFInvoiceNumber.Value.Length!=0)
                    {
                        MarkSpFnRecordAsBilled(DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, HFInvoiceNumber.Value);
                    }
                }
            }


        }

        private void MarkSpFnRecordAsBilled(string ProgYear, string DistID, string InvNum)
        {
            string UpdateQuery = "UPDATE [SPFNInvoiceHDR] SET BillingDate=@BillingDate " +
                " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@InvoiceNumber ";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand(UpdateQuery, dbConnection))
            {

                //KEY Values
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = ProgYear;
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistID;
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = InvNum;
                dbParam_InvNum.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_InvNum);

                //Post Date
                SqlParameter dbParam_BillingDate = new SqlParameter();
                dbParam_BillingDate.ParameterName = "@BillingDate";
                dbParam_BillingDate.Value = TrxDate.Text;
                dbParam_BillingDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_BillingDate);


                try
                {
                    dbConnection.Open();
                    int recordcount = 0;
                    recordcount = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //RegisterDOMReadyScript("error message", "alert('Error creating revision record.');");
                }
                finally
                {
                    dbConnection.Close();
                }

            }
        }

        protected void IDelete_Click(object sender, EventArgs e)
        {
            Label LabelLocked = (Label)FindControlRecursive(FormViewSales, "LabelLocked");
            if(LabelLocked!=null && LabelLocked.Visible==true)
            {
                ShowDialog("Error", MessageType.Error, "This record is locked and cannot be deleted.", "");
                return;
            }
            //SqlDataSourceSales.Delete();
            try
            {
                FormViewSales.DeleteItem();



            }
            catch (Exception ex) { }
        }

        protected void SqlDataSourceSales_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //update the datepicker color coding
            PopulateSchoolDates();
            PopulateSchoolPostingDates();
            PopulateSchoolRevisedDate();

            UpdateDaysTable(  (DateTime)e.Command.Parameters["@RecapDate"].Value, DropDownListDistricts.SelectedValue, DDLProgramYears.SelectedValue);

            ShowMessageIcon("Deleted", MessageType.Success);
            HFDeleting.Value = "0";
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
                    HFBudgetYear.Value = drvSql["BudgetYear"].ToString().TrimEnd(' ');
                    HFProgYearStartDate.Value = drvSql["StartDate"].ToString();
                    HFProgYearEndDate.Value = drvSql["EndDate"].ToString();

                    //Set the default user date - then check to see if it falls outside the program year and adjust accordingly
                    DateTime UseDate = DateTime.Now.Date;

                    DateTime stDate;
                    DateTime enDate;

                    if (DateTime.TryParse(HFProgYearStartDate.Value, out stDate) && DateTime.TryParse(HFProgYearEndDate.Value, out enDate))
                    {
                        if (UseDate >= stDate && UseDate <= enDate)
                        { /*Do nothing */}
                        else if (UseDate < stDate)
                            UseDate = stDate;
                        else if (UseDate > enDate)
                            UseDate = enDate;
                    }

                    TrxDate.Text = UseDate.ToShortDateString();

                    //New 8/13/2020 - when program year changes, districts could change as well
                    //Dists are now based on ProgramYear and must be rebound
                    bool DistrictChanged = false;

                    DropDownListDistricts.DataBind();
                    if (DropDownListDistricts.Items.Count == 0)
                        Response.Redirect("~/default.aspx");
                    if (Session["CurrentDistrict"] != null)
                    {
                        if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                            DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                        else
                        {
                            DropDownListDistricts_SelectedIndexChanged(null, null);
                            DistrictChanged = true;
                        }
                    }

                    //Rebind the schools as well and try to restore the current working school
                    string storedSchoolNum = "";
                    if (DistrictChanged == false)//if district didn't change - store the current school and try to re-select after bind
                    {
                        if (DropDownListSchools.Items.Count > 0 && DropDownListSchools.SelectedIndex > -1)
                            storedSchoolNum = DropDownListSchools.SelectedValue;
                    }
                    DropDownListSchools.DataBind();
                    if (DropDownListSchools.Items.Count == 0)
                        Response.Redirect("~/Setup/SetupSchools.aspx");
                    else
                    {
                        if (DropDownListSchools.Items.FindByValue(storedSchoolNum) != null)
                            DropDownListSchools.SelectedValue = storedSchoolNum;
                    }

                    //Repopulate the date indicators
                    DropDownListSchools_SelectedIndexChanged(null, null);

                    //PopulateSchoolDates();
                    //PopulateSchoolPostingDates();
                    //PopulateSchoolRevisedDate();



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
                //if no program years could be populated - redirect home
                Response.Redirect("~/default.aspx");
            }


        }

    }
}
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupDistrictInfo : Paperwork_Global
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
            }
            else
            {
                VerifyAccess(false);
            }

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if(!IsPostBack)
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
                Response.Redirect("~/Setup/SetupDistrictInfo.aspx");
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
        protected void FormViewDistInfo_PreRender(object sender, EventArgs e)
        {
            HiddenField HFDistInfoSaved = (HiddenField)FindControlRecursive(FormViewDistInfo, "HFDistInfoSaved");
            HiddenField HFDistSaved = (HiddenField)FindControlRecursive(FormViewDistInfo, "HFDistSaved");

            if(HFDistInfoSaved.Value=="1" && HFDistSaved.Value=="1")
                ShowMessageIcon("District Info & Invoice Details Saved. ", MessageType.Success);
            else if(HFDistInfoSaved.Value == "1")
                ShowMessageIcon("District Info Saved. ", MessageType.Success);
            else if (HFDistSaved.Value == "1")
                ShowMessageIcon("Invoice Details Saved. ", MessageType.Success);

            if (HFDistInfoSaved != null)
                HFDistInfoSaved.Value = "0";

            if (HFDistSaved != null)
                HFDistSaved.Value = "0";
        }

        /// <summary>
        /// Updates the DistrictYearInfo table
        /// </summary>
        protected void SaveDistInfo()
        {
            HiddenField HFDistInfoSaved = (HiddenField)FindControlRecursive(FormViewDistInfo, "HFDistInfoSaved");
            if (HFDistInfoSaved.Value == "1")
                return;

            if (DropDownListDistricts.Items.Count == 0)
                return;

            #region ReferenceControls
            //TextBox TextBoxRM = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxRM");
            String KeyPYear = FormViewDistInfo.DataKey.Values["ProgramYear"].ToString();
            String KeyDistID = FormViewDistInfo.DataKey.Values["DistrictID"].ToString();

            DropDownList DropDownListRMs = (DropDownList)FindControlRecursive(FormViewDistInfo, "DropDownListRMs");
            TextBox TextBoxRMCode = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxRMCode");
            TextBox TextBoxFSD = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxFSD");
            TextBox TextBoxPhone = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxPhone");
            TextBox TextBoxCell = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxCell");
            TextBox TextBoxAddress = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxAddress");
            TextBox TextBoxFSDCity = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxFSDCity");
            TextBox TextBoxFSDState = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxFSDState");
            TextBox TextBoxFSDZip = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxFSDZip");
            TextBox TextBoxBusMgr = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxBusMgr");
            TextBox TextBoxSuper = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxSuper");
            TextBox TextBoxFSDEmail = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxFSDEmail");
            TextBox TextBoxAsstFSD = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxAsstFSD");
            TextBox TextBoxAsstFSDPhone = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxAsstFSDPhone");

            TextBox TextBoxDistCEPPaidPct = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDistCEPPaidPct");
            TextBox TextBoxDistCEPFreePct = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDistCEPFreePct");
            TextBox TextBoxCPMOverride = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxCPMOverride");

            if (!Decimal.TryParse(TextBoxDistCEPPaidPct.Text, out Decimal decPaidPct))
                decPaidPct = 0M;
            if (!Decimal.TryParse(TextBoxDistCEPFreePct.Text, out Decimal decFreePct))
                decFreePct = 0M;
            if (!Decimal.TryParse(TextBoxCPMOverride.Text, out Decimal decCPMOverride))
                decCPMOverride = 0M;

            if (decPaidPct + decFreePct >0M && decPaidPct + decFreePct !=100M)
            {
                RegisterDOMReadyScript("error message", "alert('CEP percentages do not add to 100. Save cancelled.');");
                return;
            }

            DropDownList DDL_AccountType = (DropDownList)FindControlRecursive(FormViewDistInfo, "DDL_AccountType");
            #endregion

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryUpdateDistInfo = "SELECT COUNT(*) FROM [DistrictYearInfo] " +
                                          " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID  ";
            int existingRecordCount = 0;

            using (SqlCommand cmd = new SqlCommand(queryUpdateDistInfo, dbConnection))
            {

                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = KeyPYear;//DDLProgramYears.SelectedValue;
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = KeyDistID;// Session["CurrentDistrict"].ToString();
                dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID);
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
                catch (SqlException ex)
                { 
                    ShowDialog("Error", MessageType.Error, "SQL Error reading the district info table.", ex.Message); 
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error reading the district info table.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                    cmd.Dispose();
                }
            }

            string InsertUpdateQuery;
            if (existingRecordCount == 0)
            {
                InsertUpdateQuery = "INSERT INTO [DistrictYearInfo] ( [ProgramYear],[DistrictID],[AcctType],[GM],[RMCode],[RMName],[RMNumber],[FSDName],[FSDAddress] " +
                    " ,[FSDCity],[FSDState],[FSDZip],[FSDEmail],[FSDPhone],[FSDCell],[BusinessMgr],[Superintendent],[AsstFSDName],[AsstFSDPhone]" +
                    " ,[DistCEPPaidPct],[DistCEPFreePct],[CPMBillingRate]) " +
                    " VALUES (@ProgramYear,@DistrictID,@AcctType,'',@RMCode,@RMName,@RMNumber,@FSDName,@FSDAddress " +
                    " ,@FSDCity,@FSDState,@FSDZip,@FSDEmail,@FSDPhone,@FSDCell,@BusinessMgr,@Superintendent,@AsstFSDName,@AsstFSDPhone" +
                    " ,@CEPPaidPct,@CEPFreePct,@CPMBillingRate) ";
            }
            else
            {
                InsertUpdateQuery = "UPDATE [DistrictYearInfo] SET AcctType=@AcctType " + //,GM=@GM
                    " ,RMCode=@RMCode,RMName=@RMName,RMNumber=@RMNumber,FSDName=@FSDName,FSDAddress=@FSDAddress " + //,RMCode=@RMCode
                    " ,FSDCity=@FSDCity,FSDState=@FSDState,FSDZip=@FSDZip,FSDEmail=@FSDEmail " +
                    " ,FSDPhone=@FSDPhone,FSDCell=@FSDCell,BusinessMgr=@BusinessMgr,Superintendent=@Superintendent" +
                    " ,AsstFSDName=@AsstFSDName,AsstFSDPhone=@AsstFSDPhone,DistCEPPaidPct=@CEPPaidPct,DistCEPFreePct=@CEPFreePct " +
                    " ,CPMBillingRate=@CPMBillingRate" +
                    " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID ";
            }

            using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection))
            {

                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = KeyPYear;// DDLProgramYears.SelectedValue;
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = KeyDistID;// Session["CurrentDistrict"].ToString();
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_DistID2);

                #endregion

                //Updateable Values
                #region MainValues

                SqlParameter dbParam_AcctType = new SqlParameter();
                dbParam_AcctType.ParameterName = "@AcctType";
                dbParam_AcctType.Value = DDL_AccountType.SelectedValue;
                dbParam_AcctType.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_AcctType);

                SqlParameter dbParam_RMName = new SqlParameter();
                dbParam_RMName.ParameterName = "@RMName";
                dbParam_RMName.Value = DropDownListRMs.SelectedItem.Text;
                dbParam_RMName.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_RMName);

                SqlParameter dbParam_RMNumber = new SqlParameter();
                dbParam_RMNumber.ParameterName = "@RMNumber";
                dbParam_RMNumber.Value = DropDownListRMs.SelectedValue;
                dbParam_RMNumber.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_RMNumber);

                SqlParameter dbParam_RMCode = new SqlParameter();
                dbParam_RMCode.ParameterName = "@RMCode";
                dbParam_RMCode.Value = TextBoxRMCode.Text;
                dbParam_RMCode.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_RMCode);

                SqlParameter dbParam_FSDName = new SqlParameter();
                dbParam_FSDName.ParameterName = "@FSDName";
                dbParam_FSDName.Value = TextBoxFSD.Text;
                dbParam_FSDName.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_FSDName);

                SqlParameter dbParam_FSDPhone = new SqlParameter();
                dbParam_FSDPhone.ParameterName = "@FSDPhone";
                dbParam_FSDPhone.Value = TextBoxPhone.Text;
                dbParam_FSDPhone.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_FSDPhone);

                SqlParameter dbParam_FSDCell = new SqlParameter();
                dbParam_FSDCell.ParameterName = "@FSDCell";
                dbParam_FSDCell.Value = TextBoxCell.Text;
                dbParam_FSDCell.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_FSDCell);

                SqlParameter dbParam_FSDEmail = new SqlParameter();
                dbParam_FSDEmail.ParameterName = "@FSDEmail";
                dbParam_FSDEmail.Value = TextBoxFSDEmail.Text;
                dbParam_FSDEmail.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_FSDEmail);

                SqlParameter dbParam_FSDAddress = new SqlParameter();
                dbParam_FSDAddress.ParameterName = "@FSDAddress";
                dbParam_FSDAddress.Value = TextBoxAddress.Text;
                dbParam_FSDAddress.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_FSDAddress);

                SqlParameter dbParam_FSDCity = new SqlParameter();
                dbParam_FSDCity.ParameterName = "@FSDCity";
                dbParam_FSDCity.Value = TextBoxFSDCity.Text;
                dbParam_FSDCity.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_FSDCity);

                SqlParameter dbParam_FSDState = new SqlParameter();
                dbParam_FSDState.ParameterName = "@FSDState";
                dbParam_FSDState.Value = TextBoxFSDState.Text;
                dbParam_FSDState.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_FSDState);

                SqlParameter dbParam_FSDZip = new SqlParameter();
                dbParam_FSDZip.ParameterName = "@FSDZip";
                dbParam_FSDZip.Value = TextBoxFSDZip.Text;
                dbParam_FSDZip.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_FSDZip);

                SqlParameter dbParam_BusinessMgr = new SqlParameter();
                dbParam_BusinessMgr.ParameterName = "@BusinessMgr";
                dbParam_BusinessMgr.Value = TextBoxBusMgr.Text;
                dbParam_BusinessMgr.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_BusinessMgr);

                SqlParameter dbParam_Superintendent = new SqlParameter();
                dbParam_Superintendent.ParameterName = "@Superintendent";
                dbParam_Superintendent.Value = TextBoxSuper.Text;
                dbParam_Superintendent.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_Superintendent);

                SqlParameter dbParam_AsstFSDName = new SqlParameter();
                dbParam_AsstFSDName.ParameterName = "@AsstFSDName";
                dbParam_AsstFSDName.Value = TextBoxAsstFSD.Text;
                dbParam_AsstFSDName.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_AsstFSDName);

                SqlParameter dbParam_AsstFSDPhone = new SqlParameter();
                dbParam_AsstFSDPhone.ParameterName = "@AsstFSDPhone";
                dbParam_AsstFSDPhone.Value = TextBoxAsstFSDPhone.Text;
                dbParam_AsstFSDPhone.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_AsstFSDPhone);

                SqlParameter dbParam_CEPPaidPct = new SqlParameter();
                dbParam_CEPPaidPct.ParameterName = "@CEPPaidPct";
                dbParam_CEPPaidPct.Value = decPaidPct;
                dbParam_CEPPaidPct.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_CEPPaidPct);

                SqlParameter dbParam_CEPFreePct = new SqlParameter();
                dbParam_CEPFreePct.ParameterName = "@CEPFreePct";
                dbParam_CEPFreePct.Value = decFreePct;
                dbParam_CEPFreePct.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_CEPFreePct);

                SqlParameter dbParam_CPMBillingRate = new SqlParameter();
                dbParam_CPMBillingRate.ParameterName = "@CPMBillingRate";
                dbParam_CPMBillingRate.Value = decCPMOverride;
                dbParam_CPMBillingRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_CPMBillingRate);
                #endregion

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();
                    //if(recordcount>0)
                    //    ShowMessageIcon("Saved. " , MessageType.Success);
                }
                catch(SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Error saving the district info.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error saving the district info.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
            

            //Mark hdr as saved
            HFDistInfoSaved.Value = "1";

        }

        /// <summary>
        /// Updates the District table
        /// </summary>
        protected void SaveDist()
        {
            HiddenField HFDistSaved = (HiddenField)FindControlRecursive(FormViewDistInfo, "HFDistSaved");
            if (HFDistSaved.Value == "1")
                return;

            if (DropDownListDistricts.Items.Count == 0)
                return;

            #region ReferenceControls
            TextBox TextBoxInvoiceName = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxInvoiceName");
            TextBox TextBoxAddress1 = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxAddress1");
            TextBox TextBoxAddress2 = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxAddress2");
            TextBox TextBoxInvCity = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxInvCity");
            TextBox TextBoxInvState = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxInvState");
            TextBox TextBoxInvZip = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxInvZip");
            TextBox TextBoxInvContact = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxInvContact");
            #endregion

            string InsertUpdateQuery = "UPDATE [District] SET DistrictFullName=@DistrictFullName " + //,GM=@GM
                    " ,DistrictAddress1=@DistrictAddress1,DistrictAddress2=@DistrictAddress2,DistrictCity=@DistrictCity " + //,RMCode=@RMCode
                    " ,DistrictState=@DistrictState,DistrictZip=@DistrictZip,DistrictContact=@DistrictContact " +
                    " WHERE DistrictID=@DistrictID ";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(InsertUpdateQuery, dbConnection))
            {

                #region KeyVals
                //KEY Values
                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = Session["CurrentDistrict"].ToString();
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID2);
                #endregion

                //Updateable Values
                #region MainValues

                SqlParameter dbParam_DistName = new SqlParameter();
                dbParam_DistName.ParameterName = "@DistrictFullName";
                dbParam_DistName.Value = TextBoxInvoiceName.Text;
                dbParam_DistName.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistName);

                SqlParameter dbParam_DistAddr1 = new SqlParameter();
                dbParam_DistAddr1.ParameterName = "@DistrictAddress1";
                dbParam_DistAddr1.Value = TextBoxAddress1.Text;
                dbParam_DistAddr1.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistAddr1);

                SqlParameter dbParam_DistAddr2 = new SqlParameter();
                dbParam_DistAddr2.ParameterName = "@DistrictAddress2";
                dbParam_DistAddr2.Value = TextBoxAddress2.Text;
                dbParam_DistAddr2.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistAddr2);

                SqlParameter dbParam_DistCity = new SqlParameter();
                dbParam_DistCity.ParameterName = "@DistrictCity";
                dbParam_DistCity.Value = TextBoxInvCity.Text;
                dbParam_DistCity.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistCity);

                SqlParameter dbParam_DistState = new SqlParameter();
                dbParam_DistState.ParameterName = "@DistrictState";
                dbParam_DistState.Value = TextBoxInvState.Text;
                dbParam_DistState.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistState);

                SqlParameter dbParam_DistZip = new SqlParameter();
                dbParam_DistZip.ParameterName = "@DistrictZip";
                dbParam_DistZip.Value = TextBoxInvZip.Text;
                dbParam_DistZip.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistZip);

                SqlParameter dbParam_DistContact = new SqlParameter();
                dbParam_DistContact.ParameterName = "@DistrictContact";
                dbParam_DistContact.Value = TextBoxInvContact.Text;
                dbParam_DistContact.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistContact);
                #endregion

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd.ExecuteNonQuery();
                    //if (recordcount > 0)
                    //    ShowMessageIcon("Saved. ", MessageType.Success);
                }
                catch(SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Error saving the district invoice details.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error saving the district invoice details.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


            HFDistSaved.Value = "1";
        }

        protected void DistInfoControl_Changed(object sender, EventArgs e)
        {
            UpdateCPMOverrideField();

            SaveDistInfo();
        }

        /// <summary>
        /// Show or hide appropriate fields depending on CPM account type
        /// </summary>
        protected void UpdateCPMOverrideField()
        {
            DropDownList DDL_AccountType = (DropDownList)FindControlRecursive(FormViewDistInfo, "DDL_AccountType");
            TextBox TextBoxCPMOverride = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxCPMOverride");
            Label LabelCPMBillRate = (Label)FindControlRecursive(FormViewDistInfo, "LabelCPMBillRate");
            Label LabelOverrideCPM = (Label)FindControlRecursive(FormViewDistInfo, "LabelOverrideCPM");

            if (DDL_AccountType == null || TextBoxCPMOverride == null || LabelCPMBillRate == null || LabelOverrideCPM==null)
                return;

            if (DDL_AccountType != null)
            {
                if (DDL_AccountType.SelectedValue == "1")
                {
                    TextBoxCPMOverride.Visible = true;
                    LabelCPMBillRate.Visible = true;
                    LabelOverrideCPM.Visible = true;

                    //Restrict Access to field for Office or Admins
                    if (Session["AccessLevel"] != null)
                    {
                        switch (Session["AccessLevel"].ToString().ToUpper())
                        {
                            case "ADMIN":
                            case "OFFICE":
                                TextBoxCPMOverride.Enabled = true;
                                break;
                            default:
                                TextBoxCPMOverride.Enabled = false;
                                break;
                        }

                    }

                }
                else
                {
                    TextBoxCPMOverride.Visible = false;
                    LabelCPMBillRate.Visible = false;
                    LabelOverrideCPM.Visible = false;
                }
            }

        }

        protected void DistFieldsChanged(object sender, EventArgs e)
        {
            SaveDist();
        }

        protected void FormViewDistInfo_DataBound(object sender, EventArgs e)
        {
            //Show or Hide the CPM Billing fields depending on if this is a CPM account type
            UpdateCPMOverrideField();

            #region Fields
            //Populate the Budget Values from the Budget Connection and Datasource
            TextBox TextBoxOpDays = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxOpDays");
            TextBox TextBoxDailyReimb = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDailyReimb");
            TextBox TextBoxDailyCosts = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDailyCosts");
            TextBox TextBoxDailyOSales = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDailyOSales");
            TextBox TextBoxAnnualDistCost = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxAnnualDistCost");
            TextBox TextBoxCashBud = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxCashBud");
            TextBox TextBoxDailyFICA = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDailyFICA");
            TextBox TextBoxDailyInterest = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDailyInterest");
            TextBox TextBoxDailySFALabor = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDailySFALabor");
            TextBox TextBoxDistPayRel = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDistPayRel");
            TextBox TextBoxFoodCPM = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxFoodCPM");
            TextBox TextBoxPaperCleanCPM = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxPaperCleanCPM");
            TextBox TextBoxNProdHours = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxNProdHours");
            TextBox TextBoxNNonProdHours = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxNNonProdHours");
            TextBox TextBoxNSpFnHours = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxNSpFnHours");
            TextBox TextBoxDProdHours = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDProdHours");
            TextBox TextBoxDNonProdHours = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDNonProdHours");
            TextBox TextBoxDSpFnHours = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxDSpFnHours");

            TextBox TextBoxCPM = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxCPM");
            TextBox TextBoxMealEquiv = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxMealEquiv");
            TextBox TextBoxMgmtFee = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxMgmtFee");
            TextBox TextBoxAdminFee = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxAdminFee");
            TextBox TextBoxMgmtPay = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxMgmtPay");
            TextBox TextBoxAddonFee = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxAddonFee");


            TextBox TextBoxCEPCoverage = (TextBox)FindControlRecursive(FormViewDistInfo, "TextBoxCEPCoverage");
            CheckBox CB_CEP = (CheckBox)FindControlRecursive(FormViewDistInfo, "CB_CEP");
            HtmlGenericControl CEPPaidRow = (HtmlGenericControl)FindControlRecursive(FormViewDistInfo, "CEPPaidRow");
            HtmlGenericControl CEPFreeRow = (HtmlGenericControl)FindControlRecursive(FormViewDistInfo, "CEPFreeRow");
            #endregion

            string currBudget = HFBudgetYear.Value;

            SqlDataSourceBDGT.DataBind();
            SqlDataReader rdr =(SqlDataReader)SqlDataSourceBDGT.Select(DataSourceSelectArguments.Empty);
            if (rdr != null && rdr.Read())
            {
                //Invoice Details Section
                TextBoxCPM.Text = Decimal.Round(((Decimal)rdr["CostPerMeal"]), 4).ToString("F4");
                TextBoxMealEquiv.Text = Decimal.Round(((Decimal)rdr["MealEquivalentFactor"]), 4).ToString("F4");
                TextBoxMgmtFee.Text = Decimal.Round(((Decimal)rdr["CBS_FSMCMgmt"]), 2).ToString("C2");
                TextBoxAdminFee.Text = Decimal.Round(((Decimal)rdr["CBS_FSMCAdmin"]), 2).ToString("C2");
                TextBoxMgmtPay.Text = Decimal.Round(((Decimal)rdr["FSDCosts"]), 2).ToString("C2");
                TextBoxAddonFee.Text = Decimal.Round(((Decimal)rdr["AddOnFees"]), 2).ToString("C2");


                CB_CEP.Checked = ((Byte)rdr["CEP"]) == 0 ? false : true;
                if (CB_CEP.Checked)
                {
                    TextBoxCEPCoverage.Text = ((Int16)rdr["CEPCoverage"]) == 0 ? "District Wide" : "Partial";
                    CEPPaidRow.Visible = true;
                    CEPFreeRow.Visible = true;
                }
                else
                {
                    TextBoxCEPCoverage.Text = string.Empty;
                    CEPPaidRow.Visible = false;
                    CEPFreeRow.Visible = false;
                }

                //Budget Section
                TextBoxOpDays.Text = rdr["DaysOfSvcLunch"].ToString();
                TextBoxCashBud.Text = Decimal.Round(((Decimal)rdr["DailyCashCalc"]), 2).ToString("C2");
                TextBoxDailyReimb.Text = Decimal.Round(((Decimal)rdr["DailyReimBudget"]),2).ToString("C2");
                TextBoxDailyFICA.Text = Decimal.Round(((Decimal)rdr["DailyPSERSCredit"]), 2).ToString("C2");
                TextBoxDailyFICA.Text = Decimal.Round(((Decimal)rdr["DailyPSERSCredit"]), 2).ToString("C2");
                TextBoxDailyInterest.Text = Decimal.Round(((Decimal)rdr["DailyInterest"]), 2).ToString("C4");
                TextBoxDailyOSales.Text = Decimal.Round(((Decimal)rdr["DailyOtherSales"]), 2).ToString("C2");
                TextBoxDailyCosts.Text = Decimal.Round(((Decimal)rdr["DailyExpenses"]),2).ToString("C2");
                TextBoxAnnualDistCost.Text = Decimal.Round(((Decimal)rdr["DistrictCosts"]), 2).ToString("C2");
                TextBoxDailySFALabor.Text = Decimal.Round(((Decimal)rdr["DailySFALabor"]), 2).ToString("C2");
                TextBoxDistPayRel.Text = Decimal.Round(((Decimal)rdr["MaxofFringePercent"]), 6).ToString("P4");
                TextBoxNProdHours.Text = Decimal.Round(((Decimal)rdr["NUTRProdHours"]), 2).ToString("F2");
                TextBoxNNonProdHours.Text = Decimal.Round(((Decimal)rdr["NUTRNonProdHours"]), 2).ToString("F2");
                TextBoxDProdHours.Text = Decimal.Round(((Decimal)rdr["SFAProdHours"]), 2).ToString("F2");
                TextBoxDNonProdHours.Text = Decimal.Round(((Decimal)rdr["SFANonProdHours"]), 2).ToString("F2");

                TextBoxDSpFnHours.Text = Decimal.Round(((Decimal)rdr["DistSPFNLaborHours"]), 2).ToString("F2");
                TextBoxNSpFnHours.Text = Decimal.Round(((Decimal)rdr["NutSPFNLaborHours"]), 2).ToString("F2");

                //Billing Section
                TextBoxFoodCPM.Text = Decimal.Round(((Decimal)rdr["FoodCPM"]), 4).ToString("C4");
                TextBoxPaperCleanCPM.Text = Decimal.Round(((Decimal)rdr["PaperCleanCPM"]), 4).ToString("C4");
            }
            else
            {
                //Invoice Section
                TextBoxCPM.Text = (0M).ToString("F4");
                TextBoxMealEquiv.Text = (0M).ToString("F4");
                TextBoxMgmtFee.Text = (0M).ToString("C2");
                TextBoxAdminFee.Text = (0M).ToString("C2");
                TextBoxMgmtPay.Text = (0M).ToString("C2");
                TextBoxAddonFee.Text = (0M).ToString("C2");


                CB_CEP.Checked = false;
                TextBoxAddonFee.Text = string.Empty;

                //Budget Section
                TextBoxOpDays.Text = "0";
                TextBoxCashBud.Text = (0M).ToString("C2");
                TextBoxDailyReimb.Text = (0M).ToString("C2");
                TextBoxDailyFICA.Text = (0M).ToString("C2");
                TextBoxDailyInterest.Text = (0M).ToString("C4");
                TextBoxDailyOSales.Text = (0M).ToString("C2");
                TextBoxDailyCosts.Text = (0M).ToString("C2");
                TextBoxAnnualDistCost.Text = (0M).ToString("C2");
                TextBoxDailySFALabor.Text = (0M).ToString("C2");
                TextBoxDistPayRel.Text = (0M).ToString("P2");
                TextBoxNProdHours.Text = (0M).ToString("F2");
                TextBoxNNonProdHours.Text = (0M).ToString("F2");
                TextBoxDProdHours.Text = (0M).ToString("F2");
                TextBoxDNonProdHours.Text = (0M).ToString("F2");

                TextBoxDSpFnHours.Text = (0M).ToString("F2");
                TextBoxNSpFnHours.Text = (0M).ToString("F2");

                TextBoxFoodCPM.Text = (0M).ToString("C4");
                TextBoxPaperCleanCPM.Text = (0M).ToString("C4");

            }
        }


        protected void DropDownListRMs_DataBound(object sender, EventArgs e)
        {
            DropDownList DropDownListRMs = (DropDownList)sender;
            HiddenField HFRMName = (HiddenField)FindControlRecursive(FormViewDistInfo, "HFRMName");

            // in case of Inactive or missing RM which was added to the list - replace the selection to match RM
            if (HFRMName != null && HFRMName.Value != "" && DropDownListRMs.SelectedValue=="0")
            {

                foreach (ListItem it in DropDownListRMs.Items)
                {
                    if (it.Text == "- Select -")
                        continue;
                    else if(it.Text == HFRMName.Value)
                    {
                        DropDownListRMs.Items[0].Selected = false;
                        it.Selected = true;
                        break;
                    }
                }

            }

        }

        protected void LinkButtonContacts_Click(object sender, EventArgs e)
        {
            Response.Redirect("SetupDistrictContacts.aspx");
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlDataSourceProgramYears.DataBind();
            
            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int recordIndex = dvSql.Find(DDLProgramYears.SelectedValue);
                DataRowView drvSql = dvSql[recordIndex];
                HFBudgetYear.Value = drvSql["BudgetYear"].ToString().TrimEnd(' ');
            }
            else
            {
                HFBudgetYear.Value = "";
            }

        }
    }
}
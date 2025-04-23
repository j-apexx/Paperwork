using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class Days : Paperwork_Global
    {

        //static DateTime minDate;
        //static DateTime maxDate;
        static DateTime emptyDate;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");
            else
                LabelProgramYear.Text = Session["ProgramYear"].ToString();

            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;

            if (!IsPostBack)
            {
                VerifyAccess(true);
                if (Session["AccessLevel"] == null)
                    Response.Redirect("~/Default.aspx");



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
            else
            {
                VerifyAccess(false);
            }
        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (Session["ProgramYear"] == null)
                e.Command.Parameters["@ProgramYear"].Value = "";
            else
                e.Command.Parameters["@ProgramYear"].Value = Session["ProgramYear"].ToString();
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

                UpdatePanel1.Update();
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            //if (minDate == emptyDate)
            //    minDate = e.Day.Date;
            if (HFMinDate.Value == "")
                HFMinDate.Value = e.Day.Date.ToShortDateString();

            //LabelMinDate.Text = minDate.ToShortDateString();
            LabelMinDate.Text = HFMinDate.Value;

            //if (maxDate < e.Day.Date)
            //    maxDate = e.Day.Date;
            DateTime MaxDateVal = new DateTime();
            if (DateTime.TryParse(HFMaxDate.Value, out MaxDateVal) == false)
                MaxDateVal = new DateTime();

            if (MaxDateVal < e.Day.Date)
                HFMaxDate.Value = e.Day.Date.ToShortDateString();

            //LabelMaxDate.Text = maxDate.ToShortDateString();
            LabelMaxDate.Text = HFMaxDate.Value;

            CheckBox cbBKDay = new CheckBox();
            cbBKDay.Text = "Breakfast";
            cbBKDay.ID = "CBBreakfast" + e.Day.Date.ToShortDateString();
            if (!String.IsNullOrEmpty(Request.Form[cbBKDay.ID]))
                cbBKDay.Checked = true;

            CheckBox cbLNDay = new CheckBox();
            cbLNDay.Text = "Lunch";
            cbLNDay.ID = "CBLunch" + e.Day.Date.ToShortDateString();
            if (!String.IsNullOrEmpty(Request.Form[cbLNDay.ID]))
                cbLNDay.Checked = true;

            //New 9/21/2020
            CheckBox cbBSSO = new CheckBox();
            cbBSSO.Text = "SSO/SFSP Bkft";
            cbBSSO.ID = "CBBSSO" + e.Day.Date.ToShortDateString();
            if (!String.IsNullOrEmpty(Request.Form[cbBSSO.ID]))
                cbBSSO.Checked = true;

            CheckBox cbLSSO = new CheckBox();
            cbLSSO.Text = "SSO/SFSP Lnch";
            cbLSSO.ID = "CBLSSO" + e.Day.Date.ToShortDateString();
            if (!String.IsNullOrEmpty(Request.Form[cbLSSO.ID]))
                cbLSSO.Checked = true;

            bool overridden=false;
            LoadCheckBoxStates(ref cbBKDay, ref cbLNDay, e.Day.Date, out overridden, ref cbBSSO, ref cbLSSO);

            Literal literalbr1 = new Literal();
            literalbr1.Text = "<br/>";

            Literal literalbr2 = new Literal();
            literalbr2.Text = "<br/>";

            Literal literalbr3 = new Literal();
            literalbr3.Text = "<br/>";

            Literal divBoxStart = new Literal();
            if(overridden)
                divBoxStart.Text = "<div id='dayDiv' style='border:solid;border-width:2px; border-color:pink;padding-top:4px;'>";
            else
                divBoxStart.Text= "<div id='dayDiv' style='border:solid;border-width:2px; border-color:white;padding-top:4px;'>";
            Literal divBoxEnd = new Literal();
            divBoxEnd.Text = "</div>";

            
            e.Cell.Controls.Add(divBoxStart);
            //e.Cell.Controls.Add(literalbr1);

            e.Cell.Controls.Add(cbBKDay);
            e.Cell.Controls.Add(literalbr1);
            e.Cell.Controls.Add(cbLNDay);
            e.Cell.Controls.Add(literalbr2);
            e.Cell.Controls.Add(cbBSSO);
            e.Cell.Controls.Add(literalbr3);
            e.Cell.Controls.Add(cbLSSO);
            e.Cell.Controls.Add(divBoxEnd);
        }

        /// <summary>
        /// Checks the Days table for the specified day/dist combo and checks or unchecks the boxes
        /// </summary>
        /// <param name="cbBKDay">Breakfast Checkbox</param>
        /// <param name="cbLNDay">Lunch Checkbox</param>
        /// <param name="calDate">Date</param>
        private void LoadCheckBoxStates(ref CheckBox cbBKDay, ref CheckBox cbLNDay, DateTime calDate, out bool overridden, ref CheckBox cbBSSO, ref CheckBox cbLSSO)
        {
            overridden = false;//set default value

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryRetrieveDays = "SELECT [BreakfastDay], [LunchDay], [Locked], [Overridden], [BSSO],[LSSO] FROM [Days] " +
                                    " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND ServingDate=@ServingDate ";
            using (SqlCommand cmd = new SqlCommand(queryRetrieveDays, dbConnection))
            {

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

                SqlParameter dbParam_srvDate = new SqlParameter();
                dbParam_srvDate.ParameterName = "@ServingDate";
                dbParam_srvDate.Value = calDate;
                dbParam_srvDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_srvDate);

                try
                {
                    dbConnection.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        cbBKDay.Checked = (bool)rdr["BreakfastDay"];
                        cbBKDay.Enabled = !(bool)rdr["Locked"];
                        cbBKDay.ToolTip = (bool)rdr["Locked"] ? "Locked" : "Breakfast " + calDate.ToShortDateString();

                        cbLNDay.Checked = (bool)rdr["LunchDay"];
                        cbLNDay.Enabled = !(bool)rdr["Locked"];
                        cbLNDay.ToolTip = (bool)rdr["Locked"] ? "Locked" : "Lunch " + calDate.ToShortDateString();

                        cbBSSO.Checked = (bool)rdr["BSSO"];
                        cbBSSO.Enabled = !(bool)rdr["Locked"];
                        cbBSSO.ToolTip = (bool)rdr["Locked"] ? "Locked" : "SSO/SFSP Bkft " + calDate.ToShortDateString();

                        cbLSSO.Checked = (bool)rdr["LSSO"];
                        cbLSSO.Enabled = !(bool)rdr["Locked"];
                        cbLSSO.ToolTip = (bool)rdr["Locked"] ? "Locked" : "SSO/SFSP Lunch " + calDate.ToShortDateString();

                        overridden = (bool)rdr["Overridden"];

                    }
                    else
                    {
                        cbBKDay.Checked = false;
                        cbLNDay.Checked = false;
                        cbBSSO.Checked = false;
                        cbLSSO.Checked = false;
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error loading operating days record.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void ISave_Click(object sender, EventArgs e)
        {
            //Reset message
            LabelMessage.Text = string.Empty;
            LabelMessage.ForeColor = System.Drawing.Color.Black;

            //Breakfast
            string calBKeys = "";
            string[] selectedBDates = Array.Empty<string>();
            //Lunch
            string calLKeys = "";
            string[] selectedLDates = Array.Empty<string>();
            //SSO
            string calBSSOKeys = "";
            string[] selectedBSSODates = Array.Empty<string>();

            string calLSSOKeys = "";
            string[] selectedLSSODates = Array.Empty<string>();

            try
            {
                //Looping through all keys 
                for (int i = 0; i < Request.Form.Keys.Count; i++)
                {
                    //skip null keys
                    if (Request.Form.Keys[i] == null)
                        continue;

                    //Get only keys having "CBBreakfast" word
                    if (Request.Form.Keys[i].ToString().Contains("CBBreakfast"))
                        calBKeys += Request.Form.Keys[i].ToString() + ",";
                    //Get only keys having "CBLunch" word
                    if (Request.Form.Keys[i].ToString().Contains("CBLunch"))
                        calLKeys += Request.Form.Keys[i].ToString() + ",";
                    //Get only keys having "CBBSSO" word
                    if (Request.Form.Keys[i].ToString().Contains("CBBSSO"))
                        calBSSOKeys += Request.Form.Keys[i].ToString() + ",";
                    //Get only keys having "CBLSSO" word
                    if (Request.Form.Keys[i].ToString().Contains("CBLSSO"))
                        calLSSOKeys += Request.Form.Keys[i].ToString() + ",";
                }

                if (calBKeys != string.Empty)
                {
                    calBKeys = calBKeys.Trim(',');

                    selectedBDates = calBKeys.Split(',');

                    for (int i = 0; i < selectedBDates.Length; i++)
                    {
                        //Removing "CBBreakfast" word from date.
                        selectedBDates[i] = selectedBDates[i].Substring(11);
                    }
                }

                if (calLKeys != string.Empty)
                {
                    calLKeys = calLKeys.Trim(',');

                    selectedLDates = calLKeys.Split(',');

                    for (int i = 0; i < selectedLDates.Length; i++)
                    {
                        //Removing "CBLunch" word from date.
                        selectedLDates[i] = selectedLDates[i].Substring(7);
                    }
                }

                if (calBSSOKeys != string.Empty)
                {
                    calBSSOKeys = calBSSOKeys.Trim(',');

                    selectedBSSODates = calBSSOKeys.Split(',');

                    for (int i = 0; i < selectedBSSODates.Length; i++)
                    {
                        //Removing "CBSSO" word from date.
                        selectedBSSODates[i] = selectedBSSODates[i].Substring(6);
                    }
                }

                if (calLSSOKeys != string.Empty)
                {
                    calLSSOKeys = calLSSOKeys.Trim(',');

                    selectedLSSODates = calLSSOKeys.Split(',');

                    for (int i = 0; i < selectedLSSODates.Length; i++)
                    {
                        //Removing "CBSSO" word from date.
                        selectedLSSODates[i] = selectedLSSODates[i].Substring(6);
                    }
                }

                //Show selected dates in Lable control
                if (selectedBDates.Length > 0)
                    HFBDates.Value = string.Join(",", selectedBDates);
                else
                    HFBDates.Value = string.Empty;

                if (selectedLDates.Length > 0)
                    HFLDates.Value = string.Join(",", selectedLDates);
                else
                    HFLDates.Value = string.Empty;

                if (selectedBSSODates.Length > 0)
                    HFBSSODates.Value = string.Join(",", selectedBSSODates);
                else
                    HFBSSODates.Value = string.Empty;

                if (selectedLSSODates.Length > 0)
                    HFLSSODates.Value = string.Join(",", selectedLSSODates);
                else
                    HFLSSODates.Value = string.Empty;
            }
            catch(Exception ex)
            {
                LabelMessage.Text = ex.Message;
                LabelMessage.ForeColor = System.Drawing.Color.Red;
            }

            string errorDetail = "";

            if(SaveDaysRecords( ref errorDetail))
                ShowMessageIcon("Saved. ", MessageType.Success);
            else
                ShowMessageIcon("Errors encountered. " + errorDetail, MessageType.Error);
        }

        private bool SaveDaysRecords(ref string errorDetail)
        {
            bool saveSuccessful = true;
            //DateTime CurrentDate = minDate;
            DateTime CurrentDate = new DateTime();
            if (DateTime.TryParse(HFMinDate.Value, out CurrentDate) == false)
            {
                errorDetail = "Missing beginning date range.";
                return false;
            }

            int existingRecordCount = 0;

            DateTime MaxDateVal = new DateTime();
            if (DateTime.TryParse(HFMaxDate.Value, out MaxDateVal) == false)
            {
                errorDetail = "Missing ending date range.";
                return false;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            //Cycle through all the displayed calendar dates and update the serving days as needed
            while (CurrentDate <= MaxDateVal)//maxDate
            {
                byte bDay;
                byte lDay;
                byte bssoDay;
                byte lssoDay;
                byte Override=0;

                //Search hidden field values to see if they contain the current date which indicates whether or not the box is checked
                if (HFBDates.Value.IndexOf(CurrentDate.ToShortDateString()) >= 0)
                    bDay = 1;
                else
                    bDay = 0;

                if (HFLDates.Value.IndexOf(CurrentDate.ToShortDateString()) >= 0)
                    lDay = 1;
                else
                    lDay = 0;

                if (HFBSSODates.Value.IndexOf(CurrentDate.ToShortDateString()) >= 0)
                    bssoDay = 1;
                else
                    bssoDay = 0;

                if (HFLSSODates.Value.IndexOf(CurrentDate.ToShortDateString()) >= 0)
                    lssoDay = 1;
                else
                    lssoDay = 0;

                if (ServingDayOverride(bDay, lDay, CurrentDate))
                    Override = 1;

                //Check to see if a record already exists
                string checkForDateRecord = "SELECT count(*) FROM [Days] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND ServingDate=@ServingDate ";
                SqlConnection dbConnection = new SqlConnection(connectionString);
                using (SqlCommand cmd = new SqlCommand(checkForDateRecord, dbConnection))
                {
                    dbConnection.Open();
                    SqlParameter dbParam_ProgYear = new SqlParameter();
                    dbParam_ProgYear.ParameterName = "@ProgramYear";
                    dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
                    dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_ProgYear);

                    SqlParameter dbParam_dist = new SqlParameter();
                    dbParam_dist.ParameterName = "@DistrictID";
                    dbParam_dist.Value = DropDownListDistricts.SelectedValue;
                    dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_dist);

                    SqlParameter dbParam_srvDate = new SqlParameter();
                    dbParam_srvDate.ParameterName = "@ServingDate";
                    dbParam_srvDate.Value = CurrentDate;
                    dbParam_srvDate.DbType = System.Data.DbType.Date;
                    cmd.Parameters.Add(dbParam_srvDate);

                    try
                    {
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            int.TryParse(obj.ToString(), out existingRecordCount);
                        }
                    }
                    catch (SqlException ex)
                    {
                        saveSuccessful = false;
                        LabelMessage.Text = ex.Message;
                        LabelMessage.ForeColor = System.Drawing.Color.Red;
                        return saveSuccessful;
                    }
                    catch (Exception ex)
                    {
                        saveSuccessful = false;
                        LabelMessage.Text = ex.Message;
                        LabelMessage.ForeColor = System.Drawing.Color.Red;
                        return saveSuccessful;
                    }
                    finally
                    {
                        dbConnection.Close();
                    }
                }

                string updateCreateDayRecord = "";
                if (existingRecordCount == 0 )//Create new only if needed
                {
                    if ((bDay == 1 || lDay == 1 || bssoDay == 1 || lssoDay == 1 || Override == 1))
                    {
                        //Insert
                        updateCreateDayRecord = "INSERT INTO [Days] ([ProgramYear],[DistrictID],[ServingDate],[BreakfastDay],[LunchDay],[Overridden],[OverriddenBy],[Locked],[BSSO],[LSSO]) " +
                        " VALUES(@ProgramYear, @DistrictID, @ServingDate, @BreakfastDay,@LunchDay,@Overridden,@OverriddenBy,0,@BSSO,@LSSO)";//Creating new record, so Locked=0
                    }
                    else
                    {
                        updateCreateDayRecord = "";//Creating new record, so Locked=0
                    }
                    
                }
                else
                {
                    updateCreateDayRecord = "UPDATE [Days] SET BreakfastDay=@BreakfastDay, LunchDay=@LunchDay, Overridden=@Overridden, BSSO=@BSSO, LSSO=@LSSO, " +
                        //if not override - clear username. If override and username already specified - do not change, otherwise assign current username.
                        " OverriddenBy=CASE @Overridden WHEN 0 THEN '' ELSE  CASE WHEN OverriddenBy<>'' AND @OverriddenBy <>'' THEN OverriddenBy ELSE @OverriddenBy END END " +
                        " WHERE (ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND ServingDate=@ServingDate AND Locked=0)";//Force only unlocked records
                }

                if (updateCreateDayRecord != "")
                {

                    SqlConnection dbConnection2 = new SqlConnection(connectionString);
                    using (SqlCommand cmd2 = new SqlCommand(updateCreateDayRecord, dbConnection2))
                    {
                        dbConnection2.Open();

                        SqlParameter dbParam_ProgYear = new SqlParameter();
                        dbParam_ProgYear.ParameterName = "@ProgramYear";
                        dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
                        dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmd2.Parameters.Add(dbParam_ProgYear);

                        SqlParameter dbParam_dist = new SqlParameter();
                        dbParam_dist.ParameterName = "@DistrictID";
                        dbParam_dist.Value = DropDownListDistricts.SelectedValue;
                        dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                        cmd2.Parameters.Add(dbParam_dist);

                        SqlParameter dbParam_srvDate = new SqlParameter();
                        dbParam_srvDate.ParameterName = "@ServingDate";
                        dbParam_srvDate.Value = CurrentDate;
                        dbParam_srvDate.DbType = System.Data.DbType.Date;
                        cmd2.Parameters.Add(dbParam_srvDate);

                        SqlParameter dbParam_bDay = new SqlParameter();
                        dbParam_bDay.ParameterName = "@BreakfastDay";
                        dbParam_bDay.Value = bDay;
                        dbParam_bDay.DbType = System.Data.DbType.Byte;
                        cmd2.Parameters.Add(dbParam_bDay);

                        SqlParameter dbParam_lDay = new SqlParameter();
                        dbParam_lDay.ParameterName = "@LunchDay";
                        dbParam_lDay.Value = lDay;
                        dbParam_lDay.DbType = System.Data.DbType.Byte;
                        cmd2.Parameters.Add(dbParam_lDay);

                        SqlParameter dbParam_overridden = new SqlParameter();
                        dbParam_overridden.ParameterName = "@Overridden";
                        dbParam_overridden.Value = Override;
                        dbParam_overridden.DbType = System.Data.DbType.Byte;
                        cmd2.Parameters.Add(dbParam_overridden);

                        SqlParameter dbParam_overriddenBy = new SqlParameter();
                        dbParam_overriddenBy.ParameterName = "@OverriddenBy";
                        dbParam_overriddenBy.Value = Override == 1 ? Context.User.Identity.Name.ToUpper() : "";
                        dbParam_overriddenBy.DbType = System.Data.DbType.String;
                        cmd2.Parameters.Add(dbParam_overriddenBy);

                        SqlParameter dbParam_bssoDay = new SqlParameter();
                        dbParam_bssoDay.ParameterName = "@BSSO";
                        dbParam_bssoDay.Value = bssoDay;
                        dbParam_bssoDay.DbType = System.Data.DbType.Byte;
                        cmd2.Parameters.Add(dbParam_bssoDay);

                        SqlParameter dbParam_lssoDay = new SqlParameter();
                        dbParam_lssoDay.ParameterName = "@LSSO";
                        dbParam_lssoDay.Value = lssoDay;
                        dbParam_lssoDay.DbType = System.Data.DbType.Byte;
                        cmd2.Parameters.Add(dbParam_lssoDay);

                        try
                        {
                            object obj = cmd2.ExecuteNonQuery();


                        }
                        catch (SqlException ex)
                        {
                            saveSuccessful = false;
                            //RegisterDOMReadyScript("error message", "alert('Error saving the school header info (eligibility, enrollment...).');");
                        }
                        catch (Exception ex)
                        {
                            saveSuccessful = false;
                        }
                        finally
                        {
                            dbConnection2.Close();
                        }
                    }
                }


                //finally
                CurrentDate = CurrentDate.Date.AddDays(1);
            }

            return saveSuccessful;

        }

        /// <summary>
        /// Validates the breakfast and lunch checkboxes against the Recap records for the day
        /// </summary>
        /// <param name="bDay"></param>
        /// <param name="lDay"></param>
        /// <param name="currentDate"></param>
        private bool ServingDayOverride(byte bDay, byte lDay, DateTime currentDate)
        {
            bool overrideServingDays = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string checkForDateRecord = "SELECT ISNULL(MAX([BreakfastDay]),0) BreakfastDay, ISNULL(MAX([LunchDay]),0) LunchDay FROM [dbo].[RecapEntry] " +
                                        " WHERE[ProgramYear] = @ProgramYear AND[DistrictID] = @DistrictID AND[RecapDate] = @RecapDate ";
            SqlConnection dbConnection = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(checkForDateRecord, dbConnection))
            {
                dbConnection.Open();
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_dist = new SqlParameter();
                dbParam_dist.ParameterName = "@DistrictID";
                dbParam_dist.Value = DropDownListDistricts.SelectedValue;
                dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_dist);

                SqlParameter dbParam_srvDate = new SqlParameter();
                dbParam_srvDate.ParameterName = "@RecapDate";
                dbParam_srvDate.Value = currentDate;
                dbParam_srvDate.DbType = System.Data.DbType.Date;
                cmd.Parameters.Add(dbParam_srvDate);

                try
                {
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        if (((byte)rdr["BreakfastDay"] == bDay) && ((byte)rdr["LunchDay"] == lDay))
                            overrideServingDays = false;
                        else
                            overrideServingDays = true;
                    }
                    else
                    {
                        //record could not be read - doesn't exist?
                        if(bDay==0 && lDay==0)
                            overrideServingDays = false;
                        else
                            overrideServingDays = true;
                    }
                }
                catch (SqlException ex)
                {
                    LabelMessage.Text = ex.Message;
                    LabelMessage.ForeColor = System.Drawing.Color.Red;
                    return false;
                }
                catch (Exception ex)
                {
                    LabelMessage.Text = ex.Message;
                    LabelMessage.ForeColor = System.Drawing.Color.Red;
                    return false;
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return overrideServingDays;
        }

        protected void Calendar1_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            //Reset the min and max dates - these are used to only update the displayed dates on the calendar.
            HFMinDate.Value = "";
            HFMaxDate.Value = "";
            LabelMinDate.Text = "cleared";
            LabelMaxDate.Text = "cleared";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class Days : Paperwork_Global
    {
        //TODO - get start and end of dates displayed.
        //Scroll through all dates and compare to selected breakfast and lunch dates
        //Update each day as needed
        //If displayed / selected data does not match what is already in Days table - mark the record as Overridden with Username
        //Enforce Locks

        static DateTime minDate;
        static DateTime maxDate;
        static DateTime emptyDate;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");
            else
                LabelProgramYear.Text = Session["ProgramYear"].ToString();

            if (!IsPostBack)
            {
                VerifyAccess(true);
                if (Session["AccessLevel"] == null)
                    Response.Redirect("~/Default.aspx");

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
            else
            {
                VerifyAccess(false);
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

        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            if (minDate == emptyDate)
                minDate = e.Day.Date;

            if (maxDate < e.Day.Date)
                maxDate = e.Day.Date;


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
            bool overridden=false;
            LoadCheckBoxStates(ref cbBKDay, ref cbLNDay, e.Day.Date, out overridden);

            Literal literalbr1 = new Literal();
            literalbr1.Text = "<br/>";
            Literal literalbr2 = new Literal();
            literalbr2.Text = "<br/>";
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
            e.Cell.Controls.Add(literalbr2);
            e.Cell.Controls.Add(cbLNDay);
            e.Cell.Controls.Add(divBoxEnd);
        }

        /// <summary>
        /// Checks the Days table for the specified day/dist combo and checks or unchecks the boxes
        /// </summary>
        /// <param name="cbBKDay">Breakfast Checkbox</param>
        /// <param name="cbLNDay">Lunch Checkbox</param>
        /// <param name="calDate">Date</param>
        private void LoadCheckBoxStates(ref CheckBox cbBKDay, ref CheckBox cbLNDay, DateTime calDate, out bool overridden)
        {
            overridden = false;//set default value

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryRetrieveDays = "SELECT [BreakfastDay], [LunchDay], [Locked], [Overridden] FROM [Days] " +
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
                        cbBKDay.ToolTip = (bool)rdr["Locked"] ? "Locked" : "";

                        cbLNDay.Checked = (bool)rdr["LunchDay"];
                        cbLNDay.Enabled = !(bool)rdr["Locked"];
                        cbLNDay.ToolTip = (bool)rdr["Locked"] ? "Locked" : "";

                        overridden = (bool)rdr["Overridden"];

                    }
                    else
                    {
                        cbBKDay.Checked = false;
                        cbLNDay.Checked = false;
                    }

                }
                catch (Exception ex)
                {
                    //RegisterDOMReadyScript("error message", "alert('Error saving the school header info (eligibility, enrollment...).');");
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
                }

                if (calBKeys != string.Empty)
                {
                    calBKeys = calBKeys.Trim(',');

                    selectedBDates = calBKeys.Split(',');

                    for (int i = 0; i < selectedBDates.Length; i++)
                    {
                        //Removing "cal" word from date.
                        selectedBDates[i] = selectedBDates[i].Substring(11);
                    }
                }

                if (calLKeys != string.Empty)
                {
                    calLKeys = calLKeys.Trim(',');

                    selectedLDates = calLKeys.Split(',');

                    for (int i = 0; i < selectedLDates.Length; i++)
                    {
                        //Removing "cal" word from date.
                        selectedLDates[i] = selectedLDates[i].Substring(7);
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
            }
            catch(Exception ex)
            {
                LabelMessage.Text = ex.Message;
                LabelMessage.ForeColor = System.Drawing.Color.Red;
            }

            if(SaveDaysRecords())
                ShowMessageIcon("Saved. ", MessageType.Success);
            else
                ShowMessageIcon("Errors encountered. ", MessageType.Error);
        }

        private bool SaveDaysRecords()
        {
            bool saveSuccessful = true;
            DateTime CurrentDate = minDate;
            int existingRecordCount = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            //Cycle through all the displayed calendar dates and update the serving days as needed
            while (CurrentDate <= maxDate)
            {
                byte bDay;
                byte lDay;
                byte Override=0;

                if (HFBDates.Value.IndexOf(CurrentDate.ToShortDateString()) >= 0)
                    bDay = 1;
                else
                    bDay = 0;

                if (HFLDates.Value.IndexOf(CurrentDate.ToShortDateString()) >= 0)
                    lDay = 1;
                else
                    lDay = 0;

                if (ServingDayOverride(bDay, lDay, CurrentDate))
                    Override = 1;


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
                if (existingRecordCount == 0)//Create new if needed
                {
                    //Insert
                    updateCreateDayRecord = "INSERT INTO [Days] ([ProgramYear],[DistrictID],[ServingDate],[BreakfastDay],[LunchDay],[Overridden],[OverriddenBy],[Locked]) " +
                        " VALUES(@ProgramYear, @DistrictID, @ServingDate, @BreakfastDay,@LunchDay,@Overridden,@OverriddenBy,0)";//Creating new record, so Locked=0
                }
                else
                {
                    updateCreateDayRecord = "UPDATE [Days] SET BreakfastDay=@BreakfastDay, LunchDay=@LunchDay, Overridden=@Overridden, " +
                        //if not override - clear username. If override and username already specified - do not change, otherwise assign current username.
                        " OverriddenBy=CASE @Overridden WHEN 0 THEN '' ELSE  CASE WHEN OverriddenBy<>'' AND @OverriddenBy <>'' THEN OverriddenBy ELSE @OverriddenBy END END " +
                        " WHERE (ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND ServingDate=@ServingDate AND Locked=0)";//Force only unlocked records
                }

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
                    dbParam_overriddenBy.Value = Override==1? Context.User.Identity.Name.ToUpper() : "";
                    dbParam_overriddenBy.DbType = System.Data.DbType.String;
                    cmd2.Parameters.Add(dbParam_overriddenBy);

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
            minDate = emptyDate;
            maxDate = emptyDate;
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class DistrictPayrollEntry : Paperwork_Global
    {
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

                DateTime UseDate = DateTime.Now.Date;

                if (UseDate >= (DateTime)Session["ProgramYearStartDate"] && UseDate <= (DateTime)Session["ProgramYearEndDate"])
                { /*Do nothing */}
                else if (UseDate < (DateTime)Session["ProgramYearStartDate"])
                    UseDate = (DateTime)Session["ProgramYearStartDate"];
                else if (UseDate > (DateTime)Session["ProgramYearEndDate"])
                    UseDate = (DateTime)Session["ProgramYearEndDate"];

                TextBoxPayrollDate.Text = UseDate.ToShortDateString();

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

                PopulateSchoolDates();
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

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
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
            HFEnteredPayrollDates.Value = string.Empty;
            HFLockedPayrollDates.Value = string.Empty;
            StringBuilder sbPayrollDates = new StringBuilder();
            StringBuilder sbLockedDates = new StringBuilder();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT DISTINCT PayDate, MAX(CONVERT(INT, Locked)) Locked FROM [Payroll] "
                +" WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND PayDate>=@StartDate AND PayDate<=@EndDate "
                +" GROUP BY PayDate";

            if (DropDownListDistricts.Items.Count == 0)
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
                SqlDataReader dataReader = dbCommand.ExecuteReader();

                while(dataReader.Read())
                {
                    if((int)dataReader["Locked"]==0)
                    {
                        if(sbPayrollDates.Length==0)
                            sbPayrollDates.Append(((DateTime)dataReader["PayDate"]).ToShortDateString());
                        else
                        {
                            sbPayrollDates.Append(",");
                            sbPayrollDates.Append(((DateTime)dataReader["PayDate"]).ToShortDateString());
                        }
                    }
                    else
                    {
                        if (sbLockedDates.Length == 0)
                            sbLockedDates.Append(((DateTime)dataReader["PayDate"]).ToShortDateString());
                        else
                        {
                            sbLockedDates.Append(",");
                            sbLockedDates.Append(((DateTime)dataReader["PayDate"]).ToShortDateString());
                        }
                    }
                }

                HFEnteredPayrollDates.Value = sbPayrollDates.ToString();
                HFLockedPayrollDates.Value = sbLockedDates.ToString();

                if (dataReader != null)
                    dataReader.Close();
            }
            catch (Exception)
            {

            }
            finally
            {
                dbConnection.Close();
            }

        }

        protected void GridViewPayroll_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                SqlDataSourcePayroll.Insert();
            }
        }

        protected void SqlDataSourcePayroll_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and Paydate are already set
            DropDownList DropDownListFEmps = (DropDownList)GridViewPayroll.FooterRow.FindControl("DropDownListFEmps");
            DropDownList DropDownListFSchools = (DropDownList)GridViewPayroll.FooterRow.FindControl("DropDownListFSchools");
            DropDownList DropDownListFCodes = (DropDownList)GridViewPayroll.FooterRow.FindControl("DropDownListFCodes");
            TextBox TextBoxFHours = (TextBox)GridViewPayroll.FooterRow.FindControl("TextBoxFHours");
            TextBox TextBoxFRate = (TextBox)GridViewPayroll.FooterRow.FindControl("TextBoxFRate");
            TextBox TextBoxFOther = (TextBox)GridViewPayroll.FooterRow.FindControl("TextBoxFOther");
            TextBox TextBoxFHealth = (TextBox)GridViewPayroll.FooterRow.FindControl("TextBoxFHealth");

            if (Int32.TryParse(DropDownListFEmps.SelectedValue, out Int32 OutEmpNum))
                e.Command.Parameters["@EmployeeNum"].Value = OutEmpNum;
            else
            {
                e.Cancel = true;return;
            }

            if (Int16.TryParse(DropDownListFSchools.SelectedValue, out Int16 OutSchNum))
                e.Command.Parameters["@SchoolNum"].Value = OutSchNum;
            else
            {
                e.Cancel = true; return;
            }

            if (Int16.TryParse(DropDownListFCodes.SelectedValue, out Int16 OutPayCode))
                e.Command.Parameters["@PayCode"].Value = OutPayCode;
            else
            {
                e.Cancel = true; return;
            }


            Decimal.TryParse(TextBoxFHours.Text, out Decimal HoursOut);
            e.Command.Parameters["@Hours"].Value = HoursOut;

            Decimal.TryParse(TextBoxFRate.Text, out Decimal RateOut);
            e.Command.Parameters["@Rate"].Value = RateOut;

            Decimal.TryParse(TextBoxFOther.Text, out Decimal OtherOut);
            e.Command.Parameters["@OtherPay"].Value = OtherOut;

            Decimal.TryParse(TextBoxFHealth.Text, out Decimal HealthOut);
            e.Command.Parameters["@Health"].Value = HealthOut;

        }

        protected void GridViewPayroll_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["Counter"].ToString()=="0" )
                e.Row.Visible = false;
        }

        protected void SqlDataSourcePayroll_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if(e.AffectedRows>0)
            {
                ShowMessageIcon("Saved", MessageType.Success);
                //Refocus to allow quicker entry of new records
                DropDownList DropDownListFEmps = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListFEmps");
                if (DropDownListFEmps != null)
                    DropDownListFEmps.Focus();
            }
            else if(e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        /// <summary>
        /// IWhen the Paycode is selected or changed changes - the Rate might change as well
        /// </summary>
        /// <param name="sender">PayCodes DropDownList</param>
        /// <param name="e"></param>
        protected void DropDownListPayCodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Boolean WeAreEditing = false;
            DropDownList DDLPayCodes = (DropDownList)sender;
            if (DDLPayCodes == null || DDLPayCodes.SelectedIndex == -1)
                return;

            if (DDLPayCodes.ID == "DropDownListECodes")
                WeAreEditing = true;
            
            if (WeAreEditing)
            {
                DropDownList DDLEmps = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListEmps");
                TextBox TextBoxRate = (TextBox)FindControlRecursive(GridViewPayroll, "TextBoxEditRate");

                if (DDLEmps != null && DDLEmps.Items.Count > 1 && DDLEmps.SelectedIndex > 0 && TextBoxRate !=null)
                {
                    PopulatePayRate(int.Parse(DDLEmps.SelectedValue), int.Parse(DDLPayCodes.SelectedValue), ref TextBoxRate);
                    TextBoxRate.Focus();
                }

            }
            else //we are working with new record in the footer
            {
                DropDownList DDLEmps = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListFEmps");
                TextBox TextBoxRate = (TextBox)FindControlRecursive(GridViewPayroll, "TextBoxFRate");

                if (DDLEmps != null && DDLEmps.Items.Count > 1 && DDLEmps.SelectedIndex > 0 && TextBoxRate != null)
                {
                    PopulatePayRate(int.Parse(DDLEmps.SelectedValue), int.Parse(DDLPayCodes.SelectedValue), ref TextBoxRate);
                    TextBoxRate.Focus();
                }
            }
        }

        /// <summary>
        /// When the employee changes or is selected - Select the default school, and populate the rate is the Paycode is selected.
        /// </summary>
        /// <param name="sender">Employees DropDownList</param>
        /// <param name="e"></param>
        protected void DropDownListEmps_SelectedIndexChanged(object sender, EventArgs e)
        {
            Boolean WeAreEditing = false;

            DropDownList DDLEmps = (DropDownList)sender;
            if (DDLEmps == null || DDLEmps.SelectedIndex == -1)
                return;

            if (DDLEmps.ID == "DropDownListEmps")
                WeAreEditing = true;

            int EmpNum = Int32.Parse(DDLEmps.SelectedValue);
            string Dist = DropDownListDistricts.SelectedValue;

            Int16 outSchoolNum = 0;
            decimal outRate1 = 0M;
            decimal outRate2 = 0M;
            decimal outRate3 = 0M;
            decimal outRate4 = 0M;
            decimal outRate5 = 0M;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT SchoolNum, Rate1, Rate2, Rate3, Rate4, Rate5 FROM [Employees] "
                + " WHERE EmployeeNum=@EmployeeNum AND DistrictID=@DistrictID ";

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_DistID = new SqlParameter();
            dbParam_DistID.ParameterName = "@DistrictID";
            dbParam_DistID.Value = Dist;
            dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
            dbCommand.Parameters.Add(dbParam_DistID);

            SqlParameter dbParam_empNum = new SqlParameter();
            dbParam_empNum.ParameterName = "@EmployeeNum";
            dbParam_empNum.Value = EmpNum;
            dbParam_empNum.DbType = System.Data.DbType.Int32;
            dbCommand.Parameters.Add(dbParam_empNum);

            try
            {
                dbConnection.Open();
                SqlDataReader dataReader = dbCommand.ExecuteReader();

                if (dataReader.Read())
                {
                    outSchoolNum = (Int16)dataReader["SchoolNum"];
                    outRate1 = (decimal)dataReader["Rate1"];
                    outRate2 = (decimal)dataReader["Rate2"];
                    outRate3 = (decimal)dataReader["Rate3"];
                    outRate4 = (decimal)dataReader["Rate4"];
                    outRate5 = (decimal)dataReader["Rate5"];
                }

                if (dataReader != null)
                    dataReader.Close();
            }
            catch (Exception)
            {

            }
            finally
            {
                dbConnection.Close();
            }

            if (WeAreEditing)
            {
                DropDownList DropDownListSchools = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListSchools");
                SelectSchool(ref DropDownListSchools, outSchoolNum);

                DropDownList DDLPayCodes = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListECodes");
                TextBox TextBoxRate = (TextBox)FindControlRecursive(GridViewPayroll, "TextBoxEditRate");

                if (DDLPayCodes != null && DDLPayCodes.Items.Count>1 && DDLPayCodes.SelectedIndex> 0 && TextBoxRate != null)
                {
                    PopulatePayRate(int.Parse(DDLEmps.SelectedValue), int.Parse(DDLPayCodes.SelectedValue), ref TextBoxRate);
                }

                TextBox TextBoxHours = (TextBox)FindControlRecursive(GridViewPayroll, "TextBoxEditHours");
                if (TextBoxHours != null && (TextBoxHours.Text == "0" || TextBoxHours.Text == String.Empty))
                    TextBoxHours.Focus();
            }
            else //we are working with new record in the footer
            {
                DropDownList DropDownListFSchools = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListFSchools");
                SelectSchool(ref DropDownListFSchools, outSchoolNum);

                DropDownList DDLPayCodes = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListFCodes");
                TextBox TextBoxRate = (TextBox)FindControlRecursive(GridViewPayroll, "TextBoxFRate");

                if (DDLPayCodes != null && DDLPayCodes.Items.Count > 1 && DDLPayCodes.SelectedIndex > 0 && TextBoxRate != null)
                {
                    PopulatePayRate(int.Parse(DDLEmps.SelectedValue), int.Parse(DDLPayCodes.SelectedValue), ref TextBoxRate);
                }

                TextBox TextBoxHours = (TextBox)FindControlRecursive(GridViewPayroll, "TextBoxFHours");
                if (TextBoxHours != null && (TextBoxHours.Text == "0" || TextBoxHours.Text == String.Empty))
                    TextBoxHours.Focus();
            }


        }

        /// <summary>
        /// When the employee is changed - this procedure sets the employees default school
        /// </summary>
        /// <param name="DDLSchools">Dropdown reference for schools</param>
        /// <param name="schoolInt">School Num Integer</param>
        protected void SelectSchool (ref DropDownList DDLSchools, Int16 schoolInt)
        {
            if (DDLSchools == null)
                return;

            if (DDLSchools.Items.Count == 0)
                return;

            if (DDLSchools.Items.FindByValue(schoolInt.ToString()) != null)
                DDLSchools.SelectedValue = schoolInt.ToString();
        }

        protected void PopulatePayRate(int EmpNum, int PayCodeNum, ref TextBox TextBoxPayRate)
        {
            if (EmpNum == 0 || PayCodeNum == 0 || TextBoxPayRate == null || DropDownListDistricts.Items.Count==0 || DropDownListDistricts.SelectedIndex==-1)
                return;

            string Dist = DropDownListDistricts.SelectedValue;

            decimal outRate = 0M;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT CASE [RateTier] WHEN 1 THEN [Rate1] "
                                + " WHEN 2 THEN[Rate2] "
                                + " WHEN 3 THEN[Rate3] "
                                + " WHEN 4 THEN[Rate4] "
                                + " WHEN 5 THEN[Rate5] END Rate "
                                + " FROM [Employees] E "
                                + " INNER JOIN [DistrictPayCodes] DPC ON DPC.DistrictID = E.DistrictID "
                                + " WHERE E.EmployeeNum=@EmployeeNum AND E.DistrictID=@DistrictID AND DPC.ProgramYear=@ProgramYear AND PayCode=@PayCode";

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_DistID = new SqlParameter();
            dbParam_DistID.ParameterName = "@DistrictID";
            dbParam_DistID.Value = Dist;
            dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
            dbCommand.Parameters.Add(dbParam_DistID);

            SqlParameter dbParam_empNum = new SqlParameter();
            dbParam_empNum.ParameterName = "@EmployeeNum";
            dbParam_empNum.Value = EmpNum;
            dbParam_empNum.DbType = System.Data.DbType.Int32;
            dbCommand.Parameters.Add(dbParam_empNum);

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
            dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
            dbCommand.Parameters.Add(dbParam_ProgYear);

            SqlParameter dbParam_payCode = new SqlParameter();
            dbParam_payCode.ParameterName = "@PayCode";
            dbParam_payCode.Value = PayCodeNum;
            dbParam_payCode.DbType = System.Data.DbType.Int32;
            dbCommand.Parameters.Add(dbParam_payCode);


            try
            {
                dbConnection.Open();
                object pcodeObj=dbCommand.ExecuteScalar();
                if(pcodeObj!=null)
                {
                    Decimal.TryParse(pcodeObj.ToString(), out outRate);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                dbConnection.Close();
            }
            TextBoxPayRate.Text = outRate.ToString();
        }

        protected void GridViewPayroll_PreRender(object sender, EventArgs e)
        {
            // These register the postback only when focus leaves the dropdowns allowing users to use keyboard and type or arrow between values
            // without the index change script running everytime
            DropDownList DropDownListEmps = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListEmps");
            if(DropDownListEmps!=null)
            {
                DropDownListEmps.Attributes.Add("onchange", "__doPostBack('DropDownListEmps','');");
            }


            DropDownList DropDownListFEmps = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListFEmps");
            if (DropDownListFEmps != null)
            {
                DropDownListFEmps.Attributes.Add("onchange", "__doPostBack('DropDownListFEmps','');");
            }

            DropDownList DropDownListECodes = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListECodes");
            if (DropDownListECodes != null)
            {
                DropDownListECodes.Attributes.Add("onchange", "__doPostBack('DropDownListECodes','');");
            }


            DropDownList DropDownListFCodes = (DropDownList)FindControlRecursive(GridViewPayroll, "DropDownListFCodes");
            if (DropDownListFCodes != null)
            {
                DropDownListFCodes.Attributes.Add("onchange", "__doPostBack('DropDownListFCodes','');");
            }



        }
    }
}
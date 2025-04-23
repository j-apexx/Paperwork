using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class Purchases : Paperwork_Global
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

                if (Session["AccessLevel"] == null)
                    Response.Redirect("~/Default.aspx");

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


                //Apply default date range filter
                DDLDateRanges.SelectedIndex = 0;
                DDLDateRanges_SelectedIndexChanged(null, null);

                //Set default invoice date and post date
                DetailsViewInsert.DataBind();
                Control TextBoxInvoiceDate = FindControlRecursive(DetailsViewInsert, "TextBoxInvoiceDate");
                ((TextBox)TextBoxInvoiceDate).Text = DateTime.Now.ToShortDateString();
                UpdateWeekEnding();

                //focus on the first data entry field
                Control ddlVendors = FindControlRecursive(DetailsViewInsert, "ddlVendors");
                ddlVendors.Focus();

                GetPostingDates();

                if (isSFSP())
                    LabelHeader2.Text = "SFSP Weekending Dates:";


            }
            else
            {
                VerifyAccess(false);
            }

        }

        /// <summary>
        /// Calculates the week ending date for the second date field
        /// </summary>
        protected void UpdateWeekEnding()
        {
            Control TextBoxInvoiceDate = FindControlRecursive(DetailsViewInsert, "TextBoxInvoiceDate");
            Control TextBoxWEDate = FindControlRecursive(DetailsViewInsert, "TextBoxWEDate");

            if (Session["LastWeekEndDate"] != null)
            {
                ((TextBox)TextBoxWEDate).Text = Session["LastWeekEndDate"].ToString();
                return;
            }


            try
            {
                DateTime invDate = new DateTime();
                DateTime postDate;

                if (DateTime.TryParse(((TextBox)TextBoxInvoiceDate).Text, out invDate))
                {
                    if (invDate.DayOfWeek <= DayOfWeek.Friday)
                    {
                        postDate = invDate.AddDays(5 - (int)invDate.DayOfWeek);
                        ((TextBox)TextBoxWEDate).Text = postDate.ToShortDateString();
                        ((TextBox)TextBoxInvoiceDate).Text = invDate.ToShortDateString();//just updating formatting
                    }
                    else
                    {
                        postDate = invDate.AddDays(6);
                        ((TextBox)TextBoxWEDate).Text = invDate.AddDays(6).ToShortDateString();
                        ((TextBox)TextBoxInvoiceDate).Text = invDate.ToShortDateString();//just updating formatting
                    }

                    //if posting date is before program year start - set to starting date.
                    if (DateTime.TryParse(HFProgYearStartDate.Value, out DateTime stDate) && postDate != null && (stDate > postDate))
                    {
                        if (stDate.DayOfWeek <= DayOfWeek.Friday)
                        {
                            postDate = stDate.AddDays(5 - (int)stDate.DayOfWeek);
                            ((TextBox)TextBoxWEDate).Text = postDate.ToShortDateString();
                        }
                        else
                        {
                            postDate = stDate.AddDays(6);
                            ((TextBox)TextBoxWEDate).Text = postDate.ToShortDateString();
                        }
                    }
                    else if (DateTime.TryParse(HFProgYearEndDate.Value, out DateTime endDate) && postDate != null && (endDate < postDate))
                    {
                        //if it's friday, then it's okay
                        if (endDate.DayOfWeek == DayOfWeek.Friday)
                        {
                            ((TextBox)TextBoxWEDate).Text = endDate.ToShortDateString();
                        }
                        //if less than friday - we have to go backwards to the previous friday
                        else if (endDate.DayOfWeek < DayOfWeek.Friday)
                        {
                            postDate = endDate.AddDays(-2 - (int)endDate.DayOfWeek);
                            ((TextBox)TextBoxWEDate).Text = postDate.ToShortDateString();
                        }
                        //
                        else
                        {
                            //unlike above - for end date we have to go backwards.
                            postDate = endDate.AddDays(-1);
                            ((TextBox)TextBoxWEDate).Text = postDate.ToShortDateString();
                        }
                    }

                }

            }
            catch
            { }
        }

        protected void GetPostingDates()
        { 
            for (int i=1;i<=12;i++)
            {
                Control lbl = FindControlRecursive(this, "Label" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3) + "Dates");
                if (lbl != null)
                    ((Label)lbl).Visible = false;
            }


            SqlDataSourcePostingDates.DataBind();
            DataView dv = (DataView)SqlDataSourcePostingDates.Select(DataSourceSelectArguments.Empty);
            foreach (DataRow dr in dv.Table.Rows)
            {
                int monthKeyIntInt = int.Parse(dr["MonthKey"].ToString());
                switch (monthKeyIntInt)
                {
                    case 1:
                        LabelJanDates.Text = dr["OpenPostingDates"].ToString();
                        LabelJanDates.Visible = true;
                        break;
                    case 2:
                        LabelFebDates.Text = dr["OpenPostingDates"].ToString();
                        LabelFebDates.Visible = true;
                        break;
                    case 3:
                        LabelMarDates.Text = dr["OpenPostingDates"].ToString();
                        LabelMarDates.Visible = true;
                        break;
                    case 4:
                        LabelAprDates.Text = dr["OpenPostingDates"].ToString();
                        LabelAprDates.Visible = true;
                        break;
                    case 5:
                        LabelMayDates.Text = dr["OpenPostingDates"].ToString();
                        LabelMayDates.Visible = true;
                        break;
                    case 6:
                        LabelJunDates.Text = dr["OpenPostingDates"].ToString();
                        LabelJunDates.Visible = true;
                        break;
                    case 7:
                        LabelJulDates.Text = dr["OpenPostingDates"].ToString();
                        LabelJulDates.Visible = true;
                        break;
                    case 8:
                        LabelAugDates.Text = dr["OpenPostingDates"].ToString();
                        LabelAugDates.Visible = true;
                        break;
                    case 9:
                        LabelSepDates.Text = dr["OpenPostingDates"].ToString();
                        LabelSepDates.Visible = true;
                        break;
                    case 10:
                        LabelOctDates.Text = dr["OpenPostingDates"].ToString();
                        LabelOctDates.Visible = true;
                        break;
                    case 11:
                        LabelNovDates.Text = dr["OpenPostingDates"].ToString();
                        LabelNovDates.Visible = true;
                        break;
                    case 12:
                        LabelDecDates.Text = dr["OpenPostingDates"].ToString();
                        LabelDecDates.Visible = true;
                        break;
                    default:
                        break;

                }
            }

            PopulatePostingDates();
        }

        /// <summary>
        /// The following creates an array of entered dates which is then used with datepickers
        /// </summary>
        protected void PopulatePostingDates()
        {
            //clear out the hidden fields entry
            HFAvailPostingDates.Value = string.Empty;

            StringBuilder sbPostDates = new StringBuilder();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            //NOTE: the serving date is actually the EditCheckDate - not the RecapDate
            //Initially they are the same, but revisions of a past date match the EditCheckDate, and the update numbers post to the RecapDate
            string queryString = "SELECT OpenPostDate FROM [PurchasesPostDates] WHERE ProgramYear=@ProgramYear";

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
            dbParam_ProgYear.DbType = DbType.String;
            dbCommand.Parameters.Add(dbParam_ProgYear);

            try
            {
                dbConnection.Open();
                SqlDataReader dataReader = dbCommand.ExecuteReader();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    if(sbPostDates.Length==0)
                        sbPostDates.Append(((DateTime)dataReader["OpenPostDate"]).ToShortDateString());
                    else
                    {
                        sbPostDates.Append(",");
                        sbPostDates.Append(((DateTime)dataReader["OpenPostDate"]).ToShortDateString());
                    }

                }

                HFAvailPostingDates.Value = sbPostDates.ToString();

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


        protected void UpdateInsertTotal(object sender, EventArgs e)
        {
            decimal ttl = 0M;
            decimal ttlGroc = 0M;
            decimal ttlPaper = 0M;
            decimal ttlClean = 0M;
            decimal ttlOther = 0M;
            decimal ttlMilk = 0M;

            TextBox TextBoxGroceries = (TextBox)FindControlRecursive(DetailsViewInsert, "TextBoxGroceries");
            TextBox TextBoxPaper = (TextBox)FindControlRecursive(DetailsViewInsert, "TextBoxPaper");
            TextBox TextBoxCleaning = (TextBox)FindControlRecursive(DetailsViewInsert, "TextBoxCleaning");
            TextBox TextBoxOther = (TextBox)FindControlRecursive(DetailsViewInsert, "TextBoxOther");
            Label LabelTotal = (Label)FindControlRecursive(DetailsViewInsert, "LabelTotal");
            TextBox TextBoxMilk = (TextBox)FindControlRecursive(DetailsViewInsert, "TextBoxMilk");

            CultureInfo CI = CultureInfo.CurrentCulture;

            _ = decimal.TryParse(TextBoxGroceries.Text, out ttlGroc);
            TextBoxGroceries.Text = ttlGroc.ToString("F2", CI);
            _ = decimal.TryParse(TextBoxPaper.Text, out ttlPaper);
            TextBoxPaper.Text = ttlPaper.ToString("F2", CI);
            _ = decimal.TryParse(TextBoxCleaning.Text, out ttlClean);
            TextBoxCleaning.Text = ttlClean.ToString("F2", CI);
            _ = decimal.TryParse(TextBoxOther.Text, out ttlOther);
            TextBoxOther.Text = ttlOther.ToString("F2", CI);
            _ = decimal.TryParse(TextBoxMilk.Text, out ttlMilk);
            TextBoxMilk.Text = ttlMilk.ToString("F2", CI);



            ttl = ttlGroc + ttlPaper + ttlClean + ttlOther + ttlMilk;

            LabelTotal.Text = ttl.ToString("C2", CI);
        }

        protected void CustomValidatorOtherCat_ServerValidate(object source, ServerValidateEventArgs args)
        {
            Control TextBoxOther = FindControlRecursive(DetailsViewInsert, "TextBoxOther");
            Control ddlOtherCats = FindControlRecursive(DetailsViewInsert, "ddlOtherCats");
            decimal ttlOther = 0M;

            if (decimal.TryParse(((TextBox)TextBoxOther).Text, out ttlOther) == false)
                ttlOther = 0M;

            if (ttlOther != 0M && (((DropDownList)ddlOtherCats).SelectedValue == "NONE" || ((DropDownList)ddlOtherCats).SelectedValue == "_"))
            {
                if (args != null)
                    args.IsValid = false;
            }
            else if (ttlOther == 0M)
                ((DropDownList)ddlOtherCats).SelectedValue = "NONE";
        }

        protected void CustomValidatorOtherCat_GridViewServerValidate(object source, ServerValidateEventArgs args)
        {
            TextBox TextBoxEditOther = (TextBox)FindControlRecursive(GridViewPurchases, "TextBoxEditOther");
            DropDownList ddlOtherCatsEdit = (DropDownList)FindControlRecursive(GridViewPurchases, "ddlOtherCatsEdit");
            decimal ttlOther = 0M;

            if (decimal.TryParse(TextBoxEditOther.Text, out ttlOther) == false)
                ttlOther = 0M;

            if (ttlOther != 0M && ( (ddlOtherCatsEdit.SelectedValue == "NONE") || (ddlOtherCatsEdit.SelectedValue == "_") ))
            {
                TextBoxEditOther.ForeColor = System.Drawing.Color.DimGray;
                TextBoxEditOther.BackColor = System.Drawing.Color.Yellow;
                if (args != null)
                    args.IsValid = false;
            }
            else
            {
                TextBoxEditOther.ForeColor = System.Drawing.Color.DimGray;
                TextBoxEditOther.BackColor = System.Drawing.Color.White;

                if (ttlOther == 0M)
                    ddlOtherCatsEdit.SelectedValue = "NONE";
            }

        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox dateTextBox = ((TextBox)sender);

            // If just digits are typed - convert to standard date
            NormalizeSimplifiedDate(ref dateTextBox);






            if (dateTextBox.ID == "TextBoxWEDate" || dateTextBox.ID == "TextBoxEditWEDate")
            {

                if (DateTime.TryParse(dateTextBox.Text, out DateTime currPostDate) == false)
                {
                    string JqueryHiglight = "$( '[id$= " + dateTextBox.ID + "]' ).addClass('highlight');";
                    ShowDialogPlus("W/E Date Error", MessageType.Error, "Entered W/E Date could not be parsed. Please Re-enter.", "", JqueryHiglight);
                    dateTextBox.Focus();
                    return;
                }

                if(HFAvailPostingDates.Value.IndexOf(currPostDate.ToShortDateString())==-1)
                {
                    string JqueryHiglight = "$( '[id$= " + dateTextBox.ID + "]' ).addClass('highlight');";
                    ShowDialogPlus("W/E Date Error", MessageType.Error, "Entered W/E Date is not an allowable W/E date. Please Re-enter.", "", JqueryHiglight);
                    dateTextBox.Focus();
                    return;
                }



                // Ensure the date falls in the Program Year. Validate weekend entries.
                ValidateTrxEntryDate(ref dateTextBox, true, HFProgYearStartDate.Value, HFProgYearEndDate.Value);

                //if (DateTime.TryParse(dateTextBox.Text, out DateTime dateResult))
                //{
                    //if not end of week, and not end of month - display warning
                    if (currPostDate.DayOfWeek != DayOfWeek.Friday && new DateTime(currPostDate.AddMonths(1).Year, currPostDate.AddMonths(1).Month, 1).AddDays(-1) != currPostDate)
                    {
                        string JqueryPost = "$( '[id$= " + dateTextBox.ID + "]' ).addClass('highlight');";
                        ShowDialogPlus("W/E Date Alert", MessageType.Warning, "Entered W/E Date is not a Friday and is not the last day of the month.", "", JqueryPost);
                    }
                    else
                    {
                        RegisterDOMReadyScript("Normal W/E", "$( '[id$= " + dateTextBox.ID + "]' ).removeClass('highlight');");
                        Session["LastWeekEndDate"] = dateTextBox.Text;
                    }
                //}
            }
            else if (dateTextBox.ID == "TextBoxInvoiceDate")//only update for invoice date field
                UpdateWeekEnding();


        }

        protected void SqlDataSourcePurchases_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            try { Session["LastPurchaseDate"] = ((DateTime)e.Command.Parameters["@InvoiceDate"].Value).ToShortDateString(); } catch { }

            try { Session["LastWeekEndDate"] = ((DateTime)e.Command.Parameters["@PostDate"].Value).ToShortDateString(); } catch { }

        }

        /// <summary>
        /// If another invoice has been entered, prepopulate with the recently used dates and focus the vendor dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DetailsViewInsert_PreRender(object sender, EventArgs e)
        {
            DetailsView dv = (DetailsView)sender;

            //this should only happen after a save. Set value and focus Vendor DDL
            if (Session["LastPurchaseDate"] != null)
            {
                Control TextBoxInvoiceDate = FindControlRecursive(DetailsViewInsert, "TextBoxInvoiceDate");
                ((TextBox)TextBoxInvoiceDate).Text = Session["LastPurchaseDate"].ToString();
                Session.Remove("LastPurchaseDate");

                Control ddlVendors = FindControlRecursive(DetailsViewInsert, "ddlVendors");
                ddlVendors.Focus();
            }

            if (Session["LastWeekEndDate"] != null)
            {
                Control TextBoxWEDate = FindControlRecursive(DetailsViewInsert, "TextBoxWEDate");
                ((TextBox)TextBoxWEDate).Text = Session["LastWeekEndDate"].ToString();
            }

            if(isTOOLBOX())
            {
                foreach(DataControlField df in dv.Fields)
                {
                    if (df.HeaderText == "School")
                        df.HeaderText=df.HeaderText.Replace("School", "Building");
                }
            }


        }

        protected void DetailsViewInsert_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            ShowMessage("Transmittal added.", MessageType.Success);
        }

        protected void DetailsViewInsert_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            DetailsView dv = (DetailsView)sender;
            if(dv == null)
                return;

            DateTime PostDate;
            if (DateTime.TryParse(e.Values["PostDate"].ToString(), out PostDate) == false)
            {
                e.Cancel = true;
                return;
            }

            string ProgYear = DDLProgramYears.SelectedValue;

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(PostDate, ProgYear, Session["CurrentDistrict"].ToString(), "Purchases") == false)
            {
                e.Cancel = true;
                ShowDialog("Error", MessageType.Error, "W/E date falls within a closed period. Add was cancelled.", "");
                return;
            }
            else if (HFAvailPostingDates.Value.IndexOf(PostDate.ToShortDateString()) == -1)
            {
                e.Cancel = true;
                ShowDialog("Error", MessageType.Error, "W/E date specified is not an allowable date. Transmittal add was cancelled.", "");
                return;
            }

            DropDownList ddlOtherCats = (DropDownList)FindControlRecursive(dv, "ddlOtherCats");

            if (ddlOtherCats==null)
                return;
            else if(ddlOtherCats.Items.Count==0)
            {
                e.Cancel = true;
                ShowDialog("Error", MessageType.Error, "Transmittal Other Categories is not properly initialized. Contact support to have them check on missing categories. Transmittal add was cancelled.", "");
                return;
            }
        }

        protected void GridViewPurchases_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //GridView gv = (GridView)sender;
            //GridViewRow gvr = gv.Rows[e.RowIndex];

            if (DateTime.TryParse(e.NewValues["PostDate"].ToString(), out DateTime NewPostDate) == false) { }
            string ProgYear = DDLProgramYears.SelectedValue;
            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(NewPostDate, ProgYear, Session["CurrentDistrict"].ToString(), "Purchases") == false)
            {
                e.Cancel = true;
                ShowDialog("Error", MessageType.Error, "W/E date falls within a closed period. Update was cancelled.", "");
                return;
            }
            else if (HFAvailPostingDates.Value.IndexOf(NewPostDate.ToShortDateString()) == -1)
            {
                e.Cancel = true;
                ShowDialog("Error", MessageType.Error, "W/E date specified is not an allowable date. Update was cancelled.", "");
                return;
            }

            //DropDownList ddlOtherCatsEdit = (DropDownList)FindControlRecursive(GridViewPurchases, "ddlOtherCatsEdit");
            //if(ddlOtherCatsEdit != null)
            //{
            //    e.NewValues["OtherCategoryID"] = ddlOtherCatsEdit.SelectedValue;
            //}
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Print/PrintTransmittals.aspx");
        }

        protected void ddlVendorsEdit_DataBound(object sender, EventArgs e)
        {
            DropDownList ddlVendorsEdit = (DropDownList)sender;

            //Remove the blank vendor from the control - " -SELECT- " has a value of "_"
            ListItem liRemove = ddlVendorsEdit.Items.FindByValue("_");
            if (liRemove != null)
                ddlVendorsEdit.Items.Remove(liRemove);

            //if the vendor record is not available anymore, or was inactivated - try to  find it an add it to the dropdown
            HiddenField HFSelectedVendorID = (HiddenField)FindControlRecursive(GridViewPurchases, "HFSelectedVendorID");
            HiddenField HFVendorName = (HiddenField)FindControlRecursive(GridViewPurchases, "HFVendorName");
            Label LabelInactiveNote = (Label)FindControlRecursive(GridViewPurchases, "LabelInactiveNote");
            if (HFSelectedVendorID == null || HFVendorName == null)
                return;

            HFSelectedVendorID.DataBind();
            HFVendorName.DataBind();

            if (HFSelectedVendorID.Value != string.Empty)
            {
                if (ddlVendorsEdit.Items.FindByValue(HFSelectedVendorID.Value) == null)
                {
                    //add the item to the list
                    ddlVendorsEdit.Items.Add(new ListItem("*" + HFVendorName.Value, HFSelectedVendorID.Value));

                    if (LabelInactiveNote != null)
                        LabelInactiveNote.Visible = true;
                }
            }

            ddlVendorsEdit.SelectedValue = HFSelectedVendorID.Value;
        }

        protected void SqlDataSourcePurchases_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Due to the way inactive or missing dropdown values are being handled. Need to map the DDL to the parameter.
            DropDownList ddlVendorsEdit = (DropDownList)FindControlRecursive(GridViewPurchases, "ddlVendorsEdit");
            DropDownList ddlOtherCatsEdit = (DropDownList)FindControlRecursive(GridViewPurchases, "ddlOtherCatsEdit");
            DropDownList DDLProgramTypeEdit = (DropDownList)FindControlRecursive(GridViewPurchases, "DDLProgramTypeEdit");

            if (ddlVendorsEdit == null)
                return;

            if (ddlOtherCatsEdit == null)
                return;

            if (DDLProgramTypeEdit == null)
                return;


            e.Command.Parameters["@VendorID"].Value = ddlVendorsEdit.SelectedValue;
            e.Command.Parameters["@OtherCategoryID"].Value = ddlOtherCatsEdit.SelectedValue;
            e.Command.Parameters["@ProgramType"].Value = DDLProgramTypeEdit.SelectedValue;
        }

        protected void DDLDateRanges_SelectedIndexChanged(object sender, EventArgs e)
        {
            HFBeginningDate.Value = DateTime.Now.AddDays(-int.Parse(DDLDateRanges.SelectedValue)).ToShortDateString();
        }


        protected void ddlVendors_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkForDuplicateInvoice();
        }

        protected void TextBoxInvoice_TextChanged(object sender, EventArgs e)
        {
            checkForDuplicateInvoice();
        }

        protected void checkForDuplicateInvoice()
        {
            string InvoiceNumber = ((TextBox)FindControlRecursive(DetailsViewInsert, "TextBoxInvoice")).Text;
            string VendorID = ((DropDownList)FindControlRecursive(DetailsViewInsert, "ddlVendors")).SelectedValue;

            if (string.IsNullOrEmpty(InvoiceNumber) || string.IsNullOrEmpty(VendorID))
                return;

            if (PurchasesDoesInvExist(DDLProgramYears.SelectedValue, Session["CurrentDistrict"].ToString(), VendorID, InvoiceNumber))
            {
                ShowMessageByName("Invoice already exists for this vendor.", MessageType.Warning, "invoiceexists_alert_container", 20000);
            }
        }

        protected void ISearch_Click(object sender, EventArgs e)
        {
            TextBoxClientSearch.Focus();
        }

        protected void IClear_Click(object sender, EventArgs e)
        {
            TextBoxClientSearch.Text = string.Empty;
        }

        protected void GridViewPurchases_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Unlock")
            {
                if (Int64.TryParse(e.CommandArgument.ToString(), out Int64 counterInt64) == false) { };
                UnlockPurchase(counterInt64);
                GridViewPurchases.DataBind();
                //GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                //gvr.DataBind();
            }


        }

        private void UnlockPurchase(long counter)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            //string UnlockQuery = "UPDATE [Purchases] SET [Imported]=0, [Locked]=0 WHERE [Counter]=@Counter";
            string UnlockQuery = "UPDATE [Purchases] SET [ExpToAcctng]=0, [Locked]=0 WHERE [Counter]=@Counter";
            using (SqlCommand cmd = new SqlCommand(UnlockQuery, dbConnection))
            {
                #region KeyVals
                SqlParameter dbParam_Counter = new SqlParameter();
                dbParam_Counter.ParameterName = "@Counter";
                dbParam_Counter.Value = counter;
                dbParam_Counter.DbType = System.Data.DbType.Int64;
                cmd.Parameters.Add(dbParam_Counter);

                #endregion

                try
                {
                    dbConnection.Open();
                    object recordcount = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error unlocking invoice.", "");
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void ddlOtherCatsEdit_DataBound(object sender, EventArgs e)
        {
            //Check to see if a previously allowed category was used and needs to be added for editing purposes
            DropDownList ddlOtherCatsEdit = (DropDownList)sender;

            HiddenField HFOtherPurchCatID = (HiddenField)FindControlRecursive(GridViewPurchases, "HFOtherPurchCatID");
            HiddenField HFOtherPurchCatName = (HiddenField)FindControlRecursive(GridViewPurchases, "HFOtherPurchCatName");
            Label LabelInactiveCat = (Label)FindControlRecursive(GridViewPurchases, "LabelInactiveCat");

            if (HFOtherPurchCatID == null || HFOtherPurchCatName == null)
                return;

            HFOtherPurchCatID.DataBind();
            HFOtherPurchCatName.DataBind();

            if (HFOtherPurchCatID.Value != string.Empty)
            {
                if (ddlOtherCatsEdit.Items.FindByValue(HFOtherPurchCatID.Value) == null)
                {
                    //add the item to the list
                    ddlOtherCatsEdit.Items.Add(new ListItem("*" + HFOtherPurchCatName.Value, HFOtherPurchCatID.Value));

                    if (LabelInactiveCat != null)
                        LabelInactiveCat.Visible = true;
                }
            }

            ddlOtherCatsEdit.SelectedValue = HFOtherPurchCatID.Value;

        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;

            SqlDataSourceProgramYears.DataBind();

            //Program Year has changed - remove the related session quick-entry variables
            Session.Remove("LastPurchaseDate");
            Session.Remove("LastWeekEndDate");

            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int recordIndex = dvSql.Find(DDLProgramYears.SelectedValue);
                if (recordIndex != -1)
                {
                    DataRowView drvSql = dvSql[recordIndex];
                    HFProgYearStartDate.Value = drvSql["StartDate"].ToString();
                    HFProgYearEndDate.Value = drvSql["EndDate"].ToString();
                }
                else
                {
                    HFProgYearStartDate.Value = "";
                    HFProgYearEndDate.Value = "";
                }
            }
            else
            {
                HFProgYearStartDate.Value = "";
                HFProgYearEndDate.Value = "";
            }

            GetPostingDates();

        }

        protected void DDLProgramType_DataBound(object sender, EventArgs e)
        {
            DropDownList DDLProgramType = (DropDownList)sender;
            if (DDLProgramType == null)
                return;
            if (DDLProgramType.Items.Count > 0)
            {
                ListItem liSchoolYear = DDLProgramType.Items.FindByValue("School Year");
                if (liSchoolYear != null)
                    DDLProgramType.SelectedValue = liSchoolYear.Value;

            }
        }

        protected void DDLProgramTypeEdit_DataBound(object sender, EventArgs e)
        {
            HiddenField HFProgramType = (HiddenField)FindControlRecursive(GridViewPurchases, "HFProgramType");
            DropDownList DDLProgramTypeEdit = (DropDownList)sender;

            if (DDLProgramTypeEdit == null || HFProgramType == null)
                return;

            HFProgramType.DataBind();

            if (HFProgramType.Value.Length != 0)
            {
                if (DDLProgramTypeEdit.Items.FindByValue(HFProgramType.Value) == null)
                {
                    //add the item to the list
                    DDLProgramTypeEdit.Items.Add(new ListItem(HFProgramType.Value, HFProgramType.Value));

                }
            }

            DDLProgramTypeEdit.SelectedValue = HFProgramType.Value;

        }

        protected void SqlDataSourcePurchases_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            //Replace SQL Keyword issue which would break filter
            if (e.ParameterValues["SearchString"] != null)
                e.ParameterValues["SearchString"] = TextBoxClientSearch.Text.Replace("'", "''").Replace("%", "[%]");
        }

        protected void GridViewPurchases_PreRender(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            if (isTOOLBOX())
            {
                
                foreach (DataControlField df in gv.Columns)
                {
                    if (df.HeaderText == "School")
                        df.HeaderText = df.HeaderText.Replace("School", "Building");
                }
            }

        }

        protected void ddlOtherCats_DataBinding(object sender, EventArgs e)
        {
            DropDownList ddlOtherCats = (DropDownList)sender;
            if (isTOOLBOX())
            {
                ddlOtherCats.DataSourceID = "SqlDataSourceOtherCatsToolbox";
            }
        }

        //Needed to handle and add inactivated schools
        protected void DropDownListSchools_Load(object sender, EventArgs e)
        {
            //Load runs before Binding - need to use this to add
            DropDownList DDLSchoolList = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)FindParentControlOfType(DDLSchoolList, "GridViewRow");
            
            if(gvr == null || gvr.DataItem==null)
                return;

            string selSchoolNum = ((Int16)DataBinder.Eval(gvr.DataItem, "SchoolNum")).ToString();
            string selSchoolName = (string)DataBinder.Eval(gvr.DataItem, "SchoolName");

            if (DDLSchoolList == null || string.IsNullOrEmpty(selSchoolNum) || string.IsNullOrEmpty(selSchoolName))
                return;



            bool RecordFound = false;

            //Check to see if the commissary is in the list
            SqlDataSource sds = DDLSchoolList.DataSourceObject as SqlDataSource;
            sds.DataBind(); //Bind the dropdownlist
            if(sds.DataSourceMode == SqlDataSourceMode.DataSet)
            {
                DataSet ds = (DataSet)sds.Select(DataSourceSelectArguments.Empty);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["SchoolNum"].ToString() == selSchoolNum)
                    {
                        RecordFound = true;
                        break;
                    }
                }
            }
            else if(sds.DataSourceMode == SqlDataSourceMode.DataReader)
            {
                SqlDataReader dr = (SqlDataReader)sds.Select(DataSourceSelectArguments.Empty);
                while (dr.Read())
                {
                    if (dr["SchoolNum"].ToString() == selSchoolNum)
                    {
                        RecordFound = true;
                        break;
                    }
                }
            }


            if (DDLSchoolList.Items.FindByValue(selSchoolNum) == null && RecordFound==false)
            {
                //add the commissary to the list
                DDLSchoolList.AppendDataBoundItems = true;
                DDLSchoolList.Items.Add(new ListItem("*" + selSchoolName, selSchoolNum));
            }
            //else
            //{
            //    DDLSchoolList.AppendDataBoundItems = false;
            //}

        }
    }
}
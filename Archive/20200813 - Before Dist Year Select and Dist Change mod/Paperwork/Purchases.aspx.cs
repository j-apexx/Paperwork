using System;
using System.Data;
using System.Data.SqlClient;
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
                    else if(DateTime.TryParse(HFProgYearEndDate.Value, out DateTime endDate) && postDate != null && (endDate < postDate))
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

            decimal.TryParse(TextBoxGroceries.Text, out ttlGroc);
            decimal.TryParse(TextBoxPaper.Text, out ttlPaper);
            decimal.TryParse(TextBoxCleaning.Text, out ttlClean);
            decimal.TryParse(TextBoxOther.Text, out ttlOther);
            decimal.TryParse(TextBoxMilk.Text, out ttlMilk);

            ttl = ttlGroc + ttlPaper + ttlClean + ttlOther + ttlMilk;

            LabelTotal.Text = ttl.ToString("F2");
        }

        protected void CustomValidatorOtherCat_ServerValidate(object source, ServerValidateEventArgs args)
        {
            Control TextBoxOther = FindControlRecursive(DetailsViewInsert, "TextBoxOther");
            Control ddlOtherCats = FindControlRecursive(DetailsViewInsert, "ddlOtherCats");
            decimal ttlOther = 0M;

            decimal.TryParse(((TextBox)TextBoxOther).Text, out ttlOther);

            if (ttlOther != 0M && ((DropDownList)ddlOtherCats).SelectedValue == "NONE")
                args.IsValid = false;

        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox dateTextBox = ((TextBox)sender);

            // If just digits are typed - convert to standard date
            NormalizeSimplifiedDate(ref dateTextBox);

            // Ensure the date falls in the Program Year. Validate weekend entries.
            ValidateTrxEntryDate(ref dateTextBox, true, HFProgYearStartDate.Value, HFProgYearEndDate.Value);

            if (dateTextBox.ID == "TextBoxWEDate" || dateTextBox.ID == "TextBoxEditWEDate")
            {
                

                if(DateTime.TryParse(dateTextBox.Text, out DateTime dateResult))
                {
                    //if not end of week, and not end of month - display warning
                    if (dateResult.DayOfWeek != DayOfWeek.Friday && new DateTime(dateResult.AddMonths(1).Year, dateResult.AddMonths(1).Month, 1).AddDays(-1) != dateResult)
                    {
                        RegisterDOMReadyScript("Alert W/E", "alert('Entered W/E Date is not a Friday and is not the last day of the month.');$( '[id *= " + dateTextBox.ID + "]' ).addClass('highlight');");
                    }
                    else
                    {
                        RegisterDOMReadyScript("Normal W/E", "$( '[id *= " + dateTextBox.ID + "]' ).removeClass('highlight');");
                        Session["LastWeekEndDate"] = dateTextBox.Text;
                    }
                }
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


        }

        protected void DetailsViewInsert_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            ShowMessage("Transmittal added.", MessageType.Success);
        }

        protected void DetailsViewInsert_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            //Check to ensure the dates fall in an open year
            //TextBox TextBoxInvoiceDate = (TextBox)FindControlRecursive(DetailsViewInsert, "TextBoxInvoiceDate");

            //DateTime InvoiceDate;
            //DateTime.TryParse(e.Values["InvoiceDate"].ToString(), out InvoiceDate);
            //if (InvoiceDate == null)
            //{
            //    e.Cancel = true;
            //    return;
            //}

            // 1/17/2020 - Tom wants to allow Invoice dates to be within closed periods - just enforce the posting date.
            //if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(InvoiceDate) == false)
            //{
            //    e.Cancel = true;
            //    RegisterDOMReadyScript("error message", "alert('Invoice date falls within a closed period. Add was cancelled.');");
            //    return;
            //}

            DateTime PostDate;
            DateTime.TryParse(e.Values["PostDate"].ToString(), out PostDate);
            if (PostDate == null)
            {
                e.Cancel = true;
                return;
            }

            string ProgYear = DDLProgramYears.SelectedValue;

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(PostDate, ProgYear) == false)
            {
                e.Cancel = true;
                RegisterDOMReadyScript("error message", "alert('W/E date falls within a closed period. Add was cancelled.');");
                return;
            }
        }

        protected void GridViewPurchases_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // 1/17/2020 - Tom wants to allow Invoice dates to be within closed periods - just enforce the posting date.
            //DateTime.TryParse(e.NewValues["InvoiceDate"].ToString(), out DateTime NewInvoiceDate);
            ////Check to see the new date is within an open period
            //if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(NewInvoiceDate) == false)
            //{
            //    e.Cancel = true;
            //    RegisterDOMReadyScript("error message", "alert('Invoice date falls within a closed period. Update was cancelled.');");
            //    return;
            //}

            DateTime.TryParse(e.NewValues["PostDate"].ToString(), out DateTime NewPostDate);
            string ProgYear = DDLProgramYears.SelectedValue;
            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(NewPostDate, ProgYear) == false)
            {
                e.Cancel = true;
                RegisterDOMReadyScript("error message", "alert('W/E date falls within a closed period. Update was cancelled.');");
                return;
            }
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Print/PrintTransmittals.aspx");
        }

        protected void ddlVendorsEdit_DataBound(object sender, EventArgs e)
        {
            //if the vendor record is not available anymore, or was inactivated - try to  find it an add it to the dropdown
            DropDownList ddlVendorsEdit = (DropDownList)sender;

            HiddenField HFSelectedVendorID = (HiddenField)FindControlRecursive(GridViewPurchases, "HFSelectedVendorID");
            HiddenField HFVendorName = (HiddenField)FindControlRecursive(GridViewPurchases, "HFVendorName");
            Label LabelInactiveNote = (Label)FindControlRecursive(GridViewPurchases, "LabelInactiveNote");
            if (HFSelectedVendorID == null || HFVendorName==null)
                return;

            HFSelectedVendorID.DataBind();
            HFVendorName.DataBind();

            if (HFSelectedVendorID.Value!=string.Empty)
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

            if (ddlVendorsEdit == null)
                return;

            if (ddlOtherCatsEdit == null)
                return;


            e.Command.Parameters["@VendorID"].Value = ddlVendorsEdit.SelectedValue;
            e.Command.Parameters["@OtherCategoryID"].Value = ddlOtherCatsEdit.SelectedValue;
        }

        protected void DDLDateRanges_SelectedIndexChanged(object sender, EventArgs e)
        {
            HFBeginningDate.Value = DateTime.Now.AddDays(-int.Parse(DDLDateRanges.SelectedValue)).ToShortDateString();
        }


        private bool InvoiceExists(string InvoiceNumber, string VendorID)
        {
            bool InvoiceExistsAlready = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string InvoiceQuery;
            InvoiceQuery = "SELECT COUNT(*) FROM [Purchases] " +
                "WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@InvoiceNumber AND VendorID=@VendorID ";
            using (SqlCommand cmd = new SqlCommand(InvoiceQuery, dbConnection))
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = DDLProgramYears.SelectedValue;// Session["ProgramYear"].ToString();
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = Session["CurrentDistrict"].ToString();
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_InvoiceNum = new SqlParameter();
                dbParam_InvoiceNum.ParameterName = "@InvoiceNumber";
                dbParam_InvoiceNum.Value = InvoiceNumber;
                dbParam_InvoiceNum.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_InvoiceNum);

                SqlParameter dbParam_VendorID = new SqlParameter();
                dbParam_VendorID.ParameterName = "@VendorID";
                dbParam_VendorID.Value = VendorID;
                dbParam_VendorID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_VendorID);
                #endregion

                try
                {
                    dbConnection.Open();
                    object recordcount = cmd.ExecuteScalar();
                    if (recordcount != null && (int)recordcount > 0)
                        InvoiceExistsAlready = true;
                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error checking for duplicate Invoices.');");
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return InvoiceExistsAlready;
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

            if (InvoiceExists(InvoiceNumber, VendorID))
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
            if (e.CommandName == "Unlock" )
            {
                Int64.TryParse(e.CommandArgument.ToString(), out Int64 counterInt64);
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

            string UnlockQuery = "UPDATE [Purchases] SET [Imported]=0, [Locked]=0 WHERE [Counter]=@Counter";
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
                    RegisterDOMReadyScript("error message", "alert('Error unlocking Invoice.');");
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

        }
    }
}
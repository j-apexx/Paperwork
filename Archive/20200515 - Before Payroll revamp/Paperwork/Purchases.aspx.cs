using System;
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

            if(!IsPostBack)
            {
                VerifyAccess(true);
                if (Session["AccessLevel"] == null)
                    Response.Redirect("~/Default.aspx");

                if (Session["ProgramYear"] == null)
                    Response.Redirect("~/Default.aspx");

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

                //Update HiddenFields which are used to constrain the DatePickers
                if (Session["ProgramYearStartDate"] != null)
                    HFProgYearStartDate.Value = Session["ProgramYearStartDate"].ToString();
                if (Session["ProgramYearEndDate"] != null)
                    HFProgYearEndDate.Value = Session["ProgramYearEndDate"].ToString();
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
                if (DateTime.TryParse(((TextBox)TextBoxInvoiceDate).Text, out invDate))
                {
                    if (invDate.DayOfWeek <= DayOfWeek.Friday)
                    {
                        ((TextBox)TextBoxWEDate).Text = invDate.AddDays(5 - (int)invDate.DayOfWeek).ToShortDateString();
                        ((TextBox)TextBoxInvoiceDate).Text = invDate.ToShortDateString();//just updating formatting
                    }
                    else
                    {
                        ((TextBox)TextBoxWEDate).Text = invDate.AddDays(6).ToShortDateString();
                        ((TextBox)TextBoxInvoiceDate).Text = invDate.ToShortDateString();//just updating formatting
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

            Control TextBoxGroceries = FindControlRecursive(DetailsViewInsert, "TextBoxGroceries");
            Control TextBoxPaper = FindControlRecursive(DetailsViewInsert, "TextBoxPaper");
            Control TextBoxCleaning = FindControlRecursive(DetailsViewInsert, "TextBoxCleaning");
            Control TextBoxOther = FindControlRecursive(DetailsViewInsert, "TextBoxOther");
            Control LabelTotal = FindControlRecursive(DetailsViewInsert, "LabelTotal");
            Control TextBoxMilk = FindControlRecursive(DetailsViewInsert, "TextBoxMilk");

            decimal.TryParse(((TextBox)TextBoxGroceries).Text, out ttlGroc);
            decimal.TryParse(((TextBox)TextBoxPaper).Text, out ttlPaper);
            decimal.TryParse(((TextBox)TextBoxCleaning).Text, out ttlClean);
            decimal.TryParse(((TextBox)TextBoxOther).Text, out ttlOther);
            decimal.TryParse(((TextBox)TextBoxMilk).Text, out ttlMilk);

            ttl = ttlGroc + ttlPaper + ttlClean + ttlOther + ttlMilk;

            ((Label)LabelTotal).Text = ttl.ToString("F2");
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
            ValidateTrxEntryDate(ref dateTextBox, true);

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
            TextBox TextBoxInvoiceDate = (TextBox)FindControlRecursive(DetailsViewInsert, "TextBoxInvoiceDate");

            DateTime InvoiceDate;
            DateTime.TryParse(e.Values["InvoiceDate"].ToString(), out InvoiceDate);
            if (InvoiceDate == null)
            {
                e.Cancel = true;
                return;
            }

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

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(PostDate) == false)
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
            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(NewPostDate) == false)
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
            DropDownList ddlVendorsEdit = (DropDownList)FindControlRecursive(GridViewPurchases, "ddlVendorsEdit");

            if (ddlVendorsEdit == null)
                return;

            e.Command.Parameters["@VendorID"].Value = ddlVendorsEdit.SelectedValue;
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
                dbParam_ProgYear2.Value = Session["ProgramYear"].ToString();
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
    }
}
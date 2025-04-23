using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
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

            if (dateTextBox.ID == "TextBoxWEDate")
                Session["LastWeekEndDate"] = dateTextBox.Text;
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

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(InvoiceDate) == false)
            {
                e.Cancel = true;
                RegisterDOMReadyScript("error message", "alert('Invoice date falls within a closed period. Add was cancelled.');");
                return;
            }

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
            DateTime.TryParse(e.NewValues["InvoiceDate"].ToString(), out DateTime NewInvoiceDate);
            //Check to see the new date is within an open period
            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && IsDateOpenForEntry(NewInvoiceDate) == false)
            {
                e.Cancel = true;
                RegisterDOMReadyScript("error message", "alert('Invoice date falls within a closed period. Update was cancelled.');");
                return;
            }

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
    }
}
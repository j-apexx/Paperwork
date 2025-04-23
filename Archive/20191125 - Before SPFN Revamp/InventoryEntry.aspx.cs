using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class InventoryEntry : Paperwork_Global
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

            HFStartDate.Value = ((DateTime)Session["ProgramYearStartDate"]).ToShortDateString();
            HFEndDate.Value = ((DateTime)Session["ProgramYearEndDate"]).ToShortDateString();

            if (!IsPostBack)
            {
                //TrxMonth.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
            }
        }

        protected void TextboxYearMonth_TextChanged(object sender, EventArgs e)
        {
            int Year = 0;
            int Month = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            TextBox TextBoxFYearmonth = (TextBox)sender;

            try
            {
                startDate = DateTime.Parse(HFStartDate.Value);
                endDate = DateTime.Parse(HFEndDate.Value);
            }
            catch(Exception ex)//session values not set?
            {
                Response.Redirect("~/default.aspx");
            }

            //If we cannot parse the year and month - set to current year month
            if (int.TryParse(TextBoxFYearmonth.Text.Substring(0, 4), out Year) == false || int.TryParse(TextBoxFYearmonth.Text.Substring(5, 2), out Month) == false)
            {
                TextBoxFYearmonth.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0');
                Year = DateTime.Now.Year;
                Month = DateTime.Now.Month;
            }

            DateTime testDate = new DateTime(Year, Month, 1);
            //if date falls before the beginning or after the end of the program year - set to limits
            if(testDate< startDate)
            {
                TextBoxFYearmonth.Text = startDate.Year.ToString() + "-" + startDate.Month.ToString().PadLeft(2, '0');
                Year = startDate.Year;
                Month = startDate.Month;
            }
            else if(testDate > endDate)
            {
                TextBoxFYearmonth.Text = endDate.Year.ToString() + "-" + endDate.Month.ToString().PadLeft(2, '0');
                Year = endDate.Year;
                Month = endDate.Month;
            }


        }

        protected void CBOpen_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox CBOpen = (CheckBox)sender;
            TextBox TextBoxFYearmonth = (TextBox)FindControlRecursive(GridViewInventory, "TextBoxFYearmonth");
            //RegularExpressionValidator RegexTrxMonth = (RegularExpressionValidator)FindControlRecursive(GridViewInventory, "RegexTrxMonth");

            if (CBOpen.Checked)
            {
                //Set the YearMonth TextBox to *OPEN*
                TextBoxFYearmonth.Text = "*OPEN*";
                TextBoxFYearmonth.Enabled = false;
            }
            else
            {
                //Clear the YearMonth Textbox and allow normal datepicker functionality
                TextBoxFYearmonth.Text = "";
                TextBoxFYearmonth.Enabled = true;
            }
        }

        protected void GridViewInventory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceInventory.Insert();
                }
                catch(InvalidOperationException ex)
                {
                    ShowMessageIcon("Error", MessageType.Error);
                }
            }
        }

        protected void GridViewInventory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["YearMonth"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceInventory_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null)
                return;

            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // Program Year and District are already set from Session or page dropdown
            TextBox TextBoxFYearmonth = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFYearmonth");
            TextBox TextBoxFFood = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFFood");
            TextBox TextBoxFPaper = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFPaper");
            TextBox TextBoxFCleaning = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFCleaning");
            TextBox TextBoxFMilk = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFMilk");
            TextBox TextBoxFUSDAProtOpen = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAProtOpen");
            TextBox TextBoxFUSDAProtRcvd = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAProtRcvd");
            TextBox LabelFUSDAProtIUse = (TextBox)GridViewInventory.FooterRow.FindControl("LabelFUSDAProtIUse");
            TextBox TextBoxFUSDAProtTrns = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAProtTrns");
            TextBox TextBoxFUSDAProtEnd = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAProtEnd");
            TextBox TextBoxFUSDAGrocOpen = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocOpen");
            TextBox TextBoxFUSDAGrocRcvd = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocRcvd");
            TextBox TextBoxFUSDAGrocIUse = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocIUse");
            TextBox TextBoxFUSDAGrocTrns = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocTrns");
            TextBox TextBoxFUSDAGrocEnd = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFUSDAGrocEnd");
            TextBox TextBoxFComments = (TextBox)GridViewInventory.FooterRow.FindControl("TextBoxFComments");

            e.Command.Parameters["@YearMonth"].Value = TextBoxFYearmonth.Text;

            if (Decimal.TryParse(TextBoxFFood.Text, out Decimal outFood))
                e.Command.Parameters["@Groceries"].Value = outFood;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFPaper.Text, out Decimal outPaper))
                e.Command.Parameters["@Paper"].Value = outPaper;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFCleaning.Text, out Decimal outClean))
                e.Command.Parameters["@Cleaning"].Value = outClean;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFMilk.Text, out Decimal outMilk))
                e.Command.Parameters["@Milk"].Value = outMilk;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAProtOpen.Text, out Decimal outProtOpen))
                e.Command.Parameters["@USDAProteinOpen"].Value = outProtOpen;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAProtRcvd.Text, out Decimal outProtRcvd))
                e.Command.Parameters["@USDAProteinRcvd"].Value = outProtRcvd;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(LabelFUSDAProtIUse.Text, out Decimal outProtIUse))
                e.Command.Parameters["@USDAProteinInstUse"].Value = outProtIUse;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAProtTrns.Text, out Decimal outProtTrans))
                e.Command.Parameters["@USDAProteinTrans"].Value = outProtTrans;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAProtEnd.Text, out Decimal outProtEnd))
                e.Command.Parameters["@USDAProteinEnding"].Value = outProtEnd;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocOpen.Text, out Decimal outGrocOpen))
                e.Command.Parameters["@USDAGroceryOpen"].Value = outGrocOpen;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocRcvd.Text, out Decimal outGrocRcvd))
                e.Command.Parameters["@USDAGroceryRcvd"].Value = outGrocRcvd;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocIUse.Text, out Decimal outGrocIUse))
                e.Command.Parameters["@USDAGroceryInstUse"].Value = outGrocIUse;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocTrns.Text, out Decimal outGrocTrans))
                e.Command.Parameters["@USDAGroceryTrans"].Value = outGrocTrans;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFUSDAGrocEnd.Text, out Decimal outGrocEnd))
                e.Command.Parameters["@USDAGroceryEnding"].Value = outGrocEnd;
            else
            {
                e.Cancel = true; return;
            }

            e.Command.Parameters["@Comments"].Value = TextBoxFComments.Text;



        }

        protected void SqlDataSourceInventory_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {


            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);
            }
            else if (e.Exception != null)
            {
                if (e.Exception.Message.IndexOf("Duplicate") >= 0)
                {
                    ShowMessageIcon("Error-Duplicate", MessageType.Error);
                    e.ExceptionHandled = true;
                }
                else
                {
                    ShowMessageIcon("SQL Error:" + e.Exception.Message, MessageType.Error);
                    e.ExceptionHandled = true;
                }
            }


        }

        protected void SqlDataSourceInventory_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Deleted", MessageType.Success);
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        protected void SqlDataSourceInventory_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Updated", MessageType.Success);
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }
    }
}
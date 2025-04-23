using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class OtherSales : Paperwork_Global
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
            }
        }

        protected void GridViewOtherSales_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                SqlDataSourceOtherSalesEntry.Insert();
            }
        }

        protected void GridViewOtherSales_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;

        }

        protected void SqlDataSourceOtherSalesEntry_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Saved", MessageType.Success);

                //Refocus to allow quicker entry of new records
                //TextBox TextBoxFDate = (TextBox)FindControlRecursive(GridViewOtherSales, "TextBoxFDate");
                //if (TextBoxFDate != null)
                //    TextBoxFDate.Focus();
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        protected void SqlDataSourceOtherSalesEntry_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Updated", MessageType.Success);
            }
        }

        protected void SqlDataSourceOtherSalesEntry_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                ShowMessageIcon("Deleted", MessageType.Success);

        }

        protected void SqlDataSourceOtherSalesEntry_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            TextBox TextBoxFDate = (TextBox)GridViewOtherSales.FooterRow.FindControl("TextBoxFDate");
            TextBox TextBoxFAmount = (TextBox)GridViewOtherSales.FooterRow.FindControl("TextBoxFAmount");
            DropDownList DDLFSalesCats = (DropDownList)GridViewOtherSales.FooterRow.FindControl("DDLFSalesCats");

            if (DDLFSalesCats == null || DDLFSalesCats.SelectedIndex == -1)
                return;

            if (Int16.TryParse(DDLFSalesCats.SelectedValue, out Int16 OutSalesID))
                e.Command.Parameters["@OtherSalesID"].Value = OutSalesID;
            else
            {
                e.Cancel = true; return;
            }

            if (TextBoxFAmount.Text == string.Empty)
                return;
            else if (Decimal.TryParse(TextBoxFAmount.Text, out Decimal OutSalesAmt))
                e.Command.Parameters["@OtherSalesAmount"].Value = OutSalesAmt;
            else
            {
                e.Cancel = true; return;
            }

            if (TextBoxFDate.Text == string.Empty)
                return;
            e.Command.Parameters["@OtherSalesDate"].Value = TextBoxFDate.Text;

        }
    }
}
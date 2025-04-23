using System;
using System.Collections.Generic;
using System.Data;
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

            DateTime checkDateNew;
            if (DateTime.TryParse(e.Command.Parameters["@OtherSalesDate"].Value.ToString(), out checkDateNew) == false)
            {
                e.Cancel = true;
                return;
            }
            
            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true &&
                (IsDateOpenForEntry(checkDateNew, DDLProgramYears.SelectedValue) == false || IsDateOpenForEntry(checkDateNew, DDLProgramYears.SelectedValue) == false))
            {
                ShowDialog("Warning", MessageType.Warning, "Period has been closed and record cannot be inserted.", "");
                e.Cancel = true;
                return;
            }
        }

        protected void SqlDataSourceOtherSalesEntry_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            if(e==null)
                return;

            DateTime checkDateOld= (DateTime)e.Command.Parameters["@old_OtherSalesDate"].Value; 
            DateTime checkDateNew = (DateTime)e.Command.Parameters["@OtherSalesDate"].Value;

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true && 
                (IsDateOpenForEntry(checkDateOld, DDLProgramYears.SelectedValue) == false || IsDateOpenForEntry(checkDateNew, DDLProgramYears.SelectedValue) == false))
            {
                ShowDialog("Warning", MessageType.Warning, "Period has been closed and record cannot be updated.", "");
                e.Cancel = true;
                return;
            }

        }

        protected void SqlDataSourceOtherSalesEntry_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null)
                return;

            DateTime checkDateOld = (DateTime)e.Command.Parameters["@old_OtherSalesDate"].Value;

            if (IsDistrictEnforcingPeriodClosures(Session["CurrentDistrict"].ToString()) == true &&
                IsDateOpenForEntry(checkDateOld, DDLProgramYears.SelectedValue) == false )
            {
                ShowDialog("Warning", MessageType.Warning, "Period has been closed and record cannot be deleted.", "");
                e.Cancel = true;
                return;
            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlDataSourceProgramYears.DataBind();

            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int recordIndex = dvSql.Find(DDLProgramYears.SelectedValue);
                if (recordIndex != -1)
                {
                    DataRowView drvSql = dvSql[recordIndex];
                    HFStartDate.Value = drvSql["StartDate"].ToString();
                    HFEndDate.Value = drvSql["EndDate"].ToString();
                }
                else
                {
                    HFStartDate.Value = "";
                    HFEndDate.Value = "";
                }
            }
            else
            {
                //if no program years could be populated - redirect home
                Response.Redirect("~/default.aspx");
            }
        }
    }
}
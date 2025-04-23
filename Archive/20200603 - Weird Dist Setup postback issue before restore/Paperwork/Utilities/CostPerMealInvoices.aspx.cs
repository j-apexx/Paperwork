using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class CPMInvoices : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            switch (Session["AccessLevel"].ToString().ToUpper())
            {
                case "ADMIN":
                case "OFFICE":
                    break;
                default:
                    //Denied
                    Response.Redirect("~/Default.aspx");
                    break;
            }

            if (!IsPostBack)
            {
                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count > 0 && Session["ProgramYear"] != null && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                    DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                DDLProgramYears_SelectedIndexChanged(DDLProgramYears, EventArgs.Empty);
            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update the hidden fields which contain the start and end dates.
            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int rowInt = dvSql.Find(DDLProgramYears.SelectedValue);
                DataRowView drvSql = dvSql[rowInt];
                HFProgYearStartDate.Value = ((DateTime)drvSql["StartDate"]).ToShortDateString();
                HFProgYearEndDate.Value = ((DateTime)drvSql["EndDate"]).ToShortDateString();
            }
        }

        protected void ICreate_Click(object sender, EventArgs e)
        {
            SqlDataSourceInvoiceList.Insert();
        }

        protected void GridViewInvoiceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            if (gv.SelectedIndex == -1 && gv.Rows.Count > 0)
                gv.SelectedIndex = 0;


            if (gv.SelectedIndex != -1 && (gv.SelectedIndex + 1) <= gv.DataKeys.Count)
            {
                //Set the hidden value to the selected invoicenumber then update the displayed invoices
                HFInvoiceNumber.Value = gv.DataKeys[(gv.SelectedIndex)].Values["InvoiceNumber"].ToString();

                HiddenField HFInvoiceRowLocked = (HiddenField)FindControlRecursive(gv.SelectedRow, "HFInvoiceRowLocked");
                if (HFInvoiceRowLocked != null)
                    HFInvoiceLocked.Value = HFInvoiceRowLocked.Value;
                else
                    HFInvoiceLocked.Value = "False";

                if (HFInvoiceLocked.Value == "False")
                {
                    ISave.Enabled = true;
                    ISave.ForeColor = Color.Green;
                }
                else
                {
                    ISave.Enabled = false;
                    ISave.ForeColor = Color.Gray;
                }
            }
            else
            {
                HFInvoiceNumber.Value = string.Empty;
            }

            UpdatePanelEditInvoice.Update();
        }

        protected void GridViewInvoiceList_PreRender(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            foreach (GridViewRow row in gv.Rows)
            {
                if (row.RowIndex == gv.SelectedIndex)
                {
                    row.ToolTip = string.Empty;
                    row.BorderColor = Color.Green;
                    row.BorderWidth = 2;
                    row.BorderStyle = BorderStyle.Solid;

                }
                else
                {
                    row.ToolTip = "Click to select this row.";
                    row.BorderColor = Color.Empty;
                    row.BorderWidth = 0;
                    row.ForeColor = Color.Empty;
                }
            }
        }

        protected void GridViewInvoiceList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Make the row clickable
            GridviewClickableRowDatabound(sender, e);
        }

        protected void GridViewInvoiceList_PageIndexChanged(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.DataBind();
            gv.SelectRow(0);
        }

        protected void ISave_Click(object sender, EventArgs e)
        {

        }

        protected void IPrint_Click(object sender, EventArgs e)
        {

        }

        protected void GridViewInvoiceList_Sorted(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            gv.DataBind();

            for (int i=0;i<gv.DataKeys.Count;i++)
            {
                if(gv.DataKeys[i].Values["InvoiceNumber"].ToString()==HFInvoiceNumber.Value)
                {
                    gv.SelectedIndex = i;
                    break;
                }
            }

            
        }


    }
}
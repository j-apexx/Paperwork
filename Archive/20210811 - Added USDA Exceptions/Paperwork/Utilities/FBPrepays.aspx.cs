using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class FBPrepays : Paperwork_Global
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
                HFProgramYearStartDate.Value = ((DateTime)drvSql["StartDate"]).ToShortDateString();
                HFProgramYearEndDate.Value = ((DateTime)drvSql["EndDate"]).ToShortDateString();
            }
        }

        protected void CBActiveOnly_CheckedChanged(object sender, EventArgs e)
        {
            ListItem storedLI = DDLDistricts.SelectedItem;
            DDLDistricts.DataBind();
            ListItem newLI = DDLDistricts.Items.FindByValue(storedLI.Value);
            if (newLI != null)
                newLI.Selected = true;
            else
                DDLDistricts.SelectedIndex = 0;

            GridViewFBPrepays.DataBind();
        }

        protected void SqlDataSourceDistricts_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            DDLDistricts.Items.Clear();
        }

        protected void GridViewFBPrepays_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["Counter"].ToString() == "0")
                e.Row.Visible = false;

            //Set footer row to match selected district - if selected
            if (e.Row.RowType == DataControlRowType.Footer )
            {
                ListItem storedLI = DDLDistricts.SelectedItem;
                DropDownList DDLFDistricts = (DropDownList)e.Row.FindControl("DDLFDistricts");
                if (DDLFDistricts != null)
                {
                    DDLFDistricts.DataBind();

                    if (DDLDistricts.SelectedValue != "%")
                    {
                        
                        ListItem newLI = DDLFDistricts.Items.FindByValue(storedLI.Value);
                        if (newLI != null)
                        {
                            newLI.Selected = true;
                            DDLFDistricts.Enabled = false;
                        }
                        else
                            DDLFDistricts.SelectedIndex = 0;
                    }
                    else
                    {
                        DDLFDistricts.Enabled = true;
                    }
                }

                TextBox TextBoxFAdjDate = (TextBox)e.Row.FindControl("TextBoxFPrepayDate");
                if(TextBoxFAdjDate != null)
                {
                    TextBoxFAdjDate.Text = DateTime.Now.ToShortDateString();
                }
            }
        }

        protected void SqlDataSourceFBPrepays_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null)
                return;

            DropDownList DDLFDistricts = (DropDownList)GridViewFBPrepays.FooterRow.FindControl("DDLFDistricts");
            TextBox TextBoxFPrepayDate = (TextBox)GridViewFBPrepays.FooterRow.FindControl("TextBoxFPrepayDate");
            TextBox TextBoxFPrepayAmount = (TextBox)GridViewFBPrepays.FooterRow.FindControl("TextBoxFPrepayAmount");
            TextBox TextBoxFDescription = (TextBox)GridViewFBPrepays.FooterRow.FindControl("TextBoxFDescription");

            if (DDLFDistricts==null || TextBoxFPrepayDate==null || TextBoxFPrepayAmount==null || TextBoxFDescription==null)
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFPrepayAmount.Text, out Decimal outAmount))
                e.Command.Parameters["@PrepayAmount"].Value = outAmount;
            else
            {
                e.Cancel = true; return;
            }

            if (DateTime.TryParse(TextBoxFPrepayDate.Text, out DateTime outDate))
                e.Command.Parameters["@PrepayDate"].Value = outDate;
            else
            {
                e.Cancel = true; return;
            }

            if(DDLFDistricts.SelectedIndex==-1 || DDLFDistricts.SelectedValue=="")
            {
                e.Cancel = true; return;
            }
            else
            {
                e.Command.Parameters["@DistrictID"].Value = DDLFDistricts.SelectedValue;
            }

            e.Command.Parameters["@Description"].Value = TextBoxFDescription.Text;

        }

        protected void GridViewFBPrepays_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceFBPrepays.Insert();
                }
                catch(SqlException ex)
                {
                    ShowMessageIcon("Database Error", MessageType.Error);
                }
                catch (InvalidOperationException ex)
                {
                    ShowMessageIcon("Error", MessageType.Error);
                }
            }
        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox dateTextBox = (TextBox)sender;

            // If just digits are typed - convert to standard date
            NormalizeSimplifiedDate(ref dateTextBox);
            // Ensure the date falls in the Program Year. Validate weekend entries.
            ValidateTrxEntryDate(ref dateTextBox, false, HFProgramYearStartDate.Value, HFProgramYearEndDate.Value);

            if(dateTextBox.ID== "TextBoxEPrepayDate")
            {
                TextBox TextBoxEPrepayAmount = (TextBox)FindControlRecursive(GridViewFBPrepays, "TextBoxEPrepayAmount");
                if (TextBoxEPrepayAmount != null)
                    TextBoxEPrepayAmount.Focus();
            }
            else if (dateTextBox.ID == "TextBoxFPrepayDate")
            {
                TextBox TextBoxFPrepayAmount = (TextBox)GridViewFBPrepays.FooterRow.FindControl("TextBoxFPrepayAmount");
                if (TextBoxFPrepayAmount != null)
                    TextBoxFPrepayAmount.Focus();
            }

        }

        protected void SqlDataSourceFBPrepays_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if(e.AffectedRows>0)
            {
                TextBox TextBoxFPrepayDate = (TextBox)GridViewFBPrepays.FooterRow.FindControl("TextBoxFPrepayDate");
                if (TextBoxFPrepayDate != null)
                    TextBoxFPrepayDate.Focus();
            }
        }
    }
}
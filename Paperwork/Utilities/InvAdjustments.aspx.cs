using System;
using System.Data;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class InvAdjustments : Paperwork_Global
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
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count > 0 && Session["ProgramYear"] != null && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                    DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                DDLProgramYears_SelectedIndexChanged(DDLProgramYears, EventArgs.Empty);
            }

        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
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

            GridViewInvAdjustments.DataBind();
        }

        protected void SqlDataSourceDistricts_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            DDLDistricts.Items.Clear();
        }

        protected void GridViewInvAdjustments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ProgramYear"].ToString() == "")
                e.Row.Visible = false;

            //Set footer row to match selected district - if selected
            if (e.Row.RowType == DataControlRowType.Footer)
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

                //TextBox TextBoxFAdjDate = (TextBox)e.Row.FindControl("TextBoxFAdjDate");
                //if (TextBoxFAdjDate != null)
                //{
                //    TextBoxFAdjDate.Text = DateTime.Now.ToShortDateString();
                //}
            }
        }

        protected void GridViewInvAdjustments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceInvAdjusts.Insert();
                }
                catch (InvalidOperationException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occured during Insert.", ex.Message);
                    e.Handled= true;
                    //ShowMessageIcon("Error", MessageType.Error);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occured during Insert.", ex.Message);
                    e.Handled = true;
                    //ShowMessageIcon("Error", MessageType.Error);
                }
            }
        }

        protected void SqlDataSourceInvAdjusts_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null)
                return;

            DropDownList DDLFDistricts = (DropDownList)GridViewInvAdjustments.FooterRow.FindControl("DDLFDistricts");
            TextBox TextBoxFYearmonth = (TextBox)GridViewInvAdjustments.FooterRow.FindControl("TextBoxFYearmonth");
            TextBox TextBoxFAmount = (TextBox)GridViewInvAdjustments.FooterRow.FindControl("TextBoxFAmount");
            TextBox TextBoxFComments = (TextBox)GridViewInvAdjustments.FooterRow.FindControl("TextBoxFComments");

            if (DDLFDistricts == null || TextBoxFYearmonth == null || TextBoxFAmount == null || TextBoxFComments==null)
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFAmount.Text, out Decimal outAmount))
                e.Command.Parameters["@CommodRcvdInclNOI_Adjust"].Value = outAmount;
            else
            {
                e.Cancel = true; return;
            }

            DateTime checkDate;
            try
            {
                checkDate = new DateTime(int.Parse(TextBoxFYearmonth.Text.Substring(0, 4)), int.Parse(TextBoxFYearmonth.Text.Substring(5, 2)), 1);
            }
            catch
            {
                ShowDialog("Error", MessageType.Error, "Error with Year-Month format.", "");
                return;
            }
            e.Command.Parameters["@YearMonth"].Value = TextBoxFYearmonth.Text;

            e.Command.Parameters["@USDAComments"].Value = TextBoxFComments.Text;

            if (DDLFDistricts.SelectedIndex == -1 || DDLFDistricts.SelectedValue == "")
            {
                e.Cancel = true; return;
            }
            else
            {
                e.Command.Parameters["@DistrictID"].Value = DDLFDistricts.SelectedValue;
            }

        }
    }
}
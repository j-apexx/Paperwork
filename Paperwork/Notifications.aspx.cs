using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class Notifications : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;

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

                DDLProgramYears_SelectedIndexChanged(null, null);

            }
            else
            {
                VerifyAccess(false);
            }


        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.Parameters["@ProgramYear"].Value = DDLProgramYears.SelectedValue;
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                this.Master.MstrHdrUpdatePanel.Update();

                //Response.Redirect(Request.RawUrl);//Request.RawUrl provides the current page URL
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void GridViewNotifications_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceNotifications.Insert();
                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate key") >= 0)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "Duplicate record - you cannot insert another record with the same date.", "");
                    }
                    else
                    {
                        ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }
            }
        }

        protected void GridViewNotifications_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ProgramYear"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceNotifications_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // Program Year and District are already set from Session or page dropdown
            TextBox TexttBoxFNotificationDate = (TextBox)GridViewNotifications.FooterRow.FindControl("TexttBoxFNotificationDate");
            CheckBox cbFTrans = (CheckBox)GridViewNotifications.FooterRow.FindControl("cbFTrans");
            CheckBox cbFNoTrans = (CheckBox)GridViewNotifications.FooterRow.FindControl("cbFNoTrans");
            CheckBox cbFRecaps = (CheckBox)GridViewNotifications.FooterRow.FindControl("cbFRecaps");
            CheckBox cbFMonthEnd = (CheckBox)GridViewNotifications.FooterRow.FindControl("cbFMonthEnd");
            TextBox TextBoxFNote = (TextBox)GridViewNotifications.FooterRow.FindControl("TextBoxFNote");
            CheckBox cbFNFL = (CheckBox)GridViewNotifications.FooterRow.FindControl("cbFNFL");

            if (DateTime.TryParse(TexttBoxFNotificationDate.Text, out DateTime DateOut))
                e.Command.Parameters["@NotificationDate"].Value = DateOut;
            else
            {
                e.Cancel = true; return;
            }

            //Check to ensure date is a Friday or End of Month
            if (IsDateFridayOrEom(ref TexttBoxFNotificationDate) == false)
            {
                e.Cancel = true; return;
            }

            //Check date range period
            if (IsDistrictEnforcingPeriodClosures(DropDownListDistricts.SelectedValue) && !IsDateOpenForEntry(DateOut, DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, "Notifications"))
            {
                ShowMessageIcon("Period Closed", MessageType.Warning);
                e.Cancel = true; return;
            }


            if (DropDownListDistricts.SelectedIndex == -1 || DropDownListDistricts.SelectedValue == string.Empty)
            {
                e.Cancel = true; return;
            }

            e.Command.Parameters["@Note"].Value = TextBoxFNote.Text;
            e.Command.Parameters["@MonthEnd"].Value = cbFMonthEnd.Checked;
            e.Command.Parameters["@Recaps"].Value = cbFRecaps.Checked;
            e.Command.Parameters["@NoTransmittals"].Value = cbFNoTrans.Checked;
            e.Command.Parameters["@Transmittals"].Value = cbFTrans.Checked;
            e.Command.Parameters["@NutritionForLife"].Value = cbFNFL.Checked;

            //all values are set, insert should be able to run
        }

        protected void SqlDataSourceNotifications_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        protected void SqlDataSourceNotifications_Deleted(object sender, SqlDataSourceStatusEventArgs e)
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

        protected void TexttBoxFNotificationDate_TextChanged(object sender, EventArgs e)
        {
            TextBox TexttBoxFNotificationDate = ((TextBox)sender);

            // If just digits are typed - convert to standard date
            NormalizeSimplifiedDate(ref TexttBoxFNotificationDate);

            IsDateFridayOrEom(ref TexttBoxFNotificationDate);

        }

        protected Boolean IsDateFridayOrEom(ref TextBox DateTextBox)
        {
            Boolean result = false;

            if (DateTextBox != null && DateTime.TryParse(DateTextBox.Text, out DateTime dateResult))
            {
                //if not end of week, and not end of month - display warning
                if (dateResult.DayOfWeek != DayOfWeek.Friday && new DateTime(dateResult.AddMonths(1).Year, dateResult.AddMonths(1).Month, 1).AddDays(-1) != dateResult)
                {
                    RegisterDOMReadyScript("Alert Date", "alert('Entered Notification Date is not a Friday and is not the last day of the month.');$( '[id *= " + DateTextBox.ID + "]' ).addClass('highlight');");
                    result = false;
                }
                else
                {
                    RegisterDOMReadyScript("Normal Date", "$( '[id *= " + DateTextBox.ID + "]' ).removeClass('highlight');");
                    result = true;
                }
            }

            return result;

        }

        protected void cbTransNonTrans_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox senderCB = (CheckBox)sender;
            if (senderCB != null)
            {
                //Edit line
                if (senderCB.ID == "cbETrans" || senderCB.ID == "cbENoTrans")
                {
                    CheckBox cbETrans = (CheckBox)FindControlRecursive(GridViewNotifications, "cbETrans");
                    CheckBox cbENoTrans = (CheckBox)FindControlRecursive(GridViewNotifications, "cbENoTrans");
                    if (cbETrans == null || cbENoTrans == null)
                        return;
                    if (senderCB.ID == "cbETrans" && cbETrans.Checked)
                        cbENoTrans.Checked = false;
                    if (senderCB.ID == "cbENoTrans" && cbENoTrans.Checked)
                        cbETrans.Checked = false;

                }
                //footer
                else if (senderCB.ID == "cbFTrans" || senderCB.ID == "cbFNoTrans")
                {
                    CheckBox cbFTrans = (CheckBox)FindControlRecursive(GridViewNotifications, "cbFTrans");
                    CheckBox cbFNoTrans = (CheckBox)FindControlRecursive(GridViewNotifications, "cbFNoTrans");
                    if (cbFTrans == null || cbFNoTrans == null)
                        return;
                    if (senderCB.ID == "cbFTrans" && cbFTrans.Checked)
                        cbFNoTrans.Checked = false;
                    if (senderCB.ID == "cbFNoTrans" && cbFNoTrans.Checked)
                        cbFTrans.Checked = false;
                }
            }
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            SqlDataSourceProgramYears.DataBind();

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



            //New 8/13/2020 - when program year changes, districts could change as well
            //Dists are now based on ProgramYear and must be rebound
            DropDownListDistricts.DataBind();
            if (DropDownListDistricts.Items.Count == 0)
                Response.Redirect("~/default.aspx");
            if (Session["CurrentDistrict"] != null)
            {
                if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                    DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                else
                    DropDownListDistricts_SelectedIndexChanged(null, null);
            }
        }

        protected void SqlDataSourceNotifications_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check date range period
            if (IsDistrictEnforcingPeriodClosures(DropDownListDistricts.SelectedValue) && !IsDateOpenForEntry((DateTime)e.Command.Parameters["@NotificationDate"].Value, DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, "Notifications"))
            {
                ShowMessageIcon("Period Closed, Delete Canceled.", MessageType.Warning);
                e.Cancel = true; return;
            }
        }

        protected void SqlDataSourceNotifications_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check date range period
            if (IsDistrictEnforcingPeriodClosures(DropDownListDistricts.SelectedValue) && !IsDateOpenForEntry((DateTime)e.Command.Parameters["@NotificationDate"].Value, DDLProgramYears.SelectedValue, DropDownListDistricts.SelectedValue, "Notifications"))
            {
                ShowMessageIcon("Period Closed, Update Canceled.", MessageType.Warning);
                e.Cancel = true; return;
            }
        }
    }
}
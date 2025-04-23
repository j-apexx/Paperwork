using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupTransmittalOptions : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            //Check for custom security
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            bool accessExplicitlyGranted = false;
            bool accessExplicitlyDenied = false;
            CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

            if (accessExplicitlyDenied == true)
                Response.Redirect("~/Default.aspx");
            if (accessExplicitlyGranted)
            {
                //Do nothing
            }
            else
            {
                switch (Session["AccessLevel"].ToString().ToUpper())
                {
                    case "ADMIN":
                    case "OFFICE":
                        break;
                    default:
                        Response.Redirect("~/Default.aspx");
                        break;
                }
            }

            if (!IsPostBack)
            {
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

                //Update the PY related Hidden Fields
                DDLProgramYears_SelectedIndexChanged(null, null);
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

        protected void GridViewPostDates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["OpenPostDate"].ToString().IndexOf("1/1/1900")>=0)
                    e.Row.Visible = false;
            }
            catch (Exception ex)
            { }

        }

        protected void TextBoxEOpenPostDate_TextChanged(object sender, EventArgs e)
        {
            TextBox TextBoxEOpenPostDate = (TextBox)sender;
            if (TextBoxEOpenPostDate == null) return;
            GridViewRow gvr = (GridViewRow)FindParentControlOfType(TextBoxEOpenPostDate, "GridViewRow");
            if(gvr==null) return;
            GridViewPostDates.UpdateRow(gvr.RowIndex, true);
        }

        protected void SqlDataSourceWEDates_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            TextBox TextBoxFOpenPostDate = (TextBox)GridViewPostDates.FooterRow.FindControl("TextBoxFOpenPostDate");

            if (TextBoxFOpenPostDate == null || TextBoxFOpenPostDate.Text.Length == 0)
                return;

            if(DateTime.TryParse(TextBoxFOpenPostDate.Text, out DateTime testDate)==false)
            {
                e.Cancel = true;
                ShowDialog("Error", MessageType.Error, "Invalid date.", TextBoxFOpenPostDate.Text);
                TextBoxFOpenPostDate.Text=string.Empty;
                return;
            }

            e.Command.Parameters["@OpenPostDate"].Value = TextBoxFOpenPostDate.Text;

        }

        protected void SqlDataSourceBOMsgs_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            TextBox TextBoxFStartDate = (TextBox)GridViewBOMessages.FooterRow.FindControl("TextBoxFStartDate");
            TextBox TextBoxFEndDate = (TextBox)GridViewBOMessages.FooterRow.FindControl("TextBoxFEndDate");
            TextBox TextBoxFBOMessage = (TextBox)GridViewBOMessages.FooterRow.FindControl("TextBoxFBOMessage");
            


            if (TextBoxFStartDate == null || TextBoxFStartDate.Text.Length == 0 || TextBoxFEndDate == null || TextBoxFEndDate.Text.Length == 0 ||
                TextBoxFBOMessage == null || TextBoxFBOMessage.Text.Length == 0)
                return;
            
            if (DateTime.TryParse(TextBoxFStartDate.Text, out DateTime testDate) == false)
            {
                e.Cancel = true;
                ShowDialog("Error", MessageType.Error, "Invalid date.", TextBoxFStartDate.Text);
                TextBoxFStartDate.Text = string.Empty;
                return;
            }

            if (DateTime.TryParse(TextBoxFEndDate.Text, out DateTime testDate2) == false)
            {
                e.Cancel = true;
                ShowDialog("Error", MessageType.Error, "Invalid date.", TextBoxFEndDate.Text);
                TextBoxFEndDate.Text = string.Empty;
                return;
            }

            e.Command.Parameters["@StartDate"].Value = TextBoxFStartDate.Text;
            e.Command.Parameters["@EndDate"].Value = TextBoxFEndDate.Text;
            e.Command.Parameters["@DashboardMessage"].Value = TextBoxFBOMessage.Text;
        }

        protected void SqlDataSourceGenericInserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e != null && e.AffectedRows > 0)
                ShowMessageIcon("Inserted", MessageType.Success);
        }

        protected void SqlDataSourceGenericDeleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e != null && e.AffectedRows > 0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }

        protected void SqlDataSourceGenericUpdated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e != null && e.AffectedRows > 0)
                ShowMessageIcon("", MessageType.Success);
        }

        protected void GridViewRowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView gv = (GridView)sender;
            int recordCount = 0;


            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    switch (gv.ID)
                    {
                        case "GridViewBOMessages":
                            recordCount=SqlDataSourceBOMsgs.Insert();
                            //if(recordCount > 0)
                            //    gv.FooterRow.Controls[0].FindControl("TextBoxFStartDate").Focus();
                            break;
                        case "GridViewPostDates":
                            recordCount=SqlDataSourceWEDates.Insert();
                            //if (recordCount > 0)
                            //    gv.FooterRow.Controls[0].FindControl("TextBoxFOpenPostDate").Focus();
                            break;
                        default:
                            break;
                    }
                    
                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate key") >= 0)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "Duplicate record.", "");
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

        protected void GridViewBOMessages_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["Counter"].ToString()=="0")
                    e.Row.Visible = false;
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// Generic procedure to update BO Message fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TextBoxBOMessageTextChanged(object sender, EventArgs e)
        {
            TextBox TextBoxMsgsTxt = (TextBox)sender;
            if (TextBoxMsgsTxt == null) return;
            GridViewRow gvr = (GridViewRow)FindParentControlOfType(TextBoxMsgsTxt, "GridViewRow");
            if (gvr == null) return;
            GridViewBOMessages.UpdateRow(gvr.RowIndex, true);
        }
    }
}
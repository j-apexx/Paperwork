using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupDistrictContacts : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");


            if (!IsPostBack)
                VerifyAccess(true);
            else
                VerifyAccess(false);

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if(!IsPostBack)
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

                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;

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
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                Response.Redirect("~/Setup/SetupDistrictContacts.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void GridViewContacts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try { 
                    SqlDataSourceContacts.Insert();
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

        protected void GridViewContacts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceContacts_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Saved", MessageType.Success);

            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        protected void SqlDataSourceContacts_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }

        protected void SqlDataSourceContacts_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            TextBox TextBoxFTitle = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFTitle");
            TextBox TextBoxFFName = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFFName");
            TextBox TextBoxFLName = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFLName");
            TextBox TextBoxFEmail = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFEmail");
            TextBox TextBoxFPosition = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFPosition");
            TextBox TextBoxFBuilding = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFBuilding");
            TextBox TextBoxFNote = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFNote");
            TextBox TextBoxFWorkPhone = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFWorkPhone");
            TextBox TextBoxFCellPhone = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFCellPhone");
            TextBox TextBoxFAddress = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFAddress");
            TextBox TextBoxFCity = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFCity");
            TextBox TextBoxFState = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFState");
            TextBox TextBoxFZip = (TextBox)GridViewContacts.FooterRow.FindControl("TextBoxFZip");

            e.Command.Parameters["@Title"].Value = TextBoxFTitle.Text;
            e.Command.Parameters["@FirstName"].Value = TextBoxFFName.Text;
            e.Command.Parameters["@LastName"].Value = TextBoxFLName.Text;
            e.Command.Parameters["@Email"].Value = TextBoxFEmail.Text;
            e.Command.Parameters["@Position"].Value = TextBoxFPosition.Text;
            e.Command.Parameters["@Building"].Value = TextBoxFBuilding.Text;
            e.Command.Parameters["@Note"].Value = TextBoxFNote.Text;
            e.Command.Parameters["@WorkPhone"].Value = TextBoxFWorkPhone.Text;
            e.Command.Parameters["@CellPhone"].Value = TextBoxFCellPhone.Text;
            e.Command.Parameters["@Address"].Value = TextBoxFAddress.Text;
            e.Command.Parameters["@City"].Value = TextBoxFCity.Text;
            e.Command.Parameters["@State"].Value = TextBoxFState.Text;
            e.Command.Parameters["@Zip"].Value = TextBoxFZip.Text;


        }

        protected void GridViewContacts_PreRender(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if(gv.EditIndex!=-1)
            {
                GridViewRow gvr = ((GridView)sender).Rows[gv.EditIndex];
                DropDownList DDLETitles = (DropDownList)FindControlRecursive(gvr, "DDLETitles");
                DropDownList DDLEPositions = (DropDownList)FindControlRecursive(gvr, "DDLEPositions");
                TextBox TextBoxETitle = (TextBox)FindControlRecursive(gvr, "TextBoxETitle");
                TextBox TextBoxEPosition = (TextBox)FindControlRecursive(gvr, "TextBoxEPosition");

                ListItem liTitle = DDLETitles.Items.FindByValue(TextBoxETitle.Text);
                if(liTitle==null)
                {
                    DDLETitles.SelectedValue = "Other";
                    //show the hidden div which contains the custom title field
                    RegisterDOMReadyScript("Trigger Show", "$('#TitleHiderE').css('visibility', 'visible');");
                }
                else
                {
                    DDLETitles.SelectedValue = TextBoxETitle.Text;
                }

                ListItem liPosition = DDLEPositions.Items.FindByText(TextBoxEPosition.Text);
                if (liPosition == null)
                {
                    DDLEPositions.SelectedValue = "99";
                    //show the hidden div which contains the custom title field
                    RegisterDOMReadyScript("Trigger Show", "$('#PositionHiderE').css('visibility', 'visible');");
                }
                else
                {
                    DDLEPositions.SelectedValue = liPosition.Value;
                }

            }
        }
    }
}
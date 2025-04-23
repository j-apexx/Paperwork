using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupSpFnOrganizations : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            VerifyAccess(true);

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");
            }

        }

        protected void GridViewSpFnOrgs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow gvr = e.Row;

            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["SpFnOrgID"].ToString() == "0")
                e.Row.Visible = false;

        }

        protected void GridViewSpFnOrgs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert -
            //we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceSpFnOrgs.Insert();
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

        protected void SqlDataSourceSpFnOrgs_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);

                //Refocus to allow quicker entry of new records
                TextBox TextBoxFSpFnOrgName = (TextBox)FindControlRecursive(GridViewSpFnOrgs, "TextBoxFSpFnOrgName");
                if (TextBoxFSpFnOrgName != null)
                    TextBoxFSpFnOrgName.Focus();
            }
            else if (e.Exception != null && e.ExceptionHandled == false)
            {
                if (e.Exception.Message.IndexOf("duplicate") > 0)
                    ShowDialog("Error", MessageType.Error, "Organization could not be created.", "Duplicate found.");
                else
                    ShowDialog("Error", MessageType.Error, "Organization could not be created.", e.Exception.Message);

                e.ExceptionHandled = true;
            }
        }

        protected void SqlDataSourceSpFnOrgs_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            TextBox TextBoxESpFnOrgName = (TextBox)FindControlRecursive(GridViewSpFnOrgs, "TextBoxESpFnOrgName");
            TextBox TextBoxESpFnOrgContact = (TextBox)FindControlRecursive(GridViewSpFnOrgs, "TextBoxESpFnOrgContact");
            TextBox TextBoxESpFnOrgRemitTo = (TextBox)FindControlRecursive(GridViewSpFnOrgs, "TextBoxESpFnOrgRemitTo");
            if (TextBoxESpFnOrgName == null || TextBoxESpFnOrgContact == null || TextBoxESpFnOrgRemitTo == null)
            {
                e.Cancel = true; return;
            }
            e.Command.Parameters["@SpFnOrgName"].Value = TextBoxESpFnOrgName.Text.PadRight(50).Substring(0, 50).Trim();
            e.Command.Parameters["@SpFnOrgContact"].Value = TextBoxESpFnOrgContact.Text.PadRight(50).Substring(0, 50).Trim();
            e.Command.Parameters["@SpFnOrgRemitTo"].Value = TextBoxESpFnOrgRemitTo.Text.PadRight(50).Substring(0, 50).Trim();
        }

        protected void SqlDataSourceSpFnOrgs_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Due to  using the footer for inserts - we need to map the values before the insert can happen

            //District is already set from Session variable
            
            TextBox TextBoxFSpFnOrgName = (TextBox)GridViewSpFnOrgs.FooterRow.FindControl("TextBoxFSpFnOrgName");
            CheckBox CBF3086 = (CheckBox)GridViewSpFnOrgs.FooterRow.FindControl("CBF3086");
            CheckBox CBFCACFP = (CheckBox)GridViewSpFnOrgs.FooterRow.FindControl("CBFCACFP");
            TextBox TextBoxFSpFnOrgContact = (TextBox)GridViewSpFnOrgs.FooterRow.FindControl("TextBoxFSpFnOrgContact");
            TextBox TextBoxFSpFnOrgRemitTo = (TextBox)GridViewSpFnOrgs.FooterRow.FindControl("TextBoxFSpFnOrgRemitTo");

            if(TextBoxFSpFnOrgName==null || CBF3086==null || CBFCACFP==null || TextBoxFSpFnOrgContact==null || TextBoxFSpFnOrgRemitTo==null)
            {
                e.Cancel = true; return;
            }

            if (TextBoxFSpFnOrgName.Text == String.Empty)
            {
                e.Cancel = true; return;
            }
            e.Command.Parameters["@SpFnOrgName"].Value = TextBoxFSpFnOrgName.Text.PadRight(50).Substring(0,50).Trim();
            e.Command.Parameters["@SpFnOrgContact"].Value = TextBoxFSpFnOrgContact.Text.PadRight(50).Substring(0, 50).Trim();
            e.Command.Parameters["@SpFnOrgRemitTo"].Value = TextBoxFSpFnOrgRemitTo.Text.PadRight(50).Substring(0, 50).Trim();
            e.Command.Parameters["@3086"].Value = CBF3086.Checked;
            e.Command.Parameters["@CACFP"].Value = CBFCACFP.Checked;
        }

        protected void SqlDataSourceSpFnOrgs_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if(e.AffectedRows==0)
                ShowDialog("Error", MessageType.Error, "Organization could not be updated.", e.Exception.Message);
        }

    }
}
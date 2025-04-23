using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupProgramTypes : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            switch (Session["AccessLevel"].ToString().ToUpper())
            {
                case "ADMIN":
                    break;
                default:
                    //Denied
                    Response.Redirect("~/Default.aspx");
                    break;
            }

            //Check for custom security
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            bool accessExplicitlyGranted = false;
            bool accessExplicitlyDenied = false;
            CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

            if (accessExplicitlyDenied == true)
                Response.Redirect("~/Default.aspx");
        }

        protected void GridViewProgramTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceProgramTypes.Insert();
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

        protected void GridViewProgramTypes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ProgramType"].ToString() == "")
                    e.Row.Visible = false;
            }
            catch (Exception ex)
            { }
        }

        protected void SqlDataSourceProgramTypes_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set

            TextBox TextBoxFProgramType = (TextBox)GridViewProgramTypes.FooterRow.FindControl("TextBoxFProgramType");

            if (TextBoxFProgramType == null || TextBoxFProgramType.Text.Length == 0)
                return;

            e.Command.Parameters["@ProgramType"].Value = TextBoxFProgramType.Text;
        }
    }
}
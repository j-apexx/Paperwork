using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupGallikerCustNums : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);


            if (!IsPostBack)
            {
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

                if(!accessExplicitlyGranted)
                {
                    switch (Session["AccessLevel"].ToString().ToUpper())
                    {
                        case "ADMIN":
                            break;
                        default:
                            //Denied
                            Response.Redirect("~/Default.aspx");
                            break;
                    }
                }


                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DDLDistricts.DataSource = SqlDataSourceMyDistsMstr;

                DDLDistricts.DataBind();
            }
        }

        protected void GridViewGLKRCustNums_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceGLKRCusts.Insert();
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

        protected void GridViewGLKRCustNums_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["GLKRCustNum"].ToString() == "0")
                    e.Row.Visible = false;
            }
            catch (Exception ex) { }
        }

        protected void TextBoxFDistrictID_TextChanged(object sender, EventArgs e)
        {
            HiddenField HFFDistrictID = (HiddenField)GridViewGLKRCustNums.FooterRow.FindControl("HFFDistrictID");
            HFFDistrictID.Value = ((TextBox)sender).Text;
            DropDownList DropDownListFSchools = (DropDownList)GridViewGLKRCustNums.FooterRow.FindControl("DropDownListFSchools");
            DropDownListFSchools.Focus();
        }

        protected void SqlDataSourceGLKRCusts_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            TextBox TextBoxFDistrictID = (TextBox)GridViewGLKRCustNums.FooterRow.FindControl("TextBoxFDistrictID");
            DropDownList DropDownListFSchools = (DropDownList)GridViewGLKRCustNums.FooterRow.FindControl("DropDownListFSchools");
            TextBox TextBoxFGLKRCustNum = (TextBox)GridViewGLKRCustNums.FooterRow.FindControl("TextBoxFGLKRCustNum");

            if (TextBoxFDistrictID == null || String.IsNullOrEmpty(TextBoxFDistrictID.Text))
            {
                ShowMessageIcon("District Missing.", MessageType.Info);
                e.Cancel = true; return;
            }

            if (TextBoxFGLKRCustNum == null || String.IsNullOrEmpty(TextBoxFGLKRCustNum.Text))
            {
                ShowMessageIcon("Galliker Cust Num Missing.", MessageType.Info);
                e.Cancel = true; return;
            }
            if (int.TryParse(TextBoxFGLKRCustNum.Text, out int GLKRInt) == false)
            {
                ShowMessageIcon("Galliker Cust Num could not be converted to integer.", MessageType.Info);
                e.Cancel = true; return;
            }

            if (DropDownListFSchools == null || DropDownListFSchools.Items.Count == 0 || DropDownListFSchools.SelectedIndex == -1)
            {
                ShowMessageIcon("School not selected.", MessageType.Info);
                e.Cancel = true; return;
            }

            e.Command.Parameters["@DistrictID"].Value = TextBoxFDistrictID.Text;
            e.Command.Parameters["@SchoolNum"].Value = DropDownListFSchools.SelectedValue;
            e.Command.Parameters["@GLKRCustNum"].Value = GLKRInt.ToString();

        }

        protected void SqlDataSourceGLKRCusts_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);

                //Refocus to allow quicker entry of new records
                TextBox TextBoxFDistrictID = (TextBox)GridViewGLKRCustNums.FooterRow.FindControl("TextBoxFDistrictID");
                if (TextBoxFDistrictID != null)
                    TextBoxFDistrictID.Focus();
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
            else if (e.AffectedRows == 0)
            {
                ShowMessageIcon("Could not add row.", MessageType.Info);
            }
        }

        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            //to help with scrolling through districts - maintain focus
            DDLDistricts.Focus();
        }
    }
}
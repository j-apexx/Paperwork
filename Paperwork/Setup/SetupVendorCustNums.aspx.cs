using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupVendorCustNums : Paperwork_Global
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

            if (!IsPostBack)
            {
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");


                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DDLDistricts.DataSource = SqlDataSourceMyDistsMstr;

                DDLDistricts.DataBind();
            }
        }

        protected void GridViewCustomerNumbers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceVendorCusts.Insert();
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

        protected void GridViewCustomerNumbers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["VendorID"].ToString() == "")
                    e.Row.Visible = false;
            }
            catch (Exception ex) { }
        }

        protected void TextBoxFDistrictID_TextChanged(object sender, EventArgs e)
        {
            HiddenField HFFDistrictID = (HiddenField)GridViewCustomerNumbers.FooterRow.FindControl("HFFDistrictID");
            HFFDistrictID.Value = ((TextBox)sender).Text;
            DropDownList DropDownListFSchools = (DropDownList)GridViewCustomerNumbers.FooterRow.FindControl("DropDownListFSchools");
            DropDownListFSchools.Focus();
        }

        protected void SqlDataSourceVendorCusts_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            TextBox TextBoxFDistrictID = (TextBox)GridViewCustomerNumbers.FooterRow.FindControl("TextBoxFDistrictID");
            DropDownList DropDownListFSchools = (DropDownList)GridViewCustomerNumbers.FooterRow.FindControl("DropDownListFSchools");
            TextBox TextBoxFCustomerNumber = (TextBox)GridViewCustomerNumbers.FooterRow.FindControl("TextBoxFCustomerNumber");
            TextBox TextBoxFVendorID = (TextBox)GridViewCustomerNumbers.FooterRow.FindControl("TextBoxFVendorID");
            CheckBox CB_FUseDefaultSchool = (CheckBox)GridViewCustomerNumbers.FooterRow.FindControl("CB_FUseDefaultSchool");

            if (TextBoxFDistrictID == null || String.IsNullOrEmpty(TextBoxFDistrictID.Text))
            {
                ShowMessageIcon("District Missing.", MessageType.Info);
                e.Cancel = true; return;
            }

            if (TextBoxFCustomerNumber == null || String.IsNullOrEmpty(TextBoxFCustomerNumber.Text))
            {
                ShowMessageIcon("Customer Number Missing.", MessageType.Info);
                e.Cancel = true; return;
            }

            //if (TextBoxFVendorID == null || String.IsNullOrEmpty(TextBoxFVendorID.Text))
            //{
            //    ShowMessageIcon("Vendor ID Missing.", MessageType.Info);
            //    e.Cancel = true; return;
            //}

            if (CB_FUseDefaultSchool == null)
            { e.Cancel = true; return; }

            if (DropDownListFSchools == null || DropDownListFSchools.Items.Count == 0 || DropDownListFSchools.SelectedIndex == -1)
            {
                ShowMessageIcon("School not selected.", MessageType.Info);
                e.Cancel = true; return;
            }



            e.Command.Parameters["@DistrictID"].Value = TextBoxFDistrictID.Text;
            e.Command.Parameters["@SchoolNum"].Value = CB_FUseDefaultSchool.Checked? "0" : DropDownListFSchools.SelectedValue;
            //e.Command.Parameters["@Division"].Value = TextBoxFVendorID.Text;
            e.Command.Parameters["@CustomerNumber"].Value = TextBoxFCustomerNumber.Text;
            e.Command.Parameters["@CB_UseDefaultSchool"].Value = CB_FUseDefaultSchool.Checked;

        }

        protected void SqlDataSourceVendorCusts_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);

                //Refocus to allow quicker entry of new records
                TextBox TextBoxFDistrictID = (TextBox)GridViewCustomerNumbers.FooterRow.FindControl("TextBoxFDistrictID");
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
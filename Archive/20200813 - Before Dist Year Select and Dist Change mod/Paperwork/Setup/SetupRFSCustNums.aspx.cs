using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupRFSCustNums : Paperwork_Global
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
                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DDLDistricts.DataSource = SqlDataSourceMyDistsMstr;

                DDLDistricts.DataBind();
            }
        }

        protected void GridViewRFSCustNums_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                SqlDataSourceRFSCusts.Insert();
            }
        }

        protected void GridViewRFSCustNums_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["Division"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void TextBoxFDistrictID_TextChanged(object sender, EventArgs e)
        {
            HiddenField HFFDistrictID = (HiddenField)GridViewRFSCustNums.FooterRow.FindControl("HFFDistrictID");
            HFFDistrictID.Value = ((TextBox)sender).Text;
            DropDownList DropDownListFSchools = (DropDownList)GridViewRFSCustNums.FooterRow.FindControl("DropDownListFSchools");
            DropDownListFSchools.Focus();
        }

        protected void SqlDataSourceRFSCusts_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            TextBox TextBoxFDistrictID = (TextBox)GridViewRFSCustNums.FooterRow.FindControl("TextBoxFDistrictID");
            DropDownList DropDownListFSchools = (DropDownList)GridViewRFSCustNums.FooterRow.FindControl("DropDownListFSchools");
            TextBox TextBoxFRFSCustNum = (TextBox)GridViewRFSCustNums.FooterRow.FindControl("TextBoxFRFSCustNum");
            TextBox TextBoxFDivision = (TextBox)GridViewRFSCustNums.FooterRow.FindControl("TextBoxFDivision");

            if (TextBoxFDistrictID==null ||  String.IsNullOrEmpty(TextBoxFDistrictID.Text))
            {
                ShowMessageIcon("District Missing.", MessageType.Info);
                e.Cancel = true; return;
            }

            if (TextBoxFRFSCustNum==null || String.IsNullOrEmpty(TextBoxFRFSCustNum.Text))
            {
                ShowMessageIcon("RFS Cust Num Missing.", MessageType.Info);
                e.Cancel = true; return;
            }
            if(int.TryParse(TextBoxFRFSCustNum.Text, out int RFSInt)==false)
            {
                ShowMessageIcon("RFS Cust Num could not be converted to integer.", MessageType.Info);
                e.Cancel = true; return;
            }

            if (TextBoxFDivision==null || String.IsNullOrEmpty(TextBoxFDivision.Text))
            {
                ShowMessageIcon("RFS Division Missing.", MessageType.Info);
                e.Cancel = true; return;
            }

            if(DropDownListFSchools==null || DropDownListFSchools.Items.Count==0 || DropDownListFSchools.SelectedIndex==-1)
            {
                ShowMessageIcon("School not selected.", MessageType.Info);
                e.Cancel = true; return;
            }

            e.Command.Parameters["@DistrictID"].Value = TextBoxFDistrictID.Text;
            e.Command.Parameters["@SchoolNum"].Value = DropDownListFSchools.SelectedValue;
            e.Command.Parameters["@Division"].Value = TextBoxFDivision.Text;
            e.Command.Parameters["@RFSCustNum"].Value = RFSInt.ToString();

        }

        protected void SqlDataSourceRFSCusts_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);

                //Refocus to allow quicker entry of new records
                TextBox TextBoxFDistrictID = (TextBox)GridViewRFSCustNums.FooterRow.FindControl("TextBoxFDistrictID");
                if (TextBoxFDistrictID != null)
                    TextBoxFDistrictID.Focus();
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
            else if(e.AffectedRows==0)
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
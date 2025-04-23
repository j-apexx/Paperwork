using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupMilkDelivery : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                VerifyAccess(true);
            }
            else
            {
                VerifyAccess(false);
            }

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if(!IsPostBack)
            {
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
                Response.Redirect("~/Setup/SetupMilkDelivery.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void GridViewMilkDeliv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                SqlDataSourceMilkDeliv.Insert();
            }
        }

        protected void GridViewMilkDeliv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;


        }

        protected void SqlDataSourceMilkDeliv_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Saved", MessageType.Success);

                //Refocus to allow quicker entry of new records
                DropDownList DDLFSchools = (DropDownList)FindControlRecursive(GridViewMilkDeliv, "DDLFSchools");
                if (DDLFSchools != null)
                    DDLFSchools.Focus();
            }
            else if (e.Exception != null)
            {
                ShowDialog("Error", MessageType.Error, "Error inserting record.", e.Exception.Message);
            }
        }

        protected void SqlDataSourceMilkDeliv_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }

        protected void SqlDataSourceMilkDeliv_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            DropDownList DDLFSchools = (DropDownList)GridViewMilkDeliv.FooterRow.FindControl("DDLFSchools");
            TextBox TextBoxFMMBArea = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFMMBArea");
            TextBox TextBoxFMonday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFMonday");
            TextBox TextBoxFTuesday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFTuesday");
            TextBox TextBoxFWednesday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFWednesday");
            TextBox TextBoxFThursday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFThursday");
            TextBox TextBoxFFriday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFFriday");
            TextBox TextBoxFJuiceCases = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFJuiceCases");
            TextBox TextBoxFTeaCases = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFTeaCases");
            CheckBox CBFWalkinCooler = (CheckBox)GridViewMilkDeliv.FooterRow.FindControl("CBFWalkinCooler");
            TextBox TextBoxFEightCaseCoolers = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFEightCaseCoolers");
            TextBox TextBoxFTwelveCaseCoolers = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFTwelveCaseCoolers");
            TextBox TextBoxFSixteenCaseCoolers = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFSixteenCaseCoolers");
            TextBox TextBoxFPALicense = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFPALicense");
            DropDownList DDLFVendors = (DropDownList)GridViewMilkDeliv.FooterRow.FindControl("DDLFVendors");
            CheckBox CBFMMBSheet = (CheckBox)GridViewMilkDeliv.FooterRow.FindControl("CBFMMBSheet");

            if (Int16.TryParse(DDLFSchools.SelectedValue, out Int16 OutSchoolNum))
                e.Command.Parameters["@SchoolNum"].Value = OutSchoolNum;
            else
            {
                e.Cancel = true; return;
            }

            e.Command.Parameters["@MMBArea"].Value = TextBoxFMMBArea.Text;

            Int16.TryParse(TextBoxFMonday.Text, out Int16 OutMon);
            e.Command.Parameters["@Monday"].Value = OutMon;
            Int16.TryParse(TextBoxFTuesday.Text, out Int16 OutTue);
            e.Command.Parameters["@Tuesday"].Value = OutTue;
            Int16.TryParse(TextBoxFWednesday.Text, out Int16 OutWed);
            e.Command.Parameters["@Wednesday"].Value = OutWed;
            Int16.TryParse(TextBoxFThursday.Text, out Int16 OutThu);
            e.Command.Parameters["@Thursday"].Value = OutMon;
            Int16.TryParse(TextBoxFFriday.Text, out Int16 OutFri);
            e.Command.Parameters["@Friday"].Value = OutFri;
            Int16.TryParse(TextBoxFJuiceCases.Text, out Int16 OutJuice);
            e.Command.Parameters["@JuiceCases"].Value = OutJuice;
            Int16.TryParse(TextBoxFTeaCases.Text, out Int16 OutTea);
            e.Command.Parameters["@TeaCases"].Value = OutTea;

            e.Command.Parameters["@WalkInCooler"].Value = CBFWalkinCooler.Checked ? (byte)1 : (byte)0;

            Int16.TryParse(TextBoxFEightCaseCoolers.Text, out Int16 Out8Cooler);
            e.Command.Parameters["@EightCaseCoolers"].Value = Out8Cooler;
            Int16.TryParse(TextBoxFTwelveCaseCoolers.Text, out Int16 Out12Cooler);
            e.Command.Parameters["@TwelveCaseCoolers"].Value = Out12Cooler;
            Int16.TryParse(TextBoxFSixteenCaseCoolers.Text, out Int16 Out16Cooler);
            e.Command.Parameters["@SixteenCaseCoolers"].Value = Out16Cooler;

            e.Command.Parameters["@PALicense"].Value = TextBoxFPALicense.Text;

            e.Command.Parameters["@VendorID"].Value = DDLFVendors.SelectedValue;

            e.Command.Parameters["@MMBSheet"].Value = CBFMMBSheet.Checked ? (byte)1 : (byte)0;
        }
    }
}
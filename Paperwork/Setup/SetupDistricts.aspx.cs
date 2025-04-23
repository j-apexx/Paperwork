using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupDistricts : Paperwork_Global
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
            }
        }

        protected void CbEnforcePeriodsEdit_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string checkFieldStr = cb.Checked ? "1" : "0";
            HiddenField hf = (HiddenField)FindControlRecursive(cb.Parent, "hfEnforcePeriods");

            hf.Value = checkFieldStr;
        }

        protected void cbInactiveEdit_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string checkFieldStr = cb.Checked ? "1" : "0";
            HiddenField hf = (HiddenField)FindControlRecursive(cb.Parent, "hfInactive");

            hf.Value = checkFieldStr;
        }

        protected void SqlDataSourceDistricts_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //string errors = "";
        }

        protected void SqlDataSourceDistricts_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e == null || e.AffectedRows == 0)
            {
                if (e.Exception != null)
                {
                    if (e.Exception.Message.IndexOf("duplicate") >= 0)
                        ShowDialog("Error", MessageType.Error, "District could not be created in Paperwork.", "Duplicate found.");
                    else
                        ShowDialog("Error", MessageType.Error, "District was not created in Paperwork.", e.Exception.Message);
                    e.ExceptionHandled = true;
                }
                return;
            }
        }

        protected void SqlDataSourceDistricts_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (isSFSP() || isTOOLBOX() || isTRAINING())
                return;
            if (e != null && e.AffectedRows > 0)
            {

                string distId=e.Command.Parameters["@DistrictID"].Value.ToString();
                bool inactive = ((Int16)e.Command.Parameters["@Inactive"].Value)==0?false:true;
                UpdateUSDADistrict(distId, inactive);
            }
        }

        protected void UpdateUSDADistrict(string dist, bool inactive)
        {
            string sql = "UPDATE [Districts_Setup] SET Inactive=@Inactive WHERE DistrictID=@DistrictID;";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnUSDA"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@DistrictID", dist);
                        cmd.Parameters.AddWithValue("@Inactive", inactive);
                        conn.Open();
                        int recordcount = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        //ShowDialog("Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }
        }

        protected void buttonLookup_Click(object sender, EventArgs e)
        {
            LinkButton linkButton = (LinkButton)sender;
            DropDownList ddl = (DropDownList)FindControlRecursive(linkButton.Parent, "DDLCompanies");
            TextBox TextBoxCustomerNumber = (TextBox)FindControlRecursive(linkButton.Parent, "TextBoxCustomerNumber");
            if (ddl.SelectedValue == "")
            {
                ShowDialog("Error", MessageType.Error, "Please select a company.", "");
                return;
            }

            hfINTERID.Value = ddl.SelectedValue;
            TextBoxSearch.Text = string.Empty;//Reset the search box

            SqlDataSourceGPCustomers.SelectCommand = "SELECT [Customer Number]Custnmbr,[Customer Name]Custname,[City],[State] FROM "+ hfINTERID.Value + "..Customers WHERE Inactive='No'";
            GridViewCustomers.DataBind();
            bool custFound = false; ;
            if(TextBoxCustomerNumber!=null && TextBoxCustomerNumber.Text!="")
            {
                foreach (GridViewRow row in GridViewCustomers.Rows)
                {
                    if (row.Cells[0].Text == TextBoxCustomerNumber.Text)
                    {
                        GridViewCustomers.SelectedIndex = row.RowIndex;
                        hfSelectedIndex.Value = (GridViewCustomers.SelectedIndex + 1).ToString();
                        custFound = true;
                        break;
                    }
                }
            }


            UpdatePanelModal.Update();

            if(custFound)
                RegisterDOMReadyScript("modalBox", "$('#myModal').modal('show');higlightSelectedRow();");
            else
                RegisterDOMReadyScript("modalBox", "$('#myModal').modal('show');");
        }

        protected void buttonCloseModal_Click(object sender, EventArgs e)
        {
            TextBox TextBoxCustomerNumber = (TextBox)FindControlRecursive(GridViewDistricts, "TextBoxCustomerNumber");


            if (GridViewCustomers.SelectedIndex == -1 || GridViewCustomers.DataKeys.Count == 0 || GridViewCustomers.DataKeys.Count < GridViewCustomers.SelectedIndex + 1)
            {
                TextBoxCustomerNumber.Text = string.Empty;
            }
            else
            {
                TextBoxCustomerNumber.Text = GridViewCustomers.DataKeys[GridViewCustomers.SelectedIndex].Values["Custnmbr"].ToString();
            }
        }

        protected void GridViewCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gv = (GridView)sender;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv, "Select$" + e.Row.RowIndex);
                e.Row.ToolTip = "Click to select this row.";
            }
        }

        protected void GridViewCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            //the postback process which locks in the selected row, forgets the highlight formatting. this recreates it
            //need to add one since the html fields count the headers as number zero
            //note: EnableEventValidation="false" needs to be added to page for this to fire
            hfSelectedIndex.Value = (GridViewCustomers.SelectedIndex + 1).ToString();
            //RegisterDOMReadyScript("highlight", "higlightSelectedRow();");
            RegisterDOMReadyScript("button", "__doPostBack('ctl00$MainContent$buttonCloseModal', '');restoreDivPosition();");
            buttonCloseModal_Click(null, null);
            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "DoPostBack", "__doPostBack(sender, e)", true);
        }

        protected void GridViewCustomers_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Datasource select script was established during modal open. On sorting, the page has forgotten the select script
            SqlDataSourceGPCustomers.SelectCommand = "SELECT [Customer Number]Custnmbr,[Customer Name]Custname,[City],[State] FROM " + hfINTERID.Value + "..Customers WHERE Inactive='No'";

        }

        protected void SqlDataSourceGPCustomers_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            //SqlDataSourceGPCustomers.SelectCommand = "SELECT [Customer Number]Custnmbr,[Customer Name]Custname,[City],[State] FROM " + hfINTERID.Value + "..Customers WHERE Inactive='No'";
        }

        protected void TextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            SqlDataSourceGPCustomers.SelectCommand = "SELECT [Customer Number]Custnmbr,[Customer Name]Custname,[City],[State] FROM " + hfINTERID.Value + "..Customers WHERE Inactive='No'";
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Print/PrintDistrictList.aspx");
        }
    }
}
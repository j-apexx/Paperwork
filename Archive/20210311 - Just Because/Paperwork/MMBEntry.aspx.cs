using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class MMBEntry : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");
            else
                LabelProgramYear.Text = Session["ProgramYear"].ToString();

            if (!IsPostBack)
            {
                VerifyAccess(true);
                if (Session["AccessLevel"] == null)
                    Response.Redirect("~/Default.aspx");

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
            else
            {
                VerifyAccess(false);
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
                Response.Redirect(Request.RawUrl);//Request.RawUrl provides the current page URL
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void GridViewMMBEntry_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try { 
               SqlDataSourceMMBEntry.Insert();
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

        protected void GridViewMMBEntry_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ProgramYear"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceMMBEntry_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // Program Year and District are already set from Session or page dropdown
            TextBox TextBoxFDate = (TextBox)GridViewMMBEntry.FooterRow.FindControl("TextBoxFDate");
            DropDownList ddlFVendors = (DropDownList)GridViewMMBEntry.FooterRow.FindControl("ddlFVendors");
            DropDownList ddlFProducts = (DropDownList)GridViewMMBEntry.FooterRow.FindControl("ddlFProducts");
            DropDownList DropDownListFSchools = (DropDownList)GridViewMMBEntry.FooterRow.FindControl("DropDownListFSchools");
            TextBox TextBoxFQuantity = (TextBox)GridViewMMBEntry.FooterRow.FindControl("TextBoxFQuantity");
            TextBox TextBoxFPrice = (TextBox)GridViewMMBEntry.FooterRow.FindControl("TextBoxFPrice");
            CheckBox CBFMilkItem = (CheckBox)GridViewMMBEntry.FooterRow.FindControl("CBFMilkItem");

            //**TO DO** run script for date interpretation from 2 or 4 digit entry
            if (DateTime.TryParse(TextBoxFDate.Text, out DateTime DateOut))
                e.Command.Parameters["@MMBDate"].Value = DateOut;
            else
            {
                e.Cancel = true; return;
            }

            if(ddlFVendors.SelectedIndex==-1 || ddlFVendors.SelectedValue==string.Empty)
            { 
                e.Cancel = true; return; 
            }
            e.Command.Parameters["@VendorID"].Value = ddlFVendors.SelectedValue;

            if (Int16.TryParse(ddlFProducts.SelectedValue, out Int16 OutProdNum))
                e.Command.Parameters["@ProductCode"].Value = OutProdNum;
            else
            {
                e.Cancel = true; return;
            }

            if (Int16.TryParse(DropDownListFSchools.SelectedValue, out Int16 OutSchoolNum))
                e.Command.Parameters["@SchoolNum"].Value = OutSchoolNum;
            else
            {
                e.Cancel = true; return;
            }

            if (Int32.TryParse(TextBoxFQuantity.Text, out Int32 outQty))
                e.Command.Parameters["@Quantity"].Value = outQty;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFPrice.Text, out Decimal outPrice))
                e.Command.Parameters["@Price"].Value = outPrice;
            else
            {
                e.Cancel = true; return;
            }

            e.Command.Parameters["@MilkItem"].Value = CBFMilkItem.Checked;

            //all values are set, insert should be able to run
        }

        protected void SqlDataSourceMMBEntry_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);
                TextBox TextBoxFDate = (TextBox)GridViewMMBEntry.FooterRow.FindControl("TextBoxFDate");
                if (TextBoxFDate != null)
                {
                    TextBoxFDate.Focus();
                    
                }

                String MMBDateStr = DateTime.Parse(e.Command.Parameters["@MMBDate"].Value.ToString()).ToShortDateString();
                //Need to recreate the datepicker after postback - can't just show it.
                RegisterDOMReadyScript("Focus", "$('[id *= TextBoxFDate]').val('"+ MMBDateStr + "');$('[id *= TextBoxFDate]').datepicker({changeMonth: true,changeYear: true});");

            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        protected void SqlDataSourceMMBEntry_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Deleted", MessageType.Success);
            }
            else if(e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }


        }

        protected void ddlProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            Boolean WeAreEditing = false;
            DropDownList ddlProducts = (DropDownList)sender;
            if (ddlProducts == null || ddlProducts.SelectedIndex == -1)
                return;

            if (ddlProducts.ID == "ddlEProducts")
                WeAreEditing = true;

            if (WeAreEditing)
            {
                CheckBox CBEMilkItem = (CheckBox)FindControlRecursive(GridViewMMBEntry, "CBEMilkItem");
                TextBox TextBoxEPrice = (TextBox)FindControlRecursive(GridViewMMBEntry, "TextBoxEPrice");
                DropDownList DropDownListESchools = (DropDownList)FindControlRecursive(GridViewMMBEntry, "DropDownListESchools");

                if (CBEMilkItem != null && TextBoxEPrice != null)
                {
                    PopulateMMBItemValues(int.Parse(ddlProducts.SelectedValue), ref CBEMilkItem, ref TextBoxEPrice);
                    DropDownListESchools.Focus();
                }

            }
            else
            {
                CheckBox CBFMilkItem = (CheckBox)FindControlRecursive(GridViewMMBEntry, "CBFMilkItem");
                TextBox TextBoxFPrice = (TextBox)FindControlRecursive(GridViewMMBEntry, "TextBoxFPrice");
                DropDownList DropDownListFSchools = (DropDownList)FindControlRecursive(GridViewMMBEntry, "DropDownListFSchools");

                if (CBFMilkItem != null && TextBoxFPrice != null)
                {
                    PopulateMMBItemValues(int.Parse(ddlProducts.SelectedValue), ref CBFMilkItem, ref TextBoxFPrice);
                    DropDownListFSchools.Focus();
                }
            }
        }

        private void PopulateMMBItemValues(int prodCodeInt, ref CheckBox cBEMilkItem, ref TextBox textBoxEPrice)
        {
            if (prodCodeInt == 0 || cBEMilkItem == null || textBoxEPrice == null )
                return;

            string Dist = DropDownListDistricts.SelectedValue;

            decimal outprice = 0M;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT Price, MilkItem"
                                + " FROM [MMBItems]  "
                                + " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND ProductCode=@ProductCode";

            SqlCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = queryString;
            dbCommand.Connection = dbConnection;

            SqlParameter dbParam_DistID = new SqlParameter();
            dbParam_DistID.ParameterName = "@DistrictID";
            dbParam_DistID.Value = Dist;
            dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
            dbCommand.Parameters.Add(dbParam_DistID);

            SqlParameter dbParam_ProgYear = new SqlParameter();
            dbParam_ProgYear.ParameterName = "@ProgramYear";
            dbParam_ProgYear.Value = Session["ProgramYear"].ToString();
            dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
            dbCommand.Parameters.Add(dbParam_ProgYear);

            SqlParameter dbParam_prdCode = new SqlParameter();
            dbParam_prdCode.ParameterName = "@ProductCode";
            dbParam_prdCode.Value = prodCodeInt;
            dbParam_prdCode.DbType = System.Data.DbType.Int16;
            dbCommand.Parameters.Add(dbParam_prdCode);

            try
            {
                dbConnection.Open();
                SqlDataReader rdr = dbCommand.ExecuteReader();
                if(rdr.Read())
                {
                    Decimal.TryParse(rdr["Price"].ToString(), out outprice);//if this fails- zero is returned
                    textBoxEPrice.Text = outprice.ToString();

                    cBEMilkItem.Checked = rdr["Price"].ToString() == "0" ? false : true;
                }
                else { }

                if (rdr != null)
                    rdr.Close();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                dbConnection.Close();
            }
            
        }

        protected void GridViewMMBEntry_PreRender(object sender, EventArgs e)
        {
            // These register the postback only when focus leaves the dropdowns allowing users to use keyboard and type or arrow between values
            // without the index change script running everytime
            DropDownList ddlEProducts = (DropDownList)FindControlRecursive(GridViewMMBEntry, "ddlEProducts");
            if (ddlEProducts != null)
            {
                ddlEProducts.Attributes.Add("onchange", "__doPostBack('ddlEProducts','');");
            }

            DropDownList ddlFProducts = (DropDownList)FindControlRecursive(GridViewMMBEntry, "ddlFProducts");
            if (ddlFProducts != null)
            {
                ddlFProducts.Attributes.Add("onchange", "__doPostBack('ddlFProducts','');");
            }
        }
    }
}
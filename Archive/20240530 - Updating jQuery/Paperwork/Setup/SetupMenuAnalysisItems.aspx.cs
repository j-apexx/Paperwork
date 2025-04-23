using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupMenuAnalysisItems : Paperwork_Global
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

                //Set the programyear
                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                else
                {
                    if (Session["ProgramYear"] != null && DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }

                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;

                if(Session["AccessLevel"].ToString().ToUpper()=="ADMIN")
                {
                    ListItem li = new ListItem("*Default Items*", "000");
                    DropDownListDistricts.Items.Add(li);
                    DropDownListDistricts.AppendDataBoundItems = true;
                }

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

                PopulateDefaults();


                if (Session["LastItemCat"] !=null)
                {
                    DDLCategories.SelectedValue = Session["LastItemCat"].ToString();
                }

                DDLCategories_SelectedIndexChanged(null, null);


            }

            
        }


        protected void SqlDataSourceMenuCostItemsSetup_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Saved", MessageType.Success);

                //Refocus to allow quicker entry of new records
                TextBox TextBoxFDesc = (TextBox)FindControlRecursive(GridViewItemCostSetup, "TextBoxFDesc");
                if (TextBoxFDesc != null)
                    TextBoxFDesc.Focus();
            }
            else if (e.Exception != null)
            {
                //ShowMessageIcon("Error", MessageType.Error);
                ShowDialog("Error", MessageType.Error, "Menu Item was not inserted.", e.Exception.Message);
            }
        }

        protected void SqlDataSourceMenuCostItemsSetup_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            TextBox TextBoxFDesc = (TextBox)GridViewItemCostSetup.FooterRow.FindControl("TextBoxFDesc");
            TextBox TextBoxFCost = (TextBox)GridViewItemCostSetup.FooterRow.FindControl("TextBoxFCost");

            if (TextBoxFCost != null && string.IsNullOrEmpty(TextBoxFCost.Text))
                TextBoxFCost.Text = "0";

            if (decimal.TryParse(TextBoxFCost.Text, out decimal OutCost))
                e.Command.Parameters["@ItemCost"].Value = OutCost;
            else
            {
                e.Cancel = true; return;
            }

            if (TextBoxFDesc.Text == string.Empty)
                return;
            e.Command.Parameters["@ItemDescription"].Value = TextBoxFDesc.Text;
        }

        protected void SqlDataSourceMenuCostItemsSetup_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                Session["LastItemCat"] = DDLCategories.SelectedValue.ToString();  
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void GridViewItemCostSetup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceMenuCostItemsSetup.Insert();
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

        protected void GridViewItemCostSetup_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["ItemID"].ToString() == "0")
                e.Row.Visible = false;

            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null)
            {
                Label LabelCost = (Label)e.Row.FindControl("LabelCost");
                if(LabelCost!=null && !string.IsNullOrEmpty(HFCatMidLower.Value) && !string.IsNullOrEmpty(HFCatMidUpper.Value))
                {
                    decimal decCost = 0M;
                    decimal decLower = decimal.MaxValue;
                    decimal decUpper = decimal.MaxValue;
                    if (decimal.TryParse(LabelCost.Text, out decCost) == false)
                        decCost = 0M;
                    decimal.TryParse(HFCatMidLower.Value, out decLower);
                    decimal.TryParse(HFCatMidUpper.Value, out decUpper);

                    if (decCost < decLower)
                        LabelCost.ForeColor = System.Drawing.Color.LimeGreen;
                    else if (decCost >= decLower && decCost <= decUpper)
                        LabelCost.ForeColor = System.Drawing.Color.Goldenrod;
                    else if (decCost > decUpper)
                        LabelCost.ForeColor = System.Drawing.Color.Red;
                    else
                        LabelCost.ForeColor = System.Drawing.Color.Black;

                }
            }
        }

        protected void DDLCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["LastItemCat"] = DDLCategories.SelectedValue.ToString();

            //Category Changed - update the range hidden fields
            UpdatePriceRanges();
        }

        protected void UpdatePriceRanges()
        {
            //reset values
            HFCatMidLower.Value = "";
            HFCatMidUpper.Value = "";

            SqlDataSourceRanges.DataBind();
            DataView dv = (DataView)SqlDataSourceRanges.Select(DataSourceSelectArguments.Empty);
            foreach (DataRow dr in dv.Table.Rows)
            {
                decimal lowerRange = decimal.MaxValue;
                decimal upperRange = decimal.MaxValue;
                if (dr["MidrangeLower"] != null)
                {
                    decimal.TryParse(dr["MidrangeLower"].ToString(), out lowerRange);
                }
                if (dr["MidrangeUpper"] != null)
                {
                    decimal.TryParse(dr["MidrangeUpper"].ToString(), out upperRange);
                }

                HFCatMidLower.Value = lowerRange.ToString();
                HFCatMidUpper.Value = upperRange.ToString();

            }
        }

        protected void PopulateDefaults()
        {
            int existingRecordCount = 0;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            string checkForMenuCostRecords = "SELECT COUNT(*) FROM [MenuCostItemsSetup] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID";
            SqlConnection dbConnection = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(checkForMenuCostRecords, dbConnection))
            {
                dbConnection.Open();
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_dist = new SqlParameter();
                dbParam_dist.ParameterName = "@DistrictID";
                dbParam_dist.Value = DropDownListDistricts.SelectedValue;
                dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_dist);

                try
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out existingRecordCount);
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "SQL Error while trying to populate defaults.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error while trying to populate defaults.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

            if (existingRecordCount == 0)//Create new only if needed
            {
                string insertDefaultsQuery = "INSERT INTO [MenuCostItemsSetup] ([ProgramYear],[DistrictID],[ItemCategoryID],[ItemDescription],[ItemCost],[ItemID]) " +
                " SELECT @ProgramYear, @DistrictID, [ItemCategoryID],[ItemDescription],[ItemCost],[ItemID]" +
                " FROM [MenuCostItemsSetup] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]='000'";
                SqlConnection dbConnection2 = new SqlConnection(connectionString);
                using (SqlCommand cmd2 = new SqlCommand(insertDefaultsQuery, dbConnection2))
                {
                    dbConnection2.Open();
                    SqlParameter dbParam_ProgYear = new SqlParameter();
                    dbParam_ProgYear.ParameterName = "@ProgramYear";
                    dbParam_ProgYear.Value = DDLProgramYears.SelectedValue;
                    dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_ProgYear);

                    SqlParameter dbParam_dist = new SqlParameter();
                    dbParam_dist.ParameterName = "@DistrictID";
                    dbParam_dist.Value = DropDownListDistricts.SelectedValue;
                    dbParam_dist.DbType = System.Data.DbType.StringFixedLength;
                    cmd2.Parameters.Add(dbParam_dist);

                    try
                    {
                        int recordCount = cmd2.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "SQL Error while trying to populate defaults.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "Error while trying to populate defaults.", ex.Message);
                    }
                    finally
                    {
                        dbConnection2.Close();
                    }
                }

            }
        }

        protected void btnSubmitPastedRecords_Click(object sender, EventArgs e)
        {
            string[] PasteRow = TextBoxExcelPaste.Text.Split('\n');
            foreach(string row in PasteRow)
            {
                string[] rowCell = row.Split('\t');
                if(!string.IsNullOrEmpty(rowCell[0]))
                {
                    string menuItem = rowCell[0].Trim();

                    decimal decDost = 0M;

                    //if only menu items are imported - price will default to zero
                    if (rowCell.Length > 1)
                    {
                        string menuCost = rowCell[1];
                        if (menuCost.IndexOf('$') != -1)
                            menuCost = rowCell[1].Replace('$', ' ');
                        if (decimal.TryParse(menuCost, out decDost) == false)
                            decDost = 0M;
                    }

                    InsertItems(menuItem, decDost);
                }
            }

            TextBoxExcelPaste.Text = string.Empty;
        }

        private void InsertItems(string menuItem, decimal decDost)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            TextBox TextBoxFDesc = (TextBox)GridViewItemCostSetup.FooterRow.FindControl("TextBoxFDesc");
            TextBox TextBoxFCost = (TextBox)GridViewItemCostSetup.FooterRow.FindControl("TextBoxFCost");

            TextBoxFDesc.Text = menuItem;
            TextBoxFCost.Text = decDost.ToString();

            SqlDataSourceMenuCostItemsSetup.Insert();
        }


    }
}
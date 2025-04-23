using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupMMBItems : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");
            else
                LabelProgramYear.Text = Session["ProgramYear"].ToString();

            if (!IsPostBack)
            {
                VerifyAccess(true);

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

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");
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
        protected void GridViewMMBItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try { 
                    SqlDataSourceMMBItems.Insert();
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

        protected void GridViewMMBItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceMMBItems_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);

                //Refocus to allow quicker entry of new records
                TextBox TextBoxFProdCode = (TextBox)FindControlRecursive(GridViewMMBItems, "TextBoxFProdCode");
                if (TextBoxFProdCode != null)
                    TextBoxFProdCode.Focus();
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        protected void SqlDataSourceMMBItems_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            TextBox TextBoxFProdCode = (TextBox)GridViewMMBItems.FooterRow.FindControl("TextBoxFProdCode");
            TextBox TextBoxFProdName = (TextBox)GridViewMMBItems.FooterRow.FindControl("TextBoxFProdName");
            TextBox TextBoxFPrice = (TextBox)GridViewMMBItems.FooterRow.FindControl("TextBoxFPrice");
            CheckBox CBFMilkItem = (CheckBox)GridViewMMBItems.FooterRow.FindControl("CBFMilkItem");

            if (TextBoxFProdName.Text == String.Empty)
            {
                e.Cancel = true; return;
            }
            e.Command.Parameters["@ProductName"].Value = TextBoxFProdName.Text;

            if (Int16.TryParse(TextBoxFProdCode.Text, out Int16 ProdCode))
                e.Command.Parameters["@ProductCode"].Value = ProdCode;
            else
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFPrice.Text, out Decimal Price))
                e.Command.Parameters["@Price"].Value = Price;
            else
            {
                e.Cancel = true; return;
            }

            e.Command.Parameters["@MilkItem"].Value = CBFMilkItem.Checked ;
        }

        protected void SqlDataSourceMMBItems_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check to ensure the item with the specified key is not being used in a MMB Transaction
            //Check to ensure the Paycode has not been used
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.SqlClient.SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT Count(*) FROM [MMBTransactions] WHERE UPPER(DistrictID)=@DistrictID AND ProgramYear=@ProgramYear AND ProductCode=@ProductCode ";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                SqlParameter dbParam_DistrictID = new SqlParameter();
                dbParam_DistrictID.ParameterName = "@DistrictID";
                dbParam_DistrictID.Value = e.Command.Parameters["@old_DistrictID"].Value;
                dbParam_DistrictID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistrictID);

                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = e.Command.Parameters["@old_ProgramYear"].Value;
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_PrdCode = new SqlParameter();
                dbParam_PrdCode.ParameterName = "@ProductCode";
                dbParam_PrdCode.Value = e.Command.Parameters["@old_ProductCode"].Value;
                dbParam_PrdCode.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_PrdCode);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object paycodeUseCount = cmd.ExecuteScalar();
                    if (paycodeUseCount != null && (int)paycodeUseCount > 0)
                    {
                        e.Cancel = true;
                        ShowMessageIcon("Cannot Delete - records exist.", MessageType.Error);
                    }
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void SqlDataSourceMMBItems_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if(e.AffectedRows>0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }
    }
}
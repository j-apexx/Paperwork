using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupOtherSales : Paperwork_Global
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
            }
            else
            {
                VerifyAccess(false);
            }

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {

            }
        }

        protected void GridViewOtherSales_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                SqlDataSourceOtherSalesSetup.Insert();
            }

        }

        protected void GridViewOtherSales_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceOtherSalesSetup_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Saved", MessageType.Success);

                //Refocus to allow quicker entry of new records
                TextBox TextBoxFDesc = (TextBox)FindControlRecursive(GridViewOtherSales, "TextBoxFOtherSalesID");
                if (TextBoxFDesc != null)
                    TextBoxFDesc.Focus();
            }
            else if (e.Exception != null)
            {
                ShowMessageIcon("Error", MessageType.Error);
            }
        }

        protected void SqlDataSourceOtherSalesSetup_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }

        protected void SqlDataSourceOtherSalesSetup_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Updated", MessageType.Success);
            }
        }

        protected void SqlDataSourceOtherSalesSetup_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            TextBox TextBoxFOtherSalesID = (TextBox)GridViewOtherSales.FooterRow.FindControl("TextBoxFOtherSalesID");
            TextBox TextBoxFDescription = (TextBox)GridViewOtherSales.FooterRow.FindControl("TextBoxFDescription");

            if (Int16.TryParse(TextBoxFOtherSalesID.Text, out Int16 OutSalesID))
                e.Command.Parameters["@OtherSalesID"].Value = OutSalesID;
            else
            {
                e.Cancel = true; return;
            }

            if (TextBoxFDescription.Text == string.Empty)
                return;
            e.Command.Parameters["@OtherSalesDesc"].Value = TextBoxFDescription.Text;
        }

        protected void SqlDataSourceOtherSalesSetup_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check to ensure the Other Sales ID has not been used for current year transactions
            //Check to ensure the Paycode has not been used
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.SqlClient.SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT Count(*) FROM [OtherSales] WHERE UPPER(DistrictID)=@DistrictID AND ProgramYear=@ProgramYear AND OtherSalesID=@OtherSalesID ";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                SqlParameter dbParam_DistrictID = new SqlParameter();
                dbParam_DistrictID.ParameterName = "@DistrictID";
                dbParam_DistrictID.Value = e.Command.Parameters["@DistrictID"].Value;
                dbParam_DistrictID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistrictID);

                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = e.Command.Parameters["@ProgramYear"].Value;
                dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_OtherSalesID = new SqlParameter();
                dbParam_OtherSalesID.ParameterName = "@OtherSalesID";
                dbParam_OtherSalesID.Value = e.Command.Parameters["@OtherSalesID"].Value;
                dbParam_OtherSalesID.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_OtherSalesID);

                dbConnection.Open();
                try
                {
                    object oSalesUseCount = cmd.ExecuteScalar();
                    if (oSalesUseCount != null && (int)oSalesUseCount > 0)
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

    }
}
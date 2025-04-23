using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupPaycodes : Paperwork_Global
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
                DDLProgramYears.DataBind();
                if (DDLProgramYears.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                else
                {
                    if (DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }

            }
        }

        protected void GridViewPaycodes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try { 
                    SqlDataSourcePaycodes.Insert();
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

        protected void GridViewPaycodes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourcePaycodes_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Saved", MessageType.Success);

                //Refocus to allow quicker entry of new records
                TextBox TextBoxFDesc = (TextBox)FindControlRecursive(GridViewPaycodes, "TextBoxFDesc");
                if (TextBoxFDesc != null)
                    TextBoxFDesc.Focus();
            }
            else if (e.Exception != null)
            {
                //ShowMessageIcon("Error", MessageType.Error);
                ShowDialog("Error", MessageType.Error, "Paycode was not inserted.", e.Exception.Message);
            }
        }

        protected void SqlDataSourcePaycodes_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            TextBox TextBoxFDesc = (TextBox)GridViewPaycodes.FooterRow.FindControl("TextBoxFDesc");
            DropDownList DDLFHoursType = (DropDownList)GridViewPaycodes.FooterRow.FindControl("DDLFHoursType");
            DropDownList DDLFRateTier = (DropDownList)GridViewPaycodes.FooterRow.FindControl("DDLFRateTier");

            if (Int16.TryParse(DDLFHoursType.SelectedValue, out Int16 OutHrsTyp))
                e.Command.Parameters["@HoursType"].Value = OutHrsTyp;
            else
            {
                e.Cancel = true; return;
            }

            if (Int16.TryParse(DDLFRateTier.SelectedValue, out Int16 OutRateTier))
                e.Command.Parameters["@RateTier"].Value = OutRateTier;
            else
            {
                e.Cancel = true; return;
            }

            if (TextBoxFDesc.Text == string.Empty)
                return;
            e.Command.Parameters["@PayCodeDesc"].Value = TextBoxFDesc.Text;

        }

        protected void SqlDataSourcePaycodes_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if(e.AffectedRows>0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }

        protected void SqlDataSourcePaycodes_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check to ensure the Paycode has not been used
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.SqlClient.SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT Count(*) FROM [Payroll] WHERE UPPER(DistrictID)=@DistrictID AND ProgramYear=@ProgramYear AND PayCode=@PayCode ";

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

                SqlParameter dbParam_PayCode = new SqlParameter();
                dbParam_PayCode.ParameterName = "@PayCode";
                dbParam_PayCode.Value = e.Command.Parameters["@PayCode"].Value;
                dbParam_PayCode.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_PayCode);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object paycodeUseCount = cmd.ExecuteScalar();
                    if (paycodeUseCount != null && (int)paycodeUseCount>0)
                    {
                        e.Cancel = true;
                        ShowMessageIcon("Cannot Delete - records exist.", MessageType.Error);
                    }
                }
                catch (Exception ex )
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
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupRegionalMgrs : Paperwork_Global
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
        }

        protected void GridViewRMs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceRMs.Insert();
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

        protected void GridViewRMs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["RMNumber"].ToString() == "0")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceRMs_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check to ensure the RM has not been used
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.SqlClient.SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT Count(*) FROM [DistrictYearInfo] WHERE RMNUmber=@RMNumber ";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                SqlParameter dbParam_RMNum = new SqlParameter();
                dbParam_RMNum.ParameterName = "@RMNumber";
                dbParam_RMNum.Value = e.Command.Parameters["@RMNumber"].Value;
                dbParam_RMNum.DbType = System.Data.DbType.Int16;
                cmd.Parameters.Add(dbParam_RMNum);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object rmUseCount = cmd.ExecuteScalar();
                    if (rmUseCount != null && (int)rmUseCount > 0)
                    {
                        e.Cancel = true;
                        RegisterDOMReadyScript("Warning", "alert('Cannot delete this RM because they are in use on a District Year setup.');");
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

        protected void SqlDataSourceRMs_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            
            TextBox TextBoxFRMName = (TextBox)GridViewRMs.FooterRow.FindControl("TextBoxFRMName");
            CheckBox CBFActive=(CheckBox)GridViewRMs.FooterRow.FindControl("CBFActive");


            if (TextBoxFRMName.Text == string.Empty)
                return;

            e.Command.Parameters["@RMName"].Value = TextBoxFRMName.Text;
            e.Command.Parameters["@Active"].Value = CBFActive.Checked;
        }
    }
}
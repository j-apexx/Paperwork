using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupVPs : Paperwork_Global
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

            //Check for custom security
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            bool accessExplicitlyGranted = false;
            bool accessExplicitlyDenied = false;
            CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

            if (accessExplicitlyDenied == true)
                Response.Redirect("~/Default.aspx");
        }

        protected void GridViewVPs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceVPs.Insert();
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

        protected void GridViewVPs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["GM"].ToString() == "")
                e.Row.Visible = false;
        }

        protected void SqlDataSourceVPs_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check to ensure the RM has not been used
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.SqlClient.SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT Count(*) FROM [DistrictYearInfo] WHERE GM=@GM ";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                SqlParameter dbParam_RMNum = new SqlParameter();
                dbParam_RMNum.ParameterName = "@GM";
                dbParam_RMNum.Value = e.Command.Parameters["@old_GM"].Value;
                dbParam_RMNum.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_RMNum);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object vpUseCount = cmd.ExecuteScalar();
                    if (vpUseCount != null && (int)vpUseCount > 0)
                    {
                        e.Cancel = true;
                        ShowDialog("Warning", MessageType.Warning, "", "Cannot delete this VP because they are in use on a District Year setup.");
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    e.Cancel = true;
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                    e.Cancel = true;
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void SqlDataSourceVPs_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set

            TextBox TextBoxFGM = (TextBox)GridViewVPs.FooterRow.FindControl("TextBoxFGM");
            TextBox TextBoxFGMEmail = (TextBox)GridViewVPs.FooterRow.FindControl("TextBoxFGMEmail");
            TextBox TextBoxFBDGTUserID = (TextBox)GridViewVPs.FooterRow.FindControl("TextBoxFBDGTUserID");


            if (TextBoxFGM == null || TextBoxFGMEmail==null || TextBoxFBDGTUserID==null || string.IsNullOrEmpty(TextBoxFGM.Text) || string.IsNullOrEmpty(TextBoxFGMEmail.Text))
                return;

            e.Command.Parameters["@GM"].Value = TextBoxFGM.Text;
            e.Command.Parameters["@GMEmail"].Value = TextBoxFGMEmail.Text;
            e.Command.Parameters["@BDGTUserID"].Value = TextBoxFBDGTUserID.Text;
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupUsers:Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"]==null || Session["AccessLevel"].ToString() != "Admin")
                Response.Redirect("~/Default.aspx");
        }

        protected void DetailsViewUserEntry_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (e.Exception != null)
            {
                if (e.Exception.Message.IndexOf("Cannot insert duplicate key in object") > 0)
                    LabelInsertErrors.Text = "Error Inserting: User record already exists.";
                else
                    LabelInsertErrors.Text = "Error Inserting: " + e.Exception.Message;
                e.ExceptionHandled = true;
            }
            else
                LabelInsertErrors.Text = String.Empty;
        }

        protected void DetailsView1_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            Label lErrors = (Label)FindControlRecursive(GridView1, "LabelErrors");
            if (e.Exception != null)
            {
                if (e.Exception.Message.IndexOf("Cannot insert duplicate key in object") > 0)
                {
                    if (lErrors != null)
                        lErrors.Text = "Error Inserting: record already exists.";
                }
                else
                    lErrors.Text = string.Empty;
                //    LabelErrors.Text = "Error Inserting: " + e.Exception.Message;
                e.ExceptionHandled = true;
            }
            else
                lErrors.Text = string.Empty;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (Context.User.Identity.Name.ToUpper().TrimEnd(' ') == e.Keys["UserID"].ToString().ToUpper().TrimEnd(' '))
            {
                //LabelInsertErrors.Text = "Counts are entered for the selected building. You cannot delete a building with counts.";
                RegisterDOMReadyScript("alert message", "alert('You cannot delete your own account.');");
                e.Cancel = true;
            }

        }

        protected void SqlDataSource2_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            ListBox lbDistricts = (ListBox)FindControlRecursive(GridView1, "lbDistricts");

            if (lbDistricts != null)
            {

                foreach (ListItem itm in lbDistricts.Items)
                {
                    if (itm.Selected)
                    {
                        AssignDistrictAccess(e.Command.Parameters["@UserID"].Value.ToString(), itm.Value.ToString());
                    }
                }
            }
            //no need for the embedded insert to run at this point.
            e.Cancel = true;
            //update the GridView
            GridView1.DataBind();
        }

        protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            //If user was deleted - delete the  assignments
            if (e.Exception == null)
            {

            }
        }

        /// <summary>
        /// Creates a use access record for the specified user and district
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dist"></param>
        protected void AssignDistrictAccess(string user, string distID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string queryRemoveOtherActives = "INSERT INTO [UserDistrictAccess] ([UserID], [DistrictID]) VALUES (UPPER(@UserID), @DistID)";

            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                SqlCommand cmdUpdateInvYear = new SqlCommand(queryRemoveOtherActives, dbConnection);

                SqlParameter dbParam_uSrID = new SqlParameter();
                dbParam_uSrID.ParameterName = "@UserID";
                dbParam_uSrID.Value = user;
                dbParam_uSrID.DbType = System.Data.DbType.StringFixedLength;
                cmdUpdateInvYear.Parameters.Add(dbParam_uSrID);

                SqlParameter dbParam_dIstID = new SqlParameter();
                dbParam_dIstID.ParameterName = "@DistID";
                dbParam_dIstID.Value = distID;
                dbParam_dIstID.DbType = System.Data.DbType.StringFixedLength;
                cmdUpdateInvYear.Parameters.Add(dbParam_dIstID);

                try
                {
                    cmdUpdateInvYear.ExecuteNonQuery();
                }
                catch
                { }
                cmdUpdateInvYear.Dispose();
            }
        }


    }
}
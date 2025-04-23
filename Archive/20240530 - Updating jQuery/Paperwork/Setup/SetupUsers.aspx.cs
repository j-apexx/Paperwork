using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupUsers : Paperwork_Global
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

        protected void ISearch_Click(object sender, EventArgs e)
        {
            TextBoxSearch.Focus();
        }

        protected void IClear_Click(object sender, EventArgs e)
        {
            TextBoxSearch.Text = "";
            TextBoxSearch.Focus();
        }

        protected void SqlDataSourceUsers_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            for (int i = 0; i < e.ParameterValues.Count; i++)
            {
                if (e.ParameterValues[i] != null)
                    e.ParameterValues[i] = e.ParameterValues[i].ToString().Replace("'", "''").Replace("%", "[%]");
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName=="Update")
            {
            }
        }

        protected void IAdvanced_Click(object sender, EventArgs e)
        {
            LabelUserID.Text = ((LinkButton)sender).CommandArgument.ToString().Trim();

            RegisterDOMReadyScript("show", "$('#ModalCustomSecurity').modal('show');");
            UpdatePanelModalCustomSecurity.Update();
        }

        protected void GridViewCustomSecurity_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["SecurityObject"].ToString() == "")
                    e.Row.Visible = false;
            }
            catch (Exception ex) { }
        }

        protected void CBAccess_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbSender = (CheckBox)sender;
            GridViewRow gvr = (GridViewRow)FindParentControlOfType(cbSender, "GridViewRow");
            if (gvr == null) { return; }
            TextBox TextBoxFSecurityObject = new TextBox();
            Label LabelSecurityObject = new Label();

            switch (cbSender.ID)
            {
                case ("CBAccessGranted"):
                    CheckBox CBAccessDenied = (CheckBox)cbSender.Parent.FindControl("CBAccessDenied");
                    if(CBAccessDenied != null)
                    {
                        if(cbSender.Checked)
                            CBAccessDenied.Checked = false;
                        else
                            CBAccessDenied.Checked = true;
                    }
                    LabelSecurityObject = (Label)FindControlRecursive(gvr, "LabelSecurityObject");
                    if (LabelSecurityObject != null)
                        UpdateSecurity(LabelUserID.Text, LabelSecurityObject.Text, cbSender.Checked, CBAccessDenied.Checked);
                    break;
                case ("CBAccessDenied"):
                    CheckBox CBAccessGranted = (CheckBox)cbSender.Parent.FindControl("CBAccessGranted");
                    if (CBAccessGranted != null)
                    {
                        if (cbSender.Checked)
                            CBAccessGranted.Checked = false;
                        else
                            CBAccessGranted.Checked = true;
                    }
                    LabelSecurityObject = (Label)FindControlRecursive(gvr, "LabelSecurityObject");
                    if (LabelSecurityObject != null)
                        UpdateSecurity(LabelUserID.Text, LabelSecurityObject.Text, CBAccessGranted.Checked, cbSender.Checked);
                    break;
                case ("CBFAccessGranted"):
                    CheckBox CBFAccessDenied = (CheckBox)cbSender.Parent.FindControl("CBFAccessDenied");
                    if (CBFAccessDenied != null)
                    {
                        if (cbSender.Checked)
                            CBFAccessDenied.Checked = false;
                        else
                            CBFAccessDenied.Checked = true;
                    }
                    TextBoxFSecurityObject = (TextBox)FindControlRecursive(gvr, "TextBoxFSecurityObject");
                    break;
                case ("CBFAccessDenied"):
                    CheckBox CBFAccessGranted = (CheckBox)cbSender.Parent.FindControl("CBFAccessGranted");
                    if (CBFAccessGranted != null)
                    {
                        if (cbSender.Checked)
                            CBFAccessGranted.Checked = false;
                        else
                            CBFAccessGranted.Checked = true;
                    }
                    TextBoxFSecurityObject = (TextBox)FindControlRecursive(gvr, "TextBoxFSecurityObject");
                    break;
                default:
                    break;
            }
        }

        protected void UpdateSecurity(string userID, string secObj, bool granted, bool denied)
        {
            string sqlUpdate = "UPDATE [UserAccessCustomSecurity] SET [AccessAllowed]=@AccessAllowed, [AccessDenied]=@AccessDenied WHERE [UserID]=@UserID AND [SecurityObject]=@SecurityObject";
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString); 
            using (SqlCommand cmd = new SqlCommand(sqlUpdate, dbConnection))
            {
                #region keyvals
                SqlParameter dbParam_user = new SqlParameter();
                dbParam_user.ParameterName = "@UserID";
                dbParam_user.Value = userID;
                dbParam_user.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_user);

                SqlParameter dbParam_secObject = new SqlParameter();
                dbParam_secObject.ParameterName = "@SecurityObject";
                dbParam_secObject.Value = secObj;
                dbParam_secObject.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_secObject);
                #endregion
                SqlParameter dbParam_granted = new SqlParameter();
                dbParam_granted.ParameterName = "@AccessAllowed";
                dbParam_granted.Value = granted?1:0;
                dbParam_granted.DbType = System.Data.DbType.Byte;
                cmd.Parameters.Add(dbParam_granted);

                SqlParameter dbParam_denied = new SqlParameter();
                dbParam_denied.ParameterName = "@AccessDenied";
                dbParam_denied.Value = denied?1:0;
                dbParam_denied.DbType = System.Data.DbType.Byte;
                cmd.Parameters.Add(dbParam_denied);

                try
                {
                    dbConnection.Open();
                    int recordCount = 0;
                    recordCount = cmd.ExecuteNonQuery();
                }
                catch(SqlException ex)
                {
                    ShowDialog("SQL Error", MessageType.Error, "A SQL error occurred while updating the custom security.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred while updating the custom security.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }


        }

        protected void SqlDataSourceCustomSecurity_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            TextBox TextBoxFSecurityObject = (TextBox)GridViewCustomSecurity.FooterRow.FindControl("TextBoxFSecurityObject");
            CheckBox CBFAccessGranted = (CheckBox)GridViewCustomSecurity.FooterRow.FindControl("CBFAccessGranted");
            CheckBox CBFAccessDenied = (CheckBox)GridViewCustomSecurity.FooterRow.FindControl("CBFAccessDenied");
            if (string.IsNullOrEmpty(LabelUserID.Text) || string.IsNullOrEmpty(TextBoxFSecurityObject.Text))
                return;

            if( CBFAccessGranted.Checked && CBFAccessDenied.Checked ) { CBFAccessGranted.Checked = false; }

            e.Command.Parameters["@UserID"].Value = LabelUserID.Text;
            e.Command.Parameters["@SecurityObject"].Value = TextBoxFSecurityObject.Text;
            e.Command.Parameters["@AccessAllowed"].Value = CBFAccessGranted.Checked;
            e.Command.Parameters["@AccessDenied"].Value = CBFAccessDenied.Checked;
        }

        protected void GridViewCustomSecurity_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceCustomSecurity.Insert();
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
    }
}
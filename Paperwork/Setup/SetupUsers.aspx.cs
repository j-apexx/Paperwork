using System;
using System.Configuration;
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

        /// <summary>
        /// Used when districts are added to user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            string uID = e.Keys["UserID"].ToString().Trim();

            if (Context.User.Identity.Name.ToUpper().TrimEnd(' ') == uID)
            {
                //LabelInsertErrors.Text = "Counts are entered for the selected building. You cannot delete a building with counts.";
                ShowDialog("Error", MessageType.Error, "You cannot delete your own account.", "");
                //RegisterDOMReadyScript("alert message", "alert('You cannot delete your own account.');");
                e.Cancel = true;
            }

            DeleteUserFromUSDA(uID);

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

        protected void DetailsViewAvailDists_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            DetailsView DetailsViewAvailDists = (DetailsView)sender;
            HiddenField HFCurrentUser = (HiddenField)FindControlRecursive(DetailsViewAvailDists.Parent, "HFCurrentUser");
            if (HFCurrentUser == null)
                return;
            string userID = HFCurrentUser.Value.Trim();
            if (e.Values["DistrictID"] == null)
                return;
            string distID = e.Values["DistrictID"].ToString();

            AddUserDistrictToUSDA(userID, distID);
        }

        protected void DetailsViewUserEntry_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            DetailsView DetailsViewUserEntry = (DetailsView)sender;
            CheckBox cbUSDA = (CheckBox)DetailsViewUserEntry.FindControl("cbUSDA");
            if (cbUSDA != null && cbUSDA.Checked)
            {
                //e.Values["USDA"] = cbUSDA.Checked;
                string userID = e.Values["UserID"].ToString();
                int.TryParse(e.Values["AccessLevel"].ToString(), out int intAccessLevel);
                if (intAccessLevel == 10)
                    intAccessLevel = 99;//There is no Office level in USDA. Set to admin.

                int recordCount = AddUserToUSDA(userID, intAccessLevel);
            }
        }

        #region USDA Procedures

        protected int AddUserToUSDA(string userID, int accessLevel)
        {
            if (isTRAINING() || isTOOLBOX() || isSFSP())
            {
                return 0;
            }

            if (IsUserInUSDA(userID))
            {
                return 1;
            }

            string sql = "INSERT INTO [UserAccess] (UserID, AccessLevel) VALUES (@UserID, @AccessLevel)";
            int recordCount = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnUSDA"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@AccessLevel", accessLevel);
                        conn.Open();
                        recordCount = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }

            return recordCount;
        }

        protected int AddUserDistrictToUSDA(string userID, string districtID)
        {
            if (isTRAINING() || isTOOLBOX() || isSFSP())
            {
                return 0;
            }

            if (IsUserInUSDADistrict(userID, districtID))
            {
                return 1;
            }

            string sql = "INSERT INTO [UserDistrictAccess] (UserID, DistrictID) VALUES (@UserID, @DistrictID)";
            int recordCount = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnUSDA"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@DistrictID", districtID);
                        conn.Open();
                        recordCount = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }

            return recordCount;
        }

        protected int DeleteUserFromUSDA(string userID)
        {
            if(isTRAINING() || isTOOLBOX() || isSFSP())
            {
                return 0;
            }   


            string sql = "DELETE [UserAccess] WHERE UserID = @UserID;DELETE [UserDistrictAccess] WHERE UserID=@UserID";
            int recordCount = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnUSDA"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        conn.Open();
                        recordCount = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }

            return recordCount;
        }

        protected int DeleteUserDistrictFromUSDA(string userID, string districtID)
        {
            if (isTRAINING() || isTOOLBOX() || isSFSP())
            {
                return 0;
            }

            string sql = "DELETE [UserDistrictAccess] WHERE UserID = @UserID AND DistrictID=@DistrictID";
            int recordCount = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnUSDA"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@DistrictID", districtID);
                        conn.Open();
                        recordCount = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }

            return recordCount;
        }

        protected bool IsUserInUSDA(string userID)
        {
            bool isInUSDA = false;
            string sql = "SELECT COUNT(*) FROM [UserAccess] WHERE [UserID]=@UserID";
            int recordCount = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnUSDA"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        SqlParameter dbParam_user = new SqlParameter();
                        dbParam_user.ParameterName = "@UserID";
                        dbParam_user.Value = userID;
                        dbParam_user.DbType = System.Data.DbType.String;
                        cmd.Parameters.Add(dbParam_user);


                        conn.Open();
                        object objCount = cmd.ExecuteScalar();
                        if (objCount != null)
                        {
                            int.TryParse(objCount.ToString(), out recordCount);
                            if(recordCount > 0)
                                isInUSDA = true;
                        }
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }
            return isInUSDA;
        }

        protected bool IsUserInUSDADistrict(string userID, string districtID)
        {
            bool isInUSDADist = false;
            string sql = "SELECT COUNT(*) FROM [UserDistrictAccess] WHERE [UserID]=@UserID AND [DistrictID]=@DistrictID";
            int recordCount = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnUSDA"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        SqlParameter dbParam_user = new SqlParameter();
                        dbParam_user.ParameterName = "@UserID";
                        dbParam_user.Value = userID;
                        dbParam_user.DbType = System.Data.DbType.String;
                        cmd.Parameters.Add(dbParam_user);

                        SqlParameter dbParam_dist = new SqlParameter();
                        dbParam_dist.ParameterName = "@DistrictID";
                        dbParam_dist.Value = districtID;
                        dbParam_dist.DbType = System.Data.DbType.String;
                        cmd.Parameters.Add(dbParam_dist);


                        conn.Open();
                        object objCount = cmd.ExecuteScalar();
                        if (objCount != null)
                        {
                            int.TryParse(objCount.ToString(), out recordCount);
                            if (recordCount > 0)
                                isInUSDADist = true;
                        }
                    }
                    catch (SqlException ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An SQL error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("USDA Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                }
            }
            return isInUSDADist;
        }


        #endregion

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow gvr = e.Row;
            
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string uID = (string)GridView1.DataKeys[gvr.RowIndex].Values["UserID"].ToString().Trim();
                if (IsUserInUSDA(@uID))
                {
                    CheckBox CBInUSDA = (CheckBox)gvr.FindControl("CBInUSDA");
                    if (CBInUSDA != null)
                        CBInUSDA.Checked = true;
                    CheckBox CBInUSDAEdit = (CheckBox)gvr.FindControl("CBInUSDAEdit");
                    if (CBInUSDAEdit != null)
                        CBInUSDAEdit.Checked = true;
                }
            }
        }

        protected void CBInUSDA_CheckedChanged(object sender, EventArgs e)
        {
            if (isTRAINING() || isTOOLBOX() || isSFSP())
            {
                return;
            }

            CheckBox cbUSDA = (CheckBox)sender;
            GridViewRow gvr = (GridViewRow)FindParentControlOfType(cbUSDA, "GridViewRow");
            string uID = (string)GridView1.DataKeys[gvr.RowIndex].Values["UserID"].ToString().Trim();


            if (cbUSDA.Checked)
            {
                DropDownList ddlAccessLevel = (DropDownList)FindControlRecursive(gvr,"ddlAccessLevel");
                int intAccessLevel = 0;

                if (ddlAccessLevel != null)
                    int.TryParse(ddlAccessLevel.SelectedValue, out intAccessLevel);

                if (intAccessLevel == 10)
                    intAccessLevel = 99;//There is no Office level in USDA. Set to admin.

                int recordCount = AddUserToUSDA(uID, intAccessLevel);

                if(intAccessLevel==0)
                {
                    GridView GridViewDistrictAccess = (GridView)FindControlRecursive(gvr, "GridViewDistrictAccess");
                    if(GridViewDistrictAccess != null)
                    {
                        foreach(DataKey dk in GridViewDistrictAccess.DataKeys)
                        {
                            string dist = dk.Values["DistrictID"].ToString().Trim();
                            AddUserDistrictToUSDA(uID, dist);
                        }
                    }
                }
            }
            else
            {
                DeleteUserFromUSDA(uID);

            }
        }

        protected void GridViewDistrictAccess_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (isTRAINING() || isTOOLBOX() || isSFSP())
            {
                return;
            }

            string distID = e.Keys["DistrictID"].ToString();
            string userID = e.Keys["UserID"].ToString().Trim();
            DeleteUserDistrictFromUSDA(userID, distID);
        }
    }
}
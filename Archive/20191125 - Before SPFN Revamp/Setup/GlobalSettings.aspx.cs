using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class GlobalSettings : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"] == null || Session["AccessLevel"].ToString() != "Admin")
                Response.Redirect("~/Default.aspx");

            if(!IsPostBack)
            {
                HFSettingsSaved.Value = "0";
            }
            else
            {
                //if save failed - try again
                if (HFSettingsSaved.Value == "FAILED")
                    SaveSettingsInfo();
            }
        }

        protected void TextBoxSettings_TextChanged(object sender, EventArgs e)
        {
            SaveSettingsInfo();
        }

        private void SaveSettingsInfo()
        {
            if (HFSettingsSaved.Value == "1")
                return;

            #region ReferenceControls
            TextBox TextBoxReinhartInvSuffix = (TextBox)FindControlRecursive(FormViewGlobalSettings, "TextBoxReinhartInvSuffix");
            TextBox TextBoxSSRSUrl = (TextBox)FindControlRecursive(FormViewGlobalSettings, "TextBoxSSRSUrl");
            TextBox TextBoxPayablesExportPath = (TextBox)FindControlRecursive(FormViewGlobalSettings, "TextBoxPayablesExportPath");
            #endregion

            string UpdateQuery = "UPDATE [PaperworkGlobalSettings] SET ReinhartInvSuffix=@ReinhartInvSuffix " +
                    " ,SSRS_URL=@SSRS_URL,PayablesExportFolder=@PayablesExportFolder " +
                    " WHERE [Index]=0 ";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(UpdateQuery, dbConnection))
            {
                SqlParameter dbParam_ReinInvSuffix = new SqlParameter();
                dbParam_ReinInvSuffix.ParameterName = "@ReinhartInvSuffix";
                dbParam_ReinInvSuffix.Value = TextBoxReinhartInvSuffix.Text;
                dbParam_ReinInvSuffix.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_ReinInvSuffix);

                SqlParameter dbParam_SSRSUrl = new SqlParameter();
                dbParam_SSRSUrl.ParameterName = "@SSRS_URL";
                dbParam_SSRSUrl.Value = TextBoxSSRSUrl.Text;
                dbParam_SSRSUrl.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_SSRSUrl);

                SqlParameter dbParam_PayExportPath = new SqlParameter();
                dbParam_PayExportPath.ParameterName = "@PayablesExportFolder";
                dbParam_PayExportPath.Value = TextBoxPayablesExportPath.Text;
                dbParam_PayExportPath.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_PayExportPath);

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd.ExecuteNonQuery();
                    //if (recordcount > 0)
                    //    ShowMessageIcon("Saved. ", MessageType.Success);
                }
                catch (SqlException ex)
                {
                    RegisterDOMReadyScript("error message", "alert('SQL Error saving the district invoice details.');");
                    HFSettingsSaved.Value = "FAILED";//failsafe - if encountered during postback save will try again
                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error saving the district invoice details.');");
                    HFSettingsSaved.Value = "FAILED";
                }
                finally
                {
                    dbConnection.Close();
                }

                if (recordcount > 0)
                    HFSettingsSaved.Value = "1";
            }
        }

        protected void FormViewGlobalSettings_PreRender(object sender, EventArgs e)
        {
            if (HFSettingsSaved.Value == "1")
            {
                ShowMessageIcon("Settings Saved. ", MessageType.Success);

                //reset the changed record indicator
                HFSettingsSaved.Value = "0";
            }
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupEmail : Paperwork_Global
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

            if (!IsPostBack)
            {
                HFSettingsSaved.Value = "0";

                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");
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



        protected void CB_UseEWS_CheckedChanged(object sender, EventArgs e)
        {
            HiddenField HFCB = (HiddenField)FindControlRecursive(DetailsViewEmailSettings, "HFEWS");
            if (HFCB != null)
            {
                if (((CheckBox)sender).Checked)
                    HFCB.Value = "1";
                else
                    HFCB.Value = "0";
            }
        }

        protected void CB_UseSSL_CheckedChanged(object sender, EventArgs e)
        {
            HiddenField HFCB = (HiddenField)FindControlRecursive(DetailsViewEmailSettings, "HFSSL");
            if (HFCB != null)
            {
                if (((CheckBox)sender).Checked)
                    HFCB.Value = "1";
                else
                    HFCB.Value = "0";
            }
        }



    }
}
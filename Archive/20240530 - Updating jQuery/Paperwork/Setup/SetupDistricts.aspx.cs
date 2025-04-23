using System;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupDistricts : Paperwork_Global
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
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");
            }
        }

        protected void CbEnforcePeriodsEdit_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string checkFieldStr = cb.Checked ? "1" : "0";
            HiddenField hf = (HiddenField)FindControlRecursive(cb.Parent, "hfEnforcePeriods");

            hf.Value = checkFieldStr;
        }

        protected void cbInactiveEdit_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string checkFieldStr = cb.Checked ? "1" : "0";
            HiddenField hf = (HiddenField)FindControlRecursive(cb.Parent, "hfInactive");

            hf.Value = checkFieldStr;
        }

        protected void SqlDataSourceDistricts_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //string errors = "";
        }

        protected void SqlDataSourceDistricts_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e == null || e.AffectedRows == 0)
            {
                if (e.Exception != null)
                {
                    if (e.Exception.Message.IndexOf("duplicate") >= 0)
                        ShowDialog("Error", MessageType.Error, "District could not be created in Paperwork.", "Duplicate found.");
                    else
                        ShowDialog("Error", MessageType.Error, "District was not created in Paperwork.", e.Exception.Message);
                    e.ExceptionHandled = true;
                }
                return;
            }
        }
    }
}
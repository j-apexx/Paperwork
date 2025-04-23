using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupDistricts : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);
            if (Session["AccessLevel"] == null || Session["AccessLevel"].ToString() != "Admin")
                Response.Redirect("~/Default.aspx");
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
            //string errors = "";
        }
    }
}
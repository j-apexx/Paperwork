using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupDistrictsInvTerms : Paperwork_Global
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

                DDLProgramYears.DataBind();
                try
                {
                    if (Session["ProgramYear"] != null)
                    {
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    }
                }
                catch (Exception ex) { }

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

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            SqlDataSourceProgramYears.DataBind();

            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                dvSql.Sort = "ProgramYear";
                int recordIndex = dvSql.Find(DDLProgramYears.SelectedValue);
                DataRowView drvSql = dvSql[recordIndex];
                HFBudgetYear.Value = drvSql["BudgetYear"].ToString().TrimEnd(' ');
            }
            else
            {
                HFBudgetYear.Value = "";
            }
        }

        protected void GridViewDistricts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Retrieve the Budget Info and update the Advance checkbox
            if (GridViewDistricts.DataKeys.Count > 0 && e.Row.RowIndex>=0)
            {
                string distID = GridViewDistricts.DataKeys[e.Row.RowIndex]["DistrictID"].ToString();
                if (string.IsNullOrEmpty(distID))
                    return;
                GridViewRow gvr = e.Row;

                SqlDataSourceBDGT.SelectParameters["BidNumber"].DefaultValue = distID;
                SqlDataSourceBDGT.DataBind();

                DataView dvSql = (DataView)SqlDataSourceBDGT.Select(DataSourceSelectArguments.Empty);

                if (dvSql != null && dvSql.Count > 0)
                {
                    DataRowView drvSql = dvSql[0];

                    CheckBox cbPrePay = (CheckBox)FindControlRecursive(gvr, "cbPrePay");
                    if (cbPrePay != null)
                        cbPrePay.Checked = (bool)drvSql["TNGAdvancePaymentBit"];

                    CheckBox cbPrePayEdit = (CheckBox)FindControlRecursive(gvr, "cbPrePayEdit");
                    if (cbPrePayEdit != null)
                        cbPrePayEdit.Checked = (bool)drvSql["TNGAdvancePaymentBit"];

                    dvSql.Dispose();
                }



            }


        }
    }
}
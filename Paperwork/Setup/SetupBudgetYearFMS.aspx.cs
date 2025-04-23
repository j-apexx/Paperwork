using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupBudgetYearFMS : Paperwork_Global
    {
        private string InsertStatusString = "";

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

        protected void CBBudgets_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            string checkFieldName = "";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);

            string rowKey = GridViewBudgetYears.DataKeys[gvr.RowIndex]["BudgetYear"].ToString();

            switch (((CheckBox)sender).ID)
            {
                case "CheckBoxEntryAllowed":
                    checkFieldName = "CB_AllowNewBudgets";
                    break;
                case "CheckBoxEditingAllowed":    
                    checkFieldName = "CB_AllowEditExisting";
                    break;
                default:
                    return;
                    break;
            }


            //Now update the individual row
            string querySetActive = "UPDATE FMSBudgetYear_Setup SET "+ checkFieldName + "=" + checkFieldStr + " WHERE UPPER(BudgetYear) = @BudgetYear ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                SqlCommand cmdUpdateBudgetYear = new SqlCommand(querySetActive, dbConnection);

                SqlParameter dbParam_pRgYear = new SqlParameter();
                dbParam_pRgYear.ParameterName = "@BudgetYear";
                dbParam_pRgYear.Value = rowKey;
                dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                cmdUpdateBudgetYear.Parameters.Add(dbParam_pRgYear);

                cmdUpdateBudgetYear.ExecuteNonQuery();
                cmdUpdateBudgetYear.Dispose();
            }

            if(((CheckBox)sender).Checked)
                CreateLogEntry(((CheckBox)sender).ID + " checked for BudgetYear " + rowKey);
            else
                CreateLogEntry(((CheckBox)sender).ID + " unchecked for BudgetYear " + rowKey);

            ((GridView)gvr.Parent.Parent).DataBind();
        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox TextBoxDate = (TextBox)sender;
            NormalizeSimplifiedDate(ref TextBoxDate);

            if (TextBoxDate.ID == "TextBoxStartDate")
            {
                TextBox TextBoxEndDate = (TextBox)FindControlRecursive(DetailsViewInsertBudgetYear, "TextBoxEndDate");
                TextBoxEndDate.Focus();
            }

            if (TextBoxDate.ID == "TextBoxEndDate")
            {
                DropDownList DDLBudgetYears = (DropDownList)FindControlRecursive(DetailsViewInsertBudgetYear, "DDLBudgetYears");
                DDLBudgetYears.Focus();
            }
        }

        protected void CreateLogEntry(string LogBasicString)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

            StringBuilder sb = new StringBuilder();

            sb.Append(LogBasicString);

            string fullLogEntry = sb.ToString();

            CreateGlobalLogEntry(className, fullLogEntry);
        }

    }

}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupProgramYear : Paperwork_Global
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
        }

        protected void CheckBoxEntryAllowed_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);
            string rowKey = gvr.Cells[0].Text;

            if (((CheckBox)sender).Checked == true)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

                string queryRemoveOtherActives = "UPDATE PROGRAMYEAR SET AllowDataEntry=0 WHERE UPPER(ProgramYear) != @ProgramYear ";

                using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
                {
                    dbConnection.Open();
                    SqlCommand cmdUpdateProgramYear = new SqlCommand(queryRemoveOtherActives, dbConnection);

                    SqlParameter dbParam_pRgYear = new SqlParameter();
                    dbParam_pRgYear.ParameterName = "@ProgramYear";
                    dbParam_pRgYear.Value = rowKey;
                    dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmdUpdateProgramYear.Parameters.Add(dbParam_pRgYear);

                    cmdUpdateProgramYear.ExecuteNonQuery();
                    cmdUpdateProgramYear.Dispose();
                }
            }

            //Now update the individual row
            string querySetActive = "UPDATE PROGRAMYEAR SET AllowDataEntry=" + checkFieldStr + " WHERE UPPER(ProgramYear) = @ProgramYear ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                SqlCommand cmdUpdateProgramYear = new SqlCommand(querySetActive, dbConnection);

                SqlParameter dbParam_pRgYear = new SqlParameter();
                dbParam_pRgYear.ParameterName = "@ProgramYear";
                dbParam_pRgYear.Value = rowKey;
                dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                cmdUpdateProgramYear.Parameters.Add(dbParam_pRgYear);

                cmdUpdateProgramYear.ExecuteNonQuery();
                cmdUpdateProgramYear.Dispose();
            }

            ((GridView)gvr.Parent.Parent).DataBind();
        }

        protected void CheckBoxEntryMonthClosed_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);
            string rowKey = gvr.Cells[0].Text;
            string columnName = ((CheckBox)sender).ID;

            //Now update the individual row
            string querySetActive = "UPDATE PROGRAMYEAR SET " + columnName + "=" + checkFieldStr + " WHERE UPPER(ProgramYear) = @ProgramYear ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                using (SqlCommand cmdUpdatePrgYear = new SqlCommand(querySetActive, dbConnection))
                {
                    SqlParameter dbParam_pRgYear = new SqlParameter();
                    dbParam_pRgYear.ParameterName = "@ProgramYear";
                    dbParam_pRgYear.Value = rowKey;
                    dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmdUpdatePrgYear.Parameters.Add(dbParam_pRgYear);

                    cmdUpdatePrgYear.ExecuteNonQuery();
                }
            }

            ((GridView)gvr.Parent.Parent).DataBind();
        }

        protected void DDLEditBudgetYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlBudYear = ((DropDownList)sender);
            GridViewRow gvr = ((GridViewRow)ddlBudYear.Parent/*Cell*/.Parent);
            string rowKey = gvr.Cells[0].Text;

            //Now update the individual row
            string querySetActive = "UPDATE PROGRAMYEAR SET BudgetYear=@BudgetYear WHERE UPPER(ProgramYear) = @ProgramYear ";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                try
                {
                    using (SqlCommand cmdUpdatePrgYear = new SqlCommand(querySetActive, dbConnection))
                    {
                        SqlParameter dbParam_pRgYear = new SqlParameter();
                        dbParam_pRgYear.ParameterName = "@ProgramYear";
                        dbParam_pRgYear.Value = rowKey;
                        dbParam_pRgYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdUpdatePrgYear.Parameters.Add(dbParam_pRgYear);

                        SqlParameter dbParam_bdgtYear = new SqlParameter();
                        dbParam_bdgtYear.ParameterName = "@BudgetYear";
                        dbParam_bdgtYear.Value = ddlBudYear.SelectedValue;
                        dbParam_bdgtYear.DbType = System.Data.DbType.StringFixedLength;
                        cmdUpdatePrgYear.Parameters.Add(dbParam_bdgtYear);

                        cmdUpdatePrgYear.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { }
                
            }

            ((GridView)gvr.Parent.Parent).DataBind();
        }
    }

}

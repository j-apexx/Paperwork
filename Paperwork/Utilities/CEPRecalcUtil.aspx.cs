using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Windows.Forms.VisualStyles;

namespace Paperwork
{


    public partial class CEPRecalcUtil : Paperwork_Global
    {
        protected static string ExportPath = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            switch (Session["AccessLevel"].ToString().ToUpper())
            {
                case "ADMIN":
                case "OFFICE":
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
                if (DDLProgramYears.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                else
                {
                    if (DDLProgramYears.Items.FindByValue(Session["ProgramYear"].ToString()) != null)
                        DDLProgramYears.SelectedValue = Session["ProgramYear"].ToString();
                    else
                        DDLProgramYears.SelectedIndex = 0;
                }
                DDLDistricts.DataBind();
                
            }


            if (Session["AccessLevel"] == null || (Session["AccessLevel"].ToString() != "Admin" && Session["AccessLevel"].ToString() != "Office"))
                Response.Redirect("~/Default.aspx");
        }

        /// <summary>
        /// Using the same page parameters - mark the purchase records as imported
        /// </summary>
        private bool RunUtilitySP()
        {
            bool success = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;


            string queryString = "RecalculateCEPCountsForDistrict";

            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand dbCommand = new SqlCommand(queryString, dbConnection))
                {
                    dbCommand.CommandType= System.Data.CommandType.StoredProcedure;

                    SqlParameter dbParam_progYear = new SqlParameter();
                    dbParam_progYear.ParameterName = "@ProgramYear";
                    dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                    dbParam_progYear.DbType = System.Data.DbType.String;
                    dbCommand.Parameters.Add(dbParam_progYear);

                    SqlParameter dbParam_distID = new SqlParameter();
                    dbParam_distID.ParameterName = "@DistrictID";
                    dbParam_distID.Value = DDLDistricts.SelectedValue;  
                    dbParam_distID.DbType = System.Data.DbType.String;
                    dbCommand.Parameters.Add(dbParam_distID);

                    SqlParameter dbParam_startingPostDate = new SqlParameter();
                    dbParam_startingPostDate.ParameterName = "@StartDate";
                    dbParam_startingPostDate.Value = TextBoxBeginningDate.Text;
                    dbParam_startingPostDate.DbType = System.Data.DbType.DateTime;
                    dbCommand.Parameters.Add(dbParam_startingPostDate);

                    SqlParameter dbParam_endingPostDate = new SqlParameter();
                    dbParam_endingPostDate.ParameterName = "@EndDate";
                    dbParam_endingPostDate.Value = TextBoxEndingDate.Text;
                    dbParam_endingPostDate.DbType = System.Data.DbType.DateTime;
                    dbCommand.Parameters.Add(dbParam_endingPostDate);

                    SqlParameter dbParam_queryOnly = new SqlParameter();
                    dbParam_queryOnly.ParameterName = "@QueryOnly";
                    dbParam_queryOnly.Value = 0;
                    dbParam_queryOnly.DbType = System.Data.DbType.Byte;
                    dbCommand.Parameters.Add(dbParam_queryOnly);

                    try
                    {
                        dbConnection.Open();
                        dbCommand.ExecuteNonQuery();
                        success = true;
                    }
                    catch (SqlException ex)
                    {
                        TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                    catch (Exception ex)
                    {
                        TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                        TextBoxStatus.Text += Environment.NewLine + Environment.NewLine + ex.Message;
                        TextBoxStatus.Rows++;
                        TextBoxStatus.Rows++;
                    }
                }
            }

            return success;
        }


        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
        }

        protected void BtnProcess_Click(object sender, EventArgs e)
        {
            if(Page.IsValid == false)
            {
                TextBoxStatus.Text = "Please correct the errors on the page.";
                TextBoxStatus.ForeColor = System.Drawing.Color.Red;
                TextBoxStatus.Rows = 2;
                return;
            }

            TextBoxStatus.Text = "Starting process...";
            TextBoxStatus.Rows = 2;
            if(RunUtilitySP())
            {
                TextBoxStatus.ForeColor = System.Drawing.Color.Green;
                TextBoxStatus.Text += Environment.NewLine + "CEP Counts Recalculated for " + DDLDistricts.SelectedItem.Text;
                TextBoxStatus.Rows++;
            }
            else
            {
                TextBoxStatus.Text += Environment.NewLine + "Process completed with errors.";
                TextBoxStatus.Rows++;
            }
        }

        protected void CBActiveOnly_CheckedChanged(object sender, EventArgs e)
        {
            ListItem storedLI = DDLDistricts.SelectedItem;
            DDLDistricts.DataBind();
            ListItem newLI = DDLDistricts.Items.FindByValue(storedLI.Value);
            if (newLI != null)
                newLI.Selected = true;
            else
                DDLDistricts.SelectedIndex = 0;

        }

        protected void SqlDataSourceDistricts_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            DDLDistricts.Items.Clear();
        }


    }
}
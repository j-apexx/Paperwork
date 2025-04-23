using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupSpFnInvoices : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");
            else
                LabelProgramYear.Text = Session["ProgramYear"].ToString();

            if (!IsPostBack)
            {
                VerifyAccess(true);
            }
            else
            {
                VerifyAccess(false);
            }

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistricts;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;

                DropDownListDistricts.DataBind();
                if (DropDownListDistricts.Items.Count == 0)
                    Response.Redirect("~/default.aspx");
                if (Session["CurrentDistrict"] != null)
                {
                    if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                    else
                        DropDownListDistricts_SelectedIndexChanged(null, null);
                }
            }

        }

        protected void SqlDataSourceInvoiceSetup_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            ShowMessageIcon("Invoicing Settings Saved", MessageType.Success);
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                Response.Redirect("~/Setup/SetupSpFnInvoices.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void SqlDataSourceInvoiceSetup_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {

        }

        protected void SqlDataSourceInvoiceSetup_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if(e.AffectedRows==0)
            {
                bool recordInitialized = false;
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
                SqlConnection dbConnection = new SqlConnection(connectionString);

                // Need to check to see if the record exists, then run the Insert or Update statement
                // If UPDATE - need to check to see if they are allowed to update
                string queryInsertSFPNSetup = "INSERT INTO [SPFNInvoiceSetup] VALUES( @ProgramYear, @DistrictID, " +
                    "ISNULL((SELECT TOP 1 InvoicePrefix FROM [SPFNInvoiceSetup] WHERE DistrictID=@DistrictID),''), 1 )";

                using (SqlCommand cmd = new SqlCommand(queryInsertSFPNSetup, dbConnection))
                {
                    SqlParameter dbParam_ProgYear = new SqlParameter();
                    dbParam_ProgYear.ParameterName = "@ProgramYear";
                    dbParam_ProgYear.Value = e.Command.Parameters["@ProgramYear"].Value.ToString();
                    dbParam_ProgYear.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_ProgYear);

                    SqlParameter dbParam_DistID = new SqlParameter();
                    dbParam_DistID.ParameterName = "@DistrictID";
                    dbParam_DistID.Value = e.Command.Parameters["@DistrictID"].Value.ToString(); ;
                    dbParam_DistID.DbType = System.Data.DbType.StringFixedLength;
                    cmd.Parameters.Add(dbParam_DistID);

                    try
                    {
                        dbConnection.Open();
                        int recordCount = cmd.ExecuteNonQuery();

                        if(recordCount>0)
                        {
                            //try to select again
                            //DetailsViewInvoiceSetup.DataBind();
                            //SqlDataSourceInvoiceSetup.Select(DataSourceSelectArguments.Empty);
                            //nether of the above would display records, changing to page refresh for now...
                            recordInitialized = true;
                        }
                    }
                    catch (SqlException ex) {
                        RegisterDOMReadyScript("error message", "alert('SQL Error generating the District Special Functions setup record.');");
                    }
                    catch (Exception ex)
                    {
                        RegisterDOMReadyScript("error message", "alert('Error generating the District Special Functions setup record.');");
                    }
                    finally
                    {
                        dbConnection.Close();
                    }
                }

                if(recordInitialized)
                    Response.Redirect("~/Setup/SetupSpFnInvoices.aspx");
            }
        }

        protected void DetailsViewInvoiceSetup_DataBound(object sender, EventArgs e)
        {
            int recordCount = 0;
        }
    }
}
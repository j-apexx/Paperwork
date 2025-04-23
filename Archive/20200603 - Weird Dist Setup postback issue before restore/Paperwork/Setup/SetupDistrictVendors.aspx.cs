using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupDistrictVendors : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                VerifyAccess(true);
            }
            else
            {
                VerifyAccess(false);
            }

            if (!IsPostBack)
            {
                if (Session["VendorRequestMsg"] != null && Session["VendorRequestMsg"].ToString() != "")
                {
                    LabelVendorRequestStatus.Text = Session["VendorRequestMsg"].ToString();
                    Session.Remove("VendorRequestMsg");
                }
                else
                    LabelVendorRequestStatus.Text = "";
            }
            else
            {
                LabelVendorRequestStatus.Text = "";
            }
        }

        protected void GridViewAvailVendors_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView gv = ((GridView)sender);


            string VendorToAdd = ((String)gv.SelectedValue).ToString();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string addDistrictVendor = "INSERT INTO [DistrictVendors] ([DistrictID], [VendorID]) VALUES ( @DistrictID, @VendorID)";

            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                SqlCommand cmdAddDistVndr = new SqlCommand(addDistrictVendor, dbConnection);

                SqlParameter dbParam_vendID = new SqlParameter();
                dbParam_vendID.ParameterName = "@VendorID";
                dbParam_vendID.Value = VendorToAdd;
                dbParam_vendID.DbType = System.Data.DbType.StringFixedLength;
                cmdAddDistVndr.Parameters.Add(dbParam_vendID);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = Session["CurrentDistrict"].ToString();
                dbParam_distID.DbType = System.Data.DbType.StringFixedLength;
                cmdAddDistVndr.Parameters.Add(dbParam_distID);

                int rowsAffected = 0;

                try
                {
                    rowsAffected = cmdAddDistVndr.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        gv.SelectedRow.Visible = false;
                    }
                }
                catch (Exception ex)
                {

                }
                cmdAddDistVndr.Dispose();


                //refresh the grid views
                GridViewSelectedVendors.DataBind();
                //GridViewAvailVendors.DataBind();
            }
        }

        protected void SqlDataSourceSelectedVendors_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //need to refresh the available vendors as it is not refreshed automatically
            GridViewAvailVendors.DataBind();
        }

        protected void DropDownListNumRecs1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewAvailVendors.DataBind();
        }

        protected void IClear_Click(object sender, EventArgs e)
        {
            TextBoxVndSearch.Text = string.Empty;
        }
    }
}
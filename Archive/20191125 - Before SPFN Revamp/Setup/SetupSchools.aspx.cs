using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupSchools : Paperwork_Global
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

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if(!IsPostBack)
            {
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

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                Response.Redirect("~/Setup/SetupSchools.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void CB_Inactive_CheckedChanged(object sender, EventArgs e)
        {
            string checkFieldStr = ((CheckBox)sender).Checked ? "1" : "0";
            GridViewRow gvr = ((GridViewRow)((CheckBox)sender).Parent/*Cell*/.Parent);
            GridView GVSchools = ((GridView)gvr.Parent.Parent);
            string schoolNum = GVSchools.DataKeys[gvr.RowIndex]["SchoolNum"].ToString();
            string distID = GVSchools.DataKeys[gvr.RowIndex]["DistrictID"].ToString(); 

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string queryUpdateSchool = "UPDATE [DistrictSchools] SET Inactive=@Inactive WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum ";

            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                SqlCommand cmdUpdateInv = new SqlCommand(queryUpdateSchool, dbConnection);

                SqlParameter dbParam_inactive = new SqlParameter();
                dbParam_inactive.ParameterName = "@Inactive";
                dbParam_inactive.Value = checkFieldStr;
                dbParam_inactive.DbType = System.Data.DbType.Byte;
                cmdUpdateInv.Parameters.Add(dbParam_inactive);

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = distID;
                dbParam_distID.DbType = System.Data.DbType.StringFixedLength;
                cmdUpdateInv.Parameters.Add(dbParam_distID);

                SqlParameter dbParam_schoolNumber = new SqlParameter();
                dbParam_schoolNumber.ParameterName = "@SchoolNum";
                dbParam_schoolNumber.Value = schoolNum;
                dbParam_schoolNumber.DbType = System.Data.DbType.Int16;
                cmdUpdateInv.Parameters.Add(dbParam_schoolNumber);

                cmdUpdateInv.ExecuteNonQuery();
                cmdUpdateInv.Dispose();
            }

            //update the Gridview and redraw it and the Details View
            GVSchools.DataBind();


        }
    }
}
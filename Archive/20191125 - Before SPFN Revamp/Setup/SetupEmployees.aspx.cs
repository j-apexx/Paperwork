using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupEmployees : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            VerifyAccess(true);

            if (Session["AccessLevel"] == null)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {

                
            }
        }


        protected void GridViewEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow gvr = e.Row;
            TextBox EditLifeAddDt = (TextBox)FindControlRecursive(GridViewEmployees, "EditLifeAddDt");
            TextBox EditLifeDeleteDt = (TextBox)FindControlRecursive(GridViewEmployees, "EditLifeDeleteDt");

            // I don't want to display 1/1/1900 and thereby will force datepicker to use the current date
            if (EditLifeAddDt != null)
            {
                if (EditLifeAddDt.Text == "1/1/1900 12:00:00 AM")
                {
                    EditLifeAddDt.Text = "";
                }
            }

            // I don't want to display 1/1/1900 and thereby will force datepicker to use the current date
            if (EditLifeDeleteDt != null)
            {
                if (EditLifeDeleteDt.Text == "1/1/1900 12:00:00 AM")
                {
                    EditLifeDeleteDt.Text = "";
                }
            }

        }

        protected void SqlDataSourceEmployees_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            int count=e.Command.Parameters.Count;
        }
    }
}
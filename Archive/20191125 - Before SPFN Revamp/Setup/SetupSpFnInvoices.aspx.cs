using System;
using System.Collections.Generic;
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
            VerifyAccess(true);
            if (Session["AccessLevel"] == null || Session["AccessLevel"].ToString() != "Admin")
                Response.Redirect("~/Default.aspx");
        }

        protected void SqlDataSourceInvoiceSetup_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            ShowMessageIcon("Invoicing Settings Saved", MessageType.Success);
        }
    }
}
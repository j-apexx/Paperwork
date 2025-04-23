using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupTaxRates : Paperwork_Global
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

        protected void GridViewTaxRates_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            TextBox TextBoxTaxYear = (TextBox)FindControlRecursive(GridViewTaxRates, "TextBoxTaxYear");
            TextBox TextBoxPayProc = (TextBox)FindControlRecursive(GridViewTaxRates, "TextBoxPayProc");
            TextBox TextBoxWComp = (TextBox)FindControlRecursive(GridViewTaxRates, "TextBoxWComp");
            TextBox TextBoxOtherPayRelated = (TextBox)FindControlRecursive(GridViewTaxRates, "TextBoxOtherPayRelated");

            if(TextBoxTaxYear==null || TextBoxPayProc==null || TextBoxWComp==null || TextBoxOtherPayRelated==null)
            { e.Cancel = true;return; }

            if(int.TryParse(TextBoxTaxYear.Text, out int outTextBoxTaxYear)==false)
            { e.Cancel = true;return; }

            if (decimal.TryParse(TextBoxPayProc.Text, out decimal outTextBoxPayProc) == false)
            { e.Cancel = true; return; }

            if (decimal.TryParse(TextBoxWComp.Text, out decimal outTextBoxWComp) == false)
            { e.Cancel = true; return; }

            if (decimal.TryParse(TextBoxOtherPayRelated.Text, out decimal outTextBoxOtherPayRelated) == false)
            { e.Cancel = true; return; }
        }
    }
}
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using System.Net;
using System.IO;
using System.Net.Mail;
using System.Collections;
using System.Web;




namespace Paperwork
{
    [System.Web.Script.Services.ScriptService]
    public partial class SpecialFunctionsEntry : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            if(Session["CurrentDistrict"]==null)
                Response.Redirect("~/default.aspx");

            if (!IsPostBack)
            {
                VerifyAccess(true);
                if (Session["AccessLevel"] == null)
                    Response.Redirect("~/Default.aspx");

                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

                //Set the programyear
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

                //On initial load - select the first invoice
                GridViewInvoices.DataBind();

                if (GridViewInvoices.SelectedIndex == -1 && GridViewInvoices.Rows.Count > 0)
                {
                    //if (string.IsNullOrEmpty(Session["CurrentInvoice"].ToString()) == false)
                    //{
                    //    foreach(DataKey dk in GridViewInvoices.DataKeys)
                    //    {
                    //        if(dk["InvoiceNumber"].ToString()== Session["CurrentInvoice"].ToString())
                    //        {
                    //            GridViewInvoices.SelectRow(GridViewInvoices.DataKeys.);
                    //        }
                    //    }
                    //    GridViewInvoices.DataKeys[0].Values["InvoiceNumber"]
                    //}
                    //else
                    GridViewInvoices.SelectRow(0);
                }

                //Initialize the indicator to unsaved
                HFInvoiceSaved.Value = "0";

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                HFSSRSBaseFolder.DataBind();
                HFSSRSBaseFolder.Value = SSRSBaseFolder;

                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                ReportViewer2.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer2.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer2.ServerReport.ReportPath;

#if !DEBUG
                //For now only testing quick print on training site
                //Using BaseFolder to check for training company
                if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING") > 0)
                {
                    quickPrintSpan.Visible = true;
                }
                else
                {
                    quickPrintSpan.Visible = false;
                }
#endif
            }
            else
            {
                VerifyAccess(false);
            }
        }

        /// <summary>
        /// Web method to retrieve a list of Special Function Organizations matching the passed in string for the current district
        /// </summary>
        /// <param name="prefixText">String used to match organizations</param>
        /// <param name="distID">District ID</param>
        /// <returns>List of strings representing matching Special Functions Organizations</returns>
        [WebMethod]
        public static List<string> getSPFNOrgNames(string prefixText, string distID)
        {
            List<string> customers = new List<string>();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT SpFnOrgName As Name from SPFNOrganizations WHERE DistrictID=@distID AND " +
                    "SpFnOrgName LIKE '%'+ @SearchText + '%' ";
                    cmd.Parameters.AddWithValue("@SearchText", prefixText);
                    cmd.Parameters.AddWithValue("@distID", distID);
                    cmd.Connection = conn;
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            customers.Add(dr["Name"].ToString());
                        }
                    }
                    conn.Close();
                }

            }
            return customers;
        }

        protected void GridViewLines_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid && HFInvoiceLocked.Value == "False")
            {
                if (SqlDataSourceInvLines.Insert() > 0)
                {
                    //clear out the footer to prevent my other code from trying to insert again
                    GridView GridViewMiscLines = (GridView)FindControlRecursive(FormViewSpFnOrg, "GridViewMiscLines");
                    TextBox TextBoxFMiscDesc = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMiscDesc");
                    TextBox TextBoxFMiscAmount = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMiscAmount");
                    TextBox TextBoxFMarkupPct = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMarkupPct");
                    TextBox TextBoxFMarkupDlr = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMarkupDlr");
                    DropDownList DDLFMarkupType = (DropDownList)GridViewMiscLines.FooterRow.FindControl("DDLFMarkupType");

                    TextBoxFMiscDesc.Text = string.Empty;
                    TextBoxFMiscAmount.Text = string.Empty;
                    TextBoxFMarkupPct.Text = string.Empty;
                    TextBoxFMarkupDlr.Text = string.Empty;
                    DDLFMarkupType.SelectedValue = "0";
                }
            }
        }

        /// <summary>
        /// This is generic currently and can be used by both gridviews
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewLines_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;
        }

        /// <summary>
        /// Checks and sets parameters for the insert from the footer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SqlDataSourceInvLines_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null)
                return;

            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            GridView GridViewMiscLines = (GridView)FindControlRecursive(FormViewSpFnOrg, "GridViewMiscLines");
            TextBox TextBoxFMiscDesc = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMiscDesc");
            TextBox TextBoxFMiscAmount = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMiscAmount");
            TextBox TextBoxFMarkupPct = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMarkupPct");
            TextBox TextBoxFMarkupDlr = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMarkupDlr");
            DropDownList DDLFMarkupType = (DropDownList)GridViewMiscLines.FooterRow.FindControl("DDLFMarkupType");
            CheckBox CBFMiscAmountTaxable = (CheckBox)GridViewMiscLines.FooterRow.FindControl("CBFMiscAmountTaxable");

            if(GridViewMiscLines==null || TextBoxFMiscDesc==null || TextBoxFMiscAmount==null || TextBoxFMarkupPct==null ||
                TextBoxFMarkupDlr==null || DDLFMarkupType==null || CBFMiscAmountTaxable==null)
                return;

            if (string.IsNullOrEmpty(TextBoxFMiscAmount.Text))
            {
                TextBoxFMiscAmount.Text = "0.00";
            }

            if (string.IsNullOrEmpty(TextBoxFMarkupPct.Text))
            {
                TextBoxFMarkupPct.Text = "0.00";
            }

            if (string.IsNullOrEmpty(TextBoxFMarkupDlr.Text))
            {
                TextBoxFMarkupDlr.Text = "0.00";
            }

            if (DDLFMarkupType.SelectedValue == "0")
            {
                TextBoxFMarkupPct.Text = "0.00";
                TextBoxFMarkupDlr.Text = "0.00";
            }
            else if (DDLFMarkupType.SelectedValue == "1")
                TextBoxFMarkupDlr.Text = "0.00";
            else if (DDLFMarkupType.SelectedValue == "2")
                TextBoxFMarkupPct.Text = "0.00";

            if (Decimal.TryParse(TextBoxFMiscAmount.Text, out Decimal OutSvcAmt) == false)
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFMarkupPct.Text, out Decimal OutMarkupPct) == false)
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFMarkupDlr.Text, out Decimal OutMarkupDlr) == false)
            {
                e.Cancel = true; return;
            }

            if (string.IsNullOrEmpty(TextBoxFMiscDesc.Text))
            {
                e.Cancel = true; return;
            }


            if (FormViewSpFnOrg.DataKey.Values["InvoiceNumber"] == null || string.IsNullOrEmpty(FormViewSpFnOrg.DataKey.Values["InvoiceNumber"].ToString()))
            {
                e.Cancel = true; return;
            }

            if (TextBoxFMiscDesc.Text == "" && OutSvcAmt == 0M && DDLFMarkupType.SelectedValue == "0")
            {
                e.Cancel = true; return;
            }

            e.Command.Parameters["@MiscAmount"].Value = OutSvcAmt;
            e.Command.Parameters["@MarkupPct"].Value = OutMarkupPct;
            e.Command.Parameters["@MarkupDlr"].Value = OutMarkupDlr;
            e.Command.Parameters["@MarkupType"].Value = DDLFMarkupType.SelectedValue;
            e.Command.Parameters["@MiscDescription"].Value = TextBoxFMiscDesc.Text;
            e.Command.Parameters["@InvoiceNumber"].Value = FormViewSpFnOrg.DataKey.Values["InvoiceNumber"].ToString();
            e.Command.Parameters["@MiscAmountTaxable"].Value = CBFMiscAmountTaxable.Checked;
        }

        protected void GridViewInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv == null)
                return;

            foreach (GridViewRow row in gv.Rows)
            {
                if (row.RowIndex == gv.SelectedIndex)
                {
                    row.ToolTip = string.Empty;
                    row.BorderColor = Color.Green;
                    row.BorderWidth = 2;
                    row.BorderStyle = BorderStyle.Solid;

                    //Latest Firefox update does not display row borders
                    foreach (TableCell tc in row.Cells)
                    {
                        tc.CssClass += " blueHighighter";
                    }
                }
                else
                {
                    row.ToolTip = "Click to select this row.";
                    row.BorderColor = Color.Empty;
                    row.BorderWidth = 0;
                    row.ForeColor = Color.Empty;

                    //Latest Firefox update does not display row borders
                    foreach (TableCell tc in row.Cells)
                    {
                        tc.CssClass = tc.CssClass.Replace(" blueHighighter", "");
                    }
                }
            }

            //if(gv.SelectedIndex != -1 && (gv.SelectedIndex + (gv.PageSize * gv.PageIndex)) + 1 <= gv.DataKeys.Count)
            if (gv.SelectedIndex != -1 && (gv.SelectedIndex + 1) <= gv.DataKeys.Count)
            {
                //Set the hidden value to the selected invoicenumber then update the displayed invoices
                //HFInvoiceNumber.Value = gv.DataKeys[(gv.SelectedIndex + (gv.PageSize * gv.PageIndex))].Values["InvoiceNumber"].ToString();
                HFInvoiceNumber.Value = gv.DataKeys[(gv.SelectedIndex)].Values["InvoiceNumber"].ToString();

                HiddenField HFInvoiceRowLocked = (HiddenField)FindControlRecursive(gv.SelectedRow, "HFInvoiceRowLocked");
                if (HFInvoiceRowLocked != null)
                    HFInvoiceLocked.Value = HFInvoiceRowLocked.Value;
                else
                    HFInvoiceLocked.Value = "False";

                //Keeping save button enabled to save payment info
                //if(HFInvoiceLocked.Value == "False")
                //{
                //    ISave.Enabled = true;
                //    ISave.ForeColor = System.Drawing.Color.Green;
                //}
                //else
                //{
                //    ISave.Enabled = false;
                //    ISave.ForeColor = System.Drawing.Color.Gray;
                //}
            }
            else
            {
                HFInvoiceNumber.Value = string.Empty;
            }


        }

        protected void ICreateClick(object sender, EventArgs e)
        {
            string InvoiceNumber = GetNextSpFnInvoiceNumber();

            if (string.IsNullOrEmpty(InvoiceNumber))
            {
                //RegisterDOMReadyScript("alert message", "alert('The next invoice number could not be retrieved. Process cancelled.');");
                ShowDialog("Alert", MessageType.Warning, "The next invoice number could not be retrieved. Process cancelled.", "");
                return;
            }

            if (GenerateInvoiceHdr(InvoiceNumber) == false)
            {
                //RegisterDOMReadyScript("alert message", "alert('The next invoice header could not be created. Process cancelled.');");
                ShowDialog("Alert", MessageType.Warning, "The next invoice header could not be created. Process cancelled.", "");
                return;
            }
            else
            {
                ShowMessageIcon("Invoice " + InvoiceNumber + " Created", MessageType.Success);
                GridViewInvoices.DataBind();
                if (GridViewInvoices.Rows.Count > 0)
                {
                    GridViewInvoices.SelectRow(0);
                    GridViewInvoices_SelectedIndexChanged(GridViewInvoices, EventArgs.Empty);
                }
            }

        }

        /// <summary>
        /// Generates the Master Invoice Header record
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        private bool GenerateInvoiceHdr(string invoiceNumber)
        {
            bool invoiceGenerated = false;

            string sqlGenerateInvoiceHdr = "INSERT INTO [SPFNInvoiceHDR] ([ProgramYear],[DistrictID],[InvoiceNumber],[CreatedDate],[CreatedUser],[Locked],[BillingDate],[FunctionDate],[SalesTaxPct]) "
                + " VALUES(@ProgramYear, @DistrictID, @InvoiceNumber, @CreatedDate, @CreatedUser, 0, '1/1/1900' , GETDATE()," +
                " (SELECT IIF(DistrictState='SC', SC_SalesTax, 0.00000) SalesTax FROM TaxRates,District WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID) );";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmdHdr = new SqlCommand(sqlGenerateInvoiceHdr, dbConnection))
                {
                    SqlParameter dbParam_progYear = new SqlParameter();
                    dbParam_progYear.ParameterName = "@ProgramYear";
                    dbParam_progYear.Value = DDLProgramYears.SelectedValue;
                    dbParam_progYear.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_progYear);

                    SqlParameter dbParam_distID = new SqlParameter();
                    dbParam_distID.ParameterName = "@DistrictID";
                    dbParam_distID.Value = Session["CurrentDistrict"].ToString();
                    dbParam_distID.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_distID);

                    SqlParameter dbParam_InvNum = new SqlParameter();
                    dbParam_InvNum.ParameterName = "@InvoiceNumber";
                    dbParam_InvNum.Value = invoiceNumber;
                    dbParam_InvNum.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_InvNum);

                    SqlParameter dbParam_CreatedDate = new SqlParameter();
                    dbParam_CreatedDate.ParameterName = "@CreatedDate";
                    dbParam_CreatedDate.Value = DateTime.Now.ToShortDateString();
                    dbParam_CreatedDate.DbType = System.Data.DbType.Date;
                    cmdHdr.Parameters.Add(dbParam_CreatedDate);

                    SqlParameter dbParam_CreatedBy = new SqlParameter();
                    dbParam_CreatedBy.ParameterName = "@CreatedUser";
                    dbParam_CreatedBy.Value = Context.User.Identity.Name;
                    dbParam_CreatedBy.DbType = System.Data.DbType.String;
                    cmdHdr.Parameters.Add(dbParam_CreatedBy);

                    try
                    {
                        int recordCount = 0;
                        recordCount = cmdHdr.ExecuteNonQuery();
                        if (recordCount > 0)
                            invoiceGenerated = true;
                    }
                    catch (SqlException ex)
                    { }
                    catch (Exception ex)
                    { }
                }
            }

            return invoiceGenerated;

        }

        /// <summary>
        /// This procedure runs the SQL Stored procedure and returns the next unused Invoice Number from settings and verifying against the SpFnInvoices table.
        /// </summary>
        /// <returns>Invoice Number</returns>
        protected string GetNextSpFnInvoiceNumber()
        {
            string InvoiceNumber = "";
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            string GetNextSPFNInvoiceNumber = "GetNextSPFNInvoiceNumber";
            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                using (SqlCommand cmdSP = new SqlCommand(GetNextSPFNInvoiceNumber, dbConnection))
                {
                    SqlParameter dbParam_ProgYear2 = new SqlParameter();
                    dbParam_ProgYear2.ParameterName = "@ProgramYear";
                    dbParam_ProgYear2.Value = DDLProgramYears.SelectedValue;
                    dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                    cmdSP.Parameters.Add(dbParam_ProgYear2);

                    SqlParameter dbParam_DistID2 = new SqlParameter();
                    dbParam_DistID2.ParameterName = "@DistrictID";
                    dbParam_DistID2.Value = Session["CurrentDistrict"].ToString();
                    dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                    cmdSP.Parameters.Add(dbParam_DistID2);

                    SqlParameter dbParam_outInvNum = new SqlParameter();
                    dbParam_outInvNum.ParameterName = "@IO_OutInvoiceNum";
                    dbParam_outInvNum.Value = null;
                    dbParam_outInvNum.Direction = System.Data.ParameterDirection.Output;
                    dbParam_outInvNum.DbType = System.Data.DbType.String;
                    dbParam_outInvNum.Size = 20;
                    cmdSP.Parameters.Add(dbParam_outInvNum);

                    cmdSP.CommandType = System.Data.CommandType.StoredProcedure;

                    try
                    {
                        cmdSP.ExecuteNonQuery();
                        InvoiceNumber = dbParam_outInvNum.Value.ToString();
                    }
                    catch (SqlException ex)
                    { }
                    catch (Exception ex)
                    { }
                }

            }
            return InvoiceNumber;
        }

        protected void SaveInvoiceData()
        {
            bool invoiceNumberChanged = false;

            if (HFInvoiceSaved.Value == "1")
                return;

            #region ReferenceControls
            TextBox TextBoxInvoice = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxInvoice");
            TextBox TextBoxOrgBilled = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxOrgBilled");
            TextBox TextBoxContactPerson = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxContactPerson");
            TextBox TextBoxSpecFuncDesc = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSpecFuncDesc");
            TextBox TextBoxFunctionDate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxFunctionDate");
            TextBox TextBoxBillingDate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBillingDate");
            TextBox TextBoxPONumber = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxPONumber");
            TextBox TextBoxDatePaid = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDatePaid");
            TextBox TextBoxFoodCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxFoodCost");
            TextBox TextBoxPaperCleanSup = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxPaperCleanSup");
            TextBox TextBoxMileage = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxMileage");
            TextBox TextBoxFSDCosts = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxFSDCosts");
            TextBox TextBoxTNGLabor = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxTNGLabor");
            TextBox TextBoxTNGFringePct = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxTNGFringePct");
            TextBox TextBoxDistLabor = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDistLabor");
            TextBox TextBoxDistFringePct = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDistFringePct");
            TextBox TextBoxInvoiceNotes = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxInvoiceNotes");
            TextBox TextBoxMarkupPct = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxMarkupPct");
            TextBox TextBoxMarkupDlr = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxMarkupDlr");
            TextBox TextBoxDueTo = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDueTo");
            DropDownList DDLMarkupType = (DropDownList)FindControlRecursive(FormViewSpFnOrg, "DDLMarkupType");
            CheckBox CBPrintNotes = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBPrintNotes");
            CheckBox CB3086 = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CB3086");
            CheckBox CBCACFP = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBCACFP");
            TextBox TextBoxPaymentNumber = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxPaymentNumber");
            CheckBox CBFoodCostTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBFoodCostTaxable");
            CheckBox CBPaperCleanSupplTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBPaperCleanSupplTaxable");
            TextBox TextBoxSalesTaxPct = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSalesTaxPct");

            if (TextBoxInvoice == null || TextBoxOrgBilled == null || TextBoxContactPerson == null || TextBoxSpecFuncDesc == null || TextBoxFunctionDate == null ||
                TextBoxBillingDate == null || TextBoxPONumber == null || TextBoxDatePaid == null || TextBoxFoodCost == null || TextBoxMileage == null || TextBoxFSDCosts == null ||
                TextBoxTNGLabor == null || TextBoxTNGFringePct == null || TextBoxInvoiceNotes == null || TextBoxMarkupPct == null || TextBoxMarkupDlr == null || TextBoxDueTo == null ||
                DDLMarkupType == null || CBPrintNotes == null || CB3086 == null || CBCACFP == null || TextBoxPaymentNumber == null || TextBoxPaperCleanSup == null || TextBoxDistLabor == null ||
                TextBoxDistFringePct == null || CBFoodCostTaxable==null || CBPaperCleanSupplTaxable==null || TextBoxSalesTaxPct==null)
                return;
            #endregion


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string UpdateInvoiceQuery;
            UpdateInvoiceQuery = "UPDATE [SPFNInvoiceHDR] SET [InvoiceNumber]=@NewInvoiceNumber, [OrgBilled]=@OrgBilled, OrgContact=@OrgContact, FunctionDate=@FunctionDate," +
                "BillingDate=@BillingDate, OrgRemitTo=@OrgRemitTo, SpecialFunctionDesc=@SpecialFunctionDesc, FoodCost=@FoodCost," +
                "PaperCleanSuppl=@PaperCleanSuppl, Mileage=@Mileage, FSDCosts=@FSDCosts, TNGLabor=@TNGLabor, TNGFringePct=@TNGFringePct," +
                "DistrictLabor=@DistrictLabor, DistrictFringePct=@DistrictFringePct, MarkupType=@MarkupType, MarkupDlr=@MarkupDlr, " +
                "MarkupPct=@MarkupPct, InvoiceNotes=@InvoiceNotes, PrintNotes=@PrintNotes, PONumber=@PONumber, DatePaid=@DatePaid," +
                "Org3086=@Org3086, OrgCACFP=@OrgCACFP, PaymentNumber=@PaymentNumber, FoodCostTaxable=@FoodCostTaxable, PaperCleanSupplTaxable=@PaperCleanSupplTaxable, SalesTaxPct=@SalesTaxPct " +
                "WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@old_InvoiceNumber ";

            using (SqlCommand cmd2 = new SqlCommand(UpdateInvoiceQuery, dbConnection))
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = DDLProgramYears.SelectedValue;
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = Session["CurrentDistrict"].ToString();
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_InvoiceNum = new SqlParameter();
                dbParam_InvoiceNum.ParameterName = "@old_InvoiceNumber";
                dbParam_InvoiceNum.Value = HFInvoiceNumber.Value;
                dbParam_InvoiceNum.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_InvoiceNum);

                SqlParameter dbParam_NewInvoiceNum2 = new SqlParameter();
                dbParam_NewInvoiceNum2.ParameterName = "@NewInvoiceNumber";
                dbParam_NewInvoiceNum2.Value = TextBoxInvoice.Text;
                dbParam_NewInvoiceNum2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_NewInvoiceNum2);
                #endregion

                #region MainValues
                SqlParameter dbParam_MarkupType = new SqlParameter();
                dbParam_MarkupType.ParameterName = "@MarkupType";
                dbParam_MarkupType.Value = DDLMarkupType.SelectedValue;
                dbParam_MarkupType.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_MarkupType);

                SqlParameter dbParam_OrgBilled = new SqlParameter();
                dbParam_OrgBilled.ParameterName = "@OrgBilled";
                dbParam_OrgBilled.Value = TextBoxOrgBilled.Text;
                dbParam_OrgBilled.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_OrgBilled);

                SqlParameter dbParam_Org3086 = new SqlParameter();
                dbParam_Org3086.ParameterName = "@Org3086";
                dbParam_Org3086.Value = CB3086.Checked;
                dbParam_Org3086.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_Org3086);

                SqlParameter dbParam_OrgCACFP = new SqlParameter();
                dbParam_OrgCACFP.ParameterName = "@OrgCACFP";
                dbParam_OrgCACFP.Value = CBCACFP.Checked;
                dbParam_OrgCACFP.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_OrgCACFP);

                SqlParameter dbParam_OrgContact = new SqlParameter();
                dbParam_OrgContact.ParameterName = "@OrgContact";
                dbParam_OrgContact.Value = TextBoxContactPerson.Text;
                dbParam_OrgContact.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_OrgContact);

                SqlParameter dbParam_FuncDate = new SqlParameter();
                dbParam_FuncDate.ParameterName = "@FunctionDate";
                dbParam_FuncDate.Value = TextBoxFunctionDate.Text;
                dbParam_FuncDate.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_FuncDate);

                if (DateTime.TryParse(TextBoxBillingDate.Text, out DateTime PostDate) == false)
                    PostDate = new DateTime(1900, 01, 01);

                SqlParameter dbParam_BillDate = new SqlParameter();
                dbParam_BillDate.ParameterName = "@BillingDate";
                dbParam_BillDate.Value = PostDate.ToShortDateString();
                dbParam_BillDate.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_BillDate);

                SqlParameter dbParam_OrgRemitTo = new SqlParameter();
                dbParam_OrgRemitTo.ParameterName = "@OrgRemitTo";
                dbParam_OrgRemitTo.Value = TextBoxDueTo.Text;
                dbParam_OrgRemitTo.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_OrgRemitTo);

                SqlParameter dbParam_SpecFuncDesc = new SqlParameter();
                dbParam_SpecFuncDesc.ParameterName = "@SpecialFunctionDesc";
                dbParam_SpecFuncDesc.Value = TextBoxSpecFuncDesc.Text;
                dbParam_SpecFuncDesc.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_SpecFuncDesc);

                SqlParameter dbParam_FoodCost = new SqlParameter();
                dbParam_FoodCost.ParameterName = "@FoodCost";
                dbParam_FoodCost.Value = TextBoxFoodCost.Text;
                dbParam_FoodCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_FoodCost);

                SqlParameter dbParam_PaperCleanSuppl = new SqlParameter();
                dbParam_PaperCleanSuppl.ParameterName = "@PaperCleanSuppl";
                dbParam_PaperCleanSuppl.Value = TextBoxPaperCleanSup.Text;
                dbParam_PaperCleanSuppl.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_PaperCleanSuppl);

                SqlParameter dbParam_Mileage = new SqlParameter();
                dbParam_Mileage.ParameterName = "@Mileage";
                dbParam_Mileage.Value = TextBoxMileage.Text;
                dbParam_Mileage.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_Mileage);

                SqlParameter dbParam_FSDCosts = new SqlParameter();
                dbParam_FSDCosts.ParameterName = "@FSDCosts";
                dbParam_FSDCosts.Value = TextBoxFSDCosts.Text;
                dbParam_FSDCosts.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_FSDCosts);

                SqlParameter dbParam_TNGLabor = new SqlParameter();
                dbParam_TNGLabor.ParameterName = "@TNGLabor";
                dbParam_TNGLabor.Value = TextBoxTNGLabor.Text;
                dbParam_TNGLabor.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_TNGLabor);

                SqlParameter dbParam_TNGFringePct = new SqlParameter();
                dbParam_TNGFringePct.ParameterName = "@TNGFringePct";
                dbParam_TNGFringePct.Value = TextBoxTNGFringePct.Text;
                dbParam_TNGFringePct.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_TNGFringePct);

                SqlParameter dbParam_DistLabor = new SqlParameter();
                dbParam_DistLabor.ParameterName = "@DistrictLabor";
                dbParam_DistLabor.Value = TextBoxDistLabor.Text;
                dbParam_DistLabor.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DistLabor);

                SqlParameter dbParam_DistFringePct = new SqlParameter();
                dbParam_DistFringePct.ParameterName = "@DistrictFringePct";
                dbParam_DistFringePct.Value = TextBoxDistFringePct.Text;
                dbParam_DistFringePct.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DistFringePct);

                SqlParameter dbParam_MarkupDlr = new SqlParameter();
                dbParam_MarkupDlr.ParameterName = "@MarkupDlr";
                dbParam_MarkupDlr.Value = TextBoxMarkupDlr.Text;
                dbParam_MarkupDlr.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_MarkupDlr);

                SqlParameter dbParam_MarkupPct = new SqlParameter();
                dbParam_MarkupPct.ParameterName = "@MarkupPct";
                dbParam_MarkupPct.Value = TextBoxMarkupPct.Text;
                dbParam_MarkupPct.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_MarkupPct);

                SqlParameter dbParam_InvoiceNote = new SqlParameter();
                dbParam_InvoiceNote.ParameterName = "@InvoiceNotes";
                dbParam_InvoiceNote.Value = TextBoxInvoiceNotes.Text;
                dbParam_InvoiceNote.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_InvoiceNote);

                SqlParameter dbParam_PrintNotes = new SqlParameter();
                dbParam_PrintNotes.ParameterName = "@PrintNotes";
                dbParam_PrintNotes.Value = CBPrintNotes.Checked;
                dbParam_PrintNotes.DbType = System.Data.DbType.Boolean;
                cmd2.Parameters.Add(dbParam_PrintNotes);

                SqlParameter dbParam_PONumber = new SqlParameter();
                dbParam_PONumber.ParameterName = "@PONumber";
                dbParam_PONumber.Value = TextBoxPONumber.Text;
                dbParam_PONumber.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_PONumber);

                if (DateTime.TryParse(TextBoxDatePaid.Text, out DateTime DatePaid) == false)
                    DatePaid = new DateTime(1900, 01, 01);

                SqlParameter dbParam_PaidDate = new SqlParameter();
                dbParam_PaidDate.ParameterName = "@DatePaid";
                dbParam_PaidDate.Value = DatePaid.ToShortDateString();
                dbParam_PaidDate.DbType = System.Data.DbType.Date;
                cmd2.Parameters.Add(dbParam_PaidDate);

                SqlParameter dbParam_PaymentNumber = new SqlParameter();
                dbParam_PaymentNumber.ParameterName = "@PaymentNumber";
                dbParam_PaymentNumber.Value = TextBoxPaymentNumber.Text;
                dbParam_PaymentNumber.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_PaymentNumber);

                SqlParameter dbParam_FoodCostTaxable = new SqlParameter();
                dbParam_FoodCostTaxable.ParameterName = "@FoodCostTaxable";
                dbParam_FoodCostTaxable.Value = CBFoodCostTaxable.Checked;
                dbParam_FoodCostTaxable.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_FoodCostTaxable);

                SqlParameter dbParam_PaperCleanSupplTaxable = new SqlParameter();
                dbParam_PaperCleanSupplTaxable.ParameterName = "@PaperCleanSupplTaxable";
                dbParam_PaperCleanSupplTaxable.Value = CBPaperCleanSupplTaxable.Checked;
                dbParam_PaperCleanSupplTaxable.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_PaperCleanSupplTaxable);

                decimal.TryParse(TextBoxSalesTaxPct.Text, out decimal SalesTax);

                SqlParameter dbParam_SalesTaxPct = new SqlParameter();
                dbParam_SalesTaxPct.ParameterName = "@SalesTaxPct";
                dbParam_SalesTaxPct.Value = SalesTax/100;
                dbParam_SalesTaxPct.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_SalesTaxPct);

                #endregion


                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();
                    if (recordcount > 0)
                    {
                        // if the invoice number was updated, need to update any misc lines as well
                        if (TextBoxInvoice.Text != HFInvoiceNumber.Value)
                        {
                            UpdateMiscLinesInvoiceNum(DDLProgramYears.SelectedValue, Session["CurrentDistrict"].ToString(), HFInvoiceNumber.Value, TextBoxInvoice.Text);
                            HFInvoiceNumber.Value = TextBoxInvoice.Text;
                        }

                        //Check the misc lines footer for unsaved inserts
                        //if fields are empty, the insert will not happen
                        SqlDataSourceInvLines.Insert();
                    }

                }
                catch (Exception ex)
                {
                    //RegisterDOMReadyScript("error message", "alert('Error saving the Invoice.');");
                    ShowDialog("Error", MessageType.Error, "Error saving the Invoice.", ex.Message);
                    HFInvoiceSaved.Value = "ERROR";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            HFInvoiceSaved.Value = "1";

        }

        private void UpdateMiscLinesInvoiceNum(string ProgramYear, string DistrictID, string OldInvNum, string NewInvNum)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string UpdateInvoiceQuery;
            UpdateInvoiceQuery = "UPDATE [SPFNInvoiceLines] SET [InvoiceNumber]=@NewInvoiceNumber " +
                "WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@old_InvoiceNumber ";

            using (SqlCommand cmd2 = new SqlCommand(UpdateInvoiceQuery, dbConnection))
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = ProgramYear;
                dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = DistrictID;
                dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_InvoiceNum = new SqlParameter();
                dbParam_InvoiceNum.ParameterName = "@old_InvoiceNumber";
                dbParam_InvoiceNum.Value = OldInvNum;
                dbParam_InvoiceNum.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_InvoiceNum);

                SqlParameter dbParam_NewInvoiceNum2 = new SqlParameter();
                dbParam_NewInvoiceNum2.ParameterName = "@NewInvoiceNumber";
                dbParam_NewInvoiceNum2.Value = NewInvNum;
                dbParam_NewInvoiceNum2.DbType = System.Data.DbType.StringFixedLength;
                cmd2.Parameters.Add(dbParam_NewInvoiceNum2);
                #endregion

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error saving the Invoice Miscellaneous Lines.');");
                    HFInvoiceSaved.Value = "ERROR";
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void FormViewSpFnOrg_PreRender(object sender, EventArgs e)
        {
            if (HFInvoiceSaved.Value == "1")
            {
                ShowMessageByName("Invoice Saved. ", MessageType.Success, "alert_container_InvoiceHdr", 3000);
                HFInvoiceSaved.Value = "0";
            }
            else if (HFInvoiceSaved.Value == "ERROR")
            {
                ShowMessageByName("Error Saving. ", MessageType.Error, "alert_container_InvoiceHdr", 5000);
            }

            RegisterDOMReadyScript("CallCalcs", "RecalcInvoiceTotal();");
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            SaveInvoiceData();
            Session["CurrentInvoice"] = HFInvoiceNumber.Value;
            Session["CurrentProgramYear"] = DDLProgramYears.SelectedValue;
            Response.Redirect("~/Print/PrintSpecialFuncInvoices.aspx");
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openTab", "window.open('Print/PrintSpecialFuncInvoices.aspx' ,'_blank');", true);

            //This did not work - keeping to try again in the future (8-2020)
            //UpdateReportParams();
            //PrintReport();
            //UpdatePanel1.Update();

        }

        protected void ISave_Click(object sender, EventArgs e)
        {
            SaveInvoiceData();

            //Update the GridView so the columns show the updated values
            GridViewInvoices.DataBind();

            for (int i = 0; i < GridViewInvoices.DataKeys.Count; i++)
            {
                if (GridViewInvoices.DataKeys[i].Values["InvoiceNumber"].ToString() == HFInvoiceNumber.Value)
                {
                    GridViewInvoices.SelectRow(i);
                    break;
                }
            }

            //Redraw the outer panel to refresh the datagrid
            UpdatePanelOverall.Update();


        }

        protected void SqlDataSourceInvLines_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //Save any pending changes before rebinding
            SaveInvoiceData();
            //The Gridview delete does not trigger the FormView to DataBind. Calling this updates the gridview and the formview
            FormViewSpFnOrg.DataBind();
        }

        protected void SqlDataSourceInvLines_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            //Save any pending changes before rebinding
            SaveInvoiceData();
            //The Gridview update does not trigger the FormView to DataBind. Calling this updates the gridview and the formview
            FormViewSpFnOrg.DataBind();
        }

        protected void SqlDataSourceInvLines_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //clear out the footer to prevent my other code from trying to insert again
            GridView GridViewMiscLines = (GridView)FindControlRecursive(FormViewSpFnOrg, "GridViewMiscLines");
            TextBox TextBoxFMiscDesc = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMiscDesc");
            TextBox TextBoxFMiscAmount = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMiscAmount");
            TextBox TextBoxFMarkupPct = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMarkupPct");
            TextBox TextBoxFMarkupDlr = (TextBox)GridViewMiscLines.FooterRow.FindControl("TextBoxFMarkupDlr");
            DropDownList DDLFMarkupType = (DropDownList)GridViewMiscLines.FooterRow.FindControl("DDLFMarkupType");


            TextBoxFMiscDesc.Text = string.Empty;
            TextBoxFMiscAmount.Text = "0.00";
            TextBoxFMarkupPct.Text = "0.00;";
            TextBoxFMarkupDlr.Text = "0.00";
            DDLFMarkupType.SelectedValue = "0";

            //Save any pending changes before rebinding
            SaveInvoiceData();

            //The Gridview insert does not trigger the FormView to DataBind. Calling this updates the gridview and the formview
            FormViewSpFnOrg.DataBind();
        }

        protected void SqlDataSourceInvHdrsList_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            bool invoicefound = false;

            GridViewInvoices.DataBind();

            //Update the select invoice - select first if it was deleted
            for (int i = 0; i < GridViewInvoices.DataKeys.Count; i++)
            {
                if (GridViewInvoices.DataKeys[i].Values["InvoiceNumber"].ToString() == HFInvoiceNumber.Value)
                {
                    GridViewInvoices.SelectRow(i);
                    invoicefound = true;
                    break;
                }
            }

            if (invoicefound == false)
            {
                if (GridViewInvoices.Rows.Count > 0)
                {
                    GridViewInvoices.SelectRow(0);
                }
            }

            GridViewInvoices_SelectedIndexChanged(GridViewInvoices, EventArgs.Empty);

        }

        protected void GridViewInvoices_PageIndexChanged(object sender, EventArgs e)
        {
            GridViewInvoices.DataBind();
            GridViewInvoices.SelectRow(0);
        }

        protected void TextBox_TextChanged(object sender, EventArgs e)
        {
            HFInvoiceSaved.Value = "";

            TextBox txtBox = (TextBox)sender;
            if (txtBox.ID == "TextBoxOrgBilled")
                UpdateFieldsForOrgBilled(txtBox.Text);
        }

        private void UpdateFieldsForOrgBilled(string text)
        {
            //placed up here to use afterward to refocus
            CheckBox CB3086 = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CB3086");
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * from SPFNOrganizations WHERE DistrictID=@distID AND " +
                    "SpFnOrgName=@SpFnOrgName AND Inactive=0 ";
                    cmd.Parameters.AddWithValue("@SpFnOrgName", text);
                    cmd.Parameters.AddWithValue("@distID", Session["CurrentDistrict"]);
                    cmd.Connection = conn;
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if(CB3086 != null) { CB3086.Checked = (bool)dr["3086"]; }
                            CheckBox CBCACFP = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBCACFP");
                            if (CBCACFP != null) { CBCACFP.Checked= (bool)dr["CACFP"]; }
                            TextBox TextBoxContactPerson = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxContactPerson");
                            if(TextBoxContactPerson != null) { TextBoxContactPerson.Text = dr["SpFnOrgContact"].ToString(); }
                            TextBox TextBoxDueTo = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDueTo");
                            if(TextBoxDueTo != null) { TextBoxDueTo.Text = dr["SpFnOrgRemitTo"].ToString(); }
                        }
                    }
                    conn.Close();
                }

            }
            if (CB3086 != null)
                CB3086.Focus();
        }

        protected void SPFN_CheckedChanged(object sender, EventArgs e)
        {
            HFInvoiceSaved.Value = "";

            CheckBox cb = (CheckBox)sender;
            CheckBox CB3086 = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CB3086");
            CheckBox CBCACFP = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBCACFP");

            //only allow one of them to be checked
            if (cb.ID == "CB3086" && cb.Checked)
                CBCACFP.Checked = false;

            if (cb.ID == "CBCACFP" && cb.Checked)
                CB3086.Checked = false;

            //postback was losing focus - return the focus to the calling checkbox
            cb.Focus();


        }

        protected void GridViewMiscLines_DataBound(object sender, EventArgs e)
        {
            GridView GridViewMiscLines = (GridView)sender;
            HiddenField HFShowSalesTax = (HiddenField)FindControlRecursive(FormViewSpFnOrg, "HFShowSalesTax");
            bool boolShowTaxCol = false;
            if(HFShowSalesTax != null && HFShowSalesTax.Value=="True")
                boolShowTaxCol = true;

            //Footer row is used to add misc lines. Hide the footer when the invoice is locked.
            if (HFInvoiceLocked.Value == "True")
                GridViewMiscLines.ShowFooter = false;
            else
                GridViewMiscLines.ShowFooter = true;

            //TAX Column Handling
            HtmlGenericControl divTaxColumnHdr = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "divTaxColumnHdr");
            if (divTaxColumnHdr != null)
                divTaxColumnHdr.Visible = boolShowTaxCol;
            HtmlGenericControl divTotalColumnHdr = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "divTotalColumnHdr");
            if (divTotalColumnHdr != null)
            {
                if(boolShowTaxCol)
                {
                    divTotalColumnHdr.Attributes.Remove("col-sm-3");
                    divTotalColumnHdr.Attributes.Add("class","col-sm-2");
                }
                else
                {
                    divTotalColumnHdr.Attributes.Remove("col-sm-2");
                    divTotalColumnHdr.Attributes.Add("class", "col-sm-3");
                }
            }

            HtmlGenericControl divTaxColumnEdit = (HtmlGenericControl)FindControlRecursive(GridViewMiscLines, "divTaxColumnEdit");
            HtmlGenericControl divTaxColumnItem = (HtmlGenericControl)FindControlRecursive(GridViewMiscLines, "divTaxColumnItem");
            HtmlGenericControl divTaxColumnFooter = (HtmlGenericControl)FindControlRecursive(GridViewMiscLines, "divTaxColumnFooter");
            HtmlGenericControl divTotalColumnEdit = (HtmlGenericControl)FindControlRecursive(GridViewMiscLines, "divTotalColumnEdit");
            HtmlGenericControl divTotalColumnItem = (HtmlGenericControl)FindControlRecursive(GridViewMiscLines, "divTotalColumnItem");
            HtmlGenericControl divTotalColumnFooter = (HtmlGenericControl)FindControlRecursive(GridViewMiscLines, "divTotalColumnFooter");

            if (divTaxColumnEdit != null && divTotalColumnEdit!=null)
            {
                divTaxColumnEdit.Visible = boolShowTaxCol;
                if (boolShowTaxCol)
                {
                    divTotalColumnEdit.Attributes.Remove("col-sm-3");
                    divTotalColumnEdit.Attributes.Add("class", "col-sm-2");
                }
                else
                {
                    divTotalColumnEdit.Attributes.Remove("col-sm-2");
                    divTotalColumnEdit.Attributes.Add("class", "col-sm-3");
                }

            }

            if (divTaxColumnItem != null && divTotalColumnItem!=null)
            {
                divTaxColumnItem.Visible = boolShowTaxCol;
                if (boolShowTaxCol)
                {
                    divTotalColumnItem.Attributes.Remove("col-sm-3");
                    divTotalColumnItem.Attributes.Add("class", "col-sm-2");
                }
                else
                {
                    divTotalColumnItem.Attributes.Remove("col-sm-2");
                    divTotalColumnItem.Attributes.Add("class", "col-sm-3");
                }
            }

            if (divTaxColumnFooter != null && divTotalColumnFooter!=null)
            {
                divTaxColumnFooter.Visible = boolShowTaxCol;
                if (boolShowTaxCol)
                {
                    divTotalColumnFooter.Attributes.Remove("col-sm-3");
                    divTotalColumnFooter.Attributes.Add("class", "col-sm-2");
                }
                else
                {
                    divTotalColumnFooter.Attributes.Remove("col-sm-2");
                    divTotalColumnFooter.Attributes.Add("class", "col-sm-3");
                }
            }

        }

        protected void GridViewMiscLines_RowEditing(object sender, GridViewEditEventArgs e)
        {
            RegisterDOMReadyScript("UpdateDropdown", "MarkupShowHideEdit();");
        }

        #region Direct Printing Code

        //Note this code mostly came from the following page with some modification and updating
        //https://www.codeproject.com/Articles/95750/High-fidelity-printing-through-the-SSRS-Report-Vie

        protected void IPrintDirect_Click(object sender, EventArgs e)
        {
            //SaveInvoiceData();


            //This did not work - keeping to try again in the future (8-2020)
            string[] invoices = { HFInvoiceNumber.Value };
            UpdateReportParams(invoices, CBPrintOnlyMisc.Checked);
            //PrintReport();
            Print();
            UpdatePanelOverall.Update();

        }

        private Point _PrintingDPI;
        //private int _PrintingIndex, _PrintingPageCount;
        //private List<System.IO.Stream> _PrintingStreams;
        private byte[] _ImageFile;

        void Print()
        {

            //ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            //ReportParameter paramDistID = new ReportParameter("DistrictID", Session["CurrentDistrict"].ToString());
            //ReportParameter paramInvNum = new ReportParameter("InvoiceNumber", HFInvoiceNumber.Value);
            //ReportParameter paramMiscOnly = new ReportParameter("OnlyPrintMisc", "false");//need to add checkbox
            //try
            //{
            //    ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramInvNum, paramMiscOnly });
            //    ReportViewer1.ServerReport.Refresh();
            //}
            //catch { }

            UpdatePanelInvoices.Update();

            //Assume that the user has been shown the dialog and chosen a printer
            //otherwise, these can be obtained programmatically given a known
            //printer name.
            //var ps = printDialog1.PrinterSettings;
            //PrinterSettings ps = new PrinterSettings();
            //System.Windows.Forms.PrintDialog printDlg = new System.Windows.Forms.PrintDialog();
            //printDlg.AllowSelection = true;
            //printDlg.AllowSomePages = true;

            //allow selection of printer
            //if (printDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    ps = printDlg.PrinterSettings;



            //***********
            //LabelDefaultPrinter.Text = ps.PrinterName;

            //
            //var sbDeviceInfo = new StringBuilder(1024);
            //var xr = XmlWriter.Create(sbDeviceInfo);
            //xr.WriteStartElement("DeviceInfo");
            //xr.WriteElementString("OutputFormat", "EMF");
            //xr.WriteElementString("OutputFormat", "JPG");

            //This is only an estimate
            //_PrintingPageCount = ReportViewer1.ServerReport.GetTotalPages();

            //Ensure EMF is recorded on a bitmap with the same resolution as the printer
            //_PrintingDPI.X = 300;// ps.DefaultPageSettings.PrinterResolution.X;
            //_PrintingDPI.Y = 300;// ps.DefaultPageSettings.PrinterResolution.Y;
            //xr.WriteElementString("PrintDpiX", _PrintingDPI.X.ToString());
            //xr.WriteElementString("PrintDpiY", _PrintingDPI.Y.ToString());

            //xr.Close();

            //***********
            //LabelDefaultPrinter.Text += Environment.NewLine + sbDeviceInfo.ToString();


            //Estimate list capacity
            //_PrintingStreams = new List<System.IO.Stream>(_PrintingPageCount);
            //Render using the callback API

            //NameValueCollection nvc = null;
            //string exts;

            //var stream = new MemoryStream();


            string mimetype;
            string encoding;
            string fileext;
            string[] streams;
            Warning[] warnings;
            String deviceInfo = "<DeviceInfo>" +
                "<OutputFormat>PNG</OutputFormat>" +
                "<PrintDpiX>1200</PrintDpiX>" +
                "<PrintDpiY>1200</PrintDpiY>" +
                "<DpiX>1200</DpiX>" +
                "<DpiY>1200</DpiY>" +
                "</DeviceInfo>";


            _ImageFile = ReportViewer1.ServerReport.Render("IMAGE", deviceInfo, out mimetype, out encoding, out fileext, out streams, out warnings);
            //_ImageFile = ReportViewer1.ServerReport.Render("IMAGE", null, out mimetype, out encoding, out fileext, out streams, out warnings);

            String ImageUrl = "data:image/PNG;base64," + Convert.ToBase64String(_ImageFile);

            //System.Threading.Thread.Sleep(1500);

            RegisterDOMReadyScript("Printer", "PrintElem('" + ImageUrl + "');");


            //ReportViewer1.ServerReport.Render("IMAGE", null, nvc, stream, out mimetype, out exts);
            //_PrintingStreams.Add(stream);

            //("Image",
            //      sb.ToString(), lr_CreateStream, out warnings);
            //Reset all streams to the beginning
            //foreach (Stream s in _PrintingStreams) s.Position = 0;
            //And print the document as normal
            //PrintDocument pd = new PrintDocument();




            //pd.PrinterSettings = ps;

            //pd.DefaultPageSettings.Landscape = false; //true or false!
            //pd.PrintPage += pd_PrintPage;
            //pd.EndPrint += pd_EndPrint;


            //Synchronous
            //pd.Print();

            //pd.Dispose();
        }




        #endregion



        protected void BtnOpenEmailDialog_Click(object sender, EventArgs e)
        {
            RegisterDOMReadyScript("show", "$('#myModal').modal('show');");
            UpdatePanelModal.Update();
        }

        protected void ButtonSend_Click(object sender, EventArgs e)
        {
            //validate email domains
            TextBoxEmailTo_TextChanged(null, null);

            PreventMultipleClicks(ref ButtonSend);
            
            SendEmail(sender, e);

            //Normal modal hide was not working. Needed to add the following predecessors to remove the grayed out page
            //RegisterDOMReadyScript("pop", "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$('#myModal').hide();");
            //RegisterDOMReadyScript("pop", "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$('#myModal').modal('hide');");
            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "pop", "$('#myModal').hide();", true);
            //UpdatePanelOverall.Update();
        }

        protected void UpdateReportParams(string [] InvoiceNumbers, bool OnlyPrintMisc)
        {
            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramDistID = new ReportParameter("DistrictID", Session["CurrentDistrict"].ToString());
            ReportParameter paramMiscOnly = new ReportParameter("OnlyPrintMisc", value: OnlyPrintMisc.ToString());//CBPrintOnlyMisc.Checked.ToString()

            ReportParameter paramInvNum = new ReportParameter("InvoiceNumber");
            foreach (string inv in InvoiceNumbers)
            {
                if (string.IsNullOrEmpty(inv)==false)
                    paramInvNum.Values.Add(inv);
            }

            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramInvNum, paramMiscOnly });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }

        }


        protected void SendEmail(object sender, EventArgs e)
        {
            string emailServer = "";

            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view = (DataView)SqlDataSourceEmailSettings.Select(args);
            DataTable table = view.ToTable();

            emailServer = table.Rows[0]["emailServer"].ToString();
            Boolean emailUseEWS = table.Rows[0]["emailUseEWS"].ToString() == "0" ? false : true;
            Boolean emailUseSSL = table.Rows[0]["emailUseSSL"].ToString() == "0" ? false : true;
            string emailFrom = table.Rows[0]["emailFromAddress"].ToString();

            string emailTo = TextBoxEmailTo.Text;
            int emailPort = (int)table.Rows[0]["emailPort"];
            string emailDomain = table.Rows[0]["emailAuthDomain"].ToString();
            string emailUsername = table.Rows[0]["emailAuthUser"].ToString();
            string emailPassword = table.Rows[0]["emailAuthPassword"].ToString();

            

            string subject = "Paperwork Web - Special Functions Invoice - " + HFInvoiceNumber.Value.Trim();

            //Email body
            StringBuilder sb = new StringBuilder();
            sb.Append(" * *Automated email from the Paperwork Web Special Functions page. Do not reply to this email.* *\r\n");
            sb.Append("\r\n");
            sb.Append("Attached is Specail Functions Invoice "); sb.Append(HFInvoiceNumber.Value.Trim());sb.Append( " for district: "); sb.Append(Session["CurrentDistrictName"].ToString()); sb.Append(".\r\n");
            sb.Append("Invoice sent by user:  "); sb.Append(Session["CurrentUser"].ToString()); sb.Append("\r\n");

            string content = sb.ToString();

            if (emailUseEWS)
            {
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
                string uriPrefix = "";
                string ewsString = "";
                if (emailUseSSL)
                    uriPrefix = @"https://";
                else
                    uriPrefix = @"http://";

                if (emailUseEWS)
                    ewsString = @"/EWS/Exchange.asmx";

                Uri serverURI = new Uri(@uriPrefix + @emailServer + @ewsString);
                service.Url = serverURI;

                if (service.Url.ToString() != String.Empty)
                {
                    if (emailUsername == String.Empty)
                        service.UseDefaultCredentials = true;
                    else if (emailDomain == String.Empty)
                    {
                        ExchangeCredentials ec = new NetworkCredential(emailUsername, emailPassword);
                        service.Credentials = ec;
                    }
                    else
                    {
                        ExchangeCredentials ec = new NetworkCredential(emailUsername, emailPassword, emailDomain);
                        service.Credentials = ec;
                    }

                    EmailMessage message = new EmailMessage(service);

                    String[] addresses;

                    addresses = emailTo.Split(',');

                    foreach (string address in addresses)
                    {
                        EmailAddress To = new EmailAddress(address);
                        message.ToRecipients.Add(To);
                    }
                    message.Subject = subject;
                    message.Importance = Importance.High;
                    MessageBody body = new MessageBody();

                    body.BodyType = BodyType.Text;

                    body.Text += content;

                    message.Body = body;
                    message.From = new EmailAddress(emailFrom);

                    //Collection of memorystreams which will hold the attachments
                    var msList = new List<MemoryStream>();
                    var bwList = new List<BinaryWriter>();

                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string filenameExtension;
                    string reportName;

                    //Nutrition is not using EWS, but this might work to add attachments is switch to down the road.
                    //if (CBSalesSchool.Checked)
                    //{
                    //    reportName = "SalesTotals";
                    //    ReportViewer1.ServerReport.ReportPath = HFSSRSBaseFolder.Value + reportName;
                    //    ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
                    //    ReportParameter paramDistID = new ReportParameter("DistrictID", DropDownListDistricts.SelectedValue);
                    //    DateTime startDate = new DateTime(int.Parse(TextBoxFYearmonth.Text.Substring(0, 4)), int.Parse(TextBoxFYearmonth.Text.Substring(5, 2)), 1);
                    //    DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                    //    ReportParameter paramStartDate = new ReportParameter("RecapDate", startDate.ToShortDateString());
                    //    ReportParameter paramEndDate = new ReportParameter("RecapDate2", endDate.ToShortDateString());
                    //    ReportParameter paramRevisions = new ReportParameter("ShowRevisions", "true");//report runs with this checked by default
                    //    ReportParameter paramComments = new ReportParameter("ShowComments", "true");//report runs with this checked by default
                    //    try
                    //    {
                    //        ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramStartDate, paramEndDate, paramRevisions, paramComments });
                    //        ReportViewer1.ServerReport.Refresh();
                    //    }
                    //    catch { }


                    //    byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    //    MemoryStream ms1 = new System.IO.MemoryStream(bytes);
                    //    msList.Add(ms1);
                    //    BinaryWriter bw1 = new BinaryWriter(ms1);
                    //    bwList.Add(bw1);
                    //    bw1.Write(bytes);
                    //    bw1.Flush();

                    //    ms1.Position = 0;

                    //    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                    //    //System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms1, ct);
                    //    //attach.ContentDisposition.FileName = reportName + ".pdf";

                    //    message.Attachments.AddFileAttachment(reportName, ms1);

                    //}

                    try
                    {
                        FolderId draftID = new FolderId(WellKnownFolderName.Drafts, emailFrom);
                        //message.SendAndSaveCopy(sentID);
                        message.Save(draftID);
                        message.Send();
                    }
                    catch (Microsoft.Exchange.WebServices.Data.ServiceRequestException ex)
                    {
                        ShowDialog("Error", MessageType.Error, "Exchange Webservices Error occurred.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                    }
                    finally
                    {
                        //Session["VendorRequestMsg"] = "Vendor request successfully sent.";
                        //Response.Redirect("~/Setup/SetupDistrictVendors");
                    }
                }//service URL is not empty
            }//using EWS
            else
            {
                MailAddress Fm = new MailAddress(emailFrom, emailFrom);
                System.Net.Mail.MailMessage mailMessage = new MailMessage();
                mailMessage.From = Fm;

                String[] addresses;
                if (emailTo.Contains(","))
                    addresses = emailTo.Split(',');
                else 
                    addresses = emailTo.Split(';');

                bool illegalAddressesFound = false;
                foreach (string address in addresses)
                {
                    if (string.IsNullOrEmpty(address))
                        continue;
                    MailAddress To = new MailAddress(address);
                    if(To.Address.ToLower().Contains("thenutritiongroup.biz") || To.Address.ToLower().Contains("iceburghsolutions.com"))
                        mailMessage.To.Add(To);
                    else
                        illegalAddressesFound = true;
                }

                if (mailMessage.To.Count == 0)
                    return;

                mailMessage.Subject = subject;
                mailMessage.Priority = System.Net.Mail.MailPriority.Normal;
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = content;

                SmtpClient client = new SmtpClient();
                client.Host = emailServer;
                client.Port = emailPort;

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 30000;
                client.EnableSsl = emailUseSSL;

                if (emailUsername == String.Empty && emailPassword == String.Empty)
                    client.UseDefaultCredentials = true;
                else if (emailDomain != String.Empty)
                {
                    client.UseDefaultCredentials = false;
                    NetworkCredential nc = new NetworkCredential(emailUsername, emailPassword, emailDomain);
                    client.Credentials = nc;
                }
                else
                {
                    client.UseDefaultCredentials = false;
                    NetworkCredential nc = new NetworkCredential(emailUsername, emailPassword);
                    client.Credentials = nc;
                }

                //Collection of memorystreams which will hold the attachments
                var msList = new List<MemoryStream>();
                var bwList = new List<BinaryWriter>();

                //Attachments
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                string[] invoices = { HFInvoiceNumber.Value };
                UpdateReportParams(invoices, CBPrintOnlyMisc.Checked);

                byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                MemoryStream ms1 = new System.IO.MemoryStream(bytes);
                msList.Add(ms1);
                BinaryWriter bw1 = new BinaryWriter(ms1);
                bwList.Add(bw1);
                bw1.Write(bytes);
                bw1.Flush();

                ms1.Position = 0;

                string reportName = "Special Functions Invoice " + HFInvoiceNumber.Value.Trim();

                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms1, ct);
                attach.ContentDisposition.FileName = reportName + ".pdf";

                mailMessage.Attachments.Add(attach);

                try
                {
                    //Check to see if anything was marked and attached
                    if (msList.Count > 0)
                    {
                        client.Send(mailMessage);
                    }
                    ShowMessageIcon("Invoice " + HFInvoiceNumber.Value.Trim() + " Emailed", MessageType.Success);
                    //ShowDialog("Success", MessageType.Success, "Email sent","");

                }
                catch (SmtpFailedRecipientsException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SMTP Failed Recipients Error occurred.", ex.Message);
                }
                catch (SmtpException ex)
                {
                    ShowDialog("Error", MessageType.Error, "An SMTP Error occurred.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An Error occurred.", ex.Message);
                }
                finally
                {
                    foreach (var bw in bwList)
                    {
                        bw.Dispose();
                    }
                    foreach (var ms in msList)
                    {
                        ms.Dispose();
                    }

                    if (mailMessage != null)
                        mailMessage.Dispose();

                    if (client != null)
                        client.Dispose();
                }

            }

        }

        protected void TextBoxEmailTo_TextChanged(object sender, EventArgs e)
        {
            regexEmailValid.Validate();
            if (regexEmailValid.IsValid == false)
                return;

            //Validate email domains
            string emailTo = TextBoxEmailTo.Text;
            String[] addresses;
            if (emailTo.Contains(","))
                addresses = emailTo.Split(',');
            else
                addresses = emailTo.Split(';');

            bool illegalAddressesFound = false;
            foreach (string address in addresses)
            {
                if (address.ToLower().Contains("thenutritiongroup.biz") == false && address.ToLower().Contains("iceburghsolutions.com") == false)
                {
                    illegalAddressesFound = true;
                    TextBoxEmailTo.Text=TextBoxEmailTo.Text.Replace(address, "");
                }
            }

            if (illegalAddressesFound)
            {
                customValidator.IsValid = false;
                customValidator.ErrorMessage = "Only thenutritiongroup.biz emails are allowed.";
                TextBoxEmailTo.Focus();
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
                if (recordIndex != -1)
                {
                    DataRowView drvSql = dvSql[recordIndex];
                    HFStartDate.Value = drvSql["StartDate"].ToString();
                    HFEndDate.Value = drvSql["EndDate"].ToString();
                }
                else
                {
                    HFStartDate.Value = "";
                    HFEndDate.Value = "";
                }
            }
            else
            {
                //if no program years could be populated - redirect home
                Response.Redirect("~/default.aspx");
            }
        }

        protected void IPrintRangeOpen_Click(object sender, EventArgs e)
        {
            RegisterDOMReadyScript("show", "$('#ModalRangePrint').modal('show');");
            UpdatePanelModalPrintRange.Update();
        }

        protected void ButtonPrintRange_Click(object sender, EventArgs e)
        {
            PreventMultipleClicks(ref ButtonSend);

            PrintSelectedInvoices();

        }

        protected void PrintSelectedInvoices()
        {
            string SelectedInvoices = string.Empty;
            foreach( GridViewRow gvr in GridViewPrintInvoices.Rows)
            {
                if(gvr.RowType==DataControlRowType.DataRow)
                {
                    CheckBox CBPrint = (CheckBox)gvr.FindControl("CBPrint");
                    if (CBPrint != null)
                    {
                        if (CBPrint.Checked)
                        {
                            if (!string.IsNullOrEmpty(SelectedInvoices)) SelectedInvoices += ",";
                            
                            SelectedInvoices += GridViewPrintInvoices.DataKeys[gvr.RowIndex].Values["InvoiceNumber"].ToString();
                        }
                    }
                }
            }

            if(SelectedInvoices != null && SelectedInvoices.Length>0) 
            {
                Session["CurrentInvoice"] = SelectedInvoices;
                Session["CurrentProgramYear"] = DDLProgramYears.SelectedValue;
                Response.Redirect("~/Print/PrintSpecialFuncInvoices.aspx");
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openTab", "window.open('Print/PrintSpecialFuncInvoices.aspx' ,'_blank');", true);
            }

        }

    }
}
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;




namespace Paperwork
{
    //[System.Web.Script.Services.ScriptService]
    public partial class SpecialFunctionsEntryTEST : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            if (Session["CurrentDistrict"] == null)
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
                //Run to set the ProgramYear date boundaries
                DDLProgramYears_SelectedIndexChanged(DDLProgramYears, EventArgs.Empty);

                //On initial load - select the first invoice
                GridViewInvoices.DataBind();

                if (GridViewInvoices.SelectedIndex <=0 && GridViewInvoices.Rows.Count > 0)
                {
                    if (Session["CurrentInvoice"] != null && !string.IsNullOrEmpty(Session["CurrentInvoice"].ToString()) && Session["CurrentInvoice"].ToString().IndexOf(',') == -1)
                    {
                        try
                        {
                            bool invoiceFound = false;
                            for(int page=0; page<GridViewInvoices.PageCount; page++)
                            {
                                GridViewInvoices.PageIndex = page;
                                GridViewInvoices.DataBind();
                                for(int i = 0; i < GridViewInvoices.DataKeys.Count; i++)
                                {
                                    if(GridViewInvoices.DataKeys[i].Values["InvoiceNumber"].ToString() == Session["CurrentInvoice"].ToString())
                                    {
                                        GridViewInvoices.SelectRow(i);
                                        invoiceFound = true;
                                        break;
                                    }
                                }
                                //need to stop the page search - otherwise it will keep going to the last page
                                if(invoiceFound == true)
                                    break;
                            }

                            if(invoiceFound == false)
                            {
                                GridViewInvoices.PageIndex= 0;
                                GridViewInvoices.SelectRow(0);
                            }
                        }
                        catch
                        {
                            GridViewInvoices.SelectRow(0);
                        }
                    }
                    else
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

            if (GridViewMiscLines == null || TextBoxFMiscDesc == null || TextBoxFMiscAmount == null || TextBoxFMarkupPct == null ||
                TextBoxFMarkupDlr == null || DDLFMarkupType == null || CBFMiscAmountTaxable == null)
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

                    HiddenField HFOrg3086 = (HiddenField)FindControlRecursive(row, "HFOrg3086");
                    HiddenField HFOrgCACFP = (HiddenField)FindControlRecursive(row, "HFOrgCACFP");
                    if (HFOrg3086 != null && HFOrgCACFP != null)
                    {
                        if (HFOrg3086.Value == "True" || HFOrgCACFP.Value == "True")
                        {
                            FormViewSpFnCosts.Visible = false;
                        }
                        else
                        {
                            FormViewSpFnCosts.Visible = true;
                        }

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
        }

        protected void ICreateClick(object sender, EventArgs e)
        {
            string InvoiceNumber = GetNextSpFnInvoiceNumber();

            if (string.IsNullOrEmpty(InvoiceNumber))
            {
                ShowDialog("Alert", MessageType.Warning, "The next invoice number could not be retrieved. Process cancelled.", "");
                return;
            }

            if (GenerateInvoiceHdr(InvoiceNumber) == false)
            {
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

            string sqlGenerateInvoiceHdr = "INSERT INTO [SPFNInvoiceHDR] ([ProgramYear],[DistrictID],[InvoiceNumber],[CreatedDate],[CreatedUser],[Locked],[BillingDate],[FunctionDate],[SalesTaxPct],[TNGFringePct]) "
                + " VALUES(@ProgramYear, @DistrictID, @InvoiceNumber, @CreatedDate, @CreatedUser, 0, '1/1/1900' , GETDATE()," +
                " (SELECT IIF(DistrictState='SC', SC_SalesTax, 0.00000) SalesTax FROM TaxRates,District WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID)," +
                " (SELECT [TNGFringe]*100 FROM [TaxRates] WHERE [ProgramYear]=@ProgramYear) );";

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

        protected bool SaveDFNLabor(decimal decDFNLabor)
        {
            bool success = false;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);


            string UpdateInvoiceQuery;
            UpdateInvoiceQuery = "UPDATE [SPFNInvoiceHDR] SET FSDCosts=@FSDCosts " +
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
                #endregion

                SqlParameter dbParam_DFNLabor = new SqlParameter();
                dbParam_DFNLabor.ParameterName = "@FSDCosts";
                dbParam_DFNLabor.Value = decDFNLabor;
                dbParam_DFNLabor.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DFNLabor);

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();
                    if (recordcount > 0)
                    {
                        success = true;
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error updating the DFN labor on the invoice.", ex.Message);
                    HFInvoiceSaved.Value = "ERROR";
                }
                finally
                {
                    dbConnection.Close();
                }
            }
            return success;
        }

        protected bool SaveInvoiceData()
        {
            bool invoiceNumberChanged = false;
            bool invoiceSaved = false;

            if (HFInvoiceSaved.Value == "1")
                return true;

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
            CheckBox CBMealsTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBMealsTaxable");//New 2024 for 3086/CACFP form
            CheckBox CBPaperCleanSupplTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBPaperCleanSupplTaxable");
            TextBox TextBoxSalesTaxPct = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSalesTaxPct");
            //New 2024 - expanding out the Paper Clean Supplies
            TextBox TextBoxPaperCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxPaperCost");
            TextBox TextBoxCleaningCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxCleaningCost");
            TextBox TextBoxSuppliesCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSuppliesCost");
            CheckBox CBPaperTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBPaperTaxable");
            CheckBox CBCleaningTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBCleaningTaxable");
            CheckBox CBSuppliesTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBSuppliesTaxable");

            //Costs - kept in separate formview
            TextBox TextBoxFoodCostActual = (TextBox)FindControlRecursive(FormViewSpFnCosts,"TextBoxFoodCostActual");
            TextBox TextBoxPaperCostActual = (TextBox)FindControlRecursive(FormViewSpFnCosts, "TextBoxPaperCostActual");
            TextBox TextBoxCleaningCostActual = (TextBox)FindControlRecursive(FormViewSpFnCosts, "TextBoxCleaningCostActual");
            TextBox TextBoxSuppliesCostActual = (TextBox)FindControlRecursive(FormViewSpFnCosts, "TextBoxSuppliesCostActual");

            //New Sep 2024 - Enhancements to entry for 3086 and CACFP
            TextBox TextBoxBreakFoodCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakFoodCPM");
            TextBox TextBoxBreakPaperCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakPaperCPM");
            TextBox TextBoxBreakCleanCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakCleanCPM");
            TextBox TextBoxBreakOtherCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakOtherCPM");
            TextBox TextBoxLunchFoodCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchFoodCPM");
            TextBox TextBoxLunchPaperCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchPaperCPM");
            TextBox TextBoxLunchCleanCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchCleanCPM");
            TextBox TextBoxLunchOtherCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchOtherCPM");
            TextBox TextBoxDinnerFoodCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerFoodCPM");
            TextBox TextBoxDinnerPaperCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerPaperCPM");
            TextBox TextBoxDinnerCleanCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerCleanCPM");
            TextBox TextBoxDinnerOtherCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerOtherCPM");
            TextBox TextBoxSnackFoodCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackFoodCPM");
            TextBox TextBoxSnackPaperCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackPaperCPM");
            TextBox TextBoxSnackCleanCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackCleanCPM");
            TextBox TextBoxSnackOtherCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackOtherCPM");

            TextBox TextBoxBreakRate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakRate");
            TextBox TextBoxBreakCount = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakCount");
            TextBox TextBoxLunchRate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchRate");
            TextBox TextBoxLunchCount = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchCount");
            TextBox TextBoxDinnerRate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerRate");
            TextBox TextBoxDinnerCount = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerCount");
            TextBox TextBoxSnackRate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackRate");
            TextBox TextBoxSnackCount = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackCount");



            if (TextBoxInvoice == null || TextBoxOrgBilled == null || TextBoxContactPerson == null || TextBoxSpecFuncDesc == null || TextBoxFunctionDate == null ||
                TextBoxBillingDate == null || TextBoxPONumber == null || TextBoxDatePaid == null || TextBoxFoodCost == null || TextBoxMileage == null || TextBoxFSDCosts == null ||
                TextBoxTNGLabor == null || TextBoxTNGFringePct == null || TextBoxInvoiceNotes == null || TextBoxMarkupPct == null || TextBoxMarkupDlr == null || TextBoxDueTo == null ||
                DDLMarkupType == null || CBPrintNotes == null || CB3086 == null || CBCACFP == null || TextBoxPaymentNumber == null || TextBoxPaperCleanSup == null || TextBoxDistLabor == null ||
                TextBoxDistFringePct == null || CBFoodCostTaxable == null || CBMealsTaxable==null || CBPaperCleanSupplTaxable == null || TextBoxSalesTaxPct == null ||
                TextBoxPaperCost == null || TextBoxCleaningCost == null || TextBoxSuppliesCost == null || CBPaperTaxable == null || CBCleaningTaxable == null || CBSuppliesTaxable == null ||
                //Costs
                TextBoxFoodCostActual == null || TextBoxPaperCostActual == null || TextBoxCleaningCostActual == null || TextBoxSuppliesCostActual == null ||
                TextBoxBreakFoodCPM==null || TextBoxBreakPaperCPM==null || TextBoxBreakCleanCPM==null || TextBoxBreakOtherCPM==null || TextBoxLunchFoodCPM==null || TextBoxLunchPaperCPM==null ||
                TextBoxLunchCleanCPM==null || TextBoxLunchOtherCPM==null || TextBoxDinnerFoodCPM==null || TextBoxDinnerPaperCPM==null || TextBoxDinnerCleanCPM==null || TextBoxDinnerOtherCPM==null ||
                TextBoxSnackFoodCPM==null || TextBoxSnackPaperCPM==null || TextBoxSnackCleanCPM==null || TextBoxSnackOtherCPM==null ||
                TextBoxBreakRate==null || TextBoxBreakCount==null || TextBoxLunchRate==null || TextBoxLunchCount==null || TextBoxDinnerRate==null || TextBoxDinnerCount==null || TextBoxSnackRate==null || TextBoxSnackCount==null)
                    return false;
            #endregion


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string UpdateInvoiceQuery;
            UpdateInvoiceQuery = "UPDATE [SPFNInvoiceHDR] SET [InvoiceNumber]=@NewInvoiceNumber, [OrgBilled]=@OrgBilled, OrgContact=@OrgContact, FunctionDate=@FunctionDate, " +
                "BillingDate=@BillingDate, OrgRemitTo=@OrgRemitTo, SpecialFunctionDesc=@SpecialFunctionDesc, FoodCost=@FoodCost, " +
                "PaperCleanSuppl=@PaperCleanSuppl, PaperCost=@PaperCost, CleaningCost=@CleaningCost, SuppliesCost=@SuppliesCost, " +
                "Mileage=@Mileage, FSDCosts=@FSDCosts, TNGLabor=@TNGLabor, TNGFringePct=@TNGFringePct, " +
                "DistrictLabor=@DistrictLabor, DistrictFringePct=@DistrictFringePct, MarkupType=@MarkupType, MarkupDlr=@MarkupDlr, " +
                "MarkupPct=@MarkupPct, InvoiceNotes=@InvoiceNotes, PrintNotes=@PrintNotes, PONumber=@PONumber, DatePaid=@DatePaid, " +
                "Org3086=@Org3086, OrgCACFP=@OrgCACFP, PaymentNumber=@PaymentNumber, FoodCostTaxable=@FoodCostTaxable, PaperCleanSupplTaxable=@PaperCleanSupplTaxable, SalesTaxPct=@SalesTaxPct, " +
                "PaperCostTaxable=@PaperCostTaxable, CleaningCostTaxable=@CleaningCostTaxable, SuppliesCostTaxable=@SuppliesCostTaxable, " +
                //New Actual Costs values
                "FoodCostActual=@FoodCostActual, PaperCostActual=@PaperCostActual, CleaningCostActual=@CleaningCostActual, SuppliesCostActual=@SuppliesCostActual, " +
                //New CACFP&3086 values
                "BreakFoodCPM=@BreakFoodCPM, BreakPaperCPM=@BreakPaperCPM, BreakCleanCPM=@BreakCleanCPM, BreakOtherCPM=@BreakOtherCPM, LunchFoodCPM=@LunchFoodCPM, LunchPaperCPM=@LunchPaperCPM, " +
                "LunchCleanCPM=@LunchCleanCPM, LunchOtherCPM=@LunchOtherCPM, SnackFoodCPM=@SnackFoodCPM, SnackPaperCPM=@SnackPaperCPM, SnackCleanCPM=@SnackCleanCPM, SnackOtherCPM=@SnackOtherCPM, " +
                "DinnerFoodCPM=@DinnerFoodCPM, DinnerPaperCPM=@DinnerPaperCPM, DinnerCleanCPM=@DinnerCleanCPM, DinnerOtherCPM=@DinnerOtherCPM, BreakRate=@BreakRate, BreakCount=@BreakCount, " +
                "LunchRate=@LunchRate, LunchCount=@LunchCount, SnackRate=@SnackRate, SnackCount=@SnackCount, DinnerRate=@DinnerRate, DinnerCount=@DinnerCount " +
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

                SqlParameter dbParam_Paper = new SqlParameter();
                dbParam_Paper.ParameterName = "@PaperCost";
                dbParam_Paper.Value = TextBoxPaperCost.Text;
                dbParam_Paper.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_Paper);

                SqlParameter dbParam_Cleaning = new SqlParameter();
                dbParam_Cleaning.ParameterName = "@CleaningCost";
                dbParam_Cleaning.Value = TextBoxCleaningCost.Text;
                dbParam_Cleaning.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_Cleaning);

                SqlParameter dbParam_Supplies = new SqlParameter();
                dbParam_Supplies.ParameterName = "@SuppliesCost";
                dbParam_Supplies.Value = TextBoxSuppliesCost.Text;
                dbParam_Supplies.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_Supplies);

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
                dbParam_FoodCostTaxable.Value = CBFoodCostTaxable.Checked || CBMealsTaxable.Checked?true:false;
                dbParam_FoodCostTaxable.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_FoodCostTaxable);

                SqlParameter dbParam_PaperCleanSupplTaxable = new SqlParameter();
                dbParam_PaperCleanSupplTaxable.ParameterName = "@PaperCleanSupplTaxable";
                dbParam_PaperCleanSupplTaxable.Value = CBPaperCleanSupplTaxable.Checked;
                dbParam_PaperCleanSupplTaxable.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_PaperCleanSupplTaxable);

                SqlParameter dbParam_PaperTaxable = new SqlParameter();
                dbParam_PaperTaxable.ParameterName = "@PaperCostTaxable";
                dbParam_PaperTaxable.Value = CBPaperTaxable.Checked;
                dbParam_PaperTaxable.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_PaperTaxable);

                SqlParameter dbParam_CleaningTaxable = new SqlParameter();
                dbParam_CleaningTaxable.ParameterName = "@CleaningCostTaxable";
                dbParam_CleaningTaxable.Value = CBCleaningTaxable.Checked;
                dbParam_CleaningTaxable.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_CleaningTaxable);

                SqlParameter dbParam_SuppliesTaxable = new SqlParameter();
                dbParam_SuppliesTaxable.ParameterName = "@SuppliesCostTaxable";
                dbParam_SuppliesTaxable.Value = CBSuppliesTaxable.Checked;
                dbParam_SuppliesTaxable.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_SuppliesTaxable);

                decimal.TryParse(TextBoxSalesTaxPct.Text, out decimal SalesTax);

                SqlParameter dbParam_SalesTaxPct = new SqlParameter();
                dbParam_SalesTaxPct.ParameterName = "@SalesTaxPct";
                dbParam_SalesTaxPct.Value = SalesTax / 100;
                dbParam_SalesTaxPct.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_SalesTaxPct);

                #endregion
                #region New Actual Costs
                SqlParameter dbParam_FoodCostActual = new SqlParameter();
                dbParam_FoodCostActual.ParameterName = "@FoodCostActual";
                dbParam_FoodCostActual.Value = TextBoxFoodCostActual.Text;
                dbParam_FoodCostActual.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_FoodCostActual);

                SqlParameter dbParam_PaperCostActual = new SqlParameter();
                dbParam_PaperCostActual.ParameterName = "@PaperCostActual";
                dbParam_PaperCostActual.Value = TextBoxPaperCostActual.Text;
                dbParam_PaperCostActual.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_PaperCostActual);

                SqlParameter dbParam_CleaningCostActual = new SqlParameter();
                dbParam_CleaningCostActual.ParameterName = "@CleaningCostActual";
                dbParam_CleaningCostActual.Value = TextBoxCleaningCostActual.Text;
                dbParam_CleaningCostActual.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_CleaningCostActual);

                SqlParameter dbParam_SuppliesCostActual = new SqlParameter();
                dbParam_SuppliesCostActual.ParameterName = "@SuppliesCostActual";
                dbParam_SuppliesCostActual.Value = TextBoxSuppliesCostActual.Text;
                dbParam_SuppliesCostActual.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_SuppliesCostActual);

                #endregion
                #region New CACFP&3086

                SqlParameter dbParam_BreakFoodCost = new SqlParameter();
                dbParam_BreakFoodCost.ParameterName = "@BreakFoodCPM";
                dbParam_BreakFoodCost.Value = TextBoxBreakFoodCPM.Text;
                dbParam_BreakFoodCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BreakFoodCost);

                SqlParameter dbParam_BreakPaperCost = new SqlParameter();
                dbParam_BreakPaperCost.ParameterName = "@BreakPaperCPM";
                dbParam_BreakPaperCost.Value = TextBoxBreakPaperCPM.Text;
                dbParam_BreakPaperCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BreakPaperCost);

                SqlParameter dbParam_BreakCleanCost = new SqlParameter();
                dbParam_BreakCleanCost.ParameterName = "@BreakCleanCPM";
                dbParam_BreakCleanCost.Value = TextBoxBreakCleanCPM.Text;
                dbParam_BreakCleanCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BreakCleanCost);

                SqlParameter dbParam_BreakOtherCost = new SqlParameter();
                dbParam_BreakOtherCost.ParameterName = "@BreakOtherCPM";
                dbParam_BreakOtherCost.Value = TextBoxBreakOtherCPM.Text;
                dbParam_BreakOtherCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BreakOtherCost);

                SqlParameter dbParam_LunchFoodCost = new SqlParameter();
                dbParam_LunchFoodCost.ParameterName = "@LunchFoodCPM";
                dbParam_LunchFoodCost.Value = TextBoxLunchFoodCPM.Text;
                dbParam_LunchFoodCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchFoodCost);

                SqlParameter dbParam_LunchPaperCost = new SqlParameter();
                dbParam_LunchPaperCost.ParameterName = "@LunchPaperCPM";
                dbParam_LunchPaperCost.Value = TextBoxLunchPaperCPM.Text;
                dbParam_LunchPaperCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchPaperCost);

                SqlParameter dbParam_LunchCleanCost = new SqlParameter();
                dbParam_LunchCleanCost.ParameterName = "@LunchCleanCPM";
                dbParam_LunchCleanCost.Value = TextBoxLunchCleanCPM.Text;
                dbParam_LunchCleanCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchCleanCost);

                SqlParameter dbParam_LunchOtherCost = new SqlParameter();
                dbParam_LunchOtherCost.ParameterName = "@LunchOtherCPM";
                dbParam_LunchOtherCost.Value = TextBoxLunchOtherCPM.Text;
                dbParam_LunchOtherCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchOtherCost);

                SqlParameter dbParam_SnackFoodCost = new SqlParameter();
                dbParam_SnackFoodCost.ParameterName = "@SnackFoodCPM";
                dbParam_SnackFoodCost.Value = TextBoxSnackFoodCPM.Text;
                dbParam_SnackFoodCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_SnackFoodCost);

                SqlParameter dbParam_SnackPaperCost = new SqlParameter();
                dbParam_SnackPaperCost.ParameterName = "@SnackPaperCPM";
                dbParam_SnackPaperCost.Value = TextBoxSnackPaperCPM.Text;
                dbParam_SnackPaperCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_SnackPaperCost);

                SqlParameter dbParam_SnackCleanCost = new SqlParameter();
                dbParam_SnackCleanCost.ParameterName = "@SnackCleanCPM";
                dbParam_SnackCleanCost.Value = TextBoxSnackCleanCPM.Text;
                dbParam_SnackCleanCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_SnackCleanCost);

                SqlParameter dbParam_SnackOtherCost = new SqlParameter();
                dbParam_SnackOtherCost.ParameterName = "@SnackOtherCPM";
                dbParam_SnackOtherCost.Value = TextBoxSnackOtherCPM.Text;
                dbParam_SnackOtherCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_SnackOtherCost);

                SqlParameter dbParam_DinnerFoodCost = new SqlParameter();
                dbParam_DinnerFoodCost.ParameterName = "@DinnerFoodCPM";
                dbParam_DinnerFoodCost.Value = TextBoxDinnerFoodCPM.Text;
                dbParam_DinnerFoodCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DinnerFoodCost);

                SqlParameter dbParam_DinnerPaperCost = new SqlParameter();
                dbParam_DinnerPaperCost.ParameterName = "@DinnerPaperCPM";
                dbParam_DinnerPaperCost.Value = TextBoxDinnerPaperCPM.Text;
                dbParam_DinnerPaperCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DinnerPaperCost);

                SqlParameter dbParam_DinnerCleanCost = new SqlParameter();
                dbParam_DinnerCleanCost.ParameterName = "@DinnerCleanCPM";
                dbParam_DinnerCleanCost.Value = TextBoxDinnerCleanCPM.Text;
                dbParam_DinnerCleanCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DinnerCleanCost);

                SqlParameter dbParam_DinnerOtherCost = new SqlParameter();
                dbParam_DinnerOtherCost.ParameterName = "@DinnerOtherCPM";
                dbParam_DinnerOtherCost.Value = TextBoxDinnerOtherCPM.Text;
                dbParam_DinnerOtherCost.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DinnerOtherCost);

                SqlParameter dbParam_BreakRate = new SqlParameter();
                dbParam_BreakRate.ParameterName = "@BreakRate";
                dbParam_BreakRate.Value = TextBoxBreakRate.Text;
                dbParam_BreakRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_BreakRate);

                SqlParameter dbParam_BreakCount = new SqlParameter();
                dbParam_BreakCount.ParameterName = "@BreakCount";
                dbParam_BreakCount.Value = TextBoxBreakCount.Text;
                dbParam_BreakCount.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_BreakCount);

                SqlParameter dbParam_LunchRate = new SqlParameter();
                dbParam_LunchRate.ParameterName = "@LunchRate";
                dbParam_LunchRate.Value = TextBoxLunchRate.Text;
                dbParam_LunchRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_LunchRate);

                SqlParameter dbParam_LunchCount = new SqlParameter();
                dbParam_LunchCount.ParameterName = "@LunchCount";
                dbParam_LunchCount.Value = TextBoxLunchCount.Text;
                dbParam_LunchCount.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_LunchCount);

                SqlParameter dbParam_SnackRate = new SqlParameter();
                dbParam_SnackRate.ParameterName = "@SnackRate";
                dbParam_SnackRate.Value = TextBoxSnackRate.Text;
                dbParam_SnackRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_SnackRate);

                SqlParameter dbParam_SnackCount = new SqlParameter();
                dbParam_SnackCount.ParameterName = "@SnackCount";
                dbParam_SnackCount.Value = TextBoxSnackCount.Text;
                dbParam_SnackCount.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_SnackCount);

                SqlParameter dbParam_DinnerRate = new SqlParameter();
                dbParam_DinnerRate.ParameterName = "@DinnerRate";
                dbParam_DinnerRate.Value = TextBoxDinnerRate.Text;
                dbParam_DinnerRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_DinnerRate);

                SqlParameter dbParam_DinnerCount = new SqlParameter();
                dbParam_DinnerCount.ParameterName = "@DinnerCount";
                dbParam_DinnerCount.Value = TextBoxDinnerCount.Text;
                dbParam_DinnerCount.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_DinnerCount);


                #endregion

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();
                    if (recordcount > 0)
                    {
                        invoiceSaved = true;
                        ShowMessageByName("Invoice Saved. ", MessageType.Success, "alert_container_InvoiceHdr", 3000);
                        // if the invoice number was updated, need to update any misc lines as well
                        if (TextBoxInvoice.Text != HFInvoiceNumber.Value)
                        {
                            UpdateMiscLinesInvoiceNum(DDLProgramYears.SelectedValue, Session["CurrentDistrict"].ToString(), HFInvoiceNumber.Value, TextBoxInvoice.Text);
                            UpdatePayAuthInvoiceNum(DDLProgramYears.SelectedValue, Session["CurrentDistrict"].ToString(), HFInvoiceNumber.Value, TextBoxInvoice.Text);
                            HFInvoiceNumber.Value = TextBoxInvoice.Text;
                        }

                        //Check the misc lines footer for unsaved inserts
                        //if fields are empty, the insert will not happen
                        SqlDataSourceInvLines.Insert();
                    }

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error saving the Invoice.", ex.Message);
                    HFInvoiceSaved.Value = "ERROR";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            HFInvoiceSaved.Value = "1";
            return invoiceSaved;

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
                    ShowDialog("Error", MessageType.Error, "Error updating the invoice number on the Invoice Miscellaneous Lines.", ex.Message);
                    HFInvoiceSaved.Value = "ERROR";
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        private void RemoveSPFNMiscLines(string ProgramYear, string DistrictID, string InvNum)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string UpdateInvoiceQuery;
            UpdateInvoiceQuery = "DELETE [SPFNInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber]=@InvoiceNumber))";

            using (SqlCommand cmd2 = new SqlCommand(UpdateInvoiceQuery, dbConnection))
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = ProgramYear;
                dbParam_ProgYear2.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_ProgYear2);

                SqlParameter dbParam_DistID2 = new SqlParameter();
                dbParam_DistID2.ParameterName = "@DistrictID";
                dbParam_DistID2.Value = DistrictID;
                dbParam_DistID2.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_DistID2);

                SqlParameter dbParam_InvoiceNum = new SqlParameter();
                dbParam_InvoiceNum.ParameterName = "@InvoiceNumber";
                dbParam_InvoiceNum.Value = InvNum;
                dbParam_InvoiceNum.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_InvoiceNum);
                #endregion

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error while removing Invoice Miscellaneous Lines.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }



        private void UpdatePayAuthInvoiceNum(string ProgramYear, string DistrictID, string OldInvNum, string NewInvNum)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string UpdateInvoiceQuery;
            UpdateInvoiceQuery = "UPDATE [SPFNPaymentAuth] SET [InvoiceNumber]=@NewInvoiceNumber " +
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
                    ShowDialog("Error", MessageType.Error, "Error updating the invoice number on the Payment Authorization.", ex.Message);
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
                //ShowMessageByName("Invoice Saved. ", MessageType.Success, "alert_container_InvoiceHdr", 3000);
                HFInvoiceSaved.Value = "0";
            }
            else if (HFInvoiceSaved.Value == "ERROR")
            {
                ShowMessageByName("Error Saving. ", MessageType.Error, "alert_container_InvoiceHdr", 5000);
            }

            RegisterDOMReadyScript("CallCalcs", "RecalcInvoiceTotal();");
        }

        protected void FormViewSpFnOrg_DataBound(object sender, EventArgs e)
        {

            //Check for the legacy paper/clean/supplies fields to show or hide the new fields
            HtmlGenericControl divLegacyPaperCleanSup = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "divLegacyPaperCleanSup");
            TextBox TextBoxPaperCleanSup = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxPaperCleanSup");
            if(divLegacyPaperCleanSup != null && TextBoxPaperCleanSup!=null)
            {
                Decimal.TryParse(TextBoxPaperCleanSup.Text, out decimal decPaperCleanSuppl);
                if (decPaperCleanSuppl > 0)
                    divLegacyPaperCleanSup.Visible = true;
                else
                    divLegacyPaperCleanSup.Visible = false;
            }

            //Check program year and usage of old fields for new 3086 and CACFP fields
            DateTime.TryParse(HFStartDate.Value, out DateTime PYStartDate);
            DateTime PYMinDate = new DateTime(2024, 07, 1);
            HtmlGenericControl DivPCACFP3086Invoice = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "DivPCACFP3086Invoice");
            HtmlGenericControl DivStandardSpFnInvoice = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "DivStandardSpFnInvoice");
            CheckBox CB3086 = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CB3086");
            CheckBox CBCACFP = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBCACFP");

            if (DivPCACFP3086Invoice != null && DivStandardSpFnInvoice != null && CB3086!= null && CBCACFP!=null)
            { 
                if (PYStartDate < PYMinDate)
                {
                    DivPCACFP3086Invoice.Visible = false;
                    DivStandardSpFnInvoice.Visible = true;
                    FormViewSpFnCosts.Visible = true;
                }
                else//ProgramYear 2024-2025 and forward
                {
                    //need to check to see if data was entered in the old fields
                    TextBox TextBoxFoodCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxFoodCost");
                    TextBox TextBoxMileage = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxMileage");
                    TextBox TextBoxFSDCosts = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxFSDCosts");
                    TextBox TextBoxTNGLabor = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxTNGLabor");
                    TextBox TextBoxDistLabor = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDistLabor");
                    TextBox TextBoxPaperCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxPaperCost");
                    TextBox TextBoxCleaningCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxCleaningCost");
                    TextBox TextBoxSuppliesCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSuppliesCost");
                    if(TextBoxFoodCost!=null && TextBoxPaperCleanSup!=null && TextBoxPaperCost!=null && TextBoxCleaningCost!=null && TextBoxSuppliesCost!=null && TextBoxFSDCosts!=null && TextBoxTNGLabor!=null && TextBoxDistLabor!= null)
                    {
                        Decimal.TryParse(TextBoxFoodCost.Text, out decimal decFoodCost);
                        Decimal.TryParse(TextBoxPaperCleanSup.Text, out decimal decPaperCleanSuppl);
                        Decimal.TryParse(TextBoxPaperCost.Text, out decimal decPaperCost);
                        Decimal.TryParse(TextBoxCleaningCost.Text, out decimal decCleaningCost);
                        Decimal.TryParse(TextBoxSuppliesCost.Text, out decimal decSuppliesCost);
                        Decimal.TryParse(TextBoxFSDCosts.Text, out decimal decFSDCosts);
                        Decimal.TryParse(TextBoxTNGLabor.Text, out decimal decTNGLabor);
                        Decimal.TryParse(TextBoxDistLabor.Text, out decimal decDistLabor);
                        Decimal.TryParse(TextBoxMileage.Text, out decimal decMileage);
                        if((decFoodCost + decPaperCleanSuppl + decPaperCost + decCleaningCost + decSuppliesCost + decFSDCosts + decTNGLabor + decDistLabor + decMileage)>0)
                        {
                            DivPCACFP3086Invoice.Visible = false;
                            DivStandardSpFnInvoice.Visible = true;
                            FormViewSpFnCosts.Visible = true;
                        }
                        else
                        {
                            if (CB3086.Checked || CBCACFP.Checked)
                            {
                                DivPCACFP3086Invoice.Visible = true;
                                DivStandardSpFnInvoice.Visible = false;
                                FormViewSpFnCosts.Visible = false;
                            }
                            else
                            {
                                DivPCACFP3086Invoice.Visible = false;
                                DivStandardSpFnInvoice.Visible = true;
                                FormViewSpFnCosts.Visible = true;
                            }
                        }

                    }



                }
            }
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

        protected void GridViewInvoices_DataBound(object sender, EventArgs e)
        {
            if(GridViewInvoices.Rows.Count > 0 )
            {
                if (HFInvoiceNumber.Value == "")
                {
                    GridViewInvoices.SelectRow(0);
                    GridViewInvoices_SelectedIndexChanged(GridViewInvoices, EventArgs.Empty);
                }
                else
                {
                    for (int i = 0; i < GridViewInvoices.DataKeys.Count; i++)
                    {
                        if (GridViewInvoices.DataKeys[i].Values["InvoiceNumber"].ToString() == HFInvoiceNumber.Value)
                        {
                            GridViewInvoices.SelectRow(i);
                            break;
                        }
                    }
                }
            }
        }

        protected void TextBox_TextChanged(object sender, EventArgs e)
        {
            HFInvoiceSaved.Value = "";

            TextBox txtBox = (TextBox)sender;
            if (txtBox.ID == "TextBoxOrgBilled")
                UpdateFieldsForOrgBilled(txtBox.Text);
        }

        protected void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //Mark the invoice as unsaved
            HFInvoiceSaved.Value = "";

            //Need th keep the food taxable checkbox in sync with the meals taxable checkbox
            CheckBox cb = (CheckBox)sender;
            if (cb.ID == "CBMealsTaxable")
            {
                CheckBox CBFoodCostTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBFoodCostTaxable");
                if(CBFoodCostTaxable != null)
                {
                    CBFoodCostTaxable.Checked = cb.Checked;
                }
            }
            else if(cb.ID == "CBFoodCostTaxable")
            {
                CheckBox CBMealsTaxable = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBMealsTaxable");
                if (CBMealsTaxable != null)
                {
                    CBMealsTaxable.Checked = cb.Checked;
                }
            }
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
                            if (CB3086 != null) { CB3086.Checked = (bool)dr["3086"]; }
                            CheckBox CBCACFP = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBCACFP");
                            if (CBCACFP != null) { CBCACFP.Checked = (bool)dr["CACFP"]; }
                            TextBox TextBoxContactPerson = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxContactPerson");
                            if (TextBoxContactPerson != null) { TextBoxContactPerson.Text = dr["SpFnOrgContact"].ToString(); }
                            TextBox TextBoxDueTo = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDueTo");
                            if (TextBoxDueTo != null) { TextBoxDueTo.Text = dr["SpFnOrgRemitTo"].ToString(); }
                        }
                    }
                    conn.Close();
                }

            }
        }

        protected void SPFN_CheckedChanged(object sender, EventArgs e)
        {
            HFInvoiceSaved.Value = "";

            CheckBox cb = (CheckBox)sender;
            CheckBox CB3086 = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CB3086");
            CheckBox CBCACFP = (CheckBox)FindControlRecursive(FormViewSpFnOrg, "CBCACFP");
            HtmlGenericControl DivPCACFP3086Invoice = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "DivPCACFP3086Invoice");
            HtmlGenericControl DivStandardSpFnInvoice = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "DivStandardSpFnInvoice");

            //only allow one of them to be checked
            if (cb.ID == "CB3086" && cb.Checked)
                CBCACFP.Checked = false;

            if (cb.ID == "CBCACFP" && cb.Checked)
                CB3086.Checked = false;

            if(CB3086.Checked || CBCACFP.Checked)
            {
                DivPCACFP3086Invoice.Visible = true;
                DivStandardSpFnInvoice.Visible = false;
                FormViewSpFnCosts.Visible = false;
                clearSPFNValuesBasedOnInvoiceType(true);
            }
            else
            {
                DivPCACFP3086Invoice.Visible = false;
                DivStandardSpFnInvoice.Visible = true;
                FormViewSpFnCosts.Visible = true;
                clearSPFNValuesBasedOnInvoiceType(false);
            }

        }

        protected void clearSPFNValuesBasedOnInvoiceType(bool PDECACFPInvoiceType)
        {
            if(PDECACFPInvoiceType)
            {
                TextBox TextBoxFoodCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxFoodCost");
                TextBox TextBoxMileage = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxMileage");
                TextBox TextBoxFSDCosts = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxFSDCosts");
                TextBox TextBoxTNGLabor = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxTNGLabor");
                TextBox TextBoxDistLabor = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDistLabor");
                TextBox TextBoxPaperCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxPaperCost");
                TextBox TextBoxCleaningCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxCleaningCost");
                TextBox TextBoxSuppliesCost = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSuppliesCost");
                TextBox TextBoxPaperCleanSup = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxPaperCleanSup");
                if (TextBoxFoodCost != null && TextBoxMileage != null && TextBoxFSDCosts != null && TextBoxTNGLabor != null && TextBoxDistLabor != null && TextBoxPaperCost != null && TextBoxCleaningCost != null && TextBoxSuppliesCost != null && TextBoxPaperCleanSup != null)
                {
                    TextBoxFoodCost.Text = "0.00";
                    TextBoxMileage.Text = "0.00";
                    TextBoxFSDCosts.Text = "0.00";
                    TextBoxTNGLabor.Text = "0.00";
                    TextBoxDistLabor.Text = "0.00";
                    TextBoxPaperCost.Text = "0.00";
                    TextBoxCleaningCost.Text = "0.00";
                    TextBoxSuppliesCost.Text = "0.00";
                    TextBoxPaperCleanSup.Text = "0.00";
                }

                //Reset the miscellaneous lines
                RemoveSPFNMiscLines(DDLProgramYears.SelectedValue, Session["CurrentDistrict"].ToString(), HFInvoiceNumber.Value);

                TextBox TextBoxFoodCostActual = (TextBox)FindControlRecursive(FormViewSpFnCosts, "TextBoxFoodCostActual");
                TextBox TextBoxPaperCostActual = (TextBox)FindControlRecursive(FormViewSpFnCosts, "TextBoxPaperCostActual");
                TextBox TextBoxCleaningCostActual = (TextBox)FindControlRecursive(FormViewSpFnCosts, "TextBoxCleaningCostActual");
                TextBox TextBoxSuppliesCostActual = (TextBox)FindControlRecursive(FormViewSpFnCosts, "TextBoxSuppliesCostActual");
                if(TextBoxFoodCostActual != null && TextBoxPaperCostActual != null && TextBoxCleaningCostActual != null && TextBoxSuppliesCostActual!= null)
                {
                    TextBoxFoodCostActual.Text = "0.00";
                    TextBoxPaperCostActual.Text = "0.00";
                    TextBoxCleaningCostActual.Text = "0.00";
                    TextBoxSuppliesCostActual.Text = "0.00";
                }

            }
            else //Not a PDECACFPInvoiceType - reset those fields to zero
            {
                TextBox TextBoxBreakFoodCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakFoodCPM");
                TextBox TextBoxBreakPaperCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakPaperCPM");
                TextBox TextBoxBreakCleanCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakCleanCPM");
                TextBox TextBoxBreakOtherCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakOtherCPM");
                TextBox TextBoxLunchFoodCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchFoodCPM");
                TextBox TextBoxLunchPaperCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchPaperCPM");
                TextBox TextBoxLunchCleanCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchCleanCPM");
                TextBox TextBoxLunchOtherCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchOtherCPM");
                TextBox TextBoxSnackFoodCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackFoodCPM");
                TextBox TextBoxSnackPaperCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackPaperCPM");
                TextBox TextBoxSnackCleanCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackCleanCPM");
                TextBox TextBoxSnackOtherCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackOtherCPM");
                TextBox TextBoxDinnerFoodCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerFoodCPM");
                TextBox TextBoxDinnerPaperCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerPaperCPM");
                TextBox TextBoxDinnerCleanCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerCleanCPM");
                TextBox TextBoxDinnerOtherCPM = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerOtherCPM");
                TextBox TextBoxBreakRate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakRate");
                TextBox TextBoxBreakCount = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxBreakCount");
                TextBox TextBoxLunchRate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchRate");
                TextBox TextBoxLunchCount = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxLunchCount");
                TextBox TextBoxSnackRate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackRate");
                TextBox TextBoxSnackCount = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxSnackCount");
                TextBox TextBoxDinnerRate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerRate");
                TextBox TextBoxDinnerCount = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TextBoxDinnerCount");
                if (TextBoxBreakFoodCPM == null || TextBoxBreakPaperCPM == null || TextBoxBreakCleanCPM == null || TextBoxBreakOtherCPM == null || TextBoxLunchFoodCPM == null || TextBoxLunchPaperCPM == null ||
                    TextBoxLunchCleanCPM == null || TextBoxLunchOtherCPM == null || TextBoxSnackFoodCPM == null || TextBoxSnackPaperCPM == null || TextBoxSnackCleanCPM == null || TextBoxSnackOtherCPM == null ||
                    TextBoxDinnerFoodCPM == null || TextBoxDinnerPaperCPM == null || TextBoxDinnerCleanCPM == null || TextBoxDinnerOtherCPM == null || TextBoxBreakRate == null || TextBoxBreakCount == null ||
                    TextBoxLunchRate == null || TextBoxLunchCount == null || TextBoxSnackRate == null || TextBoxSnackCount == null || TextBoxDinnerRate == null || TextBoxDinnerCount == null)
                    return;

                TextBoxBreakFoodCPM.Text = "0.00";
                TextBoxBreakPaperCPM.Text = "0.00";
                TextBoxBreakCleanCPM.Text = "0.00";
                TextBoxBreakOtherCPM.Text = "0.00";
                TextBoxLunchFoodCPM.Text = "0.00";
                TextBoxLunchPaperCPM.Text = "0.00";
                TextBoxLunchCleanCPM.Text = "0.00";
                TextBoxLunchOtherCPM.Text = "0.00";
                TextBoxSnackFoodCPM.Text = "0.00";
                TextBoxSnackPaperCPM.Text = "0.00";
                TextBoxSnackCleanCPM.Text = "0.00";
                TextBoxSnackOtherCPM.Text = "0.00";
                TextBoxDinnerFoodCPM.Text = "0.00";
                TextBoxDinnerPaperCPM.Text = "0.00";
                TextBoxDinnerCleanCPM.Text = "0.00";
                TextBoxDinnerOtherCPM.Text = "0.00";
                TextBoxBreakRate.Text = "0.00";
                TextBoxBreakCount.Text = "0";
                TextBoxLunchRate.Text = "0.00";
                TextBoxLunchCount.Text = "0";
                TextBoxSnackRate.Text = "0.00";
                TextBoxSnackCount.Text = "0";
                TextBoxDinnerRate.Text = "0.00";
                TextBoxDinnerCount.Text = "0";
                    

            }

        }

        protected void GridViewMiscLines_DataBound(object sender, EventArgs e)
        {
            GridView GridViewMiscLines = (GridView)sender;
            HiddenField HFShowSalesTax = (HiddenField)FindControlRecursive(FormViewSpFnOrg, "HFShowSalesTax");
            bool boolShowTaxCol = false;
            if (HFShowSalesTax != null && HFShowSalesTax.Value == "True")
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
                if (boolShowTaxCol)
                {
                    divTotalColumnHdr.Attributes.Remove("col-sm-3");
                    divTotalColumnHdr.Attributes.Add("class", "col-sm-2");
                }
                else
                {
                    divTotalColumnHdr.Attributes.Remove("col-sm-2");
                    divTotalColumnHdr.Attributes.Add("class", "col-sm-3");
                }
            }

            foreach(GridViewRow row in GridViewMiscLines.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    HtmlGenericControl divTaxColumnEdit = (HtmlGenericControl)FindControlRecursive(row, "divTaxColumnEdit");
                    HtmlGenericControl divTaxColumnItem = (HtmlGenericControl)FindControlRecursive(row, "divTaxColumnItem");
                    HtmlGenericControl divTotalColumnEdit = (HtmlGenericControl)FindControlRecursive(row, "divTotalColumnEdit");
                    HtmlGenericControl divTotalColumnItem = (HtmlGenericControl)FindControlRecursive(row, "divTotalColumnItem");

                    if (divTaxColumnEdit != null && divTotalColumnEdit != null)
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

                    if (divTaxColumnItem != null && divTotalColumnItem != null)
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
                }
            }


            
            HtmlGenericControl divTaxColumnFooter = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "divTaxColumnFooter");
            HtmlGenericControl divTotalColumnFooter = (HtmlGenericControl)FindControlRecursive(FormViewSpFnOrg, "divTotalColumnFooter");
            if (divTaxColumnFooter != null && divTotalColumnFooter != null)
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

            RegisterDOMReadyScript("show", "$('#myModal').modal('hide');");
            UpdatePanelModal.Update();

            //Normal modal hide was not working. Needed to add the following predecessors to remove the grayed out page
            //RegisterDOMReadyScript("pop", "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$('#myModal').hide();");
            //RegisterDOMReadyScript("pop", "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$('#myModal').modal('hide');");
            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "pop", "$('#myModal').hide();", true);
            //UpdatePanelOverall.Update();
        }

        protected void UpdateReportParams(string[] InvoiceNumbers, bool OnlyPrintMisc)
        {
            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramDistID = new ReportParameter("DistrictID", Session["CurrentDistrict"].ToString());
            ReportParameter paramMiscOnly = new ReportParameter("OnlyPrintMisc", value: OnlyPrintMisc.ToString());//CBPrintOnlyMisc.Checked.ToString()

            ReportParameter paramInvNum = new ReportParameter("InvoiceNumber");
            foreach (string inv in InvoiceNumbers)
            {
                if (string.IsNullOrEmpty(inv) == false)
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

            if (customEmailValidator.IsValid == false)
                return;



            string subject = "Paperwork Web - Special Functions Invoice - " + HFInvoiceNumber.Value.Trim();

            //Email body
            StringBuilder sb = new StringBuilder();
            sb.Append(" * *Automated email from the Paperwork Web Special Functions page. Do not reply to this email.* *\r\n");
            sb.Append("\r\n");
            sb.Append("Attached is Special Functions Invoice "); sb.Append(HFInvoiceNumber.Value.Trim()); sb.Append(" for district: "); sb.Append(Session["CurrentDistrictName"].ToString()); sb.Append(".\r\n");
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


                    try
                    {
                        MailAddress To = new MailAddress(address);
                        if (To.Address.ToLower().Contains("thenutritiongroup.biz") || To.Address.ToLower().Contains("iceburghsolutions.com"))
                            mailMessage.To.Add(To);
                        else
                            illegalAddressesFound = true;
                    }
                    catch (FormatException ex)
                    {
                        ShowDialog("Error", MessageType.Error, "A format error occurred.", ex.Message);
                        illegalAddressesFound = true;
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                        illegalAddressesFound = true;
                    }


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

            if(emailIsValid(ref TextBoxEmailTo)==false)
            {
                customEmailValidator.IsValid = false;
                customEmailValidator.ErrorMessage = "Only thenutritiongroup.biz emails are allowed.";
                TextBoxEmailTo.Focus();
            }

            ////Validate email domains
            //string emailTo = TextBoxEmailTo.Text;
            //String[] addresses;
            //if (emailTo.Contains(","))
            //    addresses = emailTo.Split(',');
            //else
            //    addresses = emailTo.Split(';');

            //bool illegalAddressesFound = false;
            //foreach (string address in addresses)
            //{
            //    if (address.ToLower().Contains("thenutritiongroup.biz") == false && address.ToLower().Contains("iceburghsolutions.com") == false)
            //    {
            //        illegalAddressesFound = true;
            //        TextBoxEmailTo.Text = TextBoxEmailTo.Text.Replace(address, " ");
            //    }
            //}

            //if (illegalAddressesFound)
            //{
            //    customValidator.IsValid = false;
            //    customValidator.ErrorMessage = "Only thenutritiongroup.biz emails are allowed.";
            //    TextBoxEmailTo.Focus();
            //    RegisterDOMReadyScript("show", "$('#myModal').modal('show');");
            //    UpdatePanelModal.Update();
            //}

        }

        protected bool emailIsValid(ref TextBox emailTextBox)
        {
            if(emailTextBox==null || emailTextBox.Text == "")
                return false;

            try
            {
                //Validate email domains
                string emailTo = emailTextBox.Text;
                String[] addresses;
                if (emailTo.Contains(","))
                    addresses = emailTo.Split(',');
                else
                    addresses = emailTo.Split(';');

                bool illegalAddressesFound = false;
                foreach (string address in addresses)
                {
                    //test format
                    MailAddress To = new MailAddress(address);
                    //check domain
                    if (address.ToLower().Contains("thenutritiongroup.biz") == false && address.ToLower().Contains("iceburghsolutions.com") == false)
                    {
                        illegalAddressesFound = true;
                        emailTextBox.Text = emailTextBox.Text.Replace(address, " ");
                    }
                }

                if (illegalAddressesFound)
                    return false;
                else
                    return true;
            }
            catch (FormatException)
            {
                return false;
            }

            

        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;

            //reset the selected invoice
            HFInvoiceNumber.Value = ""; 

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

        #region Printing Procedures

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
            foreach (GridViewRow gvr in GridViewPrintInvoices.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
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

            if (SelectedInvoices != null && SelectedInvoices.Length > 0)
            {
                Session["CurrentInvoice"] = SelectedInvoices;
                Session["CurrentProgramYear"] = DDLProgramYears.SelectedValue;
                Response.Redirect("~/Print/PrintSpecialFuncInvoices.aspx");
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openTab", "window.open('Print/PrintSpecialFuncInvoices.aspx' ,'_blank');", true);
            }

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


        #endregion

        #region PayAuth
        protected void IApproval_Click(object sender, EventArgs e)
        {
            RegisterDOMReadyScript("show", "$('#ModalPayAuthorization').modal('show');");
            FormViewPayAuth.DataBind();
            UpdatePanelModalPayAuthorization.Update();
        }

        protected void ISubmit_Click(object sender, EventArgs e)
        {
            LinkButton LBSubmit = (LinkButton)sender;
            PreventMultipleClicks(ref LBSubmit);

            string KeyPYear = FormViewPayAuth.DataKey["ProgramYear"].ToString();
            string KeyDistID = FormViewPayAuth.DataKey["DistrictID"].ToString();
            string KeyInvoiceNumber = FormViewPayAuth.DataKey["InvoiceNumber"].ToString();
            int KeyPayauthnum = FormViewPayAuth.DataKey["PaymentAuthNumber"] == DBNull.Value ? 0 : (int)FormViewPayAuth.DataKey["PaymentAuthNumber"];

            TextBox TextBoxFSDHourlyRate = (TextBox)FormViewPayAuth.FindControl("TextBoxFSDHourlyRate");
            TextBox TextBoxHours = (TextBox)FormViewPayAuth.FindControl("TextBoxHours");
            HiddenField HFTNGFringePct = (HiddenField)FormViewPayAuth.FindControl("HFTNGFringePct");
            HiddenField HFRMNumber = (HiddenField)FormViewPayAuth.FindControl("HFRMNumber");
            HiddenField HFRMEmail = (HiddenField)FormViewPayAuth.FindControl("HFRMEmail");

            if (TextBoxFSDHourlyRate == null || TextBoxHours == null || HFTNGFringePct == null || HFRMNumber == null || HFRMEmail == null)
                return;

            Decimal.TryParse(TextBoxFSDHourlyRate.Text, out decimal decFSDHourlyRate);
            Decimal.TryParse(TextBoxHours.Text, out decimal decFSDHours);
            Decimal.TryParse(HFTNGFringePct.Value, out decimal decFringePct);
            int.TryParse(HFRMNumber.Value, out int intRMNumber);
            int intNewPayAuthStatus = 1;

            if (decFSDHourlyRate == 0M || decFSDHours == 0M)
                return;

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            // Need to check to see if the record exists, then run the Insert or Update statement
            // If UPDATE - need to check to see if they are allowed to update
            string queryCountAuthRecords = "SELECT COUNT(*) FROM [SPFNPaymentAuth] " +
                                          " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@InvoiceNumber AND PaymentAuthNumber=@PaymentAuthNumber  ";
            int existingRecordCount = 0;

            using (SqlCommand cmd = new SqlCommand(queryCountAuthRecords, dbConnection))
            {

                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = KeyPYear;
                dbParam_ProgYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = KeyDistID;
                dbParam_DistID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = KeyInvoiceNumber;
                dbParam_InvNum.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_PayAuthNum = new SqlParameter();
                dbParam_PayAuthNum.ParameterName = "@PaymentAuthNumber";
                dbParam_PayAuthNum.Value = KeyPayauthnum;
                dbParam_PayAuthNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_PayAuthNum);
                #endregion

                try
                {
                    dbConnection.Open();
                    Object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out existingRecordCount);
                    }

                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Error reading the pay auth table.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error reading the pay auth table.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                    cmd.Dispose();
                }
            }

            string InsertUpdateQuery;
            if (existingRecordCount == 0)
            {
                InsertUpdateQuery = "INSERT INTO [SPFNPaymentAuth] (ProgramYear, DistrictID, InvoiceNumber, PaymentAuthNumber, FSDHourlyRate, Hours, TNGFringe, RMNumber, CreatedUser, CreatedDateTime, PaymentAuthStatus, ModifiedDateTime, ModifiedUser) " +
                                    " VALUES (@ProgramYear, @DistrictID, @InvoiceNumber, @PaymentAuthNumber, @FSDHourlyRate, @Hours, @TNGFringe, @RMNumber, @CreatedUser, GETDATE(), @PaymentAuthStatus, GETDATE(), @ModifiedUser) ";
            }
            else
            {
                InsertUpdateQuery = "UPDATE [SPFNPaymentAuth] SET FSDHourlyRate=@FSDHourlyRate, Hours=@Hours, TNGFringe=@TNGFringe, RMNumber=@RMNumber, ModifiedUser=@ModifiedUser, ModifiedDateTime=GETDATE(), PaymentAuthStatus=@PaymentAuthStatus " +
                                    " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@InvoiceNumber AND PaymentAuthNumber=@PaymentAuthNumber ";
            }

            using (SqlCommand cmd2 = new SqlCommand(InsertUpdateQuery, dbConnection))
            {
                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = KeyPYear;
                dbParam_ProgYear.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = KeyDistID;
                dbParam_DistID.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = KeyInvoiceNumber;
                dbParam_InvNum.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_PayAuthNum = new SqlParameter();
                dbParam_PayAuthNum.ParameterName = "@PaymentAuthNumber";
                dbParam_PayAuthNum.Value = KeyPayauthnum;
                dbParam_PayAuthNum.DbType = System.Data.DbType.Int32;
                cmd2.Parameters.Add(dbParam_PayAuthNum);
                #endregion
                #region other values
                SqlParameter dbParam_FSDRate = new SqlParameter();
                dbParam_FSDRate.ParameterName = "@FSDHourlyRate";
                dbParam_FSDRate.Value = decFSDHourlyRate;
                dbParam_FSDRate.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_FSDRate);

                SqlParameter dbParam_Hours = new SqlParameter();
                dbParam_Hours.ParameterName = "@Hours";
                dbParam_Hours.Value = decFSDHours;
                dbParam_Hours.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_Hours);

                SqlParameter dbParam_FringePct = new SqlParameter();
                dbParam_FringePct.ParameterName = "@TNGFringe";
                dbParam_FringePct.Value = decFringePct;
                dbParam_FringePct.DbType = System.Data.DbType.Decimal;
                cmd2.Parameters.Add(dbParam_FringePct);

                SqlParameter dbParam_RMNum = new SqlParameter();
                dbParam_RMNum.ParameterName = "@RMNumber";
                dbParam_RMNum.Value = intRMNumber;
                dbParam_RMNum.DbType = System.Data.DbType.Int16;
                cmd2.Parameters.Add(dbParam_RMNum);

                SqlParameter dbParam_ModUser = new SqlParameter();
                dbParam_ModUser.ParameterName = "@ModifiedUser";
                dbParam_ModUser.Value = Session["CurrentUser"].ToString();
                dbParam_ModUser.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_ModUser);

                SqlParameter dbParam_CrUser = new SqlParameter();
                dbParam_CrUser.ParameterName = "@CreatedUser";
                dbParam_CrUser.Value = Session["CurrentUser"].ToString();
                dbParam_CrUser.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(dbParam_CrUser);

                SqlParameter dbParam_AuthStatus = new SqlParameter();
                dbParam_AuthStatus.ParameterName = "@PaymentAuthStatus";
                dbParam_AuthStatus.Value = intNewPayAuthStatus;
                dbParam_AuthStatus.DbType = System.Data.DbType.Byte;
                cmd2.Parameters.Add(dbParam_AuthStatus);
                #endregion

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();

                    if (recordcount > 0)
                    {
                        FormViewPayAuth.DataBind();

                        if (HFRMEmail.Value == "")
                            ShowDialog("Error", MessageType.Error, "Could not retrieve RM Email address from setup. Cannot submit authorization request at this time.", "");
                        else
                            SendPayAuthorizationEmail(sender, e);
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Error saving the payment authorization.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error saving the payment authorization.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }

            }

        }

        protected void UpdatePayAuthStatus(string ProgramYear, string DistrictID, string InvoiceNumber, int PayAuthNumber, int PayAuthStatus)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string InsertUpdateQuery;
            InsertUpdateQuery = "UPDATE [SPFNPaymentAuth] SET ModifiedUser=@ModifiedUser, ModifiedDateTime=GETDATE(), PaymentAuthStatus=@PaymentAuthStatus " +
                                " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@InvoiceNumber AND PaymentAuthNumber=@PaymentAuthNumber ";

            using (SqlCommand cmd = new SqlCommand(InsertUpdateQuery, dbConnection))
            {
                #region KeyVals
                //KEY Values
                SqlParameter dbParam_ProgYear = new SqlParameter();
                dbParam_ProgYear.ParameterName = "@ProgramYear";
                dbParam_ProgYear.Value = ProgramYear;
                dbParam_ProgYear.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_ProgYear);

                SqlParameter dbParam_DistID = new SqlParameter();
                dbParam_DistID.ParameterName = "@DistrictID";
                dbParam_DistID.Value = DistrictID;
                dbParam_DistID.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_DistID);

                SqlParameter dbParam_InvNum = new SqlParameter();
                dbParam_InvNum.ParameterName = "@InvoiceNumber";
                dbParam_InvNum.Value = InvoiceNumber;
                dbParam_InvNum.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_InvNum);

                SqlParameter dbParam_PayAuthNum = new SqlParameter();
                dbParam_PayAuthNum.ParameterName = "@PaymentAuthNumber";
                dbParam_PayAuthNum.Value = PayAuthNumber;
                dbParam_PayAuthNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_PayAuthNum);
                #endregion
                #region other values
                SqlParameter dbParam_ModUser = new SqlParameter();
                dbParam_ModUser.ParameterName = "@ModifiedUser";
                dbParam_ModUser.Value = Session["CurrentUser"].ToString();
                dbParam_ModUser.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(dbParam_ModUser);

                SqlParameter dbParam_AuthStatus = new SqlParameter();
                dbParam_AuthStatus.ParameterName = "@PaymentAuthStatus";
                dbParam_AuthStatus.Value = PayAuthStatus;
                dbParam_AuthStatus.DbType = System.Data.DbType.Byte;
                cmd.Parameters.Add(dbParam_AuthStatus);
                #endregion

                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd.ExecuteNonQuery();

                    if (recordcount > 0)
                    {
                        FormViewPayAuth.DataBind();
                        //UpdatePanelModalPayAuthorization.Update();
                    }
                }
                catch (SqlException ex)
                {
                    ShowDialog("Error", MessageType.Error, "SQL Error saving the payment authorization.", ex.Message);
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "Error saving the payment authorization.", ex.Message);
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void UpdateApprovalStatus_Click(object sender, EventArgs e)
        {
            LinkButton lbSender = (LinkButton)sender;
            switch (lbSender.ID)
            {
                case "IApprove":
                    UpdatePayAuthStatus(FormViewPayAuth.DataKey["ProgramYear"].ToString(), FormViewPayAuth.DataKey["DistrictID"].ToString(), FormViewPayAuth.DataKey["InvoiceNumber"].ToString(), (int)FormViewPayAuth.DataKey["PaymentAuthNumber"], 2);
                    TextBox TextBoxFSDHourlyRate = (TextBox)FormViewPayAuth.FindControl("TextBoxFSDHourlyRate");
                    TextBox TextBoxHours = (TextBox)FormViewPayAuth.FindControl("TextBoxHours");
                    HiddenField HFTNGFringePct = (HiddenField)FormViewPayAuth.FindControl("HFTNGFringePct");
                    if (TextBoxFSDHourlyRate != null && TextBoxHours != null)
                    {
                        Decimal.TryParse(TextBoxFSDHourlyRate.Text, out decimal decFSDHourlyRate);
                        Decimal.TryParse(TextBoxHours.Text, out decimal decFSDHours);
                        Decimal decTotalWages = decFSDHourlyRate * decFSDHours;
                        Decimal.TryParse(HFTNGFringePct.Value, out decimal decFringePct);
                        Decimal decTotalFringe = decTotalWages * decFringePct;

                        if(SaveDFNLabor(decTotalWages + decTotalFringe))
                        {
                            FormViewSpFnOrg.DataBind();
                            UpdatePanelInvoices.Update();
                        }

                    }

                    break;
                case "IDeny":
                    UpdatePayAuthStatus(FormViewPayAuth.DataKey["ProgramYear"].ToString(), FormViewPayAuth.DataKey["DistrictID"].ToString(), FormViewPayAuth.DataKey["InvoiceNumber"].ToString(), (int)FormViewPayAuth.DataKey["PaymentAuthNumber"], 3);
                    break;
                default:
                    break;
            }

            FormViewPayAuth.DataBind();
        }

        protected void SendPayAuthorizationEmail(object sender, EventArgs e)
        {
            string emailServer = "";

            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view = (DataView)SqlDataSourceEmailSettings.Select(args);
            DataTable table = view.ToTable();

            emailServer = table.Rows[0]["emailServer"].ToString();
            Boolean emailUseEWS = table.Rows[0]["emailUseEWS"].ToString() == "0" ? false : true;
            Boolean emailUseSSL = table.Rows[0]["emailUseSSL"].ToString() == "0" ? false : true;
            string emailFrom = table.Rows[0]["emailFromAddress"].ToString();
            string emailTo = "";
            HiddenField HFRMEmail = (HiddenField)FormViewPayAuth.FindControl("HFRMEmail");
            if(HFRMEmail == null || string.IsNullOrEmpty(HFRMEmail.Value))
            {
                ShowDialog("Error", MessageType.Error, "Could not retrieve RM Email address from setup. Cannot submit authorization request at this time.", "");
                return;
            }
            else
            {
                emailTo = HFRMEmail.Value;
            }


            int emailPort = (int)table.Rows[0]["emailPort"];
            string emailDomain = table.Rows[0]["emailAuthDomain"].ToString();
            string emailUsername = table.Rows[0]["emailAuthUser"].ToString();
            string emailPassword = table.Rows[0]["emailAuthPassword"].ToString();

            string subject = "Paperwork Web - Special Functions Pay Authorization Request - " + HFInvoiceNumber.Value.Trim();

            //Email body
            StringBuilder sb = new StringBuilder();
            if(isTRAINING())
            {
                sb.Append(" * *TRAINING COMPANY* *\r\n");
            }
            sb.Append("* *Automated email from the Paperwork Web Special Functions page. Do not reply to this email.* *\r\n");
            sb.Append("\r\n");

            sb.Append("A pay authorization request has been submitted for your review. "); sb.Append("\r\n");
            sb.Append("INVOICE #: "); sb.Append(HFInvoiceNumber.Value.Trim()); sb.Append("\r\n"); 
            sb.Append("DISTRICT: ");  sb.Append(Session["CurrentDistrictName"].ToString()); sb.Append(".\r\n");
            sb.Append("SUBMITTED BY:  "); sb.Append(Session["CurrentUser"].ToString()); sb.Append("\r\n");
            sb.Append("\r\n");

            //additional content if available
            TextBox TextBoxFSDHourlyRate = (TextBox)FormViewPayAuth.FindControl("TextBoxFSDHourlyRate");
            TextBox TextBoxHours = (TextBox)FormViewPayAuth.FindControl("TextBoxHours");
            HiddenField HFTNGFringePct = (HiddenField)FormViewPayAuth.FindControl("HFTNGFringePct");
            HiddenField HFRMNumber = (HiddenField)FormViewPayAuth.FindControl("HFRMNumber");

            if (TextBoxFSDHourlyRate != null && TextBoxHours != null && HFTNGFringePct != null)
            {
                Decimal.TryParse(TextBoxFSDHourlyRate.Text, out decimal decFSDHourlyRate);
                Decimal.TryParse(TextBoxHours.Text, out decimal decFSDHours);
                Decimal.TryParse(HFTNGFringePct.Value, out decimal decFringePct);

                sb.Append("DFN HOURLY RATE:  "); sb.Append(decFSDHourlyRate.ToString("C2")); sb.Append("\r\n");
                sb.Append("HOURS:  "); sb.Append(decFSDHours.ToString("F0")); sb.Append("\r\n");
                decimal decTotalWages= decFSDHourlyRate * decFSDHours;
                sb.Append("TOTAL WAGES:  "); sb.Append(decTotalWages.ToString("C2")); sb.Append("\r\n");
                sb.Append("\r\n");
                sb.Append("EMPLOYER PAYROLL %:  "); sb.Append(decFringePct.ToString("P2")); sb.Append("\r\n");
                decimal decTotalFringe = decTotalWages * decFringePct;
                sb.Append("EMPLOYER PR % AMT.:  "); sb.Append(decTotalFringe.ToString("C2")); sb.Append("\r\n");
                sb.Append("\r\n");
                sb.Append("TOTAL WAGES:  "); sb.Append((decTotalFringe + decTotalWages).ToString("C2")); sb.Append("\r\n");

            }


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
                    if (To.Address.ToLower().Contains("thenutritiongroup.biz") || To.Address.ToLower().Contains("iceburghsolutions.com"))
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

                /* Attachment not needed for this email currently 

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

                *************************************/

                try
                {
                    //Check to see if anything was marked and attached
                    //if (msList.Count > 0)
                    //{
                        client.Send(mailMessage);
                    //}

                    ShowMessageIcon("Invoice " + HFInvoiceNumber.Value.Trim() + " Pay Authorization Request Emailed", MessageType.Success);
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
                    //foreach (var bw in bwList)
                    //{
                    //    bw.Dispose();
                    //}
                    //foreach (var ms in msList)
                    //{
                    //    ms.Dispose();
                    //}

                    if (mailMessage != null)
                        mailMessage.Dispose();

                    if (client != null)
                        client.Dispose();
                }

            }

        }
        #endregion

        protected void DDLEMarkupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList DDLEMarkupType = (DropDownList)sender;
            if (DDLEMarkupType == null)
                return;

            DataControlFieldCell cell = (DataControlFieldCell)DDLEMarkupType.Parent;
            if (cell == null)
                return;

            TextBox TextBoxEMarkupPct = (TextBox)cell.FindControl("TextBoxEMarkupPct");
            TextBox TextBoxEMarkupDlr = (TextBox)cell.FindControl("TextBoxEMarkupDlr");
            TextBox TextBoxENoMarkup = (TextBox)cell.FindControl("TextBoxENoMarkup");
            if (TextBoxEMarkupPct == null || TextBoxEMarkupDlr == null || TextBoxENoMarkup == null)
                return;

            switch (DDLEMarkupType.SelectedValue)
            {
                case "0":
                    TextBoxEMarkupPct.Visible = false;
                    TextBoxEMarkupPct.Text = "0.0000";
                    TextBoxEMarkupDlr.Visible = false;
                    TextBoxEMarkupDlr.Text = "0.00";
                    TextBoxENoMarkup.Visible = true;
                    break;
                case "1":
                    TextBoxEMarkupPct.Visible = true;
                    TextBoxEMarkupDlr.Visible = false;
                    TextBoxEMarkupDlr.Text = "0.00";
                    TextBoxENoMarkup.Visible = false;
                    break;
                case "2":
                    TextBoxEMarkupPct.Visible = false;
                    TextBoxEMarkupPct.Text = "0.0000";
                    TextBoxEMarkupDlr.Visible = true;
                    TextBoxENoMarkup.Visible = false;
                    break;
                default:
                    break;
            }
        }

    }
}
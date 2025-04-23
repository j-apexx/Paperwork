using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SpecialFunctionsEntry : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");
            else
                LabelProgramYear.Text = Session["ProgramYear"].ToString();

            if (!IsPostBack)
            {
                VerifyAccess(true);
                if (Session["AccessLevel"] == null)
                    Response.Redirect("~/Default.aspx");

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
            }
            else
            {
                VerifyAccess(false);
            }
        }

        protected void GridViewLines_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid && HFInvoiceLocked.Value=="False")
            {
                if(SqlDataSourceInvLines.Insert()>0)
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

            if(DDLFMarkupType.SelectedValue=="0")
            {
                TextBoxFMarkupPct.Text = "0.00";
                TextBoxFMarkupDlr.Text = "0.00";
            }
            else if(DDLFMarkupType.SelectedValue == "1")
                TextBoxFMarkupDlr.Text = "0.00";
            else if (DDLFMarkupType.SelectedValue == "2")
                TextBoxFMarkupPct.Text = "0.00";

            if (Decimal.TryParse(TextBoxFMiscAmount.Text, out Decimal OutSvcAmt)==false)
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFMarkupPct.Text, out Decimal OutMarkupPct)==false)
            {
                e.Cancel = true; return;
            }

            if (Decimal.TryParse(TextBoxFMarkupDlr.Text, out Decimal OutMarkupDlr)==false)
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

                }
                else
                {
                    row.ToolTip = "Click to select this row.";
                    row.BorderColor = Color.Empty;
                    row.BorderWidth = 0;
                    row.ForeColor = Color.Empty;
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

            if (string.IsNullOrEmpty( InvoiceNumber ))
            {
                RegisterDOMReadyScript("alert message", "alert('The next invoice number could not be retrieved. Process cancelled.');");
                return;
            }

            if (GenerateInvoiceHdr(InvoiceNumber) == false)
            {
                RegisterDOMReadyScript("alert message", "alert('The next invoice header could not be created. Process cancelled.');");
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

            string sqlGenerateInvoiceHdr = "INSERT INTO [SPFNInvoiceHDR] ([ProgramYear],[DistrictID],[InvoiceNumber],[CreatedDate],[CreatedUser],[Locked],[BillingDate],[FunctionDate]) "
                + " VALUES(@ProgramYear, @DistrictID, @InvoiceNumber, @CreatedDate, @CreatedUser, 0, '1/1/1900' , GETDATE() );";

            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmdHdr = new SqlCommand(sqlGenerateInvoiceHdr, dbConnection))
                {
                    SqlParameter dbParam_progYear = new SqlParameter();
                    dbParam_progYear.ParameterName = "@ProgramYear";
                    dbParam_progYear.Value = Session["ProgramYear"].ToString();
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
                    dbParam_ProgYear2.Value = Session["ProgramYear"].ToString();
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
            #endregion

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string UpdateInvoiceQuery;
            UpdateInvoiceQuery = "UPDATE [SPFNInvoiceHDR] SET [InvoiceNumber]=@NewInvoiceNumber, [OrgBilled]=@OrgBilled, OrgContact=@OrgContact, FunctionDate=@FunctionDate," +
                "BillingDate=@BillingDate, OrgRemitTo=@OrgRemitTo, SpecialFunctionDesc=@SpecialFunctionDesc, FoodCost=@FoodCost," +
                "PaperCleanSuppl=@PaperCleanSuppl, Mileage=@Mileage, FSDCosts=@FSDCosts, TNGLabor=@TNGLabor, TNGFringePct=@TNGFringePct," +
                "DistrictLabor=@DistrictLabor, DistrictFringePct=@DistrictFringePct, MarkupType=@MarkupType, MarkupDlr=@MarkupDlr, " +
                "MarkupPct=@MarkupPct, InvoiceNotes=@InvoiceNotes, PrintNotes=@PrintNotes, PONumber=@PONumber, DatePaid=@DatePaid," +
                "Org3086=@Org3086, OrgCACFP=@OrgCACFP, PaymentNumber=@PaymentNumber " +
                "WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InvoiceNumber=@old_InvoiceNumber ";

            using (SqlCommand cmd2 = new SqlCommand(UpdateInvoiceQuery, dbConnection))
            {
                #region KeyVals
                SqlParameter dbParam_ProgYear2 = new SqlParameter();
                dbParam_ProgYear2.ParameterName = "@ProgramYear";
                dbParam_ProgYear2.Value = Session["ProgramYear"].ToString();
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

                #endregion


                int recordcount = 0;
                try
                {
                    dbConnection.Open();
                    recordcount = cmd2.ExecuteNonQuery();
                    if (recordcount > 0 )
                    {
                        // if the invoice number was updated, need to update any misc lines as well
                        if(TextBoxInvoice.Text != HFInvoiceNumber.Value)
                        {
                            UpdateMiscLinesInvoiceNum(Session["ProgramYear"].ToString(), Session["CurrentDistrict"].ToString(), HFInvoiceNumber.Value, TextBoxInvoice.Text);
                            HFInvoiceNumber.Value= TextBoxInvoice.Text;
                        }

                        //Check the misc lines footer for unsaved inserts
                        //if fields are empty, the insert will not happen
                        SqlDataSourceInvLines.Insert();
                    }
                        
                }
                catch (Exception ex)
                {
                    RegisterDOMReadyScript("error message", "alert('Error saving the Invoice.');");
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
                //ShowMessageIcon("Invoice Saved. ", MessageType.Success);
                ShowMessageByName("Invoice Saved. ", MessageType.Success, "alert_container_InvoiceHdr", 3000);
                HFInvoiceSaved.Value = "0";
            }
            else if(HFInvoiceSaved.Value == "ERROR")
            {
                ShowMessageByName("Error Saving. ", MessageType.Error, "alert_container_InvoiceHdr", 5000);
                //ShowMessageIcon("Error Saving Invoice. ", MessageType.Error);
            }

            RegisterDOMReadyScript("CallCalcs", "RecalcInvoiceTotal();");
        }

        protected void FormViewSpFnOrg_DataBound(object sender, EventArgs e)
        {
            //RegisterDOMReadyScript("CallCalcs", "RecalcInvoiceTotal();");
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            SaveInvoiceData();

            Session["CurrentInvoice"] = HFInvoiceNumber.Value;
            Response.Redirect("~/Print/PrintSpecialFuncInvoices.aspx");
        }

        protected void ISave_Click(object sender, EventArgs e)
        {
            SaveInvoiceData();

            //Update the GridView so the columns show the updated values
            GridViewInvoices.DataBind();

            for(int i=0; i< GridViewInvoices.DataKeys.Count; i++)
            {
                if(GridViewInvoices.DataKeys[i].Values["InvoiceNumber"].ToString() == HFInvoiceNumber.Value)
                {
                    GridViewInvoices.SelectRow(i);
                    break;
                }
            }

            //Redraw the outer panel to refresh the datagrid
            UpdatePanel1.Update();


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

            if(invoicefound==false)
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

            //Footer row is used to add misc lines. Hide the footer when the invoice is locked.
            if (HFInvoiceLocked.Value == "True")
                GridViewMiscLines.ShowFooter = false;
            else
                GridViewMiscLines.ShowFooter = true;

        }

        protected void GridViewMiscLines_RowEditing(object sender, GridViewEditEventArgs e)
        {
            RegisterDOMReadyScript("UpdateDropdown", "MarkupShowHideEdit();");
        }
    }
}
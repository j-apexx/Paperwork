using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
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
                    GridViewInvoices.SelectRow(0);
            }
            else
            {
                VerifyAccess(false);
            }
        }

        protected void GridViewLinesA_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                SqlDataSourceInvSubLinesA.Insert();
            }
        }

        protected void GridViewLinesB_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                SqlDataSourceInvSubLinesB.Insert();
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
        protected void SqlDataSourceInvSubLinesA_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null)
                return;

            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            GridView GridViewLinesA = (GridView)FindControlRecursive(FormViewSpFnOrg, "GridViewLinesA");
            TextBox TextBoxFDescription = (TextBox)GridViewLinesA.FooterRow.FindControl("TextBoxFDescription");
            TextBox TextBoxFAmountA = (TextBox)GridViewLinesA.FooterRow.FindControl("TextBoxFAmountA");

            if (string.IsNullOrEmpty(TextBoxFAmountA.Text))
                return;
            else if (Decimal.TryParse(TextBoxFAmountA.Text, out Decimal OutSvcAmt))
                e.Command.Parameters["@ServiceAmount"].Value = OutSvcAmt;
            else
            {
                e.Cancel = true; return;
            }

            if (string.IsNullOrEmpty(TextBoxFDescription.Text))
                return;
            e.Command.Parameters["@ServiceDescription"].Value = TextBoxFDescription.Text;

            if (FormViewSpFnOrg.DataKey.Values["InvoiceNumber"] == null || string.IsNullOrEmpty(FormViewSpFnOrg.DataKey.Values["InvoiceNumber"].ToString()))
                return;
            if (FormViewSpFnOrg.DataKey.Values["InvoiceSuffix"] == null || string.IsNullOrEmpty(FormViewSpFnOrg.DataKey.Values["InvoiceSuffix"].ToString()))
                return;


            e.Command.Parameters["@InvoiceNumber"].Value = FormViewSpFnOrg.DataKey.Values["InvoiceNumber"].ToString();
            e.Command.Parameters["@InvoiceSuffix"].Value = FormViewSpFnOrg.DataKey.Values["InvoiceSuffix"].ToString();
        }

        /// <summary>
        /// Checks and sets parameters for the insert from the footer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SqlDataSourceInvSubLinesB_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            if (e == null)
                return;

            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            GridView GridViewLinesB = (GridView)FindControlRecursive(FormViewSpFnDist, "GridViewLinesB");
            TextBox TextBoxFDescription = (TextBox)GridViewLinesB.FooterRow.FindControl("TextBoxFDescription");
            TextBox TextBoxFAmountB = (TextBox)GridViewLinesB.FooterRow.FindControl("TextBoxFAmountB");

            if (string.IsNullOrEmpty(TextBoxFAmountB.Text))
                return;
            else if (Decimal.TryParse(TextBoxFAmountB.Text, out Decimal OutSvcAmt))
                e.Command.Parameters["@ServiceAmount"].Value = OutSvcAmt;
            else
            {
                e.Cancel = true; return;
            }

            if (string.IsNullOrEmpty(TextBoxFDescription.Text))
                return;
            e.Command.Parameters["@ServiceDescription"].Value = TextBoxFDescription.Text;

            if (FormViewSpFnDist.DataKey.Values["InvoiceNumber"] == null || string.IsNullOrEmpty(FormViewSpFnDist.DataKey.Values["InvoiceNumber"].ToString()))
                return;
            if (FormViewSpFnDist.DataKey.Values["InvoiceSuffix"] == null || string.IsNullOrEmpty(FormViewSpFnDist.DataKey.Values["InvoiceSuffix"].ToString()))
                return;


            e.Command.Parameters["@InvoiceNumber"].Value = FormViewSpFnDist.DataKey.Values["InvoiceNumber"].ToString();
            e.Command.Parameters["@InvoiceSuffix"].Value = FormViewSpFnDist.DataKey.Values["InvoiceSuffix"].ToString();
        }

        protected void SqlDataSourceInvSubLinesB_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //The Gridview insert does not trigger the FormView to DataBind. Calling this updates the gridview and the formview
            FormViewSpFnDist.DataBind();
        }
        protected void SqlDataSourceInvSubLinesA_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //The Gridview insert does not trigger the FormView to DataBind. Calling this updates the gridview and the formview
            FormViewSpFnOrg.DataBind();
        }

        protected void GridViewInvoices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
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

                    //Set Contract ID for the Import Section
                    //hfPrimaryContract.Value = row.Cells[2].Text;

                    //Set Customer ID hidden field (for dropdown contracts import)
                    //HiddenField HFCustomerID = (HiddenField)FindControlRecursive(row, "HFCustomerID");
                    //if (HFCustomerID != null)
                    //    hfCustomerID.Value = HFCustomerID.Value;


                }
                else
                {
                    row.ToolTip = "Click to select this row.";
                    row.BorderColor = Color.Empty;
                    row.BorderWidth = 0;
                    row.ForeColor = Color.Empty;
                }
            }

            if(gv.SelectedIndex != -1)
            {
                //Set the hidden value to the selected invoicenumber then update the displayed invoices
                HFInvoiceNumber.Value = gv.DataKeys[(gv.SelectedIndex + (gv.PageSize * gv.PageIndex))].Values["InvoiceNumber"].ToString();
            }

            //DetailsViewInvoiceDetails.PageIndex = (gv.SelectedIndex + (gv.PageSize * gv.PageIndex));
            //DetailsViewInvoiceDetails.Visible = true;
            ////this wasn't running - force it to update the Gridview
            //DetailsViewInvoiceDetails_PageIndexChanged(DetailsViewInvoiceDetails, EventArgs.Empty);
            //try
            //{
            //    ddlImportContracts.DataBind();
            //    if (ddlImportContracts.Items.FindByValue(hfPrimaryContract.Value) != null)
            //        ddlImportContracts.SelectedValue = hfPrimaryContract.Value;
            //}
            //catch { }

            //UpdatePanelInvoiceDetails.Update();
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

        }

        /// <summary>
        /// Generates the Master Invoice Header record
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        private bool GenerateInvoiceHdr(string invoiceNumber)
        {
            bool invoiceGenerated = false;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;

            string sqlGenerateInvoiceHdr = "INSERT INTO [SPFNInvoiceHDR] ([ProgramYear],[DistrictID],[InvoiceNumber],[CreatedDate],[CreatedUser],[Locked]) "
                + " VALUES(@ProgramYear, @DistrictID, @InvoiceNumber, @CreatedDate, @CreatedUser, 0);"
                //SubInvoice - A (Org)
                + " INSERT INTO [SPFNSubInvoiceHdr] ([ProgramYear],[DistrictID],[InvoiceNumber],[InvoiceSuffix],[OrganizationBilled],[ContactPerson],[FunctionDate],[BillingDate],[DueTo]) "
                + " VALUES (@ProgramYear, @DistrictID, @InvoiceNumber, 'A', '' Org, '' Contact, GETDATE() FuncDate, GETDATE() BillDate, '' DueTo);"
                //SubInvoice - B (District)
                + " INSERT INTO [SPFNSubInvoiceHdr] ([ProgramYear],[DistrictID],[InvoiceNumber],[InvoiceSuffix],[OrganizationBilled],[ContactPerson],[FunctionDate],[BillingDate],[DueTo]) "
                + " VALUES (@ProgramYear, @DistrictID, @InvoiceNumber, 'B', '' Org, '' Contact, GETDATE() FuncDate, GETDATE() BillDate, '' DueTo);";

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
                    dbParam_distID.ParameterName = "@CommissaryID";
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
        protected static string GetNextSpFnInvoiceNumber()
        {
            string InvoiceNumber = "";
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            string GetNextSPFNInvoiceNumber = "GetNextSPFNInvoiceNumber";
            using (SqlConnection dbConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString))
            {
                dbConnection.Open();
                using (SqlCommand cmdSP = new SqlCommand(GetNextSPFNInvoiceNumber, dbConnection))
                {
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


        protected void GridViewInvoices_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            
        }
    }
}
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupContracts : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (Session["ProgramYear"] == null)
                Response.Redirect("~/default.aspx");

            if (!IsPostBack)
            {
                VerifyAccess(true);

                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");
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

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
                Response.Redirect(Request.RawUrl);//self redirect to refresh page
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void SqlDataSourceContracts_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
        {
            for (int i = 0; i < e.ParameterValues.Count; i++)
            {
                if (e.ParameterValues[i] != null)
                    e.ParameterValues[i] = e.ParameterValues[i].ToString().Replace("'", "''").Replace("%", "[%]");
            }
        }

        /// <summary>
        /// Download the file from the Docattach table by clicking on the filename link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinkButtonFileName_Click(object sender, EventArgs e)
        {
            LinkButton clickedButton = (LinkButton)sender;
            string fileName = clickedButton.Text; // Assuming the file name is set as the text of the LinkButton
            GridViewRow gvr = (GridViewRow)FindParentControlOfType(clickedButton, "GridViewRow");
            if (gvr == null) { return; }
            int attachID = (int)GridViewFileAttachments.DataKeys[gvr.RowIndex]["AttachmentID"];

            // Retrieve file data from the database
            byte[] fileData = GetFileAttachment(Session["CurrentDistrict"].ToString(), LabelSelectedContract.Text, fileName, attachID);

            string contentType = GetContentTypeFromExtension(fileName);

            // Send file data to the client for download
            if (fileData != null)
            {
                Response.Clear();
                Response.ContentType = contentType; // Set the appropriate content type (e.g., for PDF files)
                Response.AppendHeader("Content-Disposition", $"attachment; filename={fileName}");
                Response.BinaryWrite(fileData);
                Response.Flush();//added to flush the buffer withing update panel
                Response.End();
            }

            //UpdatePanelAttachments.Update();
        }

        private byte[] GetFileAttachment(string distID, string contractID, string fileName, int attachmentID)
        {
            byte[] fileData = null;
            if (string.IsNullOrEmpty(contractID) || string.IsNullOrEmpty(distID) || string.IsNullOrEmpty(fileName) || attachmentID == 0) { return fileData; }
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT FileData FROM [DistrictContractAttachments] WHERE AttachmentID=@AttachmentID AND DistrictID=@DistrictID AND ContractID=@ContractID";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@DistrictID", distID);
                    command.Parameters.AddWithValue("@ContractID", contractID);
                    command.Parameters.AddWithValue("@AttachmentID", attachmentID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fileData = (byte[])reader["FileData"];
                        }
                    }
                }
            }
            return fileData;
        }

        protected void ButtonUpld_Click(object sender, EventArgs e)
        {
            if (FileUpload1.PostedFiles.Count > 0)
            {
                foreach (HttpPostedFile infile in FileUpload1.PostedFiles)
                {
                    if (infile != null && !string.IsNullOrEmpty(infile.FileName))
                    {
                        try
                        {
                            // Get the file name and data
                            string fileName = infile.FileName;
                            byte[] fileData = new byte[infile.ContentLength];
                            infile.InputStream.Read(fileData, 0, fileData.Length);

                            // Save the file to the database
                            SaveFileToAttachments(DropDownListDistricts.SelectedValue, LabelSelectedContract.Text, fileName, fileData);

                        }
                        catch (InvalidOperationException ex)
                        {
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

            }

            GridViewFileAttachments.DataBind();
            //Ensure the Attach Button is updated
            gvContracts.DataBind();
        }

        private void SaveFileToAttachments(string distID, string contractID, string fileName, byte[] fileData)
        {
            if (string.IsNullOrEmpty(contractID) || string.IsNullOrEmpty(distID) || string.IsNullOrEmpty(fileName) || fileData.Length == 0) { return; }

            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO [DistrictContractAttachments] (DistrictID, ContractID, FileName, FileData) VALUES (@DistrictID, @ContractID, @FileName, @FileData)";
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@DistrictID", distID);
                    command.Parameters.AddWithValue("@ContractID", contractID);
                    command.Parameters.AddWithValue("@FileName", fileName);
                    command.Parameters.AddWithValue("@FileData", fileData);
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void LinkButtonNewContract_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                RegisterDOMReadyScript("show", "$('#NewContractModal').modal('show');");

            UpdatePanel1.Update();
        }

        protected void DetailsViewContractEntry_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (e.Exception != null)
            {
                if (e.Exception.Message.Contains("Violation of PRIMARY KEY constraint"))
                {
                    ShowDialog("Error", MessageType.Error, "A contract with that ID already exists. Please choose a different ID.","");
                }
                else
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred during new contract submission. Details follow:", e.Exception.Message);
                }
                e.ExceptionHandled = true;
            }

        }

        /// <summary>
        /// Opens the modal which allows attachment upload / download for the selected contract
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void IButtonAttach_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)FindParentControlOfType(button, "GridViewRow");
            if (gvr != null)
            {
                LabelSelectedContract.Text = gvContracts.DataKeys[gvr.RowIndex]["ContractID"].ToString();
                GridViewFileAttachments.DataBind();
                gvContracts.SelectedIndex = gvr.RowIndex;
            }

            //UpdatePanelAttachments.Update();

            if (Page.IsValid)
                RegisterDOMReadyScript("modalBox", "$('#myAttachmentModal').modal('show');");
        }

        protected void GridViewFileAttachments_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            //Ensure the docattach button is updated
            gvContracts.DataBind();

        }
    }
}
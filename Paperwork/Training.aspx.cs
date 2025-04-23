using Microsoft.Exchange.WebServices.Data;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
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
    [System.Web.Script.Services.ScriptService]
    public partial class Training : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //if (!IsPostBack)
            //{
            //    // Data binding and other initialization
            //    GridViewTrainings.RowDeleting += GridViewTrainings_RowDeleting;
            //}


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
                // DDLProgramYears_SelectedIndexChanged(DDLProgramYears, EventArgs.Empty);

                //On initial load - select the first invoice
                if (!IsPostBack)
                {
                    GridViewTrainings.DataBind();
                }
                if (GridViewTrainings.SelectedIndex <= 0 && GridViewTrainings.Rows.Count > 0)
                {
                    if (Session["CurrentTraining"] != null && !string.IsNullOrEmpty(Session["CurrentTraining"].ToString()) && Session["CurrentTraining"].ToString().IndexOf(',') == -1)
                    {
                        try
                        {
                            bool TrainingFound = false;
                            for (int page = 0; page < GridViewTrainings.PageCount; page++)
                            {
                                GridViewTrainings.PageIndex = page;
                                GridViewTrainings.DataBind();
                                for (int i = 0; i < GridViewTrainings.DataKeys.Count; i++)
                                {
                                    if (GridViewTrainings.DataKeys[i].Values["TrainingID"].ToString() == Session["CurrentTraining"].ToString())
                                    {
                                        GridViewTrainings.SelectRow(i);
                                        TrainingFound = true;
                                        break;
                                    }
                                }
                                //need to stop the page search - otherwise it will keep going to the last page
                                if (TrainingFound == true)
                                    break;
                            }

                            if (TrainingFound == false)
                            {
                                GridViewTrainings.PageIndex = 0;
                                GridViewTrainings.SelectRow(0);
                            }
                        }
                        catch
                        {
                            GridViewTrainings.SelectRow(0);
                        }
                    }
                    else
                        GridViewTrainings.SelectRow(0);
                }

                UpdatePanelTrainings.Visible = true;
                FormViewSpFnOrg.Visible = true;
                //Initialize the indicator to unsaved
                //HFInvoiceSaved.Value = "0";

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                //JB Temp
                //HFSSRSBaseFolder.DataBind();
                //HFSSRSBaseFolder.Value = SSRSBaseFolder;

                //Uri SSRSUri = new Uri(SSRSURL);
                //ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                //ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                //JB Temp
                //ReportViewer2.ServerReport.ReportServerUrl = SSRSUri;
                //ReportViewer2.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer2.ServerReport.ReportPath;

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

        protected void FormViewSpFnOrg_DataBound(object sender, EventArgs e)
        {
            string test = "test";
        }

        protected void SqlDataSourceTrainHdrsListDeleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                GridViewTrainings.DataBind(); // Refresh the grid
            }
        }

        protected void GridViewTrainings_DataBound(object sender, EventArgs e)
        {
            if (GridViewTrainings.Rows.Count > 0)
            {
                if (HFTrainingID.Value == "")
                {
                    GridViewTrainings.SelectRow(0);
                    GridViewTrainings_SelectedIndexChanged(GridViewTrainings, EventArgs.Empty);
                }
                else
                {
                    for (int i = 0; i < GridViewTrainings.DataKeys.Count; i++)
                    {
                        if (GridViewTrainings.DataKeys[i].Values["TrainingID"].ToString() == HFTrainingID.Value)
                        {
                            GridViewTrainings.SelectRow(i);
                            break;
                        }
                    }
                }
            }
        }
        protected void SqlDataSourceTrainHdrsListDeleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            var programYear = e.Command.Parameters["@ProgramYear"].Value;
            var districtID = e.Command.Parameters["@DistrictID"].Value;
            var trainingID = e.Command.Parameters["@TrainingID"].Value;

            System.Diagnostics.Debug.WriteLine($"ProgramYear: {programYear}, DistrictID: {districtID}, TrainingID: {trainingID}");
        }

        protected void GridViewTrainings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                // Retrieve the row index from CommandArgument
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                // Use DataKeys to get the ID of the item to delete
                string trainingId = GridViewTrainings.DataKeys[rowIndex].Value.ToString();

                // Perform your deletion logic, such as a database operation
                // Ensure the method signature matches the delegate
               

                // Add your deletion logic
                string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM TrainingHDR WHERE TrainingID = @TrainingID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    
                    cmd.Parameters.AddWithValue("@TrainingID", trainingId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Rebind the GridView to reflect changes
                // BindGrid();
            }
        }

        private void DeleteTraining(string trainingId)
        {

        }

        protected void GridViewTrainings_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Ensure the method signature matches the delegate
            int rowIndex = e.RowIndex;
            var programYear = GridViewTrainings.DataKeys[rowIndex]["ProgramYear"];
            var districtID = GridViewTrainings.DataKeys[rowIndex]["DistrictID"];
            var trainingID = GridViewTrainings.DataKeys[rowIndex]["TrainingID"];

            // Add your deletion logic
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM TrainingHDR WHERE ProgramYear = @ProgramYear AND DistrictID = @DistrictID AND TrainingID = @TrainingID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProgramYear", programYear);
                cmd.Parameters.AddWithValue("@DistrictID", districtID);
                cmd.Parameters.AddWithValue("@TrainingID", trainingID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Rebind the GridView to reflect the changes
           // BindGridView();
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
        protected void GridviewClickable_RowDatabound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(
                    GridViewTrainings, "Select$" + e.Row.RowIndex);

                e.Row.Attributes["style"] = "cursor:pointer";
            }
        }

        protected void ISave_Click(object sender, EventArgs e)
        {
            SaveTrainingData();

            //Update the GridView so the columns show the updated values
            GridViewTrainings.DataBind();

            for (int i = 0; i < GridViewTrainings.DataKeys.Count; i++)
            {
                if (GridViewTrainings.DataKeys[i].Values["TrainingID"].ToString() == HFTrainingID.Value)
                {
                    GridViewTrainings.SelectRow(i);
                    break;
                }
            }

            //Redraw the outer panel to refresh the datagrid
          //  UpdatePan.Update();


        }

        private void SaveTrainingData()
        {

            TextBox TxtBxTrainingID = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TxtBxTrainingID");
            TextBox TrainingTitle = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TrainingTitle");
            TextBox TrainingDescription = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TrainingDescription");
            TextBox TBLocation = (TextBox)FindControlRecursive(FormViewSpFnOrg, "Location");
            TextBox TrainerName = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TrainerName");
            TextBox TrainingTime = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TrainingTime");
            TextBox TrainingDate = (TextBox)FindControlRecursive(FormViewSpFnOrg, "TrainingDate");

            if (DateTime.TryParse(TrainingDate.Text, out DateTime DateTraining) == false)
                DateTraining = new DateTime(1900, 01, 01);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string UpdateInvoiceQuery;
            UpdateInvoiceQuery = "UPDATE TrainingHDR SET TrainingTitle=@TrainingTitle, TrainingDescription = @TrainingDescription, " +
                "  Location = @Location, TrainerName = @TrainerName, TrainingTime = @TrainingTime, " +
                "  TrainingDate = " + DateTraining.Date.ToShortDateString() + " "+
                " WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND TrainingID=@TrainingID ";


            using (SqlCommand cmd2 = new SqlCommand(UpdateInvoiceQuery, dbConnection))
            {
                //SqlParameter dbParam_ProgYear2 = new SqlParameter();
                //dbParam_ProgYear2.ParameterName = "@ProgramYear";
                //dbParam_ProgYear2.Value = DDLProgramYears.SelectedValue;
                //dbParam_ProgYear2.DbType = System.Data.DbType.StringFixedLength;
                //cmd2.Parameters.Add(dbParam_ProgYear2);

                //SqlParameter dbParam_DistID2 = new SqlParameter();
                //dbParam_DistID2.ParameterName = "@DistrictID";
                //dbParam_DistID2.Value = Session["CurrentDistrict"].ToString();
                //dbParam_DistID2.DbType = System.Data.DbType.StringFixedLength;
                //cmd2.Parameters.Add(dbParam_DistID2);

                //SqlParameter dbParam_InvoiceNum = new SqlParameter();
                //dbParam_InvoiceNum.ParameterName = "@TrainingID";
                //dbParam_InvoiceNum.Value = HFTrainingID.Value;
                //dbParam_InvoiceNum.DbType = System.Data.DbType.StringFixedLength;
                //cmd2.Parameters.Add(dbParam_InvoiceNum);

                //SqlParameter dbParam_Title = new SqlParameter();
                //dbParam_Title.ParameterName = "@TrainingTitle";
                //dbParam_Title.Value = TrainingTitle.Text;
                //dbParam_Title.DbType = System.Data.DbType.StringFixedLength;
                //cmd2.Parameters.Add(dbParam_Title);

                //SqlParameter dbParam_Desc = new SqlParameter();
                //dbParam_Desc.ParameterName = "@TrainingDescription";
                //dbParam_Desc.Value = TrainingDescription.Text;
                //dbParam_Desc.DbType = System.Data.DbType.StringFixedLength;
                //cmd2.Parameters.Add(dbParam_Desc);

                //if (DateTime.TryParse(TrainingDate.Text, out DateTime DateTraining) == false)
                //    DateTraining = new DateTime(1900, 01, 01);

               
                //SqlParameter dbParam_Date= new SqlParameter();
                //dbParam_Date.ParameterName = "@TrainingDate";
                //dbParam_Date.Value = DateTraining.Date;
                //dbParam_Date.DbType = System.Data.DbType.Date;
                //cmd2.Parameters.Add(dbParam_Date);

                //SqlParameter dbParam_Time = new SqlParameter();
                //dbParam_Time.ParameterName = "@TrainingTime";
                //dbParam_Time.Value = TrainingTitle.Text;
                //dbParam_Time.DbType = System.Data.DbType.Time;
                //cmd2.Parameters.Add(dbParam_Time);

                //SqlParameter dbParam_Name = new SqlParameter();
                //dbParam_Name.ParameterName = "@TrainerName";
                //dbParam_Name.Value = TrainingTitle.Text;
                //dbParam_Name.DbType = System.Data.DbType.StringFixedLength;
                //cmd2.Parameters.Add(dbParam_Name);

                //SqlParameter dbParam_Location = new SqlParameter();
                //dbParam_Location.ParameterName = "@Location";
                //dbParam_Location.Value = TrainingTitle.Text;
                //dbParam_Location.DbType = System.Data.DbType.StringFixedLength;
                //cmd2.Parameters.Add(dbParam_Location);


                try
                {
                    cmd2.CommandText = "UPDATE TrainingHDR SET TrainingDate = CONVERT(DATE, '2025-04-04', 102) WHERE ProgramYear = '2024-2025' AND DistrictID = 'ICE' AND TrainingID = 7";
                    dbConnection.Open();
                    var recordcount = cmd2.ExecuteNonQuery();
                }
                finally
                { }
            }
        }

        protected void GridViewLines_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;
        }


        protected void IDeleteClick(object sender, EventArgs e)
        {
            // Get the LinkButton that triggered the event
            LinkButton deleteButton = (LinkButton)sender;

            // Find the GridViewRow containing the clicked button
            GridViewRow row = (GridViewRow)deleteButton.NamingContainer;

            // Retrieve the row index
            int rowIndex = row.RowIndex;

            // Retrieve DataKey for the row being deleted
            string trainingId = GridViewTrainings.DataKeys[rowIndex].Value.ToString();

            // Perform your deletion logic (e.g., database operation)
            DeleteTraining(trainingId);

            // Rebind the GridView to reflect changes
          
        }

        protected void GridViewTrainings_SelectedIndexChanged(object sender, EventArgs e)
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

                    HFTrainingID.Value = row.Cells[0].Text;
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

        }

        protected void ICreateClick(object sender, EventArgs e)
        {
            //string InvoiceNumber = GetNextSpFnInvoiceNumber();

            //if (string.IsNullOrEmpty(InvoiceNumber))
            //{
            //    ShowDialog("Alert", MessageType.Warning, "The next invoice number could not be retrieved. Process cancelled.", "");
            //    return;
            //}

            //if (GenerateInvoiceHdr(InvoiceNumber) == false)
            //{
            //    ShowDialog("Alert", MessageType.Warning, "The next invoice header could not be created. Process cancelled.", "");
            //    return;
            //}
            //else
            //{
            //    ShowMessageIcon("Invoice " + InvoiceNumber + " Created", MessageType.Success);
            //    GridViewTrainings.DataBind();
            //    if (GridViewTrainings.Rows.Count > 0)
            //    {
            //        GridViewTrainings.SelectRow(0);
            //        GridViewTrainings_SelectedIndexChanged(GridViewTrainings, EventArgs.Empty);
            //    }
            //}

        }

        protected void GridViewTrainings_PageIndexChanged(object sender, EventArgs e)
        {
            GridViewTrainings.DataBind();
            GridViewTrainings.SelectRow(0);
        }


        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;

            //reset the selected invoice
            //  HFInvoiceNumber.Value = "";

            SqlDataSourceProgramYears.DataBind();
            DataView dvSql = (DataView)SqlDataSourceProgramYears.Select(DataSourceSelectArguments.Empty);
            //if (dvSql.Count > 0)
            //{
            //    dvSql.Sort = "ProgramYear";
            //    int recordIndex = dvSql.Find(DDLProgramYears.SelectedValue);
            //    if (recordIndex != -1)
            //    {
            //        DataRowView drvSql = dvSql[recordIndex];
            //        HFStartDate.Value = drvSql["StartDate"].ToString();
            //        HFEndDate.Value = drvSql["EndDate"].ToString();
            //    }
            //    else
            //    {
            //        HFStartDate.Value = "";
            //        HFEndDate.Value = "";
            //    }
            //}
            //else
            //{
            //    //if no program years could be populated - redirect home
            //    Response.Redirect("~/default.aspx");
            //}
        }

        protected void IDelete_Command(object sender, CommandEventArgs e)
        {
            var test = "string";
        }
    }
}
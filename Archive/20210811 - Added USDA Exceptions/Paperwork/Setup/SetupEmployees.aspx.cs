using System;
using System.Data.SqlClient;
using System.Reflection;
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

                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");
            }

        }


        protected void GridViewEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow gvr = e.Row;
            TextBox EditLifeAddDt = (TextBox)FindControlRecursive(GridViewEmployees, "EditLifeAddDt");
            TextBox EditLifeDeleteDt = (TextBox)FindControlRecursive(GridViewEmployees, "EditLifeDeleteDt");

            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;

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
            if(count>0)
                ShowMessageIcon("Updated", MessageType.Success);
        }

        protected void GridViewEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try { 
                    SqlDataSourceEmployees.Insert();
                }
                catch (SqlException ex)
                {
                    if (ex.Message.IndexOf("duplicate key") >= 0)
                    {
                        ShowDialog("SQL Error", MessageType.Error, "Duplicate record.", "");
                    }
                    else
                    {
                        ShowDialog("SQL Error", MessageType.Error, "A database error occurred.", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowDialog("Error", MessageType.Error, "An error occurred.", ex.Message);
                }
            }
        }

        protected void SqlDataSourceEmployees_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Added", MessageType.Success);
                
                //Refocus to allow quicker entry of new records
                TextBox TextBoxFEmployeeNum = (TextBox)FindControlRecursive(GridViewEmployees, "TextBoxFEmployeeNum");
                if (TextBoxFEmployeeNum != null)
                    TextBoxFEmployeeNum.Focus();
            }
            else if (e.Exception != null && e.ExceptionHandled==false)
            {
                if (e.Exception.Message.IndexOf("duplicate") > 0)
                    ShowDialog("Error", MessageType.Error, "Employee could not be created.", "Duplicate found.");
                else
                    ShowDialog("Error", MessageType.Error, "Employee could not be created.", e.Exception.Message);
                
                e.ExceptionHandled = true;
            }
        }

        protected void SqlDataSourceEmployees_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }

        protected void SqlDataSourceEmployees_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //Check to ensure the employee with the specified key is not being used in a Transactions
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            System.Data.SqlClient.SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            string queryString = "SELECT Count(*) FROM [Payroll] WHERE UPPER(DistrictID)=@DistrictID AND EmployeeNum=@EmployeeNum ";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = queryString;
                cmd.Connection = dbConnection;

                SqlParameter dbParam_DistrictID = new SqlParameter();
                dbParam_DistrictID.ParameterName = "@DistrictID";
                dbParam_DistrictID.Value = e.Command.Parameters["@old_DistrictID"].Value;
                dbParam_DistrictID.DbType = System.Data.DbType.StringFixedLength;
                cmd.Parameters.Add(dbParam_DistrictID);

                SqlParameter dbParam_EmpNum = new SqlParameter();
                dbParam_EmpNum.ParameterName = "@EmployeeNum";
                dbParam_EmpNum.Value = e.Command.Parameters["@old_EmployeeNum"].Value;
                dbParam_EmpNum.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(dbParam_EmpNum);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object paycodeUseCount = cmd.ExecuteScalar();
                    if (paycodeUseCount != null && (int)paycodeUseCount > 0)
                    {
                        e.Cancel = true;
                        ShowMessageIcon("Cannot Delete - records exist.", MessageType.Error);
                    }
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                }
                finally
                {
                    dbConnection.Close();
                }
            }
        }

        protected void SqlDataSourceEmployees_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District is already set
            TextBox TextBoxFEmployeeNum = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFEmployeeNum");
            TextBox TextBoxFLastName = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFLastName");
            TextBox TextBoxFFirstName = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFFirstName");
            TextBox TextBoxFMiddleName = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFMiddleName");
            DropDownList DropDownListFSchools = (DropDownList)GridViewEmployees.FooterRow.FindControl("DropDownListFSchools");
            TextBox TextBoxFRate1 = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFRate1");
            TextBox TextBoxFRate2 = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFRate2");
            TextBox TextBoxFRate3 = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFRate3");
            TextBox TextBoxFRate4 = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFRate4");
            TextBox TextBoxFRate5 = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFRate5");
            TextBox TextBoxFHealth = (TextBox)GridViewEmployees.FooterRow.FindControl("TextBoxFHealth");

            if (Int32.TryParse(TextBoxFEmployeeNum.Text, out Int32 EmpNum))
                e.Command.Parameters["@EmployeeNum"].Value = EmpNum;
            else
            {
                e.Cancel = true; return;
            }

            if (TextBoxFLastName.Text == String.Empty)
            {
                e.Cancel = true; return;
            }
            e.Command.Parameters["@LastName"].Value = TextBoxFLastName.Text;
            e.Command.Parameters["@FirstName"].Value = TextBoxFFirstName.Text;
            e.Command.Parameters["@MiddleName"].Value = TextBoxFMiddleName.Text;

            if (DropDownListFSchools == null || Int16.TryParse(DropDownListFSchools.SelectedValue, out Int16 schNum) == false)
            {
                e.Cancel = true; return;
            }
            e.Command.Parameters["@SchoolNum"].Value = schNum;

            Decimal rate1 = 0;Decimal rate2 = 0;Decimal rate3 = 0;Decimal rate4 = 0;Decimal rate5 = 0;Decimal health = 0M;

            if (TextBoxFRate1 != null)
            {
                Decimal.TryParse(TextBoxFRate1.Text, out rate1);
            }
            e.Command.Parameters["@Rate1"].Value = rate1;

            if (TextBoxFRate2 != null)
            {
                Decimal.TryParse(TextBoxFRate2.Text, out rate2);
            }
            e.Command.Parameters["@Rate2"].Value = rate2;

            if (TextBoxFRate3 != null)
            {
                Decimal.TryParse(TextBoxFRate3.Text, out rate3);
            }
            e.Command.Parameters["@Rate3"].Value = rate3;

            if (TextBoxFRate4 != null)
            {
                Decimal.TryParse(TextBoxFRate4.Text, out rate4);
            }
            e.Command.Parameters["@Rate4"].Value = rate4;

            if (TextBoxFRate5 != null)
            {
                Decimal.TryParse(TextBoxFRate5.Text, out rate5);
            }
            e.Command.Parameters["@Rate5"].Value = rate5;

            if (TextBoxFHealth != null)
            {
                Decimal.TryParse(TextBoxFHealth.Text, out health);
            }
            e.Command.Parameters["@Health"].Value = health;

        }
    }
}
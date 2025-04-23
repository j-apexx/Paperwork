using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SetupMilkDelivery : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

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

                HFDistrictState.Value = GetDistrictState(DropDownListDistricts.SelectedValue);
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
                Response.Redirect("~/Setup/SetupMilkDelivery.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void GridViewMilkDeliv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Since we are using the footer to insert records - we capture the RowCommand call and if the command is Insert - 
            // we tell the SqlDataSource to run the insert command.
            if (e.CommandName == "Insert" && Page.IsValid)
            {
                try
                {
                    SqlDataSourceMilkDeliv.Insert();
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

        protected void GridViewMilkDeliv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check for the existence of my dummy row and hide it.
            if (e.Row.RowType == DataControlRowType.DataRow && ((GridView)sender).DataKeys[e.Row.DataItemIndex] != null && ((GridView)sender).DataKeys[e.Row.DataItemIndex].Values["DistrictID"].ToString() == "")
                e.Row.Visible = false;


        }

        protected void SqlDataSourceMilkDeliv_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
            {
                ShowMessageIcon("Saved", MessageType.Success);

                //Refocus to allow quicker entry of new records
                DropDownList DDLFSchools = (DropDownList)FindControlRecursive(GridViewMilkDeliv, "DDLFSchools");
                if (DDLFSchools != null)
                    DDLFSchools.Focus();
            }
            else if (e.Exception != null)
            {
                ShowDialog("Error", MessageType.Error, "Error inserting record.", e.Exception.Message);
            }
        }

        protected void SqlDataSourceMilkDeliv_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                ShowMessageIcon("Deleted", MessageType.Success);
        }

        protected void SqlDataSourceMilkDeliv_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // Due to  using the footer for inserts - we need to map the values before the insert can happen
            // District and ProgramYear are already set
            DropDownList DDLFSchools = (DropDownList)GridViewMilkDeliv.FooterRow.FindControl("DDLFSchools");
            //TextBox TextBoxFMMBArea = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFMMBArea");
            DropDownList DDLFMMBArea = (DropDownList)GridViewMilkDeliv.FooterRow.FindControl("DDLFMMBArea");

            TextBox TextBoxFMonday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFMonday");
            TextBox TextBoxFTuesday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFTuesday");
            TextBox TextBoxFWednesday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFWednesday");
            TextBox TextBoxFThursday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFThursday");
            TextBox TextBoxFFriday = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFFriday");
            TextBox TextBoxFJuiceCases = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFJuiceCases");
            TextBox TextBoxFTeaCases = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFTeaCases");
            CheckBox CBFWalkinCooler = (CheckBox)GridViewMilkDeliv.FooterRow.FindControl("CBFWalkinCooler");
            TextBox TextBoxFEightCaseCoolers = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFEightCaseCoolers");
            TextBox TextBoxFTwelveCaseCoolers = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFTwelveCaseCoolers");
            TextBox TextBoxFSixteenCaseCoolers = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFSixteenCaseCoolers");
            //TextBox TextBoxFPALicense = (TextBox)GridViewMilkDeliv.FooterRow.FindControl("TextBoxFPALicense");
            DropDownList DDLFPALicense = (DropDownList)GridViewMilkDeliv.FooterRow.FindControl("DDLFPALicense");

            DropDownList DDLFVendors = (DropDownList)GridViewMilkDeliv.FooterRow.FindControl("DDLFVendors");
            CheckBox CBFMMBSheet = (CheckBox)GridViewMilkDeliv.FooterRow.FindControl("CBFMMBSheet");

            if (Int16.TryParse(DDLFSchools.SelectedValue, out Int16 OutSchoolNum))
                e.Command.Parameters["@SchoolNum"].Value = OutSchoolNum;
            else
            {
                e.Cancel = true; return;
            }

            if(HFDistrictState.Value == "PA" && DDLFMMBArea!=null)
            {
                e.Command.Parameters["@MMBArea"].Value = DDLFMMBArea.SelectedValue;
            }
            else
            {
                e.Command.Parameters["@MMBArea"].Value = string.Empty;
            }

            if(HFDistrictState.Value == "PA" && DDLFPALicense!=null)
            {
                e.Command.Parameters["@PALicense"].Value = DDLFPALicense.SelectedValue;
            }
            else
            {
                e.Command.Parameters["@PALicense"].Value = string.Empty;
            }

            Decimal.TryParse(TextBoxFMonday.Text, out Decimal OutMon);
            e.Command.Parameters["@Monday"].Value = OutMon;
            Decimal.TryParse(TextBoxFTuesday.Text, out Decimal OutTue);
            e.Command.Parameters["@Tuesday"].Value = OutTue;
            Decimal.TryParse(TextBoxFWednesday.Text, out Decimal OutWed);
            e.Command.Parameters["@Wednesday"].Value = OutWed;
            Decimal.TryParse(TextBoxFThursday.Text, out Decimal OutThu);
            e.Command.Parameters["@Thursday"].Value = OutThu;
            Decimal.TryParse(TextBoxFFriday.Text, out Decimal OutFri);
            e.Command.Parameters["@Friday"].Value = OutFri;
            Int16.TryParse(TextBoxFJuiceCases.Text, out Int16 OutJuice);
            e.Command.Parameters["@JuiceCases"].Value = OutJuice;
            Int16.TryParse(TextBoxFTeaCases.Text, out Int16 OutTea);
            e.Command.Parameters["@TeaCases"].Value = OutTea;

            e.Command.Parameters["@WalkInCooler"].Value = CBFWalkinCooler.Checked ? (byte)1 : (byte)0;

            Int16.TryParse(TextBoxFEightCaseCoolers.Text, out Int16 Out8Cooler);
            e.Command.Parameters["@EightCaseCoolers"].Value = Out8Cooler;
            Int16.TryParse(TextBoxFTwelveCaseCoolers.Text, out Int16 Out12Cooler);
            e.Command.Parameters["@TwelveCaseCoolers"].Value = Out12Cooler;
            Int16.TryParse(TextBoxFSixteenCaseCoolers.Text, out Int16 Out16Cooler);
            e.Command.Parameters["@SixteenCaseCoolers"].Value = Out16Cooler;

            e.Command.Parameters["@PALicense"].Value = DDLFPALicense.SelectedValue;

            e.Command.Parameters["@VendorID"].Value = DDLFVendors.SelectedValue;

            e.Command.Parameters["@MMBSheet"].Value = CBFMMBSheet.Checked ? (byte)1 : (byte)0;
        }


        protected void GridViewMilkDeliv_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            DropDownList DDLVend = (DropDownList)FindControlRecursive(GridViewMilkDeliv, "DDLEVendors");
            e.NewValues["VendorID"] = DDLVend.SelectedValue;
            DropDownList DDLMMBArea = (DropDownList)FindControlRecursive(GridViewMilkDeliv, "DDLEMMBArea");
            e.NewValues["MMBArea"] = DDLMMBArea.SelectedValue;
            DropDownList DDLPALicense = (DropDownList)FindControlRecursive(GridViewMilkDeliv, "DDLEPALicense");
            e.NewValues["PALicense"] = DDLPALicense.SelectedValue;
        }

        protected void IPrint_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Print/PrintMilkDeliveryInfoDist.aspx");
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;

        }

        protected string GetDistrictState(string DistrictID)
        {
            string OutDistState = "";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT DistrictState FROM District WHERE DistrictID=@DistrictID";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_distID = new SqlParameter();
                dbParam_distID.ParameterName = "@DistrictID";
                dbParam_distID.Value = DistrictID;
                dbParam_distID.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_distID);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object objState = dbCommand.ExecuteScalar();
                    if (objState != null)
                        OutDistState = ((string)objState).ToUpper();
                }
                catch (Exception)
                {
                    OutDistState = "";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return OutDistState;
        }

        protected void GridViewMilkDeliv_Load(object sender, EventArgs e)
        {
            if(HFDistrictState.Value == "PA")
            {
                GridViewMilkDeliv.Columns[2].Visible = true;//MMBArea
                GridViewMilkDeliv.Columns[14].Visible = true;//License
            }
            else
            {
                GridViewMilkDeliv.Columns[2].Visible = false;//MMBArea
                GridViewMilkDeliv.Columns[14].Visible = false;//License
            }
        }
        #region DDLDataBinding
        protected void DDLVendors_DataBound(object sender, EventArgs e)
        {
            DropDownList DDLVend = (DropDownList)sender;
            if (DDLVend == null)
                return;

            DataControlFieldCell cell = (DataControlFieldCell)DDLVend.Parent;
            if (cell == null)
                return;

            HiddenField HFSelectedVendorID = (HiddenField)FindControlRecursive(cell, "HFVendorID");
            HiddenField HFVendorName = (HiddenField)FindControlRecursive(cell, "HFVendName");
            Label LabelInactiveNote = (Label)FindControlRecursive(cell, "LabelInactiveNote");

            if (LabelInactiveNote == null || HFSelectedVendorID == null || HFVendorName == null)
                return;

            HFSelectedVendorID.DataBind();
            HFVendorName.DataBind();

            //Only add vendor if missing for the Item or Edit Item version, not the footer for new records.
            if (DDLVend.ID == "DDLEVendors" || DDLVend.ID == "DDLVendors")
            {
                if (HFSelectedVendorID.Value != string.Empty)
                {
                    if (DDLVend.Items.FindByValue(HFSelectedVendorID.Value) == null)
                    {
                        //add the item to the list
                        DDLVend.Items.Add(new ListItem("*" + HFVendorName.Value, HFSelectedVendorID.Value));

                        if (LabelInactiveNote != null)
                            LabelInactiveNote.Visible = true;
                    }
                }

                DDLVend.SelectedValue = HFSelectedVendorID.Value;

            }
        }


        protected void DDLMMBArea_DataBound(object sender, EventArgs e)
        {
            DropDownList DDLMMBArea = (DropDownList)sender;
            if (DDLMMBArea == null)
                return;

            DataControlFieldCell cell = (DataControlFieldCell)DDLMMBArea.Parent;
            if (cell == null)
                return;

            HiddenField HFMMBArea = (HiddenField)FindControlRecursive(cell, "HFMMBArea");

            if (HFMMBArea == null)
                return;

            HFMMBArea.DataBind();

            //Only add vendor if missing for the Item or Edit Item version, not the footer for new records.
            if (HFMMBArea.Value != string.Empty)
            {
                if (DDLMMBArea.Items.FindByValue(HFMMBArea.Value) == null)
                {
                    //add the item to the list
                    DDLMMBArea.Items.Add(new ListItem("*" + HFMMBArea.Value, HFMMBArea.Value));

                }
            }

            DDLMMBArea.SelectedValue = HFMMBArea.Value;
        }

        protected void DDLPALicense_DataBound(object sender, EventArgs e)
        {
            DropDownList DDLPALicense = (DropDownList)sender;
            if (DDLPALicense == null)
                return;

            DataControlFieldCell cell = (DataControlFieldCell)DDLPALicense.Parent;
            if (cell == null)
                return;

            HiddenField HFPALicense = (HiddenField)FindControlRecursive(cell, "HFPALicense");

            if (HFPALicense == null)
                return;

            HFPALicense.DataBind();

            //Only add vendor if missing for the Item or Edit Item version, not the footer for new records.
            if (HFPALicense.Value != string.Empty)
            {
                if (DDLPALicense.Items.FindByValue(HFPALicense.Value) == null)
                {
                    //add the item to the list
                    DDLPALicense.Items.Add(new ListItem("*" + HFPALicense.Value, HFPALicense.Value));

                }
            }

            DDLPALicense.SelectedValue = HFPALicense.Value;
        }
        #endregion
    }
}
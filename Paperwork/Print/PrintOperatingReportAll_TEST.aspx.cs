using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintOperatingReportAll_TEST : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            //ProgramYear not set
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/Default.aspx");

            //District Session ID not set
            if (Session["CurrentDistrict"] == null || string.IsNullOrEmpty(Session["CurrentDistrict"].ToString()))
                Response.Redirect("~/Default.aspx");



            SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
            CheckBoxListDists.DataSource = SqlDataSourceMyDistsMstr;
            SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;


            if (!IsPostBack)
            {
                //Check for custom security
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                bool accessExplicitlyGranted = false;
                bool accessExplicitlyDenied = false;
                CheckCustomAccess(className, ref accessExplicitlyGranted, ref accessExplicitlyDenied);

                if (accessExplicitlyDenied == true)
                    Response.Redirect("~/Default.aspx");

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

                TextBoxStartDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1).ToShortDateString();
                TextBoxEndDate.Text = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1)).ToShortDateString();

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                if(isSFSP())
                    ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath + " SFSP";
                else
                    ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                DDLProgramYears_SelectedIndexChanged(null, null);
            }
            else
            {
                Page.Validate();
            }
        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (DDLProgramYears.Items.Count == 0)
                e.Command.Parameters["@ProgramYear"].Value = "";
            else
                e.Command.Parameters["@ProgramYear"].Value = DDLProgramYears.SelectedValue;
        }

        protected void UpdateReportParams()
        {

            //prevent invalid dates from crashing the report
            if (DateTime.TryParse(TextBoxStartDate.Text, out DateTime testDate) == false)
                return;
            if (DateTime.TryParse(TextBoxEndDate.Text, out DateTime testDate2) == false)
                return;

            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramStartDate = new ReportParameter("StartingDate", TextBoxStartDate.Text);
            ReportParameter paramEndDate = new ReportParameter("EndingDate", TextBoxEndDate.Text);

            ReportParameter paramDistID = new ReportParameter("DistrictID");
            foreach (ListItem item in CheckBoxListDists.Items)
            {
                if(item.Selected && item.Value!="%")
                    paramDistID.Values.Add(item.Value);
            }


            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramDistID, paramStartDate, paramEndDate });
                ReportViewer1.ServerReport.Refresh();
            }
            catch { }

            ReportViewer1.ShowPrintButton = true;
            //ReportViewer1.ShowParameterPrompts = true;
        }

        //When month is changed - reopen the calendar
        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            TextBox tbDate = (TextBox)sender;
            NormalizeSimplifiedDate(ref tbDate);
            this.Validate();

            UpdateReportParams();
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;

            //stored selections
            string[] storedDists = { };
            foreach (ListItem item in CheckBoxListDists.Items)
            {
                if (item.Selected && item.Value != "%")
                {
                    Array.Resize(ref storedDists, storedDists.Length + 1);
                    storedDists[storedDists.Length-1] = item.Value;
                }
            }

            //Reset the checkboxlist
            CheckBoxListDists.Items.Clear();
            ListItem li_init= new ListItem("-All Districts -","%");
            CheckBoxListDists.Items.Add(li_init);


            //CheckBoxListDists List is Year Based and needs to be rebound
            CheckBoxListDists.DataBind();

            if (CheckBoxListDists.Items.Count == 1)
                Response.Redirect("~/default.aspx");
            else if (CheckBoxListDists.Items.Count <= 5)
            {
                divCbList.Style.Remove("height");
                divCbList.Style.Add("height", "25px;");
            }
            else if (CheckBoxListDists.Items.Count <= 10)
            {
                divCbList.Style.Remove("height");
                divCbList.Style.Add("height", "50px;");
            }
            else if (CheckBoxListDists.Items.Count <= 15)
            {
                divCbList.Style.Remove("height");
                divCbList.Style.Add("height", "75px;");
            }
            else if (CheckBoxListDists.Items.Count <= 20)
            {
                divCbList.Style.Remove("height");
                divCbList.Style.Add("height", "100px;");
            }
            else 
            {
                divCbList.Style.Remove("height");
                divCbList.Style.Add("height", "150px;");
            }

            if (Session["CurrentDistrict"] != null)
            {
                if(CheckBoxListDists.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                    CheckBoxListDists.SelectedValue = Session["CurrentDistrict"].ToString();
            }

            if(HFCheckSelectAllState.Value == "true")
            {
                foreach (ListItem item in CheckBoxListDists.Items)
                {
                    item.Selected = true;
                }
            }
            else if (storedDists != null && storedDists.Length > 0 )
            {
                foreach (string dist in storedDists)
                {
                    if (CheckBoxListDists.Items.FindByValue(dist) != null)
                    {
                        ListItem li_found = CheckBoxListDists.Items.FindByValue(dist);
                        li_found.Selected= true;
                    }
                        
                }

            }

            UpdateReportParams();
        }

        protected void CheckBoxListDists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckBoxListDists == null || CheckBoxListDists.Items.Count == 0 || CheckBoxListDists.SelectedIndex==-1)
                return;

            if (CheckBoxListDists.Items[CheckBoxListDists.SelectedIndex].Value == "%" && (HFCheckSelectAllState.Value == "false" || HFCheckSelectAllState.Value == ""))
            {
                foreach (ListItem item in CheckBoxListDists.Items)
                {
                    item.Selected = true;
                }
                HFCheckSelectAllState.Value = "true";
            }
            else
            {
                //if hidden field was true but select all is false - select all was unchecked. Uncheck all fields
                if (CheckBoxListDists.Items[0].Value=="%" && CheckBoxListDists.Items[0].Selected==false && HFCheckSelectAllState.Value == "true")
                {
                    foreach (ListItem item in CheckBoxListDists.Items)
                    {
                        item.Selected = false;
                    }
                    HFCheckSelectAllState.Value = "false";
                }

                bool AllDistsChecked = true;
                foreach (ListItem item in CheckBoxListDists.Items)
                {
                    if (item.Value == "%")
                        continue;

                    if (item.Selected == false)
                    {
                        AllDistsChecked = false;
                        break;
                    }
                }

                CheckBoxListDists.Items[0].Selected = AllDistsChecked;
                HFCheckSelectAllState.Value = AllDistsChecked ? "true" : "false";

            }
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class _Default : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            SqlDataSourceInvYear.DataBind();
            DataView dvSql = (DataView)SqlDataSourceInvYear.Select(DataSourceSelectArguments.Empty);
            if (dvSql.Count > 0)
            {
                DataRowView drvSql = dvSql[0];
                LabelInventoryYear.Text = drvSql["ProgramYear"].ToString().TrimEnd(' ');
                Session["ProgramYear"] = LabelInventoryYear.Text.TrimEnd(' ');
                Session["ProgramYearStartDate"] = (DateTime)drvSql["StartDate"];
                Session["ProgramYearEndDate"] = (DateTime)drvSql["EndDate"];
                Session["BudgetYear"] = drvSql["BudgetYear"].ToString().TrimEnd(' ');
            }
            else
            {
                LabelInventoryYear.Text = "**Not Open**";
                Session.Remove("ProgramYear");
                Session.Remove("ProgramYearStartDate");
                Session.Remove("ProgramYearEndDate");
                Session.Remove("BudgetYear");
            }

            if (!IsPostBack)
            {
                //Using BaseFolder to check for training company
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"].ToUpper();

                if(SSRSBaseFolder!=null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING")>0)
                {
                    LabelTrainIndicator.Text = "Training";
                    LiteralTrainingMessage.Visible = true;

                    //using Session variable to popup message only on first access
                    if (Session["CurrentDistrict"] == null)
                        ShowDialog("--Training Only--", MessageType.Error, "** Do not use this site to enter live data! **", "");
                }



                SqlDataSource SqlDataSourceMyDistsMstr = this.Master.SQLMasterDistrictsPY;
                DropDownListDistricts.DataSource = SqlDataSourceMyDistsMstr;
                SqlDataSourceMyDistsMstr.Selecting += SqlDataSourceMyDistsMstr_Selecting;

                DropDownListDistricts.DataBind();

                if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                {
                    string lastDist = GetLastDistrictID();
                    if (lastDist != String.Empty)
                    {
                        ListItem li = DropDownListDistricts.Items.FindByValue(lastDist);
                        if (li != null)
                            li.Selected = true;
                    }

                    DropDownListDistricts_SelectedIndexChanged(null, null);
                }
                else
                {
                    if (DropDownListDistricts.Items.FindByValue(Session["CurrentDistrict"].ToString()) != null)
                        DropDownListDistricts.SelectedValue = Session["CurrentDistrict"].ToString();
                    else
                        DropDownListDistricts_SelectedIndexChanged(null, null);
                }
            }

            if (Session["AccessLevel"] != null)
            {

                switch(Session["AccessLevel"].ToString().ToUpper())
                {
                    case "ADMIN":
                        DivAdminSetup.Visible = true;
                        DivAdminOfficeUtilsSub.Visible = true;
                        DivUserOnlyUtils.Visible = false;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                        DivMgmtReports.Visible = true;
                        LinkOpStatsReport.Visible = true;
                        LinkRMEOMReport.Visible = true;
                        linkPrintNotifications.Visible = true;
                        LinkInvDistTotals.Visible = true;
                        LinkPurchSummary.Visible = true;
                        LinkUsageSummary.Visible = true;
                        LinkUSDACPM.Visible = true;
                        //Old style
                        //DivAdminSetup.Style.Add("display", "inherit");
                        break;
                    case "OFFICE":
                        DivAdminSetup.Visible = false;
                        DivAdminOfficeUtilsSub.Visible = true;
                        DivUserOnlyUtils.Visible = false;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                        DivMgmtReports.Visible = true;
                        LinkOpStatsReport.Visible = true;
                        LinkRMEOMReport.Visible = true;
                        linkPrintNotifications.Visible = true;
                        LinkInvDistTotals.Visible = true;
                        LinkPurchSummary.Visible = true;
                        LinkUsageSummary.Visible = true;
                        LinkUSDACPM.Visible = true;
                        break;
                    case "RM":
                        DivAdminSetup.Visible = false;
                        DivAdminOfficeUtilsSub.Visible = false;
                        DivUserOnlyUtils.Visible = true;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                        DivMgmtReports.Visible = true;
                        LinkOpStatsReport.Visible = true;
                        LinkRMEOMReport.Visible = true;
                        linkPrintNotifications.Visible = true;
                        LinkInvDistTotals.Visible = true;
                        LinkPurchSummary.Visible = false;
                        LinkUsageSummary.Visible = false;
                        LinkUSDACPM.Visible = false;
                        break;
                    default:
                        DivAdminSetup.Visible = false;
                        DivAdminOfficeUtilsSub.Visible = false;
                        DivUserOnlyUtils.Visible = true;//Segregating these since they are not under submenus. Admin/office have their own duplicate
                        DivMgmtReports.Visible = false;
                        LinkOpStatsReport.Visible = false;
                        LinkRMEOMReport.Visible = false;
                        linkPrintNotifications.Visible = false;
                        LinkInvDistTotals.Visible = false;
                        LinkPurchSummary.Visible = false;
                        LinkUsageSummary.Visible = false;
                        LinkUSDACPM.Visible = false;
                        break;

                }

            }


        }

        private void SqlDataSourceMyDistsMstr_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (Session["ProgramYear"] == null)
                e.Command.Parameters["@ProgramYear"].Value = "";
            else
                e.Command.Parameters["@ProgramYear"].Value = Session["ProgramYear"].ToString();
        }

        protected void DropDownListDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListDistricts.Items.Count > 0)
            {
                Session["CurrentDistrict"] = DropDownListDistricts.SelectedValue.ToString();
                Session["CurrentDistrictName"] = DropDownListDistricts.SelectedItem.ToString();
                this.Master.MasterDistrictLabel = DropDownListDistricts.SelectedItem.ToString();
                SetLastDistrictID(DropDownListDistricts.SelectedValue.ToString().ToUpper());
            }
            else
            {
                Session.Remove("CurrentDistrict");
                Session.Remove("CurrentDistrictName");
                this.Master.MasterDistrictLabel = " No District Selected ";
                
            }
        }

    }
}
using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CurrentDistrictName"] == null || Session["CurrentDistrictName"].ToString() == string.Empty)
                {
                    CurrDistrict.Text = "*";
                }
                else
                    CurrDistrict.Text = Session["CurrentDistrictName"].ToString();
            }

            //Security Settings
            if (Session["AccessLevel"] != null)
            {
                switch (Session["AccessLevel"].ToString().ToUpper())
                {
                    case "ADMIN":
                        MstrAdminSetup.Visible = true;
                        MstrAdminOfficeUtils.Visible = true;
                        MstrMgmtReports.Visible = true;
                        MstrMgmtOpReports.Visible = true;
                        DivRMEOMReport.Visible = true;
                        linkPrintNotifications.Visible = true;
                        InvDistTotals.Visible = true;
                        linkPurchSummary.Visible = true;
                        linkUsageSummary.Visible = true;
                        break;
                    case "OFFICE":
                        MstrAdminSetup.Visible = false;
                        MstrAdminOfficeUtils.Visible = true;
                        MstrMgmtReports.Visible = true;
                        MstrMgmtOpReports.Visible = true;
                        DivRMEOMReport.Visible = true;
                        linkPrintNotifications.Visible = true;
                        InvDistTotals.Visible = true;
                        linkPurchSummary.Visible = true;
                        linkUsageSummary.Visible = true;
                        break;
                    case "RM":
                        MstrAdminSetup.Visible = false;
                        MstrAdminOfficeUtils.Visible = false;
                        MstrMgmtReports.Visible = true;
                        MstrMgmtOpReports.Visible = true;
                        DivRMEOMReport.Visible = true;
                        linkPrintNotifications.Visible = true;
                        InvDistTotals.Visible = true;
                        linkPurchSummary.Visible = false;
                        linkUsageSummary.Visible = false;
                        break;
                    default:
                        MstrAdminSetup.Visible = false;
                        MstrAdminOfficeUtils.Visible = false;
                        MstrMgmtReports.Visible = false;
                        MstrMgmtOpReports.Visible = false;
                        DivRMEOMReport.Visible = false;
                        linkPrintNotifications.Visible = false;
                        InvDistTotals.Visible = false;
                        linkPurchSummary.Visible = false;
                        linkUsageSummary.Visible = false;
                        break;

                }

            }

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }


        public string MasterDistrictLabel
        {
            get
            {
                return CurrDistrict.Text;
            }
            set
            {
                CurrDistrict.Text = value;
            }
        }

        //Unified Districts list
        public SqlDataSource SQLMasterDistricts
        {
            get { return SqlDataSourceMyDistrictsMstr; }
        }

        //Unified Districts list Updated to only show Active and Inactive with Trx in Year
        public SqlDataSource SQLMasterDistrictsPY
        {
            get { return SqlDataSourceMyDistrictsMstrPY; }
        }

        public UpdatePanel MstrHdrUpdatePanel
        {
            get { return UpdatePanelMstrHdr; }
        }


    }
}
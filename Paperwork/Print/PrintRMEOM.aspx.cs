using Microsoft.Exchange.WebServices.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paperwork
{
    public partial class PrintRMEOM : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess(true);

            //District Session ID not set
            if (Session["ProgramYear"] == null)
                Response.Redirect("~/Default.aspx");

            //District Session ID not set
            if (Session["CurrentDistrict"] == null || string.IsNullOrEmpty(Session["CurrentDistrict"].ToString()))
                Response.Redirect("~/Default.aspx");

            switch (Session["AccessLevel"].ToString().ToUpper())
            {
                case "ADMIN":
                case "OFFICE":
                case "RM":
                    break;
                default:
                    //Denied
                    Response.Redirect("~/Default.aspx");
                    break;
            }


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

                DDLRMs.DataBind();
                DDLVPs.DataBind();

                //Update the SSRS Url based on settings in Global Settings
                string SSRSURL = GetGlobalSettingString("SSRS_URL");
                string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];
                Uri SSRSUri = new Uri(SSRSURL);
                ReportViewer1.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer1.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer1.ServerReport.ReportPath;

                //Email version of report
                HFSSRSBaseFolder2.DataBind();
                HFSSRSBaseFolder2.Value = SSRSBaseFolder;
                ReportViewer2.ServerReport.ReportServerUrl = SSRSUri;
                ReportViewer2.ServerReport.ReportPath = SSRSBaseFolder + ReportViewer2.ServerReport.ReportPath;

                DDLProgramYears_SelectedIndexChanged(null, null);
            }
            else
            {
                Page.Validate();
            }
        }

        protected void UpdateReportParams()
        {

            //prevent invalid dates from crashing the report
            if (DateTime.TryParse(TextBoxStartDate.Text, out DateTime testDate) == false)
                return;
            if (DateTime.TryParse(TextBoxEndDate.Text, out DateTime testDate2) == false)
                return;

            ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
            ReportParameter paramRM = new ReportParameter("RMNumber", DDLRMs.SelectedValue);
            ReportParameter paramStartDate = new ReportParameter("StartingDate", TextBoxStartDate.Text);
            ReportParameter paramEndDate = new ReportParameter("EndingDate", TextBoxEndDate.Text);
            ReportParameter paramGM = new ReportParameter("GM", DDLVPs.SelectedValue);
            try
            {
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramRM, paramStartDate, paramEndDate, paramGM });
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

        protected void DDLDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected void DDLProgramYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ProgramYear"] = DDLProgramYears.SelectedValue;
            UpdateReportParams();
        }

        protected void DDLRMs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }

        protected string EmailsSentTo;

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (DDLVPs.Items.Count == 0 || DDLVPs.SelectedIndex == -1 || DDLVPs.SelectedValue == "")
            {
                ShowDialog("Error", MessageType.Error, "Invalid email address. Email cancelled.", "");
                return;
            }
            else
            {
                EmailsSentTo = "";

                if (DDLVPs.SelectedValue == "*")
                {
                    foreach(ListItem vpItem in DDLVPs.Items)
                    {
                        if (vpItem.Value == "*" || vpItem.Value == "")
                            continue;

                        EmailReport(vpItem.Value);
                    }
                }
                else
                    EmailReport(DDLVPs.SelectedValue);

                if(EmailsSentTo!="")
                    ShowDialog("Sent", MessageType.Success, "Report(s) sent successfully to " + EmailsSentTo, "");

            }
        }

        protected void EmailReport(string VP)
        {
            string emailServer = "";

            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view = (DataView)SqlDataSourceEmailSettings.Select(args);
            DataTable table = view.ToTable();

            emailServer = table.Rows[0]["emailServer"].ToString();
            Boolean emailUseEWS = table.Rows[0]["emailUseEWS"].ToString() == "0" ? false : true;
            Boolean emailUseSSL = table.Rows[0]["emailUseSSL"].ToString() == "0" ? false : true;
            string emailFrom = table.Rows[0]["emailFromAddress"].ToString();
            //string emailTo = table.Rows[0]["NewVendorRequests"].ToString();

            if (DDLVPs.Items.Count == 0 || DDLVPs.SelectedIndex == -1 || DDLVPs.SelectedValue == "")
            {
                ShowDialog("Error", MessageType.Error, "Invalid email address. Email cancelled.", "");
                return;
            }

            string emailTo = GetVPEmailAddress(VP);
            int emailPort = (int)table.Rows[0]["emailPort"];
            string emailDomain = table.Rows[0]["emailAuthDomain"].ToString();
            string emailUsername = table.Rows[0]["emailAuthUser"].ToString();
            string emailPassword = table.Rows[0]["emailAuthPassword"].ToString();

            string subject = "Paperwork Web - RM EOM Reports";

            //Email body
            StringBuilder sb = new StringBuilder();
            sb.Append(" * *Automated email from the Paperwork Web RM EOM page. Do not reply to this email.* *\r\n");
            sb.Append("\r\n");
            sb.Append("Attached are the RM EOM reports for period ending "); sb.Append(TextBoxEndDate.Text); sb.Append(".\r\n");
            sb.Append("EOM report sent by user:  "); sb.Append(Session["CurrentUser"].ToString()); sb.Append("\r\n");

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
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = Fm;

                String[] addresses;
                addresses = emailTo.Split(',');
                foreach (string address in addresses)
                {
                    MailAddress To = new MailAddress(address);
                    mailMessage.To.Add(To);
                }
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
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                string reportName;

                reportName = "RMEOM(New)";
                ReportViewer2.ServerReport.ReportPath = HFSSRSBaseFolder2.Value + reportName;
                ReportParameter paramProgYear = new ReportParameter("ProgramYear", DDLProgramYears.SelectedValue);
                ReportParameter paramRM = new ReportParameter("RMNumber", DDLRMs.SelectedValue);
                ReportParameter paramStartDate = new ReportParameter("StartingDate", TextBoxStartDate.Text);
                ReportParameter paramEndDate = new ReportParameter("EndingDate", TextBoxEndDate.Text);


                try
                {
                    ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { paramProgYear, paramRM, paramStartDate, paramEndDate });
                    ReportViewer1.ServerReport.Refresh();
                }
                catch { }


                byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                MemoryStream ms1 = new System.IO.MemoryStream(bytes);
                msList.Add(ms1);
                BinaryWriter bw1 = new BinaryWriter(ms1);
                bwList.Add(bw1);
                bw1.Write(bytes);
                bw1.Flush();

                ms1.Position = 0;

                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms1, ct);
                attach.ContentDisposition.FileName = "RMEOM.pdf";

                mailMessage.Attachments.Add(attach);



                try
                {
                    //Check to see if anything was marked and attached
                    if (msList.Count > 0)
                    {
                        client.Send(mailMessage);

                        //Mail sent - update record as sent
                        //setEOMBatchSent();
                        if (EmailsSentTo == "")
                            EmailsSentTo = VP;
                        else
                            EmailsSentTo += ", " + VP;
                        
                    }

                    //Update the Formview - which updates the report sent label at the bottom
                    //FormViewEOMChecklist.DataBind();

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


                    //if(bw != null)
                    //    bw.Dispose();
                    //if (ms != null)
                    //{
                    //    ms.Close();
                    //    ms.Dispose();
                    //}

                    //Session["VendorRequestMsg"] = "Vendor request successfully sent.";
                    //Response.Redirect("~/Setup/SetupDistrictVendors");
                    if (mailMessage != null)
                        mailMessage.Dispose();

                    if (client != null)
                        client.Dispose();
                }

            }

        }

        private string GetVPEmailAddress(string vP)
        {
            string retEmailAddress = "";
            string connectionString = ConfigurationManager.ConnectionStrings["connPaperwork"].ConnectionString;
            SqlConnection dbConnection = new SqlConnection(connectionString);

            string queryString = "SELECT RTRIM(GMEMail) FROM [GMSetup] WHERE UPPER(RTRIM(GM))=UPPER(@GM)";

            using (SqlCommand dbCommand = new SqlCommand())
            {
                dbCommand.CommandText = queryString;
                dbCommand.Connection = dbConnection;

                SqlParameter dbParam_vP = new SqlParameter();
                dbParam_vP.ParameterName = "@GM";
                dbParam_vP.Value = vP;
                dbParam_vP.DbType = DbType.String;
                dbCommand.Parameters.Add(dbParam_vP);

                dbConnection.Open();
                try
                {
                    //System.Data.IDataReader dataReader = dbCommand.ExecuteScalar();// dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    object objEmail = dbCommand.ExecuteScalar();
                    if (objEmail == null)
                        retEmailAddress = "";
                    else
                        retEmailAddress = objEmail.ToString();
                }
                catch (Exception)
                {
                    retEmailAddress = "";
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return retEmailAddress;

        }

        protected void DDLVPs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReportParams();
        }
    }
}
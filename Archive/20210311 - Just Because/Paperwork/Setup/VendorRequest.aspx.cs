using Microsoft.Exchange.WebServices.Data;
using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.UI;

namespace Paperwork
{
    public partial class VendorRequest : Paperwork_Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //District Session ID not set
            if (Session["CurrentDistrict"] == null || Session["CurrentDistrict"].ToString() == string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                VerifyAccess(true);
                TextBoxVendorName.Focus();
            }
            else
            {
                VerifyAccess(false);
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            //USE FOR TRAINING INDICATOR
            string SSRSBaseFolder = ConfigurationManager.AppSettings["SSRS_BaseFolder"];



            string emailServer = "";

            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view = (DataView)SqlDataSourceEmailSettings.Select(args);
            DataTable table = view.ToTable();

            emailServer = table.Rows[0]["emailServer"].ToString();
            Boolean emailUseEWS = table.Rows[0]["emailUseEWS"].ToString() == "0" ? false : true;
            Boolean emailUseSSL = table.Rows[0]["emailUseSSL"].ToString() == "0" ? false : true;
            string emailFrom = table.Rows[0]["emailFromAddress"].ToString();
            string emailTo = table.Rows[0]["NewVendorRequests"].ToString();
            int emailPort = (int)table.Rows[0]["emailPort"];
            string emailDomain = table.Rows[0]["emailAuthDomain"].ToString();
            string emailUsername = table.Rows[0]["emailAuthUser"].ToString();
            string emailPassword = table.Rows[0]["emailAuthPassword"].ToString();

            string subject = "New Vendor Request - Paperwork Web";

            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING") > 0)
                subject = "*TRAINING* " + subject + " *TRAINING*";

            StringBuilder sb = new StringBuilder();
            if (SSRSBaseFolder != null && SSRSBaseFolder.IndexOf("PAPERWORKWEBTRAINING") > 0)
                sb.Append("***** PAPERWORK TRAINING WEB ***** \r\n");
            sb.Append(" * *Automated email from the Paperwork Web Vendor Request form. Do not reply to this email.* *\r\n");
            sb.Append("\r\n");
            sb.Append("District "); sb.Append(Session["CurrentDistrictName"].ToString()); sb.Append(" is requesting a new Vendor to be added to GP.\r\n");
            sb.Append("User:  "); sb.Append(Session["CurrentUser"].ToString()); sb.Append("\r\n");
            sb.Append("User Contact Info:  "); sb.Append(TextBoxUserInfo.Text); sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("Vendor Name:  "); sb.Append(TextBoxVendorName.Text); sb.Append("\r\n");
            sb.Append("Vendor Contact:  "); sb.Append(TextBoxContact.Text); sb.Append("\r\n");
            sb.Append("Address1:  "); sb.Append(TextBoxAddress1.Text); sb.Append("\r\n");
            sb.Append("Address2: "); sb.Append(TextBoxAddress2.Text); sb.Append("\r\n");
            sb.Append("City:  "); sb.Append(TextBoxCity.Text); sb.Append("\r\n");
            sb.Append("State:  "); sb.Append(TextBoxState.Text); sb.Append("\r\n");
            sb.Append("Zipcode:  "); sb.Append(TextBoxZip.Text); sb.Append("\r\n");
            sb.Append("Phone:  "); sb.Append(TextBoxPhone.Text); sb.Append("\r\n");
            sb.Append("FAX:  "); sb.Append(TextBoxFax.Text); sb.Append("\r\n");
            sb.Append("Additional Info:  "); sb.Append(TextBoxNotes.Text); sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("\r\n");

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
                    try
                    {
                        FolderId draftID = new FolderId(WellKnownFolderName.Drafts, emailFrom);
                        //message.SendAndSaveCopy(sentID);
                        message.Save(draftID);
                        message.Send();
                    }
                    catch (Microsoft.Exchange.WebServices.Data.ServiceRequestException ex)
                    {
                        LabelErrors.Text = ex.ToString();
                    }
                    catch (Exception ex)
                    {
                        LabelErrors.Text = ex.ToString();
                    }
                    finally
                    {
                        Session["VendorRequestMsg"] = "Vendor request successfully sent.";
                        Response.Redirect("~/Setup/SetupDistrictVendors");
                    }
                }//service URL is not empty
            }//using EWS
            else
            {
                System.Net.Mail.MailAddress Fm = new System.Net.Mail.MailAddress(emailFrom, emailFrom);
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = Fm;

                String[] addresses;
                addresses = emailTo.Split(',');
                foreach (string address in addresses)
                {
                    System.Net.Mail.MailAddress To = new System.Net.Mail.MailAddress(address);
                    mailMessage.To.Add(To);
                }
                mailMessage.Subject = subject;
                mailMessage.Priority = System.Net.Mail.MailPriority.High;
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = content;

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
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

                try
                {
                    client.Send(mailMessage);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    LabelErrors.Text = ex.ToString();
                }
                catch (SmtpException ex)
                {
                    LabelErrors.Text = ex.ToString();
                }
                catch (Exception ex)
                {
                    LabelErrors.Text = ex.ToString();
                }
                finally
                {
                    Session["VendorRequestMsg"] = "Vendor request successfully sent.";
                    Response.Redirect("~/Setup/SetupDistrictVendors");
                }

            }
        }

    }
}
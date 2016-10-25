using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MuleMonitor.MuleSystemService;
using System.Configuration;

namespace MuleMonitor
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new SystemServiceClient();
            try
            {
                var response = new GetAllTaskControllersResponse();

                client.GetAllTaskControllers(new MetaRequest(), new GetAllTaskControllersRequest(), out response);

                foreach (var muleTask in response.TaskControllers)
                {
                    if (!muleTask.IsOnlinek__BackingField)
                    {
                        SendEmailToUsers(muleTask);
                        break;
                    }
                }

            }
            catch (Exception)
            {
                SendNotificationEmail(string.Format("Unable to reach mule server: {0}, please check and restart all three mule services.", client.Endpoint.ListenUri));
            }
        }

        public static void SendEmailToUsers(TaskController muleTask)
        {
            SendNotificationEmail(string.Format("Mule service is Turned Off: {0}, please check and restart all three mule services.", muleTask.ServiceEndpointk__BackingField));

        }


        public static void SendNotificationEmail(string mailMessage)
        {
            var server = ConfigurationManager.AppSettings["SMTPHost"];

            var message = new MailMessage();
            var stringBuilder = new StringBuilder();

            message.From = new MailAddress("Turniverse@Turner.com");
            message.To.Add(new MailAddress(ConfigurationManager.AppSettings["MailTo"]));
            message.CC.Add(new MailAddress(ConfigurationManager.AppSettings["MailCC"]));

            message.BodyEncoding = Encoding.ASCII;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            message.Subject = "Mule Server is down.";

            stringBuilder.Append("Hi,<br><br>");
            stringBuilder.Append(mailMessage);
            stringBuilder.Append("<br><br>Thanks,<br>Turniverse Team");

            message.Body = stringBuilder.ToString();

            var client = new SmtpClient(server);
            client.Send(message);
        }
    }
}

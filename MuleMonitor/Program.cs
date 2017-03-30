using System;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using MuleMonitor.MuleSystemService;

namespace MuleMonitor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new SystemServiceClient();
            try
            {
                var response = new GetAllTaskControllersResponse();

                client.GetAllTaskControllers(new MetaRequest(), new GetAllTaskControllersRequest(), out response);

                foreach (var muleTask in response.TaskControllers)
                    if (!muleTask.IsOnlinek__BackingField || muleTask.IsSuspendedk__BackingField)
                    {
                        SendEmailToUsers(muleTask);
                        break;
                    }
            }
            catch (Exception)
            {
                SendNotificationEmail(
                    string.Format(
                        "Unable to reach Turniverse mule server: {0}, please check and restart all three mule services.",
                        client.Endpoint.ListenUri));
            }
        }

        public static void SendEmailToUsers(TaskController muleTask)
        {
            SendNotificationEmail(
                string.Format(
                    "Turniverse Mule service was Turned Off: {0}, please check and restart all three mule services.",
                    muleTask.ServiceEndpointk__BackingField));
        }


        public static void SendNotificationEmail(string mailMessage)
        {
            var server = ConfigurationManager.AppSettings["SMTPHost"];

            var message = new MailMessage();
            var stringBuilder = new StringBuilder();

            message.From = new MailAddress("Turniverse@Turner.com");

            AddReceipient(message.To, ConfigurationManager.AppSettings["MailTo"]);
            AddReceipient(message.CC, ConfigurationManager.AppSettings["MailCC"]);

            message.BodyEncoding = Encoding.ASCII;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            message.Subject = "Mule Server is down.";

            stringBuilder.Append("Hello Everyone, <br><br>");
            stringBuilder.Append(mailMessage);
            stringBuilder.Append("<br><br>Thanks,<br>Turniverse Team");

            message.Body = stringBuilder.ToString();

            var client = new SmtpClient(server);
            client.Send(message);
        }

        private static void AddReceipient(MailAddressCollection messageTo, string recepients)
        {
            var recepientArray = Regex.Split(recepients, ",");

            foreach (var recepient in recepientArray)
                if (recepient.Contains("@"))
                    messageTo.Add(recepient.Trim());
        }
    }
}
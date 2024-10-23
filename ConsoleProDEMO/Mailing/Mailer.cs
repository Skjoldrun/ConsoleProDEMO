using ConsoleProDEMO.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Mail;
using System.Text;

namespace ConsoleProDEMO.Mailing
{
    public class Mailer : AMailerBase, IMailer
    {
        private readonly ILogger<Mailer> _logger;

        public IMailerConfig MailerConfig { get; set; }

        /// <summary>
        /// Mailer class for AlertMailing or just plaintext Mails.
        /// </summary>
        /// <param name="configManager">If null then Defaults is used</param>
        /// <param name="logger">If null then NullLogger Instance is used</param>
        public Mailer(IConfigManager configManager = null, ILogger<Mailer> logger = null)
        {
            _logger = logger ?? NullLogger<Mailer>.Instance;

            if (configManager != null)
                MailerConfig = configManager.DeserializeConfig<MailerConfig>(MailerConfigName);
            else
                MailerConfig = GetDefaultConfigValues();
        }

        /// <summary>
        /// Generates an instance of MailerConfig with defaulted config values.
        /// This is due to non DI implementations and the possibility to not have a config injected to the constructor and Lib internal calls.
        /// </summary>
        private IMailerConfig GetDefaultConfigValues()
        {
            return new MailerConfig();
        }

        /// <summary>
        /// Sends a message to a configured smtp host with a given HTML template and replaces placeholders with the given parameters.
        /// Uses a HTML template and replaces placeholders with the given parameters from mailerconfig.
        /// </summary>
        /// <param name="subject">subject for the mail</param>
        /// <param name="message">normally formatted message</param>
        /// <param name="monospaceMessage">monospace formatted message, eg. callstack, or code</param>
        /// <param name="applicationName">name of the calling application</param>
        /// <param name="notificationType">type for formatting the mail. possible types are: Failure, Critical, Warning, Healthy, Information</param>
        public void SendMessage(
            string subject,
            string message,
            string monospaceMessage = "",
            string applicationName = "",
            NotificationType notificationType = NotificationType.Information)
        {
            SendMessageFromTo(sender: MailerConfig.Sender,
                recipients: MailerConfig.RecipientList,
                subject: subject,
                message: message,
                monospaceMessage: monospaceMessage,
                applicationName: applicationName,
                notificationType: notificationType);
        }

        /// <summary>
        /// Sends a message to a given recipients list from a given sender and configured smtp host.
        /// Uses a HTML template and replaces placeholders with the given parameters from mailerconfig.
        /// </summary>
        /// <param name="sender">Sender address</param>
        /// <param name="recipients">List of recipient addresses</param>
        /// <param name="subject">subject for the mail</param>
        /// <param name="message">normally formatted message</param>
        /// <param name="monospaceMessage">monospace formatted message, eg. callstack, or code</param>
        /// <param name="applicationName">name of the calling application</param>
        /// <param name="notificationType">type for formatting the mail. possible types are: Failure, Critical, Warning, Healthy, Information</param>
        public void SendMessageFromTo(
            string sender,
            List<string> recipients,
            string subject,
            string message,
            string monospaceMessage = "",
            string applicationName = "",
            NotificationType notificationType = NotificationType.Information)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                _applicationName = AssemblyInfoProcessor.GetEntryAssemblyName();
            }

            // Prevent spam if the mail was sent within the amount of minutes before.
            // If value was not set in the mailerconfig file all mails get sent.
            if (CheckIfSpam(subject, MailerConfig.SpamPreventionSubjectAge, MailerConfig.SpamPreventionStored))
            {
                return;
            }

            string mailTemplate = Properties.Resource.AlertMailTemplate;
            MailMessage mailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();

            message = FixNewLineFormattingForPTag(message);
            mailMessage.Subject = $"{_applicationName} - {notificationType}: {subject}";
            mailMessage.From = new MailAddress(sender);
            foreach (var recipient in recipients)
            {
                mailMessage.To.Add(recipient);
            }
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = mailTemplate
                .Replace("{{NotificationType}}", notificationType.ToString())
                .Replace("{{MachineName}}", Environment.MachineName)
                .Replace("{{AssemblyName}}", AssemblyInfoProcessor.GetEntryAssemblyName())
                .Replace("{{AssemblyVersion}}", AssemblyInfoProcessor.GetEntryAssemblyVersion())
                .Replace("{{ApplicationName}}", _applicationName)
                .Replace("{{Message}}", message)
                .Replace("{{MonospaceMessage}}", monospaceMessage);
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;

            smtpClient.Host = MailerConfig.SmtpHost;

            TrySendMessage(smtpClient, mailMessage, message, monospaceMessage);
        }

        /// <summary>
        /// Sends a message as plain text body without Mailtemplate.
        /// Uses mailerconfig parameters.
        /// </summary>
        /// <param name="subject">subject for the mail</param>
        /// <param name="message">normally formatted message</param>
        public void SendPlainTextMessage(string subject, string message)
        {
            SendPlainTextMessageFromTo(sender: MailerConfig.Sender,
                recipients: MailerConfig.RecipientList,
                subject: subject,
                message: message);
        }

        /// <summary>
        /// Sends a message as mail only as text body without Mailtemplate.
        /// Uses mailerconfig parameters.
        /// </summary>
        /// <param name="subject">subject for the mail</param>
        /// <param name="message">normally formatted message</param>
        public void SendPlainTextMessageFromTo(string sender, List<string> recipients, string subject, string message)
        {
            if (CheckIfSpam(subject, MailerConfig.SpamPreventionSubjectAge, MailerConfig.SpamPreventionStored))
            {
                return;
            }

            MailMessage mailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();

            mailMessage.Subject = $"{subject}";
            mailMessage.From = new MailAddress(sender);
            foreach (var recipient in recipients)
            {
                mailMessage.To.Add(recipient);
            }
            mailMessage.IsBodyHtml = false;
            mailMessage.Body = $"{message}";
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;

            smtpClient.Host = MailerConfig.SmtpHost;

            TrySendMessage(smtpClient, mailMessage, message);
        }

        /// <summary>
        /// Fixes new line markers in messages like \n, \r or \r\n to br tags for better formatting the text.
        /// </summary>
        /// <param name="message">text with possible new line markers</param>
        /// <returns>text with converted new line markers</returns>
        public string FixNewLineFormattingForPTag(string message)
        {
            // Check order is important because else it will replace \r\n with two br tags
            if (message.Contains("\r\n"))
            {
                message = message.Replace("\r\n", "<br>");
            }

            if (message.Contains("\n"))
            {
                message = message.Replace("\n", "<br>");
            }

            if (message.Contains("\r"))
            {
                message = message.Replace("\r", "<br>");
            }

            return message;
        }

        /// <summary>
        /// Tries to send a perared mailMessage with the perared smtpClient.
        /// If sending fails, it throws an exception filled with the given message and monospaceMessage texts or "not provided" if null.
        /// This should enable preserving the alertMail messages in case of failuere.
        /// </summary>
        /// <param name="smtpClient">prepared smtpClient from calling method</param>
        /// <param name="mailMessage">prepared mailMessage from calling method</param>
        /// <param name="message">message part of the given mailMessage to be preserved as exception text</param>
        /// <param name="monospaceMessage">monospaceMessage part of the given mailMessage to be preserved as exception text</param>
        private void TrySendMessage(SmtpClient smtpClient, MailMessage mailMessage, string message = null, string monospaceMessage = null)
        {
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                message = message ?? "not provided";
                monospaceMessage = monospaceMessage ?? "not provided";

                string errorMessage = new StringBuilder()
                    .AppendLine("Error while sending an AlertMail!")
                    .AppendLine()
                    .AppendLine($"Mailer Exception: {ex.Message}")
                    .AppendLine($"Mail Message Text: {message}")
                    .AppendLine($"Mail MonospaceMessage Text:")
                    .AppendLine($"{monospaceMessage}")
                    .ToString();

                EventLogWriter.WriteApplicationEventEntry(errorMessage, System.Diagnostics.EventLogEntryType.Error);

                throw new Exception(errorMessage);
            }
            finally
            {
                smtpClient.Dispose();
            }
        }
    }
}
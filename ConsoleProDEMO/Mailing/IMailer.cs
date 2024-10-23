namespace ConsoleProDEMO.Mailing
{
    public interface IMailer
    {
        IMailerConfig MailerConfig { get; set; }

        string FixNewLineFormattingForPTag(string message);

        void SendMessage(string subject, string message, string monospaceMessage = "", string applicationName = "", NotificationType notificationType = NotificationType.Information);

        void SendMessageFromTo(string sender, List<string> recipients, string subject, string message, string monospaceMessage = "", string applicationName = "", NotificationType notificationType = NotificationType.Information);

        void SendPlainTextMessage(string subject, string message);

        void SendPlainTextMessageFromTo(string sender, List<string> recipients, string subject, string message);
    }
}
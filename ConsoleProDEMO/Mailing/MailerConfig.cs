using System.Collections.Generic;

namespace ConsoleProDEMO.Mailing
{
    public class MailerConfig : IMailerConfig
    {
        public string SmtpHost { get; set; } = "YOUR_DEFAULT_MAILSERVER_HERE";
        public string Sender { get; set; } = "AlertMail@YOUR_DEFAULT_DOMAIN_HERE.de";

        public int SpamPreventionSubjectAge { get; set; } = 30;
        public bool SpamPreventionStored { get; set; } = false;

        public List<string> RecipientList { get; set; } = new List<string>
            {
                "SOMEBODY@SOMEDOMAIN.de"
            };
    }
}
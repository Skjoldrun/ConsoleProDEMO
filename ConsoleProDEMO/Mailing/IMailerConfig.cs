namespace ConsoleProDEMO.Mailing
{
    public interface IMailerConfig
    {
        string SmtpHost { get; set; }
        string Sender { get; set; }
        bool SpamPreventionStored { get; set; }
        int SpamPreventionSubjectAge { get; set; }
        List<string> RecipientList { get; set; }
    }
}
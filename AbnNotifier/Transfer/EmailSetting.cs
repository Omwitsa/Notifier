namespace AbnNotifier.Transfer
{
    public class EmailSetting
    {
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string LogoUrl { get; set; }
        public string SenderFromEmail { get; set; }
        public string SenderFromName { get; set; }
    }
}

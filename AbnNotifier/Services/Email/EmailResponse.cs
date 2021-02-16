namespace AbnNotifier.Services.Email
{
    public class EmailResponse
    {
        public EmailResponse()
        {
            Sent = false;
        }
        public bool Sent { get; set; }
        public string Message { get; set; }
    }
}

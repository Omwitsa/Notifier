namespace AbnNotifier.Transfer
{
    public class UnisolResponse<T>
    {
        public UnisolResponse()
        {
            Success = false;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}

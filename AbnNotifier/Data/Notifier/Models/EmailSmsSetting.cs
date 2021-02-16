using System;

namespace AbnNotifier.Data.Notifier.Models
{
    public class EmailSmsSetting : BaseEntity
    {
        public EmailSmsSetting()
        {

            Code = "SET-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
            CanSend = false;
        }
        public SettingKey Key { get; set; }
        public string Data { get; set; }
        public bool CanSend { get; set; }
    }

    public enum SettingKey
    {
        Email,
        Sms
    }
}

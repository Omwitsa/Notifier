using System;

namespace AbnNotifier.Data.Notifier
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
            Status = EntityStatus.Active;
        }
        public Guid Id { get; set; }
        public string Code { get; set; }
        public EntityStatus Status { get; set; }
        public string StatusStr => Status.ToString();
        public DateTime DateCreated { get; set; }
        public string DateCreatedStr => DateCreated.ToString("G");
        public DateTime DateUpdated { get; set; }
        public string DateUpdatedStr => DateUpdated.ToString("G");

    }

    public enum EntityStatus
    {
        Inactive,
        Active
    }
}

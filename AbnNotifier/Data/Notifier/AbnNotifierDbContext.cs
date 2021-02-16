using AbnNotifier.Data.Notifier.Models;
using Microsoft.EntityFrameworkCore;

namespace AbnNotifier.Data.Notifier
{
    public class AbnNotifierDbContext : DbContext
    {
        public AbnNotifierDbContext(DbContextOptions<AbnNotifierDbContext> options) : base(options)
        {

        }

        public DbSet<EmailSmsSetting> EmailSmsSettings { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Approver> Approver { get; set; }
    }
}

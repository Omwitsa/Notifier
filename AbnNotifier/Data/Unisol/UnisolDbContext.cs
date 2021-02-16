using AbnNotifier.Data.Unisol.Models;
using Microsoft.EntityFrameworkCore;

namespace AbnNotifier.Data.Unisol
{
    public class UnisolDbContext : DbContext
    {
        public UnisolDbContext(DbContextOptions<UnisolDbContext> options) : base(options)
        {

        }
        public DbSet<hrpContract> HrpContracts { get; set; }
        public DbSet<hrpEmployee> HrpEmployees { get; set; }
        public DbSet<Imprest> Imprests { get; set; }
        public DbSet<ImprestDisb> ImprestDisbs { get; set; }
        public DbSet<ImprestSur> ImprestSurs { get; set; }
        public DbSet<hrLeaveApp> HrLeaveApps { get; set; }
        public DbSet<WfdocCentre> WfdocCentre { get; set; }
        public DbSet<WfdocCentreDetails> WfdocCentreDetails { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}

using iie_erp_assist_server.Commons;
using Microsoft.EntityFrameworkCore;

namespace iie_erp_assist_server.Services
{
    public partial class AssistContext : DbContext
    {
        public AssistContext()
        {
        }

        public AssistContext(DbContextOptions<AssistContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AppSettings.GetAppSetting("DbConnectString"))/*.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)*/;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

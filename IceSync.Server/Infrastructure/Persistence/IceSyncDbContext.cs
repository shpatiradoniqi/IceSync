using IceSync.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IceSync.Server.Infrastructure.Persistence
{
    public class IceSyncDbContext : DbContext
    {

        public IceSyncDbContext(DbContextOptions<IceSyncDbContext> options)
          : base(options)
        { }

        public DbSet<Workflow> Workflows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Workflow>(builder =>
            {
                builder.HasKey(w => w.WorkflowId); 
                builder.Property(w => w.WorkflowId).ValueGeneratedNever();
            });
        }



    }
}

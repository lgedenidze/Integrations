using Integrations.Model;
using Integrations.Model.Integrations.Models;
using Microsoft.EntityFrameworkCore;

namespace Integrations.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<LineUp> LineUps { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SoonEvent> SoonEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<LineUp>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<LineUp>()
                .Property(l => l.Floor)
                .HasConversion<string>();
          
            modelBuilder.Entity<SoonEvent>()
          .HasOne(se => se.Event)
          .WithMany()
          .HasForeignKey(se => se.EventId)
          .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
        }
    }
}

using Integrations.Model;
using Integrations.Model.Integrations.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Integrations.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<LineUp> LineUps { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SoonEvent> SoonEvents { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketBasket> TicketBaskets { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

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

            modelBuilder.Entity<TicketBasket>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Baskets)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Basket)
                .WithMany()
                .HasForeignKey(t => t.BasketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(p => p.Ticket)
                .WithMany()
                .HasForeignKey(p => p.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}

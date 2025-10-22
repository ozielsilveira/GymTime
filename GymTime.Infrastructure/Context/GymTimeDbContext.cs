using GymTime.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Infrastructure.Context
{
    public class GymTimeDbContext(DbContextOptions<GymTimeDbContext> options) : DbContext(options)
    {
        public DbSet<GymMember> GymMembers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.GymMember)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GymMemberId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Class)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.ClassId);
        }
    }
}

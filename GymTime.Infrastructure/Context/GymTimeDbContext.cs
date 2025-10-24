using GymTime.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Infrastructure.Context;

public class GymTimeDbContext(DbContextOptions<GymTimeDbContext> options) : DbContext(options)
{
    public DbSet<GymMember> GymMembers { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassSession> ClassSessions { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // GymMember -> Booking
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.GymMember)
            .WithMany(g => g.Bookings)
            .HasForeignKey(b => b.GymMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        // Class -> Booking
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Class)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.ClassId)
            .OnDelete(DeleteBehavior.Restrict);

        // ClassSession -> Booking
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.ClassSession)
            .WithMany(cs => cs.Bookings)
            .HasForeignKey(b => b.ClassSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Class -> ClassSession
        modelBuilder.Entity<ClassSession>()
            .HasOne(cs => cs.Class)
            .WithMany(c => c.Sessions)
            .HasForeignKey(cs => cs.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: A member can only book a class session once
        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.GymMemberId, b.ClassSessionId })
            .IsUnique();
    }
}

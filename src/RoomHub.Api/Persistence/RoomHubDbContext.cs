using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Persistence;

public sealed class RoomHubDbContext(DbContextOptions<RoomHubDbContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SQLite loses DateTime.Kind; values are always stored as UTC, so read them back as UTC
        // (keeps the "Z" suffix in the JSON responses).
        var utc = new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Room>(room =>
        {
            room.ToTable("Rooms");
            room.HasKey(r => r.Id);
            room.Property(r => r.Id).ValueGeneratedNever();
            room.Property(r => r.Name).IsRequired().HasMaxLength(200);
            room.Property(r => r.Capacity).IsRequired();
        });

        modelBuilder.Entity<Booking>(booking =>
        {
            booking.ToTable("Bookings");
            booking.HasKey(b => b.Id);
            booking.Property(b => b.Id).ValueGeneratedNever();
            booking.Property(b => b.BookedBy).IsRequired().HasMaxLength(200);
            booking.Property(b => b.StartUtc).HasConversion(utc);
            booking.Property(b => b.EndUtc).HasConversion(utc);
            booking.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

            booking.HasOne<Room>()
                .WithMany()
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            booking.Ignore(b => b.DayUtc);

            booking.HasIndex(b => new { b.RoomId, b.Status });
            booking.HasIndex(b => new { b.BookedBy, b.Status, b.StartUtc });
        });
    }
}

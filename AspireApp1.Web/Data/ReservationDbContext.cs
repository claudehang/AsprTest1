using AspireApp1.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.Web.Data;

public class ReservationDbContext : DbContext
{
    public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Seat> Seats { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 配置关系
        modelBuilder.Entity<Seat>()
            .HasMany(s => s.Reservations)
            .WithOne(r => r.Seat)
            .HasForeignKey(r => r.SeatId);

        // 添加20个座位的初始数据
        var seats = new List<Seat>();
        for (int i = 1; i <= 20; i++)
        {
            seats.Add(new Seat { Id = i, IsAvailable = true });
        }

        modelBuilder.Entity<Seat>().HasData(seats);
    }
}

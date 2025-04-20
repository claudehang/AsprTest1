using AspireApp1.Web.Data;
using AspireApp1.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.Web.Services;

public class ReservationService
{
    private readonly ReservationDbContext _context;

    public ReservationService(ReservationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Seat>> GetAllSeatsAsync()
    {
        // 使用日期范围解决时区问题 (转换为UTC)
        var todayStart = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        var todayEnd = DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        return await _context.Seats
            .Include(s => s.Reservations.Where(r =>
                r.ReservationTime >= todayStart &&
                r.ReservationTime <= todayEnd))
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetUserReservationsAsync(string userId)
    {
        // 获取今天及以后的预约 (使用UTC时间)
        var todayStart = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

        return await _context.Reservations
            .Include(r => r.Seat)
            .Where(r => r.UserId == userId && r.ReservationTime >= todayStart)
            .OrderBy(r => r.ReservationTime)
            .ToListAsync();
    }

    public async Task<Reservation> ReserveSeatAsync(int seatId, string userId)
    {
        var seat = await _context.Seats.FindAsync(seatId);
        if (seat == null)
        {
            throw new ArgumentException("座位不存在", nameof(seatId));
        }

        // 检查座位是否已被预约 (使用UTC时间)
        var todayStart = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        var todayEnd = DateTime.SpecifyKind(DateTime.Today.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var existingReservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.SeatId == seatId &&
                               r.ReservationTime >= todayStart &&
                               r.ReservationTime <= todayEnd);

        if (existingReservation != null)
        {
            throw new InvalidOperationException("该座位今天已被预约");
        }

        var now = DateTime.UtcNow; // 使用UTC时间
        var reservationCode = $"座位 #{seatId} - {now:yyyy-MM-dd HH:mm}";
        var reservation = new Reservation
        {
            SeatId = seatId,
            UserId = userId,
            ReservationTime = now,
            ReservationCode = reservationCode
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return reservation;
    }

    public async Task CancelReservationAsync(int reservationId, string userId)
    {
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation == null)
        {
            throw new ArgumentException("预约不存在", nameof(reservationId));
        }

        if (reservation.UserId != userId)
        {
            throw new UnauthorizedAccessException("无权取消此预约");
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
    }
}

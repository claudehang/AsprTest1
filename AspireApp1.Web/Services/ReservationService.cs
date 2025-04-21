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
        var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

        return await _context.Seats
            .Include(s => s.Reservations.Where(r => r.EndTime >= today))
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetUserReservationsAsync(string userId)
    {
        // 获取今天及以后的预约 (使用UTC时间)
        var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

        return await _context.Reservations
            .Include(r => r.Seat)
            .Where(r => r.UserId == userId && r.EndTime >= today)
            .OrderBy(r => r.ReservationTime)
            .ToListAsync();
    }

    public async Task<Reservation> ReserveSeatAsync(int seatId, string userId, DateTime endDate)
    {
        var seat = await _context.Seats.FindAsync(seatId);
        if (seat == null)
        {
            throw new ArgumentException("座位不存在", nameof(seatId));
        }

        // 确保开始和结束日期都是UTC时间
        var startDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        endDate = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);

        // 检查是否有冲突的预约
        var hasConflict = await _context.Reservations
            .AnyAsync(r => r.SeatId == seatId &&
                      ((r.ReservationTime <= endDate && r.EndTime >= startDate) ||
                       (r.ReservationTime <= endDate && r.EndTime >= endDate) ||
                       (r.ReservationTime <= startDate && r.EndTime >= startDate)));

        if (hasConflict)
        {
            throw new InvalidOperationException("该座位在选定的日期范围内已被预约");
        }

        var now = DateTime.UtcNow;
        var reservationCode = $"座位 #{seatId} - {startDate:yyyy-MM-dd}至{endDate:yyyy-MM-dd}";
        var reservation = new Reservation
        {
            SeatId = seatId,
            UserId = userId,
            ReservationTime = startDate,
            EndTime = endDate,
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

    // 获取当月最后一天的方法
    public DateTime GetLastDayOfMonth()
    {
        var today = DateTime.Today;
        var lastDay = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
        return lastDay;
    }
}

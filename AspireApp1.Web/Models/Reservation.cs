namespace AspireApp1.Web.Models;

public class Reservation
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public Seat? Seat { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime ReservationTime { get; set; }
    public DateTime EndTime { get; set; } // 新增结束时间
    public string ReservationCode { get; set; } = string.Empty;
}

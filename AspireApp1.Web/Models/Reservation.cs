namespace AspireApp1.Web.Models;

public class Reservation
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public Seat? Seat { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime ReservationTime { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
}

namespace AspireApp1.Web.Models;

public class Seat
{
    public int Id { get; set; }
    public bool IsAvailable { get; set; } = true;
    public List<Reservation> Reservations { get; set; } = new List<Reservation>();
}

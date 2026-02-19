namespace PetsHotel.Models;

public class Stay
{
    public int Id { get; set; }
    public int DogId { get; set; }
    public string DogName { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerPhone { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsActive => CheckOutDate == null;

    public int DaysStayed
    {
        get
        {
            var end = CheckOutDate ?? DateTime.Today;
            return Math.Max(1, (int)(end - CheckInDate.Date).TotalDays);
        }
    }
}

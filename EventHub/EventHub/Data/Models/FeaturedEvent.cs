namespace EventHub.Data.Models;

public class FeaturedEvent
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Participants { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
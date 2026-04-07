using Microsoft.AspNetCore.Identity;
using EventHub.Data.Models;

namespace EventHub.Data;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public bool IsProfileComplete => !string.IsNullOrEmpty(FirstName) &&
                                      !string.IsNullOrEmpty(LastName) &&
                                      DateOfBirth.HasValue;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Навигационные свойства
    public virtual ICollection<Event> CreatedEvents { get; set; } = new List<Event>();
    public virtual ICollection<Participant> Participations { get; set; } = new List<Participant>();
   
}
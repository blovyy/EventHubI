using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub.Data.Models;

public class Participant
{
    [Key]
    public int Id { get; set; }

    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public int EventId { get; set; }

    [ForeignKey("EventId")]
    public virtual Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public bool IsCheckedIn { get; set; }

    public DateTime? CheckedInAt { get; set; }
}
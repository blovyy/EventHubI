using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub.Data.Models;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TicketNumber { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(100)]
    public string Type { get; set; } = "Standard"; // Standard, VIP, EarlyBird и т.д.

    public decimal Price { get; set; }

    public int EventId { get; set; }

    [ForeignKey("EventId")]
    public virtual Event Event { get; set; } = null!;

    public string? OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public virtual ApplicationUser? Owner { get; set; }

    public bool IsPaid { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsUsed { get; set; } // Использован ли билет

    public DateTime? UsedAt { get; set; }
}
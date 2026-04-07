using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub.Data.Models;

public class Event
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название мероприятия")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите описание")]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [StringLength(500)]
    public string Location { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [Range(1, 100000, ErrorMessage = "Количество участников должно быть больше 0")]
    public int MaxParticipants { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 1000000, ErrorMessage = "Цена не может быть отрицательной")]
    public decimal Price { get; set; }

    public EventType EventType { get; set; } = EventType.Other;

    public EventModerationStatus ModerationStatus { get; set; } = EventModerationStatus.Pending;

    public DateTime? ModeratedAt { get; set; }

    public string? ModeratedById { get; set; }

    [ForeignKey(nameof(ModeratedById))]
    public virtual ApplicationUser? ModeratedBy { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Draft;

    public int? CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }

    public string? OrganizerId { get; set; }

    [ForeignKey(nameof(OrganizerId))]
    public virtual ApplicationUser? Organizer { get; set; }

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}

public enum EventModerationStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public enum EventStatus
{
    Draft = 0,
    Published = 1,
    Cancelled = 2,
    Completed = 3
}

public enum EventType
{
    Lecture = 0,
    Seminar = 1,
    Workshop = 2,
    Conference = 3,
    Training = 4,
    Webinar = 5,
    Meeting = 6,
    Other = 7
}
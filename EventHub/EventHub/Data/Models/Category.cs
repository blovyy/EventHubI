using System.ComponentModel.DataAnnotations;

namespace EventHub.Data.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    public string? Icon { get; set; }

    // Навигационное свойство - список мероприятий этой категории
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
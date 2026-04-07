using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventHub.Data.Models;

namespace EventHub.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Event> Events { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Event -> Category
        builder.Entity<Event>()
            .HasOne(e => e.Category)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Event -> Organizer (User)
        builder.Entity<Event>()
            .HasOne(e => e.Organizer)
            .WithMany(u => u.CreatedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Participant -> Event
        builder.Entity<Participant>()
            .HasOne(p => p.Event)
            .WithMany(e => e.Participants)
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Restrict); // 👈 ИЗМЕНЕНО: было Cascade, стало Restrict

       

        // Participant -> User
        builder.Entity<Participant>()
            .HasOne(p => p.User)
            .WithMany(u => u.Participations)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict); // 👈 ИЗМЕНЕНО: было SetNull, стало Restrict

        

        // Индексы
        builder.Entity<Event>()
            .HasIndex(e => e.StartDate);

        builder.Entity<Event>()
            .HasIndex(e => e.Status);

      

        builder.Entity<Ticket>()
            .HasOne(t => t.Event)
            .WithMany()
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ticket>()
            .HasOne(t => t.Owner)
            .WithMany()
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Participant>()
            .HasIndex(p => new { p.EventId, p.UserId });

        builder.Entity<Ticket>()
            .Property(t => t.Price)
            .HasPrecision(18, 2);

        builder.Entity<Ticket>()
            .HasIndex(t => t.TicketNumber)
            .IsUnique();

        builder.Entity<Review>()
             .HasOne(r => r.Event)
             .WithMany(e => e.Reviews)
             .HasForeignKey(r => r.EventId)
             .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasIndex(r => new { r.EventId, r.UserId })
            .IsUnique();
    }
}

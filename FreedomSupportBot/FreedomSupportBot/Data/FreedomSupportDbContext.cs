using FreedomSupportBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FreedomSupportBot.Data;

public class FreedomSupportDbContext : DbContext
{
    public FreedomSupportDbContext(DbContextOptions<FreedomSupportDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<SupportMessage> SupportMessages { get; set; }
    
    public DbSet<Conversation> Conversations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<SupportMessage>()
            .HasOne(sm => sm.Conversation)
            .WithMany(c => c.Messages)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
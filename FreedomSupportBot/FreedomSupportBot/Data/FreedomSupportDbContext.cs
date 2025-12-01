using FreedomSupportBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FreedomSupportBot.Data;

public class FreedomSupportDbContext : DbContext
{
    public FreedomSupportDbContext(DbContextOptions<FreedomSupportDbContext> options) 
        : base( options)
    {
        
    }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<SupportMessage> SupportMessages { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreedomSupportBot.Data.Models;

public class SupportMessage
{
    [Key]
    public int Id { get; set; }
    
    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    public bool FromCustomer { get; set; }

    [Required]
    public required string Text { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
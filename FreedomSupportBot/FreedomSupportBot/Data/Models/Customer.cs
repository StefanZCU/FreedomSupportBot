using System.ComponentModel.DataAnnotations;

namespace FreedomSupportBot.Data.Models;

public class Customer
{
    [Key]
    public int Id { get; set; }

    public long TelegramUserId { get; set; }

    public string? Username { get; set; }

    public DateTime FirstSeen { get; set; }

    public DateTime LastSeen { get; set; }

    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}
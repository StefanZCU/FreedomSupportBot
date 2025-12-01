namespace FreedomSupportBot.Services.Interfaces;

public interface IAiSupportService
{
    Task<string> GetReplyAsync(int customerId, string customerMessage);
}
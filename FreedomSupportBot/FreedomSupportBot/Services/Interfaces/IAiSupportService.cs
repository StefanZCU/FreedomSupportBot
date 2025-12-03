namespace FreedomSupportBot.Services.Interfaces;

public interface IAiSupportService
{
    Task<string> GetReplyAsync(string customerMessage);
}
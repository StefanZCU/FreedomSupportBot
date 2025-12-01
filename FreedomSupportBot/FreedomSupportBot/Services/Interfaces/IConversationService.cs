using FreedomSupportBot.Data.Models;

namespace FreedomSupportBot.Services.Interfaces;

public interface IConversationService
{
    Task<Customer> GetOrCreateCustomerAsync(long telegramUserId, string? username);
    Task SaveCustomerMessageAsync(int customerId, string text);
    Task SaveBotMessageAsync(int customerId, string text);
    Task<string> HandleMessageAsync(long telegramUserId, string? username, string text);
}
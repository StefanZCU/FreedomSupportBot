using FreedomSupportBot.Data.Models;

namespace FreedomSupportBot.Services.Interfaces;

public interface IConversationService
{
    Task<Customer> GetOrCreateCustomerAsync(long telegramUserId, string? username);
    Task<Conversation> GetOrCreateActiveConversationAsync(int customerId);
    Task SaveCustomerMessageAsync(int conversationId, string text);
    Task SaveBotMessageAsync(int conversationId, string text);
    Task<string> GetLast30MessagesAsync(int conversationId, int maxCount);
    string GenerateNewPrompt(string conversationText, string customerMessage);
    
    Task<string> HandleMessageAsync(long telegramUserId, string? username, string text);
}
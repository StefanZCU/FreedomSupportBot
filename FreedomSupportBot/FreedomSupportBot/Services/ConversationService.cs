using FreedomSupportBot.Data;
using FreedomSupportBot.Data.Models;
using FreedomSupportBot.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreedomSupportBot.Services;

public class ConversationService : IConversationService
{
    private readonly FreedomSupportDbContext _dbContext;
    private readonly IAiSupportService _aiSupportService;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(FreedomSupportDbContext dbContext,
        IAiSupportService aiSupportService,
        ILogger<ConversationService> logger)
    {
        _dbContext = dbContext;
        _aiSupportService = aiSupportService;
        _logger = logger;
    }

    public async Task<Customer> GetOrCreateCustomerAsync(long telegramUserId, string? username)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.TelegramUserId == telegramUserId);

        if (customer == null)
        {
            var now = DateTime.UtcNow;

            customer = new Customer
            {
                TelegramUserId = telegramUserId,
                Username = username,
                FirstSeen = now,
                LastSeen = now
            };

            await _dbContext.Customers.AddAsync(customer);
        }
        else
        {
            customer.LastSeen = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        return customer;
    }

    public async Task<Conversation> GetOrCreateActiveConversationAsync(int customerId)
    {
        var conversation = await _dbContext.Conversations
            .Where(c => c.CustomerId == customerId && c.IsActive)
            .OrderByDescending(c => c.StartedAt)
            .FirstOrDefaultAsync();

        if (conversation != null) return conversation;

        conversation = new Conversation()
        {
            CustomerId = customerId,
            StartedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _dbContext.Conversations.AddAsync(conversation);
        await _dbContext.SaveChangesAsync();
        return conversation;
    }

    public async Task SaveCustomerMessageAsync(int conversationId, string text)
    {
        var message = new SupportMessage
        {
            ConversationId = conversationId,
            FromCustomer = true,
            Text = text,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.SupportMessages.AddAsync(message);
        _logger.LogInformation("Saved customer message for conversation {ConversationId}: {Text}", conversationId,
            text);
    }

    public async Task SaveBotMessageAsync(int conversationId, string text)
    {
        var message = new SupportMessage
        {
            ConversationId = conversationId,
            FromCustomer = false,
            Text = text,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.SupportMessages.AddAsync(message);
        _logger.LogInformation("Saved bot message for conversation {ConversationId}: {Text}", conversationId, text);
    }

    public async Task<string> HandleMessageAsync(long telegramUserId, string? username, string text)
    {
        var customer = await GetOrCreateCustomerAsync(telegramUserId, username);
        var conversation = await GetOrCreateActiveConversationAsync(customer.Id);

        await SaveCustomerMessageAsync(conversation.Id, text);

        var replyText = await _aiSupportService.GetReplyAsync(text);

        await SaveBotMessageAsync(conversation.Id, replyText);

        await _dbContext.SaveChangesAsync();

        return replyText;
    }
}
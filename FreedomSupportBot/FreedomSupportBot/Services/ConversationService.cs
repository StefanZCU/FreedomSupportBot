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
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created new customer with id {CustomerId}", customer.Id);
        }
        else
        {
            customer.LastSeen = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        return customer;
    }

    public async Task SaveCustomerMessageAsync(int customerId, string text)
    {
        var message = new SupportMessage
        {
            CustomerId = customerId,
            FromCustomer = true,
            Text = text,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.SupportMessages.AddAsync(message);
        _logger.LogInformation("Saved customer message for customer {CustomerId}: {Text}", customerId, text);
    }

    public async Task SaveBotMessageAsync(int customerId, string text)
    {
        var message = new SupportMessage
        {
            CustomerId = customerId,
            FromCustomer = false,
            Text = text,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.SupportMessages.AddAsync(message);
        _logger.LogInformation("Saved bot message for customer {CustomerId}: {Text}", customerId, text);
    }

    public async Task<string> HandleMessageAsync(long telegramUserId, string? username, string text)
    {
        var customer = await GetOrCreateCustomerAsync(telegramUserId, username);
        
        await SaveCustomerMessageAsync(customer.Id, text);
        
        var reply = await _aiSupportService.GetReplyAsync(customer.Id, text);
        
        await SaveBotMessageAsync(customer.Id, reply);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation($"Handled message: {text}");
        
        return reply;
    }
}
using FreedomSupportBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FreedomSupportBot.Services;

public class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TelegramBotService> _logger;

    public TelegramBotService(
        ITelegramBotClient botClient,
        IServiceScopeFactory scopeFactory,
        ILogger<TelegramBotService> logger)
    {
        _botClient = botClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TelegramBotService starting...");

        try
        {
            var me = await _botClient.GetMe(stoppingToken);
            _logger.LogInformation("Telegram bot connected. Username: {Username}, Name: {FirstName}",
                me.Username, me.FirstName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Telegram. Check your bot token.");
            return;
        }

        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = [UpdateType.Message]
        };
        
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: stoppingToken);

        _logger.LogInformation("TelegramBotService is now listening for messages...");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient, 
        Update update,
        CancellationToken cancellationToken)
    {
        
        if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
        {
            return;
        }

        var message = update.Message;
        var text = message.Text ?? string.Empty;
        var chatId = message.Chat.Id;

        _logger.LogInformation("Received message from {ChatId}: {Text}", chatId, text);
        
        using var scope = _scopeFactory.CreateScope();
        var conversationService = scope.ServiceProvider.GetRequiredService<IConversationService>();

        var replyText = await conversationService.HandleMessageAsync(
            chatId, 
            message.Chat.Username, 
            text);

        await _botClient.SendMessage(
            chatId: chatId,
            text: replyText,
            cancellationToken: cancellationToken);
    }
    
    private Task HandleErrorAsync(
        ITelegramBotClient bot,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram polling error");
        return Task.CompletedTask;
    }
}
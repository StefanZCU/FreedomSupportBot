namespace FreedomSupportBot.Services;

public class OpenAiResponse
{
    public List<Choice> choices { get; set; } = new();
}

public class Choice
{
    public Message message { get; set; } = null!;
}

public class Message
{
    public string role { get; set; } = string.Empty;
    public string content { get; set; } = string.Empty;
}
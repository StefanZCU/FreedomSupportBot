using FreedomSupportBot.Services.Interfaces;

namespace FreedomSupportBot.Services;

public class OpenAiSupportService : IAiSupportService
{
    private readonly HttpClient _httpClient;
    private readonly string _openAiApiKey;
    private readonly string _openAiModel;
    private readonly string _openAiPersona;

    public OpenAiSupportService(string openAiApiKey, string openAiModel, string openAiPersona)
    {
        _httpClient = new HttpClient();
        _openAiApiKey = openAiApiKey;
        _openAiModel = openAiModel;
        _openAiPersona = openAiPersona;
    }

    public async Task<string> GetReplyAsync(string customerMessage)
    {
        var body = new
        {
            model = _openAiModel,
            messages = new[]
            {
                new { role = "system", content = _openAiPersona },
                new { role = "user", content = customerMessage },
            }
        };
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");
        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);
        
        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("OpenAi error" + response.StatusCode);
            Console.WriteLine(responseText);
            
            return "AI error, please try again later";
        }

        var result = System.Text.Json.JsonSerializer.Deserialize<OpenAiResponse>(responseText);

        var reply = result?
            .choices?
            .FirstOrDefault()?
            .message
            .content;
        
        return reply ?? "I don't know what to say.";
    }
}
using FreedomSupportBot.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreedomSupportBot.Controllers;

public class TestController : Controller
{
    private readonly IAiSupportService _aiSupportService;

    public TestController(IAiSupportService aiSupportService)
    {
        _aiSupportService = aiSupportService;
    }

    public async Task<IActionResult> AiReply(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            message = "I have an issue with my order, how can you help me?";
        }
        
        var reply = await _aiSupportService.GetReplyAsync(message);  
        
        return Content(reply);
    }
}
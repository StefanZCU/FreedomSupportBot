using FreedomSupportBot.Data;
using FreedomSupportBot.Services;
using FreedomSupportBot.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var connectionString = config.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<FreedomSupportDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var openAiKey = config["OpenAi:ApiKey"];
var openAiModel = config["OpenAi:Model"];
var openAiPersona = config["OpenAi:Persona"];

builder.Services.AddSingleton<IAiSupportService>(new OpenAiSupportService(openAiKey!, openAiModel!, openAiPersona!));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
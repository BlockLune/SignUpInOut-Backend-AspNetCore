using Hei.Captcha;
using Microsoft.EntityFrameworkCore;
using SignUpInOut_Backend_AspNetCore.Models;
using DotNetEnv;
using Discord.WebSocket;
using SignUpInOut_Backend_AspNetCore.Hubs;
using SignUpInOut_Backend_AspNetCore.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// db context
var dbConnectionStr = Environment.GetEnvironmentVariable("SIGNUPINOUT_DB_CONNECTION_STRING") ??
    builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SignupinoutDbContext>(options =>
    options.UseMySql(dbConnectionStr, ServerVersion.AutoDetect(dbConnectionStr)));
// http client
builder.Services.AddHttpClient();
// SignalR
builder.Services.AddSignalR();
// captcha
builder.Services.AddSingleton<SecurityCodeHelper>(); // 注册 SecurityCodeHelper 为单例
builder.Services.AddScoped<CaptchaCacheService>(); // 注册 CaptchaCache 为作用域范围的服务
// user
builder.Services.AddScoped<UserService>();
// discord bot
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<DiscordBotService>();

// controllers
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var botService = app.Services.GetRequiredService<DiscordBotService>();
await botService.InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/hub");

app.Run();

using Hei.Captcha;
using Microsoft.EntityFrameworkCore;
using SignUpInOut_Backend_AspNetCore.Models;
using DotNetEnv;
using Discord;
using Discord.WebSocket;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// discord bot
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<DiscordBotService>();

// captcha
builder.Services.AddSingleton<SecurityCodeHelper>(); // 注册 SecurityCodeHelper 为单例
builder.Services.AddScoped<CaptchaCache>(); // 注册 CaptchaCache 为作用域范围的服务

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
var dbContextString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SignupinoutDbContext>(options =>
    options.UseMySql(dbContextString, ServerVersion.AutoDetect(dbContextString)));

// controllers
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.Run();

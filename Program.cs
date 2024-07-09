using Microsoft.EntityFrameworkCore;
using SignUpInOut_Backend_AspNetCore.Models;

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
var dbContextString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SignupinoutDbContext>(options =>
    options.UseMySql(dbContextString, ServerVersion.AutoDetect(dbContextString)));

// controllers
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

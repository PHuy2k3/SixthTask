using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Notifications.Api.Hubs;
using Notifications.Api.Middlewares;
using Notifications.Api.Realtime;
using Notifications.Biz;
using Notifications.Biz.Model;
using Notifications.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddPolicy("FE", p => p
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// Data + Biz
builder.Services.AddNotificationsData(builder.Configuration);
builder.Services.AddScoped<INotificationService, NotificationService>();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationPusher, SignalRNotificationPusher>();

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["JwtKey"] ?? throw new Exception("Missing Jwt:JwtKey");
var issuer = jwt["Issuer"] ?? throw new Exception("Missing Jwt:Issuer");
var audience = jwt["Audience"] ?? throw new Exception("Missing Jwt:Audience");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(10)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FE");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();

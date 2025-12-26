using Identity.Api;
using Identity.Biz;
using Identity.Biz.Security;
using Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Kernel.Exceptions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (CRA)
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("FE", p =>
        p.WithOrigins("http://localhost:3000")
         .AllowAnyHeader()
         .AllowAnyMethod()
    // .AllowCredentials() // chỉ bật nếu dùng cookie
    );
});

// JWT Options
var jwtOpt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new Exception("Missing Jwt settings in appsettings.json");
builder.Services.AddSingleton<IJwtOptions>(jwtOpt);

// Data + Biz
builder.Services.AddIdentityData(builder.Configuration);
builder.Services.AddIdentityBiz();
builder.Services.AddHttpClient("Notifications", client =>
{
    var baseUrl = builder.Configuration["Notifications:BaseUrl"] ?? "";
    if (!string.IsNullOrWhiteSpace(baseUrl))
        client.BaseAddress = new Uri(baseUrl);
});
// JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOpt.Issuer,
            ValidAudience = jwtOpt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpt.JwtKey)),
            ClockSkew = TimeSpan.FromSeconds(10)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Global exception -> BizException
app.UseMiddleware<ExceptionMiddleware>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// CORS (đặt trước auth)
app.UseCors("FE");

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

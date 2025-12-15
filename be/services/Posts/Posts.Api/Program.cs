using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Posts.Biz;
using Posts.Data;
using Shared.Kernel;
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

// Data + Biz
builder.Services.AddPostsData(builder.Configuration);
builder.Services.AddPostsBiz();

// JWT validate (Posts cũng cần verify token nếu endpoint yêu cầu [Authorize])
// Dùng chung secret với Identity
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["JwtKey"] ?? throw new Exception("Missing Jwt:JwtKey");
var issuer = jwt["Issuer"] ?? throw new Exception("Missing Jwt:Issuer");
var audience = jwt["Audience"] ?? throw new Exception("Missing Jwt:Audience");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
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

// Global exception -> BizException
app.UseMiddleware<ExceptionMiddleware>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// CORS
app.UseCors("FE");

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

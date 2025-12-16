using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Posts.Biz;
using Posts.Biz.Interfaces;
using Posts.Data;
using Posts.Data.Interfaces;
using Posts.Data.Repositories;
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
    opt.AddPolicy("FE", p => p
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});


// Data + Biz
//builder.Services.AddPostsData(builder.Configuration);
builder.Services.AddPostsBiz();
builder.Services.AddDbContext<PostsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("PostsDb")));

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IPostMediaRepository, PostMediaRepository>();
builder.Services.AddScoped<IPostService, PostService>();


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

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// ✅ CORS đặt sớm
app.UseCors("FE");

// ✅ Static files (uploads) nên trước auth nếu bạn muốn public
app.UseStaticFiles();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Global exception (đặt sau CORS để response luôn có header CORS)
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();
app.Run();

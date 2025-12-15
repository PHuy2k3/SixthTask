using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Shared.Kernel;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (BizException ex)
        {
            ctx.Response.StatusCode = ex.HttpStatus;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { code = ex.Code, message = ex.Message }));
        }
    }
}

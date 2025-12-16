using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifications.Biz;
using Notifications.Biz.Model;
using System.Security.Claims;

namespace Notifications.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController(INotificationService biz) : ControllerBase
{
    static Guid Me(ClaimsPrincipal u)
        => Guid.Parse(u.FindFirstValue("sub") ?? throw new Exception("Missing sub claim"));

    [Authorize]
    [HttpGet("me")]
    public Task<List<NotificationDto>> Mine([FromQuery] int size = 20)
        => biz.GetMineAsync(Me(User), size);

    // Endpoint để Posts service gọi tạo notification (tạm thời mở, sau bạn thêm service-to-service auth)
    [HttpPost]
    public Task<NotificationDto> Create([FromBody] CreateNotificationReq req)
        => biz.CreateAsync(req);

    [Authorize]
    [HttpPost("{id:guid}/read")]
    public Task Read(Guid id) => biz.MarkReadAsync(id);
}

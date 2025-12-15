using Microsoft.AspNetCore.Mvc;
using Posts.Biz;
using Posts.Biz.Model;

namespace Posts.Api.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController(IPostService biz) : ControllerBase
{
    [HttpPost]
    public Task<PostDto> Create([FromBody] CreatePostReq req)
    {
        // tạm thời hardcode userId (sau nối JWT)
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        return biz.CreateAsync(req, userId);
    }

    [HttpGet("latest")]
    public Task<List<PostDto>> Latest([FromQuery] int size = 20)
        => biz.GetLatestAsync(size);
}

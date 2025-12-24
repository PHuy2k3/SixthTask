using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Api;
using Posts.Biz.Interfaces;
using Posts.Biz.Model;
using Shared.Kernel.Claims;

[ApiController]
[Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly IPostService _biz;
    public PostsController(IPostService biz) => _biz = biz;

    [Authorize]
    [HttpPost]
    public Task<PostDto> Create([FromBody] CreatePostReq req)
        => _biz.CreateAsync(req, User.GetUserId(), User.GetUserName());

    [Authorize]
    [HttpGet("latest")]
    public Task<List<PostDto>> Latest([FromQuery] int size = 20)
        => _biz.GetLatestAsync(User.GetUserId(), size);

    [Authorize]
    [HttpGet("me")]
    public Task<List<PostDto>> Mine([FromQuery] int size = 20)
        => _biz.GetMyPostsAsync(User.GetUserId(), size);

    [Authorize]
    [HttpPut("{postId:guid}/reaction")]
    public Task<PostDto> SetReaction(Guid postId, [FromBody] SetReactionReq req)
        => _biz.SetReactionAsync(postId, req.Type, User.GetUserId(), User.GetUserName());

    // ✅ COMMENT
    [Authorize]
    [HttpPost("{postId:guid}/comments")]
    public Task<PostCommentDto> AddComment(Guid postId, [FromBody] AddCommentReq req)
        => _biz.AddCommentAsync(postId, req, User.GetUserId(), User.GetUserName());

    [Authorize]
    [HttpGet("{postId:guid}/comments")]
    public Task<List<PostCommentDto>> GetComments(Guid postId)
        => _biz.GetCommentsAsync(postId);
    [Authorize]
    [HttpGet("user/{userId:guid}")]
    public Task<List<PostDto>> ByUser(Guid userId, [FromQuery] int size = 20)
    => _biz.GetByUserAsync(User.GetUserId(), userId, size);

}

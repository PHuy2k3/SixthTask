using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Api;
using Posts.Biz.Interfaces;
using Posts.Biz.Model;

namespace Posts.Api.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly IPostService _biz;
    private readonly IWebHostEnvironment _env;

    public PostsController(IPostService biz, IWebHostEnvironment env)
    {
        _biz = biz;
        _env = env;
    }

    // ✅ JSON create (giữ endpoint cũ)
    [Authorize]
    [HttpPost]
    public Task<PostDto> Create([FromBody] CreatePostReq req)
        => _biz.CreateAsync(req, User.GetUserId(), User.GetUserName(), imageUrls: null);

    // ✅ Create có ảnh: multipart/form-data
    // form fields: content, privacy, files
    [Authorize]
    [HttpPost("with-media")]
    [RequestSizeLimit(30_000_000)] // 30MB
    public async Task<PostDto> CreateWithMedia([FromForm] CreatePostWithMediaForm form)
    {
        var urls = new List<string>();

        if (form.Files is { Count: > 0 })
        {
            var root = _env.WebRootPath ?? "wwwroot";
            var uploadsDir = Path.Combine(root, "uploads");
            Directory.CreateDirectory(uploadsDir);

            foreach (var file in form.Files)
            {
                if (file is null || file.Length <= 0) continue;

                var ext = Path.GetExtension(file.FileName);
                var safeExt = string.IsNullOrWhiteSpace(ext) ? ".bin" : ext.ToLowerInvariant();

                // (optional) chỉ cho ảnh
                // if (safeExt is not (".png" or ".jpg" or ".jpeg" or ".webp")) continue;

                var fileName = $"{Guid.NewGuid()}{safeExt}";
                var fullPath = Path.Combine(uploadsDir, fileName);

                await using var stream = System.IO.File.Create(fullPath);
                await file.CopyToAsync(stream);

                urls.Add($"/uploads/{fileName}");
            }
        }

        var req = new CreatePostReq
        {
            Content = form.Content ?? "",
            Privacy = string.IsNullOrWhiteSpace(form.Privacy) ? "public" : form.Privacy
        };

        return await _biz.CreateAsync(req, User.GetUserId(), User.GetUserName(), urls);
    }

    [Authorize]
    [HttpPost("{postId:guid}/like")]
    public Task<PostDto> ToggleLike(Guid postId)
        => _biz.ToggleLikeAsync(postId, User.GetUserId());

    [Authorize]
    [HttpPost("{postId:guid}/comments")]
    public Task<PostCommentDto> AddComment(Guid postId, [FromBody] AddCommentReq req)
        => _biz.AddCommentAsync(postId, req, User.GetUserId(), User.GetUserName());

    [Authorize]
    [HttpGet("latest")]
    public Task<List<PostDto>> Latest([FromQuery] int size = 20)
        => _biz.GetLatestAsync(User.GetUserId(), size);

    [Authorize]
    [HttpGet("me")]
    public Task<List<PostDto>> MyPosts([FromQuery] int size = 20)
        => _biz.GetMyPostsAsync(User.GetUserId(), size);

    [Authorize]
    [HttpGet("{postId:guid}/comments")]
    public Task<List<PostCommentDto>> GetComments(Guid postId)
        => _biz.GetCommentsAsync(postId);
}

// ✅ Form model nằm chung file cũng được (cho gọn)
public class CreatePostWithMediaForm
{
    public string? Content { get; set; }
    public string? Privacy { get; set; } = "public";
    public List<IFormFile> Files { get; set; } = new();
}

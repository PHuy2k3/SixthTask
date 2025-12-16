using Microsoft.AspNetCore.Http;

namespace Posts.Api.Models;

public class CreatePostFormReq
{
    public string Content { get; set; } = "";
    public string Privacy { get; set; } = "public";
    public List<IFormFile> Files { get; set; } = new();
}

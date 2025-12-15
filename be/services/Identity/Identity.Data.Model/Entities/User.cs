namespace Identity.Data.Model.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public List<RefreshToken> RefreshTokens { get; set; } = new();
}

namespace Xellarium.Shared.DTO;

public class PostUserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}
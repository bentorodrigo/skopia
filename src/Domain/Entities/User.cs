using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserFunction Function { get; set; }

    public List<Project> Projects { get; set; } = new();

    public User()
    {
        
    }
}
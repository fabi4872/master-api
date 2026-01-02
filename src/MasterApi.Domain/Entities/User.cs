namespace MasterApi.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public string PasswordSalt { get; private set; }

    // Private constructor for EF Core
    private User() {
        Email = string.Empty;
        Name = string.Empty;
        PasswordHash = string.Empty;
        PasswordSalt = string.Empty;
    }

    public User(Guid id, string email, string name, string passwordHash, string passwordSalt)
    {
        Id = id;
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
}

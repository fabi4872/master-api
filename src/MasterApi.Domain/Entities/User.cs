namespace MasterApi.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public string Password { get; private set; }

    // Private constructor for EF Core
    private User() {
        Email = string.Empty;
        Name = string.Empty;
        Password = string.Empty;
    }

    public User(Guid id, string email, string name, string password)
    {
        Id = id;
        Email = email;
        Name = name;
        // TODO: Hash password before storing
        Password = password;
    }
}

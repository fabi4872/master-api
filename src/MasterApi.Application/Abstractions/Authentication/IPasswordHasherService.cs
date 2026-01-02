namespace MasterApi.Application.Abstractions.Authentication;

public interface IPasswordHasherService
{
    (string Hash, string Salt) Hash(string password);
    bool Verify(string password, string hash, string salt);
}

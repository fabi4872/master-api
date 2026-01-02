using System.Security.Cryptography;
using MasterApi.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MasterApi.Infrastructure.Authentication;

public class PasswordHasherService : IPasswordHasherService
{
    // These constants can be moved to configuration if needed
    private const int SaltSize = 128 / 8; // 128 bits
    private const int HashSize = 256 / 8; // 256 bits
    private const int Iterations = 100_000;
    private static readonly KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA256;

    public (string Hash, string Salt) Hash(string password)
    {
        // Generate a random salt
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        
        // Hash the password
        var hashBytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: Prf,
            iterationCount: Iterations,
            numBytesRequested: HashSize
        );

        // Return the hash and salt as base64 encoded strings
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool Verify(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var hashBytes = Convert.FromBase64String(hash);

        // Hash the password with the provided salt
        var newHashBytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: Prf,
            iterationCount: Iterations,
            numBytesRequested: HashSize
        );
        
        // Compare the hashes
        return CryptographicOperations.FixedTimeEquals(hashBytes, newHashBytes);
    }
}

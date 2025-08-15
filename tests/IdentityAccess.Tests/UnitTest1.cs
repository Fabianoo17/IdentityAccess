using FluentAssertions;
using IdentityAccess.Domain;
using Microsoft.AspNetCore.Identity;

namespace IdentityAccess.Tests;

public class PasswordTests
{
    [Fact]
    public void Hash_And_Verify_Password()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User { Username = "TesteUser", PasswordHash = "" };
        var hash = hasher.HashPassword(user, "P@ssw0rd!");
        hasher.VerifyHashedPassword(user, hash, "P@ssw0rd!")
            .Should().NotBe(PasswordVerificationResult.Failed);
    }
}